using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using MiniTwitch.Common;
using MiniTwitch.Helix.Internal.Json;
using MiniTwitch.Helix.Internal.Models;
using MiniTwitch.Helix.Models;

namespace MiniTwitch.Helix.Internal;

public sealed class HelixApiClient
{
    internal DefaultMiniTwitchLogger<HelixWrapper> Logger { get; } = new();
    internal static JsonSerializerOptions SerializerOptions { get; } = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        PropertyNamingPolicy = new SnakeCaseNamingPolicy()
    };
    internal long UserId { get; private set; }

    private readonly SemaphoreSlim _validateLock = new(1);
    private readonly HttpClient _httpClient = new();
    private readonly string _tokenValidationUrl;
    private readonly ILogger? _logger;
    private TokenInfo? _tokenInfo;

    internal HelixApiClient(string token, long userId, ILogger? logger, string tokenValidationUrl)
    {
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        _tokenValidationUrl = tokenValidationUrl;
        _logger = logger;
        this.UserId = userId;
        GetLogger().BeginScope($"Helix-UserId={this.UserId}");
    }

    public void ChangeToken(string newToken, long newUserId)
    {
        _httpClient.DefaultRequestHeaders.Remove("Authorization");
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {newToken}");
        _httpClient.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
        this.UserId = newUserId;
        _tokenInfo = null;
    }

    internal Task<(HttpResponseMessage, TimeSpan)> RequestAsync(RequestData requestObject, CancellationToken ct) => requestObject._method switch
    {
        "POST" => PostAsync(requestObject, ct),
        "GET" => GetAsync(requestObject, ct),
        "PUT" => PutAsync(requestObject, ct),
        "DELETE" => DeleteAsync(requestObject, ct),
        "PATCH" => PatchAsync(requestObject, ct),
        _ => throw new NotImplementedException($"HTTP method {requestObject._method} is not supported")
    };

    private async Task<(HttpResponseMessage, TimeSpan)> PostAsync(RequestData requestObject, CancellationToken ct)
    {
        await ValidateToken(ct);
        string url = requestObject.GetUrl();
        var sw = Stopwatch.StartNew();
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(url, requestObject.Body, SerializerOptions, ct);
        sw.Stop();
        TimeSpan elapsed = sw.Elapsed;
        LogLevel logLevel = response.IsSuccessStatusCode ? LogLevel.Debug : LogLevel.Warning;
        Log(logLevel, "POST [{Code}] {Url} {Elapsed}", response.StatusCode, url, elapsed);
        return (response, elapsed);
    }

    private async Task<(HttpResponseMessage, TimeSpan)> GetAsync(RequestData requestObject, CancellationToken ct)
    {
        await ValidateToken(ct);
        string url = requestObject.GetUrl();
        var sw = Stopwatch.StartNew();
        HttpResponseMessage response = await _httpClient.GetAsync(url, ct);
        sw.Stop();
        TimeSpan elapsed = sw.Elapsed;
        LogLevel logLevel = response.IsSuccessStatusCode ? LogLevel.Debug : LogLevel.Warning;
        Log(logLevel, "GET [{Code}] {Url} {Elapsed}", response.StatusCode, url, elapsed);
        return (response, elapsed);
    }

    private async Task<(HttpResponseMessage, TimeSpan)> PutAsync(RequestData requestObject, CancellationToken ct)
    {
        await ValidateToken(ct);
        string url = requestObject.GetUrl();
        var sw = Stopwatch.StartNew();
        HttpResponseMessage response = await _httpClient.PutAsJsonAsync(url, requestObject.Body, SerializerOptions, ct);
        sw.Stop();
        TimeSpan elapsed = sw.Elapsed;
        LogLevel logLevel = response.IsSuccessStatusCode ? LogLevel.Debug : LogLevel.Warning;
        Log(logLevel, "PUT [{Code}] {Url} {Elapsed}", response.StatusCode, url, elapsed);
        return (response, elapsed);
    }

    private async Task<(HttpResponseMessage, TimeSpan)> DeleteAsync(RequestData requestObject, CancellationToken ct)
    {
        await ValidateToken(ct);
        string url = requestObject.GetUrl();
        var sw = Stopwatch.StartNew();
        HttpResponseMessage response = await _httpClient.DeleteAsync(url, ct);
        sw.Stop();
        TimeSpan elapsed = sw.Elapsed;
        LogLevel logLevel = response.IsSuccessStatusCode ? LogLevel.Debug : LogLevel.Warning;
        Log(logLevel, "DELETE [{Code}] {Url} {Elapsed}", response.StatusCode, url, elapsed);
        return (response, elapsed);
    }

    private async Task<(HttpResponseMessage, TimeSpan)> PatchAsync(RequestData requestObject, CancellationToken ct)
    {
        await ValidateToken(ct);
        string url = requestObject.GetUrl();
        string rawContent = JsonSerializer.Serialize(requestObject.Body, SerializerOptions);
        var content = new StringContent(rawContent, Encoding.UTF8, "application/json");
        var sw = Stopwatch.StartNew();
        HttpResponseMessage response = await _httpClient.PatchAsync(url, content, ct);
        sw.Stop();
        TimeSpan elapsed = sw.Elapsed;
        LogLevel logLevel = response.IsSuccessStatusCode ? LogLevel.Debug : LogLevel.Warning;
        Log(logLevel, "PATCH [{Code}] {Url} {Elapsed}", response.StatusCode, url, elapsed);
        return (response, elapsed);
    }

    internal async ValueTask ValidateToken(CancellationToken ct)
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        await _validateLock.WaitAsync(ct);
        try
        {
            if (_tokenInfo is not null)
            {
                var expiresIn = TimeSpan.FromSeconds(_tokenInfo.ReceivedAt + _tokenInfo.ExpiresIn - now);
                if (_tokenInfo.IsPermaToken)
                {
                    Log(LogLevel.Trace, "Request sent with access token from user {Username} [No expiry]", _tokenInfo.Login);
                    return;
                }

                switch (expiresIn)
                {
                    case { TotalSeconds: <= -1 }:
                        throw new InvalidTokenException(null, $"Access token for user \"{_tokenInfo.Login}\" has expired");
                    case { TotalHours: < 0 }:
                        Log(LogLevel.Warning, "Access token for user {Username} expires in {ExpiresInMinutes} minutes", expiresIn.Minutes);
                        break;
                    case { TotalDays: < 0 }:
                        Log(LogLevel.Warning, "Access token for user {Username} expires in {ExpiresInHours} hours", expiresIn.Hours);
                        break;
                    default:
                        Log(LogLevel.Trace, "Request sent with access token from user {Username} [Expires in: {ExpiresIn}]", expiresIn);
                        break;
                }

                return;
            }

            HttpResponseMessage response = await _httpClient.GetAsync(_tokenValidationUrl, ct);
            if (!response.IsSuccessStatusCode)
            {
                InvalidToken? invalid = await response.Content.ReadFromJsonAsync<InvalidToken>(SerializerOptions, cancellationToken: ct);
                throw new InvalidTokenException(invalid?.Message, "Provided access token is either invalid or has expired");
            }

            _tokenInfo = await response.Content.ReadFromJsonAsync<TokenInfo>(SerializerOptions, cancellationToken: ct);
            if (_tokenInfo is null)
                throw new InvalidTokenException(null, "Validating access token failed");

            _httpClient.DefaultRequestHeaders.Remove("Client-Id");
            _httpClient.DefaultRequestHeaders.Add("Client-Id", $"{_tokenInfo.ClientId}");
            _tokenInfo.ReceivedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (_tokenInfo.IsPermaToken)
            {
                Log(
                    LogLevel.Information,
                    "Validated permanent access token from user {Username} with {ScopeCount} scopes",
                    _tokenInfo.Login, _tokenInfo.Scopes.Count
                );

                return;
            }

            if (_tokenInfo.Login is null)
            {
                Log(
                    LogLevel.Information,
                    "Validated app access token. The token expires at {ExpiresAt}",
                    DateTimeOffset.FromUnixTimeSeconds(_tokenInfo.ReceivedAt + _tokenInfo.ExpiresIn)
                );

                return;
            }

            Log(
                LogLevel.Information,
                "Validated access token from user {Username} with {ScopeCount} scopes. The token expires at {ExpiresAt}",
                _tokenInfo.Login, _tokenInfo.Scopes.Count, DateTimeOffset.FromUnixTimeSeconds(_tokenInfo.ReceivedAt + _tokenInfo.ExpiresIn)
            );
        }
        finally
        {
            _validateLock.Release();
        }
    }

    private void Log(LogLevel level, string template, params object[] properties) => GetLogger().Log(level, template, properties);
    private ILogger GetLogger() => _logger ?? this.Logger;
}

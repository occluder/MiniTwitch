using MiniTwitch.Helix.Internal.Models;

namespace MiniTwitch.Helix.Test.Internal;

public class RequestDataTest
{
    static RequestData CreateRequest() => new("https://api.twitch.tv/helix", Endpoints.GetUserBlockList);

    [Fact]
    public void AddParam_Bool_SerializesLowercase()
    {
        string url = CreateRequest().AddParam("flag", true).GetUrl();

        Assert.EndsWith("?flag=true", url);
    }

    [Fact]
    public void AddParam_FalseBool_SerializesLowercase()
    {
        string url = CreateRequest().AddParam("flag", false).GetUrl();

        Assert.EndsWith("?flag=false", url);
    }

    [Fact]
    public void AddParam_NullableBool_SerializesLowercase()
    {
        string url = CreateRequest().AddParam("flag", (bool?)true).GetUrl();

        Assert.EndsWith("?flag=true", url);
    }

    [Fact]
    public void AddParam_NullableBoolNull_IsOmitted()
    {
        string url = CreateRequest().AddParam("flag", (bool?)null).GetUrl();

        Assert.DoesNotContain("flag", url);
    }

    [Fact]
    public void AddParam_BareNull_IsOmitted()
    {
        // A bare null must not become ambiguous between the DateTime? and object? overloads.
        string url = CreateRequest().AddParam("flag", null).GetUrl();

        Assert.DoesNotContain("flag", url);
    }

    [Fact]
    public void AddParam_OtherValues_UseInvariantFormatting()
    {
        string url = CreateRequest()
            .AddParam("first", 20)
            .AddParam("user_id", 1234L)
            .GetUrl();

        Assert.EndsWith("?first=20&user_id=1234", url);
    }

    [Fact]
    public void AddMultiParam_RepeatsTheKeyForEveryValue()
    {
        long[] ids = [1234L, 5678L];

        string url = CreateRequest()
            .AddMultiParam("user_id", ids)
            .GetUrl();

        Assert.EndsWith("?user_id=1234&user_id=5678", url);
    }
}

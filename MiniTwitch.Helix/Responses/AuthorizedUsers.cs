using System.Text.Json.Serialization;
using MiniTwitch.Helix.Models;

namespace MiniTwitch.Helix.Responses;

public class AuthorizedUsers : BaseResponse<AuthorizedUsers.User>
{
    public record User(
        long UserId,
        [property: JsonPropertyName("user_name")] string UserDisplayName,
        [property: JsonPropertyName("user_login")] string Username,
        string[] Scopes
    );
}

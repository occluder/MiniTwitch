using System.Text.Json.Serialization;
using MiniTwitch.Helix.Enums;
using MiniTwitch.Helix.Models;
using MiniTwitch.Helix.Interfaces;

namespace MiniTwitch.Helix.Responses;

public class Commercial : SingleResponse<Commercial.Info>
{
   public record Info(
       [property: JsonPropertyName("length")] int Length,
       [property: JsonPropertyName("message")] string Message,
       [property: JsonPropertyName("retry_after")] int RetryAfter
   );
}
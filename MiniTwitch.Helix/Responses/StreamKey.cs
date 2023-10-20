using System.Text.Json.Serialization;
using MiniTwitch.Helix.Enums;
using MiniTwitch.Helix.Models;
using MiniTwitch.Helix.Interfaces;

namespace MiniTwitch.Helix.Responses;

public class StreamKey : SingleResponse<StreamKey.Info>
{
   public record Info(
       [property: JsonPropertyName("stream_key")] string StreamKey
   );
}
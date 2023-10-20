using System.Text.Json.Serialization;
using MiniTwitch.Helix.Enums;
using MiniTwitch.Helix.Models;
using MiniTwitch.Helix.Interfaces;

namespace MiniTwitch.Helix.Responses;

public class GetPolls : PaginableResponse<GetPolls.Datum>
{
   public record Choice(
       [property: JsonPropertyName("id")] string Id,
       [property: JsonPropertyName("title")] string Title,
       [property: JsonPropertyName("votes")] int Votes,
       [property: JsonPropertyName("channel_points_votes")] int ChannelPointsVotes
   );

   public record Datum(
       [property: JsonPropertyName("id")] string Id,
       [property: JsonPropertyName("broadcaster_id")] long BroadcasterId,
       [property: JsonPropertyName("broadcaster_name")] string BroadcasterDisplayName,
       [property: JsonPropertyName("broadcaster_login")] string BroadcasterName,
       [property: JsonPropertyName("title")] string Title,
       [property: JsonPropertyName("choices")] IReadOnlyList<Choice> Choices,
       [property: JsonPropertyName("channel_points_voting_enabled")] bool ChannelPointsVotingEnabled,
       [property: JsonPropertyName("channel_points_per_vote")] int ChannelPointsPerVote,
       [property: JsonPropertyName("status")] string Status,
       [property: JsonPropertyName("duration")] int Duration,
       [property: JsonPropertyName("started_at")] DateTime StartedAt,
       [property: JsonPropertyName("ended_at")] DateTime? EndedAt
   );
}
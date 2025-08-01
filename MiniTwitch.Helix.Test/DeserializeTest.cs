using System.Text.Json;
using MiniTwitch.Helix.Enums;
using MiniTwitch.Helix.Internal;
using MiniTwitch.Helix.Responses;

namespace MiniTwitch.Helix.Test;

public class DeserializeTest
{
    static readonly JsonSerializerOptions options = HelixApiClient.SerializerOptions;

    [Fact]
    public void ChannelsInformation()
    {
        string json = "{\"data\":[{\"broadcaster_id\":\"93300843\",\"broadcaster_login\":\"sneeziu\",\"broadcaster_name\":\"sneeziu\",\"broadcaster_language\":\"en\",\"game_id\":\"66170\",\"game_name\":\"Warframe\",\"title\":\"RISE AND SLIME! MAN I LOVE MICROPLASTICS, YUMMY YUMMY ????\",\"delay\":0,\"tags\":[\"Vtuber\",\"English\",\"Slime\",\"ASMR\",\"ENVtuber\",\"Yapper\",\"LGBTQIAPlus\",\"BrainRot\",\"TennoCreate\",\"Microplastics\"],\"content_classification_labels\":[\"MatureGame\"],\"is_branded_content\":false}]}";
        ChannelsInformation? response = JsonSerializer.Deserialize<ChannelsInformation>(json, options);
        Assert.NotNull(response);
        var info = Assert.Single(response.Data);
        Assert.Equal(93300843, info.Id);
        Assert.Equal("sneeziu", info.Name);
        Assert.Equal("sneeziu", info.DisplayName);
        Assert.Equal("en", info.Language);
        Assert.Equal("Warframe", info.GameName);
        Assert.Equal("66170", info.GameId);
        Assert.Equal("RISE AND SLIME! MAN I LOVE MICROPLASTICS, YUMMY YUMMY ????", info.Title);
        Assert.Equal(0, info.Delay);
        Assert.Equal(["Vtuber", "English", "Slime", "ASMR", "ENVtuber", "Yapper", "LGBTQIAPlus", "BrainRot", "TennoCreate", "Microplastics"], info.Tags);
        Assert.Equal([ContentLabelId.MatureGame], info.ContentClassificationLabels);
        Assert.False(info.IsBrandedContent);
    }

    [Fact]
    public void HypeTrainStatus()
    {
        string json = "{\"data\":[{\"current\":{\"id\":\"1b0AsbInCHZW2SQFQkCzqN07Ib2\",\"broadcaster_user_id\":\"1337\",\"broadcaster_user_login\":\"cool_user\",\"broadcaster_user_name\":\"Cool_User\",\"level\":2,\"total\":700,\"progress\":200,\"goal\":1000,\"top_contributions\":[{\"user_id\":\"123\",\"user_login\":\"pogchamp\",\"user_name\":\"PogChamp\",\"type\":\"bits\",\"total\":50},{\"user_id\":\"456\",\"user_login\":\"kappa\",\"user_name\":\"Kappa\",\"type\":\"subscription\",\"total\":45}],\"shared_train_participants\":[{\"broadcaster_user_id\":\"456\",\"broadcaster_user_login\":\"pogchamp\",\"broadcaster_user_name\":\"PogChamp\"},{\"broadcaster_user_id\":\"321\",\"broadcaster_user_login\":\"pogchamp\",\"broadcaster_user_name\":\"PogChamp\"}],\"started_at\":\"2020-07-15T17:16:03.17106713Z\",\"expires_at\":\"2020-07-15T17:16:11.17106713Z\",\"type\":\"golden_kappa\"},\"all_time_high\":{\"level\":6,\"total\":2850,\"achieved_at\":\"2020-04-24T20:12:21.003802269Z\"},\"shared_all_time_high\":{\"level\":16,\"total\":23850,\"achieved_at\":\"2020-04-27T20:12:21.003802269Z\"}}]}";
        HypeTrainStatus? status = JsonSerializer.Deserialize<HypeTrainStatus>(json, options);
        Assert.NotNull(status);
        var info = Assert.Single(status.Data);
        Assert.NotNull(info.Current);
        Assert.Equal("1b0AsbInCHZW2SQFQkCzqN07Ib2", info.Current.Id);
        Assert.Equal(1337, info.Current.BroadcasterId);
        Assert.Equal("cool_user", info.Current.BroadcasterName);
        Assert.Equal("Cool_User", info.Current.BroadcasterDisplayName);
        Assert.Equal(2, info.Current.Level);
        Assert.Equal(700, info.Current.Total);
        Assert.Equal(200, info.Current.Progress);
        Assert.Equal(1000, info.Current.Goal);

        // Top Contributions
        Assert.Equal(2, info.Current.TopContributions.Length);
        Assert.Equal(123, info.Current.TopContributions[0].UserId);
        Assert.Equal("pogchamp", info.Current.TopContributions[0].Username);
        Assert.Equal("PogChamp", info.Current.TopContributions[0].UserDisplayName);
        Assert.Equal("bits", info.Current.TopContributions[0].Type);
        Assert.Equal(50, info.Current.TopContributions[0].Total);
        Assert.Equal(456, info.Current.TopContributions[1].UserId);
        Assert.Equal("kappa", info.Current.TopContributions[1].Username);
        Assert.Equal("Kappa", info.Current.TopContributions[1].UserDisplayName);
        Assert.Equal("subscription", info.Current.TopContributions[1].Type);
        Assert.Equal(45, info.Current.TopContributions[1].Total);

        // Shared Participants
        Assert.Equal(2, info.Current.SharedTrainParticipants.Length);
        Assert.Equal(456, info.Current.SharedTrainParticipants[0].BroadcasterId);
        Assert.Equal("pogchamp", info.Current.SharedTrainParticipants[0].BroadcasterName);
        Assert.Equal("PogChamp", info.Current.SharedTrainParticipants[0].BroadcasterDisplayName);
        Assert.Equal(321, info.Current.SharedTrainParticipants[1].BroadcasterId);
        Assert.Equal("pogchamp", info.Current.SharedTrainParticipants[1].BroadcasterName);
        Assert.Equal("PogChamp", info.Current.SharedTrainParticipants[1].BroadcasterDisplayName);

        Assert.Equal("2020-07-15T17:16:03.1710671Z", info.Current.StartedAt.ToString("o"));
        Assert.Equal("2020-07-15T17:16:11.1710671Z", info.Current.ExpiresAt.ToString("o"));
        Assert.Equal("golden_kappa", info.Current.Type);

        // All Time High
        Assert.NotNull(info.AllTimeHigh);
        Assert.Equal(6, info.AllTimeHigh.Level);
        Assert.Equal(2850, info.AllTimeHigh.Total);
        Assert.Equal("2020-04-24T20:12:21.0038022Z", info.AllTimeHigh.AchievedAt.ToString("o"));

        // Shared All Time High
        Assert.NotNull(info.SharedAllTimeHigh);
        Assert.Equal(16, info.SharedAllTimeHigh.Level);
        Assert.Equal(23850, info.SharedAllTimeHigh.Total);
        Assert.Equal("2020-04-27T20:12:21.0038022Z", info.SharedAllTimeHigh.AchievedAt.ToString("o"));
    }
}


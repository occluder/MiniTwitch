using System.Text.Json;
using MiniTwitch.Helix.Enums;
using MiniTwitch.Helix.Internal;
using MiniTwitch.Helix.Requests;
using MiniTwitch.Helix.Responses;

namespace MiniTwitch.Helix.Test;

public class ChangelogTests
{
    private static readonly JsonSerializerOptions _options = HelixApiClient.SerializerOptions;

    [Fact]
    public void AdSchedule_UsesUnixTimestamps()
    {
        string json = """
            {
              "data": [
                {
                  "next_ad_at": 1690931298,
                  "last_ad_at": 1690930398,
                  "duration": 60,
                  "preroll_free_time": 90,
                  "snooze_count": 1,
                  "snooze_refresh_at": 1690932198
                }
              ]
            }
            """;

        var response = JsonSerializer.Deserialize<AdSchedule>(json, _options);
        Assert.NotNull(response);
        var ad = Assert.Single(response.Data);
        Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1690931298).UtcDateTime, ad.NextAdAt);
        Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1690930398).UtcDateTime, ad.LastAdAt);
        Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1690932198).UtcDateTime, ad.SnoozeRefreshAt);
    }

    [Fact]
    public void AdSchedule_TreatsZeroTimestampsAsNull()
    {
        string json = """{"data":[{"next_ad_at":0,"last_ad_at":0,"duration":0,"preroll_free_time":0,"snooze_count":0,"snooze_refresh_at":0}]}""";

        var response = JsonSerializer.Deserialize<AdSchedule>(json, _options);
        Assert.NotNull(response);
        var ad = Assert.Single(response.Data);
        Assert.Null(ad.NextAdAt);
        Assert.Null(ad.LastAdAt);
        Assert.Null(ad.SnoozeRefreshAt);
    }

    [Fact]
    public void SnoozedAd_UsesUnixTimestamps()
    {
        string json = """{"data":[{"snooze_count":1,"snooze_refresh_at":1690932198,"next_ad_at":1690931298}]}""";

        var response = JsonSerializer.Deserialize<SnoozedAd>(json, _options);
        Assert.NotNull(response);
        var ad = Assert.Single(response.Data);
        Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1690932198).UtcDateTime, ad.SnoozeRefreshAt);
        Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1690931298).UtcDateTime, ad.NextAdAt);
    }

    [Fact]
    public void EventSubSubscription_ReadsConditionAsObjectAndConduitTransport()
    {
        string json = """
            {
              "total": 1,
              "data": [
                {
                  "id": "26b1c993-bfcf-44d9-b876-379dacafe75a",
                  "status": "conduit_deleted",
                  "type": "stream.online",
                  "version": "1",
                  "condition": {
                    "broadcaster_user_id": "1234",
                    "reward_id": "5678"
                  },
                  "created_at": "2020-11-10T20:08:33.12345678Z",
                  "transport": {
                    "method": "conduit",
                    "conduit_id": "bfcfc993-26b1-b876-44d9-afe75a379dac"
                  },
                  "cost": 1,
                  "is_batching_enabled": true
                }
              ],
              "total_cost": 1,
              "max_total_cost": 10000,
              "pagination": {}
            }
            """;

        var response = JsonSerializer.Deserialize<EventSubSubscriptions>(json, _options);
        Assert.NotNull(response);
        var sub = Assert.Single(response.Data);
        Assert.Equal(EventSubStatus.ConduitDeleted, sub.Status);
        Assert.Equal("1234", sub.Condition["broadcaster_user_id"]);
        Assert.Equal("5678", sub.Condition["reward_id"]);
        Assert.Equal("conduit", sub.Transport.Method);
        Assert.Equal("bfcfc993-26b1-b876-44d9-afe75a379dac", sub.Transport.ConduitId);
        Assert.True(sub.IsBatchingEnabled);
    }

    [Fact]
    public void NewSubscription_SerializesBatchingFlag()
    {
        var body = new NewSubscription(
            "drop.entitlement.grant",
            "1",
            new NewSubscription.EventsubTransport("webhook", callback: "https://example.com"),
            new { organization_id = "1234" },
            isBatchingEnabled: true
        );

        var json = JsonSerializer.Serialize(body, _options);
        Assert.Contains("\"is_batching_enabled\":true", json);
    }

    [Fact]
    public void GuestStarSession_ReadsAudioAndVideoObjects()
    {
        string json = """
            {
              "data": [
                {
                  "id": "session-id",
                  "guests": [
                    {
                      "slot_id": "0",
                      "user_id": "1234",
                      "is_live": true,
                      "volume": 100,
                      "assigned_at": "2024-01-01T00:00:00Z",
                      "audio": {
                        "is_host_enabled": true,
                        "is_guest_enabled": true,
                        "is_available": true
                      },
                      "video": {
                        "is_host_enabled": true,
                        "is_guest_enabled": false,
                        "is_available": true
                      }
                    }
                  ]
                }
              ]
            }
            """;

        var response = JsonSerializer.Deserialize<GuestStarSession>(json, _options);
        Assert.NotNull(response);
        var session = Assert.Single(response.Data);
        var guest = Assert.Single(session.Guests);
        Assert.Equal("0", guest.SlotId);
        Assert.True(guest.Audio.IsHostEnabled);
        Assert.True(guest.Audio.IsGuestEnabled);
        Assert.False(guest.Video.IsGuestEnabled);
    }

    [Fact]
    public void ActiveExtensions_AllowsInactiveSlots()
    {
        string json = """
            {
              "data": {
                "panel": {
                  "1": { "active": false }
                },
                "overlay": {
                  "1": { "active": false }
                },
                "component": {
                  "1": { "active": false },
                  "2": {
                    "active": true,
                    "id": "component-id",
                    "version": "1.0.0",
                    "name": "Component",
                    "x": 0,
                    "y": 5
                  }
                }
              }
            }
            """;

        var response = JsonSerializer.Deserialize<ActiveExtensions>(json, _options);
        Assert.NotNull(response);
        var panel = response.Data.Panel[1];
        Assert.False(panel.Active);
        Assert.Null(panel.Id);
        Assert.Null(panel.Version);
        Assert.Null(panel.Name);
        var component = response.Data.Component[2];
        Assert.True(component.Active);
        Assert.Equal(5, component.Y);
    }

    [Fact]
    public void StreamSchedule_ReadsTopLevelPagination()
    {
        string json = """
            {
              "data": {
                "segments": [],
                "broadcaster_id": "1234",
                "broadcaster_name": "Broadcaster",
                "broadcaster_login": "broadcaster",
                "vacation": null
              },
              "pagination": {
                "cursor": "eyJiIjpudWxsLCJhIjp7Ik9mZnNldCI6NX19"
              }
            }
            """;

        var response = JsonSerializer.Deserialize<StreamSchedule>(json, _options);
        Assert.NotNull(response);
        Assert.Equal("eyJiIjpudWxsLCJhIjp7Ik9mZnNldCI6NX19", response.Pagination.Cursor);
    }

    [Fact]
    public void NewScheduleSegment_SerializesDuration()
    {
        var body = new NewScheduleSegment(
            new DateTime(2026, 11, 10, 18, 0, 0, DateTimeKind.Utc),
            "UTC",
            60
        );

        var json = JsonSerializer.Serialize(body, _options);
        Assert.Contains("\"duration\":60", json);
        Assert.DoesNotContain("duration_hours", json);
    }

    [Fact]
    public void ExtensionConfigurationSegment_ReadsBroadcasterId()
    {
        string json = """
            {
              "data": [
                {
                  "segment": "broadcaster",
                  "broadcaster_id": "1234",
                  "content": "{}",
                  "version": "1"
                }
              ]
            }
            """;

        var response = JsonSerializer.Deserialize<ExtensionConfigurationSegment>(json, _options);
        Assert.NotNull(response);
        var segment = Assert.Single(response.Data);
        Assert.Equal("1234", segment.BroadcasterId);
    }

    [Fact]
    public void BroadcasterSubscriptions_OmittedPointsAndTotalAreNull()
    {
        string json = """
            {
              "data": [
                {
                  "broadcaster_id": "1234",
                  "broadcaster_login": "broadcaster",
                  "broadcaster_name": "Broadcaster",
                  "gifter_id": "",
                  "gifter_login": "",
                  "gifter_name": "",
                  "is_gift": false,
                  "tier": "1000",
                  "plan_name": "Channel Subscription (broadcaster)",
                  "user_id": "5678",
                  "user_login": "subscriber",
                  "user_name": "Subscriber"
                }
              ],
              "pagination": {}
            }
            """;

        var response = JsonSerializer.Deserialize<BroadcasterSubscriptions>(json, _options);
        Assert.NotNull(response);
        Assert.Null(response.Points);
        Assert.Null(response.Total);
        var subscription = Assert.Single(response.Data);
        Assert.Null(subscription.GifterId);
    }

    [Fact]
    public void ChannelExtensions_ReadsLiveConfigView()
    {
        string json = """
            {
              "data": [
                {
                  "views": {
                    "live_config": {
                      "viewer_url": "https://example.com/live_config.html",
                      "can_link_external_content": true
                    }
                  }
                }
              ]
            }
            """;

        var response = JsonSerializer.Deserialize<ChannelExtensions>(json, _options);
        Assert.NotNull(response);
        var extension = Assert.Single(response.Data);
        Assert.NotNull(extension.Views.LiveConfig);
        var liveConfig = extension.Views.LiveConfig!;
        Assert.Equal("https://example.com/live_config.html", liveConfig.ViewerUrl);
        Assert.True(liveConfig.CanLinkExternalContent);
    }
}

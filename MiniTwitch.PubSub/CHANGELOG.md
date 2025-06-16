# MiniTwitch.PubSub Changelog

## 2.1.1
### Minor changes
- Added collaboration viewer properties and status enum (#163)
***

## 2.1.0
### Minor changes
- 'Official' PubSub topics that are scheduled for decommission by Twitch are now marked as obsolete.
***

## 2.0.0
This release marks the removal of .NET 6 and .NET 7 as target frameworks, as both versions are out-of-support by now.
***

## 1.0.4

### Minor changes
- `PubSubClient` now has the property `IsConnected` for checking whether the underlying WebSocket is connected

***

## 1.0.3

### Minor changes

- Updated authentication comment for `following.{id}` topic

### Dev

- Changed internal messagetopic & topicinfo values to match `ReadOnlySpan<byte>.MSum()` rather than `.Sum()`. Usages of the latter were also switched
- Removed unused hypetrain topic from messagetopic

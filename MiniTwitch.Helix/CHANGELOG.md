# MiniTwitch.Helix Changelog

## 2.0.0
- Target frameworks updated to include .NET 10
***

## 1.0.4
### Fixes
- Fixed DateTime parameters not being formatted to RFC3339 (again)

## 1.0.3
### Minor Changes
- Marked `GetHypeTrainEvents` as deprecated (#181)
- Added `Get Hype Train Status` Endpoint (#173)
- Added `Get Clips Download` Endpoint (#182)
- Added `Get Authorization By User` Endpoint (#186)

### Fixes
- Parameters for the `Get Clips` endpoint are now mutually exclusive, conforming to docs (#189)
- Fixed DateTime parameters not being formatted to RFC3339 (#191)
***

## 1.0.2
### Minor Changes
- Added ForSourceOnly property to ChatMessage (#161)
- Added subscription_id to GetEventSubSubscriptions response (#164)

### Fixes
- Fixed CustomReward related issues (#157)
- Fixed enum converter parsing and add more detailed exceptions (#158)
***

## 1.0.1
### Minor changes
- Updated relevant eventsub models to include conduit fields/properties.
- Added missing content classification label ids.
- Set `DefaultVersionPolicy` for API calls to `HttpVersionPolicy.RequestVersionOrHigher`.

### Fixes
- Fixed several deserialization issues and missing properties on `ChannelsInformation.Information`.
***

## 1.0.0
- No longer in pre-release!
- Added some missing content classification ids (#136)

This release marks the removal of .NET 6 and .NET 7 as target frameworks, as both versions are out-of-support by now.
***

## 0.4.2-prerelease

### Minor Changes
- Added IAsyncEnumerable pagination API `HelixResult<T>.EnumeratePages()` (#122)
- Added links to the API reference page for every call (#120)

### Fixes
- Fixed getting 0 as the ChannelId for followed channels (#123)
- Fixed deserialization exception in `GetBannedUsers()` (#121)

## 0.4.1-prerelease

### Fixes
-  Fix getting JsonException when a result without value is a success (#119)

****

## 0.4.0-prerelease

### Breaking changes
- `SortedHelixWrapper` has been removed (#110)

### Minor Changes
- Changing the token of `HelixWrapper` is now possible through `HelixWrapper.Client.ChangeToken()` (#110)
- Added `Warn Chat User` endpoint (#111)
- Error messages are now provided from Helix responses rather than scraped data (#115)

### Fixes
- Fixed an exception that occurs when empty strings are provided to SnakeCase.ConvertToCase
- Fixed an exception that occurs when calling `GetChatSettings()` on a channel that does not have any special modes enabled 
- The HttpResponseMessage from making calls with HttpClient now gets disposed (#118)

****

## 0.3.0-prerelease

### Minor changes
- Added `Get User Emotes` endpoint (#83)
- Added Conduit endpoints (#87)
- Added `Get Unban Requests` endpoint (#91)
- Added `Resolve Unban Requests` endpoint (#93)

### Fixes
- Fixed NullReferenceException when attempting to validate certain token types

### Development changes
- Updated naming policy used for (de)serialization

****

## 0.2.3-prerelease

### Minor changes

- Added "Send Chat Message" endpoint

****

## 0.2.2-prerelease

### Minor changes

- Added "Get Moderated Channels" endpoint
- Added a semaphore lock to token validation to ensure consistency

### Fixes

- [Updated "Get Ad Schedule" response](https://dev.twitch.tv/docs/change-log/#:~:text=2023%E2%80%9112%E2%80%9111)
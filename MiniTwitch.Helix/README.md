# MiniTwitch.Helix

MiniTwitch.Helix conveniently wraps the Twitch Helix API and exposes them through the `HelixWrapper`class.
## Features

- Contains all generally available and beta Helix API endpoints
- Virtually no dependencies
- Returns meaningful information about responses with `HelixResult`:
	- **HelixResult.Success**: Whether the request was successful
	- **HelixResult.StatusCode**: Status code of the response
	- **HelixResult.Message**: Contains the error message for the request, if not successful
	- **HelixResult.Elapsed**: The amount of time the request took to get a response
	- **HelixResult.RateLimit.Limit**: Maximum amount of requests that can be made in a period
	- **HelixResult.RateLimit.Remaining**: The amount of requests that can be made before the ratelimit resets
	- **HelixResult.RateLimit.ResetsIn**: The amount of time before the ratelimit resets

- Validates access tokens & warns before their expiry
- Easy pagination API for `HelixResult<T>`:
	- **HelixResult.CanPaginate**: Determines whether the next page of content can be requested
	- **HelixResult.Paginate()**: Fetches the next page of content

## Getting Started

This example demonstrates the usage of `HelixWrapper` and  pagination through `HelixResult<T>`

```csharp
using MiniTwitch.Helix;
using MiniTwitch.Helix.Models;
using MiniTwitch.Helix.Responses;

namespace MiniTwitchExample;

public class Program
{
    public static HelixWrapper Helix { get; set; }

    static async Task Main()
    {
        Helix = new HelixWrapper("mytoken", 783267696);

        var emotes = await GetAllMyEmotes();
    }

    private static async Task<List<string>> GetAllMyEmotes()
    {
        HelixResult<UserEmotes> emotesResult = await Helix.GetUserEmotes();
        if (!emotesResult.Success)
        {
            return [];
        }

        List<string> emoteList = [];
        foreach (var emote in emotesResult.Value.Data)
        {
            emoteList.Add(emote.Name);
        }

        // Fetch the next pages of content.
        // The code inside will not run if there are no more pages.
        await foreach (var nextEmotesResult in emotesResult.EnumeratePages())
        {
            foreach (var emote in nextEmotesResult.Value.Data)
            {
                emoteList.Add(emote.Name);
            }
        }

        return emoteList;
    }
}
```
# MiniTwitch.Helix

MiniTwitch.Helix conveniently wraps the Twitch Helix API and exposes it through the `HelixWrapper` class.

## Features

- Full coverage of all generally available and beta Helix API endpoints
- Minimal dependencies: only [MiniTwitch.Common](https://www.nuget.org/packages/MiniTwitch.Common/)
- Informative `HelixResult<T>` responses with:
  - **Success**: Whether the request was successful
  - **StatusCode**: HTTP status code of the response
  - **Message**: Error message (if not successful)
  - **Elapsed**: Time the request took to complete
  - **Ratelimit.Limit**: Max requests allowed in the current window
  - **Ratelimit.Remaining**: Requests remaining before the limit resets
  - **Ratelimit.ResetsIn**: Time until the rate-limit window resets
- Automatic access token validation with expiry warnings
- Built-in pagination: check `CanPaginate`, fetch the next page with `Paginate()`, or iterate all pages with `EnumeratePages()`

## Getting Started

This example demonstrates the usage of `HelixWrapper` and pagination through `HelixResult<T>`:

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

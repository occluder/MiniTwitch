using MiniTwitch.Helix.Enums;

namespace MiniTwitch.Helix.Test.Enums;

public class LabelLocaleTest
{
    [Fact]
    public void AllLocales_MatchDocumentedValues()
    {
        // https://dev.twitch.tv/docs/api/reference/ccls#get-content-classification-labels
        string[] documented =
        [
            "ar-SA", "bg-BG", "cs-CZ", "da-DK", "de-DE", "el-GR", "en-GB", "en-US",
            "es-ES", "es-MX", "fi-FI", "fr-FR", "hu-HU", "it-IT", "ja-JP", "ko-KR",
            "nl-NL", "no-NO", "pl-PL", "pt-BR", "pt-PT", "ro-RO", "ru-RU", "sk-SK",
            "sv-SE", "th-TH", "tr-TR", "uk-UA", "vi-VN", "zh-CN", "zh-TW"
        ];

        string[] actual = [.. Enum.GetValues<LabelLocale>().Select(locale => locale.ToString().Replace('_', '-'))];

        Assert.Equal(documented, actual);
    }
}

using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>An object that contains a monetary amount</summary>
[EventProperty]
public partial struct CampaignAmount
{
    /// <summary>
    /// The monetary amount in the currency's minor unit.
    /// For example, USD uses cents, so $5.50 is <c>550</c>
    /// </summary>
    public partial int Value { get; }
    /// <summary>
    /// The number of decimal places used by the currency.
    /// For example, USD uses two decimal places
    /// </summary>
    public partial int DecimalPlaces { get; }
    /// <summary>The ISO-4217 three-letter currency code for the amount</summary>
    [Intern]
    public partial string Currency { get; }
}

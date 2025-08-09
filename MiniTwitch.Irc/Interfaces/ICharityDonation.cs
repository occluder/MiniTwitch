using MiniTwitch.Irc.Enums;
using MiniTwitch.Irc.Models;

namespace MiniTwitch.Irc.Interfaces;

/// <summary>
/// Contains information about a charity donation
/// </summary>
public interface ICharityDonation : IUnixTimestamped, IUsernotice
{
    /// <summary>
    /// Name of the charity
    /// </summary>
    string CharityName { get; }
    /// <summary>
    /// The donation amount
    /// </summary>
    double DonationAmount { get; }
    /// <summary>
    /// Currency of the donation
    /// </summary>
    CurrencyCode DonationCurrency { get; }
    /// <summary>
    /// The message emitted in chat when the event occurs
    /// </summary>
    string SystemMessage { get; }
    /// <inheritdoc cref="Usernotice.Source"/>
    NoticeSource Source { get; }
}

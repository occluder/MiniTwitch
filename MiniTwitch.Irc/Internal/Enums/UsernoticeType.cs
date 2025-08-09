namespace MiniTwitch.Irc.Internal.Enums;

internal enum UsernoticeType
{
    Unknown,
    Sub = 3, // "sub"
    Raid = 4, // "raid"
    Resub = 5, // "resub"
    Unraid = 6, // "unraid"
    Subgift = 7, // "subgift"
    Announcement = 12, // "announcement"
    BitsBadgeTier = 13, // "bitsbadgetier"
    SubMysteryGift = 14, // "submysterygift"
    GiftPaidUpgrade = 15, // "giftpaidupgrade"
    PrimePaidUpgrade = 16, // "primepaidupgrade"
    StandardPayForward = 18, // "standardpayforward"
    AnonGiftPaidUpgrade = 19, // "anongiftpaidupgrade"
    CharityDonation, // "charitydonation"; Length conflict (15)
    SharedChatNotice, // "sharedchatnotice"; Length conflict (16)
}
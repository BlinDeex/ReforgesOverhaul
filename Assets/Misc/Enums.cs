using System;

namespace ModifiersOverhaul.Assets.Misc;

[Flags]
public enum SpecializedPrefixType
{
    Empty = 0,
    Pickaxe = 1,
    Axe = 2,
    Hammer = 4,
    MinionWeapon = 8,
    Whip = 16,
    Headwear = 32,
    Chestplate = 64,
    Leggings = 128
}

public enum ChallengerOrbType
{
    Green = 0,
    Blue = 1,
    Yellow = 2,
    Red = 3
}

public enum MessageType
{
    ChallengerScore,
    TrueDamageText,
    TimeStop,
    CharmOnKilled
}
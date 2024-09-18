using Terraria;
using Terraria.DataStructures;

namespace ModifiersOverhaul.Assets;

public class MiscStuff
{
    public static readonly PlayerDeathReason MANA_SURGE_DEATH =
        PlayerDeathReason.ByCustomReason($"{Main.LocalPlayer.name} was not capable enough to withstand mana surge");

    public static readonly PlayerDeathReason CHALLENGER_RED_ORB_DEATH =
        PlayerDeathReason.ByCustomReason($"{Main.LocalPlayer.name} hit the wrong orb one too many times");

    public static readonly PlayerDeathReason CHAOTIC_WEAPON_DEATH =
        PlayerDeathReason.ByCustomReason($"{Main.LocalPlayer.name} weapon was too chaotic");
}
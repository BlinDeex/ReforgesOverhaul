using ModifiersOverhaul.Assets.Balance;
using ModifiersOverhaul.Assets.Misc;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.ModPrefixes.Magic;

public class PrefixEndless : ModPrefix
{
    public override PrefixCategory Category => PrefixCategory.Magic;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    }
    
    public override LocalizedText DisplayName => LocalizationManager.GetPrefixLocalization(this,"Endless", "DisplayName");

    public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult,
        ref float scaleMult,
        ref float shootSpeedMult, ref float manaMult, ref int critBonus)
    {
        manaMult *= PrefixBalance.ENDLESS_MANA_USAGE;
        damageMult *= PrefixBalance.ENDLESS_DAMAGE;
    }
}
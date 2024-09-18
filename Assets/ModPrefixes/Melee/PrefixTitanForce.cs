using ModifiersOverhaul.Assets.Balance;
using ModifiersOverhaul.Assets.Misc;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.ModPrefixes.Melee;

public class PrefixTitanForce : ModPrefix
{
    public override PrefixCategory Category => PrefixCategory.Melee;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    }
    
    public override LocalizedText DisplayName => LocalizationManager.GetPrefixLocalization(this,"TitanForce", "DisplayName");


    public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult,
        ref float scaleMult,
        ref float shootSpeedMult, ref float manaMult, ref int critBonus)
    {
        knockbackMult *= PrefixBalance.TITAN_FORCE_KB;
        damageMult *= PrefixBalance.TITAN_FORCE_DAMAGE;
    }
}
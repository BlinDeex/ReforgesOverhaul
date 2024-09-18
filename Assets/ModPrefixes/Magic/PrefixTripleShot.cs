using System.Collections.Generic;
using ModifiersOverhaul.Assets.Balance;
using ModifiersOverhaul.Assets.Misc;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.ModPrefixes.Magic;

public class PrefixTripleShot : ModPrefix
{
    public override PrefixCategory Category => PrefixCategory.Magic;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    }

    public static LocalizedText Desc { get; private set; }
    
    public override LocalizedText DisplayName => LocalizationManager.GetPrefixLocalization(this,"TripleShot", "DisplayName");

    public override void SetStaticDefaults()
    {
        Desc = LocalizationManager.GetPrefixLocalization(this,"TripleShot", nameof(Desc));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        var newLine = new TooltipLine(Mod, "newLine",
            Desc.Value)
        {
            IsModifier = true
        };

        yield return newLine;
    }

    public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult,
        ref float scaleMult,
        ref float shootSpeedMult, ref float manaMult, ref int critBonus)
    {
        damageMult *= PrefixBalance.TRIPLE_SHOT_DAMAGE_MUL;
        useTimeMult *= PrefixBalance.TRIPLE_SHOT_USE_TIME_MUL;
        manaMult *= PrefixBalance.TRIPLE_SHOT_MANA_MUL;
    }
}
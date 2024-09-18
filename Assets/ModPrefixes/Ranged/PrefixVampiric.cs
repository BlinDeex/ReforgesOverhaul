using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ModifiersOverhaul.Assets.Balance;
using ModifiersOverhaul.Assets.Misc;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.ModPrefixes.Ranged;

public class PrefixVampiric : ModPrefix
{
    public override PrefixCategory Category => PrefixCategory.Ranged;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    }

    public static LocalizedText Desc { get; private set; }
    
    public override LocalizedText DisplayName => LocalizationManager.GetPrefixLocalization(this,"Vampiric", "DisplayName");


    public override void SetStaticDefaults()
    {
        Desc = LocalizationManager.GetPrefixLocalization(this,"Vampiric", nameof(Desc));
    }

    public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult,
        ref float scaleMult,
        ref float shootSpeedMult, ref float manaMult, ref int critBonus)
    {
        useTimeMult *= PrefixBalance.VAMPIRIC_FIRERATE;
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        var newLine2 = new TooltipLine(Mod, "newLine2",
            Desc.Format(Math.Round(PrefixBalance.VAMPIRIC_LIFESTEAL, 2)))
        {
            OverrideColor = new Color(255, 100, 100)
        };

        yield return newLine2;
    }
}
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ModifiersOverhaul.Assets.Balance;
using ModifiersOverhaul.Assets.Misc;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.ModPrefixes.Melee;

public class PrefixPerceptive : ModPrefix
{
    public override PrefixCategory Category => PrefixCategory.Melee;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    }

    public static LocalizedText Desc { get; private set; }
    
    public override LocalizedText DisplayName => LocalizationManager.GetPrefixLocalization(this,"Perceptive", "DisplayName");


    public override void SetStaticDefaults()
    {
        Desc = LocalizationManager.GetPrefixLocalization(this,"Perceptive", nameof(Desc));
    }

    public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult,
        ref float scaleMult,
        ref float shootSpeedMult, ref float manaMult, ref int critBonus)
    {
        critBonus += PrefixBalance.PERCEPTIVE_CRIT;
        damageMult *= PrefixBalance.PERCEPTIVE_CRIT_DAMAGE;
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        var newLine = new TooltipLine(Mod, "newLine",
            Desc.Format(Math.Clamp(Main.LocalPlayer.GetWeaponCrit(item), 0, 200) - 100))
        {
            OverrideColor = Color.YellowGreen
        };

        yield return newLine;
    }
}
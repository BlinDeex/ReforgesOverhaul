﻿using System.Collections.Generic;
using ModifiersOverhaul.Assets.Balance;
using ModifiersOverhaul.Assets.Misc;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.ModPrefixes.Ranged;

public class PrefixAdaptable : ModPrefix
{
    public override PrefixCategory Category => PrefixCategory.Ranged;
    
    public override LocalizedText DisplayName => LocalizationManager.GetPrefixLocalization(this,"Adaptable", "DisplayName");


    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    }

    public static LocalizedText Desc { get; private set; }

    public override void SetStaticDefaults()
    {
        Desc = LocalizationManager.GetPrefixLocalization(this,"Adaptable", nameof(Desc));
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
        useTimeMult *= PrefixBalance.ADAPTABLE_FIRERATE;
    }
}
﻿using System.Collections.Generic;
using ModifiersOverhaul.Assets.Balance;
using ModifiersOverhaul.Assets.Misc;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.ModPrefixes.Magic;

public class PrefixChaotic : ModPrefix
{
    public override PrefixCategory Category => PrefixCategory.Magic;
    public override LocalizedText DisplayName => LocalizationManager.GetPrefixLocalization(this,"Chaotic", "DisplayName");
    public static LocalizedText Desc { get; private set; }

    public override void SetStaticDefaults()
    {
        Desc = LocalizationManager.GetPrefixLocalization(this,"Chaotic", nameof(Desc));
    }

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        var newLine = new TooltipLine(Mod, "newLine",
            Desc.Value)
        {
            OverrideColor = Main.DiscoColor
        };

        yield return newLine;
    }
}
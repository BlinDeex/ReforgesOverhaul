﻿using System;
using System.Collections.Generic;
using ModifiersOverhaul.Assets.Balance;
using ModifiersOverhaul.Assets.Misc;
using ModifiersOverhaul.Assets.ModPlayers;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.ModPrefixes.Accessory;

public class PrefixLethal : ModPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;

    public override LocalizedText DisplayName => LocalizationManager.GetPrefixLocalization(this,"Lethal", "DisplayName");
    public static LocalizedText Desc { get; private set; }

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.ACCESSORY_REFORGING_MULTIPLIER;
    }

    public override void SetStaticDefaults()
    {
        Desc = LocalizationManager.GetPrefixLocalization(this,"Lethal", nameof(Desc));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        var newLine =
            new TooltipLine(Mod, "newLine", Desc.Format(MathF.Round(PrefixBalance.LETHAL_CRIT_DMG_MUL * 100, 2)))
            {
                IsModifier = true
            };

        yield return newLine;
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        if (!player.TryGetModPlayer(out GeneralStatPlayer statPlayer)) return;
        statPlayer.CritDamageMul += PrefixBalance.LETHAL_CRIT_DMG_MUL;
    }
}
﻿using System;
using System.Collections.Generic;
using ModifiersOverhaul.Assets.Balance;
using ModifiersOverhaul.Assets.Misc;
using ModifiersOverhaul.Assets.ModPlayers;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.ModPrefixes.Accessory;

public class PrefixRisky : ModPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;

    public override LocalizedText DisplayName => LocalizationManager.GetPrefixLocalization(this,"Risky", "DisplayName");
    public static LocalizedText DescDefense { get; private set; }
    public static LocalizedText DamageDesc { get; private set; }

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.ACCESSORY_REFORGING_MULTIPLIER;
    }

    public override void SetStaticDefaults()
    {
        DescDefense = LocalizationManager.GetPrefixLocalization(this,"Risky", nameof(DescDefense));
        DamageDesc = SharedLocalization.GetSharedLocalizedText(SharedLocalization.XDamageAdded);
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        var newLine =
            new TooltipLine(Mod, "newLine", DescDefense.Format(PrefixBalance.RISKY_DEFENSE_DECREASE * 100))
            {
                IsModifier = true,
                IsModifierBad = true
            };

        var newLine2 =
            new TooltipLine(Mod, "newLine", DamageDesc.Format(Math.Round(PrefixBalance.RISKY_DAMAGE_INCREASE * 100, 2)))
            {
                IsModifier = true
            };

        yield return newLine;
        yield return newLine2;
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        if (!player.TryGetModPlayer(out GeneralStatPlayer statPlayer)) return;
        statPlayer.DamageMul += PrefixBalance.RISKY_DAMAGE_INCREASE;
        //player.GetDamage<RangedDamageClass>() *= PrefixBalance.RISKY_DAMAGE_INCREASE;
        statPlayer.DefenseMul += PrefixBalance.RISKY_DEFENSE_DECREASE;
    }
}
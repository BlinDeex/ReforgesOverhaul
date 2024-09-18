﻿using System.Collections.Generic;
using ModifiersOverhaul.Assets.Balance;
using ModifiersOverhaul.Assets.Misc;
using ModifiersOverhaul.Assets.ModPlayers;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.ModPrefixes.Accessory;

public class PrefixAerodynamic : ModPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;

    public override LocalizedText DisplayName => LocalizationManager.GetPrefixLocalization(this,"Aerodynamic", "DisplayName");
    public static LocalizedText Desc { get; private set; }
    
    

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.ACCESSORY_REFORGING_MULTIPLIER;
    }

    public override void SetStaticDefaults()
    {
        Desc = LocalizationManager.GetPrefixLocalization(this,"Aerodynamic", nameof(Desc));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        var newLine =
            new TooltipLine(Mod, "newLine",
                Desc.Format(PrefixBalance.AERODYNAMIC_MOVEMENT_MULTIPLIER * 100,
                    PrefixBalance.AERODYNAMIC_WING_TIME_TICKS))
            {
                IsModifier = true
            };

        yield return newLine;
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        if (!player.TryGetModPlayer(out GeneralStatPlayer statPlayer)) return;
        statPlayer.MovementSpeedMul += PrefixBalance.AERODYNAMIC_MOVEMENT_MULTIPLIER;
        statPlayer.WingTime += PrefixBalance.AERODYNAMIC_WING_TIME_TICKS;
    }
}
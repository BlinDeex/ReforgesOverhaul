using System;
using System.Collections.Generic;
using ModifiersOverhaul.Assets.Balance;
using ModifiersOverhaul.Assets.Misc;
using ModifiersOverhaul.Assets.ModPlayers;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.ModPrefixes.Accessory;

public class PrefixStoic : ModPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;

    public override LocalizedText DisplayName => LocalizationManager.GetPrefixLocalization(this,"Stoic", "DisplayName");
    public static LocalizedText DescDefense { get; private set; }
    public static LocalizedText DescDamage { get; private set; }

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.ACCESSORY_REFORGING_MULTIPLIER;
    }

    public override void SetStaticDefaults()
    {
        DescDefense = LocalizationManager.GetPrefixLocalization(this,"Stoic", nameof(DescDefense));
        DescDamage = LocalizationManager.GetPrefixLocalization(this,"Stoic", nameof(DescDamage));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        var newLine =
            new TooltipLine(Mod, "newLine",
                DescDefense.Format(Math.Round(PrefixBalance.STOIC_DEFENSE_INCREASE * 100, 2)))
            {
                IsModifier = true
            };

        var newLine2 =
            new TooltipLine(Mod, "newLine",
                DescDamage.Format(Math.Round(PrefixBalance.STOIC_DAMAGE_DECREASE * 100, 2)))
            {
                IsModifier = true,
                IsModifierBad = true
            };

        yield return newLine;
        yield return newLine2;
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        if (!player.TryGetModPlayer(out GeneralStatPlayer statPlayer)) return;
        statPlayer.DamageMul -= PrefixBalance.STOIC_DAMAGE_DECREASE;
        statPlayer.DefenseMul += PrefixBalance.STOIC_DEFENSE_INCREASE;
    }
}
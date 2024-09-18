using System;
using System.Collections.Generic;
using ModifiersOverhaul.Assets.Balance;
using ModifiersOverhaul.Assets.Globals;
using ModifiersOverhaul.Assets.Globals.Items;
using ModifiersOverhaul.Assets.Misc;
using ModifiersOverhaul.Assets.ModPlayers;
using ModifiersOverhaul.Assets.ModPrefixes.Magic;
using Terraria;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.InstancedGlobalItems;

public class InstancedMagicPrefix : GlobalItem
{
    public override bool InstancePerEntity => true;

    public float DamageAdded { get; private set; }

    public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
    {
        var statPlayer = player.GetModPlayer<GeneralStatPlayer>();

        if (item.prefix == ModContent.PrefixType<PrefixManaCharged>())
        {
            var multiplier = (float)player.statMana / player.statManaMax2;
            var damageAdded = multiplier * PrefixBalance.MANA_CHARGED_MAX_DAMAGE_GAIN;
            damage.Base += damageAdded * item.damage;
            DamageAdded = damageAdded;
            return;
        }

        if (item.prefix == ModContent.PrefixType<PrefixChaotic>()) damage *= statPlayer.DamageMul;
    }

    public override float UseSpeedMultiplier(Item item, Player player)
    {
        var statPlayer = player.GetModPlayer<GeneralStatPlayer>();

        if (item.prefix == ModContent.PrefixType<PrefixChaotic>())
        {
            var modifiedUseTime = Math.Clamp(statPlayer.UseTimeMul, 0f, 10f);
            return modifiedUseTime;
        }

        return 1f;
    }

    public override float UseAnimationMultiplier(Item item, Player player)
    {
        var statPlayer = player.GetModPlayer<GeneralStatPlayer>();

        if (item.prefix == ModContent.PrefixType<PrefixChaotic>())
        {
            var modifiedUseTime = Math.Clamp(statPlayer.UseTimeMul, 0f, 10f);
            return modifiedUseTime;
        }

        return 1f;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (item.prefix == ModContent.PrefixType<PrefixManaCharged>())
        {
            var newLine = new TooltipLine(Mod, "newLine",
                SharedLocalization.GetSharedLocalizedText(SharedLocalization.XDamageAdded)
                    .Format(Math.Round(DamageAdded * 100, 2)))
            {
                IsModifier = true
            };

            tooltips.Add(newLine);
            return;
        }

        if (item.prefix == ModContent.PrefixType<PrefixChaotic>())
        {
            tooltips.AddRange(GlobalMagicPrefix.CurrentChaoticTooltipLines);
            return;
        }
    }
}
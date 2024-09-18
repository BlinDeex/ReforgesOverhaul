using Microsoft.Xna.Framework;
using ModifiersOverhaul.Assets.ModPlayers;
using ModifiersOverhaul.Assets.ModPrefixes.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.Globals.Items;

public class GlobalMeleePrefix : GlobalItem
{
    public override bool CanUseItem(Item item, Player player)
    {
        if (item.prefix == 0) return base.CanUseItem(item, player);

        if (item.prefix == ModContent.PrefixType<PrefixArcaneInfused>())
        {
            var usedMana = player.CheckMana(10, true);
            player.manaRegenDelay = 30;
            return usedMana;
        }

        return true;
    }

    public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
    {
        var prefixPlayer = player.GetModPlayer<PrefixPlayer>();
        if (player.HasBuff(BuffID.ManaSickness) && item.prefix == ModContent.PrefixType<PrefixArcaneInfused>())
        {
            var index = player.FindBuffIndex(BuffID.ManaSickness);
            var timeLeft = player.buffTime[index];
            var targetReduction = timeLeft * 0.1f;
            damage.Flat = 1f - targetReduction;
        }

        if (item.prefix == ModContent.PrefixType<PrefixUntouchable>())
            damage *= prefixPlayer.UntouchableDamageIncrease + 1;
    }

    public override bool? CanAutoReuseItem(Item item, Player player)
    {
        return item.prefix == ModContent.PrefixType<PrefixUltraLight>() ? true : base.CanAutoReuseItem(item, player);
    }

    public override void ModifyHitNPC(Item item, Player player, NPC target, ref NPC.HitModifiers modifiers)
    {
        PrefixPerceptive(item, player, target, ref modifiers);
    }

    private void PrefixPerceptive(Item item, Player player, NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.ModifyHitInfo += (ref NPC.HitInfo info) =>
        {
            if (!info.Crit) return;
            if (item.crit <= 100) return;
            var leftoverCrit = item.crit - 100;
            var dice = Main.rand.Next(0, 100);
            if (dice > leftoverCrit) return;
            info.Damage *= 2;
            info.HideCombatText = true;
            var combatText = CombatText.NewText(UtilMethods.GetCombatTextRect(target), Color.LightGoldenrodYellow,
                info.Damage, true);
            if (combatText > Main.combatText.Length - 1) return;
            Main.combatText[combatText].scale = 1.4f;
        };
    }
}
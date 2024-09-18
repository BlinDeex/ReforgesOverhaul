using Microsoft.Xna.Framework;
using ModifiersOverhaul.Assets.InstancedGlobalItems;
using Terraria;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.Globals.Projectiles;

public class MeleeProjectile : GlobalProjectile
{
    public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
    {
        var projPrefix = projectile.GetGlobalProjectile<InstancedProjectilePrefix>();

        PrefixPerceptive(projectile, target, ref modifiers, projPrefix);
    }

    public static void PrefixPerceptive(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers,
        InstancedProjectilePrefix projPrefix)
    {
        modifiers.ModifyHitInfo += (ref NPC.HitInfo info) =>
        {
            if (!info.Crit) return;
            if (projPrefix.ItemCrit <= 100) return;
            var leftoverCrit = projPrefix.ItemCrit - 100;
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
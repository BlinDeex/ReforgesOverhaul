using Microsoft.Xna.Framework;
using ModifiersOverhaul.Assets.Balance;
using ModifiersOverhaul.Assets.InstancedGlobalItems;
using ModifiersOverhaul.Assets.Misc;
using ModifiersOverhaul.Assets.ModPlayers;
using ModifiersOverhaul.Assets.ModPrefixes.Magic;
using ModifiersOverhaul.Assets.Projectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.Globals.Projectiles;

public class MagicProjectile : GlobalProjectile
{
    public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
    {
        if (!projectile.TryGetGlobalProjectile(out InstancedProjectilePrefix projPrefix))
        {
            UtilMethods.LogError($"Couldn't get {nameof(InstancedProjectilePrefix)}!", 003);
            return;
        }

        PrefixChaotic(projectile, target, ref modifiers, projPrefix);
        PrefixSplintering(projectile, target, ref modifiers);
    }

    private static void PrefixChaotic(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers,
        InstancedProjectilePrefix projPrefix)
    {
        if (projPrefix.GunPrefixType != ModContent.PrefixType<PrefixChaotic>()) return;

        var statPlayer = Main.LocalPlayer.GetModPlayer<GeneralStatPlayer>();

        if (statPlayer.MaxHealthDMG == 0) return;

        float targetDamage = target.realLife;
        WeaponUtils.DealTrueDamage(target, ref modifiers, targetDamage, false);
    }

    private static void PrefixSplintering(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
    {
        if (Main.player[projectile.owner].HeldItem.prefix != ModContent.PrefixType<PrefixSplintering>()) return;
        if (projectile.type == ModContent.ProjectileType<SplinteringProjectile>()) return;
        if (Main.rand.NextFloat() > PrefixBalance.SPLINTERING_CHANCE && !PrefixBalance.DEV_MODE) return;

        var nodes = Main.rand.Next(PrefixBalance.SPLINTERING_MIN_SHARD_NODES,
            PrefixBalance.SPLINTERING_MAX_SHARD_NODES);

        modifiers.ModifyHitInfo += (ref NPC.HitInfo info) =>
        {
            if (info.Crit) nodes *= PrefixBalance.SPLINTERING_SHARD_NODES_CRIT_MULT;
        };

        for (var i = 0; i < nodes; i++)
        {
            var nodePos = UtilMethods.GetRandomPositionInRectangle(target.Hitbox, Main.rand);
            var projectiles = Main.rand.Next(PrefixBalance.SPLINTERING_MIN_NODE_PROJ,
                PrefixBalance.SPLINTERING_MAX_NODE_PROJ);
            var variation = Main.rand.NextFloat(-PrefixBalance.SPLINTERING_PROJ_VELOCITY_VARIATION_MUL,
                PrefixBalance.SPLINTERING_PROJ_VELOCITY_VARIATION_MUL);
            var velMult = PrefixBalance.SPLINTERING_PROJ_VELOCITY + variation;

            for (var j = 0; j < projectiles; j++)
            {
                var projPos = UtilMethods.RandomPointInCircle(nodePos.X, nodePos.Y, 1f, Main.rand);
                var dir = Vector2.Zero;
                dir.X = Main.rand.NextFloat();
                dir.Y = 1 - dir.X;
                dir.X = Main.rand.NextBool() ? dir.X : -dir.X;
                dir.Y = Main.rand.NextBool() ? dir.Y : -dir.Y;
                var velocity = dir * velMult;

                Projectile.NewProjectile(new EntitySource_Misc("SplinteringSpawn"), projPos, velocity,
                    ModContent.ProjectileType<SplinteringProjectile>(), projectile.damage, 0, Main.LocalPlayer.whoAmI, ai0: target.whoAmI);
            }
        }
    }

    public static void OnTripleShotRootSpawn(Projectile projectile, InstancedProjectilePrefix projPrefix)
    {
        if (projPrefix.GunPrefixType != ModContent.PrefixType<PrefixTripleShot>()) return;
        if (projPrefix.TripleShootClone) return;


        var newVelocities = CombatUtils.TripleShotRotatedVelocities(projectile.velocity,
            PrefixBalance.TRIPLE_SHOT_DEGREES, PrefixBalance.TRIPLE_SHOT_DEGREES_VARIATION);

        var newProj1 = Projectile.NewProjectileDirect(projectile.GetSource_FromThis(null), projectile.position,
            newVelocities.Item1, projectile.type, projectile.damage, projectile.knockBack, projectile.owner,
            projectile.ai[0], projectile.ai[1], projectile.ai[2]);

        var newProj2 = Projectile.NewProjectileDirect(projectile.GetSource_FromThis(null), projectile.position,
            newVelocities.Item2, projectile.type, projectile.damage, projectile.knockBack, projectile.owner,
            projectile.ai[0], projectile.ai[1], projectile.ai[2]);

        newProj1.GetGlobalProjectile<InstancedProjectilePrefix>().TripleShootClone = true;
        newProj2.GetGlobalProjectile<InstancedProjectilePrefix>().TripleShootClone = true;
    }
}
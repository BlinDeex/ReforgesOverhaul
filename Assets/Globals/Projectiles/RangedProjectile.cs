using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using ModifiersOverhaul.Assets.Balance;
using ModifiersOverhaul.Assets.InstancedGlobalItems;
using ModifiersOverhaul.Assets.Misc;
using ModifiersOverhaul.Assets.ModPrefixes.Ranged;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.Globals.Projectiles;

public class RangedProjectile : GlobalProjectile
{
    public static void AdaptableSpawn(Projectile projectile, IEntitySource source, InstancedProjectilePrefix projPrefix)
    {
        if (projPrefix.AdaptableSwapped) return;
        var ammoID = projPrefix.AmmoTypeUsed;
        
        if (ammoID < 0) return;
        
        var itemID = projPrefix.ItemUsed.type;

        var rocketWeapon = AdaptableUtils.ROCKET_WEAPON_IDS.Contains(itemID);
        var rocketProj = AdaptableUtils.ROCKET_TO_PROJ_IDS.Select(x => x.RocketAmmoID).Contains(ammoID);
        var rocketInsideNotRocketWeap = rocketProj && !rocketWeapon;

        if (!rocketInsideNotRocketWeap) return;

        var targetSwap = AdaptableUtils.ROCKET_TO_PROJ_IDS.First(x => x.RocketAmmoID == ammoID).RocketProjectileID;
        projectile.Kill();
        var swappedProj = Projectile.NewProjectileDirect(projectile.GetSource_FromThis(), projectile.position, projectile.velocity,
            targetSwap, projectile.damage, projectile.knockBack, projectile.owner);
        swappedProj.GetGlobalProjectile<InstancedProjectilePrefix>().AdaptableSwapped = true;
        
    }

    public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
    {
        var projPrefix = projectile.GetGlobalProjectile<InstancedProjectilePrefix>();

        PrefixAscendant(projPrefix, damageDone);
    }

    public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
    {
        var projPrefix = projectile.GetGlobalProjectile<InstancedProjectilePrefix>();

        PrefixGiantSlayer(target, ref modifiers, projPrefix);
    }

    private void PrefixGiantSlayer(NPC target, ref NPC.HitModifiers modifiers, InstancedProjectilePrefix projPrefix)
    {
        if (projPrefix.GunPrefixType != ModContent.PrefixType<PrefixGiantSlayer>()) return;
        

        var targetDamage = target.lifeMax * PrefixBalance.GIANT_SLAYER_PERCENT_DAMAGE;

        WeaponUtils.DealTrueDamage(target, ref modifiers, targetDamage, true);
    }

    private static void PrefixAscendant(InstancedProjectilePrefix projPrefix, int damageDone)
    {
        if (projPrefix.GunPrefixType != ModContent.PrefixType<PrefixAscendant>()) return;
        if (!projPrefix.ItemUsed.TryGetGlobalItem(out InstancedRangedPrefix rangedPrefix)) return;
        rangedPrefix.DamageDone += damageDone;
    }

    public static void TracerSpawn(Projectile projectile, InstancedProjectilePrefix projPrefix)
    {
        if (projPrefix.GunPrefixType != ModContent.PrefixType<PrefixTracer>()) return;
        projPrefix.Tracer = true;
        projectile.timeLeft = PrefixBalance.TRACER_MAXIMUM_LIFETIME;
    }

    public static void TracerPreAI(Projectile projectile, InstancedProjectilePrefix projPrefix)
    {
        if (projectile.timeLeft % PrefixBalance.TRACER_PATH_RESOLUTION != 0) return;
        
        projPrefix.TracerPathPoints.Add(projectile.position);
    }

    public static void TracerLineBoom(Projectile projectile, InstancedProjectilePrefix projPrefix)
    {
        int targetDustPoints = projPrefix.TracerPathPoints.Count * PrefixBalance.TRACER_DUST_POSITIONS_BETWEEN_POINTS;
        List<Vector2> dustPositions = new(targetDustPoints);
        List<Vector2> tracerPoints = projPrefix.TracerPathPoints;

        for (int i = 0; i < tracerPoints.Count; i++)
        {
            Vector2 tracerPoint = tracerPoints[i];
            dustPositions.Add(tracerPoint);

            if (i == tracerPoints.Count - 1) break;
            Vector2 nextTracerPoint = tracerPoints[i + 1];
            dustPositions.AddRange(GetEvenlySpacedPoints(tracerPoint, nextTracerPoint, PrefixBalance.TRACER_DUST_POSITIONS_BETWEEN_POINTS));
        }

        foreach (Vector2 dustPosition in dustPositions)
        {
            Dust.NewDust(dustPosition, 0, 0, DustID.ShadowbeamStaff);
        }
    }
    
    public static List<Vector2> GetEvenlySpacedPoints(Vector2 start, Vector2 end, int numPoints)
    {
        List<Vector2> points = [];
        
        Vector2 step = (end - start) / (numPoints - 1);
        
        for (int i = 1; i < numPoints - 1; i++)
        {
            points.Add(start + step * i);
        }

        return points;
    }
}
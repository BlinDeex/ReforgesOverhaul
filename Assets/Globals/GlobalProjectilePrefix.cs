using Microsoft.Xna.Framework;
using ModifiersOverhaul.Assets.Globals.Projectiles;
using ModifiersOverhaul.Assets.InstancedGlobalItems;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.Globals;

public class GlobalProjectilePrefix : GlobalProjectile
{
    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        InstancedProjectilePrefix projPrefix = projectile.GetGlobalProjectile<InstancedProjectilePrefix>();

        if (source is IEntitySource_WithStatsFromItem itemUsed)
        {
            projPrefix.GunPrefixType = itemUsed.Item.prefix;
            projPrefix.ItemCrit = itemUsed.Item.crit;
            projPrefix.ItemUsed = itemUsed.Item;
        }

        if (source is EntitySource_ItemUse_WithAmmo ammo) projPrefix.AmmoTypeUsed = ammo.AmmoItemIdUsed;

        SummonerProjectile.OnMinionSpawn(projectile, source, projPrefix);
        RangedProjectile.AdaptableSpawn(projectile, source, projPrefix);
        MagicProjectile.OnTripleShotRootSpawn(projectile, projPrefix);
        RangedProjectile.TracerSpawn(projectile, projPrefix);
    }

    public override bool PreAI(Projectile projectile)
    {
        InstancedProjectilePrefix projPrefix = projectile.GetGlobalProjectile<InstancedProjectilePrefix>();
        bool runPreAI = true;
        
        RangedProjectile.TracerPreAI(projectile, projPrefix, ref runPreAI);
        TimeStop(projPrefix, projectile, ref runPreAI);
        return runPreAI;
    }

    private void TimeStop(InstancedProjectilePrefix projPrefix, Projectile projectile, ref bool runAI)
    {
        if (!projPrefix.TimeStop) return;
       
        projectile.position = projectile.oldPosition;
        projectile.timeLeft++;
        
        projPrefix.TimeStopTicks--;

        if (projPrefix.TimeStopTicks > 0)
        {
            runAI = false;
            return;
        }
            
        projPrefix.TimeStop = false;
        projectile.netUpdate = true;
        runAI = false;
    }
    
    
    public override void OnKill(Projectile projectile, int timeLeft)
    {
        InstancedProjectilePrefix projPrefix = projectile.GetGlobalProjectile<InstancedProjectilePrefix>();
        
        if (projPrefix.Tracer)
        {
            RangedProjectile.TracerLineBoom(projectile, projPrefix);
            return;
        }
    }
}
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
        //if (projectile.owner != Main.myPlayer) return;
        if (!projectile.TryGetGlobalProjectile(out InstancedProjectilePrefix projPrefix))
        {
            UtilMethods.LogError($"Failed to get {nameof(InstancedProjectilePrefix)}", 001);
            return;
        }

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
        
        if (projPrefix.Tracer)
        {
            RangedProjectile.TracerPreAI(projectile, projPrefix);
            return true;
        }
        
        if (projPrefix.TimeStop)
        {
            if (!projPrefix.TimeStopVelocity.HasValue)
            {
                projPrefix.TimeStopVelocity = projectile.velocity;
            }
            
            projPrefix.TimeStopTicks--;
            projectile.velocity = Vector2.Zero;
            
            if (projPrefix.TimeStopTicks <= 0)
            {
                projPrefix.TimeStop = false;
                projectile.velocity = projPrefix.TimeStopVelocity.Value;
                projPrefix.TimeStopVelocity = null;
                return false;
            }
            return false;
        }
        
        return true;
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
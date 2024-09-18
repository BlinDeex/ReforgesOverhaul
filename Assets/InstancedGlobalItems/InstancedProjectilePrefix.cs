using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.InstancedGlobalItems;

public class InstancedProjectilePrefix : GlobalProjectile
{
    public override bool InstancePerEntity => true;
    public Item ItemUsed { get; set; }
    public int GunPrefixType { get; set; }
    public int ItemCrit { get; set; }

    public bool TripleShootClone { get; set; }

    /// <summary>
    /// Used for minions by frenzied prefix
    /// </summary>
    public bool Frenzied { get; set; }

    public bool AdaptableSwapped { get; set; }

    public int AmmoTypeUsed { get; set; } = -1;
    
    public bool Tracer { get; set; }
    public List<Vector2> TracerPathPoints { get; set; } = [];
    
    public bool TimeStop { get; set; }
    public int TimeStopTicks { get; set; }
    public Vector2? TimeStopVelocity { get; set; } = null;
    public float[] TimeStopAI = null;

    public void ActivateTimeStop(int ticks, Projectile proj)
    {
        TimeStop = true;
        TimeStopTicks = ticks * proj.extraUpdates + 1;
        TimeStopAI = proj.ai;
    }
}
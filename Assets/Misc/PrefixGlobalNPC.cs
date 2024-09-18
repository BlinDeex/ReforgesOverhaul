using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.Misc;

public class PrefixGlobalNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;
    
    public int ChallengerOwner { get; set; }

    private int frameHeightAtTimeStop;
    private bool initiateTimeStop;
    private int timeStopTicks;
    private bool timeStopActive;
    

    public override bool PreAI(NPC npc)
    {
        bool runAI = true;
        TickTimeStop(ref runAI, npc);
        
        return runAI;
    }

    private void TickTimeStop(ref bool runAI, NPC npc)
    {
        if (initiateTimeStop)
        {
            initiateTimeStop = false;
            timeStopActive = true;
            runAI = false;

            frameHeightAtTimeStop = npc.frame.Y;
            
            return;
        }

        if (!timeStopActive) return;
        
        if (timeStopTicks <= 0)
        {
            timeStopActive = false;
            return;
        }
        
        npc.velocity = Vector2.Zero;
        npc.GravityMultiplier *= 0f;

        timeStopTicks--;

        runAI = false;
    }

    public void TimeStop(int ticks)
    {
        initiateTimeStop = true;
        timeStopTicks = ticks;
    }

    public override void FindFrame(NPC npc, int frameHeight)
    {
        if (timeStopActive)
        {
            npc.frame = new Rectangle(npc.frame.X, frameHeightAtTimeStop, npc.frame.Width, npc.frame.Height);
            return;
        }

        base.FindFrame(npc, frameHeight);
    }

    public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
    {
        // ReSharper disable once ReplaceWithSingleAssignment.True
        bool canHit = true;
        if (timeStopActive) canHit = false;
        return canHit;
    }
    
    
}
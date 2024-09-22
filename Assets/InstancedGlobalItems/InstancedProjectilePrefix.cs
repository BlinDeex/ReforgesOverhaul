using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

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

    public void ActivateTimeStop(int ticks, Projectile proj) // called on server/singleplayer only
    {
        if (Main.projHook[proj.type]) return;
        TimeStop = true; // used in another GlobalProjectile inside PreAI
        TimeStopTicks = ticks * proj.extraUpdates + 1; // used in another GlobalProjectile inside PreAI
        //proj.netUpdate = true; // some weird desync issues, projectile still moving then teleporting back and moving again when timestop ends
        NetMessage.SendData(MessageID.SyncProjectile, number: proj.identity); // works perfectly
    }

    public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
    {
        bitWriter.WriteBit(TimeStop);
        binaryWriter.Write(TimeStopTicks);
    }

    public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
    {
        TimeStop = bitReader.ReadBit();
        TimeStopTicks = binaryReader.ReadInt32();
    }
}
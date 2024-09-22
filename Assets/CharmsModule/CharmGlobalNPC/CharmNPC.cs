using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ModifiersOverhaul.Assets.CharmsModule.Manager;
using ModifiersOverhaul.Assets.Misc;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.CharmsModule.CharmGlobalNPC;

public class CharmNPC : GlobalNPC
{

    public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
    {
        if(npc.life <= 0) OnKilled(npc, player, item); 
    }

    private void OnKilled(NPC npc, Player player, Item item = null, Projectile projectile = null)
    {
        if (Main.myPlayer != player.whoAmI) return;
        if (Main.netMode == NetmodeID.Server) return;
        if (CharmBalance.ExcludedNPCSFromCharmDrops.Contains(npc.type)) return;
        
        if (Main.netMode == NetmodeID.SinglePlayer)
        {
            List<(CharmRarity, CharmType)> rolledCharms = CharmsManager.RollForCharms(npc: npc);
            CharmsManager.SpawnCharms(rolledCharms, npc);
            return;
        }

        ModPacket packet = Mod.GetPacket();
        packet.Write((byte)MessageType.CharmOnKilled);
        packet.Write(npc.Center.X);
        packet.Write(npc.Center.Y);
        packet.Write(npc.boss);
        packet.Send();
    }

    public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
    {
        if(npc.life <= 0) OnKilled(npc, Main.player[projectile.owner], projectile: projectile); 
    }
}
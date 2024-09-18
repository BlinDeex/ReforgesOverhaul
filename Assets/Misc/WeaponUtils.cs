using Microsoft.Xna.Framework;
using ModifiersOverhaul.Assets.Balance;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.Misc;

public static class WeaponUtils
{
    public static void DealTrueDamage(NPC target, ref NPC.HitModifiers modifiers, float trueDamageDealt,
        bool disableNormalDamage)
    {
        if (target.type == NPCID.TargetDummy && !PrefixBalance.DEV_MODE) return;
        
        if (disableNormalDamage)
        {
            modifiers.FinalDamage *= 0;
            modifiers.HideCombatText();
            target.life++; // damage cant be lower than 1
        }
        
        if (trueDamageDealt < 1) trueDamageDealt = 1;
        int dmgInt = (int)trueDamageDealt;

        NPC.HitInfo hit = new()
        {
            HideCombatText = true,
            Damage = dmgInt,
            Crit = false
        };
        
        target.StrikeNPC(hit);
        target.HitEffect(hit);
        
        int textWhoAmI = CombatText.NewText(UtilMethods.GetCombatTextRect(target), Color.WhiteSmoke, dmgInt, true, true);

        if (Main.netMode == NetmodeID.SinglePlayer) return;
        
        float x = Main.combatText[textWhoAmI].position.X;
        float y = Main.combatText[textWhoAmI].position.Y;
        
        NetMessage.SendStrikeNPC(target,in hit);

        ModPacket packet = ModifiersOverhaul.Instance.GetPacket();
        packet.Write((byte)MessageType.TrueDamageText);
        packet.Write((int)Color.WhiteSmoke.PackedValue);
        packet.Write(x);
        packet.Write(y);
        packet.Write(dmgInt);
        packet.Write(Main.LocalPlayer.whoAmI);
        packet.Send();
        
        
    }

    public static void ShowTrueDamageText(int targetWhoAmI, int damage)
    {
        NPC target = Main.npc[targetWhoAmI];
        if (!target.active) return;
        CombatText.NewText(UtilMethods.GetCombatTextRect(target), Color.WhiteSmoke, damage, true, true);
    }
}
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ModifiersOverhaul.Assets.Balance;
using ModifiersOverhaul.Assets.InstancedGlobalItems;
using ModifiersOverhaul.Assets.Misc;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.ModPlayers.Armor;

public class ChronoArmorPlayer : ModPlayer
{
    public int ChronoPiecesEquipped { get; set; }
    
    public bool ChronoSetBonus;
    
    private static SoundStyle soundEffect = new("ModifiersOverhaul/Assets/Sounds/TimeStop");
    
    public override void PostUpdateEquips()
    {
        ChronoSetBonus = ChronoPiecesEquipped == 3;
        if(ChronoSetBonus) Player.GetModPlayer<ArmorAbilityPlayer>().SetArmorAbility(StopTimeAbility);
    }

    public override void ResetEffects()
    {
        ChronoPiecesEquipped = 0;
    }

    private int StopTimeAbility()
    {
        List<NPC> npcsInRange = [];
        List<Projectile> projInRange = [];
        Vector2 playerPos = Player.Center;
        float maxDist = PrefixBalance.CHRONO_ABILITY_RANGE;
        
        foreach (NPC npc in Main.ActiveNPCs)
        {
            if (npc.friendly) continue;
            float range = Vector2.DistanceSquared(playerPos, npc.Center);
            if (range > maxDist) continue;
            npcsInRange.Add(npc);
        }

        foreach (Projectile proj in Main.ActiveProjectiles)
        {
            if (proj.friendly) continue;
            float range = Vector2.DistanceSquared(playerPos, proj.Center);
            if (range > maxDist) continue;
            projInRange.Add(proj);
        }

        foreach (NPC npc in npcsInRange)
        {
            npc.GetGlobalNPC<PrefixGlobalNPC>().TimeStop(PrefixBalance.CHRONO_ABILITY_LENGTH);
        }

        foreach (Projectile proj in projInRange)
        {
            proj.GetGlobalProjectile<InstancedProjectilePrefix>().ActivateTimeStop(PrefixBalance.CHRONO_ABILITY_LENGTH, proj);
        }
        
        
        SoundEngine.PlaySound(soundEffect);
        
        return PrefixBalance.CHRONO_ABILITY_COOLDOWN;
    }
}
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.ModPlayers.Armor;

public class MiscArmorPlayer : ModPlayer
{
    public bool WaterBreathing { get; set; }
    
    

    public override void ResetEffects()
    {
        WaterBreathing = false;
        
    }

    public int WaterBreathingPrefix()
    {
        int breath = Player.breath;
        
        if (!WaterBreathing) return breath;
        if (Player.breath >= Player.breathMax) return breath;
        if (Player.breathCD != Player.breathCDMax - 1) return breath;
        
        float dice = Main.rand.NextFloat();
        if (dice > 0.67f) return breath;
        
        breath++;
        
        return breath;
    }
}
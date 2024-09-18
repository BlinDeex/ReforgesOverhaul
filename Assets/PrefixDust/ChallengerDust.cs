﻿using Microsoft.Xna.Framework;
using ModifiersOverhaul.Assets.Misc;
using ModifiersOverhaul.Assets.PrefixNPCS;
using Terraria;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.PrefixDust;

public class ChallengerDust : ModDust
{
    public override bool Update(Dust dust)
    {
        dust.rotation += 0.1f * (dust.dustIndex % 2 == 0 ? -1 : 1);
        //dust.velocity *= 0.99f;
        dust.scale -= 0.02f;

        var orbType = dust.customData is ChallengerOrbType type ? type : 0;
        Lighting.AddLight(dust.position, ColorToVector(ChallengerOrb.GetOrbDustColor(orbType), 750f));
        dust.position += dust.velocity;
        dust.velocity *= 0.92f;
        if (dust.scale < 0.2f) dust.active = false;

        return false;
    }

    public override void OnSpawn(Dust dust)
    {
        var frameX = 0;
        var frameY = Main.rand.Next(0, 3) * 10;
        dust.frame = new Rectangle(frameX, frameY, 8, 8);

        dust.noGravity = true;
    }

    private Vector3 ColorToVector(Color color, float divisor)
    {
        return new Vector3(color.R / divisor, color.G / divisor, color.B / divisor);
    }
}
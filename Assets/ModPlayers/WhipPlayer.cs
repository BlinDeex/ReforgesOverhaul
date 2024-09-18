﻿using ModifiersOverhaul.Assets.Balance;
using ModifiersOverhaul.Assets.Buffs;
using ModifiersOverhaul.Assets.ModPrefixes.Summoner.Whips;
using Terraria;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.ModPlayers;

public class WhipPlayer : ModPlayer
{
    public int SacrificialCooldown { get; set; }

    public override void OnHitAnything(float x, float y, Entity victim)
    {
        if (Player.HeldItem.prefix == ModContent.PrefixType<PrefixSacrificial>()) SacrificialHit();
    }

    public override void PostUpdateEquips()
    {
        SacrificialCooldown--;
    }

    private void SacrificialHit()
    {
        if (SacrificialCooldown > 0) return;
        Player.AddBuff(ModContent.BuffType<SacrificialBuff>(), PrefixBalance.SACRIFICIAL_ON_HIT_BUFF_TICKS);
    }
}
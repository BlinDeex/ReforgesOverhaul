using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ModifiersOverhaul.Assets.Balance;
using ModifiersOverhaul.Assets.Buffs;
using ModifiersOverhaul.Assets.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.ModPlayers.Armor;

public class PhalanxArmorPlayer : ModPlayer
{
    public int PhalanxPiecesEquipped { get; set; }
    
    public bool PhalanxSetBonus;

    private static SoundStyle explosion = new("ModifiersOverhaul/Assets/Sounds/PhalanxExplosion");
    
    public override void PostUpdateEquips()
    {
        PhalanxSetBonus = PhalanxPiecesEquipped == 3;
    }

    public override void ResetEffects()
    {
        PhalanxPiecesEquipped = 0;
    }

    public override bool CanBeHitByProjectile(Projectile proj)
    {
        if (!PhalanxSetBonus) return true;
        if (proj.damage < PrefixBalance.PHALANX_MIN_DAMAGE_TO_REACT && Player.statLife > proj.damage) return true;
        if (Player.HasBuff(ModContent.BuffType<ArmorAbilityCooldownBuff>())) return true;
        
        Player.AddBuff(ModContent.BuffType<ArmorAbilityCooldownBuff>(), PrefixBalance.PHALANX_REACT_COOLDOWN_TICKS);

        GeneralStatPlayer gsPlayer = Player.GetModPlayer<GeneralStatPlayer>();
        gsPlayer.SetInvincibilityTicks(30);

        Vector2 playerToProjDir = (proj.Center - Player.Center).SafeNormalize(Vector2.UnitY);
        
        float midBurstSpeed = 20f;
        float sideBurstSpeed = 6f;
        float shockwaveSpeed = 1f;

        List<Vector2> sideSpawns = [];
        List<Vector2> frontSpawns = [];
        List<Vector2> shockwaveSpawns = [];
        
        sideSpawns.Add(RotateVector(playerToProjDir, 90 + Main.rand.Next(-5,5)));
        sideSpawns.Add(RotateVector(playerToProjDir, 80 + Main.rand.Next(-5,5)));
        sideSpawns.Add(RotateVector(playerToProjDir, -90 + Main.rand.Next(-5,5)));
        sideSpawns.Add(RotateVector(playerToProjDir, -80 + Main.rand.Next(-5,5)));
        
        sideSpawns.Add(RotateVector(playerToProjDir, 70 + Main.rand.Next(-5,5)));
        sideSpawns.Add(RotateVector(playerToProjDir, 60 + Main.rand.Next(-5,5)));
        sideSpawns.Add(RotateVector(playerToProjDir, -70 + Main.rand.Next(-5,5)));
        sideSpawns.Add(RotateVector(playerToProjDir, -60 + Main.rand.Next(-5,5)));
        
        frontSpawns.Add(playerToProjDir);
        frontSpawns.Add(RotateVector(playerToProjDir, 10 + Main.rand.Next(-5,5)));
        frontSpawns.Add(RotateVector(playerToProjDir, -10 + Main.rand.Next(-5,5)));

        for (int i = 0; i < 4; i++)
        {
            Vector2 shockwaveSpawn = RotateVector(playerToProjDir, Main.rand.Next(-180, 180));
            shockwaveSpawns.Add(shockwaveSpawn);
        }
        
        int projID = ModContent.ProjectileType<PhalanxPlatingProjectile>();
        
        foreach (Vector2 sideSpawn in sideSpawns)
        {
            Projectile.NewProjectile(null, proj.Center, sideSpawn * sideBurstSpeed, projID, 100, 3f);
        }
        
        foreach (Vector2 frontSpawn in frontSpawns)
        {
            Projectile.NewProjectile(null, proj.Center, frontSpawn * midBurstSpeed, projID, 100, 3f);
        }
        
        foreach (Vector2 shockwaveSpawn in shockwaveSpawns)
        {
            Projectile.NewProjectile(null, proj.Center, shockwaveSpawn * shockwaveSpeed, projID, 100, 3f);
        }

        SoundEngine.PlaySound(explosion);

        return false;
    }

    private static Vector2 RotateVector(Vector2 v, float degrees)
    {
        float radians = degrees * (float)Math.PI / 180f;
        
        float cosTheta = (float)Math.Cos(radians);
        float sinTheta = (float)Math.Sin(radians);

        float xNew = v.X * cosTheta - v.Y * sinTheta;
        float yNew = v.X * sinTheta + v.Y * cosTheta;

        return new Vector2(xNew, yNew);
    }
}
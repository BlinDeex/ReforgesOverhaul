using System;
using Microsoft.Xna.Framework;
using ModifiersOverhaul.Assets.Balance;
using ModifiersOverhaul.Assets.Buffs;
using ModifiersOverhaul.Assets.ModPrefixes.Melee;
using ModifiersOverhaul.Assets.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace ModifiersOverhaul.Assets.ModPlayers;

public class PrefixPlayer : ModPlayer
{
    public float ManaSurgeMultiplier { get; set; }


    public float UntouchableDamageIncrease { get; set; }
    
    
    /// <summary>
    /// only stored when maximum increase is reached
    /// </summary>
    private Item untouchableBuffedWeapon;
    private int untouchableBuffedWeaponOldUseTime;
    private int untouchableBuffedWeaponOldAnimTime;

    private readonly SoundStyle manaSurgeDeathSS = new($"{nameof(ModifiersOverhaul)}/Assets/Sounds/InvertedExplosion");

    public override void OnHurt(Player.HurtInfo info)
    {
        if (untouchableBuffedWeapon != null) UntouchableOnHit(info);
    }

    private void UntouchableOnHit(Player.HurtInfo info)
    {
        untouchableBuffedWeapon.useTime = untouchableBuffedWeaponOldUseTime;
        untouchableBuffedWeapon.useAnimation = untouchableBuffedWeaponOldAnimTime;
        info.Damage = (int)(info.Damage * (1 + UntouchableDamageIncrease));
        UntouchableDamageIncrease = 0;
        untouchableBuffedWeapon = null;
    }

    public override void PostUpdateEquips()
    {
        ChaoticRollPool.Tick(Player);

        UntouchableTick();
    }

    private void UntouchableTick()
    {
        if (Player.HeldItem.prefix == ModContent.PrefixType<PrefixUntouchable>())
            UntouchableDamageIncrease += PrefixBalance.RAZORS_EDGE_INCREASE_PER_TICK;
        else
            UntouchableDamageIncrease = 0;
        
        UntouchableDamageIncrease =
            MathHelper.Clamp(UntouchableDamageIncrease, 0f, PrefixBalance.RAZORS_EDGE_MAX_INCREASE);

        bool maxIncrease = Math.Abs(UntouchableDamageIncrease - PrefixBalance.RAZORS_EDGE_MAX_INCREASE) < 0.01f;
        
        if (untouchableBuffedWeapon != null) UntouchableEffects();
        
        if (maxIncrease && untouchableBuffedWeapon == null)
        {
            var heldItem = Player.HeldItem;
            untouchableBuffedWeaponOldUseTime = heldItem.useTime;
            untouchableBuffedWeaponOldAnimTime = heldItem.useAnimation;
            untouchableBuffedWeapon = heldItem;
            var newUseTime = (int)(Player.HeldItem.useTime * PrefixBalance.RAZORS_EDGE_SWING_DECREASE);
            if (newUseTime == 0) newUseTime = 1;
            heldItem.useTime = newUseTime;
            heldItem.useAnimation = newUseTime;
            return;
        }
        
        if (!maxIncrease && untouchableBuffedWeapon != null)
        {
            untouchableBuffedWeapon.useTime = untouchableBuffedWeaponOldUseTime;
            untouchableBuffedWeapon.useAnimation = untouchableBuffedWeaponOldAnimTime;
            untouchableBuffedWeapon = null;
        }
    }

    private void UntouchableEffects()
    {
        Player.yoraiz0rEye = 7;
    }

    public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust,
        ref PlayerDeathReason damageSource)
    {
        if (Player.HasBuff<SacrificialBuff>())
        {
            Player.immuneTime = Player.numMinions * PrefixBalance.SACRIFICIAL_IMMUNE_FRAMES_PER_MINION;
            Player.numMinions = 0;
            Player.ClearBuff(ModContent.BuffType<SacrificialBuff>());
            Player.GetModPlayer<WhipPlayer>().SacrificialCooldown = PrefixBalance.SACRIFICIAL_COOLDOWN_TICKS;
            return false;
        }

        if (damageSource == MiscStuff.MANA_SURGE_DEATH)
            return ManaSurgeDeath(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
        return true;
    }

    private bool ManaSurgeDeath(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust,
        ref PlayerDeathReason damageSource)
    {
        genDust = false;
        playSound = false;
        var damageEffect = Main.rand.Next(2000, 6000);
        HurtEffect(damageEffect, Main.LocalPlayer);
        SoundEngine.PlaySound(manaSurgeDeathSS);

        var playerCenter = Main.LocalPlayer.Center;

        var nodes = 10;

        for (var i = 0; i < nodes; i++)
        {
            var nodePos = UtilMethods.RandomPointInCircle(playerCenter.X, playerCenter.Y, 4f, Main.rand);
            var projectiles = Main.rand.Next(10, 20);
            var variation = Main.rand.NextFloat(-0.2f, 0.2f);
            var velMult = 10f + variation;

            for (var j = 0; j < projectiles; j++)
            {
                var projPos = UtilMethods.RandomPointInCircle(nodePos.X, nodePos.Y, 1f, Main.rand);
                var dir = (Main.LocalPlayer.Hitbox.Center() - projPos).SafeNormalize(Vector2.Zero);
                var velocity = dir * velMult;

                Projectile.NewProjectile(new EntitySource_Death(Main.LocalPlayer, "InvertedPrefix_Explosion"), projPos,
                    velocity,
                    ModContent.ProjectileType<InvertedProjectile>(), 20, 0, Main.LocalPlayer.whoAmI);
            }
        }

        return true;
    }

    public void HurtEffect(int damageAmount, Player player, bool broadcast = true)
    {
        CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height),
            CombatText.DamagedHostileCrit, damageAmount);
        if (broadcast && Main.netMode == NetmodeID.MultiplayerClient && Main.LocalPlayer.whoAmI == Main.myPlayer)
            NetMessage.SendData(MessageID.HurtPlayer, -1, -1, null, Main.LocalPlayer.whoAmI, damageAmount);
    }
}
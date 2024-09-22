using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using ModifiersOverhaul.Assets.CharmsModule;

namespace ModifiersOverhaul.Assets.ModPlayers;

public class GeneralStatPlayer : ModPlayer
{
    public Dictionary<CharmStat, (Func<float> Getter, Action<float> Setter)> StatAccessors { get; private set; }

    public GeneralStatPlayer()
    {
        StatAccessors = new Dictionary<CharmStat, (Func<float> Getter, Action<float> Setter)>()
        {
            { CharmStat.LifeSteal, (() => Lifesteal, value => Lifesteal = value) },
            { CharmStat.HealingMul, (() => HealingMul, value => HealingMul = value) },
            { CharmStat.CritDamage, (() => CritDamageMul, value => CritDamageMul = value) },
            { CharmStat.ManaUsage, (() => ManaUsageMul, value => ManaUsageMul = value) },
            { CharmStat.UseSpeed, (() => UseTimeMul, value => UseTimeMul = value) },
            { CharmStat.MoveSpeed, (() => MovementSpeedMul, value => MovementSpeedMul = value) },
            { CharmStat.WingTime, (() => WingTime, value => WingTime = (int)value) },
            { CharmStat.MaxHealthMul, (() => MaxHealthMul, value => MaxHealthMul = value) },
            { CharmStat.Crit, (() => CritMul, value => CritMul = value) },
            { CharmStat.Damage, (() => DamageMul, value => DamageMul = value) },
            { CharmStat.PickSpeed, (() => PickSpeedMul, value => PickSpeedMul = value) },
            { CharmStat.MeleeDamage, (() => MeleeDamageMul, value => MeleeDamageMul = value) },
            { CharmStat.RangedDamage, (() => RangedDamageMul, value => RangedDamageMul = value) },
            { CharmStat.MagicDamage, (() => MagicDamageMul, value => MagicDamageMul = value) },
            { CharmStat.SummonDamage, (() => SummonDamageMul, value => SummonDamageMul = value) },
        };
    }
    
    private float additionalMinions;
    public float AdditionalMinions // lmao
    {
        get => additionalMinions;
        set
        {
            if ((int)additionalMinions != (int)(additionalMinions + value))
            {
                var newMinions = (int)(additionalMinions + value) - (int)additionalMinions;
                Player.maxMinions += newMinions;
            }

            additionalMinions = value;
        }
    }
    /// <summary>
    /// 0.4 lifesteal would have 40% chance to steal 1hp, 2.3 would have 100% chance to steal 2hp and 30% chance for 1hp, if negative hurts player instead
    /// </summary>
    public float Lifesteal { get; set; }
    public float HealingMul { get; set; } = 1f;
    public float CritDamageMul { get; set; }
    public float ManaUsageMul { get; set; }
    public float UseTimeMul { get; set; }
    public float CoinDropValue { get; set; }
    public float CoinDropValueMul { get; set; }
    public int Regen { get; set; }
    public float MovementSpeedMul { get; set; } = 1f;
    public int WingTime { get; set; }
    private float maxHealthMul = 1f;
    public float MaxHealthMul
    {
        get => maxHealthMul;
        set => maxHealthMul = value > 0 ? value : 0.01f;
    }

    public float CritMul { get; set; } = 1f;
    public float Crit { get; set; }
    public float DamageMul { get; set; } = 1f;
    public float MeleeDamageMul { get; set; }
    public float RangedDamageMul { get; set; } = 1f;
    public float MagicDamageMul { get; set; } = 1f;
    public float SummonDamageMul { get; set; } = 1f;


    public float PickSpeedMul { get; set; } = 1f;
    private float defenseMul;

    private int invincibilityTicks;

    public bool IsInvincible() => invincibilityTicks > 0;
    public void SetInvincibilityTicks(int ticks) => invincibilityTicks = ticks;

    public float DefenseMul
    {
        get => defenseMul;
        set => defenseMul = value > 0 ? value : 0.01f;
    }

    public void AddHealingMul(float value)
    {
        HealingMul += value - 1;
    }
    public float MaxHealthDMG { get; set; } //TODO: implement

    public override void ResetEffects()
    {
        Player.maxMinions -= (int)AdditionalMinions;
        AdditionalMinions = 0;

        PickSpeedMul = 1f;
        ManaUsageMul = 1f;
        UseTimeMul = 1f;
        CoinDropValue = 0f;
        CoinDropValueMul = 1f;
        Lifesteal = 0f;
        HealingMul = 1f;
        Regen = 0;
        MovementSpeedMul = 1f;
        WingTime = 0;
        MaxHealthMul = 1f;
        Crit = 0f;
        DamageMul = 1f;
        DefenseMul = 1f;
        MaxHealthDMG = 0f;
        CritDamageMul = 1f;
        CritMul = 1f;
        MeleeDamageMul = 1f;
        RangedDamageMul = 1f;
        MagicDamageMul = 1f;
        SummonDamageMul = 1f;

        invincibilityTicks--;
    }
    public override void UpdateLifeRegen()
    {
        Player.lifeRegen += Regen;
    }

    public override void OnHitAnything(float x, float y, Entity victim)
    {
        if (Lifesteal != 0) RunLifesteal(victim, new Vector2(x, y));
        if (CoinDropValue != 0) DropCoins(victim, Player);
    }

    private void DropCoins(Entity victim, Entity player)
    {
        CombatUtils.DropCoins(CoinDropValue, CoinDropValue > 0 ? victim : player);
    }

    public override bool CanBeHitByProjectile(Projectile proj)
    {
        return !IsInvincible();
    }

    public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
    {
        return !IsInvincible();
    }

    private void RunLifesteal(Entity victim, Vector2 hitPos)
    {
        float currentLifeSteal = Lifesteal * HealingMul;
        var isNegative = currentLifeSteal < 0;
        var lifestealAbs = Math.Abs(currentLifeSteal);
        var fullLifesteals = (int)lifestealAbs;

        var leftOver = lifestealAbs - fullLifesteals;

        var dice = Main.rand.NextFloat();

        if (dice <= leftOver) fullLifesteals++;

        if (fullLifesteals == 0) return;

        if (isNegative)
        {
            Player.Hurt(MiscStuff.CHAOTIC_WEAPON_DEATH, fullLifesteals, 0, armorPenetration: float.MaxValue,
                dodgeable: false, cooldownCounter: ImmunityCooldownID.General);
            Player.immuneTime = 0;
        }
        else
        {
            CombatUtils.Lifesteal(victim, hitPos, fullLifesteals, Player);
        }
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.CritDamage += CritDamageMul - 1;
    }

    public override void PostUpdateEquips()
    {
        Player.wingTimeMax += WingTime;
        Player.GetDamage<GenericDamageClass>() *= DamageMul;
        Player.GetDamage<MeleeDamageClass>() *= MeleeDamageMul;
        Player.GetDamage<RangedDamageClass>() *= RangedDamageMul;
        Player.GetDamage<MagicDamageClass>() *= MagicDamageMul;
        Player.GetDamage<SummonDamageClass>() *= SummonDamageMul;
        Player.statDefense.FinalMultiplier *= defenseMul;
        Player.pickSpeed = CalculateMiningSpeed(Player.pickSpeed, PickSpeedMul);
    }
    
    private static float CalculateMiningSpeed(float baseSpeed, float speedMul)
    {
        float finalSpeed = baseSpeed / speedMul;
        if (finalSpeed <= 0) finalSpeed = 0.01f;
        return finalSpeed;
    }

    public override void PostUpdateRunSpeeds()
    {
        Player.accRunSpeed *= MovementSpeedMul;
        Player.maxRunSpeed *= MovementSpeedMul;
        Player.runAcceleration *= MovementSpeedMul;
    }

    public void ApplyCharmStats(List<CharmRoll> rolls)
    {
        foreach (var roll in rolls)
        {
            if (StatAccessors.TryGetValue(roll.Stat, out var accessor))
            {
                if(roll.Stat == CharmStat.MoveSpeed) Main.NewText(roll.RawStrength);
                accessor.Setter(accessor.Getter.Invoke() + roll.RawStrength);
            }
        }
    }

    public override void ModifyWeaponCrit(Item item, ref float crit)
    {
        crit += Crit;
        crit *= CritMul;
    }

    public override void ModifyManaCost(Item item, ref float reduce, ref float mult)
    {
        mult *= Math.Clamp(ManaUsageMul, 0, 10);
    }

    public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
    {
        healValue = (int)(healValue * HealingMul);
    }

    public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
    {
        if (item != Main.LocalPlayer.HeldItem) return;
        //damage *= DamageMul;
    }

    public override void ModifyMaxStats(out StatModifier health, out StatModifier mana)
    {
        base.ModifyMaxStats(out health, out mana);
        health.Base += Player.statLifeMax * (MaxHealthMul - 1);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace ModifiersOverhaul.Assets.Balance;

public static class PrefixBalance
{
    public static bool DEV_MODE = true;

    // 29 shrapnel magic

    public static float ACCESSORY_REFORGING_MULTIPLIER = 2.5f;
    public static float WEAPON_REFORGING_MULTIPLIER = 2f;
    public static float TOOL_REFORGING_MULTIPLIER = 1.5f;
    public static float ARMOR_REFORGING_MULTIPLIER = 3f;

    #region Misc

    public static readonly Color ASCENDANT_MIN_COLOR = new(255, 90, 90);
    public static readonly Color ASCENDANT_MAX_COLOR = new(90, 90, 255);

    #endregion


    #region MeleePrefixes

    // Gigantic
    public static float GIGANTIC_SIZE = 2f;
    public static float GIGANTIC_USE = 1.15f;

    // Ultra Light
    public static float ULTRA_LIGHT_USE = 0.33f;
    public static float ULTRA_LIGHT_DAMAGE = 0.7f;
    public static float ULTRA_LIGHT_SIZE = 0.9f;

    // Arcane Infused
    public static float ARCANE_INFUSED_DAMAGE = 1.7f;
    public static float ARCANE_INFUSED_MANA_PER_SWING = 10f;

    // Titan Force
    public static float TITAN_FORCE_KB = 3f;
    public static float TITAN_FORCE_DAMAGE = 1.1f;

    // Perceptive
    public static int PERCEPTIVE_CRIT = 120;
    public static float PERCEPTIVE_CRIT_DAMAGE = 0.9f;

    // Untouchable
    public static float RAZORS_EDGE_INCREASE_PER_TICK = 1f / (60f * 20f); // 20 seconds 
    public static float RAZORS_EDGE_MAX_INCREASE = 2f; // 200% increase
    public static float RAZORS_EDGE_SWING_DECREASE = 0.8f;

    #endregion

    #region RangedPrefixes

    // Adaptable
    public static float ADAPTABLE_FIRERATE = 0.75f; // 25% increase

    // Controlled
    public static float CONTROLLED_BULLET_VELOCITY = 1.5f;
    public static int CONTROLLED_BURST_DURATION_TICKS = 10;
    public static int CONTROLLED_BURST_COOLDOWN_TICKS = 30;
    public static float CONTROLLED_FIRERATE = 0.2f;

    // Vampiric
    public static float VAMPIRIC_FIRERATE = 0.8f;
    public static float VAMPIRIC_LIFESTEAL = 0.5f;

    // Ascendant
    public static float ASCENDANT_RANGED_MAX_DAMAGE = 1.5f; // 150% increase
    public static float ASCENDANT_RANGED_MAX_CRIT = 30f;

    // Giant Slayer
    public static float GIANT_SLAYER_PERCENT_DAMAGE = 0.2f / 100f; // 0.2%

    // Challenger
    public static float CHALLENGER_GOOD_ORB_HIT_MULTIPLIER = 1.1f;
    public static float CHALLENGER_GOOD_ORB_PERFECT_HIT_MULTIPLIER = 1.25f;
    public static float CHALLENGER_ORB_MISS_MULTIPLIER = 0.8f;

    public static float CHALLENGER_ACTIVATION_SCORE_THRESHOLD = 2f;
    public static float CHALLENGER_DEACTIVATION_SCORE_THRESHOLD = 1.5f;

    public static float CHALLENGER_GREEN_ORB_DEFENSE = 0.02f; // 2% additional multiplier to defense
    public static float CHALLENGER_GREEN_ORB_DAMAGE = 0.02f; // 2% additional multiplier to damage
    public static float CHALLENGER_BLUE_ORB_CRIT = 0.05f; // 5% additional multiplier to crit chance
    public static float CHALLENGER_BLUE_ORB_VELOCITY = 0.1f; // 10% additional multiplier to projectile velocity
    public static int CHALLENGER_YELLOW_ORB_HEAL = 10;
    public static int CHALLENGER_RED_ORB_DAMAGE = 25;

    private static readonly (float, int)[] ScoreToSpawnRate =
    [
        (1f, 240),
        (2f, 200),
        (2.5f, 180),
        (3f, 160),
        (3.5f, 145),
        (4f, 120),
        (4.5f, 105),
        (5f, 95),
        (5.5f, 85),
        (6f, 80),
        (6.5f, 75),
        (7f, 70),
        (7.5f, 60),
        (8f, 45),
        (9f, 30),
        (9.5f, 30),
        (float.MaxValue, 45)
    ];

    public static int GetChallengerSpawnRate(float score)
    {
        return ScoreToSpawnRate.First(x => x.Item1 >= score).Item2;
    }


    /// <summary>
    /// controls how fast frames change and with it lifetime of the orb, orb dies when it reaches last animation frame
    /// </summary>
    public static float CHALLENGER_FRAME_RATE_MULTIPLIER = 0.4f;

    public static float CHALLENGER_BASELINE_ORB_SPEED = 1f;

    public static float CHALLENGER_ORB_SPEED_SCORE_PERCENTAGE_MULTIPLIER = 0.35f; // 35% of score will be added to the speed
    
    // Tracer
    public static float TRACER_PROJ_VEL_MUL = 0.7f;
    /// <summary>
    /// How many ticks to way to add another point to tracer bullets path
    /// </summary>
    public static int TRACER_PATH_RESOLUTION = 4;

    public static int TRACER_DUST_POSITIONS_BETWEEN_POINTS = 16;

    public static int TRACER_MAXIMUM_LIFETIME = 300;

    #endregion


    #region MagicPrefixes

    // Endless
    public static float ENDLESS_MANA_USAGE = 0f;
    public static float ENDLESS_DAMAGE = 0.67f;

    // ManaCharged
    public static float MANA_CHARGED_MAX_DAMAGE_GAIN = 0.7f; // 70% increase

    // Inverted
    public static int INVERTED_EXPLOSION_DUST_ID = 206;
    public static float INVERTED_MAX_DAMAGE_INCREASE = 0.5f; // 50% increase
    public static float INVERTED_MANA_SURGE_CHANCE_TO_DAMAGE = 0.02f; // 2%
    public static int INVERTED_MANA_SURGE_DAMAGE = 10;
    public static int INVERTED_MANA_SURGE_DUST_RATE = 2; // 10 per tick
    public static int INVERTED_MANA_SURGE_DUST_ID = 86; //223
    public static float INVERTED_MANA_SURGE_MAX_SCALE = 1f;

    // Splintering
    public static int SPLINTERING_DUST_ID = 206; //173 //175 //206 //306
    public static int SPLINTERING_MIN_SHARD_NODES = 1;
    public static int SPLINTERING_MAX_SHARD_NODES = 4;
    public static int SPLINTERING_SHARD_NODES_CRIT_MULT = 2;
    public static int SPLINTERING_MIN_NODE_PROJ = 1;
    public static int SPLINTERING_MAX_NODE_PROJ = 3;
    public static float SPLINTERING_PROJ_VELOCITY = 10f;
    public static float SPLINTERING_PROJ_VELOCITY_VARIATION_MUL = 4f;
    public static float SPLINTERING_CHANCE = 0.2f;

    // Chaotic
    public static int CHAOTIC_ROLL_LENGTH = 30;

    public static float CHAOTIC_COMMON_ROLL_CHANCE = 10f;
    public static float CHAOTIC_RARE_ROLL_CHANCE = 5f;
    public static float CHAOTIC_EPIC_ROLL_CHANCE = 3f;
    public static float CHAOTIC_LEGENDARY_ROLL_CHANCE = 1.5f;

    public static float CHAOTIC_NEGATIVE_ROLL_CHANCE = 4f;

    public static float CHAOTIC_DEBUG_GUARANTEED_ROLL_CHANCE = 9999999f;

    public static readonly Color CHAOTIC_COMMON_ROLL_COLOR = Color.LightGray;
    public static readonly Color CHAOTIC_RARE_ROLL_COLOR = Color.LightBlue;
    public static readonly Color CHAOTIC_EPIC_ROLL_COLOR = Color.Purple;
    public static readonly Color CHAOTIC_LEGENDARY_ROLL_COLOR = Color.YellowGreen;

    public static readonly Color CHAOTIC_DEBUG_ROLL_COLOR = Color.DarkGray;

    public static readonly Color CHAOTIC_NEGATIVE_ROLL_COLOR = Color.Red;

    // Triple Shot
    public static float TRIPLE_SHOT_DEGREES = 10f;
    public static float TRIPLE_SHOT_DEGREES_VARIATION = 5f;
    public static float TRIPLE_SHOT_USE_TIME_MUL = 0.7f;
    public static float TRIPLE_SHOT_DAMAGE_MUL = 0.5f;
    public static float TRIPLE_SHOT_MANA_MUL = 3f;

    #endregion

    #region SummonerPrefixes

    #region MinionWeapons

    // Frenzied
    public static int FRENZIED_ADDITIONAL_UPDATES = 4;

    #endregion


    #region Whips

    // Sacrificial
    public static int SACRIFICIAL_ON_HIT_BUFF_TICKS = 300;
    public static int SACRIFICIAL_IMMUNE_FRAMES_PER_MINION = 20;
    public static int SACRIFICIAL_COOLDOWN_TICKS = 3600;

    #endregion

    #endregion

    #region Accessories

    // Warlord
    public static float WARLORD_ADDITIONAL_MINIONS = 0.75f;

    // Fortified
    public static float FORTIFIED_INCREASED_DEFENSE = 0.07f; // 5% increase

    // Revitalizing
    public static int REVITALIZING_REGENERATION = 2; // 1hp/s regen

    // Aerodynamic
    public static float AERODYNAMIC_MOVEMENT_MULTIPLIER = 0.05f; // 5% increase
    public static int AERODYNAMIC_WING_TIME_TICKS = 30;

    // Sharpshooter
    public static int SHARPSHOOTER_CRIT = 6; // 6% increase

    // Efficient
    public static float EFFICIENT_MANA_SAVED = 0.05f; // 5% reduced mana usage

    // EarthShaper
    public static float EARTH_SHAPER_PICK_SPEED_REDUCE = 0.1f; // 10% increase

    // Equilibrium
    public static float EQUILIBRIUM_MOVEMENT_MULTIPLIER = 0.01f; // 1% increase
    public static int EQUILIBRIUM_WING_TIME_TICKS = 5;
    public static int EQUILIBRIUM_REGENERATION = 1; // 0.5hp/s regen
    public static float EQUILIBRIUM_MINIONS = 0.25f;
    public static float EQUILIBRIUM_DEFENSE = 0.02f; // 2% increase

    // Risky
    public static float RISKY_DEFENSE_DECREASE = -0.1f; // 10%
    public static float RISKY_DAMAGE_INCREASE = 0.15f; // 15%

    // Stoic
    public static float STOIC_DEFENSE_INCREASE = 0.15f; // 15%
    public static float STOIC_DAMAGE_DECREASE = 0.08f; // 8%

    public static float BLOOD_FORGED_DEFENSE = 0.30f; // 30% increase
    public static float BLOOD_FORGED_MAX_HEALTH = -0.15f; // 15% decrease

    // Lethal
    public static float LETHAL_CRIT_DMG_MUL = 0.1f; // 10% increase

    #endregion

    #region Tools

    // Clearing
    public static float CLEARING_CHANCE_TO_LOSE_MINED_BLOCK = 0.5f;

    // Vein miner
    public static int VEIN_MINER_MAXIMUM_BLOCKS_MINED = 50;
    public static float VEIN_MINER_MINING_SPEED = 0.8f;

    // Fortune
    public static float FORTUNE_CHANCE_FOR_EXTRA_DROPS = 0.7f;
    public static int FORTUNE_EXTRA_DROP_NUM = 1;

    // Revealing
    public static int REVEALING_TICKS = 20;
    public static float REVEALING_CHANCE = 0.2f;
    public static float REVEALING_BRIGHTNESS_MUL = 0.5f;

    #endregion
    
    #region Armor
    
    // Phalanx
    public static int PHALANX_MIN_DAMAGE_TO_REACT = 1;
    public static int PHALANX_BURN_EFFECT_TICKS = 180;
    public static int PHALANX_REACT_COOLDOWN_TICKS = 60 * 1;
    public static float PHALANX_DAMAGE_INCREASE = 1.04f; // 4% increase
    
    // Vitalis
    public static float VITALIS_LIFESTEAL_AMP = 1.5f; // 50% increase
    
    // Chrono
    public static float CHRONO_MOVEMENT_SPEED = 1.04f; // 4% increase
    public static int CHRONO_ABILITY_COOLDOWN = 60 * 8;
    public static float CHRONO_ABILITY_RANGE = 400000f;
    public static int CHRONO_ABILITY_LENGTH = 60 * 7;
    
    // Augmented
    public static Dictionary<int, int> AUGMENTED_HELMET_THRESHOLDS_AND_BUFFS = new()
    {
        {10, BuffID.Hunter},
        {20, BuffID.WellFed3},
        {30, BuffID.Lifeforce}
    };
    
    public static Dictionary<int, int> AUGMENTED_CHESTPLATE_THRESHOLDS_AND_BUFFS = new()
    {
        {10, BuffID.Ironskin},
        {20, BuffID.Wrath},
        {30, BuffID.Endurance}
    };
    
    public static Dictionary<int, int> AUGMENTED_LEGGINGS_THRESHOLDS_AND_BUFFS = new()
    {
        {10, BuffID.Swiftness},
        {20, BuffID.Rage},
        {30, BuffID.Gravitation}
    };

    public static float AUGMENTED_SET_BONUS_DAMAGE_PER_BUFF = 0.02f;
    
    #endregion


    private static float ASCENDANT_DAMAGE_REQUIRED_SCALAR = 1000f;
    

    public static int GetAscendantDamageRequired(Item item)
    {
        var damageRequired = item.damage * (50f / item.useTime) * ASCENDANT_DAMAGE_REQUIRED_SCALAR;
        damageRequired = RoundToNearestThousand(damageRequired);
        return (int)damageRequired;
    }

    private static float RoundToNearestThousand(float value)
    {
        if (value < 1000) return 1000;
        return (float)Math.Round(value / 1000) * 1000;
    }
}
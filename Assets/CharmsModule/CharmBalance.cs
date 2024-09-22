using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModifiersOverhaul.Assets.PrefixNPCS;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.CharmsModule;

public static class CharmBalance
{
    public static int RollTriesForBosses = 10;
    public static int MinStatRolls = 1;
    public static int MaxStatRolls = 3;

    public static List<int> ExcludedNPCSFromCharmDrops = new()
    {
        ModContent.NPCType<ChallengerOrb>()
    };
    
    public static Dictionary<float, CharmType> CharmTypeChance = new()
    {
        { 0.33f, CharmType.Circle },
        { 0.33f + 0.33f, CharmType.Square },
        { 1f, CharmType.Triangle }
    };

    public static Dictionary<float, CharmRarity> CharmRarityChance = new()
    {
        { 0.2f, CharmRarity.Common },
        { 0.1f, CharmRarity.Rare },
        { 0.04f, CharmRarity.Epic },
        { 0.02f, CharmRarity.Legendary },
        { 0.01f, CharmRarity.Mythical }
    };
    
    public static Dictionary<CharmType, Texture2D> CharmTextures = new()
    {
        { CharmType.NotInitialized , ModContent.Request<Texture2D>("ModifiersOverhaul/Assets/CharmsModule/Textures/Charm_NoTex", AssetRequestMode.ImmediateLoad).Value},
        { CharmType.Circle , ModContent.Request<Texture2D>("ModifiersOverhaul/Assets/CharmsModule/Textures/Charm_Circle", AssetRequestMode.ImmediateLoad).Value},
        { CharmType.Square , ModContent.Request<Texture2D>("ModifiersOverhaul/Assets/CharmsModule/Textures/Charm_Square", AssetRequestMode.ImmediateLoad).Value},
        { CharmType.Triangle , ModContent.Request<Texture2D>("ModifiersOverhaul/Assets/CharmsModule/Textures/Charm_Triangle", AssetRequestMode.ImmediateLoad).Value}
    };

    private static Dictionary<CharmRarity, Color> CharmColors = new()
    {
        { CharmRarity.NotInitialized , new Color(255,255,255)},
        { CharmRarity.Common, new Color(255, 255, 255) },
        { CharmRarity.Rare, new Color(100, 100, 255) },
        { CharmRarity.Epic, new Color(255, 0, 255) },
        { CharmRarity.Legendary, new Color(255, 255, 0) },
        { CharmRarity.Mythical, new Color(255, 100, 100) }
    };

    public static Color GetCharmColor(CharmRarity rarity)
    {
        if (CharmColors.TryGetValue(rarity, out var color)) return color;
        
        UtilMethods.BroadcastOrNewText($"{nameof(GetCharmColor)}: {nameof(CharmColors)} didnt contain rarity of {rarity.ToString()}!", Color.Red);
        return Color.Black;
    }

    private static Dictionary<CharmStat, (float min, float max)> StatBounds = new()
    {
        { CharmStat.NotInitialized, (0, 0) },
        { CharmStat.Damage, (0.01f, 0.02f) },
        { CharmStat.MeleeDamage, (0.01f, 0.02f) },
        { CharmStat.RangedDamage, (0.01f, 0.02f) },
        { CharmStat.MagicDamage, (0.01f, 0.02f) },
        { CharmStat.SummonDamage, (0.01f, 0.02f) },
        { CharmStat.MoveSpeed, (0.005f, 0.01f) },
        { CharmStat.WingTime, (3f, 7f) },
        { CharmStat.UseSpeed, (0.01f, 0.03f) },
        { CharmStat.LifeSteal, (0.1f, 0.2f) },
        { CharmStat.CharmLuck, (0.01f, 0.2f) },
        { CharmStat.CritDamage, (0.01f, 0.02f) },
        { CharmStat.ManaUsage, (-0.02f, -0.04f) },
        { CharmStat.HealingMul, (0.01f, 0.03f) },
        { CharmStat.Regen, (0.05f, 0.1f) },
        { CharmStat.PickSpeed, (0.02f, 0.05f) },
        { CharmStat.MaxHealthMul, (0.01f, 0.02f) },
        { CharmStat.Crit, (0.03f, 0.05f) }
    };

    private static Dictionary<CharmStat, CharmRarity> StatRarities = new()
    {
        { CharmStat.NotInitialized, CharmRarity.NotInitialized },
        { CharmStat.Damage, CharmRarity.Rare },
        { CharmStat.MeleeDamage, CharmRarity.Common },
        { CharmStat.RangedDamage, CharmRarity.Common },
        { CharmStat.MagicDamage, CharmRarity.Common },
        { CharmStat.SummonDamage, CharmRarity.Common },
        { CharmStat.MoveSpeed, CharmRarity.Common },
        { CharmStat.WingTime, CharmRarity.Rare },
        { CharmStat.UseSpeed, CharmRarity.Rare },
        { CharmStat.LifeSteal, CharmRarity.Epic },
        { CharmStat.CharmLuck, CharmRarity.Common },
        { CharmStat.CritDamage, CharmRarity.Epic },
        { CharmStat.ManaUsage, CharmRarity.Rare },
        { CharmStat.HealingMul, CharmRarity.Epic },
        { CharmStat.Regen, CharmRarity.Rare },
        { CharmStat.PickSpeed, CharmRarity.Common },
        { CharmStat.MaxHealthMul, CharmRarity.Epic },
        { CharmStat.Crit, CharmRarity.Common }
    };

    private static readonly Array charmStatValues = Enum.GetValues(typeof(CharmStat));
    
    public static CharmStat GetRandomStat()
    {
        while (true)
        {
            CharmStat stat = (CharmStat)charmStatValues.GetValue(Main.rand.Next(charmStatValues.Length))!;
            if (stat != CharmStat.NotInitialized) return stat;
        }
    }

    public static bool MultiplyStatForDisplay(CharmStat stat)
    {
        bool multiply = stat is not (CharmStat.LifeSteal or CharmStat.WingTime);
        return multiply;
    }

    public static readonly Dictionary<CharmRarity, float> StatRarityMultipliers = new()
    {
        { CharmRarity.Common, 1f },
        { CharmRarity.Rare, 2f },
        { CharmRarity.Epic, 4f },
        { CharmRarity.Legendary, 8f },
        { CharmRarity.Mythical, 16f },
    };

    public static float GetRarityMultiplier(CharmRarity rarity)
    {
        if (!StatRarityMultipliers.TryGetValue(rarity, out var statRarity))
        {
            throw new ArgumentException($"Stat multiplier not defined for {rarity}");
        }

        return statRarity;
    }

    public static bool IsCharmRareEnoughForStat(CharmStat stat, CharmRarity rarity)
    {
        if (!StatRarities.TryGetValue(stat, out var statRarity))
        {
            throw new ArgumentException($"Stat rarity not defined for {stat}");
        }
        
        return rarity >= statRarity;
    }
    
    public static string SplitCamelCase(CharmName charmRarity)
    {
        string input = charmRarity.ToString();

        if (string.IsNullOrEmpty(input))
        {
            return input;
        }
        
        var result = new System.Text.StringBuilder();

        foreach (char c in input)
        {
            if (char.IsUpper(c) && result.Length > 0)
            {
                result.Append(' ');
            }
            result.Append(c);
        }

        return result.ToString();
    }

    public static CharmRarity GetStatRarity(CharmStat stat)
    {
        if (!StatRarities.TryGetValue(stat, out var statRarity))
        {
            throw new ArgumentException($"Stat rarity not defined for {stat}");
        }

        return statRarity;
    }
    
    /// <summary>
    /// Rarity accounted for
    /// </summary>
    public static (float min, float max) GetStatBounds(CharmStat stat, CharmRarity rarity)
    {
        if (!StatBounds.TryGetValue(stat, out var bounds))
        {
            throw new ArgumentException($"Stat bounds not defined for {stat}");
        }

        if (!StatRarityMultipliers.TryGetValue(rarity, out var mul))
        {
            throw new ArgumentException($"Rarity multiplier is not defined for {rarity}");
        }
        
        return (bounds.min * mul, bounds.max * mul);
    }
}
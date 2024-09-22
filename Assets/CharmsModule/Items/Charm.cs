using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModifiersOverhaul.Assets.Balance;
using ModifiersOverhaul.Assets.CharmsModule.Manager;
using ModifiersOverhaul.Assets.Misc;
using ReLogic.Content;
using Terraria;
using Terraria.Chat;
using Terraria.Graphics.Light;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ModifiersOverhaul.Assets.CharmsModule.Items;

public class Charm : ModItem
{
    public CharmType CharmType { get; set; } = CharmType.NotInitialized;
    public CharmRarity CharmRarity { get; set; } = CharmRarity.NotInitialized;

    public int CharmNameID { get; set; }
    
    public string CharmName => CharmBalance.SplitCamelCase((CharmName)CharmNameID);

    public Color CharmColor => CharmBalance.GetCharmColor(CharmRarity);

    public static readonly Dictionary<CharmStat, LocalizedText> StatTexts = new();

    public List<CharmRoll> Stats { get; set; } = [];

    private static Texture2D circleTex = CharmBalance.CharmTextures[CharmType.Circle];
    private static Texture2D squareTex = CharmBalance.CharmTextures[CharmType.Square];
    private static Texture2D triangleTex = CharmBalance.CharmTextures[CharmType.Triangle];
    private static Texture2D noTex = CharmBalance.CharmTextures[CharmType.NotInitialized];

    public override string Texture => "ModifiersOverhaul/Assets/CharmsModule/Textures/Charm_NoTex";

    public override void SetDefaults()
    {
        Item.width = 16;
        Item.height = 16;
        Item.maxStack = 1;
        Item.value = 0;
    }

    public override void SetStaticDefaults()
    {
        StatTexts.Add(CharmStat.NotInitialized, LocalizationManager.GetCharmText(CharmStat.NotInitialized));
        StatTexts.Add(CharmStat.Damage, LocalizationManager.GetCharmText(CharmStat.Damage));
        StatTexts.Add(CharmStat.MeleeDamage, LocalizationManager.GetCharmText(CharmStat.MeleeDamage));
        StatTexts.Add(CharmStat.RangedDamage, LocalizationManager.GetCharmText(CharmStat.RangedDamage));
        StatTexts.Add(CharmStat.MagicDamage, LocalizationManager.GetCharmText(CharmStat.MagicDamage));
        StatTexts.Add(CharmStat.SummonDamage, LocalizationManager.GetCharmText(CharmStat.SummonDamage));
        StatTexts.Add(CharmStat.MoveSpeed, LocalizationManager.GetCharmText(CharmStat.MoveSpeed));
        StatTexts.Add(CharmStat.WingTime, LocalizationManager.GetCharmText(CharmStat.WingTime));
        StatTexts.Add(CharmStat.UseSpeed, LocalizationManager.GetCharmText(CharmStat.UseSpeed));
        StatTexts.Add(CharmStat.LifeSteal, LocalizationManager.GetCharmText(CharmStat.LifeSteal));
        StatTexts.Add(CharmStat.CharmLuck, LocalizationManager.GetCharmText(CharmStat.CharmLuck));
        StatTexts.Add(CharmStat.CritDamage, LocalizationManager.GetCharmText(CharmStat.CritDamage));
        StatTexts.Add(CharmStat.ManaUsage, LocalizationManager.GetCharmText(CharmStat.ManaUsage));
        StatTexts.Add(CharmStat.HealingMul, LocalizationManager.GetCharmText(CharmStat.HealingMul));
        StatTexts.Add(CharmStat.Regen, LocalizationManager.GetCharmText(CharmStat.Regen));
        StatTexts.Add(CharmStat.PickSpeed, LocalizationManager.GetCharmText(CharmStat.PickSpeed));
        StatTexts.Add(CharmStat.MaxHealthMul, LocalizationManager.GetCharmText(CharmStat.MaxHealthMul));
        StatTexts.Add(CharmStat.Crit, LocalizationManager.GetCharmText(CharmStat.Crit));
    }

    public override void SaveData(TagCompound tag)
    {
        tag.Add(nameof(CharmType),(int)CharmType);
        tag.Add(nameof(CharmRarity),(int)CharmRarity);
        SaveStats(tag);
    }

    private const string statTypesTag = "statTypes";
    private const string strengthsTag = "strengths";

    private void SaveStats(TagCompound tag)
    {
        if (Stats.Count == 0) return;
        
        List<int> statTypes = Stats.Select(x => (int)x.Stat).ToList();
        List<float> strengths = Stats.Select(x => x.RawStrength).ToList();
        
        tag.Add(statTypesTag, statTypes);
        tag.Add(strengthsTag, strengths);
        
        if(CharmNameID != 0) tag.Add(nameof(CharmNameID), CharmNameID);
    }

    public override void LoadData(TagCompound tag)
    {
        try
        {
            CharmType = tag.ContainsKey(nameof(CharmType)) ? (CharmType)tag.GetInt(nameof(CharmType)) : CharmType.NotInitialized;
            CharmRarity = tag.ContainsKey(nameof(CharmRarity)) ? (CharmRarity)tag.GetInt(nameof(CharmRarity)) : CharmRarity.NotInitialized;
            CharmNameID = tag.ContainsKey(nameof(CharmNameID)) ? tag.GetInt(nameof(CharmNameID)) : 0;
            LoadStats(tag);
        }
        catch (Exception e)
        {
            Mod.Logger.Error(e);
        }
    }
    
    private void LoadStats(TagCompound tag)
    {
        if (!tag.ContainsKey(statTypesTag) || !tag.ContainsKey(strengthsTag)) return;
        
        List<CharmStat> statTypes = tag.GetList<int>(statTypesTag).Select(x => (CharmStat)x).ToList();
        List<float> strengths = tag.Get<List<float>>(strengthsTag);

        if (statTypes.Count != strengths.Count)
        {
            throw new InvalidOperationException($"{nameof(LoadStats)}: Stat types and strengths count mismatch.");
        }

        for (int i = 0; i < statTypes.Count; i++)
        {
            CharmRoll roll = new CharmRoll(statTypes[i], strengths[i]);
            Stats.Add(roll);
        }
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale,
        int whoAmI)
    {
        Texture2D targetTex = CharmType switch
        {
            CharmType.Circle => circleTex,
            CharmType.Square => squareTex,
            CharmType.Triangle => triangleTex,
            _ => noTex
        };
        Color targetColor;

        Color rarityColor = CharmBalance.GetCharmColor(CharmRarity);
        
        targetColor = alphaColor.MultiplyRGB(rarityColor);
        Lighting.AddLight(Item.Center, CharmsManager.GetRarityWorldColor(CharmRarity));
        
        targetColor = targetColor.MultiplyRGB(lightColor);
        spriteBatch.Draw(targetTex, Item.position - Main.screenPosition, null, targetColor, rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
        
        return false;
    }

    

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips[0].Text = CharmName;
        tooltips[0].OverrideColor = CharmBalance.GetCharmColor(CharmRarity);

        TooltipLine rarity = new TooltipLine(Mod, "charmRarity", CharmRarity.ToString())
        {
            OverrideColor = CharmColor
        };

        tooltips.Add(rarity);
        
        TooltipLine type = new TooltipLine(Mod, "charmType", CharmType.ToString());
        
        tooltips.Add(type);

        if (Stats.Count == 0) return;

        foreach (CharmRoll roll in Stats)
        {
            float str = roll.GetStrength(CharmBalance.MultiplyStatForDisplay(roll.Stat));
            
            string text = StatTexts[roll.Stat].Format(str);
            TooltipLine statLine = new TooltipLine(Mod, "statLine", text)
            {
                OverrideColor = CharmBalance.GetCharmColor(CharmRarity)
            };
            tooltips.Add(statLine);
        }
    }

    public override void NetSend(BinaryWriter writer)
    {
        writer.Write((sbyte)CharmRarity);
        writer.Write((byte)CharmType);
        writer.Write((byte)CharmNameID);
        
        writer.Write((byte)Stats.Count);
        
        foreach (CharmRoll roll in Stats)
        {
            writer.Write((byte)roll.Stat);
            writer.Write(roll.RawStrength);
        }
    }

    public override void NetReceive(BinaryReader reader)
    {
        CharmRarity = (CharmRarity)reader.ReadSByte();
        CharmType = (CharmType)reader.ReadByte();
        CharmNameID = reader.ReadByte();
        
        int statsCount = reader.ReadByte();
        
        Stats.Clear(); // idk, just in case?
        
        for (int i = 0; i < statsCount; i++)
        {
            CharmStat stat = (CharmStat)reader.ReadByte();
            float strength = reader.ReadSingle();
            
            Stats.Add(new CharmRoll(stat, strength));
        }
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor,
        Vector2 origin, float scale)
    {
        Texture2D targetTex = CharmType switch
        {
            CharmType.Circle => circleTex,
            CharmType.Square => squareTex,
            CharmType.Triangle => triangleTex,
            _ => noTex
        };
        
        Color targetColor = drawColor.MultiplyRGB(CharmBalance.GetCharmColor(CharmRarity));
        
        spriteBatch.Draw(targetTex, position, frame, targetColor, 0, origin, scale, SpriteEffects.None, 0f);
        
        return false;
    }
}
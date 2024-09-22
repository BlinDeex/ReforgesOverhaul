using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ModifiersOverhaul.Assets.CharmsModule.Items;
using ModifiersOverhaul.Assets.CharmsModule.Manager;
using ModifiersOverhaul.Assets.ModPlayers;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ModifiersOverhaul.Assets.CharmsModule;

public class CharmGlobalItem : GlobalItem
{
    public override bool InstancePerEntity => true;

    private List<CharmRoll> appliedStats = [];
    private CharmRarity appliedCharmRarity = CharmRarity.NotInitialized;
    private const string statTypesTag = "statTypesItem";
    private const string strengthsTag = "strengthsItem";

    private bool hasCharm => appliedStats.Count > 0;

    private void ApplyCharm(Charm charm, string targetItemName)
    {
        appliedStats = charm.Stats;
        appliedCharmRarity = charm.CharmRarity;
        Main.mouseItem.TurnToAir();
        Color textColor = CharmBalance.GetCharmColor(charm.CharmRarity);
        
        Main.NewText($"Applied {charm.CharmRarity} charm to {targetItemName}!", textColor);
    }
    
    public override void RightClick(Item item, Player player)
    {
        if (!CanApplyCharm(Main.LocalPlayer, item)) return;
        item.stack++;
        
        ApplyCharm((Charm)player.inventory[58].ModItem, item.Name);
    }

    private bool CanApplyCharm(Player player, Item targetItem)
    {
        Item mouseItem = player.inventory[58];
        bool isCharm = mouseItem.type == ModContent.ItemType<Charm>();
        
        if (!isCharm) return false;
        Charm charmItem = (Charm)mouseItem.ModItem;
        
        bool weapon = targetItem.IsWeapon();
        bool armor = targetItem.IsArmor();
        bool accessory = targetItem.accessory && !targetItem.IsArmor();
        
        CharmType type = charmItem.CharmType;

        return type switch
        {
            CharmType.NotInitialized => false,
            CharmType.Circle => weapon,
            CharmType.Square => armor,
            CharmType.Triangle => accessory,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public override bool CanRightClick(Item item)
    {
        return CanApplyCharm(Main.LocalPlayer, item);
    }

    public override void UpdateEquip(Item item, Player player)
    {
        if (appliedStats.Count == 0) return;
        
        player.GetModPlayer<GeneralStatPlayer>().ApplyCharmStats(appliedStats);
    }

    public override void HoldItem(Item item, Player player)
    {
        if (!item.IsWeapon()) return;
        CharmGlobalItem charmItem = item.GetGlobalItem<CharmGlobalItem>();
        if (!charmItem.hasCharm) return;
        
        player.GetModPlayer<GeneralStatPlayer>().ApplyCharmStats(appliedStats);
    }

    public override void SaveData(Item item, TagCompound tag)
    {
        SaveStats(tag);
    }

    public override void LoadData(Item item, TagCompound tag)
    {
        try
        {
            LoadStats(tag);
        }
        catch (Exception e)
        {
            Mod.Logger.Error(e.Message);
        }
    }

    private void SaveStats(TagCompound tag)
    {
        if (appliedStats.Count == 0) return;
        
        tag.Add(nameof(appliedCharmRarity),(int)appliedCharmRarity);
        
        List<int> statTypes = appliedStats.Select(x => (int)x.Stat).ToList();
        List<float> strengths = appliedStats.Select(x => x.RawStrength).ToList();
        
        tag.Add(statTypesTag, statTypes);
        tag.Add(strengthsTag, strengths);
    }
    
    private void LoadStats(TagCompound tag)
    {
        if (!tag.ContainsKey(statTypesTag) || !tag.ContainsKey(strengthsTag)) return;

        appliedCharmRarity = tag.ContainsKey(nameof(appliedCharmRarity)) ? (CharmRarity)tag.GetInt(nameof(appliedCharmRarity)) : CharmRarity.NotInitialized;
        
        List<CharmStat> statTypes = tag.GetList<int>(statTypesTag).Select(x => (CharmStat)x).ToList();
        List<float> strengths = tag.Get<List<float>>(strengthsTag);

        if (statTypes.Count != strengths.Count)
        {
            throw new InvalidOperationException($"{nameof(LoadStats)}: Stat types and strengths count mismatch.");
        }

        for (int i = 0; i < statTypes.Count; i++)
        {
            CharmRoll roll = new CharmRoll(statTypes[i], strengths[i]);
            appliedStats.Add(roll);
        }
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (appliedStats.Count == 0) return;
        
        bool weapon = item.IsWeapon();
        
        if (weapon)
        {
            bool beingApplied = item == Main.LocalPlayer.HeldItem;
            string text = beingApplied ? "Active" : "You must hold this weapon to gain charm stats";
            
            tooltips.Add(new TooltipLine(Mod, "CharmWeaponTooltipLine", text)
            {
                IsModifier = true,
                IsModifierBad = !beingApplied
            });
        }

        foreach (CharmRoll roll in appliedStats)
        {
            float str = roll.GetStrength(CharmBalance.MultiplyStatForDisplay(roll.Stat));
            
            string text = Charm.StatTexts[roll.Stat].Format(str);
            TooltipLine statLine = new TooltipLine(Mod, "statLine", text)
            {
                OverrideColor = CharmBalance.GetCharmColor(appliedCharmRarity)
            };
            tooltips.Add(statLine);
        }
    }
}
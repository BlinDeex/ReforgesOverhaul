using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using ModifiersOverhaul.Assets.Balance;
using ModifiersOverhaul.Assets.CharmsModule.Items;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.CharmsModule.Manager;

public static class CharmsManager
{
    public static void Load()
    {
        
    }
    
    public static List<(CharmRarity, CharmType)> RollForCharms(NPC npc = null, bool? boss = null,  float luck = 1f)
    {
        List<(CharmRarity, CharmType)> rolledRarities = [];
        bool wasBoss = npc?.boss ?? (boss != null && boss.Value);
        int tries = wasBoss ? CharmBalance.RollTriesForBosses : 1;
        tries = (int)(tries * luck);
        for (int i = 0; i < tries; i++)
        {
            float rarityDice = Main.rand.NextFloat();
            foreach (var rarityChance in CharmBalance.CharmRarityChance)
            {
                if (rarityDice > rarityChance.Key) continue;
                CharmType type = RollCharmType();
                rolledRarities.Add((rarityChance.Value, type));
            }
        }

        return rolledRarities;
    }

    public static CharmType RollCharmType()
    {
        float typeDice = Main.rand.NextFloat();
        CharmType type = CharmBalance.CharmTypeChance.First(x => x.Key >= typeDice).Value;
        return type;
    }

    private static int GetNameStartPoint(CharmRarity rollRarity)
    {
        int nameID = 1;
        if ((int)rollRarity > (int)CharmRarity.Common) nameID += 10;
        if ((int)rollRarity > (int)CharmRarity.Rare) nameID += 10;
        if ((int)rollRarity > (int)CharmRarity.Epic) nameID += 10;
        if ((int)rollRarity > (int)CharmRarity.Legendary) nameID += 10;
        return nameID;
    }

    public static Vector3 GetRarityWorldColor(CharmRarity rarity)
    {
        Color rarityColor = CharmBalance.GetCharmColor(rarity);
        
        float multiplier = rarity switch
        {
            CharmRarity.NotInitialized => 1f,
            CharmRarity.Common => 1f,
            CharmRarity.Rare => 2f,
            CharmRarity.Epic => 3f,
            CharmRarity.Legendary => 4f,
            CharmRarity.Mythical => 5f,
            _ => throw new ArgumentOutOfRangeException(nameof(rarity), rarity, null)
        };
        
        return rarityColor.ToVector3() * multiplier;
    }
    
    /// <summary>
    /// atleast one of npc or spawnPos must be set
    /// </summary>
    public static void SpawnCharms(List<(CharmRarity rarity, CharmType type)> rolls, NPC npc = null, Vector2? spawnPos = null)
    {
        if (rolls.Count == 0) return;

        Vector2 pos = spawnPos ?? npc!.Center;
        int itemType = ModContent.ItemType<Charm>();
        
        foreach (var (rollRarity, type) in rolls)
        {
            Item item = Main.item[Item.NewItem(npc?.GetSource_Death(), pos, itemType)];
            Charm charmItem = (Charm)item.ModItem;
            charmItem.CharmRarity = rollRarity;
            charmItem.CharmType = type;
            int nameID = GetNameStartPoint(rollRarity);
            int rolledName = Main.rand.Next(nameID, nameID + 10);
            charmItem.CharmNameID = rolledName;

            int minRolls = CharmBalance.MinStatRolls + (int)rollRarity;
            int maxRolls = CharmBalance.MaxStatRolls + (int)rollRarity;
            int dice = Main.rand.Next(minRolls, maxRolls + 1);
            
            for (int i = 0; i < dice; i++)
            {
                CharmStat rolledStat;
                
                while (true)
                {
                    rolledStat = CharmBalance.GetRandomStat();
                    if (CharmBalance.IsCharmRareEnoughForStat(rolledStat, rollRarity)) break;
                }

                (float minStatBound, float maxStatBound) = CharmBalance.GetStatBounds(rolledStat, rollRarity);
                float strength = Main.rand.NextFloat(minStatBound, maxStatBound);
                
                charmItem.Stats.Add(new CharmRoll(rolledStat, strength));
            }
            
            ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral($"{rollRarity} {type}"), Color.Azure);
            
            if (Main.netMode == NetmodeID.MultiplayerClient)
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item.whoAmI);
        }
    }
}
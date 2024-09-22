﻿using System;
using System.Collections.Generic;
using System.Linq;
using ModifiersOverhaul.Assets.Balance;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets;

public static class CombatUtils
{
    public static void Lifesteal(Entity victim, Vector2 hitPosition, int healAmount, int playerWhoAmI)
    {
        if (victim is NPC { type: NPCID.TargetDummy } && !PrefixBalance.DEV_MODE) return;

        Projectile.NewProjectile(new EntitySource_OnHit(Main.player[playerWhoAmI], victim),
            hitPosition.X, hitPosition.Y, 0f, 0f, 305, 0, 0f,
            playerWhoAmI, playerWhoAmI, healAmount);
    }

    public static void Lifesteal(Entity victim, Vector2 hitPosition, int healAmount, Player player)
    {
        Lifesteal(victim, hitPosition, healAmount, player.whoAmI);
    }

    private static readonly int[] coinIds =
        [ItemID.CopperCoin, ItemID.SilverCoin, ItemID.GoldCoin, ItemID.PlatinumCoin];

    public static void DropCoins(float value, Entity entityToDropCoinsFrom)
    {
        if (entityToDropCoinsFrom is NPC { type: NPCID.TargetDummy } && !PrefixBalance.DEV_MODE) return;

        var isNegative = value < 0;
        Player player = null;
        if (isNegative) player = (Player)entityToDropCoinsFrom;

        var absValue = Math.Abs(value);
        var platinumCoins = (int)(absValue / 1000000);
        absValue -= platinumCoins * 1000000;

        var goldCoins = (int)(absValue / 10000);
        absValue -= goldCoins * 10000;

        var silverCoins = (int)(absValue / 100);
        absValue -= silverCoins * 100;

        var copperCoins = (int)absValue;

        List<(int coinType, int count)> coinList =
        [
            (coinIds[3], platinumCoins),
            (coinIds[2], goldCoins),
            (coinIds[1], silverCoins),
            (coinIds[0], copperCoins)
        ];

        while (coinList.Any(c => c.count > 0))
        {
            var availableCoins = coinList.Where(c => c.count > 0).ToList();
            var selectedCoin = availableCoins[Main.rand.Next(availableCoins.Count)];
            if (isNegative) player.BuyItem(selectedCoin.coinType);

            DropCoin(entityToDropCoinsFrom, selectedCoin.coinType);

            for (var i = 0; i < coinList.Count; i++)
            {
                if (coinList[i].coinType != selectedCoin.coinType) continue;

                coinList[i] = (coinList[i].coinType, coinList[i].count - 1);
                break;
            }
        }
    }

    private static void DropCoin(Entity entityToDropCoinsFrom, int coinType)
    {
        var itemDropID = Item.NewItem(new EntitySource_Loot(entityToDropCoinsFrom, "ModifiersOverhaul DropCoins"),
            (int)entityToDropCoinsFrom.position.X, (int)entityToDropCoinsFrom.position.Y, entityToDropCoinsFrom.width,
            entityToDropCoinsFrom.height, coinType);
        var itemDrop = Main.item[itemDropID];

        itemDrop.velocity.Y = Main.rand.Next(-20, 1) * 0.2f;
        itemDrop.velocity.X = Main.rand.Next(-20, 21) * 0.2f;
        itemDrop.noGrabDelay = 100;
        itemDrop.newAndShiny = true;

        if (Main.netMode == NetmodeID.MultiplayerClient)
            NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemDropID);

        var dice = Main.rand.NextFloat();
        if (dice > 0.2f) TryMergeCoins(itemDrop.position, itemDrop.type);
    }

    /// <param name="velocity">root projectile triple shot originates from velocity</param>
    /// <param name="degrees">degrees</param>
    /// <param name="variation">variation to the degrees per projectile</param>
    /// <returns>Two other projectile velocities rotated by degrees</returns>
    public static (Vector2, Vector2) TripleShotRotatedVelocities(Vector2 velocity, float degrees, float variation)
    {
        var radians1 = MathF.PI / 180 * (degrees + Main.rand.NextFloat(-variation, variation));
        var radians2 = MathF.PI / 180 * (degrees + Main.rand.NextFloat(-variation, variation));

        var rotatedVelocityPositive = UtilMethods.RotateVector(velocity, radians1);
        var rotatedVelocityNegative = UtilMethods.RotateVector(velocity, -radians2);

        return (rotatedVelocityPositive, rotatedVelocityNegative);
    }

    public static bool TryFindSegments(NPC npc, out List<NPC> segments)
    {
        segments = [];
        int targetNpcRealLife = npc.realLife;
        
        foreach (NPC testNpc in Main.ActiveNPCs)
        {
            int testNpcRealLife = testNpc.realLife;
            
            if (testNpcRealLife != targetNpcRealLife) continue;
            
            segments.Add(testNpc);
        }

        if (segments.Count <= 0) return false;
        
        segments.Add(Main.npc[npc.realLife]);
        return true;
    }

    public static bool IsWeapon(this Item item)
    {
        bool isPickaxe = item.pick > 0;
        bool isAxe = item.axe > 0;
        bool isHammer = item.hammer > 0;
        return item.damage > 0 && !isPickaxe && !isAxe && !isHammer;
    }

    private static void TryMergeCoins(Vector2 rootCoinPos, int coinType)
    {
        if (coinType == ItemID.PlatinumCoin) return;

        var allSameCoins = Main.item.Where(x => x.type == coinType).ToArray();
        if (allSameCoins.Length < 100) return;
        List<Item> sameCoinsInArea = new();
        const float maxDist = 500000f;

        foreach (var sameTypeCoin in allSameCoins)
        {
            var dist = Vector2.DistanceSquared(sameTypeCoin.Center, rootCoinPos);
            if (dist > maxDist) continue;
            sameCoinsInArea.Add(sameTypeCoin);
        }

        if (sameCoinsInArea.Count < 100) return;

        int mergeInto = coinType switch
        {
            ItemID.CopperCoin => ItemID.SilverCoin,
            ItemID.SilverCoin => ItemID.GoldCoin,
            ItemID.GoldCoin => ItemID.PlatinumCoin,
            _ => throw new ArgumentOutOfRangeException(nameof(coinType), coinType, null)
        };


        for (var i = 0; i < 100; i++) DeleteCoin(sameCoinsInArea[i]);

        var itemDropID = Item.NewItem(new EntitySource_Misc("CoinMerge"), rootCoinPos, Vector2.One, mergeInto);
        var itemDrop = Main.item[itemDropID];
        itemDrop.noGrabDelay = 100;
        itemDrop.newAndShiny = true;

        if (Main.netMode == NetmodeID.MultiplayerClient)
            NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemDropID);
    }

    private static void DeleteCoin(Item coin)
    {
        //TODO: main has merge item method
        coin.TurnToAir();
        if (Main.netMode == NetmodeID.MultiplayerClient)
            NetMessage.SendData(MessageID.SyncItem, -1, -1, null, coin.whoAmI);
    }
}
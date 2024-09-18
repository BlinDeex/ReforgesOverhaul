using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ModifiersOverhaul.Assets.Balance;
using ModifiersOverhaul.Assets.ModPlayers;
using ModifiersOverhaul.Assets.ModPrefixes.Axe;
using ModifiersOverhaul.Assets.ModPrefixes.Pickaxe;
using ModifiersOverhaul.Assets.ModPrefixes.Tool;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.Misc;

public class ToolPrefixManipulation : ModSystem
{
    public override void PostSetupContent()
    {
        ModifiedTileBreaking.Instance.OnPlayerKilledTile += OnTileKilled;
    }

    private void OnTileKilled(Player player, int x, int y, Item item, bool wall)
    {
        if (item.prefix < PrefixID.Count) return;
        if (player.whoAmI != Main.myPlayer) return;
        var itemPrefix = PrefixLoader.GetPrefix(item.prefix);
        if (itemPrefix is not ISpecializedPrefix) return;

        if (itemPrefix.Type == ModContent.PrefixType<PrefixClearing>())
        {
            ClearingPrefix(x, y, wall);
            return;
        }

        if (itemPrefix.Type == ModContent.PrefixType<PrefixVeinMiner>())
        {
            VeinMinerPrefix(x, y, item);
            return;
        }

        if (itemPrefix.Type == ModContent.PrefixType<PrefixFortune>())
        {
            FortunePrefix(player);
            return;
        }

        if (itemPrefix.Type == ModContent.PrefixType<PrefixRevealing>())
        {
            RevealingPrefix(player);
            return;
        }
    }

    private static void RevealingPrefix(Player player)
    {
        var dice = Main.rand.NextFloat();
        if (dice > PrefixBalance.REVEALING_CHANCE) return;

        player.AddBuff(BuffID.Spelunker, 20);
        player.GetModPlayer<ToolPlayer>().SetRevealing();
    }

    private static void FortunePrefix(Player player)
    {
        player.GetModPlayer<ToolPlayer>().SetAxeFortune();
    }

    private static void VeinMinerPrefix(int x, int y, Item pickaxe)
    {
        int tileType = Main.tile[x, y].TileType;
        var isSpelunkable = Main.tileSpelunker[tileType];
        if (!isSpelunkable) return;

        var connectedTiles = UtilMethods.GetConnectedTiles(x, y, pickaxe.pick);

        foreach (var connectedTile in connectedTiles) KillTileAndSync(connectedTile, false, false);
    }

    private static void ClearingPrefix(int x, int y, bool wall)
    {
        List<Point> additionalArea =
        [
            new Point(x + 1, y),
            new Point(x - 1, y),
            new Point(x, y + 1),
            new Point(x, y - 1),
            new Point(x + 1, y + 1),
            new Point(x + 1, y - 1),
            new Point(x - 1, y + 1),
            new Point(x - 1, y - 1)
        ];

        foreach (var pos in additionalArea)
            if (wall)
            {
                if (Main.tile[pos].WallType == 0) continue;
                var loseItem = Main.rand.NextFloat() < PrefixBalance.CLEARING_CHANCE_TO_LOSE_MINED_BLOCK;
                KillTileAndSync(pos, loseItem, true);
            }
            else
            {
                if (!Main.tile[pos].HasTile) continue;
                var loseItem = Main.rand.NextFloat() < PrefixBalance.CLEARING_CHANCE_TO_LOSE_MINED_BLOCK;
                KillTileAndSync(pos, loseItem, false);
            }
    }

    public static void KillTileAndSync(Point pos, bool loseItem, bool wall)
    {
        if (wall)
            ModifiedTileBreaking.KillWallReplacement(pos.X, pos.Y, noItem: loseItem);
        else
            WorldGen.KillTile(pos.X, pos.Y, noItem: loseItem);

        if (Main.netMode != NetmodeID.MultiplayerClient) return;

        if (!loseItem)
        {
            NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, wall ? 2 : 0, pos.X, pos.Y);
            return;
        }

        NetMessage.SendTileSquare(-1, pos.X, pos.Y);
    }
}
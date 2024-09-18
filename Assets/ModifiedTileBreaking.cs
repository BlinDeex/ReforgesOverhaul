using System.Reflection;
using Microsoft.Xna.Framework;
using ModifiersOverhaul.Assets.Balance;
using ModifiersOverhaul.Assets.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets;

public class ModifiedTileBreaking : ModSystem
{
    private MethodInfo getPickaxeDamage;
    private MethodInfo clearMiningCacheAt;

    private static MethodInfo killWall_CheckFailure;
    private static MethodInfo killWall_PlaySounds;
    private static MethodInfo killWall_MakeWallDust;
    private static MethodInfo killWall_DropItems;

    public static ModifiedTileBreaking Instance { get; private set; }

    public delegate void PlayerKilledTile(Player player, int x, int y, Item item, bool wall = false);


    /// <summary>
    /// called just before killing the tile/wall
    /// </summary>
    public event PlayerKilledTile OnPlayerKilledTile;

    public override void Load()
    {
        Instance = this;
    }

    public override void PostSetupContent()
    {
        getPickaxeDamage = typeof(Player).GetMethod("GetPickaxeDamage", BindingFlags.Instance | BindingFlags.NonPublic);
        clearMiningCacheAt =
            typeof(Player).GetMethod("ClearMiningCacheAt", BindingFlags.Instance | BindingFlags.NonPublic);

        killWall_CheckFailure =
            typeof(WorldGen).GetMethod("KillWall_CheckFailure", BindingFlags.Static | BindingFlags.NonPublic);
        killWall_PlaySounds =
            typeof(WorldGen).GetMethod("KillWall_PlaySounds", BindingFlags.Static | BindingFlags.NonPublic);
        killWall_MakeWallDust =
            typeof(WorldGen).GetMethod("KillWall_MakeWallDust", BindingFlags.Static | BindingFlags.NonPublic);
        killWall_DropItems =
            typeof(WorldGen).GetMethod("KillWall_DropItems", BindingFlags.Static | BindingFlags.NonPublic);

        On_Player.ItemCheck_UseMiningTools_ActuallyUseMiningTool += ItemCheck_MiningTools;
        On_Player.ItemCheck_UseMiningTools_TryHittingWall += ItemCheck_UseMiningTools_TryHittingWall_Replacement;
    }


    // there must be an easier way to find out who kills the tile/wall dawg
    private void ItemCheck_MiningTools(On_Player.orig_ItemCheck_UseMiningTools_ActuallyUseMiningTool orig, Player self,
        Item sitem, out bool canhitwalls, int x, int y)
    {
        var targetTile = Main.tile[x, y];

        canhitwalls = sitem.hammer > 0;

        if (canhitwalls && targetTile.WallType != 0) 
        {
            // orig invoke cause ItemCheck_UseMiningTools_ActuallyUseMiningTool will end up calling ItemCheck_UseMiningTools_TryHittingWall
            // to break the wall which is detoured as well
            orig.Invoke(self, sitem, out canhitwalls, x, y);
            return;
        }
        
        if (!targetTile.HasTile)
        {
            orig.Invoke(self, sitem, out canhitwalls, x, y);
            return;
        }

        var hitTile = Main.LocalPlayer.hitTile;

        var hitBufferIndex = hitTile.HitObject(x, y, 1);

        var pickaxeDamageObj = getPickaxeDamage.Invoke(Main.LocalPlayer,
            [x, y, Main.LocalPlayer.HeldItem.pick, hitBufferIndex, targetTile]);
        var pickaxeDamage = (int)pickaxeDamageObj!;
        var totalDamage = hitTile.data[hitBufferIndex].damage + pickaxeDamage;

        if (totalDamage < 100)
        {
            orig.Invoke(self, sitem, out canhitwalls, x, y);
            return;
        }

        OnPlayerKilledTile?.Invoke(Main.LocalPlayer, x, y, sitem);

        orig.Invoke(self, sitem, out canhitwalls, x, y);
    }

    private void ItemCheck_UseMiningTools_TryHittingWall_Replacement(
        On_Player.orig_ItemCheck_UseMiningTools_TryHittingWall orig, Player self, Item sItem, int wX, int wY)
    {
        var lPlayer = Main.LocalPlayer;
        if (Main.tile[wX, wY].WallType <= 0) return;
        if (Main.tile[wX, wY].HasTile && wX == Player.tileTargetX && wY == Player.tileTargetY &&
            (Main.tileHammer[Main.tile[wX, wY].TileType] || lPlayer.poundRelease)) return;
        if (lPlayer.toolTime != 0) return;
        if (lPlayer.itemAnimation <= 0) return;
        if (!lPlayer.controlUseItem) return;
        if (sItem.hammer <= 0) return;
        if (!Player.CanPlayerSmashWall(wX, wY)) return;
        
        var damage = (int)(sItem.hammer * 1.5f);
        PickWallReplacement(wX, wY, damage, sItem);
        Main.LocalPlayer.itemTime = sItem.useTime / 2;
    }

    private void PickWallReplacement(int x, int y, int damage, Item item)
    {
        var hitTile = Main.LocalPlayer.hitTile;
        var tileId = hitTile.HitObject(x, y, 2);
        if (hitTile.AddDamage(tileId, damage) >= 100)
        {
            hitTile.Clear(tileId);
            clearMiningCacheAt.Invoke(Main.LocalPlayer, [x, y, 2]);

            OnPlayerKilledTile?.Invoke(Main.LocalPlayer, x, y, item, true);
            WorldGen.KillWall(x, y);


            var dice = Main.rand.NextFloat();
            var loseItem = dice <= PrefixBalance.CLEARING_CHANCE_TO_LOSE_MINED_BLOCK;

            if (loseItem && damage != 0) hitTile.Prune();

            ToolPrefixManipulation.KillTileAndSync(new Point(x, y), loseItem, true);
        }
        else
        {
            WorldGen.KillWall(x, y, true);

            if (Main.netMode == NetmodeID.MultiplayerClient)
                NetMessage.SendData(17, -1, -1, null, 2, x, y, 1f);
        }

        if (damage != 0)
            hitTile.Prune();
    }


    public static void KillWallReplacement(int i, int j, bool fail = false, bool noItem = false)
    {
        if (i < 0 || j < 0 || i >= Main.maxTilesX || j >= Main.maxTilesY) return;

        var tile = Main.tile[i, j];

        if (tile.WallType <= 0) return;

        var failObj = killWall_CheckFailure.Invoke(null, [fail, tile]);
        fail = (bool)failObj!;

        WallLoader.KillWall(i, j, tile.WallType, ref fail);

        killWall_PlaySounds.Invoke(null, [i, j, tile, fail]);
        var dustCount = 10;
        if (fail) dustCount = 3;

        WallLoader.NumDust(i, j, tile.WallType, fail, ref dustCount);

        for (var k = 0; k < dustCount; k++) killWall_MakeWallDust.Invoke(null, [i, j, tile]);

        if (fail)
        {
            WorldGen.SquareWallFrame(i, j);
            return;
        }

        if (!noItem) killWall_DropItems.Invoke(null, [i, j, tile]);

        tile.WallType = 0;
        tile.ClearWallPaintAndCoating();
        WorldGen.SquareWallFrame(i, j);
        if (TileID.Sets.FramesOnKillWall[tile.TileType]) WorldGen.TileFrame(i, j);
    }
}
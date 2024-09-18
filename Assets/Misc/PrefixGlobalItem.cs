using Microsoft.Xna.Framework;
using ModifiersOverhaul.Assets.Balance;
using ModifiersOverhaul.Assets.InstancedGlobalItems;
using ModifiersOverhaul.Assets.ModPlayers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.Misc;

public class PrefixGlobalItem : GlobalItem
{
    public override void OnSpawn(Item item, IEntitySource source)
    {
        OnSpawnFortune(item, source);
    }

    private void OnSpawnFortune(Item item, IEntitySource source)
    {
        if (source is not EntitySource_TileBreak tileBreak) return;
        var fortuneDrop = item.GetGlobalItem<InstancedGlobalItem>().FortuneDrop;
        if (fortuneDrop) return;
        var isATree = TileID.Sets.IsATreeTrunk[Main.tile[tileBreak.TileCoords].TileType];
        if (!isATree) return;
        var isFortuneActive = Main.LocalPlayer.GetModPlayer<ToolPlayer>().AxeFortune > 0;
        if (!isFortuneActive) return; //TODO: probably not compatible in multiplayer

        var stack = item.stack;
        for (var i = 0; i < stack; i++)
        {
            var dice = Main.rand.NextFloat();
            if (dice > PrefixBalance.FORTUNE_CHANCE_FOR_EXTRA_DROPS) return;
            var bonusItem = Item.NewItem(new EntitySource_Misc("FortuneDrop"), item.position, new Vector2(8, 8),
                item.type,
                PrefixBalance.FORTUNE_EXTRA_DROP_NUM);

            Main.item[bonusItem].GetGlobalItem<InstancedGlobalItem>().FortuneDrop = true;
        }
    }
}
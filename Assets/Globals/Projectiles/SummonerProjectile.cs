using ModifiersOverhaul.Assets.InstancedGlobalItems;
using ModifiersOverhaul.Assets.ModPrefixes.Summoner.MinionWeapons;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.Globals.Projectiles;

public class SummonerProjectile : GlobalProjectile
{
    public static void OnMinionSpawn(Projectile projectile, IEntitySource source, InstancedProjectilePrefix projPrefix)
    {
        if (!projectile.minion) return;
        var heldItem = Main.LocalPlayer.HeldItem;
        if (heldItem.shoot != projectile.type) return;
        if (heldItem.prefix == ModContent.PrefixType<PrefixFrenzied>()) projPrefix.Frenzied = true;
    }
}
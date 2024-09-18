using Terraria.ID;

namespace ModifiersOverhaul.Assets.Misc;

public static class AdaptableUtils
{
    public static readonly RocketAndProjID[] ROCKET_TO_PROJ_IDS =
    [
        new RocketAndProjID(ItemID.RocketI, ProjectileID.RocketI),
        new RocketAndProjID(ItemID.RocketII, ProjectileID.RocketII),
        new RocketAndProjID(ItemID.RocketIII, ProjectileID.RocketIII),
        new RocketAndProjID(ItemID.RocketIV, ProjectileID.RocketIV),
        new RocketAndProjID(ItemID.ClusterRocketI, ProjectileID.ClusterRocketI),
        new RocketAndProjID(ItemID.ClusterRocketII, ProjectileID.ClusterRocketII),
        new RocketAndProjID(ItemID.DryRocket, ProjectileID.DryRocket),
        new RocketAndProjID(ItemID.WetRocket, ProjectileID.WetRocket),
        new RocketAndProjID(ItemID.LavaRocket, ProjectileID.LavaRocket),
        new RocketAndProjID(ItemID.HoneyRocket, ProjectileID.HoneyRocket),
        new RocketAndProjID(ItemID.MiniNukeI, ProjectileID.MiniNukeRocketI),
        new RocketAndProjID(ItemID.MiniNukeII, ProjectileID.MiniNukeRocketII)
    ];

    public readonly struct RocketAndProjID(int rocketAmmoID, int rocketProjectileID)
    {
        public int RocketAmmoID => rocketAmmoID;
        public int RocketProjectileID => rocketProjectileID;
    }

    public static readonly int[] ROCKET_WEAPON_IDS =
    [
        ItemID.GrenadeLauncher,
        ItemID.RocketLauncher,
        ItemID.ProximityMineLauncher,
        ItemID.SnowmanCannon,
        ItemID.ElectrosphereLauncher,
        ItemID.FireworksLauncher,
        ItemID.Celeb2
    ];
}
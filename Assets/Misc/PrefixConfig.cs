using Terraria.ModLoader.Config;

namespace ModifiersOverhaul.Assets.Misc;

public class PrefixConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ServerSide;

    public bool AllowVanillaPrefixes { get; set; }
}
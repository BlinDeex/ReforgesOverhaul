using Microsoft.Xna.Framework.Input;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.Misc;

public class PrefixKeybinds : ModSystem
{
    public static ModKeybind ArmorActivationKeybind;

    public override void PostSetupContent()
    {
        ArmorActivationKeybind =
            KeybindLoader.RegisterKeybind(ModifiersOverhaul.Instance, nameof(ArmorActivationKeybind), Keys.R);
    }
}
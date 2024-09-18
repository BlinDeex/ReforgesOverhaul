﻿using Terraria.Localization;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.Buffs;

public class SacrificialBuff : ModBuff
{
    public override LocalizedText DisplayName => Mod.GetLocalization($"Buffs.{nameof(SacrificialBuff)}");

    public override LocalizedText Description => Mod.GetLocalization($"Buffs.{nameof(SacrificialBuff)}Desc");

    public override string Texture => $"{Mod.Name}/Assets/Textures/Buffs/SacrificialIcon";
}
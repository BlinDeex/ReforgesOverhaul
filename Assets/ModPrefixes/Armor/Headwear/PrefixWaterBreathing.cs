using System;
using System.Collections.Generic;
using ModifiersOverhaul.Assets.Balance;
using ModifiersOverhaul.Assets.Misc;
using ModifiersOverhaul.Assets.ModPlayers.Armor;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.ModPrefixes.Armor.Headwear;

public class PrefixWaterBreathing : ModPrefix, ISpecializedPrefix
{
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Headwear;
    
    public override PrefixCategory Category => PrefixCategory.Accessory;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    }

    public static LocalizedText Desc { get; private set; }
    public static LocalizedText SetBonus { get; private set; }
    
    public override LocalizedText DisplayName => LocalizationManager.GetPrefixLocalization(this,"WaterBreathing", "DisplayName");

    public override void SetStaticDefaults()
    {
        Desc = LocalizationManager.GetPrefixLocalization(this,"WaterBreathing", nameof(Desc));
        SetBonus = SharedLocalization.GetSharedLocalizedText(SharedLocalization.NoArmorSetBonus);
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        var newLine = new TooltipLine(Mod, "newLine", Desc.Value)
        {
            IsModifier = true
        };
        
        var newLine2 = new TooltipLine(Mod, "newLine2", SetBonus.Value)
        {
            IsModifier = true,
            IsModifierBad = true
        };

        yield return newLine;
        yield return newLine2;
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        player.GetModPlayer<MiscArmorPlayer>().WaterBreathing = true;
    }
}
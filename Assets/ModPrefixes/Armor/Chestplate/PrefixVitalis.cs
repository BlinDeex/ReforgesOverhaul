using System;
using System.Collections.Generic;
using ModifiersOverhaul.Assets.Balance;
using ModifiersOverhaul.Assets.Misc;
using ModifiersOverhaul.Assets.ModPlayers;
using ModifiersOverhaul.Assets.ModPlayers.Armor;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.ModPrefixes.Armor.Chestplate;

public class PrefixVitalis : ModPrefix, ISpecializedPrefix
{
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Chestplate;
    
    public override PrefixCategory Category => PrefixCategory.Accessory;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    }

    public static LocalizedText Desc { get; private set; }
    public static LocalizedText NoSetBonus { get; private set; }
    
    public override LocalizedText DisplayName => LocalizationManager.GetPrefixLocalization(this,"Vitalis", "DisplayName");

    public override void SetStaticDefaults()
    {
        Desc = LocalizationManager.GetPrefixLocalization(this,"Vitalis", nameof(Desc));
        NoSetBonus = SharedLocalization.GetSharedLocalizedText(SharedLocalization.NoArmorSetBonus);
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        var newLine = new TooltipLine(Mod, "newLine", Desc.Format(Math.Round((PrefixBalance.VITALIS_LIFESTEAL_AMP - 1) * 100), 0))
        {
            IsModifier = true
        };
        
        var newLine2 = new TooltipLine(Mod, "newLine2", NoSetBonus.Value)
        {
            IsModifier = true,
            IsModifierBad = true
        };

        yield return newLine;
        yield return newLine2;
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        player.GetModPlayer<GeneralStatPlayer>().AddHealingMul(PrefixBalance.VITALIS_LIFESTEAL_AMP);
    }
}
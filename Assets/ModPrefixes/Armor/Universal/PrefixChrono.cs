using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ModifiersOverhaul.Assets.Balance;
using ModifiersOverhaul.Assets.Misc;
using ModifiersOverhaul.Assets.ModPlayers;
using ModifiersOverhaul.Assets.ModPlayers.Armor;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.ModPrefixes.Armor.Universal;

public class PrefixChrono : ModPrefix, ISpecializedPrefix
{
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Headwear |
                                                          SpecializedPrefixType.Chestplate |
                                                          SpecializedPrefixType.Leggings;
    

    public override PrefixCategory Category => PrefixCategory.Accessory;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.ARMOR_REFORGING_MULTIPLIER;
    }
    public static LocalizedText SetBonus { get; private set; }
    
    public static LocalizedText Desc { get; private set; }
    
    public override LocalizedText DisplayName => LocalizationManager.GetPrefixLocalization(this,"Chrono", "DisplayName");


    public override void SetStaticDefaults()
    {
        SetBonus = LocalizationManager.GetPrefixLocalization(this,"Chrono", nameof(SetBonus));
        Desc = LocalizationManager.GetPrefixLocalization(this,"Chrono", nameof(Desc));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        var newLine = new TooltipLine(Mod, "newLine",
            Desc.Format(MathF.Round((PrefixBalance.CHRONO_MOVEMENT_SPEED - 1) * 100, 1)))
        {
            IsModifier = true
        };
        bool setBonusActive = Main.LocalPlayer.GetModPlayer<ChronoArmorPlayer>().ChronoSetBonus;
        
        
        var newLine2 = new TooltipLine(Mod, "newLine2",
            SetBonus.Format(MathF.Round((int)(PrefixBalance.CHRONO_ABILITY_LENGTH / 60f))))
        {
            IsModifier = true,
            IsModifierBad = !setBonusActive
        };
        
        yield return newLine;
        yield return newLine2;
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        player.GetModPlayer<ChronoArmorPlayer>().ChronoPiecesEquipped++;
        player.GetModPlayer<GeneralStatPlayer>().MovementSpeedMul += PrefixBalance.CHRONO_MOVEMENT_SPEED - 1;
    }
}
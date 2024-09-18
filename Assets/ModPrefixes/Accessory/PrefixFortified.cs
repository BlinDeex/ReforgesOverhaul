using System.Collections.Generic;
using ModifiersOverhaul.Assets.Balance;
using ModifiersOverhaul.Assets.Misc;
using ModifiersOverhaul.Assets.ModPlayers;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.ModPrefixes.Accessory;

public class PrefixFortified : ModPrefix
{
    public override PrefixCategory Category => PrefixCategory.Accessory;

    public override LocalizedText DisplayName => LocalizationManager.GetPrefixLocalization(this,"Fortified", "DisplayName");
    public static LocalizedText Desc { get; private set; }

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.ACCESSORY_REFORGING_MULTIPLIER;
    }

    public override void SetStaticDefaults()
    {
        Desc = LocalizationManager.GetPrefixLocalization(this,"Fortified", nameof(Desc));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        var newLine =
            new TooltipLine(Mod, "newLine", Desc.Format(PrefixBalance.FORTIFIED_INCREASED_DEFENSE * 100))
            {
                IsModifier = true
            };

        yield return newLine;
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        var statPlayer = player.GetModPlayer<GeneralStatPlayer>();
        statPlayer.DefenseMul += PrefixBalance.FORTIFIED_INCREASED_DEFENSE;
    }
}
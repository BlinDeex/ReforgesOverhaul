using System.Collections.Generic;
using ModifiersOverhaul.Assets.Balance;
using ModifiersOverhaul.Assets.Misc;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;

namespace ModifiersOverhaul.Assets.ModPrefixes.Ranged;

public class PrefixGiantSlayer : ModPrefix
{
    public override PrefixCategory Category => PrefixCategory.Ranged;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    }

    public static LocalizedText GiantSlayerDesc { get; private set; }
    
    public override LocalizedText DisplayName => LocalizationManager.GetPrefixLocalization(this,"GiantSlayer", "DisplayName");


    public override void SetStaticDefaults()
    {
        GiantSlayerDesc = LocalizationManager.GetPrefixLocalization(this,"GiantSlayer", nameof(GiantSlayerDesc));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        var newLine = new TooltipLine(Mod, "newLine",
            GiantSlayerDesc.Format(PrefixBalance.GIANT_SLAYER_PERCENT_DAMAGE))
        {
            OverrideColor = Color.WhiteSmoke
        };

        yield return newLine;
    }
}
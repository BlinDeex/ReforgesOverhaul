using System.Collections.Generic;
using ModifiersOverhaul.Assets.Balance;
using ModifiersOverhaul.Assets.Misc;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.ModPrefixes.Axe;

public class PrefixFortune : ModPrefix, ISpecializedPrefix
{
    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.Axe;

    public override PrefixCategory Category => PrefixCategory.AnyWeapon;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.TOOL_REFORGING_MULTIPLIER;
    }

    public static LocalizedText Desc { get; private set; }
    
    public override LocalizedText DisplayName => LocalizationManager.GetPrefixLocalization(this,"Fortune", "DisplayName");

    public override void SetStaticDefaults()
    {
        Desc = LocalizationManager.GetPrefixLocalization(this,"Fortune", nameof(Desc));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        var newLine = new TooltipLine(Mod, "newLine",
            Desc.Format((int)(PrefixBalance.FORTUNE_CHANCE_FOR_EXTRA_DROPS * 100f),
                PrefixBalance.FORTUNE_EXTRA_DROP_NUM))
        {
            IsModifier = true
        };

        yield return newLine;
    }
}
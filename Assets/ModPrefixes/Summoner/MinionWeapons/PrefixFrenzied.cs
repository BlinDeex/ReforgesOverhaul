using System.Collections.Generic;
using ModifiersOverhaul.Assets.Balance;
using ModifiersOverhaul.Assets.Misc;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;

namespace ModifiersOverhaul.Assets.ModPrefixes.Summoner.MinionWeapons;

public class PrefixFrenzied : ModPrefix, ISpecializedPrefix
{
    public override PrefixCategory Category => PrefixCategory.Magic;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    }

    public static LocalizedText Desc { get; private set; }
    
    public override LocalizedText DisplayName => LocalizationManager.GetPrefixLocalization(this,"Frenzied", "DisplayName");


    public override void SetStaticDefaults()
    {
        Desc = LocalizationManager.GetPrefixLocalization(this,"Frenzied", nameof(Desc));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        var newLine = new TooltipLine(Mod, "newLine",
            Desc.Format(PrefixBalance.GIANT_SLAYER_PERCENT_DAMAGE))
        {
            OverrideColor = Color.WhiteSmoke
        };

        yield return newLine;
    }

    public SpecializedPrefixType SpecializedPrefixType => SpecializedPrefixType.MinionWeapon;
}
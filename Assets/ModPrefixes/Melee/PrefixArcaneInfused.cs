﻿using System.Collections.Generic;
using ModifiersOverhaul.Assets.Balance;
using ModifiersOverhaul.Assets.Misc;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.ModPrefixes.Melee;

public class PrefixArcaneInfused : ModPrefix
{
    public override PrefixCategory Category => PrefixCategory.Melee;

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = PrefixBalance.WEAPON_REFORGING_MULTIPLIER;
    }

    public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult,
        ref float scaleMult,
        ref float shootSpeedMult, ref float manaMult, ref int critBonus)
    {
        damageMult *= PrefixBalance.ARCANE_INFUSED_DAMAGE;
    }

    public static LocalizedText ManaPerSwing { get; private set; }
    public static LocalizedText ManaSicknessWorks { get; private set; }
    
    public override LocalizedText DisplayName => LocalizationManager.GetPrefixLocalization(this,"ArcaneInfused", "DisplayName");

    public override void SetStaticDefaults()
    {
        ManaPerSwing = LocalizationManager.GetPrefixLocalization(this,"ArcaneInfused", nameof(ManaPerSwing));
        ManaSicknessWorks = LocalizationManager.GetPrefixLocalization(this,"ArcaneInfused", nameof(ManaSicknessWorks));
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        var manaLine = new TooltipLine(Mod, "manaLine",
            ManaPerSwing.Format(PrefixBalance.ARCANE_INFUSED_MANA_PER_SWING))
        {
            IsModifier = true,
            IsModifierBad = true
        };

        var manaLine2 = new TooltipLine(Mod, "manaLine2",
            ManaSicknessWorks.Value)
        {
            IsModifier = true,
            IsModifierBad = true
        };

        yield return manaLine;
        yield return manaLine2;
    }
}
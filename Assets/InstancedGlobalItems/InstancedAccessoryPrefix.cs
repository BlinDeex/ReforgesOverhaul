﻿using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.InstancedGlobalItems;

public class InstancedAccessoryPrefix : GlobalItem
{
    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (!item.accessory) return;
        if (item.prefix == 0) return;
        var title = tooltips[0];
        title.OverrideColor = Main.DiscoColor;
    }
}
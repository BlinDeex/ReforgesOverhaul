using Terraria;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.Globals.Armor;

public class GlobalArmorPrefix : GlobalItem
{
    public override void SetDefaults(Item entity)
    {
        entity.accessory = entity.IsArmor() || entity.accessory;
    }
}
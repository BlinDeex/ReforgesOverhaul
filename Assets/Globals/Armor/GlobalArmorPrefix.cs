using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.Globals.Armor;

public class GlobalArmorPrefix : GlobalItem
{
    public override void SetDefaults(Item entity)
    {
        entity.accessory = entity.accessory || entity.IsArmor();
    }
}
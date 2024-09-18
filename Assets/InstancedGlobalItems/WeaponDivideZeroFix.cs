using Terraria;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.InstancedGlobalItems;

public class WeaponDivideZeroFix : GlobalItem
{
    public override bool CanUseItem(Item item, Player player)
    {
        if (item.useTime <= 0) item.useTime = 1; // tmod should do this for me ong
        if (item.useAnimation <= 0) item.useAnimation = 1;
        return true;
    }
}
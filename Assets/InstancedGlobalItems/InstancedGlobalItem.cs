using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.InstancedGlobalItems;

public class InstancedGlobalItem : GlobalItem
{
    public override bool InstancePerEntity => true;

    public bool FortuneDrop { get; set; }
}
using ModifiersOverhaul.Assets.Balance;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.ModPlayers;

public class ToolPlayer : ModPlayer
{
    public int AxeFortune { get; private set; }

    public int RevealingTicks { get; private set; }

    public void SetRevealing()
    {
        RevealingTicks = PrefixBalance.REVEALING_TICKS;
    }

    public void SetAxeFortune()
    {
        AxeFortune = 2; // activates fortune drops for 2 ticks after hitting tree with fortune axe
    }

    public override void PostUpdateEquips()
    {
        AxeFortune--;
        RevealingTicks--;
    }
}
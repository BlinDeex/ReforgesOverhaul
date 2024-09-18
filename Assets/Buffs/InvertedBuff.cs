using Microsoft.Xna.Framework;
using ModifiersOverhaul.Assets.Balance;
using ModifiersOverhaul.Assets.ModPlayers;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.Buffs;

public class InvertedBuff : ModBuff
{
    public override bool ReApply(Player player, int time, int buffIndex)
    {
        var newTime = time + 300;
        newTime = (int)MathHelper.Clamp(newTime, 0, 600);
        player.buffTime[buffIndex] = newTime;
        return true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        var timeLeft = player.buffTime[buffIndex];
        var targetMultiplier = timeLeft / 600f;
        var damageAdded = PrefixBalance.INVERTED_MAX_DAMAGE_INCREASE * targetMultiplier;
        player.GetDamage<MagicDamageClass>() += damageAdded;

        if (player.TryGetModPlayer(out PrefixPlayer prefixPlayer)) prefixPlayer.ManaSurgeMultiplier = damageAdded;

        var playerCenter = player.Center;
        var targetScale = targetMultiplier * PrefixBalance.INVERTED_MANA_SURGE_MAX_SCALE;

        for (var i = 0; i < PrefixBalance.INVERTED_MANA_SURGE_DUST_RATE; i++)
        {
            var xOffset = Main.rand.Next(-16, 16);
            var yOffset = Main.rand.Next(-32, 32);
            var dustPos = playerCenter + new Vector2(xOffset, yOffset);
            Dust.NewDust(dustPos, 1, 1, PrefixBalance.INVERTED_MANA_SURGE_DUST_ID, Scale: targetScale);
        }

        var dice = Main.rand.NextFloat();
        if (dice > PrefixBalance.INVERTED_MANA_SURGE_CHANCE_TO_DAMAGE) return;

        player.Hurt(MiscStuff.MANA_SURGE_DEATH,
            PrefixBalance.INVERTED_MANA_SURGE_DAMAGE, 0, quiet: true);
    }

    public override LocalizedText DisplayName => Mod.GetLocalization("Buffs.InvertedDisplayName");

    public override LocalizedText Description => Mod.GetLocalization("Buffs.InvertedDesc");

    public override string Texture => $"{Mod.Name}/Assets/Textures/Buffs/ManaSurgeIcon";

    public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
    {
        PrefixPlayer prefixPlayer = Main.LocalPlayer.GetModPlayer<PrefixPlayer>();
        tip = Description.Format((int)(prefixPlayer.ManaSurgeMultiplier * 100));
    }

    public override bool RightClick(int buffIndex)
    {
        return false;
    }
}
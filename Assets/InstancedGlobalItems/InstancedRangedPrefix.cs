using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using ModifiersOverhaul.Assets.Balance;
using ModifiersOverhaul.Assets.ModPlayers;
using ModifiersOverhaul.Assets.ModPrefixes.Ranged;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ModifiersOverhaul.Assets.InstancedGlobalItems;

public class InstancedRangedPrefix : GlobalItem
{
    public override bool InstancePerEntity => true;

    public int CurrentBurstCooldown { get; private set; }

    public float DamageDone { get; set; }
    public float DamageDoneRequired { get; private set; }

    public float DamageAdded { get; private set; }
    public float CritAdded { get; private set; }


    public override void SaveData(Item item, TagCompound tag)
    {
        if (DamageDone != 0)
            tag.Add(new KeyValuePair<string, object>(nameof(DamageDone), DamageDone));
        if (DamageDoneRequired != 0)
            tag.Add(new KeyValuePair<string, object>(nameof(DamageDoneRequired), DamageDoneRequired));
    }

    public override void LoadData(Item item, TagCompound tag)
    {
        DamageDone = tag.TryGet(nameof(DamageDone), out float damage) ? damage : 0;
        DamageDoneRequired = tag.TryGet(nameof(DamageDoneRequired), out float damageDoneRequired)
            ? damageDoneRequired
            : 0;
    }

    public override void NetSend(Item item, BinaryWriter writer)
    {
        var dontRead = DamageDone == 0 && DamageDoneRequired == 0;
        writer.Write(dontRead);

        if (dontRead) return;

        writer.Write(DamageDone);
        writer.Write(DamageDoneRequired);
    }

    public override void NetReceive(Item item, BinaryReader reader)
    {
        var dontRead = reader.ReadBoolean();

        if (dontRead) return;

        DamageDone = reader.ReadSingle();
        DamageDoneRequired = reader.ReadSingle();
    }

    public override bool CanStack(Item destination, Item source)
    {
        var ascendantPrefix = ModContent.PrefixType<PrefixAscendant>();
        return destination.prefix != ascendantPrefix && source.prefix != ascendantPrefix;
    }

    public override bool CanUseItem(Item item, Player player)
    {
        PrefixControlled(item, out var canUse);
        return canUse;
    }

    private void PrefixControlled(Item item, out bool canUse)
    {
        if (item.prefix == ModContent.PrefixType<PrefixControlled>())
        {
            if (CurrentBurstCooldown <= 0)
                CurrentBurstCooldown = PrefixBalance.CONTROLLED_BURST_COOLDOWN_TICKS +
                                       PrefixBalance.CONTROLLED_BURST_DURATION_TICKS;

            if (CurrentBurstCooldown <= PrefixBalance.CONTROLLED_BURST_COOLDOWN_TICKS)
            {
                canUse = false;
                return;
            }

            canUse = true;
            return;
        }

        canUse = true;
    }

    public override void PostReforge(Item item)
    {
        DamageDone = 0;
        DamageDoneRequired = item.prefix == ModContent.PrefixType<PrefixAscendant>()
            ? PrefixBalance.GetAscendantDamageRequired(item)
            : 0;
    }

    public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
    {
        if (item.prefix == ModContent.PrefixType<PrefixAscendant>())
        {
            var multiplier = MathHelper.Clamp(DamageDone / DamageDoneRequired, 0f, 1f);
            var damageAdded = multiplier * PrefixBalance.ASCENDANT_RANGED_MAX_DAMAGE;
            damage.Base += damageAdded * item.damage;
            DamageAdded = damageAdded;
        }
    }

    public override void ModifyWeaponCrit(Item item, Player player, ref float crit)
    {
        if (item.prefix == ModContent.PrefixType<PrefixAscendant>())
        {
            var multiplier = MathHelper.Clamp(DamageDone / DamageDoneRequired, 0f, 1f);
            var critAdded = multiplier * PrefixBalance.ASCENDANT_RANGED_MAX_CRIT;
            crit += critAdded;
            CritAdded = critAdded;
        }
    }

    public override void HoldItem(Item item, Player player)
    {
        if (item.prefix == 0) return;

        if (item.prefix == ModContent.PrefixType<PrefixControlled>()) CurrentBurstCooldown--;

        if (item.prefix == ModContent.PrefixType<PrefixVampiric>())
            player.GetModPlayer<GeneralStatPlayer>().Lifesteal += PrefixBalance.VAMPIRIC_LIFESTEAL;
    }
}
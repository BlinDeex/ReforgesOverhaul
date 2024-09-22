using System;
using ModifiersOverhaul.Assets.Buffs;
using ModifiersOverhaul.Assets.Misc;
using Terraria.GameContent.UI;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.ModPlayers.Armor;

public class ArmorAbilityPlayer : ModPlayer
{
    /// <summary>
    /// returns armor ability cooldown in ticks
    /// </summary>
    private Func<int> armorAbility = null;

    public void SetArmorAbility(Func<int> ability)
    {
        armorAbility = ability;
    }

    public override void ResetEffects()
    {
        armorAbility = null;
    }

    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        bool activateArmor = PrefixKeybinds.ArmorActivationKeybind.JustPressed;
        if (activateArmor) ActivateArmorAbility();
    }

    public void ActivateArmorAbility()
    {
        if (Player.HasBuff<ArmorAbilityCooldownBuff>()) return;
        if (armorAbility == null) return;

        int cooldown = armorAbility.Invoke();
        
        Player.AddBuff(ModContent.BuffType<ArmorAbilityCooldownBuff>(), cooldown);
        
        // TODO: play sound
    }
}
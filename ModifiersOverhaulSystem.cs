using System;
using System.Linq;
using System.Reflection;
using ModifiersOverhaul.Assets;
using ModifiersOverhaul.Assets.Balance;
using ModifiersOverhaul.Assets.Buffs;
using ModifiersOverhaul.Assets.InstancedGlobalItems;
using ModifiersOverhaul.Assets.Misc;
using ModifiersOverhaul.Assets.ModPlayers;
using ModifiersOverhaul.Assets.ModPlayers.Armor;
using ModifiersOverhaul.Assets.ModPrefixes.Magic;
using ModifiersOverhaul.Assets.ModPrefixes.Ranged;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace ModifiersOverhaul;

public class ModifiersOverhaulSystem : ModSystem
{
    private static On_Item.orig_CanApplyPrefix origCanApplyPrefix;

    public override void PostSetupContent()
    {
        On_Item.CanApplyPrefix += (orig, self, prefix) =>
        {
            origCanApplyPrefix = orig;
            return CanApplyPrefix(self, prefix);
        };

        SpelunkerEdit();
        
        On_Player.CheckDrowning += (orig, self) =>
        {
            self.breath = self.GetModPlayer<MiscArmorPlayer>().WaterBreathingPrefix();
            orig.Invoke(self);
        };
    }

    private void SpelunkerEdit()
    {
        IL_TileDrawing.DrawSingleTile += TileDrawingSpelunkerEdit;
        IL_TileDrawing.DrawAnimatedTile_AdjustForVisionChangers += TileDrawingSpelunkerEdit;
    }

    private static void TileDrawingSpelunkerEdit(ILContext il)
    {
        var c = new ILCursor(il);
        const MoveType moveType = MoveType.After;
        if (!c.TryGotoNext(moveType, x => x.Match(OpCodes.Ldc_I4, 200)))
            throw new Exception($"Failed DrawAnimatedTileSpelunkerEdit 200 1");
        if (!c.TryGotoNext(moveType, x => x.Match(OpCodes.Ldc_I4, 200)))
            throw new Exception($"Failed DrawAnimatedTileSpelunkerEdit 200 2");

        // red
        c.EmitDelegate((int _) =>
        {
            var revealingTicks = Main.LocalPlayer.GetModPlayer<ToolPlayer>().RevealingTicks;
            if (revealingTicks <= 0) return 200;
            return (int)(200f / PrefixBalance.REVEALING_TICKS * revealingTicks *
                         PrefixBalance.REVEALING_BRIGHTNESS_MUL);
        });

        if (!c.TryGotoNext(moveType, x => x.Match(OpCodes.Ldc_I4, 170)))
            throw new Exception("Failed DrawAnimatedTileSpelunkerEdit 170 1");
        if (!c.TryGotoNext(moveType, x => x.Match(OpCodes.Ldc_I4, 170)))
            throw new Exception("Failed DrawAnimatedTileSpelunkerEdit 170 2");

        // green
        c.EmitDelegate((int _) =>
        {
            var revealingTicks = Main.LocalPlayer.GetModPlayer<ToolPlayer>().RevealingTicks;
            if (revealingTicks <= 0) return 170;
            return (int)(170f / PrefixBalance.REVEALING_TICKS * revealingTicks *
                         PrefixBalance.REVEALING_BRIGHTNESS_MUL);
        });
    }


    private static bool CanApplyPrefix(Item item, int prefix)
    {
        var allowVanillaPrefixes = ModContent.GetInstance<PrefixConfig>().AllowVanillaPrefixes;
        var vanillaPrefix = prefix < PrefixID.Count;
        if (!allowVanillaPrefixes && vanillaPrefix) return false;

        if (vanillaPrefix) return origCanApplyPrefix.Invoke(item, prefix);

        bool isPickaxe = item.pick > 0;
        bool isMinionWeapon = Main.projPet[item.shoot];
        bool isAxe = item.axe > 0;
        bool isHammer = item.hammer > 0;
        bool isWhip = item.DamageType == DamageClass.SummonMeleeSpeed && !Main.projPet[item.shoot];
        
        bool isHeadwear = item.headSlot >= 0;
        bool isChestplate = item.bodySlot >= 0;
        bool isLeggings = item.legSlot >= 0;
        bool isArmor = isHeadwear || isChestplate || isLeggings;

        ModPrefix modPrefix = PrefixLoader.GetPrefix(prefix);

        bool adaptablePrefix = modPrefix.Type == ModContent.PrefixType<PrefixAdaptable>();
        bool rocketWeapon = AdaptableUtils.ROCKET_WEAPON_IDS.Contains(item.type);

        if (adaptablePrefix && rocketWeapon)
        {
            return false; // all rocket weapons shooting normal bullets would be boring and I would need array mapping ALL normal ammo into its projectiles
        }


        if (modPrefix is not ISpecializedPrefix specializedPrefix)
        {
            return !isPickaxe && !isMinionWeapon && !isAxe && !isHammer && !isWhip && !isArmor;
        }
            

        var specializedPrefixType = specializedPrefix.SpecializedPrefixType;

        if (isPickaxe && specializedPrefixType.HasFlag(SpecializedPrefixType.Pickaxe)) return true;
        if (isAxe && specializedPrefixType.HasFlag(SpecializedPrefixType.Axe)) return true;
        if (isHammer && specializedPrefixType.HasFlag(SpecializedPrefixType.Hammer)) return true;
        if (isMinionWeapon && specializedPrefixType.HasFlag(SpecializedPrefixType.MinionWeapon)) return true;
        if (isWhip && specializedPrefixType.HasFlag(SpecializedPrefixType.Whip)) return true;
        if (isHeadwear && specializedPrefixType.HasFlag(SpecializedPrefixType.Headwear)) return true;
        if (isChestplate && specializedPrefixType.HasFlag(SpecializedPrefixType.Chestplate)) return true;
        if (isLeggings && specializedPrefixType.HasFlag(SpecializedPrefixType.Leggings)) return true;

        return false;
    }

    public override void SetupContent()
    {
        On_Item.RollAPrefix += (On_Item.orig_RollAPrefix orig, Item self, UnifiedRandom random, ref int prefix) =>
        {
            var tries = 0;
            while (tries < 1000)
            {
                tries++;
                orig.Invoke(self, random, ref prefix);
                if (CanApplyPrefix(self, prefix)) return true;
            }

            UtilMethods.LogError("SetupContent RollAPrefix detour failed", 101);
            return true;
        };


        On_Player.AddBuff += (orig, self, type, add, quiet, hack) =>
        {
            if (self.HeldItem.prefix == ModContent.PrefixType<PrefixInverted>() && type == BuffID.ManaSickness)
            {
                orig.Invoke(self, ModContent.BuffType<InvertedBuff>(), add, quiet, hack);
                return;
            }

            orig.Invoke(self, type, add, quiet, hack);
        };

        On_Projectile.AI += (orig, self) =>
        {
            if (!self.TryGetGlobalProjectile(out InstancedProjectilePrefix projPrefix) || !projPrefix.Frenzied)
            {
                orig.Invoke(self);
                return;
            }

            for (var i = 0; i < PrefixBalance.FRENZIED_ADDITIONAL_UPDATES; i++) orig.Invoke(self);
        };
        /*
        IL_Item.Prefix += il =>
        {
            var c = new ILCursor(il);

            if (!c.TryGotoNext(MoveType.After, x => x.MatchLdarg1()))
            {
                UtilMethods.LogError("Failed Prefix TryGotoNext ldarg1 1!", 101);
                return;
            }
            if (!c.TryGotoNext(MoveType.Before, x => x.MatchLdarg1()))
            {
                UtilMethods.LogError("Failed Prefix TryGotoNext ldarg1 2!", 101);
                return;
            }

            c.RemoveRange(9);

        };
        */

        var canRollMethod = typeof(PrefixLoader).GetMethod("CanRoll", BindingFlags.Public | BindingFlags.Static);
        MonoModHooks.Modify(canRollMethod, EditCanRoll);
    }

    private void EditCanRoll(ILContext il)
    {
        var c = new ILCursor(il);
        if (!c.TryGotoNext(MoveType.Before, x => x.MatchCall(typeof(Item).GetMethod("GetVanillaPrefixes")!)))
        {
            UtilMethods.LogError("Failed EditCanRoll GetVanillaPrefixes!", 102);
            return;
        }

        c.Index -= 10;

        c.EmitLdcI4(0);
        c.EmitRet();
    }
}
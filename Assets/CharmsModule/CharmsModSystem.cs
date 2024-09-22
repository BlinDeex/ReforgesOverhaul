using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using ModifiersOverhaul.Assets.CharmsModule.Items;
using ModifiersOverhaul.Assets.CharmsModule.Manager;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ID;

namespace ModifiersOverhaul.Assets.CharmsModule;

public class CharmsModSystem : ModSystem
{

    public override void Load()
    {
        /*
        IL_PopupText.NewText_PopupTextContext_Item_int_bool_bool += il =>
        {
            ILCursor c = new(il);
            MoveType moveType = MoveType.After;
            
            c.GotoNext(moveType, x => x.MatchCallvirt<Item>("AffixName"));
            c.GotoNext(moveType, x => x.MatchCallvirt<Item>("AffixName"));
            c.GotoNext(moveType, x => x.MatchCallvirt<Item>("AffixName"));
            
            c.EmitLdarg1();
            c.EmitDelegate<Func<string,Item, string>>((itemName, item) =>
            {
                bool charm = itemName == "Charm";
                if (!charm)
                {
                    Main.NewText($"Not a charm: {itemName}");
                    return itemName;
                }
                Charm charmItem = (Charm)item.ModItem;
                string charmName = CharmBalance.SplitCamelCase((CharmName)charmItem.CharmNameID);
                Main.NewText($"Charm: ID:{charmItem.CharmNameID} Name: {charmName}");
                return charmName;
            });

        };
        */

        On_PopupText.NewText_PopupTextContext_Item_int_bool_bool += (orig, context, item, stack, noStack, text) =>
        {
            if (item.type != ModContent.ItemType<Charm>())
            {
                return orig.Invoke(context, item, stack, noStack, text);
            }
            
            if (!Main.showItemText)
                return -1;

            if (item.Name == null || !item.active)
                return -1;

            if (Main.netMode == NetmodeID.Server)
                return -1;

            Charm charmItem = (Charm)item.ModItem;

            AdvancedPopupRequest req = new()
            {
                Color = charmItem.CharmColor,
                DurationInFrames = 130,
                Text = charmItem.CharmName,
                Velocity = new Vector2(Main.rand.NextFloat() * 5f, -10f)
            };

            PopupText.NewText(req, item.position);
            
            return FindNextItemTextSlot();
        };
    }
    
    private static int FindNextItemTextSlot()
    {
        int num = -1;
        for (int i = 0; i < 20; i++)
        {
            if (Main.popupText[i].active) continue;
            num = i;
            break;
        }

        if (num != -1) return num;
        
        double num2 = Main.bottomWorld;
        for (int j = 0; j < 20; j++) {
            if (num2 > Main.popupText[j].position.Y) {
                num = j;
                num2 = Main.popupText[j].position.Y;
            }
        }

        return num;
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ModifiersOverhaul.Assets.Balance;
using ModifiersOverhaul.Assets.Misc;
using Terraria.ModLoader;

namespace ModifiersOverhaul;

public class ModifiersOverhaul : Mod
{
    public static ModifiersOverhaul Instance { get; private set; }
    
    public override void Load()
    {
        Instance = this;
        SharedLocalization.Load();
        ChaoticRollPool.Load();
    }

    public override void Unload()
    {
        SpriteBatchSnapshotCache.Unload();
    }

    public override void PostSetupContent()
    {
        //Example();
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
        Networking.Instance.HandlePacket(reader, whoAmI);
    }

    public override object Call(params object[] args)
    {
        switch (args.Length)
        {
            case 1:
                if (args[0] is not Dictionary<string, object>)
                    return $"Expected {typeof(Dictionary<string, object>)}, got {args[0].GetType()}";
                return Rebalance((Dictionary<string, object>)args[0]);
            default:
                return $"Invalid amount of args ({args.Length})";
        }
    }

    private static string Rebalance(Dictionary<string, object> values)
    {
        var status = "Success!";

        foreach (var (targetField, targetValue) in values)
        {
            var fieldInfo = typeof(PrefixBalance).GetField(targetField, BindingFlags.Public | BindingFlags.Static);

            if (fieldInfo == null)
            {
                status = $"Field '{targetField}' not found!";
                break;
            }

            var fieldType = fieldInfo.FieldType;
            if (!fieldInfo.FieldType.IsInstanceOfType(targetValue))
                try
                {
                    var convertedValue = Convert.ChangeType(targetValue, fieldType);
                    fieldInfo.SetValue(null, convertedValue);
                }
                catch (Exception ex)
                {
                    status = $"Failed to set value for field '{targetField}': {ex.Message}";
                    break;
                }
            else
                fieldInfo.SetValue(null, targetValue);
        }

        return status;
    }

    private readonly Dictionary<string, object> exampleChanges = new()
    {
        { "ACCESSORY_REFORGING_MULTIPLIER", 2.24f },
        { "WEAPON_REFORGING_MULTIPLIER", "2.24" },
        { "EFFICIENT_MANA_SAVED", 2 }
    };

    private void Example()
    {
        if (ModLoader.TryGetMod("ModifiersOverhaul", out var modifiersOverhaul))
        {
            var result = (string)modifiersOverhaul.Call(exampleChanges);
            Logger.Info(result);
        }
    }
}
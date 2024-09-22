using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using ModifiersOverhaul.Assets.Balance;
using Terraria;
using Terraria.Chat;
using Terraria.Localization;
using Terraria.Utilities;

namespace ModifiersOverhaul.Assets;

public static class UtilMethods
{
    public static string FormatNumber(float number)
    {
        return number switch
        {
            >= 1000000000000 => (number / 1000000000000f).ToString("0.#") + "T",
            >= 1000000000 => (number / 1000000000f).ToString("0.#") + "B",
            >= 1000000 => (number / 1000000f).ToString("0.#") + "M",
            >= 1000 => (number / 1000f).ToString("0.#") + "K",
            _ => number.ToString(CultureInfo.InvariantCulture)
        };
    }
    
    public static bool IsArmor(this Item item)
    {
        return item.headSlot != -1 || item.bodySlot != -1 || item.legSlot != -1;
    }
    
    public static void BroadcastOrNewText(string message, Color color)
    {
        if (Main.dedServ)
        {
            ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(message), color);
            return;
        }
        
        Main.NewText(message, color);
    }
    
    public static byte[] IntListToByteArray(List<int> intList)
    {
        byte[] byteArray = new byte[intList.Count * 4];

        for (int i = 0; i < intList.Count; i++)
        {
            byte[] bytes = BitConverter.GetBytes(intList[i]);
            Buffer.BlockCopy(bytes, 0, byteArray, i * 4, 4);
        }

        return byteArray;
    }
    
    public static Vector3 ColorToVector3(this Color color)
    {
        return new Vector3(color.R, color.G, color.B);
    }
    
    public static List<int> ByteArrayToIntList(byte[] byteArray)
    {
        List<int> intList = [];

        for (int i = 0; i < byteArray.Length; i += 4)
        {
            int value = BitConverter.ToInt32(byteArray, i);
            intList.Add(value);
        }

        return intList;
    }

    public static List<Point> GetConnectedTiles(int startX, int startY, int maxTiles)
    {
        List<Point> connectedTiles = [];
        HashSet<(int x, int y)> visited = [];
        var targetType = Main.tile[startX, startY].TileType;

        Point[] directions =
        [
            new Point(-1, 0), // left
            new Point(1, 0), // right
            new Point(0, -1), // up
            new Point(0, 1) // down
        ];

        var toExplore = new Queue<Point>();
        toExplore.Enqueue(new Point(startX, startY));
        visited.Add((startX, startY));

        while (toExplore.Count > 0 && connectedTiles.Count < maxTiles)
        {
            var currentExplore = toExplore.Dequeue();
            connectedTiles.Add(new Point(currentExplore.X, currentExplore.Y));

            foreach (var direction in directions)
            {
                var newX = currentExplore.X + direction.X;
                var newY = currentExplore.Y + direction.Y;

                if (!IsWithinBounds(newX, newY) || visited.Contains((newX, newY)) ||
                    Main.tile[newX, newY].TileType != targetType) continue;

                toExplore.Enqueue(new Point(newX, newY));
                visited.Add((newX, newY));
            }
        }

        return connectedTiles;
    }

    private static bool IsWithinBounds(int x, int y)
    {
        return x >= 0 && x < Main.maxTilesX && y >= 0 && y < Main.maxTilesY;
    }


    public static Vector2 RandomPointInCircle(float centerX, float centerY, float radius, UnifiedRandom random)
    {
        var angle = random.NextDouble() * 2 * Math.PI;

        var randomRadius = radius * Math.Sqrt(random.NextDouble());

        var x = centerX + randomRadius * Math.Cos(angle);
        var y = centerY + randomRadius * Math.Sin(angle);

        return new Vector2((float)x, (float)y);
    }

    public static Vector2 GetRandomPositionInRectangle(Rectangle rect, UnifiedRandom random)
    {
        var randomX = random.Next(rect.Left, rect.Right);
        var randomY = random.Next(rect.Top, rect.Bottom);
        return new Vector2(randomX, randomY);
    }

    public static TEnum SwapFlag<TEnum>(this TEnum target, TEnum flag) where TEnum : Enum
    {
        if (target.HasFlag(flag)) return (TEnum)(object)((int)(object)target & ~(int)(object)flag);

        return (TEnum)(object)((int)(object)target | (int)(object)flag);
    }


    public static Vector2 RotateVector(Vector2 vector, float radians)
    {
        var cosTheta = MathF.Cos(radians);
        var sinTheta = MathF.Sin(radians);

        var newX = vector.X * cosTheta - vector.Y * sinTheta;
        var newY = vector.X * sinTheta + vector.Y * cosTheta;

        return new Vector2(newX, newY);
    }

    public static void LogError(string message, int errorCode)
    {
        Main.NewText($"{message} error code: {errorCode}", Color.Red);
    }

    public static Rectangle GetCombatTextRect(NPC target)
    {
        return new Rectangle((int)target.position.X, (int)target.position.Y, 20, 20);
    }
}
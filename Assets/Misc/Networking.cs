using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using ModifiersOverhaul.Assets.CharmsModule.Manager;
using ModifiersOverhaul.Assets.ModPlayers;
using ModifiersOverhaul.Assets.ModPlayers.Armor;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModifiersOverhaul.Assets.Misc;

public class Networking : ModSystem
{

    public static Networking Instance;

    public override void Load()
    {
        Instance = this;
    }

    public void HandlePacket(BinaryReader reader, int whoAmI)
    {
        MessageType messageType = (MessageType)reader.ReadByte();

        switch (messageType)
        {
            case MessageType.ChallengerScore:
                byte playerID = reader.ReadByte();
                ChallengerPlayer challengerPlayer = Main.player[playerID].GetModPlayer<ChallengerPlayer>();
                challengerPlayer.ReceivePlayerSync(reader);
                if (Main.netMode is NetmodeID.Server)
                {
                    challengerPlayer.SyncPlayer(-1, whoAmI, false);
                }
                break;
            case MessageType.TrueDamageText:
                int colorPacked = reader.ReadInt32();
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                int damage = reader.ReadInt32();
                int ignorePlayer = reader.ReadInt32();
                //TODO: dramatic and dot doesnt sync
                NetMessage.SendData(MessageID.CombatTextInt, number: colorPacked, number2: x, number3: y, number4: damage, ignoreClient: ignorePlayer);
                break;
            case MessageType.TimeStop:
                byte whoActivated = reader.ReadByte();
                ChronoArmorPlayer.PacketTimeStop(whoActivated);
                break;

            case MessageType.CharmOnKilled:
                Vector2 pos = Vector2.Zero;
                pos.X = reader.ReadSingle();
                pos.Y = reader.ReadSingle();
                bool boss = reader.ReadBoolean();
                var rolls = CharmsManager.RollForCharms(boss: boss);
                CharmsManager.SpawnCharms(rolls, spawnPos: pos);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
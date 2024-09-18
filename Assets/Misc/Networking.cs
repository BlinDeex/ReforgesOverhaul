using System;
using System.IO;
using ModifiersOverhaul.Assets.ModPlayers;
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
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
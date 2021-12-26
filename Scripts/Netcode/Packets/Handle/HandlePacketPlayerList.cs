using Common.Networking.IO;
using Common.Networking.Packet;
using ENet;
using System.Linq;
using KRU.UI;
using Godot;

namespace KRU.Networking
{
    public class HandlePacketPlayerList : HandlePacket
    {
        public override ServerPacketOpcode Opcode { get; set; }

        public HandlePacketPlayerList() => Opcode = ServerPacketOpcode.PlayerList;

        public override void Handle(Event netEvent, PacketReader packetReader)
        {
            var data = new RPacketPlayerList();
            data.Read(packetReader);

            UIGame.Players = data.Players;

            AddPlayersToUserList();
        }

        // The players that are displayed in the "Users" window
        private static void AddPlayersToUserList()
        {
            foreach (var p in UIGame.Players) 
            {
                if (p.Key == UIGame.ClientPlayerId)
                    continue;

                if (UIUsers.uiUsers.ContainsKey(p.Key)) 
                {
                    GD.Print($"WARNING: User ID {p.Key} exists in user list already! (Ignoring)");
                    GD.Print("UIGame.Players");
                    foreach (var a in UIGame.Players)
                        GD.Print($"{a.Key}: {a.Value}");
                    continue;
                }

                UIUsers.AddUser(p.Value, Status.Online, p.Key);
            } 
        }
    }
}
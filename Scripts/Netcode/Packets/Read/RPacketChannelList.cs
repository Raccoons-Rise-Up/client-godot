using Common.Networking.IO;
using Common.Networking.Packet;
using Common.Networking.Message;
using Godot;
using System.Collections.Generic;
using Common.Game;
using KRU.UI;

namespace KRU.Networking
{
    public class RPacketChannelList 
    {
        public Dictionary<uint, Channel> Channels { get; set; }

        public void Read(PacketReader reader)
        {
            // Channels
            Channels = new Dictionary<uint, Channel>();
            var channelCount = reader.ReadUInt16();
            for (int i = 0; i < channelCount; i++)
            {
                var channelId = reader.ReadUInt32();
                var creatorId = reader.ReadUInt32();
                
                var messages = new List<UIMessage>();
                var messageCount = reader.ReadUInt32();
                for (int j = 0; j < messageCount; j++)
                {
                    var messageUserId = reader.ReadUInt32();
                    var messageMessage = reader.ReadString();
                    messages.Add(new UIMessage {
                        UserId = messageUserId,
                        Message = messageMessage
                    });
                }

                var users = new Dictionary<uint, User>();
                var userCount = reader.ReadUInt16();
                for (int j = 0; j < userCount; j++)
                {
                    var userId = reader.ReadUInt32();
                    var userUsername = reader.ReadString();

                    var user = new User(userUsername);
                    user.CreateUIUser(userId);

                    users.Add(userId, user);
                }

                Channels.Add(channelId, new Channel {
                    Users = users,
                    CreatorId = creatorId,
                    Messages = messages
                });
            }
        }
    }
}
using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using System.Net;
//using BuildoLand_CommonClasses;

namespace BuildoLand
{
    public static class Network
    {
        public static string ip;

        public static void Setup()
        {
            Connection.StartListening(ConnectionType.TCP, new IPEndPoint(IPAddress.Any, 0));
            NetworkComms.AppendGlobalIncomingPacketHandler<byte[]>("Chunk", Chunk_ClientSide.RecieveChunk);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("SetBlock", Chunk_ClientSide.RecieveSetBlock);
            NetworkComms.AppendGlobalIncomingPacketHandler<uint>("YourID", Program.player.GetID);
            NetworkComms.AppendGlobalIncomingPacketHandler<byte[]>("PlayerTodo", OtherPlayer.AddTodo);
            NetworkComms.AppendGlobalIncomingPacketHandler<uint>("AddPlayer", AddPlayer);
            NetworkComms.AppendGlobalIncomingPacketHandler<uint>("RemovePlayer", RemovePlayer);           
        }

        public static void Connect(string ip)
        {
            try
            {
                NetworkComms.SendObject("Connect", ip, Options.PORT, true);
                Network.ip = ip;
            }
            catch
            {
                try
                {
                    Network.ip = Dns.GetHostEntry(ip).AddressList[0].ToString();
                    NetworkComms.SendObject("Connect", Network.ip, Options.PORT, true);
                }
                catch
                {
                    Program.Crash("Could not connect");
                }
            }
        }

        public static void AddPlayer(PacketHeader header, Connection connection, uint id)
        {
            Program.otherPlayers.Add(new OtherPlayer(id));
        }

        public static void RemovePlayer(PacketHeader header, Connection connection, uint id)
        {
            OtherPlayer toRemove = null;
            foreach (OtherPlayer o in Program.otherPlayers)
            {
                if (o.id == id)
                {
                    toRemove = o;
                    break;
                }
            }
            if (toRemove != null)
                Program.otherPlayers.Remove(toRemove);
        }
    }
}

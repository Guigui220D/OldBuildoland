using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using System.Net;
using SFML.System;
using BuildoLand_CommonClasses;
//using System.Runtime.Serialization.Formatters.Binary;
//using System.Runtime.Serialization;
//using System.IO;

namespace BuildoLand_Server
{
    static class Program
    {
        static Dictionary<Connection, PlayerInfo> connections = new Dictionary<Connection, PlayerInfo>();

        static Dictionary<Vector2i, Chunk> chunks = new Dictionary<Vector2i, Chunk>();

        static uint nextID = 1;

        public static Random rnd;

        static void Main(string[] args)
        {
            rnd = new Random();
            Connection.StartListening(ConnectionType.TCP, new IPEndPoint(IPAddress.Any, 54321));
            NetworkComms.AppendGlobalIncomingPacketHandler<bool>("Connect", HandleConnect);
            NetworkComms.AppendGlobalIncomingPacketHandler<bool>("Disconnect", HandleDisconnect);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("AskChunk", SendChunk);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("Move", HandleMove);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("SetBlock", SetBlock);
            Console.WriteLine("Server set up on " + Connection.ExistingLocalListenEndPoints(ConnectionType.TCP)[0].ToString());
            while (true)
            {

            }
        }

        static void HandleConnect(PacketHeader header, Connection connection, bool a)
        {
            connections.Add(connection, new PlayerInfo(nextID));
            Console.WriteLine(connection.ConnectionInfo.RemoteEndPoint.ToString() + " connected");
            connection.SendObjectSafe("YourID", nextID);
            nextID++;
            foreach (PlayerInfo p in connections.Values)
            {
                connection.SendObjectSafe("AddPlayer", p.id);     
            }
            foreach (KeyValuePair<Connection, PlayerInfo> p in connections)
            {
                p.Key.SendObjectSafe("AddPlayer", connections[connection].id);
            }
            foreach (PlayerInfo p in connections.Values)
            {
                connection.SendObjectSafe("PlayerTodo", Conversion.ObjectToBytes(new PlayerTodo(p.id, p.position)));
            }
        }
        static void HandleDisconnect(PacketHeader header, Connection connection, bool a)
        {
            Disconnect(connection);
        }
        static void SendChunk(PacketHeader header, Connection connection, string text)
        {
            Vector2i pos = Conversion.StringToVectori(text);
            if (!chunks.ContainsKey(pos))
            {
                Chunk n = new Chunk(pos);
                for (int x = 0; x < 16; x++)
                {
                    for (int y = 0; y < 16; y++)
                    {
                        if (rnd.Next(0, 16) == 0)
                            n.blocks[x, y] = (byte)rnd.Next(1, Blocks.blocksLimit + 1);
                    }
                }
                if (rnd.Next(0, 10) == 0)
                    Structures.GenerateStructure(rnd.Next(0, Structures.Number()), n);
                chunks[pos] = n;
                Console.WriteLine("Chunk " + text + " generated.");
            }
            connection.SendObjectSafe("Chunk", Conversion.ObjectToBytes(chunks[pos]));
        }
        static void HandleMove(PacketHeader header, Connection connection, string pos)
        {
            connections[connection] = new PlayerInfo(connections[connection].id, Conversion.StringToVectori(pos));
            foreach (KeyValuePair<Connection, PlayerInfo> p in connections)
            {
                p.Key.SendObjectSafe("PlayerTodo", Conversion.ObjectToBytes(new PlayerTodo(connections[connection].id, connections[connection].position)));
            }
        }
        static void SetBlock(PacketHeader header, Connection connection, string info)
        {
            int[] data = Conversion.StringToIntArray(info);
            Vector2i world = new Vector2i(data[0], data[1]);
            Vector2i block = Coordinates.WorldToBlock(world);
            Vector2i chunk = Coordinates.WorldToChunk(world);
            if (!chunks.ContainsKey(chunk))
                return;
            chunks[chunk].blocks[block.X, block.Y] = (byte)data[2];
            foreach (Connection co in connections.Keys)
            {
                co.SendObjectSafe("SetBlock", info);
            }
        }

        public static void SendObjectSafe(this Connection co, string text, object obj)
        {
            try
            {
                co.SendObject(text, obj);
            }
            catch
            {
                Disconnect(co);
            }
        }

        private static void Disconnect(uint id)
        {
            Connection toRemove = null;
            foreach (KeyValuePair<Connection, PlayerInfo> co in connections)
            {
                if (co.Value.id == id)
                {
                    toRemove = co.Key;
                    break;
                }
            }
            if (toRemove != null)
                connections.Remove(toRemove);
            foreach (Connection co in connections.Keys)
            {
                co.SendObjectSafe("RemovePlayer", id);
            }
            Console.WriteLine(toRemove.ConnectionInfo.RemoteEndPoint.ToString() + " disconnected");
        }
        private static void Disconnect(Connection co)
        {
            Connection toRemove = null;
            uint id = 0;
            foreach (KeyValuePair<Connection, PlayerInfo> con in connections)
            {
                if (con.Key.ConnectionInfo.RemoteEndPoint.ToString() == co.ConnectionInfo.RemoteEndPoint.ToString())
                {
                    toRemove = con.Key;
                    id = con.Value.id;
                    break;
                }
            }
            if (toRemove != null)
                connections.Remove(toRemove);
            foreach (Connection coe in connections.Keys)
            {
                coe.SendObjectSafe("RemovePlayer", id);
            }
            Console.WriteLine(toRemove.ConnectionInfo.RemoteEndPoint.ToString() + " disconnected");
        }

        public static void SetBlock(Vector2i pos, byte block)
        {
            
        }
    }

    public struct PlayerInfo
    {
        public Vector2i position;
        public uint id;

        public PlayerInfo(uint id)
        {
            position = new Vector2i();
            this.id = id;
        }

        public PlayerInfo(uint id, Vector2i pos)
        {
            position = pos;
            this.id = id;
        }
    }
}

using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using BuildoLand_CommonClasses;
using SFML.System;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
//using System.Runtime.Serialization.Formatters.Binary;
//using System.Runtime.Serialization;
//using System.IO;

namespace BuildoLand
{
    public static class Chunk_ClientSide
    {
        public static Dictionary<Vector2i, Chunk> loadedChunks = new Dictionary<Vector2i, Chunk>();
        public static List<Vector2i> askedChunks = new List<Vector2i>();
        
        public static byte GetBlock(Vector2i pos)
        {
            Vector2i chunk = Coordinates.WorldToChunk(pos);
            if (!loadedChunks.ContainsKey(chunk))
            {
                AskChunk(chunk);
                return 255;
            }
            else
            {
                Vector2i block = Coordinates.WorldToBlock(pos);
                Chunk c = loadedChunks[chunk];
                if (c == null)
                {
                    loadedChunks.Remove(pos);
                    return 255;
                }
                return c.blocks[block.X, block.Y];
            }
        }

        public static void SetBlock(Vector2i pos, byte block)
        {
            NetworkComms.SendObject("SetBlock", Network.ip, Options.PORT, Conversion.IntArrayToString(new int[] { pos.X, pos.Y, block }));
        }

        public static void RecieveSetBlock(PacketHeader header, Connection connection, string info)
        {
            int[] data = Conversion.StringToIntArray(info);
            Vector2i world = new Vector2i(data[0], data[1]);
            Vector2i block = Coordinates.WorldToBlock(world);
            Vector2i chunk = Coordinates.WorldToChunk(world);
            if (!loadedChunks.ContainsKey(chunk))
                return;
            loadedChunks[chunk].blocks[block.X, block.Y] = (byte)data[2];
        }

        private static void AskChunk(Vector2i chunk)
        {
            if (askedChunks.Contains(chunk))
                return;
            NetworkComms.SendObject("AskChunk", Network.ip, Options.PORT, Conversion.VectoriToString(chunk));
            askedChunks.Add(chunk);
            Console.WriteLine("Asked for chunk " + chunk);
            return;
        }

        public static void RecieveChunk(PacketHeader header, Connection connection, byte[] data)
        {
            Chunk obj = Conversion.BytesToObject<Chunk>(data);
            if (obj != null)
            {
                loadedChunks[new Vector2i(obj.posX, obj.posY)] = obj;
                Console.WriteLine("Recieved chunk " + new Vector2i(obj.posX, obj.posY));
                askedChunks.Clear();
            }     
        }
    }
}

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using SFML.System;
using System.Runtime.Serialization.Formatters.Binary;
//using System.Runtime.Serialization;
using System.IO;

namespace BuildoLand_CommonClasses
{
    public static class Conversion
    {
        public static Vector2i StringToVectori(string text)
        {
            string[] coords = text.Split(',');
            return new Vector2i(int.Parse(coords[0]), int.Parse(coords[1]));
        }

        public static string VectoriToString(Vector2i v)
        {
            return "" + v.X + "," + v.Y;
        }

        public static int[] StringToIntArray(string text)
        {
            string[] data = text.Split(',');
            int[] ret = new int[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                ret[i] = int.Parse(data[i]);
            }
            return ret;
        }

        public static string IntArrayToString(int[] data)
        {
            string ret = data[0].ToString();
            for (int i = 1; i < data.Length; i++)
            {
                ret += "," + data[i];
            }
            return ret;
        }

        public static byte[] ObjectToBytes(object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static T BytesToObject<T>(byte[] bytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(bytes, 0, bytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            T obj = (T)binForm.Deserialize(memStream);
            return obj;
        }
    }
}

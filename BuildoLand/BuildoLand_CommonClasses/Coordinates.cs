using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using SFML.System;

namespace BuildoLand_CommonClasses
{
    public static class Coordinates
    {
        public static Vector2i WorldToChunk(Vector2i pos)
        {
            if (pos.X < 0)
                pos -= new Vector2i(16, 0);
            if (pos.Y < 0)
                pos -= new Vector2i(0, 16);
            return pos / 16;
        }

        public static Vector2i WorldToBlock(Vector2i pos)
        {
            return Abs(new Vector2i(pos.X % 16, pos.Y % 16));
        }

        public static Vector2i BlockToWorld(Vector2i pos, Vector2i chunk)
        {
            return new Vector2i(pos.X + chunk.X * 16, pos.Y + chunk.Y * 16);
        }      

        public static Vector2i Abs(Vector2i v)
        {
            return new Vector2i(Math.Abs(v.X), Math.Abs(v.Y));
        }

        public static Vector2f FtoI(Vector2i v)
        {
            return new Vector2f(v.X, v.Y);
        }

        public static Vector2f Floor(Vector2f v)
        {
            return new Vector2f((int)Math.Floor(v.X), (int)Math.Floor(v.Y));
        }
    }
}

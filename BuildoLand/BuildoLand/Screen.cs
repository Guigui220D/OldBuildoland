//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using SFML.System;
using BuildoLand_CommonClasses;

namespace BuildoLand
{
    public static class Screen
    {
        public static Vector2f WorldToScreen(Vector2i pos)
        {
            return Coordinates.Floor(new Vector2f(pos.X * Options.squareSize, pos.Y * Options.squareSize));
        }

        public static Vector2f WorldToScreen(Vector2f pos)
        {
            return Coordinates.Floor(new Vector2f(pos.X * Options.squareSize, pos.Y * Options.squareSize));
        }
    }
}

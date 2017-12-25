using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.IO;
using VarSave;
using SFML.System;

namespace BuildoLand
{
    public static class Options
    {
        public const string VERSION = "Bêta 1.1";    //The version name
        public const string OPTIONS_PATH = "options\\";
        public const int DISPLAY_SIZE = 16;  //The number of blocs displayed on the screen
        public const int PORT = 54321;

        public static int squareSize;   //The size of one block on the screen
        public const int DEFAULT_SQUARE_SIZE = 60;
        public static float blockQuart;
        public static float blockHalf;
        public static float blockThirdQuart;

        public static Vector2f GetWindowSize()
        {
            return new Vector2f(squareSize * DISPLAY_SIZE, squareSize * DISPLAY_SIZE);
        }

        public static void Load()
        {
            if (!Directory.Exists(OPTIONS_PATH))
                Directory.CreateDirectory(OPTIONS_PATH);
            if (!File.Exists(OPTIONS_PATH + "square_size"))
                VSave.WriteToFile(OPTIONS_PATH + "square_size", DEFAULT_SQUARE_SIZE);
            squareSize = VSave.LoadFromFile<int>(OPTIONS_PATH + "square_size");
            blockQuart = squareSize / 4;
            blockHalf = squareSize / 2;
            blockThirdQuart = blockQuart + blockHalf; ;
        }
    }
}

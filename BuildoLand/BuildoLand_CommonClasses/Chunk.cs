using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using SFML.System;

namespace BuildoLand_CommonClasses
{
    [Serializable]
    public class Chunk
    {
        public int posX { private set; get; }
        public int posY { private set; get; }
        public uint world { private set; get; }
        public byte[,] blocks; 
        
        public Chunk(Vector2i pos)
        {
            posX = pos.X;
            posY = pos.Y;
            world = 0;
            blocks = new byte[16, 16];
        }  
    }
}

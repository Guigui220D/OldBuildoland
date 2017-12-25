using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using BuildoLand_CommonClasses;

namespace BuildoLand_Server
{
    public static class Structures
    {
        private static byte[][,] STRUCTURES =
        {
            new byte[,]{
                {2,2,7,7,7,2,2},
                {2,0,0,0,0,0,2},
                {2,0,0,0,0,0,2},
                {2,0,0,0,0,0,7},
                {2,0,0,0,0,0,7},
                {2,0,0,0,0,0,2},
                {2,0,0,0,0,0,2},
                {2,2,2,5,2,2,2},
            },
            new byte[,]{
                {2,0,0,2,6,2,2},
                {2,0,2,0,0,0,2},
                {0,2,0,0,0,0,0},
                {0,0,0,0,0,0,6},
                {7,0,0,0,0,0,7},
                {2,0,0,0,4,0,2},
                {2,2,0,0,0,0,0},
                {2,0,2,0,2,2,0},
            },
            new byte[,]{
                {9,9,0},
                {9,5,9},
                {0,9,9},
            },
            new byte[,]{
                {2,0,2,2,2,2,2,2,2,2,2},
                {2,0,0,0,0,0,2,0,2,0,2},
                {2,2,0,2,0,2,2,0,2,0,2},
                {2,0,0,2,0,0,0,0,2,0,2},
                {2,0,0,2,2,2,2,2,2,0,2},
                {2,0,0,0,0,0,0,0,0,0,2},
                {2,0,2,0,2,2,2,2,2,2,2},
                {2,0,0,0,0,0,2,0,0,0,2},
                {2,0,2,0,2,0,0,0,2,0,2},
                {2,2,2,2,2,2,2,2,2,1,2},
            }
        };

        public static void GenerateStructure(int structure, Chunk c)
        {
            for (int x = 0; x < STRUCTURES[structure].GetLength(0); x++)
            {
                for (int y = 0; y < STRUCTURES[structure].GetLength(1); y++)
                {
                    c.blocks[x, y] = STRUCTURES[structure][x, y];
                }
            }
            Console.WriteLine("Structure generated");
        }

        public static int Number()
        {
            return STRUCTURES.Length;
        }
    }
}

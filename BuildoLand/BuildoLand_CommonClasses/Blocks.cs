using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildoLand_CommonClasses
{
    public static class Blocks
    {
        public static int GetTexture(byte block)
        {
            if (block / 2 >= blockTypes)
                return 0;
            return block / 2;
        }

        public static bool IsBlock(byte block)
        {
            return (block <= blocksLimit);
        }

        public static Types GetType(byte block)
        {
            if (block == 0)
                return Types.Air;
            if (block == 1)
                return Types.Hole;
            if (block % 2 == 0)
            {
                return Types.Block;
            }
            else
            {
                return Types.Wall;
            }
        }

        public static string TypeString(Types type)
        {
            switch (type)
            {
                case Types.Air:
                    return "Air";
                case Types.Hole:
                    return "Dig";
                case Types.Block:
                    return "Block";
                case Types.Wall:
                    return "Wall";
            }
            return "error";
        }

        public const int blockTypes = 10;
        public const int blocksLimit = blockTypes * 2 - 1;

        public enum Types
        {
            Air,
            Hole,
            Block,
            Wall
        };
    }
}

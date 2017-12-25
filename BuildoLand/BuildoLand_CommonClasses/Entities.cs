using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using SFML.System;

namespace BuildoLand_CommonClasses
{
    [Serializable]
    public struct PlayerTodo
    {
        public uint id;
        public string moveTo;

        public PlayerTodo(uint id, Vector2i pos)
        {
            this.id = id;
            moveTo = Conversion.VectoriToString(pos);
        }
    }
}

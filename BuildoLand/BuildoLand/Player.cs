//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using SFML.System;
using SFML.Graphics;
using SFML.Window;
using BuildoLand_CommonClasses;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
//using System.Threading;

namespace BuildoLand
{
    public class Player : Entity
    {
        public byte itemInHand = 1;
        public uint ressources = 12;

        public Player() : base(0)
        {
            Texture = new Texture("ressources\\textures\\players\\robot.png");
            FillColor = new Color(200, 127, 127);
        }

        public override void Act()
        {
            if (moving)
            {
                Position = Screen.WorldToScreen(new Vector2f(lastPosition.X + ((newPosition.X - lastPosition.X) * (animation.ElapsedTime.AsSeconds() / animDuration)), lastPosition.Y + ((newPosition.Y - lastPosition.Y) * (animation.ElapsedTime.AsSeconds() / animDuration))));
                if (animation.ElapsedTime.AsSeconds() >= animDuration)
                {
                    moving = false;
                    position = newPosition;
                }
            }
            else
            {
                if (Program.gameWindow.HasFocus())
                {
                    Vector2i move = new Vector2i();
                    if (Keyboard.IsKeyPressed(Keyboard.Key.Right))
                    {
                        move += new Vector2i(1, 0);
                        goto endKeys;
                    }
                    if (Keyboard.IsKeyPressed(Keyboard.Key.Down))
                    {
                        move += new Vector2i(0, 1);
                        goto endKeys;
                    }
                    if (Keyboard.IsKeyPressed(Keyboard.Key.Left))
                    {
                        move += new Vector2i(-1, 0);
                        goto endKeys;
                    }
                    if (Keyboard.IsKeyPressed(Keyboard.Key.Up))
                    {
                        move += new Vector2i(0, -1);
                        goto endKeys;
                    }
                endKeys:
                    if (move != new Vector2i())
                        MoveTo(position + move);
                }
                Position = Screen.WorldToScreen(new Vector2f(position.X, position.Y));
            }
        }

        public override void MoveTo(Vector2i newPos)
        {
            if (Chunk_ClientSide.GetBlock(newPos) != 0 && Chunk_ClientSide.GetBlock(newPos) != 255)
                return;
            base.MoveTo(newPos);
            NetworkComms.SendObject("Move", Network.ip, Options.PORT, Conversion.VectoriToString(newPos));
        }

        public void GetID(PacketHeader header, Connection connection, uint id)
        {
            this.id = id;
        }
    }

    public class OtherPlayer : Entity
    {
        public OtherPlayer(uint id) : base(id)
        {
            Texture = new Texture("ressources\\textures\\players\\robot.png");
        }

        public override void Act()
        {
            if (moving)
            {
                Position = Screen.WorldToScreen(new Vector2f(lastPosition.X + ((newPosition.X - lastPosition.X) * (animation.ElapsedTime.AsSeconds() / animDuration)), lastPosition.Y + ((newPosition.Y - lastPosition.Y) * (animation.ElapsedTime.AsSeconds() / animDuration))));
                if (animation.ElapsedTime.AsSeconds() >= animDuration)
                {
                    moving = false;
                }
            }
            else
            {
                Position = Screen.WorldToScreen(new Vector2f(position.X, position.Y));
            }
        }

        public override void MoveTo(Vector2i newPos)
        {
            position = newPosition;
            animation.Restart();
            lastPosition = position;
            animation.Restart();
            moving = true;
            newPosition = newPos;
        }

        public static void AddTodo(PacketHeader header, Connection connection, byte[] data)
        {
            PlayerTodo todo = Conversion.BytesToObject<PlayerTodo>(data);
            foreach (OtherPlayer p in Program.otherPlayers)
            {
                if (p.id == todo.id)
                    p.MoveTo(Conversion.StringToVectori(todo.moveTo));
            }
        }
    }

    public abstract class Entity : RectangleShape
    {
        public uint id { get; protected set; }

        public Vector2i position;

        protected Vector2i lastPosition;
        public Vector2i newPosition;
        protected bool moving;

        protected const float animDuration = 0.2f;
        protected Clock animation;

        public Entity(uint id) : base()
        {
            this.id = id;
            animation = new Clock();
            Size = new Vector2f(Options.squareSize, Options.squareSize);
            position = new Vector2i();
            lastPosition = position;
            Position = Screen.WorldToScreen(position);
            moving = false;
        }

        public virtual void MoveTo(Vector2i newPos)
        {          
            animation.Restart();
            lastPosition = position;
            animation.Restart();
            moving = true;
            newPosition = newPos;    
        }

        public abstract void Act();
    }
}

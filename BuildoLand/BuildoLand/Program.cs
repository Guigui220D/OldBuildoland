using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using VarSave;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using BuildoLand_CommonClasses;
using System.Threading;
using NetworkCommsDotNet;
//using NetworkCommsDotNet.Connections;

namespace BuildoLand
{
    static class Program
    {
        public static Random rnd = new Random();

        public static RenderWindow gameWindow;
        static View view = new View();
        static View gui = new View();

        public static bool writing = false;
        static string enter = "";

        static bool crashed = false;

        public static List<OtherPlayer> otherPlayers;

        public static Player player;

        static ConvexShape wall;
        static RectangleShape block;
        static RectangleShape hole;
        static RectangleShape ground;
        static RectangleShape notLoaded;
        static RectangleShape screen;
        static RectangleShape selectedBlock;
        static Text blockMode;
        static Text ressourcesCount;

        public static Texture[] textures;
        static Texture groundT;
        static Texture holeT;
        static Texture back = new Texture("ressources\\buildoland.PNG");
        static Texture screenShadow;
        static Texture shovel;
        static Font vgafix;

        static void Main(string[] args)
        {
            Options.Load();
            LoadFonts();
            SetupWindow();
            Clock clock = new Clock();
            using (Text load = new Text())
            {
                RectangleShape backr = new RectangleShape();
                backr.Texture = back;
                backr.Size = Options.GetWindowSize();
                backr.Position = new Vector2f(0, 0);
                load.Color = Color.Black;
                load.Font = vgafix;
                load.DisplayedString = "Loading...";
                load.Position = new Vector2f(20, 100);
                load.Scale = new Vector2f(1, 1);
                Text title = new Text();
                title.Font = vgafix;
                title.Position = new Vector2f(20, 20);
                title.DisplayedString = "Buildoland ©";
                title.CharacterSize = (uint)(load.CharacterSize * 1.5);
                title.Color = Color.White;
                Text trm = new Text();
                trm.Font = vgafix;
                trm.Position = new Vector2f(20, Options.GetWindowSize().Y - 40);
                trm.DisplayedString = "Guillaume Derex - Buildoland, tous droits réservés 2017";
                trm.Color = Color.Black;
                trm.CharacterSize = (uint)(load.CharacterSize * 0.6);
                gameWindow.Clear(Color.Black);
                gameWindow.Draw(backr);
                gameWindow.Draw(load);
                gameWindow.Draw(title);
                gameWindow.Draw(trm);
                gameWindow.Display();
                LoadTextures();
                LoadSounds();
                SetupAssets();
                Thread.Sleep(500);
                load.DisplayedString = "Please enter server's ip : ";               
                player = new Player();
                otherPlayers = new List<OtherPlayer>();
                Network.Setup();
                enter = "";
                writing = true;
                string underscore;                
                while (!Keyboard.IsKeyPressed(Keyboard.Key.Return) || !gameWindow.HasFocus())
                {
                    gameWindow.DispatchEvents();
                    underscore = "";
                    if ((int)(clock.ElapsedTime.AsSeconds() * 2) % 2 == 0)
                        underscore += "_";
                    load.DisplayedString = "Please enter server's ip : \n\n" + enter + underscore;
                    gameWindow.Clear(Color.Black);
                    gameWindow.Draw(backr);
                    gameWindow.Draw(load);
                    gameWindow.Draw(title);
                    gameWindow.Draw(trm);
                    gameWindow.Display();
                }
                writing = false;
                Console.WriteLine("\"" + enter + "\"");
                Network.Connect(enter);
            }
            gameWindow.Clear(Color.Black);
            gameWindow.Display();
            SetupCamera();
            Console.WriteLine("Waiting for id assign...");
            while (player.id == 0) { };
            Console.WriteLine("Got id " + player.id);
            float delta = 0.0f;
            while (gameWindow.IsOpen)
            {
                gameWindow.DispatchEvents();
                delta = clock.Restart().AsSeconds();
                Update(delta);
                DrawAll();
            }   //The main loop
            Environment.Exit(0);
        }
      
        static void LoadTextures()
        {
            textures = new Texture[Blocks.blockTypes];
            for (int i = 0; i < textures.Length - 1; i++)
                textures[i + 1] = new Texture("ressources\\textures\\block" + i +".png");
            textures[0] = new Texture("ressources\\textures\\unknown.png");
            groundT = new Texture("ressources\\textures\\grass.png");
            holeT = new Texture("ressources\\textures\\hole.png");
            screenShadow = new Texture("ressources\\textures\\screenShadow.png");
            shovel = new Texture("ressources\\textures\\shovel.png");
        }   //Load all textures from ressources/sounds/
        static void LoadSounds()
        {

        }   //Load all sounds from ressources/sounds/
        static void SetupAssets()
        {
            block = new RectangleShape();
            block.Size = new Vector2f(Options.squareSize, Options.squareSize);
            block.OutlineThickness = 2;
            block.OutlineColor = Color.Black;

            wall = new ConvexShape();
            wall.SetPointCount(8);
            wall.OutlineThickness = 2;
            wall.OutlineColor = Color.Black;

            hole = new RectangleShape();
            hole.Size = new Vector2f(Options.squareSize, Options.squareSize);
            hole.Texture = holeT;

            ground = new RectangleShape();
            ground.Size = new Vector2f(Options.squareSize, Options.squareSize);
            ground.Texture = groundT;

            notLoaded = new RectangleShape();
            notLoaded.Size = new Vector2f(Options.squareSize,  Options.squareSize);
            notLoaded.FillColor = Color.Magenta;

            //Gui assets

            screen = new RectangleShape();
            screen.Size = new Vector2f(Options.squareSize * Options.DISPLAY_SIZE, Options.squareSize * Options.DISPLAY_SIZE);
            screen.Texture = screenShadow;

            selectedBlock = new RectangleShape();
            selectedBlock.Size = new Vector2f(64, 64);
            selectedBlock.Position = Options.GetWindowSize() - new Vector2f(80, 80);

            blockMode = new Text();
            blockMode.Font = vgafix;
            blockMode.Position = Options.GetWindowSize() - new Vector2f(80, 80);
            blockMode.DisplayedString = "Dig";
            blockMode.CharacterSize = 22;
            //blockMode.Color = Color.Black;

            ressourcesCount = new Text();
            ressourcesCount.Font = vgafix;
            ressourcesCount.Position = new Vector2f(10, Options.GetWindowSize().Y - 80);
            ressourcesCount.DisplayedString = "Ressources : ";
            //ressourcesCount.Color = Color.Black;

        }   //Setup of the drawing rectangles
        static void SetupWindow()
        {
            gameWindow = new RenderWindow(new VideoMode((uint)(Options.DISPLAY_SIZE * Options.squareSize), (uint)(Options.DISPLAY_SIZE * Options.squareSize)), "BuildoLand - " + Options.VERSION, Styles.Close);
            gameWindow.SetActive(false);           
            gameWindow.Closed += GameWindow_Closed;
            gameWindow.LostFocus += GameWindow_LostFocus;
            gameWindow.KeyPressed += GameWindow_KeyPressed;
            gameWindow.TextEntered += GameWindow_TextEntered;
        }   //The setup of the display window
        static void SetupCamera()
        {
            view.Size = new Vector2f(gameWindow.Size.X, gameWindow.Size.Y);
            gui = gameWindow.DefaultView;
            gameWindow.SetView(view);
            
        }
        static void LoadFonts()
        {
            vgafix = new Font("ressources\\fonts\\VCR_OSD_MONO.ttf");
        }   //Load fonts from ressources/fonts/

        static void Update(float delta)
        {
            view.Center = player.Position + new Vector2f(Options.squareSize / 2, Options.squareSize / 2);
            gameWindow.SetView(view);
            player.Act();
            foreach (OtherPlayer pl in otherPlayers)
            {
                if (pl != null)
                    pl.Act();
            }
        }   //Do all physics stuff
        static void DrawAll()
        {
            gameWindow.Clear(Color.Magenta);
            gameWindow.SetView(view);
            DrawWorld();
            foreach (OtherPlayer pl in otherPlayers)
            {
                if (pl.id != player.id)
                    gameWindow.Draw(pl);
            }
            gameWindow.Draw(player);
            gameWindow.SetView(gui);
            gameWindow.Draw(screen);
            switch (player.itemInHand)
            {
                case 1:
                    selectedBlock.Texture = shovel;
                    blockMode.Color = Color.Black;
                    break;
                default:
                    selectedBlock.Texture = textures[Blocks.GetTexture(player.itemInHand)];
                    blockMode.Color = Color.White;
                    break;
            }
            blockMode.DisplayedString = Blocks.TypeString(Blocks.GetType(player.itemInHand));
            ressourcesCount.DisplayedString = "Ressources : " + player.ressources;
            gameWindow.Draw(selectedBlock);
            gameWindow.Draw(blockMode);
            gameWindow.Draw(ressourcesCount);
            gameWindow.Display();
        }   //Do all drawing stuff
        static void DrawWorld()
        {
            for (int y = -11; y < 12; y++)
            {
                for (int x = -11; x < 12; x++)
                {
                    Vector2i pos = new Vector2i(player.position.X + x, player.position.Y + y);
                    Vector2f drawPos = Screen.WorldToScreen(pos);
                    ground.Position = drawPos;
                    gameWindow.Draw(ground);
                }
            }
            for (int y = -11; y < 12; y++)
            {
                for (int x = -11; x < 12; x++)
                {
                    Vector2i pos = new Vector2i(player.position.X + x, player.position.Y + y);
                    byte bl = Chunk_ClientSide.GetBlock(pos);
                    if (bl == 0)
                        continue;
                    Vector2f drawPos = Screen.WorldToScreen(pos);
                    if (Blocks.GetType(bl) == Blocks.Types.Hole)
                    {
                        hole.Position = drawPos;
                        gameWindow.Draw(hole);
                        continue;
                    }
                    if (Blocks.GetType(bl) == Blocks.Types.Block)
                    {
                        block.Position = drawPos;
                        block.Texture = textures[Blocks.GetTexture(bl)];
                        gameWindow.Draw(block);
                        continue;
                    }
                    else
                    {

                        if (Chunk_ClientSide.GetBlock(pos + new Vector2i(1, 0)) > 1)
                        {
                            wall.SetPoint(0, new Vector2f(Options.squareSize, Options.blockQuart));
                            wall.SetPoint(1, new Vector2f(Options.squareSize, Options.blockThirdQuart));
                        }
                        else
                        {
                            wall.SetPoint(0, new Vector2f(Options.blockThirdQuart, Options.blockQuart));
                            wall.SetPoint(1, new Vector2f(Options.blockThirdQuart, Options.blockThirdQuart));
                        }
                        if (Chunk_ClientSide.GetBlock(pos + new Vector2i(0, 1)) > 1)
                        {
                            wall.SetPoint(2, new Vector2f(Options.blockThirdQuart, Options.squareSize));
                            wall.SetPoint(3, new Vector2f(Options.blockQuart, Options.squareSize));
                        }
                        else
                        {
                            wall.SetPoint(2, new Vector2f(Options.blockThirdQuart, Options.blockThirdQuart));
                            wall.SetPoint(3, new Vector2f(Options.blockQuart, Options.blockThirdQuart));
                        }
                        if (Chunk_ClientSide.GetBlock(pos + new Vector2i(-1, 0)) > 1)
                        {
                            wall.SetPoint(4, new Vector2f(0, Options.blockThirdQuart));
                            wall.SetPoint(5, new Vector2f(0, Options.blockQuart));
                        }
                        else
                        {
                            wall.SetPoint(4, new Vector2f(Options.blockQuart, Options.blockThirdQuart));
                            wall.SetPoint(5, new Vector2f(Options.blockQuart, Options.blockQuart));
                        }
                        if (Chunk_ClientSide.GetBlock(pos + new Vector2i(0, -1)) > 1)
                        {
                            wall.SetPoint(6, new Vector2f(Options.blockQuart, 0));
                            wall.SetPoint(7, new Vector2f(Options.blockThirdQuart, 0));
                        }
                        else
                        {
                            wall.SetPoint(6, new Vector2f(Options.blockQuart, Options.blockQuart));
                            wall.SetPoint(7, new Vector2f(Options.blockThirdQuart, Options.blockQuart));
                        }
                        wall.Position = drawPos;
                        wall.Texture = textures[Blocks.GetTexture(bl)];
                        gameWindow.Draw(wall);
                        continue;
                    }                   
                }
            }
        }   //Draw near blocksLimit

        public static void Crash(string info)
        {
            RectangleShape rs = new RectangleShape();
            rs.Texture = new Texture("ressources\\crash.png");
            rs.Size = Options.GetWindowSize();
            gameWindow.SetView(gameWindow.DefaultView);
            Text message = new Text();
            message.Font = vgafix;
            message.Position = new Vector2f(20, 320);
            message.DisplayedString = "> " + info;
            Text crash = new Text();
            crash.Font = vgafix;
            crash.Position = new Vector2f(20, 20);
            crash.DisplayedString = "Your game crashed :/";
            crash.CharacterSize = (uint)(message.CharacterSize * 1.5);
            crashed = true;
            while (gameWindow.IsOpen)
            {
                gameWindow.DispatchEvents();
                gameWindow.Clear(Color.Black);
                gameWindow.Draw(rs);
                gameWindow.Draw(message);
                gameWindow.Draw(crash);
                gameWindow.Display();
            }
            Environment.Exit(-1);
        }

        private static void GameWindow_KeyPressed(object sender, KeyEventArgs e)
        {
            if (crashed)
                return;
            if (writing && gameWindow.HasFocus())
            {
                if (e.Code == Keyboard.Key.BackSpace && enter.Length > 0)
                    enter = enter.Remove(enter.Length - 1);
                return;
            }
            switch (e.Code)
            {
                case Keyboard.Key.Subtract:
                    player.itemInHand -= 1;
                    if (player.itemInHand == 0)
                        player.itemInHand = 1;
                    break;
                case Keyboard.Key.Add:
                    player.itemInHand += 1;
                    if (!Blocks.IsBlock(player.itemInHand))
                        player.itemInHand = 1;
                    break;                
                case Keyboard.Key.D:
                    if (Chunk_ClientSide.GetBlock(player.newPosition + new Vector2i(1, 0)) == 0)
                    {
                        Chunk_ClientSide.SetBlock(player.newPosition + new Vector2i(1, 0), player.itemInHand);
                    }
                    else
                    {
                        Chunk_ClientSide.SetBlock(player.newPosition + new Vector2i(1, 0), 0);
                        player.ressources++;
                    }
                    break;
                case Keyboard.Key.S:
                    if (Chunk_ClientSide.GetBlock(player.newPosition + new Vector2i(0, 1)) == 0)
                    {
                        Chunk_ClientSide.SetBlock(player.newPosition + new Vector2i(0, 1), player.itemInHand);
                    }
                    else
                    {
                        Chunk_ClientSide.SetBlock(player.newPosition + new Vector2i(0, 1), 0);
                        player.ressources++;
                    }
                    break;
                case Keyboard.Key.Q:
                    if (Chunk_ClientSide.GetBlock(player.newPosition + new Vector2i(-1, 0)) == 0)
                    {
                        Chunk_ClientSide.SetBlock(player.newPosition + new Vector2i(-1, 0), player.itemInHand);
                    }
                    else
                    {
                        Chunk_ClientSide.SetBlock(player.newPosition + new Vector2i(-1, 0), 0);
                    }
                    break;
                case Keyboard.Key.Z:
                    if (Chunk_ClientSide.GetBlock(player.newPosition + new Vector2i(0, -1)) == 0)
                    {
                        Chunk_ClientSide.SetBlock(player.newPosition + new Vector2i(0, -1), player.itemInHand);
                    }
                    else
                    {
                        Chunk_ClientSide.SetBlock(player.newPosition + new Vector2i(0, -1), 0);
                    }
                    break;
            }
        }
        private static void GameWindow_LostFocus(object sender, EventArgs e)
        {
            //TODO
        }
        private static void GameWindow_Closed(object sender, EventArgs e)
        {
            try
            {
                NetworkComms.SendObject("Disconnect", Network.ip, Options.PORT, true);
            }
            catch { }
            gameWindow.Close();
            Environment.Exit(0);
        }
        private static void GameWindow_TextEntered(object sender, TextEventArgs e)
        {
            if (writing)
            {
                string add = "";
                foreach (char c in e.Unicode)
                    if (char.IsLetterOrDigit(c) || char.IsPunctuation(c))
                        add += c;
                enter += add;
            }
        }
    }
}

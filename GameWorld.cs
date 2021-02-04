using System;
using System.Collections.Generic;
using System.Linq;

namespace Snake
{
    public sealed class GameWorld
    {
        public List<GameObject> GameObjects = new List<GameObject>();
        public PhysicsEngine2D.Collision2D collisionDetection = new();
        readonly Timer Timer = Timer.TimerInstance;

        public Vector2D TopLeftCornerPos { get; set; }
        public Vector2D BottomRightCornerPos { get; set; }
        public int Height { get; private set; }
        public int Width { get; private set; }
        public int Score { get; set; } = 0;

        // This is my very first time using a singleton.
        // I ams so excited
        private GameWorld() { }
        private static readonly Lazy<GameWorld> Game = new Lazy<GameWorld>(() => new GameWorld());
        public static GameWorld GameWorldInstance
        {
            get
             {
                return Game.Value;
            }
        }

        /// <summary>
        /// Resets the game to factory defaults
        /// </summary>
        public void ResetGame()
        {
            Console.Clear();
            Timer.GameEnd = true;
            Console.SetCursorPosition(0, 3);
            Console.WriteLine("Exiting game...");
            // Thread.Sleep will block the current thread x amount of milliseconds
            System.Threading.Thread.Sleep(1200);
            Console.SetCursorPosition(0, 4);
            Console.WriteLine(":(");
            GameObjects.Clear();
        }

        /// <summary>
        /// Renders the player on the gameWorld
        /// </summary>
        public void RenderPlayer()
        {
            // Select all players that exists (Including AI)
            IEnumerable<Player> selectedPlayers = from p in GameObjects
                                                 where p.GetObjectType() == ObjectType.Player
                                                 select (Player)p;

            // If we have multiple players (including AI), this will be useful for rendering all this.
            // Otherwise, the foreach loop is just resource consuming
            foreach(Player player in selectedPlayers.ToList())
            {
                // Clear the old player and body positions
                Console.SetCursorPosition(player.OldPosition.Y, player.OldPosition.X);
                Console.Write(" ");
                foreach (BodyPart bodyPart in player.BodyParts)
                {
                    Console.SetCursorPosition(bodyPart.OldPosition.Y, bodyPart.OldPosition.X);
                    Console.Write(" ");
                }
                // Render the player and the body
                Console.SetCursorPosition(player.GetPosition().Y, player.GetPosition().X);
                Console.Write(player.LookType);

                foreach (BodyPart bodyPart in player.BodyParts)
                {
                    Console.SetCursorPosition(bodyPart.GetPosition().Y, bodyPart.GetPosition().X);
                    Console.Write(bodyPart.LookType);
                }
            }
        }

        /// <summary>
        /// rerenders every piece of food object
        /// </summary>
        public void RenderFood()
        {
            // Find every food object inside the GameObjects list
            List<GameObject> allFoods = GameObjects.FindAll((x) => x.GetObjectType() == ObjectType.Food);

            foreach(Food food in allFoods)
            {
                Console.SetCursorPosition(food.GetPosition().Y, food.GetPosition().X);
                Console.Write(food.LookType);
            }
        }

        /// <summary>
        /// Generate every wall inside the GameObjects list
        /// </summary>
        public void RenderWalls()
        {
            // Find every wall object inside the GameObjects list
            List<GameObject> allWalls = GameObjects.FindAll((x) => x.GetObjectType() == ObjectType.Wall);
            List<GameObject> allPortals = GameObjects.FindAll((x) => x.GetObjectType() == ObjectType.Portal);

            foreach (Wall wall in allWalls)
            {
                Console.SetCursorPosition(wall.GetPosition().Y, wall.GetPosition().X);
                Console.Write(wall.LookType);
            }
            foreach (Portal portal in allPortals)
            {
                Console.SetCursorPosition(portal.GetPosition().Y, portal.GetPosition().X);
                Console.Write(portal.LookType);
            }
        }

        /// <summary>
        /// rerenders the score, highscore and time played
        /// </summary>
        public void RenderInformation()
        {
            Console.SetCursorPosition(0, 0);
            Console.Write($"Score: {Score}\t Time Alive: {Timer.GameTime}");
        }

        /// <summary>
        /// Generate a square game arena with the specified top left and bottom right corners
        /// </summary>
        /// <param name="topLeftCorner">The top left corner of the arena</param>
        /// <param name="bottomRightCorner">To bottom right corner of the arena</param>
        public void GenerateArena(Vector2D[] arenaSize, int difficulty)
        {
            Random rand = new();
            int portalChance = difficulty;

            TopLeftCornerPos = arenaSize[0];
            BottomRightCornerPos = arenaSize[1];

            Food firstFood = Food.GenerateNewFood();
            GameObjects.Add(firstFood);

            // Create a wall of walls around the arena
            // Give a chance to create portals instead of walls
            for (int i = TopLeftCornerPos.Y+1; i < BottomRightCornerPos.Y; i++)
            {
                // Generate the top and bottom column
                Vector2D wallTopPos = new(TopLeftCornerPos.X, i);
                Vector2D wallBottomPos = new(BottomRightCornerPos.X, i);
                if( rand.Next(1, 3) >= portalChance && wallTopPos.X != TopLeftCornerPos.Y && wallBottomPos.X != BottomRightCornerPos.Y)
                {
                    GameObjects.Add(new Portal(wallTopPos));
                    GameObjects.Add(new Portal(wallBottomPos));
                }
                else
                {
                    GameObjects.Add(new Wall(wallTopPos));
                    GameObjects.Add(new Wall(wallBottomPos));
                }
            }

            for (int i = TopLeftCornerPos.X; i <= BottomRightCornerPos.X; i++)
            {
                // Generate the left and right column
                Vector2D wallLeftPos = new(i, TopLeftCornerPos.Y);
                Vector2D wallRightPos = new(i, BottomRightCornerPos.Y);
                if (rand.Next(1, 3) >= portalChance && wallLeftPos.X != TopLeftCornerPos.X && wallRightPos.X != BottomRightCornerPos.X)
                {
                    GameObjects.Add(new Portal(wallLeftPos, '│'));
                    GameObjects.Add(new Portal(wallRightPos, '│'));
                }
                else
                {
                    GameObjects.Add(new Wall(wallLeftPos));
                    GameObjects.Add(new Wall(wallRightPos));
                }
            }
        }

        /// <summary>
        /// Check if the position sent in, is inside the arena walls.
        /// </summary>
        /// <param name="objectPos">Position of the object to check</param>
        /// <returns>true if the incoming position exists inside the arena</returns>
        public bool IsInsideArena(Vector2D objectPos)
        {
            int x = objectPos.X;
            int y = objectPos.Y;

            if (x > TopLeftCornerPos.X && x < BottomRightCornerPos.X && y > TopLeftCornerPos.Y && y < BottomRightCornerPos.Y)
                return true;

            return false;
        }
    }
}

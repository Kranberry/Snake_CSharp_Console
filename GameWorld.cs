using System;
using System.Collections.Generic;
using System.Linq;

namespace Snake
{
    public class GameWorld
    {
        public List<GameObject> CollisionObjects = new();
        readonly ScoreBoard ScoreBoard = ScoreBoard.ScoreBoardInstance;

        public Vector2D TopLeftCornerPos { get; set; }
        public Vector2D BottomRightCornerPos { get; set; }

        // This is my very first time using a singleton. 
        // I ams so excited
        private GameWorld() { } // Private constructor. Only one instance allowed
        private static readonly Lazy<GameWorld> Game = new Lazy<GameWorld>(() => new GameWorld());  // Create a Lazy<T> object that will only initialize the first instance when it is called upon
        public static GameWorld GameWorldInstance
        {
            get => Game.Value;  // Get our instance
        }

        /// <summary>
        /// Renders the player on the gameWorld
        /// </summary>
        public void RenderPlayer()
        {
            // Select all players that exists (Including AI)
            List<GameObject> selectedPlayers = CollisionObjects.FindAll((x) => x.GetObjectType() == ObjectType.Player);

            // Render each player (Including AI, if I ever add one)
            foreach(Player player in selectedPlayers.ToList())
            {
                if(!player.HasTeleported)
                    RenderPlayerAndBody(player);
            }
        }

        /// <summary>
        /// Rerender the player and the body
        /// </summary>
        /// <param name="player">The player to render</param>
        /// <param name="doRender">false if you want to clear the old render first, true if you want to only render</param>
        private void RenderPlayerAndBody(Player player, bool doRender = false)
        {
            // Get the x and y of either the old position or the current one. Depening on wheter we clear or draw
            (int x, int y) = doRender switch
            {
                true => (player.GetPosition().X, player.GetPosition().Y),
                false => (player.GetOldPosition().X, player.GetOldPosition().Y)
            };

            Console.SetCursorPosition(y, x);
            Console.Write(doRender ? player.LookType : " ");

            // Clear or render every bodypart the player has
            foreach (BodyPart bodyPart in player.BodyParts.ToList())
            {
                (x, y) = doRender switch // TUUUUUUUPLES!
                {
                    true => (bodyPart.GetPosition().X, bodyPart.GetPosition().Y),
                    false => (bodyPart.GetOldPosition().X, bodyPart.GetOldPosition().Y)
                };

                Console.SetCursorPosition(y, x);
                Console.ForegroundColor = (ConsoleColor)bodyPart.FoodColor;
                Console.Write(doRender ? bodyPart.LookType : " ");
            }
            Console.ForegroundColor = ConsoleColor.White;

            // If we have cleared the player, rerun the method and draw the player
            if (doRender)
                return;
            else
                RenderPlayerAndBody(player, true);
        }

        /// <summary>
        /// rerenders every piece of food object
        /// </summary>
        public void RenderFood()
        {
            // Find every food object inside the GameObjects list
            List<GameObject> allFoods = CollisionObjects.FindAll((x) => x.GetObjectType() == ObjectType.Food);

            foreach(Food food in allFoods)
            {
                Console.SetCursorPosition(food.GetPosition().Y, food.GetPosition().X);
                Console.ForegroundColor = (ConsoleColor)food.FoodColor;
                Console.Write(food.LookType);
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Generate every wall inside the CollisionObjects list
        /// </summary>
        public void RenderWalls()
        {
            // Find every wall or portal object inside the CollisionObjects list
            List<GameObject> allWalls = CollisionObjects.FindAll((x) => x.GetObjectType() == ObjectType.Wall || x.GetObjectType() == ObjectType.Portal);

            for(int i = 0; i < allWalls.Count; i++)
            {
                IRenderable wallRender = (IRenderable)allWalls[i];
#if DEBUG
                Console.SetCursorPosition(allWalls[i].GetPosition().Y, allWalls[i].GetPosition().X);
#else
                Console.SetCursorPosition(allWalls[i].GetPosition().Y, allWalls[i].GetPosition().X + 1);    // The +1 must be here for release, otherwise it will draw every wall 1 step up
#endif
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(wallRender.LookType);
            }
        }

        /// <summary>
        /// Rerender said position, with whatever object is there
        /// </summary>
        public void RenderPosition(Vector2D position)
        {
            GameObject obj = CollisionObjects.Find((x) => x.GetPosition() == position); // Get the object to render
            IRenderable renderObject = (IRenderable)obj;
            if(renderObject != null)
            {
                Console.SetCursorPosition(position.Y, position.X);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(renderObject.LookType);
            }
        }

        /// <summary>
        /// rerenders the score, highscore and time played
        /// </summary>
        public void RenderInformation()
        {
            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"Time Alive: {ScoreBoard.PlayTime}\nScore: {ScoreBoard.Score}");
        }

        /// <summary>
        /// Generate a square game arena with the specified top left and bottom right corners
        /// </summary>
        /// <param name="arenaSize">An array refering to the size of the arena</param>
        /// <param name="difficulty">An int representing the amount of portals</param>
        public void GenerateArena(Vector2D[] arenaSize, int difficulty)
        {
            Random rand = new();
            int portalChance = difficulty;  // Chance to create a portal

            TopLeftCornerPos = arenaSize[0];    // Top left corner of the arena
            BottomRightCornerPos = arenaSize[1];    // Bottom right corner of the arena
            List<Vector2D> corners = new List<Vector2D>()   // List of the 4 corners of the arena. Will always be wall
            {
                TopLeftCornerPos,
                new Vector2D(BottomRightCornerPos.X, TopLeftCornerPos.Y),   // Top right
                new Vector2D(TopLeftCornerPos.X, BottomRightCornerPos.Y),   // Bottom left
                BottomRightCornerPos
            };

            // Generate the first food
            Food firstFood = Food.GenerateFirstFood();
            CollisionObjects.Add(firstFood);

            // Generate the walls and portals
            for (int x = TopLeftCornerPos.X; x <= BottomRightCornerPos.X; x++)
            {
                for(int y = TopLeftCornerPos.Y; y <= BottomRightCornerPos.Y; y++)
                {
                    Vector2D newWallPos = new Vector2D(x, y);   // Position to be
                    if(!IsInsideArena(newWallPos) && !CollisionObjects.Exists((wall) => wall.GetPosition() == newWallPos))  // Check if we at the sides of the arena, and that there isn't a wall already here
                    {
                        bool doPortalExist = CollisionObjects.Exists((portal) => portal.GetPosition() == newWallPos);   // Is there already a portal here? Used on the bottom and right hand side of the arena
                        bool isCorner = corners.Contains(newWallPos);   // Is it a corner position?

                        // If we create a portal, make sure to create the opposite portal aswell.
                        if (rand.Next(1, 3) >= portalChance && !doPortalExist && (x == TopLeftCornerPos.X || y == TopLeftCornerPos.Y) && !isCorner)
                        {
                            int oppositeX = x == TopLeftCornerPos.X ? BottomRightCornerPos.X : x; // The opposite side of the arena
                            int oppositeY = x != TopLeftCornerPos.X ? BottomRightCornerPos.Y : y; // The opposite side of the arena
                            char lookType = x == TopLeftCornerPos.X ? '─' : '│';    // Vertical or Horizontal portal?

                            Portal portal = new(newWallPos, lookType);
                            Portal portal2 = new(new Vector2D(oppositeX, oppositeY), lookType);  // Create the same on the opposite wall
                            CollisionObjects.Add(portal);
                            CollisionObjects.Add(portal2);
                        }
                        else
                            CollisionObjects.Add(new Wall(new Vector2D(x, y)));
                    }
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

        /// <summary>
        /// Check if incoming bool is true or false
        /// </summary>
        /// <param name="myBool">The boolean to check</param>
        /// <returns>True if the boolean is true, and false if it is false</returns>
        public static bool IsThisBoolTrue(bool myBool) => !myBool != !false && !!myBool != !!false || true.ToString() == myBool.ToString() ? !true == false && myBool != !!false ? !(typeof(decimal) != myBool.GetType()) || false != !!myBool ? myBool == !false : !false && myBool && !myBool != true : myBool.GetType() != typeof(string) && false != myBool || true != false && false != myBool && myBool == false : false; // :)
    }
}
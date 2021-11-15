using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Snake
{
    public class Player : GameObject, IRenderable
    {
        private readonly ScoreBoard ScoreBoard = ScoreBoard.ScoreBoardInstance;
        public Direction Direction { get; set; }
        public List<BodyPart> BodyParts { get; init; }
        public char LookType { get; set; }
        public bool HasMoved { get; set; }
        public bool HasTeleported { get; set; } = false;
        public bool EnableController { get; private set; }

        public Player(string name, Vector2D position) : base(name, position, ObjectType.Player)
        {
            LookType = (char)17;
            BodyParts = new List<BodyPart>();
            HasMoved = false;
            EnableController = true;
        }

        /// <summary>
        /// Adds a new bodypart and changes the other connections
        /// </summary>
        /// <param name="bodyPart">The new bodypart</param>
        public void AddNewBodyPart(BodyPart bodyPart)
        {
            // When adding a new body part, change the conenctions so the parts follow the new bodypart
            BodyParts.Add(bodyPart);
            for (int i = 0; i < BodyParts.Count; i++)
            {
                BodyPart oldBodyPart = BodyParts[i];
                if(i == BodyParts.Count - 1)
                    oldBodyPart.NewConnection(this);
                else
                    oldBodyPart.NewConnection(BodyParts[i + 1]);
            }
        }

        /// <summary>
        /// Kill the player and stop the game.
        /// But first show the endscore
        /// </summary>
        public async override void Destroy()
        {
            GameWorld game = GameWorld.GameWorldInstance;
            GameUpdateTimer Timer = GameUpdateTimer.TimerInstance;
            //ThreadPool.QueueUserWorkItem((x) => HighScore.HighScoreInstance.UpdateHighScore(GetName(), game.Score, Timer.GameTime, game.Difficulty));

            // Stop movement and remove all bodyparts from the GameObjects list
            Direction = Direction.None;
            EnableController = false;   // Block the player from movement
            HardSetPosition(GetOldPosition());
            Timer.GameIsEnding = true;  // The game is ending. Stop collision detection

            foreach(BodyPart bodyPart in BodyParts)
            {
                bodyPart.LookType = '×';
                await Task.Delay(100);
                bodyPart.LookType = ' ';
                game.CollisionObjects.Remove(bodyPart);
            }

            Timer.GameHasEnded = true;  // Game has ended, stop the gameWorld update
            BodyParts.Clear();

            await Task.Delay(1000);
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Game Over!!!");
            Console.WriteLine($"Stayed alive for {ScoreBoard.PlayTime} seconds");
            Console.WriteLine($"Reached a score of {ScoreBoard.Score}");
            Console.WriteLine("Press the ESC key to return to the main menu");
        }

        /// <summary>
        /// Set the new position of the player depending on the direction
        /// </summary>
        public void UpdatePlayerPosition()
        {
            // If the player has died. Then do not let the player to keep moving
            if (!EnableController)
                return;

            // Determine the new position of the player
            Vector2D NewPosition = this.Direction switch    // I know "this" is not nessecary in this context. But, I use it for clarificarion through this method
            {
                Direction.North => new Vector2D(-1, 0),
                Direction.East => new Vector2D(0, 1),
                Direction.South => new Vector2D(1, 0),
                Direction.West => new Vector2D(0, -1),
                _ => new Vector2D(0, 0)
            };

            this.SoftSetPosition(NewPosition);

            // Set the new position of the body parts
            for (int i = this.BodyParts.Count - 1; i >= 0; i--)
            {
                BodyPart bodyPart = this.BodyParts[i];
                // Get the connecteds objects old position
                Vector2D nextPos = bodyPart.ConnectedTo.GetOldPosition();
                // And set that position to this objects new position
                bodyPart.HardSetPosition(nextPos);
            }

            this.HasMoved = false;
        }

        // Called from the Collision2D class. Gives a refrence to every object that overlaps positional data
        public override void OnCollisionEnter(GameObject[] collidedWith)
        {
            GameWorld game = GameWorld.GameWorldInstance;

            foreach(GameObject gObject in collidedWith)
            {
                switch (gObject.GetObjectType())
                {
                    // Add points, create new food and add a bodypart
                    case ObjectType.Food:
                        Food food = (Food)gObject;
                        food.OnEaten();
                        BodyPart newBodyPart = new BodyPart(this, this.GetPosition(), food.FoodColor);
                        this.AddNewBodyPart(newBodyPart);
                        game.CollisionObjects.Add(newBodyPart);
                        break;
                        // Teleport player and rerender the walls
                    case ObjectType.Portal:
                        Portal.TeleportPlayer(this);
                        break;
                        // Die, and end the game
                    case ObjectType.BodyPart:
                    case ObjectType.Wall:
                        this.Destroy();
                        break;
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Snake
{
    public class Player : GameObject, IRenderable
    {
        public int Speed { get; init; } = 1;
        public Direction Direction { get; set; }
        public List<BodyPart> BodyParts { get; private set; }
        public char LookType { get; set; }
        public bool HasMoved { get; set; }
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
        public async void Die()
        {
            GameWorld game = GameWorld.GameWorldInstance;
            Timer Timer = Timer.TimerInstance;

            // Stop movement and remove all bodyparts from the GameObjects list
            Direction = Direction.None;
            EnableController = false;
            HardSetPosition(OldPosition);
            foreach(BodyPart bodyPart in BodyParts)
            {
                bodyPart.LookType = '×';
                await Task.Delay(100);
                bodyPart.LookType = ' ';
                game.GameObjects.Remove(bodyPart);
            }

            Timer.GameEnd = true;
            BodyParts.Clear();

            await Task.Delay(1000);
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Game Over!!!");
            Console.WriteLine($"Stayed alive for {Timer.GameTime} seconds");
            Console.WriteLine($"Reached a score of {game.Score}");
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
            Vector2D NewPosition = this.Direction switch    // I know this is not nessecary in this context. But, I use it for clarificarion through this method
            {
                Direction.North => new Vector2D(-1, 0),
                Direction.East => new Vector2D(0, 1),
                Direction.South => new Vector2D(1, 0),
                Direction.West => new Vector2D(0, -1),
                _ => new Vector2D(0, 0)
            };

            bool isPortal = false;
            CollidedWithGameObject((this.GetPosition() + NewPosition), ref isPortal);

            if (!isPortal)
                this.SetPosition(NewPosition);

            // Move this below inside the player controller class with the above part
            for (int i = this.BodyParts.Count - 1; i >= 0; i--)
            {
                BodyPart bodyPart = this.BodyParts[i];
                // Get the connecteds objects old position
                Vector2D nextPos = bodyPart.ConnectedTo.OldPosition;
                // And set that position to this objects new position
                bodyPart.HardSetPosition(nextPos);
            }

            this.HasMoved = false;
        }

        /// <summary>
        /// Check for collision at the new position. Used before the new position is set
        /// </summary>
        /// <param name="newPosition">The new position to be set</param>
        /// <param name="isPortal">If you come from a portal or not</param>
        public void CollidedWithGameObject(Vector2D newPosition, ref bool isPortal)
        {
            GameWorld game = GameWorld.GameWorldInstance;
            GameObject collidedWith;
            // Get the gameobject that is on the new player position. Do something depening on what it is
            collidedWith = game.GameObjects.Find((x) => PhysicsEngine2D.Collision2D.HasCollided(x.GetPosition(), newPosition));

            if (collidedWith != null)
            {   // Get what ObjectType the collided object has
                switch (collidedWith.GetObjectType())
                {
                    // Eat the food and generate a new food item
                    // When a food is eaten, the snake grows.
                    // Add the new bodypart to the players list, aswell as a possible collision object inside the GameObjects list
                    case ObjectType.Food:
                        Food foodObject = (Food)collidedWith;
                        foodObject.OnEaten();
                        BodyPart newBodyPart = new BodyPart(this, this.GetPosition());
                        this.AddNewBodyPart(newBodyPart);
                        game.GameObjects.Add(newBodyPart);
                        isPortal = false;
                        break;
                    case ObjectType.Portal:
                        Portal portal = (Portal)collidedWith;
                        portal.TeleportPlayer(this);
                        isPortal = true;
                        break;
                    case ObjectType.BodyPart:
                    case ObjectType.Wall:
                        this.Die();
                        isPortal = false;
                        break;
                }
            }
        }
    }

    // The PlayerController is made to tidy up the Player class a bit
    // The playercontrollers constructor must have a Player instance in order to function
    // Might be fun to use this controller class to change the target player to control. Play with 2 snakes at the same time
    // If one die, both die
    public class PlayerController
    {
        Player Player { get; init; }

        public PlayerController(Player playerInstance)
        {
            Player = playerInstance;
        }

        /// <summary>
        /// Determine a legal direction for the player
        /// </summary>
        /// <param name="inputDirection">The direction you want</param>
        /// <returns>The legal direction you get. Tou can not get the opposite direction of what the player currently has</returns>
        private Direction DeterminePlayerDirection(Direction inputDirection)
        {
            switch (inputDirection)
            {
                case Direction.North:
                    // You cannot go to the opposite direction of movement
                    if (Player.Direction == Direction.South)
                        inputDirection = Direction.South;
                    break;
                case Direction.East:
                    if (Player.Direction == Direction.West)
                        inputDirection = Direction.West;
                    break;
                case Direction.South:
                    if (Player.Direction == Direction.North)
                        inputDirection = Direction.North;
                    break;
                case Direction.West:
                    if (Player.Direction == Direction.East)
                        inputDirection = Direction.East;
                    break;
                default:
                    inputDirection = Player.Direction;
                    break;
            }

            return inputDirection;
        }

        /// <summary>
        /// Changes the direction of the player connected to this instance
        /// The directions are of intergers, 1 -> 4, N, E, S, W
        /// </summary>
        /// <param name="direction"></param>
        public void ChangeDirection(Direction inputDirection)
        {
            if (Player.HasMoved)
                return;

            // Determine the new direction depening on the rules set in the Func delegate
            Player.Direction = DeterminePlayerDirection(inputDirection);

            // Set the players looktype to be the more logical look when moving a certain direction
            Player.LookType = Player.Direction switch
            {
                Direction.North => (char)30,
                Direction.East => (char)16,
                Direction.South => (char)31,
                Direction.West => (char)17,
                _ => (char)50
            };

            Player.HasMoved = true;
        }
    }
}
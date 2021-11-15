using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    class PlayerController
    {
        // The playercontrollers constructor must have a Player instance in order to function
        // Might be fun to use this controller class to change the target player to control. Play with 2 snakes at the same time
        // If one die, both die. Maybe.
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
            }

            return inputDirection;
        }

        /// <summary>
        /// Changes the direction of the player connected to this instance
        /// The directions are of Direction, North, South, West, East and None
        /// </summary>
        /// <param name="direction"></param>
        public void ChangeDirection(Direction inputDirection)
        {
            if (Player.HasMoved)
                return;

            // Determine the new direction depening on the rules set in said method
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    class Portal : GameObject, IRenderable
    {
        public char LookType { get; set; }

        public Portal(Vector2D position, char lookType = '─') : base("Portal", position, ObjectType.Portal)
        {
            LookType = lookType;
        }

        /// <summary>
        /// Teleport the player to the opposite side of the arena
        /// </summary>
        /// <param name="player">The player to be teleported</param>
        public void TeleportPlayer(Player player)
        {
            GameWorld game = GameWorld.GameWorldInstance;
            // Create new variables to not refrence the players value
            int X = player.GetPosition().X;
            int Y = player.GetPosition().Y;

            // Set the new teleported area
            Vector2D playerPosition = player.Direction switch
            {
                Direction.North => new(game.BottomRightCornerPos.X - 1, Y),
                Direction.South => new(game.TopLeftCornerPos.X + 1, Y),
                Direction.East => new(X, game.TopLeftCornerPos.Y + 1),
                Direction.West => new(X, game.BottomRightCornerPos.Y - 1),
                _ => new(0, 0)
            };

            bool isPortal = true;
            // Check collision with the new position
            player.CollidedWithGameObject(playerPosition, ref isPortal);
            // Set the new position of the player
            player.HardSetPosition(playerPosition);
        }
    }
}

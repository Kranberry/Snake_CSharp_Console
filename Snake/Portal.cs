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
        public static void TeleportPlayer(Player player)
        {
            player.HasTeleported = true;
            GameWorld game = GameWorld.GameWorldInstance;
            // Create new variables to not refrence the players values
            int X = player.GetPosition().X;
            int Y = player.GetPosition().Y;

            // Set the new teleported position
            Vector2D playerPosition = player.Direction switch
            {
                Direction.North => new(game.BottomRightCornerPos.X - 1, Y),
                Direction.South => new(game.TopLeftCornerPos.X + 1, Y),
                Direction.East => new(X, game.TopLeftCornerPos.Y + 1),
                Direction.West => new(X, game.BottomRightCornerPos.Y - 1),
                _ => new(0, 0)
            };

            game.RenderPosition(player.GetPosition());
            // Set the new position of the player, while keeping the old position pure
            player.HarSetPositionEX(playerPosition);
        }
    }
}

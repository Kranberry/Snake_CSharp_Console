using System;
using System.Collections.Generic;
using System.Text;

namespace Snake
{
    class Food : GameObject, IFood, IRenderable
    { 
        public int RewardPoints { get; set; }
        public GameWorld game { get; } = GameWorld.GameWorldInstance;
        public char LookType { get; set; }

        public Food(string name, Vector2D position, int rewardPoints) : base(name, position, ObjectType.Food)
        {
            RewardPoints = rewardPoints;
            LookType = '☼';
        }

        /// <summary>
        /// Destroy this food instance and create a new one. Adding this instance RewardPoints to the score
        /// </summary>
        public void OnEaten()
        {
            game.Score += RewardPoints;

            Food newFood = GenerateNewFood();
            game.GameObjects.Add(newFood);

            game.GameObjects.Remove(this);
        }

        /// <summary>
        /// Generate a new Food instance that has a legal Vector2D position
        /// </summary>
        /// <returns>A food instance with a legal Vector2D position</returns>
        public static Food GenerateNewFood()
        {
            Random rand = new Random();
            GameWorld game = GameWorld.GameWorldInstance;
            Vector2D SpawnPosition;

            do
            {
                int xPos = rand.Next(game.TopLeftCornerPos.X + 1, game.BottomRightCornerPos.X);
                int yPos = rand.Next(game.TopLeftCornerPos.Y + 1, game.BottomRightCornerPos.Y);

                SpawnPosition = new Vector2D(xPos, yPos);
            } while (!game.IsInsideArena(SpawnPosition));

            return new Food("BadFood", SpawnPosition, 1);
        }
    }
}

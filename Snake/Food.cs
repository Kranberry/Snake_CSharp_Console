using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    public class Food : GameObject, IRenderable
    {
        private readonly ScoreBoard ScoreBoard = ScoreBoard.ScoreBoardInstance;
        public int RewardPoints { get; set; }
        public GameWorld Game { get; } = GameWorld.GameWorldInstance;
        public char LookType { get; set; }
        public FoodColor FoodColor { get; set; }

        private Food(): base("", new(0, 0), ObjectType.Food) { }  // Används för att skapa första food

        public Food(string name, Vector2D position, int rewardPoints, FoodColor foodColor = FoodColor.Bad) : base(name, position, ObjectType.Food)
        {
            FoodColor = foodColor;
            RewardPoints = rewardPoints;
            LookType = '☼';
        }

        /// <summary>
        /// Generate the very first food. Always a bad food 1 point
        /// </summary>
        /// <returns>An instance of Food</returns>
        public static Food GenerateFirstFood()
        {
            Food food = new();
            food = food.GenerateBadFood(GenerateSpawnPosition());
            return food;
        }

        /// <summary>
        /// Decay the food, after 3 seconds, the food will turn worse
        /// </summary>
        private async void DecayFood()
        {
            if (RewardPoints == 1)
                return;

            await Task.Delay(3000);
            switch (RewardPoints)
            {
                case 2:
                    FoodColor = FoodColor.Bad;
                    RewardPoints--;
                    return;
                case 3:
                    FoodColor = FoodColor.Medium;
                    RewardPoints--;
                    DecayFood();
                    break;
            }
        }

        /// <summary>
        /// Destroy this food instance and create a new one. Adding this instance RewardPoints to the score
        /// </summary>
        public void OnEaten()
        {
            Func<Vector2D, Food> generateNewFood;   // Makes a more clean code. Instead of multiple if/else, just use switch expression
            Random rand = new();

            ScoreBoard.Score += RewardPoints;

            // Generate a new food to take this foods place
            generateNewFood = rand.Next(0, 6) switch
            {
                1 => GenerateBadFood,
                2 => GenerateMediocreFood,
                3 => GenerateBestFood,
                _ => GenerateBadFood        // Higher chance for bad food
            };

            Vector2D spawnPosition = GenerateSpawnPosition();
            Food newFood = generateNewFood(spawnPosition);
            newFood.DecayFood();
            Game.CollisionObjects.Add(newFood);

            // Destroy the instance of this object.
            // It belongs to the garbage collector now.
            // Poor soul :(
            Destroy();
        }

        /// <summary>
        /// Generate a new Food instance that has a legal Vector2D position, food gives 1 point
        /// </summary>
        /// <returns>A food instance with a legal Vector2D position</returns>
        public Food GenerateBadFood(Vector2D spawnPos) => new Food("BadFood", spawnPos, 1);

        /// <summary>
        /// Generate a new Food instance that has a legal Vector2D position, food gives 2 point
        /// </summary>
        /// <returns>A food instance with a legal Vector2D position</returns>
        public Food GenerateMediocreFood(Vector2D spawnPos) => new Food("MediumFood", spawnPos, 2, FoodColor.Medium);

        /// <summary>
        /// Generate a new Food instance that has a legal Vector2D position, food gives 3 point
        /// </summary>
        /// <returns>A food instance with a legal Vector2D position</returns>
        public Food GenerateBestFood(Vector2D spawnPos) => new Food("BestFood", spawnPos, 3, FoodColor.Good);

        /// <summary>
        /// Generate a Vector2D object that exists within the walls of the arena
        /// </summary>
        /// <returns>A legal Vector2D object within the arena walls</returns>
        private static Vector2D GenerateSpawnPosition()
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

            return SpawnPosition;
        }
    }
}

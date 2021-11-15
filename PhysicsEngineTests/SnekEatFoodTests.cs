using Microsoft.VisualStudio.TestTools.UnitTesting;
using Snake;
using System;

namespace GameEngineTests
{
    [TestClass]
    public class SnekEatFoodTests
    {
        [TestMethod]
        public void PointsOnEatTest()
        {
            GameWorld game = GameWorld.GameWorldInstance;
            game.GenerateArena(new Vector2D[] { new(0, 4), new(10, 10) }, 3);   // Generate a Mock arena'ish

            Player player = new("test", new Vector2D(5, 5));

            ScoreBoard scoreboard = ScoreBoard.ScoreBoardInstance;
            scoreboard.Score = 0;

            Food food = new("test", new(), 1, FoodColor.Bad);
            player.OnCollisionEnter(new GameObject[] { food.GenerateBadFood(new Vector2D(5, 5)) });
            Assert.AreEqual(1, scoreboard.Score);
            
            player.OnCollisionEnter(new GameObject[] { food.GenerateBestFood(new Vector2D(5, 5)) });
            Assert.AreEqual(4, scoreboard.Score);

            player.OnCollisionEnter(new GameObject[] { food.GenerateMediocreFood(new Vector2D(5, 5)) });
            Assert.AreEqual(6, scoreboard.Score);
        }

        [TestMethod]
        public void SnekGrowTest()
        {
            GameWorld game = GameWorld.GameWorldInstance;
            game.GenerateArena(new Vector2D[] { new(0, 4), new(10, 10) }, 3);   // Generate a Mock arena'ish

            Player player = new("test", new Vector2D(5, 5));
            Food food = new("test", new(), 1, FoodColor.Bad);

            player.OnCollisionEnter(new GameObject[] { food.GenerateBadFood(new Vector2D(5, 5)) });
            Assert.AreEqual(1, player.BodyParts.Count);

            player.OnCollisionEnter(new GameObject[] { food.GenerateBadFood(new Vector2D(5, 5)) });
            Assert.AreEqual(2, player.BodyParts.Count);

            player.OnCollisionEnter(new GameObject[] { food.GenerateBadFood(new Vector2D(5, 5)) });
            Assert.AreEqual(3, player.BodyParts.Count);
        }
    }
}

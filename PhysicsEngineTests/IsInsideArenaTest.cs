using Microsoft.VisualStudio.TestTools.UnitTesting;
using Snake;

namespace GameEngineTests
{
    [TestClass]
    public class IsInsideArenaTest
    {
        [TestMethod]
        public void IsInsideArenaMTest()
        {
            GameWorld game = GameWorld.GameWorldInstance;
            Vector2D[] arenaSize = new Vector2D[2] { new(4, 0), new(10, 10) };
            Vector2D insideArena = new(5, 5);
            Vector2D outsideArena = new(2, 0);
            Vector2D onArenaWalls = new(10, 5);

            game.GenerateArena(arenaSize, 3);
            bool gainedInside = game.IsInsideArena(insideArena);
            bool gainedOutside = game.IsInsideArena(outsideArena);
            bool gainedOnWall = game.IsInsideArena(onArenaWalls);

            Assert.IsTrue(gainedInside);
            Assert.IsFalse(gainedOutside);
            Assert.IsFalse(gainedOnWall);
        }

        [TestMethod]
        public void GenerateFirstFoodTest()
        {
            GameWorld game = GameWorld.GameWorldInstance;
            Vector2D[] arenaSize = new Vector2D[2] { new(4, 0), new(10, 10) };
            game.GenerateArena(arenaSize, 3);

            Food food = Food.GenerateFirstFood();

            bool isInsideArena = game.IsInsideArena(food.GetPosition());
            Assert.IsTrue(isInsideArena);
        }
    }
}

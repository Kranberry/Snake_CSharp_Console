using Microsoft.VisualStudio.TestTools.UnitTesting;
using Snake;

namespace GameEngineTests
{
    [TestClass]
    public class GameWorldTests
    {
        [TestMethod]
        public void IsBoolTrueTrueTest()
        {
            Assert.AreEqual(true, GameWorld.IsThisBoolTrue(true));
            Assert.IsTrue(GameWorld.IsThisBoolTrue(true));
        }

        [TestMethod]
        public void IsBoolTrueFalseTest()
        {
            Assert.AreEqual(false, GameWorld.IsThisBoolTrue(false));
            Assert.IsFalse(GameWorld.IsThisBoolTrue(false));
        }

        [TestMethod]
        public void CreateLegalFoodPositionTest()
        {
            GameWorld game = GameWorld.GameWorldInstance;
            game.GenerateArena(new Vector2D[] { new(0, 4), new(10, 10) }, 3);   // Generate a Mock arena'ish

            Food food = Food.GenerateFirstFood();   // Generate and test the first food
            Assert.IsTrue(game.IsInsideArena(food.GetPosition()));
            food.OnEaten();

            // Check 100 new foods. Should bring something outside if it is programmed wrong
            for (int i = 0; i < 100; i++)
            {
                Food foodNew = (Food)game.CollisionObjects.Find((x) => x.GetObjectType() == ObjectType.Food);
                Assert.IsTrue(game.IsInsideArena(foodNew.GetPosition()));
                foodNew.OnEaten();
            }
        }
    }
}

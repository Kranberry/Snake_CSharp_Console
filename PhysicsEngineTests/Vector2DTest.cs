using Microsoft.VisualStudio.TestTools.UnitTesting;
using Snake;

namespace GameEngineTests
{
    [TestClass]
    public class Vector2DTest
    {
        [TestMethod]
        public void TestOverloadedPlus()
        {
            Vector2D firstVector = new(10, 15);
            Vector2D secondVector = new(24, 10);
            Vector2D expectedVectorToReturn = new(34, 25);
            Vector2D actualVectorReturned = firstVector + secondVector;

            Assert.AreEqual(expectedVectorToReturn, actualVectorReturned);
        }

        [TestMethod]
        public void TestOverloadedMinus()
        {
            Vector2D firstVector = new(10, 15);
            Vector2D secondVector = new(24, 19);
            Vector2D expectedVectorToReturn = new(14, 4);
            Vector2D actualVectorReturned = secondVector - firstVector;

            Assert.AreEqual(expectedVectorToReturn, actualVectorReturned);
        }

        [TestMethod]
        public void TestOverloadedEquals()
        {
            Vector2D firstVector = new(10, 15);
            Vector2D secondVector = new(10, 15);
            bool gainedValue = firstVector == secondVector;
            Assert.IsTrue(gainedValue);

            Vector2D falseVector = new(10, 54);
            bool gainedValueFalse = firstVector == falseVector;
            Assert.IsFalse(gainedValueFalse);

            Vector2D falseSecondVector = new(40, 15);
            bool gainedSecondValueFalse = firstVector == falseSecondVector;
            Assert.IsFalse(gainedSecondValueFalse);
        }

        [TestMethod]
        public void TestOverloadedNotEquals()
        {
            Vector2D firstVector = new(10, 15);
            Vector2D secondVector = new(10, 15);
            bool gainedValue = firstVector != secondVector;
            Assert.IsFalse(gainedValue);

            Vector2D falseVector = new(10, 54);
            bool gainedValueFalse = firstVector != falseVector;
            Assert.IsTrue(gainedValueFalse);

            Vector2D falseSecondVector = new(40, 15);
            bool gainedSecondValueFalse = firstVector != falseSecondVector;
            Assert.IsTrue(gainedSecondValueFalse);

            Player player = new("radan", new(5, 5));
            bool aGainedFalseValue = firstVector.Equals(player);
            Assert.IsFalse(aGainedFalseValue);
        }

        [TestMethod]
        public void TestOverloadedMultiplikation()
        {
            Vector2D firstVector = new(10, 15);
            int multipliedWith = 2;
            Vector2D expected = new(20, 30);
            Vector2D gainedValue = firstVector * multipliedWith;
            Assert.AreEqual(expected, gainedValue);

            Vector2D gainedValueTwo = multipliedWith * firstVector;
            Assert.AreEqual(expected, gainedValueTwo);
        }
    }
}

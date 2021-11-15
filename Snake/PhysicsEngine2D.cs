using System;
using System.Collections.Generic;
using System.Linq;

namespace Snake
{
    public class Collision2D
    {
        /// <summary>
        /// Compare 2 Vector2D objects to see if the two objects are ovelapping
        /// </summary>
        /// <param name="v1">The first Vector2D object to compare</param>
        /// <param name="v2">The second Vector2D object to compare></param>
        /// <returns></returns>
        //public static bool HasCollided(Vector2D v1, Vector2D v2) => v1 == v2;

        /// <summary>
        /// Detect collision between GameObjects that resides inside the CollisionObjects list in the GameWorld instance
        /// </summary>
        public static void CollisionDetction2D()
        {

            GameWorld game = GameWorld.GameWorldInstance;
            // Create a group of collections where every collection contatins the exact same position
            IEnumerable<IGrouping<Vector2D, GameObject>> collidedObjects = from c in game.CollisionObjects
                                                                           group c by c.GetPosition();
            // For every collection, we want to convert our collection to a list
            foreach(IGrouping<Vector2D, GameObject> groups in collidedObjects.ToList())
            {
                List<GameObject> gameObjectList = groups.ToList();
                if (gameObjectList.Count < 2)  // If this collection consists of only 1 object, continue. There is no collition to detect
                    continue;

                for(int i = 0; i < gameObjectList.Count; i++)
                {
                    GameObject[] collidedWithObjects = gameObjectList.FindAll((x) => !object.ReferenceEquals(x, gameObjectList[i])).ToArray();   // Find every object where the refrence is not equal to the object we are sending all refrences
                    gameObjectList[i].OnCollisionEnter(collidedWithObjects);  // Give the object a refrence to every object collided with
                }
            }

        }
    }

    public struct Vector2D
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Vector2D(int x, int y)
        {
            X = x;
            Y = y;
        }

        #region override methods
        // Override the Equals method to remove annoying warnings.
        /// <summary>
        /// Check if the incoming object has the same x and y properties as the current object
        /// </summary>
        /// <param name="obj">Incoming object. Will return false if it is not a Vector2D</param>
        /// <returns>true if the 2 Vector2D X and Y properties match. False if it is another object type, or a mismatch</returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Vector2D))  // I want to make sure incoming value is a Vector2D object
                return false;

            Vector2D targetVector = (Vector2D)obj;

            if (targetVector.X == X && targetVector.Y == Y)
                return true;

            return false;
        }

        // Override the GetHashCode method to remove annoying warnings.
        public override int GetHashCode() => Tuple.Create(X, Y).GetHashCode();
        #endregion

        // Overriding the +, *, -, == and != operations.
        #region Overloaded operators
        // Overload the + operator
        public static Vector2D operator +(Vector2D v1, Vector2D v2) => new Vector2D(v1.X + v2.X, v1.Y + v2.Y);
        
        // Overload the - operator. Not sure if this will ever be used
        public static Vector2D operator -(Vector2D v1, Vector2D v2) => new Vector2D(v1.X - v2.X, v1.Y - v2.Y);
        
        public static Vector2D operator *(Vector2D v1, int multiplier) => new(v1.X * multiplier, v1.Y * multiplier);
        
        public static Vector2D operator *(int multiplier, Vector2D v1) => new(v1.X * multiplier, v1.Y * multiplier);
        
        public static Vector2D operator *(Vector2D v1, Vector2D v2) => new(v1.X * v2.X, v1.Y * v2.Y);

        // Overload the == operator.
        public static bool operator ==(Vector2D v1, Vector2D v2) => (v1.X == v2.X && v1.Y == v2.Y);

        // Overload the != operator.
        public static bool operator !=(Vector2D v1, Vector2D v2) => (v1.X != v2.X || v1.Y != v2.Y);
        #endregion
    }
}

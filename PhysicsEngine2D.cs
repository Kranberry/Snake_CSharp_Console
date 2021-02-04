using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    public class PhysicsEngine2D
    {
        public class Collision2D
        {
            /// <summary>
            /// Compare 2 Vector2D object to see if two Vector2D are ovelapping
            /// </summary>
            /// <param name="v1">The first Vector2D object to compare</param>
            /// <param name="v2">The second Vector2D object to compare></param>
            /// <returns></returns>
            public static bool HasCollided(Vector2D v1, Vector2D v2) => v1 == v2;
        }
    }

    public class Vector2D
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
        public override bool Equals(object obj)
        {
            Vector2D targetVector = (Vector2D)obj;

            if (targetVector.X == X && targetVector.Y == Y)
                return true;

            return false;
        }

        // Override the Equals method to remove annoying warnings.
        public override int GetHashCode()
        {
            return X;
        }
        #endregion


        // Overriding the +, *, -, == and != operations.
        #region Overloaded operators
        // Overload the + operator
        public static Vector2D operator +(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1.X + v2.X, v1.Y + v2.Y);
        }
        // Overload the - operator. Not sure if this will ever be used
        public static Vector2D operator -(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Vector2D operator *(Vector2D v1, int multiplier)
        {
            return new(v1.X * multiplier, v1.Y * multiplier);
        }
        public static Vector2D operator *(int multiplier, Vector2D v1)
        {
            return new(v1.X * multiplier, v1.Y * multiplier);
        }
        public static Vector2D operator *(Vector2D v1, Vector2D v2)
        {
            return new(v1.X * v2.X, v1.Y * v2.Y);
        }

        // Overload the == operator.
        public static bool operator ==(Vector2D v1, Vector2D v2)
        {
            if (v1.X == v2.X && v1.Y == v2.Y)
                return true;

            return false;
        }

        // Overload the != operator.
        public static bool operator !=(Vector2D v1, Vector2D v2)
        {
            if (v1.X != v2.X || v1.Y != v2.Y)
                return false;

            return true;
        }
        #endregion
    }
}

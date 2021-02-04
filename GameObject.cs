using System;
using System.Collections.Generic;
using System.Text;

namespace Snake
{
    abstract public class GameObject
    {
        private string Name { get; init; }
        private ObjectType Type { get; init; }
        private Vector2D Position { get; set; }
        public Vector2D OldPosition { get; set; }

        public GameObject(string name, Vector2D position, ObjectType type)
        {
            Name = name;
            Position = position;
            OldPosition = position;
            Type = type;
        }

        /// <summary>
        /// Get the current position of the gameObject
        /// </summary>
        /// <returns>Returns a Vector2D object representing the objects position on the console</returns>
        public Vector2D GetPosition() => Position;

        /// <summary>
        /// A soft way of changing the objects position.
        /// </summary>
        /// <param name="newPosition">Vector2D object where the X and Y property will be added to the gameobjects position Vector2D</param>
        public void SetPosition(Vector2D newPosition) 
        {
            OldPosition = GetPosition();
            Position += newPosition;
        }
        /// <summary>
        /// As a difference from SetPosition(Vector2D), this is a hard set. The new position is the new position
        /// </summary>
        /// <param name="newPosition">The new position will be the new position</param>
        public void HardSetPosition(Vector2D newPosition)
        {
            OldPosition = GetPosition();
            Position = newPosition;
        }
        
        /// <summary>
        /// Get the current Name of the GameObject
        /// </summary>
        /// <returns>Returns the name property of the GameObject</returns>
        public string GetName() => Name;

        /// <summary>
        /// Get the ObjectType as an interger value of the current GameObject
        /// </summary>
        /// <returns>An interger value representing the objects type (ObjectType)</returns>
        public ObjectType GetObjectType() => Type;

        /// <summary>
        /// Update the objects positioning
        /// </summary>
        public virtual void Update()
        {
            // override it where it is nessesary
        }
    }
}

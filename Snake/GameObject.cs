namespace Snake
{
    public abstract class GameObject
    {
        private string Identifier { get; init; }
        private ObjectType Type { get; init; }
        private Vector2D Position { get; set; }
        private Vector2D OldPosition { get; set; }

        public GameObject(string identity, Vector2D position, ObjectType type)
        {
            Identifier = identity;
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
        /// Get the last known position of the gameObject
        /// </summary>
        /// <returns>Returns a Vector2D object representing the objects last known position on the console</returns>
        public Vector2D GetOldPosition() => OldPosition;

        /// <summary>
        /// A soft way of changing the objects position, add the incoming Vector2D to the objects Position.
        /// </summary>
        /// <param name="newPosition">Vector2D object where the X and Y property will be added to the gameobjects position Vector2D</param>
        public void SoftSetPosition(Vector2D newPosition) 
        {
            OldPosition = GetPosition();
            Position += newPosition;
        }
        /// <summary>
        /// As a difference from SoftSetPosition(Vector2D), this is a hard set. Replace the objects position with the incoming Vector2D
        /// </summary>
        /// <param name="newPosition">The new position will be the new position</param>
        public void HardSetPosition(Vector2D newPosition)
        {
            OldPosition = GetPosition();
            Position = newPosition;
        }
        /// <summary>
        /// As a difference from the HardSetPosition(Vector2D). Set the new position of the gameobject without altering the OldPosition property
        /// </summary>
        /// <param name="newPosition">The new position will be the new position</param>
        public void HarSetPositionEX(Vector2D newPosition) => Position = newPosition;

        /// <summary>
        /// Get the current Name of the GameObject
        /// </summary>
        /// <returns>Returns the name property of the GameObject</returns>
        public string GetIdentity() => Identifier;

        /// <summary>
        /// Get the ObjectType as an interger value of the current GameObject
        /// </summary>
        /// <returns>An interger value representing the objects type (ObjectType)</returns>
        public ObjectType GetObjectType() => Type;

        /// <summary>
        /// Gets the collided with objects, when a collision is detected
        /// </summary>
        /// <param name="collidedWith">The gameobjects collided with</param>
        public virtual void OnCollisionEnter(GameObject[] collidedWith) { }

        /// <summary>
        /// Remove this object from the game instance
        /// </summary>
        public virtual void Destroy()
        {
            GameWorld game = GameWorld.GameWorldInstance;
            game.CollisionObjects.Remove(this);
        }
    }
}

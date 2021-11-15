namespace Snake
{
    public class BodyPart : GameObject, IRenderable
    {
        public Vector2D NewPosition { get; set; }
        public Player Head { get; init; }   // This will never change
        public GameObject ConnectedTo { get; set; } // This will change whenever a new food is consumed by the player
        public char LookType { get; set; }
        public FoodColor FoodColor { get; init; }

        public BodyPart(Player connectedTo, Vector2D position, FoodColor foodColor = FoodColor.Bad) : base("playerBody", position, ObjectType.BodyPart)
        {
            Head = connectedTo;
            ConnectedTo = connectedTo;
            LookType = '©';
            FoodColor = foodColor;
            NewPosition = ConnectedTo.GetPosition();
        }

        /// <summary>
        /// Change the current connected gameobject to a new connected gameobject
        /// </summary>
        /// <param name="newConnection">The new connected gameObject</param>
        public void NewConnection(GameObject newConnection) => ConnectedTo = newConnection;
    }
}

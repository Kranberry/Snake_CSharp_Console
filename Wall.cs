namespace Snake
{
    class Wall : GameObject, IRenderable
    {
        public char LookType { get; set; }

        public Wall(Vector2D position) : base("Wall", position, ObjectType.Wall)
        {
            LookType = '█';
        }

    }
}

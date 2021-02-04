using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

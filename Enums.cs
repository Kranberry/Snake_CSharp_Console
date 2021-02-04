using System;
using System.Collections.Generic;
using System.Text;

namespace Snake
{
    public enum Direction
    {
        None = 0,
        North = 1,
        East = 2,
        South = 3,
        West = 4
    }

    public enum ObjectType
    {
        Player = 1,
        Food = 2,
        Wall = 3,
        BodyPart = 4,
        Portal = 5
    }
}

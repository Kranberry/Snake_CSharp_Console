using System;

namespace Snake
{
    public enum Direction
    {
        None,
        North,
        East,
        South,
        West
    }

    public enum ObjectType
    {
        Player,
        Food,
        Wall,
        BodyPart,
        Portal
    }

    public enum FoodColor
    {
        Good = ConsoleColor.Green,      // Best food 3 points
        Medium = ConsoleColor.Yellow,   // Second best food 2 points
        Bad = ConsoleColor.White        // worst food 1 point
    }
}

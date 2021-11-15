using System;

namespace Snake
{
    // Used for recording the current players score. Used in HighScore.
    record PlayerScore(string PlayerName, int Score, int PlayTime, int Difficulty);

    // As the name says, this is a shared scoreboard for the current game loop
    public class ScoreBoard
    {
        public int Score { get; set; }
        public int Difficulty { get; set; }
        public int PlayTime { get; set; }

        private static readonly Lazy<ScoreBoard> scoreBoard = new Lazy<ScoreBoard>(() => new ScoreBoard());
        public static ScoreBoard ScoreBoardInstance
        {
            get => scoreBoard.Value;
        }
    }
}

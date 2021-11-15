using System;
using System.Linq;

namespace Snake
{
    public class GameMenu
    {
        private static string PlayerName { get; set; } = null;

        /// <summary>
        /// Run the method where the main loop resides. The Game menu.
        /// </summary>
        public static void GenerateGameMenu()
        {
            Action CallChoosenMethod;    // This delegate is used to keep the code more neat and clean
            Console.SetCursorPosition(0, 0);

            while (true)
            {
                #pragma warning disable CA1416 // Validate platform compatibility
                Console.SetWindowSize(100, 20);
                Console.SetBufferSize(100, 20);
                Console.CursorVisible = true;
                int choosenOption = ShowGameMenu();

                // Depening of what the user chose, call said method
                CallChoosenMethod = choosenOption switch
                {
                    1 => StartNewGame,
                    2 => ShowHighScore,
                    3 => HowToPlay,
                    4 => RollCredits,
                    5 => ExitGame,
                    _ => ExitGame   // Not actually nessecary. But removes annoying warning
                };

                CallChoosenMethod();
            }
        }

        /// <summary>
        /// Show the game menu.
        /// </summary>
        /// <returns>An int between 1 and 5 (inclusive) that represents the choosen value</returns>
        private static int ShowGameMenu()
        {
            Console.Clear();
            Console.WriteLine("(1) : New Game");
            Console.WriteLine("(2) : HighScore");
            Console.WriteLine("(3) : How To Play");
            Console.WriteLine("(4) : Credits");
            Console.WriteLine("(5) : Exit game");

            int? ChoosenOption = 0;
            bool validInput;
            do
            {
                Console.Write(ChoosenOption == null ? "Not a valid input. 1 - 4: " : "\r");
                string userChoice = Console.ReadLine();

                ChoosenOption = userChoice switch
                {
                    "1" => 1,
                    "2" => 2,
                    "3" => 3,
                    "4" => 4,
                    "5" => 5,
                    _ => null
                };

                validInput = ChoosenOption != null;
            } while (!validInput);

            return (int)ChoosenOption;
        }

        /// <summary>
        /// Show the top 10 scores (The only ones stored) in sorted order
        /// </summary>
        private static void ShowHighScore()
        {
            Console.Clear();

            HighScore highScore = HighScore.HighScoreInstance;
            PlayerScore[] playerScores = highScore.GetHighScores();

            Console.WriteLine($"{"", 4}{"Name", -13}{"Score", -8}{"Time", -8}Difficulty");
            for(int i = 0; i < playerScores.Length; i++)
            {
                // More fun seeing the difficulty in text, rather than numbers
                string difficultyText = playerScores[i].Difficulty switch
                {
                    1 => "Easy",
                    2 => "Medium",
                    3 => "Hard",
                    _ => ""
                };

                Console.WriteLine($"{i+1, -2}: {playerScores[i].PlayerName, -13}{playerScores[i].Score, -8}{playerScores[i].PlayTime, -8}{difficultyText}");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// Reset the game settings and start a new game
        /// </summary>
        private static void StartNewGame()
        {
            ScoreBoard ScoreBoard = ScoreBoard.ScoreBoardInstance;
            GameUpdateTimer timer = GameUpdateTimer.TimerInstance;
            ScoreBoard.PlayTime = 0;
            timer.GameIsEnding = false;
            timer.GameHasEnded = false;
            // Tuples!!!!
            (Vector2D[] arenaSize, int difficulty) = ChoosenDifficulty();

            SnakeGame game = new();
            game.StartGame(arenaSize, difficulty, PlayerName);   // Start the game
        }

        /// <summary>
        /// Returns a Vector2D array and an interger in the range of 1 to 3 (inclusive)
        /// </summary>
        /// <returns>A Vector2D array representing the arena size and an interger that represents the difficulty choosen</returns>
        private static (Vector2D[], int) ChoosenDifficulty()
        {
            Console.Clear();
            Console.WriteLine("(0) : Choose new player name");
            Console.WriteLine("(1) : Easy");    // Only portal walls            (4, 0) --> (20, 35)
            Console.WriteLine("(2) : Medium");  // Few portal walls             (4, 0) --> (15, 30)
            Console.WriteLine("(3) : Hard");    // Only walls, no portal walls  (4, 0) --> (15, 30)

            Vector2D[] arenaSize;
            int? difficulty = 0;    // Nullable because of checking on the input. Could be anything, But wanted nullable
            bool validInput;
            do
            {
                Console.Write(difficulty == null ? "Not a valid input. 1 - 3: " : "\r");
                string input = Console.ReadLine();

                while (input == "0")    // Choose your name dear player
                {
                    Console.WriteLine("Enter your name (Maximum 12 characters)");
                    PlayerName = Console.ReadLine();
                    PlayerName = PlayerName.Length > 12 ? PlayerName.Remove(11) : PlayerName;   // If the name is greater 12 characters. Cut it down to 12
                    Console.WriteLine("Choose a fifficulty");   // As it says, choose a fifficulty
                    input = Console.ReadLine();
                }

                // TUUUUUUUPLES!
                (arenaSize, difficulty) = input switch
                {
                    "1" => (new Vector2D[] { new(4, 0), new(20, 35) }, 1),
                    "2" => (new Vector2D[] { new(4, 0), new(15, 30) }, 2),
                    "3" => (new Vector2D[] { new(4, 0), new(15, 30) }, 3),
                    _ => (new Vector2D[] { new(4, 0), new(4, 0) }, (int?)null)
                };

                validInput = difficulty != null;
            } while (!validInput);

            return (arenaSize, (int)difficulty);
        }

        /// <summary>
        /// Exit the game. Kill the process
        /// </summary>
        private static void ExitGame() => Environment.Exit(0);

        /// <summary>
        /// :)
        /// </summary>
        private static void RollCredits()
        {
            Console.Clear();
            Console.WriteLine("Written by yours truly");
            Console.WriteLine("......................\n\n");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// Shows the noobs how to play the game
        /// </summary>
        private static void HowToPlay()
        {
            Console.Clear();
            Console.WriteLine("Change directions with: W, A, S, D. OR arrow keys");
            Console.WriteLine("Return to Main Menu at any time by pressing the ESCAPE key");
            Console.WriteLine("─ or │: represents a portal. Passing through will send you to the opposite side");
            Console.WriteLine("█ : represents a wall. Yeah, don't touch it");
            Console.WriteLine("☼ : represents food. Green = 3 points, Yellow = 2 points, white = 1 point");
            Console.ReadKey();
        }
    }
}

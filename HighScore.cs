using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Snake
{
    class HighScore
    {
        private PlayerScore[] PlayerScores = new PlayerScore[10];   // The highscore. up to 10 peeps
        private readonly string Path = "SnakeHighscores.csv";

        private HighScore() 
        {
            if (!File.Exists(Path))
            {
                // Create default highscores
                PlayerScores[0] = new("Pigh", 42, 82, 3);
                PlayerScores[1] = new("Fido", 37, 75, 1);
                PlayerScores[2] = new("Niant", 30, 54, 2);
                PlayerScores[3] = new("Gengar", 61, 118, 3);
                for (int i = 4; i < PlayerScores.Length; i++)
                {
                    PlayerScores[i] = new(null, 0, 9999, 0);
                }

                // Sort it, and write it to the file
                SortHighScores();
                WriteToCsvFile();
            }
            else
            {
                ReadFromCsvFile();
            }
        }

        // Singletons!
        private static readonly Lazy<HighScore> High = new Lazy<HighScore>(() => new HighScore());
        public static HighScore HighScoreInstance
        {
            get => High.Value;
        }

        // Read the Highscore and names
        private void ReadFromCsvFile()
        {
            string[] dataFromCsv = File.ReadAllLines(Path); // Get an array of every line

            // Perfect place for a try catch. Anything can happen when reading outside files.
            // Kind of like parsing errors when extracting the code
            try
            {
                for (int i = 1; i < dataFromCsv.Length; i++)    // Start i from 1, so we don't include "Name" and "Score"
                {
                    string[] data = dataFromCsv[i].Split(';');
                    PlayerScores[i - 1] = new PlayerScore(data[0], Int32.Parse(data[1]), Int32.Parse(data[2]), Int32.Parse(data[3]));
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"Something went wrong reading the file {Path}, make sure it's valid syntax. Name: String, Score: Int, Time: Int, Difficulty: Int");
                Console.WriteLine(e);
                System.Threading.Thread.Sleep(10000);
            }
        }

        // Update the highscore
        private void WriteToCsvFile()
        {
            string text = "Name;Score;Time;Difficulty" + Environment.NewLine;
            for(int i = 0; i < PlayerScores.Length; i++)
            {
                // Separate everything by semicolon (Semicoloon will separate the values by cells when opening in software like excel), and add a new line for readability
                text += $"{PlayerScores[i].PlayerName};{PlayerScores[i].Score};{PlayerScores[i].PlayTime};{PlayerScores[i].Difficulty}" + Environment.NewLine;
            }

            File.WriteAllText(Path, text);
        }

        /// <summary>
        /// Update the highscore if the provided score is higher than any current highscore in the file
        /// </summary>
        /// <param name="name">players name</param>
        /// <param name="score">players score</param>
        /// <param name="time">players play time</param>
        /// <param name="difficulty">players difficuly 1-3</param>
        public void UpdateHighScore(PlayerScore newPlayerScore)
        {
            PlayerScore temp;

            void AddNewPerson(int index)    // Local function to access this methods variables
            {
                for (int j = index; j < PlayerScores.Length; j++)
                {
                    temp = PlayerScores[j];
                    PlayerScores[j] = newPlayerScore;
                    newPlayerScore = temp;
                }
            };

            // The highscore is sorted by Score, Difficulty and then time played
            // The difference with these ifs, is only the criteria.
            for(int i = 0; i < PlayerScores.Length; i++)
            {
                if(newPlayerScore.Score > PlayerScores[i].Score)
                {
                    AddNewPerson(i);
                    break;
                }
                else if (newPlayerScore.Score == PlayerScores[i].Score && newPlayerScore.Difficulty >= PlayerScores[i].Difficulty)
                {
                    AddNewPerson(i);
                    break;
                }
                else if (newPlayerScore.Score == PlayerScores[i].Score && newPlayerScore.PlayTime <= PlayerScores[i].PlayTime && newPlayerScore.Difficulty >= PlayerScores[i].Difficulty)
                {
                    AddNewPerson(i);
                    break;
                }
            }

            SortHighScores();
        }

        /// <summary>
        /// Sort the PlayerScores array starting with score, difficulty and then Play time
        /// </summary>
        private void SortHighScores()
        {
            IEnumerable<PlayerScore> sorted = from p in PlayerScores
                                              orderby p.Score, p.Difficulty, p.PlayTime descending
                                              select p;
            PlayerScores = sorted.ToArray();
            Array.Reverse(PlayerScores);

            WriteToCsvFile();
        }

        /// <summary>
        /// Gets the array of players name, and highscores, time and difficulty. 
        /// </summary>
        /// <returns>An array of Players highscore</returns>
        public PlayerScore[] GetHighScores() => PlayerScores;
    }
}
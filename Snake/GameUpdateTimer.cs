using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Snake
{
    // Static classes are not very thread safe. But singletons are more threadsafe than static classes
    public class GameUpdateTimer
    {
        private readonly ScoreBoard ScoreBoard = ScoreBoard.ScoreBoardInstance;
        private int GameUpdateInterval { get; set; } = 80;
        public bool GameHasEnded { get; set; }
        public bool GameIsEnding { get; set; }

        // Making use of a singleton here instead of making it all static.
        // This is a threadsafe way to make it public
        private GameUpdateTimer() { }   // Private constructor. Only one instance allowed
        private static readonly Lazy<GameUpdateTimer> timer = new Lazy<GameUpdateTimer>(() => new GameUpdateTimer());   // Create a Lazy<T> object that will only initialize the first instance when it is called upon
        public static GameUpdateTimer TimerInstance
        {
            get
            {
                return timer.Value;
            }
        }

        /// <summary>
        /// This timer only ticks once every 1000ms
        /// Adding 1 to GameTime
        /// </summary>
        public async void GameTimeTimer()
        {
            while (true)
            {
                if (GameWorld.IsThisBoolTrue(GameIsEnding))
                    return;

                await Task.Delay(1000);
                ScoreBoard.PlayTime++;
            }
        }

        /// <summary>
        /// The core of the rendering and collision detection. Renders the game, and detects collision
        /// </summary>
        public async void WorldUpdate()
        {
            GameWorld game = GameWorld.GameWorldInstance;
            IEnumerable<GameObject> playerObjects = game.CollisionObjects.FindAll((x) => x.GetObjectType() == ObjectType.Player);

            while (true)
            {
                // Stop the task if the game has ended
                if (GameWorld.IsThisBoolTrue(GameHasEnded))
                    return;

                // If the game is ending, we do not need to check for further collisions
                if (!GameWorld.IsThisBoolTrue(GameIsEnding))  // It is probably super unnessecary to do this on another thread since we delay right after.
                    ThreadPool.QueueUserWorkItem((x) => Collision2D.CollisionDetction2D());

                await Task.Delay(GameUpdateInterval);

                // Update every players position (including AI)
                foreach(Player player in playerObjects)
                {
                    if (!player.HasTeleported)
                    {
                        player.UpdatePlayerPosition();
                    }
                    else
                        player.HasTeleported = false;
                }
                // Render the game synchronously
                // If the rendering is done asynchronously, the rendering will be fu##ed. Since every task will share the same console, and console properties
                game.RenderPlayer();
                game.RenderInformation();
                game.RenderFood();
            }
        }
    }
}

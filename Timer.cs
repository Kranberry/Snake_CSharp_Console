using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace Snake
{
    // Static classes are not very thread safe. But singletons are more threadsafe than static classes
    public class Timer
    {
        /* TODO
         * Add a few difficulty levels. 
         * GameUpdateInterval will be lower with higher difficulty
         */
        private int GameUpdateInterval { get; set; } = 80;

        public int GameTime { get; set; } = 0;
        public bool GameEnd { get; set; }

        // Making use of a singleton here instead of making it all static.
        // This is a threadsafe way to make it public
        private Timer() { }
        private static readonly Lazy<Timer> timer = new Lazy<Timer>(() => new Timer());
        public static Timer TimerInstance
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
                if (GameEnd)
                    return;
                await Task.Delay(1000);
                GameTime++;
            }
        }

        /// <summary>
        /// Increment the value of ticks by 1 each set milliseconds
        /// </summary>
        /// <param name="delayMs">The delay between every tick of the timer in milliseconds</param>
        public async void WorldUpdate()
        {
            GameWorld game = GameWorld.GameWorldInstance;
            IEnumerable<GameObject> playerObjects = game.GameObjects.FindAll((x) => x.GetObjectType() == ObjectType.Player);

            game.Score = 0;

            while (true)
            {
                // Stop the task if the game has ended
                if (GameEnd)
                    return;

                await Task.Delay(GameUpdateInterval);

                // Update every players position (including AI)
                foreach(Player player in playerObjects)
                    player.UpdatePlayerPosition();
                // Render the game synchrounosly(?)
                // If the rendering is done asynchronously, the rendering will be fu##ed. Since every task will share the same console, and console properties
                game.RenderInformation();
                game.RenderPlayer();
                game.RenderFood();
            }
        }
    }
}

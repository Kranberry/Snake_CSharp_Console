using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    interface IFood
    {
        public int RewardPoints { get; set; }
        public GameWorld game { get; } 

        // If the food is eaten by the player
        public void OnEaten();

    }
}

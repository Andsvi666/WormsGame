using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WormsGame
{
    public class Tile
    {
        int type = 0;
        Worm currentWorm = null;

        public int Type
        {
            set
            {
                if(value < 0 || value > 4)
                {
                    Console.WriteLine("Tile value must be between 0 and 4. " +
                        "0 - empty tile, 1 - terrain tile, 2 and 3 - player is in tile, 4 - arrow");
                }
                else
                {
                    type = value;
                }
            }
            get { return type; }
        }

        public Worm CurrentWorm
        {
            set { currentWorm = value;}
            get { return currentWorm; }
        }
        public Tile() 
        {

        }

        public Tile(int type, Worm currentWorm)
        {
            Type = type;
            CurrentWorm = currentWorm;
        }
    }
}

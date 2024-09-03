using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WormsGame
{
    //Base map class that only hold name
    public class MapBase
    {
        public string name = "";

        public MapBase() { }

        public MapBase(string name)
        { 
            this.name = name;
        }

        //Displays info line in console
        public virtual void PrintInfoLine()
        {
            Console.WriteLine($"Base map only has a name {name}");
        }

    }
}

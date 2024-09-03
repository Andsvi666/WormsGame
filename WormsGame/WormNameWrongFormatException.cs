using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WormsGame
{
    public class WrongNameFormatException : Exception
    {
        public WrongNameFormatException() 
            : base("Worm name can't have spaces or be longer than 20 characters")
        {
            
        }
    }
}

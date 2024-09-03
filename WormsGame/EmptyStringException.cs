using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WormsGame
{
    public class EmptyStringException : Exception
    {
        public EmptyStringException() : base("Input can't be empty")
        {

        }
    }
}

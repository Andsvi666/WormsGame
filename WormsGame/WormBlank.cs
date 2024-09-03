using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WormsGame
{
    public abstract class WormBlank
    {
        public abstract string Name { get; set; }
        public abstract int Health { get; set; }
        public abstract void TakeDamage(int damage);
    }
}

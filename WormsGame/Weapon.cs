using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WormsGame
{
    public class Weapon : IExtendedPrintable
    {
        string[] possibleTypes = new string[]{"throwable", "ranged", "melle" };
        string name;
        string type;
        int damage;
        int range;
        int uses;

        public string Name
        {
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    Console.WriteLine("No value chosen for weapon name");
                }
                else if (value.Length > 20)
                {
                    Console.WriteLine("Chosen weapon name is too long, must be under 20 characters");
                }
                else
                {
                    name = value;
                }
            }
            get { return name; }
        }

        public string Type
        {
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    Console.WriteLine("No value chosen for weapon type");
                }
                else if (!possibleTypes.Contains(value))
                {
                    Console.WriteLine("There is no such weapon type");
                }
                else
                {
                    type = value;
                }
            }
            get { return type; }
        }

        public int Damage
        {
            set 
            {
                if (value < 0)
                {
                    Console.WriteLine("Chosen weapon damage value is bellow 0");
                }
                else
                {
                    damage = value;
                }
            }
            get { return damage; }
        }

        public int Range
        {
            set 
            {
                if (value < 1 && value > 100)
                {
                    Console.WriteLine("Chosen weapon range value must be between 1 and 100");
                }
                else
                {
                    range = value;
                }
            }
            get { return range; }
        }

        public int Uses
        {
            set
            {
                if (value < 1)
                {
                    Console.WriteLine("Weapon uses value is bellow 1");
                    return;
                }
                else
                {
                    uses = value;
                }
            }
            get { return uses; }
        }

        public Weapon() { }

        public Weapon(string name, string type, int damage, int range, int uses)
        {
            Name = name;
            Type = type;
            Damage = damage;
            Range = range;
            Uses = uses;
        }

        //Method prints info line of weapon
        public void PrintInfoLine(int id)
        {
            Console.WriteLine($"{id} Weapon's name: {name}, type: {type}," +
                $" damage: {damage}, range: {range}, uses: {uses}");
        }
    }
}

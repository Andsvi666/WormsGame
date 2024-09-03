using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WormsGame
{
    public class Worm : WormBlank, IPrintable
    {
        string name;
        int teamID;
        int health = 100;
        //1 - x coordinate, 2 - y coordinate
        Point position = new Point(0, 0);
        List<Weapon> weapons = new List<Weapon>();
        Map playingMap = null;
        //static variable to count all worms made
        public static int wormsCount;

        public override string Name
        {
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    Console.WriteLine("No value chosen for worm name");
                    return;
                }
                else if(value.Length > 10)
                {
                    Console.WriteLine("Chosen worm name is too long, must be under 20 characters");
                    return;
                }
                else
                {
                    name = value;
                }
            }
            get { return name; }
        }

        public int TeamID
        {
            set 
            {
                if(value == 2 || value == 3)
                {
                    teamID = value;
                }
                else
                {
                    Console.WriteLine("Team ID must be either 2 or 3");
                }
            }
            get { return teamID; }
        }

        public override int Health
        {
            set
            {
                if (value < 0 || value > 100)
                {
                    Console.WriteLine("Health must be between 0 and 100");
                }
                else
                {
                    health = value;
                }
            }
            get { return health; }
        }

        public List<Weapon> Weapons
        {
            set 
            {
                if (value.Count < 1)
                {
                    Console.WriteLine("Starting weapon list must have at least 1 weapon");
                }
                else
                {
                    foreach (Weapon weapon in value)
                    {
                        Weapons.Add(weapon);
                    }
                }
            }
            get { return weapons; }
        }

        public Point Position
        {
            set 
            { 
                if(value == null)
                {
                    Console.WriteLine("Given position point is empty");
                }
                else if (value.X < 0 || value.Y < 0)
                {
                    Console.WriteLine("Position coordinates must be size 0 or higher");
                }
                else
                {
                    position = value;
                }
            }
            get { return position; }
        }

        public Map PlayingMap
        {
            set 
            { 
                if(value != null)
                {
                    playingMap = value;
                }
                else
                {
                    Console.WriteLine("Selected map is empty");
                }
            }
            get { return playingMap; }
        }

        public Worm()
        {
            wormsCount++;
        }

        public Worm(string name, int teamID, int health, Point position, List<Weapon> weapons, Map playingMap)
        {
            Name = name;
            TeamID = teamID;
            Health = health;
            Position = position;
            Weapons = weapons;
            PlayingMap = playingMap;
            wormsCount++;
        }

        public Worm(string name, int teamID, int health, Point position)
        {
            Name = name;
            TeamID = teamID;
            Health = health;
            Position = position;
            wormsCount++;
        }

        //Displays weapons in console
        public void DisplayWeapons()
        {
            int count = 1;
            foreach(Weapon wp in weapons)
            {
                wp.PrintInfoLine(count);
                count++;
            }
        }

        //Worm takes given damage, if it is greater than current worms health then health is reduced to 0 instead
        public override void TakeDamage(int count)
        {
            if(health - count < 0)
            {
                health = 0;
            }
            else
            {
                health = health - count;
            }
        }

        //Method removes 1 use from selected weapon, if weapon ran out of uses it is removed from worm
        public void RemoveWeaponUse(Weapon selectedWeapon)
        {
            foreach (Weapon weapon in weapons)
            {
                if(weapon.Name.Equals(selectedWeapon.Name))
                {
                    weapon.Uses--;
                    if(weapon.Uses == 0)
                    {
                        weapons.Remove(weapon);
                    }
                }
            }
        }

        //Method checks if given weapon name is in weapon list
        public bool CheckWeapon(string weaponName)
        {
            foreach(Weapon weapon in weapons)
            {
                if(weapon.Name.Equals(weaponName))
                {
                    return true;
                }
            }
            return false;
        }

        //Method prints worm info and list of owned weapons
        public void PrintInfoLine()
        {
            Console.WriteLine($"Worm's name: {name}, Team ID: {teamID}, health: {health}," +
                $" current position: X = {position.X}, Y = {Position.Y}" +
                $" number of weapons: {weapons.Count}, playing map: {playingMap}");
            if(weapons.Count > 0)
            {
                Console.WriteLine("List of weapon: ");
                int count = 1;
                foreach (Weapon w in weapons)
                {
                    w.PrintInfoLine(count);
                    count++;
                }
            }
        }
    }
}

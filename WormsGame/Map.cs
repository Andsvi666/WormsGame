using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WormsGame
{
    //Map class that inherets base map class name and printInfoline method
    public class Map : MapBase, IExtendedPrintable, IPrintable
    {
        Tile[,] tiles;
        public int length = 0;
        public int height = 0;

        public Tile[,] Tiles
        {
            set 
            { 
                if(tiles == null)
                {
                    Console.WriteLine("Give array of tiles is empty");
                }
                else
                {
                    tiles = value;
                }
            }
            get { return tiles; }
        }

        public Map() { }

        //Constructor for just tiles
        public Map(Tile[,] tiles)
        {
            Tiles = tiles;
        }

        //Constructor for jump template
        public Map(string name, int length, int height)
        {
            this.name = name;
            this.length = length;
            this.height = height;
        }

        //method sets map size 
        public void SetSize()
        {
            if (height != 0)
            {
                if (length != 0)
                {
                    tiles = new Tile[length, height];
                    for (int i = 0; i < length; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            tiles[i, j] = new Tile();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Map length is not yet set");
                }
            }
            else
            {
                Console.WriteLine("Map height is not yet set");
            }
        }

        //method from MapBase class that fills rows of map
        public void FillRow(int rowNumber, int[] rowIndexes)
        {
            foreach (int index in rowIndexes)
            {
                tiles[index, rowNumber].Type = 1;
            }
        }

        //Returns list of possible spawn points
        public List<Point> GetPossibleSpawnPoints()
        {
            List<Point> spawnSpots = new List<Point>();
            for (int i = 0; i < length; i++)
            {
                for (int j = height - 1; j >= 0; j--)
                {
                    if (tiles[i, j].Type == 0)
                    {
                        if(height - j - 1 != 0)
                        {
                            spawnSpots.Add(new Point(i,  j));
                        }
                        break;
                    }
                }
            }
            return spawnSpots;
        }

        //Inhereted virtual method removes range between 2 given worms
        public double GetRangeBetweenWorms(Worm worm1, Worm worm2)
        {
            double range = 0;
            int x1 = worm1.Position.X;
            int x2 = worm2.Position.X;
            int y1 = worm1.Position.Y;
            int y2 = worm2.Position.Y;
            double x = x1 - x2;
            double y = y1 - y2;
            //Pythagorian theorem for finding distance between 2 points, +1 for rounding
            range = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
            return range;
        }

        //Inhereted virtual method returns list of tiles where worms shot path is
        public List<Tile> GetPath(double len, Worm startWorm, Worm endWorm)
        {
            List<Tile> path = new List<Tile>();
            //length and height difference between worms
            double difX = endWorm.Position.X - startWorm.Position.X;
            double difY = endWorm.Position.Y - startWorm.Position.Y;
            //vector unit for each bullet position
            double dx = difX / len;
            double dy = difY / len;
            double ySum = 0;
            double xSum = 0;
            int tileX = 0;
            int tileY = 0;
            for (int i = 0; i < len - 1; i++)
            {
                xSum = xSum + dx;
                ySum = ySum + dy;
                //Converts double sum to int so it can be used as index
                tileX = Convert.ToInt32(xSum);
                tileY = Convert.ToInt32(ySum);
                path.Add(tiles[startWorm.Position.X + tileX, startWorm.Position.Y + tileY]);
            }
            return path;
        }

        //Inhereted virtual method removes worm from the map
        public void RemoveWorm(Worm givenWorm)
        {
            tiles[givenWorm.Position.X, givenWorm.Position.Y].CurrentWorm = null;
            tiles[givenWorm.Position.X, givenWorm.Position.Y].Type = 0;
            tiles[givenWorm.Position.X, givenWorm.Position.Y - 1].Type = 0;
        }

        //Inhereted virtual method displays where worm is by adding arrow above it
        public void HighlightWorm(Worm givenWorm)
        {
            tiles[givenWorm.Position.X, givenWorm.Position.Y - 1].Type = 4;
            PrintInfoLine(0);
        }

        //Inhereted virtual method removes arrow that displayed the worm
        public void RemoveHightlight(Worm givenWorm)
        {
            tiles[givenWorm.Position.X, givenWorm.Position.Y - 1].Type = 0;
            PrintInfoLine(0);
        }

        //Method clears console and prints current map tiles
        public void PrintInfoLine(int time)
        {
            Thread.Sleep(time);
            ResetMap();
            string line = "+" + new string('-', length) + "+";
            Console.WriteLine(line);
            for (int i = 0; i < height; i++)
            {
                Console.Write("|");
                for (int j = 0; j < length; j++)
                {
                    //Terrain  tile
                    if(tiles[j, i].Type == 1)
                    {
                        Console.Write("▓");
                    }
                    //Empty tile
                    else if(tiles[j, i].Type == 0)
                    {
                        Console.Write(" ");
                    }
                    //Player 1 worm
                    else if (tiles[j, i].Type == 2)
                    {
                        Console.Write("X");
                    }
                    //Player 2 worm
                    else if (tiles[j, i].Type == 3)
                    {
                        Console.Write("O");
                    }
                    else if (tiles[j, i].Type == 4)
                    {
                        Console.Write("↓");
                    }
                }
                Console.Write("|");
                Console.WriteLine();
            }
            Console.WriteLine(line);
        }

        //Inhereted virtual method rints empty lines where map would have been
        public void ResetMap()
        {
            Console.SetCursorPosition(0, 4);
            for (int i = 0; i < height + 2; i++)
            {
                Console.WriteLine(new String(' ', Console.WindowWidth));
            }
            Console.SetCursorPosition(0, 4);
        }

        //inhered method displays info line in console
        public override void PrintInfoLine()
        {
            Console.WriteLine($"{name}, length: {length}, height: {height}");
        }
    }
}

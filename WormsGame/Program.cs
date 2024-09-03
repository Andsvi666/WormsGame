using System;
using System.Data;
using System.Drawing;
using System.Xml.Linq;

namespace WormsGame
{
    public class Program
    {
        static void Main(string[] args)
        {
            string file = ChooseGame();
            if (file == "")
            {
                NewGame();
            }
            else
            {
                ResumeGameFromFile(file);
            }
        }

        //Method for user to choose game
        public static string ChooseGame()
        {
            string fileName = "";
            while(true)
            {
                Console.WriteLine("Do you want to start new game or continue game?(Y - new game, N - continue game)");
                string ans = Console.ReadKey(true).KeyChar.ToString();
                if (ans == "N" || ans == "n")
                {
                    string path = "./../../../../GameLogs";
                    string[] fileNames = Directory.GetFiles(path);
                    while(true)
                    {
                        Console.WriteLine($"Which game do you want to continue?(from 1 to {fileNames.Length})");
                        for (int i = 0; i < fileNames.Length; i++)
                        {
                            string name = fileNames[i];
                            Console.WriteLine(i + 1 + " " + name.Substring(23));
                        }
                        string answer = Console.ReadLine();
                        int num = 0;
                        if(Int32.TryParse(answer, out num))
                        {
                            if(num < 1 || num > fileNames.Length)
                            {
                                Console.WriteLine($"Number must be between 1 and {fileNames.Length}");
                            }
                            else
                            {
                                fileName = fileNames[num - 1];
                                Console.WriteLine($"You chose game {fileName.Substring(23)}");
                                return fileName;
                            }
                        }
                        else
                        {
                            Console.WriteLine("You need to write number");
                        }
                    }
                    
                }
                else if (ans == "Y" || ans == "y")
                {
                    return "";
                }
                else
                {
                    Console.WriteLine("You need to press Y or N button");
                }
            }
            return fileName;
        }

        //--------------------Methods for setting game to continue--------------------
        //Main method for resuming game from log file
        public static void ResumeGameFromFile(string path)
        {
            string[] gameLog = File.ReadAllLines(path);
            int round = ChooseRound(gameLog);
            Map chosenMap = ChooseMap(MapNameFromLog(gameLog, round));
            int groupCount = GetGroupCountFromLog(gameLog, round);
            List<Worm> allWorms = GenerateWormsFromLog(gameLog, round, groupCount, chosenMap);
            PrintRules(chosenMap.length);
            chosenMap.PrintInfoLine(0);
            UpdateMessegeBox($"{Worm.wormsCount} worms spawned on map, game continues", true, chosenMap);
            List<string> newLog = ClearLogFromRound(gameLog, round);
            round--;
            GameMain(allWorms, chosenMap, groupCount, newLog, round);
        }

        //Method checks how many rounds happaned in log file and asks player to pick one
        public static int ChooseRound(string[] gameLog)
        {
            int rounds = 0;
            foreach (string line in gameLog)
            {
                if(line.Length > 20 && line.Substring(0, 17) == "------------Round")
                {
                    rounds++;
                }
            }
            if(rounds == 1)
            {
                return 1;
            }
            else if(rounds == 0)
            {
                Console.WriteLine("Selected game has no rounds to start in, so starting new game instead");
                NewGame();
            }
            else
            {
                while(true)
                {
                    Console.WriteLine($"Which round do you want to start in?(from 1 to {rounds})");
                    string answer = Console.ReadLine();
                    int num = 0;
                    if (Int32.TryParse(answer, out num))
                    {
                        if (num < 1 || num > rounds)
                        {
                            Console.WriteLine($"Number must be between 1 and {rounds})");
                        }
                        else
                        {
                            Console.WriteLine($"You chose round {num}");
                            return num;
                        }
                    }
                    else
                    {
                        Console.WriteLine("You need to write number");
                    }
                }
            }
            return -1;
        }

        //Method returns map name from gameLog at selected round
        public static string MapNameFromLog(string[] gameLog, int round)
        {
            string name = "";
            for (int i = 0; i < gameLog.Length; i++)
            {
                if (gameLog[i] == $"------------Round {round} starts------------")
                {
                    name = gameLog[i + 1].Substring(4);
                    break;
                }
            }
            return name;
        }

        //Method retuns number of players from game log at selected round
        public static int GetGroupCountFromLog(string[] gameLog, int round)
        {
            int count = 0;
            for (int i = 0; i < gameLog.Length; i++)
            {
                if (gameLog[i] == $"------------Round {round} starts------------")
                {
                    if(Int32.TryParse(gameLog[i + 2].Substring(11, 1), out count))
                    {
                        return count;
                    }
                    else
                    {
                        Console.WriteLine("GameLog file is formated incorrectly");
                    }
                    break;
                }
            }
            return count;
        }

        //Method generates list of worms from given game log and round and puts them on map
        public static List<Worm> GenerateWormsFromLog(string[] gameLog, int round, int count, Map map)
        {
            List<Worm> worms = new List<Worm>();
            for (int i = 0; i < gameLog.Length; i++)
            {
                if (gameLog[i] == $"------------Round {round} starts------------")
                {
                    int j = i + 3;
                    string nextLine = gameLog[j];
                    while(nextLine != "----------------------------------------")
                    {
                        string[] values = nextLine.Split(' ');
                        Worm worm = new Worm(values[1], int.Parse(values[2]), int.Parse(values[3]), new Point(int.Parse(values[4]), int.Parse(values[5])));
                        worm.PlayingMap = map;
                        j++;
                        nextLine = gameLog[j];
                        while (nextLine.Substring(0, 6) == "Weapon")
                        {
                            string[] valuesWpn = nextLine.Split("  ");
                            Weapon wp = new Weapon(valuesWpn[1], valuesWpn[2], int.Parse(valuesWpn[3]), int.Parse(valuesWpn[4]), int.Parse(valuesWpn[5]));
                            worm.Weapons.Add(wp);
                            j++;
                            nextLine = gameLog[j];
                        }
                        worms.Add(worm);
                        map.Tiles[worm.Position.X, worm.Position.Y].CurrentWorm = worm;
                        map.Tiles[worm.Position.X, worm.Position.Y].Type = worm.TeamID;
                    }
                }
            }
            if(worms.Count != count)
            {
                Console.WriteLine("Game log is incorrect: player number is not right");
                worms.Clear();
            }
            return worms;
        }

        //Method clears given long from chosen rounds so it  is ready to be used for continuation of the game
        public static List<string> ClearLogFromRound(string[] gameLog, int round)
        {
            List<string> newLog = new List<string>();
            for (int i = 0; i < gameLog.Length; i++)
            {
                if (gameLog[i] == $"------------Round {round} starts------------")
                {
                    break;
                }
                newLog.Add(gameLog[i]);
            }
            return newLog;
        }

        //--------------------Methods for new game--------------------
        //Main method for new game
        public static void NewGame()
        {
            int groupCount = ChooseTeamCount();
            Map chosenMap = ChooseMap("");
            List<Point> spawnPoints = chosenMap.GetPossibleSpawnPoints();
            Worm[] player1 = new Worm[groupCount];
            Worm[] player2 = new Worm[groupCount];
            GenerateWorms(player1, 2, chosenMap, 2);
            GenerateWorms(player2, 3, chosenMap, 3);
            SpawnWorms(chosenMap, player1, 1, spawnPoints);
            SpawnWorms(chosenMap, player2, 2, spawnPoints);
            List<Worm> allWorms = CombineWorms(player1, player2);
            PrintRules(chosenMap.length);
            chosenMap.PrintInfoLine(0);
            UpdateMessegeBox($"{Worm.wormsCount} worms spawned on map, game begins", true, chosenMap);
            GameMain(allWorms, chosenMap, groupCount, null, 0);
        }

        //Method for picking number of worms in both team
        public static int ChooseTeamCount()
        {
            int count = 0;
            while(true)
            {
                Console.WriteLine("How many worms do you want to have per team?(from 1 to 4)");
                string ans = Console.ReadLine();
                if(int.TryParse(ans, out count))
                {
                    if(count > 0 && count < 5)
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Number must be between 1 and 4");
                    }
                }
                else
                {
                    Console.WriteLine("You must write a number");
                }
            }
            return count;
        }

        //Method to choose on which map game will be played
        public static Map ChooseMap(string name)
        {
            Map generatedMap = new Map();
            while (true)
            {

                List<Map> mapTemplates = new List<Map>();
                List<string> mapNames = new List<string> {"Flatlands", "Mountains", "Islands"};
                string pickedName = name;
                if (name == "")
                {
                    mapTemplates.Add(new Map("Flatlands", 80, 6));
                    mapTemplates.Add(new Map("Mountains", 50, 25));
                    mapTemplates.Add(new Map("Islands", 100, 4));
                    Console.WriteLine($"Choose from one from {mapTemplates.Count} maps available:");
                    foreach (Map map in mapTemplates)
                    {
                        DisplayMap(map);
                    }
                    pickedName = Console.ReadLine();
                }
                if (mapNames.Contains(pickedName))
                {
                    generatedMap = new Map();
                    generatedMap.name = pickedName;
                    if (pickedName == "Flatlands")
                    {
                        generatedMap = GenerateFlatlands(generatedMap);
                    }
                    if (pickedName == "Mountains")
                    {
                        generatedMap = GenerateMountains(generatedMap);
                    }
                    if (pickedName == "Islands")
                    {
                        generatedMap = GenerateIslands(generatedMap);
                    }
                    break;
                }
                else
                {
                    Console.WriteLine("There is no map with such name");

                }
            }
            return generatedMap;
        }

        //Method displays MapBase or it is children info
        public static void DisplayMap(MapBase map)
        {
            map.PrintInfoLine();
        }

        //Method to make map Flatlands
        public static Map GenerateFlatlands(Map generatedMap)
        {
            generatedMap.height = 6;
            generatedMap.length = 80;
            generatedMap.SetSize();
            //Setting rows
            int[] fill = new int[generatedMap.length];
            for(int i = 0; i < generatedMap.length; i++)
            {
                fill[i] = i;
            }
            generatedMap.FillRow(4, fill);
            generatedMap.FillRow(5, fill);
            return generatedMap;
        }

        //Method to make map Mountains
        public static Map GenerateMountains(Map generatedMap)
        {
            generatedMap.height = 25;
            generatedMap.length = 50;
            generatedMap.SetSize();
            //Setting rows
            int[] fill = new int[generatedMap.length];
            for (int i = 0; i < generatedMap.length; i++)
            {
                fill[i] = i;
            }
            generatedMap.FillRow(6, new int[] { 0 });
            generatedMap.FillRow(7, new int[] { 0, 1});
            generatedMap.FillRow(8, new int[] { 0, 1, 2});
            generatedMap.FillRow(9, new int[] { 0, 1, 2, 3 });
            generatedMap.FillRow(10, new int[] { 0, 1, 2, 3, 4 });
            generatedMap.FillRow(11, new int[] { 0, 1, 2, 3, 4, 5, });
            generatedMap.FillRow(12, new int[] { 0, 1, 2, 3, 4, 5, 6,24, 49 });
            generatedMap.FillRow(13, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 23, 24,25, 48, 49 });
            generatedMap.FillRow(14, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 22, 23, 24, 25,26, 30, 31, 47, 48, 49 });
            generatedMap.FillRow(15, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 21, 22, 23, 24, 25, 26, 27, 29, 30, 31, 32, 46, 47, 48, 49 });
            generatedMap.FillRow(16, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 45, 46, 47, 48, 49 });
            generatedMap.FillRow(17, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 44, 45, 46, 47, 48, 49 });
            generatedMap.FillRow(18, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 43, 44, 45, 46, 47, 48, 49 });
            generatedMap.FillRow(19, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 42, 43, 44, 45, 46, 47, 48, 49 });
            generatedMap.FillRow(20, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 41, 42, 43, 44, 45, 46, 47, 48, 49 });
            generatedMap.FillRow(21, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14,15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37,38,40, 41, 42, 43, 44, 45, 46, 47, 48, 49 });
            generatedMap.FillRow(22, fill);
            generatedMap.FillRow(23, fill);
            generatedMap.FillRow(24, fill);
            return generatedMap;
        }

        //Method to make map Islands
        public static Map GenerateIslands(Map generatedMap)
        {
            generatedMap.height = 4;
            generatedMap.length = 100;
            generatedMap.SetSize();
            generatedMap.FillRow(3, new int[] {3, 4, 5, 6, 7, 8, 9, 10, 
                11, 12, 16, 17, 18, 19, 20, 21, 22, 23, 27, 31, 32, 33, 34, 35, 36, 37, 
                40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 54, 55, 56, 57, 58, 59,
            63, 64, 65, 66, 67,68,69,70, 71, 72, 73, 74, 75, 79, 80, 81, 82, 86,87, 90, 91, 92, 93, 94, 95 });
            return generatedMap;
        }

        //Method creates list of weapons for worms to use
        public static List<Weapon> GetAllWeapons()
        {
            List<Weapon> allWeapons = new List<Weapon>();
            allWeapons.Add(new Weapon("Baseball Bat", "melle", 50, 1, 20));
            allWeapons.Add(new Weapon("Katana", "melle", 75, 1, 5));
            allWeapons.Add(new Weapon("Shotgun", "ranged", 40, 4, 3));
            allWeapons.Add(new Weapon("Sniper Rifle", "ranged", 60, 15, 2));
            allWeapons.Add(new Weapon("Bazooka", "ranged", 70, 10, 1));
            allWeapons.Add(new Weapon("Pistol", "ranged", 25, 7, 5));
            allWeapons.Add(new Weapon("F-1 Grenade", "throwable", 40, 10, 2));
            allWeapons.Add(new Weapon("Holy Hand Grenade", "throwable", 80, 6, 1));
            return allWeapons;
        }

        //Method generates worms for a team
        public static void GenerateWorms(Worm[] group, int player, Map selectedMap, int id)
        {
            List<string> names = new List<string>();
            Console.WriteLine($"Player {player - 1} name your worms:");
            for (int i = 0; i < group.Length; i++)
            {
                while(true)
                {
                    Console.Write($"Worm {i + 1} name: ");
                    try
                    {
                        string name = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(name))
                        {
                            throw new EmptyStringException();
                        }
                        else if (name.Contains(' ') || name.Length > 20)
                        {
                            throw new WrongNameFormatException();
                        }
                        else if (names.Contains(name))
                        {
                            Console.WriteLine("This name is already selected in your team");
                        }
                        else
                        {
                            List<Weapon> allWeapons = GetAllWeapons();
                            List<Weapon> wormWeapons = new List<Weapon>();
                            wormWeapons.Add(allWeapons[5]);
                            group[i] = new Worm(name, player, 100, new Point(0, 0), wormWeapons, selectedMap);
                            names.Add(name);
                            break;
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        //Method returns list of all worms that will play the game
        public static List<Worm> CombineWorms(Worm[] team1, Worm[] team2)
        {
            List<Worm> allWorms = new List<Worm>();
            for (int i = 0; i < team1.Length; i++)
            {
                allWorms.Add(team1[i]);
                allWorms.Add(team2[i]);
            }
            return allWorms;
        }

        //Method spawns worms in possible spawns points on map
        public static void SpawnWorms(Map chosenMap, Worm[] group, int groupNum, List<Point> spots)
        {
            Random random = new Random();
            foreach (Worm worm in group)
            {
                Point chosen = spots[random.Next(spots.Count)];
                spots.Remove(chosen);
                worm.Position = chosen;
                chosenMap.Tiles[chosen.X, chosen.Y].CurrentWorm = worm;
                chosenMap.Tiles[chosen.X, chosen.Y].Type = 1 + groupNum;
            }
        }

        //----------------------Methods for the game------------------------
        //Main game method
        public static void GameMain(List<Worm> allWorms, Map mainMap, int count, List<string> log, int rnd)
        {
            bool gameGoing = true;
            int roundCount = rnd;
            List<string> gameLog = new List<string>();
            if (log != null)
            {
                gameLog = log;
            }
            while (gameGoing)
            {
                roundCount++;
                gameLog.Add($"------------Round {roundCount} starts------------");
                gameLog = AddInfoToLog(gameLog, mainMap, allWorms);
                gameLog.Add($"----------------------------------------");
                CheckLogSaving(gameLog, mainMap, allWorms);
                int action;
                foreach(Worm pickedWorm in allWorms.ToList())
                {
                    gameLog.Add($"~~~Worm {pickedWorm.Name} turn~~~");
                    //To check if worm can still walk or jump this turn, attacking automaticly ends the turn
                    List<string> actions = new List<string> {"Walk", "Jump", "Attack"};
                    bool notAttacked = true;
                    mainMap.HighlightWorm(pickedWorm);
                    mainMap.PrintInfoLine(0);
                    while(notAttacked)
                    {
                        action = PickAction(actions, pickedWorm.Name, mainMap);
                        switch (action)
                        {
                            case 1:
                                gameLog.Add($"Worm {pickedWorm.Name} chose walk option");
                                actions.Remove("Walk");
                                WormWalks(mainMap, pickedWorm, gameLog);
                                break;
                            case 2:
                                gameLog.Add($"Worm {pickedWorm.Name} chose jump option");
                                actions.Remove("Jump");
                                WormJumps(mainMap, pickedWorm, gameLog);
                                break;
                            case 3:
                                gameLog.Add($"Worm {pickedWorm.Name} chose attack option");
                                allWorms = WormAttacks(mainMap, pickedWorm, allWorms, gameLog);
                                string msg = GiveRandomWeapon(pickedWorm, gameLog);
                                UpdateMessegeBox(msg, true, mainMap);
                                mainMap.RemoveHightlight(pickedWorm);
                                notAttacked = false;
                                break;
                            default:
                                break;

                        }
                        //If worm died during his walking or jumping phase
                        if (pickedWorm.Health == 0)
                        {
                            allWorms.Remove(pickedWorm);
                            notAttacked = false;
                        }
                    }
                    int status = CheckTeams(allWorms);
                    if (status != 0)
                    {
                        gameLog.Add($"------------Game finished------------");
                        SaveLog(gameLog, mainMap, "Finished");
                        UpdateMessegeBox($"Player {status} has won, other player lost all his worms", false, mainMap);
                        gameGoing = false;
                        return;
                    }
                }
            }
        }

        //Method returns one of three action
        public static int PickAction(List<string> actions, string name, Map mainMap)
        {
            //Only time there is 1 action left it will be attack
            if(actions.Count == 1 && actions[0] == "Attack")
            {
                return 3;
            }
            string action = "";
            while (true)
            {
                UpdateMessegeBox($"{name} turn. {string.Join(", ", actions)}?", false, mainMap);
                action = Console.ReadLine();
                if (!actions.Contains(action))
                {
                    UpdateMessegeBox($"Action must be {string.Join(", ", actions)}", true, mainMap);
                }
                else
                {
                    break;
                }

            }
            if(action.Equals("Walk"))
            {
                return 1;
            }
            if (action.Equals("Jump"))
            {
                return 2;
            }
            if (action.Equals("Attack"))
            {
                return 3;
            }
            return 0;
        }

        //Main Worm walking method
        public static void WormWalks(Map mainMap, Worm pickedWorm, List<string> gameLog)
        {
            int count = 10;
            int ret;
            while (true)
            {
                ret = 0;
                UpdateMessegeBox($"To move right: R, left: L, to stop: X. Moves: {count}", false, mainMap);
                string key = Console.ReadKey(true).Key.ToString();
                if (key == "X")
                {
                    gameLog.Add($"Worm {pickedWorm.Name} stopped walking");
                    return;
                }
                if (count <= 0)
                {
                    gameLog.Add($"Worm {pickedWorm.Name} is out of walking points, so it stops");
                    UpdateMessegeBox($"Worm {pickedWorm.Name} is out of moves", true, mainMap);
                    return;
                }
                if (key == "L")
                {
                    gameLog.Add($"Worm {pickedWorm.Name} moves 1 tile left");
                    var removeOne = (int x) => x - 1;
                    ret = MoveWormATile(mainMap, pickedWorm, 0, removeOne);
                    count = count - 1 + ret;
                }
                if (key == "R")
                {
                    gameLog.Add($"Worm {pickedWorm.Name} moves 1 tile right");
                    var addOne = (int x) => x + 1;
                    ret = MoveWormATile(mainMap, pickedWorm, mainMap.length - 1, addOne);
                    count = count - 1 + ret;
                }
                if (CheckWaterDeath(mainMap, pickedWorm, gameLog))
                {
                    return;
                }
            }
        }

        //Main Worm jumping method
        public static void WormJumps(Map mainMap, Worm pickedWorm, List<string> gameLog)
        {
            while(true)
            {
                UpdateMessegeBox("Press L to jump left and R to jump Right", false, mainMap);
                string ans = Console.ReadKey(true).Key.ToString();
                if(ans != "R" && ans != "L")
                {
                    UpdateMessegeBox("You need to press L or R to jump", true, mainMap);
                }
                else
                {
                    int distance = 0;
                    if(ans == "R")
                    {
                        gameLog.Add($"Worm {pickedWorm.Name} jumped 4 tiles right");
                        distance = 4;
                    }
                    else
                    {
                        gameLog.Add($"Worm {pickedWorm.Name} jumped 4 tiles left");
                        distance = -4;
                    }
                    LandOnTile(mainMap, pickedWorm, distance, gameLog);
                    CheckWaterDeath(mainMap, pickedWorm, gameLog);
                    break;
                }
            }
        }

        //Main Worm attacking 
        public static List<Worm> WormAttacks(Map mainMap, Worm pickedWorm, List<Worm> allWorms, List<string> gameLog)
        {
            Weapon chosenWeapon = ChooseWeapon(mainMap, pickedWorm);
            if(chosenWeapon != null)
            {
                gameLog.Add($"Worm {pickedWorm.Name} equiped {chosenWeapon.Name}");
                UpdateMessegeBox($"Worm {pickedWorm.Name} equiped {chosenWeapon.Name}", true, mainMap);
                Worm attackedWorm = ChooseWormToAttack(mainMap, pickedWorm, allWorms);
                attackedWorm = AttackSuccess(mainMap, pickedWorm, attackedWorm, chosenWeapon, gameLog);
                if (attackedWorm != null)
                {
                    gameLog.Add($"Worm {pickedWorm.Name} attacked worm {attackedWorm.Name} for {chosenWeapon.Damage} hitpoints");
                    UpdateMessegeBox($"Worm {pickedWorm.Name} attacked worm {attackedWorm.Name} for {chosenWeapon.Damage} hitpoints", true, mainMap);
                    attackedWorm.TakeDamage(chosenWeapon.Damage);
                    if(attackedWorm.Health == 0)
                    {
                        mainMap.RemoveWorm(attackedWorm);
                        allWorms.Remove(attackedWorm);
                        mainMap.PrintInfoLine(0);
                        gameLog.Add($"Worm {attackedWorm.Name} got killed by worm {pickedWorm.Name}");
                        UpdateMessegeBox($"Worm {attackedWorm.Name} died", true, mainMap);
                    }
                }
                pickedWorm.RemoveWeaponUse(chosenWeapon);
            }
            else
            {
                gameLog.Add($"Worm {pickedWorm.Name} decided to not attack anyone");
            }
            return allWorms;
        }

        //Method moves worm on map by one tile to left or right depending on given anonymous function and returns 1 if worm didnt move
        public static int MoveWormATile(Map mainMap, Worm pickedWorm, int limit, Func<int, int> change)
        {
            int posX = pickedWorm.Position.X;
            int posY = pickedWorm.Position.Y;
            if (pickedWorm.Position.X == limit)
            {
                return 1;
            }
            else
            {
                //either add or remove 1 depending on given function
                posX = change(posX);
                Tile nextTile = mainMap.Tiles[posX, posY];
                Tile nextTileUp = mainMap.Tiles[posX, posY - 1];
                Tile nextTileDown = mainMap.Tiles[posX, posY + 1];
                //next tile is empty
                if (nextTile.Type == 0)
                {
                    //next tile bellow has terrain
                    if (nextTileDown.Type == 1)
                    {
                        ChangeLocation(mainMap, pickedWorm, posX, posY, 0);
                        return 0;
                    }
                    //Next tile bellow is a worm
                    if (nextTileDown.Type == 2 || nextTileDown.Type == 3)
                    {
                        return 1;
                    }
                    //Next tile bellow is empty
                    if (nextTileDown.Type == 0)
                    {
                        ChangeLocation(mainMap, pickedWorm, posX, posY, 1);
                        return 0;
                    }
                }
                //Next tile is terrain
                if(nextTile.Type == 1)
                {
                    //next tile up is empty
                    if(nextTileUp.Type == 0)
                    {
                        ChangeLocation(mainMap, pickedWorm, posX, posY, -1);
                        return 0;
                    }
                    //Next tile up is a worm
                    if (nextTileUp.Type == 2 || nextTileUp.Type == 3)
                    {
                        return 1;
                    }
                }
                //Next tile is a worm
                if (nextTile.Type == 2 || nextTile.Type == 3)
                {
                    return 1;
                }
            }
            return 0;
        }

        //Method checks lands worm after jumping
        public static void LandOnTile(Map mainMap, Worm pickedWorm, int distance, List<string> gameLog)
        {
            int posX = pickedWorm.Position.X;
            if(posX == 0 && distance < 0 || posX == mainMap.length - 1 && distance > 0)
            {
                UpdateMessegeBox($"Worm {pickedWorm.Name} is already at the edge of the map", true, mainMap);
                return;
            }
            int posY = pickedWorm.Position.Y;
            int landX = posX + distance;
            if(landX < 0 )
            {
                landX = 0;
            }
            if(landX > mainMap.length - 1)
            {
                landX = mainMap.length - 1;
            }
            int landY = posY;
            int bellowLandY = posY + 1;
            int aboveLandY = posY - 1;
            Tile landTile = mainMap.Tiles[landX, landY];
            Tile bellowLandTile = mainMap.Tiles[landX, bellowLandY];
            Tile aboveLandTile = mainMap.Tiles[landX, aboveLandY];
            while (true)
            {
                //landing tile is empty
                if(landTile.Type == 0)
                {
                    //bellow is terrain
                    if(bellowLandTile.Type == 1)
                    {
                        ChangeLocation(mainMap, pickedWorm, landX, landY, 0);
                        return;
                    }
                    //bellow is empty
                    if (bellowLandTile.Type == 0)
                    {
                        while(true)
                        {        
                            //if hit terrain
                            if (bellowLandTile.Type == 1)
                            {
                                ChangeLocation(mainMap, pickedWorm, landX, landY, 0);
                                return;
                            }
                            if(bellowLandY == mainMap.height - 1)
                            {
                                ChangeLocation(mainMap, pickedWorm, landX, landY, 1);
                                return;
                            }
                            //If after jump you fall on  another worm
                            if(bellowLandTile.Type == 2 || bellowLandTile.Type == 3)
                            {
                                gameLog.Add($"Worm {pickedWorm.Name} jumped on another worm so he bounced back");
                                UpdateMessegeBox($"Worm {pickedWorm.Name} jumped on another worm and bounced back", true, mainMap);
                                return;
                            }
                            landY = bellowLandY;
                            bellowLandY++;
                            landTile = mainMap.Tiles[landX, landY];
                            bellowLandTile = mainMap.Tiles[landX, bellowLandY];
                        }
                    }
                    if (bellowLandTile.Type == 2 || bellowLandTile.Type == 3)
                    {
                        gameLog.Add($"Worm {pickedWorm.Name} jumped on another worm so he bounced back");
                        UpdateMessegeBox($"Worm {pickedWorm.Name} jumped on another worm and bounced back", true, mainMap);
                        return;
                    }

                }
                //If landing tile is terrain
                if (landTile.Type == 1)
                {
                    while (true)
                    {
                        //if terrain is above and map ends
                        if (aboveLandTile.Type == 1 && aboveLandY == 0)
                        {
                            ChangeLocation(mainMap, pickedWorm, landX, landY, 0);
                            return;
                        }
                        //if landing tile turned to empty one
                        if(landTile.Type == 0)
                        {
                            ChangeLocation(mainMap, pickedWorm, landX, landY, 0);
                            return;
                        }
                        //if worm is above
                        if (aboveLandTile.Type == 2 || aboveLandTile.Type == 3)
                        {
                            gameLog.Add($"Worm {pickedWorm.Name} jumped on another worm so he bounced back");
                            UpdateMessegeBox($"Worm {pickedWorm.Name} jumped on another worm and bounced back", true, mainMap);
                            return;
                        }
                        landY = aboveLandY;
                        aboveLandY--;
                        landTile = mainMap.Tiles[landX, landY];
                        aboveLandTile = mainMap.Tiles[landX, aboveLandY];
                    }
                }
                //if landing tile has another worm in it
                if (landTile.Type == 2 || landTile.Type == 3)
                {
                    gameLog.Add($"Worm {pickedWorm.Name} jumped on another worm so he bounced back");
                    UpdateMessegeBox($"Worm {pickedWorm.Name} jumped on another worm and bounced back", true, mainMap);
                    return;
                }
                break;
            }
        }

        //Method for changing worm location with given coordinates and change number
        public static void ChangeLocation(Map mainMap, Worm pickedWorm, int x, int y, int change)
        {
            mainMap.RemoveWorm(pickedWorm);
            mainMap.Tiles[x, y+change].Type = pickedWorm.TeamID;
            mainMap.Tiles[x, y + change].CurrentWorm = pickedWorm;
            mainMap.Tiles[x, y + change - 1].Type = 4;
            pickedWorm.Position = new Point(x, y + change);
            mainMap.PrintInfoLine(0);
        }

        //Method checks if worm landed on water and kills it if that happaned
        public static bool CheckWaterDeath(Map mainMap, Worm pickedWorm, List<string> gameLog)
        {
            if (pickedWorm.Position.Y == mainMap.height - 1)
            {
                pickedWorm.Health = 0;
                mainMap.RemoveWorm(pickedWorm);
                mainMap.PrintInfoLine(0);
                gameLog.Add($"Worm {pickedWorm.Name} fell in water and died");
                UpdateMessegeBox($"Worm {pickedWorm.Name} fell into water and died", true, mainMap);
                return true;
            }
            return false;
        }

        //Method returns a weapon from worm weapon list when player picks one
        public static Weapon ChooseWeapon(Map mainMap, Worm pickedWorm)
        {
            Weapon pickedWeapon = null;
            while(true)
            {
                UpdateMessegeBox($"Choose one of worms weapons to use by number between 1 and {pickedWorm.Weapons.Count} (N for none):", false, mainMap);
                pickedWorm.DisplayWeapons();
                string ans = Console.ReadKey(true).KeyChar.ToString();
                if (int.TryParse(ans, out int num))
                {
                    if(num >= 1 && num <= pickedWorm.Weapons.Count)
                    {
                        pickedWeapon = pickedWorm.Weapons[num - 1];
                        break;
                    }
                    else
                    {
                        UpdateMessegeBox($"Number must be between 1 and {pickedWorm.Weapons.Count}", true, mainMap);
                    }
                }
                else if(ans == "N" || ans == "n")
                {
                    return pickedWeapon;
                }
                else
                {
                    UpdateMessegeBox("Chose weapon number from given list or type N", true, mainMap);
                }
            }
            return pickedWeapon;
        }

        //Method returns enemy team worm that player wants to attack
        public static Worm ChooseWormToAttack(Map mainMap, Worm pickedWorm, List<Worm> allWorms)
        {
            //if there is only 2 worms in the game
            if (allWorms.Count == 2)
            {
                Worm enemyWorm = allWorms.Where(w => w.TeamID != pickedWorm.TeamID).FirstOrDefault();
                mainMap.HighlightWorm(enemyWorm);
                UpdateMessegeBox($"Selected worm {enemyWorm.Name}", true, mainMap);
                mainMap.RemoveHightlight(enemyWorm);
                return enemyWorm;
            }
            UpdateMessegeBox("Choose worm you want to attack, N to switch, X to select", true, mainMap);
            while(true)
            {
                foreach(Worm enemyWorm in allWorms.Where(w => w.TeamID != pickedWorm.TeamID))
                {
                    mainMap.HighlightWorm(enemyWorm);
                    while(true)
                    {
                        UpdateMessegeBox($"Selected Worm {enemyWorm.Name}, health: {enemyWorm.Health}", false, mainMap);
                        string ans = Console.ReadKey(true).Key.ToString();
                        if (ans != "N" && ans != "X")
                        {
                            UpdateMessegeBox($"Press N to switch next worm or X to select", true, mainMap);
                        }
                        else if(ans == "N")
                        {
                            mainMap.RemoveHightlight(enemyWorm);
                            break;
                        }
                        else if(ans == "X")
                        {
                            //UpdateMessegeBox($"Selected worm {enemyWorm.Name}", mainMap.Length, true, mainMap.Height + 5);
                            mainMap.RemoveHightlight(enemyWorm);
                            return enemyWorm;
                        }
                    }
                }
            }
        }

        //Method checks Attack success, if attack hits another worm then that worm is returned
        public static Worm AttackSuccess(Map mainMap, Worm pickedWorm, Worm attackedWorm, Weapon chosenWeapon, List<string> gameLog)
        {
            double rangeToWorm = mainMap.GetRangeBetweenWorms(pickedWorm, attackedWorm);
            //if gun is ranged
            if (chosenWeapon.Type == "ranged")
            {
                List<Tile> projectilePath = mainMap.GetPath(rangeToWorm, pickedWorm, attackedWorm);
                int count = 0;
                foreach (Tile tile in projectilePath)
                {
                    count++;
                    //hit terrain on the path
                    if(tile.Type == 1)
                    {
                        gameLog.Add($"Worm {pickedWorm.Name} attack failed, shot at terrain");
                        UpdateMessegeBox($"Worm {pickedWorm.Name} shot at terrain", true, mainMap);
                        return null;
                    }
                    //hit wrong worm on the path, so now it is returned and damaged instead
                    if(tile.Type == 2 || tile.Type == 3)
                    {
                        //if path reaches chosen worm
                        if(attackedWorm.Name == tile.CurrentWorm.Name && attackedWorm.TeamID == tile.CurrentWorm.TeamID)
                        {
                            return attackedWorm;
                        }
                        //if path reaches other worm
                        else
                        {
                            attackedWorm = tile.CurrentWorm;
                            gameLog.Add($"Worm {pickedWorm.Name} shot hit wrong worm {attackedWorm.Name}");
                            UpdateMessegeBox($"Worm {pickedWorm.Name} shot hit wrong worm {attackedWorm.Name}", true, mainMap);
                            return attackedWorm;
                        }
                    }
                    //Bullet range ended without hitting anything
                    if(count >= chosenWeapon.Range)
                    {
                        gameLog.Add($"Worm {pickedWorm.Name} shot didn't reach anything");
                        UpdateMessegeBox($"Worm {pickedWorm.Name} shot didn't reach anything", true, mainMap);
                        return null;
                    }
                }
                return attackedWorm;
            }
            //if guns is melle or throwable
            else if (chosenWeapon.Type == "melle" || chosenWeapon.Type == "throwable")
            {
                int roundRange = Convert.ToInt32(rangeToWorm) + 1;
                if(roundRange > chosenWeapon.Range)
                {
                    gameLog.Add($"Worm {pickedWorm.Name} is too far from {attackedWorm.Name} to attack with {chosenWeapon.Name}");
                    UpdateMessegeBox($"Worm {pickedWorm.Name} is too far from {attackedWorm.Name} to attack with {chosenWeapon.Name}", true, mainMap);
                    return null;
                }
                else
                {
                    return attackedWorm;
                }
            }
            return null;
        }

        //Method picks one random weapon out of given weapon list and gives it to worm
        public static string GiveRandomWeapon(Worm pickedWorm, List<string> gameLog)
        {
            Random rnd = new Random();
            List<Weapon> allWeapons = GetAllWeapons();
            Weapon randomWp = allWeapons[rnd.Next(allWeapons.Count)];
            if (pickedWorm.CheckWeapon(randomWp.Name))
            {
                pickedWorm.Weapons.Find(weapon => weapon.Name.Equals(randomWp.Name)).Uses = randomWp.Uses;
                gameLog.Add($"Worm {pickedWorm.Name} already has {randomWp.Name}, so it is uses has been refilled");
                return $"Worm {pickedWorm.Name} already has {randomWp.Name}, so it is uses has been refilled";
            }
            else if (pickedWorm.Weapons.Count == 6)
            {
                gameLog.Add($"Worm {pickedWorm.Name} couldn't aquire {randomWp.Name} because weapon limit is reached");
                return $"Worm {pickedWorm.Name} couldn't aquire {randomWp.Name} because weapon limit is reached";
            }
            else
            {
                pickedWorm.Weapons.Add(randomWp);
                gameLog.Add($"Worm {pickedWorm.Name} aquired {randomWp.Name}");
                return $"Worm {pickedWorm.Name} aquired {randomWp.Name}";
            }
        }

        //Checks if both playersave any worms left, returns player number if other player doesnt have any
        public static int CheckTeams(List<Worm> allWorms)
        {
            bool team1 = true;
            bool team2 = true;
            foreach (Worm worm in allWorms)
            {
                if(worm.TeamID == 2)
                {
                    team1 = false;
                }
                if (worm.TeamID == 3)
                {
                    team2 = false;
                }
            }
            if(team1)
            {
                return 2;
            }
            if(team2)
            {
                return 1;
            }
            return 0;
        }

        //Method updates the messege box, by overwriting it
        public static void UpdateMessegeBox(string messege, bool val, Map mainMap)
        {
            int len = mainMap.length;
            int spot = mainMap.height + 6;
            //If messege is bigger than map then messege box is doubled in size
            if (messege.Length > len)
            {
                len = len * 2;
            }
            //Clearing messege box and other stuff bellow it
            Console.SetCursorPosition(0, spot - 1);
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(new String(' ', Console.WindowWidth));
            }
            Console.SetCursorPosition(0, spot - 1);
            string line = "+" + new string('-', len) + "+";
            Console.WriteLine(line);
            Console.WriteLine("|" + messege.PadRight(len) + "|");
            if (val)
            {
                Console.WriteLine("|" + new string("Press enter to continue").PadRight(len) + "|");
            }
            Console.WriteLine(line);
            if (val)
            {
                while (true)
                {
                    string key = Console.ReadKey(true).Key.ToString();
                    if (key.Equals("Enter"))
                    {
                        break;
                    }
                }
            }
        }

        //Method prints rules and explanation that is above the map
        public static void PrintRules(int len)
        {
            Console.Clear();
            string line = "+" + new string('-', len) + "+";
            Console.WriteLine(line);
            string rules1 = "Worm can do in one turn: move 10 tiles,";
            string rules2 = "jump once 4 tiles and attack, which ends turns.";
            string rules3 = "Player 1 worms - X, Player 2 worms - O";
            Console.WriteLine("|" + rules1.PadRight(len) + "|");
            Console.WriteLine("|" + rules2.PadRight(len) + "|");
            Console.WriteLine("|" + rules3.PadRight(len) + "|");
        }

        //----------------------Logging methods----------------------

        //Method to print info for spawning game next time
        public static List<string> AddInfoToLog(List<string> log, Map mainMap, List<Worm> allWorms)
        {
            log.Add($"Map {mainMap.name}");
            log.Add($"WormsCount {allWorms.Count}");
            foreach (Worm worm in allWorms)
            {
                log.Add($"Worm {worm.Name} {worm.TeamID} {worm.Health} {worm.Position.X} {worm.Position.Y}");
                foreach(Weapon weapon in worm.Weapons)
                {
                    log.Add($"Weapon  {weapon.Name}  {weapon.Type}  {weapon.Damage}  {weapon.Range}  {weapon.Uses}");
                }
            }
            return log;
        }

        //Method to varify if player wants to save game log
        public static void CheckLogSaving(List<string> log, Map mainMap, List<Worm> allWorms)
        {
            while(true)
            {
                UpdateMessegeBox($"Do you want to save game log?(y/n)", false, mainMap);
                string ans = Console.ReadKey(true).KeyChar.ToString();
                if(ans == "N" || ans == "n")
                {
                    return;
                }
                else if(ans == "Y" || ans == "y")
                {
                    SaveLog(log, mainMap, "");
                    return;
                }
                else
                {
                    UpdateMessegeBox($"Click button Y or N to answer", true, mainMap);
                }

            }

        }

        //Method to save game log in file
        public static void SaveLog(List<string> log, Map mainMap, string type)
        {
            string path = "./../../../../GameLogs";
            if (type == "Finished")
            {
                path = "./../../../../GameLogs/Finished Games";
            }
            int id = GetIDForFile(path, type);
            if(id == -1)
            {
                UpdateMessegeBox($"Wrong file in GameLogs folder", true, mainMap);
            }
            else
            {
                id++;
                File.WriteAllLines(path + "/" + type + "Game" + id + ".txt", log);
                UpdateMessegeBox($"Log created succesfully as {type}Game{id}.txt", true, mainMap);
            }
        }

        //Method returns last log i hd
        public static int GetIDForFile(string path, string type)
        {
            int len = 27;
            if(type == "Finished")
            {
                len = len + 23;
            }
            string[] fileNames = Directory.GetFiles(path);
            if (fileNames.Length > 0)
            {
                string lastGameName = fileNames.Last().Substring(len);
                int id;
                if (int.TryParse(lastGameName.Substring(0, lastGameName.Length - 4), out id))
                {
                    return id;
                }
            }
            else
            {
                return 0;
            }
            return -1;
        }
    }
}

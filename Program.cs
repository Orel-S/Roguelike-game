using System;
using System.Collections.Generic;

namespace Game
{
    
    public class Mapping
    {
        public const char WALL = '#';
        public const char FLOOR = ' ';
        public const char BLANK = '*';

        private void Initialize_Map(ref char[,] Map)
        {
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    Map[i, j] = '*';
                }
            }
        }
        public void Insert_Room(Room room, char[,] map, int x, int y)
        {
            for (int i = y; i < (y + room.Height); ++i)
            {
                for (int j = x; j < (x + room.Width); ++j)
                {
                    if (j == x || j == (x + room.Width) - 1 || i == y || i == (y + room.Height) - 1)
                    {
                        map[i, j] = WALL;
                    }
                    else
                    {
                        map[i, j] = FLOOR;
                    }
                }
            }
        }
        public bool Room_For_Room(Room room, char[,] map, int x, int y)
        {
            for (int i = y; i < (y + room.Height); ++i)
            {
                for (int j = x; j < (x + room.Width); ++j)
                {
                    if (i >= 50 || j >= 50 || i < 0 || j < 0)
                    {
                        return false;
                    }
                    if (map[i, j] == WALL || map[i, j] == FLOOR)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public void Generate_Rooms(char[,] map)
        {
            Random random = new Random();
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    Room room = new Room();
                    room.Width = random.Next(3, 6);
                    room.Height = random.Next(3, 6);
                    int Determine = random.Next(1, 10000);
                    if (Room_For_Room(room, map, j, i) && Determine < 1000000 && map[i, j] == BLANK)
                    {
                        Insert_Room(room, map, j, i);
                    }
                }
            }
        }
        public void Render_Map(char[,] map)
        {
            for (int i = 0; i < 50; ++i)
            {
                for (int j = 0; j < 50; ++j)
                {
                    Console.Write("{0}", map[i, j]);
                }
                Console.WriteLine();
            }
        }
        public void Fix_Neighbours(char[,] map)
        {
            //49 is key here to avoid out of bounds error
            for (int i = 1; i < 49; ++i)
            {
                for (int j = 1; j < 49; ++j)
                {
                    if (map[i,j] == WALL &&
                        map[i + 1, j] == WALL &&
                        (map[i, j - 1] != BLANK &&
                        map[i, j + 1] != BLANK) &&
                        (map[i + 1, j - 1] != BLANK &&
                        map[i + 1, j + 1] != BLANK))
                    {
                        map[i, j] = FLOOR;
                        map[i + 1, j] = FLOOR;
                    }
                }
            }
        }
        public void Fix_Walls(char[,] map)
        {
            // First loop cleans up empty spaces where walls should be
            for (int i = 1; i < 49; ++i)
            {
                for (int j = 1; j < 49; ++j)
                {
                    if (map[i, j] == FLOOR &&
                        (map[i - 1, j] == BLANK ||
                        map[i + 1, j] == BLANK ||
                        map[i , j + 1] == BLANK ||
                        map[i , j - 1] == BLANK ||
                        map[i - 1, j - 1] == BLANK ||
                        map[i + 1, j + 1] == BLANK ||
                        map[i + 1, j - 1] == BLANK ||
                        map[i - 1, j + 1] == BLANK))
                    {
                        map[i, j] = WALL;
                    } 
                }
            }
            // Second loop cleans up walls where empty spaces should be
            for (int i = 1; i < 49; ++i)
            {
                for (int j = 1; j < 49; ++j)
                {
                    if (map[i, j] == WALL &&
                        (map[i - 1, j] == FLOOR &&
                        map[i + 1, j] == FLOOR &&
                        map[i, j + 1] == FLOOR &&
                        map[i, j - 1] == FLOOR &&
                        map[i - 1, j - 1] == FLOOR &&
                        map[i + 1, j + 1] == FLOOR &&
                        map[i + 1, j - 1] == FLOOR &&
                        map[i - 1, j + 1] == FLOOR))
                    {
                        map[i, j] = FLOOR;
                    }
                }
            }
            // Third loop cleans up outer border areas
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; ++j)
                {
                    if ((i == 0 || i == 49 || j == 0 || j == 49) && map[i, j] == FLOOR)
                    {
                        map[i, j] = WALL;
                    }
                }
            }
        }
        public void Initialize_Walls(char[,] map)
        {
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; ++j)
                {
                    if (map[i, j] == BLANK)
                    {
                        map[i, j] = WALL;
                    }
                }
            }
        }

        public int Size_Area(char[,] map, int x, int y, ref HashSet<(int, int)> Counted_Coords)
        {
            List<(int, int)> Direction_List = new List<(int, int)>();
            if (Counted_Coords.Contains((x, y)))
            {
                return 0;
            }
            else
            {
                Counted_Coords.Add((x, y));
                
                if (map[x - 1, y] == FLOOR)
                {
                    Direction_List.Add((x - 1, y));
                }
                if (map[x + 1, y] == FLOOR)
                {
                    Direction_List.Add((x + 1, y));
                }
                if (map[x, y - 1] == FLOOR)
                {
                    Direction_List.Add((x, y - 1));
                }
                if (map[x, y + 1] == FLOOR)
                {
                    Direction_List.Add((x, y + 1));
                }
            }
            int sum = 1;
            foreach (var direction in Direction_List)
            {
                
                sum += Size_Area(map, direction.Item1, direction.Item2, ref Counted_Coords);
            }
            return sum;
        }

        public void Remove_Exterior_Rooms(char[,] map)
        {
            int counter = 1;
            for (int i = 1; i < 49; i++)
            {
                for (int j = 1; j < 49; ++j)
                {

                    HashSet<(int, int)> Temp = new HashSet<(int, int)>();
                    if (map[i, j] == FLOOR)
                    {
                        if (Size_Area(map, i, j, ref Temp) <= 500)
                        {
                            map[i, j] = WALL;
                            //                          Console.WriteLine("RER #: {0}", counter);
                            counter++;
                        }
                    }
                }
            }
        }
        public char[,] Create_Random_Map()
        {
            char[,] map = new char[50, 50];
            Initialize_Map(ref map);
            Generate_Rooms(map);
            Fix_Neighbours(map);
            Fix_Walls(map);
            Initialize_Walls(map);
            Remove_Exterior_Rooms(map);
            return map;
        }
    }
    public class Entity
    {
        private int[] location = new int[2];
        public int[] Location
        {
            get { return location; }
            set { location = value; }
        }
        private int healthPoints;
        public int HealthPoints
        {
            get { return healthPoints; }
            set { healthPoints = value; }
        }
        private int manaPoints;
        public int ManaPoints
        {
            get { return manaPoints; }
            set { manaPoints = value; }
        }
        //public attack[] attackArray = new attack[4];
        //public spell[] spellArray = new spell[4];
    }
    public class Player : Entity
    { }
    public class Room
    {
        private int width;
        public int Width
        {
            get { return width; }
            set { width = value; }
        }
        private int height;
        public int Height
        {
            get { return height; }
            set { height = value; }
        }
        public bool[] doors = new bool[4];
        public Room[] neighbours = new Room[4];
        public void Draw_Room(Room room)
        {
            for (int i = 0; i < room.height; ++i)
            {
                for (int j = 0; j < room.width; j++)
                {
                    if (i == 0 || i == height - 1 || j == 0 || j == width - 1)
                    {
                        Console.Write("|");
                    }
                    else
                    {
                        Console.Write("*");
                    }
                }
                Console.WriteLine();
            }

        }
        
    }
    class Program
    {

/*        public static void BuildTestRoom(int a, int b)
        {
            Room test_room = new Room();
            test_room.Width = a;
            test_room.Height = b;
            test_room.Draw_Room(test_room);}
 */      
        static void Main(string[] args)
        {
            Player player = new Player();            
            Mapping mapping = new Mapping();
            char[,] map = mapping.Create_Random_Map();
            mapping.Render_Map(map);
            Console.WriteLine("Program Compiled Successfully!");
        }
    }
}


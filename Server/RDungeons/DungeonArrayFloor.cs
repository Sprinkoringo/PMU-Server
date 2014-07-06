using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.RDungeons {
    public class DungeonArrayFloor {

        static readonly Random random = new Random(Environment.TickCount);

        public const int CLOSED = 0; //closed room/hall, doesn't exist
        public const int OPEN = 1; //open room/hall
        public const int HALL = 2; // room being treated as hall

        public const int START = 0; //starting position room
        public const int END = 1; //staircase room

        public const int UTILE = 0;//0 - Unspecified tile
        public const int UBLOCK = 1;//1 - Unspecified block
        public const int UWATER = 2;//2 - Unspecified Water
        public const int GROUND = 3;//3 - Ground
        public const int HALLTILE = 4;//4 - Hall
        public const int DOORTILE = 5;//5 - Entrance
        public const int STARTTILE = 6;//6 - Start
        public const int ENDTILE = 7;//7 - End
        public const int TRAPTILE = 8;//8 - trap
        public const int ITEMTILE = 9;//9 - item

        //256-511 - Wall tiles (2^8 = 256 possibilities for wall adjacency)
        //512-767 - Water tiles (2^8 = 256 possibilities for water adjacency)
        //1024 and above - Chamber data
        public DungeonPoint Start { get; set; }
        public DungeonPoint End { get; set; }
        public DungeonPoint Chamber { get; set; }

        public DungeonArrayRoom[,] Rooms { get; set; }
        public DungeonArrayHall[,] VHalls { get; set; }
        public DungeonArrayHall[,] HHalls { get; set; }
        public int[,] MapArray { get; set; }

        public DungeonArrayFloor(GeneratorOptions options, RDungeonChamberReq req) {
            Rooms = new DungeonArrayRoom[4, 4]; //array of all rooms
            VHalls = new DungeonArrayHall[4, 3]; //vertical halls
            HHalls = new DungeonArrayHall[3, 4]; //horizontal halls
            Start = new DungeonPoint(-1, -1);     //marks spawn point
            End = new DungeonPoint(-1, -1);       //marks stairs
            Chamber = new DungeonPoint(-1, -1);       //marks chamber
            int i, j, a = -1, b; //counter variables
            MapArray = new int[50, 50]; // stores map grid
            int x, y;      // used for navigating map array
            bool isDone;   // bool used for various purposes

            // Part of Options class now
            //int trapFreq = 66; // adjust to number between 0 and 100 to see changes
            //int trapMin = 5;  // adjust to number between 0 and 255 to see changes
            //int trapMax = 30; // adjust to number between 0 and 255 to see changes

            //initialize map array to empty
            for (y = 0; y < 50; y++) {
                for (x = 0; x < 50; x++) {
                    MapArray[x, y] = UBLOCK;
                }
            }

            //initialize all rooms+halls to closed by default
            for (x = 0; x <= Rooms.GetUpperBound(0); x++) {
                for (y = 0; y <= Rooms.GetUpperBound(1); y++) {
                    Rooms[x, y] = new DungeonArrayRoom();
                }
            }



            for (x = 0; x <= VHalls.GetUpperBound(0); x++) {
                for (y = 0; y <= VHalls.GetUpperBound(1); y++) {
                    VHalls[x, y] = new DungeonArrayHall();
                }
            }

            for (x = 0; x <= HHalls.GetUpperBound(0); x++) {
                for (y = 0; y <= HHalls.GetUpperBound(1); y++) {
                    HHalls[x, y] = new DungeonArrayHall();
                }
            }

            // path generation algorithm
            Start = new DungeonPoint(random.Next(0, Rooms.GetUpperBound(0) + 1), random.Next(0, Rooms.GetUpperBound(1) + 1)); // randomly determine start room
            DungeonPoint wanderer = Start;

            //start = random.Next(0, 16); // randomly determine start room
            //x = start;
            i = 0;
            j = random.Next(0, 6) + 5; // magic numbers, determine what the dungeon looks like (in general, paths)
            b = -1; // direction of movement
            do {
                if (random.Next(0, (2 + i)) == 0) {//will end the current path and start a new one from the start
                    if (random.Next(0, 2) == 0) {//determine if the room should be open or a hall
                        Rooms[wanderer.X, wanderer.Y].Opened = OPEN;
                    } else {
                        Rooms[wanderer.X, wanderer.Y].Opened = HALL;
                    }
                    i++;
                    wanderer = Start;
                    b = -1;
                } else {
                    bool working = true;
                    do {
                        DungeonPoint sample = wanderer;
                        y = random.Next(0, 4);
                        if (y != b) {//makes sure there is no backtracking
                            switch (y) {
                                case 0:

                                    sample.Y--;
                                    b = 1;
                                    break;
                                case 1:

                                    sample.Y++;
                                    b = 0;
                                    break;
                                case 2:

                                    sample.X--;
                                    b = 3;
                                    break;
                                case 3:

                                    sample.X++;
                                    b = 2;
                                    break;
                            }
                            if (sample.X >= 0 && sample.X <= Rooms.GetUpperBound(0) && sample.Y >= 0 && sample.Y <= Rooms.GetUpperBound(1)) {// a is the room to be checked after making a move between rooms
                                openHallBetween(wanderer, sample);
                                wanderer = sample;
                                working = false;
                            }
                        } else {
                            b = -1;
                        }
                    } while (working);

                    if (random.Next(0, 2) == 0) {//determine if the room should be open or a hall
                        Rooms[wanderer.X, wanderer.Y].Opened = OPEN;
                    } else {
                        Rooms[wanderer.X, wanderer.Y].Opened = HALL;
                    }
                }
            } while (i < j);

            Rooms[Start.X, Start.Y].Opened = OPEN;

            //Determine key rooms
            if (req == null || req.End != Enums.Acceptance.Always) {
                isDone = false;
                do { //determine ending room randomly
                    x = random.Next(0, Rooms.GetUpperBound(0) + 1);
                    y = random.Next(0, Rooms.GetUpperBound(1) + 1);
                    if (Rooms[x, y].Opened == OPEN) {
                        End = new DungeonPoint(x, y);
                        //Console.WriteLine(x);
                        isDone = true;
                    }
                } while (!isDone);
            }
            Start = new DungeonPoint(-1, -1);
            if (req == null || req.Start != Enums.Acceptance.Always) {
                isDone = false;
                do { //determine starting room randomly
                    x = random.Next(0, Rooms.GetUpperBound(0) + 1);
                    y = random.Next(0, Rooms.GetUpperBound(1) + 1);
                    if (Rooms[x, y].Opened == OPEN) {
                        Start = new DungeonPoint(x, y);
                        //Console.WriteLine(x);
                        isDone = true;
                    }
                } while (!isDone);
            }

            if (req != null) {
                isDone = false;
                for (int n = 0; n < 100; n++) { //determine chamber room randomly
                    x = random.Next(0, Rooms.GetUpperBound(0) + 1);
                    y = random.Next(0, Rooms.GetUpperBound(1) + 1);
                    bool approved = true;
                    //if the chamber cannot be on a start, and it picked the start
                    if (req.Start == Enums.Acceptance.Never && Start.X == x && Start.Y == y) {
                        approved = false;
                    }
                    //if the chamber cannot be on an end, and it picked the end
                    if (req.End == Enums.Acceptance.Never && End.X == x && End.Y == y) {
                        approved = false;
                    }
                    //if the chamber demands a certain setup, and it doesn't meet the setup
                    if (req.TopAcceptance == Enums.Acceptance.Always) {
                        if (y == 0 || !VHalls[x, y - 1].Open) approved = false;
                    } else if (req.TopAcceptance == Enums.Acceptance.Never) {
                        if (y > 0 && VHalls[x, y - 1].Open) approved = false;
                    }
                    if (req.BottomAcceptance == Enums.Acceptance.Always) {
                        if (y == Rooms.GetUpperBound(1) || !VHalls[x, y].Open) approved = false;
                    } else if (req.BottomAcceptance == Enums.Acceptance.Never) {
                        if (y < Rooms.GetUpperBound(1) && VHalls[x, y].Open) approved = false;
                    }
                    if (req.LeftAcceptance == Enums.Acceptance.Always) {
                        if (x == 0 || !HHalls[x - 1, y].Open) approved = false;
                    } else if (req.LeftAcceptance == Enums.Acceptance.Never) {
                        if (x > 0 && HHalls[x - 1, y].Open) approved = false;
                    }
                    if (req.RightAcceptance == Enums.Acceptance.Always) {
                        if (x == Rooms.GetUpperBound(0) || !HHalls[x, y].Open) approved = false;
                    } else if (req.RightAcceptance == Enums.Acceptance.Never) {
                        if (x < Rooms.GetUpperBound(0) && HHalls[x, y].Open) approved = false;
                    }
                    if (Rooms[x, y].Opened == OPEN && approved) {
                        //set chamber to the point
                        Chamber = new DungeonPoint(x, y);

                        //if start or end is on this point, set to this location
                        if (End.X == -1 && End.Y == -1) {
                            End = new DungeonPoint(x, y);
                        }
                        if (Start.X == -1 && Start.Y == -1) {
                            Start = new DungeonPoint(x, y);
                        }

                        //Console.WriteLine(x);
                        isDone = true;
                        break;
                    }
                }

                if (!isDone) {
                    //chamber could not be placed; reroll start and end if they were set to -1 assuming that chamber was going to take care of it
                    if (End.X == -1 && End.Y == -1) {
                        isDone = false;
                        do { //determine ending room randomly
                            x = random.Next(0, Rooms.GetUpperBound(0) + 1);
                            y = random.Next(0, Rooms.GetUpperBound(1) + 1);
                            if (Rooms[x, y].Opened == OPEN) {
                                End = new DungeonPoint(x, y);
                                //Console.WriteLine(x);
                                isDone = true;
                            }
                        } while (!isDone);
                    }

                    if (Start.X == -1 && Start.Y == -1) {
                        isDone = false;
                        do { //determine starting room randomly
                            x = random.Next(0, Rooms.GetUpperBound(0) + 1);
                            y = random.Next(0, Rooms.GetUpperBound(1) + 1);
                            if (Rooms[x, y].Opened == OPEN) {
                                Start = new DungeonPoint(x, y);
                                //Console.WriteLine(x);
                                isDone = true;
                            }
                        } while (!isDone);
                    }
                }
            }

            // begin part 2, creating ASCII map
            //create rooms
            //Console.WriteLine("ROOMS:");
            //Console.WriteLine(Chamber.X + "," + Chamber.Y);
            for (i = 0; i <= Rooms.GetUpperBound(0); i++) {
                for (j = 0; j <= Rooms.GetUpperBound(1); j++) {
                    if (Rooms[i, j].Opened != CLOSED && (Chamber.X != i || Chamber.Y != j)) {
                        createRoom(i, j, options, req);
                    }
                }
            }
            //Console.WriteLine(Chamber.X + "," + Chamber.Y);
            //create chamber
            if (Chamber.X > -1 && Chamber.Y > -1) {
                createChamber(options, req);
            }

            //Console.WriteLine("DRAW:");
            for (i = 0; i <= Rooms.GetUpperBound(0); i++) {
                for (j = 0; j <= Rooms.GetUpperBound(1); j++) {
                    if (Rooms[i, j].Opened != CLOSED) {
                        drawRoom(i, j);
                    }
                }
            }

            for (i = 0; i <= Rooms.GetUpperBound(0); i++) {
                for (j = 0; j <= Rooms.GetUpperBound(1); j++) {
                    if (Rooms[i, j].Opened != CLOSED) {
                        padSingleRoom(i, j);
                    }
                }
            }
            //Console.WriteLine("HALLS:");

            for (i = 0; i <= VHalls.GetUpperBound(0); i++) {
                for (j = 0; j <= VHalls.GetUpperBound(1); j++) {
                    if (VHalls[i, j].Open) {
                        createVHall(i, j, options, req);
                    }
                }
            }

            for (i = 0; i <= HHalls.GetUpperBound(0); i++) {
                for (j = 0; j <= HHalls.GetUpperBound(1); j++) {
                    if (HHalls[i, j].Open) {
                        createHHall(i, j, options, req);
                    }
                }
            }


            for (i = 0; i <= VHalls.GetUpperBound(0); i++) {
                for (j = 0; j <= VHalls.GetUpperBound(1); j++) {
                    if (VHalls[i, j].Open) {
                        DrawHalls(VHalls[i, j]);
                    }
                }
            }

            for (i = 0; i <= HHalls.GetUpperBound(0); i++) {
                for (j = 0; j <= HHalls.GetUpperBound(1); j++) {
                    if (HHalls[i, j].Open) {
                        DrawHalls(HHalls[i, j]);
                    }
                }
            }

            // create halls
            //for (i = 0; i < 12; i++) {
            //    if (VHalls[i] == OPEN) {
            //        createVHall(i, options);
            //    }
            //    if (HHalls[i] == OPEN) {
            //        createHHall(i, options);
            //    }
            //}
            //Console.WriteLine("SE:");
            if (Start.X != Chamber.X || Start.Y != Chamber.Y) {
                addSEpos(Start, START, req);
            }
            if (End.X != Chamber.X || End.Y != Chamber.Y) {
                addSEpos(End, END, req);
            }

            //Console.WriteLine("WATER:");
            //add water
            DrawWater(options);

            DrawCraters(options);



            //Console.WriteLine("TRAPS:");
            // add traps
            int finalTraps = random.Next(options.TrapMin, options.TrapMax + 1);
            if (finalTraps > 0) {
                for (i = 0; i < finalTraps; i++) { // add traps
                    for (j = 0; j < 200; j++) {
                        a = random.Next(0, Rooms.GetUpperBound(0) + 1);
                        b = random.Next(0, Rooms.GetUpperBound(1) + 1);
                        x = random.Next(Rooms[a, b].StartX, Rooms[a, b].EndX + 1);
                        y = random.Next(Rooms[a, b].StartY, Rooms[a, b].EndY + 1);
                        if (Rooms[a, b].Opened == OPEN && MapArray[x, y] == GROUND) {
                            MapArray[x, y] = TRAPTILE;
                            break;
                        }
                    }
                }
            }
            //if (options.TrapFrequency > 0 && options.TrapMax > 0) {
            //    for (i = options.TrapMin; i < options.TrapMax; i++) {
            //        if (random.Next(0, 100) + 1 >= options.TrapFrequency) { // then generate a trap
            //            a = random.Next(0, 16);
            //            convertRoomToXY(a, ref x, ref  y);
            //            x = x + 1 + (random.Next(0, 6));
            //            y = y + 1 + (random.Next(0, 5));
            //            if (mapArray[y, x] == '.') {
            //                mapArray[y, x] = 'Q';
            //            }
            //        }
            //    }
            //}
            //Console.WriteLine("ITEMS:");
            // generate items (up to 16) ~moved to its own method
            //for (i = 0; i < 16; i++) {
            //bool success = false;
            //    for (int k = 0; k < 200; k++) { // do until you succeed
            //        a = random.Next(0, 16);
            //convertRoomToXY(a, ref x, ref y);
            //        x = random.Next(room[a,1], room[a,3]+1);
            //        y = random.Next(room[a,2], room[a,4]+1);
            //        if (mapArray[y, x] == '.' /* mapArray[y, x] != 'S' && mapArray[y, x] != 'E' &&
            //                                     mapArray[y, x] != ' '*/) {
            //            mapArray[y, x] = (char)(96 + i); //ascii for lowercase letters
            //            break;
            //        }
            //    }
            //}

            //return MapArray;
        }

        public static void TextureDungeon(int[,] mapArray) {
            TextureDungeon(mapArray, 0, 0, mapArray.GetUpperBound(0), mapArray.GetUpperBound(1));
        }

        public static void TextureDungeon(int[,] mapArray, int startX, int startY, int endX, int endY) {
            //Console.WriteLine("TEXTURE:");
            //texture water

            textureMainBlocks(mapArray, startX, startY, endX, endY);
            textureLooseBlocks(mapArray, startX, startY, endX, endY);
        }

        static void textureMainBlocks(int[,] mapArray, int startX, int startY, int endX, int endY) {

            bool[] checkCross = new bool[4];

            for (int y = startY; y <= endY; y++) {

                for (int x = startX; x <= endX; x++) {
                    bool chamber = false;
                    if (mapArray[x, y] >= 1024) {
                        chamber = true;
                        mapArray[x, y] -= 1024;
                    }
                    if (mapArray[x, y] == UBLOCK) {

                        //} else {

                        checkCross[0] = CheckWall(mapArray, x, y, 1);
                        checkCross[1] = CheckWall(mapArray, x, y, 2);
                        checkCross[2] = CheckWall(mapArray, x, y, 3);
                        checkCross[3] = CheckWall(mapArray, x, y, 4);

                        if (checkCross[0] == false && checkCross[1] == false && checkCross[2] == false && checkCross[3] == false) {

                            mapArray[x, y] = 256;

                        } else if (checkCross[0] == true && checkCross[1] == false && checkCross[2] == false && checkCross[3] == false) {


                        } else if (checkCross[0] == false && checkCross[1] == true && checkCross[2] == false && checkCross[3] == false) {

                        } else if (checkCross[0] == false && checkCross[1] == false && checkCross[2] == true && checkCross[3] == false) {

                        } else if (checkCross[0] == false && checkCross[1] == false && checkCross[2] == false && checkCross[3] == true) {

                        } else if (checkCross[0] == true && checkCross[1] == true && checkCross[2] == false && checkCross[3] == false) {

                            if (CheckWall(mapArray, x, y, 5) == true) {

                                mapArray[x, y] = 263;

                            }
                        } else if (checkCross[0] == true && checkCross[1] == false && checkCross[2] == true && checkCross[3] == false) {


                        } else if (checkCross[0] == true && checkCross[1] == false && checkCross[2] == false && checkCross[3] == true) {

                            if (CheckWall(mapArray, x, y, 8) == true) {

                                mapArray[x, y] = 449;

                            }

                        } else if (checkCross[0] == false && checkCross[1] == true && checkCross[2] == true && checkCross[3] == false) {

                            if (CheckWall(mapArray, x, y, 6) == true) {

                                mapArray[x, y] = 284;
                                RightWallLoop(mapArray, x, y);
                            }
                        } else if (checkCross[0] == false && checkCross[1] == true && checkCross[2] == false && checkCross[3] == true) {

                        } else if (checkCross[0] == false && checkCross[1] == false && checkCross[2] == true && checkCross[3] == true) {

                            if (CheckWall(mapArray, x, y, 7) == true) {

                                mapArray[x, y] = 368;

                            }

                        } else if (checkCross[0] == false && checkCross[1] == true && checkCross[2] == true && checkCross[3] == true) {

                            if (CheckWall(mapArray, x, y, 6) == true && CheckWall(mapArray, x, y, 7) == true) {

                                mapArray[x, y] = 380;

                            } else if (CheckWall(mapArray, x, y, 6) == false && CheckWall(mapArray, x, y, 7) == true) {

                                mapArray[x, y] = 368;

                            } else if (CheckWall(mapArray, x, y, 6) == true && CheckWall(mapArray, x, y, 7) == false) {

                                mapArray[x, y] = 284;
                            }

                        } else if (checkCross[0] == true && checkCross[1] == false && checkCross[2] == true && checkCross[3] == true) {

                            if (CheckWall(mapArray, x, y, 7) == true && CheckWall(mapArray, x, y, 8) == true) {

                                mapArray[x, y] = 497;

                            } else if (CheckWall(mapArray, x, y, 7) == false && CheckWall(mapArray, x, y, 8) == true) {

                                mapArray[x, y] = 449;

                            } else if (CheckWall(mapArray, x, y, 7) == true && CheckWall(mapArray, x, y, 8) == false) {

                                mapArray[x, y] = 368;

                            }

                        } else if (checkCross[0] == true && checkCross[1] == true && checkCross[2] == false && checkCross[3] == true) {

                            if (CheckWall(mapArray, x, y, 5) == true && CheckWall(mapArray, x, y, 8) == true) {

                                mapArray[x, y] = 455;

                            } else if (CheckWall(mapArray, x, y, 5) == false && CheckWall(mapArray, x, y, 8) == true) {

                                mapArray[x, y] = 449;

                            } else if (CheckWall(mapArray, x, y, 5) == true && CheckWall(mapArray, x, y, 8) == false) {

                                mapArray[x, y] = 263;

                            }

                        } else if (checkCross[0] == true && checkCross[1] == true && checkCross[2] == true && checkCross[3] == false) {

                            if (CheckWall(mapArray, x, y, 5) == true && CheckWall(mapArray, x, y, 6) == true) {

                                mapArray[x, y] = 287;
                                RightWallLoop(mapArray, x, y);
                            } else if (CheckWall(mapArray, x, y, 5) == false && CheckWall(mapArray, x, y, 6) == true) {

                                mapArray[x, y] = 284;
                                RightWallLoop(mapArray, x, y);
                            } else if (CheckWall(mapArray, x, y, 5) == true && CheckWall(mapArray, x, y, 6) == false) {

                                mapArray[x, y] = 263;

                            }

                        } else if (checkCross[0] == true && checkCross[1] == true && checkCross[2] == true && checkCross[3] == true) {

                            checkCross[0] = CheckWall(mapArray, x, y, 5);
                            checkCross[1] = CheckWall(mapArray, x, y, 6);
                            checkCross[2] = CheckWall(mapArray, x, y, 7);
                            checkCross[3] = CheckWall(mapArray, x, y, 8);

                            if (checkCross[0] == false && checkCross[1] == false && checkCross[2] == false && checkCross[3] == false) {

                            } else if (checkCross[0] == true && checkCross[1] == false && checkCross[2] == false && checkCross[3] == false) {

                                mapArray[x, y] = 263;

                            } else if (checkCross[0] == false && checkCross[1] == true && checkCross[2] == false && checkCross[3] == false) {

                                mapArray[x, y] = 284;
                                RightWallLoop(mapArray, x, y);
                            } else if (checkCross[0] == false && checkCross[1] == false && checkCross[2] == true && checkCross[3] == false) {

                                mapArray[x, y] = 368;

                            } else if (checkCross[0] == false && checkCross[1] == false && checkCross[2] == false && checkCross[3] == true) {

                                mapArray[x, y] = 449;

                            } else if (checkCross[0] == true && checkCross[1] == true && checkCross[2] == false && checkCross[3] == false) {

                                mapArray[x, y] = 287;
                                RightWallLoop(mapArray, x, y);

                            } else if (checkCross[0] == true && checkCross[1] == false && checkCross[2] == true && checkCross[3] == false) {

                            } else if (checkCross[0] == true && checkCross[1] == false && checkCross[2] == false && checkCross[3] == true) {

                                mapArray[x, y] = 455;

                            } else if (checkCross[0] == false && checkCross[1] == true && checkCross[2] == true && checkCross[3] == false) {

                                mapArray[x, y] = 380;

                            } else if (checkCross[0] == false && checkCross[1] == true && checkCross[2] == false && checkCross[3] == true) {

                                mapArray[x, y] = 449;

                            } else if (checkCross[0] == false && checkCross[1] == false && checkCross[2] == true && checkCross[3] == true) {

                                mapArray[x, y] = 497;


                            } else if (checkCross[0] == false && checkCross[1] == true && checkCross[2] == true && checkCross[3] == true) {

                                mapArray[x, y] = 509;

                            } else if (checkCross[0] == true && checkCross[1] == false && checkCross[2] == true && checkCross[3] == true) {

                                mapArray[x, y] = 503;

                            } else if (checkCross[0] == true && checkCross[1] == true && checkCross[2] == false && checkCross[3] == true) {

                                mapArray[x, y] = 479;
                                RightWallLoop(mapArray, x, y);
                            } else if (checkCross[0] == true && checkCross[1] == true && checkCross[2] == true && checkCross[3] == false) {

                                mapArray[x, y] = 383;

                            } else if (checkCross[0] == true && checkCross[1] == true && checkCross[2] == true && checkCross[3] == true) {

                                mapArray[x, y] = 511;

                            }
                        }
                    } else if (mapArray[x, y] == UWATER) {
                        checkCross[0] = CheckWater(mapArray, x, y - 1);
                        checkCross[1] = CheckWater(mapArray, x + 1, y);
                        checkCross[2] = CheckWater(mapArray, x, y + 1);
                        checkCross[3] = CheckWater(mapArray, x - 1, y);

                        mapArray[x, y] = 512;

                        if (checkCross[0] == false && checkCross[1] == false && checkCross[2] == false && checkCross[3] == false) {

                        } else if (checkCross[0] == true && checkCross[1] == false && checkCross[2] == false && checkCross[3] == false) {

                            mapArray[x, y] = mapArray[x, y] + 1;

                        } else if (checkCross[0] == false && checkCross[1] == true && checkCross[2] == false && checkCross[3] == false) {

                            mapArray[x, y] = mapArray[x, y] + 2;

                        } else if (checkCross[0] == false && checkCross[1] == false && checkCross[2] == true && checkCross[3] == false) {

                            mapArray[x, y] = mapArray[x, y] + 3;

                        } else if (checkCross[0] == false && checkCross[1] == false && checkCross[2] == false && checkCross[3] == true) {

                            mapArray[x, y] = mapArray[x, y] + 4;

                        } else if (checkCross[0] == true && checkCross[1] == true && checkCross[2] == false && checkCross[3] == false) {

                            mapArray[x, y] = mapArray[x, y] + 5;

                        } else if (checkCross[0] == true && checkCross[1] == false && checkCross[2] == true && checkCross[3] == false) {

                            mapArray[x, y] = mapArray[x, y] + 6;

                        } else if (checkCross[0] == true && checkCross[1] == false && checkCross[2] == false && checkCross[3] == true) {

                            mapArray[x, y] = mapArray[x, y] + 7;

                        } else if (checkCross[0] == false && checkCross[1] == true && checkCross[2] == true && checkCross[3] == false) {

                            mapArray[x, y] = mapArray[x, y] + 8;

                        } else if (checkCross[0] == false && checkCross[1] == true && checkCross[2] == false && checkCross[3] == true) {

                            mapArray[x, y] = mapArray[x, y] + 9;

                        } else if (checkCross[0] == false && checkCross[1] == false && checkCross[2] == true && checkCross[3] == true) {

                            mapArray[x, y] = mapArray[x, y] + 10;

                        } else if (checkCross[0] == false && checkCross[1] == true && checkCross[2] == true && checkCross[3] == true) {

                            mapArray[x, y] = mapArray[x, y] + 11;

                        } else if (checkCross[0] == true && checkCross[1] == false && checkCross[2] == true && checkCross[3] == true) {

                            mapArray[x, y] = mapArray[x, y] + 12;

                        } else if (checkCross[0] == true && checkCross[1] == true && checkCross[2] == false && checkCross[3] == true) {

                            mapArray[x, y] = mapArray[x, y] + 13;

                        } else if (checkCross[0] == true && checkCross[1] == true && checkCross[2] == true && checkCross[3] == false) {

                            mapArray[x, y] = mapArray[x, y] + 14;

                        } else if (checkCross[0] == true && checkCross[1] == true && checkCross[2] == true && checkCross[3] == true) {

                            mapArray[x, y] = mapArray[x, y] + 15;

                        }

                        if (mapArray[x, y] == 517 || mapArray[x, y] == 519 || mapArray[x, y] == 520 || mapArray[x, y] > 521) {

                            checkCross[0] = CheckWater(mapArray, x + 1, y - 1);
                            checkCross[1] = CheckWater(mapArray, x + 1, y + 1);
                            checkCross[2] = CheckWater(mapArray, x - 1, y + 1);
                            checkCross[3] = CheckWater(mapArray, x - 1, y - 1);


                            //checks diagonal /

                            if (checkCross[0] == true && checkCross[2] == true) {


                            } else if (checkCross[0] == false && checkCross[2] == true && (mapArray[x, y] == 517 || mapArray[x, y] == 525 || mapArray[x, y] == 526 || mapArray[x, y] == 527)) {

                                mapArray[x, y] = mapArray[x, y] + 16;

                            } else if (checkCross[0] == true && checkCross[2] == false && (mapArray[x, y] == 522 || mapArray[x, y] == 523 || mapArray[x, y] == 524 || mapArray[x, y] == 527)) {

                                mapArray[x, y] = mapArray[x, y] + 32;

                            } else if (checkCross[0] == false && checkCross[2] == false) {

                                if (mapArray[x, y] == 527) {

                                    mapArray[x, y] = mapArray[x, y] + 48;

                                } else if (mapArray[x, y] == 517 || mapArray[x, y] == 525 || mapArray[x, y] == 526) {

                                    mapArray[x, y] = mapArray[x, y] + 16;

                                } else if (mapArray[x, y] == 522 || mapArray[x, y] == 523 || mapArray[x, y] == 524) {

                                    mapArray[x, y] = mapArray[x, y] + 32;

                                }
                            }

                            // checks diagonal \

                            if (checkCross[1] == true && checkCross[3] == true) {


                            } else if (checkCross[1] == false && checkCross[3] == true && ((int)mapArray[x, y] % 16 == 8 || (int)mapArray[x, y] % 16 == 11 || (int)mapArray[x, y] % 16 == 14 || (int)mapArray[x, y] % 16 == 15)) {

                                mapArray[x, y] = mapArray[x, y] + 64;

                            } else if (checkCross[1] == true && checkCross[3] == false && ((int)mapArray[x, y] % 16 == 7 || (int)mapArray[x, y] % 16 == 12 || (int)mapArray[x, y] % 16 == 13 || (int)mapArray[x, y] % 16 == 15)) {

                                mapArray[x, y] = mapArray[x, y] + 128;

                            } else if (checkCross[1] == false && checkCross[3] == false) {

                                if ((int)mapArray[x, y] % 16 == 15) {

                                    mapArray[x, y] = mapArray[x, y] + 192;

                                } else if ((int)mapArray[x, y] % 16 == 8 || (int)mapArray[x, y] % 16 == 11 || (int)mapArray[x, y] % 16 == 14) {

                                    mapArray[x, y] = mapArray[x, y] + 64;

                                } else if ((int)mapArray[x, y] % 16 == 7 || (int)mapArray[x, y] % 16 == 12 || (int)mapArray[x, y] % 16 == 13) {

                                    mapArray[x, y] = mapArray[x, y] + 128;

                                }
                            }

                        }


                    }

                    if (chamber) {
                        mapArray[x, y] += 1024;
                    }
                }
            }

        }

        static void textureLooseBlocks(int[,] mapArray, int startX, int startY, int endX, int endY) {

            bool[] checkCross = new bool[4];

            for (int y = startY; y <= endY; y++) {

                for (int x = startX; x <= endX; x++) {

                    bool chamber = false;
                    if (mapArray[x, y] >= 1024) {
                        chamber = true;
                        mapArray[x, y] -= 1024;
                    }

                    if (mapArray[x, y] == UBLOCK) {


                        checkCross[0] = CheckWall(mapArray, x, y, 1);
                        checkCross[1] = CheckWall(mapArray, x, y, 2);
                        checkCross[2] = CheckWall(mapArray, x, y, 3);
                        checkCross[3] = CheckWall(mapArray, x, y, 4);

                        if (checkCross[0] == false && checkCross[1] == false && checkCross[2] == false && checkCross[3] == false) {

                            mapArray[x, y] = 256;

                        } else if (checkCross[0] == true && checkCross[1] == false && checkCross[2] == false && checkCross[3] == false) {
                            mapArray[x, y] = 257;

                        } else if (checkCross[0] == false && checkCross[1] == true && checkCross[2] == false && checkCross[3] == false) {
                            mapArray[x, y] = 260;
                            RowLoop(mapArray, x, y);
                        } else if (checkCross[0] == false && checkCross[1] == false && checkCross[2] == true && checkCross[3] == false) {
                            mapArray[x, y] = 272;
                            ColumnLoop(mapArray, x, y);
                        } else if (checkCross[0] == false && checkCross[1] == false && checkCross[2] == false && checkCross[3] == true) {
                            mapArray[x, y] = 320;
                        } else if (checkCross[0] == true && checkCross[1] == true && checkCross[2] == false && checkCross[3] == false) {

                            mapArray[x, y] = 257;
                        } else if (checkCross[0] == true && checkCross[1] == false && checkCross[2] == true && checkCross[3] == false) {

                            mapArray[x, y] = 273;
                        } else if (checkCross[0] == true && checkCross[1] == false && checkCross[2] == false && checkCross[3] == true) {

                            mapArray[x, y] = 257;

                        } else if (checkCross[0] == false && checkCross[1] == true && checkCross[2] == true && checkCross[3] == false) {

                            mapArray[x, y] = 272;
                            ColumnLoop(mapArray, x, y);
                        } else if (checkCross[0] == false && checkCross[1] == true && checkCross[2] == false && checkCross[3] == true) {

                            mapArray[x, y] = 324;

                        } else if (checkCross[0] == false && checkCross[1] == false && checkCross[2] == true && checkCross[3] == true) {

                            mapArray[x, y] = 320;

                        } else if (checkCross[0] == false && checkCross[1] == true && checkCross[2] == true && checkCross[3] == true) {

                            mapArray[x, y] = 324;

                        } else if (checkCross[0] == true && checkCross[1] == false && checkCross[2] == true && checkCross[3] == true) {

                            mapArray[x, y] = 273;

                        } else if (checkCross[0] == true && checkCross[1] == true && checkCross[2] == false && checkCross[3] == true) {

                            mapArray[x, y] = 324;

                        } else if (checkCross[0] == true && checkCross[1] == true && checkCross[2] == true && checkCross[3] == false) {

                            mapArray[x, y] = 273;

                        } else if (checkCross[0] == true && checkCross[1] == true && checkCross[2] == true && checkCross[3] == true) {

                            checkCross[0] = CheckWall(mapArray, x, y, 5);
                            checkCross[1] = CheckWall(mapArray, x, y, 6);
                            checkCross[2] = CheckWall(mapArray, x, y, 7);
                            checkCross[3] = CheckWall(mapArray, x, y, 8);

                            if (checkCross[0] == false && checkCross[1] == false && checkCross[2] == false && checkCross[3] == false) {
                                mapArray[x, y] = 273;
                            }
                        }
                    }

                    if (chamber) {
                        mapArray[x, y] += 1024;
                    }

                }
            }

        }

        static void RightWallLoop(int[,] mapArray, int x, int y) {
            //Console.Write("/RWL:"+x+","+y);
            bool[] checkDown = new bool[4];

            while (true) {
                y++;

                bool chamber = false;
                if (mapArray[x, y] >= 1024) {
                    chamber = true;
                    mapArray[x, y] -= 1024;
                }

                if (mapArray[x, y] != UBLOCK) {
                    
                    if (chamber) {
                        mapArray[x, y] += 1024;
                    }
                    break;
                }


                checkDown[0] = CheckWall(mapArray, x, y, 4);
                checkDown[1] = CheckWall(mapArray, x, y, 7);
                checkDown[2] = CheckWall(mapArray, x, y, 3);
                checkDown[3] = CheckWall(mapArray, x, y, 6);

                if (checkDown[2] == false || checkDown[3] == false) {
                    mapArray[x, y] = 263;
                    
                    if (chamber) {
                        mapArray[x, y] += 1024;
                    }
                    break;
                } else if (checkDown[0] == true && checkDown[1] == true) {
                    mapArray[x, y] = 383;
                    
                    if (chamber) {
                        mapArray[x, y] += 1024;
                    }
                    break;

                } else {

                    mapArray[x, y] = 287;
                    
                    if (chamber) {
                        mapArray[x, y] += 1024;
                    }
                }
            }
        }


        static void ColumnLoop(int[,] mapArray, int x, int y) {



            while (true) {
                y++;

                bool chamber = false;
                if (mapArray[x, y] >= 1024) {
                    chamber = true;
                    mapArray[x, y] -= 1024;
                }

                if (mapArray[x, y] != UBLOCK) {
                    
                    if (chamber) {
                        mapArray[x, y] += 1024;
                    }
                    break;
                }

                bool checkColumn = CheckWall(mapArray, x, y, 3);

                if (checkColumn) {

                    mapArray[x, y] = 273;
                    
                    if (chamber) {
                        mapArray[x, y] += 1024;
                    }
                } else {

                    mapArray[x, y] = 257;
                    
                    if (chamber) {
                        mapArray[x, y] += 1024;
                    }
                    break;

                }
            }
        }

        static void RowLoop(int[,] mapArray, int x, int y) {



            while (true) {
                x++;
                bool chamber = false;
                if (mapArray[x, y] >= 1024) {
                    chamber = true;
                    mapArray[x, y] -= 1024;
                }
                if (mapArray[x, y] != UBLOCK) {
                    
                    if (chamber) {
                        mapArray[x, y] += 1024;
                    }
                    break;
                }

                bool checkColumn = CheckWall(mapArray, x, y, 2);

                if (checkColumn) {

                    mapArray[x, y] = 324;
                    
                    if (chamber) {
                        mapArray[x, y] += 1024;
                    }
                } else {

                    mapArray[x, y] = 320;
                    
                    if (chamber) {
                        mapArray[x, y] += 1024;
                    }
                    break;

                }
            }
        }

        public void GenItem(bool ground, bool water, bool wall, ref int x, ref int y) {

            for (int i = 0; i <= MapArray.GetUpperBound(0); i++) {
                for (int j = 0; j <= MapArray.GetUpperBound(1); j++) {
                    if (MapArray[i, j] == ITEMTILE) {
                        x = i;
                        y = j;
                        MapArray[i, j] = GROUND;
                        return;
                    }
                }
            }

            for (int k = 0; k < 300; k++) { // do until you succeed
                x = random.Next(1, 49);
                y = random.Next(1, 49);
                if (MapArray[x, y] == GROUND) {
                    //mapArray[y, x] = (char)(96 + i); //ascii for lowercase letters
                    if (ground) return;
                } else if (MapArray[x, y] >= 512 && MapArray[x, y] < 768) {
                    //mapArray[y, x] = (char)(96 + i); //ascii for lowercase letters
                    if (water) return;
                } else if (MapArray[x, y] >= 256 && MapArray[x, y] < 512) {
                    //mapArray[y, x] = (char)(96 + i); //ascii for lowercase letters
                    if (wall) return;
                }
            }
            x = -1;
            y = -1;
            //}


        }

        void createRoom(int roomX, int roomY, GeneratorOptions options, RDungeonChamberReq req) {
            if (Rooms[roomX, roomY].StartX > -1) {
                return;
            }

            int x = 0, y = 0, u, v, w, l; // variables used for position
            convertRoomToXY(roomX, roomY, ref x, ref y);

            //Determine room length/width
            if (Rooms[roomX, roomY].Opened == HALL) {
                w = 0;
                l = 0;
            } else {

                w = random.Next(options.RoomWidthMin, options.RoomWidthMax + 1) - 1;
                l = random.Next(options.RoomLengthMin, options.RoomLengthMax + 1) - 1;
                if (w < 1) w = 1;
                if (l < 1) l = 1;
                if (w > 47) w = 47;
                if (l > 47) l = 47;
            }


            //move X and Y to a random starting point that still would include the original x/y; exceptional case for l/w under or equal to 6
            if (w <= 6) {
                x -= (random.Next(0, (13 - w)) + w - 7);
            } else {
                x -= random.Next(0, w + 1);
            }
            if (l <= 6) {
                y -= (random.Next(0, (13 - l)) + l - 7);
            } else {
                y -= random.Next(0, l + 1);
            }

            if (x < 1) x = 1;
            if ((x + w) > 48) x = (48 - w);

            if (y < 1) y = 1;
            if ((y + l) > 48) y = (48 - l);

            // once we have our room coords, render it on the map
            u = x + w;
            v = y + l;


            for (int i = 0; i <= Rooms.GetUpperBound(0); i++) {
                for (int j = 0; j <= Rooms.GetUpperBound(1); j++) {
                    if ((roomX == i && roomY == j) ||//if the room is this room
                        ((6 + i * 12) >= x && (6 + i * 12) <= u && (6 + j * 12) >= y && (6 + j * 12) <= v)) {//if the room's anchor point was eclipsed in this room
                        Rooms[i, j].StartX = x;
                        Rooms[i, j].StartY = y;
                        Rooms[i, j].EndX = u;
                        Rooms[i, j].EndY = v;
                    }
                }
            }

            // done 
        }

        void createChamber(GeneratorOptions options, RDungeonChamberReq req) {

            int x = 0, y = 0, u = 0, v = 0, w, l; // variables used for position
            convertRoomToXY(Chamber.X, Chamber.Y, ref x, ref y);
            int roomX = Chamber.X;
            int roomY = Chamber.Y;
            bool eclipse = true;

            int iterations = 100;
            if (req.MinX == req.MaxX && req.MinY == req.MaxY) {
                iterations = 1;
            }

            for (int k = 0; k < iterations && eclipse == true; k++) {
                //Determine room length/width

                //chamber cannot be a hall
                w = random.Next(req.MinX, req.MaxX + 1) - 1;
                l = random.Next(req.MinY, req.MaxY + 1) - 1;
                if (w < 1) w = 1;
                if (l < 1) l = 1;
                if (w > 47) w = 47;
                if (l > 47) l = 47;


                //move X and Y to a random starting point that still would include the original x/y; exceptional case for l/w under or equal to 6
                if (w <= 6) {
                    x -= (random.Next(0, (13 - w)) + w - 7);
                } else {
                    x -= random.Next(0, w + 1);
                }
                if (l <= 6) {
                    y -= (random.Next(0, (13 - l)) + l - 7);
                } else {
                    y -= random.Next(0, l + 1);
                }

                if (x < 1) x = 1;
                if ((x + w) > 48) x = (48 - w);

                if (y < 1) y = 1;
                if ((y + l) > 48) y = (48 - l);

                // once we have our room coords, render it on the map
                u = x + w;
                v = y + l;

                eclipse = false;
                if (req.Start == Enums.Acceptance.Never || req.End == Enums.Acceptance.Never) {//if we're dealing with a chamber with no tolerance to entrances/exits

                    for (int i = 0; i <= Rooms.GetUpperBound(0); i++) {
                        for (int j = 0; j <= Rooms.GetUpperBound(1); j++) {
                            if (Chamber.X != i || Chamber.Y != j) {//if the room not is this room
                                if (doesBigRoomEclipseSmallRoom(x, y, u, v, Rooms[i, j].StartX, Rooms[i, j].StartY, Rooms[i, j].EndX, Rooms[i, j].EndY)) {
                                    if (req.Start == Enums.Acceptance.Never && Start.X == i && Start.Y == j) {
                                        eclipse = true;
                                    }
                                    if (req.End == Enums.Acceptance.Never && End.X == i && End.Y == j) {
                                        eclipse = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (eclipse) {
                //the chamber has eclipsed a start/end room; it can't stay
                Chamber = new DungeonPoint(-1, -1);

                //reroll everything

                w = random.Next(options.RoomWidthMin, options.RoomWidthMax + 1) - 1;
                l = random.Next(options.RoomLengthMin, options.RoomLengthMax + 1) - 1;

                if (w < 1) w = 1;
                if (l < 1) l = 1;
                if (w > 47) w = 47;
                if (l > 47) l = 47;

                //move X and Y to a random starting point that still would include the original x/y; exceptional case for l/w under or equal to 6
                if (w <= 6) {
                    x -= (random.Next(0, (13 - w)) + w - 7);
                } else {
                    x -= random.Next(0, w + 1);
                }
                if (l <= 6) {
                    y -= (random.Next(0, (13 - l)) + l - 7);
                } else {
                    y -= random.Next(0, l + 1);
                }

                if (x < 1) x = 1;
                if ((x + w) > 48) x = (48 - w);

                if (y < 1) y = 1;
                if ((y + l) > 48) y = (48 - l);

                // once we have our room coords, render it on the map
                u = x + w;
                v = y + l;
            }


            for (int i = 0; i <= Rooms.GetUpperBound(0); i++) {
                for (int j = 0; j <= Rooms.GetUpperBound(1); j++) {
                    if ((roomX == i && roomY == j) ||//if the room is this room
                        doesBigRoomEclipseSmallRoom(x, y, u, v, Rooms[i, j].StartX, Rooms[i, j].StartY, Rooms[i, j].EndX, Rooms[i, j].EndY)) {
                        Rooms[i, j].StartX = x;
                        Rooms[i, j].StartY = y;
                        Rooms[i, j].EndX = u;
                        Rooms[i, j].EndY = v;
                    }
                }
            }

            // done 
        }

        void drawRoom(int roomX, int roomY) {
            int x, y, u, v;

            x = Rooms[roomX, roomY].StartX;
            y = Rooms[roomX, roomY].StartY;

            u = Rooms[roomX, roomY].EndX;
            v = Rooms[roomX, roomY].EndY;

            int drawnTile = GROUND;

            //Console.WriteLine("#" + roomNum + "," + x + "," + y + "," + u + "," + v);
            if (x == u || y == v) drawnTile = HALLTILE;

            for (int i = x; i <= u; i++) {
                for (int j = y; j <= v; j++) {
                    MapArray[i, j] = drawnTile;
                }
            }

        }

        void padSingleRoom(int roomX, int roomY) {
            int x, y, u, v;

            x = Rooms[roomX, roomY].StartX;
            y = Rooms[roomX, roomY].StartY;

            u = Rooms[roomX, roomY].EndX;
            v = Rooms[roomX, roomY].EndY;


            if (x == u) {
                if (y - 1 >= 0 && MapArray[x, y - 1] == GROUND) {
                    MapArray[x, y - 1] = DOORTILE;
                }
                if (v + 1 <= MapArray.GetUpperBound(1) && MapArray[x, v + 1] == GROUND) {
                    MapArray[x, v + 1] = DOORTILE;
                }
            }

            if (y == v) {
                if (x - 1 >= 0 && MapArray[x - 1, y] == GROUND) {
                    MapArray[x - 1, y] = DOORTILE;
                }
                if (u + 1 <= MapArray.GetUpperBound(0) && MapArray[u + 1, y] == GROUND) {
                    MapArray[u + 1, y] = DOORTILE;
                }
            }

        }

        void createHHall(int hallX, int hallY, GeneratorOptions options, RDungeonChamberReq req) {
            //Console.Write("H"+hall+":");
            DungeonArrayRoom startRoom = Rooms[hallX, hallY];
            DungeonArrayRoom endRoom = Rooms[hallX + 1, hallY];

            int n = endRoom.StartX - startRoom.EndX; //distance between rooms

            if (n < 1) {
                if (startRoom.StartY > endRoom.EndY || startRoom.EndY < endRoom.StartY) {
                    n++;
                } else {
                    //Console.WriteLine(";");
                    return;
                }
            }
            n--;

            int x, y, m,/* n,*/ h, r = 0, var;

            x = startRoom.EndX;

            m = random.Next(options.HallTurnMin, options.HallTurnMax + 1); // the number of vertical pieces in the hall


            if (m > ((n - 1) / 2)) m = (n - 1) / 2; //reduces the number of hall turns to something the length of the hall can accept


            y = random.Next(startRoom.StartY, startRoom.EndY + 1); //picks a Y coordinate to start at

            if (Chamber.X == hallX && Chamber.Y == hallY && req != null && req.RightPassage > -1) {
                y = startRoom.StartY + req.RightPassage;
            }

            HHalls[hallX, hallY].TurnPoints.Add(new DungeonPoint(x, y));

            if (m <= 0 && (y > endRoom.EndY || y < endRoom.StartY ||
                (Chamber.X == hallX + 1 && Chamber.Y == hallY && req != null && req.LeftPassage > -1 && y != endRoom.StartY + req.LeftPassage))) {//checks if at least one turn is needed to make rooms meet
                m = 1;
            }


            //Console.Write("("+room[startRoom,2]+"."+room[startRoom,4]+")");
            for (int i = 0; i <= m; i++) {
                if (i == m) {
                    var = random.Next(endRoom.StartY, endRoom.EndY + 1) - y;
                    if (Chamber.X == hallX + 1 && Chamber.Y == hallY && req != null && req.LeftPassage > -1) {
                        var = endRoom.StartY + req.LeftPassage - y;
                    }
                } else {
                    var = random.Next(options.HallVarMin, options.HallVarMax + 1);
                    if (random.Next(0, 2) == 0) {
                        var = -var;
                    }
                }
                if (i != 0) {
                    if ((y + var) < 1) var = 1 - y;
                    if ((y + var) > 48) var = 48 - y;
                    //addVertHall(x, y, var, mapArray);

                    y += var;
                    HHalls[hallX, hallY].TurnPoints.Add(new DungeonPoint(x, y));
                }// else {
                //mapArray[y,x] = ',';
                //}

                if (n >= 0) {
                    h = (r + n) / (m + 1);
                    r = (r + n) % (m + 1);
                } else {
                    h = -(r - n) / (m + 1);
                    r = (r - n) % (m + 1);
                }
                //Console.Write("("+n+","+r+")");
                //addHorizHall(x, y, h, mapArray);

                x += h;
                HHalls[hallX, hallY].TurnPoints.Add(new DungeonPoint(x, y));
            }

            HHalls[hallX, hallY].TurnPoints[HHalls[hallX, hallY].TurnPoints.Count - 1] = new DungeonPoint(x + 1, y);
            //mapArray[y, x + 1] = ',';


            //int x, y, h, j;
            //h = hall % 3;
            //j = hall / 3;

            //x = 8 + 13 * h;
            //y = 3 + 12 * j;

            //addHorizHall(x, y, 6, mapArray);
            //Console.WriteLine(";");
        }

        void createVHall(int hallX, int hallY, GeneratorOptions options, RDungeonChamberReq req) {
            //Console.Write("V"+hall+":");            
            DungeonArrayRoom startRoom = Rooms[hallX, hallY];
            DungeonArrayRoom endRoom = Rooms[hallX, hallY + 1];

            int n = endRoom.StartY - startRoom.EndY; //distance between rooms

            if (n < 1) {
                if (startRoom.StartX > endRoom.EndX || startRoom.EndX < endRoom.StartX) {
                    n++;
                } else {
                    //Console.WriteLine(";");
                    return;
                }
            }
            n--;

            int x, y, m,/* n,*/ h, r = 0, var;

            y = startRoom.EndY;

            m = random.Next(options.HallTurnMin, options.HallTurnMax + 1); // the number of horizontal pieces in the hall


            if (m > ((n - 1) / 2)) m = (n - 1) / 2; //reduces the number of hall turns to something the length of the hall can accept


            x = random.Next(startRoom.StartX, startRoom.EndX + 1); //picks a X coordinate to start at

            if (Chamber.X == hallX && Chamber.Y == hallY && req != null && req.BottomPassage > -1) {
                x = startRoom.StartX + req.BottomPassage;
            }

            VHalls[hallX, hallY].TurnPoints.Add(new DungeonPoint(x, y));


            if (m <= 0 && (x > endRoom.EndX || x < endRoom.StartX ||
                (Chamber.X == hallX && Chamber.Y == hallY + 1 && req != null && req.TopPassage > -1 && x != endRoom.StartX + req.TopPassage))) {//checks if at least one turn is needed to make rooms meet
                m = 1;
            }


            //Console.Write("("+room[startRoom,1]+"."+room[startRoom,3]+")");
            for (int i = 0; i <= m; i++) {
                if (i == m) {
                    var = random.Next(endRoom.StartX, endRoom.EndX + 1) - x;
                    if (Chamber.X == hallX && Chamber.Y == hallY + 1 && req != null && req.TopPassage > -1) {
                        var = endRoom.StartX + req.TopPassage - x;
                    }
                } else {
                    var = random.Next(options.HallVarMin, options.HallVarMax + 1);
                    if (random.Next(0, 2) == 0) {
                        var = -var;
                    }
                }

                if (i != 0) {
                    if ((x + var) < 1) var = 1 - x;
                    if ((x + var) > 48) var = 48 - x;
                    //addHorizHall(x, y, var, mapArray);

                    x += var;
                    VHalls[hallX, hallY].TurnPoints.Add(new DungeonPoint(x, y));
                }// else {
                //mapArray[y,x] = ',';
                //}



                if (n >= 0) {
                    h = (r + n) / (m + 1);
                    r = (r + n) % (m + 1);
                } else {
                    h = -(r - n) / (m + 1);
                    r = (r - n) % (m + 1);
                }
                //Console.Write("("+n+","+r+")");
                //addVertHall(x, y, h, mapArray);

                y += h;
                VHalls[hallX, hallY].TurnPoints.Add(new DungeonPoint(x, y));
            }

            VHalls[hallX, hallY].TurnPoints[VHalls[hallX, hallY].TurnPoints.Count - 1] = new DungeonPoint(x, y + 1);
            //mapArray[y + 1, x] = ',';


            //int x, y, h, j; // variables for position
            //h = hall % 4;
            //j = hall / 4;

            //x = 3 + Convert.ToInt32(13.5 * h);
            //y = 7 + 12 * j;

            //addVertHall(x, y, mapArray);
            //Console.WriteLine(";");
        }

        void DrawHalls(DungeonArrayHall hall) {
            if (hall.TurnPoints.Count > 0) {
                bool addedEntrance = false;
                DrawHallTile(hall.TurnPoints[0].X, hall.TurnPoints[0].Y, ref addedEntrance);
                for (int i = 0; i < hall.TurnPoints.Count - 1; i++) {
                    DrawHall(hall.TurnPoints[i], hall.TurnPoints[i + 1], ref addedEntrance);
                }
                for (int i = hall.TurnPoints.Count - 1; i > 0; i--) {
                    DrawHall(hall.TurnPoints[i], hall.TurnPoints[i - 1], ref addedEntrance);
                }
            }

        }

        void DrawHall(DungeonPoint point1, DungeonPoint point2, ref bool addedEntrance) {
            if (point1.X == point2.X) {
                if (point2.Y > point1.Y) {
                    for (int i = point1.Y; i <= point2.Y; i++) {
                        DrawHallTile(point1.X, i, ref addedEntrance);
                    }
                } else if (point2.Y < point1.Y) {
                    for (int i = point1.Y; i >= point2.Y; i--) {
                        DrawHallTile(point1.X, i, ref addedEntrance);
                    }
                }
            } else if (point1.Y == point2.Y) {
                if (point2.X > point1.X) {
                    for (int i = point1.X; i <= point2.X; i++) {
                        DrawHallTile(i, point1.Y, ref addedEntrance);
                    }
                } else if (point2.X < point1.X) {
                    for (int i = point1.X; i >= point2.X; i--) {
                        DrawHallTile(i, point1.Y, ref addedEntrance);
                    }
                }
            }
        }

        void DrawHallTile(int x, int y, ref bool addedEntrance) {
            if (MapArray[x, y] == GROUND || MapArray[x, y] == DOORTILE) {
                if (!addedEntrance) {
                    MapArray[x, y] = DOORTILE;
                    addedEntrance = true;
                }

            } else {
                if (MapArray[x, y] != HALLTILE && MapArray[x, y] != DOORTILE) {
                    MapArray[x, y] = HALLTILE;
                }
                addedEntrance = false;
            }
        }

        void DrawWater(GeneratorOptions options) {
            for (int y = 1; y < 49; y++) {

                for (int x = 1; x < 49; x++) {

                    if (MapArray[x, y] != UBLOCK) {

                    } else if (options.WaterFrequency < 1) {

                    } else if (options.WaterFrequency > 99) {

                        MapArray[x, y] = UWATER;

                    } else if (random.Next(0, 100) + 1 <= options.WaterFrequency) { // dotchi ~nyo? Water or Wall?

                        MapArray[x, y] = UWATER;

                    } else {

                    }

                }
            }
        }

        void DrawCraters(GeneratorOptions options) {
            if (options.Craters > 0) {

                for (int i = 0; i < options.Craters; i++) {

                    int centerX = random.Next(1, 50);
                    int centerY = random.Next(1, 50);

                    int craterLength = random.Next(options.CraterMinLength, options.CraterMaxLength);

                    int startX;
                    int startY;

                    if (craterLength % 2 == 1) {

                        startX = centerX - craterLength / 2;
                        startY = centerY - craterLength / 2;

                    } else {

                        startX = centerX + 1 - craterLength / 2;
                        startY = centerY + 1 - craterLength / 2;

                    }



                    for (int y = startY; y < startY + craterLength; y++) {

                        for (int x = startX; x < startX + craterLength; x++) {



                            if (x < 1 || x > 47 || y < 1 || y > 47) {

                            } else if (MapArray[x, y] != UBLOCK) {

                            } else {

                                if (System.Math.Abs(x - centerX) + System.Math.Abs(y - centerY) <= craterLength / 2) {//makes a crater in a "diamond" shape

                                    MapArray[x, y] = UWATER;

                                } else if (options.CraterFuzzy == true && System.Math.Abs(x - centerX) + System.Math.Abs(y - centerY) <= craterLength * 0.75) {//makes a "fuzzy" edge to the diamond crater

                                    if (random.Next(0, 100) > 49) {
                                        MapArray[x, y] = UWATER;
                                    }
                                }
                            }
                        }
                    }


                }
            }
        }


        static bool CheckWall(int[,] mapArray, int x, int y, int dir) {

            if (dir == 2 || dir == 5 || dir == 6) {
                x++;
            } else if (dir == 4 || dir == 7 || dir == 8) {
                x--;
            }

            if (dir == 1 || dir == 5 || dir == 8) {
                y--;
            } else if (dir == 3 || dir == 6 || dir == 7) {
                y++;
            }

            if (x < 0 || x > 49 || y < 0 || y > 49) {
                return true;
            }
            bool chamber = false;
            if (mapArray[x, y] >= 1024) {
                chamber = true;
                mapArray[x, y] -= 1024;
            }

            if (mapArray[x, y] == UBLOCK) {
                
                if (chamber) {
                    mapArray[x, y] += 1024;
                }
                return true;
            }
            if (mapArray[x, y] < 256 || mapArray[x, y] >= 512) {
                
                if (chamber) {
                    mapArray[x, y] += 1024;
                }
                return false;
            }
            
            if (chamber) {
                mapArray[x, y] += 1024;
            }
            switch (dir) {
                case 1: {
                        if (mapArray[x, y] / 16 % 2 == 0) return false;
                    }
                    break;
                case 2: {
                        if (mapArray[x, y] / 64 % 2 == 0) return false;
                    }
                    break;
                case 3: {
                        if (mapArray[x, y] % 2 == 0) return false;
                    }
                    break;
                case 4: {
                        if (mapArray[x, y] / 4 % 2 == 0) return false;
                    }
                    break;
                case 5: {
                        if (mapArray[x, y] / 32 % 2 == 0) return false;
                    }
                    break;
                case 6: {
                        if (mapArray[x, y] / 128 % 2 == 0) return false;
                    }
                    break;
                case 7: {
                        if (mapArray[x, y] / 2 % 2 == 0) return false;
                    }
                    break;
                case 8: {
                        if (mapArray[x, y] / 8 % 2 == 0) return false;
                    }
                    break;
            }

            return true;
        }

        static bool CheckWater(int[,] mapArray, int x, int y) {
            bool chamber = false;
            if (mapArray[x, y] >= 1024) {
                chamber = true;
                mapArray[x, y] -= 1024;
            }
            if (mapArray[x, y] == UWATER || mapArray[x, y] >= 512) {
                
                if (chamber) {
                    mapArray[x, y] += 1024;
                }
                return true;
            } else {
                
                if (chamber) {
                    mapArray[x, y] += 1024;
                }
                return false;
            }
        }

        void addSEpos(DungeonPoint point, int type, RDungeonChamberReq req) {
            //Console.WriteLine("SE Added: " + type);
            int x = 0, y = 0, u = 0, v = 0;
            int c;
            if (type == START) c = STARTTILE;
            else c = ENDTILE;

            //convertRoomToXY(roomNum, ref x, ref y);
            x = Rooms[point.X, point.Y].StartX;
            y = Rooms[point.X, point.Y].StartY;
            u = Rooms[point.X, point.Y].EndX;
            v = Rooms[point.X, point.Y].EndY;

            //Console.WriteLine(roomNum + " " + type);
            //bool done = false;
            int randx = 0, randy = 0;
            for (int i = 0; i < 200; i++) {
                bool approved = true;
                randx = random.Next(x, u + 1);
                randy = random.Next(y, v + 1);
                //Console.WriteLine(x + "," + y + "," + u + "," + v);
                if (MapArray[randx, randy] != GROUND) {
                    approved = false;
                }
                if (Chamber.X > -1 && Chamber.Y > -1) {
                    if (randx >= Rooms[Chamber.X, Chamber.Y].StartX && randx <= Rooms[Chamber.X, Chamber.Y].EndX &&
                        randy >= Rooms[Chamber.X, Chamber.Y].StartY && randy <= Rooms[Chamber.X, Chamber.Y].EndY) {
                        //if the start or end was picked to be inside a chamber
                        if (type == START) {
                            if (req.Start == Enums.Acceptance.Never) {
                                approved = false;
                            } else {
                                //the chamber is okay with having a START inside it
                                Start = new DungeonPoint(Chamber.X, Chamber.Y);
                                return;
                            }
                        } else {
                            if (req.End == Enums.Acceptance.Never) {
                                approved = false;
                            } else {
                                //the chamber is okay with having a End inside it
                                End = new DungeonPoint(Chamber.X, Chamber.Y);
                                return;
                            }
                        }
                    }
                }

                if (approved) {
                    MapArray[randx, randy] = c;
                    return;
                }
            }
            while (true) {//backup plan in case rooms are so small that all there's left is halls and doors
                bool approved = true;
                randx = random.Next(x, u + 1);
                randy = random.Next(y, v + 1);
                //Console.WriteLine(x + "," + y + "," + u + "," + v);
                if (MapArray[randx, randy] != HALLTILE && MapArray[randx, randy] != DOORTILE && MapArray[randx, randy] != GROUND) {
                    approved = false;
                }

                if (Chamber.X > -1 && Chamber.Y > -1) {
                    if (randx >= Rooms[Chamber.X, Chamber.Y].StartX && randx <= Rooms[Chamber.X, Chamber.Y].EndX &&
                        randy >= Rooms[Chamber.X, Chamber.Y].StartY && randy <= Rooms[Chamber.X, Chamber.Y].EndY) {
                        //if the start or end was picked to be inside a chamber
                        if (type == START) {
                            if (req.Start == Enums.Acceptance.Never) {
                                approved = false;
                            } else {
                                //the chamber is okay with having a START inside it
                                Start = new DungeonPoint(Chamber.X, Chamber.Y);
                                return;
                            }
                        } else {
                            if (req.End == Enums.Acceptance.Never) {
                                approved = false;
                            } else {
                                //the chamber is okay with having a End inside it
                                End = new DungeonPoint(Chamber.X, Chamber.Y);
                                return;
                            }
                        }
                    }
                }

                if (approved) {
                    MapArray[randx, randy] = c;
                    return;
                }
            }

        }

        static void convertRoomToXY(int roomX, int roomY, ref int x, ref int y) {

            x = 6 + roomX * 12;
            y = 6 + roomY * 12;
        }

        static bool doesBigRoomEclipseSmallRoom(int bigStartX, int bigStartY, int bigEndX, int bigEndY,
            int smallStartX, int smallStartY, int smallEndX, int smallEndY) {

            if (bigStartX <= smallStartX && smallEndX <= bigEndX) {
                if (bigStartY <= smallStartY && smallEndY <= bigEndY) {
                    return true;
                }
            }

            return false;
        }

        void openHallBetween(DungeonPoint room1, DungeonPoint room2) {

            if (room1.X == room2.X) {
                int d = room2.Y - room1.Y;
                if (d == 1) {
                    VHalls[room1.X, room1.Y].Open = true;
                } else if (d == -1) {
                    VHalls[room2.X, room2.Y].Open = true;
                }
            } else if (room1.Y == room2.Y) {
                int d = room2.X - room1.X;
                if (d == 1) {
                    HHalls[room1.X, room1.Y].Open = true;
                } else if (d == -1) {
                    HHalls[room2.X, room2.Y].Open = true;
                }
            }

            //if (room2 < room1) {
            //    int temp = room1;
            //    room1 = room2;
            //    room2 = temp;
            //}
            //if (room2 - room1 == 1) { //horizontal
            //    x = room2 / 4;
            //    hhall[room1 - x] = OPEN;
            //} else { //vertical
            //    vhall[room1] = OPEN;
            //}
        }

        static int getRoomUp(int room) {
            if (room < 4)
                return -1;
            return room - 4;
        }

        static int getRoomDown(int room) {
            if (room > 11)
                return -1;
            return room + 4;
        }

        static int getRoomLeft(int room) {
            if (room % 4 == 0)
                return -1;
            return room - 1;
        }

        static int getRoomRight(int room) {
            if (room % 4 == 3)
                return -1;
            return room + 1;
        }

    }
}

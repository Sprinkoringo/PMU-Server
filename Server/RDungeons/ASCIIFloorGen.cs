/*The MIT License (MIT)

Copyright (c) 2014 PMU Staff

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/


using System;
using System.Collections.Generic;
using System.Text;

namespace Server.RDungeons
{
    public class ASCIIFloorGen {

        static readonly Random random = new Random(Environment.TickCount);

        public const int CLOSED = 0; //closed room/hall, doesn't exist
        public const int OPEN = 1; //open room/hall
        public const int HALL = 2; // room being treated as hall

        public const int START = 0; //starting position room
        public const int END = 1; //staircase room

        public static char[,] GenASCIIMap(GeneratorOptions options) {
            int[,] room = new int[16, 5]; //array of all rooms,[closed/open/hall, startx, starty, endx, endy]
            int[] vhall = new int[12]; //vertical halls
            int[] hhall = new int[12]; //horizontal halls
            int start;     //marks spawn point
            int end = 0;       //marks stairs
            int i, j, a = -1, b; //counter variables
            char[,] mapArray = new char[50, 50]; // stores map grid
            int x, y;      // used for navigating map array
            bool isDone;   // bool used for various purposes

            // Part of Options class now
            //int trapFreq = 66; // adjust to number between 0 and 100 to see changes
            //int trapMin = 5;  // adjust to number between 0 and 255 to see changes
            //int trapMax = 30; // adjust to number between 0 and 255 to see changes

            //initialize map array to empty
            for (y = 0; y < 50; y++) {
                for (x = 0; x < 50; x++) {
                    mapArray[y, x] = ' ';
                }
            }

            //initialize all rooms+halls to closed by default
            for (i = 0; i < 16; i++) {
                room[i, 0] = CLOSED;
            }
            for (i = 0; i < 12; i++) {
                vhall[i] = CLOSED;
                hhall[i] = CLOSED;
            }

            // path generation algorithm
            start = random.Next(0, 16); // randomly determine start room
            x = start;
            i = 0;
            j = random.Next(0, 6) + 5; // magic numbers, determine what the dungeon looks like
            b = -1;
            do {
                if (random.Next(0, (2 + i)) == 0) {
                    room[x, 0] = OPEN;
                    i++;
                    x = start;
                    b = -1;
                } else {
                    bool working = true;
                    do {
                        y = random.Next(0, 4);
                        if (y != b) {//makes sure there is no backtracking
                            switch (y) {
                                case 0:
                                    a = getRoomUp(x);
                                    b = 1;
                                    break;
                                case 1:
                                    a = getRoomDown(x);
                                    b = 0;
                                    break;
                                case 2:
                                    a = getRoomLeft(x);
                                    b = 3;
                                    break;
                                case 3:
                                    a = getRoomRight(x);
                                    b = 2;
                                    break;
                            }
                            if (a > -1) {
                                openHallBetween(x, a, hhall, vhall);
                                x = a;
                                working = false;
                            }
                        } else {
                            b = -1;
                        }
                    } while (working);
                    if (random.Next(0, 2) == 0) {
                        room[x, 0] = OPEN;
                    } else {
                        room[x, 0] = HALL;
                    }
                }
            } while (i < j);
            room[start, 0] = OPEN;

            //Determine key rooms

            isDone = false;
            do { //determine ending room randomly
                x = random.Next(0, 16);
                if (room[x, 0] == OPEN) {
                    end = x;
                    //Console.WriteLine(x);
                    isDone = true;
                }
            } while (!isDone);

            isDone = false;
            do { //determine starting room randomly
                x = random.Next(0, 16);
                if (room[x, 0] == OPEN) {
                    start = x;
                    isDone = true;
                }
            } while (!isDone);


            // begin part 2, creating ASCII map
            //create rooms
            //Console.WriteLine("ROOMS:");
            for (i = 0; i < 16; i++) {
                if (room[i, 0] != CLOSED) {
                    createRoom(i, room, options);
                }
            }
            //Console.WriteLine("DRAW:");
            for (i = 0; i < 16; i++) {
                if (room[i, 0] != CLOSED) {
                    drawRoom(i, mapArray, room);
                }
            }
            //Console.WriteLine("HALLS:");

            for (i = 0; i < 16; i++) {
                if (room[i, 0] != CLOSED) {
                    Console.WriteLine(i + ": X=" + room[i, 1] + "-" + room[i, 3] + " Y=" + room[i, 2] + "-" + room[i, 4]);
                }
            }

            // create halls
            for (i = 0; i < 12; i++) {
                if (vhall[i] == OPEN) {
                    createVHall(i, mapArray, room, options);
                }
                if (hhall[i] == OPEN) {
                    createHHall(i, mapArray, room, options);
                }
            }
            //Console.WriteLine("SE:");
            addSEpos(start, START, mapArray, room);

            addSEpos(end, END, mapArray, room);

            //Console.WriteLine("WATER:");
            //add water
            for (y = 1; y < 49; y++) {

                for (x = 1; x < 49; x++) {

                    if (mapArray[y, x] != ' ') {

                    } else if (options.WaterFrequency < 1) {

                    } else if (options.WaterFrequency > 99) {

                        mapArray[y, x] = '~';

                    } else if (random.Next(0, 100) + 1 <= options.WaterFrequency) { // dotchi ~nyo? Water or Wall?

                        mapArray[y, x] = '~';

                    } else {

                    }

                }
            }

            if (options.Craters > 0) {

                for (i = 0; i < options.Craters; i++) {

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



                    for (y = startY; y < startY + craterLength; y++) {

                        for (x = startX; x < startX + craterLength; x++) {



                            if (x < 1 || x > 47 || y < 1 || y > 47) {

                            } else if (mapArray[y, x] == '~' || mapArray[y, x] == ',' || mapArray[y, x] == '.' || mapArray[y, x] == 'S' || mapArray[y, x] == 'E') {

                            } else {

                                if (System.Math.Abs(x - centerX) + System.Math.Abs(y - centerY) <= craterLength / 2) {//makes a crater in a "diamond" shape

                                    mapArray[y, x] = '~';

                                } else if (options.CraterFuzzy == true && System.Math.Abs(x - centerX) + System.Math.Abs(y - centerY) <= craterLength * 0.75) {//makes a "fuzzy" edge to the diamond crater

                                    if (random.Next(0, 100) > 49) {
                                        mapArray[y, x] = '~';
                                    }
                                }
                            }
                        }
                    }


                }
            }


            //Console.WriteLine("TRAPS:");
            // add traps
            int finalTraps = random.Next(options.TrapMin, options.TrapMax + 1);
            if (finalTraps > 0) {
                for (i = 0; i < finalTraps; i++) { // add traps
                    for (j = 0; j < 200; j++) {
                        a = random.Next(0, 16);
                        x = random.Next(room[a, 1], room[a, 3] + 1);
                        y = random.Next(room[a, 2], room[a, 4] + 1);
                        if (mapArray[y, x] == '.') {
                            mapArray[y, x] = 'Q';
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

            return mapArray;
        }

        public static void TextureDungeon(char[,] mapArray) {
            //Console.WriteLine("TEXTURE:");
            //texture water

            bool[] checkCross = new bool[4];

            for (int y = 0; y < 50; y++) {

                for (int x = 0; x < 50; x++) {

                    if (mapArray[y, x] == ' ') {

                        //} else {

                        checkCross[0] = CheckWall(x, y, 1, mapArray);
                        checkCross[1] = CheckWall(x, y, 2, mapArray);
                        checkCross[2] = CheckWall(x, y, 3, mapArray);
                        checkCross[3] = CheckWall(x, y, 4, mapArray);

                        if (checkCross[0] == false && checkCross[1] == false && checkCross[2] == false && checkCross[3] == false) {

                            mapArray[y, x] = 'O';

                        } else if (checkCross[0] == true && checkCross[1] == false && checkCross[2] == false && checkCross[3] == false) {

                            if (CheckWall(x, y, 5, mapArray) == false && CheckWall(x, y, 8, mapArray) == false) {

                                mapArray[y, x] = 'U';
                            } else {

                                mapArray[y, x] = 'O';
                            }
                        } else if (checkCross[0] == false && checkCross[1] == true && checkCross[2] == false && checkCross[3] == false) {

                            if (CheckWall(x, y, 5, mapArray) == false && CheckWall(x, y, 6, mapArray) == false) {

                                mapArray[y, x] = 'C';
                                RowLoop(x, y, mapArray);
                            } else {

                                mapArray[y, x] = 'O';
                            }

                        } else if (checkCross[0] == false && checkCross[1] == false && checkCross[2] == true && checkCross[3] == false) {

                            if (CheckWall(x, y, 6, mapArray) == false && CheckWall(x, y, 7, mapArray) == false) {

                                mapArray[y, x] = 'A';
                                ColumnLoop(x, y, mapArray);
                            } else {

                                mapArray[y, x] = 'O';
                            }

                        } else if (checkCross[0] == false && checkCross[1] == false && checkCross[2] == false && checkCross[3] == true) {

                            if (CheckWall(x, y, 7, mapArray) == false && CheckWall(x, y, 8, mapArray) == false) {

                                mapArray[y, x] = 'D';
                                RowLoop(x, y, mapArray);
                            } else {

                                mapArray[y, x] = 'O';
                            }



                        } else if (checkCross[0] == true && checkCross[1] == true && checkCross[2] == false && checkCross[3] == false) {

                            if (CheckWall(x, y, 5, mapArray) == true) {

                                mapArray[y, x] = 'L';

                            } else {

                                mapArray[y, x] = 'O';
                            }

                        } else if (checkCross[0] == true && checkCross[1] == false && checkCross[2] == true && checkCross[3] == false) {

                            if (CheckWall(x, y, 5, mapArray) == false && CheckWall(x, y, 6, mapArray) == false && CheckWall(x, y, 7, mapArray) == false && CheckWall(x, y, 8, mapArray) == false) {

                                mapArray[y, x] = 'H';
                                ColumnLoop(x, y, mapArray);
                            } else {

                                mapArray[y, x] = 'O';
                            }

                        } else if (checkCross[0] == true && checkCross[1] == false && checkCross[2] == false && checkCross[3] == true) {

                            if (CheckWall(x, y, 8, mapArray) == true) {

                                mapArray[y, x] = 'J';

                            } else {

                                mapArray[y, x] = 'O';
                            }

                        } else if (checkCross[0] == false && checkCross[1] == true && checkCross[2] == true && checkCross[3] == false) {

                            if (CheckWall(x, y, 6, mapArray) == true) {

                                mapArray[y, x] = 'F';
                                //Console.WriteLine("Corner1&2");
                                RightWallLoop(x, y, mapArray);
                                BottomWallLoop(x, y, mapArray);
                            } else {

                                mapArray[y, x] = 'O';
                            }

                        } else if (checkCross[0] == false && checkCross[1] == true && checkCross[2] == false && checkCross[3] == true) {

                            if (CheckWall(x, y, 5, mapArray) == false && CheckWall(x, y, 6, mapArray) == false && CheckWall(x, y, 7, mapArray) == false && CheckWall(x, y, 8, mapArray) == false) {

                                mapArray[y, x] = 'Z';

                            } else {

                                mapArray[y, x] = 'O';
                            }

                        } else if (checkCross[0] == false && checkCross[1] == false && checkCross[2] == true && checkCross[3] == true) {

                            if (CheckWall(x, y, 7, mapArray) == true) {

                                mapArray[y, x] = '7';

                            } else {

                                mapArray[y, x] = 'O';
                            }

                        } else if (checkCross[0] == false && checkCross[1] == true && checkCross[2] == true && checkCross[3] == true) {

                            if (CheckWall(x, y, 6, mapArray) == true && CheckWall(x, y, 7, mapArray) == true) {

                                mapArray[y, x] = 'T';
                                BottomWallLoop(x, y, mapArray);

                            } else if (CheckWall(x, y, 6, mapArray) == false && CheckWall(x, y, 7, mapArray) == true) {

                                mapArray[y, x] = '7';

                            } else if (CheckWall(x, y, 6, mapArray) == true && CheckWall(x, y, 7, mapArray) == false) {

                                mapArray[y, x] = 'F';
                                //Console.WriteLine("Corner1&2&3");
                                RightWallLoop(x, y, mapArray);
                                BottomWallLoop(x, y, mapArray);

                            } else {

                                mapArray[y, x] = 'O';
                            }

                        } else if (checkCross[0] == true && checkCross[1] == false && checkCross[2] == true && checkCross[3] == true) {

                            if (CheckWall(x, y, 7, mapArray) == true && CheckWall(x, y, 8, mapArray) == true) {

                                mapArray[y, x] = ']';

                            } else if (CheckWall(x, y, 7, mapArray) == false && CheckWall(x, y, 8, mapArray) == true) {

                                mapArray[y, x] = 'J';

                            } else if (CheckWall(x, y, 7, mapArray) == true && CheckWall(x, y, 8, mapArray) == false) {

                                mapArray[y, x] = '7';

                            } else {

                                mapArray[y, x] = 'O';
                            }

                        } else if (checkCross[0] == true && checkCross[1] == true && checkCross[2] == false && checkCross[3] == true) {

                            if (CheckWall(x, y, 5, mapArray) == true && CheckWall(x, y, 8, mapArray) == true) {

                                mapArray[y, x] = '_';

                            } else if (CheckWall(x, y, 5, mapArray) == false && CheckWall(x, y, 8, mapArray) == true) {

                                mapArray[y, x] = 'J';

                            } else if (CheckWall(x, y, 5, mapArray) == true && CheckWall(x, y, 8, mapArray) == false) {

                                mapArray[y, x] = 'L';

                            } else {

                                mapArray[y, x] = 'O';
                            }

                        } else if (checkCross[0] == true && checkCross[1] == true && checkCross[2] == true && checkCross[3] == false) {

                            if (CheckWall(x, y, 5, mapArray) == true && CheckWall(x, y, 6, mapArray) == true) {

                                mapArray[y, x] = '[';
                                RightWallLoop(x, y, mapArray);

                            } else if (CheckWall(x, y, 5, mapArray) == false && CheckWall(x, y, 6, mapArray) == true) {

                                mapArray[y, x] = 'F';
                                //Console.WriteLine("Corner0&1&2");
                                RightWallLoop(x, y, mapArray);
                                BottomWallLoop(x, y, mapArray);

                            } else if (CheckWall(x, y, 5, mapArray) == true && CheckWall(x, y, 6, mapArray) == false) {

                                mapArray[y, x] = 'L';

                            } else {

                                mapArray[y, x] = 'O';
                            }

                        } else if (checkCross[0] == true && checkCross[1] == true && checkCross[2] == true && checkCross[3] == true) {

                            checkCross[0] = CheckWall(x, y, 5, mapArray);
                            checkCross[1] = CheckWall(x, y, 6, mapArray);
                            checkCross[2] = CheckWall(x, y, 7, mapArray);
                            checkCross[3] = CheckWall(x, y, 8, mapArray);

                            if (checkCross[0] == false && checkCross[1] == false && checkCross[2] == false && checkCross[3] == false) {

                                mapArray[y, x] = 'O';

                            } else if (checkCross[0] == true && checkCross[1] == false && checkCross[2] == false && checkCross[3] == false) {

                                mapArray[y, x] = 'L';

                            } else if (checkCross[0] == false && checkCross[1] == true && checkCross[2] == false && checkCross[3] == false) {

                                mapArray[y, x] = 'F';
                                //Console.WriteLine("CornerAll+1");
                                RightWallLoop(x, y, mapArray);
                                BottomWallLoop(x, y, mapArray);

                            } else if (checkCross[0] == false && checkCross[1] == false && checkCross[2] == true && checkCross[3] == false) {

                                mapArray[y, x] = '7';

                            } else if (checkCross[0] == false && checkCross[1] == false && checkCross[2] == false && checkCross[3] == true) {

                                mapArray[y, x] = 'J';

                            } else if (checkCross[0] == true && checkCross[1] == true && checkCross[2] == false && checkCross[3] == false) {

                                mapArray[y, x] = '[';
                                RightWallLoop(x, y, mapArray);

                            } else if (checkCross[0] == true && checkCross[1] == false && checkCross[2] == true && checkCross[3] == false) {

                                mapArray[y, x] = 'O';

                            } else if (checkCross[0] == true && checkCross[1] == false && checkCross[2] == false && checkCross[3] == true) {

                                mapArray[y, x] = '_';

                            } else if (checkCross[0] == false && checkCross[1] == true && checkCross[2] == true && checkCross[3] == false) {

                                mapArray[y, x] = 'T';
                                BottomWallLoop(x, y, mapArray);

                            } else if (checkCross[0] == false && checkCross[1] == true && checkCross[2] == false && checkCross[3] == true) {

                                mapArray[y, x] = 'J';

                            } else if (checkCross[0] == false && checkCross[1] == false && checkCross[2] == true && checkCross[3] == true) {

                                mapArray[y, x] = ']';


                            } else if (checkCross[0] == false && checkCross[1] == true && checkCross[2] == true && checkCross[3] == true) {

                                mapArray[y, x] = '3';

                            } else if (checkCross[0] == true && checkCross[1] == false && checkCross[2] == true && checkCross[3] == true) {

                                mapArray[y, x] = '1';

                            } else if (checkCross[0] == true && checkCross[1] == true && checkCross[2] == false && checkCross[3] == true) {

                                mapArray[y, x] = '2';

                            } else if (checkCross[0] == true && checkCross[1] == true && checkCross[2] == true && checkCross[3] == false) {

                                mapArray[y, x] = '4';

                            } else if (checkCross[0] == true && checkCross[1] == true && checkCross[2] == true && checkCross[3] == true) {

                                mapArray[y, x] = 'X';

                            }
                        }
                    } else if (mapArray[y, x] == '~') {
                        checkCross[0] = CheckWater(x, y - 1, mapArray);
                        checkCross[1] = CheckWater(x + 1, y, mapArray);
                        checkCross[2] = CheckWater(x, y + 1, mapArray);
                        checkCross[3] = CheckWater(x - 1, y, mapArray);

                        mapArray[y, x] = (char)256;

                        if (checkCross[0] == false && checkCross[1] == false && checkCross[2] == false && checkCross[3] == false) {

                        } else if (checkCross[0] == true && checkCross[1] == false && checkCross[2] == false && checkCross[3] == false) {

                            mapArray[y, x] = (char)((int)mapArray[y, x] + 1);

                        } else if (checkCross[0] == false && checkCross[1] == true && checkCross[2] == false && checkCross[3] == false) {

                            mapArray[y, x] = (char)((int)mapArray[y, x] + 2);

                        } else if (checkCross[0] == false && checkCross[1] == false && checkCross[2] == true && checkCross[3] == false) {

                            mapArray[y, x] = (char)((int)mapArray[y, x] + 3);

                        } else if (checkCross[0] == false && checkCross[1] == false && checkCross[2] == false && checkCross[3] == true) {

                            mapArray[y, x] = (char)((int)mapArray[y, x] + 4);

                        } else if (checkCross[0] == true && checkCross[1] == true && checkCross[2] == false && checkCross[3] == false) {

                            mapArray[y, x] = (char)((int)mapArray[y, x] + 5);

                        } else if (checkCross[0] == true && checkCross[1] == false && checkCross[2] == true && checkCross[3] == false) {

                            mapArray[y, x] = (char)((int)mapArray[y, x] + 6);

                        } else if (checkCross[0] == true && checkCross[1] == false && checkCross[2] == false && checkCross[3] == true) {

                            mapArray[y, x] = (char)((int)mapArray[y, x] + 7);

                        } else if (checkCross[0] == false && checkCross[1] == true && checkCross[2] == true && checkCross[3] == false) {

                            mapArray[y, x] = (char)((int)mapArray[y, x] + 8);

                        } else if (checkCross[0] == false && checkCross[1] == true && checkCross[2] == false && checkCross[3] == true) {

                            mapArray[y, x] = (char)((int)mapArray[y, x] + 9);

                        } else if (checkCross[0] == false && checkCross[1] == false && checkCross[2] == true && checkCross[3] == true) {

                            mapArray[y, x] = (char)((int)mapArray[y, x] + 10);

                        } else if (checkCross[0] == false && checkCross[1] == true && checkCross[2] == true && checkCross[3] == true) {

                            mapArray[y, x] = (char)((int)mapArray[y, x] + 11);

                        } else if (checkCross[0] == true && checkCross[1] == false && checkCross[2] == true && checkCross[3] == true) {

                            mapArray[y, x] = (char)((int)mapArray[y, x] + 12);

                        } else if (checkCross[0] == true && checkCross[1] == true && checkCross[2] == false && checkCross[3] == true) {

                            mapArray[y, x] = (char)((int)mapArray[y, x] + 13);

                        } else if (checkCross[0] == true && checkCross[1] == true && checkCross[2] == true && checkCross[3] == false) {

                            mapArray[y, x] = (char)((int)mapArray[y, x] + 14);

                        } else if (checkCross[0] == true && checkCross[1] == true && checkCross[2] == true && checkCross[3] == true) {

                            mapArray[y, x] = (char)((int)mapArray[y, x] + 15);

                        }

                        if (mapArray[y, x] == 261 || mapArray[y, x] == 263 || mapArray[y, x] == 264 || mapArray[y, x] > 265) {

                            checkCross[0] = CheckWater(x + 1, y - 1, mapArray);
                            checkCross[1] = CheckWater(x + 1, y + 1, mapArray);
                            checkCross[2] = CheckWater(x - 1, y + 1, mapArray);
                            checkCross[3] = CheckWater(x - 1, y - 1, mapArray);


                            //checks diagonal /

                            if (checkCross[0] == true && checkCross[2] == true) {


                            } else if (checkCross[0] == false && checkCross[2] == true && (mapArray[y, x] == 261 || mapArray[y, x] == 269 || mapArray[y, x] == 270 || mapArray[y, x] == 271)) {

                                mapArray[y, x] = (char)((int)mapArray[y, x] + 16);

                            } else if (checkCross[0] == true && checkCross[2] == false && (mapArray[y, x] == 266 || mapArray[y, x] == 267 || mapArray[y, x] == 268 || mapArray[y, x] == 271)) {

                                mapArray[y, x] = (char)((int)mapArray[y, x] + 32);

                            } else if (checkCross[0] == false && checkCross[2] == false) {

                                if (mapArray[y, x] == 271) {

                                    mapArray[y, x] = (char)((int)mapArray[y, x] + 48);

                                } else if (mapArray[y, x] == 261 || mapArray[y, x] == 269 || mapArray[y, x] == 270) {

                                    mapArray[y, x] = (char)((int)mapArray[y, x] + 16);

                                } else if (mapArray[y, x] == 266 || mapArray[y, x] == 267 || mapArray[y, x] == 268) {

                                    mapArray[y, x] = (char)((int)mapArray[y, x] + 32);

                                }
                            }

                            // checks diagonal \

                            if (checkCross[1] == true && checkCross[3] == true) {


                            } else if (checkCross[1] == false && checkCross[3] == true && ((int)mapArray[y, x] % 16 == 8 || (int)mapArray[y, x] % 16 == 11 || (int)mapArray[y, x] % 16 == 14 || (int)mapArray[y, x] % 16 == 15)) {

                                mapArray[y, x] = (char)((int)mapArray[y, x] + 64);

                            } else if (checkCross[1] == true && checkCross[3] == false && ((int)mapArray[y, x] % 16 == 7 || (int)mapArray[y, x] % 16 == 12 || (int)mapArray[y, x] % 16 == 13 || (int)mapArray[y, x] % 16 == 15)) {

                                mapArray[y, x] = (char)((int)mapArray[y, x] + 128);

                            } else if (checkCross[1] == false && checkCross[3] == false) {

                                if ((int)mapArray[y, x] % 16 == 15) {

                                    mapArray[y, x] = (char)((int)mapArray[y, x] + 192);

                                } else if ((int)mapArray[y, x] % 16 == 8 || (int)mapArray[y, x] % 16 == 11 || (int)mapArray[y, x] % 16 == 14) {

                                    mapArray[y, x] = (char)((int)mapArray[y, x] + 64);

                                } else if ((int)mapArray[y, x] % 16 == 7 || (int)mapArray[y, x] % 16 == 12 || (int)mapArray[y, x] % 16 == 13) {

                                    mapArray[y, x] = (char)((int)mapArray[y, x] + 128);

                                }
                            }

                        }


                    }
                }
            }
        }

        public static void GenItem(char[,] mapArray, bool ground, bool water, bool wall, ref int x, ref int y) {
            //int n = random.Next(options.ItemMin, options.ItemMax + 1);
            //for (i = 0; i < n; i++) {
            //bool success = false;
            for (int k = 0; k < 300; k++) { // do until you succeed
                x = random.Next(1, 49);
                y = random.Next(1, 49);
                if (mapArray[y, x] == '.') {
                    //mapArray[y, x] = (char)(96 + i); //ascii for lowercase letters
                    if (ground) return;
                } else if (mapArray[y, x] > 255) {
                    //mapArray[y, x] = (char)(96 + i); //ascii for lowercase letters
                    if (water) return;
                } else if (mapArray[y, x] != 'Q' && mapArray[y, x] != 'E' && mapArray[y, x] != 'S' && mapArray[y, x] != ',' && mapArray[y, x] < 256) {
                    //mapArray[y, x] = (char)(96 + i); //ascii for lowercase letters
                    if (wall) return;
                }
            }
            x = -1;
            y = -1;
            //}


        }

        static void createRoom(int roomNum, int[,] room, GeneratorOptions options) {
            if (room[roomNum, 1] > 0) {
                return;
            }

            int x = 0, y = 0, u, v, w, l; // variables used for position
            convertRoomToXY(roomNum, ref x, ref y);

            //Determine room length/width
            if (room[roomNum, 0] == HALL) {
                w = 0;
                l = 0;
            } else {
                w = random.Next(options.RoomWidthMin, options.RoomWidthMax + 1);
                l = random.Next(options.RoomLengthMin, options.RoomLengthMax + 1);
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

            // fill in the walls first, then plot the inside
            //mapArray[y, x] = '5';
            //x++;
            //for (; x < u; x++) mapArray[y, x] = 'T';
            //mapArray[y, x] = '6';
            //y++;
            //for (; y < v; y++) mapArray[y, x] = 'R';
            //mapArray[v, u] = '7';
            //x = u - 7;
            //y = v - 6;
            //u--;
            //for (; u > x; u--) mapArray[v, u] = 'B';
            //mapArray[v, u] = '8';
            //v--;
            //for (; v > y; v--) mapArray[v, u] = 'L';

            //u = x + 6;
            //v = y + 5;
            // now render inside as ground tiles

            //count all rooms whose centers have been touched by this room as this room
            //Console.Write("#" + roomNum + "," + x + "," + y + "," + u + "," + v);
            //if (l <=6 || w <= 6) {

            //                room[roomNum,1] = x;
            //		room[roomNum,2] = y;
            //		room[roomNum,3] = u;
            //		room[roomNum,4] = v;
            //} else {
            for (int i = 0; i < 16; i++) {
                if (i == roomNum || ((6 + i % 4 * 12) >= x && (6 + i % 4 * 12) <= u && (6 + i / 4 * 12) >= y && (6 + i / 4 * 12) <= v)) {
                    room[i, 1] = x;
                    room[i, 2] = y;
                    room[i, 3] = u;
                    room[i, 4] = v;
                    //Console.Write(" =" + i);
                }
            }
            //}

            //Console.WriteLine(";");



            // done 
        }

        static void drawRoom(int roomNum, char[,] mapArray, int[,] room) {
            int x, y, u, v;

            x = room[roomNum, 1];
            y = room[roomNum, 2];

            u = room[roomNum, 3];
            v = room[roomNum, 4];

            //Console.WriteLine("#" + roomNum + "," + x + "," + y + "," + u + "," + v);
            for (int i = y; i <= v; i++) {
                for (int j = x; j <= u; j++) {
                    mapArray[i, j] = '.';
                }
            }

        }

        static void createHHall(int hall, char[,] mapArray, int[,] room, GeneratorOptions options) {
            //Console.Write("H"+hall+":");
            int startRoom = hall + hall / 3;
            int endRoom = hall + hall / 3 + 1;

            int n = room[endRoom, 1] - room[startRoom, 3];

            if (n < 1) {
                if (room[startRoom, 2] > room[endRoom, 4] || room[startRoom, 4] < room[endRoom, 2]) {
                    n++;
                } else {
                    //Console.WriteLine(";");
                    return;
                }
            }
            n--;

            int x, y, m,/* n,*/ h, r = 0, var;

            x = room[startRoom, 3];

            m = random.Next(options.HallTurnMin, options.HallTurnMax + 1);


            if (m > ((n - 1) / 2)) m = (n - 1) / 2;


            y = random.Next(room[startRoom, 2], room[startRoom, 4] + 1);

            if (m <= 0 && (y > room[endRoom, 4] || y < room[endRoom, 2])) {
                m = 1;
            }


            //Console.Write("("+room[startRoom,2]+"."+room[startRoom,4]+")");
            for (int i = 0; i <= m; i++) {
                if (i == m) {
                    var = random.Next(room[endRoom, 2], room[endRoom, 4] + 1) - y;
                } else {
                    var = random.Next(options.HallVarMin, options.HallVarMax + 1);
                    if (random.Next(0, 2) == 0) {
                        var = -var;
                    }
                }
                if (i != 0) {
                    if ((y + var) < 1) var = 1 - y;
                    if ((y + var) > 48) var = 48 - y;
                    addVertHall(x, y, var, mapArray);

                    y += var;
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
                addHorizHall(x, y, h, mapArray);

                x += h;
            }

            mapArray[y, x + 1] = ',';


            //int x, y, h, j;
            //h = hall % 3;
            //j = hall / 3;

            //x = 8 + 13 * h;
            //y = 3 + 12 * j;

            //addHorizHall(x, y, 6, mapArray);
            //Console.WriteLine(";");
        }

        static void createVHall(int hall, char[,] mapArray, int[,] room, GeneratorOptions options) {
            //Console.Write("V"+hall+":");            
            int startRoom = hall;
            int endRoom = hall + 4;

            int n = room[endRoom, 2] - room[startRoom, 4];

            if (n < 1) {
                if (room[startRoom, 1] > room[endRoom, 3] || room[startRoom, 3] < room[endRoom, 1]) {

                    n++;
                } else {
                    //Console.WriteLine(";");
                    return;
                }
            }
            n--;

            int x, y, m,/* n,*/ h, r = 0, var;

            y = room[startRoom, 4];

            m = random.Next(options.HallTurnMin, options.HallTurnMax + 1);


            if (m > ((n - 1) / 2)) m = (n - 1) / 2;


            x = random.Next(room[startRoom, 1], room[startRoom, 3] + 1);

            if (m <= 0 && (x > room[endRoom, 3] || x < room[endRoom, 1])) {
                m = 1;
            }


            //Console.Write("("+room[startRoom,1]+"."+room[startRoom,3]+")");
            for (int i = 0; i <= m; i++) {
                if (i == m) {
                    var = random.Next(room[endRoom, 1], room[endRoom, 3] + 1) - x;
                } else {
                    var = random.Next(options.HallVarMin, options.HallVarMax + 1);
                    if (random.Next(0, 2) == 0) {
                        var = -var;
                    }
                }
                if (i != 0) {
                    if ((x + var) < 1) var = 1 - x;
                    if ((x + var) > 48) var = 48 - x;
                    addHorizHall(x, y, var, mapArray);

                    x += var;
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
                addVertHall(x, y, h, mapArray);

                y += h;
            }

            mapArray[y + 1, x] = ',';


            //int x, y, h, j; // variables for position
            //h = hall % 4;
            //j = hall / 4;

            //x = 3 + Convert.ToInt32(13.5 * h);
            //y = 7 + 12 * j;

            //addVertHall(x, y, mapArray);
            //Console.WriteLine(";");
        }

        static void addHorizHall(int x, int y, int steps, char[,] mapArray) {//starts at first tile AFTER, works RIGHT >
            //int u;
            //u = x + 6;
            //Console.Write("H~" + x + "," + y + "," + steps);
            //mapArray[y, x] = '2';
            /*
            for (x++; x < u; x++) {
                mapArray[y, x] = 'T';
            }
            */
            bool steppedInRoom = (mapArray[y, x] == '.');
            //mapArray[y, x] = '1';
            //y++;
            if (steps > 0) {
                for (int i = 1; i <= steps; i++) {
                    if (mapArray[y, x + i] == ' ') {
                        mapArray[y, x + i - 1] = ',';
                        mapArray[y, x + i] = ',';
                    } else if (mapArray[y, x + i] == '.' && !steppedInRoom) {
                        steppedInRoom = true;
                        mapArray[y, x + i - 1] = ',';
                        mapArray[y, x + i] = ',';
                    }
                    //if (x+i > 48 || x+i<1) Console.Write("!!!");
                }
            } else {
                for (int i = -1; i >= steps; i--) {
                    //if (x+i > 48 || x+i<1) Console.Write("!!!");
                    if (mapArray[y, x + i] == ' ') {
                        mapArray[y, x + i + 1] = ',';
                        mapArray[y, x + i] = ',';
                    } else if (mapArray[y, x + i] == '.' && !steppedInRoom) {
                        steppedInRoom = true;
                        mapArray[y, x + i - 1] = ',';
                        mapArray[y, x + i] = ',';
                    }
                }
            }
            //y++;
            //x = u - 6;
            //mapArray[y, x] = '3';
            /*
            for (x++; x < u; x++) {
                mapArray[y, x] = 'B';
            }
            */
            //mapArray[y, x] = '4';
            // done
        }

        static void addVertHall(int x, int y, int steps, char[,] mapArray) {//starts at first tile AFTER, works DOWN v
            //int v;
            //v = y + 6;
            bool steppedInRoom = (mapArray[y, x] == '.');
            //mapArray[y, x] = '4';
            /*
            for (y++; y < v; y++) {
                mapArray[y, x] = 'L';
            }
            */
            //mapArray[y, x] = '1';
            //x++;
            //Console.Write("V~" + x + "," + y + "," + steps);
            if (steps > 0) {
                for (int i = 1; i <= steps; i++) {
                    //if (y+i > 48 || y+i<1) Console.Write("!!!");
                    if (mapArray[y + i, x] == ' ') {
                        mapArray[y + i - 1, x] = ',';
                        mapArray[y + i, x] = ',';
                    } else if (mapArray[y + i, x] == '.' && !steppedInRoom) {
                        steppedInRoom = true;
                        mapArray[y + i - 1, x] = ',';
                        mapArray[y + i, x] = ',';
                    }
                }
            } else {
                for (int i = -1; i >= steps; i--) {
                    //if (y+i > 48 || y+i<1) Console.Write("!!!");
                    if (mapArray[y + i, x] == ' ') {
                        mapArray[y + i + 1, x] = ',';
                        mapArray[y + i, x] = ',';
                    } else if (mapArray[y + i, x] == '.' && !steppedInRoom) {
                        steppedInRoom = true;
                        mapArray[y + i - 1, x] = ',';
                        mapArray[y + i, x] = ',';
                    }
                }
            }
            //x++;
            //y = v - 6;
            //mapArray[y, x] = '3';
            /*
            for (y++; y < v; y++) {
                mapArray[y, x] = 'R';
            }
            */
            //mapArray[y, x] = '2';
            // done
        }

        //Wall Loops and Wall Checks ~Spkun

        static void RightWallLoop(int x, int y, char[,] mapArray) {
            //Console.Write("/RWL:"+x+","+y);
            bool[] checkDown = new bool[4];

            while (true) {
                y++;

                if (mapArray[y, x] != ' ') {
                    break;
                }


                checkDown[0] = CheckWall(x, y, 4, mapArray);
                checkDown[1] = CheckWall(x, y, 7, mapArray);
                checkDown[2] = CheckWall(x, y, 3, mapArray);
                checkDown[3] = CheckWall(x, y, 6, mapArray);

                if (checkDown[2] == false || checkDown[3] == false) {

                    mapArray[y, x] = 'L';
                    break;

                } else if (checkDown[0] == true && checkDown[1] == true) {

                    mapArray[y, x] = '4';
                    break;

                } else {

                    mapArray[y, x] = '[';

                }
            }
        }

        static void BottomWallLoop(int x, int y, char[,] mapArray) {
            //Console.Write("/BWL:"+x+","+y);
            bool[] checkRight = new bool[3];

            while (true) {
                x++;

                if (mapArray[y, x] != ' ') {
                    break;
                }


                checkRight[0] = CheckWall(x, y, 5, mapArray);
                checkRight[1] = CheckWall(x, y, 2, mapArray);
                checkRight[2] = CheckWall(x, y, 6, mapArray);

                if (checkRight[0] == true && checkRight[1] == true && checkRight[2] == true) {

                    mapArray[y, x] = 'T';
                    break;

                } else if (checkRight[0] == false && checkRight[1] == true && checkRight[2] == true) {

                    mapArray[y, x] = 'T';

                } else {

                    mapArray[y, x] = '7';
                    break;

                }
            }
        }

        static void ColumnLoop(int x, int y, char[,] mapArray) {

            bool[] checkColumn = new bool[3];

            while (true) {
                y++;

                if (mapArray[y, x] != ' ') {
                    break;
                }

                checkColumn[0] = CheckWall(x, y, 7, mapArray);
                checkColumn[1] = CheckWall(x, y, 3, mapArray);
                checkColumn[2] = CheckWall(x, y, 6, mapArray);

                if (checkColumn[0] == false && checkColumn[1] == true && checkColumn[2] == false) {

                    mapArray[y, x] = 'H';

                } else {

                    mapArray[y, x] = 'U'; ;
                    break;

                }
            }
        }

        static void RowLoop(int x, int y, char[,] mapArray) {

            bool[] checkRow = new bool[3];

            while (true) {
                x++;

                if (mapArray[y, x] != ' ') {
                    break;
                }

                checkRow[0] = CheckWall(x, y, 5, mapArray);
                checkRow[1] = CheckWall(x, y, 2, mapArray);
                checkRow[2] = CheckWall(x, y, 6, mapArray);

                if (checkRow[0] == false && checkRow[1] == true && checkRow[2] == false) {

                    mapArray[y, x] = 'Z';

                } else {

                    mapArray[y, x] = 'D';
                    break;

                }
            }
        }

        static bool CheckWall(int x, int y, int dir, char[,] mapArray) {

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

            if (mapArray[y, x] == 'O' || mapArray[y, x] == '.' || mapArray[y, x] == ',' || mapArray[y, x] == '~' || mapArray[y, x] > 255 || mapArray[y, x] == 'S' || mapArray[y, x] == 'E') {
                return false;
            }

            if (dir == 1 || dir == 5 || dir == 8) {
                if (mapArray[y, x] == '_' || mapArray[y, x] == 'L' || mapArray[y, x] == 'J' || mapArray[y, x] == 'C' || mapArray[y, x] == 'Z' || mapArray[y, x] == 'D' || mapArray[y, x] == 'U') {
                    return false;
                }
                if (dir == 5 && (mapArray[y, x] == 'F' || mapArray[y, x] == '[' || mapArray[y, x] == 'A' || mapArray[y, x] == 'H' || mapArray[y, x] == '2')) {
                    return false;
                }
                if (dir == 8 && (mapArray[y, x] == '7' || mapArray[y, x] == ']' || mapArray[y, x] == 'A' || mapArray[y, x] == 'H' || mapArray[y, x] == '1')) {
                    return false;
                }
            } else if (dir == 2 || dir == 4) {
                if (mapArray[y, x] == 'A' || mapArray[y, x] == 'H' || mapArray[y, x] == 'U') {
                    return false;
                }
                if (dir == 2 && (mapArray[y, x] == '[' || mapArray[y, x] == 'C' || mapArray[y, x] == 'F' || mapArray[y, x] == 'L')) {
                    return false;
                }
                if (dir == 4 && (mapArray[y, x] == ']' || mapArray[y, x] == 'D' || mapArray[y, x] == '7' || mapArray[y, x] == 'J')) {
                    return false;
                }
            } else if (dir == 3 || dir == 6 || dir == 7) {
                if (mapArray[y, x] == 'T' || mapArray[y, x] == 'F' || mapArray[y, x] == '7' || mapArray[y, x] == 'C' || mapArray[y, x] == 'Z' || mapArray[y, x] == 'D' || mapArray[y, x] == 'A') {
                    return false;
                }
                if (dir == 6 && (mapArray[y, x] == 'L' || mapArray[y, x] == '[' || mapArray[y, x] == 'U' || mapArray[y, x] == 'H' || mapArray[y, x] == '4')) {
                    return false;
                }
                if (dir == 7 && (mapArray[y, x] == 'J' || mapArray[y, x] == ']' || mapArray[y, x] == 'U' || mapArray[y, x] == 'H' || mapArray[y, x] == '3')) {
                    return false;
                }
            }

            return true;
        }

        static bool CheckWater(int x, int y, char[,] mapArray) {
            if (mapArray[y, x] == '~' || mapArray[y, x] > 255) {
                return true;
            } else {
                return false;
            }
        }

        static void addSEpos(int roomNum, int type, char[,] mapArray, int[,] room) {

            int x = 0, y = 0, u = 0, v = 0;
            char c;
            if (type == START) c = 'S';
            else c = 'E';

            //convertRoomToXY(roomNum, ref x, ref y);
            x = room[roomNum, 1];
            y = room[roomNum, 2];
            u = room[roomNum, 3];
            v = room[roomNum, 4];

            //Console.WriteLine(roomNum + " " + type);
            //bool done = false;
            int randx = 0, randy = 0;
            for (int i = 0; i < 200; i++) {
                randx = random.Next(x, u + 1);
                randy = random.Next(y, v + 1);
                //Console.WriteLine(x + "," + y + "," + u + "," + v);
                if (mapArray[randy, randx] == '.') {
                    mapArray[randy, randx] = c;
                    return;
                }
            }
            while (true) {//backup plan in case rooms are so small that all there's left is ","
                randx = random.Next(x, u + 1);
                randy = random.Next(y, v + 1);
                //Console.WriteLine(x + "," + y + "," + u + "," + v);
                if (mapArray[randy, randx] == ',') {
                    mapArray[randy, randx] = c;
                    return;
                }
            }

        }

        static void convertRoomToXY(int room, ref int x, ref int y) {
            int a, b;
            a = room % 4;
            b = room / 4;
            x = 6 + a * 12;
            y = 6 + b * 12;
        }

        static void openHallBetween(int room1, int room2, int[] hhall, int[] vhall) {
            int x;
            if (room2 < room1) {
                int temp = room1;
                room1 = room2;
                room2 = temp;
            }
            if (room2 - room1 == 1) { //horizontal
                x = room2 / 4;
                hhall[room1 - x] = OPEN;
            } else { //vertical
                vhall[room1] = OPEN;
            }
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

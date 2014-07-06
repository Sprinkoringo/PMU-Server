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
using Server.Maps;

namespace Server.Missions
{
    public class GoalPoint
    {
        public int JobListIndex { get; set; }
        public int GoalX { get; set; }
        public int GoalY { get; set; }

        public void DetermineGoalPoint(IMap map)
        {
            
            // We'll try 100 times to randomly select a tile
            for (int i = 0; i < 100; i++)
            {
                int x = Server.Math.Rand(0, map.MaxX + 1);
                int y = Server.Math.Rand(0, map.MaxY + 1);

                // Check if the tile is walk able
                if (map.Tile[x, y].Type == Enums.TileType.Walkable)
                {
                    GoalX = x;
                    GoalY = y;
                    return;
                }
            }

            // Didn't select anything, so now we'll just try to find a free tile
            //if (!selected)
            //{
                for (int Y = 0; Y <= map.MaxY; Y++)
                {
                    for (int X = 0; X <= map.MaxX; X++)
                    {
                        if (map.Tile[X, Y].Type == Enums.TileType.Walkable)
                        {
                            GoalX = X;
                            GoalY = Y;
                            return;
                        }
                    }
                }
            //}
        }

        
    }
}

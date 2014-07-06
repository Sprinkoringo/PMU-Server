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

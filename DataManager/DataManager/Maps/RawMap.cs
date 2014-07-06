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

namespace DataManager.Maps
{
    public class RawMap
    {
        public RawMap(string mapID) {
            MapID = mapID;
            Name = "";
            Music = "";
            Npc = new List<MapNpcPreset>();
            MaxX = 19;
            MaxY = 14;
            Tile = new Tile[MaxX + 1, MaxY + 1];
        }

        public int Down {
            get;
            set;
        }

        public bool Indoors {
            get;
            set;
        }

        public int Left {
            get;
            set;
        }

        public string MapID {
            get;
            set;
        }

        public int MaxX {
            get;
            set;
        }

        public int MaxY {
            get;
            set;
        }

        public byte Moral {
            get;
            set;
        }

        public string Music {
            get;
            set;
        }

        public string Name {
            get;
            set;
        }

        public int MinNpcs { get; set; }
        public int MaxNpcs { get; set; }

        public int NpcSpawnTime { get; set; }

        public List<MapNpcPreset> Npc {
            get;
            set;
        }

        public int Darkness {
            get;
            set;
        }

        public int TimeLimit {
            get;
            set;
        }

        public int Revision {
            get;
            set;
        }

        public int Right {
            get;
            set;
        }

        public Tile[,] Tile {
            get;
            set;
        }

        public int Up {
            get;
            set;
        }

        public virtual byte Weather {
            get;
            set;
        }

        public int DungeonIndex {
            get;
            set;
        }

        public bool HungerEnabled {
            get;
            set;
        }

        public bool RecruitEnabled {
            get;
            set;
        }

        public bool ExpEnabled {
            get;
            set;
        }
    }
}

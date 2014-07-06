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
using System.Linq;
using System.Text;
using Server.Players;
using Server.Network;

namespace Server.Maps
{
    public class Void : BasicMap, IMap
    {
        public Enums.MapType MapType {
            get { return Enums.MapType.Void; }
        }

        public int Down {
            get { return 0; }
            set { }
        }

        public int Left {
            get { return 0; }
            set { }
        }

        public string MapID {
            get { return "void-" + owner.CharID; }
            set { }
        }

        public string IDPrefix {
            get { return "void"; }
        }

        public Enums.MapMoral Moral {
            get { return Enums.MapMoral.None; }
            set { }
        }

        public string Music {
            get;
            set;
        }

        public string Name {
            get { return ""; }
            set { }
        }

        public string Owner {
            get { return ""; }
            set { }
        }

        public int Revision {
            get { return 1; }
            set { }
        }

        public int Right {
            get { return 0; }
            set { }
        }

        public int Up {
            get { return 0; }
            set { }
        }

        public bool HungerEnabled {
            get { return false; }
            set { }
        }

        public void Save() {
            // No saving The Void!
        }

        public void Save(string filePath) {
            // No saving The Void!
        }

        public bool Load() {
            return true;
        }

        public bool Load(string filePath) {
            return true;
        }

        public void RemakePlayersList() {
            PlayersOnMap.Clear();
            PlayersOnMap.Add(owner.CharID);
        }

        public Player PlayerOwner {
            get { return owner; }
        }

        public bool Cacheable {
            get { return false; }
        }

        public bool IsProcessingComplete() {
            if (Npc.Count < 1) return true;

            int npcsActive = 0;


            for (int i = 0; i < Constants.MAX_MAP_NPCS; i++) {
                if (ActiveNpc[i].Num > 0) {
                    npcsActive++;
                    // An npc is still dead, so processing of this map is incomplete

                }
            }

            if (npcsActive >= MaxNpcs) {
                return true;
            } else {
                return false;
            }
        }

        public bool SafeExit { get; set; }
        Player owner;
        public Void(Player owner)
            : base(new DataManager.Maps.MapDump("void-" + owner.CharID, Constants.MAX_MAP_NPCS, Constants.MAX_MAP_ITEMS)) {
            this.owner = owner;
            RemakePlayersList();
            MaxX = 19;
            MaxY = 14;
            Tile = new TileCollection(BaseMap, MaxX, MaxY);
            Load();
        }
    }
}

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
using Server.Network;

namespace Server.Maps
{
    public class House : BasicMap, IMap
    {
        internal DataManager.Maps.HouseMap baseMap;

        Object lockObject = new object();

        public int Room {
            get { return baseMap.Room; }
            set { baseMap.Room = value; }
        }
        public int StartX {
            get { return baseMap.StartX; }
            set { baseMap.StartX = value; }
        }
        public int StartY {
            get { return baseMap.StartY; }
            set { baseMap.StartY = value; }
        }

        public string OwnerID {
            get { return baseMap.Owner; }
        }

        public const string ID_PREFIX = "h";
        public const int SHOP_PRICE = 2;
        public const int NOTICE_PRICE = 600;
        public const int SOUND_PRICE = 500;
        public const int WORD_PRICE = 10;
        public const int WEATHER_PRICE = 300;
        public const int LIGHT_PRICE = 400;
        public const int TILE_PRICE = 500;

        public Enums.MapType MapType {
            get { return Enums.MapType.House; }
        }

        public bool Cacheable {
            get { return true; }
        }

        public House(DataManager.Maps.HouseMap baseMap)
            : base(baseMap) {
            this.baseMap = baseMap;
        }

        //public House(string ownerID, int room)
        //    : base(MapManager.GenerateHouseID(ownerID, room)) {
        //    this.Moral = Enums.MapMoral.House;

        //    Client client = ClientManager.GetClient(Players.PlayerID.FindTcpID(ownerID));
        //    if (client != null) {
        //        base.Name = client.Player.Name + "'s House";
        //    } else {
        //        //base.Name = Players.PlayerManager.RetrieveCharacterName(client.Database, ownerID) + "'s House";
        //    }

        //    string filePath = IO.Paths.MapsFolder + "Houses\\" + ownerID + "\\" + MapID + ".mapdat";
        //    if (IO.IO.FileExists(filePath) == false) {
        //        Save(filePath);
        //    }
        //}

        public string IDPrefix {
            get { return ID_PREFIX; }
        }

        public bool IsProcessingComplete() {
            return true;
        }

        public void Save() {
            lock (lockObject) {
                using (Database.DatabaseConnection dbConnection = new Database.DatabaseConnection(Database.DatabaseID.Data)) {
                    MapManager.SaveHouseMap(dbConnection, MapID, this);
                }
            }
        }
    }
}

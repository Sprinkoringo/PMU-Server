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
using DataManager.Maps;

namespace Server.Maps
{
    public class TileCollection
    {
        RawMap rawMap;
        Tile[,] tiles;

        public TileCollection(RawMap rawMap) {
            this.rawMap = rawMap;
            tiles = new Tile[rawMap.MaxX + 1, rawMap.MaxY + 1];

            if (rawMap.Tile != null) {
                for (int x = 0; x <= MaxX; x++) {
                    for (int y = 0; y <= MaxY; y++) {
                        if (rawMap.Tile[x, y] == null) {
                            rawMap.Tile[x, y] = new DataManager.Maps.Tile();
                        }
                        tiles[x, y] = new Tile(rawMap.Tile[x, y]);
                    }
                }
            }
        }

        public TileCollection(RawMap rawMap, int maxX, int maxY) {
            this.rawMap = rawMap;
            rawMap.MaxX = maxX;
            rawMap.MaxY = maxY;

            rawMap.Tile = new DataManager.Maps.Tile[MaxX + 1, MaxY + 1];
            tiles = new Tile[MaxX + 1, MaxY + 1];

            for (int x = 0; x <= MaxX; x++) {
                for (int y = 0; y <= MaxY; y++) {
                    if (rawMap.Tile[x, y] == null) {
                        rawMap.Tile[x, y] = new DataManager.Maps.Tile();
                    }
                    tiles[x, y] = new Tile(rawMap.Tile[x, y]);
                }
            }
        }

        public int MaxX {
            get {
                return rawMap.MaxX;
            }
        }

        public int MaxY {
            get {
                return rawMap.MaxY;
            }
        }

        public Tile this[int x, int y] {
            get {
                return tiles[x, y];
            }
            set {
                tiles[x, y] = value;
                //rawMap.Tile[x, y] = value.RawTile;
            }
        }
    }
}

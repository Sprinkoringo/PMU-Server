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


namespace Server.Maps
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Data = DataManager.Maps;

    public class Tile
    {
        Data.Tile tile;

        public Data.Tile RawTile {
            get { return tile; }
        }

        #region Constructors

        public Tile(Data.Tile tile)
        {
            this.tile = tile;
        }

        #endregion Constructors

        #region Properties

        public int Anim
        {
            get { return tile.Anim; }
            set { tile.Anim = value; }
        }

        public int AnimSet
        {
            get { return tile.AnimSet; }
            set { tile.AnimSet = value; }
        }

        public int Data1
        {
            get { return tile.Data1; }
            set { tile.Data1 = value; }
        }

        public int Data2
        {
            get { return tile.Data2; }
            set { tile.Data2 = value; }
        }

        public int Data3
        {
            get { return tile.Data3; }
            set { tile.Data3 = value; }
        }

        public bool DoorOpen
        {
            get; set;
        }

        public TickCount DoorTimer
        {
            get; set;
        }

        public int F2Anim
        {
            get { return tile.F2Anim; }
            set { tile.F2Anim = value; }
        }

        public int F2AnimSet
        {
            get { return tile.F2AnimSet; }
            set { tile.F2AnimSet = value; }
        }

        public int FAnim
        {
            get { return tile.FAnim; }
            set { tile.FAnim = value; }
        }

        public int FAnimSet
        {
            get { return tile.FAnimSet; }
            set {
                tile.FAnimSet = value;
            }
        }

        public int Fringe
        {
            get { return tile.Fringe; }
            set { tile.Fringe = value; }
        }

        public int Fringe2
        {
            get { return tile.Fringe2; }
            set { tile.Fringe2 = value; }
        }

        public int Fringe2Set
        {
            get { return tile.Fringe2Set; }
            set { tile.Fringe2Set = value; }
        }

        public int FringeSet
        {
            get { return tile.FringeSet; }
            set { tile.FringeSet = value; }
        }

        public int Ground
        {
            get { return tile.Ground; }
            set { tile.Ground = value; }
        }

        public int GroundSet
        {
            get { return tile.GroundSet; }
            set { tile.GroundSet = value; }
        }

        public int GroundAnim
        {
            get { return tile.GroundAnim; }
            set { tile.GroundAnim = value; }
        }

        public int GroundAnimSet
        {
            get { return tile.GroundAnimSet; }
            set { tile.GroundAnimSet = value; }
        }
        public int RDungeonMapValue
        {
            get { return tile.RDungeonMapValue; }
            set { tile.RDungeonMapValue = value; }
        }

        public int M2Anim
        {
            get { return tile.M2Anim; }
            set { tile.M2Anim = value; }
        }

        public int M2AnimSet
        {
            get { return tile.M2AnimSet; }
            set { tile.M2AnimSet = value; }
        }

        public int Mask
        {
            get { return tile.Mask; }
            set { tile.Mask = value; }
        }

        public int Mask2
        {
            get { return tile.Mask2; }
            set { tile.Mask2 = value; }
        }

        public int Mask2Set
        {
            get { return tile.Mask2Set; }
            set { tile.Mask2Set = value; }
        }

        public int MaskSet
        {
            get { return tile.MaskSet; }
            set { tile.MaskSet = value; }
        }

        public string String1
        {
            get { return tile.String1; }
            set { tile.String1 = value; }
        }

        public string String2
        {
            get { return tile.String2; }
            set { tile.String2 = value; }
        }

        public string String3
        {
            get { return tile.String3; }
            set { tile.String3 = value; }
        }

        public Enums.TileType Type
        {
            get { return (Enums.TileType)tile.Type; }
            set { tile.Type = (int)value; }
        }

        #endregion Properties
    }
}
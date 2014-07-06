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

namespace Server.Maps
{
    public class MapNpcPreset
    {
        DataManager.Maps.MapNpcPreset rawNpc;

        public DataManager.Maps.MapNpcPreset RawNpcPreset {
            get { return rawNpc; }
        }

        public int SpawnX {
            get { return rawNpc.SpawnX; }
            set { rawNpc.SpawnX = value; }
        }

        public int SpawnY {
            get { return rawNpc.SpawnY; }
            set { rawNpc.SpawnY = value; }
        }


        public int NpcNum {
            get { return rawNpc.NpcNum; }
            set { rawNpc.NpcNum = value; }
        }

        public int MinLevel {
            get { return rawNpc.MinLevel; }
            set { rawNpc.MinLevel = value; }
        }
        public int MaxLevel {
            get { return rawNpc.MaxLevel; }
            set { rawNpc.MaxLevel = value; }
        }

        public int AppearanceRate {
            get { return rawNpc.AppearanceRate; }
            set { rawNpc.AppearanceRate = value; }
        }

        public Enums.StatusAilment StartStatus {
            get { return (Enums.StatusAilment)rawNpc.StartStatus; }
            set { rawNpc.StartStatus = (int)value; }
        }
        public int StartStatusCounter {
            get { return rawNpc.StartStatusCounter; }
            set { rawNpc.StartStatusCounter = value; }
        }
        public int StartStatusChance {
            get { return rawNpc.StartStatusChance; }
            set { rawNpc.StartStatusChance = value; }
        }

        public MapNpcPreset() {
            this.rawNpc = new DataManager.Maps.MapNpcPreset();

            NpcNum = 0;
            SpawnX = -1;
            SpawnY = -1;
            MinLevel = -1;
            MaxLevel = -1;
            AppearanceRate = 100;
        }

        public MapNpcPreset(DataManager.Maps.MapNpcPreset rawNpc) {
            this.rawNpc = rawNpc;
        }
    }
}

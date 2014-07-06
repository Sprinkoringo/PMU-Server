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

namespace DataManager.Players
{
    public class PlayerDataJobListItem
    {
        public int Accepted { get; set; }
        public int SendsRemaining { get; set; }



        public int MissionClientIndex { get; set; }
        public int TargetIndex { get; set; }
        public int RewardIndex { get; set; }

        public int MissionType { get; set; }

        /// <summary>
        /// Item number if Mission Type is Item Retrieval. If Escort, pokemon number.
        /// </summary>
        public int Data1 { get; set; }

        /// <summary>
        /// Item amount if Mission Type is Item Retrieval.
        /// </summary>
        public int Data2 { get; set; }

        public int DungeonIndex { get; set; }
        //map index or RDungeon index
        public int GoalMapIndex { get; set; }
        //floor for an RDungeon, 0 otherwise
        public bool RDungeon { get; set; }

        public int StartStoryScript { get; set; }
        public int WinStoryScript { get; set; }
        public int LoseStoryScript { get; set; }
    }
}

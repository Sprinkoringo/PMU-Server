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

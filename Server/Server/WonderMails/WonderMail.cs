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


#region Header

/*
 * Created by SharpDevelop.
 * User: Pikachu
 * Date: 15/10/2009
 * Time: 11:07 PM
 *
 */

#endregion Header

namespace Server.WonderMails
{
    using System;
    using System.Text;
    using DataManager.Players;

    /// <summary>
    /// Description of WonderMail.
    /// </summary>
    public class WonderMail
    {
        #region Fields

        PlayerDataJobListItem baseJobItem;

        #endregion Fields

        #region Constructors

        public WonderMail(PlayerDataJobListItem baseJobItem) {
            this.baseJobItem = baseJobItem;
        }

        #endregion Constructors

        #region Properties

        public PlayerDataJobListItem RawMission { get { return baseJobItem; } }

        public int MissionClientIndex {
            get { return baseJobItem.MissionClientIndex; }
            set { baseJobItem.MissionClientIndex = value; }
        }

        public int TargetIndex {
            get { return baseJobItem.TargetIndex; }
            set { baseJobItem.TargetIndex = value; }
        }

        public int RewardIndex {
            get { return baseJobItem.RewardIndex; }
            set { baseJobItem.RewardIndex = value; }
        }

        public Enums.MissionType MissionType {
            get { return (Enums.MissionType)baseJobItem.MissionType; }
            set { baseJobItem.MissionType = (int)value; }
        }

        /// <summary>
        /// Item number if Mission Type is Item Retrieval. If Escort, pokemon number.
        /// </summary>
        public int Data1 {
            get { return baseJobItem.Data1; }
            set { baseJobItem.Data1 = value; }
        }

        /// <summary>
        /// Item amount if Mission Type is Item Retrieval.
        /// </summary>
        public int Data2 {
            get { return baseJobItem.Data2; }
            set { baseJobItem.Data2 = value; }
        }

        public int DungeonIndex {
            get { return baseJobItem.DungeonIndex; }
            set { baseJobItem.DungeonIndex = value; }
        }

        //map number or RDungeon index
        public int GoalMapIndex {
            get { return baseJobItem.GoalMapIndex; }
            set { baseJobItem.GoalMapIndex = value; }
        }

        //floor for an RDungeon, 0 otherwise
        public bool RDungeon {
            get { return baseJobItem.RDungeon; }
            set { baseJobItem.RDungeon = value; }
        }

        public int StartStoryScript {
            get { return baseJobItem.StartStoryScript; }
            set { baseJobItem.StartStoryScript = value; }
        }

        public int WinStoryScript {
            get { return baseJobItem.WinStoryScript; }
            set { baseJobItem.WinStoryScript = value; }
        }

        public int LoseStoryScript {
            get { return baseJobItem.LoseStoryScript; }
            set { baseJobItem.LoseStoryScript = value; }
        }

        //extra, calculated info
        public int DungeonMapNum { get; set; }
        public int RDungeonFloor { get; set; }
        public Enums.JobDifficulty Difficulty { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string GoalName { get; set; }
        public int Mugshot { get; set; }

        #endregion Properties

        
    }
}
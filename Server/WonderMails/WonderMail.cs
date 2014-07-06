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

        public int DungeonMapNum { get; set; }
        public int RDungeonFloor { get; set; }
        public Enums.JobDifficulty Difficulty { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string GoalName { get; set; }

        #endregion Properties

        
    }
}
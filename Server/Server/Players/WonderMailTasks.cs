namespace Server.Players
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;

    using Server.WonderMails;
    using Server.Network;

    using DataManager.Players;
    using Server.Database;

    public class WonderMailTasks
    {
        #region Fields

        List<WonderMailJob> jobList;
        Player owner;

        #endregion Fields

        #region Constructors

        public WonderMailTasks(Player owner) {
            this.owner = owner;
            jobList = new List<WonderMailJob>();
        }

        #endregion Constructors

        #region Properties

        public List<WonderMailJob> JobList {
            get { return jobList; }
        }

        #endregion Properties

        #region Methods

        public void LoadJobList(List<PlayerDataJobListItem> databaseJobList) {
            for (int i = 0; i < databaseJobList.Count; i++) {
                WonderMailJob job = new WonderMailJob(databaseJobList[i]);
                if (Generator.AreIndicesLegal(job.Mission)) {
                    Generator.CalculateMailGoal(job.Mission);
                    Scripting.ScriptManager.InvokeSub("CreateMissionInfo", job.Mission);
                    jobList.Add(job);
                }
            }
        }

        public void LoadData() {
            jobList.Clear();
            LoadJobList(owner.PlayerData.JobList);
        }

        public void SaveJobList(DatabaseConnection dbConnection) {
            owner.PlayerData.JobList.Clear();
            for (int i = 0; i < jobList.Count; i++) {
                owner.PlayerData.JobList.Add(jobList[i].RawJob);
            }
            PlayerDataManager.SavePlayerJobList(dbConnection.Database, owner.PlayerData);
        }

        public bool AddJob(WonderMails.WonderMailJob job) {
            //if (HasJob(job) == false) {
            if (jobList.Count < Constants.MAX_JOB_LIST) {

                if (owner.Client.Player.GetDungeonCompletionCount(job.Mission.DungeonIndex) > 0) {
                    job.SendsRemaining = Constants.MAX_JOB_SENDS;
                    AddJobSimple(job);
                    Messenger.PlayerMsg(owner.Client, "You have accepted this job!", Text.BrightGreen);
                    return true;
                } else {
                    Messenger.PlayerMsg(owner.Client, "You haven't completed this dungeon yet.", Text.BrightRed);
                }
            } else {
                Messenger.PlayerMsg(owner.Client, "Your job list is full!", Text.BrightRed);
            }
            //} else {
            //    Messenger.PlayerMsg(owner.Client, "You already have accepted this job!", Text.BrightRed);
            //}
            return false;
        }

        public void AddJobSimple(WonderMails.WonderMailJob job) {
            job.Accepted = Enums.JobStatus.Obtained;
            JobList.Add(job);
        }

        public bool HasJob(WonderMails.WonderMailJob job) {
            if (job != null) {
                for (int i = 0; i < JobList.Count; i++) {
                    if (JobList[i].Mission.MissionClientIndex == job.Mission.MissionClientIndex &&
                        JobList[i].Mission.TargetIndex == job.Mission.TargetIndex &&
                        JobList[i].Mission.RewardIndex == job.Mission.RewardIndex &&
                        JobList[i].Mission.MissionType == job.Mission.MissionType &&
                        JobList[i].Mission.Data1 == job.Mission.Data1 &&
                        JobList[i].Mission.Data2 == job.Mission.Data2 &&
                        JobList[i].Mission.DungeonIndex == job.Mission.DungeonIndex &&
                        JobList[i].Mission.GoalMapIndex == job.Mission.GoalMapIndex &&
                        JobList[i].Mission.RDungeon == job.Mission.RDungeon &&
                        JobList[i].Mission.StartStoryScript == job.Mission.StartStoryScript &&
                        JobList[i].Mission.WinStoryScript == job.Mission.WinStoryScript &&
                        JobList[i].Mission.LoseStoryScript == job.Mission.LoseStoryScript) {
                        return true;
                    }
                }
            }
            return false;
        }

        public void RemoveJob(int index) {
            JobList.RemoveAt(index);
        }

        /*
        public bool HasCompletedMission(DatabaseConnection dbConnection, string wonderMail) {
            bool result = PlayerDataManager.IsWonderMailCompleted(dbConnection.Database, owner.CharID, wonderMail);
            return result;
        }

        public int CountCompletedMail(DatabaseConnection dbConnection) {
            return PlayerDataManager.CountPlayerCompletedMail(dbConnection.Database, owner.CharID);
        }

        public void AddCompletedMail(DatabaseConnection dbConnection, string code) {
            if (HasCompletedMission(dbConnection, code) == false) {
                PlayerDataManager.AddPlayerCompletedMail(dbConnection.Database, owner.CharID, code);
            }
        }
        */

        #endregion Methods
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using Server.Players;
using Server.Database;

namespace Server.WonderMails
{
    public class MissionBoard
    {
        Player owner;
        List<WonderMail> boardMissions;

        public List<WonderMail> BoardMissions {
            get { return boardMissions; }
        }

        public int LastGenTime {
            get { return owner.PlayerData.LastGenTime; } 
            set { owner.PlayerData.LastGenTime = value; }
        }

        public MissionBoard(Player owner) {
            this.owner = owner;
            boardMissions = new List<WonderMail>();
        }

        public void GenerateMission() {
            WonderMail mail = Generator.Generate(owner.SelectCompletedDungeons(), owner.Client);
            if (mail != null) {
                if (boardMissions.Count >= Constants.MAX_JOB_LIST) {
                    int excess = boardMissions.Count - Constants.MAX_JOB_LIST + 1;
                    for (int i = 0; i < excess; i++) {
                        boardMissions.RemoveAt(boardMissions.Count - 1);
                    }
                }
                boardMissions.Insert(0, mail);

                if (owner.Map != null && owner.Map.Tile[owner.X, owner.Y].Type == Enums.TileType.MissionBoard) {
                    Network.Messenger.SendAddedMission(owner.Client);
                }
            }
            LastGenTime = Core.GetTickCount().Tick;
        }

        //public void TakeMission(int slot) {
        //    owner.JobList.AddJob(new WonderMailJob(boardMissions[slot].RawMission));
        //    boardMissions.RemoveAt(slot);
        //}

        public void LoadMissionBoardData() {
            for (int i = 0; i < owner.PlayerData.MissionBoardMissions.Count; i++) {
                WonderMail mail = new WonderMail(owner.PlayerData.MissionBoardMissions[i]);
                if (Generator.AreIndicesLegal(mail)) {
                    Generator.CalculateMailGoal(mail);
                    Scripting.ScriptManager.InvokeSub("CreateMissionInfo", mail);
                    boardMissions.Add(mail);
                }
            }
        }

        public void SaveMissionBoardData() {
            owner.PlayerData.MissionBoardMissions = new List<DataManager.Players.PlayerDataJobListItem>();
            for (int i = 0; i < boardMissions.Count; i++) {
                owner.PlayerData.MissionBoardMissions.Add(boardMissions[i].RawMission);
            }
        }

        public void UpdateMissionBoard() {
            TimeSpan currentTime = new TimeSpan(0, 0, 0, 0, Core.GetTickCount().Tick);
            TimeSpan time = new TimeSpan(0, 0, 0, 0, LastGenTime);
            int hour = (int)time.TotalHours % 12;
            Enums.Time dayTime = Enums.Time.Day;
            TimeSpan snappedTime = new TimeSpan();
            if (hour < 4) {
                dayTime = Enums.Time.Day;
                snappedTime = new TimeSpan((int)time.TotalHours - hour, 0, 0);
            } else if (hour < 6) {
                dayTime = Enums.Time.Dusk;
                snappedTime = new TimeSpan((int)time.TotalHours - hour + 4, 0, 0);
            } else if (hour < 10) {
                dayTime = Enums.Time.Night;
                snappedTime = new TimeSpan((int)time.TotalHours - hour + 6, 0, 0);
            } else {
                dayTime = Enums.Time.Dawn;
                snappedTime = new TimeSpan((int)time.TotalHours - hour + 10, 0, 0);
            }

            int generateCount = 0;
            //continue generating jobs until the max is generated or until we exceed the current time
            for (int i = 0; i < Constants.MAX_JOB_LIST; i++) {
                if (dayTime == Enums.Time.Day) {
                    time += new TimeSpan(4, 0, 0);
                    dayTime = Enums.Time.Dusk;
                } else if (dayTime == Enums.Time.Dusk) {
                    time += new TimeSpan(2, 0, 0);
                    dayTime = Enums.Time.Night;
                } else if (dayTime == Enums.Time.Night) {
                    time += new TimeSpan(4, 0, 0);
                    dayTime = Enums.Time.Dawn;
                } else if (dayTime == Enums.Time.Dawn) {
                    time += new TimeSpan(2, 0, 0);
                    dayTime = Enums.Time.Dawn;
                }
                if (time <= currentTime) {
                    generateCount++;
                } else {
                    break;
                }
            }
            for (int i = 0; i < generateCount; i++) {
                GenerateMission();
            }

            if (Core.GetTickCount().Tick < LastGenTime) {
                LastGenTime = Core.GetTickCount().Tick;
            }
        }
    }
}

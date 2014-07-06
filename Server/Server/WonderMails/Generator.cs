namespace Server.WonderMails
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text;

    using Server.Maps;

    public class Generator
    {
        #region Methods

        //public static Point DetermineGoalPoint(WonderMail wonderMail) {
        //    Point val = new Point(0, 0);
        //    bool selected = false;
        //
        //    IMap map;
        //    if (MapManager.IsMapActive(MapManager.GenerateMapID(wonderMail.GoalMap))) {
        //        map = MapManager.RetrieveActiveMap(MapManager.GenerateMapID(wonderMail.GoalMap));
        //    } else {
        //        map = MapManager.LoadMap(wonderMail.GoalMap);
        //    }

        // We'll try 100 times to randomly select a tile
        //    for (int i = 0; i < 100; i++) {
        //        int x = Server.Math.Rand(0, map.MaxX + 1);
        //        int y = Server.Math.Rand(0, map.MaxY + 1);

        // Check if the tile is walk able
        //        if (map.Tile[x, y].Type == Enums.TileType.Walkable) {
        //            val.X = x;
        //            val.Y = y;
        //            selected = true;
        //            break;
        //        }
        //    }

        // Didn't select anything, so now we'll just try to find a free tile
        //    if (!selected) {
        //        for (int Y = 0; Y <= map.MaxY; Y++) {
        //            for (int X = 0; X <= map.MaxX; X++) {
        //                if (map.Tile[X, Y].Type == Enums.TileType.Walkable) {
        //                    val.X = X;
        //                    val.Y = Y;
        //                    selected = true;
        //                }
        //            }
        //        }
        //    }

        //    return val;
        //}

        public static bool IsGoalMap(WonderMail wonderMail, IMap map) {
            if (wonderMail.RDungeonFloor == -1 && map.MapType == Enums.MapType.Standard) {
                if (((Map)map).MapNum == wonderMail.DungeonMapNum) {
                    return true;
                }
            } else if (wonderMail.RDungeonFloor == -1 && map.MapType == Enums.MapType.Instanced) {
                if (((InstancedMap)map).MapBase == wonderMail.DungeonMapNum) {
                    return true;
                }
            } else if (wonderMail.RDungeonFloor > -1 && map.MapType == Enums.MapType.RDungeonMap) {
                if (((RDungeonMap)map).RDungeonIndex == wonderMail.DungeonMapNum && ((RDungeonMap)map).RDungeonFloor == wonderMail.RDungeonFloor) {
                    return true;
                }
            }
            return false;
        }

        public static string GetGoalName(WonderMail wonderMail) {
            //if (wonderMail.MissionType == Enums.MissionType.CompleteDungeon) {
            //    return Dungeons.DungeonManager.Dungeons[wonderMail.DungeonIndex].Name;
            //}
            if (wonderMail.RDungeonFloor == -1) {

                string mapName = null;
                using (Database.DatabaseConnection dbConnection = new Database.DatabaseConnection(Database.DatabaseID.Data)) {
                    mapName = DataManager.Maps.MapDataManager.GetMapName(dbConnection.Database, MapManager.GenerateMapID(wonderMail.DungeonMapNum));
                }
                if (!string.IsNullOrEmpty(mapName)) {
                    return mapName;
                } else {
                    return "???";
                }
            } else {
                return RDungeons.RDungeonFloorGen.GenerateName(wonderMail.DungeonMapNum, wonderMail.RDungeonFloor);
            }

        }

        public static WonderMails.WonderMail Generate(List<int> dungeons, Server.Network.Client client) {
            
            Enums.JobDifficulty playerMax = Enums.JobDifficulty.E;
            List<Enums.JobDifficulty> minDiffs = new List<Enums.JobDifficulty>();
            List<Enums.JobDifficulty> maxDiffs = new List<Enums.JobDifficulty>();
            for (int i = 0; i < dungeons.Count; i++) {
                Enums.JobDifficulty minDiff = Enums.JobDifficulty.NineStar;
                Enums.JobDifficulty maxDiff = Enums.JobDifficulty.E;
                GetDifficultyRange(dungeons[i], out minDiff, out maxDiff);
                if (maxDiff >= minDiff) {
                    minDiffs.Add(minDiff);
                    maxDiffs.Add(maxDiff);
                    if (maxDiff > playerMax) playerMax = maxDiff;
                } else {
                    dungeons.RemoveAt(i);
                    i--;
                }
            }

            Enums.JobDifficulty pickedDifficulty = (Enums.JobDifficulty)Server.Math.Rand(1, (int)playerMax + 1);

            int count = dungeons.Count;
            for (int i = 0; i < count; i++) {
                int rand = Server.Math.Rand(0, dungeons.Count);
                if (minDiffs[rand] <= pickedDifficulty && pickedDifficulty <= maxDiffs[rand]) {
                    WonderMails.WonderMail mail = Generate(dungeons[rand], pickedDifficulty, client);
                    if (mail != null) {
                        return mail;
                    } else {
                        dungeons.RemoveAt(rand);
                        minDiffs.RemoveAt(rand);
                        maxDiffs.RemoveAt(rand);
                    }
                } else {
                    dungeons.RemoveAt(rand);
                    minDiffs.RemoveAt(rand);
                    maxDiffs.RemoveAt(rand);
                }
            }

            return null;
        }

        public static WonderMails.WonderMail Generate(int dungeon, Enums.JobDifficulty difficulty, Server.Network.Client client) {
            try {
                
                DataManager.Players.PlayerDataJobListItem rawJob = new DataManager.Players.PlayerDataJobListItem();
                WonderMails.WonderMail mail = new WonderMails.WonderMail(rawJob);

                mail.DungeonIndex = dungeon;

                int goal;
                bool isRDungeon;
                SelectDungeonMap(difficulty, mail.DungeonIndex, out goal, out isRDungeon);

                if (goal == -1) {
                    return null;
                }

                mail.GoalMapIndex = goal;
                mail.RDungeon = isRDungeon;

                CalculateMailGoal(mail);

                Enums.MissionType missionType = 0;
                if (Server.Math.Rand(0, 2) == 0) {
                    
                } else {
                    missionType = (Enums.MissionType)Server.Math.Rand(1, 3);
                }
                
                mail.MissionType = missionType;

                if (WonderMailManager.Missions[(int)mail.Difficulty - 1].MissionClients.Count == 0) return null;
                mail.MissionClientIndex = Server.Math.Rand(0, WonderMailManager.Missions[(int)mail.Difficulty-1].MissionClients.Count);
                if (WonderMailManager.Missions[(int)mail.Difficulty - 1].Enemies.Count == 0) {
                    if (missionType == Enums.MissionType.Outlaw) {
                        return null;
                    } else {
                        mail.TargetIndex = 0;
                    }
                } else {
                    mail.TargetIndex = Server.Math.Rand(0, WonderMailManager.Missions[(int)mail.Difficulty - 1].Enemies.Count);
                }
                if (WonderMailManager.Missions[(int)mail.Difficulty - 1].Rewards.Count == 0) return null;
                mail.RewardIndex = Server.Math.Rand(0, WonderMailManager.Missions[(int)mail.Difficulty - 1].Rewards.Count);
                
                // Section 5
                switch ((Enums.MissionType)mail.MissionType) {
                    case Enums.MissionType.ItemRetrieval: {
                            int index = Server.Math.Rand(0, WonderMailManager.Missions[0].Rewards.Count);
                            if (Items.ItemManager.Items[WonderMailManager.Missions[0].Rewards[index].ItemNum].StackCap > 0) return null;
                            mail.Data1 = WonderMailManager.Missions[0].Rewards[index].ItemNum;
                            mail.Data2 = WonderMailManager.Missions[0].Rewards[index].Amount;
                            break;
                        }
                    case Enums.MissionType.Escort: {
                            int index = Server.Math.Rand(0, WonderMailManager.Missions[0].MissionClients.Count);
                            mail.Data1 = WonderMailManager.Missions[0].MissionClients[index].Species;
                            mail.Data2 = WonderMailManager.Missions[0].MissionClients[index].Form;
                            break;
                        }
                    default: {
                        mail.Data1 = -1;
                        mail.Data2 = -1;
                            break;
                        }
                }

                bool approved = Scripting.ScriptManager.InvokeFunction("IsMissionAcceptable", client, mail).ToBool();
                if (!approved) return null;
                Scripting.ScriptManager.InvokeSub("CreateMissionInfo", mail);
                

                mail.StartStoryScript = -1;
                mail.WinStoryScript = -1;
                mail.LoseStoryScript = -1;


                return mail;
            } catch {
                return null;
            }
        }

        //private static int GetDungeonFloorNumber(int dungeon, int mapIndex) {
        //    return GetDungeonMap(dungeon, mapIndex).FloorNum;
        //}

        public static Dungeons.StandardDungeonMap GetDungeonMap(int dungeon, int mapIndex) {
            return Dungeons.DungeonManager.Dungeons[dungeon].StandardMaps[mapIndex];//TODO: +RandomMaps
        }

        public static Enums.JobDifficulty GetDungeonMapDifficulty(int dungeon, int mapIndex) {
            return GetDungeonMap(dungeon, mapIndex).Difficulty;
        }

        public static bool AreIndicesLegal(WonderMail mail) {
            if (mail.DungeonIndex >= Dungeons.DungeonManager.Dungeons.Count) {
                return false;
            }
            Dungeons.IDungeonMap goalMap = null;
            if (mail.RDungeon) {
                if (mail.GoalMapIndex >= Dungeons.DungeonManager.Dungeons[mail.DungeonIndex].RandomMaps.Count) {
                    return false;
                } else {
                    goalMap = Dungeons.DungeonManager.Dungeons[mail.DungeonIndex].RandomMaps[mail.GoalMapIndex];
                }
            } else {
                if (mail.GoalMapIndex >= Dungeons.DungeonManager.Dungeons[mail.DungeonIndex].StandardMaps.Count) {
                    return false;
                } else {
                    goalMap = Dungeons.DungeonManager.Dungeons[mail.DungeonIndex].StandardMaps[mail.GoalMapIndex];
                }
            }

            if (mail.MissionClientIndex >= WonderMailManager.Missions[(int)goalMap.Difficulty - 1].MissionClients.Count) {
                return false;
            }
            if (mail.RewardIndex >= WonderMailManager.Missions[(int)goalMap.Difficulty - 1].Rewards.Count) {
                return false;
            }


            return true;
        }

        public static void CalculateMailGoal(WonderMail mail) {
            Dungeons.IDungeonMap goalMap = null;
            if (mail.RDungeon) {
                goalMap = Dungeons.DungeonManager.Dungeons[mail.DungeonIndex].RandomMaps[mail.GoalMapIndex];
            } else {
                goalMap = Dungeons.DungeonManager.Dungeons[mail.DungeonIndex].StandardMaps[mail.GoalMapIndex];
            }

            mail.Difficulty = goalMap.Difficulty;
            
            if (goalMap.DungeonMapType == Enums.MapType.Standard) {
                mail.DungeonMapNum = ((Dungeons.StandardDungeonMap)goalMap).MapNum;
                mail.RDungeonFloor = -1;
            } else if (goalMap.DungeonMapType == Enums.MapType.RDungeonMap) {
                mail.DungeonMapNum = ((Dungeons.RandomDungeonMap)goalMap).RDungeonIndex;
                mail.RDungeonFloor = ((Dungeons.RandomDungeonMap)goalMap).RDungeonFloor;
            }
            mail.GoalName = GetGoalName(mail);
        }


        private static void SelectDungeonMap(Enums.JobDifficulty difficulty, int dungeon, out int goal, out bool isRdungeon) {
            List<int> validStandardMaps = new List<int>();
            List<int> validRandomMaps = new List<int>();
            for (int i = 0; i < Dungeons.DungeonManager.Dungeons[dungeon].StandardMaps.Count; i++) {
                if (!Dungeons.DungeonManager.Dungeons[dungeon].StandardMaps[i].IsBadGoalMap) {
                    if (Dungeons.DungeonManager.Dungeons[dungeon].StandardMaps[i].Difficulty == difficulty) {
                        validStandardMaps.Add(i);
                    }
                }
            }
            for (int i = 0; i < Dungeons.DungeonManager.Dungeons[dungeon].RandomMaps.Count; i++) {
                if (!Dungeons.DungeonManager.Dungeons[dungeon].RandomMaps[i].IsBadGoalMap) {
                    if (Dungeons.DungeonManager.Dungeons[dungeon].RandomMaps[i].Difficulty == difficulty) {
                        validRandomMaps.Add(i);
                    }
                }
            }
            int rand = -1;
            if (validStandardMaps.Count + validRandomMaps.Count > 0) {
                rand = Server.Math.Rand(0, (validStandardMaps.Count + validRandomMaps.Count));
            }

            if (rand == -1) {
                goal = rand;
                isRdungeon = false;
            } else if (rand < validStandardMaps.Count) {
                goal = rand;
                isRdungeon = false;
            } else {
                goal = rand - validStandardMaps.Count;
                isRdungeon = true;
            }


        }

        private static void GetDifficultyRange(int dungeon, out Enums.JobDifficulty minDifficulty, out Enums.JobDifficulty maxDifficulty) {
            List<int> validStandardMaps = new List<int>();
            List<int> validRandomMaps = new List<int>();
            maxDifficulty = Enums.JobDifficulty.E;
            minDifficulty = Enums.JobDifficulty.NineStar;
            for (int i = 0; i < Dungeons.DungeonManager.Dungeons[dungeon].StandardMaps.Count; i++) {
                if (!Dungeons.DungeonManager.Dungeons[dungeon].StandardMaps[i].IsBadGoalMap) {
                    if (Dungeons.DungeonManager.Dungeons[dungeon].StandardMaps[i].Difficulty > maxDifficulty) {
                        maxDifficulty = Dungeons.DungeonManager.Dungeons[dungeon].StandardMaps[i].Difficulty;
                    } else if (Dungeons.DungeonManager.Dungeons[dungeon].StandardMaps[i].Difficulty < minDifficulty) {
                        minDifficulty = Dungeons.DungeonManager.Dungeons[dungeon].StandardMaps[i].Difficulty;
                    }
                }
            }
            for (int i = 0; i < Dungeons.DungeonManager.Dungeons[dungeon].RandomMaps.Count; i++) {
                if (!Dungeons.DungeonManager.Dungeons[dungeon].RandomMaps[i].IsBadGoalMap) {
                    if (Dungeons.DungeonManager.Dungeons[dungeon].RandomMaps[i].Difficulty > maxDifficulty) {
                        maxDifficulty = Dungeons.DungeonManager.Dungeons[dungeon].RandomMaps[i].Difficulty;
                    } else if (Dungeons.DungeonManager.Dungeons[dungeon].RandomMaps[i].Difficulty < minDifficulty) {
                        minDifficulty = Dungeons.DungeonManager.Dungeons[dungeon].RandomMaps[i].Difficulty;
                    }
                }
            }
            
        }


        #endregion Methods
    }
}
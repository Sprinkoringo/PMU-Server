namespace Server.Players
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using DataManager.Players;
    using Server.Maps;
    using Server.Network;
    using Server.Scripting;
    using PMU.Sockets;
    using Server.Players.Parties;
    using Server.Npcs;
    using Server.Items;
    using Server.WonderMails;
    using Server.Stories;
    using Server.Missions;
    using Server.RDungeons;
    using PMU.Core;
    using Server.Database;

    public partial class Player
    {
        #region Methods

        public bool CharacterExists(DatabaseConnection dbConnection, int slot) {
            return !PlayerDataManager.IsCharacterSlotEmpty(dbConnection.Database, accountName, slot);
        }

        internal void ChangePassword(DatabaseConnection dbConnection, string currentPassword, string newPassword) {
            if (!string.IsNullOrEmpty(accountName)) {
                PlayerDataManager.ChangePassword(dbConnection.Database, accountName, currentPassword, newPassword);
            }
        }

        public void CreateCharacter(DatabaseConnection dbConnection, string name, Enums.Sex sex, int slot, bool silent) {
            PlayerData characterData = new PlayerData(PlayerID.GeneratePlayerID());
            characterData.Name = name;

            characterData.Map = MapManager.GenerateMapID(Settings.StartMap);
            characterData.X = Settings.StartX;
            characterData.Y = Settings.StartY;

            characterData.MaxInv = 20;
            characterData.MaxBank = 100;

            RecruitData leaderData = new RecruitData();
            leaderData.Name = name;
            leaderData.Sex = (byte)sex;
            leaderData.Species = 0;
            leaderData.Form = 1;
            leaderData.Level = 1;
            leaderData.HP = -1;

            Scripting.ScriptManager.InvokeSub("CharacterCreated", accountName, slot, characterData, leaderData);

            PlayerDataManager.CreateNewCharacter(dbConnection.Database, accountName, slot, characterData);

            //PlayerDataManager.SavePlayerAvailableExpKitModules(client.Database, playerData);
            //PlayerDataManager.SavePlayerCharacteristics(client.Database, playerData);
            //PlayerDataManager.SavePlayerGuild(client.Database, playerData);
            //PlayerDataManager.SavePlayerItemGenerals(client.Database, playerData);
            //PlayerDataManager.SavePlayerJobList(client.Database, playerData);
            //PlayerDataManager.SavePlayerLocation(client.Database, playerData);
            //PlayerDataManager.SavePlayerMissionBoardGenerals(client.Database, playerData);
            //PlayerDataManager.SavePlayerMissionBoardMissions(client.Database, playerData);
            //PlayerDataManager.SavePlayerMissionGenerals(client.Database, playerData);
            //PlayerDataManager.SavePlayerStoryGenerals(client.Database, playerData);
            //PlayerDataManager.SavePlayerStoryHelperStateSettings(client.Database, playerData);

            //// Save inventory
            //ListPair<int, PlayerDataInventoryItem> updatedInventory = new ListPair<int, PlayerDataInventoryItem>();
            //for (int i = 1; i <= Inventory.Count; i++) {
            //    if (Inventory[i].Updated) {
            //        updatedInventory.Add(i, Inventory[i].BaseInventoryItem);
            //        Inventory[i].Updated = false;
            //    }
            //}
            //PlayerDataManager.SavePlayerInventoryUpdates(client.Database, CharID, updatedInventory);

            //// Save bank
            //ListPair<int, PlayerDataInventoryItem> updatedBank = new ListPair<int, PlayerDataInventoryItem>();
            //for (int i = 1; i <= Bank.Count; i++) {
            //    if (Bank[i].Updated) {
            //        updatedBank.Add(i, Bank[i].BaseInventoryItem);
            //        Bank[i].Updated = false;
            //    }
            //}
            //PlayerDataManager.SavePlayerBankUpdates(client.Database, CharID, updatedBank);

            //// Save trigger events
            //playerData.TriggerEvents.Clear();
            //for (int i = 0; i < triggerEvents.Count; i++) {
            //    PlayerDataTriggerEvent triggerEvent = new PlayerDataTriggerEvent();
            //    triggerEvents[i].Save(triggerEvent);
            //    playerData.TriggerEvents.Add(triggerEvent);
            //}
            //PlayerDataManager.SavePlayerTriggerEvents(client.Database, playerData);

            //// Save team
            //for (int i = 0; i < team.Length; i++) {
            //    playerData.TeamMembers[i].RecruitIndex = team[i].RecruitIndex;
            //    playerData.TeamMembers[i].UsingTempStats = team[i].InTempMode;
            //    if (team[i].Loaded) {
            //        team[i].Save();
            //    }
            //}
            //PlayerDataManager.SavePlayerTeam(client.Database, playerData);

            PlayerDataManager.SavePlayerRecruit(dbConnection.Database, characterData.CharID, 0, leaderData);

            if (silent == false) {
                Messenger.SendChars(dbConnection, client);
                Messenger.PlainMsg(client, "Character has been created!", Enums.PlainMsgType.Chars);
            }
        }

        // TODO: DeleteCharacter

        public Recruit GetActiveRecruit() {
            return team[ActiveSlot];
        }

        public Recruit GetRecruit(int teamSlot) {
            return team[teamSlot];
        }

        public int GetRecruitIndex(int teamSlot) {
            return playerData.TeamMembers[teamSlot].RecruitIndex;
        }

        public void SwapActiveRecruit(int teamSlot) {
            SwapActiveRecruit(teamSlot, false);
        }

        public void SwapActiveRecruit(int teamSlot, bool ignoreChecks) {
            if (CanSwapRecruit(teamSlot) || ignoreChecks) {
                if (team[teamSlot].Loaded && teamSlot != ActiveSlot) {
                    int oldSlot = ActiveSlot;
                    ScriptManager.InvokeSub("BeforeRecruitActiveCharSwap", client, oldSlot, teamSlot);
                    ActiveSlot = teamSlot;
                    PacketHitList hitList = null;
                    PacketHitList.MethodStart(ref hitList);
                    PacketBuilder.AppendActiveTeamNum(client, hitList);
                    PacketBuilder.AppendBelly(client, hitList);
                    PacketBuilder.AppendStats(client, hitList);
                    PacketBuilder.AppendPlayerData(client, hitList);
                    PacketBuilder.AppendPlayerMoves(client, hitList);
                    Party playerParty = PartyManager.FindPlayerParty(client);
                    if (playerParty != null) {
                        PacketBuilder.AppendPartyMemberData(client, hitList, playerParty.GetMemberSlot(CharID));
                    }
                    PacketHitList.MethodEnded(ref hitList);
                    ScriptManager.InvokeSub("RecruitActiveCharSwap", client, oldSlot, teamSlot);
                    PauseTimer = new Server.TickCount(Core.GetTickCount().Tick + 1000);

                }
            } else if (!Core.GetTickCount().Elapsed(PauseTimer, 0)) {
                Messenger.PlayerMsg(client, "You can't switch out right now.", Text.BrightRed);
            } else {
                Messenger.PlayerMsg(client, "You can't switch out!", Text.BrightRed);
            }
        }

        public void SwapLeader(int teamSlot) {
            if (client.Player.Team[teamSlot].Loaded) {
                if (client.Player.Team[teamSlot].RecruitIndex != -2) {
                    PlayerDataTeamMember oldTeamMember = playerData.TeamMembers[0];
                    PlayerDataTeamMember newTeamMember = playerData.TeamMembers[teamSlot];
                    Recruit oldRecruit = GetRecruit(0);
                    Recruit newRecruit = GetRecruit(teamSlot);
                    Scripting.ScriptManager.InvokeSub("BeforeRecruitLeaderChanged", client, oldRecruit, newRecruit);
                    playerData.TeamMembers[0] = newTeamMember;
                    team[0] = newRecruit;
                    playerData.TeamMembers[teamSlot] = oldTeamMember;
                    team[teamSlot] = oldRecruit;

                    PacketHitList hitList = null;
                    PacketHitList.MethodStart(ref hitList);
                    PacketBuilder.AppendActiveTeam(client, hitList);
                    PacketBuilder.AppendStats(client, hitList);
                    PacketBuilder.AppendPlayerMoves(client, hitList);
                    PacketBuilder.AppendPlayerData(client, hitList);
                    hitList.AddPacket(client, PacketBuilder.CreatePlayerAllRecruitsPacket(client));
                    PacketHitList.MethodEnded(ref hitList);

                    Scripting.ScriptManager.InvokeSub("RecruitLeaderChanged", client, teamSlot);
                } else {
                    Messenger.PlayerMsg(client, "You cannot make temporary Pokemon your team leader!", Text.BrightRed);
                }
            } else {
                Messenger.PlayerMsg(client, "Error: Pokémon not loaded.", Text.BrightRed);
            }
        }

        public bool CanSwapRecruit(int teamSlot) {
            if (Dead) {
                return false;
            }
            if (!CanSwapActiveRecruit) {
                return false;
            }
            //if (partyID != null) {
            //    Party party = PartyManager.FindParty(partyID);
            //    if (party != null) {
            //        switch (party.MemberCount) {
            //            case 2: {
            //                    if (teamSlot == 2 || teamSlot == 3) {
            //                        return false;
            //                    }
            //                }
            //                break;
            //            case 3:
            //            case 4: {
            //                    if (teamSlot != 0) {
            //                        return false;
            //                    }
            //                }
            //                break;
            //        }
            //    }
            //}
            return true;
        }

        public bool IsInTeam(int recruitIndex) {
            for (int i = 0; i < team.Length; i++) {
                if (team[i].Loaded) {
                    if (team[i].RecruitIndex == recruitIndex) {
                        return true;
                    }
                }
            }
            return false;
        }

        public int FindTeamSlot(int recruitIndex) {
            for (int i = 0; i < team.Length; i++) {
                if (team[i].Loaded) {
                    if (team[i].RecruitIndex == recruitIndex)
                        return i;
                }
            }
            return -1;
        }

        public int FindOpenTeamSlot() {
            for (int i = 0; i < team.Length; i++) {
                if (team[i].Loaded == false || team[i].RecruitIndex == -1) {
                    return i;
                }
            }
            return -1;
        }

        public bool RecruitExists(DatabaseConnection dbConnection, int recruitIndex) {
            return !PlayerDataManager.IsRecruitSlotEmpty(dbConnection.Database, CharID, recruitIndex);
        }

        //public int GetTeamSize() {
        //    int total = 0;
        //    for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
        //        if (team[i].Loaded) {
        //            if (team[i].RecruitIndex > -1 || team[i].RecruitBoost == -2) {
        //                total += team[i].Size;
        //            }
        //        }
        //    }
        //    return total;
        //}

        public void AddToTeam(int recruitIndex, int teamSlot) {
            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                if (((recruitIndex > -1 && IsInTeam(recruitIndex) == false && RecruitExists(dbConnection, recruitIndex)) || recruitIndex == -2) && (!(teamSlot < 0) || !(teamSlot > 3))) {
                    Recruit recruit = new Recruit(client);
                    recruit.LoadFromRecruitData(PlayerDataManager.LoadPlayerRecruit(dbConnection.Database, CharID, recruitIndex, false), recruitIndex);
                    team[teamSlot] = recruit;
                    for (int i = 0; i < team.Length; i++) {
                        if (team[i].RecruitIndex != -1) {
                            team[i].LoadActiveItemList();
                        }
                    }
                    if (recruitIndex != -2) {
                        ScriptManager.InvokeSub("RecruitAddedToTeam", client, teamSlot, recruitIndex);
                    }
                }
            }
        }

        public void RemoveFromTeam(int teamSlot) {
            if (teamSlot > 0 && teamSlot < 4 && team[teamSlot].Loaded) {
                ScriptManager.InvokeSub("BeforeRecruitRemovedFromTeam", client, teamSlot);
                //if (ActiveMission != null) {
                //    if (ActiveMission.WonderMail.MissionType == Enums.MissionType.Escort) {
                //        if (team[teamSlot].RecruitIndex == -2 && missionFailed) {
                //            Messenger.PlayerMsg(client, team[teamSlot].Name + " has went home. The mission has ended.", Text.Yellow);
                //            if (Settings.Scripting) {
                //                Scripting.ScriptManager.InvokeSub("OnMissionFailed", client, ActiveMission);
                //            }
                //            for (int i = 0; i < JobList.JobList.Count; i++) {
                //                JobList.JobList[i].Accepted = false;
                //            }
                //            activeMission = null;
                //            Messenger.SendMissionComplete(client);
                //            Messenger.SendJobList(client);
                //        }
                //    }
                //}
                if (teamSlot == ActiveSlot) {
                    SwapActiveRecruit(0, true);
                }
                if (team[teamSlot].RecruitIndex > -1) {
                    if (team[teamSlot].InTempMode) {
                        RestoreRecruitStats(teamSlot);
                    } else {
                        using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                            team[teamSlot].Save(dbConnection);
                        }
                    }
                }
                team[teamSlot] = new Recruit(client);
                ScriptManager.InvokeSub("RecruitRemovedFromTeam", client, teamSlot);
                Messenger.SendActiveTeam(client);
            }
        }

        internal void AddToRequested(MapNpc npc) {
            if (requestedRecruit == null) {
                requestedRecruit = new Recruit(client);
            }
            requestedRecruit = CreateRecruitFromNpc(npc);
            //requestedRecruit.Name = npc.Name;
            //requestedRecruit.Species = npc.Species;
            //requestedRecruit.Form = npc.Form;
            //requestedRecruit.Sex = npc.Sex;
            Pokedex.PokemonForm pokemon = Pokedex.Pokedex.GetPokemonForm(npc.Species, npc.Form);

            //if (npc.Level <= 0) {
            //    requestedRecruit.Level = 1;//NpcRec.RecruitLevel;
            //} else {
            //    requestedRecruit.Level = npc.Level;
            //}
            //requestedRecruit.Exp = 0;
            //requestedRecruit.IQ = 0;
            //requestedRecruit.NpcBase = npc.Num;
            //requestedRecruit.AtkBonus = npc.AtkBonus;
            //requestedRecruit.DefBonus = npc.DefBonus;
            //requestedRecruit.SpclAtkBonus = npc.SpclAtkBonus;
            //requestedRecruit.SpclDefBonus = npc.SpclDefBonus;
            //requestedRecruit.SpdBonus = npc.SpdBonus;
            //requestedRecruit.MaxHPBonus = npc.MaxHPBonus;
            //for (int i = 0; i < Constants.MAX_PLAYER_MOVES; i++) {
            //    requestedRecruit.Moves[i].MoveNum = npc.Moves[i].MoveNum;
            //    requestedRecruit.Moves[i].MaxPP = npc.Moves[i].MaxPP;
            //}


            Hunted = false;
            if (pokemon != null) {
                Messenger.AskQuestion(client, "RecruitPokemon", requestedRecruit.Name + " wants to join your team! Will you accept it?", requestedRecruit.Species);
            } else {
                Messenger.AskQuestion(client, "RecruitPokemon", requestedRecruit.Name + " wants to join your team! Will you accept it?", -1);
            }
        }

        public Recruit CreateRecruitFromNpc(MapNpc npc) {
            Recruit recruit = new Recruit(client);

            recruit.Name = npc.Name;
            recruit.Species = npc.Species;
            recruit.Form = npc.Form;
            recruit.Sex = npc.Sex;

            if (npc.Level <= 0) {
                recruit.Level = 1;//NpcRec.RecruitLevel;
            } else {
                recruit.Level = npc.Level;
            }
            recruit.Exp = 0;
            recruit.IQ = 0;
            recruit.NpcBase = npc.Num;
            recruit.AtkBonus = npc.AtkBonus;
            recruit.DefBonus = npc.DefBonus;
            recruit.SpclAtkBonus = npc.SpclAtkBonus;
            recruit.SpclDefBonus = npc.SpclDefBonus;
            recruit.SpdBonus = npc.SpdBonus;
            recruit.MaxHPBonus = npc.MaxHPBonus;
            for (int i = 0; i < Constants.MAX_PLAYER_MOVES; i++) {
                recruit.Moves[i].MoveNum = npc.Moves[i].MoveNum;
                recruit.Moves[i].MaxPP = npc.Moves[i].MaxPP;
            }

            return recruit;
        }

        private int FindOpenRecruitmentSlot(DatabaseConnection dbConnection) {
            int slot = PlayerDataManager.FindOpenRecruitmentSlot(dbConnection.Database, CharID);
            if (slot > Constants.MAX_RECRUITMENTS) {
                slot = -1;
            }
            return slot;
        }

        public int AddToRecruitmentBank(DatabaseConnection dbConnection, Recruit requestedRecruit) {
            int index = FindOpenRecruitmentSlot(dbConnection);
            if (index > -1 && index <= Constants.MAX_RECRUITMENTS) {

                PokemonCaught(requestedRecruit.Species);

                Recruit recruit = new Recruit(client);
                recruit.RecruitIndex = index;
                recruit.Level = requestedRecruit.Level;
                recruit.Species = requestedRecruit.Species;
                recruit.Form = requestedRecruit.Form;
                recruit.Sex = requestedRecruit.Sex;
                recruit.CalculateOriginalStats();
                recruit.Exp = requestedRecruit.Exp;
                recruit.IQ = requestedRecruit.IQ;
                //recruit.HP = recruit.MaxHP;
                recruit.Name = requestedRecruit.Name;
                recruit.NpcBase = requestedRecruit.NpcBase;
                recruit.AtkBonus = requestedRecruit.AtkBonus;
                recruit.DefBonus = requestedRecruit.DefBonus;
                recruit.SpclAtkBonus = requestedRecruit.SpclAtkBonus;
                recruit.SpclDefBonus = requestedRecruit.SpclDefBonus;
                recruit.SpdBonus = requestedRecruit.SpdBonus;
                recruit.MaxHPBonus = requestedRecruit.MaxHPBonus;

                for (int i = 0; i < Constants.MAX_PLAYER_MOVES; i++) {
                    recruit.Moves[i].MoveNum = requestedRecruit.Moves[i].MoveNum;
                    recruit.Moves[i].MaxPP = requestedRecruit.Moves[i].MaxPP;
                    recruit.Moves[i].CurrentPP = recruit.Moves[i].MaxPP;
                    recruit.Moves[i].Sealed = false;
                }
                recruit.Save(dbConnection);
                //Return the index of the recruited player
                return index;
            }
            //Return -1 so we know we're full
            return -1;
        }

        public void ReleaseRecruit(DatabaseConnection dbConnection, int recruitIndex) {
            if (recruitIndex > 0) {
                if (PlayerDataManager.IsRecruitSlotEmpty(dbConnection.Database, CharID, recruitIndex) == false) {
                    if (IsInTeam(recruitIndex)) {
                        Messenger.PlayerMsg(client, "This Pokémon is in your team! You can't release it!", Text.BrightRed);
                        return;
                    }
                    DeleteFromRecruitmentBank(dbConnection, recruitIndex, true);
                    Messenger.PlayerMsg(client, "Pokémon released!", Text.BrightRed);
                    ScriptManager.InvokeSub("RecruitReleased", client, recruitIndex);
                    Messenger.SendAllRecruits(client);
                    //Messenger.OpenAssembly(client);
                }
            }
        }

        public void DeleteFromRecruitmentBank(DatabaseConnection dbConnection, int recruitIndex, bool RemoveFromTeamSlot) {
            if (recruitIndex > 0) {
                if (RemoveFromTeamSlot) {
                    for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
                        if (Team[i].RecruitIndex == recruitIndex) {
                            RemoveFromTeam(i);
                        }
                    }
                }
                // Delete both the normal version, and the temp stat mode copy
                PlayerDataManager.DeletePlayerRecruit(dbConnection.Database, CharID, recruitIndex, false);
                PlayerDataManager.DeletePlayerRecruit(dbConnection.Database, CharID, recruitIndex, true);
            } else if (recruitIndex == 0) {
                Messenger.PlayerMsg(client, "You can't release your starter Pokémon!", Text.BrightRed);
            } else {
                Messenger.PlayerMsg(client, "Error: Unable to release selected Pokémon", Text.BrightRed);
            }
        }

        public void ChangeRecruitName(int teamSlot, string newName) {

            if (teamSlot > -1 && teamSlot < 4) {
                if (newName != "") {
                    if (Team[teamSlot].RecruitIndex > -1) {
                        Team[teamSlot].Name = newName;
                        Team[teamSlot].Nickname = true;
                        //Messenger.OpenAssembly(client);

                    }
                } else {
                    if (Team[teamSlot].RecruitIndex > -1) {
                        Team[teamSlot].Name = Pokedex.Pokedex.GetPokemon(Team[teamSlot].Species).Name;
                        Team[teamSlot].Nickname = false;
                        //Messenger.OpenAssembly(client);

                    }
                }
                Messenger.SendActiveTeam(client);
            }

            Messenger.SendAllRecruits(client);
        }

        public int GetLevelBonus() {
            int level = GetActiveRecruit().Level;
            if (level >= 1 && level < 20) {
                return 0;
            } else if (level > 19 && level < 30) {
                return 45;
            } else if (level > 29 && level < 40) {
                return 75;
            } else if (level > 39 && level < 50) {
                return 100;
            } else if (level > 49 && level < 60) {
                return 125;
            } else if (level > 59 && level < 70) {
                return 150;
            } else if (level > 69 && level < 80) {
                return 175;
            } else if (level > 79 && level < 90) {
                return 200;
            } else if (level > 89 && level < 101) {
                return 250;
            } else {
                return 0;
            }
        }

        public int GetRecruitBonus() {
            return GetActiveRecruit().RecruitBoost + GetLevelBonus();
        }

        public void AddToTeamTemp(int teamSlot, int species, int form, int level, int missionIndex) {

            Team[teamSlot].RecruitIndex = -2 - missionIndex;
            Team[teamSlot].Loaded = true;
            Team[teamSlot].Species = species;
            Team[teamSlot].Form = form;
            Team[teamSlot].Sex = Pokedex.Pokedex.GetPokemonForm(species, form).GenerateLegalSex();
            //Team[teamSlot].Form = Npcs.NpcManager.Npcs[npcNum].Form;
            client.Player.Team[teamSlot].Level = level;
            client.Player.Team[teamSlot].Name = Pokedex.Pokedex.GetPokemon(species).Name;
            client.Player.Team[teamSlot].Nickname = false;
            //client.Player.Team[teamSlot].Level = NpcManager.Npcs[npcNum].RecruitLevel;
            client.Player.Team[teamSlot].Exp = 0;
            client.Player.Team[teamSlot].IQ = 0;
            client.Player.Team[teamSlot].CalculateOriginalSprite();
            client.Player.Team[teamSlot].CalculateOriginalStats();
            client.Player.Team[teamSlot].HP = client.Player.Team[teamSlot].MaxHP;
            client.Player.Team[teamSlot].MaxBelly = 100;
            client.Player.Team[teamSlot].Belly = client.Player.Team[teamSlot].MaxBelly;
            client.Player.Team[teamSlot].GenerateMoveset();

        }

        #region Stats

        public int GetMaxHP() {
            return GetActiveRecruit().MaxHP;
        }

        public void SetHP(int HP) {
            SetHP(ActiveSlot, HP);
        }

        public void SetHP(int teamSlot, int HP) {
            Team[teamSlot].HP = HP;
        }

        #endregion

        public void ProcessHunger(PacketHitList hitlist) {
            if (BellyStepCounter >= 100) {
                if (GetActiveRecruit().Belly > 20) {
                    GetActiveRecruit().Belly -= (BellyStepCounter / 100);
                    if (GetActiveRecruit().Belly <= 20) {
                        hitlist.AddPacket(client, PacketBuilder.CreateBattleMsg("Getting hungry...", Text.Red));
                        hitlist.AddPacket(client, PacketBuilder.CreateSoundPacket("magic129.wav"));
                    }
                    PacketBuilder.AppendBelly(client, hitlist);

                } else if (GetActiveRecruit().Belly > 10) {
                    GetActiveRecruit().Belly -= (BellyStepCounter / 100);
                    if (GetActiveRecruit().Belly <= 10) {
                        hitlist.AddPacket(client, PacketBuilder.CreateBattleMsg("Getting dizzy from hunger...", Text.Red));
                        hitlist.AddPacket(client, PacketBuilder.CreateSoundPacket("magic129.wav"));
                    }
                    PacketBuilder.AppendBelly(client, hitlist);

                } else if (GetActiveRecruit().Belly > 0) {
                    GetActiveRecruit().Belly -= (BellyStepCounter / 100);
                    if (GetActiveRecruit().Belly <= 0) {
                        GetActiveRecruit().Belly = 0;
                        hitlist.AddPacket(client, PacketBuilder.CreateBattleMsg("Oh no!  Your belly is empty!", Text.Red));
                        hitlist.AddPacket(client, PacketBuilder.CreateSoundPacket("magic129.wav"));
                    }
                    PacketBuilder.AppendBelly(client, hitlist);
                }
                BellyStepCounter = BellyStepCounter % 100;
            }
        }

        #region Friends List

        public void SendFriendsList() {
            TcpPacket packet = new TcpPacket("friendslist");
            packet.AppendParameter(Friends.Count);
            for (int i = 0; i < Friends.Count; i++) {
                Client clientIsOn = ClientManager.FindClient(Friends[i]);
                int ison;
                if (clientIsOn != null) {
                    ison = 1;
                } else {
                    ison = 0;
                }
                packet.AppendParameters(Friends[i], ison.ToString());
            }
            packet.FinalizePacket();
            Messenger.SendDataTo(client, packet);
        }

        public void AppendFriendsList(PacketHitList hitlist) {
            TcpPacket packet = new TcpPacket("friendslist");
            packet.AppendParameter(Friends.Count);
            for (int i = 0; i < Friends.Count; i++) {
                Client clientIsOn = ClientManager.FindClient(Friends[i]);
                int ison;
                if (clientIsOn != null) {
                    ison = 1;
                } else {
                    ison = 0;
                }
                packet.AppendParameters(Friends[i], ison.ToString());
            }
            packet.FinalizePacket();
            hitlist.AddPacket(client, packet);
        }

        public void AddFriend(string friendName) {
            Client pIndex = ClientManager.FindClient(friendName.Trim());
            string pName;
            if (pIndex != null) {
                pName = pIndex.Player.Name.Trim();
                if (pName.Trim().ToLower() == Name.Trim().ToLower()) {
                    Messenger.PlayerMsg(client, "You can't add yourself!", Text.BrightRed);
                    return;
                }
            } else {
                Messenger.PlayerMsg(client, "Player is not online", Text.White);
                return;
            }

            if (Friends.Count < 20) {
                int result;
                using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                    result = Friends.AddFriend(dbConnection.Database, pName);
                }
                if (result == 0) {
                    SendFriendsList();
                    Messenger.PlayerMsg(client, pName + " has been added!", Text.Yellow);
                    ScriptManager.InvokeSub("FriendAdded", client, pName);
                } else if (result == 1) {
                    Messenger.PlayerMsg(client, "Player is already on your friends list!", Text.BrightRed);
                }
            } else {
                Messenger.PlayerMsg(client, "Friends list full", Text.BrightRed);
            }
        }

        public void RemoveFriend(string friendName) {
            int result;
            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                result = Friends.RemoveFriend(dbConnection.Database, friendName);
            }
            if (result == 0) {
                SendFriendsList();
                Messenger.PlayerMsg(client, friendName + " has been removed!", Text.Yellow);
                ScriptManager.InvokeSub("FriendRemoved", client, friendName);
            } else if (result == 1) {
                Messenger.PlayerMsg(client, "Unable to find " + friendName + " on your friends list", Text.BrightRed);
            }
        }

        public bool IsFriend(string friendName) {
            return Friends.HasFriend(friendName.Trim());
        }


        #endregion

        #region Missions

        //public void CancelActiveJob() {
        //    if (ActiveMission != null) {
        //        int activeSlot = -1;
        //        for (int i = 0; i < JobList.JobList.Count; i++) {
        //            if (JobList.JobList[i].Accepted) {
        //                activeSlot = i;
        //                break;
        //            }
        //        }
        //        if (activeSlot > -1) {
        //            JobList.JobList[activeSlot].Accepted = false;
        //        }
        //        if (ActiveMission.WonderMail.MissionType == Enums.MissionType.Escort) {
        //            RemoveFromTeam(FindTeamSlot(-2));
        //            Messenger.SendActiveTeam(client);
        //        }
        //        ActiveMission = null;
        //        Messenger.SendCancelJob(client);
        //        Messenger.SendJobList(client);
        //        Messenger.PlayerMsg(client, "You have cancelled your job!", Text.Yellow);
        //    }
        //}

        #endregion

        #region Maps

        public void InvalidateCachedMap() {
            cachedMap = null;
        }

        public IMap GetCurrentMap() {
            return Map;
        }

        public void DropItem(int invSlot, int amount) {
            DropItem(invSlot, amount, null);
        }

        public void DropItem(int invSlot, int amount, Client playerFor) {
            DropItem(invSlot, amount, playerFor, false);
        }
        public void DropItem(int invSlot, int amount, Client playerFor, bool ignorePrice) {
            if (Dead) {
                return;
            }

            bool TakeItem = false;

            // Check for subscript out of range
            if (invSlot < 0 || invSlot > MaxInv) {
                return;
            }

            IMap map = GetCurrentMap();
            if ((Inventory[invSlot].Num >= 0) && (Inventory[invSlot].Num < ItemManager.Items.MaxItems)) {
                //check to make sure player is not trying to drop an equipped sticky item
                if (Inventory[invSlot].Sticky && GetItemSlotHolder(invSlot) > -1) {
                    Messenger.PlayerMsg(client, "The " + ItemManager.Items[Inventory[invSlot].Num].Name + " is all sticky!  You can't get it off!", Text.BrightRed);
                    return;
                }

                int i = map.FindOpenItemSlot();

                if (i != -1) {

                    // Check to see if its any sort of ArmorSlot/WeaponSlot
                    //if (GetItemSlotHolder(InvNum) > -1)
                    //{
                    //    Team[GetItemSlotHolder(i)].HeldItemSlot = -1;
                    //    Messenger.SendWornEquipment(client);

                    //}
                    int itemNum = Inventory[invSlot].Num;

                    if (ItemManager.Items[itemNum].Bound) {
                        Messenger.PlayerMsg(client, "This item cannot be dropped.", Text.BrightRed);
                        return;
                    }
                    //map.ActiveItem[i].Num = itemNum;
                    //map.ActiveItem[i].X = X;
                    //map.ActiveItem[i].Y = Y;
                    //map.ActiveItem[i].PlayerFor = "";
                    //map.ActiveItem[i].TimeDropped = new TickCount(0);
                    if (map.Tile[X, Y].Type == Enums.TileType.DropShop && map.Tile[X, Y].String1 != CharID && !ignorePrice) {
                        if (String.IsNullOrEmpty(map.Tile[X, Y].String1)) {
                            if (ItemManager.Items[itemNum].Price <= 0) {
                                Messenger.PlayerMsg(client, "You can't sell this item.", Text.BrightRed);
                            } else if (ItemManager.Items[itemNum].StackCap > 0) {
                                Messenger.AskQuestion(client, "DropItem:"+invSlot+":" + amount, "Will you sell " + amount + " " + ItemManager.Items[itemNum].Name + " for " + (ItemManager.Items[itemNum].Price * amount) + " " + ItemManager.Items[1].Name + "?", -1);
                            } else {
                                Messenger.AskQuestion(client, "DropItem:" + invSlot + ":0", "Will you sell your " + ItemManager.Items[itemNum].Name + " for " + ItemManager.Items[itemNum].Price + " " + ItemManager.Items[1].Name + "?", -1);
                            }
                        } else {
                            Messenger.PlayerMsg(client, "You can't drop your items here!", Text.BrightRed);
                        }
                    } else {
                        bool sticky = Inventory[invSlot].Sticky;
                        string tag = Inventory[invSlot].Tag;

                        string Msg;

                        if (Inventory[invSlot].Sticky) {
                            Msg = "x" + ItemManager.Items[itemNum].Name.Trim();
                        } else {
                            Msg = ItemManager.Items[itemNum].Name.Trim();
                        }

                        if (ItemManager.Items[itemNum].Type == Enums.ItemType.Currency || ItemManager.Items[itemNum].StackCap > 0) {
                            // Check if its more then they have and if so drop it all
                            if (amount >= Inventory[invSlot].Amount) {
                                map.ActiveItem[i].Value = Inventory[invSlot].Amount;
                                Messenger.MapMsg(MapID, Name + " drops " + Inventory[invSlot].Amount + " " + Msg + ".", Text.Yellow);
                                TakeItem = true;
                            } else {
                                map.ActiveItem[i].Value = amount;
                                Messenger.MapMsg(MapID, Name + " drops " + amount + " " + Msg + ".", Text.Yellow);
                                Inventory[invSlot].Amount = Inventory[invSlot].Amount - amount;
                            }
                        } else {
                            // Its not a currency object so this is easy
                            map.ActiveItem[i].Value = 0;
                            Messenger.MapMsg(MapID, Name + " drops a " + Msg + ".", Text.Yellow);

                            TakeItem = true;
                        }

                        if (TakeItem == true) {

                            // Check to see if its held by anyone
                            if (GetItemSlotHolder(invSlot) > -1) { //remove from all equippers
                                RemoveItem(invSlot, false, false);
                                Messenger.SendWornEquipment(client);
                            }

                            Inventory[invSlot].Num = 0;
                            Inventory[invSlot].Amount = 0;
                            Inventory[invSlot].Sticky = false;
                            Inventory[invSlot].Tag = "";

                            //check for held-in-bag

                            //check to see if there aren't any functional copies left in bag
                            if (ItemManager.Items[itemNum].Type == Enums.ItemType.HeldInBag && HasItem(itemNum, true) == 0) {
                                for (int j = 0; j < Constants.MAX_ACTIVETEAM; j++) {
                                    if (Team[j] != null) {
                                        //the function auto-checks if it's in the activeItemlist to begin with
                                        Team[j].RemoveFromActiveItemList(itemNum);
                                    }

                                }
                            }
                        }

                        // Send inventory update
                        Messenger.SendInventoryUpdate(client, invSlot);

                        // Spawn the item before we set the num or we'll get a different free map item slot
                        map.SpawnItemSlot(i, itemNum, amount, sticky, false, tag, X, Y, playerFor);

                        Scripting.ScriptManager.InvokeSub("OnDropItem", GetActiveRecruit(), invSlot, map.ActiveItem[i]);
                    }
                } else {
                    Messenger.PlayerMsg(client, "Too many items already on the ground.", Text.BrightRed);
                }
                
            }
        }

        public void PickupItem() {
            PickupItem(false);
        }

        public void PickupItem(bool ignorePrice) {
            if (Dead) {
                return;
            }
            int i = 0;
            int n = 0;
            string Msg = null;

            IMap map = GetCurrentMap();

            for (i = 0; i < Constants.MAX_MAP_ITEMS; i++) {
                // See if theres even an item here
                if ((map.ActiveItem[i].Num >= 0) & (map.ActiveItem[i].Num < Items.ItemManager.Items.MaxItems)) {
                    // Check if item is at the same location as the player
                    if ((map.ActiveItem[i].X == X) & (map.ActiveItem[i].Y == Y)) {
                        if (map.Tile[X, Y].Type == Enums.TileType.DropShop && map.Tile[X, Y].String1 != CharID && !ignorePrice) {
                            if (map.ActiveItem[i].Num != 1) {
                                if (ItemManager.Items[map.ActiveItem[i].Num].StackCap > 0) {
                                    Messenger.AskQuestion(client, "BuyItem", "Will you purchase " + map.ActiveItem[i].Value + " " + ItemManager.Items[map.ActiveItem[i].Num].Name + " for " + map.Tile[X, Y].Data1 + " " + ItemManager.Items[1].Name + "?", -1);
                                } else {
                                    Messenger.AskQuestion(client, "BuyItem", "Will you purchase this " + ItemManager.Items[map.ActiveItem[i].Num].Name + " for " + map.Tile[X, Y].Data1 + " " + ItemManager.Items[1].Name + "?", -1);
                                }
                            } else {
                                Messenger.PlayerMsg(client, "This money was left here by another player when purchasing an item.  Only the house owner can claim it.", Text.BrightRed);
                            }
                        } else {
                            if (string.IsNullOrEmpty(map.ActiveItem[i].PlayerFor) || CharID == map.ActiveItem[i].PlayerFor || Core.GetTickCount().Elapsed(map.ActiveItem[i].TimeDropped, 20000)) {
                                // Find open slot
                                if (ItemManager.Items[map.ActiveItem[i].Num].Rarity <= (int)ExplorerRank + 1) {
                                    n = FindInvSlot(map.ActiveItem[i].Num, map.ActiveItem[i].Value);

                                    // Open slot available?
                                    if (n != -1) {
                                        // Set item in players inventory
                                        Inventory[n].Num = map.ActiveItem[i].Num;
                                        Inventory[n].Sticky = map.ActiveItem[i].Sticky;
                                        Inventory[n].Tag = map.ActiveItem[i].Tag;

                                        if (Inventory[n].Sticky) {
                                            Msg = "x" + ItemManager.Items[Inventory[n].Num].Name.Trim();
                                        } else {
                                            Msg = ItemManager.Items[Inventory[n].Num].Name.Trim();
                                        }
                                        if (ItemManager.Items[Inventory[n].Num].Type == Enums.ItemType.Currency || ItemManager.Items[Inventory[n].Num].StackCap > 0) {
                                            Inventory[n].Amount += map.ActiveItem[i].Value;
                                            if (Inventory[n].Amount > ItemManager.Items[Inventory[n].Num].StackCap) {
                                                Inventory[n].Amount = ItemManager.Items[Inventory[n].Num].StackCap;
                                            }
                                            Msg = "You picked up " + map.ActiveItem[i].Value + " " + Msg + ".";
                                        } else {
                                            Inventory[n].Amount = 0;

                                            Msg = "You picked up a " + Msg + ".";
                                        }
                                        //mInventory[n].Dur = map.ActiveItem[i].Dur;

                                        if (ItemManager.Items[Inventory[n].Num].Type == Enums.ItemType.HeldInBag) {
                                            for (int j = 0; j < Constants.MAX_ACTIVETEAM; j++) {
                                                if (Team[j] != null && !Inventory[n].Sticky && Team[j].MeetsReqs(Inventory[n].Num)) {
                                                    //the function auto-checks if it's in the activeItemlist to begin with
                                                    Team[j].AddToActiveItemList(Inventory[n].Num);
                                                }

                                            }
                                        }

                                        // Erase item from the map ~ done in spawnitemslot

                                        Messenger.SendInventoryUpdate(client, n);
                                        map.SpawnItemSlot(i, -1, 0, false, false, "", X, Y, null);
                                        Messenger.PlayerMsg(client, Msg, Text.Yellow);

                                        Scripting.ScriptManager.InvokeSub("OnPickupItem", GetActiveRecruit(), n, Inventory[n]);
                                        return;
                                    } else {
                                        Messenger.PlayerMsg(client, "Your inventory is full.", Text.BrightRed);
                                        return;
                                    }
                                } else {
                                    Messenger.PlayerMsg(client, "You do not have a high enough Explorer Rank to receive this item.", Text.BrightRed);
                                    return;
                                }
                            } else {
                                Messenger.PlayerMsg(client, "This item does not belong to you. Wait " + System.Math.Round((decimal)(map.ActiveItem[i].TimeDropped.Tick + 20000 - Core.GetTickCount().Tick) / 1000, 0) + " seconds before trying again.", Text.BrightRed);
                                return;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Stories

        public bool GetStoryState(int storyNum) {
            int index = StoryChapters.IndexOfKey(storyNum);
            if (index > -1) {
                return StoryChapters.ValueByIndex(index);
            } else {
                // Load on demand and cache
                bool state;
                using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                    state = PlayerDataManager.RetrievePlayerStoryChapterState(dbConnection.Database, CharID, storyNum);
                }
                StoryChapters.Add(storyNum, state);

                return state;
            }
        }

        public void SetStoryState(int storyNum, bool value) {
            // Cache this value
            int index = StoryChapters.IndexOfKey(storyNum);
            if (index > -1) {
                StoryChapters.Values[index] = value;
            } else {
                StoryChapters.Add(storyNum, value);
            }

            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                PlayerDataManager.SavePlayerStoryChapterState(dbConnection.Database, CharID, storyNum, value);
            }
        }

        #endregion

        #region System Info

        public string GetOSVersion() {
            return osVersion;
        }

        public string GetDotNetVersion() {
            return platformVersion.ToString();
        }

        public string GetClientEdition() {
            return clientEdition;
        }

        internal void SetSystemInfo(string osVersion, string dotNetVersion, string clientEdition) {
            this.osVersion = osVersion;
            this.platformVersion = dotNetVersion;
            this.clientEdition = clientEdition;
        }

        #endregion

        #region Dungeons

        public void SetDungeonCompletionCount(int dungeonIndex, int completionCount) {
            // On demand loading!
            if (playerData.DungeonCompletionCountsLoaded == false) {
                using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                    PlayerDataManager.LoadPlayerDungeonCompletionCounts(dbConnection.Database, playerData);
                }
            }

            int index = DungeonCompletionCounts.IndexOfKey(dungeonIndex);
            if (index > -1) {
                DungeonCompletionCounts.Values[index] = completionCount;
            } else {
                DungeonCompletionCounts.Add(dungeonIndex, completionCount);
            }

            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                PlayerDataManager.SavePlayerDungeonCompletionCount(dbConnection.Database, CharID, dungeonIndex, completionCount);
            }
        }

        public int GetDungeonCompletionCount(int dungeonIndex) {
            // On demand loading!
            if (playerData.DungeonCompletionCountsLoaded == false) {
                using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                    PlayerDataManager.LoadPlayerDungeonCompletionCounts(dbConnection.Database, playerData);
                }
            }
            int index = DungeonCompletionCounts.IndexOfKey(dungeonIndex);
            if (index > -1) {
                return DungeonCompletionCounts.ValueByIndex(index);
            } else {
                return -1;
            }
        }

        public void IncrementDungeonCompletionCount(int dungeonIndex, int incrementAmount) {
            SetDungeonCompletionCount(dungeonIndex, GetDungeonCompletionCount(dungeonIndex) + incrementAmount);

            //if (ActiveMission != null) {
            //    if (ActiveMission.WonderMail.MissionType == Enums.MissionType.CompleteDungeon) {
            //        if (ActiveMission.WonderMail.DungeonIndex == dungeonIndex) {
            //            HandleMissionComplete();
            //        }
            //    }
            //}
        }

        public void AddDungeonAttempt(int dungeonIndex) {
            if (GetDungeonCompletionCount(dungeonIndex) == -1) {
                SetDungeonCompletionCount(dungeonIndex, 0);
            }
        }

        public int GetTotalDungeonCompletionCount() {
            // On demand loading!
            if (playerData.DungeonCompletionCountsLoaded == false) {
                using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                    PlayerDataManager.LoadPlayerDungeonCompletionCounts(dbConnection.Database, playerData);
                }
            }
            int total = 0;

            for (int i = 0; i < DungeonCompletionCounts.Count; i++) {
                total += DungeonCompletionCounts.ValueByIndex(i);
            }

            return total;
        }

        #endregion

        #region Pokedex

        public void SetPokedexStatus(int pokemonID, Enums.PokedexStat status) {
            int index = RecruitList.IndexOfKey(pokemonID);
            if (index > -1) {
                RecruitList.Values[index] = (byte)status;
            } else {
                RecruitList.Add(pokemonID, (byte)status);
            }

            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                PlayerDataManager.SavePlayerRecruitListStatus(dbConnection.Database, CharID, pokemonID, (byte)status);
            }
        }

        public Enums.PokedexStat GetPokedexStatus(int pokemonID) {
            int index = RecruitList.IndexOfKey(pokemonID);
            if (index > -1) {
                return (Enums.PokedexStat)RecruitList.ValueByIndex(index);
            } else {
                byte status;
                using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                    status = PlayerDataManager.RetrievePlayerRecruitListStatus(dbConnection.Database, CharID, pokemonID);
                }
                RecruitList.Add(pokemonID, status);

                return (Enums.PokedexStat)status;
            }
        }

        public void PokemonSeen(int pokemonID) {
            if (pokemonID <= 0 || pokemonID > Constants.TOTAL_POKEMON) return;
            if (GetPokedexStatus(pokemonID) == Enums.PokedexStat.Unknown) {
                SetPokedexStatus(pokemonID, Enums.PokedexStat.Defeated);
            }
        }

        public void PokemonCaught(int pokemonID) {
            if (pokemonID <= 0 || pokemonID > Constants.TOTAL_POKEMON) return;
            if (GetPokedexStatus(pokemonID) != Enums.PokedexStat.Joined) {
                SetPokedexStatus(pokemonID, Enums.PokedexStat.Joined);
            }
        }

        #endregion Pokedex

        #region RDungeons

        public void WarpToRDungeon(int dungeonNum, int floor) {
            if (dungeonNum > -1 && floor > -1) {
                //dungeonIndex = dungeonNum;
                //dungeonFloor = floor;
                RDungeonMap map = null;

                bool generated = false;
                
                //party check
                if (IsInParty()) {
                    Party playerParty = PartyManager.FindParty(partyID);
                    foreach (Client i in playerParty.GetOnlineMemberClients()) {
                        if (i != client && i.Player.Map.MapType == Enums.MapType.RDungeonMap) {
                            if (((RDungeonMap)i.Player.Map).RDungeonIndex == dungeonNum && ((RDungeonMap)i.Player.Map).RDungeonFloor == floor) {

                                map = (RDungeonMap)i.Player.Map;

                                break;
                            }
                        }
                    }
                    if (map == null) {
                        map = RDungeonFloorGen.GenerateFloor(client, dungeonNum, floor, RDungeonManager.RDungeons[dungeonNum].Floors[floor].Options);
                        generated = true;
                    }
                }

                if (!String.IsNullOrEmpty(client.Player.GuildName)) {
                    List<RDungeonMap> maps = new List<RDungeonMap>();
                    foreach (Client i in ClientManager.GetClients()) {
                        if (i.Player.GuildName == client.Player.GuildName && i != client && i.Player.Map.MapType == Enums.MapType.RDungeonMap) {
                            bool mapAllowed = (bool)ScriptManager.InvokeFunction("IsDungeonFloorReady", client, (RDungeonMap)i.Player.Map);
                            if (mapAllowed && ((RDungeonMap)i.Player.Map).RDungeonIndex == dungeonNum && ((RDungeonMap)i.Player.Map).RDungeonFloor == floor) {
                                maps.Add((RDungeonMap)i.Player.Map);
                            }
                        }
                    }
                    if (maps.Count > 0) {
                        map = maps[Server.Math.Rand(0, maps.Count)];
                    }
                }


                if (map == null) {
                    map = RDungeonFloorGen.GenerateFloor(client, dungeonNum, floor, RDungeonManager.RDungeons[dungeonNum].Floors[floor].Options);
                    generated = true;
                }

                map.ProcessingPaused = true;
                Messenger.SendDataTo(client, TcpPacket.CreatePacket("floorchangedisplay", map.Name, "1500"));
                Messenger.PlayerWarp(client, map, map.StartX, map.StartY);
                if (generated) {
                    map.SpawnNpcs();
                    map.NpcSpawnWait = new Server.TickCount(Core.GetTickCount().Tick);
                }

                ScriptManager.InvokeSub("EnterRDungeon", client, dungeonNum, floor);
            }
        }

        #endregion RDungeons

        #region Wonder Mail

        public List<int> SelectCompletedDungeons() {
            if (playerData.DungeonCompletionCountsLoaded == false) {
                using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                    PlayerDataManager.LoadPlayerDungeonCompletionCounts(dbConnection.Database, playerData);
                }
            }

            List<int> dungeons = new List<int>();

            for (int i = 0; i < DungeonCompletionCounts.Count; i++) {
                if (DungeonCompletionCounts.ValueByIndex(i) > 0) {
                    dungeons.Add(DungeonCompletionCounts.KeyByIndex(i));
                }
            }

            return dungeons;
        }

        public void HandleMissionComplete(int missionIndex) {
            JobList.JobList[missionIndex].Accepted = Enums.JobStatus.Finished;
            MissionCompletions++;
            Messenger.SendJobAccept(client, missionIndex);
            Scripting.ScriptManager.InvokeSub("OnMissionComplete", client, missionIndex);

        }

        public bool HandleMissionRewardDump() {
            //if (ActiveMission.WonderMail.WinStoryScript > 0) {
            //    StoryManager.PlayStory(client, ActiveMission.WonderMail.EndStoryScript - 1);
            //} else {
            
            //}

            //using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
            //    JobList.AddCompletedMail(dbConnection, ActiveMission.WonderMail.Code);
            //}
            //JobList.RemoveJob(ActiveMission.WonderMail.Code);
            int completedJobs = 0;
            for (int i = JobList.JobList.Count - 1; i >= 0; i--) {
                if (JobList.JobList[i].Accepted == Enums.JobStatus.Finished) {
                    MissionPool missionPool = WonderMailManager.Missions[(int)JobList.JobList[i].Mission.Difficulty - 1];
                    GiveItem(missionPool.Rewards[JobList.JobList[i].Mission.RewardIndex].ItemNum, missionPool.Rewards[JobList.JobList[i].Mission.RewardIndex].Amount);
                    MissionExp += MissionManager.DetermineMissionExpReward(JobList.JobList[i].Mission.Difficulty);
                    completedJobs++;
                }
            }
            if (completedJobs == 0) return false;
            
            MissionManager.ExplorerRankUp(client);
            Scripting.ScriptManager.InvokeSub("ScriptedMissionWinDump", client);
            for (int i = JobList.JobList.Count - 1; i >= 0; i--) {
                if (JobList.JobList[i].Accepted == Enums.JobStatus.Finished || JobList.JobList[i].Accepted == Enums.JobStatus.Failed) {
                    JobList.JobList.RemoveAt(i);
                }
            }
            Messenger.SendJobList(client);
            Messenger.SendPlayerMissionExp(client);

            return true;
        }

        #endregion Wonder Mail

        #region ExpKit

        public void AddEnabledExpKitModules() {
            if (availableExpKitModules == null) {
                availableExpKitModules = new AvailableExpKitModuleCollection();
            }

            AvailableExpKitModules.Add(new AvailableExpKitModule(Enums.ExpKitModules.Chat, false));
            AvailableExpKitModules.Add(new AvailableExpKitModule(Enums.ExpKitModules.Counter, false));
            AvailableExpKitModules.Add(new AvailableExpKitModule(Enums.ExpKitModules.FriendsList, false));

            if (Ranks.IsAllowed(client, Enums.Rank.Mapper)) {
                AvailableExpKitModules.Add(new AvailableExpKitModule(Enums.ExpKitModules.MapReport, true));
            }
        }

        public void SetActiveExpKitModule(Enums.ExpKitModules module) {
            if (AvailableExpKitModules.Contains(module)) {
                Messenger.SendDataTo(client, TcpPacket.CreatePacket("setactivekitmodule", (int)(module)));
            }
        }

        public void AddExpKitModule(AvailableExpKitModule module) {
            if (AvailableExpKitModules.Contains(module.Type) == false) {
                AvailableExpKitModules.Add(module);
                Messenger.SendAvailableExpKitModules(client);
            }
        }

        public void RemoveExpKitModule(Enums.ExpKitModules module) {
            if (AvailableExpKitModules.Contains(module)) {
                AvailableExpKitModules.Remove(module);
                Messenger.SendAvailableExpKitModules(client);
            }
        }

        #endregion

        #region Level x system

        public void BeginTempStatMode(int level, bool keepMoves) {
            PacketHitList hitlist = null;
            PacketHitList.MethodStart(ref hitlist);

            // Save the current team
            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                for (int i = 0; i < Team.Length; i++) {
                    if (Team[i].Loaded) {
                        Team[i].Save(dbConnection);
                    }
                }
            }
            // Now that the team is saved, change the levels!
            for (int i = 0; i < Team.Length; i++) {
                if (Team[i].Loaded) {
                    Team[i].InTempMode = true;
                    Team[i].Level = level;
                    Team[i].Exp = 0;
                    Team[i].CalculateOriginalStats();
                    Team[i].HP = Team[i].MaxHP;
                    if (!keepMoves) {
                        for (int move = 0; move < Team[i].Moves.Length; move++) {
                            Team[i].Moves[move] = new RecruitMove();
                        }
                        Team[i].GenerateMoveset();
                    }
                }
            }

            PacketBuilder.AppendPlayerData(client, hitlist);
            PacketBuilder.AppendStats(client, hitlist);
            PacketBuilder.AppendEXP(client, hitlist);

            PacketBuilder.AppendPlayerMoves(client, hitlist);
            PacketBuilder.AppendActiveTeam(client, hitlist);

            PacketHitList.MethodEnded(ref hitlist);
        }

        public void EndTempStatMode() {
            PacketHitList hitlist = null;
            PacketHitList.MethodStart(ref hitlist);

            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                for (int i = 0; i < Team.Length; i++) {
                    if (Team[i].Loaded && Team[i].InTempMode) {
                        int recruitIndex = Team[i].RecruitIndex;
                        Team[i] = new Recruit(client);
                        RecruitData recruitData = PlayerDataManager.LoadPlayerRecruit(dbConnection.Database, CharID, recruitIndex, false);
                        Team[i].LoadFromRecruitData(recruitData, recruitIndex);
                        PlayerDataManager.DeletePlayerRecruit(dbConnection.Database, CharID, recruitIndex, true);
                    }
                }
            }

            PacketBuilder.AppendPlayerData(client, hitlist);
            PacketBuilder.AppendStats(client, hitlist);
            PacketBuilder.AppendEXP(client, hitlist);

            PacketBuilder.AppendPlayerMoves(client, hitlist);
            PacketBuilder.AppendActiveTeam(client, hitlist);
        }

        public void RestoreRecruitStats(int slot) {
            if (Team[slot].Loaded && Team[slot].InTempMode) {
                int recruitIndex = Team[slot].RecruitIndex;
                Team[slot] = new Recruit(client);
                using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                    RecruitData recruitData = PlayerDataManager.LoadPlayerRecruit(dbConnection.Database, CharID, recruitIndex, false);
                    Team[slot].LoadFromRecruitData(recruitData, recruitIndex);
                    PlayerDataManager.DeletePlayerRecruit(dbConnection.Database, CharID, recruitIndex, true);
                }
            }
        }

        #endregion

        #region Trading

        public void RequestTrade(Client playerToTradeWith) {
            if (playerToTradeWith.Player.CharID != this.CharID) {
                playerToTradeWith.Player.tradePartner = this.CharID;
                Messenger.AskQuestion(playerToTradeWith, "PlayerTradeRequest", "Would you like to trade with player " + this.Name + "?", this.GetActiveRecruit().Species);
            }
        }

        internal void EndTrade(bool manualEnd) {
            client.Player.readyToTrade = false;
            client.Player.setTradeItem = -1;
            client.Player.setTradeAmount = 0;
            if (!string.IsNullOrEmpty(client.Player.tradePartner)) {
                Client tradePartner = ClientManager.FindClientFromCharID(client.Player.tradePartner);
                if (tradePartner != null && tradePartner.Player.tradePartner == client.Player.CharID) {
                    tradePartner.Player.readyToTrade = false;
                    tradePartner.Player.setTradeItem = -1;
                    tradePartner.Player.setTradeAmount = 0;
                    tradePartner.Player.tradePartner = null;
                    Messenger.SendEndTrade(tradePartner);
                }
            }
            client.Player.tradePartner = null;
            if (manualEnd == false) {
                Messenger.SendEndTrade(client);
            }
        }

        public void EndTrade() {
            EndTrade(false);
        }

        #endregion

        #region Trigger Events

        public void AddTriggerEvent(Events.Player.TriggerEvents.ITriggerEvent triggerEvent) {
            if (triggerEvents.Contains(triggerEvent) == false) {
                triggerEvents.Add(triggerEvent);
            }
        }

        public void RemoveTriggerEvent(Events.Player.TriggerEvents.ITriggerEvent triggerEvent) {
            if (triggerEvents.Contains(triggerEvent)) {
                triggerEvents.Remove(triggerEvent);
            }
        }

        #endregion

        #region Mail

        #region Fields

        Mail.Mailbox mailbox;

        #endregion

        #region Properties

        public Mail.Mailbox Mailbox {
            get { return mailbox; }
        }

        #endregion

        #region Methods

        public void LoadMailbox() {
            //this.mailbox = Mail.Mailbox.LoadMailbox(GetCharFolder() + "Mail/Mailbox.xml");
        }

        #endregion

        #endregion

        public bool IsInParty() {
            return !string.IsNullOrEmpty(partyID);
        }

        public bool IsInSight(int x, int y) {
            return (x >= X - 10 && x <= X + 10 &&
                    y >= Y - 8 && y <= Y + 7);
        }

        public void BeginUninstancedWarp() {
            uninstancedWarp = true;
        }

        public void EndUninstancedWarp() {
            uninstancedWarp = false;
        }

        public void RecreateSeenCharacters() {
            IMap currentMap = GetCurrentMap();

            seenNpcs = new Dictionary<string, Dictionary<int, SeenCharacter>>();
            seenPlayers = new Dictionary<string, SeenCharacter>();
            Enums.MapID[] mapIDs = new Enums.MapID[9] { Enums.MapID.Active, Enums.MapID.Up, Enums.MapID.Down, Enums.MapID.Left, Enums.MapID.Right,
                                                        Enums.MapID.TopLeft, Enums.MapID.BottomLeft, Enums.MapID.TopRight, Enums.MapID.BottomRight };
            for (int i = 0; i < mapIDs.Length; i++) {
                IMap mapToTest = MapManager.RetrieveBorderingMap(currentMap, mapIDs[i], true);
                if (mapToTest != null) {
                    Dictionary<int, SeenCharacter> mapSeenNpcs = null;

                    lock (seenNpcs) {
                        if (!seenNpcs.TryGetValue(mapToTest.MapID, out mapSeenNpcs)) {
                            mapSeenNpcs = new Dictionary<int, SeenCharacter>();
                            seenNpcs.Add(mapToTest.MapID, mapSeenNpcs);
                        }
                    }

                    for (int n = 0; n < Constants.MAX_MAP_NPCS; n++) {
                        SeenCharacter seenCharacter = null;

                        lock (seenNpcs) {
                            if (!mapSeenNpcs.TryGetValue(n, out seenCharacter)) {
                                seenCharacter = new SeenCharacter();
                                mapSeenNpcs.Add(n, seenCharacter);
                            }
                        }

                        if (AI.MovementProcessor.WillCharacterSeeCharacter(currentMap, GetActiveRecruit(), mapToTest, mapIDs[i], mapToTest.ActiveNpc[n])) {
                            seenCharacter.InSight = true;
                        }
                        //new seencharacter
                        //if canbeseen, then isseen
                        //server cannot unsee
                    }

                    foreach (Client n in mapToTest.GetClients()) {
                        SeenCharacter seenCharacter = null;
                        lock (seenPlayers) {
                            if (!seenPlayers.TryGetValue(n.Player.CharID, out seenCharacter)) {
                                seenCharacter = new SeenCharacter();
                                seenPlayers.Add(n.Player.CharID, seenCharacter);
                            }
                        }
                        if (AI.MovementProcessor.WillCharacterSeeCharacter(currentMap, GetActiveRecruit(), mapToTest, mapIDs[i], n.Player.GetActiveRecruit())) {
                            seenCharacter.InSight = true;
                        }
                        //add new seencharacter based on player
                        //if canbeseen, then isseen
                    }
                }
            }
        }

        public void RemoveUnseenCharacters() {
            IMap map = GetCurrentMap();

            for (int n = 0; n < 9; n++) {
                IMap borderingMap = MapManager.RetrieveBorderingMap(map, (Enums.MapID)n, true);
                if (borderingMap != null) {
                    for (int i = 0; i < Constants.MAX_MAP_NPCS; i++) {
                        if (!AI.MovementProcessor.WillCharacterSeeCharacter(map, GetActiveRecruit(), borderingMap, (Enums.MapID)n, borderingMap.ActiveNpc[i])) {
                            lock (seenNpcs) {
                                Dictionary<int, SeenCharacter> mapSeenNpcs = null;
                                if (seenNpcs.TryGetValue(borderingMap.MapID, out mapSeenNpcs)) {
                                    SeenCharacter seenCharacter = null;
                                    if (mapSeenNpcs.TryGetValue(i, out seenCharacter)) {
                                        seenCharacter.InSight = false;
                                    }
                                }
                            }
                        }
                    }

                    foreach (Client i in borderingMap.GetClients()) {
                        if (!AI.MovementProcessor.WillCharacterSeeCharacter(map, GetActiveRecruit(), borderingMap, (Enums.MapID)n, i.Player.GetActiveRecruit())) {
                            lock (seenPlayers) {
                                SeenCharacter seenCharacter = null;
                                if (seenPlayers.TryGetValue(i.Player.CharID, out seenCharacter)) {
                                    seenCharacter.InSight = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void AddSeenCharacters(PacketHitList hitlist) {
            IMap map = GetCurrentMap();
            Recruit activeRecruit = GetActiveRecruit();
            for (int i = 0; i < 9; i++) {
                IMap borderingMap = MapManager.RetrieveBorderingMap(map, (Enums.MapID)i, true);
                if (borderingMap != null) {
                    for (int n = 0; n < Constants.MAX_MAP_NPCS; n++) {
                        if (AI.MovementProcessor.WillCharacterSeeCharacter(map, activeRecruit, borderingMap, (Enums.MapID)i, borderingMap.ActiveNpc[n])) {
                            //if (!IsSeenCharacterSeen(borderingMap.ActiveNpc[n])) {
                            AddActivationIfCharacterNotSeen(hitlist, borderingMap.ActiveNpc[n]);
                            //}
                        }
                    }

                    foreach (Client n in borderingMap.GetClients()) {
                        if (AI.MovementProcessor.WillCharacterSeeCharacter(map, activeRecruit, borderingMap, (Enums.MapID)i, n.Player.GetActiveRecruit())) {
                            //if (!IsSeenCharacterSeen(n.Player.GetActiveRecruit())) {
                            AddActivationIfCharacterNotSeen(hitlist, n.Player.GetActiveRecruit());
                            //}
                        }
                    }
                }
            }


        }

        public void ProcessCharacterVisibilities(PacketHitList hitlist) {
            IMap map = GetCurrentMap();
            Recruit activeRecruit = GetActiveRecruit();
            for (int i = 0; i < 9; i++) {
                IMap borderingMap = MapManager.RetrieveBorderingMap(map, (Enums.MapID)i, true);
                if (borderingMap != null) {
                    foreach (MapNpc activeNpc in borderingMap.ActiveNpc) {
                        if (AI.MovementProcessor.WillCharacterSeeCharacter(map, activeRecruit, borderingMap, (Enums.MapID)i, activeNpc)) {
                            //if (!IsSeenCharacterSeen(activeNpc)) {
                            AddActivationIfCharacterNotSeen(hitlist, activeNpc);
                            //}
                        } else {
                            // Character isn't in sight, remove from the list!
                            lock (seenNpcs) {
                                Dictionary<int, SeenCharacter> seenNpcsDictionary = null;
                                if (seenNpcs.TryGetValue(borderingMap.MapID, out seenNpcsDictionary)) {
                                    SeenCharacter seenCharacter = null;
                                    if (seenNpcsDictionary.TryGetValue(activeNpc.MapSlot, out seenCharacter)) {
                                        seenCharacter.InSight = false;
                                    }
                                }
                            }
                        }
                    }

                    foreach (Client n in borderingMap.GetClients()) {
                        if (AI.MovementProcessor.WillCharacterSeeCharacter(map, activeRecruit, borderingMap, (Enums.MapID)i, n.Player.GetActiveRecruit())) {
                            //if (!IsSeenCharacterSeen(n.Player.GetActiveRecruit())) {
                            AddActivationIfCharacterNotSeen(hitlist, n.Player.GetActiveRecruit());
                            //}
                        } else {
                            // Character isn't in sight, remove from the list!
                            lock (seenPlayers) {
                                SeenCharacter seenCharacter = null;
                                if (seenPlayers.TryGetValue(n.Player.CharID, out seenCharacter)) {
                                    seenCharacter.InSight = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void RefreshSeenCharacters(PacketHitList hitlist) {
            ProcessCharacterVisibilities(hitlist);
        }

        public void SeeNewCharacter(Combat.ICharacter target) {
            IMap currentMap = GetCurrentMap();
            IMap targetMap = MapManager.RetrieveMap(target.MapID);
            SeenCharacter seenCharacter = null;
            if (target.CharacterType == Enums.CharacterType.Recruit) {
                lock (seenPlayers) {
                    Recruit recruitTarget = (Recruit)target;
                    if (!seenPlayers.TryGetValue(recruitTarget.Owner.Player.CharID, out seenCharacter)) {
                        seenCharacter = new SeenCharacter();
                        seenPlayers.Add(recruitTarget.Owner.Player.CharID, seenCharacter);
                    }
                }
            } else if (target.CharacterType == Enums.CharacterType.MapNpc) {
                MapNpc npcTarget = (MapNpc)target;
                lock (seenNpcs) {
                    Dictionary<int, SeenCharacter> mapSeenNpcs = null;
                    if (!seenNpcs.TryGetValue(npcTarget.MapID, out mapSeenNpcs)) {
                        mapSeenNpcs = new Dictionary<int, SeenCharacter>();
                        seenNpcs.Add(npcTarget.MapID, mapSeenNpcs);
                    }
                    if (!mapSeenNpcs.TryGetValue(npcTarget.MapSlot, out seenCharacter)) {
                        seenCharacter = new SeenCharacter();
                        mapSeenNpcs.Add(npcTarget.MapSlot, seenCharacter);
                    }
                }
            }
            seenCharacter.InSight = AI.MovementProcessor.WillCharacterSeeCharacter(Map, GetActiveRecruit(), targetMap, SeamlessWorldHelper.GetBorderingMapID(Map, targetMap), target);
            seenCharacter.LocationOutdated = false;
            seenCharacter.ConditionOutdated = false;
        }

        public void UnseeLeftPlayer(Player player) {
            lock (seenPlayers) {
                if (seenPlayers.ContainsKey(player.CharID)) {
                    seenPlayers.Remove(player.CharID);
                }
            }
        }

        public void AddActivationIfCharacterNotSeen(PacketHitList hitlist, Combat.ICharacter target) {
            if (target == GetActiveRecruit()) return;
            SeenCharacter seencharacter;
            if (target.CharacterType == Enums.CharacterType.Recruit) {
                bool shouldActivate = false;
                lock (seenPlayers) {
                    if (!seenPlayers.TryGetValue(((Recruit)target).Owner.Player.CharID, out seencharacter)) {
                        seencharacter = new SeenCharacter();
                        seenPlayers.Add(((Recruit)target).Owner.Player.CharID, seencharacter);
                        shouldActivate = true;
                    } else {
                        if (!seencharacter.InSight) {
                            shouldActivate = true;
                        }
                    }
                }

                if (shouldActivate) {
                    if (seencharacter.LocationOutdated) {
                        //add location data
                        hitlist.AddPacket(client, PacketBuilder.CreatePlayerXY(((Recruit)target).Owner));
                        hitlist.AddPacket(client, PacketBuilder.CreatePlayerDir(((Recruit)target).Owner));
                        seencharacter.LocationOutdated = false;
                    }
                    if (seencharacter.ConditionOutdated) {
                        //add condition data
                        hitlist.AddPacket(client, PacketBuilder.CreatePlayerStatusAilment(((Recruit)target).Owner));
                        //hitlist.AddPacket(client, PacketBuilder.CreatePlayerConfusion(((Recruit)target).Owner));
                        hitlist.AddPacket(client, PacketBuilder.CreatePlayerVolatileStatus(((Recruit)target).Owner));
                        seencharacter.ConditionOutdated = false;
                    }
                    //add activation param
                    hitlist.AddPacket(client, PacketBuilder.CreatePlayerActivation(((Recruit)target).Owner, true));
                    //set isseen to true
                    seencharacter.InSight = true;
                }
            } else if (target.CharacterType == Enums.CharacterType.MapNpc) {
                MapNpc npcTarget = (MapNpc)target;
                bool shouldActivate = false;

                lock (seenNpcs) {
                    Dictionary<int, SeenCharacter> mapSeenNpcs = null;
                    if (!seenNpcs.TryGetValue(npcTarget.MapID, out mapSeenNpcs)) {
                        mapSeenNpcs = new Dictionary<int, SeenCharacter>();
                        seenNpcs.Add(npcTarget.MapID, mapSeenNpcs);
                        shouldActivate = true;
                    }
                    if (!mapSeenNpcs.TryGetValue(npcTarget.MapSlot, out seencharacter)) {
                        seencharacter = new SeenCharacter();
                        mapSeenNpcs.Add(npcTarget.MapSlot, seencharacter);
                        shouldActivate = true;
                    } else {
                        if (!seencharacter.InSight) {
                            shouldActivate = true;
                        }
                    }
                }

                if (shouldActivate) {
                    if (seencharacter.LocationOutdated) {
                        //add location data
                        hitlist.AddPacket(client, PacketBuilder.CreateNpcXY(npcTarget));
                        hitlist.AddPacket(client, PacketBuilder.CreateNpcDir(npcTarget));
                        seencharacter.LocationOutdated = false;
                    }
                    if (seencharacter.ConditionOutdated) {
                        //add condition data
                        hitlist.AddPacket(client, PacketBuilder.CreateNpcHP(npcTarget));
                        hitlist.AddPacket(client, PacketBuilder.CreateNpcStatusAilment(npcTarget));
                        //hitlist.AddPacket(client, PacketBuilder.CreateNpcConfusion(npcTarget));
                        hitlist.AddPacket(client, PacketBuilder.CreateNpcVolatileStatus(npcTarget));
                        seencharacter.ConditionOutdated = false;
                    }
                    //add activation param
                    hitlist.AddPacket(client, PacketBuilder.CreateNpcActivation(npcTarget, true));
                    //set isseen to true
                    seencharacter.InSight = true;
                }
            }
        }

        //public void AddUpdate(PacketHitList hitlist, Combat.ICharacter target, Enums.OutdateType updateType) {
        //    if (AI.MovementProcessor.WillCharacterSeeCharacter(Map, GetActiveRecruit(), target)) {

        //    } else {

        //    }
        //}

        public bool IsSeenCharacterSeen(Combat.ICharacter target) {
            if (target.CharacterType == Enums.CharacterType.Recruit) {
                lock (seenPlayers) {

                    SeenCharacter seenCharacter;
                    if (seenPlayers.TryGetValue(((Recruit)target).Owner.Player.CharID, out seenCharacter)) {
                        if (!seenCharacter.InSight) {
                            return false;
                        }
                    } else {
                        return false;
                    }
                }
            } else if (target.CharacterType == Enums.CharacterType.MapNpc) {
                MapNpc npcTarget = target as MapNpc;

                lock (seenNpcs) {
                    Dictionary<int, SeenCharacter> mapNpcCharacterList;
                    if (seenNpcs.TryGetValue(npcTarget.MapID, out mapNpcCharacterList)) {
                        SeenCharacter seenCharacter;
                        if (mapNpcCharacterList.TryGetValue(npcTarget.MapSlot, out seenCharacter)) {
                            if (!seenCharacter.InSight) {
                                return false;
                            }
                        } else {
                            return false;
                        }
                    } else {
                        return false;
                    }
                }
            }

            return true;
        }

        public void OutdateCharacter(Combat.ICharacter target, Enums.OutdateType outdateType) {
            SeenCharacter seencharacter = null;
            if (target.CharacterType == Enums.CharacterType.Recruit) {
                string charID = ((Recruit)target).Owner.Player.CharID;
                lock (seenPlayers) {
                    if (!seenPlayers.TryGetValue(charID, out seencharacter)) {
                        seencharacter = new SeenCharacter();
                        seenPlayers.Add(charID, seencharacter);
                    }
                }
            } else if (target.CharacterType == Enums.CharacterType.MapNpc) {
                //if (Map.ActiveNpc[((MapNpc)target).MapSlot] != target) {
                //    return false;
                //}
                MapNpc npcTarget = (MapNpc)target;

                lock (seenNpcs) {
                    Dictionary<int, SeenCharacter> mapNpcSeenCharacterList;
                    if (!seenNpcs.TryGetValue(npcTarget.MapID, out mapNpcSeenCharacterList)) {
                        mapNpcSeenCharacterList = new Dictionary<int, SeenCharacter>();
                        seenNpcs.Add(npcTarget.MapID, mapNpcSeenCharacterList);
                    }

                    if (!mapNpcSeenCharacterList.TryGetValue(npcTarget.MapSlot, out seencharacter)) {
                        seencharacter = new SeenCharacter();
                        mapNpcSeenCharacterList.Add(npcTarget.MapSlot, seencharacter);
                    }
                }
            }

            if (outdateType == Enums.OutdateType.Location) {
                seencharacter.LocationOutdated = true;
            } else if (outdateType == Enums.OutdateType.Condition) {
                seencharacter.ConditionOutdated = true;
            }
        }
        //when outdating something, be sure it exists

        #endregion Methods
    }
}
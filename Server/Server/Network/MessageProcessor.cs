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
using Server.Players;
using Server.Items;
using Server.Maps;
using Server.AI;
using Server.Moves;
using Server.RDungeons;
using Server.Npcs;

using Server.Emoticons;
using Server.Stories;
using Server.Shops;
using Server.Missions;
using Server.Evolutions;
using Server.Dungeons;
using Server.Combat;

using PMU.Core;
using PMU.Sockets;
using Server.Tournaments;
using Server.Database;

namespace Server.Network
{
    internal class MessageProcessor
    {
        private static bool CanSayMsg(Client client, string msg) {
            if (client.Player.Muted)
                return false;
            if (String.IsNullOrEmpty(msg.Trim())) {
                return false;
            }
            return true;
        }

        internal static void ProcessData(Client client, string data) {
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            long dataSplittingTime;
            watch.Start();
            string[] parse = data.Split(TcpPacket.SEP_CHAR);
            watch.Stop();
            dataSplittingTime = watch.ElapsedTicks;
            watch.Reset();

            Server.Debug.ThreadCPUWatcher cpuWatch = new Debug.ThreadCPUWatcher();
            cpuWatch.Start();
            watch.Start();
            ProcessData(client, parse);
            watch.Stop();
            cpuWatch.StopWatcher(watch.ElapsedMilliseconds);
            Statistics.PacketStatistics.AddStatistic(parse[0], watch.ElapsedTicks, dataSplittingTime, cpuWatch.CPUusage);
        }

        internal static void ProcessData(Client client, string[] parse) {
            //string[] parse = data.Split(TcpPacket.SEP_CHAR);
            if (client.Player == null) {
                if (parse[0].ToLower() == "requestnews") {
                    Messenger.SendNews(client);
                    client.InitializeClientSystem();
                    return;
                }
            }
            if (client.Player == null) {
                Messenger.PlainMsg(client, "Unable to initialize connection. Please retry.", Enums.PlainMsgType.MainMenu);
                return;
            }
            if (!client.Player.LoggedIn) {
                #region Not Logged In
                switch (parse[0].ToLower()) {
                    case "clienterror": {
                            string error = parse[1];
                            error += "\r\n\r\n------- ERROR REPORT END -------\r\n\r\n";
                            Logging.Logger.AppendToLog("/Client Error Logs/" + DateTime.Now.ToShortDateString().Replace("/", "-") + "/log.txt", error);
                        }
                        break;
                    case "requestnews":
                        Messenger.SendNews(client);
                        client.InitializeClientSystem();
                        break;
                    #region Logging In
                    case "login": {
                            if (Globals.ServerClosed) {
                                Messenger.PlainMsg(client, "Server is closed at the moment!", Enums.PlainMsgType.MainMenu);
                                return;
                            }
                            if (ClientManager.CanLogin(parse[1])) {
                                using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                                    // Check if the account exists
                                    if (PlayerManager.AccountExists(dbConnection, parse[1]) == false) {
                                        Messenger.PlainMsg(client, "This account does not exist.", Enums.PlainMsgType.MainMenu);
                                        return;
                                    }
                                    // Check the client version
                                    if (parse[3].ToInt() < Constants.CLIENT_VERSION) {
                                        Messenger.PlainMsg(client, "Your client is outdated. Please update your client.", Enums.PlainMsgType.MainMenu);
                                        return;
                                    }
                                    // Verify that the client used is a valid one
                                    if (parse[10] != "7") {
                                        Messenger.PlainMsg(client, "Error!", Enums.PlainMsgType.MainMenu);
                                        return;
                                    }

#if !DEBUG
                                    if (ClientManager.IsMacAddressConnected(parse[11])) {
                                        Messenger.PlainMsg(client, "You are already playing on this computer!", Enums.PlainMsgType.MainMenu);
                                        return;
                                    }

#endif

                                    string password = Security.Hash.GenerateMD5Hash(parse[2]).Trim();
                                    if (PlayerManager.IsPasswordCorrect(dbConnection, parse[1], password)) {

                                        client.SetMacAddress(parse[11]);
                                        // Check if they aren't banned
                                        if (Bans.IsIPBanned(dbConnection, client) == Enums.BanType.Ban || Bans.IsMacBanned(dbConnection, client) == Enums.BanType.Ban) {
                                            Logging.ChatLogger.AppendToChatLog("Staff", "IP: " + client.IP + " Mac: " + client.MacAddress + " attempted to log on, but was banned.");
                                            Messenger.PlainMsg(client, "You are banned!", Enums.PlainMsgType.MainMenu);
                                            client.SetMacAddress("");
                                            return;
                                        }

                                        client.Player.AccountName = parse[1];

                                        client.Player.SetSystemInfo(parse[8], parse[9], parse[10]);
                                        Messenger.SendChars(dbConnection, client);
                                    } else {
                                        Messenger.PlainMsg(client, "Incorrect password", Enums.PlainMsgType.MainMenu);
                                    }
                                }
                            } else {
                                Messenger.PlainMsg(client, "You are already playing!", Enums.PlainMsgType.MainMenu);
                            }
                        }
                        break;
                    case "charlistrequest": {
                            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                                Messenger.SendChars(dbConnection, client);
                            }
                        }
                        break;
                    case "usechar": {
                            int charNum = parse[1].ToInt();
                            if (charNum < 1 || charNum > 3)
                                return;
                            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                                if (client.Player.CharacterExists(dbConnection, charNum)) {
                                    client.Player.LoadCharacter(dbConnection, parse[1].ToInt(1));
                                    if (Globals.GMOnly) {
                                        if (client.Player.Access == Enums.Rank.Normal) {
                                            Messenger.PlainMsg(client, "The server is only open to GMs at the moment!", Enums.PlainMsgType.MainMenu);
                                            client.CloseConnection();
                                            //PlayerManager.Players.RemovePlayer(client);
                                            //PlayerManager.Players.AddPlayer(client);
                                            return;
                                        }
                                    }

                                    if (ClientManager.IsCharacterLoggedIn(client.Player.CharID)) {
                                        Messenger.PlainMsg(client, "You are already playing on this computer!", Enums.PlainMsgType.MainMenu);
                                        return;
                                    }

                                    if (Bans.IsCharacterBanned(dbConnection, client.Player.CharID) == Enums.BanType.Ban) {
                                        Messenger.PlainMsg(client, "This character is banned!", Enums.PlainMsgType.MainMenu);
                                        client.Player = new Player(client);
                                        client.CloseConnection();
                                        return;
                                    }

                                    client.Player.Statistics.HandleLogin(client.Player.GetOSVersion(), client.Player.GetDotNetVersion(), client.MacAddress, client.IP);

                                    Messenger.SendJoinGame(client);

                                    //Globals.ServerUI.AddLog(PlayerManager.Players.GetPlayer(client).mLogin + "/" + PlayerManager.Players.GetPlayer(client).mName + " has started playing!");
                                } else {
                                    Messenger.PlainMsg(client, "Character does not exist", Enums.PlainMsgType.Chars);
                                    return;
                                }
                            }
                        }
                        break;
                    #endregion
                    #region Account Editing
                    case "gatglasses": {//typo?  then again, we don't need classes...
                            Messenger.SendNewCharClasses(client);
                        }
                        break;
                    case "createaccount": {
                            if (client.Player.LoggedIn == false) {
                                string name = parse[1];
                                string password = parse[2];
                                string email = parse[3];

                                for (int i = 0; i < name.Length; i++) {
                                    int n = (int)Convert.ToChar(name.Substring(i, 1));

                                    if (n == 32 && (i == 0 || i == name.Length - 1)) {
                                        Messenger.PlainMsg(client, "Names cannot begin or end with spaces.", Enums.PlainMsgType.NewAccount);
                                        return;
                                    }

                                    if ((n >= 65 & n <= 90) | (n >= 97 & n <= 122) | (n == 95) | (n == 32) | (n >= 48 & n <= 57)) {
                                    } else {
                                        Messenger.PlainMsg(client, "Invalid name, only letters, numbers, spaces, and _ allowed in names.", Enums.PlainMsgType.NewAccount);
                                        return;
                                    }
                                }


                                if (name.Length < 3 || name.Length > 24) {
                                    Messenger.PlainMsg(client, "Name must be between 3 and 24 letters long.", Enums.PlainMsgType.NewAccount);
                                    return;
                                }

                                // TODO: Add email support to account creation [HIGH]
                                //if (Email.Email.IsValidEmail(email) == false) {
                                //    Messenger.PlainMsg(client, "Invalid email.", Enums.PlainMsgType.NewAccount);
                                //    return;
                                //}

                                using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                                    int result = PlayerManager.CreateNewAccount(dbConnection, name, password, "");
                                    if (result == -1) {
                                        Messenger.PlainMsg(client, "An error occurred while creating your account.", Enums.PlainMsgType.MainMenu);
                                    } else if (result == 0) {
                                        Messenger.PlainMsg(client, "Your account has been created!", Enums.PlainMsgType.MainMenu);
                                    } else if (result == 1) {
                                        Messenger.PlainMsg(client, "That account already exists!", Enums.PlainMsgType.MainMenu);
                                    }
                                }
                            }
                        }
                        break;
                    case "addchar": {
                            string name = parse[1].Trim();
                            int sex = parse[2].ToInt(-1);
                            //int CharClass = parse[3].ToInt(-1); ((This should be removed too))
                            //int CharNum = parse[4].ToInt(-1);
                            int charNum = parse[3].ToInt(-1);

                            for (int i = 0; i < name.Length; i++) {
                                int n = (int)Convert.ToChar(name.Substring(i, 1));

                                if (n == 32 && (i == 0 || i == name.Length - 1)) {
                                    Messenger.PlainMsg(client, "Names cannot begin or end with spaces.", Enums.PlainMsgType.NewChar);
                                    return;
                                }

                                if ((n >= 65 && n <= 90) || (n >= 97 && n <= 122) || (n == 95) || (n == 32) || (n >= 48 && n <= 57)) {
                                } else {
                                    //Messenger.SendNewCharClasses(client); ((More classes?))
                                    Messenger.PlainMsg(client, "Invalid name, only letters, numbers, spaces, and _ allowed in names.", Enums.PlainMsgType.NewChar);
                                    return;
                                }
                            }

                            if (name.Length < 3 || name.Length > 24) {
                                Messenger.PlainMsg(client, "Name must be between 3 and 24 letters long.", Enums.PlainMsgType.NewChar);
                                return;
                            }

                            if (charNum < 1 || charNum > 3) {
                                Messenger.PlainMsg(client, "Invalid character number", Enums.PlainMsgType.Chars);
                                return;
                            }

                            if ((sex < (int)Enums.Sex.Genderless) || (sex > (int)Enums.Sex.Female)) {
                                Messenger.PlainMsg(client, "Invalid sex", Enums.PlainMsgType.Chars);
                                return;
                            }

                            //if (CharClass < 0 || CharClass > Settings.MaxClasses) {
                            //Messenger.PlainMsg(client, "Invalid class", Enums.PlainMsgType.Chars);
                            //return;
                            //} ((this should be removed. We no longer use classes in the Add Character Window))

                            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                                if (client.Player.CharacterExists(dbConnection, charNum)) {
                                    Messenger.PlainMsg(client, "Character already exists!", Enums.PlainMsgType.NewChar);
                                    return;
                                }

                                if (PlayerManager.CharacterNameExists(dbConnection, name)) {
                                    Messenger.PlainMsg(client, "Sorry, but that name is in use!", Enums.PlainMsgType.NewChar);
                                    return;
                                }

                                client.Player.CreateCharacter(dbConnection, name, (Enums.Sex)sex, charNum, false);
                            }
                        }
                        break;
                    case "delchar": {
                            int charNum = parse[1].ToInt(-1);

                            if (charNum < 1 || charNum > 4) {
                                Messenger.PlainMsg(client, "Unable to delete character. Wrong character slot.", Enums.PlainMsgType.Chars);
                                return;
                            }

                            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                                if (client.Player.CharacterExists(dbConnection, charNum)) {
                                    PlayerManager.DeleteCharacter(dbConnection, client.Player.AccountName, charNum);
                                    Messenger.SendChars(dbConnection, client);
                                    Messenger.PlainMsg(client, "Character has been deleted!", Enums.PlainMsgType.Chars);
                                } else {
                                    Messenger.PlainMsg(client, "Character does not exist!", Enums.PlainMsgType.Chars);
                                }
                            }
                        }
                        break;
                    case "deleteaccount": {
                            string name = parse[1];
                            string password = parse[2];

                            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                                if (PlayerManager.AccountExists(dbConnection, name) == false) {
                                    Messenger.PlainMsg(client, "That account does not exist", Enums.PlainMsgType.MainMenu);
                                    return;
                                }

                                if (PlayerManager.IsPasswordCorrect(dbConnection, name, password) == false) {
                                    Messenger.PlainMsg(client, "Incorrect password", Enums.PlainMsgType.MainMenu);
                                    return;
                                }

                                PlayerManager.DeleteAccount(dbConnection, name);
                            }
                            Messenger.PlainMsg(client, "Your account has been deleted!", Enums.PlainMsgType.MainMenu);
                        }
                        break;
                    case "passchange": {
                            string name = parse[1];
                            string oldPass = Security.Hash.GenerateMD5Hash(parse[2]);
                            string newPass = Security.Hash.GenerateMD5Hash(parse[3]);

                            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                                if (PlayerManager.IsPasswordCorrect(dbConnection, name, oldPass)) {
                                    PlayerManager.ChangeAccountPassword(dbConnection, name, oldPass, newPass);
                                    client.Player = new Player(client);
                                    Messenger.PlainMsg(client, "Your password has been changed!", Enums.PlainMsgType.AccountOptions);
                                } else {
                                    Messenger.PlainMsg(client, "Unable to change password.", Enums.PlainMsgType.AccountOptions);
                                }
                            }
                        }
                        break;
                    #endregion
                }
                #endregion
            } else {
                #region Logged In
                switch (parse[0].ToLower()) {
                    #region Movement
                    case "playermove": {
                            if (client.Player.GettingMap) {
                                return;
                            }
                            int dir = parse[1].ToInt();
                            int speed = parse[2].ToInt();

                            if (dir < 0 || dir > 3) {
                                return;
                            }
                            if (speed < 0 || speed > 7) {
                                return;
                            }

                            MovementProcessor.ProcessMovement(client, (Enums.Direction)dir, (Enums.Speed)speed, false);
                        }
                        break;
                    case "critmove": {
                            if (client.Player.GettingMap) {
                                return;
                            }
                            int dir = parse[1].ToInt();
                            int speed = parse[2].ToInt();

                            if (dir < 0 || dir > 3) {
                                return;
                            }
                            if (speed < 0 || speed > 7) {
                                return;
                            }

                            MovementProcessor.ProcessMovement(client, (Enums.Direction)dir, (Enums.Speed)speed, true);
                        }
                        break;
                    case "playerdir": {
                            if (client.Player.GettingMap) {
                                return;
                            }
                            int dir = parse[1].ToInt();
                            if (dir < 0 || dir > 3) {
                                return;
                            }

                            client.Player.Direction = (Enums.Direction)dir;
                            PacketHitList hitlist = null;
                            PacketHitList.MethodStart(ref hitlist);
                            PacketBuilder.AppendPlayerDirExcept(client, hitlist);
                            PacketHitList.MethodEnded(ref hitlist);
                        }
                        break;
                    case "requestnewmap": {
                            //if (client.Player.Map.MapType != Enums.MapType.Instanced) {
                            int dir = parse[1].ToInt();

                            if (dir < 0 || dir > 3) {
                                return;
                            }

                            if (MovementProcessor.WillWalkOnWarp(client, (Enums.Direction)dir)) {
                                MovementProcessor.ProcessMovement(client, (Enums.Direction)dir, (Enums.Speed)1, true);
                            } else {
                                PacketHitList hitlist = null;
                                PacketHitList.MethodStart(ref hitlist);
                                PacketBuilder.AppendOwnXY(client, hitlist);
                                PacketBuilder.AppendPlayerLock(client, hitlist, false);
                                PacketHitList.MethodEnded(ref hitlist);
                            }
                        }
                        break;
                    case "needmap": {
                            Messenger.NeedMapCheck(client, parse[1].ToBool());
                        }
                        break;
                    case "needmapseamless": {
                            bool[] results = new bool[parse[1].ToInt()];
                            int n = 2;
                            for (int i = 0; i < results.Length; i++) {
                                results[i] = parse[n].ToBool();
                                n += 1;
                            }
                            Messenger.NeedMapCheck(client, results);
                        }
                        break;
                    case "refresh": {
                            Messenger.RefreshMap(client);
                        }
                        break;
                    case "maploaded": {
                            client.Player.Map.ProcessingPaused = true;
                        }
                        break;
                    #endregion
                    #region Attacking
                    case "attack": {
                            if (client.Player.GettingMap) return;
                            client.Player.GetActiveRecruit().UseMove(-1);

                        }
                        break;
                    #endregion
                    #region Attacking - Moves
                    case "forgetspell": {
                            // Spell slot
                            int n = parse[1].ToInt(-1);
                            // Prevent subscript out of range
                            if (n < 0 | n > Constants.MAX_PLAYER_MOVES) {
                                Messenger.HackingAttempt(client, "Invalid Move Slot");
                                return;
                            }

                            if (client.Player.GetActiveRecruit().Moves[n].MoveNum == 0) {
                                Messenger.PlayerMsg(client, "No move here.", Text.Red);
                            } else {
                                Messenger.PlayerMsg(client, "You have forgotten the move \"" + MoveManager.Moves[client.Player.GetActiveRecruit().Moves[n].MoveNum].Name.Trim() + "\"", Text.Green);
                                client.Player.GetActiveRecruit().Moves[n].MoveNum = 0;
                                client.Player.GetActiveRecruit().Moves[n].CurrentPP = -1;
                                client.Player.GetActiveRecruit().Moves[n].MaxPP = -1;
                                Messenger.SendPlayerMoves(client);
                            }

                        }
                        break;
                    case "swapmoves": {
                            int oldMoveSlot = parse[1].ToInt(-1);
                            int newMoveSlot = parse[2].ToInt(-1);
                            if (oldMoveSlot > -1 && oldMoveSlot < 4 && newMoveSlot > -1 && newMoveSlot < 4) {
                                RecruitMove oldMove = client.Player.GetActiveRecruit().Moves[oldMoveSlot];
                                RecruitMove newMove = client.Player.GetActiveRecruit().Moves[newMoveSlot];
                                client.Player.GetActiveRecruit().Moves[oldMoveSlot] = newMove;
                                client.Player.GetActiveRecruit().Moves[newMoveSlot] = oldMove;

                                Messenger.SendPlayerMoves(client);
                            }
                        }
                        break;
                    case "shiftspell": {
                            // Spell slot
                            int n = parse[1].ToInt(-1);

                            // Prevent subscript out of range
                            if (n < 0 | n > Constants.MAX_PLAYER_MOVES) {
                                Messenger.HackingAttempt(client, "Invalid Move Slot");
                                return;
                            }

                            //if (client.Player.GetActiveRecruit().Moves[n].MoveNum == 0) //~no need to check for blank space; you can move blank spaces
                            //{
                            //Messenger.PlayerMsg(client, "No move here.", Text.Red);
                            //}
                            //else
                            //{
                            int movingSpell = client.Player.GetActiveRecruit().Moves[n].MoveNum;
                            int movingSpellCurrentPP = client.Player.GetActiveRecruit().Moves[n].CurrentPP;
                            int movingSpellMaxPP = client.Player.GetActiveRecruit().Moves[n].MaxPP;

                            if (parse[2].ToBool() && n > 0)//shifting up
                                {
                                client.Player.GetActiveRecruit().Moves[n].MoveNum = client.Player.GetActiveRecruit().Moves[n - 1].MoveNum;
                                client.Player.GetActiveRecruit().Moves[n - 1].MoveNum = movingSpell;

                                client.Player.GetActiveRecruit().Moves[n].CurrentPP = client.Player.GetActiveRecruit().Moves[n - 1].CurrentPP;
                                client.Player.GetActiveRecruit().Moves[n - 1].CurrentPP = movingSpellCurrentPP;

                                client.Player.GetActiveRecruit().Moves[n].MaxPP = client.Player.GetActiveRecruit().Moves[n - 1].MaxPP;
                                client.Player.GetActiveRecruit().Moves[n - 1].MaxPP = movingSpellMaxPP;

                                Scripting.ScriptManager.InvokeSub("OnMoveSwapped", client, n, n - 1);
                            } else if (n < 3)//shifting down
                                {
                                client.Player.GetActiveRecruit().Moves[n].MoveNum = client.Player.GetActiveRecruit().Moves[n + 1].MoveNum;
                                client.Player.GetActiveRecruit().Moves[n + 1].MoveNum = movingSpell;

                                client.Player.GetActiveRecruit().Moves[n].CurrentPP = client.Player.GetActiveRecruit().Moves[n + 1].CurrentPP;
                                client.Player.GetActiveRecruit().Moves[n + 1].CurrentPP = movingSpellCurrentPP;

                                client.Player.GetActiveRecruit().Moves[n].MaxPP = client.Player.GetActiveRecruit().Moves[n + 1].MaxPP;
                                client.Player.GetActiveRecruit().Moves[n + 1].MaxPP = movingSpellMaxPP;

                                Scripting.ScriptManager.InvokeSub("OnMoveSwapped", client, n, n + 1);
                            }


                            Messenger.SendPlayerMoves(client);
                            //}
                        }
                        break;
                    case "cast": {
                            if (client.Player.GettingMap) return;
                            client.Player.GetActiveRecruit().UseMove(parse[1].ToInt(-1));
                        }
                        break;
                    #endregion
                    #region Messages
                    case "broadcastmsg":
                        if (CanSayMsg(client, parse[1])) {
                            Scripting.ScriptManager.InvokeSub("OnChatMessageRecieved", client, parse[1], Enums.ChatMessageType.Global);

                            Logging.ChatLogger.AppendToChatLog("Global", client.Player.Name + ": " + parse[1]);

                            string playerName = client.Player.Name;
                            if (client.Player.Access != Enums.Rank.Normal) {
                                playerName = "[c][" + Ranks.GetRankColor(client.Player.Access).ToArgb() + "]" + client.Player.Name + "[/c]";
                            }

                            Messenger.GlobalMsg("[Global] " + playerName + ": " + parse[1], Text.DarkGrey);
                        }
                        break;
                    case "saymsg":
                        if (CanSayMsg(client, parse[1])) {
                            Scripting.ScriptManager.InvokeSub("OnChatMessageRecieved", client, parse[1], Enums.ChatMessageType.Map);

                            Logging.ChatLogger.AppendToChatLog("Maps/Map " + client.Player.MapID, client.Player.Name + ": " + parse[1]);

                            string playerName = client.Player.Name;
                            if (client.Player.Access != Enums.Rank.Normal) {
                                playerName = "[c][" + Ranks.GetRankColor(client.Player.Access).ToArgb() + "]" + client.Player.Name + "[/c]";
                            }

                            PacketHitList hitList = null;
                            PacketHitList.MethodStart(ref hitList);
                            IMap playerMap = client.Player.GetCurrentMap();
                            hitList.AddPacketToMap(playerMap, PacketBuilder.CreateChatMsg(playerName + ": " + parse[1], Text.White));
                            hitList.AddPacketToMap(playerMap, PacketBuilder.CreateSpeechBubble(parse[1], client));
                            PacketHitList.MethodEnded(ref hitList);
                        }
                        break;
                    case "emotemsg":
                        if (CanSayMsg(client, parse[1])) {
                            Scripting.ScriptManager.InvokeSub("OnChatMessageRecieved", client, parse[1], Enums.ChatMessageType.MeMsg);

                            Logging.ChatLogger.AppendToChatLog("Maps/Map " + client.Player.MapID, "(MeMsg) " + client.Player.Name + " " + parse[1]);

                            string playerName = client.Player.Name;
                            if (client.Player.Access != Enums.Rank.Normal) {
                                playerName = "[c][" + Ranks.GetRankColor(client.Player.Access).ToArgb() + "]" + client.Player.Name + "[/c]";
                            }

                            Messenger.MapMsg(client.Player.MapID, playerName + " " + parse[1], Text.Blue);
                        }
                        break;
                    case "globalmsg":
                        if (CanSayMsg(client, parse[1])) {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                Scripting.ScriptManager.InvokeSub("OnChatMessageRecieved", client, parse[1], Enums.ChatMessageType.Announcement);

                                Logging.ChatLogger.AppendToChatLog("Global", "(Announcement) " + client.Player.Name + ": " + parse[1]);

                                string playerName = client.Player.Name;
                                if (client.Player.Access != Enums.Rank.Normal) {
                                    playerName = "[c][" + Ranks.GetRankColor(client.Player.Access).ToArgb() + "]" + client.Player.Name + "[/c]";
                                }

                                Messenger.GlobalMsg("(Announcement) " + playerName + ": " + parse[1], Text.Green);
                            }
                        }
                        break;
                    case "adminmsg":
                        if (CanSayMsg(client, parse[1])) {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {

                                Scripting.ScriptManager.InvokeSub("OnChatMessageRecieved", client, parse[1], Enums.ChatMessageType.Staff);

                                Logging.ChatLogger.AppendToChatLog("Staff", client.Player.Name + ": " + parse[1]);

                                string playerName = client.Player.Name;
                                if (client.Player.Access != Enums.Rank.Normal) {
                                    playerName = "[c][" + Ranks.GetRankColor(client.Player.Access).ToArgb() + "]" + client.Player.Name + "[/c]";
                                }

                                Messenger.AdminMsg("[Staff] " + playerName + ": " + parse[1], Text.Cyan);
                            }
                        }
                        break;
                    case "guildmsg":
                        if (CanSayMsg(client, parse[1]) && !string.IsNullOrEmpty(client.Player.GuildName)) {
                            Scripting.ScriptManager.InvokeSub("OnChatMessageRecieved", client, parse[1], Enums.ChatMessageType.Guild);

                            //Messenger.PlayerMsg(client, client.Player[client].mName + " (" + client.Player[client].mGuildName + ") " + parse[1], Text.Green);
                            Logging.ChatLogger.AppendToChatLog("Guild Chat/" + client.Player.GuildName, "(" + client.Player.GuildName + ") " + client.Player.Name + parse[1]);

                            string playerName = client.Player.Name;
                            if (client.Player.Access != Enums.Rank.Normal) {
                                playerName = "[c][" + Ranks.GetRankColor(client.Player.Access).ToArgb() + "]" + client.Player.Name + "[/c]";
                            }

                            foreach (Client i in ClientManager.GetClients()) {
                                if (i.Player.GuildName == client.Player.GuildName) {
                                    Messenger.PlayerMsg(i, "(" + client.Player.GuildName + ") " + playerName + parse[1], Text.Green);
                                }
                            }
                        }
                        break;
                    case "playermsg": {
                            if (CanSayMsg(client, parse[2])) {
                                Client msgto = ClientManager.FindClient(parse[1]);
                                if (msgto == null) {
                                    Messenger.PlayerMsg(client, "Player is offline.", Text.Grey);
                                } else if (msgto == client) {
                                    string sexString = "";
                                    if (client.Player.GetActiveRecruit().Sex == Enums.Sex.Genderless) {
                                        sexString = "it";
                                    } else if (client.Player.GetActiveRecruit().Sex == Enums.Sex.Male) {
                                        sexString = "him";
                                    } else if (client.Player.GetActiveRecruit().Sex == Enums.Sex.Female) {
                                        sexString = "her";
                                    }
                                    Messenger.MapMsg(client.Player.MapID, client.Player.Name + " begins to mumble to " + sexString + "self, what a wierdo...", Text.White);
                                } else {
                                    Scripting.ScriptManager.InvokeSub("OnChatMessageRecieved", client, parse[2], Enums.ChatMessageType.PM);

                                    Logging.ChatLogger.AppendToChatLog("PM/" + client.Player.Name, "(" + client.Player.Name + " tells " + msgto.Player.Name + ") " + parse[2]);

                                    string playerName = client.Player.Name;
                                    if (client.Player.Access != Enums.Rank.Normal) {
                                        playerName = "[c][" + Ranks.GetRankColor(client.Player.Access).ToArgb() + "]" + client.Player.Name + "[/c]";
                                    }

                                    string playerToName = msgto.Player.Name;
                                    if (msgto.Player.Access != Enums.Rank.Normal) {
                                        playerToName = "[c][" + Ranks.GetRankColor(msgto.Player.Access).ToArgb() + "]" + msgto.Player.Name + "[/c]";
                                    }

                                    Messenger.PlayerMsg(client, "You tell " + playerToName + ", '" + parse[2] + "'", Text.Pink);
                                    Messenger.PlayerMsg(msgto, playerName + " tells you, '" + parse[2] + "'", Text.Pink);
                                }

                            }
                        }
                        break;
                    #endregion
                    #region Recruits
                    case "requestactivecharswap": {//perhaps put a time requirement for switching...
                            if (client.Player.GettingMap) return;
                            //if (client.Player.PK == false) {
                            client.Player.SwapActiveRecruit(parse[1].ToInt(1));
                            //} else {
                            //    Messenger.PlayerMsg(client, "An outlaw can't switch Pokémon!", Text.BrightRed);
                            //}
                        }
                        break;
                    case "switchleader": {
                            if (client.Player.Map.Tile[client.Player.X, client.Player.Y].Type == Enums.TileType.Assembly) {
                                //if (client.Player.PK == false) {
                                client.Player.SwapLeader(parse[1].ToInt(1));

                                //} else {
                                //    Messenger.PlayerMsg(client, "An outlaw can't switch leaders!", Text.BrightRed);
                                //}
                            } else {
                                Messenger.HackingAttempt(client, "Player not in Assembly");
                            }
                        }
                        break;
                    case "addtoteam":
                        if (client.Player.Map.Tile[client.Player.X, client.Player.Y].Type == Enums.TileType.Assembly) {
                            int teamSlot = parse[1].ToInt(-1);
                            client.Player.AddToTeam(parse[2].ToInt(-1), teamSlot);
                            if (client.Player.Team[teamSlot] != null && client.Player.Team[teamSlot].Loaded) {
                                client.Player.Team[teamSlot].HP = client.Player.Team[teamSlot].MaxHP;
                            }
                            Messenger.SendActiveTeam(client);
                            Messenger.SendStats(client);
                            Messenger.SendAllRecruits(client);
                        } else {
                            Messenger.SendPlayerData(client);
                            Messenger.HackingAttempt(client, "Player not in Assembly");
                        }
                        break;
                    case "removefromteam":
                        client.Player.RemoveFromTeam(parse[1].ToInt(-1));

                        break;
                    case "standby":
                        client.Player.RemoveFromTeam(parse[1].ToInt(-1));
                        Messenger.SendAllRecruits(client);
                        break;
                    case "releaserecruit":
                        client.Player.RequestedRecruit = new Recruit(client);
                        client.Player.RequestedRecruit.RecruitIndex = parse[1].ToInt(-1);
                        Messenger.AskQuestion(client, "ReleasePokemon", "Are you sure you want to release this recruit?", -1);
                        break;
                    case "changerecruitname":
                        if (client.Player.Map.Tile[client.Player.X, client.Player.Y].Type == Enums.TileType.Assembly) {
                            if (client.Player.Muted == false) {
                                if (parse[2].Length >= 3 && parse[2].Length <= 24) {
                                    client.Player.ChangeRecruitName(parse[1].ToInt(-1), parse[2]);
                                } else {
                                    Messenger.PlayerMsg(client, "Name must be between 3 and 24 letters long.", Text.BrightRed);
                                }
                            } else {
                                Messenger.PlayerMsg(client, "You cannot change nicknames while muted!", Text.BrightRed);
                            }
                        } else {
                            Messenger.SendPlayerData(client);
                            Messenger.HackingAttempt(client, "Player not in Assembly");
                        }
                        break;
                    #endregion
                    #region Missions
                    case "acceptmission": {
                            if (client.Player.Map.Moral == Enums.MapMoral.House || client.Player.Map.Moral == Enums.MapMoral.Safe) {
                                int slot = parse[1].ToInt(-1);
                                if (slot > -1 && slot < client.Player.MissionBoard.BoardMissions.Count) {
                                    bool taken = client.Player.JobList.AddJob(new WonderMails.WonderMailJob(client.Player.MissionBoard.BoardMissions[slot]));
                                    if (taken) {
                                        client.Player.MissionBoard.BoardMissions.RemoveAt(slot);
                                        Messenger.SendRemovedMission(client, slot);
                                        Messenger.SendNewJob(client);
                                    }
                                }
                            } else {
                                Messenger.PlayerMsg(client, "You can't accept a job here!", Text.BrightRed);
                            }
                        }
                        break;
                    case "startmission": {
                            if (client.Player.Map.Moral == Enums.MapMoral.House || client.Player.Map.Moral == Enums.MapMoral.Safe) {
                                int slot = parse[1].ToInt(-1);
                                if (slot > -1 && slot < client.Player.JobList.JobList.Count) {
                                    client.Player.JobList.JobList[slot].Accepted = Enums.JobStatus.Taken;
                                    Messenger.SendJobAccept(client, slot);
                                }
                            } else {
                                Messenger.PlayerMsg(client, "You can't take a job here!", Text.BrightRed);
                            }
                        }
                        break;
                    case "deletejob": {
                            if (client.Player.Map.Moral == Enums.MapMoral.House || client.Player.Map.Moral == Enums.MapMoral.Safe) {
                                int slot = parse[1].ToInt(-1);
                                if (slot > -1 && slot < client.Player.JobList.JobList.Count) {
                                    client.Player.JobList.RemoveJob(slot);
                                    Messenger.SendDeleteJob(client, slot);
                                }
                            } else {
                                Messenger.PlayerMsg(client, "You can't delete a job here!", Text.BrightRed);
                            }
                        }
                        break;
                    case "sendmission": {
                            if (client.Player.Map.Moral == Enums.MapMoral.House || client.Player.Map.Moral == Enums.MapMoral.Safe) {
                                Client target = ClientManager.FindClient(parse[2]);
                                if (target != null) {
                                    if (target != client) {
                                        int slot = parse[1].ToInt(-1);
                                        if (target.Player.GetDungeonCompletionCount(client.Player.JobList.JobList[slot].Mission.DungeonIndex) > 0) {
                                            if (target.Player.JobList.JobList.Count < Constants.MAX_JOB_LIST) {
                                                if (Combat.MoveProcessor.IsInAreaRange(1, client.Player.X, client.Player.Y, target.Player.X, target.Player.Y)) {

                                                    if (slot > -1 && slot < client.Player.JobList.JobList.Count) {
                                                        client.Player.JobList.JobList[slot].SendsRemaining--;
                                                        if (client.Player.JobList.JobList[slot].SendsRemaining <= 0) {
                                                            //send sends remaining
                                                            Messenger.SendRemainingJobSends(client, slot);
                                                        }
                                                        target.Player.JobList.AddJobSimple(new WonderMails.WonderMailJob(client.Player.JobList.JobList[slot]));
                                                        Messenger.PlayerMsg(client, "A Wonder Mail was sent to " + target.Player.Name + "!", Text.BrightGreen);
                                                        Messenger.PlayerMsg(target, client.Player.Name + " has sent you a Wonder Mail!", Text.BrightGreen);
                                                        Messenger.SendNewJob(target);
                                                    }

                                                } else {
                                                    Messenger.PlayerMsg(client, "You have to stand next to the player to send a Wonder Mail.", Text.BrightRed);
                                                }
                                            } else {
                                                Messenger.PlayerMsg(client, "That player's job list is full.", Text.BrightRed);
                                            }
                                        } else {
                                            Messenger.PlayerMsg(client, "That player has not yet completed the dungeon specified in the Wonder Mail.", Text.BrightRed);
                                        }
                                    } else {
                                        Messenger.PlayerMsg(client, "You can't send a Wonder Mail to yourself!", Text.BrightRed);
                                    }
                                } else {
                                    Messenger.PlayerMsg(client, "There's no one online with that name.", Text.BrightRed);
                                }
                            } else {
                                Messenger.PlayerMsg(client, "You can't send a Wonder Mail here!", Text.BrightRed);
                            }
                        }
                        break;
                    case "canceljob": {
                            if (client.Player.Map.Moral == Enums.MapMoral.House || client.Player.Map.Moral == Enums.MapMoral.Safe) {
                                int slot = parse[1].ToInt(-1);
                                if (slot > -1 && slot < client.Player.JobList.JobList.Count) {
                                    client.Player.JobList.JobList[slot].Accepted = Enums.JobStatus.Suspended;
                                    Messenger.SendJobAccept(client, slot);
                                }
                            } else {
                                Messenger.PlayerMsg(client, "You can't cancel a job here!", Text.BrightRed);
                            }
                        }
                        break;
                    //case "requestmissioncode": {
                    //        int slot = parse[1].ToInt(-1);
                    //        if (slot > -1 && slot < client.Player.JobList.JobList.Count) {
                    //            if (client.Player.JobList.JobList[slot] != null) {
                    //                WonderMails.IssuedWonderMailEntry entry = WonderMails.WonderMailSystem.FindIssuedWonderMailByCode(client.Player.JobList.JobList[slot].Mission.EncryptedCode);
                    //                if (entry == null) {
                    //                    // This code has not yet been issued
                    //                } else {

                    //                }

                    //                Messenger.SendMissionCode(client, client.Player.JobList.JobList[slot].Mission);
                    //            }
                    //        }
                    //    }
                    //    break;
                    #endregion
                    #region Misc
                    case "ping":
                        //Messenger.SendDataTo(client, TcpPacket.CreatePacket("ping"));
                        break;
                    case "search": {
                            int X = parse[1].ToInt(-1);
                            int Y = parse[2].ToInt(-1);

                            IMap map = client.Player.GetCurrentMap();

                            // Prevent subscript out of range
                            if (X < 0 || X > map.MaxX || Y < 0 || Y > map.MaxY) {
                                return;
                            }

                            // Check for a player
                            foreach (Client i in map.GetClients()) {
                                if (i.IsPlaying() && map == i.Player.Map && i.Player.X == X && i.Player.Y == Y) {
                                    // Consider the player
                                    if (MovementProcessor.CanCharacterSeeCharacter(map, client.Player.GetActiveRecruit(), i.Player.GetActiveRecruit())) {
                                        if (MovementProcessor.CanCharacterIdentifyCharacter(map, client.Player.GetActiveRecruit(), i.Player.GetActiveRecruit())) {
                                            if (i == client) {
                                                Messenger.PlayerMsg(client, "You look upon yourself.", Text.Yellow);
                                            } else {
                                                Messenger.PlayerMsg(client, "You see " + i.Player.Name + ".", Text.Yellow);
                                                if (i.Player.GetActiveRecruit().Level >= client.Player.GetActiveRecruit().Level + 5) {
                                                    Messenger.PlayerMsg(client, "This Pokémon appears to be much more experienced than you.", Text.BrightRed);
                                                } else if (i.Player.GetActiveRecruit().Level > client.Player.GetActiveRecruit().Level) {
                                                    Messenger.PlayerMsg(client, "This Pokémon appears to be slightly more experienced than you.", Text.Yellow);
                                                } else if (i.Player.GetActiveRecruit().Level == client.Player.GetActiveRecruit().Level) {
                                                    Messenger.PlayerMsg(client, "This Pokémon seems to be about as experienced as you.", Text.White);
                                                } else if (client.Player.GetActiveRecruit().Level >= i.Player.GetActiveRecruit().Level + 5) {
                                                    Messenger.PlayerMsg(client, "This Pokémon appears much less experienced than you.", Text.BrightBlue);
                                                } else if (client.Player.GetActiveRecruit().Level > i.Player.GetActiveRecruit().Level) {
                                                    Messenger.PlayerMsg(client, "This Pokémon appears slightly less experienced than you.", Text.Yellow);
                                                }
                                            }
                                            return;
                                        } else {
                                            Messenger.PlayerMsg(client, "You see a Pokémon.", Text.Yellow);
                                            return;
                                        }
                                    }
                                }
                            }

                            // Check for an npc
                            for (int i = 0; i < Constants.MAX_MAP_NPCS; i++) {
                                if (map.ActiveNpc[i].Num > 0) {
                                    if (map.ActiveNpc[i].X == X && map.ActiveNpc[i].Y == Y) {
                                        if (MovementProcessor.CanCharacterSeeCharacter(map, client.Player.GetActiveRecruit(), map.ActiveNpc[i])) {
                                            if (MovementProcessor.CanCharacterIdentifyCharacter(map, client.Player.GetActiveRecruit(), map.ActiveNpc[i])) {
                                                Messenger.PlayerMsg(client, "You see a " + NpcManager.Npcs[map.ActiveNpc[i].Num].Name.Trim() + ".", Text.Yellow);

                                                if (!map.RecruitEnabled) {
                                                    Messenger.PlayerMsg(client, NpcManager.Npcs[map.ActiveNpc[i].Num].Name.Trim() + " cannot be recruited in this area.", Text.Yellow);

                                                } else if (map.ActiveNpc[i].Level >= client.Player.GetActiveRecruit().Level) {
                                                    Messenger.PlayerMsg(client, NpcManager.Npcs[map.ActiveNpc[i].Num].Name.Trim() + "'s level is too high for you to recruit it.", Text.Yellow);

                                                } else if (NpcManager.Npcs[map.ActiveNpc[i].Num].RecruitRate != 0) {
                                                    int recruitRate = NpcManager.Npcs[map.ActiveNpc[i].Num].RecruitRate;
                                                    //Messenger.PlayerMsg(client, NpcManager.Npcs[map.ActiveNpc[i].Num].Name.Trim() + "'s recruit rate is " + NpcManager.Npcs[map.ActiveNpc[i].Num].RecruitRate + " (+" + client.Player.GetRecruitBonus() + ")", Text.Yellow);
                                                    Messenger.PlayerMsg(client, "You have a " + (recruitRate / 10f) + "% of recruiting " + NpcManager.Npcs[map.ActiveNpc[i].Num].Name.Trim() + "!", Text.Yellow);
                                                } else {
                                                    Messenger.PlayerMsg(client, NpcManager.Npcs[map.ActiveNpc[i].Num].Name.Trim() + " is not recruitable.", Text.Yellow);
                                                }
                                                return;
                                            } else {
                                                Messenger.PlayerMsg(client, "You see a Pokémon.", Text.Yellow);
                                                return;
                                            }
                                        }
                                    }
                                }
                            }

                            for (int i = 0; i < Constants.MAX_MAP_ITEMS; i++) {
                                if (map.ActiveItem[i].Num > 0/* && !map.ActiveItem[i].Hidden*/) {
                                    if (map.ActiveItem[i].X == X && map.ActiveItem[i].Y == Y) {
                                        if (MovementProcessor.CanCharacterSeeDestination(map, client.Player.GetActiveRecruit(), X, Y) && !map.ActiveItem[i].Hidden) {
                                            if (MovementProcessor.CanCharacterIdentifyDestination(map, client.Player.GetActiveRecruit(), X, Y) && !map.ActiveItem[i].Hidden) {
                                                if (map.ActiveItem[i].Sticky) {
                                                    Messenger.PlayerMsg(client, "You see a x" + ItemManager.Items[map.ActiveItem[i].Num].Name.Trim() + ".", Text.Yellow);
                                                    return;
                                                } else {
                                                    Messenger.PlayerMsg(client, "You see a " + ItemManager.Items[map.ActiveItem[i].Num].Name.Trim() + ".", Text.Yellow);
                                                    return;
                                                }
                                            } else {
                                                Messenger.PlayerMsg(client, "You notice an item.", Text.Yellow);
                                                return;
                                            }
                                        }
                                    }
                                }
                            }

                            Scripting.ScriptManager.InvokeSub("OnClick", client, map, X, Y);
                        }
                        break;
                    case "mapgetitem": {
                            // Check for scripted signs
                            if (client.Player.GettingMap) return;
                            IMap map = client.Player.GetCurrentMap();
                            int newX = client.Player.X;
                            int newY = client.Player.Y;
                            Enums.Direction dir = client.Player.Direction;
                            switch (client.Player.Direction) {
                                case Enums.Direction.Up:
                                    newY--;
                                    break;
                                case Enums.Direction.Down:
                                    newY++;
                                    break;
                                case Enums.Direction.Left:
                                    newX--;
                                    break;
                                case Enums.Direction.Right:
                                    newX++;
                                    break;
                            }
                            if (newX >= 0 && newX < map.MaxX && newY >= 0 && newY < map.MaxY) {
                                if (map.Tile[newX, newY].Type == Enums.TileType.ScriptedSign) {
                                    Scripting.ScriptManager.InvokeSub("ScriptedSign", client, map.Tile[newX, newY].Data1, map.Tile[newX, newY].String1, map.Tile[newX, newY].String2, map.Tile[newX, newY].String3, dir);
                                } else if (map.Tile[newX, newY].Type == Enums.TileType.Key) {
                                    bool moveUsed = false;
                                    for (int i = 0; i < client.Player.GetActiveRecruit().Moves.Length; i++) {
                                        if (client.Player.GetActiveRecruit().Moves[i].MoveNum > -1) {
                                            Move move = MoveManager.Moves[client.Player.GetActiveRecruit().Moves[i].MoveNum];
                                            if (move.KeyItem > 0) {
                                                if (move.KeyItem == map.Tile[newX, newY].Data1) {
                                                    Messenger.AskQuestion(client, "UseMoveKeyOnEnter", "Would you like to use " + move.Name + "?", -1, new string[] { "Yes", "No" });
                                                    moveUsed = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    if (moveUsed == false) {
                                        for (int i = 1; i <= client.Player.MaxInv; i++) {
                                            if (client.Player.Inventory[i].Num == map.Tile[newX, newY].Data1) {
                                                if (ItemManager.Items[client.Player.Inventory[i].Num].Type == Enums.ItemType.Key) {
                                                    Messenger.AskQuestion(client, "UseKeyOnEnter", "Would you like to use your " + ItemManager.Items[client.Player.Inventory[i].Num].Name + " to unlock the door?", -1, new string[] { "Yes", "No" });
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            client.Player.PickupItem();
                        }
                        break;
                    case "mapdropitem": {
                            if (client.Player.GettingMap) return;
                            int invNum = parse[1].ToInt();
                            int amount = parse[2].ToInt();

                            // Prevent hacking
                            if (invNum < 1 || invNum > client.Player.MaxInv || client.Player.Inventory[invNum].Num <= 0) {
                                Messenger.HackingAttempt(client, "Invalid InvNum: " + invNum);
                                return;
                            }

                            // Prevent hacking
                            if (ItemManager.Items[client.Player.Inventory[invNum].Num].Type == Enums.ItemType.Currency || ItemManager.Items[client.Player.Inventory[invNum].Num].StackCap > 0) {
                                // Check if money and if it is we want to make sure that they aren't trying to drop 0 value
                                if (amount <= 0) {
                                    Messenger.PlayerMsg(client, "You must drop more than 0!", Text.BrightRed);
                                    return;
                                }

                                if (amount > client.Player.Inventory[invNum].Amount) {
                                    Messenger.PlayerMsg(client, "You dont have that much to drop!", Text.BrightRed);
                                    return;
                                }
                            }

                            // Prevent hacking
                            if (ItemManager.Items[client.Player.Inventory[invNum].Num].Type != Enums.ItemType.Currency && ItemManager.Items[client.Player.Inventory[invNum].Num].StackCap > 0) {
                                if (amount > client.Player.Inventory[invNum].Amount) {
                                    Messenger.HackingAttempt(client, "Item amount modification");
                                    return;
                                }
                            }

                            client.Player.DropItem(invNum, amount);
                            //Messenger.SendStats(client);
                        }
                        break;
                    case "adventurelog": {
                            Messenger.SendAdventureLog(client);
                        }
                        break;
                    case "checkarrows": {
                            //int n = ArrowManagerBase.Arrows[parse[1].ToInt()].Pic;
                            //Messenger.SendDataToMap(client.Player.MapID, TcpPacket.CreatePacket("checkarrows", client.ToString(), n.ToString()));
                        }
                        break;
                    case "checkemoticons": {
                            if (client.Player.Muted == false) {
                                int n = EmoticonManagerBase.Emoticons[parse[1].ToInt()].Pic;
                                Messenger.SendDataToMap(client.Player.MapID, TcpPacket.CreatePacket("checkemoticons", client.ToString(), n.ToString()));
                            }
                        }
                        break;
                    case "clienterror": {
                            string error = parse[1];
                            error += "\r\n\r\n------- ERROR REPORT END -------\r\n\r\n";
                            Logging.Logger.AppendToLog("/Client Error Logs/" + DateTime.Now.ToShortDateString().Replace("/", "-") + "/log.txt", error);
                        }
                        break;
                    #endregion
                    #region Scripts
                    case "hotscript1":
                        Scripting.ScriptManager.InvokeSub("HotScript1", client);
                        break;
                    case "hotscript2":
                        Scripting.ScriptManager.InvokeSub("HotScript2", client);
                        break;
                    case "hotscript3":
                        Scripting.ScriptManager.InvokeSub("HotScript3", client);
                        break;
                    case "hotscript4":
                        Scripting.ScriptManager.InvokeSub("HotScript4", client);
                        break;
                    case "questionresult":
                        if (!string.IsNullOrEmpty(client.Player.QuestionID)) {
                            string id = client.Player.QuestionID;
                            client.Player.QuestionID = "";

                            if (id.StartsWith("DropItem:")) {
                                if (parse[1] == "Yes") {
                                    string[] parseID = id.Split(':');
                                    int itemSlot = parseID[1].ToInt();
                                    int amount = parseID[2].ToInt();
                                    if (client.Player.Inventory[itemSlot].Num <= 0) return;
                                    if (client.Player.Inventory[itemSlot].Amount < amount) return;
                                    if (amount == 0) amount = 1;
                                    int payout = ItemManager.Items[client.Player.Inventory[itemSlot].Num].Price * amount;
                                    if (client.Player.FindInvSlot(1, payout) == -1) {
                                        Messenger.PlayerMsg(client, "You can't carry any more " + ItemManager.Items[1].Name + ".", Text.BrightRed);
                                        return;
                                    }
                                    client.Player.GiveItem(1, payout);
                                    Messenger.PlayerMsg(client, "Trade successful!", Text.Yellow);
                                    client.Player.TakeItemSlot(itemSlot, amount, true);
                                    //client.Player.DropItem(itemSlot, amount, null, true);
                                }
                            }

                            switch (id) {
                                case "BuyItem": {
                                        if (parse[1] == "Yes") {
                                            if (client.Player.HasItem(1) < client.Player.Map.Tile[client.Player.X, client.Player.Y].Data1) {
                                                Messenger.PlayerMsg(client, "You don't have enough " + ItemManager.Items[1].Name + " to buy this item!", Text.BrightRed);
                                                return;
                                            }

                                            if (String.IsNullOrEmpty(client.Player.Map.Tile[client.Player.X, client.Player.Y].String1)) {
                                                Messenger.PlayerMsg(client, "Trade successful!", Text.Yellow);
                                                client.Player.PickupItem(true);
                                                client.Player.TakeItem(1, client.Player.Map.Tile[client.Player.X, client.Player.Y].Data1, true);
                                            } else {
                                                int slot = client.Player.FindInvSlot(1);
                                                if (slot == -1 || client.Player.Inventory[slot].Num != 1) return;
                                                Messenger.PlayerMsg(client, "Trade successful!", Text.Yellow);
                                                client.Player.PickupItem(true);
                                                client.Player.DropItem(slot, client.Player.Map.Tile[client.Player.X, client.Player.Y].Data1, null, true);
                                            }
                                        }
                                    }
                                    break;
                                case "CreateGuild": {
                                        #region CreateGuild Result
                                        if (parse[1] == "Cancel") {
                                            client.Player.GuildName = "";
                                        }
                                        #endregion
                                    }
                                    break;
                                case "GuildStepDown": {
                                        #region GuildStepDown Result
                                        if (parse[1] == "Yes") {
                                            Guilds.GuildManager.GuildStepDown(client);
                                        }
                                        #endregion
                                    }
                                    break;
                                case "DisbandGuild": {
                                        #region DisbandGuild Result
                                        if (parse[1] == "Yes") {
                                            Guilds.GuildManager.DisbandGuild(client);
                                        }
                                        #endregion
                                    }
                                    break;
                                case "RecruitPokemon": {
                                        #region RecruitPokemon Result
                                        if (parse[1] == "Yes") {
                                            int openSlot = client.Player.FindOpenTeamSlot();
                                            int recruitIndex = 0;
                                            if (openSlot != -1) {
                                                if (client.Player.RequestedRecruit.Level > 0) {
                                                    using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                                                        recruitIndex = client.Player.AddToRecruitmentBank(dbConnection, client.Player.RequestedRecruit);
                                                    }

                                                    if (recruitIndex != -1) {
                                                        client.Player.AddToTeam(recruitIndex, openSlot);
                                                        client.Player.Team[openSlot].HP = client.Player.Team[openSlot].MaxHP;
                                                        Messenger.BattleMsg(client, "You have recruited a new team member!", Text.BrightGreen);

                                                        Messenger.SendActiveTeam(client);
                                                    } else {
                                                        Messenger.BattleMsg(client, "You cant recruit! You have too many team members in the assembly!", Text.BrightRed);
                                                    }
                                                } else {
                                                    Messenger.BattleMsg(client, "You cant recruit! Error 2", Text.BrightRed);
                                                }
                                            } else {
                                                Messenger.BattleMsg(client, "You cant recruit! Your team is full!", Text.BrightRed);
                                            }
                                        } else {
                                            client.Player.RequestedRecruit = null;
                                        }

                                        client.Player.RequestedRecruit = null;
                                        #endregion
                                    }
                                    break;
                                case "ReleasePokemon": {
                                        #region ReleasePokemon Result
                                        if (parse[1] == "Yes") {

                                            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                                                client.Player.ReleaseRecruit(dbConnection, client.Player.RequestedRecruit.RecruitIndex);
                                            }

                                        }
                                        client.Player.RequestedRecruit = null;
                                        #endregion
                                    }
                                    break;
                                case "MovesFull": {
                                        #region MovesFull Result
                                        if (parse[1] == "Yes") {
                                            bool allowLearn = true;
                                            if (client.Player.GetActiveRecruit().MoveItem != -1) {
                                                if (client.Player.HasItem(client.Player.GetActiveRecruit().MoveItem, true) > 0) {
                                                    client.Player.TakeItem(client.Player.GetActiveRecruit().MoveItem, 0, true);
                                                } else {
                                                    allowLearn = false;
                                                }
                                            }
                                            if (allowLearn) {
                                                Messenger.OpenMoveForgetMenu(client);
                                            }
                                            client.Player.GetActiveRecruit().MoveItem = -1;
                                        } else {
                                            client.Player.GetActiveRecruit().MoveItem = -1;
                                            Messenger.PlayerMsg(client, "You chose to not learn this move.", Text.White);
                                        }
                                        #endregion
                                    }
                                    break;
                                case "PlayerTradeRequest": {
                                        #region PlayerTradeRequest Result
                                        if (parse[1] == "Yes") {
                                            Client tradePartner = ClientManager.FindClientFromCharID(client.Player.TradePartner);
                                            if (tradePartner != null) {
                                                tradePartner.Player.TradePartner = client.Player.CharID;
                                                Messenger.OpenTradeMenu(client);
                                            }
                                        }
                                        #endregion
                                    }
                                    break;
                                case "SingleEvolution": {
                                        #region SingleEvolution Result

                                        PacketHitList hitlist = null;
                                        PacketHitList.MethodStart(ref hitlist);

                                        if (parse[1] == "Yes") {
                                            int speciesNum = client.Player.GetActiveRecruit().Species;
                                            int evoIndex = -1;
                                            for (int z = 0; z < EvolutionManager.Evolutions.MaxEvos; z++) {
                                                if (EvolutionManager.Evolutions[z].Species == speciesNum) {
                                                    evoIndex = z;
                                                }
                                            }
                                            if (evoIndex == -1) return;

                                            List<int> viableEvos = EvolutionManager.FindViableEvolutions(client, evoIndex);
                                            if (viableEvos.Count == 1) {
                                                EvolutionBranch evo = Evolutions.EvolutionManager.Evolutions[evoIndex].Branches[viableEvos[0]];
                                                client.Player.GetActiveRecruit().SetSpecies(evo.NewSpecies);
                                                if (client.Player.GetActiveRecruit().RecruitIndex > -2) {
                                                    client.Player.PokemonCaught(client.Player.GetActiveRecruit().Species);
                                                }
                                                PacketBuilder.AppendPlayerData(client, hitlist);
                                                Messenger.SendActiveTeam(client);
                                                PacketBuilder.AppendStats(client, hitlist);
                                                hitlist.AddPacket(client, PacketBuilder.CreateChatMsg("You have evolved into " + Pokedex.Pokedex.GetPokemon(client.Player.GetActiveRecruit().Species).Name + "! Congratulations!", Text.Yellow));
                                                Scripting.ScriptManager.InvokeFunction("OnEvoDone", client, evo.ReqScript, evo.Data1, evo.Data2, evo.Data3);
                                            }

                                        }

                                        PacketHitList.MethodEnded(ref hitlist);

                                        #endregion
                                    }
                                    break;
                                case "BranchEvolution": {
                                        #region BranchEvolution Result
                                        PacketHitList hitlist = null;
                                        PacketHitList.MethodStart(ref hitlist);
                                        if (parse[1] == "Cancel") return;
                                        int speciesNum = client.Player.GetActiveRecruit().Species;
                                        int evoIndex = -1;
                                        for (int z = 0; z < EvolutionManager.Evolutions.MaxEvos; z++) {
                                            if (EvolutionManager.Evolutions[z].Species == speciesNum) {
                                                evoIndex = z;
                                            }
                                        }
                                        if (evoIndex == -1) return;

                                        for (int i = 0; i <= EvolutionManager.Evolutions[evoIndex].Branches.Count; i++) {
                                            bool canEvolve = Scripting.ScriptManager.InvokeFunction("ScriptedEvoReq", client, EvolutionManager.Evolutions[evoIndex].Branches[i].ReqScript, EvolutionManager.Evolutions[evoIndex].Branches[i].Data1, EvolutionManager.Evolutions[evoIndex].Branches[i].Data2, EvolutionManager.Evolutions[evoIndex].Branches[i].Data3).ToBool();
                                            if (canEvolve && EvolutionManager.Evolutions[evoIndex].Branches[i].Name == parse[1]) {
                                                EvolutionBranch evo = Evolutions.EvolutionManager.Evolutions[evoIndex].Branches[i];
                                                client.Player.GetActiveRecruit().SetSpecies(evo.NewSpecies);
                                                if (client.Player.GetActiveRecruit().RecruitIndex > -2) {
                                                    client.Player.PokemonCaught(client.Player.GetActiveRecruit().Species);
                                                }
                                                PacketBuilder.AppendPlayerData(client, hitlist);
                                                Messenger.SendActiveTeam(client);
                                                PacketBuilder.AppendStats(client, hitlist);
                                                hitlist.AddPacket(client, PacketBuilder.CreateChatMsg("You have evolved into " + Pokedex.Pokedex.GetPokemon(client.Player.GetActiveRecruit().Species).Name + "! Congratulations!", Text.Yellow));
                                                Scripting.ScriptManager.InvokeFunction("OnEvoDone", client, evo.ReqScript, evo.Data1, evo.Data2, evo.Data3);
                                                break;
                                            }
                                        }


                                        PacketHitList.MethodEnded(ref hitlist);

                                        //Scripting.ScriptManager.InvokeFunction("EvoDone", player.Client, Evolutions[evoIndex].Branches[i].ReqScript, Evolutions[evoIndex].Branches[i].Data1, Evolutions[evoIndex].Branches[i].Data2, Evolutions[evoIndex].Branches[i].Data3)
                                        #endregion
                                    }
                                    break;
                                case "UseKeyOnEnter": {
                                        #region UseKeyOnEnter Result
                                        if (parse[1] == "Yes") {
                                            IMap map = client.Player.GetCurrentMap();
                                            int newX = client.Player.X;
                                            int newY = client.Player.Y;
                                            Enums.Direction dir = client.Player.Direction;
                                            switch (client.Player.Direction) {
                                                case Enums.Direction.Up:
                                                    newY--;
                                                    break;
                                                case Enums.Direction.Down:
                                                    newY++;
                                                    break;
                                                case Enums.Direction.Left:
                                                    newX--;
                                                    break;
                                                case Enums.Direction.Right:
                                                    newX++;
                                                    break;
                                            }
                                            if (newX >= 0 && newX < map.MaxX && newY >= 0 && newY < map.MaxY) {
                                                if (map.Tile[newX, newY].Type == Enums.TileType.Key) {
                                                    for (int i = 1; i <= client.Player.MaxInv; i++) {
                                                        if (client.Player.Inventory[i].Num == map.Tile[newX, newY].Data1) {
                                                            client.Player.UseItem(client.Player.Inventory[i], i);
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        } else if (parse[1] == "No") {

                                        }
                                        #endregion
                                    }
                                    break;
                                case "UseMoveKeyOnEnter": {
                                        #region UseKeyOnEnter Result
                                        if (parse[1] == "Yes") {
                                            IMap map = client.Player.GetCurrentMap();
                                            int newX = client.Player.X;
                                            int newY = client.Player.Y;
                                            Enums.Direction dir = client.Player.Direction;
                                            switch (client.Player.Direction) {
                                                case Enums.Direction.Up:
                                                    newY--;
                                                    break;
                                                case Enums.Direction.Down:
                                                    newY++;
                                                    break;
                                                case Enums.Direction.Left:
                                                    newX--;
                                                    break;
                                                case Enums.Direction.Right:
                                                    newX++;
                                                    break;
                                            }
                                            if (newX >= 0 && newX < map.MaxX && newY >= 0 && newY < map.MaxY) {
                                                if (map.Tile[newX, newY].Type == Enums.TileType.Key) {
                                                    for (int i = 0; i < client.Player.GetActiveRecruit().Moves.Length; i++) {
                                                        if (client.Player.GetActiveRecruit().Moves[i].MoveNum > -1) {
                                                            Move move = MoveManager.Moves[client.Player.GetActiveRecruit().Moves[i].MoveNum];
                                                            if (move.KeyItem > 0) {
                                                                if (move.KeyItem == map.Tile[newX, newY].Data1) {
                                                                    client.Player.GetActiveRecruit().UseMoveKey(map, move, i);
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                    break;
                                default: {
                                        Scripting.ScriptManager.InvokeSub("QuestionResult", client, id, parse[1]);
                                    }
                                    break;
                            }
                        }
                        break;
                    case "checkcommands":
                        if (!client.Player.Dead) {
                            Scripting.ScriptManager.InvokeSub("Commands", client, CommandProcessor.ParseCommand(parse[1].Trim()));
                        }
                        break;
                    case "reloadscripts": {
                            if (Ranks.IsDisallowed(client, Enums.Rank.Scripter)) {
                                Messenger.HackingAttempt(client, "Admin cloning");
                            } else {
                                Scripting.ScriptManager.Reload();
                                Messenger.PlayerMsg(client, "Scripts reloaded!", Text.Yellow);
                            }
                        }
                        break;
                    #endregion
                    #region Tile Checks
                    case "buysprite": {
                            IMap map = client.Player.GetCurrentMap();
                            if (map.Tile[client.Player.X, client.Player.Y].Type != Enums.TileType.SpriteChange) {
                                Messenger.PlayerMsg(client, "You need to be on a sprite tile to buy it!", Text.BrightRed);
                                return;
                            }

                            if (map.Tile[client.Player.X, client.Player.Y].Data2 == 0) {
                                client.Player.GetActiveRecruit().SetSpecies(map.Tile[client.Player.X, client.Player.Y].Data1);
                                Messenger.SendStats(client);
                                Messenger.SendInventory(client);
                                Messenger.SendActiveTeam(client);
                                Messenger.SendPlayerData(client);
                                return;
                            }

                            for (int i = 0; i <= client.Player.MaxInv; i++) {
                                if (client.Player.Inventory[i].Num == map.Tile[client.Player.X, client.Player.Y].Data2) {
                                    if (ItemManager.Items[client.Player.Inventory[i].Num].Type == Enums.ItemType.Currency) {
                                        if (client.Player.Inventory[i].Amount >= map.Tile[client.Player.X, client.Player.Y].Data3) {
                                            client.Player.TakeItemSlot(i, map.Tile[client.Player.X, client.Player.Y].Data3, true);
                                            Messenger.PlayerMsg(client, "You have bought a new sprite!", Text.BrightGreen);
                                            client.Player.GetActiveRecruit().SetSpecies(map.Tile[client.Player.X, client.Player.Y].Data1);
                                            Messenger.SendStats(client);
                                            //Messenger.SendDataToMap(client.Player.mMap, "checksprite" + TcpManager.SEP_CHAR + client.ToString() + TcpManager.SEP_CHAR + client.Player.GetActiveChar().mSprite.ToString() + TcpManager.SEP_CHAR + TcpManager.END_CHAR);
                                            Messenger.SendInventory(client);
                                            Messenger.SendActiveTeam(client);
                                            Messenger.SendPlayerData(client);
                                        }
                                    } else {
                                        if (client.Player.GetItemSlotHolder(i) == -1) {
                                            client.Player.TakeItemSlot(i, 1, true);
                                            Messenger.PlayerMsg(client, "You have bought a new sprite!", Text.BrightGreen);
                                            client.Player.GetActiveRecruit().SetSpecies(map.Tile[client.Player.X, client.Player.Y].Data1);
                                            Messenger.SendStats(client);
                                            //Messenger.SendDataToMap(client.Player.mMap, "checksprite" + TcpManager.SEP_CHAR + client.ToString() + TcpManager.SEP_CHAR + client.Player.GetActiveChar().mSprite.ToString() + TcpManager.SEP_CHAR + TcpManager.END_CHAR);
                                            Messenger.SendInventory(client);
                                            Messenger.SendActiveTeam(client);
                                            Messenger.SendPlayerData(client);
                                        }
                                    }
                                    if (client.Player.GetItemSlotHolder(i) == -1) {
                                        return;
                                    }
                                }
                            }

                            Messenger.PlayerMsg(client, "You dont have enough to buy this sprite!", Text.BrightRed);
                        }
                        break;
                    case "buyhouse": {
                            // TODO: Remake "buyhouse" packet processor
                            //IMap map;
                            //if (client.Player.Map != -2) {
                            //    map = MapManager.Maps[client.Player.Map];
                            //} else {
                            //    Messenger.PlayerMsg(client, "You can't buy an instanced map!", Text.BrightRed);
                            //    return;
                            //}
                            //if (map.Tile[client.Player.X, client.Player.Y].Type != Enums.TileType.House) {
                            //    Messenger.PlayerMsg(client, "You need to be on a house tile to buy it!", Text.BrightRed);
                            //    return;
                            //}

                            //if (map.Tile[client.Player.X, client.Player.Y].Data1 == 0) {
                            //    Messenger.PlayerMsg(client, "You have bought a new house!", Text.BrightGreen);
                            //    map.Owner = client.Player.Name;
                            //    map.Name = client.Player.Name + "'s House";
                            //    map.Revision = map.Revision + 1;
                            //    map.Save();
                            //    //MapManager.SaveMap(map.MapNum);
                            //    Messenger.SendDataToMap(map.MapID, TcpPacket.CreatePacket("checkformap", map.MapID.ToString(), map.Revision.ToString()));
                            //    return;
                            //}

                            //for (int i = 1; i <= Constants.MAX_INV; i++) {
                            //    if (client.Player.Inventory[i].Num == map.Tile[client.Player.X, client.Player.Y].Data1) {
                            //        if (ItemManager.Items[client.Player.Inventory[i].Num].Type == Enums.ItemType.Currency) {
                            //            if (client.Player.Inventory[i].Val >= map.Tile[client.Player.X, client.Player.Y].Data2) {
                            //                client.Player.Inventory[i].Val -= map.Tile[client.Player.X, client.Player.Y].Data2;
                            //                if (client.Player.Inventory[i].Val <= 0) {
                            //                    client.Player.Inventory[i].Val = 0;
                            //                }
                            //                Messenger.PlayerMsg(client, "You have bought a new house!", Text.BrightGreen);
                            //                map.Owner = client.Player.Name;
                            //                map.Name = client.Player.Name + "'s House";
                            //                map.Revision = map.Revision + 1;
                            //                map.Save();
                            //                //MapManager.SaveMap(map.MapNum);
                            //                Messenger.SendDataToMap(map.MapNum, TcpPacket.CreatePacket("CHECKFORMAP", map.MapNum.ToString(), map.Revision.ToString()));
                            //                Messenger.SendInventory(client);
                            //            }
                            //        } else {
                            //            if (client.Player.Weapon != i & client.Player.Weapon != i & client.Player.Shield != i & client.Player.Helmet != i & client.Player.Legs != i & client.Player.Ring != i & client.Player.Necklace != i) {
                            //                client.Player.Inventory[i].Num = 0;
                            //                Messenger.PlayerMsg(client, "You now own a new house!", Text.BrightGreen);
                            //                map.Owner = client.Player.Name;
                            //                map.Name = client.Player.Name + "'s House";
                            //                map.Revision = map.Revision + 1;
                            //                MapManager.SaveMap(map.MapNum);
                            //                Messenger.SendDataToMap(map.MapNum, TcpPacket.CreatePacket("CHECKFORMAP", map.MapNum.ToString(), map.Revision.ToString()));
                            //                Messenger.SendInventory(client);
                            //            }
                            //        }
                            //        if (client.Player.Weapon != i & client.Player.Weapon != i & client.Player.Shield != i & client.Player.Helmet != i & client.Player.Legs != i & client.Player.Ring != i & client.Player.Necklace != i) {
                            //            return;
                            //        }
                            //    }
                            //}

                            //Messenger.PlayerMsg(client, "You dont have enough to buy this house!", Text.BrightRed);
                        }
                        break;
                    #endregion
                    #region Story
                    case "needstory": {
                            if (parse[1] == "yes") {
                                Messenger.SendUpdateStoryTo(client, parse[2].ToInt());
                                if (client.Player.SegmentToStart > -1) {
                                    Messenger.SendStartStoryTo(client, parse[2].ToInt(), client.Player.SegmentToStart);
                                } else {
                                    Messenger.SendStartStoryTo(client, parse[2].ToInt());
                                }
                            } else if (parse[1] == "no") {
                                if (client.Player.SegmentToStart > -1) {
                                    Messenger.SendStartStoryTo(client, parse[2].ToInt(), client.Player.SegmentToStart);
                                } else {
                                    Messenger.SendStartStoryTo(client, parse[2].ToInt());
                                }
                            }
                        }
                        break;
                    case "updatesegment": {
                            client.Player.CurrentSegment = parse[1].ToInt(-1);
                        }
                        break;
                    case "actonaction": {
                            int storyNum = -1;
                            if (client.Player.CurrentChapter != null && !string.IsNullOrEmpty(client.Player.CurrentChapter.ID)) {
                                storyNum = client.Player.CurrentChapter.ID.ToInt(-1);
                            }
                            if (client.Player.CurrentChapter != null) {
                                StorySegment story = client.Player.CurrentChapter.Segments[client.Player.CurrentSegment];
                                switch (story.Action) {
                                    case Enums.StoryAction.RunScript: {
                                            Stories.StoryManager.RunScript(client, story.Parameters.GetValue("ScriptIndex").ToInt(),
                                                story.Parameters.GetValue("ScriptParam1"),
                                                story.Parameters.GetValue("ScriptParam2"),
                                                story.Parameters.GetValue("ScriptParam3"),
                                                story.Parameters.GetValue("Pause").ToBool());
                                        }
                                        break;
                                    case Enums.StoryAction.Padlock: {
                                            if (storyNum > -1) {
                                                if (story.Parameters.GetValue("State") == "Unlock") {
                                                    client.Player.SetStoryState(storyNum, false);
                                                } else if (story.Parameters.GetValue("State") == "Lock") {
                                                    client.Player.SetStoryState(storyNum, true);
                                                }
                                            }
                                        }
                                        break;
                                    case Enums.StoryAction.Warp: {
                                            int X, Y;
                                            if (story.Parameters.GetValue("X") == "-1") {
                                                X = client.Player.X;
                                            } else {
                                                int tmpX = story.Parameters.GetValue("X").ToInt(-1);
                                                if (tmpX > -1) {
                                                    X = tmpX;
                                                } else {
                                                    X = client.Player.X;
                                                }
                                            }
                                            if (story.Parameters.GetValue("Y") == "-1") {
                                                Y = client.Player.Y;
                                            } else {
                                                int tmpY = story.Parameters.GetValue("Y").ToInt(-1);
                                                if (tmpY > -1) {
                                                    Y = tmpY;
                                                } else {
                                                    Y = client.Player.Y;
                                                }
                                            }
                                            Messenger.PlayerWarp(client, story.Parameters.GetValue("MapID"), X, Y);
                                        }
                                        break;
                                }
                            }
                        }
                        break;
                    case "chaptercomplete": {
                            client.Player.CurrentChapter = null;
                            client.Player.CurrentSegment = -1;
                            if (client.Player.StoryPlaybackCache.Count > 0) {
                                if (!string.IsNullOrEmpty(client.Player.StoryPlaybackCache[0].ID) && client.Player.StoryPlaybackCache[0].ID.IsNumeric()) {
                                    StoryManager.PlayStory(client, client.Player.StoryPlaybackCache[0].ID.ToInt());
                                } else {
                                    StoryManager.PlayStory(client, client.Player.StoryPlaybackCache[0]);
                                }
                                client.Player.StoryPlaybackCache.RemoveAt(0);
                            }
                        }
                        break;
                    case "storyloadingcomplete": {
                            client.Player.LoadingStory = false;
                        }
                        break;
                    #endregion
                    #region Editors

                    #region Random Dungeons
                    case "requesteditrdungeon": {
                            if (Ranks.IsDisallowed(client, Enums.Rank.Mapper)) {
                                Messenger.HackingAttempt(client, "Admin cloning");
                                return;
                            }
                            Messenger.SendRDungeonEditor(client);
                        }
                        break;
                    case "addnewrdungeon": {
                            if (Ranks.IsDisallowed(client, Enums.Rank.Mapper)) {
                                Messenger.HackingAttempt(client, "Admin cloning");
                                return;
                            }
                            Messenger.SendAddRDungeonToAll(RDungeonManager.AddRDungeon());
                        }
                        break;
                    case "editrdungeon": {
                            if (Ranks.IsDisallowed(client, Enums.Rank.Mapper)) {
                                Messenger.HackingAttempt(client, "Admin cloning");
                                return;
                            }
                            int n = parse[1].ToInt();
                            if (n < 0 || n > RDungeonManager.RDungeons.Count - 1) {
                                Messenger.PlayerMsg(client, "Invalid dungeon client", Text.BrightRed);
                                return;
                            }

                            Messenger.SendEditRDungeonTo(client, n);
                        }
                        break;
                    case "saverdungeon": {//adjust to new variables
                            if (Ranks.IsDisallowed(client, Enums.Rank.Mapper)) {
                                Messenger.HackingAttempt(client, "Admin cloning");
                                return;
                            }

                            int z = parse[1].ToInt();

                            RDungeonManager.RDungeons[z].DungeonName = parse[2];
                            RDungeonManager.RDungeons[z].Direction = (Enums.Direction)parse[3].ToInt();
                            int maxFloors = parse[4].ToInt();
                            RDungeonManager.RDungeons[z].Recruitment = parse[5].ToBool();
                            RDungeonManager.RDungeons[z].Exp = parse[6].ToBool();
                            RDungeonManager.RDungeons[z].WindTimer = parse[7].ToInt();
                            RDungeonManager.RDungeons[z].DungeonIndex = parse[8].ToInt();
                            int n = 9;
                            RDungeonManager.RDungeons[z].Floors.Clear();
                            for (int i = 0; i < maxFloors; i++) {
                                RDungeonManager.RDungeons[z].Floors.Add(new RDungeonFloor());
                                RDungeonManager.RDungeons[z].Floors[i].Options.TrapMin = parse[n].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].Options.TrapMax = parse[n + 1].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].Options.ItemMin = parse[n + 2].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].Options.ItemMax = parse[n + 3].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].Options.Intricacy = parse[n + 4].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].Options.RoomWidthMin = parse[n + 5].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].Options.RoomWidthMax = parse[n + 6].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].Options.RoomLengthMin = parse[n + 7].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].Options.RoomLengthMax = parse[n + 8].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].Options.HallTurnMin = parse[n + 9].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].Options.HallTurnMax = parse[n + 10].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].Options.HallVarMin = parse[n + 11].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].Options.HallVarMax = parse[n + 12].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].Options.WaterFrequency = parse[n + 13].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].Options.Craters = parse[n + 14].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].Options.CraterMinLength = parse[n + 15].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].Options.CraterMaxLength = parse[n + 16].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].Options.CraterFuzzy = parse[n + 17].ToBool();
                                RDungeonManager.RDungeons[z].Floors[i].Options.MinChambers = parse[n + 18].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].Options.MaxChambers = parse[n + 19].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].Darkness = parse[n + 20].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].GoalType = (Enums.RFloorGoalType)parse[n + 21].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].GoalMap = parse[n + 22].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].GoalX = parse[n + 23].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].GoalY = parse[n + 24].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].Music = parse[n + 25];

                                n += 26;

                                RDungeonManager.RDungeons[z].Floors[i].StairsX = parse[n].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].StairsSheet = parse[n + 1].ToInt();

                                RDungeonManager.RDungeons[z].Floors[i].mGroundX = parse[n + 2].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mGroundSheet = parse[n + 3].ToInt();

                                RDungeonManager.RDungeons[z].Floors[i].mTopLeftX = parse[n + 4].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mTopLeftSheet = parse[n + 5].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mTopCenterX = parse[n + 6].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mTopCenterSheet = parse[n + 7].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mTopRightX = parse[n + 8].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mTopRightSheet = parse[n + 9].ToInt();

                                RDungeonManager.RDungeons[z].Floors[i].mCenterLeftX = parse[n + 10].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mCenterLeftSheet = parse[n + 11].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mCenterCenterX = parse[n + 12].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mCenterCenterSheet = parse[n + 13].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mCenterRightX = parse[n + 14].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mCenterRightSheet = parse[n + 15].ToInt();

                                RDungeonManager.RDungeons[z].Floors[i].mBottomLeftX = parse[n + 16].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mBottomLeftSheet = parse[n + 17].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mBottomCenterX = parse[n + 18].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mBottomCenterSheet = parse[n + 19].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mBottomRightX = parse[n + 20].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mBottomRightSheet = parse[n + 21].ToInt();

                                RDungeonManager.RDungeons[z].Floors[i].mInnerTopLeftX = parse[n + 22].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mInnerTopLeftSheet = parse[n + 23].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mInnerBottomLeftX = parse[n + 24].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mInnerBottomLeftSheet = parse[n + 25].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mInnerTopRightX = parse[n + 26].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mInnerTopRightSheet = parse[n + 27].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mInnerBottomRightX = parse[n + 28].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mInnerBottomRightSheet = parse[n + 29].ToInt();

                                RDungeonManager.RDungeons[z].Floors[i].mIsolatedWallX = parse[n + 30].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mIsolatedWallSheet = parse[n + 31].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mColumnTopX = parse[n + 32].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mColumnTopSheet = parse[n + 33].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mColumnCenterX = parse[n + 34].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mColumnCenterSheet = parse[n + 35].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mColumnBottomX = parse[n + 36].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mColumnBottomSheet = parse[n + 37].ToInt();

                                RDungeonManager.RDungeons[z].Floors[i].mRowLeftX = parse[n + 38].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mRowLeftSheet = parse[n + 39].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mRowCenterX = parse[n + 40].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mRowCenterSheet = parse[n + 41].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mRowRightX = parse[n + 42].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mRowRightSheet = parse[n + 43].ToInt();

                                n += 44;

                                RDungeonManager.RDungeons[z].Floors[i].mGroundAltX = parse[n].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mGroundAltSheet = parse[n + 1].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mGroundAlt2X = parse[n + 2].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mGroundAlt2Sheet = parse[n + 3].ToInt();

                                RDungeonManager.RDungeons[z].Floors[i].mTopLeftAltX = parse[n + 4].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mTopLeftAltSheet = parse[n + 5].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mTopCenterAltX = parse[n + 6].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mTopCenterAltSheet = parse[n + 7].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mTopRightAltX = parse[n + 8].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mTopRightAltSheet = parse[n + 9].ToInt();

                                RDungeonManager.RDungeons[z].Floors[i].mCenterLeftAltX = parse[n + 10].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mCenterLeftAltSheet = parse[n + 11].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mCenterCenterAltX = parse[n + 12].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mCenterCenterAltSheet = parse[n + 13].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mCenterCenterAlt2X = parse[n + 14].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mCenterCenterAlt2Sheet = parse[n + 15].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mCenterRightAltX = parse[n + 16].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mCenterRightAltSheet = parse[n + 17].ToInt();

                                RDungeonManager.RDungeons[z].Floors[i].mBottomLeftAltX = parse[n + 18].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mBottomLeftAltSheet = parse[n + 19].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mBottomCenterAltX = parse[n + 20].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mBottomCenterAltSheet = parse[n + 21].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mBottomRightAltX = parse[n + 22].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mBottomRightAltSheet = parse[n + 23].ToInt();

                                RDungeonManager.RDungeons[z].Floors[i].mInnerTopLeftAltX = parse[n + 24].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mInnerTopLeftAltSheet = parse[n + 25].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mInnerBottomLeftAltX = parse[n + 26].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mInnerBottomLeftAltSheet = parse[n + 27].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mInnerTopRightAltX = parse[n + 28].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mInnerTopRightAltSheet = parse[n + 29].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mInnerBottomRightAltX = parse[n + 30].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mInnerBottomRightAltSheet = parse[n + 31].ToInt();

                                RDungeonManager.RDungeons[z].Floors[i].mIsolatedWallAltX = parse[n + 32].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mIsolatedWallAltSheet = parse[n + 33].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mColumnTopAltX = parse[n + 34].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mColumnTopAltSheet = parse[n + 35].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mColumnCenterAltX = parse[n + 36].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mColumnCenterAltSheet = parse[n + 37].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mColumnBottomAltX = parse[n + 38].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mColumnBottomAltSheet = parse[n + 39].ToInt();

                                RDungeonManager.RDungeons[z].Floors[i].mRowLeftAltX = parse[n + 40].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mRowLeftAltSheet = parse[n + 41].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mRowCenterAltX = parse[n + 42].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mRowCenterAltSheet = parse[n + 43].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mRowRightAltX = parse[n + 44].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mRowRightAltSheet = parse[n + 45].ToInt();

                                n += 46;

                                RDungeonManager.RDungeons[z].Floors[i].mWaterX = parse[n].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mWaterSheet = parse[n + 1].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mWaterAnimX = parse[n + 2].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mWaterAnimSheet = parse[n + 3].ToInt();

                                RDungeonManager.RDungeons[z].Floors[i].mShoreTopLeftX = parse[n + 4].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreTopLeftSheet = parse[n + 5].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreTopRightX = parse[n + 6].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreTopRightSheet = parse[n + 7].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreBottomRightX = parse[n + 8].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreBottomRightSheet = parse[n + 9].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreBottomLeftX = parse[n + 10].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreBottomLeftSheet = parse[n + 11].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreDiagonalForwardX = parse[n + 12].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreDiagonalForwardSheet = parse[n + 13].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreDiagonalBackX = parse[n + 14].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreDiagonalBackSheet = parse[n + 15].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreTopX = parse[n + 16].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreTopSheet = parse[n + 17].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreRightX = parse[n + 18].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreRightSheet = parse[n + 19].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreBottomX = parse[n + 20].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreBottomSheet = parse[n + 21].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreLeftX = parse[n + 22].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreLeftSheet = parse[n + 23].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreVerticalX = parse[n + 24].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreVerticalSheet = parse[n + 25].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreHorizontalX = parse[n + 26].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreHorizontalSheet = parse[n + 27].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreInnerTopLeftX = parse[n + 28].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreInnerTopLeftSheet = parse[n + 29].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreInnerTopRightX = parse[n + 30].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreInnerTopRightSheet = parse[n + 31].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreInnerBottomRightX = parse[n + 32].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreInnerBottomRightSheet = parse[n + 33].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreInnerBottomLeftX = parse[n + 34].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreInnerBottomLeftSheet = parse[n + 35].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreInnerTopX = parse[n + 36].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreInnerTopSheet = parse[n + 37].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreInnerRightX = parse[n + 38].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreInnerRightSheet = parse[n + 39].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreInnerBottomX = parse[n + 40].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreInnerBottomSheet = parse[n + 41].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreInnerLeftX = parse[n + 42].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreInnerLeftSheet = parse[n + 43].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreSurroundedX = parse[n + 44].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreSurroundedSheet = parse[n + 45].ToInt();

                                RDungeonManager.RDungeons[z].Floors[i].mShoreTopLeftAnimX = parse[n + 46].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreTopLeftAnimSheet = parse[n + 47].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreTopRightAnimX = parse[n + 48].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreTopRightAnimSheet = parse[n + 49].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreBottomRightAnimX = parse[n + 50].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreBottomRightAnimSheet = parse[n + 51].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreBottomLeftAnimX = parse[n + 52].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreBottomLeftAnimSheet = parse[n + 53].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreDiagonalForwardAnimX = parse[n + 54].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreDiagonalForwardAnimSheet = parse[n + 55].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreDiagonalBackAnimX = parse[n + 56].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreDiagonalBackAnimSheet = parse[n + 57].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreTopAnimX = parse[n + 58].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreTopAnimSheet = parse[n + 59].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreRightAnimX = parse[n + 60].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreRightAnimSheet = parse[n + 61].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreBottomAnimX = parse[n + 62].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreBottomAnimSheet = parse[n + 63].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreLeftAnimX = parse[n + 64].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreLeftAnimSheet = parse[n + 65].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreVerticalAnimX = parse[n + 66].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreVerticalAnimSheet = parse[n + 67].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreHorizontalAnimX = parse[n + 68].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreHorizontalAnimSheet = parse[n + 69].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreInnerTopLeftAnimX = parse[n + 70].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreInnerTopLeftAnimSheet = parse[n + 71].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreInnerTopRightAnimX = parse[n + 72].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreInnerTopRightAnimSheet = parse[n + 73].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreInnerBottomRightAnimX = parse[n + 74].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreInnerBottomRightAnimSheet = parse[n + 75].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreInnerBottomLeftAnimX = parse[n + 76].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreInnerBottomLeftAnimSheet = parse[n + 77].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreInnerTopAnimX = parse[n + 78].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreInnerTopAnimSheet = parse[n + 79].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreInnerRightAnimX = parse[n + 80].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreInnerRightAnimSheet = parse[n + 81].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreInnerBottomAnimX = parse[n + 82].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreInnerBottomAnimSheet = parse[n + 83].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreInnerLeftAnimX = parse[n + 84].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreInnerLeftAnimSheet = parse[n + 85].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreSurroundedAnimX = parse[n + 86].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].mShoreSurroundedAnimSheet = parse[n + 87].ToInt();

                                n += 88;

                                RDungeonManager.RDungeons[z].Floors[i].GroundTile.Type = (Enums.TileType)parse[n].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].GroundTile.Data1 = parse[n + 1].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].GroundTile.Data2 = parse[n + 2].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].GroundTile.Data3 = parse[n + 3].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].GroundTile.String1 = parse[n + 4];
                                RDungeonManager.RDungeons[z].Floors[i].GroundTile.String2 = parse[n + 5];
                                RDungeonManager.RDungeons[z].Floors[i].GroundTile.String3 = parse[n + 6];

                                RDungeonManager.RDungeons[z].Floors[i].HallTile.Type = (Enums.TileType)parse[n + 7].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].HallTile.Data1 = parse[n + 8].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].HallTile.Data2 = parse[n + 9].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].HallTile.Data3 = parse[n + 10].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].HallTile.String1 = parse[n + 11];
                                RDungeonManager.RDungeons[z].Floors[i].HallTile.String2 = parse[n + 12];
                                RDungeonManager.RDungeons[z].Floors[i].HallTile.String3 = parse[n + 13];

                                RDungeonManager.RDungeons[z].Floors[i].WaterTile.Type = (Enums.TileType)parse[n + 14].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].WaterTile.Data1 = parse[n + 15].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].WaterTile.Data2 = parse[n + 16].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].WaterTile.Data3 = parse[n + 17].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].WaterTile.String1 = parse[n + 18];
                                RDungeonManager.RDungeons[z].Floors[i].WaterTile.String2 = parse[n + 19];
                                RDungeonManager.RDungeons[z].Floors[i].WaterTile.String3 = parse[n + 20];

                                RDungeonManager.RDungeons[z].Floors[i].WallTile.Type = (Enums.TileType)parse[n + 21].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].WallTile.Data1 = parse[n + 22].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].WallTile.Data2 = parse[n + 23].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].WallTile.Data3 = parse[n + 24].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].WallTile.String1 = parse[n + 25];
                                RDungeonManager.RDungeons[z].Floors[i].WallTile.String2 = parse[n + 26];
                                RDungeonManager.RDungeons[z].Floors[i].WallTile.String3 = parse[n + 27];

                                RDungeonManager.RDungeons[z].Floors[i].NpcSpawnTime = parse[n + 28].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].NpcMin = parse[n + 29].ToInt();
                                RDungeonManager.RDungeons[z].Floors[i].NpcMax = parse[n + 30].ToInt();

                                n += 31;

                                RDungeonManager.RDungeons[z].Floors[i].Items.Clear();
                                for (int item = 0; item < parse[n].ToInt(); item++) {
                                    RDungeonItem newItem = new RDungeonItem();
                                    newItem.ItemNum = parse[n + item * 10 + 1].ToInt();
                                    newItem.MinAmount = parse[n + item * 10 + 2].ToInt();
                                    newItem.MaxAmount = parse[n + item * 10 + 3].ToInt();
                                    newItem.AppearanceRate = parse[n + item * 10 + 4].ToInt();
                                    newItem.StickyRate = parse[n + item * 10 + 5].ToInt();
                                    newItem.Tag = parse[n + item * 10 + 6];
                                    newItem.Hidden = parse[n + item * 10 + 7].ToBool();
                                    newItem.OnGround = parse[n + item * 10 + 8].ToBool();
                                    newItem.OnWater = parse[n + item * 10 + 9].ToBool();
                                    newItem.OnWall = parse[n + item * 10 + 10].ToBool();

                                    RDungeonManager.RDungeons[z].Floors[i].Items.Add(newItem);
                                }
                                n += RDungeonManager.RDungeons[z].Floors[i].Items.Count * 10 + 1;

                                RDungeonManager.RDungeons[z].Floors[i].Npcs.Clear();
                                for (int npc = 0; npc < parse[n].ToInt(); npc++) {
                                    MapNpcPreset newNpc = new MapNpcPreset();
                                    newNpc.NpcNum = parse[n + npc * 7 + 1].ToInt();
                                    newNpc.MinLevel = parse[n + npc * 7 + 2].ToInt();
                                    newNpc.MaxLevel = parse[n + npc * 7 + 3].ToInt();
                                    newNpc.AppearanceRate = parse[n + npc * 7 + 4].ToInt();
                                    newNpc.StartStatus = (Enums.StatusAilment)parse[n + npc * 7 + 5].ToInt();
                                    newNpc.StartStatusCounter = parse[n + npc * 7 + 6].ToInt();
                                    newNpc.StartStatusChance = parse[n + npc * 7 + 7].ToInt();

                                    RDungeonManager.RDungeons[z].Floors[i].Npcs.Add(newNpc);
                                }
                                n += RDungeonManager.RDungeons[z].Floors[i].Npcs.Count * 7 + 1;

                                RDungeonManager.RDungeons[z].Floors[i].SpecialTiles.Clear();
                                for (int traps = 0; traps < parse[n].ToInt(); traps++) {
                                    RDungeonTrap newTile = new RDungeonTrap();
                                    newTile.Type = (Enums.TileType)parse[n + traps * 29 + 1].ToInt();
                                    newTile.Data1 = parse[n + traps * 29 + 2].ToInt();
                                    newTile.Data2 = parse[n + traps * 29 + 3].ToInt();
                                    newTile.Data3 = parse[n + traps * 29 + 4].ToInt();
                                    newTile.String1 = parse[n + traps * 29 + 5];
                                    newTile.String2 = parse[n + traps * 29 + 6];
                                    newTile.String3 = parse[n + traps * 29 + 7];
                                    newTile.Ground = parse[n + traps * 29 + 8].ToInt();
                                    newTile.GroundSet = parse[n + traps * 29 + 9].ToInt();
                                    newTile.GroundAnim = parse[n + traps * 29 + 10].ToInt();
                                    newTile.GroundAnimSet = parse[n + traps * 29 + 11].ToInt();
                                    newTile.Mask = parse[n + traps * 29 + 12].ToInt();
                                    newTile.MaskSet = parse[n + traps * 29 + 13].ToInt();
                                    newTile.Anim = parse[n + traps * 29 + 14].ToInt();
                                    newTile.AnimSet = parse[n + traps * 29 + 15].ToInt();
                                    newTile.Mask2 = parse[n + traps * 29 + 16].ToInt();
                                    newTile.Mask2Set = parse[n + traps * 29 + 17].ToInt();
                                    newTile.M2Anim = parse[n + traps * 29 + 18].ToInt();
                                    newTile.M2AnimSet = parse[n + traps * 29 + 19].ToInt();
                                    newTile.Fringe = parse[n + traps * 29 + 20].ToInt();
                                    newTile.FringeSet = parse[n + traps * 29 + 21].ToInt();
                                    newTile.FAnim = parse[n + traps * 29 + 22].ToInt();
                                    newTile.FAnimSet = parse[n + traps * 29 + 23].ToInt();
                                    newTile.Fringe2 = parse[n + traps * 29 + 24].ToInt();
                                    newTile.Fringe2Set = parse[n + traps * 29 + 25].ToInt();
                                    newTile.F2Anim = parse[n + traps * 29 + 26].ToInt();
                                    newTile.F2AnimSet = parse[n + traps * 29 + 27].ToInt();
                                    newTile.RDungeonMapValue = parse[n + traps * 29 + 28].ToInt();

                                    newTile.AppearanceRate = parse[n + traps * 29 + 29].ToInt();

                                    RDungeonManager.RDungeons[z].Floors[i].SpecialTiles.Add(newTile);

                                }
                                n += RDungeonManager.RDungeons[z].Floors[i].SpecialTiles.Count * 29 + 1;

                                RDungeonManager.RDungeons[z].Floors[i].Weather.Clear();
                                for (int weather = 0; weather < parse[n].ToInt(); weather++) {
                                    RDungeonManager.RDungeons[z].Floors[i].Weather.Add((Enums.Weather)parse[n + 1 + weather].ToInt());

                                }
                                n += RDungeonManager.RDungeons[z].Floors[i].Weather.Count + 1;

                                RDungeonManager.RDungeons[z].Floors[i].Options.Chambers.Clear();
                                for (int chamber = 0; chamber < parse[n].ToInt(); chamber++) {
                                    RDungeonPresetChamber newChamber = new RDungeonPresetChamber();
                                    newChamber.ChamberNum = parse[n + chamber * 4 + 1].ToInt();
                                    newChamber.String1 = parse[n + chamber * 4 + 2];
                                    newChamber.String2 = parse[n + chamber * 4 + 3];
                                    newChamber.String3 = parse[n + chamber * 4 + 4];

                                    RDungeonManager.RDungeons[z].Floors[i].Options.Chambers.Add(newChamber);
                                }
                                n += RDungeonManager.RDungeons[z].Floors[i].Options.Chambers.Count * 4 + 1;

                            }
                            Messenger.SendUpdateRDungeonToAll(z);
                            RDungeonManager.SaveRDungeon(z);

                        }
                        break;
                    #endregion
                    #region Maps
                    case "mapreportrequest": {
                            // Prevent hacking
                            if (Ranks.IsDisallowed(client, Enums.Rank.Mapper)) {
                                Messenger.HackingAttempt(client, "Admin Cloning");
                                return;
                            }
                            TcpPacket packet = new TcpPacket("mapreport");
                            packet.AppendParameter(MapManager.StandardMapCount);
                            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Data)) {
                                for (int i = 1; i <= MapManager.StandardMapCount; i++) {
                                    //packet.AppendParameter(MapManager.Maps[i].Name);
                                    packet.AppendParameter(DataManager.Maps.MapDataManager.GetMapName(dbConnection.Database, MapManager.GenerateMapID(i)));
                                }
                            }

                            Messenger.SendDataTo(client, packet);
                        }
                        break;
                    case "requesteditmap":
                        if (Ranks.IsDisallowed(client, Enums.Rank.Mapper)) {
                            Messenger.HackingAttempt(client, "Admin cloning");
                            return;
                        }
                        if (client.Player.Map.MapType == Enums.MapType.Instanced) {
                            int mapBase = ((InstancedMap)client.Player.Map).MapBase;
                            if (mapBase > 0) {
                                client.Player.BeginUninstancedWarp();
                                Messenger.PlayerWarp(client, mapBase, client.Player.X, client.Player.Y);
                                client.Player.EndUninstancedWarp();
                                Messenger.PlayerMsg(client, "You have been warped to the original map to edit it!", Text.BrightBlue);
                                return;
                            }
                        }
                        if (client.Player.Map.MapType != Enums.MapType.Standard && client.Player.Map.MapType != Enums.MapType.House) {
                            Messenger.PlayerMsg(client, "You can't edit a non-standard map!", Text.BrightRed);
                            return;
                        }
                        client.Player.InMapEditor = true;
                        Messenger.SendDataTo(client, TcpPacket.CreatePacket("editmap"));
                        Messenger.SendMapLatestPropertiesTo(client);
                        break;
                    case "requestedithouse": {
                            if (client.Player.Map.MapType == Enums.MapType.House) {
                                House house = client.Player.Map as House;
                                if (house.OwnerID != client.Player.CharID) {
                                    Messenger.PlayerMsg(client, "This is not your house!", Text.BrightRed);
                                    return;
                                }
                                Messenger.SendDataTo(client, TcpPacket.CreatePacket("edithouse"));
                            } else {
                                Messenger.PlayerMsg(client, "This is not a house!", Text.BrightRed);
                            }
                        }
                        break;
                    case "mapdata": {
                            if (Ranks.IsDisallowed(client, Enums.Rank.Mapper)) {
                                if (client.Player.Map.MapType == Enums.MapType.House) {
                                    if (((House)client.Player.Map).OwnerID != client.Player.CharID) {
                                        return;
                                    }
                                } else {
                                    return;
                                }
                            }

                            if (client.Player.Map.MapType != Enums.MapType.Standard && client.Player.Map.MapType != Enums.MapType.House) {
                                Messenger.PlayerMsg(client, "You can't edit a non-standard map!", Text.BrightRed);
                                return;
                            }

                            IMap map = null;
                            string mapID = "s0";
                            int n = 0;
                            int X = 0;
                            int Y = 0;
                            n = 1;

                            mapID = client.Player.MapID;
                            if (mapID != parse[n]) {
                                Messenger.PlayerMsg(client, "Error Saving: Incorrect Map [Error 3]", Text.BrightRed);
                                return;
                            }
                            if (client.Player.Map.MapType == Enums.MapType.Standard) {
                                map = new Map(new DataManager.Maps.Map(mapID));
                            } else if (client.Player.Map.MapType == Enums.MapType.House) {
                                House houseMap = client.Player.Map as House;

                                DataManager.Maps.HouseMap newHouseMap = new DataManager.Maps.HouseMap(MapManager.GenerateHouseID(houseMap.OwnerID, houseMap.Room));
                                newHouseMap.Owner = houseMap.OwnerID;
                                newHouseMap.Room = houseMap.Room;
                                newHouseMap.StartX = ((House)client.Player.Map).StartX;
                                newHouseMap.StartY = ((House)client.Player.Map).StartY;
                                map = new House(newHouseMap);

                            }
                            map.IsSaving = true;

                            int revision = MapManager.RetrieveActiveMap(mapID).Revision;
                            map.Name = parse[n + 1];
                            map.Revision = revision + 1;
                            map.Moral = (Enums.MapMoral)parse[n + 3].ToInt();
                            map.Up = parse[n + 4].ToInt();
                            map.Down = parse[n + 5].ToInt();
                            map.Left = parse[n + 6].ToInt();
                            map.Right = parse[n + 7].ToInt();
                            map.Music = parse[n + 8];
                            map.Indoors = parse[n + 9].ToBool();
                            map.Weather = (Enums.Weather)parse[n + 10].ToInt();
                            map.MaxX = parse[n + 11].ToInt();
                            map.MaxY = parse[n + 12].ToInt();
                            map.OriginalDarkness = parse[n + 13].ToInt();
                            map.HungerEnabled = parse[n + 14].ToBool();
                            map.RecruitEnabled = parse[n + 15].ToBool();
                            map.ExpEnabled = parse[n + 16].ToBool();
                            map.TimeLimit = parse[n + 17].ToInt();
                            map.MinNpcs = parse[n + 18].ToInt();
                            map.MaxNpcs = parse[n + 19].ToInt();
                            map.NpcSpawnTime = parse[n + 20].ToInt();

                            if (map.MapType == Enums.MapType.Standard) {
                                ((Map)map).Instanced = parse[n + 21].ToBool();
                            }

                            map.Tile = new TileCollection(map.BaseMap, map.MaxX, map.MaxY);

                            n += 22;

                            for (Y = 0; Y <= map.MaxY; Y++) {
                                for (X = 0; X <= map.MaxX; X++) {
                                    map.Tile[X, Y].Ground = parse[n].ToInt();
                                    map.Tile[X, Y].GroundAnim = parse[n + 1].ToInt();
                                    map.Tile[X, Y].Mask = parse[n + 2].ToInt();
                                    map.Tile[X, Y].Anim = parse[n + 3].ToInt();
                                    map.Tile[X, Y].Mask2 = parse[n + 4].ToInt();
                                    map.Tile[X, Y].M2Anim = parse[n + 5].ToInt();
                                    map.Tile[X, Y].Fringe = parse[n + 6].ToInt();
                                    map.Tile[X, Y].FAnim = parse[n + 7].ToInt();
                                    map.Tile[X, Y].Fringe2 = parse[n + 8].ToInt();
                                    map.Tile[X, Y].F2Anim = parse[n + 9].ToInt();
                                    map.Tile[X, Y].Type = (Enums.TileType)parse[n + 10].ToInt();
                                    map.Tile[X, Y].Data1 = parse[n + 11].ToInt();
                                    map.Tile[X, Y].Data2 = parse[n + 12].ToInt();
                                    map.Tile[X, Y].Data3 = parse[n + 13].ToInt();
                                    map.Tile[X, Y].String1 = parse[n + 14];
                                    map.Tile[X, Y].String2 = parse[n + 15];
                                    map.Tile[X, Y].String3 = parse[n + 16];
                                    map.Tile[X, Y].RDungeonMapValue = parse[n + 17].ToInt();
                                    map.Tile[X, Y].GroundSet = parse[n + 18].ToInt();
                                    map.Tile[X, Y].GroundAnimSet = parse[n + 19].ToInt();
                                    map.Tile[X, Y].MaskSet = parse[n + 20].ToInt();
                                    map.Tile[X, Y].AnimSet = parse[n + 21].ToInt();
                                    map.Tile[X, Y].Mask2Set = parse[n + 22].ToInt();
                                    map.Tile[X, Y].M2AnimSet = parse[n + 23].ToInt();
                                    map.Tile[X, Y].FringeSet = parse[n + 24].ToInt();
                                    map.Tile[X, Y].FAnimSet = parse[n + 25].ToInt();
                                    map.Tile[X, Y].Fringe2Set = parse[n + 26].ToInt();
                                    map.Tile[X, Y].F2AnimSet = parse[n + 27].ToInt();
                                    n += 28;
                                }
                            }

                            int npcCount = parse[n].ToInt();
                            n++;


                            for (X = 0; X < npcCount; X++) {
                                MapNpcPreset newNpc = new MapNpcPreset();
                                newNpc.NpcNum = parse[n].ToInt();
                                newNpc.SpawnX = parse[n + 1].ToInt();
                                newNpc.SpawnY = parse[n + 2].ToInt();
                                newNpc.MinLevel = parse[n + 3].ToInt();
                                newNpc.MaxLevel = parse[n + 4].ToInt();
                                newNpc.AppearanceRate = parse[n + 5].ToInt();
                                newNpc.StartStatus = (Enums.StatusAilment)parse[n + 6].ToInt();
                                newNpc.StartStatusCounter = parse[n + 7].ToInt();
                                newNpc.StartStatusChance = parse[n + 8].ToInt();

                                map.Npc.Add(newNpc);
                                n += 9;
                            }

                            for (int i = 0; i < Constants.MAX_MAP_NPCS; i++) {
                                map.ClearActiveNpc(i);
                            }

                            // Clear out it all
                            for (int i = 0; i < Constants.MAX_MAP_ITEMS; i++) {
                                map.SpawnItemSlot(i, -1, 0, false, false, "", map.ActiveItem[i].X, map.ActiveItem[i].Y, null);
                                map.ClearActiveItem(i);
                            }

                            map.RemakePlayersList();
                            MapManager.UpdateActiveMap(mapID, map);
                            //MapManager.Maps[mapID] = map;
                            // Save the map
                            map.Save();
                            map.IsSaving = false;
                            // Respawn
                            map.SpawnItems();
                            map.SpawnNpcs();

                            // Refresh map for everyone online
                            foreach (Client i in map.GetClients()) {
                                if (i.IsPlaying()) {
                                    if (Ranks.IsAllowed(i, Enums.Rank.Mapper)) {
                                        if (map.MapType == Enums.MapType.Standard) {
                                            Map standardMap = map as Map;
                                            if (standardMap != null) {
                                                Messenger.SendMapNameUpdate(i, standardMap.MapNum, standardMap.Name);
                                            }
                                        }
                                    }
                                    if (i.Player.MapID == map.MapID) {
                                        i.Player.InvalidateCachedMap();
                                        Messenger.SendCheckForMap(i);
                                    }
                                }
                            }
                        }
                        break;
                    case "scriptedtileinforequest": {
                            Messenger.SendScriptedTileInfoTo(client, (string)Scripting.ScriptManager.InvokeFunction("ScriptedTileInfo", client, parse[1].ToInt()));
                        }
                        break;
                    case "scriptedsigninforequest": {
                            Messenger.SendScriptedSignInfoTo(client, (string)Scripting.ScriptManager.InvokeFunction("ScriptedSignInfo", client, parse[1].ToInt()));
                        }
                        break;
                    case "mobilityinforequest": {
                            Messenger.SendMobilityInfoTo(client, (string)Scripting.ScriptManager.InvokeFunction("MobilityInfo", client, parse[1].ToInt()));
                        }
                        break;
                    #endregion
                    #region Live Map Editor
                    case "exitmapeditor": {
                            client.Player.InMapEditor = false;
                        }
                        break;
                    case "mapeditortileplaced": {
                            if (Ranks.IsDisallowed(client, Enums.Rank.Mapper)) {
                                if (client.Player.Map.MapType == Enums.MapType.House) {
                                    if (((House)client.Player.Map).OwnerID != client.Player.CharID) {
                                        return;
                                    }
                                } else {
                                    return;
                                }
                            }

                            if (client.Player.Map.MapType != Enums.MapType.Standard && client.Player.Map.MapType != Enums.MapType.House) {
                                Messenger.PlayerMsg(client, "You can't edit a non-standard map!", Text.BrightRed);
                                return;
                            }

                            int x = parse[1].ToInt();
                            int y = parse[2].ToInt();
                            int layer = parse[3].ToInt();
                            int set = parse[4].ToInt();
                            int tile = parse[5].ToInt();

                            IMap map = client.Player.Map;
                            foreach (Client mapper in map.GetClients()) {
                                // We only want to check the mappers
                                if (Ranks.IsAllowed(mapper, Enums.Rank.Mapper)) {
                                    if (mapper != client) {
                                        // We're on the same map...
                                        if (mapper.Player.InMapEditor) {
                                            // We have the map editor open...
                                            Messenger.SendLiveMapEditorTilePlacedData(mapper, x, y, layer, set, tile);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case "mapeditorattribplaced": {
                            if (Ranks.IsDisallowed(client, Enums.Rank.Mapper)) {
                                if (client.Player.Map.MapType == Enums.MapType.House) {
                                    if (((House)client.Player.Map).OwnerID != client.Player.CharID) {
                                        return;
                                    }
                                } else {
                                    return;
                                }
                            }

                            if (client.Player.Map.MapType != Enums.MapType.Standard && client.Player.Map.MapType != Enums.MapType.House) {
                                Messenger.PlayerMsg(client, "You can't edit a non-standard map!", Text.BrightRed);
                                return;
                            }

                            int x = parse[1].ToInt();
                            int y = parse[2].ToInt();
                            int type = parse[3].ToInt();
                            int data1 = parse[4].ToInt();
                            int data2 = parse[5].ToInt();
                            int data3 = parse[6].ToInt();
                            string string1 = parse[7];
                            string string2 = parse[8];
                            string string3 = parse[9];
                            int dungeonValue = parse[10].ToInt();

                            IMap map = client.Player.Map;
                            foreach (Client mapper in map.GetClients()) {
                                // We only want to check the mappers
                                if (Ranks.IsAllowed(mapper, Enums.Rank.Mapper)) {
                                    if (mapper != client) {
                                        // We're on the same map...
                                        if (mapper.Player.InMapEditor) {
                                            // We have the map editor open...
                                            Messenger.SendLiveMapEditorAttributePlacedData(mapper, x, y, type, data1, data2, data3,
                                                string1, string2, string3, dungeonValue);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case "mapeditorfilllayer": {
                            if (Ranks.IsDisallowed(client, Enums.Rank.Mapper)) {
                                if (client.Player.Map.MapType == Enums.MapType.House) {
                                    if (((House)client.Player.Map).OwnerID != client.Player.CharID) {
                                        return;
                                    }
                                } else {
                                    return;
                                }
                            }

                            if (client.Player.Map.MapType != Enums.MapType.Standard && client.Player.Map.MapType != Enums.MapType.House) {
                                Messenger.PlayerMsg(client, "You can't edit a non-standard map!", Text.BrightRed);
                                return;
                            }

                            int layer = parse[1].ToInt();
                            int set = parse[2].ToInt();
                            int tile = parse[3].ToInt();

                            IMap map = client.Player.Map;
                            foreach (Client mapper in map.GetClients()) {
                                // We only want to check the mappers
                                if (Ranks.IsAllowed(mapper, Enums.Rank.Mapper)) {
                                    if (mapper != client) {
                                        // We're on the same map...
                                        if (mapper.Player.InMapEditor) {
                                            // We have the map editor open...
                                            Messenger.SendLiveMapEditorFillLayerData(mapper, layer, set, tile);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case "mapeditorfillattribute": {
                            if (Ranks.IsDisallowed(client, Enums.Rank.Mapper)) {
                                if (client.Player.Map.MapType == Enums.MapType.House) {
                                    if (((House)client.Player.Map).OwnerID != client.Player.CharID) {
                                        return;
                                    }
                                } else {
                                    return;
                                }
                            }

                            if (client.Player.Map.MapType != Enums.MapType.Standard && client.Player.Map.MapType != Enums.MapType.House) {
                                Messenger.PlayerMsg(client, "You can't edit a non-standard map!", Text.BrightRed);
                                return;
                            }

                            int type = parse[1].ToInt();
                            int data1 = parse[2].ToInt();
                            int data2 = parse[3].ToInt();
                            int data3 = parse[4].ToInt();
                            string string1 = parse[5];
                            string string2 = parse[6];
                            string string3 = parse[7];
                            int dungeonValue = parse[8].ToInt();

                            IMap map = client.Player.Map;
                            foreach (Client mapper in map.GetClients()) {
                                // We only want to check the mappers
                                if (Ranks.IsAllowed(mapper, Enums.Rank.Mapper)) {
                                    if (mapper != client) {
                                        // We're on the same map...
                                        if (mapper.Player.InMapEditor) {
                                            // We have the map editor open...
                                            Messenger.SendLiveMapEditorFillAttributeData(mapper, type, data1, data2, data3,
                                                string1, string2, string3, dungeonValue);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    #endregion
                    #region Items
                    case "requestedititem":
                        if (Ranks.IsDisallowed(client, Enums.Rank.Developer)) {
                            Messenger.HackingAttempt(client, "Admin cloning");
                            return;
                        }
                        Messenger.SendItemEditor(client);
                        break;
                    //case "edititem": { ~unneeded due to the info already being clientside
                    //        int n = -1;
                    //        if (Ranks.IsDisallowed(client, Enums.Rank.Developer)) {
                    //            Messenger.HackingAttempt(client, "Admin cloning");
                    //            return;
                    //        }

                    //        n = parse[1].ToInt();

                    //        if (n < 0 || n > Items.ItemManager.Items.MaxItems) {
                    //            Messenger.HackingAttempt(client, "Invalid item client");
                    //            return;
                    //        }

                    // record in log here
                    //        Messenger.SendEditItemTo(client, n);
                    //    }
                    //    break;
                    case "saveitem": {
                            int n = -1;
                            if (Ranks.IsDisallowed(client, Enums.Rank.Developer)) {
                                Messenger.HackingAttempt(client, "Admin cloning");
                                return;
                            }

                            n = parse[1].ToInt();

                            if (n < 0 || n >= Items.ItemManager.Items.MaxItems) {
                                Messenger.HackingAttempt(client, "Admin cloning");
                                return;
                            }

                            ItemManager.Items[n].Name = parse[2];
                            ItemManager.Items[n].Desc = parse[3];
                            ItemManager.Items[n].Pic = parse[4].ToInt();
                            ItemManager.Items[n].Type = (Enums.ItemType)parse[5].ToInt();
                            ItemManager.Items[n].Data1 = parse[6].ToInt();
                            ItemManager.Items[n].Data2 = parse[7].ToInt();
                            ItemManager.Items[n].Data3 = parse[8].ToInt();
                            ItemManager.Items[n].Price = parse[9].ToInt();
                            ItemManager.Items[n].StackCap = parse[10].ToInt();
                            ItemManager.Items[n].Bound = parse[11].ToBool();
                            ItemManager.Items[n].Loseable = parse[12].ToBool();
                            ItemManager.Items[n].Rarity = parse[13].ToInt();
                            ItemManager.Items[n].ReqData1 = parse[14].ToInt();
                            ItemManager.Items[n].ReqData2 = parse[15].ToInt();
                            ItemManager.Items[n].ReqData3 = parse[16].ToInt();
                            ItemManager.Items[n].ReqData4 = parse[17].ToInt();
                            ItemManager.Items[n].ReqData5 = parse[18].ToInt();
                            ItemManager.Items[n].ScriptedReq = parse[19].ToInt();

                            ItemManager.Items[n].AddHP = parse[20].ToInt();
                            ItemManager.Items[n].AddPP = parse[21].ToInt();
                            ItemManager.Items[n].AddAttack = parse[22].ToInt();
                            ItemManager.Items[n].AddDefense = parse[23].ToInt();
                            ItemManager.Items[n].AddSpAtk = parse[24].ToInt();
                            ItemManager.Items[n].AddSpDef = parse[25].ToInt();
                            ItemManager.Items[n].AddSpeed = parse[26].ToInt();
                            ItemManager.Items[n].AddEXP = parse[27].ToInt();
                            ItemManager.Items[n].AttackSpeed = parse[28].ToInt();
                            ItemManager.Items[n].RecruitBonus = parse[29].ToInt();

                            Messenger.SendUpdateItemToAll(n);
                            ItemManager.SaveItem(n);
                        }
                        break;
                    #endregion
                    #region Stories
                    case "requesteditstory":
                        if (Ranks.IsDisallowed(client, Enums.Rank.Mapper)) {
                            Messenger.HackingAttempt(client, "Admin cloning");
                            return;
                        }

                        Messenger.SendDataTo(client, TcpPacket.CreatePacket("storyeditor"));
                        break;
                    case "savestory": {
                            if (Ranks.IsDisallowed(client, Enums.Rank.Mapper)) {
                                Messenger.HackingAttempt(client, "Admin cloning");
                                return;
                            }

                            int n = parse[1].ToInt();

                            if (n < 0 || n > Stories.StoryManager.Stories.MaxStories) {
                                Messenger.HackingAttempt(client, "Invalid story client");
                                return;
                            }
                            Story story = new Story(StoryManager.Stories[n].ID);
                            story.Name = parse[2];
                            story.StoryStart = parse[3].ToInt();
                            story.Revision = StoryManager.Stories[n].Revision + 1;

                            int totalSegments = parse[4].ToInt();
                            int z = 5;
                            for (int i = 0; i < totalSegments; i++) {
                                story.Segments.Add(new StorySegment());
                                int totalParameters = parse[z].ToInt();
                                story.Segments[i].Action = (Enums.StoryAction)parse[z + 1].ToInt();
                                z += 2;
                                for (int a = 0; a < totalParameters; a++) {
                                    story.Segments[i].AddParameter(parse[z], parse[z + 1]);
                                    z += 2;
                                }
                            }
                            int exitContinueCount = parse[z].ToInt();
                            z++;
                            for (int i = 0; i < exitContinueCount; i++) {
                                story.ExitAndContinue.Add(parse[z].ToInt(1));
                                z++;
                            }
                            StoryManager.Stories.Stories[n] = story;
                            Messenger.SendUpdateStoryNameToAll(n);
                            StoryManager.SaveStory(n);
                        }
                        break;
                    case "editstory": {
                            if (Ranks.IsDisallowed(client, Enums.Rank.Mapper)) {
                                Messenger.HackingAttempt(client, "Admin cloning");
                                return;
                            }
                            int n = parse[1].ToInt();
                            if (n < 0 || n > Stories.StoryManager.Stories.MaxStories) {
                                Messenger.HackingAttempt(client, "Invalid story client");
                                return;
                            }
                            Messenger.SendEditStoryTo(client, n);
                        }
                        break;
                    case "requeststory":
                        Messenger.SendUpdateStoryTo(client, parse[1].ToInt());
                        break;
                    #endregion
                    #region Evolutions
                    case "requesteditevo":
                        // Prevent hacking
                        if (Ranks.IsDisallowed(client, Enums.Rank.Developer)) {
                            Messenger.HackingAttempt(client, "Admin Cloning");
                            return;
                        }

                        Messenger.SendDataTo(client, TcpPacket.CreatePacket("evoeditor"));
                        break;
                    case "editevo": {

                            // Prevent hacking
                            if (Ranks.IsDisallowed(client, Enums.Rank.Developer)) {
                                Messenger.HackingAttempt(client, "Admin Cloning");
                                return;
                            }

                            // The evo #
                            int n = parse[1].ToInt(-1);

                            // Prevent hacking
                            if (n < 0 || n > Evolutions.EvolutionManager.Evolutions.MaxEvos) {
                                Messenger.HackingAttempt(client, "Invalid Evolution Index");
                                return;
                            }

                            Messenger.SendEditEvoTo(client, n);
                        }
                        break;
                    case "saveevo": {
                            // Prevent hacking
                            if (Ranks.IsDisallowed(client, Enums.Rank.Developer)) {
                                Messenger.HackingAttempt(client, "Admin Cloning");
                                return;
                            }

                            int n = parse[1].ToInt(-1);

                            // Prevent hacking
                            if (n < 0 | n > Evolutions.EvolutionManager.Evolutions.MaxEvos) {
                                Messenger.HackingAttempt(client, "Invalid Evolution Index");
                                return;
                            }
                            Evolution evo = new Evolution();


                            evo.Name = parse[2];
                            evo.Species = parse[3].ToInt();
                            int z = 5;
                            for (int i = 0; i < parse[4].ToInt(); i++) {
                                evo.Branches.Add(new EvolutionBranch());
                                evo.Branches[i].Name = parse[z];
                                evo.Branches[i].NewSpecies = parse[z + 1].ToInt();
                                evo.Branches[i].ReqScript = parse[z + 2].ToInt();
                                evo.Branches[i].Data1 = parse[z + 3].ToInt();
                                evo.Branches[i].Data2 = parse[z + 4].ToInt();
                                evo.Branches[i].Data3 = parse[z + 5].ToInt();
                                z += 6;
                            }
                            EvolutionManager.Evolutions[n] = evo;

                            Messenger.SendUpdateEvoToAll(n);
                            Evolutions.EvolutionManager.SaveEvo(n);
                        }
                        break;
                    #endregion
                    #region NPCs
                    case "requesteditnpc":

                        // Prevent hacking
                        if (Ranks.IsDisallowed(client, Enums.Rank.Developer)) {
                            Messenger.HackingAttempt(client, "Admin Cloning");
                            return;
                        }

                        Messenger.SendDataTo(client, TcpPacket.CreatePacket("NPCEDITOR"));
                        break;
                    // :::::::::::::::::::::
                    // :: Edit npc packet ::
                    // :::::::::::::::::::::
                    case "editnpc": {

                            // Prevent hacking
                            if (Ranks.IsDisallowed(client, Enums.Rank.Developer)) {
                                Messenger.HackingAttempt(client, "Admin Cloning");
                                return;
                            }

                            // The npc #
                            int n = parse[1].ToInt(-1);

                            // Prevent hacking
                            if (n < 0 | n > Npcs.NpcManager.Npcs.MaxNpcs) {
                                Messenger.HackingAttempt(client, "Invalid NPC Index");
                                return;
                            }

                            Messenger.SendNpcAiTypes(client);
                            Messenger.SendEditNpcTo(client, n);
                        }
                        break;
                    case "addnewnpc": {
                            if (Ranks.IsDisallowed(client, Enums.Rank.Developer)) {
                                Messenger.HackingAttempt(client, "Admin cloning");
                                return;
                            }
                            Messenger.SendAddNpcToAll(NpcManager.AddNpc());
                        }
                        break;
                    // :::::::::::::::::::::
                    // :: Save npc packet ::
                    // :::::::::::::::::::::
                    case "savenpc": {
                            // Prevent hacking
                            if (Ranks.IsDisallowed(client, Enums.Rank.Developer)) {
                                Messenger.HackingAttempt(client, "Admin Cloning");
                                return;
                            }

                            int n = parse[1].ToInt(-1);

                            // Prevent hacking
                            if (n < 0 | n > Npcs.NpcManager.Npcs.MaxNpcs) {
                                Messenger.HackingAttempt(client, "Invalid NPC Index");
                                return;
                            }

                            Npc npc = new Npc();

                            // Update the npc
                            npc.Name = parse[2];
                            npc.AttackSay = parse[3];
                            npc.Form = parse[4].ToInt();
                            npc.Species = parse[5].ToInt();
                            npc.ShinyChance = parse[6].ToInt();
                            npc.Behavior = (Enums.NpcBehavior)parse[7].ToInt();
                            npc.RecruitRate = parse[8].ToInt();
                            npc.AIScript = parse[9];
                            npc.SpawnsAtDawn = parse[10].ToBool();
                            npc.SpawnsAtDay = parse[11].ToBool();
                            npc.SpawnsAtDusk = parse[12].ToBool();
                            npc.SpawnsAtNight = parse[13].ToBool();

                            int z = 14;
                            // Load npc moves
                            for (int i = 0; i < npc.Moves.Length; i++) {
                                npc.Moves[i] = parse[z].ToInt();

                                z += 1;
                            }
                            // Load npc drops
                            for (int i = 0; i < npc.Drops.Length; i++) {
                                npc.Drops[i] = new NpcDrop();
                                npc.Drops[i].ItemNum = parse[z].ToInt();
                                npc.Drops[i].ItemValue = parse[z + 1].ToInt();
                                npc.Drops[i].Chance = parse[z + 2].ToInt();
                                npc.Drops[i].Tag = parse[z + 3];

                                z += 4;
                            }
                            NpcManager.Npcs[n] = npc;
                            // Save it
                            Messenger.SendUpdateNpcToAll(n);
                            NpcManager.SaveNpc(n);
                        }
                        break;
                    #endregion
                    #region Shops
                    case "requesteditshop":
                        // Prevent hacking
                        if (Ranks.IsDisallowed(client, Enums.Rank.Developer)) {
                            Messenger.HackingAttempt(client, "Admin Cloning");
                            return;
                        }

                        Messenger.SendDataTo(client, TcpPacket.CreatePacket("shopeditor"));
                        break;
                    // ::::::::::::::::::::::
                    // :: Edit shop packet ::
                    // ::::::::::::::::::::::
                    case "editshop": {
                            // Prevent hacking
                            if (Ranks.IsDisallowed(client, Enums.Rank.Developer)) {
                                Messenger.HackingAttempt(client, "Admin Cloning");
                                return;
                            }

                            // The shop #
                            int n = parse[1].ToInt(-1);
                            // Prevent hacking
                            if (n < 0 || n > Shops.ShopManager.Shops.MaxShops) {
                                Messenger.HackingAttempt(client, "Invalid Shop Index");
                                return;
                            }

                            Messenger.SendEditShopTo(client, n);
                        }
                        break;
                    // ::::::::::::::::::::::
                    // :: Save shop packet ::
                    // ::::::::::::::::::::::
                    case "saveshop": {
                            // Prevent hacking
                            if (Ranks.IsDisallowed(client, Enums.Rank.Developer)) {
                                Messenger.HackingAttempt(client, "Admin Cloning");
                                return;
                            }

                            int ShopNum = parse[1].ToInt(-1);
                            // Prevent hacking
                            if (ShopNum < 0 || ShopNum > Shops.ShopManager.Shops.MaxShops) {
                                Messenger.HackingAttempt(client, "Invalid Shop Index");
                                return;
                            }

                            Shop shop = new Shop();

                            // Update the shop
                            shop.Name = parse[2];
                            shop.JoinSay = parse[3];
                            shop.LeaveSay = parse[4];
                            //shop.FixesItems = parse[5].ToBool(); ;
                            int n = 5;

                            for (int i = 0; i < Constants.MAX_TRADES; i++) {
                                shop.Items[i].GiveItem = parse[n].ToInt();
                                shop.Items[i].GiveValue = parse[n + 1].ToInt();
                                shop.Items[i].GetItem = parse[n + 2].ToInt();
                                //shop.Sections[z].Items[i].GetValue = parse[n + 3].ToInt();
                                n = n + 3;
                            }


                            ShopManager.Shops[ShopNum] = shop;
                            // Save it
                            Messenger.SendUpdateShopToAll(ShopNum);
                            ShopManager.SaveShop(ShopNum);
                        }
                        break;
                    #endregion
                    #region Moves
                    case "requesteditmove":
                        // Prevent hacking
                        if (Ranks.IsDisallowed(client, Enums.Rank.Developer)) {
                            Messenger.HackingAttempt(client, "Admin Cloning");
                            return;
                        }

                        Messenger.SendDataTo(client, TcpPacket.CreatePacket("moveeditor"));
                        break;
                    // :::::::::::::::::::::::
                    // :: Edit move packet ::
                    // :::::::::::::::::::::::
                    case "editmove": {
                            // Prevent hacking
                            if (Ranks.IsDisallowed(client, Enums.Rank.Developer)) {
                                Messenger.HackingAttempt(client, "Admin Cloning");
                                return;
                            }

                            // The spell #
                            int n = parse[1].ToInt(-1);

                            // Prevent hacking
                            if (n < 0 | n > Moves.MoveManager.Moves.MaxMoves) {
                                Messenger.HackingAttempt(client, "Invalid Move Index");
                                return;
                            }

                            Messenger.SendEditMoveTo(client, n);
                        }
                        break;
                    // :::::::::::::::::::::::
                    // :: Save move packet ::
                    // :::::::::::::::::::::::
                    case "savemove": { // 
                            // Prevent hacking
                            if (Ranks.IsDisallowed(client, Enums.Rank.Developer)) {
                                Messenger.HackingAttempt(client, "Admin Cloning");
                                return;
                            }

                            // Move #
                            int n = parse[1].ToInt(-1);

                            // Prevent hacking
                            if (n < 0 | n > Moves.MoveManager.Moves.MaxMoves) {
                                Messenger.HackingAttempt(client, "Invalid Move Index");
                                return;
                            }

                            Move move = new Move();

                            // Update the spell
                            move.Name = parse[2];
                            move.MaxPP = parse[3].ToInt();
                            move.EffectType = (Enums.MoveType)parse[4].ToInt();
                            move.Element = (Enums.PokemonType)parse[5].ToInt();
                            move.MoveCategory = (Enums.MoveCategory)parse[6].ToInt();
                            move.TargetType = (Enums.MoveTarget)parse[7].ToInt();
                            move.RangeType = (Enums.MoveRange)parse[8].ToInt();
                            move.Range = parse[9].ToInt();

                            move.Data1 = parse[10].ToInt();
                            move.Data2 = parse[11].ToInt();
                            move.Data3 = parse[12].ToInt();

                            move.Accuracy = parse[13].ToInt();

                            move.HitTime = parse[14].ToInt();
                            move.HitFreeze = parse[15].ToBool();

                            move.AdditionalEffectData1 = parse[16].ToInt();
                            move.AdditionalEffectData2 = parse[17].ToInt();
                            move.AdditionalEffectData3 = parse[18].ToInt();
                            move.PerPlayer = parse[19].ToBool();
                            move.KeyItem = parse[20].ToInt();

                            move.Sound = parse[21].ToInt();

                            move.AttackerAnim.AnimationType = (Enums.MoveAnimationType)parse[22].ToInt();
                            move.AttackerAnim.AnimationIndex = parse[23].ToInt();
                            move.AttackerAnim.FrameSpeed = parse[24].ToInt();
                            move.AttackerAnim.Repetitions = parse[25].ToInt();

                            move.TravelingAnim.AnimationType = (Enums.MoveAnimationType)parse[26].ToInt();
                            move.TravelingAnim.AnimationIndex = parse[27].ToInt();
                            move.TravelingAnim.FrameSpeed = parse[28].ToInt();
                            move.TravelingAnim.Repetitions = parse[29].ToInt();

                            move.DefenderAnim.AnimationType = (Enums.MoveAnimationType)parse[30].ToInt();
                            move.DefenderAnim.AnimationIndex = parse[31].ToInt();
                            move.DefenderAnim.FrameSpeed = parse[32].ToInt();
                            move.DefenderAnim.Repetitions = parse[33].ToInt();


                            MoveManager.Moves[n] = move;

                            // Save it
                            Messenger.SendUpdateMoveToAll(n);
                            MoveManager.SaveMove(n);
                        }
                        break;
                    #endregion
                    #region Arrows
                    case "requesteditarrow":
                        // Prevent hacking
                        if (Ranks.IsDisallowed(client, Enums.Rank.Developer)) {
                            Messenger.HackingAttempt(client, "Admin Cloning");
                            return;
                        }
                        Messenger.SendArrowEditor(client);
                        break;
                    case "editarrow": {
                            // Prevent hacking
                            //if (Ranks.IsDisallowed(client, Enums.Rank.Developer)) {
                            //    Messenger.HackingAttempt(client, "Admin Cloning");
                            //    return;
                            //}
                            //int n = parse[1].ToInt(-1);
                            //if (n < 0 | n > Constants.MAX_ARROWS) {
                            //    Messenger.HackingAttempt(client, "Invalid arrow Index");
                            //    return;
                            //}
                            //Messenger.SendEditArrowTo(client, n);
                        }
                        break;
                    case "savearrow": {
                            // Prevent hacking
                            //if (Ranks.IsDisallowed(client, Enums.Rank.Developer)) {
                            //    Messenger.HackingAttempt(client, "Admin Cloning");
                            //    return;
                            //}
                            //int n = parse[1].ToInt(-1);
                            //if (n < 0 | n > Constants.MAX_ARROWS) {
                            //    Messenger.HackingAttempt(client, "Invalid arrow Index");
                            //    return;
                            //}
                            //ArrowManagerBase.Arrows[n].Name = parse[2];
                            //ArrowManagerBase.Arrows[n].Pic = parse[3].ToInt();
                            //ArrowManagerBase.Arrows[n].Range = parse[4].ToInt();
                            //ArrowManagerBase.Arrows[n].Amount = parse[5].ToInt();
                            //Messenger.SendUpdateArrowToAll(n);
                            //ArrowManagerBase.SaveArrows();
                        }
                        break;
                    #endregion
                    #region Scripts
                    case "requesteditscript": {
                            if (Ranks.IsDisallowed(client, Enums.Rank.Scripter)) {
                                Messenger.HackingAttempt(client, "Admin cloning");
                                Messenger.AdminMsg("All Admins: " + client.Player.Name + " has attempted to access the script editor", Text.BrightRed);
                            } else {
                                Scripting.Editor.EditorHelper.InitTempScript(client);
                                if (parse[1].ToBool() == false) {
                                    Messenger.SendDataTo(client, TcpPacket.CreatePacket("scriptsyntax", System.IO.File.ReadAllText(IO.Paths.ScriptsFolder + "CSharp.syn")));
                                }
                                Messenger.SendDataTo(client, TcpPacket.CreatePacket("scripteditstart"));
                                Messenger.SendDataTo(client, TcpPacket.CreatePacket("scripteditdata", "Main.cs", System.IO.File.ReadAllText(IO.Paths.ScriptsFolder + "Main.cs").Replace(TcpPacket.SEP_CHAR, '/')));
                                Scripting.Editor.EditorHelper.SendScriptClasses(client);
                                TcpPacket filelistPacket = new TcpPacket("scriptfilelist");
                                Scripting.Editor.EditorHelper.AppendFileListToPacket(client, filelistPacket);
                                filelistPacket.FinalizePacket();
                                Messenger.SendDataTo(client, filelistPacket);
                                client.Player.InScriptEditor = true;
                            }
                        }
                        break;
                    case "savescript":
                        if (Ranks.IsDisallowed(client, Enums.Rank.Scripter)) {
                            Messenger.HackingAttempt(client, "Admin Cloning");
                            return;
                        } else {
                            System.IO.File.WriteAllText(Scripting.Editor.EditorHelper.GetTempFolder(client) + parse[1] + ".cs", parse[2]);
                            Messenger.PlayerMsg(client, "File Saved! (Temp)", Text.Yellow);
                        }
                        break;
                    case "finalizescript":
                        if (Ranks.IsDisallowed(client, Enums.Rank.Scripter)) {
                            Messenger.HackingAttempt(client, "Admin Cloning");
                            return;
                        } else {
                            bool testVal = Scripting.ScriptManager.TestCompile(Scripting.Editor.EditorHelper.GetTempFolder(client));
                            if (testVal) {
                                Scripting.Editor.EditorHelper.SaveScript(client);
                                Messenger.PlayerMsg(client, "Script Compiled and Saved!", Text.Yellow);
                                Scripting.Editor.EditorHelper.SendScriptErrors(client);
                            } else {
                                Messenger.PlayerMsg(client, "Error during script compilation... Script was not saved.", Text.Yellow);
                                Scripting.Editor.EditorHelper.SendScriptErrors(client);
                            }
                        }
                        break;
                    case "savetemp":
                        if (Ranks.IsDisallowed(client, Enums.Rank.Scripter)) {
                            Messenger.HackingAttempt(client, "Admin Cloning.");
                            return;
                        } else {
                            Scripting.Editor.EditorHelper.SaveTempScript(client, parse[1], parse[2]);
                        }
                        break;
                    case "compilescript":
                        if (Ranks.IsDisallowed(client, Enums.Rank.Scripter)) {
                            Messenger.HackingAttempt(client, "Admin Cloning.");
                            return;
                        } else {
                            bool testVal = Scripting.ScriptManager.TestCompile(Scripting.Editor.EditorHelper.GetTempFolder(client));
                            if (testVal) {
                                Scripting.Editor.EditorHelper.SaveScript(client);
                                Messenger.PlayerMsg(client, "Script Compiled and Saved!", Text.Yellow);
                                Scripting.Editor.EditorHelper.SendScriptErrors(client);
                            } else {
                                Messenger.PlayerMsg(client, "Error during script compilation... Script was not saved.", Text.Yellow);
                                Scripting.Editor.EditorHelper.SendScriptErrors(client);
                            }
                        }
                        break;
                    case "scripteditexit":
                        if (Ranks.IsDisallowed(client, Enums.Rank.Scripter)) {
                            return;
                        } else {
                            client.Player.InScriptEditor = false;
                            if (System.IO.Directory.Exists(Scripting.Editor.EditorHelper.GetTempFolder(client))) {
                                System.IO.Directory.Delete(Scripting.Editor.EditorHelper.GetTempFolder(client), true);
                            }
                        }
                        break;
                    case "getscriptclasses":
                        if (Ranks.IsDisallowed(client, Enums.Rank.Scripter)) {
                            Messenger.HackingAttempt(client, "Admin Cloning");
                            return;
                        } else {
                            Scripting.Editor.EditorHelper.SendScriptClasses(client);
                        }
                        break;
                    case "getscriptmethods":
                        if (Ranks.IsDisallowed(client, Enums.Rank.Scripter)) {
                            Messenger.HackingAttempt(client, "Admin Cloning");
                            return;
                        } else {
                            Scripting.Editor.EditorHelper.SendScriptMethods(client, parse[1]);
                        }
                        break;
                    case "getscriptparam":
                        if (Ranks.IsDisallowed(client, Enums.Rank.Scripter)) {
                            Messenger.HackingAttempt(client, "Admin Cloning");
                            return;
                        } else {
                            Scripting.Editor.EditorHelper.SendScriptParameters(client, parse[1], parse[2], parse[3].ToInt(1));
                        }
                        break;
                    case "requestscriptsyntax":
                        if (Ranks.IsDisallowed(client, Enums.Rank.Scripter)) {
                            Messenger.HackingAttempt(client, "Admin Cloning");
                            return;
                        } else {
                            Messenger.SendDataTo(client, TcpPacket.CreatePacket("scriptsyntax", System.IO.File.ReadAllText(IO.Paths.ScriptsFolder + "CSharp.syn")));
                        }
                        break;
                    case "requesteditscriptfile":
                        if (Ranks.IsDisallowed(client, Enums.Rank.Scripter)) {
                            Messenger.HackingAttempt(client, "Admin Cloning");
                            return;
                        } else {
                            Messenger.SendDataTo(client, TcpPacket.CreatePacket("scriptfiledata", parse[1], Scripting.Editor.EditorHelper.GetScriptFile(client, parse[1])));
                        }
                        break;
                    case "requestfilelist":
                        if (Ranks.IsDisallowed(client, Enums.Rank.Scripter)) {
                            Messenger.HackingAttempt(client, "Admin Cloning");
                            return;
                        } else {
                            TcpPacket filelistPacket = new TcpPacket("scriptfilelist");
                            Scripting.Editor.EditorHelper.AppendFileListToPacket(client, filelistPacket);
                            filelistPacket.FinalizePacket();
                            Messenger.SendDataTo(client, filelistPacket);
                        }
                        break;
                    case "addnewclass":
                        if (Ranks.IsDisallowed(client, Enums.Rank.Scripter)) {
                            Messenger.HackingAttempt(client, "Admin Cloning");
                            return;
                        } else {
                            Scripting.Editor.EditorHelper.AddNewClass(client, parse[1]);
                            TcpPacket filelistPacket = new TcpPacket("scriptfilelist");
                            Scripting.Editor.EditorHelper.AppendFileListToPacket(client, filelistPacket);
                            filelistPacket.FinalizePacket();
                            Messenger.SendDataTo(client, filelistPacket);
                        }
                        break;

                    #endregion
                    #region Dungeons
                    case "requesteditdungeon": {
                            if (Ranks.IsDisallowed(client, Enums.Rank.Mapper)) {
                                Messenger.HackingAttempt(client, "Admin cloning");
                                return;
                            }

                            Messenger.SendDataTo(client, TcpPacket.CreatePacket("dungeoneditor"));
                        }
                        break;
                    case "addnewdungeon": {
                            if (Ranks.IsDisallowed(client, Enums.Rank.Mapper)) {
                                Messenger.HackingAttempt(client, "Admin cloning");
                                return;
                            }

                            DungeonManager.AddDungeon();
                            Messenger.SendAddDungeonToAll(DungeonManager.Dungeons.Count - 1);
                        }
                        break;
                    case "savedungeon": {
                            if (Ranks.IsDisallowed(client, Enums.Rank.Mapper)) {
                                Messenger.HackingAttempt(client, "Admin cloning");
                                return;
                            }
                            int n = parse[1].ToInt();

                            if (n < 0 || n > DungeonManager.Dungeons.Count) {
                                return;
                            }

                            Dungeons.Dungeon dungeon = new Dungeon();

                            dungeon.Name = parse[2];
                            dungeon.AllowsRescue = parse[3].ToBool();

                            int scriptCount = parse[4].ToInt();

                            int z = 5;

                            for (int i = 0; i < scriptCount; i++) {
                                dungeon.ScriptList.Add(parse[z].ToInt(), parse[z + 1]);
                                z += 2;
                            }

                            int standardMapCount = parse[z].ToInt();
                            z++;

                            for (int i = 0; i < standardMapCount; i++) {
                                StandardDungeonMap map = new StandardDungeonMap();
                                map.Difficulty = (Enums.JobDifficulty)parse[z].ToInt();
                                map.IsBadGoalMap = parse[z + 1].ToBool();
                                map.MapNum = parse[z + 2].ToInt();
                                z += 3;
                                dungeon.StandardMaps.Add(map);
                            }

                            int randomMapCount = parse[z].ToInt();
                            z++;
                            for (int i = 0; i < randomMapCount; i++) {
                                RandomDungeonMap map = new RandomDungeonMap();
                                map.Difficulty = (Enums.JobDifficulty)parse[z].ToInt();
                                map.IsBadGoalMap = parse[z + 1].ToBool();
                                map.RDungeonIndex = parse[z + 2].ToInt();
                                map.RDungeonFloor = parse[z + 3].ToInt();
                                z += 4;
                                dungeon.RandomMaps.Add(map);
                            }

                            DungeonManager.Dungeons[n] = dungeon;
                            DungeonManager.SaveDungeon(n);
                            Messenger.SendUpdateDungeonToAll(n);
                        }
                        break;
                    case "editdungeon": {
                            if (Ranks.IsDisallowed(client, Enums.Rank.Mapper)) {
                                Messenger.HackingAttempt(client, "Admin cloning");
                                return;
                            }

                            int n = parse[1].ToInt();

                            if (n < 0 || n > DungeonManager.Dungeons.Count) {
                                Messenger.HackingAttempt(client, "Invalid dungeon client");
                                return;
                            }

                            Messenger.SendEditDungeonTo(client, n);
                        }
                        break;
                    #endregion
                    #region Missions
                    case "requesteditmission": {
                            if (Ranks.IsDisallowed(client, Enums.Rank.Developer)) {
                                Messenger.HackingAttempt(client, "Admin cloning");
                                return;
                            }

                            Messenger.SendDataTo(client, TcpPacket.CreatePacket("missioneditor"));
                        }
                        break;
                    case "savemission": {
                            WonderMails.MissionPool missionPool = new WonderMails.MissionPool();

                            int difficulty = parse[1].ToInt() - 1;

                            int clientCount = parse[2].ToInt();
                            int n = 3;
                            for (int i = 0; i < clientCount; i++) {
                                WonderMails.MissionClientData missionClient = new WonderMails.MissionClientData();
                                missionClient.Species = parse[n].ToInt();
                                missionClient.Form = parse[n + 1].ToInt();
                                missionPool.MissionClients.Add(missionClient);

                                n += 2;

                            }

                            int enemyCount = parse[n].ToInt();
                            n++;

                            for (int i = 0; i < enemyCount; i++) {
                                WonderMails.MissionEnemyData missionEnemy = new WonderMails.MissionEnemyData();
                                missionEnemy.NpcNum = parse[n].ToInt();
                                missionPool.Enemies.Add(missionEnemy);

                                n++;
                            }

                            int rewardCount = parse[n].ToInt();
                            n++;

                            for (int i = 0; i < rewardCount; i++) {

                                WonderMails.MissionRewardData missionReward = new WonderMails.MissionRewardData(); ;
                                missionReward.ItemNum = parse[n].ToInt();
                                missionReward.Amount = parse[n + 1].ToInt();
                                missionReward.Tag = parse[n + 2];
                                missionPool.Rewards.Add(missionReward);

                                n += 3;

                            }

                            WonderMails.WonderMailManager.Missions[difficulty] = missionPool;
                            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Data)) {
                                WonderMails.WonderMailManager.SaveMissionPool(dbConnection, difficulty);
                            }
                        }
                        break;
                    case "editmission": {
                            if (Ranks.IsDisallowed(client, Enums.Rank.Developer)) {
                                Messenger.HackingAttempt(client, "Admin cloning");
                                return;
                            }

                            int n = parse[1].ToInt();

                            if (n < 0 || n > 16) {
                                Messenger.HackingAttempt(client, "Invalid mission client");
                                return;
                            }

                            Messenger.SendEditMissionTo(client, n - 1);
                        }
                        break;
                    #endregion

                    #endregion
                    #region Online List
                    case "whosonline":
                        Messenger.SendWhosOnline(client);
                        break;
                    case "onlinelist":
                        Messenger.SendOnlineList(client);
                        break;
                    case "playerinforequest": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                string name = parse[1];
                                Client i = ClientManager.FindClient(name);
                                if (i != null) {
                                    Messenger.PlayerMsg(client, "Account: " + i.Player.AccountName.Trim() + ", " + i.Player.Name.Trim(), Text.BrightGreen);
                                    Messenger.PlayerMsg(client, "-=- Stats for " + i.Player.Name + " -=-", Text.BrightGreen);
                                    Messenger.PlayerMsg(client, "Level: " + i.Player.GetActiveRecruit().Level.ToString() + " Exp: " + i.Player.GetActiveRecruit().Exp.ToString() + "/" + i.Player.GetActiveRecruit().GetNextLevel().ToString(), Text.BrightGreen);
                                    Messenger.PlayerMsg(client, "HP: " + i.Player.GetActiveRecruit().HP.ToString() + "/" + i.Player.GetMaxHP().ToString(), Text.BrightGreen);
                                    Messenger.PlayerMsg(client, "Atk: " + i.Player.GetActiveRecruit().Atk.ToString() + "  Def: " + i.Player.GetActiveRecruit().Def.ToString() + "  Sp.Atk: " + i.Player.GetActiveRecruit().SpclAtk.ToString() + "  Sp.Def: " + i.Player.GetActiveRecruit().SpclDef.ToString() + "  Speed: " + i.Player.GetActiveRecruit().Spd.ToString(), Text.BrightGreen);
                                    Messenger.PlayerMsg(client, "Species: " + i.Player.GetActiveRecruit().Species.ToString() + " [" + Pokedex.Pokedex.GetPokemon(i.Player.GetActiveRecruit().Species).Name + "]    Sprite: " + i.Player.GetActiveRecruit().Sprite.ToString(), Text.BrightGreen);
                                } else {
                                    Messenger.PlayerMsg(client, "Player is offline.", Text.Grey);
                                }
                            }
                        }
                        break;
                    case "getstats": {
                            Messenger.PlayerMsg(client, "-=- Stats for " + client.Player.Name + " -=-", Text.BrightGreen);
                            Messenger.PlayerMsg(client, "Level: " + client.Player.GetActiveRecruit().Level.ToString() + " Exp: " + client.Player.GetActiveRecruit().Exp.ToString() + "/" + client.Player.GetActiveRecruit().GetNextLevel().ToString(), Text.BrightGreen);
                            Messenger.PlayerMsg(client, "HP: " + client.Player.GetActiveRecruit().HP.ToString() + "/" + client.Player.GetMaxHP().ToString(), Text.BrightGreen);
                            Messenger.PlayerMsg(client, "Atk: " + client.Player.GetActiveRecruit().Atk.ToString() + "  Def: " + client.Player.GetActiveRecruit().Def.ToString() + "  Sp.Str: " + client.Player.GetActiveRecruit().SpclAtk.ToString() + "  Sp.Def: " + client.Player.GetActiveRecruit().SpclDef.ToString() + "  Speed: " + client.Player.GetActiveRecruit().Spd.ToString(), Text.BrightGreen);
                            Messenger.PlayerMsg(client, "Species: " + Pokedex.Pokedex.GetPokemon(client.Player.GetActiveRecruit().Species).Name, Text.BrightGreen);
                        }
                        break;
                    #endregion
                    #region Admin Commands
                    case "setaccess": {
                            // Prevent hacking
                            if (Ranks.IsDisallowed(client, Enums.Rank.Admin)) {
                                Messenger.HackingAttempt(client, "Admin Cloning");
                                return;
                            }

                            // The client
                            Client n = ClientManager.FindClient(parse[1]);
                            // The access
                            int i = parse[2].ToInt(-1);

                            // Check for invalid access level
                            if (i >= 0 || i <= 3) {
                                if (client.Player.Name != n.Player.Name) {
                                    // Check if player is on
                                    if (n != null) {
                                        if (Ranks.IsAllowed(client, (Enums.Rank)i)) {
                                            if (Ranks.IsDisallowed(n, Enums.Rank.Moniter)) {
                                                Messenger.GlobalMsg(n.Player.Name + " has been blessed with administrative access.", Text.BrightBlue);
                                            }

                                            n.Player.Access = (Enums.Rank)i;
                                            Messenger.SendPlayerData(n);

                                            if (Ranks.IsAllowed(n, Enums.Rank.Mapper)) {
                                                n.Player.AddExpKitModule(new AvailableExpKitModule(Enums.ExpKitModules.MapReport, true));
                                            } else {
                                                if (n.Player.AvailableExpKitModules.Contains(Enums.ExpKitModules.MapReport)) {
                                                    n.Player.RemoveExpKitModule(Enums.ExpKitModules.MapReport);
                                                }
                                            }
                                        } else {
                                            Messenger.PlayerMsg(client, "You cannot set someone's access higher than yours!", Text.BrightRed);
                                        }
                                    } else {
                                        Messenger.PlayerMsg(client, "Player is not online.", Text.White);
                                    }
                                } else {
                                    Messenger.PlayerMsg(client, "You cant change your access.", Text.Red);
                                }
                            } else {
                                Messenger.PlayerMsg(client, "Invalid access level.", Text.Red);
                            }

                        }
                        break;
                    case "setmotd": {
                            // Prevent hacking
                            if (Ranks.IsDisallowed(client, Enums.Rank.Mapper)) {
                                Messenger.HackingAttempt(client, "Admin Cloning");
                                return;
                            }

                            Settings.MOTD = parse[1];
                            Messenger.GlobalMsg("MOTD changed to: " + parse[1], Text.BrightCyan);
                            Settings.SaveMOTD();
                        }
                        break;
                    case "restartserver": {
                            // Prevent hacking
                            if (Ranks.IsDisallowed(client, Enums.Rank.ServerHost)) {
                                Messenger.HackingAttempt(client, "Admin Cloning");
                                return;
                            }

                            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                                foreach (Client i in ClientManager.GetClients()) {
                                    if (i.IsPlaying()) {
                                        i.Player.SaveCharacterData(dbConnection);
                                    }
                                }
                            }
                            PlayerManager.SavingEnabled = false;

                            System.Windows.Forms.Application.Restart();
                            System.Environment.Exit(0);
                        }
                        break;
                    // TODO: Re-add updater [HIGH]
                    case "serverupdatecheck": {
                            //if (Ranks.IsDisallowed(client, Enums.Rank.ServerHost)) {
                            //    Messenger.HackingAttempt(client, "Admin cloning");
                            //    return;
                            //} else {
                            //    libupdate.Updater updater = new libupdate.Updater("http://dl.getdropbox.com/u/465620/PMU%20Updates/Server%20Updates/updateinfo.txt", libupdate.InformType.Default);
                            //    updater.DefaultUpdateForm.AutoStartInterval = 5000;
                            //    updater.DefaultUpdateForm.Text = "PMU Server Updater";
                            //    updater.OnUpdateFound += new libupdate.OnUpdateFoundEventHandler(updater_OnUpdateFound);
                            //    updater.OnNoUpdateFound += new EventHandler(updater_OnNoUpdateFound);
                            //    updater.FileDownloading += new libupdate.FileDownloadingEventHandler(updater_FileDownloading);
                            //    updater.FileDownloadComplete += new EventHandler(updater_FileDownloadComplete);
                            //    updater.UpdateStarted += new EventHandler(updater_UpdateStarted);
                            //    updater.OnException += new libupdate.ExceptionOccuredEventHandler(updater_OnException);
                            //    updater.UpdateComplete += new EventHandler(updater_UpdateComplete);
                            //    updater.DefaultUpdateForm.ErrorMode = libupdate.frmUpdater.ReportingMode.CatchSilent;
                            //    Messenger.AdminMsg("[Admin] Update check has been started...", Text.BrightBlue);
                            //    updater.InitializeUpdate();
                            //}
                        }
                        break;
                    case "setsprite": {
                            if (Ranks.IsDisallowed(client, Enums.Rank.Mapper)) {
                                Messenger.HackingAttempt(client, "Admin cloning");
                                return;
                            } else {
                                int n = parse[1].ToInt(-1);
                                if (n > -1) {
                                    client.Player.GetActiveRecruit().SetSpecies(n);
                                    Messenger.SendPlayerData(client);
                                    Messenger.SendActiveTeam(client);
                                    Messenger.SendStats(client);
                                }
                            }
                        }
                        break;
                    case "setplayersprite": {
                            if (Ranks.IsDisallowed(client, Enums.Rank.Admin)) {
                                Messenger.HackingAttempt(client, "Admin cloning");
                                return;
                            } else {
                                int n = parse[2].ToInt(-1);
                                Client player = ClientManager.FindClient(parse[1]);
                                if (n > -1 && player != null) {
                                    player.Player.GetActiveRecruit().SetSpecies(n);
                                    Messenger.SendPlayerData(player);
                                    Messenger.SendActiveTeam(player);
                                    Messenger.SendStats(player);
                                }
                            }
                        }
                        break;
                    case "maprespawn": {
                            if (Ranks.IsDisallowed(client, Enums.Rank.Mapper)) {
                                Messenger.HackingAttempt(client, "Admin cloning");
                                return;
                            } else {
                                IMap map = client.Player.GetCurrentMap();
                                for (int i = 0; i < Constants.MAX_MAP_ITEMS; i++) {
                                    map.SpawnItemSlot(i, -1, 0, false, false, "", map.ActiveItem[i].X, map.ActiveItem[i].Y, null);
                                    map.ClearActiveItem(i);
                                }
                                map.SpawnItems();

                                // Respawn NPCs
                                for (int i = 0; i < Constants.MAX_MAP_NPCS; i++) {
                                    map.ClearActiveNpc(i);
                                }
                                map.SpawnNpcs();
                                Messenger.PlayerMsg(client, "Map respawned.", Text.Blue);
                            }
                        }
                        break;
                    case "kickplayer": {
                            if (Ranks.IsDisallowed(client, Enums.Rank.Moniter)) {
                                Messenger.HackingAttempt(client, "Admin cloning");
                                return;
                            } else {
                                Client n = ClientManager.FindClient(parse[1]);
                                if (n == client) {
                                    Messenger.PlayerMsg(client, "You can't kick yourself!", Text.BrightRed);
                                } else if (n == null) {
                                    Messenger.PlayerMsg(client, "Player is offline.", Text.Grey);
                                } else if (Ranks.IsAllowed(n, client.Player.Access)) {
                                    Messenger.PlayerMsg(client, "That is a higher access admin than you!", Text.BrightRed);
                                } else {
                                    //Messenger.GlobalMsg(n.Player.Name + " has been kicked from " + Settings.GameName + " by " + client.Player.Name + "!", Text.White);
                                    //Messenger.AlertMsg(n, "You have been kicked by " + client.Player.Name + "!");
                                    Messenger.PlayerMsg(client, n.Player.Name + " has been kicked from the server!", Text.BrightGreen);
                                    Messenger.PlainMsg(n, "You have been kicked from the server!", Enums.PlainMsgType.MainMenu);
                                    n.CloseConnection();
                                }
                            }
                        }
                        break;
                    case "banplayer": {
                            if (Ranks.IsDisallowed(client, Enums.Rank.Mapper)) {
                                Messenger.HackingAttempt(client, "Admin cloning");
                                return;
                            } else {
                                Client n = ClientManager.FindClient(parse[1]);
                                if (n == client) {
                                    Messenger.PlayerMsg(client, "You can't kick yourself!", Text.BrightRed);
                                } else if (n == null) {
                                    Messenger.PlayerMsg(client, "Player is offline.", Text.Grey);
                                } else if (Ranks.IsAllowed(n, client.Player.Access)) {
                                    Messenger.PlayerMsg(client, "That is a higher access admin than you!", Text.BrightRed);
                                } else {
                                    //  Add banning to scripts
                                    //Messenger.GlobalMsg(n.Player.Name + " has been banned from " + Settings.GameName + " by " + client.Player.Name + "!", Text.White);
                                    //PlayerManager.AddToBanList(((System.Net.IPEndPoint)n.TcpClient.Socket.RemoteEndPoint).Address.ToString(), client.Player.Name, n.Player.Name);
                                    //Messenger.AlertMsg(n, "You have been kicked by " + client.Player.Name + "!");
                                }
                            }
                        }
                        break;
                    case "banlist": {
                            if (Ranks.IsDisallowed(client, Enums.Rank.Moniter)) {
                                Messenger.HackingAttempt(client, "Admin cloning");
                                return;
                            } else {
                                // TODO: Display ban list
                                //if (PlayerManager.Bans.Count > 0) {
                                //    string list = "";
                                //    for (int i = 0; i < PlayerManager.Bans.Count; i++) {
                                //        list += i.ToString() + ". IP: " + PlayerManager.Bans[i].BannedIP + ", Banned By: " + PlayerManager.Bans[i].BannedBy + ", Banned Player: " + PlayerManager.Bans[i].BannedAccount + "\r\n";
                                //    }
                                //    Messenger.PlayerMsg(client, list, Text.BrightBlue);
                                //} else {
                                //    Messenger.PlayerMsg(client, "There are no bans.", Text.BrightBlue);
                                //}
                            }
                        }
                        break;
                    // TODO: Unban all players/unban single player [MEDIUM]
                    case "banlistdestroy": {

                        }
                        break;
                    case "warpto": {
                            if (Ranks.IsDisallowed(client, Enums.Rank.Mapper)) {
                                Messenger.HackingAttempt(client, "Admin cloning");
                                return;
                            } else {
                                Messenger.PlayerWarp(client, parse[1].ToInt(), client.Player.X, client.Player.Y);
                            }
                        }
                        break;
                    case "warptome": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Mapper)) {
                                Client n = ClientManager.FindClient(parse[1]);
                                if (n != null) {
                                    Messenger.PlayerWarp(n, client.Player.Map, client.Player.X, client.Player.Y);
                                }
                            }
                        }
                        break;

                    case "warpmeto": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Mapper)) {
                                Client n = ClientManager.FindClient(parse[1]);
                                if (n != null) {
                                    Messenger.PlayerWarp(client, n.Player.Map, n.Player.X, n.Player.Y);
                                }
                            }
                        }
                        break;
                    case "warploc": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Mapper) || client.Player.Map.MapType == Enums.MapType.House && ((House)client.Player.Map).OwnerID == client.Player.CharID) {
                                Messenger.PlayerXYWarp(client, parse[1].ToInt(), parse[2].ToInt());
                            }
                        }
                        break;
                    case "arrowhit": {
                            // TODO: Easy way to damage any player. Modify arrow code to be processed server-side [HIGH]
                            int n = parse[1].ToInt();
                            int z = parse[2].ToInt();
                            int x = parse[3].ToInt();
                            int y = parse[4].ToInt();

                            // BattleProcessor.ArrowHit(client, (Enums.TargetType)n, z);
                        }
                        break;
                    case "saveplayer": {
                            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                                client.Player.SaveCharacterData(dbConnection);
                            }
                            Messenger.PlayerMsg(client, "You have saved the game.", Text.Yellow);
                        }
                        break;
                    case "solid": {
                            if (client.Player.Solid == true) {
                                client.Player.Solid = false;
                                Messenger.PlayerMsg(client, "You can run through players now.", Text.Yellow);
                            } else {
                                client.Player.Solid = true;
                                Messenger.PlayerMsg(client, "You can no longer run through players.", Text.Yellow);
                            }
                            Messenger.SendPlayerData(client);
                        }
                        break;
                    case "weather": {
                            if (Ranks.IsAllowed(client, Enums.Rank.Developer)) {
                                int weather = parse[1].ToInt(-1);
                                if (weather > -1 && weather < 13) {
                                    Globals.ServerWeather = (Enums.Weather)weather;
                                    if (Globals.ServerWeather != Enums.Weather.Ambiguous) {
                                        IMap[] mapArray = MapManager.ToArray();
                                        for (int i = 0; i < mapArray.Length; i++) {
                                            mapArray[i].Weather = Globals.ServerWeather;
                                        }
                                    }
                                    Messenger.SendWeatherToAll();
                                }
                            } else {
                                Messenger.HackingAttempt(client, "Admin Cloning [Changing Weather]");
                            }
                        }
                        break;
                    #endregion
                    #region Friends List
                    case "sendfriendslist":
                        client.Player.SendFriendsList();
                        break;
                    case "addfriend":
                        client.Player.AddFriend(parse[1]);
                        break;
                    case "removefriend":
                        client.Player.RemoveFriend(parse[1]);
                        break;
                    #endregion
                    #region Guild
                    case "guildpromote": {

                            if (client.Player.GuildAccess < Enums.GuildRank.Founder) {
                                Messenger.PlayerMsg(client, "You are not the owner of this guild!", Text.BrightRed);
                                return;
                            }



                            ListPair<string, int> members;
                            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                                members = DataManager.Players.PlayerDataManager.LoadGuild(dbConnection.Database, client.Player.GuildName);

                                if (parse[1].ToInt() > members.Count) {
                                    Messenger.PlayerMsg(client, "That player doesn't seem to be in the guild anymore.", Text.BrightRed);
                                    return;
                                }

                                int adminCount = 0;
                                for (int i = 0; i < members.Count; i++) {
                                    if (members.ValueByIndex(i) > 1) {
                                        adminCount++;
                                    }
                                }

                                if (client.Player.HasItem(1) < Guilds.GuildManager.PROMOTE_PRICE * adminCount) {
                                    Messenger.PlayerMsg(client, "You need " + Guilds.GuildManager.PROMOTE_PRICE * adminCount + " " + ItemManager.Items[1].Name + " to promote a member.", Text.BrightRed);
                                    return;
                                }

                                client.Player.TakeItem(1, Guilds.GuildManager.PROMOTE_PRICE * adminCount, true);
                                DataManager.Players.PlayerDataManager.SetGuildAccess(dbConnection.Database, members.KeyByIndex(parse[1].ToInt()), (int)Enums.GuildRank.Admin);
                                Messenger.PlayerMsg(client, "The player has been promoted.", Text.Blue);


                            }

                            Client target = ClientManager.FindClientFromCharID(members.KeyByIndex(parse[1].ToInt()));

                            if (target != null) {
                                //Set the player's New access level
                                target.Player.GuildAccess = Enums.GuildRank.Admin;
                                //send the guild access data to all
                                Messenger.SendPlayerGuild(target);
                            }
                            //send the update to all possible guild members
                            Messenger.SendFullGuildUpdate(client.Player.GuildName);
                        }
                        break;
                    case "guilddemote": {

                            if (client.Player.GuildAccess < Enums.GuildRank.Founder) {
                                Messenger.PlayerMsg(client, "You are not the owner of this guild!", Text.BrightRed);
                                return;
                            }
                            ListPair<string, int> members;
                            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                                members = DataManager.Players.PlayerDataManager.LoadGuild(dbConnection.Database, client.Player.GuildName);

                                if (parse[1].ToInt() > members.Count) {
                                    Messenger.PlayerMsg(client, "That player doesn't seem to be in the guild anymore.", Text.BrightRed);
                                    return;
                                }

                                DataManager.Players.PlayerDataManager.SetGuildAccess(dbConnection.Database, members.KeyByIndex(parse[1].ToInt()), (int)Enums.GuildRank.Member);
                                Messenger.PlayerMsg(client, "The player has been demoted.", Text.Blue);
                            }

                            Client target = ClientManager.FindClientFromCharID(members.KeyByIndex(parse[1].ToInt()));

                            if (target != null) {
                                //Set the player's New access level
                                target.Player.GuildAccess = Enums.GuildRank.Member;
                                //send the guild access data to all
                                Messenger.SendPlayerGuild(target);
                            }
                            //send the update to all possible guild members
                            Messenger.SendFullGuildUpdate(client.Player.GuildName);
                        }
                        break;
                    // Disown
                    case "guilddisown": {

                            if (client.Player.GuildAccess < Enums.GuildRank.Admin) {
                                Messenger.PlayerMsg(client, "Your guild rank isn't high enough to disown players!", Text.BrightRed);
                                return;
                            }
                            ListPair<string, int> members;
                            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                                members = DataManager.Players.PlayerDataManager.LoadGuild(dbConnection.Database, client.Player.GuildName);

                                if (parse[1].ToInt() > members.Count) {
                                    Messenger.PlayerMsg(client, "That player doesn't seem to be in the guild anymore.", Text.BrightRed);
                                    return;
                                }

                                DataManager.Players.PlayerDataManager.RemoveGuildMember(dbConnection.Database, members.KeyByIndex(parse[1].ToInt()));
                                Messenger.PlayerMsg(client, "The player has been disowned.", Text.Blue);
                            }

                            Client target = ClientManager.FindClientFromCharID(members.KeyByIndex(parse[1].ToInt()));

                            if (target != null) {
                                target.Player.GuildName = "";
                                target.Player.GuildAccess = Enums.GuildRank.None;
                                //send the guild access data to all
                                Messenger.SendPlayerGuild(target);
                            }
                            //send the update to all possible guild members
                            Messenger.SendFullGuildUpdate(client.Player.GuildName);
                        }
                        break;
                    // Leave Guild
                    case "guildleave": {
                            // Check if they can leave
                            if (string.IsNullOrEmpty(client.Player.GuildName)) {
                                Messenger.PlayerMsg(client, "You are not in a guild.", Text.BrightRed);
                                return;
                            }
                            bool stepDown = false;
                            if (client.Player.GuildAccess > Enums.GuildRank.Member) {
                                stepDown = true;
                            }

                            int index = -1;
                            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                                ListPair<string, int> members = DataManager.Players.PlayerDataManager.LoadGuild(dbConnection.Database, client.Player.GuildName);

                                index = members.IndexOfKey(client.Player.CharID);

                                int founders = 0;
                                for (int i = 0; i < members.Count; i++) {
                                    if (members.ValueByIndex(i) == (int)Enums.GuildRank.Founder) founders++;
                                }

                                if (founders == 1 && client.Player.GuildAccess == Enums.GuildRank.Founder) {
                                    Messenger.AskQuestion(client, "DisbandGuild", "As the sole founder of this guild, if you step down, the guild will be disbanded.  Is that OK?", -1);
                                } else if (stepDown) {
                                    Messenger.AskQuestion(client, "GuildStepDown", "Are you sure you want to step down from your current guild position?", -1);
                                } else {
                                    Messenger.AskQuestion(client, "GuildStepDown", "Are you sure you want to leave the guild?", -1);
                                }
                            }
                        }
                        break;
                    // Make A New Guild
                    case "makeguild": {
                            string name = parse[1].Trim();

                            Guilds.GuildManager.RegisterGuild(client, name);

                        }
                        break;
                    case "guildmember": {

                            if (client.Player.GuildAccess < Enums.GuildRank.Admin) {
                                Messenger.PlayerMsg(client, "Your guild rank isn't high enough to add players!", Text.BrightRed);
                                return;
                            }

                            //find character ID from name in database
                            string charID;
                            string guild;
                            Client target = null;
                            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                                charID = DataManager.Players.PlayerDataManager.RetrieveCharacterID(dbConnection.Database, parse[1]);

                                if (String.IsNullOrEmpty(charID)) {
                                    Messenger.PlayerMsg(client, "The player could not be found.", Text.BrightRed);
                                    return;
                                }

                                target = ClientManager.FindClientFromCharID(charID);
                                
                                if (target == null)
                                {
                                    Messenger.PlayerMsg(client, "That player isn't online!", Text.BrightRed);
                                    return;
                                }

                                if (target.Player.MapID != client.Player.MapID || target.Player.X != client.Player.X || target.Player.Y != client.Player.Y)
                                {
                                    Messenger.PlayerMsg(client, "The joining player needs to be with you on the same tile!", Text.BrightRed);
                                    return;
                                }

                                guild = DataManager.Players.PlayerDataManager.GetGuildName(dbConnection.Database, charID);

                                if (!String.IsNullOrEmpty(guild)) {
                                    Messenger.PlayerMsg(client, "That player is already in a guild!", Text.BrightRed);
                                    return;
                                }

                                ListPair<string, int> members = DataManager.Players.PlayerDataManager.LoadGuild(dbConnection.Database, client.Player.GuildName);

                                if (client.Player.HasItem(1) < Guilds.GuildManager.RECRUIT_PRICE * members.Count) {
                                    Messenger.PlayerMsg(client, "You need " + Guilds.GuildManager.RECRUIT_PRICE * members.Count + " " + ItemManager.Items[1].Name + " to add a member.", Text.BrightRed);
                                    return;
                                }

                                client.Player.TakeItem(1, Guilds.GuildManager.RECRUIT_PRICE * members.Count, true);
                                DataManager.Players.PlayerDataManager.AddGuildMember(dbConnection.Database, client.Player.GuildName, charID);
                                Messenger.PlayerMsg(client, "The player has been added to the guild!", Text.Blue);
                            }

                            if (target != null) {
                                target.Player.GuildName = client.Player.GuildName;
                                target.Player.GuildAccess = Enums.GuildRank.Member;
                                //send the guild access data to all
                                Messenger.SendPlayerGuild(target);
                            }
                            //send the update to all possible guild members
                            Messenger.SendFullGuildUpdate(client.Player.GuildName);
                        }
                        break;
                    #endregion
                    #region Items
                    case "useitem": {
                            if (client.Player.GettingMap) return;
                            int invNum = parse[1].ToInt(-1);
                            if (invNum != -1 && (invNum < 1 || invNum > client.Player.MaxInv)) {
                                Messenger.HackingAttempt(client, "Invalid InvNum");
                                return;
                            }

                            client.Player.UseItem(client.Player.Inventory[invNum], invNum);
                        }
                        break;
                    case "throwitem": {
                            if (client.Player.GettingMap) return;
                            int invNum = parse[1].ToInt(-1);
                            if (invNum != -1 && (invNum < 1 || invNum > client.Player.MaxInv)) {
                                Messenger.HackingAttempt(client, "Invalid InvNum");
                                return;
                            }
                            client.Player.ThrowItem(client.Player.Inventory[parse[1].ToInt(-1)], parse[1].ToInt(-1));
                        }
                        break;
                    case "holditem": {
                            client.Player.HoldItem(parse[1].ToInt(-1));
                        }
                        break;
                    case "removeitem": {
                            client.Player.RemoveItem(parse[1].ToInt(-1));
                        }
                        break;
                    case "swapinvitems": {
                            int oldInvSlot = parse[1].ToInt(-1);
                            int newInvSlot = parse[2].ToInt(-1);
                            if (oldInvSlot > 0 && oldInvSlot <= client.Player.MaxInv && newInvSlot > 0 && newInvSlot <= client.Player.MaxInv) {
                                InventoryItem oldItem = client.Player.Inventory[oldInvSlot];
                                InventoryItem newItem = client.Player.Inventory[newInvSlot];

                                oldItem.Updated = true;
                                newItem.Updated = true;

                                client.Player.Inventory[oldInvSlot] = newItem;
                                client.Player.Inventory[newInvSlot] = oldItem;

                                int[] teamHeldItems = new int[client.Player.Team.Length];
                                bool heldItemsUpdated = false;
                                for (int i = 0; i < client.Player.Team.Length; i++) {
                                    if (client.Player.Team[i].Loaded) {
                                        teamHeldItems[i] = client.Player.Team[i].HeldItemSlot;
                                    }
                                }
                                for (int i = 0; i < teamHeldItems.Length; i++) {
                                    if (teamHeldItems[i] == oldInvSlot) {
                                        client.Player.Team[i].HeldItemSlot = newInvSlot;
                                        heldItemsUpdated = true;
                                    } else if (teamHeldItems[i] == newInvSlot) {
                                        client.Player.Team[i].HeldItemSlot = oldInvSlot;
                                        heldItemsUpdated = true;
                                    }
                                }

                                PacketHitList hitlist = null;
                                PacketHitList.MethodStart(ref hitlist);

                                Messenger.SendInventoryUpdate(client, oldInvSlot, hitlist);
                                Messenger.SendInventoryUpdate(client, newInvSlot, hitlist);
                                if (heldItemsUpdated) {
                                    PacketBuilder.AppendWornEquipment(client, hitlist);
                                }

                                PacketHitList.MethodEnded(ref hitlist);
                            }
                        }
                        break;
                    #endregion
                    #region Shops
                    case "shoprequest": {
                            if (client.Player.Map.Tile[client.Player.X, client.Player.Y].Type == Enums.TileType.Shop) {
                                if (client.Player.Map.Tile[client.Player.X, client.Player.Y].Data1 > 0) {
                                    Messenger.SendTrade(client, client.Player.Map.Tile[client.Player.X, client.Player.Y].Data1);
                                } else {
                                    Messenger.PlayerMsg(client, "There is no shop here.", Text.BrightRed);
                                }
                            }
                        }
                        break;
                    case "shopleave": {
                            if (client.Player.Map.Tile[client.Player.X, client.Player.Y].Type == Enums.TileType.Shop && client.Player.Map.Tile[client.Player.X, client.Player.Y].Data1 > 0) {
                                int shopNum = client.Player.Map.Tile[client.Player.X, client.Player.Y].Data1;
                                if (Shops.ShopManager.Shops[shopNum].LeaveSay.Trim() != "") {
                                    Messenger.PlayerMsg(client, Shops.ShopManager.Shops[shopNum].LeaveSay.Trim(), Text.Yellow);
                                }
                            }
                        }
                        break;
                    case "traderequest": {
                            // Trade num
                            int n = parse[1].ToInt();
                            int z = parse[2].ToInt();



                            // Prevent hacking
                            if ((z < 0) || (z > Constants.MAX_TRADES)) {
                                Messenger.HackingAttempt(client, "Trade Request Modification (index)");
                                return;
                            }
                            if (client.Player.Map.Tile[client.Player.X, client.Player.Y].Type != Enums.TileType.Shop) {
                                Messenger.HackingAttempt(client, "Trade Request Modification (tile)");
                                return;
                            }

                            // Index for shop
                            int i = client.Player.GetCurrentMap().Tile[client.Player.X, client.Player.Y].Data1;

                            // Check if inv full
                            if (i <= 0) return;
                            int X = client.Player.FindInvSlot(ShopManager.Shops[i].Items[z].GetItem, n);
                            if (X == -1) {
                                Messenger.PlayerMsg(client, "Trade unsuccessful, inventory full.", Text.BrightRed);
                                return;
                            }
                            if (n == 0) {
                                if (ItemManager.Items[ShopManager.Shops[i].Items[z].GetItem].StackCap > 0) {
                                    Messenger.PlayerMsg(client, "You must buy more than 0!", Text.BrightRed);
                                    return;
                                } else {
                                    n = 1;
                                }

                            }

                            if (ItemManager.Items[ShopManager.Shops[i].Items[z].GetItem].Rarity > (int)client.Player.ExplorerRank + 1) {
                                Messenger.PlayerMsg(client, "Your Explorer Rank is not high enough to receive this item!", Text.BrightRed);
                                return;
                            }


                            // Check if they have the item
                            if (client.Player.HasItem(ShopManager.Shops[i].Items[z].GiveItem) >= ShopManager.Shops[i].Items[z].GiveValue * n) {
                                client.Player.TakeItem(ShopManager.Shops[i].Items[z].GiveItem, ShopManager.Shops[i].Items[z].GiveValue * n);
                                client.Player.GiveItem(ShopManager.Shops[i].Items[z].GetItem, n);
                                Messenger.PlayerMsg(client, "The trade was successful!", Text.Yellow);
                                Messenger.SendInventory(client);
                            } else {
                                Messenger.PlayerMsg(client, "Trade unsuccessful.", Text.BrightRed);
                            }
                        }
                        break;
                    case "sellitem": {//sell multiples
                            int sellItemNum = parse[2].ToInt();
                            int SellItemAmount = parse[1].ToInt();

                            // Prevent hacking
                            if ((sellItemNum < 0) || (sellItemNum >= ItemManager.Items.MaxItems)) {
                                Messenger.HackingAttempt(client, "Trade Request Modification");
                                return;
                            }
                            if (client.Player.Map.Tile[client.Player.X, client.Player.Y].Type != Enums.TileType.Shop) {
                                Messenger.HackingAttempt(client, "Trade Request Modification");
                                return;
                            }
                            if (ItemManager.Items[sellItemNum].Price <= 0) {
                                Messenger.PlayerMsg(client, "You can't sell that!", Text.BrightRed);
                                break;
                            }

                            if (SellItemAmount <= 0) {
                                if (ItemManager.Items[sellItemNum].StackCap > 0) {
                                    Messenger.PlayerMsg(client, "You must sell more than 0!", Text.BrightRed);
                                    break;
                                } else {
                                    SellItemAmount = 1;
                                }
                            }
                            if (client.Player.HasItem(sellItemNum) > 0) {
                                if (client.Player.HasItem(sellItemNum) < SellItemAmount) {
                                    Messenger.PlayerMsg(client, "You can't sell more than you have!", Text.BrightRed);
                                    break;
                                }
                                client.Player.TakeItem(sellItemNum, SellItemAmount);
                                client.Player.GiveItem(1, ItemManager.Items[sellItemNum].Price * SellItemAmount);
                                Messenger.SendInventory(client);
                                Messenger.PlayerMsg(client, "You have sold " + SellItemAmount + " " + Items.ItemManager.Items[sellItemNum].Name + "!", Text.BrightGreen);
                                //Messenger.SendDataTo(client, TcpPacket.CreatePacket("updatesell"));
                            }
                        }
                        break;
                    case "moverecall": {
                            int recallMove = parse[1].ToInt();
                            // Prevent hacking
                            if ((recallMove <= 0) || (recallMove > MoveManager.Moves.MaxMoves)) {
                                Messenger.HackingAttempt(client, "Move Recall Out of Range :" + recallMove);
                                return;
                            }
                            if (client.Player.Map.Tile[client.Player.X, client.Player.Y].Type != Enums.TileType.LinkShop) {
                                return;
                            }

                            int priceItem = client.Player.Map.Tile[client.Player.X, client.Player.Y].Data1;
                            int priceAmount = client.Player.Map.Tile[client.Player.X, client.Player.Y].Data2;
                            bool earlierEvo = (client.Player.Map.Tile[client.Player.X, client.Player.Y].Data3 == 1);
                            if (Items.ItemManager.Items[priceItem].StackCap <= 0 && Items.ItemManager.Items[priceItem].Type != Enums.ItemType.Currency) {
                                priceAmount = 1;
                            }

                            for (int i = 0; i < 4; i++) {
                                if (client.Player.GetActiveRecruit().Moves[i].MoveNum == recallMove) {
                                    Messenger.PlayerMsg(client, "You already know this move.", Text.BrightRed);
                                    return;
                                }
                            }

                            if (earlierEvo) {
                                int species = EvolutionManager.FindPreEvolution(client.Player.GetActiveRecruit().Species);

                                while (species > -1) {

                                    Pokedex.PokemonForm pokemon = Pokedex.Pokedex.GetPokemonForm(species, client.Player.GetActiveRecruit().Form);
                                    if (pokemon != null) {
                                        for (int n = 0; n < pokemon.LevelUpMoves.Count; n++) {
                                            if (pokemon.LevelUpMoves[n].Level <= client.Player.GetActiveRecruit().Level) {
                                                if (pokemon.LevelUpMoves[n].Move == recallMove) {
                                                    if (client.Player.HasItem(priceItem) >= priceAmount) {
                                                        client.Player.TakeItem(priceItem, priceAmount);
                                                        client.Player.GetActiveRecruit().LearnNewMove(recallMove);
                                                        Messenger.SendPlayerMoves(client);
                                                    } else {
                                                        Messenger.PlayerMsg(client, "You do not have enough to recall a move!", Text.BrightRed);
                                                    }
                                                    return;
                                                }
                                            }
                                        }
                                    }

                                    species = EvolutionManager.FindPreEvolution(species);
                                }
                            } else {
                                Pokedex.PokemonForm pokemon = Pokedex.Pokedex.GetPokemonForm(client.Player.GetActiveRecruit().Species, client.Player.GetActiveRecruit().Form);
                                if (pokemon != null) {
                                    for (int n = 0; n < pokemon.LevelUpMoves.Count; n++) {
                                        if (pokemon.LevelUpMoves[n].Level <= client.Player.GetActiveRecruit().Level && pokemon.LevelUpMoves[n].Move == recallMove) {

                                            if (client.Player.HasItem(priceItem) >= priceAmount) {
                                                client.Player.TakeItem(priceItem, priceAmount);
                                                client.Player.GetActiveRecruit().LearnNewMove(recallMove);
                                                Messenger.SendPlayerMoves(client);
                                            } else {
                                                Messenger.PlayerMsg(client, "You do not have enough to recall a move!", Text.BrightRed);
                                            }
                                            return;

                                        }
                                    }

                                }
                            }

                        }
                        break;
                    #endregion
                    #region Bank
                    case "bankdeposit": {
                            if (client.Player.Map.Tile[client.Player.X, client.Player.Y].Type == Enums.TileType.Bank) {
                                int slot = parse[1].ToInt();
                                int amount = parse[2].ToInt();
                                int X = client.Player.Inventory[slot].Num;
                                string tag = client.Player.Inventory[slot].Tag;
                                int i = client.Player.FindBankSlot(X, amount);
                                if (i == -1) {
                                    Messenger.StorageMessage(client, "The storage is full!");
                                    return;
                                }

                                if (amount > client.Player.Inventory[slot].Amount) {
                                    Messenger.StorageMessage(client, "You cant deposit more than you have!");
                                    return;
                                }

                                if (ItemManager.Items[X].Type == Enums.ItemType.Currency || ItemManager.Items[X].StackCap > 0) {//handle clientside
                                    if (amount <= 0) {
                                        Messenger.StorageMessage(client, "You must deposit more than 0!");
                                        return;
                                    }
                                }

                                client.Player.TakeItemSlot(slot, amount, true);
                                client.Player.GiveBankItem(X, amount, tag, i);
                            } else {
                                Messenger.HackingAttempt(client, "Player not in Storage");
                            }
                            //Messenger.SendBank(client);
                        }
                        break;
                    case "bankwithdraw": {
                            if (client.Player.Map.Tile[client.Player.X, client.Player.Y].Type == Enums.TileType.Bank) {
                                int slot = parse[1].ToInt();
                                int i = client.Player.Bank[parse[1].ToInt()].Num;
                                string tag = client.Player.Bank[parse[1].ToInt()].Tag;
                                int TempVal = parse[2].ToInt();
                                int X = client.Player.FindInvSlot(i, TempVal);
                                if (X == -1) {
                                    Messenger.StorageMessage(client, "There's no more room in your bag!");
                                    return;
                                }

                                if (TempVal > client.Player.Bank[slot].Amount) {
                                    Messenger.StorageMessage(client, "You cant withdraw more than you have!");
                                    return;
                                }

                                if (ItemManager.Items[i].Type == Enums.ItemType.Currency || ItemManager.Items[i].StackCap > 0) {//handle clientside
                                    if (TempVal <= 0) {
                                        Messenger.StorageMessage(client, "You must withdraw more than 0!");
                                        return;
                                    }
                                }

                                client.Player.GiveItem(i, TempVal, tag, false);
                                client.Player.TakeBankItem(slot, TempVal);
                            } else {
                                Messenger.HackingAttempt(client, "Player not in Storage");
                            }
                            //Messenger.SendBank(client);
                        }
                        break;
                    case "bankwithdrawmenu": {
                            Messenger.SendBank(client);
                        }
                        break;
                    #endregion
                    #region Custom Menus
                    case "picclick":
                    case "lblclick":
                    case "txtclick":
                    case "menuclosed":
                        client.Player.CustomMenuManager.ProcessTCP(parse);
                        break;
                    #endregion
                    #region Housing
                    case "housevisitrequest": {
                            string ownerName = parse[1];
                            // Now, find the owner ID using the char list...
                            string ownerID;
                            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                                ownerID = PlayerManager.RetrieveCharacterID(dbConnection, ownerName);
                            }
                            if (!string.IsNullOrEmpty(ownerID)) {
                                Messenger.PlayerWarpToHouse(client, ownerID, 0);
                            } else {
                                Messenger.PlayerMsg(client, "There is no house owned by a player of that name.", Text.BrightRed);
                            }
                        }
                        break;
                    case "addshoprequest": {
                            IMap map = client.Player.Map;
                            if (map.MapType != Server.Enums.MapType.House || ((House)map).OwnerID != client.Player.CharID) {
                                Messenger.PlayerMsg(client, "You can't place a shop tile here.", Text.BrightRed);
                                return;
                            }
                            int price = parse[1].ToInt();
                            if (client.Player.HasItem(1) < price / House.SHOP_PRICE)
                            {
                                Messenger.PlayerMsg(client, "You need " + price / House.SHOP_PRICE + " " + ItemManager.Items[1].Name + " to place a shop tile.", Text.BrightRed);
                                return;
                            }
                            client.Player.TakeItem(1, price / House.SHOP_PRICE, true);

                            map.Tile[client.Player.X, client.Player.Y].Type = Enums.TileType.DropShop;
                            map.Tile[client.Player.X, client.Player.Y].Data1 = price;
                            map.Tile[client.Player.X, client.Player.Y].Data2 = 0;
                            map.Tile[client.Player.X, client.Player.Y].Data3 = 0;
                            map.Tile[client.Player.X, client.Player.Y].String1 = client.Player.CharID;
                            map.Tile[client.Player.X, client.Player.Y].String2 = "";
                            map.Tile[client.Player.X, client.Player.Y].String3 = "";


                            Messenger.PlayerMsg(client, "A shop tile has been placed!  You can now sell items for " + parse[1].ToInt() + " " + ItemManager.Items[1].Name + " by dropping your items on the tile.", Text.BrightGreen);

                            Messenger.RefreshMap(client);
                        }
                        break;
                    case "addnoticerequest": {
                            IMap map = client.Player.Map;
                            if (map.MapType != Server.Enums.MapType.House || ((House)map).OwnerID != client.Player.CharID) {
                                Messenger.PlayerMsg(client, "You can't place a special tile here.", Text.BrightRed);
                                return;
                            }

                            int totalLength = parse[1].Length + parse[2].Length;
                            if (client.Player.HasItem(1) < House.NOTICE_PRICE) {
                                Messenger.PlayerMsg(client, "You need " + (House.NOTICE_PRICE + House.WORD_PRICE * totalLength) + " " + ItemManager.Items[1].Name + " to place a special tile.", Text.BrightRed);
                                return;
                            }
                            client.Player.TakeItem(1, House.NOTICE_PRICE + House.WORD_PRICE * totalLength, true);

                            map.Tile[client.Player.X, client.Player.Y].Type = Enums.TileType.Notice;
                            map.Tile[client.Player.X, client.Player.Y].Data1 = 0;
                            map.Tile[client.Player.X, client.Player.Y].Data2 = 0;
                            map.Tile[client.Player.X, client.Player.Y].Data3 = 0;
                            map.Tile[client.Player.X, client.Player.Y].String1 = parse[1];
                            map.Tile[client.Player.X, client.Player.Y].String2 = parse[2];
                            map.Tile[client.Player.X, client.Player.Y].String3 = parse[3];

                            //Messenger.SendTile(client.Player.X, client.Player.Y, client.Player.Map);
                            Messenger.PlayerMsg(client, "A notice tile has been placed!  Step on it to read what it says.", Text.BrightGreen);

                            Messenger.RefreshMap(client);
                        }
                        break;
                    case "addsoundrequest": {
                            IMap map = client.Player.Map;
                            if (map.MapType != Server.Enums.MapType.House || ((House)map).OwnerID != client.Player.CharID) {
                                Messenger.PlayerMsg(client, "You can't place a special tile here.", Text.BrightRed);
                                return;
                            }

                            if (client.Player.HasItem(1) < House.SOUND_PRICE) {
                                Messenger.PlayerMsg(client, "You need " + House.SOUND_PRICE + " " + ItemManager.Items[1].Name + " to place a special tile.", Text.BrightRed);
                                return;
                            }
                            client.Player.TakeItem(1, House.SOUND_PRICE, true);

                            map.Tile[client.Player.X, client.Player.Y].Type = Enums.TileType.Sound;
                            map.Tile[client.Player.X, client.Player.Y].Data1 = 0;
                            map.Tile[client.Player.X, client.Player.Y].Data2 = 0;
                            map.Tile[client.Player.X, client.Player.Y].Data3 = 0;
                            map.Tile[client.Player.X, client.Player.Y].String1 = parse[1];
                            map.Tile[client.Player.X, client.Player.Y].String2 = "";
                            map.Tile[client.Player.X, client.Player.Y].String3 = "";

                            //Messenger.SendTile(client.Player.X, client.Player.Y, client.Player.Map);
                            Messenger.PlayerMsg(client, "A sound tile has been placed!  Step on it to play a sound.", Text.BrightGreen);


                            Messenger.RefreshMap(client);
                        }
                        break;
                    case "addsignrequest": {

                            IMap map = client.Player.Map;
                            if (map.MapType != Server.Enums.MapType.House || ((House)map).OwnerID != client.Player.CharID) {
                                Messenger.PlayerMsg(client, "You can't place a special tile here.", Text.BrightRed);
                                return;
                            }
                            int totalLength = parse[1].Length + parse[2].Length + parse[3].Length;
                            if (client.Player.HasItem(1) < House.WORD_PRICE * totalLength) {
                                Messenger.PlayerMsg(client, "You need " + House.WORD_PRICE * totalLength + " " + ItemManager.Items[1].Name + " to place a special tile.", Text.BrightRed);
                                return;
                            }
                            client.Player.TakeItem(1, House.WORD_PRICE * totalLength, true);

                            map.Tile[client.Player.X, client.Player.Y].Type = Enums.TileType.Sign;
                            map.Tile[client.Player.X, client.Player.Y].Data1 = 0;
                            map.Tile[client.Player.X, client.Player.Y].Data2 = 0;
                            map.Tile[client.Player.X, client.Player.Y].Data3 = 0;
                            map.Tile[client.Player.X, client.Player.Y].String1 = parse[1];
                            map.Tile[client.Player.X, client.Player.Y].String2 = parse[2];
                            map.Tile[client.Player.X, client.Player.Y].String3 = parse[3];

                            //Messenger.SendTile(client.Player.X, client.Player.Y, client.Player.Map);
                            Messenger.PlayerMsg(client, "A sign tile has been placed!  Press Enter to read signs.", Text.BrightGreen);


                            Messenger.RefreshMap(client);
                        }
                        break;
                    case "weatherrequest": {

                            IMap map = client.Player.Map;
                            if (map.MapType != Server.Enums.MapType.House || ((House)map).OwnerID != client.Player.CharID) {
                                Messenger.PlayerMsg(client, "You can't change the weather here.", Text.BrightRed);
                                return;
                            }

                            if (client.Player.HasItem(1) < House.WEATHER_PRICE) {
                                Messenger.PlayerMsg(client, "You need " + House.WEATHER_PRICE + " " + ItemManager.Items[1].Name + " to change the weather.", Text.BrightRed);
                                return;
                            }
                            client.Player.TakeItem(1, House.WEATHER_PRICE, true);

                            map.Weather = (Enums.Weather)parse[1].ToInt();

                            //Messenger.SendTile(client.Player.X, client.Player.Y, client.Player.Map);
                            Messenger.PlayerMsg(client, "The house's weather has been changed!", Text.BrightGreen);


                            Messenger.RefreshMap(client);
                        }
                        break;
                    case "darknessrequest": {
                            IMap map = client.Player.Map;
                            if (map.MapType != Server.Enums.MapType.House || ((House)map).OwnerID != client.Player.CharID) {
                                Messenger.PlayerMsg(client, "You can't change the lights here.", Text.BrightRed);
                                return;
                            }

                            if (client.Player.HasItem(1) < House.LIGHT_PRICE) {
                                Messenger.PlayerMsg(client, "You need " + House.LIGHT_PRICE + " " + ItemManager.Items[1].Name + " to change the lights.", Text.BrightRed);
                                return;
                            }
                            client.Player.TakeItem(1, House.LIGHT_PRICE, true);

                            map.Darkness = parse[1].ToInt();

                            //Messenger.SendTile(client.Player.X, client.Player.Y, client.Player.Map);
                            Messenger.PlayerMsg(client, "The house's lights have been changed!", Text.BrightGreen);

                            Messenger.RefreshMap(client);
                        }
                        break;
                    case "expansionrequest": {
                            IMap map = client.Player.Map;
                            if (map.MapType != Server.Enums.MapType.House || ((House)map).OwnerID != client.Player.CharID) {
                                Messenger.PlayerMsg(client, "You can't expand your house here.", Text.BrightRed);
                                return;
                            }
                            int x = parse[1].ToInt();
                            int y = parse[2].ToInt();
                            int total = ((x + 1) * (y + 1) - (map.MaxX + 1) * (map.MaxY + 1)) * House.TILE_PRICE;
                            if (total > 0) {
                                if (client.Player.HasItem(1) < total) {
                                    Messenger.PlayerMsg(client, "You need " + total + " " + ItemManager.Items[1].Name + " to expand your house.", Text.BrightRed);
                                    return;
                                }
                                client.Player.TakeItem(1, total, true);
                            }

                            if (client.Player.X > x) client.Player.X = x;
                            if (client.Player.Y > y) client.Player.Y = y;
                            int oldX = map.MaxX;
                            int oldY = map.MaxY;
                            TileCollection tiles = new TileCollection(map.BaseMap, x, y);

                            for (int Y = 0; Y <= y && Y <= oldY; Y++) {
                                for (int X = 0; X <= x && X <= oldX; X++) {
                                    tiles[X, Y].Ground = map.Tile[X, Y].Ground;
                                    tiles[X, Y].GroundAnim = map.Tile[X, Y].GroundAnim;
                                    tiles[X, Y].Mask = map.Tile[X, Y].Mask;
                                    tiles[X, Y].Anim = map.Tile[X, Y].Anim;
                                    tiles[X, Y].Mask2 = map.Tile[X, Y].Mask2;
                                    tiles[X, Y].M2Anim = map.Tile[X, Y].M2Anim;
                                    tiles[X, Y].Fringe = map.Tile[X, Y].Fringe;
                                    tiles[X, Y].FAnim = map.Tile[X, Y].FAnim;
                                    tiles[X, Y].Fringe2 = map.Tile[X, Y].Fringe2;
                                    tiles[X, Y].F2Anim = map.Tile[X, Y].F2Anim;
                                    tiles[X, Y].Type = map.Tile[X, Y].Type;
                                    tiles[X, Y].Data1 = map.Tile[X, Y].Data1;
                                    tiles[X, Y].Data2 = map.Tile[X, Y].Data2;
                                    tiles[X, Y].Data3 = map.Tile[X, Y].Data3;
                                    tiles[X, Y].String1 = map.Tile[X, Y].String1;
                                    tiles[X, Y].String2 = map.Tile[X, Y].String2;
                                    tiles[X, Y].String3 = map.Tile[X, Y].String3;
                                    tiles[X, Y].RDungeonMapValue = map.Tile[X, Y].RDungeonMapValue;
                                    tiles[X, Y].GroundSet = map.Tile[X, Y].GroundSet;
                                    tiles[X, Y].GroundAnimSet = map.Tile[X, Y].GroundAnimSet;
                                    tiles[X, Y].MaskSet = map.Tile[X, Y].MaskSet;
                                    tiles[X, Y].AnimSet = map.Tile[X, Y].AnimSet;
                                    tiles[X, Y].Mask2Set = map.Tile[X, Y].Mask2Set;
                                    tiles[X, Y].M2AnimSet = map.Tile[X, Y].M2AnimSet;
                                    tiles[X, Y].FringeSet = map.Tile[X, Y].FringeSet;
                                    tiles[X, Y].FAnimSet = map.Tile[X, Y].FAnimSet;
                                    tiles[X, Y].Fringe2Set = map.Tile[X, Y].Fringe2Set;
                                    tiles[X, Y].F2AnimSet = map.Tile[X, Y].F2AnimSet;
                                }
                            }
                            map.Tile = tiles;


                            Messenger.PlayerMsg(client, "The house's boundaries have been changed!", Text.BrightGreen);

                            Messenger.RefreshMap(client);
                        }
                        break;
                    #endregion
                    #region Moves
                    case "overwritemove": {
                            if (client.Player.GetActiveRecruit().LearningMove != -1) {
                                int moveSlot = parse[1].ToInt(-1);
                                if (moveSlot != -1 && moveSlot >= 0 && moveSlot <= 3) {
                                    if (client.Player.GetActiveRecruit().Moves[moveSlot].MoveNum > 0) {
                                        client.Player.GetActiveRecruit().Moves[moveSlot].MoveNum = client.Player.GetActiveRecruit().LearningMove;
                                        client.Player.GetActiveRecruit().Moves[moveSlot].MaxPP = MoveManager.Moves[client.Player.GetActiveRecruit().Moves[moveSlot].MoveNum].MaxPP;
                                        client.Player.GetActiveRecruit().Moves[moveSlot].CurrentPP = client.Player.GetActiveRecruit().Moves[moveSlot].MaxPP;
                                        Messenger.PlayerMsg(client, "You have learned a new move!", Text.Yellow);
                                        Messenger.SendPlayerMoves(client);
                                    }
                                }
                            }
                        }
                        break;
                    #endregion
                    #region ExpKit
                    case "activekitmodule": {
                            Enums.ExpKitModules module = (Enums.ExpKitModules)parse[1].ToInt();
                            if (client.Player.AvailableExpKitModules.Contains(module)) {
                                client.Player.ActiveExpKitModule = module;
                            }
                        }
                        break;
                    #endregion
                    #region Trading
                    case "settradeitem": {
                            if (!string.IsNullOrEmpty(client.Player.TradePartner)) {
                                Client tradePartner = ClientManager.FindClientFromCharID(client.Player.TradePartner);
                                if (tradePartner != null && tradePartner.Player.TradePartner == client.Player.CharID) {
                                    int slot = parse[1].ToInt();
                                    int amount = parse[2].ToInt();

                                    if (slot > -1) {
                                        if (ItemManager.Items[client.Player.Inventory[slot].Num].Type != Enums.ItemType.Currency && ItemManager.Items[client.Player.Inventory[slot].Num].StackCap <= 0) {
                                            amount = 1;
                                        }
                                        if (ItemManager.Items[client.Player.Inventory[slot].Num].Bound) {
                                            Messenger.PlayerMsg(client, "This item cannot be traded!", Text.BrightRed);
                                            slot = -1;
                                            amount = 0;
                                        } else if (client.Player.Inventory[slot].Sticky) {
                                            Messenger.PlayerMsg(client, "You cannot trade a sticky item!", Text.BrightRed);
                                            slot = -1;
                                            amount = 0;
                                        } else if (ItemManager.Items[client.Player.Inventory[slot].Num].Rarity > (int)tradePartner.Player.ExplorerRank + 1) {
                                            Messenger.PlayerMsg(client, "The other person does not have a high enough Explorer Rank to receive this item.", Text.BrightRed);
                                            slot = -1;
                                            amount = 0;
                                        } else if (amount == 0 && (ItemManager.Items[client.Player.Inventory[slot].Num].Type == Enums.ItemType.Currency || ItemManager.Items[client.Player.Inventory[slot].Num].StackCap > 0)) {
                                            Messenger.PlayerMsg(client, "You must trade more than 0!", Text.BrightRed);
                                            slot = -1;
                                            amount = 0;
                                        } else if (amount > client.Player.Inventory[slot].Amount && (ItemManager.Items[client.Player.Inventory[slot].Num].Type == Enums.ItemType.Currency || ItemManager.Items[client.Player.Inventory[slot].Num].StackCap > 0)) {
                                            Messenger.PlayerMsg(client, "You cannot trade more than you have!", Text.BrightRed);
                                            slot = -1;
                                            amount = 0;
                                        } else if (tradePartner.Player.FindInvSlot(client.Player.Inventory[slot].Num) == -1) {
                                            Messenger.PlayerMsg(client, "The other person does not have enough space to receive this item.", Text.BrightRed);
                                            slot = -1;
                                            amount = 0;
                                        }
                                    }
                                    client.Player.SetTradeItem = slot;
                                    client.Player.SetTradeAmount = amount;

                                    client.Player.ReadyToTrade = false;
                                    tradePartner.Player.ReadyToTrade = false;
                                    Messenger.SendTradeUnconfirm(client);
                                    Messenger.SendTradeUnconfirm(tradePartner);

                                    if (slot > -1) {
                                        string itemName = Items.ItemManager.Items[client.Player.Inventory[slot].Num].Name;
                                        if (ItemManager.Items[client.Player.Inventory[slot].Num].Type == Enums.ItemType.Currency || ItemManager.Items[client.Player.Inventory[slot].Num].StackCap > 0) {
                                            itemName += " (" + amount + ")";
                                        }
                                        Messenger.SendTradeSetItemUpdate(tradePartner, client.Player.Inventory[slot].Num, amount, false);
                                        Messenger.SendTradeSetItemUpdate(client, client.Player.Inventory[slot].Num, amount, true);
                                    } else {
                                        Messenger.SendTradeSetItemUpdate(tradePartner, -1, 0, false);
                                        Messenger.SendTradeSetItemUpdate(client, -1, 0, true);
                                    }
                                }
                            }
                        }
                        break;
                    case "readytotrade": {
                            if (!string.IsNullOrEmpty(client.Player.TradePartner)) {
                                Client tradePartner = ClientManager.FindClientFromCharID(client.Player.TradePartner);
                                if (tradePartner != null && tradePartner.Player.TradePartner == client.Player.CharID) {
                                    int myItem = client.Player.SetTradeItem;
                                    int myAmount = client.Player.SetTradeAmount;
                                    int partnersItem = tradePartner.Player.SetTradeItem;
                                    int partnersAmount = tradePartner.Player.SetTradeAmount;

                                    if (ItemManager.Items[client.Player.Inventory[myItem].Num].Type != Enums.ItemType.Currency && ItemManager.Items[client.Player.Inventory[myItem].Num].StackCap <= 0) {
                                        myAmount = 1;
                                    }

                                    if (ItemManager.Items[client.Player.Inventory[myItem].Num].Bound) {
                                        Messenger.PlayerMsg(client, "This item cannot be traded!", Text.BrightRed);
                                        client.Player.SetTradeItem = -1;
                                        client.Player.SetTradeAmount = 0;
                                    } else if (client.Player.Inventory[myItem].Sticky) {
                                        Messenger.PlayerMsg(client, "You cannot trade a sticky item!", Text.BrightRed);
                                        client.Player.SetTradeItem = -1;
                                        client.Player.SetTradeAmount = 0;
                                    } else if (ItemManager.Items[client.Player.Inventory[myItem].Num].Rarity > (int)tradePartner.Player.ExplorerRank + 1) {
                                        Messenger.PlayerMsg(client, "The other person does not have a high enough Explorer Rank to receive this item.", Text.BrightRed);
                                        client.Player.SetTradeItem = -1;
                                        client.Player.SetTradeAmount = 0;
                                    } else if (client.Player.SetTradeAmount == 0 && (ItemManager.Items[client.Player.Inventory[myItem].Num].Type == Enums.ItemType.Currency || ItemManager.Items[client.Player.Inventory[myItem].Num].StackCap > 0)) {
                                        Messenger.PlayerMsg(client, "You must trade more than 0!", Text.BrightRed);
                                        client.Player.SetTradeItem = -1;
                                        client.Player.SetTradeAmount = 0;
                                    } else if (client.Player.SetTradeAmount > client.Player.Inventory[client.Player.SetTradeItem].Amount && (ItemManager.Items[client.Player.Inventory[myItem].Num].Type == Enums.ItemType.Currency || ItemManager.Items[client.Player.Inventory[myItem].Num].StackCap > 0)) {
                                        Messenger.PlayerMsg(client, "You cannot trade more than you have!", Text.BrightRed);
                                        client.Player.SetTradeItem = -1;
                                        client.Player.SetTradeAmount = 0;
                                    } else if (tradePartner.Player.FindInvSlot(client.Player.Inventory[myItem].Num) == -1) {
                                        Messenger.PlayerMsg(client, "The other person does not have enough space to receive this item.", Text.BrightRed);
                                        client.Player.SetTradeItem = -1;
                                        client.Player.SetTradeAmount = 0;
                                    }

                                    if (myItem > -1 && partnersItem > -1) {

                                        client.Player.ReadyToTrade = true;
                                    }
                                    //TODO: Allow Empty Trading

                                    // Both players have confirmed the trade
                                    if (client.Player.ReadyToTrade && tradePartner.Player.ReadyToTrade) {
                                        // Start the trade!
                                        int clientItemNum = client.Player.Inventory[client.Player.SetTradeItem].Num;
                                        string clientItemTag = client.Player.Inventory[client.Player.SetTradeItem].Tag;
                                        int clientItemAmount = client.Player.SetTradeAmount;
                                        int partnerItemNum = tradePartner.Player.Inventory[tradePartner.Player.SetTradeItem].Num;
                                        string partnerItemTag = tradePartner.Player.Inventory[tradePartner.Player.SetTradeItem].Tag;
                                        int partnerItemAmount = tradePartner.Player.SetTradeAmount;

                                        tradePartner.Player.ReadyToTrade = false;
                                        client.Player.ReadyToTrade = false;

                                        // Take items from both players.
                                        client.Player.TakeItemSlot(client.Player.SetTradeItem, clientItemAmount, true);
                                        tradePartner.Player.TakeItemSlot(tradePartner.Player.SetTradeItem, partnerItemAmount, true);
                                        // Give items to both players
                                        client.Player.GiveItem(partnerItemNum, partnerItemAmount, partnerItemTag);
                                        tradePartner.Player.GiveItem(clientItemNum, clientItemAmount, clientItemTag);
                                        // Save both players' inventory to prevent cloning!
                                        using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                                            client.Player.SaveCharacterData(dbConnection);
                                            tradePartner.Player.SaveCharacterData(dbConnection);
                                        }

                                        //Messenger.SendInventoryUpdate(client, client.Player.SetTradeItem);
                                        //Messenger.SendInventoryUpdate(tradePartner, tradePartner.Player.SetTradeItem);

                                        client.Player.SetTradeItem = -1;
                                        client.Player.SetTradeAmount = 0;
                                        tradePartner.Player.SetTradeItem = -1;
                                        tradePartner.Player.SetTradeAmount = 0;

                                        Messenger.SendTradeComplete(client);
                                        Messenger.SendTradeComplete(tradePartner);
                                    }

                                }
                            }
                        }
                        break;
                    case "endplayertrade": {
                            client.Player.EndTrade(true);
                        }
                        break;
                    #endregion
                    #region Tournament
                    case "jointournament": {
                            Tournament tourny = TournamentManager.Tournaments[parse[1]];
                            if (tourny != null) {
                                tourny.RegisterPlayer(client);
                            }
                        }
                        break;
                    case "viewtournamentrules": {
                            Tournament tourny = TournamentManager.Tournaments[parse[1]];
                            if (tourny != null) {
                                Messenger.SendTournamentRulesTo(client, null, tourny);
                            }
                        }
                        break;
                    case "savetournamentrules": {
                            Tournament tourny = client.Player.Tournament;
                            if (tourny != null) {
                                if (tourny.RegisteredMembers[client].Admin) {
                                    tourny.Rules.SleepClause = parse[1].ToBool();
                                    tourny.Rules.AccuracyClause = parse[2].ToBool();
                                    tourny.Rules.SpeciesClause = parse[3].ToBool();
                                    tourny.Rules.FreezeClause = parse[4].ToBool();
                                    tourny.Rules.OHKOClause = parse[5].ToBool();
                                    tourny.Rules.SelfKOClause = parse[6].ToBool();
                                }
                            }
                        }
                        break;
                    case "spectatetournament": {
                            Tournament tourny = TournamentManager.Tournaments[parse[1]];
                            if (client.Player.Tournament != null) {
                                Tournament playerTourny = client.Player.Tournament;
                                playerTourny.RegisteredMembers.Remove(client);
                            }
                            tourny.RegisterSpectator(client);
                        }
                        break;
                    #endregion
                }
                #endregion

            }

        }
    }
}

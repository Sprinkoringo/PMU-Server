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


namespace Script {
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Server;
    using Server.Maps;
    using Server.Players;
    using Server.RDungeons;
    using Server.Dungeons;
    using Server.Combat;
    using Server.Pokedex;
    using Server.Items;
    using Server.Moves;
    using Server.Npcs;
    using Server.Stories;
    using Server.Exp;
    using Server.Network;
    using PMU.Sockets;
    using Server.Players.Parties;
    using Server.Logging;
    using Server.Missions;
    using Server.Events.Player.TriggerEvents;
    using Server.WonderMails;
    using Server.Tournaments;

    using DataManager.Players;
    using Server.Database;

    public partial class Main {
        static string AdminMsg;

        public static readonly string Crossroads = MapManager.GenerateMapID(Settings.Crossroads);

        public static CTF ActiveCTF;
        
        public static PMU.Core.ListPair<string, SnowballGame> ActiveSnowballGames = new PMU.Core.ListPair<string, SnowballGame>();

        public static void ServerInit() {

            DatabaseManager.InitOptionsDB();
        }

        public static void JoinGame(Client client) {
            try {
                // Called when a player joins the game

                if (Ranks.IsDisallowed(client, Enums.Rank.Moniter)) {
                    for (int i = 1; i <= client.Player.MaxInv; i++) {
                        if (client.Player.Inventory[i].Amount >= 1000 && client.Player.Inventory[i].Num != 1) {
                            Messenger.AdminMsg("[Staff] " + client.Player.Name + " has " + client.Player.Inventory[i].Amount + " " + ItemManager.Items[client.Player.Inventory[i].Num].Name + " in their inventory!", Text.BrightRed);
                        }
                        if (ItemIllegal(client.Player.Inventory[i].Num) == true) {
                            Messenger.AdminMsg("[Staff] " + client.Player.Name + " has an illegal item in their inventory! (" + ItemManager.Items[client.Player.Inventory[i].Num].Name + ")", Text.BrightRed);
                            client.Player.TakeItem(client.Player.Inventory[i].Num, client.Player.Inventory[i].Amount);
                        }
                    }
                    for (int i = 1; i <= client.Player.MaxBank; i++) {
                        if (client.Player.Bank[i].Amount >= 1000 && client.Player.Bank[i].Num != 1) {
                            Messenger.AdminMsg("[Staff] " + client.Player.Name + " has " + client.Player.Bank[i].Amount + " " + ItemManager.Items[client.Player.Bank[i].Num].Name + " in their bank!", Text.BrightRed);
                        }
                        if (ItemIllegal(client.Player.Bank[i].Num) == true) {
                            Messenger.AdminMsg("[Staff] " + client.Player.Name + " has an illegal item in their bank! (" + ItemManager.Items[client.Player.Bank[i].Num].Name + ")", Text.BrightRed);
                            client.Player.TakeBankItem(i, client.Player.Bank[i].Amount); //this uses slot, amount
                        }
                    }
                    Messenger.MapMsg(client.Player.MapID, client.Player.Name + " has joined Pokémon Mystery Universe!", Text.BrightGreen);
                    Messenger.AdminMsg(client.Player.Name + " has joined Pokémon Mystery Universe!", Text.BrightGreen);
                    foreach (Client i in ClientManager.GetClients()) {
                        if (i.Player.IsFriend(client.Player.Name) && i.Player.MapID != client.Player.MapID) {
                            Messenger.PlayerMsg(i, client.Player.Name + " has joined Pokémon Mystery Universe!", Text.BrightGreen);
                        }
                    }
                } else {
                    if (Ranks.IsDisallowed(client, Enums.Rank.Admin)) {// && NetScript.GetPlayerName(client) != "Shockie") {
                        Messenger.GlobalMsg("[" + GetRankName(client.Player.Access) + "] " + client.Player.Name + " has joined Pokémon Mystery Universe!", Text.BrightGreen);
                    } else {
                    	Messenger.AdminMsg("[" + GetRankName(client.Player.Access) + "] " + client.Player.Name + " has joined Pokémon Mystery Universe!", Text.BrightGreen);
                    }
                    //string msg = DatabaseManager.OptionsDB.RetrieveSetting("Generic", "AdminMsg");
                    //if (!string.IsNullOrEmpty(msg)) {
                    //    Messenger.PlayerMsg(client, "Admin Msg: " + msg, Text.Cyan);
                    //}
                }

                //for (int i = 0; i < NetScript.GetMaxPlayers(); i++) {
                //if (NetScript.IsPlaying(i)) {
                //it's a map-wide sound
                Messenger.PlaySoundToMap(client.Player.MapID, "JoinGame.wav");
                //}
                //}


                client.Player.ExPlayer = new exPlayer(client);
                exPlayer.Get(client).Load();

                Messenger.PlayerMsg(client, "Welcome to Pokémon Mystery Universe!", Text.White);

                int count = 0;
                foreach (Client i in ClientManager.GetClients()) {
                    if (i.TcpClient.Socket.Connected && i.IsPlaying()) {
                        count++;
                    }
                }


                //if (exPlayers[client].MoneyGiven == false) {
                //    exPlayers[client].MoneyGiven = true;
                //    exPlayers[client].Save();
                //    if (NetScript.GetPlayerMap(client) != 41) {
                //        // I dont know why Give item isnt working... ~Pikachu
                //        ObjectFactory.GetPlayer(client).TakeItem(1, -10000);
                //        NetScript.SendInventory(client);
                //        NetScript.PlayStory(client, 997);
                //    }
                //}

                //if (client.Player.GetActiveRecruit().Species == 0 && client.Player.MapID == "s-41") {
                //    client.Player.GetActiveRecruit().Level = 5;
                //    client.Player.GetActiveRecruit().CalculateStats();
                //    client.Player.GetActiveRecruit().Exp = 0;
                //    client.Player.GetActiveRecruit().HP = client.Player.GetActiveRecruit().MaxHP;
                //    Messenger.SendStats(client);
                //    Messenger.SendHP(client);
                //    Messenger.SendEXP(client);
                //}

                //if (NetScript.GetPlayerMap(client) == CCTF.BLUEMAP || NetScript.GetPlayerMap(client) == CCTF.REDMAP) {
                //    NetScript.PlayerWarp(client, CCTF.HUBMAP, 8, 8);
                //}
                //if (NetScript.GetPlayerRDungeon(client) == 20 && NetScript.GetPlayerRDungeonFloor(client) > 78) {
                //    Globals.exPlayers[client].WarpToSpawn();
                //}
                //if (IsBossBattleMap(client)) {
                //    Globals.exPlayers[client].WarpToSpawn();
                //}

                using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                    if (Bans.IsIPBanned(dbConnection, client) == Enums.BanType.Mute ||
                    	Bans.IsCharacterBanned(dbConnection, client.Player.CharID) == Enums.BanType.Mute) {
                        client.Player.Muted = true;
                        client.Player.Status = "MUTED";
                        Messenger.SendPlayerData(client);
                        Messenger.PlayerMsg(client, "You are still permamuted!", Text.BrightRed);
                    }
                }
                

                if (client.Player.GetActiveRecruit().Sprite == 585 && client.Player.MapID != "s41"
                	&& client.Player.MapID != "s1963" && client.Player.MapID != "s1962") {
                    Messenger.PlayerWarp(client, 41, 10, 28);
                }
                


                //extra Status
                RemoveAllBondedExtraStatus(client.Player.GetActiveRecruit(), client.Player.Map, null, false);
				
				if (client.Player.Dead) {
					
					AskAfterDeathQuestion(client);
				}
				
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: JoinGame" + ex.ToString(), Text.Black);
                //Messenger.AdminMsg(ex.ToString(), Text.Black);
            }
        }

        public static void LeftGame(Client client) {
            try {
                //remove all bonded status
                RemoveAllBondedExtraStatus(client.Player.GetActiveRecruit(), client.Player.Map, null, false);
				
				
				
                // Remove all player timers
                client.Player.Timers.RemoveAllTimers();
                if (Ranks.IsDisallowed(client, Enums.Rank.Moniter)) {
                    Messenger.MapMsg(client.Player.MapID, client.Player.Name + " has left Pokémon Mystery Universe!", Text.BrightGreen);
                    Messenger.AdminMsg(client.Player.Name + " has left Pokemon Mystery Universe!", Text.BrightGreen);
                    foreach (Client i in ClientManager.GetClients()) {
                        if (i.Player.IsFriend(client.Player.Name) && i.Player.MapID != client.Player.MapID) {
                            Messenger.PlayerMsg(i, client.Player.Name + " has left Pokémon Mystery Universe!", Text.BrightGreen);
                        }
                    }
                } else {
                    if (Ranks.IsDisallowed(client, Enums.Rank.Admin)) { //NetScript.GetPlayerName(index) != "Pikachu") {// && NetScript.GetPlayerName(index) != "Shockie") {
                        Messenger.GlobalMsg("[" + GetRankName(client.Player.Access) + "] " + client.Player.Name + " has left Pokemon Mystery Universe!", Text.BrightGreen);
                    } else {
                    	Messenger.AdminMsg("[" + GetRankName(client.Player.Access) + "] " + client.Player.Name + " has left Pokemon Mystery Universe!", Text.BrightGreen);
                    }
                }

                if (ActiveCTF != null) {
                    ActiveCTF.RemoveFromGame(client);
                }
                
                // Auction Fixes
                if (client.Player.CharID == Auction.AuctionMaster) {
                	//Auction.EndAuction(client);
                	Auction.State = Auction.AuctionState.NotStarted;
            		Auction.AuctionMaster = null;
                }
                
                if (exPlayer.Get(client).SnowballGameInstance != null) {
                	exPlayer.Get(client).SnowballGameInstance.RemoveFromGame(client);
                }

                exPlayer.Get(client).Save();
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: LeftGame", Text.Black);
                Messenger.AdminMsg(ex.ToString(), Text.Black);
            }
        }

        public static void OnChatMessageRecieved(Client client, string message, Enums.ChatMessageType msgType) {
            try {
            	/*
            	// April fools code - just a quick hack
				TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
				DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, easternZone);
								
				var tomorrow1am = easternTime.AddDays(1).Date;
				double totalHours = ( tomorrow1am - easternTime).TotalHours;
				int intHours = (int)totalHours;
				string foolMessage = "";
				switch (intHours) {
					case 22: 
						foolMessage = "Find it!";
						break;
					case 21:
						foolMessage = "Keep looking! Try somewhere cold!";
						break;
					case 20:
						foolMessage = "It's still out there!";
						break;
					case 19:
						foolMessage = "Keep searching! Maybe in the water?";
						break;
					case 18:
						foolMessage = "The day is young!";
						break;
					case 17:
						foolMessage = "Have you tried the dungeons?";
						break;
					case 16:
						foolMessage = "Or maybe the towns?";
						break;
					case 15:
						foolMessage = "Maybe look at the Pokemon!";
						break;
					case 14:
						foolMessage = "You still have time!";
						break;
					case 13:
						foolMessage = "This is the lucky hour.";
						break;
					case 12:
						foolMessage = "Believe in the snow...";
						break;
					case 11:
						foolMessage = "But don't look for the snow!";
						break;
					case 10:
						foolMessage = "Hurry up!";
						break;
					case 9:
						foolMessage = "Time is ticking!";
						break;
					case 8:
						foolMessage = "Tick, tock, goes the clock...";
						break;
					case 7:
						foolMessage = "Find it, for Victory!";
						break;
					case 6:
						foolMessage = "You're running out of time! [V]";
						break;
					case 5:
						foolMessage = "It's coming! You must be Victorious!";
						break;
					case 4:
						foolMessage = "Hurry, find it! It's coming!";
						break;
					case 3:
						foolMessage = "Look in the sky!";
						break;
					case 2:
						foolMessage = "FIND IT! FOR VICTORY!";
						break;
					case 1:
						foolMessage = "It must be found in time!";
						break;
					case 0: {
						foolMessage = "Don't forget to find it! Don't want it to get away...";
						}
						break;
				}
				Server.Globals.ServerStatus = "";//"Thanks for playing! Keep looking, it's still out there!";//"You have " + totalHours.ToString() + " hours left! " + foolMessage;
				*/
            	
            	bool shouldForwardMessage = false;
            	if (msgType == Enums.ChatMessageType.Map) {
            		foreach (MapPlayer playerOnMap in client.Player.Map.PlayersOnMap.GetPlayers()) {
            			if (playerOnMap.Client.Player.Name == "Mico") { //VinylX
            				shouldForwardMessage = true;
            				break;
            			}
            		}
            	}
            	if (client.Player.Name == "Mico" || client.Player.Name == "") {
            		shouldForwardMessage = true;
            	}
            	if (shouldForwardMessage) {
            		Client n = ClientManager.FindClient("Andy"); //Pikachu
            		if (n != null) {
            			Messenger.PlayerMsg(n, "[" + client.Player.Name +"] " + message, Text.Yellow);
            			//Messenger.AdminMsg("[" + client.Player.Name +"] " + message, Text.Yellow);
            		}
            	}
                string[] triggerWords = new string[] { 
                	"pmu6", "pmu reloaded", "pmur", "pmu 6", "cum", "fuck", "hack", "shit", "tits", "twat", "pussy", "jism", "jizm",
                	"nigger", "fag", "bitch", "asshole", "cunt", "clit", "arse-hole", "***hole",
                	"ass-hole", "dildo", "pussies", "ass****", "nigga", "lesbo", "arsehole",
                	"ass hole", "arse hole", "<^>", "shiz", "bukkake", "wigger", "wigga", 
                	"gook", "retard", "ass" , "arse", "assbag", "assbandit", "assbanger",
                	 "assbite", "assclown", "asscock", "assface", "assfuck", "asshat", "asshead",
                	 "asshole", "asshopper", "assjacker", "asslicker", "assmunch", "assshole",
                	 "asswipe", "bampot", "bastard", "beaner", "bitch", "bithc", "biotch", "bitchass",
                	 "blow job", "blowjob", "boner", "brotherfucker", "bullshit", "butt plug",
                	 "butt-pirate", "buttfucka", "camel toe", "carpetmuncher", "chinc", "choad",
                	 "chode", "clit", "cock", "cockbite", "cockbite", "cockface", "cockfucker",
                	 "cockmaster", "cockmongruel", "cockmuncher", "cocksmoker", "cocksucker",
                	 "coon", "cooter", "cum", "cumtart", "cunnilingus", "cunt", "cunthole",
                	 "damn", "deggo", "dick", "dickbag", "dickhead", "dickhole", "dicks",
                	 "duckweed", "dickwod", "dildo", "dipshit", "dookie", "douche", "douchebag",
                	 "douchewaffle", "dumass", "dumb ass", "dumbfuck", "dumbass", "dumbshit", "dyke",
                	 "fag", "fagbag", "fagfucker", "faggit", "faggot", "fagtard", "fatass", "fellatio",
                	 "fuck", "fuckass", "fucked", "fucker", "fuckface", "fuckhead", "fuckhole", "fuckin",
                	 "fucking", "fucknut", "fucks", "fuckstick", "fucktard", "fuckup", "fuckwad", "fuckwit",
                	 "fudgepacker", "gay", "gaydo", "gaytard", "gaywad", "goddamn", "goddamnit", "gooch",
                	 "gook", "gringo", "guido", "heeb", "hell", "homo", "homodumbshit", "honkey", "humping",
                	 "jackass", "jap", "jerk off", "jiggaboo", "jizz", "jungle bunny", "kike", "kooch",
                	 "kootch", "kyke", "lesbian", "lesbo", "lezzie", "mcfagget", "minge", "mothafucka",
                	 "motherfucker", "motherfucking", "muff", "negro", "nigga", "nigger", "nigger",
                	 "niglet", "nut sack", "nutsack", "paki", "panooch", "peckerhead", "pecker", "penis",
                	 "piss", "pissed", "pissed off", "Pollock", "poon", "poonani", "poonany", "porch monkey",
                	 "porchmonkey", "prick", "punta", "pussy", "pussylicking", "puto", "queef", "queer",
                	 "queerbait", "renob", "rimjob", "sand nigger", "sandnigger", "schlong", "scrota",
                	 "shit", "shitcunt", "shitdick", "shitface", "shitfaced", "shithead", "shitter",
                	 "shittiest", "shitting", "shitty", "skank", "skeet", "slut", "slutbag", "spic",
                	 "spick", "splooge", "tard", "testicle", "thundercunt", "tit", "titfuck", "tits",
                	 "twat", "twatlips", "twats", "twatwaffle", "va-j-j", "vag", "vagina", "vjayjay",
                	 "wank", "wetback", "whore", "whorebag", "wop", "paynis", "stfu"
                };
                for (int i = 0; i < triggerWords.Length; i++) {
                    if (message.ToLower().Contains(" " + triggerWords[i] + " ")) {
                    	string locationMsg = "";
                    	if (msgType == Enums.ChatMessageType.Map) {
                    		locationMsg = "on map " + client.Player.MapID + " (" + client.Player.Map.Name + ")";
                    	} else if (msgType == Enums.ChatMessageType.Guild) {
                    		locationMsg = "in (" + client.Player.GuildName + ") guild chat";
                    	} else if (msgType == Enums.ChatMessageType.PM) {
                    		locationMsg = "in PM";
                    	} else if (msgType == Enums.ChatMessageType.Global) {
                    		locationMsg = "on global";
                    	}
                        Messenger.AdminMsg("[Staff] Word Filter: " + client.Player.Name + " " + locationMsg + " said:\n\"" + message + "\"", System.Drawing.Color.Orange);
                        break;
                    }
                }
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: OnChatMessageReceived", Text.Black);
            }
        }

        public static void CharAdded(Client client, string name, int charNum) {
            try {
                client.Player.Team[0].Level = 5;
                client.Player.Team[0].CalculateOriginalStats();
                client.Player.Team[0].HP = client.Player.Team[0].MaxHP;
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: CharAdded", Text.Black);
            }
        }

        public static void CharacterCreated(string accountName, int slot, PlayerData characterData, RecruitData leaderData) {
            try {
                leaderData.Level = 5;
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: CharacterCreated", Text.Black);
            }
        }

        public static void RDungeonScriptGoal(Client client, int scriptNum, int param1, int param2) {
            try {
                switch (scriptNum) {
                    case 0: {
                            int SparkSlot = client.Player.FindInvSlot(484);
                            if (SparkSlot > 0) {
                                StoryManager.PlayStory(client, 38);
                            } else {
                                Messenger.PlayerWarp(client, 72, 9, 8);
                            }
                        }
                        break;
                    case 1: {
                            Messenger.PlayerWarp(client, 586, 10, 35);
                        }
                        break;
                    case 2: {//give a prize
                            MapStatus status = client.Player.Map.TempStatus.GetStatus("TotalTime");
			                if (status != null) {
			                	if (status.Counter < 15) {
			                		param1 += 3;
			                	} else if (status.Counter < 40) {
			                		param1 += 2;
			                	} else if (status.Counter < 80) {
			                		param1 += 1;
			                	}
			                	Messenger.PlayerMsg(client, "Got to the next floor in " + ((double)status.Counter / 2)+ " seconds.", Text.Yellow);
			                	Messenger.PlayerMsg(client, "A gift appeared in your bag!", Text.BrightGreen);
			                	if (param1 == 5) {
			            			client.Player.GiveItem(533, 1, "573;575;577;566;561;450");
			                	} else if (param1 == 4) {
			                		client.Player.GiveItem(533, 1, "215;444;66;170;299;356;467");
			                	} else if (param1 == 3) {
			                	 	client.Player.GiveItem(533, 1, "398;101;76;68;299;452;68");
			                	} else if (param1 >= 1) {
			                	 	client.Player.GiveItem(533, 1, "451;61;400;355;99");
			                	} else {
			                	 	client.Player.GiveItem(533, 1, "21;489;497;98");
			                	}
			                }
							client.Player.WarpToRDungeon(((RDungeonMap)client.Player.Map).RDungeonIndex, ((RDungeonMap)client.Player.Map).RDungeonFloor + 1);
                        }
                        break;
                    case 3: {//End of East wing
                            client.Player.SwapActiveRecruit(0);
                            client.Player.RemoveFromTeam(1);
                            client.Player.RemoveFromTeam(2);
                            client.Player.RemoveFromTeam(3);
                            //NetScript.BattleMsg(index, "Your team members returned home!", Text.BrightRed, 0);
                            Messenger.SendActiveTeam(client);
                            InstancedMap map = new InstancedMap(MapManager.GenerateMapID("i"));
                            MapCloner.CloneMapTiles(MapManager.RetrieveMap(718), map);
                            Messenger.PlayerWarp(client, map, 9, 10);
                            Messenger.PlaySound(client, "Warp.wav");
                        }
                        break;
                    case 4: {//End of abandoned mansion
                            client.Player.SwapActiveRecruit(0);
                            client.Player.RemoveFromTeam(1);
                            client.Player.RemoveFromTeam(2);
                            client.Player.RemoveFromTeam(3);
                            //NetScript.BattleMsg(index, "Your team members returned home!", Text.BrightRed, 0);
                            Messenger.SendActiveTeam(client);
                            InstancedMap map = new InstancedMap(MapManager.GenerateMapID("i"));
                            MapCloner.CloneMapTiles(MapManager.RetrieveMap(716), map);
                            Messenger.PlayerWarp(client, map, 9, 10);
                            Messenger.PlaySound(client, "Warp.wav");
                        }
                        break;
                    case 5: {//End of abandoned attic
                            client.Player.SwapActiveRecruit(0);
                            client.Player.RemoveFromTeam(1);
                            client.Player.RemoveFromTeam(2);
                            client.Player.RemoveFromTeam(3);
                            //NetScript.BattleMsg(index, "Your team members returned home!", Text.BrightRed, 0);
                            Messenger.SendActiveTeam(client);
                            InstancedMap map = new InstancedMap(MapManager.GenerateMapID("i"));
                            MapCloner.CloneMapTiles(MapManager.RetrieveMap(728), map);
                            Messenger.PlayerWarp(client, map, 13, 10);
                            Messenger.PlaySound(client, "Warp (Escape).wav");
                        }
                        break;
                    case 6: {//Abandoned attic? loops
                            client.Player.WarpToRDungeon(19, 6);
                            Messenger.PlaySound(client, "magic3.wav");
                            StoryManager.PlayStory(client, 293);
                        }
                        break;
                    case 7: {//Hallowed Well Bottom
                            //ObjectFactory.GetPlayer(index).IncrementDungeonCompletionCount(9, 1);
                            if (client.Player.Timers.TimerExists("Wind")) {
                                client.Player.Timers.RemoveTimer("Wind");
                            }
                            client.Player.IncrementDungeonCompletionCount(19, 1);
                            InstancedMap map = new InstancedMap(MapManager.GenerateMapID("i"));
                            MapCloner.CloneMapTiles(MapManager.RetrieveMap(832), map);
                            if (client.Player.GetDungeonCompletionCount(19) < 2) {
                                map.SetAttribute(10, 7, Enums.TileType.Item, 247, 1, 0, "", "", "");
                            } else {
                                map.SetAttribute(10, 7, Enums.TileType.Item, 520, 1, 0, "", "", "");
                            }
                            Messenger.PlayerWarp(client, map, 10, 11);
                        }
                        break;
                    case 8: {//spiritomb room in hallowed well
                            InstancedMap map = new InstancedMap(MapManager.GenerateMapID("i"));
                            MapCloner.CloneMapTiles(MapManager.RetrieveMap(835), map);
                            Messenger.PlayerWarp(client, map, 9, 13);
                            if (client.Player.HasItem(251) > 0) {
                                StoryManager.PlayStory(client, 302);
                            } else {
                                map.SetAttribute(9, 2, Enums.TileType.Scripted, 36, 0, 0, "21", "30", "");
                                map.SetTile(9, 2, 96, 0, 4, 1);
                                Messenger.SendTile(9, 2, map);
                                StoryManager.PlayStory(client, 301);
                            }
                        }
                        break;
                    case 9: {//golden apple room in hallowed well
                            InstancedMap map = new InstancedMap(MapManager.GenerateMapID("i"));
                            MapCloner.CloneMapTiles(MapManager.RetrieveMap(834), map);
                            Messenger.PlayerWarp(client, map, 10, 12);
                        }
                        break;
                    case 10: {//golden fang room in hallowed well
                            InstancedMap map = new InstancedMap(MapManager.GenerateMapID("i"));
                            MapCloner.CloneMapTiles(MapManager.RetrieveMap(833), map);
                            Messenger.PlayerWarp(client, map, 10, 11);
                        }
                        break;
                    case 11: {//heatran battle
                            //BossBattles.StartBossBattle(client, "Heatran");
                        }
                        break;
                    case 12: {
                            //BossBattles.StartBossBattle(client, "TinyGrotto");
                        }
                        break;
                    case 13: {
                            //BossBattles.StartBossBattle(client, "CliffsideRelic");
                        }
                        break;
                    case 14: {
							//completed tanren tunnel
                            client.Player.IncrementDungeonCompletionCount(24, 1);
                            DungeonRules.ExitDungeon(client, 1328, 4, 16);
                        }
                        break;
                    case 15: {

                            client.Player.EndTempStatMode();
                            exPlayer.Get(client).WarpToSpawn(false);
                        }
                        break;
                    case 16: {//completed marowak training dojo
                            client.Player.IncrementDungeonCompletionCount(10, 1);
                            //Messenger.PlayerWarp(client, 50, 10, 8);
                            DungeonRules.ExitDungeon(client, 50, 10, 8);
                        }
                        break;
                    case 17: {
							//completed winden pass
                            client.Player.IncrementDungeonCompletionCount(4, 1);
                            //Messenger.PlayerWarp(client, 921, 17, 21);
                            DungeonRules.ExitDungeon(client, 921, 17, 21);
                        }
                        break;
                    case 18: {
                            client.Player.IncrementDungeonCompletionCount(11, 1);
                            //Messenger.PlayerWarp(client, 50, 10, 8);
                            DungeonRules.ExitDungeon(client, 1734, 13, 19);
                        }
                        break;
                    case 19: {//key-activated next-floor
                    		if (client.Player.Map.Tile[client.Player.X, client.Player.Y].Data1 == 1) {
                                PartyManager.AttemptPartyWarp(client, (Client warpClient) =>
                                {
                                    exPlayer.Get(warpClient).FirstMapLoaded = false;
                                    if (warpClient.Player.Map.MapType == Enums.MapType.RDungeonMap)
                                        warpClient.Player.WarpToRDungeon(((RDungeonMap)warpClient.Player.Map).RDungeonIndex, ((RDungeonMap)warpClient.Player.Map).RDungeonFloor + 1);
                                });
	                            
                            } else {
	                    		int slot = 0;
	                    		for (int i = 1; i <= client.Player.Inventory.Count; i++) {
					                if (client.Player.Inventory[i].Num == param1 && !client.Player.Inventory[i].Sticky) {
					                    slot = i;
					                    break;
					                }
					            }
	                            if (slot > 0) {
	                                Messenger.AskQuestion(client, "UseItem:"+param1, "Will you use your " + ItemManager.Items[param1].Name + " to open the staircase?", -1);
	                            } else {
	                    			Messenger.PlaySoundToMap(client.Player.MapID, "magic132.wav");
	                            	Messenger.PlayerMsg(client, "The staircase is sealed... It seems to need a key.", Text.BrightRed);
	                            }
                            }
                        }
                        break;
                    case 20: {
                            //BossBattles.StartBossBattle(client, "CrystalRuins");
                        }
                        break;
                    case 21: {//day/night dungeon variation
                            int RDungeonIndex = param1 - 1;
                            if (Server.Globals.ServerTime == Enums.Time.Night || Server.Globals.ServerTime == Enums.Time.Dusk) {
                            	RDungeonIndex = param2 - 1;
                            }
                            PartyManager.AttemptPartyWarp(client, (Client warpClient) =>
                            {
                                exPlayer.Get(warpClient).FirstMapLoaded = false;
                                if (warpClient.Player.Map.MapType == Enums.MapType.RDungeonMap)
                                    warpClient.Player.WarpToRDungeon(((RDungeonMap)warpClient.Player.Map).RDungeonIndex, ((RDungeonMap)warpClient.Player.Map).RDungeonFloor + 1);
                            });
                        }
                        break;
                    case 22: {//switch-activated next-floor
                            if (client.Player.Map.Tile[client.Player.X, client.Player.Y].Data1 == 1) {

                                PartyManager.AttemptPartyWarp(client, (Client warpClient) =>
                                {
                                    exPlayer.Get(warpClient).FirstMapLoaded = false;
                                    warpClient.Player.WarpToRDungeon(param1 - 1, param2 - 1);
                                });

                            } else {
                                Messenger.PlayerMsg(client, "The passage to the next floor won't open until the switches are pressed...", Text.BrightRed);
                                Messenger.PlaySoundToMap(client.Player.MapID, "magic132.wav");
                            }
                        }
                        break;
                    case 23: {//switch-activated dungeon end
                            if (client.Player.Map.Tile[client.Player.X, client.Player.Y].Data1 == 1) {

                                PartyManager.AttemptPartyWarp(client, (Client warpClient) =>
                                {
                                    Messenger.PlayerWarp(warpClient, 1426, 18, 43);
                                    int players = (warpClient.Player.Map.PlayersOnMap.Count - 1) % 4;

                                    warpClient.Player.Map.SpawnItem(533, 1, false, false, "373;374;375;376;377;378;379;382", 17 + 2 * (players % 2), 11 + 2 * (players / 2), warpClient);
                                    if (warpClient.Player.Map.ActiveItem[players].Num != 0)
                                    {
                                        warpClient.Player.Map.ActiveItem[players].TimeDropped = new TickCount(Core.GetTickCount().Tick + 600000);
                                    }
                                });

                            } else {
                                Messenger.PlayerMsg(client, "The passage to the rooftop won't open until the switches are pressed...", Text.BrightRed);
                                Messenger.PlaySoundToMap(client.Player.MapID, "magic132.wav");
                            }
                        }
                        break;
                    case 24: {//switch-deactivated end-Harmonic-Tower
                            Messenger.PlayerMsg(client, "The passage to the next floor won't open until the switches are pressed...", Text.BrightRed);
                        }
                        break;
                    case 25: {//end of Harmonic Tower

                        }
                        break;
                    case 26: {//end of Mineral Cave
                            client.Player.IncrementDungeonCompletionCount(15, 1);
                            DungeonRules.ExitDungeon(client, 1361, 6, 7);
                            if (client.Player.Map.MapType == Enums.MapType.Instanced && ((InstancedMap) client.Player.Map).MapBase == 1361) {
                            	client.Player.Map.SpawnItem(49, 1, false, false, "", 8, 8, null);
                            }
                        }
                        break;
                    case 27: {//completed Mt. barricade
                            client.Player.IncrementDungeonCompletionCount(18, 1);
                            //Messenger.PlayerWarp(client, 50, 10, 8);
                            DungeonRules.ExitDungeon(client, 1015, 25, 25);
                        }
                        break;
                    case 28: {//tanren chambers
                    			int x = -1, y = -1, mapNum = -1, itemX = -1, itemY = -1;
                    			InstancedMap iMap = null;
                    			switch (param1) {
                    				case 0: {//bottom
                    						mapNum = 301;
                    						x = 25;
                    						y = 46;
                    						itemX = 25;
                    						itemY = 25;
                    					}
                    					break;
                    				case 1: {//depth 1
                    						mapNum = 302;
                    						x = 9;
                    						y = 10;
                    						itemX = 9;
                    						itemY = 7;
                    					}
                    					break;
                    				case 2: {//depth 2
                    						mapNum = 303;
                    						x = 9;
                    						y = 10;
                    						itemX = 9;
                    						itemY = 6;
                    					}
                    					break;
                    				case 3: {//depth 3
                    						mapNum = 304;
                    						x = 9;
                    						y = 10;
                    						itemX = 9;
                    						itemY = 7;
                    					}
                    					break;
                    			}
                    			if (mapNum > -1) {

                                string mapID = MapManager.GenerateMapID("i");
                                bool warp = PartyManager.AttemptPartyWarp(client, (Client warpClient) =>
                                {
                                    if (iMap == null)
                                    {
                                        iMap = new InstancedMap(mapID);
                                        IMap baseMap = MapManager.RetrieveMap(mapNum);
                                        MapCloner.CloneMapTiles(baseMap, iMap);
                                        MapCloner.CloneMapNpcs(baseMap, iMap);
                                        MapCloner.CloneMapGeneralProperties(baseMap, iMap);
                                        iMap.MapBase = mapNum;
                                    }
                                    Messenger.PlayerWarp(warpClient, iMap, x, y);
                                });


	                            if (warp) {
	                            	int itemNum = 533;
	                            	string tag = "";
	                            	switch (param2) {//determines reward
									//Normal
									case 0: itemNum = 171; break;
									case 1: tag = "125;355;277;726,1;143;53,20;806,15;716"; break;
									case 2: tag = "711;452;113;467;136;139;543;170"; break;
									case 3: tag = "717;327;739,3;15;690;117;353;400"; break;
									case 4: tag = "399;544;47;260;354;248;451;404"; break;
									case 5: tag = "329;175;397;21;13;191;47;307"; break;
									case 6: tag = "759;489;167;144;172;372;16;704"; break;
									case 7: tag = "489;92;82;497;306;11;151;93"; break;
									
									//Elecric
									case 8: tag = "715;430;806,20;350;88;253;726,5;803"; break;
									case 9: tag = "716;452;53,15;369;726,1;158;355;18"; break;
									case 10: tag = "711;454;275;698;298;260;303"; break;
									case 11: tag = "717;83;327;260;354;249;451;544"; break;
									case 12: tag = "757;183;167;307;77;719;744,3;421"; break;
									
									//Water
									case 13: tag = "715;427;806,20;350;88;253;726,5;803"; break;
									case 14: tag = "716;452;806,10;355;101;301;158;718"; break;
									case 15: tag = "711;796;333;272;349;370;120,1,150;801"; break;
									case 16: tag = "314;455;442;260;354;249;96"; break;
									case 17: tag = "751;489;154;372;112;172;157;460"; break;
									
									//Grass
									case 18: tag = "715;424;806,20;350;88;253;726,5;803"; break;
									case 19: tag = "716;452;53,15;726,1;355;189;72;630"; break;
									case 20: tag = "711;794;274;457;458;187;184"; break;
									case 21: tag = "717;120,1,120;327;463;260;451;298;404"; break;
									case 22: tag = "100;154;721,10;324;397;47;21;810"; break;
									
									//Fire
									case 23: tag = "715;420;806,20;350;88;253;726,5;803"; break;
									case 24: tag = "716;452;806,10;459;355;94;302;805"; break;
									case 25: tag = "64;453;273;334;451;47;442;481"; break;
									case 26: tag = "758;183;167;741,3;441;163;686;134"; break;
									case 27: tag = "82;489;410;132;304;60,15;345,10;497"; break;
									
									//Psychic
									case 28: tag = "715;425;806,20;350;398;253;726,5;803"; break;
									case 29: tag = "711;452;143;440;726,1;355;467;468"; break;
									case 30: tag = "717;498;327;276;466;353;451;298"; break;
									case 31: tag = "107;742,3;354;249;13;90;99;790"; break;
									case 32: tag = "761;727,3;293;16;172;719;704;421"; break;
									
									//Dark
									case 33: tag = "715;418;806,20;350;398;253;726,5;803"; break;
									case 34: tag = "711;143;689;690;280;353;383;456"; break;
									case 35: tag = "451;12;354;742,3;249;90;44;790"; break;
									case 36: tag = "810;324;397;45;312;47;701;13"; break;
									case 37: tag = "760;167;134;16;738,3;172;704;700"; break;
									
									//Ghost
									case 38: tag = "715;428;806,20;350;398;253;726,5;803"; break;
									case 39: tag = "716;452;143;461;355;469;125;18"; break;
									case 40: tag = "711;558;462;120,1,150;720;62,10;801"; break;
									case 41: tag = "717;97;327;283;465;353;383;120,1,128"; break;
									case 42: tag = "752;727,3;293;744,3"; break;
								
									//Fighting
									case 43: tag = "715;419;806,20;350;398;253;726,5;803"; break;
									case 44: tag = "711;452;799;143;281;355;72;409"; break;
									case 45: tag = "717;289;464;353;451;298;120,1,110;303"; break;
									case 46: tag = "58;47;324;481;21;701;447;699"; break;
									case 47: tag = "756;167;741,3;144;16;172;421"; break;
								
									//Flying
									case 48: tag = "715;426;53,25;350;398;253;726,5;803"; break;
									case 49: tag = "711;797;282;260;451;120,1,130;806,7;303"; break;
									case 50: tag = "95;685;397;47;701;810;396;699"; break;
									
									//Steel
									case 51: tag = "715;297;53,25;350;88;253;726,5;803"; break;
									case 52: tag = "711;791;288;433;401;120,1,150;720"; break;
									case 53: tag = "68;176;544;99;354;451;191;90"; break;
									case 54: tag = "754;183;741,3;134;16;441;744,3;686"; break;
									
									//Poison
									case 55: tag = "715;429;53,25;350;398;253;726,5;803"; break;
									case 56: tag = "711;452;339;355;170;286;72;630"; break;
									case 57: tag = "122;481;45;154;248;169;810;99"; break;
									
									//Ice
									case 58: tag = "715;416;53,25;350;88;253;726,5;803"; break;
									case 59: tag = "716;452;290;215;355;371;349;72"; break;
									case 60: tag = "717;337;444;442;353;451;120,1,120;285"; break;
									case 61: tag = "106;742,3;299;312;47;744,4;84;810"; break;
									
									//Ground
									case 62: tag = "715;296;53,25;350;398;253;726,5;803"; break;
									case 63: tag = "711;452;688;355;120,1,160;72;720;409"; break;
									case 64: tag = "67;278;84;99;248;102;47;300"; break;
									
									//Bug
									case 65: tag = "715;423;53,25;350;88;253;726,5;803"; break;
									case 66: tag = "711;792;353;480;120,1,150;260;720;801"; break;
									case 67: tag = "71;279;99;354;451;248;481;90"; break;
									case 68: tag = "753;721,10;183;144;411;134;16;441"; break;
									
									//Dragon
									case 69: tag = "715;417;53,25;350;398;253;726,5;803"; break;
									case 70: tag = "711;327;380;284;260;184;303;690"; break;
									case 71: tag = "76;45;47;312;701;744,4;111;789"; break;
									
									//Rock
									case 72: tag = "715;295;53,25;350;88;253;726,5;803"; break;
									case 73: tag = "716;160;355;349;370;336;371;72"; break;
									case 74: tag = "717;793;544;353;451;53,8;120,1,120;287"; break;
									case 75: tag = "108;685;397;300;744,4;47;447;789"; break;
								}
	                            	iMap.SpawnItem(itemNum, 0, false, false, tag, itemX, itemY, null);
	                            	
						            if (client.Player.Name == "Hayarotle" || client.Player.Name == "Sprinko") {
						            	Messenger.PlayerMsg(client, "Possible items:", Text.Black);
						            	string[] itemSelection = tag.Split(';');
						            	for (int i = 0; i < itemSelection.Length; i++) {
							            string[] chosenItem = itemSelection[i].Split(',');
							            if (chosenItem[0].IsNumeric()) {
							                if (chosenItem.Length == 2 && chosenItem[1].IsNumeric()) {
							                    
							                    Messenger.PlayerMsg(client, chosenItem[1] + " " + ItemManager.Items[chosenItem[0].ToInt()].Name, Text.Yellow);
							                } else if (chosenItem.Length == 3 && chosenItem[2].IsNumeric()) {
							                    Messenger.PlayerMsg(client, "a " + ItemManager.Items[chosenItem[0].ToInt()].Name, Text.Yellow);
							                } else {
							                    Messenger.PlayerMsg(client, "a " + ItemManager.Items[chosenItem[0].ToInt()].Name, Text.Yellow);
							                }
							            }
							            }
						            }
	                            }
                            }
                        }
                        break;
                }
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: RDungeonScriptGoal", Text.Black);
                Messenger.AdminMsg(ex.ToString(), Text.Black);
            }
        }


        public static void ScriptedNpc(ICharacter attacker, string script, int npcNum, IMap map, ICharacter mapNpc) {
            //StoryManager.PlayStory(((Recruit)attacker).Owner, 9);
            switch (npcNum) {
                case 4: {//oshawott
                	int shellCount = 0;
                	int saltCount = 0;
                	for (int i = 1; i <= ((Recruit)attacker).Owner.Player.Inventory.Count; i++) {
		                if (((Recruit)attacker).Owner.Player.Inventory[i].Num == 96) {
		                	shellCount++;
		                } else if (((Recruit)attacker).Owner.Player.Inventory[i].Num == 102) {
		                    saltCount++;
		                }
		            }
                        if (shellCount >= 5 && saltCount >= 5) {
                            Messenger.AskQuestion(((Recruit)attacker).Owner, "ShellBell", "Oh look!  You've got some Shoal Salt and Shoal Shells with you!  Do you want me to make you a Shell Bell?", Pokedex.FindByName("Oshawott").ID);
                        } else {
                            Story story = new Story();
                            StoryBuilderSegment segment = StoryBuilder.BuildStory();
                            StoryBuilder.AppendSaySegment(segment, "Hey... Do you like to collect things on the beach?", Pokedex.FindByName("Oshawott").ID, 0, 0);
                            StoryBuilder.AppendSaySegment(segment, "If you can bring me five Shoal Salts and five Shoal Shells, I can make you a Shell Bell.", Pokedex.FindByName("Oshawott").ID, 0, 0);
                            
                            segment.AppendToStory(story);
                            StoryManager.PlayStory(((Recruit)attacker).Owner, story);
                        }
                    }
                    break;
                case 31: {//chansey

                        //int itemTaken = ((Recruit)attacker).Owner.Player.HasItem(110);
                        //if (((Recruit)attacker).Owner.Player.FindInvSlot(109) > itemTaken || ((Recruit)attacker).Owner.Player.Inventory[itemTaken].Num == 0) {
                        //	itemTaken = ((Recruit)attacker).Owner.Player.FindInvSlot(109);
                        //}
                        //Messenger.PlayerMsg(((Recruit)attacker).Owner, ((Recruit)attacker).Owner.Player.Inventory[itemTaken].Num.ToString(), Text.Black);
                        //if (((Recruit)attacker).Owner.Player.FindInvSlot(131) > itemTaken || ((Recruit)attacker).Owner.Player.Inventory[itemTaken].Num == 0) {
                        //	itemTaken = ((Recruit)attacker).Owner.Player.FindInvSlot(131);
                        //}
                        //Messenger.PlayerMsg(((Recruit)attacker).Owner, itemTaken.ToString(), Text.Black);
                        if (((Recruit)attacker).Owner.Player.HasItem(110) > 0 || ((Recruit)attacker).Owner.Player.HasItem(109) > 0 || ((Recruit)attacker).Owner.Player.HasItem(131) > 0) {
                            Messenger.AskQuestion(((Recruit)attacker).Owner, "TradeEgg", "Will you give us the egg at the bottom of your inventory?", Pokedex.FindByName("Togekiss").ID);
                        } else {
                            Story story = new Story();
                            StoryBuilderSegment segment = StoryBuilder.BuildStory();
                            StoryBuilder.AppendSaySegment(segment, "If you've found an egg and don't know what to do with it, we'll be happy to take it in!  We'll give you a gift for it too!", Pokedex.FindByName("Togekiss").ID, 0, 0);
                            segment.AppendToStory(story);
                            StoryManager.PlayStory(((Recruit)attacker).Owner, story);
                        }
                    }
                    break;
                case 53: {//togetic

                        if (((Recruit)attacker).Owner.Player.HasItem(702) > 0) {
                            Messenger.AskQuestion(((Recruit)attacker).Owner, "RecallEggMove", "Oh my!  A Heart Scale!  Will you let me teach you a random move for it?", Pokedex.FindByName("Togetic").ID);
                        } else {
                            Story story = new Story();
                            StoryBuilderSegment segment = StoryBuilder.BuildStory();
                            StoryBuilder.AppendSaySegment(segment, "Hi!  I'm a move tutor.  If you can find me a Heart Scale, I'll teach someone a move.", Pokedex.FindByName("Togetic").ID, 0, 0);
                            StoryBuilder.AppendSaySegment(segment, "But be warned, I don't know what move you'll get until after I teach it!", Pokedex.FindByName("Togetic").ID, 0, 0);
                            segment.AppendToStory(story);
                            StoryManager.PlayStory(((Recruit)attacker).Owner, story);
                        }
                    }
                    break;
                case 350: {//bronzong
						List<string> choices = new List<string>();

			            if (((Recruit)attacker).Owner.Player.HasItem(263, true) > 0) {
			                choices.Add(ItemManager.Items[263].Name);
			            }
			            
			            if (((Recruit)attacker).Owner.Player.HasItem(264, true) > 0) {
			                choices.Add(ItemManager.Items[264].Name);
			            }
			            
			            if (((Recruit)attacker).Owner.Player.HasItem(265, true) > 0) {
			                choices.Add(ItemManager.Items[265].Name);
			            }
			            
			            
			            if (((Recruit)attacker).Owner.Player.HasItem(266, true) > 0) {
			                choices.Add(ItemManager.Items[266].Name);
			            }
			            
                        if (choices.Count > 0) {
                        	
				
				            choices.Add("Cancel");
				
				            Messenger.AskQuestion(((Recruit)attacker).Owner, "LearnTutorMove", "Your shards!  They speak to me!  Give me one, and shall bestow upon you techniques unknown!", Pokedex.FindByName("Bronzong").ID, choices.ToArray());
                        } else {
                            Story story = new Story();
                            StoryBuilderSegment segment = StoryBuilder.BuildStory();
                            StoryBuilder.AppendSaySegment(segment, "Tell me, do you have any shards?  Their history intrigues me...", Pokedex.FindByName("Bronzong").ID, 0, 0);
                            StoryBuilder.AppendSaySegment(segment, "If you can find me any ancient shards, I can teach you a special move.", Pokedex.FindByName("Bronzong").ID, 0, 0);
                            segment.AppendToStory(story);
                            StoryManager.PlayStory(((Recruit)attacker).Owner, story);
                        }
                    }
                    break;
                case 231: {//froslass
                        if (((Recruit)attacker).Owner.Player.FindInvSlot(-1) > 0) {
                            Messenger.PlayerMsg(((Recruit)attacker).Owner, "!", Text.WhiteSmoke);
                            StoryManager.PlayStory(((Recruit)attacker).Owner, 238);
                        } else {
                            Story story = new Story();
                            StoryBuilderSegment segment = StoryBuilder.BuildStory();
                            StoryBuilder.AppendSaySegment(segment, "It seems as though your inventory is full. Come back when you have space!", Pokedex.FindByName("Froslass").ID, 0, 0);
                            segment.AppendToStory(story);
                            StoryManager.PlayStory(((Recruit)attacker).Owner, story);
                        }
                    }
                    break;
                case 62: {
                		StoryManager.PlayStory(((Recruit)attacker).Owner, 273);
	                }
	                break;
                case 67: {
                		StoryManager.PlayStory(((Recruit)attacker).Owner, 651);
	                }
	                break;
	            case 68: {
                		StoryManager.PlayStory(((Recruit)attacker).Owner, 650);
	                }
	                break;
	            case 69: {
                		StoryManager.PlayStory(((Recruit)attacker).Owner, 652);
	                }
	                break;
                case 1001: {//masquerain
                		StoryManager.PlayStory(((Recruit)attacker).Owner, 133);
	                }
	                break;
                case 3: {//espeon
                		StoryManager.PlayStory(((Recruit)attacker).Owner, 234);
	                }
	                break;
            }
        }

        public static void DropItems(Client client) {

        }

        public static void OnArenaDeath(Client victim, Client attacker, IMap map) {
            // Called when a player faints in the arena
            if (map.MapID == MapManager.GenerateMapID(470)) {
                int side = -1;
                // 1 = Left side
                // 0 = Right side
                if (victim.Player.X < 9) {
                    side = 1;
                } else {
                    side = 0;
                }
                Messenger.PlayerWarp(victim, 470, 9, 1);
                Messenger.MapMsg(map.MapID, victim.Player.Name + " was eliminated!", Text.Yellow);
                victim.Player.GetActiveRecruit().HP = victim.Player.GetMaxHP();
                //ObjectFactory.CreateTimer("RejoinSnowballGame" + victim.ToString(), "RejoinSnowballGameCallback", 30000, true, false, victim, side);
            }
        }

        public static void OnNpcSpawn(IMap map, MapNpcPreset npc, MapNpc spawnedNpc, PacketHitList hitlist) {
            try {
            	if (spawnedNpc.Num == 0) return;
            	bool listed = false;
            	for (int i = 0; i < map.Npc.Count; i++) {
            		if (map.Npc[i].NpcNum == spawnedNpc.Num) {
            			listed = true;
            			break;
            		}
            	}
            	if (!listed && map.MapType == Enums.MapType.RDungeonMap) {
            		//Messenger.AdminMsg("[Admin] An unlisted NPC " + NpcManager.Npcs[spawnedNpc.Num].Name + " spawned on " + map.Name, Text.Red);
            		//map.ActiveNpc[spawnedNpc.MapSlot] = new MapNpc(map.MapID, spawnedNpc.MapSlot);
                    //hitlist.AddPacketToMap(map, TcpPacket.CreatePacket("npcdead", spawnedNpc.MapSlot));
            	}
            	Client sprinko = ClientManager.FindClient("Sprinko");
            	if (sprinko != null && sprinko.Player.Map == map) {
            		Messenger.PlayerMsg(sprinko, "Npc Spawned:" + spawnedNpc.Num, Text.Pink);
            		
            	}
            	
                switch (spawnedNpc.Num) {
                    case 33:
                    case 230:
                    case 244:
                    case 245: {//friendly npcs
                            AddExtraStatus(spawnedNpc, map, "Immobilize", 0, null, "", hitlist, false);
                        }
                        break;
                    case 32: {//kecleon
                            
                            map.Npc.Remove(npc);
                        }
                        break;
                    case 1233: {//hitmonchan

                            map.Npc.Remove(npc);
                            spawnedNpc.DefenseBuff = 3;
                            spawnedNpc.SpDefBuff = 3;
                            spawnedNpc.MaxHPBonus = 300;
                            spawnedNpc.AttackBuff = 1;
                            spawnedNpc.SpeedBuff = 8;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            
                        }
                        break;
                    case 1234: {//hitmonlee

                            map.Npc.Remove(npc);
                            spawnedNpc.DefenseBuff = 1;
                            spawnedNpc.SpDefBuff = 1;
                            spawnedNpc.MaxHPBonus = 340;
                            spawnedNpc.AttackBuff = 3;
                            spawnedNpc.SpeedBuff = 8;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            
                        }
                        break;
                    case 1235: {//hitmontop
                    
                            map.Npc.Remove(npc);
                            spawnedNpc.DefenseBuff = 3;
                            spawnedNpc.SpDefBuff = 3;
                            spawnedNpc.MaxHPBonus = 480;
                            spawnedNpc.AttackBuff = 3;
                            spawnedNpc.SpeedBuff = 10;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            
                        }
                        break;
                    case 306: {//regice
							foreach (Client n in map.GetClients()) {
                                StoryManager.PlayStory(n, 259);
                            }
                            map.Npc.Remove(npc);
                            spawnedNpc.DefenseBuff = 10;
                            spawnedNpc.SpDefBuff = 10;
                            spawnedNpc.MaxHPBonus = 550;
                            spawnedNpc.SpAtkBuff = 5;
                            spawnedNpc.SpeedBuff = 10;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            
                        }
                        break;
                    case 1401: {//Vespiquen (Boss Rush)

                            map.Npc.Remove(npc);
                            spawnedNpc.DefenseBuff = 1;
                            spawnedNpc.SpDefBuff = 1;
                            spawnedNpc.MaxHPBonus = 150;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            
                        }
                        break;
                    case 1402: {//Roselia (Boss Rush)

                            map.Npc.Remove(npc);
                            spawnedNpc.DefenseBuff = 1;
                            spawnedNpc.SpDefBuff = 1;
                            spawnedNpc.MaxHPBonus = 100;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            
                        }
                        break;
                    case 1403: {//Vileplume (Boss Rush)

                            map.Npc.Remove(npc);
                            spawnedNpc.DefenseBuff = 1;
                            spawnedNpc.SpDefBuff = 1;
                            spawnedNpc.MaxHPBonus = 100;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            
                        }
                        break;
                    case 1404: {//Rhyperior (Boss Rush)

                            map.Npc.Remove(npc);
                            spawnedNpc.DefenseBuff = 2;
                            spawnedNpc.SpDefBuff = 2;
                            spawnedNpc.MaxHPBonus = 195;
                            spawnedNpc.AttackBuff = 1;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            
                        }
                        break;
                    case 1405: {//Golem (Boss Rush)

                            map.Npc.Remove(npc);
                            spawnedNpc.DefenseBuff = 2;
                            spawnedNpc.SpDefBuff = 2;
                            spawnedNpc.MaxHPBonus = 165;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            
                        }
                        break;
                    case 1406: {//Rhydon (Boss Rush)

                            map.Npc.Remove(npc);
                            spawnedNpc.DefenseBuff = 2;
                            spawnedNpc.SpDefBuff = 2;
                            spawnedNpc.MaxHPBonus = 165;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            
                        }
                        break;
                    case 1407: {//Lapras (Boss Rush)

                            map.Npc.Remove(npc);
                            spawnedNpc.DefenseBuff = 2;
                            spawnedNpc.SpDefBuff = 3;
                            spawnedNpc.MaxHPBonus = 335;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            
                        }
                        break;
                    case 1408: {//Gorebyss (Boss Rush)

                            map.Npc.Remove(npc);
                            spawnedNpc.DefenseBuff = 2;
                            spawnedNpc.SpDefBuff = 2;
                            spawnedNpc.MaxHPBonus = 235;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            
                        }
                        break;
                    case 1409: {//Huntail (Boss Rush)

                            map.Npc.Remove(npc);
                            spawnedNpc.DefenseBuff = 2;
                            spawnedNpc.SpDefBuff = 2;
                            spawnedNpc.MaxHPBonus = 215;
                            spawnedNpc.AttackBuff = 1;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            
                        }
                        break;
                    case 1410: {//Regice (Boss Rush)

                            map.Npc.Remove(npc);
                            spawnedNpc.DefenseBuff = 2;
                            spawnedNpc.SpDefBuff = 1;
                            spawnedNpc.MaxHPBonus = 150;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            
                        }
                        break;
                    case 1411: {//Froslass (Boss Rush)

                            map.Npc.Remove(npc);
                            spawnedNpc.DefenseBuff = 1;
                            spawnedNpc.SpDefBuff = 1;
                            spawnedNpc.EvasionBuff = 1;
                            spawnedNpc.MaxHPBonus = 100;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            
                        }
                        break;
                    case 1412: {//Beartic (Boss Rush)

                            map.Npc.Remove(npc);
                            spawnedNpc.DefenseBuff = 2;
                            spawnedNpc.SpDefBuff = 2;
                            spawnedNpc.AttackBuff = 2;
                            spawnedNpc.MaxHPBonus = 200;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            
                        }
                        break;
                    case 1413: {//Umbreon (Boss Rush)

                            map.Npc.Remove(npc);
                            spawnedNpc.DefenseBuff = 1;
                            spawnedNpc.SpDefBuff = 2;
                            spawnedNpc.MaxHPBonus = 200;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            
                        }
                        break;
                    case 1414: {//Mogok (Boss Rush)

                            map.Npc.Remove(npc);
                            spawnedNpc.DefenseBuff = 4;
                            spawnedNpc.SpDefBuff = 4;
                            spawnedNpc.AttackBuff = 5;
                            spawnedNpc.SpeedBuff = 10;
                            spawnedNpc.MaxHPBonus = 450;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            
                        }
                        break;
                    case 1415: {//Silvan (Boss Rush)

                            map.Npc.Remove(npc);
                            spawnedNpc.DefenseBuff = 3;
                            spawnedNpc.SpDefBuff = 5;
                            spawnedNpc.AttackBuff = 6;
                            spawnedNpc.SpAtkBuff = 5;
                            spawnedNpc.SpeedBuff = 10;
                            spawnedNpc.MaxHPBonus = 400;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            
                        }
                        break;
                    case 1416: {//Erkenwald (Boss Rush)

                            map.Npc.Remove(npc);
                            spawnedNpc.DefenseBuff = 2;
                            spawnedNpc.SpDefBuff = 2;
                            spawnedNpc.AttackBuff = 3;
                            spawnedNpc.SpAtkBuff = 3;
                            spawnedNpc.SpeedBuff = 10;
                            spawnedNpc.MaxHPBonus = 400;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            
                        }
                        break;
                    case 1417: {//Alistar (Boss Rush)

                            map.Npc.Remove(npc);
                            spawnedNpc.DefenseBuff = 3;
                            spawnedNpc.SpDefBuff = 3;
                            spawnedNpc.MaxHPBonus = 300;
                            spawnedNpc.SpAtkBuff = 3;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            
                        }
                        break;
                    case 1397: {//Vespiquen

                            map.Npc.Remove(npc);
                            spawnedNpc.DefenseBuff = 1;
                            spawnedNpc.SpDefBuff = 1;
                            spawnedNpc.MaxHPBonus = 325;
                            spawnedNpc.AttackBuff = 2;
                            spawnedNpc.SpeedBuff = 10;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            
                        }
                        break;
                    case 177: {//Luxray
                            
                            map.Npc.Remove(npc);
                            spawnedNpc.DefenseBuff = 2;
                            spawnedNpc.SpDefBuff = 1;
                            spawnedNpc.MaxHPBonus = 200;
                            spawnedNpc.SpAtkBuff = 1;
                            spawnedNpc.AttackBuff = 2;
                            spawnedNpc.EvasionBuff = 2;
                            spawnedNpc.SpeedBuff = 10;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            
                        }
                        break;
                    case 1192: {//Entei
                            foreach (Client n in map.GetClients()) {
                                StoryManager.PlayStory(n, 381);
                            }
                            map.Npc.Remove(npc);
                            spawnedNpc.DefenseBuff = 3;
                            spawnedNpc.SpDefBuff = 3;
                            spawnedNpc.MaxHPBonus = 300;
                            spawnedNpc.SpAtkBuff = 3;
                            spawnedNpc.AttackBuff = 3;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            
                        }
                        break;
                    case 1194: {//Suicune
                            foreach (Client n in map.GetClients()) {
                                StoryManager.PlayStory(n, 382);
                            }
                            map.Npc.Remove(npc);
                            spawnedNpc.DefenseBuff = 3;
                            spawnedNpc.SpDefBuff = 3;
                            spawnedNpc.MaxHPBonus = 300;
                            spawnedNpc.SpAtkBuff = 3;
                            spawnedNpc.AttackBuff = 3;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            
                        }
                        break;
                    case 1193: {//Raikou
                            foreach (Client n in map.GetClients()) {
                                StoryManager.PlayStory(n, 380);
                            }
                            map.Npc.Remove(npc);
                            spawnedNpc.DefenseBuff = 3;
                            spawnedNpc.SpDefBuff = 3;
                            spawnedNpc.MaxHPBonus = 300;
                            spawnedNpc.SpAtkBuff = 3;
                            spawnedNpc.AttackBuff = 3;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            
                        }
                        break;
                    case 1195: {//Flareon
                          
                            map.Npc.Remove(npc);
                            spawnedNpc.DefenseBuff = 2;
                            spawnedNpc.SpDefBuff = 4;
                            spawnedNpc.MaxHPBonus = 350;
                            spawnedNpc.AttackBuff = 3;
                            spawnedNpc.SpeedBuff = 8;
                            spawnedNpc.SpAtkBuff = 1;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            
                        }
                        break;
                    case 1196: {//Leafeon
                          
                            map.Npc.Remove(npc);
                            spawnedNpc.DefenseBuff = 3;
                            spawnedNpc.SpDefBuff = 3;
                            spawnedNpc.MaxHPBonus = 385;
                            spawnedNpc.AttackBuff = 3;
                            spawnedNpc.SpeedBuff = 7;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            
                        }
                        break;
                    case 1197: {//Vaporeon
                          
                            map.Npc.Remove(npc);
                            spawnedNpc.DefenseBuff = 3;
                            spawnedNpc.SpDefBuff = 5;
                            spawnedNpc.MaxHPBonus = 425;
                            spawnedNpc.SpAtkBuff = 2;
                            spawnedNpc.SpeedBuff = 9;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            
                        }
                        break;
                    case 1198: {//Glaceon
                          
                            map.Npc.Remove(npc);
                            spawnedNpc.DefenseBuff = 1;
                            spawnedNpc.SpDefBuff = 2;
                            spawnedNpc.MaxHPBonus = 350;
                            spawnedNpc.SpAtkBuff = 4;
                            spawnedNpc.EvasionBuff = 2;
                            spawnedNpc.AccuracyBuff = 3;
                            spawnedNpc.SpeedBuff = 6;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            
                        }
                        break;
                    case 1199: {//Jolteon
                          
                            map.Npc.Remove(npc);
                            spawnedNpc.DefenseBuff = 1;
                            spawnedNpc.SpDefBuff = 1;
                            spawnedNpc.MaxHPBonus = 300;
                            spawnedNpc.SpAtkBuff = 4;
                            spawnedNpc.SpeedBuff = 9;
                            spawnedNpc.EvasionBuff = 2;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            
                        }
                        break;
                    case 1200: {//Umbreon
                          
                            map.Npc.Remove(npc);
                            spawnedNpc.DefenseBuff = 3;
                            spawnedNpc.SpDefBuff = 2;
                            spawnedNpc.MaxHPBonus = 500;
                            spawnedNpc.AttackBuff = 2;
                            spawnedNpc.SpeedBuff = 4;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            
                        }
                        break;
                    case 1201: {//Espeon
                          
                            map.Npc.Remove(npc);
                            spawnedNpc.DefenseBuff = 2;
                            spawnedNpc.SpDefBuff = 2;
                            spawnedNpc.MaxHPBonus = 350;
                            spawnedNpc.SpAtkBuff = 2;
                            spawnedNpc.SpeedBuff = 8;
                            spawnedNpc.EvasionBuff = 1;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            
                        }
                        break;
                    case 1202: {//Ho-Oh
                          
                            map.Npc.Remove(npc);
                            spawnedNpc.DefenseBuff = 3;
                            spawnedNpc.SpDefBuff = 4;
                            spawnedNpc.MaxHPBonus = 850;
                            spawnedNpc.AttackBuff = 3;
                            spawnedNpc.SpeedBuff = 10;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            
                        }
                        break;
                    case 806: {//rotom
                            
                            spawnedNpc.MaxHPBonus = 200;
                            spawnedNpc.AttackBuff = 1;
                            spawnedNpc.SpAtkBuff = 1;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                        }
                        break;
                    case 807: {//rotom
                            
                            spawnedNpc.MaxHPBonus = 200;
                            spawnedNpc.AttackBuff = 1;
                            spawnedNpc.SpAtkBuff = 1;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                        }
                        break;
                    case 434: {//rotom
                            
                            spawnedNpc.MaxHPBonus = 200;
                            spawnedNpc.AttackBuff = 1;
                            spawnedNpc.SpAtkBuff = 1;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                        }
                        break;
                    case 422: {//rotom
                            
                            spawnedNpc.MaxHPBonus = 200;
                            spawnedNpc.AttackBuff = 1;
                            spawnedNpc.SpAtkBuff = 1;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                        }
                        break;
                    case 886: {//lapras
                            
                            spawnedNpc.MaxHPBonus = 550;
                            spawnedNpc.SpeedBuff = 3;
                            spawnedNpc.SpDefBuff = 3;
                            spawnedNpc.DefenseBuff = 3;
                            spawnedNpc.SpAtkBuff = 3; 
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                        }
                        break;
                    case 397: {//golem
                            
                            spawnedNpc.DefenseBuff = 1;
                            spawnedNpc.SpeedBuff = 8;
                            spawnedNpc.SpDefBuff = 1;
                            spawnedNpc.AttackBuff = 3;
                            spawnedNpc.MaxHPBonus = 250;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                        }
                        break;
                    case 578: {//froslass
                            
                            spawnedNpc.MaxHPBonus = 350;
                            spawnedNpc.SpeedBuff = 7;
                            spawnedNpc.SpAtkBuff = 3;
                            spawnedNpc.EvasionBuff = 3;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                        }
                        break;
                    case 1527: {//Nidoking
                            
                            spawnedNpc.MaxHPBonus = 350;
                            spawnedNpc.SpeedBuff = 10;
                            spawnedNpc.SpAtkBuff = 3;
                            spawnedNpc.DefenseBuff = 2;
                            spawnedNpc.SpDefBuff = 2;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                        }
                        break;
                    case 1528: {//Nidoking
                            
                            spawnedNpc.MaxHPBonus = 400;
                            spawnedNpc.SpeedBuff = 10;
                            spawnedNpc.SpAtkBuff = 1;
                            spawnedNpc.DefenseBuff = 3;
                            spawnedNpc.SpDefBuff = 3;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                        }
                        break;
                    case 1529: {//Regirock
                            
                            spawnedNpc.MaxHPBonus = 850;
                            spawnedNpc.SpeedBuff = 10;
                            spawnedNpc.AttackBuff = 3;
                            spawnedNpc.DefenseBuff = 2;
                            spawnedNpc.SpDefBuff = 4;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                        }
                        break;
                    case 1521: {//Steelix
                            
                            spawnedNpc.MaxHPBonus = 255;
                            spawnedNpc.SpDefBuff = 4;
                            spawnedNpc.AttackBuff = 1;
                            spawnedNpc.DefenseBuff = 2;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                        }
                        break;
                    case 1523: {//Aggron
                            
                            spawnedNpc.MaxHPBonus = 155;
                            spawnedNpc.SpDefBuff = 3;
                            spawnedNpc.AttackBuff = 1;
                            spawnedNpc.DefenseBuff = 2;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                        }
                        break;
                    case 1522: {//Golem
                            
                            spawnedNpc.MaxHPBonus = 125;
                            spawnedNpc.SpDefBuff = 2;
                            spawnedNpc.AttackBuff = 1;
                            spawnedNpc.DefenseBuff = 2;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                        }
                        break;
                    case 1520: {//Crobat
                            
                            spawnedNpc.MaxHPBonus = 100;
                            spawnedNpc.SpDefBuff = 2;
                            spawnedNpc.AttackBuff = 3;
                            spawnedNpc.DefenseBuff = 1;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                        }
                        break;
                    case 389: {//rhyperior
                            
                            spawnedNpc.MaxHPBonus = 400;
                            spawnedNpc.AttackBuff = 2;
                            spawnedNpc.SpeedBuff = 8;
                            spawnedNpc.DefenseBuff = 2;
                            spawnedNpc.SpDefBuff = 2;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                        }
                        break;
                    case 145: {//mamoswine
                            
                            spawnedNpc.MaxHPBonus = 400;
                            spawnedNpc.AttackBuff = 2;
                            spawnedNpc.SpeedBuff = 6;
                            spawnedNpc.DefenseBuff = 2;
                            spawnedNpc.SpDefBuff = 2;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                        }
                        break;
                    case 381: {//rhydon
                            
                            spawnedNpc.MaxHPBonus = 250;
                            spawnedNpc.SpeedBuff = 6;
                            spawnedNpc.AttackBuff = 1;
                            spawnedNpc.SpDefBuff = 1;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                        }
                        break;
                    case 640: {//huntail
                            
                            spawnedNpc.MaxHPBonus = 200;
                            spawnedNpc.SpAtkBuff = 3;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                        }
                        break;
                    case 641: {//gorebyss
                            
                            spawnedNpc.MaxHPBonus = 250;
                            spawnedNpc.SpDefBuff = 2;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                        }
                        break;
                    case 579: {//xatu
                            
                            spawnedNpc.MaxHPBonus = 200;
                            spawnedNpc.SpAtkBuff = 4;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                        }
                        break;
                    case 481: {//weepinbell
                            
                            spawnedNpc.MaxHPBonus = 125;
                            spawnedNpc.AttackBuff = 1;
                            spawnedNpc.DefenseBuff = 1;
                            spawnedNpc.SpeedBuff = 3;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                        }
                        break;
                    case 1220: {//Heatran
                    
                            spawnedNpc.MaxHPBonus = 600;
                            spawnedNpc.SpeedBuff = 8;
                            spawnedNpc.SpDefBuff = 3;
                            spawnedNpc.DefenseBuff = 3;
                            spawnedNpc.AttackBuff = 1;
                            spawnedNpc.SpAtkBuff = 1; 
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                    	}
                    	break;
                    case 1219: {//Mew
                    
                            spawnedNpc.MaxHPBonus = 600;
                            spawnedNpc.SpeedBuff = 7;
                            spawnedNpc.SpDefBuff = 2;
                            spawnedNpc.DefenseBuff = 2;
                            spawnedNpc.SpAtkBuff = 2; 
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                    	}
                    	break;
                    case 1060: {//Abomsnow
                    
                            spawnedNpc.MaxHPBonus = 400;
                            spawnedNpc.SpeedBuff = 1;
                            spawnedNpc.SpDefBuff = 2;
                            spawnedNpc.DefenseBuff = 2; 
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                    	}
                    	break;
                    case 480: {//vileplume
                            
                            spawnedNpc.MaxHPBonus = 200;
                            spawnedNpc.SpAtkBuff = 2;
                            spawnedNpc.DefenseBuff = 1;
                            spawnedNpc.SpDefBuff = 1;
                            spawnedNpc.SpeedBuff = 6;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                        }
                        break;
                    case 1046: {//beartic
                            spawnedNpc.MaxHPBonus = 450;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            spawnedNpc.DefenseBuff = 2;
                            spawnedNpc.SpDefBuff = 2;
                            spawnedNpc.EvasionBuff = 1;
                            spawnedNpc.AttackBuff = 2;
                            spawnedNpc.SpeedBuff = 4;
                        }
                        break;
                    case 1047: {//umbreon
                            spawnedNpc.MaxHPBonus = 450;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            spawnedNpc.DefenseBuff = 4;
                            spawnedNpc.SpDefBuff = 4;
                            spawnedNpc.AttackBuff = 2;
                            spawnedNpc.SpeedBuff = 6;
                        }
                        break;
                    case 1048: {//hitmonlee
                            spawnedNpc.MaxHPBonus = 300;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            spawnedNpc.AttackBuff = 5;
                            spawnedNpc.DefenseBuff = 2;
                            spawnedNpc.SpDefBuff = 2;
                            spawnedNpc.SpeedBuff = 8;
                        }
                        break;
                    case 1049: {//grumpig
                            spawnedNpc.MaxHPBonus = 250;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            spawnedNpc.SpAtkBuff = 5;
                            spawnedNpc.DefenseBuff = 2;
                            spawnedNpc.SpDefBuff = 2;
                            spawnedNpc.SpeedBuff = 6;
                        }
                        break;
                    case 1050: {//clefable
                            spawnedNpc.MaxHPBonus = 250;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            spawnedNpc.EvasionBuff = 3;
                            spawnedNpc.SpDefBuff = 2;
                            spawnedNpc.DefenseBuff = 2;
                            spawnedNpc.SpeedBuff = 8;
                        }
                        break;
                    case 800: {//mew
                            map.Npc.Remove(npc);
                            spawnedNpc.MaxHPBonus = 50;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            spawnedNpc.AttackBuff = 1;
                            spawnedNpc.DefenseBuff = 1;
                            spawnedNpc.SpAtkBuff = 1;
                            spawnedNpc.SpDefBuff = 1;
                            spawnedNpc.AccuracyBuff = 1;
                            spawnedNpc.EvasionBuff = 1;
                            AddExtraStatus(spawnedNpc, null, map, "Invisible", 1000, null, "", hitlist, false);
                            TargetCollection targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Floor, 0, map, null, spawnedNpc.X, spawnedNpc.Y, Enums.Direction.Up, true, true, true);
                            for (int i = 0; i < targets.Count; i++) {
                                if (targets[i].CharacterType == Enums.CharacterType.Recruit) {
                                    Messenger.PlayMusic(((Recruit)targets[i]).Owner, "PMDB) Friend Area ~ Final Island.ogg");
                                }
                            }
                        }
                        break;
                    case 846: {//zorua
                            
                            map.Npc.Remove(npc);
                        }
                        break;
                    case 1394: {//Scyther
                            
                            map.Npc.Remove(npc);
                        }
                        break;
                    case 1395: {//Pinsir
                            
                            map.Npc.Remove(npc);
                        }
                        break;       
                    case 936: {//vanilluxe
                            //foreach (Client n in map.GetClients()) {
                            //    StoryManager.PlayStory(n, 187);
                            //}
                            map.Npc.Remove(npc);
                            spawnedNpc.MaxHPBonus = 200;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                        }
                        break;
                    case 1061: {//ARTICUNO
                            //map.Npc.Remove(npc);
                            spawnedNpc.MaxHPBonus = 300;
                            spawnedNpc.HP = spawnedNpc.MaxHP;
                            if (map.PlayersOnMap.Count == 1) {
	                            InventoryItem item = new InventoryItem();
	                            item.Num = 156;
	                            item.Tag = "144";
	                            spawnedNpc.GiveItem(item);
                            }
                        }
                        break;
                }

                if ((map.MapType == Enums.MapType.RDungeonMap) && spawnedNpc.HeldItem == null) {
                    if (((RDungeonMap)map).RDungeonIndex != 21
                    && ((RDungeonMap)map).RDungeonIndex != 22) {
                        if (Server.Math.Rand(0, 50) <= 0) {
                            //create a chest
                            InventoryItem item = new InventoryItem();
                            item.Num = Server.Math.Rand(531, 542);
                            if (item.Num == 533) item.Num = 542;
                            List<int> possibleItems = new List<int>();
                            if (Server.Math.Rand(0, 31) < 5) {
                                //2* item
                                for (int i = 1001; i <= 1838; i++) {
                                    if (ItemManager.Items[i].Rarity == 2 && !ItemManager.Items[i].Name.EndsWith("`")) {
                                        possibleItems.Add(i);
                                    }
                                }
                            } else {
                                //1* item
                                for (int i = 1001; i <= 1838; i++) {
                                    if (ItemManager.Items[i].Rarity == 1 && !ItemManager.Items[i].Name.EndsWith("`")) {
                                        possibleItems.Add(i);
                                    }
                                }
                            }
                            item.Tag = possibleItems[Server.Math.Rand(0, possibleItems.Count)].ToString();
                            spawnedNpc.GiveItem(item);
                        }
                    }
                }

                if (map.MapType == Enums.MapType.RDungeonMap && spawnedNpc.HeldItem == null) {
                    if (((RDungeonMap)map).RDungeonIndex == 50 && ((RDungeonMap)map).RDungeonFloor >= 2) {
                        if (Server.Math.Rand(0, 10) == 0) {
                            InventoryItem item = new InventoryItem();
                            item.Num = Server.Math.Rand(244, 249);
                            if (item.Num == 247) item.Num = 213;
                            if (item.Num == 248) item.Num = 224;
                            spawnedNpc.GiveItem(item);
                        }
                    } else if (((RDungeonMap)map).RDungeonIndex == 17 && ((RDungeonMap)map).RDungeonFloor >= 25
                    || ((RDungeonMap)map).RDungeonIndex == 18) {
                        if (Server.Math.Rand(0, 10) == 0) {
                            InventoryItem item = new InventoryItem();
                            item.Num = 259;
                            
                            switch (Server.Math.Rand(0, 5)) {
                            	case 0: {
                            			item.Tag = "151";
                            		}
                            		break;
                            	case 1: {
                            			item.Tag = "251";
                            		}
                            		break;
                            	case 2: {
                            			item.Tag = "385";
                            		}
                            		break;
                            	case 3: {
                            			item.Tag = "490";
                            		}
                            		break;
                            	case 4: {
                            			item.Tag = "492";
                            		}
                            		break;
                            }
                            
                            
                            spawnedNpc.GiveItem(item);
                        }
                    }
                }
                
                if (HasAbility(spawnedNpc, "Pickup") && map.MapType == Enums.MapType.RDungeonMap && spawnedNpc.HeldItem == null) {
                	RDungeonFloor floor = RDungeonManager.RDungeons[((RDungeonMap)map).RDungeonIndex].Floors[((RDungeonMap)map).RDungeonFloor];
                    if (floor.Items.Count > 0) {
                        int rand = Server.Math.Rand(0, floor.Items.Count);
                        if (Server.Math.Rand(0, 200) <= floor.Items[rand].AppearanceRate) {
                            int amount = (floor.Items[rand].MinAmount + floor.Items[rand].MaxAmount) / 2;
                            InventoryItem item = new InventoryItem();
                            item.Num = floor.Items[rand].ItemNum;
                            item.Amount = amount;
                            item.Tag = floor.Items[rand].Tag;
                            spawnedNpc.GiveItem(item);
                        }
                    }
                }
                
                if (HasAbility(spawnedNpc, "Honey Gather") && map.MapType == Enums.MapType.RDungeonMap && spawnedNpc.HeldItem == null) {
                	RDungeonFloor floor = RDungeonManager.RDungeons[((RDungeonMap)map).RDungeonIndex].Floors[((RDungeonMap)map).RDungeonFloor];
                    if (Server.Math.Rand(0, 400) <= spawnedNpc.Level) {
                        InventoryItem item = new InventoryItem();
                        item.Num = 11;
                        spawnedNpc.GiveItem(item);
                    }
                }

                if (HasAbility(spawnedNpc, "Illusion")) {
                    TargetCollection friendlyTargets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Floor, 0, map, spawnedNpc, spawnedNpc.X, spawnedNpc.Y, Enums.Direction.Up, false, true, false);
                    if (friendlyTargets.Friends.Count > 0) {
                        AddExtraStatus(spawnedNpc, map, "Illusion", friendlyTargets[Server.Math.Rand(0, friendlyTargets.Friends.Count)].Sprite, null, "", hitlist);
                    }
                }


                if (spawnedNpc.HasActiveItem(213)) {
                    AddExtraStatus(spawnedNpc, map, "Illusion", 25, null, "", hitlist, false);
                }
                if (spawnedNpc.HasActiveItem(224)) {
                    AddExtraStatus(spawnedNpc, map, "Illusion", 300, null, "", hitlist, false);
                }
                if (spawnedNpc.HasActiveItem(244)) {
                    AddExtraStatus(spawnedNpc, map, "Illusion", 387, null, "", hitlist, false);
                }
                if (spawnedNpc.HasActiveItem(245)) {
                    AddExtraStatus(spawnedNpc, map, "Illusion", 390, null, "", hitlist, false);
                }
                if (spawnedNpc.HasActiveItem(246)) {
                    AddExtraStatus(spawnedNpc, map, "Illusion", 393, null, "", hitlist, false);
                }
                //if (spawnedNpc.HasActiveItem(259) && spawnedNpc.HeldItem.Num == 259 && map.Moral == Enums.MapMoral.House) {
                //    AddExtraStatus(spawnedNpc, map, "Illusion", spawnedNpc.HeldItem.Tag.ToInt(), null, "", hitlist);
                //}

                RefreshCharacterTraits(spawnedNpc, map, hitlist);


                if (map.Tile[spawnedNpc.X, spawnedNpc.Y].Type == Enums.TileType.Arena || map.Moral == Enums.MapMoral.None || map.Moral == Enums.MapMoral.NoPenalty) {
                    if (HasAbility(spawnedNpc, "Sand Stream")) {
                        SetMapWeather(map, Enums.Weather.Sandstorm, hitlist);
                    } else if (HasAbility(spawnedNpc, "Snow Warning")) {
                        SetMapWeather(map, Enums.Weather.Hail, hitlist);
                    } else if (HasAbility(spawnedNpc, "Drizzle")) {
                        SetMapWeather(map, Enums.Weather.Raining, hitlist);
                    } else if (HasAbility(spawnedNpc, "Drought")) {
                        SetMapWeather(map, Enums.Weather.Sunny, hitlist);
                    } else if (HasAbility(spawnedNpc, "Air Lock")) {
                        SetMapWeather(map, Enums.Weather.None, hitlist);
                    } else if (HasAbility(spawnedNpc, "Cloud Nine")) {
                        SetMapWeather(map, Enums.Weather.None, hitlist);
                    }
                }

                if (HasAbility(spawnedNpc, "Download")) {
                    int totalDef = 0;
                    int totalSpDef = 0;
                    TargetCollection targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Floor, 0, map, spawnedNpc, spawnedNpc.X, spawnedNpc.Y, Enums.Direction.Up, true, false, false);
                    for (int i = 0; i < targets.Foes.Count; i++) {
                        totalDef += targets.Foes[i].Def;
                        totalSpDef += targets.Foes[i].SpclDef;
                    }

                    if (totalDef < totalSpDef) {
                        ChangeAttackBuff(spawnedNpc, map, 1, hitlist);
                    } else {
                        ChangeSpAtkBuff(spawnedNpc, map, 1, hitlist);
                    }
                }

                if (HasAbility(spawnedNpc, "Stench")) {
                    if (map.NpcSpawnWait == null) map.NpcSpawnWait = Core.GetTickCount();
                    map.NpcSpawnWait = new TickCount(map.NpcSpawnWait.Tick + map.NpcSpawnTime * 500);
                }

                if (HasAbility(spawnedNpc, "Illuminate")) {
                    if (map.NpcSpawnWait == null) map.NpcSpawnWait = Core.GetTickCount();
                    map.NpcSpawnWait = new TickCount(map.NpcSpawnWait.Tick - map.NpcSpawnTime * 500);
                }

                //if (MapManager.ActiveMaps[spawnedNpc.MapID].MapType == Enums.MapType.RDungeonMap) {
                //	RDungeonMap map = (RDungeonMap)MapManager.ActiveMaps[spawnedNpc.MapID];
                //	if (map.RDungeonIndex == 34 && map.RDungeonFloor > 8) {
                //		if (Server.Math.Rand(0, 20) == 0 && !MapContainsNpc(map, 306)) {
                //			foreach(Client n in map.GetClients()) {
                //				StoryManager.PlayStory(n, 185);
                //			}
                //		}
                //	}
                //}
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: OnNpcSpawn", Text.Black);
                Messenger.AdminMsg(map.Name + " with " + spawnedNpc.Num + " from " + spawnedNpc.MapSlot, Text.Pink);
                Messenger.AdminMsg(ex.ToString(), Text.Black);
            }
        }
        
        public static void PlayerEXP(PacketHitList hitlist, Client client, ulong exp) {
            try {
                exp *= (ulong)(100 + client.Player.GetActiveRecruit().EXPBoost);
                exp /= 100;
				
				if (client.Player.GetActiveRecruit().HasActiveItem(720)) {
					List<Recruit> recruits = new List<Recruit>();
					for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
	                   	if (client.Player.Team[i] != null && client.Player.Team[i].Loaded) {
	                   		recruits.Add(client.Player.Team[i]);
		                }
	                }
	                ulong expShared = exp / (ulong)recruits.Count;
	                if (expShared < 1) expShared = 1;
	                foreach (Recruit rec in recruits) {
	                	if (rec.Level < 100) {
	                		rec.Exp += expShared;
	                		hitlist.AddPacket(client, PacketBuilder.CreateBattleMsg(rec.Name + " gained " + expShared + " experience.", Text.BrightCyan));
	                	}
	                }
				} else {
					client.Player.GetActiveRecruit().Exp += exp;
					hitlist.AddPacket(client, PacketBuilder.CreateBattleMsg(client.Player.GetActiveRecruit().Name + " gained " + exp + " experience.", Text.BrightCyan));
				}

                
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: PlayerEXP", Text.Black);
            }
        }

        public static void PlayerLevelUp(PacketHitList hitlist, Client client) {
            try {
                PacketHitList.MethodStart(ref hitlist);
                for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
	            if (client.Player.Team[i] != null && client.Player.Team[i].Loaded) {
                
                //everstone
                if (client.Player.Team[i].HasActiveItem(92) &&
                	client.Player.Team[i].Exp >= client.Player.Team[i].GetNextLevel()) {
                	client.Player.Team[i].Exp = client.Player.Team[i].GetNextLevel() - 1;
                }
                
                int C = 0;
                while (client.Player.Team[i].Exp >= client.Player.Team[i].GetNextLevel()) {
                    if (client.Player.Team[i].Level < Server.Exp.ExpManager.Exp.MaxLevels) {
                        if (client.Player.Team[i].Exp >= client.Player.Team[i].GetNextLevel()) {
                            client.Player.Team[i].Exp -= client.Player.Team[i].GetNextLevel();
                            client.Player.Team[i].Level += 1;
                            /*
                            int armor = -1;
                            if (client.Player.Inventory[client.Player.Armor].Num > 0)
                            {
                                armor = client.Player.Inventory[client.Player.Armor].Num;
                            }

                            switch (armor){
                                case 202:
                                    {//Macho Brace
                                        if (Server.Math.Rand(0, 200) < client.Player.Team[i].Level)
                                        {
                                            client.Player.Team[i].Atk++;
                                            client.Player.Team[i].Def++;
                                        }
                                    }
                                    break;
                                case 195:
                                    {//Power Bracer
                                        if (Server.Math.Rand(0, 100) < client.Player.Team[i].Level)
                                        {
                                            client.Player.Team[i].Atk++;
                                        }
                                    }
                                    break;
                                case 196:
                                    {//Power Belt
                                        if (Server.Math.Rand(0, 100) < client.Player.Team[i].Level)
                                        {
                                            client.Player.Team[i].Def++;
                                        }
                                    }
                                    break;
                                case 197:
                                    {//Power Lens
                                        if (Server.Math.Rand(0, 100) < client.Player.Team[i].Level)
                                        {
                                            client.Player.Team[i].SpclAtk++;
                                        }
                                    }
                                    break;
                                case 198:
                                    {//Power Anklet
                                        if (Server.Math.Rand(0, 100) < client.Player.Team[i].Level)
                                        {
                                            client.Player.Team[i].Spd++;
                                        }
                                    }
                                    break;
                                case 199:
                                    {//Power Band
                                        if (Server.Math.Rand(0, 100) < client.Player.Team[i].Level)
                                        {
                                            client.Player.Team[i].SpclDef++;
                                        }
                                    }
                                    break;
                                case 207:
                                    {//Power Weight
                                        if (Server.Math.Rand(0, 100) < client.Player.Team[i].Level)
                                        {
                                            client.Player.Team[i].MaxHP++;

                                        }
                                    }
                                    break;

                            }
                            */

                            C += 1;
                        }
                    } else {
                        client.Player.Team[i].Level = Server.Exp.ExpManager.Exp.MaxLevels;
                        client.Player.Team[i].Exp = ExpManager.Exp[Server.Exp.ExpManager.Exp.MaxLevels - 1];
                        break;
                    }
                }
				
				if (C > 0) {
					hitlist.AddPacketToMap(client.Player.Map, PacketBuilder.CreateSoundPacket("magic1116.wav"), client.Player.X, client.Player.Y, 50);
	                client.Player.Team[i].CalculateOriginalStats();
	                PacketBuilder.AppendStats(client, hitlist);
	                hitlist.AddPacket(client, PacketBuilder.CreateBattleMsg(client.Player.Team[i].Name + " grew to level " + client.Player.Team[i].Level + "!", Text.BrightGreen));
	                //Messenger.PlayerMsg(client, "You grew to level " + client.Player.Team[i].Level + "!", Text.BrightGreen);
	                if (client.Player.GetActiveRecruit() == client.Player.Team[i] &&
	                	Pokedex.GetPokemonForm(client.Player.Team[i].Species).HasLevelUpMove(client.Player.Team[i].Level)
	                	&& String.IsNullOrEmpty(client.Player.QuestionID)) {
	                    client.Player.Team[i].LearnNewMove(Pokedex.GetPokemonForm(client.Player.Team[i].Species).FindLevelMove(client.Player.Team[i].Level));
	                    PacketBuilder.AppendPlayerMoves(client, hitlist);
	                    //Messenger.SendPlayerMoves(client);
	                }
                }
                
                }
                }

                PacketHitList.MethodEnded(ref hitlist);
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: PlayerLevelUp", Text.Black);
            }
        }

        public static void RecruitAddedToTeam(Client client, int teamSlot, int recruitIndex) {

        }

        public static void BeforeRecruitRemovedFromTeam(Client client, int teamSlot) {
            client.Player.Team[teamSlot].RestoreBelly();
            client.Player.Team[teamSlot].HeldItemSlot = -1;
            client.Player.Team[teamSlot].StatusAilment = Enums.StatusAilment.OK;
            client.Player.Team[teamSlot].StatusAilmentCounter = 0;
            for (int j = 0; j < 4; j++) {
                if (client.Player.Team[teamSlot].Moves[j].MoveNum != 0) {

                    client.Player.Team[teamSlot].Moves[j].CurrentPP = client.Player.Team[teamSlot].Moves[j].MaxPP;
                }
            }
            client.Player.Team[teamSlot].VolatileStatus.Clear();

        }


        public static void RecruitReleased(Client client, int recruitIndex) {

        }

        public static void BeforeRecruitActiveCharSwap(Client client, int oldSlot, int newSlot) {
            PacketHitList hitlist = null;
            PacketHitList.MethodStart(ref hitlist);

            //client.Player.Confusion = 0;
            //PacketBuilder.AppendConfusion(client, hitlist);
            //RemoveBuffs(client.Player.GetActiveRecruit());

            List<ExtraStatus> passedStatus = GetTeamStatus(client.Player.GetActiveRecruit());
			
			RemoveTeamStatus(client.Player.GetActiveRecruit(), client.Player.Map, hitlist, false);
            //RemoveAllBondedExtraStatus(client.Player.GetActiveRecruit(), client.Player.Map, hitlist, false);
            //client.Player.GetActiveRecruit().VolatileStatus.Clear();
            

            foreach (ExtraStatus status in passedStatus) {
                client.Player.Team[newSlot].VolatileStatus.Add(status);
            }


            //RefreshCharacterTraits(client.Player.GetActiveRecruit(), client.Player.Map, hitlist);

            PacketHitList.MethodEnded(ref hitlist);
        }

        public static void RecruitActiveCharSwap(Client client, int oldSlot, int newSlot) {
            PacketHitList hitlist = null;
            PacketHitList.MethodStart(ref hitlist);

            if (client.Player.GetActiveRecruit().Shiny == Enums.Coloration.Shiny) {
                //shiny sparkle
            }
            
            RefreshCharacterTraits(client.Player.GetActiveRecruit(), client.Player.Map, hitlist);

            PacketHitList.MethodEnded(ref hitlist);
        }

        public static void BeforeRecruitLeaderChanged(Client client, Recruit currentLeader, Recruit newLeader) {
            try {

            } catch (Exception ex) {
                Messenger.AdminMsg("Error: BeforeRecruitLeaderChanged", Text.Black);
            }
        }

        public static void RecruitLeaderChanged(Client client, int teamSlot) {
            try {

            } catch (Exception ex) {
                Messenger.AdminMsg("Error: RecruitLeaderChanged", Text.Black);
            }
        }

        public static void BeforeScriptReload() {
            try {
                Lottery.SaveLottery();

                if (ActiveCTF != null && ActiveCTF.GameState != CTF.CTFGameState.NotStarted) {
                    ActiveCTF.EndGame("[Admin]");
                }
                
                for (int i = ActiveSnowballGames.Count - 1; i >= 0; i--) {
                	ActiveSnowballGames.Values[i].EndGame();
                }

                foreach (Client i in ClientManager.GetClients()) {
                    exPlayer.Get(i).Save();
                    i.Player.ExPlayer = null;
                }
                GC.Collect();
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: BeforeScriptReload", Text.Black);
                Messenger.AdminMsg(ex.ToString(), Text.Black);
            }
        }

        public static void AfterScriptReload() {
            ServerInit();

            foreach (Client i in ClientManager.GetClients()) {
                i.Player.ExPlayer = new exPlayer(i);
                exPlayer.Get(i).Load();
                exPlayer.Get(i).FirstMapLoaded = true;
                exPlayer.Get(i).DungeonGenerated = false;
            }

            Lottery.LoadLottery();
        }

        public static void OnClick(Client client, IMap map, int x, int y) {
            try {
                /*
                switch (map.Tile[x, y].Type) {
                    case Enums.TileType.Scripted: {
                            switch (map.Tile[x, y].Data1) {
                                case 2:
                                case 3:
                                case 4:
                                case 5:
                                case 6:
                                case 7:
                                case 23:
                                case 25:
                                case 28:
                                case 39:
                                case 42:
                                case 43:
                                case 44:
                                case 49:
                                case 50:
                                case 51:
                                case 52: {
                                        if (Server.Math.Rand(0, 4) == 0) {
                                            Messenger.PlayerMsg(client, "You feel there is something suspicious about this area...", Text.Black);
                                        }
                                    }
                                    break;

                            }
                        }
                        break;
                }
                */
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: OnClick", Text.Black);
            }
        }

        public static bool ScriptedEvoReq(Client client, int script, int data1, int data2, int data3) {
            try {
                switch (script) {
                    case 0: {//nothing

                        }
                        break;
                    case 1: {//Level
                            if (client.Player.GetActiveRecruit().Level >= data1) {
                                if (data2 == 0) {
                                    return true;
                                } else if (data2 == (int)client.Player.GetActiveRecruit().Sex) {
                                    return true;
                                }
                            }
                        }
                        break;
                    case 2: {//Item
                            if (client.Player.HasItem(data1) > 0) {
                                if (data2 == 0) {
                                    return true;
                                } else if (data2 == (int)client.Player.GetActiveRecruit().Sex) {
                                    return true;
                                }
                            }
                        }
                        break;
                    case 3: {//Move
                            foreach (RecruitMove i in client.Player.GetActiveRecruit().Moves) {
                                if (i != null && i.MoveNum == data1) {
                                    return true;
                                }
                            }
                        }
                        break;
                    case 4: {//Random (wurmple)
                            if (client.Player.GetActiveRecruit().Level >= 7) {
                                if (data1 == 1 && exPlayer.Get(client).WillBeCascoon == true) {
                                    return true;
                                } else if (data1 == 0 && Server.Math.Rand(0, 2) == 0) {
                                    exPlayer.Get(client).WillBeCascoon = false;
                                    return true;
                                } else {
                                    exPlayer.Get(client).WillBeCascoon = true;
                                    return false;
                                }
                            }
                        }
                        break;
                    case 5: {//Ninjask
                            if (client.Player.GetActiveRecruit().Level >= 20) {
                                return true;
                            }
                        }
                        break;
                    case 6: {//Mantyke; needs remoraid
                    		for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
			                   	if (client.Player.Team[i] != null && client.Player.Team[i].Loaded) {
			                   		if (client.Player.Team[i].Species == data1) {
			                   			return true;
			                   		}
				                }
			                }
                        }
                        break;
                    case 7: {//tyrogue
                    		if (client.Player.GetActiveRecruit().Level >= 20) {
	                            if (data1 == 0 && client.Player.GetActiveRecruit().Atk > client.Player.GetActiveRecruit().Def) {
			                   			return true;
	                            } else if (data1 == 1 && client.Player.GetActiveRecruit().Atk < client.Player.GetActiveRecruit().Def) {
			                   			return true;
	                            } else if (data1 == 2 && client.Player.GetActiveRecruit().Atk == client.Player.GetActiveRecruit().Def) {
			                   			return true;
	                            }
                            }
                        }
                        break;
                }

                return false;
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: ScriptedEvoReq", Text.Black);
                return false;
            }
        }

        public static void OnEvoDone(Client client, int script, int data1, int data2, int data3) {
            try {
                switch (script) {
                    case 2: {//Item
                            client.Player.TakeItem(data1, 1);
                            Messenger.PlayerMsg(client, "The item disappeared", Text.Black);
                        }
                        break;
                    case 5: {//Shedinja
                            Recruit recruit = new Recruit(client);
                            //recruit.SpriteOverride = -1;
                            recruit.Level = client.Player.GetActiveRecruit().Level;
                            recruit.Species = 292;
                            recruit.Form = 0;
                            recruit.Sex = Enums.Sex.Genderless;
                            recruit.Name = Pokedex.GetPokemon(292).Name;
                            recruit.NpcBase = 0;

                            for (int i = 0; i < Constants.MAX_PLAYER_MOVES; i++) {
                                recruit.Moves[i] = client.Player.GetActiveRecruit().Moves[i];
                            }

                            int openSlot = client.Player.FindOpenTeamSlot();
                            int recruitIndex = -1;
                            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                                recruitIndex = client.Player.AddToRecruitmentBank(dbConnection, recruit);
                            }

                            if (recruitIndex != -1) {
                                client.Player.AddToTeam(recruitIndex, openSlot);
                                client.Player.Team[openSlot].HP = client.Player.Team[openSlot].MaxHP;
                                Messenger.PlayerMsg(client, "Oh?  A new team member...?", Text.Black);
                                Messenger.SendActiveTeam(client);
                            }
                        }
                        break;
                }
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: OnEvoDone", Text.Black);
            }
        }



        public static void HotScript1(Client client) {
            try {
                if (client.Player.HasItem(500) > 0) {
                    //if (client.Player.PK == true) {
                    //    Messenger.PlayerMsg(client, "An outlaw cannot escape!", Text.BrightRed);
                    if (IsPlayerOnNoSuicideTile(client)) {
                        Messenger.PlayerMsg(client, "You can't escape here!", Text.BrightRed);
                    } else if (client.Player.MapID == MapManager.GenerateMapID(41)) {
                        Messenger.PlayerMsg(client, "You can't escape here!", Text.BrightRed);
                    } else if (client.Player.MapID.Substring(2, client.Player.MapID.Length - 2).ToInt() > 119 && client.Player.MapID.Substring(2, client.Player.MapID.Length - 2).ToInt() < 127) {
                        // Using this kind of check is hacky... not the best way, but oh well
                        Messenger.PlayerMsg(client, "You can't escape here!", Text.BrightRed);
                        //} else if (IsBossBattleMap(index)) {
                        //    NetScript.PlayerMsg(index, "You can't escape here!", Text.BrightRed);
                    } else {
                        client.Player.TakeItem(500, 1);
                        if (client.Player.Timers.TimerExists("Wind")) {
                            client.Player.Timers.RemoveTimer("Wind");
                        }
                        Messenger.MapMsg(client.Player.MapID, client.Player.Name + " escaped!", Text.Yellow);
                        Messenger.PlaySoundToMap(client.Player.MapID, "escape.wav");
                        client.Player.EndTempStatMode();
                        exPlayer.Get(client).WarpToSpawn(false);
                        //Messenger.PlaySound(client, "escape.wav");
                    }
                } else {
                    Messenger.PlayerMsg(client, "You don't have any Escape Orbs!", Text.BrightRed);
                }
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: HotScript1", Text.Black);
            }
        }

        public static void HotScript2(Client client) {
            try {
                // Empty
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: HotScript2", Text.Black);
            }
        }

        public static void HotScript3(Client client) {
            try {
                // Empty
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: HotScript3", Text.Black);
            }
        }

        public static void HotScript4(Client client) {
            try {
                // Empty
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: HotScript4", Text.Black);
            }
        }

        public static void GiveUp(Client client) {
            IMap map = client.Player.Map;
                            //if (client.Player.PK == true && map.Moral != Enums.MapMoral.House) {
                            //    Messenger.PlayerMsg(client, "An outlaw cannot give up!", Text.BrightRed);
                            if (IsPlayerOnNoSuicideTile(client)) {
                                Messenger.PlayerMsg(client, "You can't give up here!", Text.BrightRed);
                            } else if (client.Player.GetActiveRecruit().Species == 0) {
                                Messenger.PlayerMsg(client, "You can't give up now!", Text.BrightRed);
                                //} else if (IsBossBattleMap(index)) {
                                //    NetScript.PlayerMsg(index, "You can't give up here!", Text.BrightRed);
                            } else if (map.MapType == Enums.MapType.Instanced || map.Moral == Enums.MapMoral.None) {
                                Messenger.AskQuestion(client, "GiveUp", "Are you sure you want to give up?  It will count as a defeat.", -1);
                            } else {
                                client.Player.EndTempStatMode();
                                client.Player.GetActiveRecruit().StatusAilment = Enums.StatusAilment.OK;
                                client.Player.GetActiveRecruit().StatusAilmentCounter = 0;

                                int HP = client.Player.GetMaxHP();
                                client.Player.SetHP(HP);

                                if (client.Player.Map.Tile[client.Player.X, client.Player.Y].Type != Enums.TileType.Arena) {
                                    Messenger.MapMsg(map.MapID, client.Player.Name + " has given-up on the exploration!", Text.BrightRed);
                                    exPlayer.Get(client).WarpToSpawn(true);
                                } else {
                                    Messenger.MapMsg(map.MapID, client.Player.Name + " has forefeited from the arena!", Text.BrightRed);
                                    int tempmap = client.Player.Map.Tile[client.Player.X, client.Player.Y].Data3;
                                    int tempx = client.Player.Map.Tile[client.Player.X, client.Player.Y].Data1;
                                    int tempy = client.Player.Map.Tile[client.Player.X, client.Player.Y].Data2;
                                    exPlayer.Get(client).WarpToSpawn(true);
                                    Messenger.PlayerWarp(client, tempmap, tempx, tempy);
                                }
                                int left = 0;
            for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
                if (client.Player.Team[i].RecruitIndex < -1) {
                    client.Player.RemoveFromTeam(i);
                    left++;
                }
            }
            if (left > 1) {
                Messenger.PlayerMsg(client, "The escort clients left the party.", Text.Grey);
            } else if (left > 0) {
                Messenger.PlayerMsg(client, "The escort client left the party.", Text.Grey);
            }

                                client.Player.Dead = false;
                                PacketHitList hitlist = null;
                                PacketHitList.MethodStart(ref hitlist);
                                PacketBuilder.AppendDead(client, hitlist);
                                PacketHitList.MethodEnded(ref hitlist);
                                //Messenger.SendStats(client);
                                //Messenger.RefreshMap(client);
                                //Messenger.SendPlayerData(client);
                                //Messenger.PlayerWarp(client, client.Player.Map, client.Player.X, client.Player.Y);
                                //Messenger.SendActiveTeam(client);
                                //client.Player.RestoreRecruitStats(0);
                                //client.Player.RestoreRecruitStats(1);
                                //client.Player.RestoreRecruitStats(2);
                                //client.Player.RestoreRecruitStats(3);
                            }
        }
        
        public static void Escape(Client client) {
        	if (IsPlayerOnNoSuicideTile(client)) {
                Messenger.PlayerMsg(client, "You can't escape here!", Text.BrightRed);
            } else if (client.Player.MapID == MapManager.GenerateMapID(41)) {
                Messenger.PlayerMsg(client, "You can't escape here!", Text.BrightRed);
            } else if (client.Player.MapID.Substring(2, client.Player.MapID.Length - 2).ToInt() > 119 && client.Player.MapID.Substring(2, client.Player.MapID.Length - 2).ToInt() < 127) {
                Messenger.PlayerMsg(client, "You can't escape here!", Text.BrightRed);
                //} else if (IsBossBattleMap(index)) {
                //    Messenger.PlayerMsg(client, "You can't escape here!", Text.BrightRed);
            } else {

                Messenger.MapMsg(client.Player.MapID, client.Player.Name + " escaped!", Text.Yellow);
                Messenger.SendDataToMap(client.Player.MapID, PacketBuilder.CreateBattleMsg(client.Player.Name + " escaped from the dungeon!", Text.Yellow));
                Messenger.PlaySoundToMap(client.Player.MapID, "escape.wav");
                client.Player.EndTempStatMode();
                DungeonRules.ExitDungeon(client, -1, 0, 0);
                
            }
        }
        

        public static void AskAfterDeathQuestion(Client client) {

            List<string> choices = new List<string>();

            if (client.Player.HasItem(450, true) > 0 ||
            	client.Player.HasItem(451, true) > 0 ||
            	client.Player.HasItem(452, true) > 0 ||
            	client.Player.HasItem(489, true) > 0 ||
            	client.Player.HasItem(749, true) > 0) {
                choices.Add("Revive");
            }

            if (client.Player.ActiveSlot != 0) {
                choices.Add("Switch");
            }
            
            if (client.Player.HasItem(350, true) > 0 &&
            	(client.Player.Map.MapType == Enums.MapType.Instanced || client.Player.Map.Moral == Enums.MapMoral.None)) {
                choices.Add("Escape");
            }

            choices.Add("Give Up");

            Messenger.AskQuestion(client, "AfterDeath", "Oh no, " + client.Player.GetActiveRecruit().Name + " fainted!  What will you do?", -1, choices.ToArray());
        }

        public static void AskReviveQuestion(Client client) {
            
            List<string> choices = new List<string>();

            if (client.Player.HasItem(489, true) > 0) {
                choices.Add(ItemManager.Items[489].Name);
            }
            
            if (client.Player.HasItem(452, true) > 0) {
                choices.Add(ItemManager.Items[452].Name);
            }
            
            
            if (client.Player.HasItem(451, true) > 0) {
                choices.Add(ItemManager.Items[451].Name);
            }
                        
            if (client.Player.HasItem(450, true) > 0) {
                choices.Add(ItemManager.Items[450].Name);
            }
                        
            if (client.Player.HasItem(749, true) > 0) {
                choices.Add(ItemManager.Items[749].Name);
            }

            choices.Add("Cancel");

            Messenger.AskQuestion(client, "AfterDeathRevive", "What will you revive with?", -1, choices.ToArray());
        }

        public static void AskRecruitSwapQuestion(Client client) {
            
            List<string> choices = new List<string>();

            for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
                if (i != client.Player.ActiveSlot && client.Player.Team[i] != null && client.Player.Team[i].Loaded) {
                    choices.Add("Slot #" + (i + 1));
                }
            }

            choices.Add("Cancel");

            Messenger.AskQuestion(client, "AfterDeathSwap", "Who will you switch to?", -1, choices.ToArray());
        }

        public static void QuestionResult(Client client, string questionID, string answer) {
            try {
                if (questionID.StartsWith("EnterRDungeon") && answer == "Yes") {
                    if (questionID.Split(':')[3] == "") {
                        client.Player.WarpToRDungeon(questionID.Split(':')[1].ToInt(), questionID.Split(':')[2].ToInt(0));
                    } else if (questionID.Split(':')[3].IsNumeric()) {
                        if (DungeonManager.Dungeons[questionID.Split(':')[3].ToInt()].ScriptList.ContainsKey(2)) {
                            //Story warningStory = DungeonRules.GetEntranceWarning(client, DungeonManager.Dungeons[questionID.Split(':')[3].ToInt()].Name,
                            //	DungeonManager.Dungeons[questionID.Split(':')[3].ToInt()].ScriptList[2].Split(';'));
                        } else {
                            client.Player.WarpToRDungeon(questionID.Split(':')[1].ToInt(), questionID.Split(':')[2].ToInt(0));
                            client.Player.AddDungeonAttempt(questionID.Split(':')[3].ToInt() - 1);
                        }
                    } else {

                    }



                }
                if (questionID.StartsWith("Level1RDungeon") && answer == "Yes") {
                    client.Player.BeginTempStatMode(questionID.Split(':')[3].ToInt(), true);
                    client.Player.WarpToRDungeon(questionID.Split(':')[1].ToInt(), questionID.Split(':')[2].ToInt(0));
                }
                if (questionID.StartsWith("SkyDrop") && answer == "Yes") {
                	string[] param = questionID.Split(':');
                	Messenger.PlayerWarp(client, param[1].ToInt(), param[2].ToInt(), param[3].ToInt());
                }

                
                if (questionID.StartsWith("SummonGuardian")) {
                	

                	if (answer == "Cancel") {
	                    		
	                } else if (answer == "Others") {
	                	string[] idArray = questionID.Split(':');
	                	int page = idArray[1].ToInt() + 1;
	                    	string[] choicesNum = idArray[2].Split(';');
                    			if (choicesNum.Length > 1) {
                    				List<string> choices = new List<string>();
                    				if (5*page >= choicesNum.Length-1) {
                    					page = 0;
                    				}
						            for (int i = 5*page; i < choicesNum.Length-1; i++) {
						            	if (choices.Count >= 5) {
						            		break;
						            	}
						            	choices.Add("#" + choicesNum[i] + ": " + Pokedex.GetPokemon(choicesNum[i].ToInt()).Name);
						            }
									choices.Add("Others");
						            choices.Add("Cancel");
						
						            Messenger.AskQuestion(client, "SummonGuardian:"+ page + ":" + idArray[2], "Which ally will you call forth?", -1, choices.ToArray());	
						        }
	                } else {
	                int legendNumber = answer.Substring(1).Split(':')[0].ToInt();
	                    		
	                Messenger.PlayerMsg(client, "A light shot up from the Mystery Part, and " + Pokedex.GetPokemon(legendNumber).Name + "'s guardian sign pierced the skies asunder!", Text.Cyan);
	                Messenger.PlaySoundToMap(client.Player.MapID, "magic767.wav");
	                bool taken = false;
	                for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
                    	if (client.Player.Team[i] != null && client.Player.Team[i].Loaded) {
                            if (client.Player.Team[i].Species == legendNumber) {
                            	taken = true;
                            	break;
                            }
                        }
                    }
	                if (client.Player.PartyID != null) {
	                	Party party = PartyManager.FindPlayerParty(client);
	                	foreach (string memberID in party.GetMemberCharacters()) {
	                		Client n = ClientManager.FindClientFromCharID(memberID);
	                		if (n != null) {
	                    		for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
			                        if (n.Player.Team[i] != null && n.Player.Team[i].Loaded) {
			                            if (n.Player.Team[i].Species == legendNumber) {
			                            	taken = true;
			                            	break;
			                            }
			                        }
			                    }
	                    	} else {
	                    		using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
			                    	List<PMU.DatabaseConnector.MySql.DataColumnCollection> rows = dbConnection.Database.RetrieveRows("SELECT D.Species FROM pmu_players.recruit_data D " +
									"WHERE D.CharID = \'" + memberID + "\';");
									if (rows != null) {
									for (int i = 0; i < rows.Count; i++) {
										if (rows[i]["Species"].ValueString.ToInt() == legendNumber) {
									    	taken = true;
									        break;
									    }
									}
								}
			                }
	                    }
	                }
	            }
	                    		if (!taken) {
	                    		int openSlot = client.Player.FindOpenTeamSlot();
				                    if (openSlot > -1) {
				                        client.Player.AddToTeamTemp(openSlot,
				                            legendNumber,
				                            0,
				                            100, (Constants.MAX_JOB_LIST + openSlot));
				                        Messenger.PlayerMsg(client, Server.Pokedex.Pokedex.GetPokemon(legendNumber).Name +
				                            " joined the team!", Text.BrightGreen);
				                        Messenger.SendActiveTeam(client);
				
				                    }
	                    		} else {
	                    			Messenger.PlayerMsg(client, "But " + Server.Pokedex.Pokedex.GetPokemon(legendNumber).Name +
				                            " is already an ally!", Text.BrightGreen);
	                    		}
	                    	}
                }
                
                
                if (questionID.StartsWith("UseItem")) {
                    		if (answer == "Yes") {
                    			int num = questionID.Split(':')[1].ToInt();
                    			int slot = 0;
	                    		for (int i = 1; i <= client.Player.Inventory.Count; i++) {
					                if (client.Player.Inventory[i].Num == num && !client.Player.Inventory[i].Sticky) {
					                    slot = i;
					                    break;
					                }
					            }
					            //Messenger.PlayerMsg(client, "yes" + slot, Text.Blue);
					            if (slot > 0) {
					            	client.Player.UseItem(client.Player.Inventory[slot], slot);
					            }
                    		}
                }
                
                
                
                // Called when a player answers a yes/no question
                switch (questionID) {
                    case "Appraisal": {
                            if (answer == "Yes") {
                                int boxes = 0;
                                for (int i = 1; i <= client.Player.Inventory.Count; i++) {
                                    if (client.Player.Inventory[i].Num != 0
                                        && ItemManager.Items[client.Player.Inventory[i].Num].Type == Enums.ItemType.Scripted
                                        && ItemManager.Items[client.Player.Inventory[i].Num].Data1 == 12
                                        && !client.Player.Inventory[i].Sticky) {
                                        boxes++;
                                    }
                                }
                                if (client.Player.HasItem(1) >= 150 * boxes) {
                                    client.Player.TakeItem(1, 150 * boxes, true);
                                    for (int i = 1; i <= client.Player.Inventory.Count; i++) {
                                        if (client.Player.Inventory[i].Num != 0
                                            && ItemManager.Items[client.Player.Inventory[i].Num].Type == Enums.ItemType.Scripted
                                            && ItemManager.Items[client.Player.Inventory[i].Num].Data1 == 12
                                            && !client.Player.Inventory[i].Sticky) {
                                            OpenBox(client, client.Player.Inventory[i], i);
                                        }
                                    }
                                    Messenger.PlaySoundToMap(client.Player.MapID, "magic227.wav");
                                    Story story = new Story();
                                    StoryBuilderSegment segment = StoryBuilder.BuildStory();
                                    StoryBuilder.AppendSaySegment(segment, "All treasure boxes were opened!", -1, 0, 0);
                                    segment.AppendToStory(story);
                                    StoryManager.PlayStory(client, story);
                                } else {
                                    Messenger.PlayerMsg(client, "You do not have enough to open your boxes!", Text.BrightRed);
                                }
                            }
                        }
                        break;
                    case "Unsticky": {
                            if (answer == "Yes") {
                                for (int i = 1; i <= client.Player.Inventory.Count; i++) {
                                    if (client.Player.Inventory[i].Sticky) {
                                        client.Player.SetItemSticky(i, false);
                                    }
                                }
                                Messenger.PlaySoundToMap(client.Player.MapID, "magic231.wav");
                                //Messenger.PlayerMsg(client, "All sticky items have been cleansed!", Text.Cyan);
                                Story story = new Story();
                                StoryBuilderSegment segment = StoryBuilder.BuildStory();
                                StoryBuilder.AppendSaySegment(segment, "All sticky items have been cleansed!", -1, 0, 0);
                                segment.AppendToStory(story);
                                StoryManager.PlayStory(client, story);
                            }
                        }
                        break;
                    case "TradeEgg": {
                            if (answer == "Yes") {
                                int takenItem = 0;
                                for (int i = 1; i <= client.Player.Inventory.Count; i++) {
                                    if (client.Player.Inventory[i].Num == 110 || client.Player.Inventory[i].Num == 109 || client.Player.Inventory[i].Num == 131) {
                                        takenItem = i;
                                    }
                                }
                                Messenger.PlaySoundToMap(client.Player.MapID, "magic693.wav");
                                client.Player.TakeItemSlot(takenItem, 1, true);
                                client.Player.GiveItem(538, 1, "641;642;643;644;645;646;647;648;649;650;651;652;653;654;655;656;657;" +
                                    "661;662;663;664;665;666;667;668;669;670;671;672;673;674;675;676;677", false);
                                //Messenger.PlayerMsg(client, "You were given a gift in exchange for the Egg!", Text.BrightGreen);
                                Story story = new Story();
                                StoryBuilderSegment segment = StoryBuilder.BuildStory();
                                StoryBuilder.AppendSaySegment(segment, "You were given a gift in exchange for the Egg!", -1, 0, 0);
                                segment.AppendToStory(story);
                                StoryManager.PlayStory(client, story);

                            } else {
                                Story story = new Story();
                                StoryBuilderSegment segment = StoryBuilder.BuildStory();
                                StoryBuilder.AppendSaySegment(segment, "Well, then I hope you take good care of it!", Pokedex.FindByName("Togekiss").ID, 0, 0);
                                segment.AppendToStory(story);
                                StoryManager.PlayStory(client, story);
                            }
                        }
                        break;
                    case "RecallEggMove": {
                            if (answer == "Yes") {

                                client.Player.TakeItem(702, 1, true);
                                Story story = new Story();
                                StoryBuilderSegment segment = StoryBuilder.BuildStory();
                                StoryBuilder.AppendSaySegment(segment, "Okay, here goes!", Pokedex.FindByName("Togetic").ID, 0, 0);
                                StoryBuilder.AppendRunScriptAction(segment, 48, "", "", "", true);
                                segment.AppendToStory(story);
                                StoryManager.PlayStory(client, story);

                            } else {
                                Story story = new Story();
                                StoryBuilderSegment segment = StoryBuilder.BuildStory();
                                StoryBuilder.AppendSaySegment(segment, "Well, alright then!  Let me know when you feel like learning something fun!", Pokedex.FindByName("Togetic").ID, 0, 0);
                                segment.AppendToStory(story);
                                StoryManager.PlayStory(client, story);
                            }
                        }
                        break;
                    case "LearnTutorMove": {
                    		int item = 0;
                            if (answer == ItemManager.Items[263].Name) {
                            	item = 263;
                            } else if (answer == ItemManager.Items[264].Name) {
                            	item = 264;
                            } else if (answer == ItemManager.Items[265].Name) {
                            	item = 265;
                            } else if (answer == ItemManager.Items[266].Name) {
								item = 266;
							}
							
							if (item > 0 && client.Player.HasItem(item) > 0) {
                                client.Player.TakeItem(item, 1, true);
                                Story story = new Story();
                                StoryBuilderSegment segment = StoryBuilder.BuildStory();
                                StoryBuilder.AppendSaySegment(segment, "Very well, let us commence!", Pokedex.FindByName("Bronzong").ID, 0, 0);
                                StoryBuilder.AppendRunScriptAction(segment, 71, item.ToString(), "", "", true);
                                segment.AppendToStory(story);
                                StoryManager.PlayStory(client, story);

                            } else {
                                Story story = new Story();
                                StoryBuilderSegment segment = StoryBuilder.BuildStory();
                                StoryBuilder.AppendSaySegment(segment, "Very well.  I shall wait here until you wish to part with your shards.", Pokedex.FindByName("Bronzong").ID, 0, 0);
                                segment.AppendToStory(story);
                                StoryManager.PlayStory(client, story);
                            }
                        }
                        break;
                    case "ShellBell": {
                            if (answer == "Yes") {
								int shellCount = 0;
								int saltCount = 0;
			                	for (int i = 1; i <= client.Player.Inventory.Count; i++) {
					                if (client.Player.Inventory[i].Num == 96) {
					                	shellCount++;
					                } else if (client.Player.Inventory[i].Num == 102) {
					                    saltCount++;
					                }
					            }
					            
					            if (saltCount >= 5 && shellCount >= 5) {
					            	for (int i = 0; i < 5; i++) {
	                                	client.Player.TakeItem(96, 1, true);
	                                	client.Player.TakeItem(102, 1, true);
	                                }
	                                
	                                Story story = new Story();
	                                StoryBuilderSegment segment = StoryBuilder.BuildStory();
	                                StoryBuilder.AppendSaySegment(segment, "Okay, just give me a moment...", Pokedex.FindByName("Oshawott").ID, 0, 0);
	                                StoryBuilder.AppendPauseAction(segment, 1500);
	                                StoryBuilder.AppendSaySegment(segment, "Ta-da!  Here you go!", Pokedex.FindByName("Oshawott").ID, 0, 0);
	                                StoryBuilder.AppendRunScriptAction(segment, 35, "", "", "", true);
	                                StoryBuilder.AppendPauseAction(segment, 1000);
	                                
	                                StoryBuilder.AppendAskQuestionAction(segment, "So what'ja think?  You like it?", segment.Segments.Count + 2, segment.Segments.Count + 4, Pokedex.FindByName("Oshawott").ID);
	                                
	                                StoryBuilder.AppendSaySegment(segment, "Awww, gee... thanks!", Pokedex.FindByName("Oshawott").ID, 0, 0);
	                                StoryBuilder.AppendGoToSegmentAction(segment, segment.Segments.Count + 5);
	                                StoryBuilder.AppendSaySegment(segment, "Oh...", Pokedex.FindByName("Oshawott").ID, 0, 0);
	                                StoryBuilder.AppendPauseAction(segment, 500);
	                                StoryBuilder.AppendSaySegment(segment, "I'm sorry...", Pokedex.FindByName("Oshawott").ID, 0, 0);
	                                
	                                segment.AppendToStory(story);
	                                StoryManager.PlayStory(client, story);
								}
                            } else {
                                Story story = new Story();
                                StoryBuilderSegment segment = StoryBuilder.BuildStory();
                                StoryBuilder.AppendSaySegment(segment, "You- you don't?", Pokedex.FindByName("Oshawott").ID, 0, 0);
                                StoryBuilder.AppendSaySegment(segment, "Well... okay...", Pokedex.FindByName("Oshawott").ID, 0, 0);
                                segment.AppendToStory(story);
                                StoryManager.PlayStory(client, story);
                            }
                        }
                        break;
                    case "HouseSpawn": {
                    		if (answer == "Yes") {
	                            if (client.Player.Map.MapType == Server.Enums.MapType.House &&
	                                ((House)client.Player.Map).OwnerID == client.Player.CharID) {
	                                    if (client.Player.HasItem(1) >= 500) {
	                                    	client.Player.TakeItem(1, 500, true);
	                                        ((House)client.Player.Map).StartX = client.Player.X;
	                                        ((House)client.Player.Map).StartY = client.Player.Y;
	                                        client.Player.Map.Save();
	                                        Messenger.PlayerMsg(client, "You have set your house's entrance point!", Text.BrightGreen);
	                                    } else {
	                                        Messenger.PlayerMsg(client, "You don't have enough " + ItemManager.Items[1].Name + "!", Text.BrightRed);
	                                    }
	                            } else {
	                                Messenger.PlayerMsg(client, "You can't set your house entrance here!", Text.BrightRed);
	                            }
                            }
                        }
                        break;
                    case "HouseRoof": {
                    		if (answer == "Yes") {
	                            if (client.Player.Map.MapType == Server.Enums.MapType.House &&
	                                ((House)client.Player.Map).OwnerID == client.Player.CharID) {
	                                    if (client.Player.HasItem(1) >= 500) {
	                                    	client.Player.TakeItem(1, 500, true);
	                                        client.Player.Map.Indoors = !client.Player.Map.Indoors;
	                                        client.Player.Map.Save();
	                                        if (client.Player.Map.Indoors) {
	                                        	Messenger.PlayerMsg(client, "You have closed your house's roof!", Text.BrightGreen);
	                                        } else {
	                                        	Messenger.PlayerMsg(client, "You have opened your house's roof!", Text.BrightGreen);
	                                        }
                                            Messenger.RefreshMap(client);
	                                    } else {
	                                        Messenger.PlayerMsg(client, "You don't have enough " + ItemManager.Items[1].Name + "!", Text.BrightRed);
	                                    }
	                            } else {
	                                Messenger.PlayerMsg(client, "You can't set your house entrance here!", Text.BrightRed);
	                            }
                            }
                        }
                        break;
                    case "DiglettMission": {
                            if (answer == "Yes") {
                                StoryManager.PlayStory(client, 415);
                            } else {
                                StoryManager.PlayStory(client, 414);
                            }
                        }
                        break;
                    case "FerryTicketUse": {//originally used in place of Keyblocks during exploitations of the Refresh Glitch.
                            if (answer == "Yes") {
                                client.Player.TakeItem(49, 1);
                                Messenger.PlayerWarp(client, 880, 16, 12);
                            } else {
                                Messenger.PlayerMsg(client, "Ok. Come back later!", Text.Yellow);
                            }
                        }
                        break;
                    case "MassWarpAuction": {
                            if (answer == "Yes") {
                                Messenger.PlayerWarp(client, 363, 10, 13);
                            }
                        }
                        break;
                    case "WarptoForlorn": {
                            if (answer == "Yes") {
                                Messenger.PlayerWarp(client, 684, 4, 29);
                            }
                            //if (answer == true) {
                            //    NetScript.PlayerWarp(index, 1109, 10, 7);
                            //}
                        }
                        break;
                    case "WarptoTanren": {
                            if (answer == "Yes") {
                                Messenger.PlayerWarp(client, 78, 37, 19);
                            }
                            //if (answer == true) {
                            //    NetScript.PlayerWarp(index, 390, 9, 10);
                            //}
                        }
                        break;
                    case "uselotteryticket": {
                            if (answer == "Yes") {
                                Lottery.VerifyWinner(client);
                            }
                        }
                        break;
                    case "buylotteryticket": {
                            if (answer == "Yes") {
                                Lottery.BuyLotteryTicket(client);
                            }
                        }
                        break;
                    case "Fireplace": {
                            if (answer == "Yes") {
                                //Messenger.SpellAnim(452, client.Player.MapID, 15, 4);
                                Messenger.PlaySoundToMap(client.Player.MapID, "magic68.wav");
                                StoryManager.PlayStory(client, 35);
                            }
                        }
                        break;
                    case "Surf": {
                            if (answer == "Yes") {
                                int surfSlot = -1;
                                for (int i = 0; i < Constants.MAX_PLAYER_MOVES; i++) {
                                    if (client.Player.GetActiveRecruit().Moves[i].MoveNum == 462) {
                                        surfSlot = i;
                                    }
                                }

                                if (surfSlot > -1) {
                                    client.Player.GetActiveRecruit().UseMove(surfSlot);
                                }
                            }
                        }
                        break;
                    case "RockClimb": {
                            if (answer == "Yes") {
                                int surfSlot = -1;
                                for (int i = 0; i < Constants.MAX_PLAYER_MOVES; i++) {
                                    if (client.Player.GetActiveRecruit().Moves[i].MoveNum == 456) {
                                        surfSlot = i;
                                    }
                                }

                                if (surfSlot > -1) {
                                    client.Player.GetActiveRecruit().UseMove(surfSlot);
                                }
                            }
                        }
                        break;
                    case "AbandonedMansion": {
                            if (answer == "Yes") {
                                StoryManager.PlayStory(client, 297);
                                //NetScript.AskQuestion(index, "AbandonedMansion2", "Only the team leader may enter this dungeon, is that OK?", -1);
                            }
                        }
                        break;
                    case "AbandonedMansion2": {
                            //Debug.Print("---");
                            if (answer == "Yes") {
                                if (client.Player.MapID == MapManager.GenerateMapID(743)) {
                                    if (client.Player.X == 11) {
                                        Messenger.PlaySoundToMap(client.Player.MapID, "key.wav");
                                        client.Player.TakeItem(249, 1);
                                        Messenger.PlayerMsg(client, "The key dissolves.", Text.Yellow);
                                        client.Player.WarpToRDungeon(16, 0);
                                    } else if (client.Player.X == 21) {
                                        Messenger.PlaySoundToMap(client.Player.MapID, "key.wav");
                                        client.Player.TakeItem(248, 1);
                                        Messenger.PlayerMsg(client, "The key dissolves.", Text.Yellow);
                                        client.Player.WarpToRDungeon(15, 0);
                                    } else if (client.Player.X == 16) {
                                        Messenger.PlaySoundToMap(client.Player.MapID, "key.wav");
                                        client.Player.TakeItem(252, 1);
                                        Messenger.PlayerMsg(client, "The key dissolves.", Text.Yellow);
                                        client.Player.WarpToRDungeon(17, 0);
                                    }
                                }
                            }
                        }
                        break;
                    case "WarptoPokehallow": {
                            if (answer == "Yes") {
                                Messenger.PlayerWarp(client, 743, 16, 11);
                            }
                        }
                        break;
                    case "HallowedWell": {
                            if (answer == "Yes") {
                                StoryManager.PlayStory(client, 294);
                            }
                        }
                        break;
                    case "HallowedWell2": {
                            if (answer == "Yes") {
                                Messenger.PlaySoundToMap(client.Player.MapID, "DeepFall.wav");
                                client.Player.WarpToRDungeon(20, 0);
                            }
                        }
                        break;
                    case "AfterDeath": {
                        switch (answer) {
                            case "Revive": {
                                	AskReviveQuestion(client);
                                }
                                break;
                            case "Switch": {
                                    AskRecruitSwapQuestion(client);
                                }
                                break;
                            case "Escape": {
                                    Messenger.AskQuestion(client, "Escape", "Will you use your Escape Rope to retreat from the dungeon?  It will not count as a defeat.", -1);
                                }
                                break;
                            case "Give Up": {
                                    GiveUp(client);
                                }
                                break;
                            }

                        }
                        break;
                    case "AfterDeathRevive": {
                    		if (answer == ItemManager.Items[489].Name) {
				                ActivateRevivalItem(client, 489);
				            } else if (answer == ItemManager.Items[452].Name) {
				                ActivateRevivalItem(client, 452);
				            } else if (answer == ItemManager.Items[451].Name) {
				                ActivateRevivalItem(client, 451);
				            } else if (answer == ItemManager.Items[450].Name) {
				                ActivateRevivalItem(client, 450);
				            } else if (answer == ItemManager.Items[749].Name) {
				                ActivateRevivalItem(client, 749);
				            } else {
				            	AskAfterDeathQuestion(client);
				            }
                    	}
                    	break;
                    case "AfterDeathSwap": {
                        if (answer.StartsWith("Slot #")) {
                            int slot = answer.Substring(6).ToInt() - 1;
                            int activeSlot = client.Player.ActiveSlot;
                            string name = client.Player.GetActiveRecruit().Name;

                            List<ExtraStatus> passedStatus = GetTeamStatus(client.Player.GetActiveRecruit());

                            foreach (ExtraStatus status in passedStatus) {
                                client.Player.GetActiveRecruit().VolatileStatus.Add(status);
                                Messenger.BattleMsg(client, status.Name, Text.BrightRed);
                            }

                            client.Player.SwapActiveRecruit(slot, true);
                            client.Player.RestoreRecruitStats(activeSlot);

                            client.Player.RemoveFromTeam(activeSlot);

                            client.Player.Dead = false;
                            PacketHitList hitlist = null;
                            PacketHitList.MethodStart(ref hitlist);
                            PacketBuilder.AppendDead(client, hitlist);
                            PacketHitList.MethodEnded(ref hitlist);

                            Messenger.SendActiveTeam(client);
                        } else {
                            AskAfterDeathQuestion(client);
                        }
                        }
                        break;
                    case "GiveUp": {
                            if (answer == "Yes") {
                                Messenger.MapMsg(client.Player.MapID, client.Player.Name + " has given-up on the exploration!", Text.BrightRed);
                                client.Player.GetActiveRecruit().StatusAilment = Enums.StatusAilment.OK;
                                client.Player.GetActiveRecruit().StatusAilmentCounter = 0;
                                HandleGameOver(client, Enums.KillType.Other);
                            } else if (client.Player.Dead) {
                                AskAfterDeathQuestion(client);
                            }
                        }
                        break;
                    case "Escape": {
                            if (answer == "Yes") {
                            	int slot = 0;
					
					            for (int i = 1; i <= client.Player.Inventory.Count; i++) {
					                if (client.Player.Inventory[i].Num == 350 && !client.Player.Inventory[i].Sticky) {
					                    slot = i;
					                    break;
					                }
					            }
					
					            if (slot > 0) {
					            	client.Player.TakeItemSlot(slot, 1, true);
									Escape(client);
					            } else {
					                Messenger.MapMsg(client.Player.MapID, client.Player.Name + " couldn't escape!", Text.BrightRed);
					            }
                                
                            } else if (client.Player.Dead) {
                                AskAfterDeathQuestion(client);
                            }
                        }
                        break;
                    case "TradeItem": {
                            if (answer == "Yes") {
                                int itemTaken = client.Player.FindInvSlot(110);
                                if (client.Player.FindInvSlot(109) > itemTaken) {
                                    itemTaken = client.Player.FindInvSlot(109);
                                }
                                if (client.Player.FindInvSlot(131) > itemTaken) {
                                    itemTaken = client.Player.FindInvSlot(131);
                                }
                                if (itemTaken > -1) {
                                    client.Player.TakeItemSlot(itemTaken, 1, true);
                                    client.Player.GiveItem(538, 1, "38;45;125;290;194;299;300;301;302");
                                }
                            }
                        }
                        break;
                    case "CliffsideTablet": {
                            if (answer == "Yes") {
                                if (client.Player.FindInvSlot(-1) > 0) {
                                    client.Player.GiveItem(479, 1);
                                    IMap map = client.Player.Map;
                                    map.SetTile(10, 1, 0, 10, 3);
                                    map.SetTile(10, 1, 0, 10, 4);
                                    map.SetAttribute(10, 1, Enums.TileType.Blocked, 0, 0, 0, "0", "0", "0");
                                    Messenger.SendTile(10, 1, map);
                                    Messenger.PlayerMsg(client, "You obtained a Tablet!", Text.Yellow);
                                } else {
                                    Messenger.PlayerMsg(client, "Your inventory is full...", Text.BrightRed);
                                }
                            }
                        }
                        break;
                    case "TournamentAdminOptions": {
                            if (answer == "Start Tournament") {
                                if (client.Player.Tournament != null) {
                                    Tournament tourny = client.Player.Tournament;
                                    if (tourny.RegisteredMembers[client].Admin) {
                                        if (tourny.IsReadyToStart()) {
                                            tourny.StartRound(new MatchUpRules());

                                            Story playerOneStory = new Story();
                                            StoryBuilderSegment playerOneSegment = StoryBuilder.BuildStory();
                                            StoryBuilder.AppendSaySegment(playerOneSegment, "Your match is ready to begin!", -1, 0, 0);
                                            StoryBuilder.AppendMovePlayerAction(playerOneSegment, 8, 2, Enums.Speed.Walking, true);
                                            playerOneSegment.AppendToStory(playerOneStory);

                                            Story playerTwoStory = new Story();
                                            StoryBuilderSegment playerTwoSegment = StoryBuilder.BuildStory();
                                            StoryBuilder.AppendSaySegment(playerTwoSegment, "Your match is ready to begin!", -1, 0, 0);
                                            StoryBuilder.AppendMovePlayerAction(playerTwoSegment, 11, 2, Enums.Speed.Walking, true);
                                            playerTwoSegment.AppendToStory(playerTwoStory);

                                            for (int i = 0; i < tourny.ActiveMatchups.Count; i++) {
                                                StoryManager.PlayStory(tourny.ActiveMatchups[i].PlayerOne.Client, playerOneStory);
                                                StoryManager.PlayStory(tourny.ActiveMatchups[i].PlayerTwo.Client, playerTwoStory);
                                            }
                                        } else {
                                            Messenger.PlayerMsg(client, "The tournament is not ready to start!", Text.BrightRed);
                                        }
                                    }
                                }
                            } else if (answer == "Edit Rules") {
                                if (client.Player.Tournament != null) {
                                    Tournament tourny = client.Player.Tournament;
                                    if (tourny.RegisteredMembers[client].Admin) {
                                        Messenger.SendTournamentRulesEditorTo(client, null);
                                    } else {
                                        Messenger.PlayerMsg(client, "You are not a tournament organizer!", Text.BrightRed);
                                    }
                                }
                            }
                        }
                        break;
                   case "SnowballGameNewGame": {
                   			if (answer == "Yes") {
                   				string waitingGameOwner = SnowballGame.FindWaitingGame();
                   				if (!string.IsNullOrEmpty(waitingGameOwner)) {
                   					Messenger.PlayerMsg(client, "Someone else created a game, please join that game.", Text.BrightRed);
                   					return;
                   				}
                   				SnowballGame snowballGame = new SnowballGame();
                   				snowballGame.Init();
                   				snowballGame.CreateGame(client);
                   				
                   				ActiveSnowballGames.Add(client.Player.CharID, snowballGame);
                   				Messenger.MapMsg(client.Player.MapID, client.Player.Name + " created a new game!", Text.BrightGreen);
                   			}
	                   }
	                   break;
	               case "SnowballGameJoinGame": {
                   			if (answer == "Yes") {
                   				// Join game here
                   				
                   				SnowballGame.GetWaitingGame().AddToGame(client);
                   			}
	                   }
	                   break;
	               case "SnowballGameStart": {
                   			if (answer == "Yes") {
                   				// Star game here
                   				
                   				//Messenger.MapMsg(client.Player.MapID, client.Player.Name + " started the game!", Text.BrightGreen);
                   				//Messenger.PlayerMsg(client, "Pretend you started the game.", Text.BrightGreen);
                   				SnowballGame.GetWaitingGame().StartGame(client);
                   			}
	                   }
	                   break;
                }
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: QuestionResult" + ex.ToString(), Text.Black);
            }
        }

        public static void StoryScript(Client client, int scriptNum, string param1, string param2, string param3, bool paused) {
            // Called when a script from a story is run
            try {
            
            switch (scriptNum) {
                case 0: {
                        Messenger.SendPlayerData(client);
                    }
                    break;
                case 1: {
                        if (client.Player.Map.MapType != Enums.MapType.Instanced) return;
            			InstancedMap map = client.Player.Map as InstancedMap;
                        MapNpcPreset npc = new MapNpcPreset();
                        string[] loc = param1.Split(':');
                        
                        string[] lvl = param3.Split(':');
                        npc.SpawnX = loc[0].ToInt();
                        npc.SpawnY = loc[1].ToInt();
                        npc.NpcNum = param2.ToInt();
                        npc.MinLevel = lvl[0].ToInt(); // TODO: Adjust npc level
                        npc.MaxLevel = lvl[1].ToInt(); // TODO: Adjust npc level
                        //Messenger.AdminMsg(npc.SpawnX+"/"+npc.SpawnY+"/"+npc.NpcNum+"/"+npc.MinLevel+"/"+npc.MaxLevel, Text.Pink);
                        map.SpawnNpc(npc);
                        
                    }
                    break;
                case 2: {
                        if (client.Player.Map.MapType != Enums.MapType.Instanced) return;
            			InstancedMap map = client.Player.Map as InstancedMap;
            			string[] loc = param1.Split(':');
            			map.SetTile(loc[0].ToInt(), loc[1].ToInt(), 3, 4, 3);
                        map.SetAttribute(loc[0].ToInt(), loc[1].ToInt(), Enums.TileType.Scripted, 46, 0, 0, param2, "-1", "");
                        Messenger.SendTile(loc[0].ToInt(), loc[1].ToInt(), map);
                    }
                    break;
                case 3: {
                        if (client.Player.Map.MapType != Enums.MapType.Instanced) return;
            			InstancedMap map = client.Player.Map as InstancedMap;
            			string[] loc = param1.Split(':');
            			string[] dest = param2.Split(':');
            			map.SetTile(loc[0].ToInt(), loc[1].ToInt(), 3, 4, 3);
                        map.SetAttribute(loc[0].ToInt(), loc[1].ToInt(), Enums.TileType.Warp, dest[0].ToInt(), dest[1].ToInt(), dest[2].ToInt(), "", "", "");
                        Messenger.SendTile(loc[0].ToInt(), loc[1].ToInt(), map);
                    }
                    break;
                case 4: {
                        IMap map = client.Player.Map;
                        map.Npc[1].SpawnX = 9;
                        map.Npc[1].SpawnY = 7;
                        map.Npc[2].SpawnX = 6;
                        map.Npc[2].SpawnY = 11;
                        map.Npc[3].SpawnX = 13;
                        map.Npc[3].SpawnY = 11;
                        map.Npc[1].NpcNum = 306;
                        map.Npc[2].NpcNum = 259;
                        map.Npc[3].NpcNum = 259;
                        //map.SetNpc(0, 500);
                        //map.SetNpc(1, 500);
                        //map.SetNpc(2, 500);
                        for (int i = 1; i <= 3; i++) {
                            //map.SpawnNpc(i);
                        }
                    }
                    break;
                case 5: {
                        //sound
                        Messenger.PlaySoundToMap(client.Player.Map.MapID, param1);
                    }
                    break;
                case 6: {
                        IMap map = client.Player.Map;
                        map.Npc[1].SpawnX = 9;
                        map.Npc[1].SpawnY = 3;
                        map.Npc[1].NpcNum = 337;
                        //map.SpawnNpc(1);
                    }
                    break;
                case 7: {// Luxio Tribe Battle
                        // 167 - Luxio
                        // 177 - Luxray
                        // 5 luxio, 1 luxray
                        IMap map = client.Player.Map;
                        map.Npc[1].SpawnX = 10;
                        map.Npc[1].SpawnY = 5;
                        map.Npc[2].SpawnX = 7;
                        map.Npc[2].SpawnY = 6;
                        map.Npc[3].SpawnX = 12;
                        map.Npc[3].SpawnY = 6;
                        map.Npc[4].SpawnX = 7;
                        map.Npc[4].SpawnY = 8;
                        map.Npc[5].SpawnX = 12;
                        map.Npc[5].SpawnY = 8;
                        map.Npc[6].SpawnX = 12;
                        map.Npc[6].SpawnY = 9;
                        map.Npc[7].SpawnX = 10;
                        map.Npc[7].SpawnY = 7;
                        map.Npc[1].NpcNum = 177;
                        for (int i = 2; i <= 7; i++) {
                            map.Npc[i].NpcNum = 167;
                        }
                        for (int i = 1; i <= 7; i++) {
                            //map.SpawnNpc(i);
                        }
                    }
                    break;
                case 8: {
                        client.Player.GiveItem(159, 1);
                    }
                    break;
                case 9: {
                        client.Player.SetStoryState(157, false);
                    }
                    break;
                case 10: {//Start Registeel Battle: First wave of bronzongs
                        BossBattles.StartBossWave(client, "Registeel", 1);
                    }
                    break;
                case 11: {//Registeel Battle: second wave of bronzongs
                        BossBattles.StartBossWave(client, "Registeel", 2);
                    }
                    break;
                case 12: {//Registeel Battle: Registeel itself
                        BossBattles.StartBossWave(client, "Registeel", 3);
                    }
                    break;
                case 13: {//leaving Forlorn Tower by fee
                        int amount = client.Player.HasItem(1);
                        if (amount > 0) {
                            client.Player.TakeItem(1, (amount / 2));
                        }
                        Messenger.PlayerWarp(client, 684, 13, 20);
                    }
                    break;
                case 14: {
                        if (exPlayer.Get(client).StoryEnabled) {
                            int currentSection = client.Player.StoryHelper.ReadSetting("[MainStory]-CurrentSection").ToInt();
                            client.Player.StoryHelper.SaveSetting("[MainStory]-CurrentSection", (currentSection + 1).ToString());
                        }
                    }
                    break;
                case 15: {//murkrows attack (wave 1)
                        IMap map = client.Player.Map;

                        map.SetAttribute(7, 10, Enums.TileType.Scripted, 3, 0, 0, "", "", "");
                        map.SetAttribute(13, 10, Enums.TileType.Scripted, 3, 0, 0, "", "", "");
                        map.SetAttribute(7, 14, Enums.TileType.Scripted, 3, 0, 0, "", "", "");
                        map.SetAttribute(13, 14, Enums.TileType.Scripted, 3, 0, 0, "", "", "");
                        map.SetAttribute(4, 18, Enums.TileType.Scripted, 3, 0, 0, "", "", "");
                        map.SetAttribute(16, 18, Enums.TileType.Scripted, 3, 0, 0, "", "", "");
                        map.SetAttribute(4, 8, Enums.TileType.Scripted, 3, 0, 0, "", "", "");
                        map.SetAttribute(16, 8, Enums.TileType.Scripted, 3, 0, 0, "", "", "");

                        map.SetNpcSpawnPoint(1, 3, 10);
                        map.SetNpcSpawnPoint(2, 4, 11);
                        map.SetNpcSpawnPoint(3, 6, 10);
                        map.SetNpcSpawnPoint(4, 7, 11);
                        map.SetNpcSpawnPoint(5, 8, 10);
                        map.SetNpcSpawnPoint(6, 9, 11);
                        map.SetNpcSpawnPoint(7, 10, 10);
                        map.SetNpcSpawnPoint(8, 11, 11);
                        map.SetNpcSpawnPoint(9, 12, 10);
                        map.SetNpcSpawnPoint(10, 13, 11);
                        map.SetNpcSpawnPoint(11, 14, 10);
                        map.SetNpcSpawnPoint(12, 16, 11);
                        map.SetNpcSpawnPoint(13, 17, 10);

                        for (int i = 1; i <= 13; i++) {
                            map.Npc[i].NpcNum = 224;
                        }

                        //map.SpawnNpc(1);
                        //map.SpawnNpc(2);
                        //map.SpawnNpc(3);
                        //map.SpawnNpc(4);
                        //map.SpawnNpc(5);
                        //map.SpawnNpc(6);
                        //map.SpawnNpc(7);
                        //map.SpawnNpc(8);
                        //map.SpawnNpc(9);
                        //map.SpawnNpc(10);
                        //map.SpawnNpc(11);
                        //map.SpawnNpc(12);
                        //map.SpawnNpc(13);



                    }
                    break;
                case 16: {//murkrows attack (ambush)
                        IMap map = client.Player.Map;

                        map.SetAttribute(7, 10, Enums.TileType.Scripted, 3, 0, 0, "", "", "");
                        map.SetAttribute(13, 10, Enums.TileType.Scripted, 3, 0, 0, "", "", "");
                        map.SetAttribute(7, 14, Enums.TileType.Scripted, 3, 0, 0, "", "", "");
                        map.SetAttribute(13, 14, Enums.TileType.Scripted, 3, 0, 0, "", "", "");
                        map.SetAttribute(4, 18, Enums.TileType.Scripted, 3, 0, 0, "", "", "");
                        map.SetAttribute(16, 18, Enums.TileType.Scripted, 3, 0, 0, "", "", "");
                        map.SetAttribute(4, 8, Enums.TileType.Scripted, 3, 0, 0, "", "", "");
                        map.SetAttribute(16, 8, Enums.TileType.Scripted, 3, 0, 0, "", "", "");

                        map.SetNpcSpawnPoint(1, 9, 7);
                        map.SetNpcSpawnPoint(2, 10, 7);
                        map.SetNpcSpawnPoint(3, 11, 7);
                        map.SetNpcSpawnPoint(4, 9, 8);
                        map.SetNpcSpawnPoint(5, 11, 8);
                        map.SetNpcSpawnPoint(6, 9, 9);
                        map.SetNpcSpawnPoint(7, 10, 9);
                        map.SetNpcSpawnPoint(8, 11, 9);
                        map.SetNpcSpawnPoint(9, 8, 8);
                        map.SetNpcSpawnPoint(10, 8, 9);
                        map.SetNpcSpawnPoint(11, 12, 8);
                        map.SetNpcSpawnPoint(12, 12, 9);
                        map.SetNpcSpawnPoint(13, 10, 10);

                        for (int i = 1; i <= 13; i++) {
                            map.Npc[i].NpcNum = 220;
                        }

                        //map.SpawnNpc(1);
                        //map.SpawnNpc(2);
                        //map.SpawnNpc(3);
                        //map.SpawnNpc(4);
                        //map.SpawnNpc(5);
                        //map.SpawnNpc(6);
                        //map.SpawnNpc(7);
                        //map.SpawnNpc(8);
                        //map.SpawnNpc(9);
                        //map.SpawnNpc(10);
                        //map.SpawnNpc(11);
                        //map.SpawnNpc(12);
                        //map.SpawnNpc(13);

                        if (client.Player.GetActiveRecruit().HeldItem.Num != 142) {
                            Messenger.BattleMsg(client, "There was a trap underneath!", Text.BrightRed);
                            Messenger.BattleMsg(client, "The trap exploded!", Text.BrightRed);
                            //Messenger.SpellAnim(497, client.Player.MapID, client.Player.X, client.Player.Y);
                            //DamagePlayer(client, Convert.ToInt32((client.Player.GetActiveRecruit().HP + 0.5) / 2), Enums.KillType.Npc);

                            Messenger.PlaySoundToMap(client.Player.MapID, "magic64.wav");
                            Messenger.BattleMsg(client, "The trap drained all your PP!", Text.BrightRed);
                            //Messenger.SpellAnim(493, client.Player.MapID, client.Player.X, client.Player.Y);
                            for (int i = 0; i < client.Player.GetActiveRecruit().Moves.Length; i++) {
                                client.Player.GetActiveRecruit().Moves[i].CurrentPP = 0;
                                //Messenger.SendMovePPUpdate(client, i);
                            }
                            Messenger.PlaySound(client, "PP Zero.wav");
                            Messenger.BattleMsg(client, "Poison spikes shot out!", Text.BrightRed);
                            //Messenger.SpellAnim(496, client.Player.MapID, client.Player.X, client.Player.Y);
                            Messenger.PlaySoundToMap(client.Player.MapID, "magic23.wav");
                            SetStatusAilment(client.Player.GetActiveRecruit(), client.Player.Map, Enums.StatusAilment.Poison, 1, null);
                            Messenger.BattleMsg(client, "Mud splashed up from the ground!", Text.BrightRed);
                            //Messenger.SpellAnim(495, client.Player.MapID, client.Player.X, client.Player.Y);
                            Messenger.PlaySoundToMap(client.Player.MapID, "magic65.wav");
                            switch (Server.Math.Rand(0, 4)) {
                                case 0: {
                                        client.Player.GetActiveRecruit().AttackBuff = 0;
                                    }
                                    break;
                                case 1: {
                                        client.Player.GetActiveRecruit().DefenseBuff = 0;
                                    }
                                    break;
                                case 2: {
                                        client.Player.GetActiveRecruit().SpAtkBuff = 0;
                                    }
                                    break;
                                case 3: {
                                        client.Player.GetActiveRecruit().SpeedBuff = 0;
                                    }
                                    break;
                            }
                            //Messenger.SpellAnim(494, client.Player.MapID, client.Player.X, client.Player.Y);
                            //Grimy(client);
                            Messenger.PlaySoundToMap(client.Player.MapID, "Magic65.wav");
                            //PlayerInvDrop(client);
                            Messenger.MapMsg(client.Player.MapID, client.Player.Name + " tripped!", Text.BrightRed);
                            Messenger.PlaySoundToMap(client.Player.MapID, "magic55.wav");
                            Messenger.BattleMsg(client, "You were pelted with chestnuts!", Text.BrightRed);
                            //DamagePlayer(client, 50, Enums.KillType.Npc);

                            Messenger.PlaySoundToMap(client.Player.MapID, "pain.wav");
                            Messenger.BattleMsg(client, "The trap broke down...", Text.Black);
                        }
                    }
                    break;
                case 17: {//murkrows and honchkrow attack
                        IMap map = client.Player.Map;
                        map.SetNpcSpawnPoint(1, 6, 9);
                        map.SetNpcSpawnPoint(2, 14, 9);
                        map.SetNpcSpawnPoint(3, 6, 11);
                        map.SetNpcSpawnPoint(4, 14, 11);
                        map.SetNpcSpawnPoint(5, 5, 13);
                        map.SetNpcSpawnPoint(6, 15, 13);
                        map.SetNpcSpawnPoint(7, 6, 15);
                        map.SetNpcSpawnPoint(8, 14, 15);
                        map.SetNpcSpawnPoint(9, 8, 14);
                        map.SetNpcSpawnPoint(10, 12, 14);
                        map.SetNpcSpawnPoint(11, 6, 18);
                        map.SetNpcSpawnPoint(12, 14, 18);
                        map.SetNpcSpawnPoint(13, 10, 16);
                        map.SetNpcSpawnPoint(14, 10, 7);

                        map.SetNpc(1, 220);
                        map.SetNpc(2, 220);
                        map.SetNpc(3, 220);
                        map.SetNpc(4, 220);
                        map.SetNpc(5, 220);
                        map.SetNpc(6, 220);
                        map.SetNpc(7, 220);
                        map.SetNpc(8, 220);
                        map.SetNpc(9, 220);
                        map.SetNpc(10, 220);
                        map.SetNpc(11, 220);
                        map.SetNpc(12, 220);
                        map.SetNpc(13, 220);
                        map.SetNpc(14, 221);

                        //map.SpawnNpc(1);
                        //map.SpawnNpc(2);
                        //map.SpawnNpc(3);
                        //map.SpawnNpc(4);
                        //map.SpawnNpc(5);
                        //map.SpawnNpc(6);
                        //map.SpawnNpc(7);
                        //map.SpawnNpc(8);
                        //map.SpawnNpc(9);
                        //map.SpawnNpc(10);
                        //map.SpawnNpc(11);
                        //map.SpawnNpc(12);
                        //map.SpawnNpc(13);
                        //map.SpawnNpc(14);


                    }
                    break;
                case 18: {//honchkrow defeated, warp opens.
                        IMap map = client.Player.Map;
                        map.SetTile(10, 6, 96, 4, 3);
                        map.SetTile(10, 5, 112576, 9, 3);
                        map.SetTile(10, 4, 112128, 9, 3);
                        // Left old code in case I did the math wrong...
                        //map.SetTile(10, 5, 128, 8032, 9, 3);
                        //map.SetTile(10, 4, 128, 8000, 9, 3);
                        map.SetAttribute(10, 6, Enums.TileType.Scripted, 46, 0, 0, "16", "683", "9:16");
                        Messenger.SendTile(10, 4, map);
                        Messenger.SendTile(10, 5, map);
                        Messenger.SendTile(10, 6, map);
                    }
                    break;
                case 19: {//daniel's fireplace question
                        if (client.Player.GetActiveRecruit().HasMove(452)) {
                            Messenger.AskQuestion(client, "Fireplace", "Will you use Rock Smash on the fireplace?", -1);
                        }
                    }
                    break;
                case 20: {//go into abandoned mansion
                        Messenger.AskQuestion(client, "AbandonedMansion2", "Only the team leader may enter this dungeon, is that OK?", -1);

                    }
                    break;
                case 21: {//vs. gengar
                        IMap map = client.Player.Map;
                        if (map.MapType != Enums.MapType.Instanced) {
                            client.CloseConnection();
                            return;
                        }
                        map.SetNpcSpawnPoint(1, 11, 6);
                        map.SetNpcSpawnPoint(2, 12, 8);
                        map.SetNpcSpawnPoint(3, 14, 8);
                        map.SetNpc(1, 804);
                        map.SetNpc(2, 809);
                        map.SetNpc(3, 812);
                        //map.SpawnNpc(1);
                        //map.SpawnNpc(2);
                        //map.SpawnNpc(3);

                    }
                    break;
                case 22: {//vs. Rotom
                        IMap map = client.Player.Map;

                        if (map.Name == "West Wing End") {
                            InstancedMap mapa = new InstancedMap(MapManager.GenerateMapID("i"));
                            MapCloner.CloneMapTiles(MapManager.RetrieveMap(825), mapa);
                            Messenger.PlayerWarp(client, mapa, 9, 10);
                            mapa.SetNpcSpawnPoint(1, 9, 3);
                            mapa.SetNpc(1, 806);
                            //mapa.SpawnNpc(1);

                        } else if (map.Name == "East Wing End") {


                            InstancedMap mapa = new InstancedMap(MapManager.GenerateMapID("i"));
                            MapCloner.CloneMapTiles(MapManager.RetrieveMap(826), mapa);
                            Messenger.PlayerWarp(client, mapa, 10, 10);
                            mapa.SetNpcSpawnPoint(1, 10, 3);
                            mapa.SetNpc(1, 807);
                            //mapa.SpawnNpc(1);

                        }
                    }
                    break;
                case 23: {//go into Hallowed Well
                        Messenger.AskQuestion(client, "HallowedWell2", "Only the team leader may enter this dungeon, and no Pokémon may be recruited.  Is that OK?", -1);
                    }
                    break;
                case 24: {//spiritomb battle
                        client.Player.TakeItem(251, 1);
                        IMap map = client.Player.Map;
                        map.SetNpcSpawnPoint(1, 9, 8);
                        map.SetNpc(1, 805);
                        //map.SpawnNpc(1);
                    }
                    break;
                case 25: {//spiritomb joins
                        // Fixed, uncomment when needed
                        // TODO: Spritomb joins
                        //client.Player.AddToRecruitmentBank(37, 209, 40, (ulong)1, 1, 0, 0, 40, 55, 10, 40, 1, 805, "Spiritomb");
                        Messenger.PlaySoundToMap(client.Player.MapID, "JoinGame.wav");
                    }
                    break;
                case 26: {//learn wish
                        client.Player.GetActiveRecruit().LearnNewMove(268);
                    }
                    break;
                case 27: {//vs. Articuno*
                        IMap map = client.Player.Map;
                        map.SetNpc(1, 440);
                        map.SetNpcSpawnPoint(1, 12, 11);
                        //map.SpawnNpc(1);
                    }
                    break;
                case 28: {//Give Natural Gift
                        Messenger.PlayerMsg(client, "You were given a Natural Gift!", Text.Yellow);
                        client.Player.GiveItem(232, 1);
                    }
                    break;
                case 29: {//Give Holly Present
                        Messenger.PlayerMsg(client, "You were given a Holly Present!", Text.Yellow);
                        client.Player.GiveItem(233, 1);
                    }
                    break;
                case 30: {//Give Mystery Gift
                        Messenger.PlayerMsg(client, "You were given a Mystery Gift!", Text.Yellow);
                        client.Player.GiveItem(234, 1);
                    }
                    break;
                case 31: {//Give PMU Present
                        Messenger.PlayerMsg(client, "You were given a PMU Present!", Text.Yellow);
                        client.Player.GiveItem(231, 1);
                    }
                    break;
                case 32: {//take white herb
                        Messenger.PlayerMsg(client, "You gave away some Candy Canes.", Text.Yellow);
                        client.Player.TakeItem(212, 12);
                    }
                    break;
                case 33: {//take soft string
                        Messenger.PlayerMsg(client, "You gave away some Soft String.", Text.Yellow);
                        client.Player.TakeItem(208, 20);
                    }
                    break;
                case 34: {//take soft snow
                        Messenger.PlayerMsg(client, "You gave away some Soft Snow.", Text.Yellow);
                        client.Player.TakeItem(209, 50);
                    }
                    break;
                case 35: {//Take ornaments
                        client.Player.GiveItem(174, 1);
                        Messenger.PlayerMsg(client, "You obtained a Shell Bell!", Text.Yellow);
                    }
                    break;
                case 36: {//vs. Heatran
                        IMap map = client.Player.Map;
                        map.SetNpc(1, 404);
                        map.SetNpcSpawnPoint(1, 12, 11);
                        //map.SpawnNpc(1);
                    }
                    break;
                case 37: {//vs. Xatu
                		BossBattles.RunPostIntroStoryBossBattle(client, "Pitch-Black Pit");
                    }
                    break;
                case 38: {
                        BossBattles.RunPostIntroStoryBossBattle(client, "TinyGrotto");
                    }
                    break;
                case 39: {

                    BossBattles.RunBossBattleCleanup(client, client.Player.Map.Name);
                    }
                    break;
                case 40: {
                        client.Player.GiveItem(110, 1, "133;15");
                    }
                    break;
                case 41: { // Post intro boss battle for Seaside Cavern
                        BossBattles.RunPostIntroStoryBossBattle(client, "SeasideCavern");
                    }
                    break;
                case 42: { // enter dungeon (random)
                		
                		DungeonRules.StartRDungeon(client, param1, param2, param3);
                    }
                    break;
                case 43: {
                        BossBattles.StartBossWave(client, "ElectrostasisTowerA1", 1);
                    }
                    break;
                case 44: {
                        BossBattles.RunBossBattleCleanup(client, "ElectrostasisTowerA1");
                    }
                    break;
                case 45: {
                        BossBattles.RunPostIntroStoryBossBattle(client, "ElectrostasisTowerA2");
                    }
                    break;
                case 46: {
                        //battle fossil
                        int npcNum = -1;
                        switch (param1.ToInt()) {
                        	case 138: //omanyte
                        		npcNum = 272;
                        	break;
                        	case 140: //kabuto
                        		npcNum = 250;
                        	break;
                        	case 142: //aerodactyl
                        		npcNum = 132;
                        	break;
                        	case 345: //lileep
                        		npcNum = 956;
                        	break;
                        	case 347: //anorith
                        		npcNum = 957;
                        	break;
                        	case 408: //cranidos
                        		npcNum = 959;
                        	break;
                        	case 410: //shieldon
                        		npcNum = 958;
                        	break;
                        	case 566: //archen
                        		npcNum = 973;
                        	break;
                        	//case 566: //tirtouga
                        	//	npcNum = 979;
                        	//break;
                        }
                        if (npcNum > 0) {
                        	
                            MapNpcPreset npc = new MapNpcPreset();
                            npc.SpawnX = 9;
                            npc.SpawnY = 3;
                            npc.NpcNum = npcNum;
                            npc.MinLevel = param2.ToInt();
                            npc.MaxLevel = param2.ToInt();
                            client.Player.Map.SpawnNpc(npc);
                        }
                    }
                    break;
                case 47: {//egg hatch
                        int item = -1;
                        string[] recruitArgs = null;
                        for (int i = 1; i <= client.Player.MaxInv; i++) {
                            if (client.Player.Inventory[i].Num == 131) {
                                string[] eggArgs = client.Player.Inventory[i].Tag.Split(';');
                                if (eggArgs.Length <= 1 || !eggArgs[1].IsNumeric()) {
                                    client.Player.TakeItemSlot(i, 1, true);
                                } else {
                                    int step = eggArgs[1].ToInt();
                                    if (step <= 1) {
                                        item = i;
                                        recruitArgs = eggArgs[0].Split(',');
                                    }
                                    break;
                                }
                            }
                        }

                        if (item != -1) {

                            Recruit recruit = new Recruit(client);
                            //recruit.SpriteOverride = -1;
                            recruit.Level = 1;
                            recruit.Species = recruitArgs[0].ToInt();
                            recruit.Sex = Pokedex.GetPokemonForm(recruitArgs[0].ToInt()).GenerateLegalSex();
                            recruit.Name = Pokedex.GetPokemon(recruitArgs[0].ToInt()).Name;
                            recruit.NpcBase = 0;

                            recruit.GenerateMoveset();

                            //for egg moves, where applicable
                            List<int> eggMoves = GenerateEggMoveset(recruit.Species, recruit.Form, -1);

							for (int i = 0; i < 4; i++) {
								if (eggMoves.Count > 0) {
									int moveNum = eggMoves[Server.Math.Rand(0, eggMoves.Count)];
                                    recruit.Moves[i] = new RecruitMove();
                                    recruit.Moves[i].MoveNum = moveNum;
                                    recruit.Moves[i].MaxPP = Server.Moves.MoveManager.Moves[moveNum].MaxPP;
                                    recruit.Moves[i].CurrentPP = recruit.Moves[i].MaxPP;
                                    eggMoves.Remove(moveNum);
								}
							}

                            for (int i = 0; i < 4; i++) {
                                if (eggMoves.Contains(client.Player.GetActiveRecruit().Moves[i].MoveNum)) {
                                	int moveNum = client.Player.GetActiveRecruit().Moves[i].MoveNum;
                                    recruit.Moves[i] = new RecruitMove();
                                    recruit.Moves[i].MoveNum = moveNum;
                                    recruit.Moves[i].MaxPP = Server.Moves.MoveManager.Moves[moveNum].MaxPP;
                                    recruit.Moves[i].CurrentPP = recruit.Moves[i].MaxPP;
                                    eggMoves.Remove(client.Player.GetActiveRecruit().Moves[i].MoveNum);
                                }
                            }


                            int openSlot = client.Player.FindOpenTeamSlot();
                            int recruitIndex = -1;
                            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                                recruitIndex = client.Player.AddToRecruitmentBank(dbConnection, recruit);
                            }


                            if (recruitIndex != -1) {
                                client.Player.AddToTeam(recruitIndex, openSlot);
                                client.Player.Team[openSlot].HP = client.Player.Team[openSlot].MaxHP;
                                Messenger.BattleMsg(client, "You have recruited a new team member!", Text.BrightGreen);

                                Messenger.SendActiveTeam(client);
                            }

                            client.Player.TakeItemSlot(item, 1, true);

                            if (recruitArgs.Length > 1) client.Player.GiveItem(recruitArgs[1].ToInt(), 1);

                        }

                    }
                    break;
                case 48: {//teach egg move

                        int species = client.Player.GetActiveRecruit().Species;

                        if (Server.Evolutions.EvolutionManager.FindPreEvolution(species) > -1) {
                            species = Server.Evolutions.EvolutionManager.FindPreEvolution(species);
                        }
                        if (Server.Evolutions.EvolutionManager.FindPreEvolution(species) > -1) {
                            species = Server.Evolutions.EvolutionManager.FindPreEvolution(species);
                        }

                        List<int> eggMoves = GenerateEggMoveset(species, client.Player.GetActiveRecruit().Form, -1);

                        Messenger.PlayerMsg(client, "Poof!", Text.BrightGreen);
                        Messenger.PlaySound(client, "magic22.wav");
                        Messenger.SpellAnim(504, client.Player.MapID, client.Player.X, client.Player.Y);
                        if (eggMoves.Count > 0) {
                            client.Player.GetActiveRecruit().LearnNewMove(eggMoves[Server.Math.Rand(0, eggMoves.Count)]);
                            Messenger.SendPlayerMoves(client);
                        } else {
                            Messenger.PlayerMsg(client, "But nothing happened...", Text.Grey);
                        }

                    }
                    break;
                case 49: {//take egg

                        int item = -1;
                        string[] recruitArgs = null;
                        for (int i = 1; i <= client.Player.MaxInv; i++) {
                            if (client.Player.Inventory[i].Num == 131) {
                                string[] eggArgs = client.Player.Inventory[i].Tag.Split(';');
                                if (eggArgs.Length <= 1 || !eggArgs[1].IsNumeric()) {
                                    client.Player.TakeItemSlot(i, 1, true);
                                } else {
                                    int step = eggArgs[1].ToInt();
                                    if (step <= 1) {
                                        item = i;
                                        recruitArgs = eggArgs[0].Split(',');
                                    }
                                    break;
                                }
                            }
                        }

                        if (item != -1) {
                            client.Player.TakeItemSlot(item, 1, true);
                        }

                    }
                    break;
                case 50: {
                        BossBattles.StartBossWave(client, "ElectrostasisTowerA1", 2);
                    }
                    break;
                case 51: {
                        BossBattles.RunPostIntroStoryBossBattle(client, "CliffsideRelic");
                    }
                    break;
                case 52: {//enter dungeon (normal map)
                		DungeonRules.StartDungeon(client, param1, param2, param3);
                    }
                    break;
                case 53: {
                        BossBattles.RunPostIntroStoryBossBattle(client, "ThunderstormForest");
                    }
                    break;
                case 54: {
                        BossBattles.RunPostIntroStoryBossBattle(client, "ThunderstormForestPart2");
                    }
                    break;
                case 55: { // Open tournament listing
                        Messenger.SendTournamentListingTo(client, null);
                    }
                    break;
                case 56: {
                        BossBattles.RunPostIntroStoryBossBattle(client, "SourRootMini");
                    }
                    break;
                case 57: {//start boss battle
                        BossBattles.PrepareBossRoom(client, (InstancedMap)client.Player.Map);
                    }
                    break;
                case 58: {
                        BossBattles.RunPostIntroStoryBossBattle(client, "SourRoot");
                    }
                    break;
                case 59: {//run post intro story boss battle

                        BossBattles.RunPostIntroStoryBossBattle(client, param1);
                    }
                    break;
                case 60: {
                        BossBattles.RunPostIntroStoryBossBattle(client, "SaunaCavernMini");
                    }
                    break;
                case 61: {//enter arena
                		for (int i = 1; i <= client.Player.MaxInv; i++) {
                            if (client.Player.Inventory[i].Num > 0
                                && ItemManager.Items[client.Player.Inventory[i].Num].Type != Enums.ItemType.Held && ItemManager.Items[client.Player.Inventory[i].Num].Type != Enums.ItemType.HeldByParty
                                && ItemManager.Items[client.Player.Inventory[i].Num].Type != Enums.ItemType.HeldInBag) {
                                bool held = false;

                                for (int j = 0; j < Constants.MAX_ACTIVETEAM; j++) {
                                	if (client.Player.Team[j] != null
                                    	&& client.Player.Team[j].HeldItemSlot == i) {
                                    	held = true;
                                	}

                            	}

                            	if (!held) {
                            		int slot = i;
	                                int amount = client.Player.Inventory[slot].Amount;
	                                int X = client.Player.Inventory[slot].Num;
	                                string tag = client.Player.Inventory[slot].Tag;
	                                int j = client.Player.FindBankSlot(X, amount);
	                                if (j == -1) {
	                                    Messenger.PlayerMsg(client, "The storage can't store all items!", Text.BrightRed);
	                                    return;
	                                }

	                                client.Player.TakeItemSlot(slot, amount, true);
	                                client.Player.GiveBankItem(X, amount, tag, j);
                            	
                            	}
                        	}
                    	}
                        Messenger.PlayerMsg(client, "Items were sent to storage!", Text.Yellow);
                		EnterArena(client, client.Player.GetActiveRecruit(), client.Player.Map, param2, param3, null);
                	}
                	break;
                case 62: {
                        BossBattles.RunPostIntroStoryBossBattle(client, "SaunaCavern");
                    }
                    break;
                case 64: {
                        BossBattles.RunPostIntroStoryBossBattle(client, "CrystalRuins");
                    }
                    break;
                case 66: {
                        BossBattles.RunPostIntroStoryBossBattle(client, "FrozenCitadel");
                    }
                    break;
                case 68: {
                        BossBattles.RunPostIntroStoryBossBattle(client, "SouthernSea");
                    }
                    break;
                case 69: {
                        BossBattles.RunPostIntroStoryBossBattle(client, "LenilePit");
                    }
                    break;
                case 70: {
                		Messenger.PlayerWarp(client, 1035, 9, 7);
	                }
	                break;
	            case 71: {//teach tutor move
	            		int species = client.Player.GetActiveRecruit().Species;


                        List<int> eggMoves = GenerateTutorMoveset(species, client.Player.GetActiveRecruit().Form, param1.ToInt());

                        Messenger.PlayerMsg(client, "GOOOOONG!", Text.BrightGreen);
                        Messenger.PlaySound(client, "magic1011.wav");
                        Messenger.SpellAnim(498, client.Player.MapID, client.Player.X, client.Player.Y);
                        if (eggMoves.Count > 0) {
                            client.Player.GetActiveRecruit().LearnNewMove(eggMoves[Server.Math.Rand(0, eggMoves.Count)]);
                            Messenger.SendPlayerMoves(client);
                        } else {
                            Messenger.PlayerMsg(client, "But nothing happened...", Text.Grey);
                        }
	            	}
	            	break;
            }
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: StoryScript " + scriptNum + ": " + param1 + "-" + param2 + "-" + param3, Text.Black);
                
                Messenger.AdminMsg(ex.ToString(), Text.Black);
            }
        }



        public static void ScriptedNPC(Client client, int script) {
            switch (script) {
                case 1: {
                    }
                    break;
                case 2: {
                        if (string.IsNullOrEmpty(client.Player.GuildName)) {

                        } else if (client.Player.GuildName == "Team Solar Flare") {
                            Messenger.PlayerMsg(client, "Honchkrow: Hello, " + client.Player.Name + ".", Text.Green);
                        } else {
                            Messenger.PlayerMsg(client, "Honchkrow: Hello. Quit your team. I'll make you an offer you can't refuse...", Text.Green);
                        }
                    }
                    break;
                case 3: {
                        if (client.Player.MapID == MapManager.GenerateMapID(684)) {
                            Messenger.AskQuestion(client, "WarptoTanren", "Would you like a ride back to Tanren Town?", 506);
                        } else {
                            Messenger.AskQuestion(client, "WarptoForlorn", "Would you like a ride to Forlorn Tower?", 506);
                        }
                        //if (NetScript.GetPlayerMap(index) == 1109) {
                        //    NetScript.AskQuestion(index, "WarptoTanren", "Would you like a ride back to Winden?", 506);
                        //} else {
                        //    NetScript.AskQuestion(index, "WarptoForlorn", "Would you like a ride to Faraway Garden?", 506);
                        //}
                    }
                    break;
                case 4: {
                        Messenger.PlayerMsg(client, "You cannot attack Sprinko!", Text.BrightBlue);
                    }
                    break;
                case 5: {
                        Messenger.PlaySoundToMap(client.Player.MapID, "Pichu!.wav");
                    }
                    break;
                case 6: {// The snowflake collector
                        
                    }
                    break;
                case 7: {//Kecleons (so people don't accidentally attack them and unleash their rage)
                        Messenger.PlayerMsg(client, "Kelceon: Ah, welcome~!  Feel free to browse among my wares~!", Text.Green);
                    }
                    break;
                case 8: {//blue gives magma rock
                        Messenger.PlayerMsg(client, "Long ago, I used to be an explorer just like you. If only you were as experienced as me, then you could understand the greatness I achieved...", Text.Green);
                    }
                    break;
                case 9: {//heat wave move tutor
                        if (client.Player.HasItem(263) > 19 && client.Player.HasItem(264) > 19 && client.Player.HasItem(265) > 19 && client.Player.GetActiveRecruit().HasMove(66) == false) {
                            if (client.Player.GetActiveRecruit().Type1 == Enums.PokemonType.Fire || client.Player.GetActiveRecruit().Type2 == Enums.PokemonType.Fire) {
                                Messenger.AskQuestion(client, "HeatWave", "Will you give 40 red shards, 20 green shards, and 20 blue shards to learn Heat Wave?", 3);
                            }
                        }
                    }
                    break;
                case 10: {//Shell Bell Trade
                        int shells = 0;
                        int salts = 0;
                        for (int i = 1; i <= client.Player.MaxInv; i++) {
                            if (client.Player.Inventory[i].Num == 96) {
                                shells++;
                            } else if (client.Player.Inventory[i].Num == 102) {
                                salts++;
                            }
                        }
                        if (shells > 4 && salts > 4) {
                            Messenger.AskQuestion(client, "ShellBell", "Will you give me five Shoal Salts and Shoal Shells for a Shell Bell?", 218);
                        } else {
                            StoryManager.PlayStory(client, 799);
                        }
                    }
                    break;
                case 11: {//Wish move tutor
                        StoryManager.PlayStory(client, 315);
                        StoryManager.PlayStory(client, 316);
                    }
                    break;
                case 12: {//Ornaments needed; PMU Present given
                        if (client.Player.GetStoryState(220) == true) {
                            StoryManager.PlayStory(client, 221);
                        } else if (client.Player.HasItem(235) > 39) {
                            StoryManager.PlayStory(client, 220);
                        } else {
                            StoryManager.PlayStory(client, 219);
                        }
                    }
                    break;
                case 13: {//Snowballs needed; Mystery Gift given
                        if (client.Player.GetStoryState(224) == true) {
                            StoryManager.PlayStory(client, 225);
                        } else if (client.Player.HasItem(209) > 49) {
                            StoryManager.PlayStory(client, 224);
                        } else {
                            StoryManager.PlayStory(client, 223);
                        }
                    }
                    break;
                case 14: {//Yarn needed; Holly Present given
                        if (client.Player.GetStoryState(228) == true) {
                            StoryManager.PlayStory(client, 229);
                        } else if (client.Player.HasItem(208) > 19) {
                            StoryManager.PlayStory(client, 228);
                        } else {
                            StoryManager.PlayStory(client, 227);
                        }
                    }
                    break;
                case 15: {//Candy needed; Natural Gift given
                        if (client.Player.HasItem(212) > 11) {
                            StoryManager.PlayStory(client, 232);
                        } else {
                            StoryManager.PlayStory(client, 231);
                        }
                    }
                    break;

            }
        }

        public static void OnPlayerVoidLoad(Client client, Server.Maps.Void mapVoid) {
            try {
                MapCloner.CloneMapTiles(MapManager.RetrieveMap(1015), mapVoid);
                mapVoid.Music = "PMD2) Battle With Dialga.ogg";
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: OnPlayerVoidLoad", Text.Black);
            }
        }

        public static void OpenStaffElevator(Client client) {
            if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                Server.CustomMenus.CustomMenu menu = client.Player.CustomMenuManager.CreateMenu("Staff Elevator", "", true);
                menu.UpdateSize(300, 350);
                menu.AddLabel(0, 2, 3, 268, 29, "Select a location", "PMU.ttf", 10, Text.Black);
                menu.AddLabel(1, 2, 45, 110, 23, "> Staff Lounge", "PMU.ttf", 8, Text.Black);
                menu.AddLabel(2, 2, 75, 110, 24, "> Bedrooms 1", "PMU.ttf", 8, Text.Black);
                menu.AddLabel(3, 2, 105, 110, 24, "> Bedrooms 2", "PMU.ttf", 8, Text.Black);
                menu.AddLabel(4, 2, 135, 110, 24, "> Admin Rooms", "PMU.ttf", 8, Text.Black);
                menu.AddLabel(5, 2, 165, 110, 24, "> Building Top", "PMU.ttf", 8, Text.Black);
                menu.AddLabel(6, 2, 195, 110, 24, "> Developer Room", "PMU.ttf", 8, Text.Black);
                menu.AddLabel(7, 2, 225, 110, 24, "> Library", "PMU.ttf", 8, Text.Black);
                menu.AddLabel(8, 2, 255, 110, 24, "> Meeting Room", "PMU.ttf", 8, Text.Black);
                menu.AddLabel(9, 2, 285, 110, 24, "> Test Room", "PMU.ttf", 8, Text.Black);
                client.Player.CustomMenuManager.AddMenu(menu);
                client.Player.CustomMenuManager.SendMenu(menu.MenuName);
            }
        }
        
        public static bool HasActiveBagItem(ICharacter character, int effect1, int effect2, int effect3) {
        	for (int i = 0; i < character.ActiveItems.Count; i++) {
        		if ((ItemManager.Items[character.ActiveItems[i]].Type == Enums.ItemType.HeldInBag ||
        			ItemManager.Items[character.ActiveItems[i]].Type == Enums.ItemType.HeldByParty) &&
        			ItemManager.Items[character.ActiveItems[i]].Data1 == effect1 &&
        			(ItemManager.Items[character.ActiveItems[i]].Data2 == effect2 || effect2 == -1) &&
        			(ItemManager.Items[character.ActiveItems[i]].Data3 == effect3 || effect3 == -1)) {
        			return true;
        		}
        	}
        	
        	return false;
        }

        public static bool ScriptedReq(ICharacter character, int itemNum) {
            try {
                IMap map = MapManager.RetrieveActiveMap(character.MapID);
                if (ItemManager.Items[itemNum].Type == Enums.ItemType.Held || ItemManager.Items[itemNum].Type == Enums.ItemType.HeldByParty
                    || ItemManager.Items[itemNum].Type == Enums.ItemType.HeldInBag) {
                    if (map != null && map.TempStatus.GetStatus("MagicRoom") != null) {
                        return false;
                    }
                }

                if (HasAbility(character, "Klutz") && ItemManager.Items[itemNum].Type == Enums.ItemType.Held) {
                    return false;
                }

                return ScriptedReq(character, ItemManager.Items[itemNum].ScriptedReq, ItemManager.Items[itemNum].ReqData1, ItemManager.Items[itemNum].ReqData2, ItemManager.Items[itemNum].ReqData3, ItemManager.Items[itemNum].ReqData4, ItemManager.Items[itemNum].ReqData5);
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: ScriptedReq", Text.Black);
                return false;
            }
        }

        public static bool ScriptedReq(ICharacter character, int script, int data1, int data2, int data3, int data4, int data5) {
            switch (script) {
                case 0: {
                        return false;
                    }
                    break;
                case 1: {//Type-specific
                        if (character.Type1 == (Enums.PokemonType)data1 || character.Type2 == (Enums.PokemonType)data1) {
                            return true;
                        }
                        return false;
                    }
                    break;
                case 2: {//Species-specific
                        if (character.Species == data1 || character.Species == data2 || character.Species == data3
                        || character.Species == data4 || character.Species == data5) {
                            return true;
                        }
                        return false;
                    }
                    break;
                case 3: {//Eevee
                        if (character.Species == 133 || character.Species == 134 || character.Species == 135
                            || character.Species == 136 || character.Species == 196 || character.Species == 197
                            || character.Species == 470 || character.Species == 471) {
                            return true;
                        }
                        return false;
                    }
                    break;
                case 4: {//prevos only
                		int speciesNum = character.Species;
                        int evoIndex = -1;
                                            for (int z = 0; z < Server.Evolutions.EvolutionManager.Evolutions.MaxEvos; z++) {
                                                if (Server.Evolutions.EvolutionManager.Evolutions[z].Species == speciesNum) {
                                                    evoIndex = z;
                                                }
                                            }
                                            if (evoIndex == -1) {
                                            	return false;
                                            } else {
                                            	return true;
                                            }
                	}
                	break;
                default: {
                        return true;
                    }

            }

        }

        public static void OnItemActivated(int itemNum, ICharacter character) {
            try {
            	PacketHitList hitlist = null;
            	PacketHitList.MethodStart(ref hitlist);
				
				IMap map = MapManager.RetrieveActiveMap(character.MapID);
				
				if (itemNum == 213) {
                    AddExtraStatus(character, map, "Illusion", 25, null, "", hitlist);
                }
                if (itemNum == 224) {
                    AddExtraStatus(character, map, "Illusion", 300, null, "", hitlist);
                }
                if (itemNum == 244) {
                    AddExtraStatus(character, map, "Illusion", 387, null, "", hitlist);
                }
                if (itemNum == 245) {
                    AddExtraStatus(character, map, "Illusion", 390, null, "", hitlist);
                }
                if (itemNum == 246) {
                    AddExtraStatus(character, map, "Illusion", 393, null, "", hitlist);
                }
                if (itemNum == 82) {
                	hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("magic460.wav"), character.X, character.Y, 10);
                	hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " is floating on a balloon!", Text.WhiteSmoke), character.X, character.Y, 10);
                }
                //if (itemNum == 259 && character.HeldItem.Num == 259 && map.Moral == Enums.MapMoral.House) {
                //    AddExtraStatus(character, map, "Illusion", character.HeldItem.Tag.ToInt(), null, "", hitlist);
                //}
                
                RefreshCharacterTraits(character, map, hitlist);
                
                PacketHitList.MethodEnded(ref hitlist);
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: OnItemActivated", Text.Black);
                Messenger.AdminMsg(itemNum.ToString(), Text.Black);
                IMap map = MapManager.RetrieveActiveMap(character.MapID);
                Messenger.AdminMsg(map.Name, Text.Black);
                
            }
        }

        public static void OnItemDeactivated(int itemNum, ICharacter character) {
            try {
                RefreshCharacterTraits(character, MapManager.RetrieveActiveMap(character.MapID), null);
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: OnItemDeactivated", Text.Black);
            }
        }

        public static bool CanUseItem(BattleSetup setup, InventoryItem item) {
            try {
                if (setup.Attacker.AttackTimer == null) {
                    setup.Attacker.AttackTimer = new TickCount(Core.GetTickCount().Tick);
                }
                if (setup.Attacker.AttackTimer.Tick > Core.GetTickCount().Tick) {
                    setup.Cancel = true;
                    return false;
                }
                if (setup.Attacker.PauseTimer == null) {
	                setup.Attacker.PauseTimer = new TickCount(Core.GetTickCount().Tick);
	            }
	            if (setup.Attacker.PauseTimer.Tick > Core.GetTickCount().Tick) {
	                setup.Cancel = true;
	                return false;
	            }

                if (item.Sticky) {
                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("The " + ItemManager.Items[item.Num].Name + " is all sticky and doesn't work!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                    setup.Cancel = true;
	                return false;
                }

                if (setup.Attacker.VolatileStatus.GetStatus("Embargo") != null) {
                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " can't use items due to the Embargo!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                    setup.Cancel = true;
                    return false;
                }
                int itemEdibility = CheckItemEdibility(item.Num);
                if (itemEdibility != 0) {
                    TargetCollection checkedTargets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 10, setup.AttackerMap, setup.Attacker, setup.Attacker.X, setup.Attacker.Y, Enums.Direction.Up, true, false, false);
                    for (int i = 0; i < checkedTargets.Foes.Count; i++) {
                        if (HasAbility(checkedTargets.Foes[i], "Unnerve")) {
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " is unnerved!  It can't eat anything!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                            setup.Cancel = true;
                            return false;
                        }
                    }
                    
                    if (setup.Attacker.VolatileStatus.GetStatus("Nausea") != null) {
                    	setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " is nauseated!  It can't eat anything!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                            setup.Cancel = true;
                            return false;
                    }


                    if (HasAbility(setup.Attacker, "Gluttony")) {
                        if (itemEdibility == 1) {
                            setup.BattleTags.Add("Gluttony:60");
                        } else if (itemEdibility == 3) {
                            setup.BattleTags.Add("Gluttony:20");
                        }
                    }
                }

                return true;
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: CanUseItem", Text.Black);
                return false;
            }
        }

        public static void ThrewItem(BattleSetup setup, InventoryItem item, int invNum) {
            try {
                if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                    setup.PacketStack.AddPacket(((Recruit)setup.Attacker).Owner, PacketBuilder.CreateChatMsg("Throwing items currently in progress.", Text.Black));
                }
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: ThrewItem", Text.Black);
            }
        }

        public static void TakeItemSlotFromCharacter(ICharacter character, int invNum, int amount) {
            if (invNum == -1) return;
            if (character.CharacterType == Enums.CharacterType.Recruit) {
                ((Recruit)character).Owner.Player.TakeItemSlot(invNum, amount, true);
            } else {
                ((MapNpc)character).TakeHeldItem(amount);
            }

        }

        public static void ScriptedItem(BattleSetup setup, InventoryItem item, int invNum) {
            try {
                // Called when a scripted item is used
                ExtraStatus status;

                int itemNum = item.Num;

                int itemEdibility = CheckItemEdibility(itemNum);
                
                if (itemEdibility == 3 && HasAbility(setup.Attacker, "Harvest") && GetBattleTagArg(setup.BattleTags, "Harvest", 0) == null) {
                	
                	setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " harvested the berry!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                	BattleSetup extraSetup = new BattleSetup();
                    extraSetup.Attacker = setup.Attacker;
                    extraSetup.BattleTags.Add("Harvest");
                    BattleProcessor.HandleItemUse(item, -1, extraSetup);
                    setup.PacketStack.AddHitList(extraSetup.PacketStack);
                }
                
                switch (itemEdibility) {
                    case 0: {
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " used the " + ItemManager.Items[itemNum].Name + ".", Text.Pink), setup.Attacker.X, setup.Attacker.Y, 10);
                        }
                        break;
                    case 1: {//apple
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " ate the " + ItemManager.Items[itemNum].Name + ".", Text.Pink), setup.Attacker.X, setup.Attacker.Y, 10);
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSoundPacket("magic143.wav"), setup.Attacker.X, setup.Attacker.Y, 10);
                        }
                        break;
                    case 2: {//gummi
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " ate the " + ItemManager.Items[itemNum].Name + ".", Text.Pink), setup.Attacker.X, setup.Attacker.Y, 10);
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSoundPacket("magic145.wav"), setup.Attacker.X, setup.Attacker.Y, 10);
                        }
                        break;
                    case 3: {//berry
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " ate the " + ItemManager.Items[itemNum].Name + ".", Text.Pink), setup.Attacker.X, setup.Attacker.Y, 10);
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSoundPacket("magic144.wav"), setup.Attacker.X, setup.Attacker.Y, 10);
                        }
                        break;
                    case 4: {//seed
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " ate the " + ItemManager.Items[itemNum].Name + ".", Text.Pink), setup.Attacker.X, setup.Attacker.Y, 10);
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSoundPacket("magic144.wav"), setup.Attacker.X, setup.Attacker.Y, 10);
                        }
                        break;
                    case 5: {//herb
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " ate the " + ItemManager.Items[itemNum].Name + ".", Text.Pink), setup.Attacker.X, setup.Attacker.Y, 10);
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSoundPacket("magic144.wav"), setup.Attacker.X, setup.Attacker.Y, 10);
                        }
                        break;
                    case 6: {//poffin
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " ate the " + ItemManager.Items[itemNum].Name + ".", Text.Pink), setup.Attacker.X, setup.Attacker.Y, 10);
                        }
                        break;
                    case 7: {//bad food
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " ate the " + ItemManager.Items[itemNum].Name + ".", Text.Pink), setup.Attacker.X, setup.Attacker.Y, 10);
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSoundPacket("magic143.wav"), setup.Attacker.X, setup.Attacker.Y, 10);
                        }
                        break;
                    case 8: {//drink
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " drank the " + ItemManager.Items[itemNum].Name + ".", Text.Pink), setup.Attacker.X, setup.Attacker.Y, 10);
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSoundPacket("magic142.wav"), setup.Attacker.X, setup.Attacker.Y, 10);
                        }
                        break;
                    case 9: {//other consumable
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " ate the " + ItemManager.Items[itemNum].Name + ".", Text.Pink), setup.Attacker.X, setup.Attacker.Y, 10);
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSoundPacket("magic143.wav"), setup.Attacker.X, setup.Attacker.Y, 10);
                        }
                        break;
                }


                switch (ItemManager.Items[itemNum].Data1) {
                    case 0: {
                        }
                        break;
                    case 1: {
                            //Level-Up
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            int leveldiff = 100 - setup.Attacker.Level;
                            if (leveldiff > ItemManager.Items[itemNum].Data2) {
                                leveldiff = ItemManager.Items[itemNum].Data2;
                            }

                            if (leveldiff < 1) {
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("But nothing happened.", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                            } else {
                                for (int i = 0; i < leveldiff; i++) {
                                    if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                                        Client client = ((Recruit)setup.Attacker).Owner;
                                        client.Player.GetActiveRecruit().Exp = client.Player.GetActiveRecruit().GetNextLevel();
                                        PlayerLevelUp(setup.PacketStack, client);
                                    } else {
                                        ((MapNpc)setup.Attacker).Level++;
                                    }
                                }
                            }
                        }
                        break;
                    case 2: {
                            //Escape Orb
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                                Client client = ((Recruit)setup.Attacker).Owner;
                                //if (client.Player.PK == true) {
                                //    Messenger.PlayerMsg(client, "An outlaw cannot escape!", Text.BrightRed);
                                Escape(client);
                                
                            } else {
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " escaped from the dungeon!", Text.Yellow), setup.Attacker.X, setup.Attacker.Y, 10);
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSoundPacket("escape.wav"), setup.Attacker.X, setup.Attacker.Y, 10);
                                setup.AttackerMap.ActiveNpc[((MapNpc)setup.Attacker).MapSlot] = new MapNpc(setup.Attacker.MapID, ((MapNpc)setup.Attacker).MapSlot);
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, TcpPacket.CreatePacket("npcdead", ((MapNpc)setup.Attacker).MapSlot));
                            }
                        }
                        break;
                    case 3: {
                            //Adds a stat
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            int rand = ItemManager.Items[itemNum].Data2;
                            switch (rand) {
                                case 0: {
                                        setup.Attacker.MaxHPBonus += ItemManager.Items[itemNum].Data3;
                                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s Maximum HP was boosted!", Text.BrightGreen), setup.Attacker.X, setup.Attacker.Y, 10);
                                    }
                                    break;
                                case 1: {
                                        setup.Attacker.AtkBonus += ItemManager.Items[itemNum].Data3;
                                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s Attack was boosted!", Text.BrightGreen), setup.Attacker.X, setup.Attacker.Y, 10);
                                    }
                                    break;
                                case 2: {
                                        setup.Attacker.DefBonus += ItemManager.Items[itemNum].Data3;
                                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s Defense was boosted!", Text.BrightGreen), setup.Attacker.X, setup.Attacker.Y, 10);
                                    }
                                    break;
                                case 3: {
                                        setup.Attacker.SpclAtkBonus += ItemManager.Items[itemNum].Data3;
                                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s Special Attack was boosted!", Text.BrightGreen), setup.Attacker.X, setup.Attacker.Y, 10);
                                    }
                                    break;
                                case 4: {
                                        setup.Attacker.SpclDefBonus += ItemManager.Items[itemNum].Data3;
                                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s Special Defense was boosted!", Text.BrightGreen), setup.Attacker.X, setup.Attacker.Y, 10);
                                    }
                                    break;
                                case 5: {
                                        setup.Attacker.SpdBonus += ItemManager.Items[itemNum].Data3;
                                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s Speed was boosted!", Text.BrightGreen), setup.Attacker.X, setup.Attacker.Y, 10);
                                    }
                                    break;
                            }
                            
			                setup.Attacker.CalculateOriginalStats();
			                if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
			                	Client client = ((Recruit)setup.Attacker).Owner;
			                	PacketBuilder.AppendStats(client, setup.PacketStack);
			                }
                            HealCharacterBelly(setup.Attacker, 5, setup.PacketStack);

                        }
                        break;
                    case 4: {
                            //Chamber Key
                            int x = setup.Attacker.X;
                            int y = setup.Attacker.Y;
							MoveInDirection(setup.Attacker.Direction, 1, ref x, ref y);
	                        if (x >= 0 && x <= setup.AttackerMap.MaxX && y >= 0 && y <= setup.AttackerMap.MaxY) {
	                        	Tile tile = setup.AttackerMap.Tile[x, y];
	                            if (tile.Type == Enums.TileType.ScriptedSign && tile.Data1 == 10) {
	                            	TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
	                                string[] start = tile.String1.Split(':');
	                                string[] end = tile.String2.Split(':');
	                                
	                                ClearChamber(setup.AttackerMap, x + start[0].ToInt(), y + start[1].ToInt(), x + end[0].ToInt(), y + end[1].ToInt(), setup.PacketStack);
	                            }
	                        }
                        }
                        break;
                    case 5: {
                            //fossil revival
                            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
								
                                Client client = ((Recruit)setup.Attacker).Owner;
                                IMap map = client.Player.Map;
                                
								if (map.Tile[client.Player.X, client.Player.Y].Type == Enums.TileType.Scripted && map.Tile[client.Player.X, client.Player.Y].Data1 == 71) {
									TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
									//Messenger.PlayerMsg(client, "And then " + Pokedex.GetPokemon(ItemManager.Items[itemNum].Data2).Name + " appears.", Text.Pink);
									map.SetTile(9, 0, 0, 1, 7);
									map.SetTile(9, 0, 0, 1, 8);
									setup.PacketStack.AddPacketToMap(map, PacketBuilder.CreateTilePacket(9, 0, map));
									map.SetTile(9, 1, 0, 1, 7);
									map.SetTile(9, 1, 0, 1, 8);
									setup.PacketStack.AddPacketToMap(map, PacketBuilder.CreateTilePacket(9, 1, map));
			                        map.SetTile(9, 2, 0, 1, 7);
									map.SetTile(9, 2, 0, 1, 8);
									setup.PacketStack.AddPacketToMap(map, PacketBuilder.CreateTilePacket(9, 2, map));
			                        map.SetAttribute(9, 3, Enums.TileType.Walkable, 0, 0, 0, "", "", "");
			                        setup.PacketStack.AddPacketToMap(map, PacketBuilder.CreateTilePacket(9, 3, map));
									
									Story story = new Story();
					                StoryBuilderSegment segment = StoryBuilder.BuildStory();
					                StoryBuilder.AppendPlayMusicAction(segment, "PMD2) Growing Anxiety.ogg", true, true);
					                StoryBuilder.AppendRunScriptAction(segment, 5, "magic654.wav", "", "", true);
											StoryBuilder.AppendSaySegment(segment, "The moonlight bathes the " + ItemManager.Items[itemNum].Name + " in its mysterious red glow...", -1, 0, 0);
											StoryBuilder.AppendSaySegment(segment, "The " + ItemManager.Items[itemNum].Name + " is glowing with an intense, ancient power...!", -1, 0, 0);
											StoryBuilder.AppendMapVisibilityAction(segment, false);
											StoryBuilder.AppendRunScriptAction(segment, 5, "magic685.wav", "", "", true);
											StoryBuilder.AppendSaySegment(segment, "!!!!!!!!!", -1, 0, 0);
											StoryBuilder.AppendRunScriptAction(segment, 5, "magic648.wav", "", "", true);
											StoryBuilder.AppendWarpAction(segment, client.Player.Map.MapID, 9, 12);
											StoryBuilder.AppendMapVisibilityAction(segment, true);
                                            StoryBuilder.AppendCreateFNPCAction(segment, "0", "s-2", 9, 3, ItemManager.Items[itemNum].Data2);
					                        StoryBuilder.AppendChangeFNPCDirAction(segment, "0", Enums.Direction.Up);
					                        StoryBuilder.AppendSaySegment(segment, Pokedex.GetPokemon(ItemManager.Items[itemNum].Data2).Name + " was revived from the fossil!", -1, 0, 0);
					                        StoryBuilder.AppendPauseAction(segment, 200);
					                        StoryBuilder.AppendChangeFNPCDirAction(segment, "0", Enums.Direction.Right);
					                        StoryBuilder.AppendPauseAction(segment, 200);
					                        StoryBuilder.AppendChangeFNPCDirAction(segment, "0", Enums.Direction.Left);
					                        StoryBuilder.AppendPauseAction(segment, 200);
					                        StoryBuilder.AppendChangeFNPCDirAction(segment, "0", Enums.Direction.Right);
					                        StoryBuilder.AppendPauseAction(segment, 200);
					                        StoryBuilder.AppendChangeFNPCDirAction(segment, "0", Enums.Direction.Left);
					                        StoryBuilder.AppendPauseAction(segment, 200);
					                        StoryBuilder.AppendSaySegment(segment, "It's looking around desperately, as if confused...", -1, 0, 0);
					                        StoryBuilder.AppendChangeFNPCDirAction(segment, "0", Enums.Direction.Down);
					                        StoryBuilder.AppendSaySegment(segment, "...It seems to take your presence as a threat!  It's going to attack!", -1, 0, 0);
					                        StoryBuilder.AppendDeleteFNPCAction(segment, "0");
					                		StoryBuilder.AppendPlayMusicAction(segment, "St2) Elite 4 Battle 2.ogg", true, true);
					                        StoryBuilder.AppendRunScriptAction(segment, 46, ItemManager.Items[itemNum].Data2.ToString(), "25", "0", false);
					
									segment.AppendToStory(story);
					                StoryManager.PlayStory(client, story);
									
					                                
	                            }
                            }
                        }
                        break;
                    case 6: {
                            //level down
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            if (!setup.Attacker.HasActiveItem(92) && setup.Attacker.Level > 1) {
	                            int negative = ItemManager.Items[itemNum].Data2;
	                            if (negative > setup.Attacker.Level - 1) negative = setup.Attacker.Level - 1;
								setup.Attacker.Level -= negative;
								if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
	                                Client client = ((Recruit)setup.Attacker).Owner;
	                                client.Player.GetActiveRecruit().Exp = client.Player.GetActiveRecruit().GetNextLevel() - 1;
	                            }
				                setup.Attacker.CalculateOriginalStats();
				                if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
				                	Client client = ((Recruit)setup.Attacker).Owner;
				                	PacketBuilder.AppendStats(client, setup.PacketStack);
				                }
				                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " dropped to level " + setup.Attacker.Level + "!", Text.BrightRed));
			                } else {
			                	setup.PacketStack.AddPacketToMap(setup.AttackerMap,  PacketBuilder.CreateBattleMsg("But nothing happened.", Text.WhiteSmoke));
			                }
                        }
                        break;
                    case 7: {
                            //level 1
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            if (!setup.Attacker.HasActiveItem(92)) {
								setup.Attacker.Level = 1;
								if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
	                                Client client = ((Recruit)setup.Attacker).Owner;
	                                client.Player.GetActiveRecruit().Exp = 0;
	                            }
	                            setup.Attacker.AtkBonus = 0;
	                            setup.Attacker.DefBonus = 0;
	                            setup.Attacker.SpdBonus = 0;
	                            setup.Attacker.SpclAtkBonus = 0;
	                            setup.Attacker.SpclDefBonus = 0;
				                setup.Attacker.CalculateOriginalStats();
				                if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
				                	Client client = ((Recruit)setup.Attacker).Owner;
				                	PacketBuilder.AppendStats(client, setup.PacketStack);
				                }
				                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " dropped to level 1!", Text.Magenta));
			                } else {
			                	setup.PacketStack.AddPacketToMap(setup.AttackerMap,  PacketBuilder.CreateBattleMsg("But nothing happened.", Text.WhiteSmoke));
			                }
                        }
                        break;
                    case 8: {
                            //Adds Speed

                        }
                        break;
                    case 9: {
                            //adds a random stat
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            int rand = Server.Math.Rand(0, 6);
                            switch (rand) {
                                case 0: {
                                        setup.Attacker.MaxHPBonus += ItemManager.Items[itemNum].Data3;
                                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s Maximum HP was boosted!", Text.BrightGreen), setup.Attacker.X, setup.Attacker.Y, 10);
                                    }
                                    break;
                                case 1: {
                                        setup.Attacker.AtkBonus += ItemManager.Items[itemNum].Data3;
                                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s Attack was boosted!", Text.BrightGreen), setup.Attacker.X, setup.Attacker.Y, 10);
                                    }
                                    break;
                                case 2: {
                                        setup.Attacker.DefBonus += ItemManager.Items[itemNum].Data3;
                                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s Defense was boosted!", Text.BrightGreen), setup.Attacker.X, setup.Attacker.Y, 10);
                                    }
                                    break;
                                case 3: {
                                        setup.Attacker.SpclAtkBonus += ItemManager.Items[itemNum].Data3;
                                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s Special Attack was boosted!", Text.BrightGreen), setup.Attacker.X, setup.Attacker.Y, 10);
                                    }
                                    break;
                                case 4: {
                                        setup.Attacker.SpclDefBonus += ItemManager.Items[itemNum].Data3;
                                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s Special Defense was boosted!", Text.BrightGreen), setup.Attacker.X, setup.Attacker.Y, 10);
                                    }
                                    break;
                                case 5: {
                                        setup.Attacker.SpdBonus += ItemManager.Items[itemNum].Data3;
                                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s Speed was boosted!", Text.BrightGreen), setup.Attacker.X, setup.Attacker.Y, 10);
                                    }
                                    break;
                            }
                            HealCharacterBelly(setup.Attacker, 5, setup.PacketStack);

                        }
                        break;
                    case 10: {//Adds all stats
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);

                            setup.Attacker.MaxHPBonus += ItemManager.Items[itemNum].Data3;
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s Maximum HP was boosted!", Text.BrightGreen), setup.Attacker.X, setup.Attacker.Y, 10);
                            setup.Attacker.AtkBonus += ItemManager.Items[itemNum].Data3;
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s Attack was boosted!", Text.BrightGreen), setup.Attacker.X, setup.Attacker.Y, 10);
                            setup.Attacker.DefBonus += ItemManager.Items[itemNum].Data3;
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s Defense was boosted!", Text.BrightGreen), setup.Attacker.X, setup.Attacker.Y, 10);
                            setup.Attacker.SpclAtkBonus += ItemManager.Items[itemNum].Data3;
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s Special Attack was boosted!", Text.BrightGreen), setup.Attacker.X, setup.Attacker.Y, 10);
                            setup.Attacker.SpclDefBonus += ItemManager.Items[itemNum].Data3;
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s Special Defense was boosted!", Text.BrightGreen), setup.Attacker.X, setup.Attacker.Y, 10);
                            setup.Attacker.SpdBonus += ItemManager.Items[itemNum].Data3;
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s Speed was boosted!", Text.BrightGreen), setup.Attacker.X, setup.Attacker.Y, 10);
                            HealCharacterBelly(setup.Attacker, 5, setup.PacketStack);

                        }
                        break;
                    case 11: {
                            //Full HP recovery
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            //if (GetBattleTagArg(setup.BattleTags, "Gluttony", 0) != null) {
                            //HealCharacter(setup.Attacker, setup.AttackerMap, ItemManager.Items[itemNum].Data2 + GetBattleTagArg(setup.BattleTags, "Gluttony", 0).ToInt(), setup.PacketStack);
                            //} else {
                            HealCharacter(setup.Attacker, setup.AttackerMap, setup.Attacker.MaxHP, setup.PacketStack);
                            //}
                            HealCharacterBelly(setup.Attacker, ItemManager.Items[itemNum].Data3, setup.PacketStack);
                        }
                        break;
                    case 12: {
                            //Box
                            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                                Client client = ((Recruit)setup.Attacker).Owner;
                                //TODO: check if the player has a key, and ask to open with the key
                                Messenger.PlaySoundToMap(client.Player.MapID, "magic669.wav");
                                Messenger.PlayerMsg(client, "The box is locked shut.  Perhaps there's someone who knows how to open it...", Text.Cyan);
                                /*
                                if (client.Player.Map.Moral == Enums.MapMoral.Safe || client.Player.Map.Moral == Enums.MapMoral.House) {
                                    Messenger.PlayerMsg(client, "You opened up the " + ItemManager.Items[itemNum].Name + "!", Text.Yellow);
                                    string[] itemSelection = client.Player.Inventory[invNum].Tag.Split(';');
                                    string[] chosenItem = itemSelection[Server.Math.Rand(0, itemSelection.Length)].Split(',');
                                    client.Player.TakeItemSlot(invNum, 1, true);
                                    if (chosenItem[0].IsNumeric()) {
                                        if (chosenItem.Length > 1 && chosenItem[1].IsNumeric()) {
                                            client.Player.GiveItem(chosenItem[0].ToInt(), chosenItem[1].ToInt());
                                            Messenger.PlayerMsg(client, "Inside was " + chosenItem[1] + " " + ItemManager.Items[chosenItem[0].ToInt()].Name + "!", Text.Yellow);
                                        } else {
                                            client.Player.GiveItem(chosenItem[0].ToInt(), 1);
                                            Messenger.PlayerMsg(client, "Inside was a " + ItemManager.Items[chosenItem[0].ToInt()].Name + "!", Text.Yellow);
                                        }
                                    } else {
                                        Messenger.PlayerMsg(client, "But there was nothing inside...", Text.Yellow);
                                    }
                                } else {
                                    Messenger.PlayerMsg(client, "This item cannot be opened in dungeons!", Text.BrightRed);
                                }
                                */
                            }
                        }
                        break;
                    case 13: {
                            //Unknown Effect Mushroom
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);

                            string[] effects = item.Tag.Split(';');
                            for (int i = 0; i < effects.Length; i++) {
                                //trigger each effect

                            }

                            HealCharacterBelly(setup.Attacker, 5, setup.PacketStack);
                        }
                        break;
                    case 14: {
                            //herbal tea
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            HealCharacter(setup.Attacker, setup.AttackerMap, 100, setup.PacketStack);
                            HealCharacterPP(setup.Attacker, setup.AttackerMap, 10, setup.PacketStack);
                            AddExtraStatus(setup.Attacker, setup.AttackerMap, "Yawn", 3, null, "", setup.PacketStack);

                            HealCharacterBelly(setup.Attacker, 5, setup.PacketStack);
                        }
                        break;
                    case 15: {
                            //warp seed
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            if (setup.AttackerMap.MapType == Enums.MapType.RDungeonMap) {
                                RandomWarp(setup.Attacker, setup.AttackerMap, true, setup.PacketStack);
                            } else {
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("But nothing happened!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                            }
                            HealCharacterBelly(setup.Attacker, 5, setup.PacketStack);
                        }
                        break;
                    case 16: {
                            //Single Nonvolatile Status Berry
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            if (setup.Attacker.StatusAilment == (Enums.StatusAilment)ItemManager.Items[itemNum].Data2) {
                                SetStatusAilment(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.OK, 0, setup.PacketStack);
                            } else {
                                //setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("But nothing happened!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                                AddExtraStatus(setup.Attacker, setup.AttackerMap, "Status Guard", 10, null, ItemManager.Items[itemNum].Data2.ToString(), setup.PacketStack);
                            }
                            HealCharacterBelly(setup.Attacker, ItemManager.Items[itemNum].Data3, setup.PacketStack);
                        }
                        break;
                    case 17: {
                            //HP recovery
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            //if (GetBattleTagArg(setup.BattleTags, "Gluttony", 0) != null) {
                            //HealCharacter(setup.Attacker, setup.AttackerMap, ItemManager.Items[itemNum].Data2 + GetBattleTagArg(setup.BattleTags, "Gluttony", 0).ToInt(), setup.PacketStack);
                            //} else {
                            if (ItemManager.Items[itemNum].Data2 >= 0) {
                            	HealCharacter(setup.Attacker, setup.AttackerMap, ItemManager.Items[itemNum].Data2, setup.PacketStack);
                            } else {
                            	DamageCharacter(setup.Attacker, setup.AttackerMap, ItemManager.Items[itemNum].Data2, Enums.KillType.Other, setup.PacketStack, true);
                            }
                            //}
                            HealCharacterBelly(setup.Attacker, ItemManager.Items[itemNum].Data3, setup.PacketStack);
                        }
                        break;
                    case 18: {
                            //PP recovery
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            HealCharacterPP(setup.Attacker, setup.AttackerMap, ItemManager.Items[itemNum].Data2, setup.PacketStack);

                            HealCharacterBelly(setup.Attacker, ItemManager.Items[itemNum].Data3, setup.PacketStack);
                        }
                        break;
                    case 19: {
                            //Belly recovery
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {

                                if (((Recruit)setup.Attacker).Belly < ((Recruit)setup.Attacker).MaxBelly) {

                                    if (ItemManager.Items[itemNum].Data2 >= 1000 && ((Recruit)setup.Attacker).MaxBelly < 200) {
                                        ((Recruit)setup.Attacker).MaxBelly += (ItemManager.Items[itemNum].Data2 / 100);
                                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s belly size increased!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                                    }

                                    HealCharacterBelly(setup.Attacker, ItemManager.Items[itemNum].Data2, setup.PacketStack);

                                } else {
                                    ((Recruit)setup.Attacker).MaxBelly += ItemManager.Items[itemNum].Data3;
                                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s belly size increased!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                                    HealCharacterBelly(setup.Attacker, ItemManager.Items[itemNum].Data3, setup.PacketStack);
                                }
                                if (((Recruit)setup.Attacker).MaxBelly > 200 && ((Recruit)setup.Attacker).MaxBelly < 200) {
                                    ((Recruit)setup.Attacker).MaxBelly = 200;
                                }
                                if (((Recruit)setup.Attacker).Belly > ((Recruit)setup.Attacker).MaxBelly) {
                                    ((Recruit)setup.Attacker).Belly = ((Recruit)setup.Attacker).MaxBelly;
                                }
                                PacketBuilder.AppendBelly(((Recruit)setup.Attacker).Owner, setup.PacketStack);
                            }
                        }
                        break;
                    case 20: {
                            //Multi-Belly recovery
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                            	Client client = ((Recruit)setup.Attacker).Owner;
								for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
				                   	if (client.Player.Team[i] != null && client.Player.Team[i].Loaded) {
				                   		HealCharacterBelly(client.Player.Team[i], ItemManager.Items[itemNum].Data2, setup.PacketStack);
					                }
				                }
                            }
                        }
                        break;
                    case 21: {
                            //sweet heart
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            int add = 5;
                            TargetCollection targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Floor, 0, setup.AttackerMap, setup.Attacker, setup.Attacker.X, setup.Attacker.Y, Enums.Direction.Up, false, true, false);
                            for (int i = 0; i < targets.Friends.Count; i++) {
                                int dx = setup.Attacker.X - targets[i].X;
                                int dy = setup.Attacker.Y - targets[i].Y;
                                if (dx < 2 && dy < 2) {
                                    add += 5;
                                } else if (dx < 8 && dy < 8) {
                                    add += 4;
                                } else if (dx < 16 && dy < 16) {
                                    add += 3;
                                } else {
                                    add += 2;
                                }
                            }
				            if (add > 5) {
				            	setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("The feelings of nearby friends made it sweeter!", Text.Pink), setup.Attacker.X, setup.Attacker.Y, 10);
				            }
                            HealCharacter(setup.Attacker, setup.AttackerMap, add*4, setup.PacketStack);
                            HealCharacterPP(setup.Attacker, setup.AttackerMap, add, setup.PacketStack);

                            HealCharacterBelly(setup.Attacker, 5, setup.PacketStack);
                        }
                        break;
                    case 22: {
                            //casteliacone
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            //HealCharacter(setup.Attacker, setup.AttackerMap, setup.Attacker.MaxHP, setup.PacketStack);
                            HealCharacterPP(setup.Attacker, setup.AttackerMap, 100, setup.PacketStack);
                            SetStatusAilment(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.OK, 0, setup.PacketStack);
                            if (Server.Math.Rand(0, 3) == 0 && !CheckStatusProtection(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.Freeze.ToString(), false, setup.PacketStack)) {
                            	SetStatusAilment(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.Freeze, 0, setup.PacketStack);
                            }

                            HealCharacterBelly(setup.Attacker, 5, setup.PacketStack);
                        }
                        break;
                    case 23: { // Pounce Orb
                            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                                Client client = ((Recruit)setup.Attacker).Owner;
                                client.Player.TakeItemSlot(invNum, 1, true);
                                IMap map = client.Player.Map;
                                Messenger.MapMsg(map.MapID, client.Player.Name + " used a Pounce Orb!", Text.BrightCyan);
                                int npc = FindNearestNpc(client);
                                int playerX = client.Player.X;
                                int playerY = client.Player.Y;
                                if (IsPathClear(client, npc)) {
                                    MapNpc activeNpc = map.ActiveNpc[npc];
                                    if (playerX == activeNpc.X) {
                                        if (playerY > activeNpc.Y) {
                                            Messenger.PlayerWarp(client, map, playerX, activeNpc.Y + 1);
                                        } else {
                                            Messenger.PlayerWarp(client, map, playerX, activeNpc.Y - 1);
                                        }
                                    } else {
                                        if (playerX > activeNpc.X) {
                                            Messenger.PlayerWarp(client, map, activeNpc.X + 1, playerY);
                                        } else {
                                            Messenger.PlayerWarp(client, map, activeNpc.X - 1, playerY);
                                        }
                                    }
                                } else {
                                    int blockX = 0;
                                    int blockY = 0;
                                    FindNearestBlock(setup.AttackerMap, client.Player.Direction, ref blockX, ref blockY);
                                    Messenger.PlayerWarp(client, map, blockX, blockY);
                                }
                                Messenger.PlaySoundToMap(setup.AttackerMap.MapID, "magic50.wav");
                            }
                        }
                        break;
                    case 24: { // Blowback Orb
                            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                                Client client = ((Recruit)setup.Attacker).Owner;
                                int npc = FindNearestNpc(client);
                                IMap map = client.Player.Map;
                                if (IsPathClear(client, npc)) {
                                    client.Player.TakeItem(181, 1);
                                    Messenger.MapMsg(map.MapID, "An enemy was blown back!", Text.BrightCyan);
                                    int x = 0;
                                    int y = 0;
                                    FindNearestBlock(setup.AttackerMap, client.Player.Direction, ref x, ref y);
                                    MapNpc activeNpc = map.ActiveNpc[npc];
                                    activeNpc.X = x;
                                    activeNpc.Y = y;
                                    int npc2 = FindNearestNpc(client);
                                    if (npc2 != npc) {
                                        MapNpc activeNpc2 = map.ActiveNpc[npc2];
                                        if (activeNpc.X == activeNpc2.X) {
                                            if (activeNpc.Y > activeNpc2.Y) {
                                                activeNpc.Y = activeNpc2.Y - 1;
                                            } else {
                                                activeNpc.Y = activeNpc2.Y + 1;
                                            }
                                        } else {
                                            if (activeNpc.X > activeNpc2.X) {
                                                activeNpc.X = activeNpc2.X - 1;
                                            } else {
                                                activeNpc.X = activeNpc2.X + 1;
                                            }
                                        }
                                        //BattleProcessor.PlayerAttackNpc(client, npc2, 15);
                                    }
                                    //BattleProcessor.PlayerAttackNpc(client, npc, 15);
                                    // SendMapNpc
                                } else {
                                    Messenger.PlayerMsg(client, "It had no effect!", Text.BrightRed);
                                }
                                Messenger.PlaySoundToMap(setup.AttackerMap.MapID, "magic50.wav");
                            }
                        }
                        break;

                    case 25: {//Two-Edge Orb
                            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                                Client client = ((Recruit)setup.Attacker).Owner;
                                client.Player.TakeItemSlot(invNum, 1, true);
                                IMap map = client.Player.Map;
                                for (int j = 0; j <= 14; j++) {
                                    if (map.ActiveNpc[j].Num > 0) {
                                        if (setup.Attacker.Level >= map.ActiveNpc[j].Level) {
                                            map.ActiveNpc[j].HP = 1;
                                        }
                                        map.ActiveNpc[j].Target = client;
                                    }
                                }


                                Messenger.BattleMsg(client, "All weaker foes in the room are now close to fainting!", Text.BrightRed);
                                //DamagePlayer(client, (setup.Attacker.HP / 2), Enums.KillType.Npc);
                                Messenger.PlaySoundToMap(setup.AttackerMap.MapID, "magic54.wav");
                            }
                        }
                        break;

                    case 26: {//White Flute ~adjust for NPC levels (still unfinished)
                            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                                Client client = ((Recruit)setup.Attacker).Owner;
                                Messenger.BattleMsg(client, "You used the White Flute!", Text.White);
                                //Messenger.SpellAnim(498, setup.AttackerMap, client.Player.X, client.Player.Y);
                                Messenger.PlaySoundToMap(setup.AttackerMap.MapID, "magic52.wav");
                                IMap map = client.Player.Map;
                                for (int j = 0; j <= 14; j++) {
                                    //if (CheckPlayerRange(index, j, 5, Enums.TargetType.Npc) == true && CheckTarget(index, j, Enums.TargetType.Npc) == true) {
                                    map.ActiveNpc[j].Target = client;
                                    //}
                                }
                                Messenger.BattleMsg(client, "Other Pokémon were lured.", Text.White);
                            }
                        }
                        break;

                    case 27: {//Black Flute
                            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                                Client client = ((Recruit)setup.Attacker).Owner;
                                Messenger.BattleMsg(client, "You used the Black Flute!", Text.White);
                                //Messenger.SpellAnim(498, setup.AttackerMap, client.Player.X, client.Player.Y);
                                Messenger.PlaySoundToMap(setup.AttackerMap.MapID, "magic52.wav");
                                IMap map = client.Player.Map;
                                for (int j = 0; j <= 14; j++) {
                                    //if (CheckPlayerRange(index, j, 5, Enums.TargetType.Npc) == true && CheckTarget(index, j, Enums.TargetType.Npc) == true && ObjectFactory.GetMapNpc(setup.AttackerMap, j).Target == index) {
                                    map.ActiveNpc[j].Target = null;
                                    //}
                                }
                                Messenger.BattleMsg(client, "Other Pokémon were repelled.", Text.White);
                            }
                        }
                        break;
                    case 28: {//trawl orb
                    		TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                    		setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSoundPacket("Magic322.wav"), setup.Attacker.X, setup.Attacker.Y, 50);
                    		if (setup.AttackerMap.MapType == Enums.MapType.RDungeonMap) {
                                
                                for (int i = 0; i < Server.Constants.MAX_MAP_ITEMS; i++) {
                                	if (setup.AttackerMap.ActiveItem[i].Num != 0 && !setup.AttackerMap.ActiveItem[i].Hidden
                                		&& setup.AttackerMap.Tile[setup.AttackerMap.ActiveItem[i].X, setup.AttackerMap.ActiveItem[i].Y].Type != Enums.TileType.DropShop) {
                                		int x = Server.Math.Rand(setup.Attacker.X-1, setup.Attacker.X+2);
                                		int y = Server.Math.Rand(setup.Attacker.Y-1, setup.Attacker.Y+2);
                                		if (x < 0) x = 0;
                                		if (x >= setup.AttackerMap.MaxX) x = setup.AttackerMap.MaxX-1;
                                		if (y < 0) y = 0;
                                		if (y >= setup.AttackerMap.MaxY) y = setup.AttackerMap.MaxY-1;
                                		setup.AttackerMap.SpawnItemSlot(i, setup.AttackerMap.ActiveItem[i].Num, setup.AttackerMap.ActiveItem[i].Value,
                                		setup.AttackerMap.ActiveItem[i].Sticky, false, setup.AttackerMap.ActiveItem[i].Tag, x, y, null);
                                	}
                                }
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("Items were pulled close to "+setup.Attacker.Name+"!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                            } else {
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("Items can only be pulled in Mystery Dungeons!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                            }
                    		
                    	}
                    	break;
                    case 29: {
                            //Quick Seed
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            //if ((int)client.Player.SpeedLimit < 6) {
                            //client.Player.SpeedLimit++;
                            //Messenger.SendSpeedLimit(client);
                            //    if (setup.Attacker.VolatileStatus.GetStatus("MovementSpeed") != null) {
                            //        AddExtraStatus(setup.Attacker, setup.AttackerMap, "MovementSpeed", setup.Attacker.VolatileStatus.GetStatus("MovementSpeed").Counter + 1, null, setup.PacketStack);
                            //    } else {
                            //        AddExtraStatus(setup.Attacker, setup.AttackerMap, "MovementSpeed", 1, null, setup.PacketStack);
                            //    }
                            //} else {
                            //    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " can't go any faster!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                            //}
                            status = setup.Attacker.VolatileStatus.GetStatus("MovementSpeed");
                            if (status != null) {
                                AddExtraStatus(setup.Attacker, setup.AttackerMap, "MovementSpeed", status.Counter + 1, null, "", setup.PacketStack);
                            } else {
                                AddExtraStatus(setup.Attacker, setup.AttackerMap, "MovementSpeed", 1, null, "", setup.PacketStack);
                            }
                            HealCharacterBelly(setup.Attacker, 5, setup.PacketStack);
                        }
                        break;
                    case 30: {
                            //Violent Seed
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            //StrBuff(index, 2, 0);
                            //PPBuff(index, 2, 0);
                            // TODO: Violent Seed [Scripts]
                        }
                        break;
                    case 31: {
                            //Bad EGG
                        }
                        break;
					case 32: {//seaside key
							if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
								
                                Client client = ((Recruit)setup.Attacker).Owner;
                                IMap map = client.Player.Map;
							if (map.Tile[client.Player.X, client.Player.Y].Type == Enums.TileType.RDungeonGoal) {
								map.Tile[client.Player.X, client.Player.Y].Mask2Set = 4;
                                map.Tile[client.Player.X, client.Player.Y].Mask2 = 2;
                                map.Tile[client.Player.X, client.Player.Y].Data1 = 1;
                                map.TempChange = true;
                                setup.PacketStack.AddPacketToMap(map, PacketBuilder.CreateTilePacket(client.Player.X, client.Player.Y, map));
                                setup.PacketStack.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("The passage to the next floor was opened!", Text.BrightGreen), client.Player.X, client.Player.Y, 50);
                                setup.PacketStack.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("Magic127.wav"), client.Player.X, client.Player.Y, 50);
                                setup.PacketStack.AddPacketToMap(map, PacketBuilder.CreateBattleDivider(), client.Player.X, client.Player.Y, 50);
                                TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            }
                            }
						}
						break;
                    case 33: {//Teru-Sama
                            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                                Client client = ((Recruit)setup.Attacker).Owner;
                                client.Player.TakeItemSlot(invNum, 1, true);
                                client.CloseConnection();
                            }
                        }
                        break;

                    case 34: {// Lottery Ticket
                            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                                Client client = ((Recruit)setup.Attacker).Owner;
                                if (DateTime.Now.DayOfWeek == Lottery.LOTTERY_DAY) {
                                    IMap map = client.Player.Map;
                                    if (map.Tile[client.Player.X, client.Player.Y].Type == Enums.TileType.ScriptedSign
                                        && map.Tile[client.Player.X, client.Player.Y].Data1 == 5) {
                                        // Do nothing...
                                    } else {
                                        Messenger.PlayerMsg(client, "Today is " + Lottery.LOTTERY_DAY.ToString() + "! Go to Felicity to see if you won!", Text.Grey);
                                    }
                                } else {
                                    Messenger.PlayerMsg(client, "Talk to Felicity on " + Lottery.LOTTERY_DAY.ToString() + " to see if you won!", Text.Grey);
                                }
                            }
                        }
                        break;

                    case 35: {
                            //Attack Items
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            //setup.SetupMove(MoveManager.Moves[ItemManager.Items[itemNum].Data2].MoveNum]);
                            //setup.Move.Name = setup.Attacker.Name + " used the " + ItemManager.Items[itemNum].Name + "!";
                            setup.moveSlot = -1;
                            setup.moveIndex = ItemManager.Items[itemNum].Data2;
                            BattleProcessor.HandleAttack(setup);
                            if (ItemManager.Items[itemNum].Data3 > 0) {
                            	HealCharacterBelly(setup.Attacker, ItemManager.Items[itemNum].Data3, setup.PacketStack);
                            }
                        }
                        break;
                    case 36: {
                            //Gummi
                            // TODO: Type of Gummi is based on data2
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            //if (NetScript.GetPlayerClass(index) == 2 || NetScript.GetPlayerClass(index) == 6 || NetScript.GetPlayerClass(index) == 10) {
                            //    NetScript.SetPlayerMP(index, NetScript.GetPlayerMP(index) + 500);
                            //    NetScript.SetPlayerHP(index, NetScript.GetPlayerHP(index) + 500);
                            //} else {
                            //    NetScript.SetPlayerMP(index, NetScript.GetPlayerMP(index) + 50);
                            //    NetScript.SetPlayerHP(index, NetScript.GetPlayerHP(index) + 50);
                            //}
                        }
                        break;
                    case 37: {//Tasty Honey
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " used the Tasty Honey!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                            if (setup.AttackerMap.Moral != Enums.MapMoral.Safe && setup.AttackerMap.Moral != Enums.MapMoral.House) {
                                for (int i = 0; i <  Constants.MAX_MAP_NPCS / 4; i++) {
                                    setup.AttackerMap.SpawnNpc();
                                }
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("Pokémon were attracted to the scent!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                            } else {
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("Pokémon can't be lured in this area!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                            }
                        }
                        break;
                    case 38: {
                            //Grimy Food
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            int rand = Server.Math.Rand(0, 1000);
                            //if (setup.AttackerMap.Name == "Spinda's Cafe") rand = 0;
                            if (rand > 750 && !CheckStatusProtection(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.Burn.ToString(), false, setup.PacketStack)) {
                                SetStatusAilment(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.Burn, 0, setup.PacketStack);
                            } else if (rand > 500 && !CheckStatusProtection(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.Paralyze.ToString(), false, setup.PacketStack)) {
                                SetStatusAilment(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.Paralyze, 0, setup.PacketStack);
                            } else if (rand > 250 && !CheckStatusProtection(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.Poison.ToString(), false, setup.PacketStack)) {
                                SetStatusAilment(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.Poison, 1, setup.PacketStack);
                            } else if (rand > 0) {
                                ChangeAttackBuff(setup.Attacker, setup.AttackerMap, -3, setup.PacketStack);
                                ChangeSpAtkBuff(setup.Attacker, setup.AttackerMap, -3, setup.PacketStack);
                            } else {
                                rand = Server.Math.Rand(0, 1000);
                                //if (setup.AttackerMap.Name == "Spinda's Cafe") rand = 0;
                                if (rand > 0) {
                                ChangeAttackBuff(setup.Attacker, setup.AttackerMap, -3, setup.PacketStack);
                                ChangeSpAtkBuff(setup.Attacker, setup.AttackerMap, -3, setup.PacketStack);
                                } else {
                                	DamageCharacter(setup.Attacker, setup.AttackerMap, Int32.MaxValue, Enums.KillType.Other, setup.PacketStack, true);
                                }
                            }
                            HealCharacterBelly(setup.Attacker, 30, setup.PacketStack);
                        }
                        break;
                    case 39: {
                            //Grimy Berry
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            int rand = Server.Math.Rand(0, 14);
                            switch (rand) {
                                case 0: {
                                        ChangeAttackBuff(setup.Attacker, setup.AttackerMap, -1, setup.PacketStack);
                                    }
                                    break;
                                case 1: {
                                        ChangeDefenseBuff(setup.Attacker, setup.AttackerMap, -1, setup.PacketStack);
                                    }
                                    break;
                                case 2: {
                                        ChangeSpAtkBuff(setup.Attacker, setup.AttackerMap, -1, setup.PacketStack);
                                    }
                                    break;
                                case 3: {
                                        ChangeSpDefBuff(setup.Attacker, setup.AttackerMap, -1, setup.PacketStack);
                                    }
                                    break;
                                case 4: {
                                        ChangeAccuracyBuff(setup.Attacker, setup.AttackerMap, -1, setup.PacketStack);
                                    }
                                    break;
                                case 5: {
                                        ChangeEvasionBuff(setup.Attacker, setup.AttackerMap, -1, setup.PacketStack);
                                    }
                                    break;
                                case 6: {
                                        ChangeSpeedBuff(setup.Attacker, setup.AttackerMap, -1, setup.PacketStack);
                                    }
                                    break;
                            }
                            HealCharacterBelly(setup.Attacker, 5, setup.PacketStack);
                        }
                        break;
                    case 40: {
                            //Attack Items Infinite Use

                            //setup.SetupMove(MoveManager.Moves[ItemManager.Items[itemNum].Data2].MoveNum]);
                            //setup.Move.Name = setup.Attacker.Name + " used the " + ItemManager.Items[itemNum].Name + "!";
                            setup.moveSlot = -1;
                            setup.moveIndex = ItemManager.Items[itemNum].Data2;
                            BattleProcessor.HandleAttack(setup);
                            HealCharacterBelly(setup.Attacker, ItemManager.Items[itemNum].Data3, setup.PacketStack);
                        }
                        break;
                    case 41: {//Musical Items
                            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                                Client client = ((Recruit)setup.Attacker).Owner;
                                for (int i = client.Player.X - 3; i <= client.Player.X + 3; i++) {
                                    for (int j = client.Player.Y - 3; j <= client.Player.Y + 3; j++) {
                                        if (i < 0 || j < 0 || i > client.Player.Map.MaxX || j > client.Player.Map.MaxY) {

                                        } else {
                                            setup.PacketStack.AddPacketToMap(client.Player.Map, PacketBuilder.CreateSpellAnim(498, i, j));
                                        }
                                    }
                                }

                                string musicName = "";

                                switch (ItemManager.Items[itemNum].Data2) {
                                    case 1: {
                                            musicName = "PMD2) Northern Desert.ogg";
                                        }
                                        break;
                                    case 2: {
                                            musicName = "PMD2) Dark Crater.ogg";
                                        }
                                        break;
                                    case 3: {
                                            musicName = "PMD2) Sky Peak Snowfield.ogg";
                                        }
                                        break;
                                    case 4: {
                                            musicName = "PMD2) Miracle Sea.ogg";
                                        }
                                        break;
                                    case 5: {
                                            musicName = "PMD2) Concealed Ruins.ogg";
                                        }
                                        break;
                                    case 6: {
                                            musicName = "PMD2) Sky Peak Forest.ogg";
                                        }
                                        break;
                                    case 7: {
                                            musicName = "PMD) Sky Tower.ogg";
                                        }
                                        break;
                                    case 8: {
                                            musicName = "PMD2) Star Cave.ogg";
                                        }
                                        break;
                                    default: {
                                            musicName = "RBY) Lavender Town.ogg";
                                        }
                                        break;

                                }
                                TargetCollection targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 3, client.Player.Map, null, client.Player.X, client.Player.Y, Enums.Direction.Up, true, true, true);
                                for (int i = 0; i < targets.Count; i++) {
                                    if (targets[i].CharacterType == Enums.CharacterType.Recruit) {
                                        Messenger.PlayMusic(((Recruit)targets[i]).Owner, musicName);
                                    }
                                }
                            }
                        }
                        break;
                    case 42: {
                            //traps
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSoundPacket("magic105.wav"), setup.Attacker.X, setup.Attacker.Y, 10);
                            bool trapPlaced = false;
                            if (setup.AttackerMap.Moral == Enums.MapMoral.None || setup.AttackerMap.Moral == Enums.MapMoral.NoPenalty) {
                                if (setup.AttackerMap.Tile[setup.Attacker.X, setup.Attacker.Y].Type == Enums.TileType.Scripted && GetTrapType(setup.AttackerMap.Tile[setup.Attacker.X, setup.Attacker.Y].Data1) != null) {
                                    //remove preexisting trap
                                    RemoveTrap(setup.AttackerMap, setup.Attacker.X, setup.Attacker.Y, setup.PacketStack);
                                }

                                if (setup.AttackerMap.Tile[setup.Attacker.X, setup.Attacker.Y].Type == Enums.TileType.Walkable) {
                                    //set the trap on a walkable tile
                                    Tile tile = setup.AttackerMap.Tile[setup.Attacker.X, setup.Attacker.Y];
                                    tile.Type = Enums.TileType.Scripted;
                                    tile.Data1 = ItemManager.Items[itemNum].Data2;
                                    tile.Data2 = 0;
                                    tile.Data3 = 2;
                                    if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                                        tile.String1 = ((Recruit)setup.Attacker).Owner.Player.CharID;
                                        tile.String2 = ((Recruit)setup.Attacker).Owner.Player.GuildName;
                                    } else {
                                        tile.String1 = "";
                                        tile.String2 = "";
                                    }
                                    tile.String2 = ItemManager.Items[itemNum].Data3.ToString();
                                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " placed a trap!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                                    trapPlaced = true;
                                }
                            }

                            if (!trapPlaced) {
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("A trap can't be placed here!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                            }
                        }
                        break;
					case 43: {//Tanren Key
							if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
								
                                Client client = ((Recruit)setup.Attacker).Owner;
                                IMap map = client.Player.Map;
                                TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                                /*
                                string param1 = map.Tile[client.Player.X, client.Player.Y].String1;
                                string param2 = map.Tile[client.Player.X, client.Player.Y].String2;
                                string param3 = map.Tile[client.Player.X, client.Player.Y].String3;
                                if (client.Player.PartyID == null) {
                                	TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
	                                client.Player.WarpToRDungeon(param1.ToInt()-1, ((RDungeonMap)client.Player.Map).RDungeonFloor + 1);
	
                                } else {
			                        bool warp = true;
			                        Party party = PartyManager.FindPlayerParty(client);
                        			IMap sourceMap = client.Player.Map;
			                        foreach (Client member in party.GetOnlineMemberClients()) {
			                            if (!member.Player.Dead && member.Player.MapID == client.Player.MapID && (member.Player.X != client.Player.X || member.Player.Y != client.Player.Y)) {
			                                warp = false;
			                            }
			                        }
			
			                        if (warp) {
			                        	TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
			                        	foreach (Client member in party.GetOnlineMemberClients()) {
			                        		if (member.Player.Map != sourceMap) continue;
			                                member.Player.WarpToRDungeon(param1.ToInt()-1, ((RDungeonMap)member.Player.Map).RDungeonFloor + 1);
			
			                        	}
			                        } else {
			                        	setup.PacketStack.AddPacket(client, PacketBuilder.CreateChatMsg("All surviving players on the floor must be on the goal in order to continue.", Text.WhiteSmoke));
			                        }
	                            }
                              */
							if (map.Tile[client.Player.X, client.Player.Y].Type == Enums.TileType.Scripted && map.Tile[client.Player.X, client.Player.Y].Data1 == 68) {
								map.Tile[client.Player.X, client.Player.Y].Mask2Set = 4;
                                map.Tile[client.Player.X, client.Player.Y].Mask2 = 2;
                                map.Tile[client.Player.X, client.Player.Y].Data1 = 69;
                                map.Tile[client.Player.X, client.Player.Y].String2 = "";
                                map.TempChange = true;
                                setup.PacketStack.AddPacketToMap(map, PacketBuilder.CreateTilePacket(client.Player.X, client.Player.Y, map));
                                setup.PacketStack.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("A secret passage was opened!", Text.BrightGreen), client.Player.X, client.Player.Y, 50);
                                setup.PacketStack.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("Magic127.wav"), client.Player.X, client.Player.Y, 50);
                                setup.PacketStack.AddPacketToMap(map, PacketBuilder.CreateBattleDivider(), client.Player.X, client.Player.Y, 50);
                                TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            }
                            
                            }
						}
						break;
                    case 44: {
                            //Gracidea
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            if (setup.Attacker.Species == 492 && setup.Attacker.Form == 0) {
                            	setup.Attacker.Form = 1;
                            	setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSoundPacket("Magic838.wav"), setup.Attacker.X, setup.Attacker.Y, 10);
                            	setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSpellAnim(502, setup.Attacker.X, setup.Attacker.Y));
                            	setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " changed forme!", Text.BrightGreen), setup.Attacker.X, setup.Attacker.Y, 10);
                            	RefreshCharacterTraits(setup.Attacker, setup.AttackerMap, setup.PacketStack);
                            } else {
                            	setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("But nothing happened.", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                            }
                            
                        }
                        break;
                    case 45: {
                            //Ragecandybar
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            HealCharacterBelly(setup.Attacker, 30, setup.PacketStack);
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSpellAnim(500, setup.Attacker.X, setup.Attacker.Y));
                            TargetCollection targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 3, setup.AttackerMap, setup.Attacker, setup.Attacker.X, setup.Attacker.Y, Enums.Direction.Up, false, true, false);
                            if (targets.Friends.Count > 0) {
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " shared some pieces with the others!", Text.Pink), setup.Attacker.X, setup.Attacker.Y, 10);
                            for (int i = 0; i < targets.Friends.Count; i++) {
                            	setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSpellAnim(Server.Moves.MoveManager.Moves[500].TravelingAnim, setup.Attacker.X, setup.Attacker.Y, targets.Friends[i].X - setup.Attacker.X, targets.Friends[i].Y - setup.Attacker.Y, 0));
                                HealCharacterBelly(targets.Friends[i], 30, setup.PacketStack);
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSpellAnim(504, targets.Friends[i].X, targets.Friends[i].Y));
                            }
                            }
                            
                        }
                        break;
                    case 46: {//scanner orb
                    		TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                    		setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSoundPacket("Magic322.wav"), setup.Attacker.X, setup.Attacker.Y, 50);
                    		for (int i = 0; i < Server.Constants.MAX_MAP_ITEMS; i++) {
                            	if (setup.AttackerMap.ActiveItem[i].Num != 0 && setup.AttackerMap.ActiveItem[i].Hidden) {
                            		setup.AttackerMap.SpawnItemSlot(i, setup.AttackerMap.ActiveItem[i].Num, setup.AttackerMap.ActiveItem[i].Value,
                            		setup.AttackerMap.ActiveItem[i].Sticky, false, setup.AttackerMap.ActiveItem[i].Tag,
                            		setup.AttackerMap.ActiveItem[i].X, setup.AttackerMap.ActiveItem[i].Y, null);
                            	}
                            }
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("All hidden items on the floor were revealed!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                		
                    	}
                    	break;
                    case 47: {
                            //Lava Cookie
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            HealCharacter(setup.Attacker, setup.AttackerMap, setup.Attacker.MaxHP, setup.PacketStack);
                            if (setup.Attacker.StatusAilment != Enums.StatusAilment.OK) {
                                SetStatusAilment(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.OK, 0, null);
                                //Messenger.BattleMsg(client, "You are no longer poisoned!", Text.White);
                            } else {
                                
                            }
                            HealCharacterBelly(setup.Attacker, 5, setup.PacketStack);
                        }
                        break;
                    case 48: {
                            //Energy Root
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            HealCharacter(setup.Attacker, setup.AttackerMap, setup.Attacker.MaxHP, setup.PacketStack);
                            HealCharacterPP(setup.Attacker, setup.AttackerMap, 100, setup.PacketStack);
                            HealCharacterBelly(setup.Attacker, 200, setup.PacketStack);
                            AddExtraStatus(setup.Attacker, setup.AttackerMap, "Nausea", 0, null, "", setup.PacketStack);
                            
                        }
                        break;
                    case 49: {
                            //Slowpoketail
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            HealCharacterBelly(setup.Attacker, 60, setup.PacketStack);
                            TargetCollection targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Floor, 0, setup.AttackerMap, setup.Attacker, setup.Attacker.X, setup.Attacker.Y, Enums.Direction.Up, true, false, false);
                            
                            for (int i = 0; i < targets.Count; i++) {
                                if (targets[i].Species == 79 || targets[i].Species == 80 || targets[i].Species == 199) {
                                	AddExtraStatus(targets[i], setup.AttackerMap, "Grudge", 0, null, "", setup.PacketStack);
                                }
                            }
                            
                        }
                        break;
                    case 53: {
                            //Wonder Gummi
                            // TODO: Wonder Gummi
                            ////if (NetScript.GetPlayerClass(index) == 2 || NetScript.GetPlayerClass(index) == 6 || NetScript.GetPlayerClass(index) == 10) {
                            //client.Player.TakeItem(520, 1);
                            //NetScript.SetPlayerMP(index, NetScript.GetPlayerMP(index) + 500);
                            //NetScript.SetPlayerHP(index, NetScript.GetPlayerHP(index) + 500);
                            ////}
                        }
                        break;
                    case 55: {
                            //Old Gateau
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            if (setup.Attacker.StatusAilment != Enums.StatusAilment.OK) {
                                SetStatusAilment(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.OK, 0, null);
                                //Messenger.BattleMsg(client, "You are no longer poisoned!", Text.White);
                            } else {
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("But nothing happened.", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                            }
                            HealCharacterBelly(setup.Attacker, 30, setup.PacketStack);
                        }
                        break;
                    case 56: {//for west wing
                            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                                Client client = ((Recruit)setup.Attacker).Owner;
                                if (client.Player.Direction == Enums.Direction.Up && setup.AttackerMap.MapID == MapManager.GenerateMapID(743) && client.Player.X == 11) {
                                    Messenger.AskQuestion(client, "AbandonedMansion", "Will you use the West Wing Key to enter?", -1);
                                } else {
                                    StoryManager.PlayStory(client, 298);
                                }
                            }
                        }
                        break;
                    case 57: {//for east wing
                            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                                Client client = ((Recruit)setup.Attacker).Owner;
                                if (client.Player.Direction == Enums.Direction.Up && setup.AttackerMap.MapID == MapManager.GenerateMapID(743) && client.Player.X == 21) {
                                    Messenger.AskQuestion(client, "AbandonedMansion", "Will you use the East Wing Key to enter?", -1);
                                } else {
                                    StoryManager.PlayStory(client, 298);
                                }
                            }
                        }
                        break;
                    case 58: {//for abandoned mansion
                            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                                Client client = ((Recruit)setup.Attacker).Owner;
                                if (client.Player.Direction == Enums.Direction.Up && setup.AttackerMap.MapID == MapManager.GenerateMapID(743) && client.Player.X == 16) {
                                    Messenger.AskQuestion(client, "AbandonedMansion", "Will you use the Mansion Key to enter?", -1);
                                } else {
                                    StoryManager.PlayStory(client, 298);
                                }
                            }
                        }
                        break;
                    case 59: {//One-Room Orb
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            if (setup.AttackerMap.MapType == Enums.MapType.RDungeonMap) {
                            	RDungeonMap map = (RDungeonMap)setup.AttackerMap;
                                int[,] intArray = new int[map.MaxX + 1, map.MaxY + 1];
                                for (int x = 0; x <= map.MaxX; x++) {
                                	for (int y = 0; y <= map.MaxY; y++) {
                                		if (x == 0 || y == 0 || x == map.MaxX || y == map.MaxY) {
                                            map.Tile[x, y].RDungeonMapValue = 1;
                                            RDungeonFloorGen.AmbiguateTile(map.Tile[x, y]);
                                        } else if (map.Tile[x, y].RDungeonMapValue == 4 || map.Tile[x, y].RDungeonMapValue == 5 || map.Tile[x, y].RDungeonMapValue >= 256 && map.Tile[x, y].RDungeonMapValue < 512) {
                                            map.Tile[x, y].RDungeonMapValue = 3;
                                            RDungeonFloorGen.AmbiguateTile(map.Tile[x, y]);
                                		}
                                        intArray[x, y] = map.Tile[x, y].RDungeonMapValue;
                                	}
                                }
                                DungeonArrayFloor.TextureDungeon(intArray);
                                
                                for (int x = 0; x <= map.MaxX; x++) {
                                    for (int y = 0; y <= map.MaxY; y++) {
                                        map.Tile[x, y].RDungeonMapValue = intArray[x, y];
                                    }
                                }
                                
                                RDungeonFloorGen.TextureDungeonMap(map);
                                
                                for (int x = 0; x <= map.MaxX; x++) {
                                	for (int y = 0; y <= map.MaxY; y++) {
                                		setup.PacketStack.AddPacketToMap(map, PacketBuilder.CreateTilePacket(x, y, map));
                                	}
                                }
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("The walls crumbled, forming one big room!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSoundPacket("magic174.wav"), setup.Attacker.X, setup.Attacker.Y, 10);
                            } else {
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("But nothing happened!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                            }
                            
                        }
                        break;
                    case 60: {//Drought Orb
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            if (setup.AttackerMap.MapType == Enums.MapType.RDungeonMap) {
                            	RDungeonMap map = (RDungeonMap)setup.AttackerMap;
                                int[,] intArray = new int[map.MaxX + 1, map.MaxY + 1];
                                for (int x = 0; x <= map.MaxX; x++) {
                                	for (int y = 0; y <= map.MaxY; y++) {
                                        if (map.Tile[x, y].RDungeonMapValue >= 512 && map.Tile[x, y].RDungeonMapValue < 768) {
                                            map.Tile[x, y].RDungeonMapValue = 3;
                                            RDungeonFloorGen.AmbiguateTile(map.Tile[x, y]);
                                		}
                                        intArray[x, y] = map.Tile[x, y].RDungeonMapValue;
                                	}
                                }

                                DungeonArrayFloor.TextureDungeon(intArray);

                                for (int x = 0; x <= map.MaxX; x++) {
                                    for (int y = 0; y <= map.MaxY; y++) {
                                        map.Tile[x, y].RDungeonMapValue = intArray[x, y];
                                    }
                                }
                                
                                RDungeonFloorGen.TextureDungeonMap(map);
                                
                                for (int x = 0; x <= map.MaxX; x++) {
                                	for (int y = 0; y <= map.MaxY; y++) {
                                		setup.PacketStack.AddPacketToMap(map, PacketBuilder.CreateTilePacket(x, y, map));
                                	}
                                }
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("All liquids were drained from the floor!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSoundPacket("magic170.wav"), setup.Attacker.X, setup.Attacker.Y, 10);
                            } else {
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("But nothing happened!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                            }
                            
                        }
                        break;
                    case 61: {
                            //heal seed
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            if (setup.Attacker.StatusAilment != Enums.StatusAilment.OK) {
                                SetStatusAilment(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.OK, 0, setup.PacketStack);
                                //Messenger.BattleMsg(client, "You are no longer poisoned!", Text.White);
                            } else {
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("But nothing happened.", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                            }
                            HealCharacterBelly(setup.Attacker, 5, setup.PacketStack);
                        }
                        break;
                    case 69: {
                            //blinker seed
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            AddExtraStatus(setup.Attacker, setup.AttackerMap, "Blind", 20, null, "", setup.PacketStack);
                            HealCharacterBelly(setup.Attacker, 5, setup.PacketStack);
                        }
                        break;
                    case 70: {//Weather Orb
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            if (setup.AttackerMap.Tile[setup.Attacker.X, setup.Attacker.Y].Type == Enums.TileType.Arena || setup.AttackerMap.Moral == Enums.MapMoral.None || setup.AttackerMap.Moral == Enums.MapMoral.NoPenalty) {
                                if (Server.Math.Rand(0, 10) == 0) {
                                    SetMapWeather(setup.AttackerMap, (Enums.Weather)ItemManager.Items[itemNum].Data3, setup.PacketStack);
                                } else {
                                    SetMapWeather(setup.AttackerMap, (Enums.Weather)ItemManager.Items[itemNum].Data2, setup.PacketStack);
                                }
                            } else {
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("The weather can only change in dungeons and arenas!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                            }
                        }
                        break;
                    case 71: {//Recycle Orb
                            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                                Client client = ((Recruit)setup.Attacker).Owner;
                                TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                                bool grimy = false;
                                for (int i = 1; i <= client.Player.Inventory.Count; i++) {
                                    if (client.Player.Inventory[i].Num != 74 && client.Player.Inventory[i].Num != 75) {
		                            } else if (client.Player.Inventory[i].Tag.IsNumeric() && client.Player.Inventory[i].Tag.ToInt() > 0) {
		                                int grimyItem = client.Player.Inventory[i].Num;
		                                client.Player.Inventory[i].Num = client.Player.Inventory[i].Tag.ToInt();
		                                client.Player.Inventory[i].Tag = "";
		                            } else {
		                                grimy = true;
		                            }
		                            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
		                                PacketBuilder.AppendInventory(((Recruit)setup.Attacker).Owner, setup.PacketStack);
		                            }
                                }
                                Messenger.PlayerMsg(client, "Grimy items in the bag have been restored!", Text.Cyan);
                                Messenger.PlayerMsg(client, "But some items were too grimy to recycle...", Text.Blue);
                            }
                        }
                        break;
                    case 72: {//heart slate
                    		if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                                Client client = ((Recruit)setup.Attacker).Owner;
                                int slot = -1;
                                for (int i = 1; i <= client.Player.Inventory.Count; i++) {
                                    if (client.Player.Inventory[i].Num == 171 && !client.Player.Inventory[i].Tag.Contains(item.Tag)) {
                                        slot = i;
                                        break;
                                    }
                                }
                                if (slot > -1) {
                                	client.Player.Inventory[slot].Tag += (item.Tag + ";");
                                	TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                                	Messenger.PlayerMsg(client, "The item disappeared, and its symbol was imprinted into the Mystery Part!", Text.Cyan);
                                	Messenger.PlaySoundToMap(client.Player.MapID, "magic765.wav");
                                } else {
                                	Messenger.PlayerMsg(client, "But nothing happened...", Text.Blue);
                                }
                                
                            }
                    	}
                    	break;
                    case 73: {//mystery part
                    		if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                    			Client client = ((Recruit)setup.Attacker).Owner;
                    			string[] choicesNum = item.Tag.Split(';');
                    			if (choicesNum.Length > 1 && IsHardModeEntrance(setup.AttackerMap)) {
                    				List<string> choices = new List<string>();
									bool tooMany = false;
						            for (int i = 0; i < choicesNum.Length-1; i++) {
						            	if (choices.Count >= 5) {
						            	tooMany = true;
						            		break;
						            	}
						            	choices.Add("#" + choicesNum[i] + ": " + Pokedex.GetPokemon(choicesNum[i].ToInt()).Name);
						            }
									if (tooMany) {
										choices.Add("Others");
									}
						            choices.Add("Cancel");
						
						            Messenger.AskQuestion(client, "SummonGuardian:0:" + item.Tag, "Which ally will you call forth?", -1, choices.ToArray());
                    			} else {
                    				setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("But nothing happened...", Text.Blue), setup.Attacker.X, setup.Attacker.Y, 10);
                    			}
                    		}
                    	}
                    	break;
                    case 74: {//Luminous Orb
                            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                                Client client = ((Recruit)setup.Attacker).Owner;
                                TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSoundPacket("magic322.wav"), setup.Attacker.X, setup.Attacker.Y, 10);
                                if (setup.AttackerMap.Tile[setup.Attacker.X, setup.Attacker.Y].Type == Enums.TileType.Arena || setup.AttackerMap.Moral == Enums.MapMoral.None || setup.AttackerMap.Moral == Enums.MapMoral.NoPenalty) {
                                	AddMapStatus(setup.AttackerMap, "Luminous", 0, "", -1, setup.PacketStack);
                                } else {
                                	setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("The map can only be lit up in dungeons and arenas!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                                }
                            }
                        }
                        break;
                    case 75: {
                            //Vanish seed
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            AddExtraStatus(setup.Attacker, setup.AttackerMap, "Invisible", 20, null, "", setup.PacketStack);
                            HealCharacterBelly(setup.Attacker, 5, setup.PacketStack);
                        }
                        break;
                    case 76: {//Cleanse Orb
                            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                                Client client = ((Recruit)setup.Attacker).Owner;
                                TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                                for (int i = 1; i <= client.Player.Inventory.Count; i++) {
                                    if (client.Player.Inventory[i].Sticky) {
                                        client.Player.SetItemSticky(i, false);
                                    }
                                }
                                Messenger.PlayerMsg(client, "All sticky items have been cleansed!", Text.Cyan);
                            }
                        }
                        break;
                    case 77: {
                            //Pinch Berry
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            // if (setup.Attacker.HP <= setup.Attacker.MaxHP / 2) {
                            int raisedStat = ItemManager.Items[itemNum].Data2;
                            if (raisedStat == -1) {
                                raisedStat = Server.Math.Rand(0, 7);
                            }
                            switch (raisedStat) {
                                case 0: {
                                        ChangeAttackBuff(setup.Attacker, setup.AttackerMap, 20, setup.PacketStack);
                                    }
                                    break;
                                case 1: {
                                        ChangeDefenseBuff(setup.Attacker, setup.AttackerMap, 20, setup.PacketStack);
                                    }
                                    break;
                                case 2: {
                                        ChangeSpAtkBuff(setup.Attacker, setup.AttackerMap, 20, setup.PacketStack);
                                    }
                                    break;
                                case 3: {
                                        ChangeSpDefBuff(setup.Attacker, setup.AttackerMap, 20, setup.PacketStack);
                                    }
                                    break;
                                case 4: {
                                        ChangeSpeedBuff(setup.Attacker, setup.AttackerMap, 20, setup.PacketStack);
                                    }
                                    break;
                                case 5: {
                                        ChangeAccuracyBuff(setup.Attacker, setup.AttackerMap, 20, setup.PacketStack);
                                    }
                                    break;
                                case 6: {
                                        ChangeEvasionBuff(setup.Attacker, setup.AttackerMap, 20, setup.PacketStack);
                                    }
                                    break;
                            }
                            //} else {
                            //    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("But nothing happened.", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                            //}
                            //HealCharacter(setup.Attacker, setup.AttackerMap, setup.Attacker.MaxHP / 2, setup.PacketStack);
                            HealCharacterBelly(setup.Attacker, 5, setup.PacketStack);
                        }
                        break;
                    case 78: {
                            //Persim Berry
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);

                            if (setup.Attacker.VolatileStatus.GetStatus("Confusion") != null) {
                                RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "Confusion", setup.PacketStack);
                            } else {
                                //setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("But nothing happened!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                                AddExtraStatus(setup.Attacker, setup.AttackerMap, "Confusion Guard", 10, null, "", setup.PacketStack);
                            }
                            HealCharacterBelly(setup.Attacker, 5, setup.PacketStack);
                        }
                        break;
                    case 79: {
                            //Lum Berry
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            if (setup.Attacker.StatusAilment != Enums.StatusAilment.OK) {
                                SetStatusAilment(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.OK, 0, setup.PacketStack);
                                //Messenger.BattleMsg(client, "You are no longer poisoned!", Text.White);
                            } else {
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("But nothing happened.", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                            }
                            for (int i = setup.Attacker.VolatileStatus.Count - 1; i > -1; i--) {
                            	if (IsStatusBad(setup.Attacker.VolatileStatus[i].Name)) {
                            		RemoveExtraStatus(setup.Attacker, setup.AttackerMap, setup.Attacker.VolatileStatus[i].Name, setup.PacketStack);
                            	}
                            }
                            HealCharacterBelly(setup.Attacker, 5, setup.PacketStack);
                        }
                        break;
                    case 80: {
                            //Type-reduce Berry
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            AddExtraStatus(setup.Attacker, setup.AttackerMap, "TypeReduce", ItemManager.Items[itemNum].Data2, null, "", setup.PacketStack);
                            HealCharacterBelly(setup.Attacker, 5, setup.PacketStack);
                        }
                        break;
                    case 81: {
                            //Trap-See Orb
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            for (int x = 0; x <= setup.AttackerMap.MaxX; x++) {
                                for (int y = 0; y <= setup.AttackerMap.MaxY; y++) {
                                    if (setup.AttackerMap.Tile[x, y].Type == Enums.TileType.Scripted && GetTrapType(setup.AttackerMap.Tile[x, y].Data1) != null && GetTrapType(setup.AttackerMap.Tile[x, y].Data1) != "SecretRoom") {
                                        RevealTrap(setup.AttackerMap, x, y, setup.PacketStack);
                                    }
                                }
                            }
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("All traps on the floor were exposed!", Text.Pink));
                        }
                        break;
                    case 82: {
                            //Trapbust Orb
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            for (int x = 0; x <= setup.AttackerMap.MaxX; x++) {
                                for (int y = 0; y <= setup.AttackerMap.MaxY; y++) {
                                    if (setup.AttackerMap.Tile[x, y].Type == Enums.TileType.Scripted && GetTrapType(setup.AttackerMap.Tile[x, y].Data1) != null && GetTrapType(setup.AttackerMap.Tile[x, y].Data1) != "SecretRoom") {
                                        RemoveTrap(setup.AttackerMap, x, y, setup.PacketStack);
                                    }
                                }
                            }
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("All traps on the floor were destroyed!", Text.Pink));
                        }
                        break;
                    case 83: {//White Herb
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            RemoveBuffs(setup.Attacker);
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s stats were returned to normal!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);

                            HealCharacterBelly(setup.Attacker, 3, setup.PacketStack);
                        }
                        break;
                    case 84: {//Evasion Orb
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            ChangeEvasionBuff(setup.Attacker, setup.AttackerMap, 2, setup.PacketStack);

                        }
                        break;
                    case 85: {
                            //Slip Seed
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            AddExtraStatus(setup.Attacker, setup.AttackerMap, "Slip", 0, null, "", setup.PacketStack);
                            HealCharacterBelly(setup.Attacker, 5, setup.PacketStack);
                        }
                        break;
                    case 86: {
                            //Mobile Orb
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            AddExtraStatus(setup.Attacker, setup.AttackerMap, "SuperMobile", 0, null, "", setup.PacketStack);
                        }
                        break;
                    case 87: {//Power Herb
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            AddExtraStatus(setup.Attacker, setup.AttackerMap, "SuperCharge", 0, null, "", setup.PacketStack);
                            HealCharacterBelly(setup.Attacker, 3, setup.PacketStack);
                        }
                        break;
                    case 88: {
                            //Type-boost gem
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            AddExtraStatus(setup.Attacker, setup.AttackerMap, "TypeBoost", ItemManager.Items[itemNum].Data2, null, "", setup.PacketStack);
                        }
                        break;
                    case 89: {//Mental Herb
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "Attract", setup.PacketStack);
                            RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "Encore", setup.PacketStack);
                            RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "Torment", setup.PacketStack);
                            RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "Taunt", setup.PacketStack);
                            RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "Disable", setup.PacketStack);
                            RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "MoveSeal:0", setup.PacketStack);
                            RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "MoveSeal:1", setup.PacketStack);
                            RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "MoveSeal:2", setup.PacketStack);
                            RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "MoveSeal:3", setup.PacketStack);
                            HealCharacterBelly(setup.Attacker, 3, setup.PacketStack);
                        }
                        break;
                    case 90: {//revives and max revives
                    		if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                                ActivateRevivalItem(((Recruit)setup.Attacker).Owner, itemNum);
                            }
                    	}
                    	break;
                    case 91: {//Pathfinder Orb
                    		TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                    		Client client = ((Recruit)setup.Attacker).Owner;
							if (setup.AttackerMap.Name != "Pitch-Black Abyss") {
								Messenger.PlayerMsg(client, "The orb fills with a brilliant light, then fades out.", Text.Cyan);
							} else {
                    			int left, right, up, down;
                    			string dir = "";
                    			int x = -1;
                    			int y = -1;
                    		
                    			for (int i = 0; i < exPlayer.Get(client).DungeonMaxX; i++) {
                    				for (int j = 0; j < exPlayer.Get(client).DungeonMaxY; j++) {
                    					if (exPlayer.Get(client).DungeonMap[i,j] == PitchBlackAbyss.Room.End) {
                    						x = i;
                    						y = j;
                    					}
                    				}
                    			}
                    			
                    			if (exPlayer.Get(client).DungeonX >= x) {
                    				left = exPlayer.Get(client).DungeonX - x;
									right = exPlayer.Get(client).DungeonMaxX - left;
								} else {
									right = x - exPlayer.Get(client).DungeonX;
									left = exPlayer.Get(client).DungeonMaxX - right;
								}
								if (exPlayer.Get(client).DungeonY >= y) {
									up = exPlayer.Get(client).DungeonY - y;
									down = exPlayer.Get(client).DungeonMaxY - up;
								} else {
									down = y - exPlayer.Get(client).DungeonY;
									up = exPlayer.Get(client).DungeonMaxY - down;
								}
									if (left == 0) {
									if (up < down) dir = "North";
									else dir = "South";
								} else if (up == 0) {
									if (left < right) dir = "West";
									else dir = "East";
								} else {
									//composite cases
									if (up < down) dir = "North";
									else dir = "South";
									if (left < right) dir = dir + "-West";
									else dir = dir + "-East";
								}
								Messenger.PlayerMsg(client, "The exit is to the " + dir + ".", Text.Cyan);
							}
						}
						break;
					case 92: { //sleep seed
							TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            SetStatusAilment(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.Sleep, Server.Math.Rand(2, 4), setup.PacketStack);
                            HealCharacterBelly(setup.Attacker, 5, setup.PacketStack);
                        }
						break;
					case 93: { //dark liquid
							TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
							HealCharacter(setup.Attacker, setup.AttackerMap, setup.Attacker.MaxHP, setup.PacketStack);
							HealCharacterPP(setup.Attacker, setup.AttackerMap, 100, setup.PacketStack);
							HealCharacterBelly(setup.Attacker, 200, setup.PacketStack);
							//grimy food effect
							int rand = Server.Math.Rand(0, 1000);
                            if (rand > 750 && !CheckStatusProtection(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.Burn.ToString(), false, setup.PacketStack)) {
                                SetStatusAilment(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.Burn, 0, setup.PacketStack);
                            } else if (rand > 500 && !CheckStatusProtection(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.Paralyze.ToString(), false, setup.PacketStack)) {
                                SetStatusAilment(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.Paralyze, 0, setup.PacketStack);
                            } else if (rand > 250 && !CheckStatusProtection(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.Poison.ToString(), false, setup.PacketStack)) {
                                SetStatusAilment(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.Poison, 1, setup.PacketStack);
                            } else if (rand > 0) {
                                ChangeAttackBuff(setup.Attacker, setup.AttackerMap, -3, setup.PacketStack);
                                ChangeSpAtkBuff(setup.Attacker, setup.AttackerMap, -3, setup.PacketStack);
                            } else {
                                rand = Server.Math.Rand(0, 1000);
                                if (rand > 0) {
                                ChangeAttackBuff(setup.Attacker, setup.AttackerMap, -3, setup.PacketStack);
                                ChangeSpAtkBuff(setup.Attacker, setup.AttackerMap, -3, setup.PacketStack);
                                } else {
                                	DamageCharacter(setup.Attacker, setup.AttackerMap, Int32.MaxValue, Enums.KillType.Other, setup.PacketStack, true);
                                }
                            }
						}
						break;
                    case 94: {
                            //jaboca/rowap Berry
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            // if (setup.Attacker.HP <= setup.Attacker.MaxHP / 2) {
                            
                            AddExtraStatus(setup.Attacker, setup.AttackerMap, "AttackReturn", ItemManager.Items[itemNum].Data2, null, "", setup.PacketStack);
                            
                            HealCharacterBelly(setup.Attacker, 5, setup.PacketStack);
                        }
                        break;
                    case 95: {
                            //lansat Berry
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            // if (setup.Attacker.HP <= setup.Attacker.MaxHP / 2) {
                            
                            AddExtraStatus(setup.Attacker, setup.AttackerMap, "FocusEnergy", 8, null, "", setup.PacketStack);
                            
                            HealCharacterBelly(setup.Attacker, 5, setup.PacketStack);
                        }
                        break;
                    case 96: {
                            //pure seed
                            TakeItemSlotFromCharacter(setup.Attacker, invNum, 1);
                            if (setup.AttackerMap.MapType == Enums.MapType.RDungeonMap) {
                            	WarpToStairs(setup.Attacker, setup.AttackerMap, setup.PacketStack, 5);
                            } else {
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("Warping works only Mystery Dungeons!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                            }
                            
                            HealCharacterBelly(setup.Attacker, 5, setup.PacketStack);
                        }
                        break;
                }
                //if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                    //Client client = ((Recruit)setup.Attacker).Owner;
                    //Logger.AppendToLog("/Player Event Logs/" + client.Player.Name + ".txt", "Used scripted item: " + ItemManager.Items[itemNum].Name, true);
                //}
            
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: ScriptedItem " + item.Num + "-" + invNum, Text.Black);
                
                Messenger.AdminMsg(ex.ToString(), Text.Black);
            }
        }

        public static void OpenBox(Client client, InventoryItem item, int invNum) {
            string itemName = ItemManager.Items[item.Num].Name;
            string[] itemSelection = client.Player.Inventory[invNum].Tag.Split(';');
            string[] chosenItem = itemSelection[Server.Math.Rand(0, itemSelection.Length)].Split(',');
            client.Player.TakeItemSlot(invNum, 1, true);
            if (chosenItem[0].IsNumeric()) {
                if (chosenItem.Length == 2 && chosenItem[1].IsNumeric()) {
                    client.Player.GiveItem(chosenItem[0].ToInt(), chosenItem[1].ToInt());
                    Messenger.PlayerMsg(client, "Inside the " + itemName + " was " + chosenItem[1] + " " + ItemManager.Items[chosenItem[0].ToInt()].Name + "!", Text.Yellow);
                } else if (chosenItem.Length == 3 && chosenItem[2].IsNumeric()) {
                	client.Player.GiveItem(chosenItem[0].ToInt(), 1, chosenItem[2]);
                    //Messenger.PlayerMsg(client, "Inside the " + itemName + " was " + chosenItem[1] + " " + ItemManager.Items[chosenItem[0].ToInt()].Name + "!", Text.Yellow);
                    Messenger.PlayerMsg(client, "Inside the " + itemName + " was a " + ItemManager.Items[chosenItem[0].ToInt()].Name + "!", Text.Yellow);
                } else {
                    client.Player.GiveItem(chosenItem[0].ToInt(), 1);
                    Messenger.PlayerMsg(client, "Inside the " + itemName + " was a " + ItemManager.Items[chosenItem[0].ToInt()].Name + "!", Text.Yellow);
                }
            } else {
                Messenger.PlayerMsg(client, "There was nothing inside the " + itemName + "...", Text.Yellow);
            }
        }
        
        
        public static void ActivateRevivalItem(Client client, int itemNum) {
            int slot = 0;

            for (int i = 1; i <= client.Player.Inventory.Count; i++) {
                if (client.Player.Inventory[i].Num == itemNum && !client.Player.Inventory[i].Sticky) {
                    slot = i;
                    break;
                }
            }

            if (slot > 0) {
            	switch (itemNum) {
            		case 489: {//reviver seed
            			ReviveCharacter(client.Player.GetActiveRecruit(), client.Player.Map, false, null);
            			client.Player.TakeItemSlot(slot, 1, true);
                		client.Player.GiveItem(488, 1);
            		}
            		break;
            		case 452: {//revival herb
            			ReviveCharacter(client.Player.GetActiveRecruit(), client.Player.Map, true, null);
            			client.Player.TakeItemSlot(slot, 1, true);
            		}
            		break;
            		case 451: {//revive
            			foreach (Client target in client.Player.Map.GetClients()) {
            				if (target.Player.Dead) {
            					ReviveCharacter(target.Player.GetActiveRecruit(), target.Player.Map, false, null);
            				}
            			}
            			
            			client.Player.TakeItemSlot(slot, 1, true);
            		}
            		break;
            		case 450: {//max revive
            			foreach (Client target in client.Player.Map.GetClients()) {
            				if (target.Player.Dead) {
            					ReviveCharacter(target.Player.GetActiveRecruit(), target.Player.Map, true, null);
            				}
            			}
            			
            			client.Player.TakeItemSlot(slot, 1, true);
            		}
            		break;
            		case 88: {//restore power
            			foreach (Client target in client.Player.Map.GetClients()) {
            				if (target.Player.Dead && target != client) {
            					ReviveCharacter(target.Player.GetActiveRecruit(), target.Player.Map, true, null);
            				}
            			}
            			
            			client.Player.TakeItemSlot(slot, 1, true);
            		}
            		break;
            		case 749: {//sacred ash
            			foreach (Client target in client.Player.Map.GetClients()) {
            				if (target.Player.Dead) {
            					ReviveCharacter(target.Player.GetActiveRecruit(), target.Player.Map, true, null);
            					PacketHitList hitlist = null;
            					PacketHitList.MethodStart(ref hitlist);
    					        ChangeAttackBuff(target.Player.GetActiveRecruit(), target.Player.Map, 20, hitlist);
                                ChangeDefenseBuff(target.Player.GetActiveRecruit(), target.Player.Map, 20, hitlist);
                                ChangeSpAtkBuff(target.Player.GetActiveRecruit(), target.Player.Map, 20, hitlist);
                                ChangeSpDefBuff(target.Player.GetActiveRecruit(), target.Player.Map, 20, hitlist);
                                ChangeSpeedBuff(target.Player.GetActiveRecruit(), target.Player.Map, 20, hitlist);
                                ChangeAccuracyBuff(target.Player.GetActiveRecruit(), target.Player.Map, 20, hitlist);
                                ChangeEvasionBuff(target.Player.GetActiveRecruit(), target.Player.Map, 20, hitlist);
                                PacketHitList.MethodEnded(ref hitlist);
            				}
            			}
            			
            			client.Player.TakeItemSlot(slot, 1, true);
            		}
            		break;
            	}

            } else {
                Messenger.MapMsg(client.Player.MapID, client.Player.Name + " couldn't revive!", Text.BrightRed);
            }
        }

        public static void OnWeatherChange(IMap map, Enums.Weather oldWeather, Enums.Weather newWeather) {
            try {
                foreach (Client i in map.GetClients()) {
                    RefreshCharacterTraits(i.Player.GetActiveRecruit(), map, null);
                }

                foreach (MapNpc n in map.ActiveNpc) {
                    if (n.Num > 0) {
                        RefreshCharacterTraits(n, map, null);
                    }
                }
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: OnWeatherChange", Text.Black);
            }
        }

        public static bool IsDungeonFloorReady(Client client, RDungeonMap map) {
            if (map.PlayersOnMap.Count > 7) return false;
            
            if (map.MapType == Enums.MapType.RDungeonMap) {
				int dungeonNum = ((RDungeonMap)map).RDungeonIndex + 1;
				if (dungeonNum >= 71 && dungeonNum <= 87 && client.Player.HasItem(81) > 0) return false;
			}
            
            MapStatus status = map.TempStatus.GetStatus("Wind");
                    
            if (status != null) {
                    	
            	if (map.TimeLimit <= 0) {

            	} else if (status.Counter > map.TimeLimit - 240) {
                    return false;
                }
           }
           
           status = map.TempStatus.GetStatus("TotalTime");
           
           if (status != null) {
            
            	if (status.Counter > map.TimeLimit) {
                    return false;
                }
           }


            return true;

        }

		public static bool MaintenanceRestart = false;
        public static void OnMapLoad(Client client, IMap oldMap, IMap map, bool loggedIn) {
        	// Maintenance mode (external program is scheduled to run at this time)
        	TimeSpan start = new TimeSpan(1, 49, 0); //1:49 AM
			TimeSpan end = new TimeSpan(1, 51, 0); //1:51 AM
			TimeSpan now = DateTime.Now.TimeOfDay;
        	
        	if ((now > start) && (now < end)) {
        		if (MaintenanceRestart == false) {
        			MaintenanceRestart = true;
        			Messenger.AdminMsg("[Staff] Automated maintenance process starting...", Text.Red);
        			RestartServer();
        		}
			}
        	
        	start = new TimeSpan(2, 0, 0); //2 AM
			end = new TimeSpan(2, 10, 0); //2:10 AM (it takes about 10 minutes to run)
			now = DateTime.Now.TimeOfDay;
			
			if ((now > start) && (now < end)) {
			   Server.Globals.ServerStatus = "The server is currently in maintenance mode, which may result in slowdowns.";
			} else {
				if (Server.Globals.ServerStatus == "The server is currently in maintenance mode, which may result in slowdowns.") {
					Server.Globals.ServerStatus = "";
				}
			}
        
            //restore stats (temporary workaround)
            if (map.MapID == MapManager.GenerateMapID(Server.Settings.Crossroads)) {
                for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
                    if (client.Player.Team[i] != null) {
                        if (client.Player.Team[i].InTempMode) {

                            client.Player.RestoreRecruitStats(i);

                        }

                    }
                }
            }
            
            //remove from party
            if (oldMap != null && oldMap.Moral == Enums.MapMoral.None && map.Moral != Enums.MapMoral.None && exPlayer.Get(client).FirstMapLoaded == true) {
            	if (client.Player.PartyID != null) {
	                Party party = PartyManager.FindPlayerParty(client);
	                PartyManager.RemoveFromParty(party, client);
                }
                if (client.Player.Dead) {
	                	ReviveCharacter(client.Player.GetActiveRecruit(), client.Player.Map, false, null);
	                }
            }
            
            //if (!loggedIn) {
            //	if (client.Player.PartyID != null) {
	        //        Party party = PartyManager.FindPlayerParty(client);
	        //        bool remove = true;
	        //        foreach (Client member in party.GetOnlineMemberClients()) {
	        //        	if (member.Player.Map == client.Player.Map) {
	        //        		remove = false;
	        //        	}
	        //        }
	        //        if (remove) {
	        //        	PartyManager.RemoveFromParty(party, client);
	        //        }
            //    }
            //}


            //restore hunger
            if (!map.HungerEnabled) {
                for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
                    if (client.Player.Team[i] != null) {
                        if (client.Player.Team[i].MaxBelly > 100) {

                            client.Player.Team[i].MaxBelly = 100;

                        }
                        client.Player.Team[i].RestoreBelly();

                    }
                }
            }

            if (client.Player.LoggedIn) {
                AddExclusives(client, map, null);
            }


            int heldItemNum = -1;
            if (client.Player.GetActiveRecruit().HeldItemSlot > -1) {
                heldItemNum = client.Player.GetActiveRecruit().HeldItem.Num;
            }
            //if (client.Player.Helmet > -1) {
            //    helmet = client.Player.Helmet;
            //}
            //TODO: restructure for party items
            switch (heldItemNum) {
                //case 66: {//x-ray specs
                //        Messenger.PlayerMsg(client, "Pokémon in this area:", Text.Yellow);
                        //substitute the 14 for max NPCs
                //        List<int> Npcs = new List<int>();
                //        for (int i = 0; i < Constants.MAX_MAP_NPCS; i++) {
                //            if (Npcs.Contains(map.Npc[i].NpcNum) == false && map.Npc[i].NpcNum > 0) {
                //                Npcs.Add(map.Npc[i].NpcNum);
                //                Messenger.PlayerMsg(client, "Lv." + map.ActiveNpc[i].Level + " " + NpcManagerBase.Npcs[map.Npc[i].NpcNum].Name, Text.Yellow);
                //            }
                //        }
                        //} else {

                        //}

                        //if (Npcs.Count == 0) {
                        //    Messenger.PlayerMsg(client, "(None)", Text.Yellow);
                        //}

                    //}
                    //break;
                case 544: {//gaggle specs
                        for (int j = 0; j < 14; j++) {
                            map.ActiveNpc[j].Target = client;
                        }
                    }
                    break;
            }



            //Castform stuff
            if (client.Player.LoggedIn) {
                //exPlayer.Get(client).TurnsTaken = 0;
            }

            //for gen5 egg
            //bool hasTriggerEvent = false;
            //for (int i = 0; i < client.Player.TriggerEvents.Count; i++) {
            //    if (client.Player.TriggerEvents[i].Trigger == TriggerEventTrigger.StepCounter &&
            //        client.Player.TriggerEvents[i].ID == "MysteryEggStepCounter") {
            //        hasTriggerEvent = true;
            //        break;
            //    }
            //}

            //if (client.Player.HasItem(131) > 0 && !hasTriggerEvent) {
            //    StepCounterTriggerEvent triggerEvent = new StepCounterTriggerEvent("MysteryEggStepCounter", TriggerEventAction.RunScript, 2, true, client, 200);
            //    client.Player.AddTriggerEvent(triggerEvent);
            //Messenger.PlayerMsg(client, "1", Text.BrightRed);
            //}
            
            //CTF issues
            if (ActiveCTF != null &&
            	map.MapID != MapManager.GenerateMapID(CTF.REDMAP) &&
            	map.MapID != MapManager.GenerateMapID(CTF.BLUEMAP) && 
            	map.MapID != MapManager.GenerateMapID(CTF.HUBMAP)) {
                
                ActiveCTF.RemoveFromGame(client);
            }
            
           
            if (client.Player.CharID == Auction.AuctionMaster && client.Player.MapID != Auction.AUCTION_MAP) {
            	//Auction.EndAuction(client);
            	Auction.State = Auction.AuctionState.NotStarted;
            	Auction.AuctionMaster = null;
            }
            
            // Snowball game cleanup
            if (exPlayer.Get(client).SnowballGameInstance != null) {
            	SnowballGame snowballGame = exPlayer.Get(client).SnowballGameInstance;
            	if (client.Player.Map != snowballGame.ArenaMap
            		&& client.Player.MapID != SnowballGame.WaitingRoomMap) {
            		
            		snowballGame.RemoveFromGame(client);
            	}
            }
            
            if (!loggedIn) {

                if (client.Player.CurrentChapter == null && BossBattles.IsBossBattleMap(client)) {
                    bool empty = true;
                    for (int i = 0; i < Constants.MAX_MAP_NPCS; i++) {
                        if (!map.IsNpcSlotEmpty(i)) {
                            empty = false;
                            break;
                        }
                    }
                    if (empty) {
                    	// TODO: You can completely skip the boss battle if you relog while the intro story
                    	// 		 is playing and no NPC has been spawned yet. Disabled until a workaround is
                    	//		 made.
                    	// --Enabled until workaround is made.  The only thing achieved from getting around a boss is dungeon completion.
                        BossBattles.RunBossBattleCleanup(client, client.Player.Map.Name);
                    }
                }
            }
        }


        public static void OnMapLoaded(Client client, IMap map, PacketHitList packetList) {
            try {
                //make sure escorts and other allies aren't taken out of the dungeon
                if (map.Moral != Enums.MapMoral.None) {
                	int left = 0;
					for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
		                if (client.Player.Team[i].RecruitIndex < -1) {
		                    client.Player.RemoveFromTeam(i);
		                    left++;
		                }
		            }
		            if (left > 1) {
		                Messenger.PlayerMsg(client, "Some members left the team.", Text.Grey);
		                client.Player.SwapActiveRecruit(0);
		            } else if (left > 0) {
		                Messenger.PlayerMsg(client, "A member left the team.", Text.Grey);
		                client.Player.SwapActiveRecruit(0);
		            }
				}
				if (exPlayer.Get(client).FirstMapLoaded) {
	                if (map.Moral == Enums.MapMoral.None && client.Player.IsInParty()) {
	                    packetList.AddPacketToParty(client.Player.PartyID, PacketBuilder.CreateChatMsg("[Party]: " + client.Player.Name + " reached " + map.Name + ".", Text.Cyan));
	                }
	                
					
					for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
	                    if (client.Player.Team[i] != null && client.Player.Team[i].Loaded) {
	                		RemoveBuffs(client.Player.Team[i]);
	                	}
	                }
	
	                //remove statuses
	                RemoveAllBondedExtraStatus(client.Player.GetActiveRecruit(), map, packetList, false);
	                for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
	                    if (client.Player.Team[i] != null && client.Player.Team[i].Loaded) {
	                		client.Player.Team[i].VolatileStatus.Clear();
	                		RefreshCharacterTraits(client.Player.Team[i], map, packetList);
	                	}
	                }
	                
	
					for (int j = 0; j < Constants.MAX_ACTIVETEAM; j++) {
						if (client.Player.Team[j] != null && client.Player.Team[j].Loaded) {
			                if (HasAbility(client.Player.Team[j], "Illusion")) {
			                    int selectedSlot = j;
			                    
			                    for (int i = 1; i < Constants.MAX_ACTIVETEAM; i++) {
			                        if (client.Player.Team[(selectedSlot + i) % Constants.MAX_ACTIVETEAM] != null && client.Player.Team[(selectedSlot + i) % Constants.MAX_ACTIVETEAM].Loaded) {
			                            selectedSlot = (selectedSlot + i) % Constants.MAX_ACTIVETEAM;
			                            break;
			                        }
			                    }
			                    if (selectedSlot != j) {
			                        AddExtraStatus(client.Player.Team[j], client.Player.Map, "Illusion", client.Player.Team[selectedSlot].Species, null, "", packetList, false);
			                    }
			                }
			                
			                //set illusion
			                if (client.Player.GetActiveRecruit().HasActiveItem(213)) {
			                    AddExtraStatus(client.Player.Team[j], client.Player.Map, "Illusion", 25, null, "", packetList, false);
			                }
			                if (client.Player.GetActiveRecruit().HasActiveItem(224)) {
			                    AddExtraStatus(client.Player.Team[j], client.Player.Map, "Illusion", 300, null, "", packetList, false);
			                }
			                if (client.Player.GetActiveRecruit().HasActiveItem(244)) {
			                    AddExtraStatus(client.Player.Team[j], client.Player.Map, "Illusion", 387, null, "", packetList, false);
			                }
			                if (client.Player.GetActiveRecruit().HasActiveItem(245)) {
			                    AddExtraStatus(client.Player.Team[j], client.Player.Map, "Illusion", 390, null, "", packetList, false);
			                }
			                if (client.Player.GetActiveRecruit().HasActiveItem(246)) {
			                    AddExtraStatus(client.Player.Team[j], client.Player.Map, "Illusion", 393, null, "", packetList, false);
			                }
		                }
	                }
	                
	                if (client.Player.Map.MapType == Enums.MapType.RDungeonMap) {
	                	
                        if (HasAbility(client.Player.GetActiveRecruit(), "Honey Gather")
	                		&& client.Player.FindInvSlot(-1) > -1) {
                        	if (Server.Math.Rand(0, 400) <= client.Player.GetActiveRecruit().Level) {
		                        client.Player.GiveItem(11, 0, "");
		                        packetList.AddPacket(client, PacketBuilder.CreateBattleMsg(client.Player.GetActiveRecruit().Name + " gathered honey from somewhere.", Text.WhiteSmoke));
		                    }
                        }
                        
                    if (client.Player.Map.MapType == Enums.MapType.RDungeonMap) {
                    
		                for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
		                   	if (client.Player.GetActiveRecruit() != null && client.Player.GetActiveRecruit().Loaded && !client.Player.Dead) {
		                   		if (HasAbility(client.Player.GetActiveRecruit(), "Pickup")
			                		&& client.Player.FindInvSlot(-1) > -1) {
			                		RDungeonFloor floor = RDungeonManager.RDungeons[((RDungeonMap)map).RDungeonIndex].Floors[((RDungeonMap)map).RDungeonFloor];
                                	if (floor.Items.Count > 0) {
                                    	int rand = Server.Math.Rand(0, floor.Items.Count);
                                    	if (Server.Math.Rand(0, 200) <= floor.Items[rand].AppearanceRate) {
                                        	int amount = (floor.Items[rand].MinAmount + floor.Items[rand].MaxAmount) / 2;
                                        	client.Player.GiveItem(floor.Items[rand].ItemNum, amount, floor.Items[rand].Tag);
                                        	packetList.AddPacket(client, PacketBuilder.CreateBattleMsg(client.Player.GetActiveRecruit().Name + " found an item from somewhere.", Text.WhiteSmoke));
                                    	}
                                	}
                                }
			                }
		                }
	                }
	              }
	
	                
		            //if (client.Player.GetActiveRecruit().HasActiveItem(259) && client.Player.GetActiveRecruit().HeldItem.Num == 259 && map.Moral == Enums.MapMoral.House) {
		            //    AddExtraStatus(client.Player.GetActiveRecruit(), client.Player.Map, "Illusion", client.Player.GetActiveRecruit().HeldItem.Tag.ToInt(), null, "", packetList);
		            //}
	
	                
	
	                if (map.Moral == Enums.MapMoral.None
	                	|| map.Moral == Enums.MapMoral.NoPenalty
	                	|| (client.Player.GetActiveRecruit().X >= 0 && client.Player.GetActiveRecruit().X <= map.MaxX
			            && client.Player.GetActiveRecruit().Y >= 0 && client.Player.GetActiveRecruit().Y <= map.MaxY
			            && map.Tile[client.Player.GetActiveRecruit().X, client.Player.GetActiveRecruit().Y].Type == Enums.TileType.Arena
			            && !client.Player.Dead)) {
	                    if (HasAbility(client.Player.GetActiveRecruit(), "Sand Stream")) {
	                        SetMapWeather(map, Enums.Weather.Sandstorm, packetList);
	                    } else if (HasAbility(client.Player.GetActiveRecruit(), "Snow Warning")) {
	                        SetMapWeather(map, Enums.Weather.Hail, packetList);
	                    } else if (HasAbility(client.Player.GetActiveRecruit(), "Drizzle")) {
	                        SetMapWeather(map, Enums.Weather.Raining, packetList);
	                    } else if (HasAbility(client.Player.GetActiveRecruit(), "Drought")) {
	                        SetMapWeather(map, Enums.Weather.Sunny, packetList);
	                    } else if (HasAbility(client.Player.GetActiveRecruit(), "Air Lock")) {
	                        SetMapWeather(map, Enums.Weather.None, packetList);
	                    } else if (HasAbility(client.Player.GetActiveRecruit(), "Cloud Nine")) {
	                        SetMapWeather(map, Enums.Weather.None, packetList);
	                    }
	                }
	
					int totalDef = 0;
	                int totalSpDef = 0;
                    TargetCollection targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Floor, 0, map, client.Player.GetActiveRecruit(), client.Player.GetActiveRecruit().X, client.Player.GetActiveRecruit().Y, Enums.Direction.Up, true, false, false);
                    for (int i = 0; i < targets.Foes.Count; i++) {
                        totalDef += targets.Foes[i].Def;
                        totalSpDef += targets.Foes[i].SpclDef;
                    }
	                for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
	                   	if (client.Player.Team[i] != null && client.Player.Team[i].Loaded
	                   		&& HasAbility(client.Player.Team[i], "Download")) {
		                   	if (totalDef < totalSpDef) {
		                       	ChangeAttackBuff(client.Player.Team[i], map, 1, packetList);
		                   	} else {
		                       	ChangeSpAtkBuff(client.Player.Team[i], map, 1, packetList);
		                   	}
	                   	}
	                }
	
	                //if (HasAbility(client.Player.GetActiveRecruit(), "Natural Cure") && !CheckStatusProtection(client.Player.GetActiveRecruit(), map, Enums.StatusAilment.OK.ToString(), false, packetList)) {
	                //    packetList.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(client.Player.GetActiveRecruit().Name + "'s Natural Cure cured its status problem!", Text.WhiteSmoke), client.Player.X, client.Player.Y, 10);
	                //    SetStatusAilment(client.Player.GetActiveRecruit(), map, Enums.StatusAilment.OK, 0, packetList);
	                //}
	                for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
	                    if (client.Player.Team[i] != null && client.Player.Team[i].Loaded) {
	                        if (HasAbility(client.Player.Team[i], "Natural Cure") && !CheckStatusProtection(client.Player.Team[i], map, Enums.StatusAilment.OK.ToString(), false, packetList)) {
	                        	packetList.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(client.Player.Team[i].Name + "'s Natural Cure cured its status problem!", Text.WhiteSmoke), client.Player.X, client.Player.Y, 10);
	                    		SetStatusAilment(client.Player.Team[i], map, Enums.StatusAilment.OK, 0, packetList);
	                        }
	
	                    }
	                }
					
	                //if (HasAbility(client.Player.GetActiveRecruit(), "Regenerator") && client.Player.GetActiveRecruit().HP < client.Player.GetActiveRecruit().MaxHP) {
	                //    HealCharacter(client.Player.GetActiveRecruit(), map, client.Player.GetActiveRecruit().MaxHP / 3, packetList);
	                //}
	                
	                for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
	                    if (client.Player.Team[i] != null && client.Player.Team[i].Loaded) {
	                        if (HasAbility(client.Player.Team[i], "Regenerator") && client.Player.Team[i].HP < client.Player.Team[i].MaxHP) {
	                        	HealCharacter(client.Player.Team[i], map, client.Player.Team[i].MaxHP / 3, packetList);
	                        }
	
	                    }
	                }
                	
                	if (map.MapType == Enums.MapType.Instanced) {
                		//Messenger.AdminMsg("Players:"+map.PlayersOnMap.Count + " / " + BossBattles.IsBossBattleMap(client), Text.Pink);
                		if (BossBattles.IsBossBattleMap(client)) {
                			foreach (Client n in map.GetClients()) {
                				if (client == n) {
                					BossBattles.StartBossBattle(n, (InstancedMap)map);
                				}
                				break;
                			}
                		}
                	}
                	
                	
                	
                } else {
                	
                	//if (Ranks.IsAllowed(client, Enums.Rank.Admin)) {
                	//	Messenger.PlayerMsg(client, "!", Text.Black);
                	//}
                }
                exPlayer.Get(client).FirstMapLoaded = true;
                
                for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
	                if (client.Player.Team[i] != null && client.Player.Team[i].Loaded) {
	                	
	               		RefreshCharacterTraits(client.Player.Team[i], map, packetList);
	              	}
	            }
                

                StoryHelper.OnMapLoaded(client, map, packetList);
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: OnMapLoaded", Text.Black);
                Messenger.AdminMsg("Map: " + map.Name + "Client: " + client.Player.Name, Text.Black);
                //Messenger.AdminMsg(ex.ToString(), Text.Black);
            }
        }

        public static void OnMapReloaded(IMap map) {
            for (int i = 0; i < Constants.MAX_MAP_NPCS; i++) {
            	if (!map.IsNpcSlotEmpty(i)) {
            		map.ActiveNpc[i].CalculateOriginalStats();
                	RefreshCharacterTraits(map.ActiveNpc[i], map, null);
                }
            }
        }

        public static void AddExclusives(Client client, IMap map, PacketHitList packetlist) {
            int idNum = client.Player.CharID.Substring(client.Player.CharID.Length - 1, 1).ToInt();
            if (map.MapType == Enums.MapType.RDungeonMap) {
				int dungeonNum = ((RDungeonMap)map).RDungeonIndex + 1;
				int floor = ((RDungeonMap)map).RDungeonFloor + 1;
				bool onlyOne = false;
				if (client.Player.PartyID == null) {
					onlyOne = true;
				} else {
					Party party = PartyManager.FindPlayerParty(client);
					if (party.GetLeader() == client) onlyOne = true;
				}
				
				//shards
				if (floor >= 21) {
					if (Server.Math.Rand(0,2) == 0) {
						int itemNum = Server.Math.Rand(263,267);
						RandomItemSpawn(map, itemNum, 1, false, true, "");
					//Messenger.AdminMsg(itemNum.ToString(), Text.Pink);
					}
				}
				
				//thunderstorm forest
                if (dungeonNum == 8 && floor >= 8) {
                    if (idNum % 2 == 0) {
                        AddNpcToMap(map, 174, 20, 20, 50);
                    } else {
                        AddNpcToMap(map, 175, 20, 20, 50);
                    }
                }
				
				//winden forest
                if (dungeonNum == 28 && floor >= 13) {
                    if (idNum % 2 == 0) {
                        AddNpcToMap(map, 556, 30, 35, 10);
                    } else {
                        AddNpcToMap(map, 557, 30, 35, 10);
                    }
                }
				
				//harmonic tower
                if (dungeonNum == 37) {
                    if (floor > 20 && floor % 10 == 1) {
                        if (Server.Math.Rand(0, 5) == 0) {
                            if (idNum % 3 == 0) {
                                RandomItemSpawn(map, 110, 1, false, false, "495;10");
                            } else if (idNum % 3 == 1) {
                                RandomItemSpawn(map, 110, 1, false, false, "498;10");
                            } else {
                                RandomItemSpawn(map, 110, 1, false, false, "501;10");
                            }
                        }
                    }
                }
				
				//southern sea
                if (dungeonNum == 47 && floor > 20) {
                    if (Server.Math.Rand(0, 20) == 0) {
                        if (idNum % 2 == 0) {
                            RandomItemSpawn(map, 96, 1, false, false, "");
                        } else {
                            RandomItemSpawn(map, 102, 1, false, false, "");
                        }
                    }
                }
				if (onlyOne) {
					//tanren catacombs
					bool spawnKey = false;
					bool hidden = false;
					bool spawnMew = false;
					if (dungeonNum == 71) {
						if (floor == 2 || floor == 4 ||
							floor == 38 || floor == 46 ||
							floor == 56) {
								spawnKey = true;
						}
						if (floor == 51 || floor == 66 ||
							floor == 81 || floor == 95) {
							spawnMew = true;
						}
					} else if (dungeonNum == 72) {
						if (floor == 7 || floor == 9 ||
							floor == 35) {
								spawnKey = true;
						}
						if (floor == 56 || floor == 66 ||
							floor == 86) {
							spawnMew = true;
						}
					} else if (dungeonNum == 73) {
						if (floor == 10 || floor == 12 ||
							floor == 37 || floor == 55 ||
							floor == 79) {
								spawnKey = true;
						}
						if (floor == 51 || floor == 76 ||
							floor == 91) {
							spawnMew = true;
						}
					} else if (dungeonNum == 74) {
						if (floor == 9 || floor == 14 ||
							floor == 74) {
								spawnKey = true;
						}
						if (floor == 61 || floor == 71 ||
							floor == 86) {
							spawnMew = true;
						}
					} else if (dungeonNum == 75) {
						if (floor == 9 || floor == 16 ||
							floor == 66) {
								spawnKey = true;
						}
						if (floor == 91) {
							spawnMew = true;
						}
					} else if (dungeonNum == 76) {
						if (floor == 16 || floor == 28 ||
							floor == 49 || floor == 85) {
								spawnKey = true;
						}
						if (floor == 28) hidden = true;
						if (floor == 56 || floor == 82) {
							spawnMew = true;
						}
					} else if (dungeonNum == 77) {
						if (floor == 19 || floor == 39 ||
							floor == 46 || floor == 48) {
								spawnKey = true;
						}
						if (floor == 66) {
							spawnMew = true;
						}
					} else if (dungeonNum == 78) {
						if (floor == 32 || floor == 65 ||
							floor == 67) {
								spawnKey = true;
						}
						if (floor == 67) hidden = true;
						if (floor == 65 || floor == 76 ||
							floor == 86) {
							spawnMew = true;
						}
					} else if (dungeonNum == 79) {
						if (floor == 62 || floor == 85 ||
							floor == 74) {
								spawnKey = true;
						}
						if (floor == 55 || floor == 82) {
							spawnMew = true;
						}
					} else if (dungeonNum == 80) {
						if (floor == 25 || floor == 39 ||
							floor == 48) {
								spawnKey = true;
						}
						if (floor == 66) {
							spawnMew = true;
						}
					} else if (dungeonNum == 81) {
						if (floor == 30 || floor == 69) {
								spawnKey = true;
						}
						if (floor == 76) {
							spawnMew = true;
						}
					} else if (dungeonNum == 82) {
						if (floor == 70) {
								spawnKey = true;
						}
						if (floor == 81) {
							spawnMew = true;
						}
					} else if (dungeonNum == 83) {
						if (floor == 41 || floor == 53) {
								spawnKey = true;
						}
						if (floor == 61 || floor == 85) {
							spawnMew = true;
						}
					} else if (dungeonNum == 84) {
						if (floor == 62 || floor == 83 ||
							floor == 89) {
								spawnKey = true;
						}
						if (floor == 81) {
							spawnMew = true;
						}
					} else if (dungeonNum == 85) {
						if (floor == 70) {
								spawnKey = true;
						}
						if (floor == 76) {
							spawnMew = true;
						}
					} else if (dungeonNum == 86) {
						if (floor == 18 || floor == 89) {
								spawnKey = true;
						}
						if (floor == 89) hidden = true;
						if (floor == 61 || floor == 85) {
							spawnMew = true;
						}
					} else if (dungeonNum == 87) {
						if (floor == 81) {
								spawnKey = true;
						}
						if (floor == 66) {
							spawnMew = true;
						}
					}
					//if (map.PlayersOnMap.Count > 0) {
					//	spawnKey = false;
					//	spawnMew = false;
					//}
					if (spawnKey) {
						RandomItemSpawn(map, 81, 1, false, hidden, "");
						if (client.Player.Name == "Sprinko") {
							Messenger.PlayerMsg(client, "Key spawned.", Text.Pink);
						}
					}
					if (spawnMew) {
						AddNpcToMap(map, map.Npc[0].NpcNum, 50, 50, 100);
						AddNpcToMap(map, 800, 80, 80, 100);
					}
				}
				
				//easter egg
                if (dungeonNum == 40 && floor > 35 && Server.Math.Rand(0, 2) == 0) {
                    //string tag = "";
                    int species = client.Player.GetActiveRecruit().Species;
                    for (int i = 0; i < Server.Evolutions.EvolutionManager.Evolutions.MaxEvos; i++)
                    {
                        foreach (Server.Evolutions.EvolutionBranch branch in Server.Evolutions.EvolutionManager.Evolutions[i].Branches) {
                            if (branch.NewSpecies == species) {
                                species = Server.Evolutions.EvolutionManager.Evolutions[i].Species;
                            }
                        }

                    }
                    for (int i = 0; i < Server.Evolutions.EvolutionManager.Evolutions.MaxEvos; i++)
                    {
                        foreach (Server.Evolutions.EvolutionBranch branch in Server.Evolutions.EvolutionManager.Evolutions[i].Branches) {
                            if (branch.NewSpecies == species) {
                                species = Server.Evolutions.EvolutionManager.Evolutions[i].Species;
                            }
                        }

                    }

                    if (Server.Math.Rand(0, 3) == 0) {
                        //Messenger.AdminMsg("[Staff] A scripted egg was generated.  Tag is: " + species + ";500", Text.Black);
                        //Messenger.AdminMsg("Source was: " + client.Player.GetActiveRecruit().Species, Text.Black);
                        RandomItemSpawn(map, 110, 1, false, false, species + ";80");
                    }
                }

            }
        }


        public static void AddNpcToMap(IMap map, int npcNum, int minLevel, int maxLevel, int appearanceRate) {
            MapNpcPreset npc = new MapNpcPreset();
            npc.NpcNum = npcNum;
            npc.MinLevel = minLevel;
            npc.MaxLevel = maxLevel;
            npc.AppearanceRate = appearanceRate;
            map.Npc.Add(npc);
        }

        public static void SpawnNpcToMap(IMap map, int npcNum, int minLevel, int maxLevel, int spawnX, int spawnY, PacketHitList packetlist) {
            PacketHitList.MethodStart(ref packetlist);
            MapNpcPreset npc = new MapNpcPreset();
            npc.SpawnX = spawnX;
            npc.SpawnY = spawnY;
            npc.NpcNum = npcNum;
            npc.MinLevel = minLevel;
            npc.MaxLevel = maxLevel;
            map.SpawnNpc(npc);
            PacketHitList.MethodEnded(ref packetlist);
        }

        public static void RandomItemSpawn(IMap map, int itemNum, int amount, bool sticky, bool hidden, string tag) {
            int x, y;

            FindFreeTile(map, 0, 0, map.MaxX, map.MaxY, out x, out y);

            if (x != -1) {
                map.SpawnItem(itemNum, amount, sticky, hidden, tag, x, y, null);
            }
        }

        public static bool MapContainsNpc(IMap map, int npcNum) {
            for (int i = 0; i < Constants.MAX_MAP_NPCS; i++) {
                if (map.ActiveNpc[i].Num > npcNum) {
                    return true;
                }
            }

            return false;
        }


        public static void EnterRDungeon(Client client, int dungeonNum, int floor) {
            // Called when a player warps to a random dungeon/changes floors

            if (client.Player.LoggedIn) {
                //exPlayer.Get(client).TurnsTaken = 0;
            }

            // Floors are 0-index, meaning Floor 1 = floor 0
            int heldItemNum = -1;
            if (client.Player.GetActiveRecruit().HeldItemSlot > -1) {
                heldItemNum = client.Player.GetActiveRecruit().HeldItem.Num;
            }
            switch (heldItemNum) {
                //case 66: {//x-ray specs
                //        IMap map = client.Player.Map;
                //        Messenger.PlayerMsg(client, "Pokémon in this area:", Text.Yellow);
                //        List<int> Npcs = new List<int>();
                        //if (NetScript.GetPlayerMap(index) != -2) {
                //        for (int i = 0; i <= 14; i++) {
                //            if (Npcs.Contains(map.ActiveNpc[i].Num) == false && map.ActiveNpc[i].Num > 0) {
                //                Npcs.Add(map.ActiveNpc[i].Num);
                //                Messenger.PlayerMsg(client, "Lv." + map.ActiveNpc[i].Level + " " + NpcManager.Npcs[Npcs[i]].Name, Text.Yellow);
                //            }
                //        }
                        //} else {

                        //}

                        //if (Npcs.Count == 0) {
                        //    Messenger.PlayerMsg(client, "(None)", Text.Yellow);
                        //}

                    //}
                    //break;
                case 217: {//Holly Neck Band
                        // TODO: Holly Neck Band
                        //NetScript.SetPlayerMP(index, NetScript.GetPlayerMP(index) + NetScript.GetPlayerMaxMP(index) / 4);
                        //Messenger.BattleMsg(client, "You regained PP!", Text.BrightGreen);
                        //Messenger.PlaySound(client, "magic60.wav");
                        //NetScript.SendMP(index);
                    }
                    break;
                case 219: {//Holly Hat
                        client.Player.GetActiveRecruit().HP += (client.Player.GetMaxHP() / 4);
                        Messenger.BattleMsg(client, "You regained HP!", Text.BrightGreen);
                        Messenger.PlaySound(client, "magic60.wav");

                    }
                    break;
                case 218: {//Crown
                        client.Player.GetActiveRecruit().DefenseBuff += 1;
                    }
                    break;
                case 216: {//Tiara
                        client.Player.GetActiveRecruit().SpAtkBuff += 1;
                    }
                    break;
                case 544: {//Gaggle specs
                        IMap map = client.Player.Map;

                        for (int j = 0; j <= 14; j++) {
                            map.ActiveNpc[j].Target = client;
                        }
                    }
                    break;
            }
            //if (((RDungeonMap)client.Player.Map).RDungeonIndex >= 15 && ((RDungeonMap)client.Player.Map).RDungeonIndex < 21) {
            //    client.Player.SwapActiveRecruit(0);
            //    client.Player.RemoveFromTeam(1);
            //    client.Player.RemoveFromTeam(2);
            //    client.Player.RemoveFromTeam(3);
                //NetScript.BattleMsg(index, "Your team members returned home!", Text.BrightRed, 0);
            //    Messenger.SendActiveTeam(client);
            //}
            //if (((RDungeonMap)client.Player.Map).RDungeonIndex == 19 && floor > 6) {
                // TODO: PP drained (EnterRDungeon) [Scripts]
                //NetScript.SetPlayerMP(index, NetScript.GetPlayerMP(index) / 2);
                //NetScript.SendMP(index);
                //NetScript.PlaySound(index, "magic63.wav");
                //NetScript.BattleMsg(index, "You feel your energy being drained...", Text.BrightRed, 0);
            //}
            //if (((RDungeonMap)client.Player.Map).RDungeonIndex == 20 && floor > 78) {
            //    if (client.Player.Timers.TimerExists("Wind")) {
            //        client.Player.Timers.RemoveTimer("Wind");
            //    }
            //    client.Player.Timers.CreateTimer("Wind", "Wind", 60000, true, false, client);
            //}

            if (dungeonNum == 38 - 1) {
                ElectrostasisTower.EnterRDungeon(client, dungeonNum, floor);
            }
        }

        public static bool IsMissionAcceptable(Client client, WonderMail mail) {
            bool noItems = false;

            if (DungeonManager.Dungeons[mail.DungeonIndex].ScriptList.Count >= 2) {
                string[] reqString = DungeonManager.Dungeons[mail.DungeonIndex].ScriptList[1].Split(';');

                for (int i = 0; i < reqString.Length; i++) {

                    if (reqString[i] == "3,0") {
                        noItems = true;
                    }
                }

                if (mail.MissionType == Enums.MissionType.ItemRetrieval && noItems) {
                    return false;
                }
            }

            if (mail.Difficulty >= Enums.JobDifficulty.SixStar && client.Player.ExplorerRank < Enums.ExplorerRank.Super) {
                return false;
            } else if (mail.Difficulty >= Enums.JobDifficulty.Star && client.Player.ExplorerRank < Enums.ExplorerRank.Gold) {
                return false;
            } else if (mail.Difficulty >= Enums.JobDifficulty.S && client.Player.ExplorerRank < Enums.ExplorerRank.Silver) {
                return false;
            } else if (mail.Difficulty >= Enums.JobDifficulty.B && client.Player.ExplorerRank < Enums.ExplorerRank.Bronze) {
                return false;
            }

            return true;
        }

        public static void CreateMissionInfo(WonderMail mail) {
        	MissionPool pool = WonderMailManager.Missions[(int)mail.Difficulty - 1];
            switch (mail.MissionType) {
                case Enums.MissionType.Rescue: {
                	int total = mail.DungeonIndex*pool.MissionClients.Count*pool.Rewards.Count
                		+ mail.MissionClientIndex*pool.Rewards.Count
                		+ mail.RewardIndex;
                	switch (total % 8) {
                		case 0: {
		                    mail.Title = "I can't get home!";
		                    mail.Summary = "Whoa!  It's too rough in here... Please, I need help!";
                    	}
                    	break;
                		case 1: {
		                    mail.Title = "I can't get home!";
		                    mail.Summary = "I was set upon by bandits!  Whoa!  Someone!  Help!";
                    	}
                    	break;
                		case 2: {
		                    mail.Title = "I was KO'd...";
		                    mail.Summary = "I can't get back!  I'm fading... Help...";
                    	}
                    	break;
                		case 3: {
		                    mail.Title = "I'm scared!";
		                    mail.Summary = "My exploration went astray!  I'm fading... Help...";
                    	}
                    	break;
                		case 4: {
		                    mail.Title = "Where am I?";
		                    mail.Summary = "I can't take another step...";
                    	}
                    	break;
                		case 5: {
		                    mail.Title = "I fainted...";
		                    mail.Summary = "I got lost in this dungeon!  Someone, please!";
                    	}
                    	break;
                		case 6: {
		                    mail.Title = "I'm sad and lonely...";
		                    mail.Summary = "Fighting that tough foe was a mistake... Aiyeeeeeeeeh!";
                    	}
                    	break;
                		case 7: {
		                    mail.Title = "I'm sad and lonely...";
		                    mail.Summary = "This dungeon is scary! My consciousness is slipping... Help...";
                    	}
                    	break;
                    }
                    }
                    break;
                case Enums.MissionType.ItemRetrieval: {
                    Item item = ItemManager.Items[mail.Data1];
                    int total = mail.DungeonIndex*pool.MissionClients.Count*pool.Rewards.Count
                		+ mail.MissionClientIndex*pool.Rewards.Count
                		+ mail.RewardIndex;
                	switch (total % 2) {
                		case 0: {
	                    	mail.Title = "Find " + item.Name + ".";
		                    mail.Summary = "I'm looking for a " + item.Name + "!  Please help me find it!";
	                    }
	                    break;
                		case 1: {
	                    	mail.Title = "Deliver one " + item.Name + ".";
	                    	mail.Summary = "I can't even find one " + item.Name + ".  Please share one with me!";
	                    }
	                    break;
	                }
                    
                    }
                    break;
                case Enums.MissionType.Escort: {
                	int total = mail.DungeonIndex*pool.MissionClients.Count*pool.Rewards.Count
                		+ mail.MissionClientIndex*pool.Rewards.Count
                		+ mail.RewardIndex;
                	switch (total % 2) {
                		case 0: {
	                    	mail.Title = "Take me!";
	                    	mail.Summary = "I want to see " + Pokedex.GetPokemon(mail.Data1).Name + ", but I can't make it alone... can somebody help?";
	                    }
	                    break;
                		case 1: {
	                    	mail.Title = "Please, take me with you!";
	                    	mail.Summary = "I want to meet with " + Pokedex.GetPokemon(mail.Data1).Name + ".  Someone, please escort me!";
	                    }
	                    break;
	                }
                    }
                    break;
            }
            mail.Mugshot = WonderMailManager.Missions.MissionPools[(int)mail.Difficulty - 1].MissionClients[mail.MissionClientIndex].Species;
        }

        public static void OnMissionComplete(Client client, int missionIndex) {
            try {
                Story story = new Story();
                StoryBuilderSegment segment = StoryBuilder.BuildStory();
                StoryBuilder.AppendSaySegment(segment, "You have completed a mission!  Return to the mission board to claim your reward.", -1, 0, 0);
                segment.AppendToStory(story);
                StoryManager.PlayStory(client, story);

            } catch (Exception ex) {
                Messenger.AdminMsg("Error: OnMissionComplete", Text.Black);
            }
        }
        
        public static void PostDungeonCompletion(Client client, int dungeonIndex) {
        	if (dungeonIndex == 23) {
        		exPlayer.Get(client).SpawnMap = Main.Crossroads;
                exPlayer.Get(client).SpawnX = 25;
                exPlayer.Get(client).SpawnY = 25;
                StoryManager.PlayStory(client, 205);
        	}
        }


        public static void ScriptedMissionWinDump(Client client) {
            try {
                int completedJobs = 0;
                
                Story story = new Story();
                StoryBuilderSegment segment = StoryBuilder.BuildStory();
                StoryBuilder.AppendWarpAction(segment, "s737", 39, 17);
                StoryBuilder.AppendPlayMusicAction(segment, "PMD2) Job Clear!.ogg", true, true);
                foreach (WonderMailJob mission in client.Player.JobList.JobList) {
                    if (mission.Accepted == Enums.JobStatus.Finished) {
                        completedJobs++;
                        MissionPool missionPool = WonderMailManager.Missions[(int)mission.Mission.Difficulty - 1];


                        StoryBuilder.AppendCreateFNPCAction(segment, "0", "s737", 39, 16, missionPool.MissionClients[mission.Mission.MissionClientIndex].Species);
                        StoryBuilder.AppendChangeFNPCDirAction(segment, "0", Enums.Direction.Down);
                        if (mission.Mission.MissionType == Enums.MissionType.Escort) {
                            PokemonForm target = Pokedex.GetPokemonForm(mission.Mission.Data1, mission.Mission.Data1);
                            StoryBuilder.AppendCreateFNPCAction(segment, "1", "s737", 40, 16, missionPool.MissionClients[mission.Mission.MissionClientIndex].Species);
                            StoryBuilder.AppendChangeFNPCDirAction(segment, "1", Enums.Direction.Down);
                        }
                        if (mission.Mission.MissionType == Enums.MissionType.Escort) {
                            StoryBuilder.AppendSaySegment(segment, "Thank you for taking me to " + Pokedex.GetPokemon(mission.Mission.Data1).Name + "! Please accept our reward!", missionPool.MissionClients[mission.Mission.MissionClientIndex].Species, 0, 0);
                        } else if (mission.Mission.MissionType == Enums.MissionType.ItemRetrieval) {
                            StoryBuilder.AppendSaySegment(segment, "Thank you for the " + ItemManager.Items[mission.Mission.Data1].Name + "! Please accept this reward!", missionPool.MissionClients[mission.Mission.MissionClientIndex].Species, 0, 0);
                        } else if (mission.Mission.MissionType == Enums.MissionType.Rescue) {
                            StoryBuilder.AppendSaySegment(segment, "Thank you for rescuing me! Please accept this reward!", missionPool.MissionClients[mission.Mission.MissionClientIndex].Species, 0, 0);
                        }

                        //List<Server.WonderMails.MissionReward> rewards = Server.WonderMails.WonderMailSystem.RewardInfo[mission.WonderMail.RewardIndex].Rewards;
                        //for (int i = 0; i < rewards.Count; i++) {
                        //    StoryBuilder.AppendSaySegment(segment, "Obtained " + rewards[i].Amount + " " + ItemManager.Items[rewards[i].ItemNum].Name + "!", -1, 0, 0);
                        //}
                        if (ItemManager.Items[missionPool.Rewards[mission.Mission.RewardIndex].ItemNum].StackCap > 0) {
                            StoryBuilder.AppendSaySegment(segment, "Obtained " + missionPool.Rewards[mission.Mission.RewardIndex].Amount + " " + ItemManager.Items[missionPool.Rewards[mission.Mission.RewardIndex].ItemNum].Name + "!", -1, 0, 0);
                        } else {
                            StoryBuilder.AppendSaySegment(segment, "Obtained a " + ItemManager.Items[missionPool.Rewards[mission.Mission.RewardIndex].ItemNum].Name + "!", -1, 0, 0);
                        }
                        StoryBuilder.AppendMapVisibilityAction(segment, false);
                        StoryBuilder.AppendDeleteFNPCAction(segment, "0");
                        if (mission.Mission.MissionType == Enums.MissionType.Escort) {
                            StoryBuilder.AppendDeleteFNPCAction(segment, "1");
                        }
                        int expRewarded = MissionManager.DetermineMissionExpReward(mission.Mission.Difficulty);
                        //if (mission.WonderMail.MissionExp == -1) {
                        //    expRewarded = MissionManager.DetermineMissionExpReward(mission.WonderMail.Difficulty);
                        //} else {
                        //    expRewarded = mission.WonderMail.MissionExp;
                        //}
                        if (expRewarded > 0) {
                            StoryBuilder.AppendSaySegment(segment, "You have been awarded with " + expRewarded + " explorer points!", -1, 0, 0);
                        }
                        StoryBuilder.AppendPauseAction(segment, 1000);
                        StoryBuilder.AppendMapVisibilityAction(segment, true);
                    }
                }

                StoryBuilder.AppendPlayMusicAction(segment, "%mapmusic%", true, true);
                segment.AppendToStory(story);

                if (completedJobs > 0) {
                    StoryManager.PlayStory(client, story);
                }


            } catch (Exception ex) {
                Messenger.AdminMsg("Error: ScriptedMissionWinDump", Text.Black);
            }
        }

        public static void OnMissionFailed(Client client, int missionIndex) {
            try {
                //PacketHitList hitlist = null;
                //PacketHitList.MethodStart(ref hitlist);

                //client.Player.GetActiveRecruit().StatusAilment = Enums.StatusAilment.OK;
                //client.Player.GetActiveRecruit().StatusAilmentCounter = 0;



                //RemoveInvOnDeath(client);
                //exPlayer.Get(client).WarpToSpawn(false);
                //client.Player.GetActiveRecruit().RestoreBelly();
                //client.Player.GetActiveRecruit().RestoreVitals();

                //PacketBuilder.AppendStats(client, hitlist);

                //PacketHitList.MethodEnded(ref hitlist);
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: OnMissionFailed", Text.Black);
            }
        }

        public static void TriggerEventScript(Client client, Server.Events.Player.TriggerEvents.ITriggerEvent triggerEvent) {
            try {
                switch (triggerEvent.TriggerCommand) {
                    // If the trigger command is -1, identify the action based on the ID
                    case -1: {
                            switch (triggerEvent.ID) {
                                default: {
                                        Messenger.PlayerMsg(client, "No trigger event script found. Please contact an Administrator!", Text.BrightRed);
                                    }
                                    break;
                            }
                        }
                        break;
                    case 1: { // Electrostasis Tower Sublevel Goal [Stepped On Tile]
                            Server.Events.Player.TriggerEvents.SteppedOnTileTriggerEvent tEvent = triggerEvent as Server.Events.Player.TriggerEvents.SteppedOnTileTriggerEvent;
                            if (tEvent != null) {
                                ElectrostasisTower.ReachedSublevelGoal(client, tEvent);
                            }
                        }
                        break;
                    case 2: {//egg
                            //Messenger.PlayerMsg(client, "2", Text.BrightRed);
                            if (client.Player.HasItem(131) > 0 && client.Player.FindOpenTeamSlot() > -1) {
                                StoryManager.PlayStory(client, Server.Math.Rand(0, 3) + 235);
                            }
                        }
                        break;
                    default: {
                            Messenger.PlayerMsg(client, "No trigger event script found. Please contact an Administrator!", Text.BrightRed);
                        }
                        break;
                }
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: TriggerEventScript", Text.Black);
            }
        }

        public static void OnPlayerRegisteredInTournament(Tournament tournament, TournamentMember member, bool isSpectator) {
            try {
                if (member.Client.Player.MapID == MapManager.GenerateMapID(1192)) {
                    if (!isSpectator) {
                        Story story = new Story();
                        StoryBuilderSegment segment = StoryBuilder.BuildStory();

                        StoryBuilder.AppendSaySegment(segment, "Please step into the left door.", -1, 0, 0);
                        StoryBuilder.AppendMovePlayerAction(segment, 9, 1, Enums.Speed.Walking, true);

                        segment.AppendToStory(story);
                        StoryManager.PlayStory(member.Client, story);
                    } else {
                        // Player is a spectator
                        tournament.WarpToHub(member.Client);
                    }
                }
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: OnPlayerRegisteredInTournament", Text.Black);
            }
        }

        public static void OnTournamentRoundComplete(Tournament tournament) {
            try {
                tournament.StartRound(new MatchUpRules());
                Story playerOneStory = new Story();
                StoryBuilderSegment playerOneSegment = StoryBuilder.BuildStory();
                StoryBuilder.AppendSaySegment(playerOneSegment, "Your match is ready to begin!", -1, 0, 0);
                StoryBuilder.AppendMovePlayerAction(playerOneSegment, 8, 2, Enums.Speed.Walking, true);
                playerOneSegment.AppendToStory(playerOneStory);

                Story playerTwoStory = new Story();
                StoryBuilderSegment playerTwoSegment = StoryBuilder.BuildStory();
                StoryBuilder.AppendSaySegment(playerTwoSegment, "Your match is ready to begin!", -1, 0, 0);
                StoryBuilder.AppendMovePlayerAction(playerTwoSegment, 11, 2, Enums.Speed.Walking, true);
                playerTwoSegment.AppendToStory(playerTwoStory);

                for (int i = 0; i < tournament.ActiveMatchups.Count; i++) {
                    StoryManager.PlayStory(tournament.ActiveMatchups[i].PlayerOne.Client, playerOneStory);
                    StoryManager.PlayStory(tournament.ActiveMatchups[i].PlayerTwo.Client, playerTwoStory);
                }
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: OnTournamentRoundComplete", Text.Black);
            }
        }

        public static void OnTournamentCanceled(Tournament tournament, string reason) {
            try {
                Story story = new Story();
                StoryBuilderSegment segment = StoryBuilder.BuildStory();

                StoryBuilder.AppendSaySegment(segment, "The tournament was canceled! " + reason, -1, 0, 0);
                segment.AppendToStory(story);

                for (int i = 0; i < tournament.RegisteredMembers.Count; i++) {
                    StoryManager.PlayStory(tournament.RegisteredMembers[i].Client, story);
                }
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: OnTournamentCanceled", Text.Black);
            }
        }

        public static void OnTournamentComplete(Tournament tournament, TournamentMember member) {
            Messenger.AdminMsg("[Staff] The winner was " + member.Client.Player.Name + "!", Text.BrightBlue);
        }

        //RejoinSnowballGame

        private static string GetRankName(Enums.Rank rank) {
            switch (rank) {
                case Enums.Rank.Normal:
                    return "";
                case Enums.Rank.Moniter:
                    return "Mod";
                case Enums.Rank.Mapper:
                    return "Mapper";
                case Enums.Rank.Developer:
                    return "Dev";
                case Enums.Rank.Admin:
                    return "Admin";
                case Enums.Rank.ServerHost:
                    return "Admin";
                case Enums.Rank.Scripter:
                    return "Admin";
                default:
                    return "";
            }
        }



        private static void InvDrop(ICharacter character, IMap map, PacketHitList hitlist) {
            PacketHitList.MethodStart(ref hitlist);
            if (HasAbility(character, "Sticky Hold")) {
                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " held on to its item with Sticky Hold!", Text.WhiteSmoke), character.X, character.Y, 10);
            } else {
                if (character.CharacterType == Enums.CharacterType.Recruit) {
                    Client client = ((Recruit)character).Owner;
                    if (client.Player.GetActiveRecruit().HeldItemSlot > -1) {
                        client.Player.DropItem(client.Player.GetActiveRecruit().HeldItemSlot, client.Player.GetActiveRecruit().HeldItem.Amount, client);
                    }
                    PacketBuilder.AppendInventory(client, hitlist);
                } else if (character.CharacterType == Enums.CharacterType.MapNpc) {
                    MapNpc npc = character as MapNpc;
                    if (npc.HeldItem != null && !npc.HeldItem.Sticky) {
                        npc.MapDropItem(npc.HeldItem.Amount, null);
                    }
                }
            }
            PacketHitList.MethodEnded(ref hitlist);
        }

        private static void Sticky(ICharacter character, IMap map, PacketHitList hitlist) {
            PacketHitList.MethodStart(ref hitlist);
            if (character.HasActiveItem(421)) {
            	hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s No-Stick Cap prevented any items from becoming sticky!", Text.WhiteSmoke), character.X, character.Y, 10);
            } else {
            
	            if (character.CharacterType == Enums.CharacterType.Recruit) {
	                Client client = ((Recruit)character).Owner;
	                List<int> itemsToCheck = new List<int>();
	                for (int i = 1; i <= client.Player.MaxInv; i++) {
	                	if (client.Player.Inventory[i].Num > 0 && !client.Player.Inventory[i].Sticky) {
	                		itemsToCheck.Add(i);
	                	}
	                }
	                if (itemsToCheck.Count > 0) {
		                int stickySlot = itemsToCheck[Server.Math.Rand(0, itemsToCheck.Count)];
		                if (client.Player.Inventory[stickySlot].Num > 0 && !client.Player.Inventory[stickySlot].Sticky) {
		                    client.Player.SetItemSticky(stickySlot, true);
		                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s " + ItemManager.Items[client.Player.Inventory[stickySlot].Num].Name + " became sticky!", Text.BrightRed), character.X, character.Y, 10);
		                } else {
		                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("Nothing particularly bad happened to " + character.Name + "...", Text.Blue), character.X, character.Y, 10);
		                }
	                } else {
	                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("Nothing particularly bad happened to " + character.Name + "...", Text.Blue), character.X, character.Y, 10);
	                }
	            } else if (character.CharacterType == Enums.CharacterType.MapNpc) {
	                MapNpc npc = character as MapNpc;
	                if (npc.HeldItem != null && !npc.HeldItem.Sticky) {
	                    npc.HeldItem.Sticky = true;
	                    if (npc.ActiveItems.Contains(npc.HeldItem.Num)) {
	                        npc.RemoveFromActiveItemList(npc.HeldItem.Num);
	                    }
	                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s " + ItemManager.Items[npc.HeldItem.Num].Name + " became sticky!", Text.BrightRed), character.X, character.Y, 10);
	                } else {
	                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("Nothing particularly bad happened to " + character.Name + "...", Text.Blue), character.X, character.Y, 10);
	                }
	            }
            }
            PacketHitList.MethodEnded(ref hitlist);
        }

        private static void Grimy(ICharacter character, IMap map, PacketHitList hitlist) {
            PacketHitList.MethodStart(ref hitlist);
            if (character.CharacterType == Enums.CharacterType.Recruit) {
                Client client = ((Recruit)character).Owner;
                int totalGrimy = 0;

                for (int i = 1; i <= client.Player.MaxInv; i++) {
                    if (Server.Math.Rand(0, 3) == 0) {
                        int itemEdibility = CheckItemEdibility(client.Player.Inventory[i].Num);
                        if (itemEdibility == 1) {
                            client.Player.Inventory[i].Tag = client.Player.Inventory[i].Num.ToString();
                            client.Player.Inventory[i].Num = 75;
                            totalGrimy++;
                        } else if (itemEdibility == 2) {
                            client.Player.Inventory[i].Tag = client.Player.Inventory[i].Num.ToString();
                            client.Player.Inventory[i].Num = 74;
                            totalGrimy++;
                        }
                    }
                }

                if (totalGrimy > 1) {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("Several of " + character.Name + "'s food items went bad!", Text.BrightRed), character.X, character.Y, 10);
                } else if (totalGrimy > 0) {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("One of " + character.Name + "'s food items went bad!", Text.BrightRed), character.X, character.Y, 10);
                } else {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("Nothing particularly bad happened to " + character.Name + "...", Text.Blue), character.X, character.Y, 10);
                }
                PacketBuilder.AppendInventory(client, hitlist);
            } else if (character.CharacterType == Enums.CharacterType.MapNpc) {
                MapNpc npc = character as MapNpc;
                if (npc.HeldItem != null && Server.Math.Rand(0, 3) == 0) {
                    int itemEdibility = CheckItemEdibility(npc.HeldItem.Num);
                    if (itemEdibility == 1) {
                        npc.HeldItem.Tag = npc.HeldItem.Num.ToString();
                        npc.HeldItem.Num = 75;
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s food item went bad!", Text.BrightRed), character.X, character.Y, 10);
                    } else if (itemEdibility == 3) {
                        npc.HeldItem.Tag = npc.HeldItem.Num.ToString();
                        npc.HeldItem.Num = 74;
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s food item went bad!", Text.BrightRed), character.X, character.Y, 10);
                    } else {
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("Nothing particularly bad happened to " + character.Name + "...", Text.Blue), character.X, character.Y, 10);
                    }
                } else {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("Nothing particularly bad happened to " + character.Name + "...", Text.Blue), character.X, character.Y, 10);
                }
            }
            PacketHitList.MethodEnded(ref hitlist);
        }

        private static int CheckItemEdibility(int itemNum) {
            switch (itemNum) {
                case 2:
                case 7:
                case 9:
                case 21:
                case 155:
                case 811:
                case 812:
                case 820: {
                    return 1;//apple/banana
                    }
                case 503:
                case 504:
                case 505:
                case 506:
                case 507:
                case 508:
                case 509:
                case 510:
                case 511:
                case 512:
                case 513:
                case 514:
                case 515:
                case 516:
                case 517:
                case 518:
                case 519:
                case 520: {
                        return 2;//gummi
                    }
                case 4:
                case 8:
                case 86:
                case 487:
                case 492:
                case 493:
                case 494:
                case 495:
                case 496:
                case 497:
                case 523:
                case 524:
                case 525:
                case 526:
                case 527:
                case 528:
                case 529:
                case 530:
                case 641:
                case 642:
                case 643:
                case 644:
                case 645:
                case 646:
                case 647:
                case 648:
                case 649:
                case 650:
                case 651:
                case 652:
                case 653:
                case 654:
                case 655:
                case 656:
                case 657: {
                        return 3;//Berry -> Grimy Berry
                    }
                case 3:
                case 20:
                case 173:
                case 341:
                case 397:
                case 449:
                case 460:
                case 478:
                case 488:
                case 489:
                case 490:
                case 491:
                case 803: {
                        return 4;//seed
                    }
                case 128:
                case 132:
                case 150:
                case 72: {
                        return 5;//herb
                    }
                case 34:
                case 35:
                case 36:
                case 37:
                case 39:
                case 54:
                case 55: {
                        return 6;//snack
                    }
                case 74:
                case 75:
                case 85: {
                        return 7;//bad food
                    }
                case 5:
                case 10:
                case 23:
                case 42:
                case 105:
                case 470:
                case 471:
                case 472:
                case 473:
                case 484:
                case 485: 
                case 557: {
                        return 8;//drinkable
                    }
                case 239:
                case 98:
                case 22:
                case 47:
                case 718:
                case 359:
                case 360:
                case 361: {
                        return 9;//other edible
                    }
                default: {
                        return 0;//inedible
                    }
            }
        }

        public static void EggStep(Client client) {
            //egg
            for (int i = 1; i <= client.Player.MaxInv; i++) {
                if (client.Player.Inventory[i].Num == 109 ||
                    client.Player.Inventory[i].Num == 110 ||
                    client.Player.Inventory[i].Num == 131) {
                    string[] eggArgs = client.Player.Inventory[i].Tag.Split(';');
                    if (eggArgs.Length <= 1 || !eggArgs[1].IsNumeric()) {
                        Messenger.PlaySound(client, "magic793.wav");
                        Messenger.PlayerMsg(client, "Bad EGG appeared!  The staff have been notified.  This egg will be removed the next time you log in.", Text.BrightRed);
                        Messenger.AdminMsg("[Staff] Bad EGG appeared from " + client.Player.Name + "!  The tag is: " + client.Player.Inventory[i].Tag, Text.BrightRed);
                        client.Player.Inventory[i].Num = 138;
                        break;
                    }
                    int step = eggArgs[1].ToInt();
                    if (step <= 1) {
                        IncubateEgg(client, i);
                        //Messenger.PlayerMsg(client, eggArgs[0], Text.Black);
                    } else {
                        client.Player.Inventory[i].Tag = eggArgs[0] + ";" + (step - 1).ToString();
                    }
                    break;
                }
            }
        }

        public static void IncubateEgg(Client client, int itemNum) {
            //works on the condition that the player has at least 1 slot open,
            //the map is a safe zone

            InventoryItem item = client.Player.Inventory[itemNum];
            string[] eggArgs = item.Tag.Split(';');
            //Messenger.PlayerMsg(client, eggArgs[1], Text.Black);
            if (item.Num == 110) {
                item.Num = 109;
                item.Tag = eggArgs[0] + ";60";
                Messenger.SendInventoryUpdate(client, itemNum);
                Messenger.PlaySoundToMap(client.Player.MapID, "magic852.wav");
            } else if (item.Num == 109) {
                item.Num = 131;
                item.Tag = eggArgs[0] + ";15";
                Messenger.SendInventoryUpdate(client, itemNum);
                Messenger.PlaySoundToMap(client.Player.MapID, "magic852.wav");
            } else if (item.Num == 131) {
                if (client.Player.FindOpenTeamSlot() > -1 &&
                    (client.Player.Map.Moral == Enums.MapMoral.Safe || client.Player.Map.Moral == Enums.MapMoral.House)) {
                    item.Tag = eggArgs[0] + ";0";
                    string[] hatchedArgs = eggArgs[0].Split(',');
                    int recruit = hatchedArgs[0].ToInt(-1);

                    if (recruit < 0) recruit = 1;

                    Story story = new Story();
                    StoryBuilderSegment segment = StoryBuilder.BuildStory();
                    StoryBuilder.AppendPlayerPadlockAction(segment, "Lock");
                    StoryBuilder.AppendSaySegment(segment, "Oh?", -1, 0, 0);
                    StoryBuilder.AppendSaySegment(segment, "The Mystery Egg is hatching!", -1, 0, 0);
                    StoryBuilder.AppendPlayMusicAction(segment, "PMD) Awakening.ogg", true, false);
                    StoryBuilder.AppendSaySegment(segment, "It's a healthy " + Pokedex.GetPokemon(recruit).Name + "!", recruit, 0, 0);
                    StoryBuilder.AppendAskQuestionAction(segment, "Will you accept the newly hatched " + Pokedex.GetPokemon(recruit).Name + " into your team?", 7, 9, recruit);
                    StoryBuilder.AppendRunScriptAction(segment, 47, "", "", "", true);
                    StoryBuilder.AppendGoToSegmentAction(segment, 10);
                    StoryBuilder.AppendSaySegment(segment, "The " + Pokedex.GetPokemon(recruit).Name + " left to live a carefree life without the team.", -1, 0, 0);
                    StoryBuilder.AppendRunScriptAction(segment, 49, "", "", "", true);
                    StoryBuilder.AppendPlayerPadlockAction(segment, "Unlock");

                    segment.AppendToStory(story);

                    Messenger.PlaySoundToMap(client.Player.MapID, "magic41.wav");
                    StoryManager.PlayStory(client, story);
                }
            }
        }


        public static List<int> GenerateTutorMoveset(int species, int form, int shardNum) {
            Server.Pokedex.PokemonForm pokemon = null;
            List<int> validEggMoves = new List<int>();
            if (species < 0) {
                return null;
            }
            pokemon = Server.Pokedex.Pokedex.GetPokemonForm(species, form);
            if (pokemon != null) {
                    for (int n = 0; n < pokemon.TutorMoves.Count; n++) {
                    	if (shardNum == 263 && MoveManager.Moves[pokemon.TutorMoves[n]].MoveCategory == Enums.MoveCategory.Physical
                    	|| shardNum == 264 && MoveManager.Moves[pokemon.TutorMoves[n]].MoveCategory == Enums.MoveCategory.Special
                    	|| shardNum == 265 && MoveManager.Moves[pokemon.TutorMoves[n]].MoveCategory == Enums.MoveCategory.Status
                    	|| shardNum == 266) {
                        	validEggMoves.Add(pokemon.TutorMoves[n]);
                        }
                    }
                
            }

            return validEggMoves;
        }


        public static List<int> GenerateEggMoveset(int species, int form, int length) {
            Server.Pokedex.PokemonForm pokemon = null;
            List<int> validEggMoves = new List<int>();
            if (species < 0) {
                return null;
            }
            pokemon = Server.Pokedex.Pokedex.GetPokemonForm(species, form);
            if (pokemon != null) {
                if (validEggMoves.Count == 0) {
                    for (int n = 0; n < pokemon.EggMoves.Count; n++) {
                        validEggMoves.Add(n);
                    }
                }
            }

            if (length < 0) length = pokemon.EggMoves.Count;

            List<int> returnedMoves = new List<int>();
            for (int i = 0; i < length; i++) {
                if (validEggMoves.Count > 0) {
                    int moveSelected = Server.Math.Rand(0, validEggMoves.Count);
                    int moveIndex = pokemon.EggMoves[validEggMoves[moveSelected]];
                    if (moveIndex > -1) {
                        returnedMoves.Add(moveIndex);
                    }
                    validEggMoves.RemoveAt(moveSelected);
                }
            }

            return returnedMoves;
        }

        public static bool ItemIllegal(int item) {
            switch (item) {
                case 815:
                    return true;
                case 138:
                    return true;
                case 446:
                	// Snowballs, we don't want them out of the game!
                	return true;
                default:
                    return false;
            }
        }
        
        public static void RestartServer() {
        	Server.Globals.GMOnly = true;
						
                            
                            System.Threading.Thread shutdownTimerThread = new System.Threading.Thread(delegate() {
                               		int waitingTime = 30;
	                            for (int i = waitingTime; i >= 1; i--) {
	                            	Server.Globals.ServerStatus = "Please prepare for a server restart. It will begin in " + i + " seconds.";
	                                System.Threading.Thread.Sleep(1000);
	                            }
	                            Messenger.AdminMsg("[Staff] Server restart in progress... Saving all players...", Text.BrightBlue);
	                            Server.Globals.ServerStatus = "Saving your data... Please wait...";
								
	                            using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
	                                try {
	                                    foreach (Client i in ClientManager.GetClients()) {
	                                        i.Player.SaveCharacterData(dbConnection);
	                                        i.Player.SavingLocked = true;
	                                        
	                                        Messenger.PlayerMsg(i, "You saved the game!", Text.BrightGreen);
	                                    }
	                                } catch (Exception ex) {
	                                    Messenger.AdminMsg(ex.ToString(), Text.BrightRed);
	                                }
	                            }
	                            Messenger.AdminMsg("Everyone has been saved!", Text.Yellow);
	
	                            PlayerManager.SavingEnabled = false;
                                
                                waitingTime = 30;
                               	for (int i = waitingTime; i >= 1; i--) {
 	                                Server.Globals.ServerStatus = "The server will be restarting in " + i + " seconds.";
    	                            System.Threading.Thread.Sleep(1000);
           	                    }
              	                Messenger.AdminMsg("!", Text.Black);
                            	System.Windows.Forms.Application.Restart();
                            	Environment.Exit(0);
               	 	          }
                           );
                           shutdownTimerThread.Start();
        }


    }
}

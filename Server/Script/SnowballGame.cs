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


using Server;
using Server.Scripting;
using Server.Database;
using System;
using System.Drawing;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
    using Server.Network;

namespace Script 
{
	public class SnowballGame
	{
		public const string WaitingRoomMap = "s1946";
		public const int MaxTeamPlayers = 50;
		
		public const int BLUEX1 = 16;
		public const int BLUEY1 = 12;
		public const int BLUEX2 = 26;
		public const int BLUEY2 = 23;
		
		public const int GREENX1 = 26;
		public const int GREENY1 = 12;
		public const int GREENX2 = 16;
		public const int GREENY2 = 23;
		
		public const int PLAYERLIVES = 5;
		public const int PointsLostPerRespawn = 50;
		public const int PointsPerHit = 10;
		public const int PointsPerKill = 20;
		
		public const int PointsToWin = 7000;
		
		public static bool IsGameOwner(Client client) {
			if (Main.ActiveSnowballGames.ContainsKey(client.Player.CharID)) {
				return true;
			} else {
				return false;
			}
		}
		
		public static string FindWaitingGame() {
			for (int i = 0; i < Main.ActiveSnowballGames.Count; i++) {
				Client client = ClientManager.FindClientFromCharID(Main.ActiveSnowballGames.Keys[i]);
				if (client != null && client.Player.MapID == WaitingRoomMap) {
					return client.Player.CharID;
				}
			}
			
			return null;
		}
		
		public static SnowballGame GetWaitingGame() {
			for (int i = 0; i < Main.ActiveSnowballGames.Count; i++) {
				Client client = ClientManager.FindClientFromCharID(Main.ActiveSnowballGames.Keys[i]);
				if (client != null && client.Player.MapID == WaitingRoomMap) {
					return Main.ActiveSnowballGames.Values[i];
				}
			}
			
			return null;
		}
		
		public enum Teams { Green, Blue };
		
		public List<Client> BlueTeam;
		public List<Client> GreenTeam;
		
		public IMap ArenaMap;
		public Client GameLeader;
		int PlayerCount;
		
		private Teams LastAdded;
		
		public int GreenPoints;
		public int BluePoints;
		
		public void Init() {
			BlueTeam = new List<Client>();
			GreenTeam = new List<Client>();
			
			PlayerCount = 0;
			
			GreenPoints = 0;
			BluePoints = 0;
			
			Random rand = new Random();
			LastAdded = (Teams)rand.Next(0, 2);
		}
		
		public void CreateGame(Client leader) {
			GameLeader = leader;
			AddToGame(leader, LastAdded);
		}
		
		public void AddToGame(Client client) {
			if (exPlayer.Get(client).InSnowballGame) {
				Messenger.PlayerMsg(client, "You have already joined the game!", Text.BrightRed);
				return;
			}
			if (PlayerCount < (MaxTeamPlayers * 2)) {
				switch (LastAdded) {
					case Teams.Blue: {
							if (GreenTeam.Count < MaxTeamPlayers) {
								// There is still room for more players
								AddToGame(client, Teams.Green);
							} else {
								// No room for more players in the red team, try to add to the blue team
								if (BlueTeam.Count < MaxTeamPlayers) {
									AddToGame(client, Teams.Blue);
								}
							}
						}
						break;
					case Teams.Green: {
							if (BlueTeam.Count < MaxTeamPlayers) {
								// There is still room for more players
								AddToGame(client, Teams.Blue);
							} else {
								// No room for more players in the blue team, try to add to the green team
								if (GreenTeam.Count < MaxTeamPlayers) {
									AddToGame(client, Teams.Green);
								}
							}
						}
						break;
				}
			} else {
				Messenger.PlayerMsg(client, "The game is full!", Text.BrightRed);
			}
		}
		
		public void AddToGame(Client client, Teams team) {
			LastAdded = team;
			exPlayer.Get(client).InSnowballGame = true;
			exPlayer.Get(client).SnowballGameSide = team;
			exPlayer.Get(client).SnowballGameInstance = this;
			exPlayer.Get(client).SnowballGameLives = PLAYERLIVES;
			if (team == Teams.Green) {
				GreenTeam.Add(client);
			} else if (team == Teams.Blue) {
				BlueTeam.Add(client);
			}
			
			PlayerCount++;
			if (client != GameLeader) {
				Messenger.PlayerMsg(client, "You have joined the game! Wait until the leader starts the game!", Text.Yellow);
				Messenger.PlayerMsg(GameLeader, client.Player.Name + " has been added to the game!", Text.Yellow);
			}
		}
		
		public void StartGame(Client client) {
			if (client == GameLeader) {
				
				InstancedMap map = new InstancedMap(MapManager.GenerateMapID("i"));
                IMap originalMap = MapManager.RetrieveMap(1945);
                map.MapBase = 1945;
                map.Moral = Enums.MapMoral.Safe;
                MapCloner.CloneMapTileProperties(originalMap, map);
                MapCloner.CloneMapTiles(originalMap, map);
                
                ArenaMap = map;
                
                bool swapSides = false;
                foreach (Client playerClient in BlueTeam) {
	                Messenger.PlayerMsg(playerClient, "You are on the Blue Team", Text.Yellow);
	                int x;
	                int y;
	                	
	                if (swapSides) {
		                x = BLUEX2;
		                y = BLUEY2;
	                } else {
		                x = BLUEX1;
		                y = BLUEY1;
	                }
	                swapSides = !swapSides;
	                
	                RemoveSnowballs(playerClient);
	                playerClient.Player.GiveItem(446, 999);
	                Messenger.PlayerMsg(playerClient, "You were given 999 snowballs! Use them well!", Text.BrightBlue);
					
					playerClient.Player.Status = PLAYERLIVES + " Lives";;
					Messenger.SendPlayerData(playerClient);
					
					Messenger.PlayerWarp(playerClient, map, x, y);
                }
                 
                swapSides = false;
                foreach (Client playerClient in GreenTeam) {
	                Messenger.PlayerMsg(playerClient, "You are on the Green Team", Text.Yellow);
	                int x;
	                int y;
	                
	                if (swapSides) {
		                x = GREENX2;
		                y = GREENY2;
	                } else {
	                	x = GREENX1;
	                	y = GREENY1;
	                }
	                swapSides = !swapSides;
	                
	                RemoveSnowballs(playerClient);
	                playerClient.Player.GiveItem(446, 999);
	                Messenger.PlayerMsg(playerClient, "You were given 999 snowballs! Use them well!", Text.BrightGreen);
					
					playerClient.Player.Status = PLAYERLIVES + " Lives";;
					Messenger.SendPlayerData(playerClient);
							
					Messenger.PlayerWarp(playerClient, map, x, y);
                }
			}	
		}
		
		public void HandleHit(Client attacker, Client defender) {
			
			if (exPlayer.Get(defender).SnowballGameSide == Teams.Green) {
				if ((defender.Player.X == GREENX1 && defender.Player.Y == GREENY1) ||
					(defender.Player.X == GREENX2 && defender.Player.Y == GREENY2)) {
					// Safe zone
					return;
				}
			} else if (exPlayer.Get(defender).SnowballGameSide == Teams.Blue) {
				if ((defender.Player.X == BLUEX1 && defender.Player.Y == BLUEY1) ||
					(defender.Player.X == BLUEX2 && defender.Player.Y == BLUEY2)) {
					// Safe zone
					return;
				}
			}
			
			if (exPlayer.Get(attacker).SnowballGameSide == Teams.Green) {
				if ((attacker.Player.X == GREENX1 && attacker.Player.Y == GREENY1) ||
					(attacker.Player.X == GREENX2 && attacker.Player.Y == GREENY2)) {
					// Safe zone
					return;
				}
			} else if (exPlayer.Get(attacker).SnowballGameSide == Teams.Blue) {
				if ((attacker.Player.X == BLUEX1 && attacker.Player.Y == BLUEY1) ||
					(attacker.Player.X == BLUEX2 && attacker.Player.Y == BLUEY2)) {
					// Safe zone
					return;
				}
			}
			
			if (exPlayer.Get(attacker).SnowballGameSide == exPlayer.Get(defender).SnowballGameSide) {
				// No friendly fire
				return;
			}
			
			bool defenderKilled = false;
		
			exPlayer.Get(defender).SnowballGameLives--;
			if (exPlayer.Get(defender).SnowballGameLives == 0) {
				defenderKilled = true;
				exPlayer.Get(defender).SnowballGameLives = PLAYERLIVES;
				if (exPlayer.Get(defender).SnowballGameSide == Teams.Green) {
					GreenPoints -= PointsLostPerRespawn;
					if (GreenPoints < 0) {
						GreenPoints = 0;
					}
					Messenger.MapMsg(attacker.Player.MapID, defender.Player.Name + " fainted and the Green team lost " + PointsLostPerRespawn + " points!", Text.BrightGreen);
				} else {
					BluePoints -= PointsLostPerRespawn;
					if (BluePoints < 0) {
						BluePoints = 0;
					}
					Messenger.MapMsg(attacker.Player.MapID, defender.Player.Name + " fainted and the Blue team lost " + PointsLostPerRespawn + " points!", Text.BrightBlue);
				}
			}
			
			defender.Player.Status = exPlayer.Get(defender).SnowballGameLives + " Lives";
			Messenger.SendPlayerData(defender);
			
			// Add points to the attacking team
			if (exPlayer.Get(attacker).SnowballGameSide == Teams.Green) {
				if (defenderKilled) {
					GreenPoints += PointsPerKill;
				} else {
					GreenPoints += PointsPerHit;
				}
				
				if (GreenPoints % 100 == 0) {
					Messenger.MapMsg(attacker.Player.MapID, "The Green team is at " + GreenPoints + " points!", Text.BrightGreen);
				}
			} else {
				if (defenderKilled) {
					BluePoints += PointsPerKill;
				} else {
					BluePoints += PointsPerHit;
				}
				
				if (BluePoints % 100 == 0) {
					Messenger.MapMsg(attacker.Player.MapID, "The Blue team is at " + BluePoints + " points!", Text.BrightBlue);
				}
			}
			
			CheckPoints();
		}
		
		public void CheckPoints() {
			if (GreenPoints >= PointsToWin) {
				Messenger.MapMsg(ArenaMap.MapID, "The Green team wins!", Text.BrightGreen);
				
				EndGame();
			} else if (BluePoints >= PointsToWin) {
				Messenger.MapMsg(ArenaMap.MapID, "The Blue team wins!", Text.BrightBlue);
				
				EndGame();
			}
		}
		
		public void EndGame() {
			foreach (Client playerClient in GreenTeam) {
				EndGame(playerClient);	
			}
			
			foreach (Client playerClient in BlueTeam) {
				EndGame(playerClient);	
			}
			
			Main.ActiveSnowballGames.RemoveAtKey(GameLeader.Player.CharID);
		}
		
		public void EndGame(Client client) {
			exPlayer.Get(client).UnloadSnowballGame();
			
			client.Player.Status = "";
			Messenger.SendPlayerData(client);
			
			Messenger.PlayerWarp(client, WaitingRoomMap, 6, 3);
			
			RemoveSnowballs(client);
		}
		
		public void RemoveSnowballs(Client client) {
			for (int i = 1; i <= client.Player.MaxInv; i++) {
	            if (client.Player.Inventory[i].Num == 446) {
		            client.Player.TakeItem(client.Player.Inventory[i].Num, client.Player.Inventory[i].Amount);
	            }
            }
		}
		
		public void RemoveFromGame(Client client) {
			if (exPlayer.Get(client).SnowballGameSide == Teams.Green) {
				GreenTeam.Remove(client);
				EndGame(client);
			} else if (exPlayer.Get(client).SnowballGameSide == Teams.Blue) {
				BlueTeam.Remove(client);
				EndGame(client);
			}
			Messenger.PlayerMsg(client, "You have left the game.", Text.BrightGreen);
			if (client == GameLeader) {
				HandleLeaderLeft();
			}
		}
		
		public void HandleLeaderLeft() {
			if (ArenaMap != null) {
				Messenger.MapMsg(ArenaMap.MapID, "The leader left the game, so the game has ended.", Text.BrightRed);
			}
			EndGame();
		}
	}
}
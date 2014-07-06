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
using Server.Maps;
using Server.Players;
using Server.Network;
using System;
using System.Drawing;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Script 
{
	public class CTF
	{
		public const int MAXTEAMCOUNT = 30;
		public const int MINTEAMCOUNT = 2;
		
		public const int REDMAP = 791;
		public const int BLUEMAP = 792;
		public const int HUBMAP = 793;
		
		public const int REDX = 23;
		public const int REDY = 7;
		public const int BLUEX = 15;
		public const int BLUEY = 40;
		
		public enum PlayerState { Free, Jailed, HoldingFlag };
		public enum Teams { Red, Blue };
		public enum CTFGameState { NotStarted, WaitingForPlayers, Started };
		
		// The actual game variables
		public Client[] RedTeam; // Members in each team
		public Client[] BlueTeam;
		public int RedTeamPlayers; // Player count
		public int BlueTeamPlayers;
		
		public int RedScore;
		public int BlueScore;
		public int RedFlags;
		public int BlueFlags;
		
		public CTFGameState GameState;
		public int PlayerCount;
		public Client GameLeader;
		
		public Client RedFlagHolder;
		public Client BlueFlagHolder;
		
		private Teams LastAdded;
		
		public CTF(CTFGameState newGameState) {
			Init(newGameState);
		}
		
		public void Init(CTFGameState newGameState) {
			RedTeam = new Client[MAXTEAMCOUNT];
			BlueTeam = new Client[MAXTEAMCOUNT];
			
			for (int i = 0; i < MAXTEAMCOUNT; i++) {
				RedTeam[i] = null;
				BlueTeam[i] = null;
			}
			RedTeamPlayers = 0;
			BlueTeamPlayers = 0;
			GameState = newGameState;
			RedScore = 0;
			BlueScore = 0;
			RedFlags = 5;
			BlueFlags = 5;
			PlayerCount = 0;
			GameLeader = null;
			RedFlagHolder = null;
			BlueFlagHolder = null;
			LastAdded = Teams.Red;
		}
		
		public void CreateGame(Client client) {
			GameLeader = client;
			GameState = CTFGameState.WaitingForPlayers;
			AddToGame(client);
			Messenger.PlayerMsg(client, "You have created a new game of Capture The Flag! Say /ctfstart to start!", Text.Yellow);
			Messenger.MapMsg(MapManager.GenerateMapID(HUBMAP), client.Player.Name + " has created a new game of Capture The Flag!", Text.Yellow);
		}
		
		public void AddToGame(Client client) {
			if (PlayerCount < (MAXTEAMCOUNT * 2)) {
				switch (LastAdded) {
					case Teams.Red: {
							int Slot = FindOpenTeamSlot(Teams.Blue);
							if (Slot != -1) {
								AddToGame(client, Teams.Blue, Slot);
							} else {
								Slot = FindOpenTeamSlot(Teams.Red);
								if (Slot != -1) {
									AddToGame(client, Teams.Red, Slot);
								} else {
									Messenger.PlayerMsg(client, "Unable to add you to the game.", Text.BrightRed);
								}
							}
						}
						break;
					case Teams.Blue: {
							int Slot = FindOpenTeamSlot(Teams.Red);
							if (Slot != -1) {
								AddToGame(client, Teams.Red, Slot);
							} else {
								Slot = FindOpenTeamSlot(Teams.Blue);
								if (Slot != -1) {
									AddToGame(client, Teams.Blue, Slot);
								} else {
									Messenger.PlayerMsg(client, "Unable to add you to the game.", Text.BrightRed);
								}
							}
						}
						break;
				}
			}
		}
		
		public int FindOpenTeamSlot(Teams team) {
			switch (team) {
				case Teams.Red: {
						for (int i = 0; i < RedTeam.Length; i++) {
							if (RedTeam[i] == null) {
								return i;
							}
						}
					}
					break;
				case Teams.Blue: {
						for (int i = 0; i < BlueTeam.Length; i++) {
							if (BlueTeam[i] == null) {
								return i;
							}
						}
					}
					break;
			}
			return -1;
		}
		
		public void AddToGame(Client client, Teams team, int slot) {
			LastAdded = team;
			exPlayer.Get(client).InCTF = true;
			exPlayer.Get(client).CTFSide = team;
			exPlayer.Get(client).CTFState = PlayerState.Free;
			if (team == Teams.Red) {
				RedTeam[slot] = client;
				RedTeamPlayers++;
				if (GameState == CTFGameState.Started) {
					Messenger.PlayerWarp(client, REDMAP, REDX, REDY);
				}
			} else if (team == Teams.Blue) {
				BlueTeam[slot] = client;
				BlueTeamPlayers++;
				if (GameState == CTFGameState.Started) {
					Messenger.PlayerWarp(client, BLUEMAP, BLUEX, BLUEY);
				}
			}
			PlayerCount++;
			if (client != GameLeader) {
				Messenger.PlayerMsg(client, "You have joined the game! Wait until the leader starts the game!", Text.Yellow);
				Messenger.PlayerMsg(GameLeader, client.Player.Name + " has been added to the game!", Text.Yellow);
			}
		}
		
		public void RemoveFromGame(Client client) {
			if (exPlayer.Get(client).InCTF) {
				if (exPlayer.Get(client).CTFSide == Teams.Red) {
					for (int i = 0; i < RedTeam.Length; i++) {
						if (RedTeam[i] == client) {
							RedTeam[i] = null;
						}
					}
				} else if (exPlayer.Get(client).CTFSide == Teams.Blue) {
					for (int i = 0; i < BlueTeam.Length; i++) {
						if (BlueTeam[i] == client) {
							BlueTeam[i] = null;
						}
					}
				}
				exPlayer.Get(client).InCTF = false;
				Messenger.PlayerWarp(client, HUBMAP, 8, 8);
				Messenger.PlayerMsg(client, "You have left the game!", Text.Yellow);
				CTFMsg(client.Player.Name + " has left the game!", Text.Yellow);
				if (client == GameLeader) {
					EndGame(client);
				}
			}
		}
		
		public void StartGame() {
			if (RedTeamPlayers >= MINTEAMCOUNT && BlueTeamPlayers >= MINTEAMCOUNT) {
				for (int i = 0; i < MAXTEAMCOUNT; i++) {
					try {
						if (BlueTeam[i] != null) {
							Messenger.PlayerMsg(BlueTeam[i], "You are on the Blue Team", Text.Yellow);
							Messenger.PlayerWarp(BlueTeam[i], BLUEMAP, BLUEX, BLUEY);
							BlueTeam[i].Player.Status = "Blue";
							Messenger.SendPlayerData(BlueTeam[i]);
						}
						if (RedTeam[i] != null) {
							Messenger.PlayerMsg(RedTeam[i], "You are on the Red Team", Text.Yellow);
							Messenger.PlayerWarp(RedTeam[i], REDMAP, REDX, REDY);
							RedTeam[i].Player.Status = "Red";
							Messenger.SendPlayerData(RedTeam[i]);
						}
					} catch (Exception ex) {
					}
				}
				GameState = CTFGameState.Started;
			} else {
				int needed = (MINTEAMCOUNT * 2) - (RedTeamPlayers + BlueTeamPlayers);
				if (needed == 1) {
					Messenger.PlayerMsg(GameLeader, "There are not enough players on each team! 1 more player needs to join before the game can start!", Text.BrightRed);
				} else {
					Messenger.PlayerMsg(GameLeader, "There are not enough players on each team! " + needed.ToString() + " more players need to join before the game can start!", Text.BrightRed);
				}
			}
		}
		
		public void EndGame(Client client) {
			if (client == GameLeader) {
				exPlayer.Get(GameLeader).UnloadCTF();
				GameLeader.Player.Status = "";
				Messenger.SendPlayerData(GameLeader);
				Messenger.PlayerWarp(GameLeader, HUBMAP, 8, 8);
				for (int i = 0; i < MAXTEAMCOUNT; i++) {
					if (BlueTeam[i] != null && BlueTeam[i] != GameLeader && exPlayer.Get(BlueTeam[i]).InCTF) {
						exPlayer.Get(BlueTeam[i]).UnloadCTF();
						Messenger.PlayerWarp(BlueTeam[i], HUBMAP, 8, 8);
						Messenger.PlayerMsg(BlueTeam[i], "The leader has ended the game.", Text.Yellow);
						BlueTeam[i].Player.Status = "";
						Messenger.SendPlayerData(BlueTeam[i]);
					}
					if (RedTeam[i] != null && RedTeam[i] != GameLeader && exPlayer.Get(RedTeam[i]).InCTF) {
						exPlayer.Get(RedTeam[i]).UnloadCTF();
						Messenger.PlayerWarp(RedTeam[i], HUBMAP, 8, 8);
						Messenger.PlayerMsg(RedTeam[i], "The leader has ended the game.", Text.Yellow);
						RedTeam[i].Player.Status = "";
						Messenger.SendPlayerData(RedTeam[i]);
					}
				}
				Init(CTFGameState.NotStarted);
			}
		}
		
		public void EndGame(string Ender) {
			exPlayer.Get(GameLeader).UnloadCTF();
			GameLeader.Player.Status = "";
			Messenger.SendPlayerData(GameLeader);
			Messenger.PlayerWarp(GameLeader, HUBMAP, 8, 8);
			for (int i = 0; i < MAXTEAMCOUNT; i++) {
				if (BlueTeam[i] != null && BlueTeam[i] != GameLeader && exPlayer.Get(BlueTeam[i]).InCTF) {
					exPlayer.Get(BlueTeam[i]).UnloadCTF();
					Messenger.PlayerWarp(BlueTeam[i], HUBMAP, 8, 8);
					Messenger.PlayerMsg(BlueTeam[i], Ender + " has ended the game.", Text.Yellow);
					BlueTeam[i].Player.Status = "";
					Messenger.SendPlayerData(BlueTeam[i]);
				}
				if (RedTeam[i] != null && RedTeam[i] != GameLeader && exPlayer.Get(RedTeam[i]).InCTF) {
					exPlayer.Get(RedTeam[i]).UnloadCTF();
					Messenger.PlayerWarp(RedTeam[i], HUBMAP, 8, 8);
					Messenger.PlayerMsg(RedTeam[i], Ender + " has ended the game.", Text.Yellow);
					RedTeam[i].Player.Status = "";
					Messenger.SendPlayerData(RedTeam[i]);
				}
			}
			Init(CTFGameState.NotStarted);
		}
		
		public void EndGame() {
			exPlayer.Get(GameLeader).UnloadCTF();
			GameLeader.Player.Status = "";
			Messenger.SendPlayerData(GameLeader);
			Messenger.PlayerWarp(GameLeader, HUBMAP, 8, 8);
			for (int i = 0; i < MAXTEAMCOUNT; i++) {
				if (BlueTeam[i] != null && BlueTeam[i] != GameLeader && exPlayer.Get(BlueTeam[i]).InCTF) {
					exPlayer.Get(BlueTeam[i]).UnloadCTF();
					Messenger.PlayerWarp(BlueTeam[i], HUBMAP, 8, 8);
					Messenger.PlayerMsg(BlueTeam[i], "The game has ended.", Text.Yellow);
					BlueTeam[i].Player.Status = "";
					Messenger.SendPlayerData(BlueTeam[i]);
				}
				if (RedTeam[i] != null && RedTeam[i] != GameLeader && exPlayer.Get(RedTeam[i]).InCTF) {
					exPlayer.Get(RedTeam[i]).UnloadCTF();
					Messenger.PlayerWarp(RedTeam[i], HUBMAP, 8, 8);
					Messenger.PlayerMsg(RedTeam[i], "The game has ended.", Text.Yellow);
					RedTeam[i].Player.Status = "";
					Messenger.SendPlayerData(RedTeam[i]);
				}
			}
			Init(CTFGameState.NotStarted);
		}
		
		public void CheckFlag(Client client, Teams team) {
			if (exPlayer.Get(client).CTFState == PlayerState.HoldingFlag) {
				if (team != exPlayer.Get(client).CTFSide) {
					switch (exPlayer.Get(client).CTFSide) {
						case Teams.Red: {
								exPlayer.Get(client).CTFState = PlayerState.Free;
								BlueFlagHolder = null;
								CTFMsg(client.Player.Name + " has captured a blue flag! The blue team has " + BlueFlags.ToString() + " remaining!", Text.Yellow);
								if (BlueFlags == 0) {
									CTFMsg("The red team wins!", Text.Yellow);
									EndGame();
								}
							}
							break;
						case Teams.Blue: {
							exPlayer.Get(client).CTFState = PlayerState.Free;
							RedFlagHolder = null;
							CTFMsg(client.Player.Name + " has captured a red flag! The red team has " + RedFlags.ToString() + " remaining!", Text.Yellow);
							if (RedFlags == 0) {
								CTFMsg("The blue team wins!", Text.Yellow);
								EndGame();
							}
						}
						break;
					}
				}
			}
		}
		
		public void CTFMsg(string msg, Color color) {
			IMap redMap = MapManager.RetrieveActiveMap(MapManager.GenerateMapID(REDMAP));
			IMap blueMap = MapManager.RetrieveActiveMap(MapManager.GenerateMapID(BLUEMAP));
			if (redMap != null && blueMap != null) {
				foreach (Client client in redMap.GetClients()) {
					Messenger.PlayerMsg(client, msg, color);
				}
				foreach (Client client in blueMap.GetClients()) {
					Messenger.PlayerMsg(client, msg, color);
				}
			}
		}
		
		public void CTFTMsg(Client mainClient, string msg, Color color) {
			IMap redMap = MapManager.RetrieveActiveMap(MapManager.GenerateMapID(REDMAP));
			IMap blueMap = MapManager.RetrieveActiveMap(MapManager.GenerateMapID(BLUEMAP));
			if (redMap != null && blueMap != null) {
				CTF.Teams clientSide = exPlayer.Get(mainClient).CTFSide;
				foreach (Client client in redMap.GetClients()) {
					if (exPlayer.Get(client).CTFSide == clientSide) {
						Messenger.PlayerMsg(client, msg, color);
					}
				}
				foreach (Client client in blueMap.GetClients()) {
					if (exPlayer.Get(client).CTFSide == clientSide) {
						Messenger.PlayerMsg(client, msg, color);
					}
				}
			}
		}
		
		public Client GetTeamMember(string charID, Teams team) {
			if (team == Teams.Blue) {
				for (int i = 0; i < BlueTeam.Length; i++) {
					if (BlueTeam[i] != null && BlueTeam[i].IsPlaying() && BlueTeam[i].Player.CharID == charID) {
						return BlueTeam[i];
					}
				}
			} else if (team == Teams.Red) {
				for (int i = 0; i < RedTeam.Length; i++) {
					if (RedTeam[i] != null && RedTeam[i].IsPlaying() && RedTeam[i].Player.CharID == charID) {
						return RedTeam[i];
					}
				}
			}
			return null;
		}
		
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server.Scripting;
using Server.IO;
using Server.Players;
using System.Xml;
using Server;
using Server.Network;
using Server.Maps;

namespace Script {
    public class PitchBlackAbyss {
    
   		public enum Difficulty {Easy, Normal};
   		
   		public enum Room {One, Two, Three, Four, Heal, Maze, Random, Start, End, Error};
   		
        public static void ChangeDungeonCoords(Client client, Enums.Direction dir) {
        	int dist = 1;
        	int rand = Server.Math.Rand(1, 10000);
        	if (rand <= 66) { //.66% chance warp
        		exPlayer.Get(client).DungeonX = Server.Math.Rand(0, exPlayer.Get(client).DungeonMaxX-1);
        		exPlayer.Get(client).DungeonY = Server.Math.Rand(0, exPlayer.Get(client).DungeonMaxY-1);
        		return;
        	} else if (rand <= 666) { //6% chance skip
        		dist = 2;
        	}
        	
            if (dir == Enums.Direction.Up) {
                exPlayer.Get(client).DungeonY = exPlayer.Get(client).DungeonY - dist;
                if (exPlayer.Get(client).DungeonY < 0) {
                	exPlayer.Get(client).DungeonY += exPlayer.Get(client).DungeonMaxY;
                }
            } else if (dir == Enums.Direction.Down) {
                exPlayer.Get(client).DungeonY = (exPlayer.Get(client).DungeonY + dist) % exPlayer.Get(client).DungeonMaxY;
            } else if (dir == Enums.Direction.Left) {
                exPlayer.Get(client).DungeonX = exPlayer.Get(client).DungeonX - dist;
                if (exPlayer.Get(client).DungeonX < 0) {
                	exPlayer.Get(client).DungeonX += exPlayer.Get(client).DungeonMaxX;
                }
            } else if (dir == Enums.Direction.Right) {
                exPlayer.Get(client).DungeonX = (exPlayer.Get(client).DungeonX + dist) % exPlayer.Get(client).DungeonMaxX;
            }
        }
		
		//change to use Enums.Direction
        public static void GetDungeonRoom(Client client, int dir) { //gets the room type from a table
            //dir: 1=up,2=down,3=left,4=right
            
            Enums.Direction direc;
            if (dir == 1) {
            	direc = Enums.Direction.Up;
            } else if (dir == 2) {
            	direc = Enums.Direction.Down;
            } else if (dir == 3) {
            	direc = Enums.Direction.Left;
            } else { //dir = 4
            	direc = Enums.Direction.Right;
            }
            ChangeDungeonCoords(client, direc);
            
            int x = 0, y = 0;
            switch (dir) {
                case 1: {
                        x = client.Player.X;
                        if (x > 19) x = x - 14; //adjust for maze maps
                        y = 12;
                    }
                    break;
                case 2: {
                        x = client.Player.X;
                        if (x > 19) x = x - 14;
                        y = 2;
                    }
                    break;
                case 3: {
                        x = 17;
                        y = client.Player.Y;
                        if (y > 14) y = y - 10;
                    }
                    break;
                case 4: {
                        x = 2;
                        y = client.Player.Y;
                        if (y > 14) y = y - 10;
                    }
                    break;
            }
            
            if (client.Player.IsInParty()) {
        		Server.Players.Parties.Party party = Server.Players.Parties.PartyManager.FindPlayerParty(client);
        		foreach (Client member in party.GetOnlineMemberClients() ) {
        			//check if they're in the same room
        			if (client != member && 
        				exPlayer.Get(client).DungeonX == exPlayer.Get(member).DungeonX &&
        				exPlayer.Get(client).DungeonY == exPlayer.Get(member).DungeonY)
        			{
        				Messenger.PlayerWarp(client, member.Player.Map, x, y);
        				return;
        			}
        		}
        	}
        		
            
            Room rtype = GetRoomType(client);
            int roomType;
            if (rtype == Room.One)
            	roomType = 1;
            else if (rtype == Room.Two)
            	roomType = 2;
            else if (rtype == Room.Three)
            	roomType = 3;
            else if (rtype == Room.Four || rtype == Room.Heal)
            	roomType = 4;
            else if (rtype == Room.Random)
            	roomType = 5;
            else if (rtype == Room.Maze)
            	roomType = 7;
            else if (rtype == Room.End)
            	roomType = 6;
            else // rtype == Room.Error
            	return;
            int smallType = roomType % 10;
            //explanation of roomTypes: normally # of openings
            //5 = random, 0 = preset/special
            //1, 2, 3, 4, 5 = not against an edge
            //6 = preset, not randomized
            //+10 is locked
            //+20 is against right edge
            //+40 is against left edge
            //+80 is against bottom edge
            //+160 is against top edge
            //modifiers stack, so +210 = top-right corner, locked
            int DungeonMaxX = exPlayer.Get(client).DungeonMaxX;
            int DungeonMaxY = exPlayer.Get(client).DungeonMaxY;
            int mapNum = 1;
            int rand;

            int[] one = new int[4] { 1549, 1547, 1548, 1550 };
            int[] two = new int[6] { 1553, 1551, 1552, 1554, 1555, 1556 };
            int[] three = new int[4] { 1557, 1560, 1558, 1559 };
            int four = 1561;
			
            switch (smallType) {
                case 1: { //
                        mapNum = one[dir - 1];
                    } break;

                case 2: { //
                        rand = Server.Math.Rand(1, 4);
                        switch (dir) {
                            case 1: {
                                    mapNum = two[rand * 2 - 2];
                                }
                                break;
                            case 2: {
                                    if (rand == 1) {
                                        mapNum = two[rand];
                                    } else {
                                        mapNum = two[rand + 1];
                                    }
                                }
                                break;
                            case 3: {
                                    if (rand == 3) {
                                        mapNum = two[rand + 2];
                                    } else {
                                        mapNum = two[rand];
                                    }
                                }
                                break;
                            case 4: {
                                    if (rand == 1) {
                                        mapNum = two[rand - 1];
                                    } else {
                                        mapNum = two[rand * 2 - 1];
                                    }
                                }
                                break;
                         }
                    }
                    break;

                case 3: {
                        rand = Server.Math.Rand(1, 4);
                        mapNum = three[(dir + rand) % 4];
                    }
                    break;
                 case 4: { //
                        mapNum = four;
                    }
                    break;
                case 5: { //random
                        rand = Server.Math.Rand(1, 11);
                        switch (rand) {
                            case 1:
                            case 9:
                            case 10: mapNum = one[dir - 1];
                                break;
                            case 2:
                            case 3:
                            case 4: {
                                    rand = rand - 1;
                                    switch (dir) {
                                        case 1: {
                                                mapNum = two[rand * 2 - 2];
                                            }
                                            break;
                                        case 2: {
                                                if (rand == 1) {
                                                    mapNum = two[rand];
                                                } else {
                                                    mapNum = two[rand + 1];
                                                }
                                            }
                                            break;
                                        case 3: {
                                                if (rand == 3) {
                                                    mapNum = two[rand + 2];
                                                } else {
                                                    mapNum = two[rand];
                                                }
                                            }
                                            break;
                                        case 4: {
                                                if (rand == 1) {
                                                    mapNum = two[rand - 1];
                                                } else {
                                                    mapNum = two[rand * 2 - 1];
                                                }
                                            }
                                            break;
                                     }
                                }
                                break;
                            case 5:
                            case 6:
                            case 7: {
                                    rand = rand - 4;
                                    mapNum = three[(dir + rand) % 4];
                                }
                                break;
                            case 8: mapNum = four;
                                break;
                        }
                    }
                    break;
                case 6: {
                        mapNum = 1562;
                    }
                    break;
               	case 7: {
               		    mapNum = 1543; //maze map
                	}
                	break;
            }

            InstancedMap imap = new InstancedMap(MapManager.GenerateMapID("i"));
            
            if (mapNum == 1543) {
            	GenerateMaze(client, direc);
            	return;
            }
            MapCloner.CloneMapTiles(MapManager.RetrieveMap(mapNum), imap);
            //if (mapNum == 1562 /*&& exPlayer.Get(client).DungeonID == 20*/) {
                //replace the boss warps
                //imap.SetAttribute(9, 3, Enums.TileType.Scripted, 41, 0, 0, "Xatu", "", "");
                //imap.SetAttribute(10, 3, Enums.TileType.Scripted, 41, 0, 0, "Xatu", "", "");
            //}
			
            if (smallType != 6) {
            	DecorateMap(client, imap);
            	//SpawnNpcs(client, imap);
            	//SpawnItems(client, imap);
            }
            Messenger.PlayerWarp(client, imap, x, y);
            if (smallType != 6) {
            	SpawnItems(client, imap);
            	SpawnNpcsBad(client, imap);
            	UnlockRoom(client, imap);  //will only unlock if no enemies
            }
		}
		
        public static void DecorateMap(Client client, IMap imap) {
            int rand, rand2;
            int x = exPlayer.Get(client).DungeonX;
            int y = exPlayer.Get(client).DungeonY;
            int id = exPlayer.Get(client).DungeonID;
            int randx, randy;
            
            if (Server.Math.Rand(0,20) == 0) { //lock
            	if (imap.MaxX == 19) {
            		for (int i = 8; i <= 11; i++) {
            			if (imap.Tile[i,1].Type == Enums.TileType.Walkable) {
            				imap.SetTile(i, 1, 6, 4, 1);
            				imap.SetAttribute(i, 1, Enums.TileType.Key, 559, 1, 0, "", "", "");
            			}
            			if (imap.Tile[i,13].Type == Enums.TileType.Walkable) {
            				imap.SetTile(i, 13, 6, 4, 1);
            				imap.SetAttribute(i, 13, Enums.TileType.Key, 559, 1, 0, "", "", "");
            			}
            		}
            		for (int i = 6; i <= 8; i++) {
            			if (imap.Tile[1,i].Type == Enums.TileType.Walkable) {
            				imap.SetTile(1,i, 6, 4, 1);
            				imap.SetAttribute(1,i, Enums.TileType.Key, 559, 1, 0, "", "", "");
            			}
            			if (imap.Tile[18,i].Type == Enums.TileType.Walkable) {
            				imap.SetTile(18,i, 6, 4, 1);
            				imap.SetAttribute(18,i, Enums.TileType.Key, 559, 1, 0, "", "", "");
            			}
            		}
            	} else if (imap.MaxX == 46) {
            		for (int i = 22; i <= 24; i++) {
            			if (imap.Tile[i,1].Type == Enums.TileType.Walkable) {
            				imap.SetTile(i, 1, 6, 4, 1);
            				imap.SetAttribute(i, 1, Enums.TileType.Key, 559, 1, 0, "", "", "");
            			}
            			if (imap.Tile[i,33].Type == Enums.TileType.Walkable) {
            				imap.SetTile(i, 33, 6, 4, 1);
            				imap.SetAttribute(i, 33, Enums.TileType.Key, 559, 1, 0, "", "", "");
            			}
            		}
            		for (int i = 16; i <= 18; i++) {
            			if (imap.Tile[1,i].Type == Enums.TileType.Walkable) {
            				imap.SetTile(1,i, 6, 4, 1);
            				imap.SetAttribute(1,i, Enums.TileType.Key, 559, 1, 0, "", "", "");
            			}
            			if (imap.Tile[45,i].Type == Enums.TileType.Walkable) {
            				imap.SetTile(45,i, 6, 4, 1);
            				imap.SetAttribute(45,i, Enums.TileType.Key, 559, 1, 0, "", "", "");
            			}
            		}
            	}
            }
            
            imap.OriginalDarkness = Server.Math.Rand(0,19);
            if (imap.MaxX > 20) imap.OriginalDarkness += 6;
            imap.Indoors = true;
            imap.Music = "PMD2) Dark Crater.ogg";
            
            //ground decorations
            if ((x != 1 && y != 1) && (x != 10) && (y != 10)){
            	rand = Server.Math.Rand(28, 42);
            	for (int i = 1; i <= rand; i++) {
            		randx = Server.Math.Rand(0, imap.MaxX+1);
            		randy = Server.Math.Rand(0, imap.MaxY+1);
            		rand2 = Server.Math.Rand(0, 2);
            		if (rand2==0) rand2 = 404;
            		else rand2 = 417;
            		imap.SetTile(randx, randy, rand2, 9, 0);
            	}
            }
            
            //wall decorations
            rand = Server.Math.Rand(0, 20);
            for (int i = 1; i <= rand; i++) {
            	randx = Server.Math.Rand(0, imap.MaxX+1);
            	randy = Server.Math.Rand(0, imap.MaxY+1);
            	rand2 = Server.Math.Rand(0, 2);
            	if (rand2==0) rand2 = 471;
            	else rand2 = 484;
            	if (imap.Tile[randx,randy].Type == Server.Enums.TileType.NPCAvoid) imap.SetTile(randx, randy, rand2, 9, 1);
            	else i--;
            }
            
            AddTraps(client, imap);
        }
        
        public static void AddTraps(Client client, IMap imap) {
        	int[] traps = new int[] {2, 3, 4, 5, 6, 7, 23, 25, 26, 28, 42, 43, 44, 50, 51, 52, 53};
        	int randx, randy;
        	int rand = Server.Math.Rand(0, 20);
            if (rand >= 17) {  //17, 18, 19; 2 traps
            	for (int i = 0; i < 2; i++) {
            		randx = Server.Math.Rand(0, imap.MaxX+1);
            		randy = Server.Math.Rand(0, imap.MaxY+1);
            		rand = traps[Server.Math.Rand(0, traps.Length)];
            		if (imap.Tile[randx,randy].Type == Server.Enums.TileType.Walkable) imap.SetAttribute(randx, randy, Enums.TileType.Scripted, rand, 0, 0, "", "", "");
            		else i--;
            	}
            } else if (rand >= 10) { //1 trap; 1/2 chance of no trap
            	for (int i = 0; i < 1; i++) {
            		randx = Server.Math.Rand(0, imap.MaxX+1);
            		randy = Server.Math.Rand(0, imap.MaxY+1);
            		rand = traps[Server.Math.Rand(0, traps.Length)];
            		if (imap.Tile[randx,randy].Type == Server.Enums.TileType.Walkable) imap.SetAttribute(randx, randy, Enums.TileType.Scripted, rand, 0, 0, "", "", "");
            		else i--;
            	}
            }
        }
        
        public static void UnlockRoom(Client client, IMap imap) {
        	int remainingEnemies = 0;
            for (int i = 0; i < Constants.MAX_MAP_NPCS; i++) {
            	if (!imap.IsNpcSlotEmpty(i)) remainingEnemies++;
            }
        	if (remainingEnemies <= 1) {
	        	if (imap.MaxX == 19) {
    	    		for (int i = 8; i <= 11; i++) {
        	    		if (imap.Tile[i,1].Type == Enums.TileType.Key) {
            				imap.SetTile(i, 1, 0, 0, 1);
            				imap.SetAttribute(i, 1, Enums.TileType.Walkable, 0, 0, 0, "", "", "");
            				Messenger.SendTile(i, 1, imap);
            			}
            			if (imap.Tile[i,13].Type == Enums.TileType.Key) {
	            			imap.SetTile(i, 13, 0, 0, 1);
    	        			imap.SetAttribute(i, 13, Enums.TileType.Walkable, 0, 0, 0, "", "", "");
    	        			Messenger.SendTile(i, 13, imap);
        	    		}
            		}
            		for (int i = 6; i <= 8; i++) {
            			if (imap.Tile[1,i].Type == Enums.TileType.Key) {
            				imap.SetTile(1, i, 0, 0, 1);
            				imap.SetAttribute(1, i, Enums.TileType.Walkable, 0, 0, 0, "", "", "");
            				Messenger.SendTile(1, i, imap);
	            		}
    	        		if (imap.Tile[18,i].Type == Enums.TileType.Key) {
        	    			imap.SetTile(18,i, 0, 0, 1);
            				imap.SetAttribute(18, i, Enums.TileType.Walkable, 0, 0, 0, "", "", "");
            				Messenger.SendTile(18, i, imap);
            			}
            		}
	        	} else if (imap.MaxX == 46) {
    	    		for (int i = 22; i <= 24; i++) {
        	    		if (imap.Tile[i,1].Type == Enums.TileType.Key) {
            				imap.SetTile(i, 1, 0, 0, 1);
            				imap.SetAttribute(i, 1, Enums.TileType.Walkable, 0, 0, 0, "", "", "");
            				Messenger.SendTile(i, 1, imap);
            			}
            			if (imap.Tile[i,33].Type == Enums.TileType.Key) {
	            			imap.SetTile(i, 33, 0, 0, 1);
    	        			imap.SetAttribute(i, 33, Enums.TileType.Walkable, 0, 0, 0, "", "", "");
    	        			Messenger.SendTile(i, 33, imap);
        	    		}
            		}
            		for (int i = 16; i <= 18; i++) {
            			if (imap.Tile[1,i].Type == Enums.TileType.Key) {
            				imap.SetTile(1, i, 0, 0, 1);
            				imap.SetAttribute(1, i, Enums.TileType.Walkable, 0, 0, 0, "", "", "");
            				Messenger.SendTile(1, i, imap);
	            		}
    	        		if (imap.Tile[45,i].Type == Enums.TileType.Key) {
        	    			imap.SetTile(45, i, 0, 0, 1);
            				imap.SetAttribute(45, i, Enums.TileType.Walkable, 0, 0, 0, "", "", "");
            				Messenger.SendTile(45, i, imap);
            			}
            		}
	        	}
        	}
        }
        
        public static void SpawnItems(Client client, IMap imap) {
        	//new item code
        	
        	#region item arrays
        	int[] heal = new int[] {
        		2, //apple 
        		4, //oran berry
        		991, //oren berry
        		487, //pecha berry
        		493, //chesto berry
        		496, //persim berry
        		492, //cheri berry
        		494, //rawst berry
        		497, //lum berry
        		489, //reviver seed
        		//reviser seed
        		452, //revival herb
        		557, //dark liquid
        		203, //elixir
        		86 //leppa berry
        		};
        		
        	int[] held = new int[] {
        		41, //power band
        		87, //def. scarf
        		153, //zinc band
        		46, //special band
        		483, //pecha scarf
        		298, //heal ribbon
        		323, //persim band
        		421, //no-stick cap
        		431, //no-slip cap
        		151, //stamina band
        		66, //x-ray specs
        		348 //y-ray specs
        		};
        		
        	int[] oneuse = new int[] {
        		559, //pitch-black key
        		432, //pathfinder orb
        		449, //sleep seed
        		460, //slip seed
        		//doom seed
        		//dough seed
        		//vile seed
        		//via seed
        		345, //gravelerock
        		//gravelyrock
        		346, //geo pebble
        		48, //stick
        		60, //iron thorn
        		642, //colbur berry
        		648, //kasib berry
        		653, //kebia berry
        		11 //tasty honey
        		};
			
        	int[] tm = new int[] {
        		19, //toxic
        		558, //shadow claw
        		124, //torment
        		548, //dream eater
        		126, //calm mind
        		184, //safeguard
        		335, //sludge bomb
        		13, //rest
        		12, //payback
        		332, //psych up
        		552, //trick room
        		440, //psyshock
        		//quash //unmade, move is unfinished
        		//443 //vacuum-cut //make chest-only //removed
        		};
        	
        	int[] orb = new int[] {
        		316, //sunny orb
        		317, //rainy orb
        		315, //hail orb
        		320, //cloudy orb
        		319, //snowy orb
        		321, //foggy orb
        		293, //slumber orb
        		294, //totter orb
        		304, //petrify orb
        		291, //trap-see orb
        		500 //escape orb
        		};
        	
        	int[] treasure = new int[] {
        		1, //poke
        		110, //egg
        		542 //sinister box
        		};
        	#endregion
        	
        	int rand, num, randx, randy, itemnum, amount;
        	string para;
        	
        	if (imap.MaxX == 19) {
        		rand = Server.Math.Rand(0,100);
        		if (rand < 50) num = 0;
        		else if (rand < 75) num = 1;
        		else if (rand < 90) num = 2;
        		else if (rand < 97) num = 3;
        		else num = 4;
        	} 
        	else {
        		rand = Server.Math.Rand(0,142);
        		if (rand < 55) num = 0;
        		else if (rand < 89) num = 1;
        		else if (rand < 110) num = 2;
        		else if (rand < 123) num = 3;
        		else if (rand < 131) num = 4;
        		else if (rand < 136) num = 5;
        		else if (rand < 139) num = 6;
        		else if (rand < 141) num = 7;
        		else num = 8;
        	}
        	//Messenger.PlayerMsg(client, "num = " + num.ToString(), Text.Black);
        	
        	for (int i = 0; i < num; i++) {
        		//find a place
        		randx = Server.Math.Rand(0, imap.MaxX + 1);
        		randy = Server.Math.Rand(0, imap.MaxY + 1);
        		if (imap.Tile[randx,randy].Type == Enums.TileType.Walkable) {
        			#region generate random item
        			rand = Server.Math.Rand(0,100);
        			if (rand < 35) { //healing item 35
        				rand = Server.Math.Rand(0,83);
        				if (rand < 10) itemnum = heal[0];
        				else if (rand < 15) itemnum = heal[1];
        				else if (rand < 18) itemnum = heal[2];
        				else if (rand < 27) itemnum = heal[3];
        				else if (rand < 36) itemnum = heal[4];
        				else if (rand < 45) itemnum = heal[5];
        				else if (rand < 54) itemnum = heal[6];
        				else if (rand < 63) itemnum = heal[7];
        				else if (rand < 66) itemnum = heal[8];
        				else if (rand < 68) itemnum = heal[9];
        				else if (rand < 69) itemnum = heal[10];
        				else if (rand < 73) itemnum = heal[11];
        				else if (rand < 77) itemnum = heal[12];
        				else itemnum = heal[13];
        			} 
        			else if (rand < 92) { //one-use item 92
        				rand = Server.Math.Rand(0,203); //203
        				if (rand < 86) { 
        					rand = rand / 8;
        					itemnum = orb[rand];
        				} else if (rand < 98) itemnum = tm[rand-86];
        				else if (rand < 108) itemnum = oneuse[0];
        				else if (rand < 158) itemnum = oneuse[1];
        				else if (rand < 166) itemnum = oneuse[2];
        				else if (rand < 170) itemnum = oneuse[3];
        				else if (rand < 175) itemnum = oneuse[4];
        				else if (rand < 182) itemnum = oneuse[5];
        				else if (rand < 188) itemnum = oneuse[6];
        				else if (rand < 192) itemnum = oneuse[7];
        				else if (rand < 195) itemnum = oneuse[8];
        				else if (rand < 198) itemnum = oneuse[9];
        				else if (rand < 201) itemnum = oneuse[10];
        				else itemnum = oneuse[11];
        			} 
        			else if (rand < 95) { //held item 95
        				rand = Server.Math.Rand(0,10);
        				if (rand < 9) {
        					itemnum = held[rand];
        				} else {
        					rand = Server.Math.Rand(0,3);
        					itemnum = held[rand+9];
        				}
        			} 
        			else { //treasure 100
        				rand = Server.Math.Rand(0,10);
        				if (rand < 8) itemnum = treasure[0];
        				else itemnum = treasure[rand-7];
        			}
        			#endregion
        			//spawn item, check for egg and Poke for appropriate extras
        			//also check for other stackables
        			amount = 1;
        			para = "";
        			if (itemnum == 1) { //Poke
        				amount = Server.Math.Rand(1,201);
        			} else if (itemnum == 110) { //egg
        				rand = Server.Math.Rand(0,8);
        				switch (rand) {
        					case 0:
        						para = "23;1000";
        						break;
        					case 1:
        						para = "41;1000";
        						break;
        					case 2:
        						para = "96;1000";
        						break;
        					case 3:
        						para = "167;1000";
        						break;
        					case 4:
        						para  = "177;1000";
        						break;
        					case 5:
        						para  = "215;1000";
        						break;
        					case 6:
        						para  = "353;1000";
        						break;
        					case 7:
        						para  = "355;1000";
        						break;
        				}
        			} else if (itemnum == 542) { //Sinister Box
        				rand = Server.Math.Rand(0,4);
        				if (rand < 3) { //TMs
        					for(int j = 0; j < tm.Count(); j++) {
        						para = para + tm[i].ToString();
        						if (j != tm.Count() - 1) para = para + ";";
        					}
        				}
        				else para = "450;451;452;489;702;117;238;461";
        			} else if (Server.Items.ItemManager.Items[itemnum].StackCap > 0) {
        				amount = Server.Math.Rand(1,7);
        			}
        			//Messenger.PlayerMsg(client, "item spawning", Text.Black);
        			//imap.SetAttribute(randx, randy, Enums.TileType.Item, itemnum, amount, 0, "", para, "");
        			imap.SpawnItem(itemnum, amount, false, false, para, randx, randy, null);
        		} else i--;
        	}
        }
        
        public static void SpawnNpcsBad(Client client, IMap imap) {
        	//super-hack-ish code, just making it work
            int rand, n;
            int[] npcs = new int[] { 558, 559, 560, 571, 572, 854, 855, 856, 857, 858, 859, 860, 861, 862, 863 };
            
            if (imap.MaxX == 19) {
            	rand = Server.Math.Rand(0, 20);
            	if (rand == 0) { //no NPCs
	            	n = 0;
    	        } else if (rand > 0 && rand <= 3) { //1 NPC
        	    	n = 1;
            	} else if (rand > 3 && rand <= 9) { //2 NPCs
	            	n = 2;
    	        } else if (rand > 9 && rand <= 15) { //3 NPCs
        	    	n = 3;
            	} else if (rand > 15 && rand <= 18) { //4 NPCs
	            	n = 4;
    	        } else { //rand == 19, 5 NPCs
        	    	n = 5;
            	}
            } 
            else {
            	rand = Server.Math.Rand(0,25);
            	if (rand == 0) { //no NPCs
	            	n = 0;
    	        } else if (rand > 0 && rand <= 2) { //1 NPC
        	    	n = 1;
            	} else if (rand > 2 && rand <= 5) { //2 NPCs
	            	n = 2;
    	        } else if (rand > 5 && rand <= 9) { //3 NPCs
        	    	n = 3;
            	} else if (rand > 9 && rand <= 14) { //4 NPCs
	            	n = 4;
    	        } else if (rand > 14 && rand <= 18) { //5 NPCs
        	    	n = 5;
            	} else if (rand > 18 && rand <= 21) { //6 NPCs
        	    	n = 6;
            	} else if (rand > 21 && rand <= 23) { //7 NPCs
        	    	n = 7;
            	} else { //rand == 24, 8 NPCs
        	    	n = 8;
            	}
           	}
           	
           	for (int i = 1; i <= n; i++) {
                MapNpcPreset npc = new MapNpcPreset();
            	rand = Server.Math.Rand(0, npcs.Length);
            	npc.NpcNum = npcs[rand];
            	if (npcs[rand] == 859) {
            		npc.MinLevel = 10;
            		npc.MaxLevel = 10;
            	} else {
            		npc.MinLevel = 50;
            		npc.MaxLevel = 80;
            	}
            	//npc.AppearanceRate = 100;
            	imap.SpawnNpc(npc);
            }
		}
		
        public static void SpawnNpcs(Client client, IMap imap) {
        	//new NPC code
            //hack it in for now, make it clean later...
            int rand;
            int[] npcs = new int[] { 558, 559, 560, 571, 572, 854, 855, 856, 857, 858, 859, 860, 861, 862, 863 };
            if (imap.MaxX == 19) imap.MaxNpcs = 8;
            else imap.MaxNpcs = 16;
            imap.MinNpcs = 2;
            imap.NpcSpawnTime = 300;
            for (int i = 1; i <= 9; i++) {
                MapNpcPreset npc = new MapNpcPreset();
            	rand = Server.Math.Rand(0, npcs.Length);
            	npc.NpcNum = npcs[rand];
            	if (npcs[rand] == 859) {
            		npc.MinLevel = 10;
            		npc.MaxLevel = 10;
            	} else {
            		npc.MinLevel = 70;
            		npc.MaxLevel = 100;
            	}
            	npc.AppearanceRate = 100;
            	imap.Npc.Add(npc);
            }
            if (imap.MaxX == 19) {
            	rand = Server.Math.Rand(0, 20);
            	if (rand == 0) { //no NPCs
	            	//do nothing
    	        } else if (rand > 0 && rand <= 3) { //1 NPC
        	    	imap.MinNpcs = 1;
            	} else if (rand > 3 && rand <= 9) { //2 NPCs
	            	imap.MinNpcs = 2;
    	        } else if (rand > 9 && rand <= 15) { //3 NPCs
        	    	imap.MinNpcs = 3;
            	} else if (rand > 15 && rand <= 18) { //4 NPCs
	            	imap.MinNpcs = 4;
    	        } else { //rand == 19, 5 NPCs
        	    	imap.MinNpcs = 5;
            	}
            } else {
            	rand = Server.Math.Rand(0,25);
            	if (rand == 0) { //no NPCs
	            	//do nothing
    	        } else if (rand > 0 && rand <= 2) { //1 NPC
        	    	imap.MinNpcs = 1;
            	} else if (rand > 2 && rand <= 5) { //2 NPCs
	            	imap.MinNpcs = 2;
    	        } else if (rand > 5 && rand <= 9) { //3 NPCs
        	    	imap.MinNpcs = 3;
            	} else if (rand > 9 && rand <= 14) { //4 NPCs
	            	imap.MinNpcs = 4;
    	        } else if (rand > 14 && rand <= 18) { //5 NPCs
        	    	imap.MinNpcs = 5;
            	} else if (rand > 18 && rand <= 21) { //6 NPCs
        	    	imap.MinNpcs = 6;
            	} else if (rand > 21 && rand <= 23) { //7 NPCs
        	    	imap.MinNpcs = 7;
            	} else { //rand == 24, 8 NPCs
        	    	imap.MinNpcs = 8;
            	}
           	}
		}
		
		public static void GenerateMap(Client client) {
			//creates a new PBA map for a player and saves it in exPlayers
			//note: this is generated even if players are in a party; leader's map is used though
			//notes on party system: 
			//GenerateMap will only be called once per party, using leader's seed
			//GenerateMap will be called once each time the map switches
			
			Random rng = new Random(exPlayer.Get(client).DungeonSeed);
			int rand;
			bool done = false;
			exPlayer.Get(client).DungeonMap = new PitchBlackAbyss.Room[
				exPlayer.Get(client).DungeonMaxX,
				exPlayer.Get(client).DungeonMaxY];
			
			for (int j = 0; j < exPlayer.Get(client).DungeonMaxY; j++) {
				for (int i = 0; i < exPlayer.Get(client).DungeonMaxX; i++) {
					rand = rng.Next() % 100;
					if (rand < 5) {
						exPlayer.Get(client).DungeonMap[i,j] = Room.One;
					} else if (rand < 20) {
						exPlayer.Get(client).DungeonMap[i,j] = Room.Two;
					} else if (rand < 55) {
						exPlayer.Get(client).DungeonMap[i,j] = Room.Three;
					} else if (rand < 65) {
						exPlayer.Get(client).DungeonMap[i,j] = Room.Four;
					} else if (rand < 96) {
						exPlayer.Get(client).DungeonMap[i,j] = Room.Random;
					} else if (rand < 99) {
						exPlayer.Get(client).DungeonMap[i,j] = Room.Maze;
					} else { //rand == 99
						exPlayer.Get(client).DungeonMap[i,j] = Room.Heal;
					}
				}
			}
			
			//don't bother separating 1s, the player can hope to skip/warp out if they get stuck
			
			//generate S
			rand = rng.Next() % (exPlayer.Get(client).DungeonMaxX * exPlayer.Get(client).DungeonMaxY);
			exPlayer.Get(client).DungeonMap[rand%exPlayer.Get(client).DungeonMaxX,rand/exPlayer.Get(client).DungeonMaxX] = Room.Start;
			
			//generate E
			while (!done) {
				rand = rng.Next() % (exPlayer.Get(client).DungeonMaxX * exPlayer.Get(client).DungeonMaxY);
				if (!(exPlayer.Get(client).DungeonMap[rand%exPlayer.Get(client).DungeonMaxX,rand/exPlayer.Get(client).DungeonMaxX] == Room.Start)) {
					exPlayer.Get(client).DungeonMap[rand%exPlayer.Get(client).DungeonMaxX,rand/exPlayer.Get(client).DungeonMaxX] = Room.End;
					done = true;
				}
			}
			
			exPlayer.Get(client).DungeonGenerated = true;
		}
		
		public static void InitializeMap(Client client, Difficulty diff) {
			Random rng = new Random();
			exPlayer.Get(client).DungeonSeed = rng.Next();
			if (diff == Difficulty.Easy) {
				exPlayer.Get(client).DungeonMaxX = 15;
				exPlayer.Get(client).DungeonMaxY = 15;
			}
			
			//else if diff = normal
			//20? 25?
			//else if diff = OmgWthIsThisEvenPossible
			//30? 35? 40?
			
			exPlayer.Get(client).DungeonMap = new Room[exPlayer.Get(client).DungeonMaxX,exPlayer.Get(client).DungeonMaxY];
			
			//generate initial coords of player
			exPlayer.Get(client).DungeonX = (rng.Next() % exPlayer.Get(client).DungeonMaxX) + 1;
			exPlayer.Get(client).DungeonY = (rng.Next() % exPlayer.Get(client).DungeonMaxY) + 1;
		}
		
		public static void GenerateMaze(Client client, Server.Enums.Direction dir) {
			//generates a brand-new maze map for the player and sends it as their current map
			//warning: code copy-pasted (and revised) from C++, so it won't be neat
			int x; 
			int y; 
			int r;
			int[] order = new int[4];
			InstancedMap imap = new InstancedMap(MapManager.GenerateMapID("i"));
			MapCloner.CloneMapTiles(MapManager.RetrieveMap(1543), imap);
			//Server.Enums.TileType[,] maze = new Server.Enums.TileType[47,35];
			int[,] cells = new int[22,16];
			
			for(int i = 2; i < 45; i++) { //initialize map
				for (int j = 2; j < 33; j++) {
					//maze[i,j] = Server.Enums.TileType.Blocked;
					imap.SetAttribute(i, j, Server.Enums.TileType.Blocked, 0, 0, 0, "", "", "");
					imap.SetTile(i, j, 446, 9, 1);
				}
			}

			for(int i = 0; i < 22; i++) { //initialize cells used for prim's algorithm
				for (int j = 0; j < 16; j++) {
					cells[i,j] = 0;
				}
			}
			
			List<int> list1 = new List<int>();

			x = Server.Math.Rand(1,22);
			y = Server.Math.Rand(1,16);
			cells[x,y] = 2;
			//maze[2*(x+1),2*(y+1)] = Server.Enums.TileType.Walkable;
			imap.SetAttribute(2*(x+1), 2*(y+1), Server.Enums.TileType.Walkable, 0, 0, 0, "", "", "");
			imap.SetTile(2*(x+1), 2*(y+1), 0, 0, 1);
			order = GenerateOrder();

			for (int i = 0; i < 4; i++) {
			    switch(order[i]) {
      				case 1: //up
        				if (y > 0) {
          				cells[x,y-1] = 1;
          				list1.Add(x + 22*(y-1));
        			}
        			break;
     				case 2: //right
	       				if (x < 21) {
    	      				cells[x+1,y] = 1;
        	  				list1.Add(x+1 + 22*y);
        				}
        				break;
      				case 3: //down
	        			if (y < 15) {
    	      				cells[x,y+1] = 1;
        	  				list1.Add(x + 22*(y+1));
        				}
        				break;
      				case 4: //left
	        			if (x > 0) {
    	      				cells[x-1,y] = 1;
        	  				list1.Add(x-1 + 22*y);
        				}
        				break;
    			}
  			}

  			while(list1.Count > 0) {
  				//Messenger.PlayerMsg(client, list1.Count.ToString(), Text.Black);
    			if (Server.Math.Rand(1,3) > 1) { //take random item from list
        			r = Server.Math.Rand(0, list1.Count);
        			x = list1[r];
        			list1.RemoveAt(r);
    			} else { //take last element
        			x = list1.Last();
        			list1.RemoveAt(list1.Count-1);
    			}
    			y = x / 22;
    			x = x % 22;
    			//attach it to random neighbor and add neighbors to queue in random order
    			order = GenerateOrder();
    			for(int i = 0; i < 4; i++) {
	      			switch(order[i]) {
		       			case 1: //up
    		      			if (y > 0) {
        		    			if (cells[x,y-1] == 0) {
            		  				//add to queue
              						cells[x,y-1] = 1;
              						list1.Add(x + 22*(y-1));
            					} else if (cells[x,y-1] == 2 && cells[x,y] == 1) {
	              					//attach
		              				cells[x,y] = 2;
    		          				//maze[2*(x+1),2*(y+1)-1] = Server.Enums.TileType.Walkable;
        		     				//maze[2*(x+1),2*(y+1)] = Server.Enums.TileType.Walkable;
									imap.SetAttribute(2*(x+1), 2*(y+1)-1, Server.Enums.TileType.Walkable, 0, 0, 0, "", "", "");
									imap.SetTile(2*(x+1), 2*(y+1)-1, 0, 0, 1);
									imap.SetAttribute(2*(x+1), 2*(y+1), Server.Enums.TileType.Walkable, 0, 0, 0, "", "", "");
									imap.SetTile(2*(x+1), 2*(y+1), 0, 0, 1);
		            			}
    		      			}
        		  			break;
        				case 2: //right
          					if (x < 21) {
            					if (cells[x+1,y] == 0) {
	              					//add to queue
    	          					cells[x+1,y] = 1;
        	      					list1.Add(x+1 + 22*y);
	            				} else if (cells[x+1,y] == 2 && cells[x,y] == 1) {
    	          					//attach
        	      					cells[x,y] = 2;
            	  					//maze[2*(x+1)+1,2*(y+1)] = Server.Enums.TileType.Walkable;
              						//maze[2*(x+1),2*(y+1)] = Server.Enums.TileType.Walkable;
									imap.SetAttribute(2*(x+1)+1, 2*(y+1), Server.Enums.TileType.Walkable, 0, 0, 0, "", "", "");
									imap.SetTile(2*(x+1)+1, 2*(y+1), 0, 0, 1);
									imap.SetAttribute(2*(x+1), 2*(y+1), Server.Enums.TileType.Walkable, 0, 0, 0, "", "", "");
									imap.SetTile(2*(x+1), 2*(y+1), 0, 0, 1);
	        	    			}
    		      			}
	    	      			break;
	        			case 3: //down
    	      				if (y < 15) {
        	    				if (cells[x,y+1] == 0) {
            	  					//add to queue
              						cells[x,y+1] = 1;
		              				list1.Add(x + 22*(y+1));
	        		    		} else if (cells[x,y+1] == 2 && cells[x,y] == 1) {
    	          					//attach
	    	          				cells[x,y] = 2;
				              		//maze[2*(x+1),2*(y+1)+1] = Server.Enums.TileType.Walkable;
        				      		//maze[2*(x+1),2*(y+1)] = Server.Enums.TileType.Walkable;
									imap.SetAttribute(2*(x+1), 2*(y+1)+1, Server.Enums.TileType.Walkable, 0, 0, 0, "", "", "");
									imap.SetTile(2*(x+1), 2*(y+1)+1, 0, 0, 1);
									imap.SetAttribute(2*(x+1), 2*(y+1), Server.Enums.TileType.Walkable, 0, 0, 0, "", "", "");
									imap.SetTile(2*(x+1), 2*(y+1), 0, 0, 1);
	            				}
			          		}
        			  		break;
	        			case 4: //left
    	      				if (x > 0) {
        	    				if (cells[x-1,y] == 0) {
            		  				//add to queue
              						cells[x-1,y] = 1;
		              				list1.Add(x-1 + 22*y);
	        	    			} else if (cells[x-1,y] == 2 && cells[x,y] == 1) {
    	        		  			//attach
        	      					cells[x,y] = 2;
		    	          			//maze[2*(x+1)-1,2*(y+1)] = Server.Enums.TileType.Walkable;
        			      			//maze[2*(x+1),2*(y+1)] = Server.Enums.TileType.Walkable;
									imap.SetAttribute(2*(x+1)-1, 2*(y+1), Server.Enums.TileType.Walkable, 0, 0, 0, "", "", "");
									imap.SetTile(2*(x+1)-1, 2*(y+1), 0, 0, 1);
									imap.SetAttribute(2*(x+1), 2*(y+1), Server.Enums.TileType.Walkable, 0, 0, 0, "", "", "");
									imap.SetTile(2*(x+1), 2*(y+1), 0, 0, 1);
	            				}
    	      				}
			          		break;
      				}
    			}
  			}
  			imap.SetTile(23, 32, 0, 0, 1);
  			imap.SetAttribute(23, 32, Server.Enums.TileType.Walkable, 0, 0, 0, "", "", "");
  			imap.SetTile(23, 2, 0, 0, 1);
  			imap.SetAttribute(23, 32, Server.Enums.TileType.Walkable, 0, 0, 0, "", "", "");
  			imap.SetTile(2, 17, 0, 0, 1);
  			imap.SetAttribute(23, 32, Server.Enums.TileType.Walkable, 0, 0, 0, "", "", "");
  			imap.SetTile(44, 17, 0, 0, 1);
  			imap.SetAttribute(44, 17, Server.Enums.TileType.Walkable, 0, 0, 0, "", "", "");
			
			DecorateMap(client, imap); //make decoratemap use map's max x/y?
  			//warp player to maze
  			if (dir == Server.Enums.Direction.Up) {
			    x = client.Player.X + 14;
			    if (x > 24) x = 24;
			    y = 32;
			} else if (dir == Server.Enums.Direction.Down) {
			    x = client.Player.X + 14;
			    if (x > 24) x = 24;
			    y = 2;
			} else if (dir == Server.Enums.Direction.Left) {
			    x = 44;
			    y = client.Player.Y + 10;
			} else { //dir == Server.Enums.Direction.Right
			    x = 2;
			    y = client.Player.Y + 10;
			}
			Messenger.PlayerWarp(client, imap, x, y);
		}
		
		static int[] GenerateOrder() { //used by GenerateMaze
			int[] order = new int[4];
		  	order[0] = Server.Math.Rand(1,5);
  			do {
    			order[1] = Server.Math.Rand(1,5);
  			} while (order[0] == order[1]);
  			do {
    			order[2] = Server.Math.Rand(1,5);
  			} while (order[0] == order[2] || order[1] == order[2]);
  			do {
    			order[3] = Server.Math.Rand(1,5);
  			} while (order[0] == order[3] || order[1] == order[3] || order[2] == order[3]);
  			
  			return order;
		}

        public static Room GetRoomType(Client client) {
        	//check to make sure that the array is properly sized first... if not, generate the array
        	try {
        		if (client.Player.IsInParty()) {
        			Server.Players.Parties.Party party = Server.Players.Parties.PartyManager.FindPlayerParty(client);
        			return exPlayer.Get(party.GetLeader()).DungeonMap[exPlayer.Get(client).DungeonX,exPlayer.Get(client).DungeonY];
        		} 
        		return exPlayer.Get(client).DungeonMap[exPlayer.Get(client).DungeonX,exPlayer.Get(client).DungeonY];
        	} catch (Exception ex) {
                Messenger.AdminMsg("Error: PBA:GetRoomType", Text.Black);
                Messenger.AdminMsg(ex.ToString(), Text.Black);
                return Room.Error;
            } 
        }
    }
}

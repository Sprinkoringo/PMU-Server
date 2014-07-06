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
using Server.WonderMails;
using Server.Stories;

namespace Script
{
    public class BossBattles
    {
        public static bool IsBossBattleMap(Client client) {
            IMap map = client.Player.Map;
            if (map.MapType == Enums.MapType.Instanced) {
                switch (((InstancedMap)map).MapBase) {
                    case 1563://"Pitch-Black Pit":
                	case 1287://"Tiny Grotto Depths":
                    case 255://"Seaside Cavern Pit":
                    case 115://"Sour Root Lair":
                    case 114://"Sour Root Chamber":
                    case 1288://"Cliffside Relic":
                	case 531://"Thunderstorm Cliff":
                    case 1248://"Sauna Cavern Chamber":
                    case 1263://"Sauna Cavern Source":
                    case 1392://"Crystal Ruins Hall":
                    case 1394://"Crystal Castle Throne":
                    case 1362://"Southern Sea Depths":
                    case 1292://"Mt. Barricade Chamber":
                    case 1294://"Mt. Barricade Peak":
                    case 1335://"Abandoned Attic End":
                    case 718://"Abandoned Basement Pit":
                    case 534://"Lenile Cavern Pit":
                    case 1638://"Mt. Skylift":
                    case 1048://"Exbel Woods Clearing":
                    case 1851://"Beach Bunker":
                    case 1637://"snowveil lair":
                    case 1670://"snowveil pit":
                    case 1662://"snowveil end":
                    case 1722://"Western Snow Cliff":
                    case 1526://"Deep Winden Forest Clearing":
                    case 1765://"Inferno Volcano":
                    case 1744://"Inferno Volcano Core":
                    case 1745://"Mysterious Jungle Undergrowth":
                    case 1701://"Sacred Tower, Fiery Passage":
                    case 1702://"Sacred Tower, Garden Passage":
                    case 1703://"Sacred Tower, Aquatic Passage":
                    case 1704://"Sacred Tower, Frigid Passage":
                    case 1705://"Sacred Tower, Electrical Passage":
                    case 1706://"Sacred Tower, Malign Passage":
                    case 1707://"Sacred Tower, Esoteric Passage":
                    case 1708://"Sacred Spire":
                    case 1692://"Tanren Dojo, Hitmonchan":
                    case 1694://"Tanren Dojo, Hitmonlee":
                    case 1715://"Tanren Dojo, Hitmontop":
                    case 982://"Honeyglazed Prairie Clearing":
                    case 1994://"Boss Rush 1":
                    case 1990://"Boss Rush 2":
                    case 1983://"Boss Rush 3":
                    case 539://"Boss Rush 4":
                    case 496://"Boss Rush 5":
                    case 1786://"Boggy Wastes Chamber":
                    case 787://"Boggy Wastes Lair":
                    case 1754://"Tanren Mines, Dead End/Zubat Ambush":
                    case 1954://"Tanren Mines Vestibule":
                    case 1007://"Tanren Mines Ancient Cavern":
                    case 1136://"Tanren Mines End":
                        return true;
                    default:
                        return false;
                }
            } else {
                return false;
            }
        }

        public static void StartBossBattle(Client client, InstancedMap bossMap) {
        	if (bossMap.TempStatus.GetStatus("Boss") != null) return;
        	
            switch (bossMap.MapBase) {
                case 1422: {//regice
                        // Currently in mission, warp to normal map
                        // TODO: Add regice code here [Scripts]
                        //if (client.Player.ActiveMission != null && client.Player.ActiveMission.WonderMail.Code == "regicecode") {
                        //    Messenger.PlayerWarp(client, 1422, 10, 22);
                        //    StoryManager.PlayStory(client, 994);
                        //} else {// Not in mission, warp to instanced map
                        //    //if (Ranks.IsAllowed(index, Enums.Rank.Moniter)) {
                        //    InstancedMap map = new InstancedMap(MapManager.GenerateMapID("i"));
                        //    MapCloner.CloneMapTiles(MapManager.RetrieveMap(1422), map);
                        //    map.MapBase = 1422;
                        //    Messenger.PlayerWarp(client, map, 10, 21);
                        //    StoryManager.PlayStory(client, 989);
                        //    //}
                        //}
                    }
                    break;
                case 914: {//jirachi
                        InstancedMap map = new InstancedMap(MapManager.GenerateMapID("i"));
                        MapCloner.CloneMapTiles(MapManager.RetrieveMap(914), map);
                        map.MapBase = 914;
                        Messenger.PlayerWarp(client, map, 9, 12);
                        StoryManager.PlayStory(client, 149);
                    }
                    break;
                case 89: {//steelix
                        InstancedMap map = new InstancedMap(MapManager.GenerateMapID("i"));
                        MapCloner.CloneMapTiles(MapManager.RetrieveMap(89), map);
                        map.MapBase = 89;
                        Messenger.PlayerWarp(client, map, 9, 11);
                        map.SetNpcSpawnPoint(1, 9, 5);
                        map.SetNpc(1, 465);
                        //map.SpawnNpc(1);
                    }
                    break;
                case 590: {//dynamo
                        InstancedMap map = new InstancedMap(MapManager.GenerateMapID("i"));
                        MapCloner.CloneMapTiles(MapManager.RetrieveMap(590), map);
                        map.MapBase = 590;
                        Messenger.PlayerWarp(client, map, 10, 7);
                        map.SetNpc(1, 276);
                        map.SetNpc(2, 277);
                        map.SetNpc(3, 278);
                        //map.SpawnNpc(1);
                        //map.SpawnNpc(2);
                        //map.SpawnNpc(3);
                    }
                    break;
                case 569: {//groudon
                        InstancedMap map = new InstancedMap(MapManager.GenerateMapID("i"));
                        MapCloner.CloneMapTiles(MapManager.RetrieveMap(569), map);
                        map.MapBase = 569;
                        Messenger.PlayerWarp(client, map, 10, 13);
                        StoryManager.PlayStory(client, 154);
                    }
                    break;
                case 667: {//honchkrow
                        InstancedMap map = new InstancedMap(MapManager.GenerateMapID("i"));
                        MapCloner.CloneMapTiles(MapManager.RetrieveMap(667), map);
                        map.MapBase = 667;
                        //NetScript.LockPlayer(index);
                        Messenger.PlayerWarp(client, map, 10, 20);
                        //if (Ranks.IsDisallowed(index, Enums.Rank.StoryWriter)) {
                        if (client.Player.GetDungeonCompletionCount(15) < 1) {
                            StoryManager.PlayStory(client, 159);
                        } else {
                            StoryManager.PlayStory(client, 162);
                        }
                    }
                    break;
                case 424: {//hermi cave
                        //ObjectFactory.GetPlayer(index).IncrementDungeonCompletionCount(9, 1);
                        InstancedMap map = new InstancedMap(MapManager.GenerateMapID("i"));
                        MapCloner.CloneMapTiles(MapManager.RetrieveMap(424), map);
                        map.MapBase = 424;
                        if (client.Player.GetDungeonCompletionCount(9) < 2) {
                            map.SetAttribute(9, 16, Enums.TileType.Item, 370, 1, 0, "", "", "");
                            map.SetAttribute(10, 16, Enums.TileType.Item, 371, 1, 0, "", "", "");
                        } else {
                            map.SetAttribute(9, 15, Enums.TileType.Item, 263, 4, 0, "", "", "");
                            map.SetAttribute(10, 15, Enums.TileType.Item, 264, 4, 0, "", "", "");
                            map.SetAttribute(8, 16, Enums.TileType.Item, 265, 4, 0, "", "", "");
                            map.SetAttribute(11, 16, Enums.TileType.Item, 266, 4, 0, "", "", "");
                            map.SetAttribute(8, 17, Enums.TileType.Item, 266, 4, 0, "", "", "");
                            map.SetAttribute(11, 17, Enums.TileType.Item, 265, 4, 0, "", "", "");
                            map.SetAttribute(9, 18, Enums.TileType.Item, 264, 4, 0, "", "", "");
                            map.SetAttribute(10, 18, Enums.TileType.Item, 263, 4, 0, "", "", "");
                            map.SetAttribute(9, 16, Enums.TileType.Blocked, 0, 0, 0, "", "", "");
                            map.SetAttribute(10, 16, Enums.TileType.Blocked, 0, 0, 0, "", "", "");
                            map.SetAttribute(9, 17, Enums.TileType.Blocked, 0, 0, 0, "", "", "");
                            map.SetAttribute(10, 17, Enums.TileType.Blocked, 0, 0, 0, "", "", "");
                            map.SetTile(9, 15, 118400, 7, 5);
                            map.SetTile(10, 15, 118432, 7, 5);
                            map.SetTile(9, 16, 118848, 7, 5);
                            map.SetTile(10, 16, 118880, 7, 5);
                            map.SetTile(9, 17, 119296, 7, 5);
                            map.SetTile(10, 17, 119328, 7, 5);
                            //map.SetTile(9, 15, 128, 8448, 7, 5);
                            //map.SetTile(10, 15, 160, 8448, 7, 5);
                            //map.SetTile(9, 16, 128, 8480, 7, 5);
                            //map.SetTile(10, 16, 160, 8480, 7, 5);
                            //map.SetTile(9, 17, 128, 8512, 7, 5);
                            //map.SetTile(10, 17, 160, 8512, 7, 5);
                            map.SetAttribute(9, 23, Enums.TileType.Story, 147, 2, 0, "", "", "");
                        }
                        Messenger.PlayerWarp(client, map, 9, 30);
                        //if (ObjectFactory.GetPlayer(index).GetDungeonCompletionCount(9) >= 2) {
                        //    NetScript.PlaySound(index, "magic68.wav");
                        //NetScript.PlayStory(index, 147);
                        //}
                    }
                    break;
                case 503: {//registeel
                        InstancedMap map = new InstancedMap(MapManager.GenerateMapID("i"));
                        MapCloner.CloneMapTiles(MapManager.RetrieveMap(503), map);
                        map.MapBase = 503;
                        Messenger.PlayerWarp(client, map, 9, 12);
                        StoryManager.PlayStory(client, 37);
                    }
                    break;
                case 1108: {//mysterious jungle
                        InstancedMap map = new InstancedMap(MapManager.GenerateMapID("i"));
                        MapCloner.CloneMapTiles(MapManager.RetrieveMap(1108), map);
                        map.MapBase = 1108;
                        Messenger.PlayerWarp(client, map, 10, 13);
                        if (client.Player.HasItem(479) > 0) {
                            StoryManager.PlayStory(client, 165);
                        } else {
                            StoryManager.PlayStory(client, 164);
                        }
                    }
                    break;
                case 174: {//pokemas
                        //InstancedMap map = new InstancedMap(MapManager.GenerateMapID("i"));
                        //MapCloner.CloneMapTiles(MapManager.RetrieveMap(174), map);
                        //map.MapBase = 174;
                        //Messenger.PlayerWarp(client, map, 12, 19);
                        //StoryManager.PlayStory(client, 170);
                    }
                    break;
                case 465: {//heatran
                        InstancedMap map = new InstancedMap(MapManager.GenerateMapID("i"));
                        MapCloner.CloneMapTiles(MapManager.RetrieveMap(465), map);
                        map.MapBase = 465;
                        Messenger.PlayerWarp(client, map, 10, 27);
                        map.SetNpc(1, 404);
                        map.SetNpcSpawnPoint(1, 10, 6);
                        //map.SpawnNpc(1);
                    }
                    break;
                case 1563: {//xatu
                        //InstancedMap map = new InstancedMap(MapManager.GenerateMapID("i"));
                        //MapCloner.CloneMapTiles(MapManager.RetrieveMap(1563), map);
                        //map.MapBase = 1563;
                        //Messenger.PlayerWarp(client, map, 7, 3);
                        StoryManager.PlayStory(client, 175);
                    }
                    break;
                //case "Darkrai": {
                //        //instanced map
                //        //warp
                //        //story
                //    }
                //    break;
                case 1287: {//tiny grotto
                        //InstancedMap map = new InstancedMap(MapManager.GenerateMapID("i"));
                        //MapCloner.CloneMapTiles(MapManager.RetrieveMap(1287), map);
                        //MapCloner.CloneMapGeneralProperties(MapManager.RetrieveMap(1287), map);
                        //map.MapBase = 1287;
                        //Messenger.PlayerWarp(client, map, 9, 10);

                        StoryManager.PlayStory(client, 102);
                        //map.SetNpc(0, 502);
                        //map.SetNpcSpawnPoint(0, 9, 3);
                    }
                    break;
                case 114: {//sour root mini
                        //InstancedMap map = new InstancedMap(MapManager.GenerateMapID("i"));
                        //MapCloner.CloneMapTiles(MapManager.RetrieveMap(114), map);
                        //MapCloner.CloneMapGeneralProperties(MapManager.RetrieveMap(114), map);
                        //map.MapBase = 114;
                        //Messenger.PlayerWarp(client, map, 7, 12);

                        StoryManager.PlayStory(client, 138);
                    }
                    break;
                case 115: {//sour root
                        //InstancedMap map = new InstancedMap(MapManager.GenerateMapID("i"));
                        //MapCloner.CloneMapTiles(MapManager.RetrieveMap(115), map);
                        //MapCloner.CloneMapGeneralProperties(MapManager.RetrieveMap(115), map);
                        //map.MapBase = 115;
                        //Messenger.PlayerWarp(client, map, 10, 10);

                        StoryManager.PlayStory(client, 136);
                    }
                    break;
                case 255: {//seaside cavern
                        //InstancedMap map = new InstancedMap(MapManager.GenerateMapID("i"));
                        //MapCloner.CloneMapTiles(MapManager.RetrieveMap(255), map);
                        //MapCloner.CloneMapGeneralProperties(MapManager.RetrieveMap(255), map);
                        //map.MapBase = 255;
                        //map.SetTile(9, 4, 6, 0, 4, 3);
                        //map.SetAttribute(9, 4, Server.Enums.TileType.Blocked, 0, 0, 0, "", "", "");
                        //Messenger.PlayerWarp(client, map, 9, 9);

                        StoryManager.PlayStory(client, 115);
                    }
                    break;
                case 1609: {
                        InstancedMap map = new InstancedMap(MapManager.GenerateMapID("i"));
                        IMap originalMap = MapManager.RetrieveMap(1609);
                        MapCloner.CloneMapTileProperties(originalMap, map);
                        MapCloner.CloneMapTiles(originalMap, map);

                        Messenger.PlayerWarp(client, map, 11, 29);
                    }
                    break;
                case 1288: {//cliffside
                        //InstancedMap map = new InstancedMap(MapManager.GenerateMapID("i"));
                        //MapCloner.CloneMapTiles(MapManager.RetrieveMap(1288), map);
                        //MapCloner.CloneMapGeneralProperties(MapManager.RetrieveMap(1288), map);
                        //map.MapBase = 1288;
                        //Messenger.PlayerWarp(client, map, 10, 10);

                        StoryManager.PlayStory(client, 118);
                    }
                    break;
                case 531: {//thunderstorm forest
                        //InstancedMap map = new InstancedMap(MapManager.GenerateMapID("i"));
                        //MapCloner.CloneMapTiles(MapManager.RetrieveMap(531), map);
                        //MapCloner.CloneMapGeneralProperties(MapManager.RetrieveMap(531), map);
                        //map.MapBase = 531;
                        //Messenger.PlayerWarp(client, map, 9, 13);

                        StoryManager.PlayStory(client, 122);
                    }
                    break;
                case 1248: {//sauna cavern mini
                        //InstancedMap map = new InstancedMap(MapManager.GenerateMapID("i"));
                        //MapCloner.CloneMapTiles(MapManager.RetrieveMap(1248), map);
                        //MapCloner.CloneMapGeneralProperties(MapManager.RetrieveMap(1248), map);
                        //map.MapBase = 1248;
                        //Messenger.PlayerWarp(client, map, 9, 12);

                        StoryManager.PlayStory(client, 142);
                    }
                    break;
                case 1263: {//sauna cavern
                        //InstancedMap map = new InstancedMap(MapManager.GenerateMapID("i"));
                        //MapCloner.CloneMapTiles(MapManager.RetrieveMap(1263), map);
                        //MapCloner.CloneMapGeneralProperties(MapManager.RetrieveMap(1263), map);
                        //map.MapBase = 1263;
                        //Messenger.PlayerWarp(client, map, 10, 12);

                        StoryManager.PlayStory(client, 140);
                    }
                    break;
                case 1392: {//crystal ruins
                        //InstancedMap map = new InstancedMap(MapManager.GenerateMapID("i"));
                        //MapCloner.CloneMapTiles(MapManager.RetrieveMap(1392), map);
                        //MapCloner.CloneMapGeneralProperties(MapManager.RetrieveMap(1392), map);
                        //map.MapBase = 1392;
                        //Messenger.PlayerWarp(client, map, 9, 11);

                        StoryManager.PlayStory(client, 186);
                    }
                    break;
                case 1394: {//frozen citadel
                        //InstancedMap map = new InstancedMap(MapManager.GenerateMapID("i"));
                        //MapCloner.CloneMapTiles(MapManager.RetrieveMap(1394), map);
                        //MapCloner.CloneMapGeneralProperties(MapManager.RetrieveMap(1394), map);
                        //map.MapBase = 1394;
                        //Messenger.PlayerWarp(client, map, 10, 10);

                        StoryManager.PlayStory(client, 188);
                    }
                    break;
                case 1362: {//southern sea
                        //InstancedMap map = new InstancedMap(MapManager.GenerateMapID("i"));
                        //MapCloner.CloneMapTiles(MapManager.RetrieveMap(1362), map);
                        //MapCloner.CloneMapGeneralProperties(MapManager.RetrieveMap(1362), map);
                        //map.MapBase = 1362;
                        //Messenger.PlayerWarp(client, map, 9, 12);

                        StoryManager.PlayStory(client, 144);
                    }
                    break;
                case 1335: {//Chandalure
                		StoryManager.PlayStory(client, 217);
                	}
                	break;
                case 718: {//rotom forme
                		StoryManager.PlayStory(client, 215);
                	}
                	break;
                case 534: {//lapras
                		StoryManager.PlayStory(client, 216);
                	}
                	break;
                case 1638: {//skylift
                		StoryManager.PlayStory(client, 197);
                	}
                	break;
				case 1048: {//Exbel Woods Clearing
                		StoryManager.PlayStory(client, 196);
                	}
                	break;
                case 1851: {//beach bunker
                		StoryManager.PlayStory(client, 203);
                	}
                	break;
                case 1637: {//snowveil lair
                		StoryManager.PlayStory(client, 326);
                	}
                	break;
                case 1670: {//snowveil pit
                		StoryManager.PlayStory(client, 328);
                	}
                	break;
                case 1662: {//snowveil end
                		StoryManager.PlayStory(client, 330);
                	}
                	break;
                case 1722: {//articuno
                		StoryManager.PlayStory(client, 170);
                	}
                	break;
                case 1526: {//Deep Winden Forest Clearing
                		StoryManager.PlayStory(client, 322);
                	}
                	break;	
               case 1744: {//Inferno Volcano Core
                		StoryManager.PlayStory(client, 393);
                	}
                	break;
               case 1745: {//Mysterious Jungle Undergrowth
                		StoryManager.PlayStory(client, 346);
                	}
                	break;	
               case 1765: {//Inferno Volcano
               			StoryManager.PlayStory(client, 141);
               		}
               		break;
               case 1701: {//Sacred Tower, Fiery Passage
               			StoryManager.PlayStory(client, 370);
               		}
               		break;
               case 1702: {//Sacred Tower, Garden Passage
               			StoryManager.PlayStory(client, 371);
               		}
               		break;
               case 1703: {//Sacred Tower, Aquatic Passage
               			StoryManager.PlayStory(client, 372);
               		}
               		break;
               case 1704: {//Sacred Tower, Frigid Passage
               			StoryManager.PlayStory(client, 373);
               		}
               		break;
               case 1705: {//Sacred Tower, Electrical Passage
               			StoryManager.PlayStory(client, 374);
               		}
               		break;
               case 1706: {//Sacred Tower, Malign Passage
               			StoryManager.PlayStory(client, 375);
               		}
               		break;
               case 1707: {//Sacred Tower, Esoteric Passage
               			StoryManager.PlayStory(client, 376);
               		}
               		break;
               case 1708: {//Sacred Spire
               			StoryManager.PlayStory(client, 378);
               		}
               		break;
               case 1692: {//Tanren Dojo, Hitmonchan
               			StoryManager.PlayStory(client, 247);
               		}
               		break;
               case 1694: {//Tanren Dojo, Hitmonlee
               			StoryManager.PlayStory(client, 267);
               		}
               		break;
               case 1715: {//Tanren Dojo, Hitmontop
               			StoryManager.PlayStory(client, 323);
               		}
               		break;	
               case 982: {//Honeydrop Meadow Clearing
               			StoryManager.PlayStory(client, 352);
               		}
               		break;
               case 1994: {//Boss Rush 1
               			StoryManager.PlayStory(client, 354);
               		}
               		break;
               case 1990: {//Boss Rush 2
               			StoryManager.PlayStory(client, 355);
               		}
               		break;	
               case 1983: {//Boss Rush 3
               			StoryManager.PlayStory(client, 356);
               		}
               		break;
               case 539: {//Boss Rush 4
               			StoryManager.PlayStory(client, 357);
               		}
               		break;	
               case 496: {//Boss Rush 5
               			StoryManager.PlayStory(client, 358);
               		}
               		break;
               case 1786: {//Boggy Wastes Chamber
               			StoryManager.PlayStory(client, 360);
               		}
               		break;
               case 787: {//Boggy Wastes Lair
               			StoryManager.PlayStory(client, 362);
               		}
               		break;	
               case 1754: {//Tanren Mines, Dead End
               			StoryManager.PlayStory(client, 425);
               		}
               		break;	
               case 1954: {//Tanren Mines Vestibule
               			StoryManager.PlayStory(client, 210);
               		}
               		break;	
               case 1136: {//Tanren Mines End
               			StoryManager.PlayStory(client, 214);
               		}
               		break;
            }
        }
        
        public static void PrepareBossRoom(Client client, InstancedMap bossMap) {
        	if (bossMap.TempStatus.GetStatus("Boss") != null) return;
        	
        	
            switch (bossMap.MapBase) {
                    //mt. barricade
                case 1294: {
                        IMap map = client.Player.Map;
                        
                        map.SetAttribute(13, 18, Enums.TileType.Walkable, 0, 0, 0, "", "", "");
                        Messenger.SendTile(13, 18, map);
                        map.SetAttribute(14, 18, Enums.TileType.Walkable, 0, 0, 0, "", "", "");
                        Messenger.SendTile(14, 18, map);
                        map.SetAttribute(15, 18, Enums.TileType.Walkable, 0, 0, 0, "", "", "");
                        Messenger.SendTile(15, 18, map);
                        map.SetAttribute(16, 18, Enums.TileType.Walkable, 0, 0, 0, "", "", "");
                        Messenger.SendTile(16, 18, map);
                        map.SetAttribute(17, 18, Enums.TileType.Walkable, 0, 0, 0, "", "", "");
                        Messenger.SendTile(17, 18, map);
                        map.SetAttribute(18, 18, Enums.TileType.Walkable, 0, 0, 0, "", "", "");
                        Messenger.SendTile(18, 18, map);
                        map.SetAttribute(19, 18, Enums.TileType.Walkable, 0, 0, 0, "", "", "");
                        Messenger.SendTile(19, 18, map);
                        map.SetAttribute(20, 18, Enums.TileType.Walkable, 0, 0, 0, "", "", "");
                        Messenger.SendTile(20, 18, map);
                        
                        map.SetTile(17, 10, 3751, 7, 3);
                        map.SetAttribute(17, 10, Enums.TileType.Blocked, 0, 0, 0, "", "", "");
                        Messenger.SendTile(17, 10, map);
                        
                        map.SetTile(20, 12, 3751, 7, 3);
                        map.SetAttribute(20, 12, Enums.TileType.Blocked, 0, 0, 0, "", "", "");
                        Messenger.SendTile(20, 12, map);
                        
                        map.SetTile(12, 14, 3751, 7, 3);
                        map.SetAttribute(12, 14, Enums.TileType.Blocked, 0, 0, 0, "", "", "");
                        Messenger.SendTile(12, 14, map);
                        
                        map.SetTile(23, 23, 3751, 7, 3);
                        map.SetAttribute(23, 23, Enums.TileType.Blocked, 0, 0, 0, "", "", "");
                        Messenger.SendTile(23, 23, map);
                        
                        map.SetTile(14, 19, 3751, 7, 3);
                        map.SetAttribute(14, 19, Enums.TileType.Blocked, 0, 0, 0, "", "", "");
                        Messenger.SendTile(14, 19, map);
                        
                        map.SetTile(14, 12, 3750, 7, 3);
                        map.SetAttribute(14, 12, Enums.TileType.Blocked, 0, 0, 0, "", "", "");
                        Messenger.SendTile(14, 12, map);
                        
                        map.SetTile(19, 15, 3750, 7, 3);
                        map.SetAttribute(19, 15, Enums.TileType.Blocked, 0, 0, 0, "", "", "");
                        Messenger.SendTile(19, 15, map);
                        
                        map.SetTile(13, 16, 3750, 7, 3);
                        map.SetAttribute(13, 16, Enums.TileType.Blocked, 0, 0, 0, "", "", "");
                        Messenger.SendTile(13, 16, map);
                        
                        map.SetTile(10, 21, 3750, 7, 3);
                        map.SetAttribute(10, 21, Enums.TileType.Blocked, 0, 0, 0, "", "", "");
                        Messenger.SendTile(10, 21, map);
                        
                        map.SetTile(19, 19, 3750, 7, 3);
                        map.SetAttribute(19, 19, Enums.TileType.Blocked, 0, 0, 0, "", "", "");
                        Messenger.SendTile(19, 19, map);
                        
                        map.SetTile(22, 18, 3750, 7, 3);
                        map.SetAttribute(22, 18, Enums.TileType.Blocked, 0, 0, 0, "", "", "");
                        Messenger.SendTile(22, 18, map);
                        
                        map.SetTile(18, 21, 3750, 7, 3);
                        map.SetAttribute(18, 21, Enums.TileType.Blocked, 0, 0, 0, "", "", "");
                        Messenger.SendTile(18, 21, map);
                		
                		
                		Messenger.PlaySoundToMap(client.Player.MapID, "magic660.wav");
                	}
                	break;
                	}
        }

        public static void RunPostIntroStoryBossBattle(Client client, string tag) {
            if (client.Player.Map.MapType != Enums.MapType.Instanced) return;
            InstancedMap map = client.Player.Map as InstancedMap;
            switch (map.MapBase) {
                case 1287: {//Tiny Grotto Depths
                        
                        //if (map.Name == "Tiny Grotto Depths") {
                            MapNpcPreset npc = new MapNpcPreset();
                            npc.SpawnX = 9;
                            npc.SpawnY = 3;
                            npc.NpcNum = 502;
                            npc.MinLevel = 6;
                            npc.MaxLevel = 6;
                            map.SpawnNpc(npc);
                        //}
                    }
                    break;
                case 114: { //sour root miniboss
                        
                        //if (map.Name == "Sour Root Chamber") {
                            MapNpcPreset npc = new MapNpcPreset();
                            npc.SpawnX = 7;
                            npc.SpawnY = 5;
                            npc.NpcNum = 481;
                            npc.MinLevel = 14;
                            npc.MaxLevel = 14;
                            map.SpawnNpc(npc);
                        //}
                    }
                    break;
                case 115: { // sour root boss
                        
                        //if (map.Name == "Sour Root Lair") {
                            MapNpcPreset npc = new MapNpcPreset();
                            npc.SpawnX = 10;
                            npc.SpawnY = 3;
                            npc.NpcNum = 480;
                            npc.MinLevel = 17;
                            npc.MaxLevel = 17;
                            map.SpawnNpc(npc);
                        //}
                    }
                    break;
                case 255: { // seaside cavern
                        
                        //if (map.Name == "Seaside Cavern Pit") {
                            MapNpcPreset npc = new MapNpcPreset();
                            npc.SpawnX = 9;
                            npc.SpawnY = 5;
                            npc.NpcNum = 487;
                            npc.MaxLevel = 15;
                            npc.MinLevel = 15;
                            map.SpawnNpc(npc);
                        //}
                    }
                    break;
//                case "ElectrostasisTowerA1": {
//                        
//                        if (map.Name == "Electrostasis Tower Chamber") {
//                            int nosepassLevel = 1;
//                            map.Npc[1].SpawnX = 8;
//                            map.Npc[1].SpawnY = 24;
//                            map.Npc[1].NpcNum = 587;
                            //map.Npc[1].Level = nosepassLevel;

//                            map.Npc[2].SpawnX = 13;
//                            map.Npc[2].SpawnY = 22;
//                            map.Npc[2].NpcNum = 587;
                            //map.Npc[2].Level = nosepassLevel;

//                            map.Npc[3].SpawnX = 7;
//                            map.Npc[3].SpawnY = 20;
//                            map.Npc[3].NpcNum = 587;
                            //map.Npc[3].Level = nosepassLevel;

//                            map.Npc[4].SpawnX = 15;
//                            map.Npc[4].SpawnY = 19;
//                            map.Npc[4].NpcNum = 587;
                            //map.Npc[4].Level = nosepassLevel;

//                            for (int i = 1; i <= 4; i++) {
                                //map.SpawnNpc(map.Npc[i]);
//                                map.ActiveNpc[i].Target = client;
//                            }
//                        }
//                    }
//                    break;
                case 1288: { // cliffside
                        
                        //if (map.Name == "Cliffside Relic") {
                            MapNpcPreset npc = new MapNpcPreset();
                            npc.SpawnX = 9;
                            npc.SpawnY = 3;
                            npc.NpcNum = 397;
                            npc.MinLevel = 30;
                            npc.MaxLevel = 30;
                            map.SpawnNpc(npc);
                        //}
                    }
                    break;
                case 531: { // thunderstorm forest
                        
                        if (tag == "ThunderstormForest") {
                            MapNpcPreset npc = new MapNpcPreset();
                            npc.SpawnX = 9;
                            npc.SpawnY = 6;
                            npc.NpcNum = 139;
                            npc.MinLevel = 20;
                            npc.MaxLevel = 20;
                            map.SpawnNpc(npc);
                        } else if (tag == "ThunderstormForestPart2") {
                            MapNpcPreset npc = new MapNpcPreset();
                            npc.SpawnX = 9;
                            npc.SpawnY = 6;
                            npc.NpcNum = 177;
                            npc.MinLevel = 30;
                            npc.MaxLevel = 30;
                            map.SpawnNpc(npc);
                        }
                    }
                    break;
                case 1248: { // sauna cave miniboss
                        
                        //if (map.Name == "Sauna Cavern Chamber") {
                            MapNpcPreset npc = new MapNpcPreset();
                            npc.SpawnX = 9;
                            npc.SpawnY = 6;
                            npc.NpcNum = 381;
                            npc.MinLevel = 40;
                            npc.MaxLevel = 40;
                            map.SpawnNpc(npc);
                        //}
                    }
                    break;
                case 1263: { //sauna cave boss
                        
                        //if (map.Name == "Sauna Cavern Source") {
                            MapNpcPreset npc = new MapNpcPreset();
                            npc.SpawnX = 11;
                            npc.SpawnY = 4;
                            npc.NpcNum = 389;
                            npc.MinLevel = 45;
                            npc.MaxLevel = 45;
                            map.SpawnNpc(npc);
                        //}
                    }
                    break;
                case 1526: { //Deep Winden Forest Clearing
                
                		//if (map.Name == "Deep Winden Forest Clearing") {
                			MapNpcPreset npc = new MapNpcPreset();
                			npc.SpawnX = 9;
                			npc.SpawnY = 8;
                			npc.MinLevel = 40;
                			npc.MaxLevel = 40;
                			npc.NpcNum = 1060;
                			
                			map.SpawnNpc(npc);
                			
                			MapNpcPreset minion = new MapNpcPreset();
                			npc.SpawnX = 8;
                			npc.SpawnY = 6;
                			npc.MinLevel = 35;
                			npc.MaxLevel = 35;
                			
                			map.SpawnNpc(minion);
                			
                			minion.SpawnX = 10;
                			
                			map.SpawnNpc(minion);
                			
                			minion.SpawnX = 11;
                			minion.SpawnY = 7;
                			
                			map.SpawnNpc(minion);
                			
                			minion.SpawnX = 7;
                			 
                			map.SpawnNpc(minion);
                		//}
                	}
                	break;
                case 1744: { //Inferno Volcano Core
                
                		//if (map.Name == "Inferno Volcano Core") {
                			MapNpcPreset npc = new MapNpcPreset();
                			npc.SpawnX = 9;
                			npc.SpawnY = 4;
                			npc.MinLevel = 80;
                			npc.MaxLevel = 80;
                			npc.NpcNum = 1220;
                			
                			map.SpawnNpc(npc);
                	  //}
                	}
                	break;
                case 1745: { //Mysterious Jungle Undergrowth
                
                		//if (map.Name == "Mysterious Jungle Undergrowth") {
                			MapNpcPreset npc = new MapNpcPreset();
                			npc.SpawnX = 10;
                			npc.SpawnY = 7;
                			npc.MinLevel = 85;
                			npc.MaxLevel = 85;
                			npc.NpcNum = 1219;
                			
                			map.SpawnNpc(npc);
                	  //}
                	}
                	break;
                case 1392: { //crystal ruins
                        
                        //if (map.Name == "Crystal Ruins Hall") {
                            MapNpcPreset npc = new MapNpcPreset();
                            npc.SpawnX = 10;
                            npc.SpawnY = 5;
                            npc.NpcNum = 145;
                            npc.MinLevel = 45;
                            npc.MaxLevel = 45;
                            map.SpawnNpc(npc);
                        //}
                    }
                    break;
                case 1394: { //crystal castle
                        
                        //if (map.Name == "Crystal Castle Throne") {
                            MapNpcPreset npc = new MapNpcPreset();
                            npc.SpawnX = 10;
                            npc.SpawnY = 4;
                            npc.NpcNum = 578;
                            npc.MinLevel = 55;
                            npc.MaxLevel = 55;

                            map.SpawnNpc(npc);

                            MapNpcPreset minion = new MapNpcPreset();
                            minion.SpawnX = 6;
                            minion.SpawnY = 5;
                            minion.NpcNum = 812;
                            minion.MinLevel = 55;
                            minion.MaxLevel = 55;

                            map.SpawnNpc(minion);

                            minion.SpawnX = 14;

                            map.SpawnNpc(minion);
                        //}
                    }
                    break;
                case 1362: { // southern sea
                        
                        //if (map.Name == "Southern Sea Depths") {
                            MapNpcPreset npc = new MapNpcPreset();
                            npc.SpawnX = 7;
                            npc.SpawnY = 9;
                            npc.NpcNum = 640;
                            npc.MinLevel = 60;
                            npc.MaxLevel = 60;
                            map.SpawnNpc(npc);
                            npc.SpawnX = 10;
                            npc.SpawnY = 9;
                            npc.NpcNum = 641;
                            map.SpawnNpc(npc);
                        //}
                    }
                    break;
                case 1994: { // Boss Rush Test Map
                        
                        //if (map.Name == "Boss Rush Test Map") {
                            MapNpcPreset npc = new MapNpcPreset();
                            npc.SpawnX = 9;
                            npc.SpawnY = 5;
                            npc.NpcNum = 1401;
                            npc.MinLevel = 30;
                            npc.MaxLevel = 30;
                            map.SpawnNpc(npc);
                            npc.SpawnX = 10;
                            npc.SpawnY = 4;
                            npc.NpcNum = 1402;
                            map.SpawnNpc(npc);
                            npc.SpawnX = 8;
                            npc.SpawnY = 4;
                            npc.NpcNum = 1403;
                            map.SpawnNpc(npc);
                        //}
                    }
                    break;
                case 1990: { // Boss Rush 2
                        
                        //if (map.Name == "Boss Rush 2") {
                            MapNpcPreset npc = new MapNpcPreset();
                            npc.SpawnX = 9;
                            npc.SpawnY = 7;
                            npc.NpcNum = 1404;
                            npc.MinLevel = 30;
                            npc.MaxLevel = 30;
                            map.SpawnNpc(npc);
                            npc.SpawnX = 8;
                            npc.SpawnY = 8;
                            npc.NpcNum = 1405;
                            map.SpawnNpc(npc);
                            npc.SpawnX = 10;
                            npc.SpawnY = 8;
                            npc.NpcNum = 1406;
                            map.SpawnNpc(npc);
                        //}
                    }
                    break;
                case 539: { // Boss Rush 4
                        
                        //if (map.Name == "Boss Rush 4") {
                            MapNpcPreset npc = new MapNpcPreset();
                            npc.SpawnX = 10;
                            npc.SpawnY = 6;
                            npc.NpcNum = 1410;
                            npc.MinLevel = 30;
                            npc.MaxLevel = 30;
                            map.SpawnNpc(npc);
                            npc.SpawnX = 10;
                            npc.SpawnY = 3;
                            npc.NpcNum = 1411;
                            map.SpawnNpc(npc);
                            npc.SpawnX = 16;
                            npc.SpawnY = 5;
                            npc.NpcNum = 1412;
                            map.SpawnNpc(npc);
                            npc.SpawnX = 4;
                            npc.SpawnY = 5;
                            npc.NpcNum = 1413;
                            map.SpawnNpc(npc);
                        //}
                    }
                    break;
                case 496: { // Boss Rush 5
                        
                        //if (map.Name == "Boss Rush 4") {
                            MapNpcPreset npc = new MapNpcPreset();
                            npc.SpawnX = 9;
                            npc.SpawnY = 10;
                            npc.NpcNum = 1414;
                            npc.MinLevel = 30;
                            npc.MaxLevel = 30;
                            map.SpawnNpc(npc);
                            npc.SpawnX = 10;
                            npc.SpawnY = 10;
                            npc.NpcNum = 1415;
                            map.SpawnNpc(npc);
                            npc.SpawnX = 7;
                            npc.SpawnY = 8;
                            npc.NpcNum = 1416;
                            map.SpawnNpc(npc);
                            npc.SpawnX = 12;
                            npc.SpawnY = 8;
                            npc.NpcNum = 1417;
                            map.SpawnNpc(npc);
                        //}
                    }
                    break;
                case 1292: { //barricade miniboss
                        
                        
                        map.SetAttribute(5, 9, Enums.TileType.Walkable, 0, 0, 0, "", "", "");
                        Messenger.SendTile(5, 9, map);
                        
                        map.SetTile(5, 10, 5, 4, 1);
                        map.SetAttribute(5, 10, Enums.TileType.Blocked, 0, 0, 0, "", "", "");
                        Messenger.SendTile(5, 10, map);
                        bool test = false;
                        if (Ranks.IsAllowed(client, Enums.Rank.Mapper) && test) {
                        	Messenger.PlayerMsg(client, "This is a test battle to investigate the freezes in the Mt. Barricade Mini boss battle", Text.Black);
                        	MapNpcPreset npc = new MapNpcPreset();
                            //Hariyama
                            npc.NpcNum = 1;
                            npc.MinLevel = 50;
                            npc.MaxLevel = 50;
                            npc.SpawnX = 5;
                            npc.SpawnY = 6;
                            map.SpawnNpc(npc);
                            npc.SpawnX = 10;
                            npc.SpawnY = 2;
                            map.SpawnNpc(npc);
                            npc.SpawnX = 12;
                            npc.SpawnY = 11;
                            map.SpawnNpc(npc);
                            npc.MinLevel = 40;
                            npc.MaxLevel = 40;
                            npc.SpawnX = 10;
                            npc.SpawnY = 5;
                            map.SpawnNpc(npc);
                            npc.SpawnX = 14;
                            npc.SpawnY = 7;
                            map.SpawnNpc(npc);
                            npc.SpawnX = 8;
                            npc.SpawnY = 8;
                            map.SpawnNpc(npc);
                        } else {
                            MapNpcPreset npc = new MapNpcPreset();
                            //Hariyama
                            npc.NpcNum = 472;
                            npc.MinLevel = 50;
                            npc.MaxLevel = 50;
                            npc.SpawnX = 5;
                            npc.SpawnY = 6;
                            map.SpawnNpc(npc);
                            npc.SpawnX = 10;
                            npc.SpawnY = 2;
                            map.SpawnNpc(npc);
                            npc.SpawnX = 12;
                            npc.SpawnY = 11;
                            map.SpawnNpc(npc);
                            
                            //Makuhita
                            npc.NpcNum = 183;
                            npc.MinLevel = 40;
                            npc.MaxLevel = 40;
                            npc.SpawnX = 10;
                            npc.SpawnY = 5;
                            map.SpawnNpc(npc);
                            npc.SpawnX = 14;
                            npc.SpawnY = 7;
                            map.SpawnNpc(npc);
                            npc.SpawnX = 8;
                            npc.SpawnY = 8;
                            map.SpawnNpc(npc);
                        }
                	}
                	break;
                case 1294: { //barricade
                        
                        
                        //if (map.Name == "Mt. Barricade Peak") {
                        	
                            MapNpcPreset npc = new MapNpcPreset();
                            
                            //Tyranitar
                            npc.NpcNum = 421;
                            npc.MinLevel = 55;
                            npc.MaxLevel = 55;
                            npc.SpawnX = 17;
                            npc.SpawnY = 12;
                            map.SpawnNpc(npc);
                            
                            //Skarmory
                            npc.NpcNum = 438;
                            npc.MinLevel = 50;
                            npc.MaxLevel = 50;
                            npc.SpawnX = 14;
                            npc.SpawnY = 13;
                            map.SpawnNpc(npc);
                            npc.SpawnX = 19;
                            npc.SpawnY = 13;
                            map.SpawnNpc(npc);
                            
                            
                            //Gliscor
                            npc.NpcNum = 437;
                            npc.MinLevel = 50;
                            npc.MaxLevel = 50;
                            npc.SpawnX = 14;
                            npc.SpawnY = 16;
                            map.SpawnNpc(npc);
                            npc.SpawnX = 19;
                            npc.SpawnY = 16;
                            map.SpawnNpc(npc);
                            
                            //Pupitar
                            npc.NpcNum = 439;
                            npc.MinLevel = 45;
                            npc.MaxLevel = 45;
                            npc.SpawnX = 13;
                            npc.SpawnY = 17;
                            map.SpawnNpc(npc);
                            npc.SpawnX = 20;
                            npc.SpawnY = 17;
                            map.SpawnNpc(npc);
                            
                            //Larvitar
                            npc.NpcNum = 416;
                            npc.MinLevel = 40;
                            npc.MaxLevel = 40;
                            npc.SpawnX = 13;
                            npc.SpawnY = 18;
                            map.SpawnNpc(npc);
                            npc.SpawnX = 16;
                            npc.SpawnY = 20;
                            map.SpawnNpc(npc);
                            npc.SpawnX = 18;
                            npc.SpawnY = 20;
                            map.SpawnNpc(npc);
                            npc.SpawnX = 20;
                            npc.SpawnY = 18;
                            map.SpawnNpc(npc);
                        //}
                    }
                    break;
                case 1335: { //abandoned attic
                        
                        //if (map.Name == "Abandoned Attic End") {
                            MapNpcPreset npc = new MapNpcPreset();
                            
                            //litwicks
                            npc.NpcNum = 840;
                            npc.MinLevel = 13;
                            npc.MaxLevel = 13;
                            npc.SpawnX = 5;
                            npc.SpawnY = 7;
                            map.SpawnNpc(npc);
                            npc.SpawnX = 7;
                            map.SpawnNpc(npc);
                            npc.SpawnX = 9;
                            map.SpawnNpc(npc);
                            npc.SpawnX = 11;
                            map.SpawnNpc(npc);
                            npc.SpawnX = 13;
                            map.SpawnNpc(npc);
                            npc.SpawnX = 5;
                            
                            //lampents
                            npc.NpcNum = 841;
                            npc.MinLevel = 20;
                            npc.MaxLevel = 20;
                            npc.SpawnX = 7;
                            npc.SpawnY = 4;
                            map.SpawnNpc(npc);
                            npc.SpawnX = 11;
                            map.SpawnNpc(npc);
                            
                            //chandelure
                            npc.SpawnX = 9;
                            npc.SpawnY = 3;
                            npc.NpcNum = 845;
                            npc.MinLevel = 28;
                            npc.MaxLevel = 28;
                            map.SpawnNpc(npc);
                        //}
                    }
                    break;
                case 718: { //abandoned basement
                        
                        //if (map.Name == "Abandoned Basement Pit") {
                            MapNpcPreset npc = new MapNpcPreset();
                            
                            
                            
                            //rotom
                            npc.SpawnX = 9;
                            npc.SpawnY = 4;
                            npc.MinLevel = 30;
                            npc.MaxLevel = 30;
                            if (Server.Math.Rand(0,2) == 0) {
                            	npc.NpcNum = 806;
                            } else {
                            	npc.NpcNum = 807;
                            }
                            map.SpawnNpc(npc);
                        //}
                    }
                    break;
                case 534: {
                        
                        
                		
                        //if (map.Name == "Lenile Cavern Pit") {
                            MapNpcPreset npc = new MapNpcPreset();
                            
                            
                            //lapras
                            npc.SpawnX = 10;
                            npc.SpawnY = 4;
                            npc.MinLevel = 45;
                            npc.MaxLevel = 45;
                            npc.NpcNum = 886;
                            map.SpawnNpc(npc);
                        //}
                    }
                    break;
                case 1563: { //pitch black pit
                		
                		
                		//if (map.Name == "Pitch-Black Pit") {
                			MapNpcPreset npc = new MapNpcPreset();
                            
                            //Xatu
                            npc.SpawnX = 10;
                            npc.SpawnY = 3;
                            npc.MinLevel = 100;
                            npc.MaxLevel = 100;
                            npc.NpcNum = 579;
                            map.SpawnNpc(npc);
                            
                            //Ninetales
                            npc.SpawnX = 4;
                            npc.SpawnY = 7;
                            npc.MinLevel = 83;
                            npc.MaxLevel = 83;
                            npc.NpcNum = 855;
                            map.SpawnNpc(npc);
                            
                            //Drapion
                            npc.SpawnX = 15;
                            npc.SpawnY = 7;
                            npc.MinLevel = 90;
                            npc.MaxLevel = 90;
                            npc.NpcNum = 856;
                            map.SpawnNpc(npc);
                        //}
                    }
                    break;
                case 1048: {//Exbel Woods Clearing
                        
                        //if (map.Name == "Exbel Woods Clearing") {
                            MapNpcPreset npc = new MapNpcPreset();
                            npc.SpawnX = 9;
                            npc.SpawnY = 6;
                            npc.NpcNum = 963;
                            npc.MinLevel = 5;
                            npc.MaxLevel = 5;
                            map.SpawnNpc(npc);
                        //}
                    }
                    break;
                case 1851: {//Beach Bunker
                        
                        //if (map.Name == "Tiny Grotto Depths") {
                            MapNpcPreset npc = new MapNpcPreset();
                            npc.SpawnX = 9;
                            npc.SpawnY = 7;
                            npc.NpcNum = 150;
                            npc.MinLevel = 60;
                            npc.MaxLevel = 60;
                            map.SpawnNpc(npc);
                            npc.SpawnX = 8;
                            npc.SpawnY = 7;
                            npc.NpcNum = 954;
                            npc.MinLevel = 55;
                            npc.MaxLevel = 55;
                            map.SpawnNpc(npc);
                            npc.SpawnX = 10;
                            npc.SpawnY = 7;
                            npc.NpcNum = 954;
                            npc.MinLevel = 55;
                            npc.MaxLevel = 55;
                            map.SpawnNpc(npc);
                            npc.SpawnX = 7;
                            npc.SpawnY = 5;
                            npc.NpcNum = 955;
                            npc.MinLevel = 55;
                            npc.MaxLevel = 55;
                            map.SpawnNpc(npc);
                            npc.SpawnX = 11;
                            npc.SpawnY = 9;
                            npc.NpcNum = 955;
                            npc.MinLevel = 55;
                            npc.MaxLevel = 55;
                            map.SpawnNpc(npc);
                        //}
                    }
                    break;
                case 1722: { //articuno
                            MapNpcPreset npc = new MapNpcPreset();
                            
                            //articuno
                            npc.SpawnX = 9;
                            npc.SpawnY = 5;
                            npc.MinLevel = 45;
                            npc.MaxLevel = 45;
                            npc.NpcNum = 1061;
                            map.SpawnNpc(npc);
                        //}
                    }
                    break;
                case 1765: {//Inferno Volcano
                		//if (map.Name == "Inferno Volcano") {
                			MapNpcPreset npc = new MapNpcPreset();
                			
                			//Torkoal
                			npc.SpawnX = 10;
                			npc.SpawnY = 2;
                			npc.MinLevel = 70;
                			npc.MaxLevel = 70;
                			npc.NpcNum = 1221;
                			map.SpawnNpc(npc);
                			
                			//Magmar
                			npc.SpawnX = 8;
                			npc.SpawnY = 4;
                			npc.MinLevel = 70;
                			npc.MaxLevel = 70;
                			npc.NpcNum = 1222;
                			map.SpawnNpc(npc);
                			
                			//Ninetales
                			npc.SpawnX = 10;
                			npc.SpawnY = 4;
                			npc.MinLevel = 70;
                			npc.MaxLevel = 70;
                			npc.NpcNum = 1223;
                			map.SpawnNpc(npc);
                			
                			//Flareon
                			npc.SpawnX = 12;
                			npc.SpawnY = 4;
                			npc.MinLevel = 70;
                			npc.MaxLevel = 70;
                			npc.NpcNum = 1224;
                			map.SpawnNpc(npc);
                			
                			//Magcargo
                			npc.SpawnX = 12;
                			npc.SpawnY = 6;
                			npc.MinLevel = 70;
                			npc.MaxLevel = 70;
                			npc.NpcNum = 1225;
                			map.SpawnNpc(npc);
                			
                			//Camerupt
                			npc.SpawnX = 14;
                			npc.SpawnY = 6;
                			npc.MinLevel = 70;
                			npc.MaxLevel = 70;
                			npc.NpcNum = 1226;
                			map.SpawnNpc(npc);
                			
                			//Charizard
                			npc.SpawnX = 10;
                			npc.SpawnY = 8;
                			npc.MinLevel = 70;
                			npc.MaxLevel = 70;
                			npc.NpcNum = 1227;
                			map.SpawnNpc(npc);
                			
                			//Magmortar
                			npc.SpawnX = 12;
                			npc.SpawnY = 8;
                			npc.MinLevel = 70;
                			npc.MaxLevel = 70;
                			npc.NpcNum = 1228;
                			map.SpawnNpc(npc);
                			
                			//Rapidash
                			npc.SpawnX = 10;
                			npc.SpawnY = 10;
                			npc.MinLevel = 70;
                			npc.MaxLevel = 70;
                			npc.NpcNum = 1229;
                			map.SpawnNpc(npc);
                			
                			//Typhlosion
                			npc.SpawnX = 8;
                			npc.SpawnY = 8;
                			npc.MinLevel = 70;
                			npc.MaxLevel = 70;
                			npc.NpcNum = 1230;
                			map.SpawnNpc(npc);
                			
                			//Houndoom
                			npc.SpawnX = 8;
                			npc.SpawnY = 6;
                			npc.MinLevel = 70;
                			npc.MaxLevel = 70;
                			npc.NpcNum = 1231;
                			map.SpawnNpc(npc);
                			
                			//Infernape
                			npc.SpawnX = 6;
                			npc.SpawnY = 6;
                			npc.MinLevel = 70;
                			npc.MaxLevel = 70;
                			npc.NpcNum = 1232;
                			map.SpawnNpc(npc);
                		//}
                	 }
                     break;
                case 1701: { //Flareon
                            MapNpcPreset npc = new MapNpcPreset();
                            
                            //Flareon
                            npc.SpawnX = 9;
                            npc.SpawnY = 4;
                            npc.MinLevel = 72;
                            npc.MaxLevel = 72;
                            npc.NpcNum = 1195;
                            map.SpawnNpc(npc);
                        //}
                    }
                    break;
                case 1702: { //Leafeon
                            MapNpcPreset npc = new MapNpcPreset();
                            
                            //Leafeon
                            npc.SpawnX = 10;
                            npc.SpawnY = 5;
                            npc.MinLevel = 72;
                            npc.MaxLevel = 72;
                            npc.NpcNum = 1196;
                            map.SpawnNpc(npc);
                        //}
                    }
                    break;
                case 1703: { //Vaporeon
                            MapNpcPreset npc = new MapNpcPreset();
                            
                            //Vaporeon
                            npc.SpawnX = 9;
                            npc.SpawnY = 4;
                            npc.MinLevel = 72;
                            npc.MaxLevel = 72;
                            npc.NpcNum = 1197;
                            map.SpawnNpc(npc);
                        //}
                    }
                    break;
                case 1704: { //Glaceon
                            MapNpcPreset npc = new MapNpcPreset();
                            
                            //Glaceon
                            npc.SpawnX = 9;
                            npc.SpawnY = 5;
                            npc.MinLevel = 72;
                            npc.MaxLevel = 72;
                            npc.NpcNum = 1198;
                            map.SpawnNpc(npc);
                        //}
                    }
                    break;
                case 1705: { //Jolteon
                            MapNpcPreset npc = new MapNpcPreset();
                            
                            //Jolteon
                            npc.SpawnX = 10;
                            npc.SpawnY = 5;
                            npc.MinLevel = 72;
                            npc.MaxLevel = 72;
                            npc.NpcNum = 1199;
                            map.SpawnNpc(npc);
                        //}
                    }
                    break;
                case 1706: { //Umbreon
                            MapNpcPreset npc = new MapNpcPreset();
                            
                            //Umbreon
                            npc.SpawnX = 10;
                            npc.SpawnY = 4;
                            npc.MinLevel = 72;
                            npc.MaxLevel = 72;
                            npc.NpcNum = 1200;
                            map.SpawnNpc(npc);
                        //}
                    }
                    break;
                case 1707: { //Espeon
                            MapNpcPreset npc = new MapNpcPreset();
                            
                            //Espeon
                            npc.SpawnX = 10;
                            npc.SpawnY = 4;
                            npc.MinLevel = 72;
                            npc.MaxLevel = 72;
                            npc.NpcNum = 1201;
                            map.SpawnNpc(npc);
                        //}
                    }
                    break;  
                case 1708: { //Ho-oh
                            MapNpcPreset npc = new MapNpcPreset();
                            
                            //Ho-oh
                            npc.SpawnX = 10;
                            npc.SpawnY = 7;
                            npc.MinLevel = 80;
                            npc.MaxLevel = 80;
                            npc.NpcNum = 1202;
                            map.SpawnNpc(npc);
                        //}
                    }
                    break;
                case 1692: { //Hitmonchan
                            MapNpcPreset npc = new MapNpcPreset();
                            
                            //Hitmonchan
                            npc.SpawnX = 10;
                            npc.SpawnY = 7;
                            npc.MinLevel = 50;
                            npc.MaxLevel = 50;
                            npc.NpcNum = 1233;
                            map.SpawnNpc(npc);
                        //}
                    }
                    break; 
                case 1694: { //Hitmonlee
                            MapNpcPreset npc = new MapNpcPreset();
                            
                            //Hitmonlee
                            npc.SpawnX = 10;
                            npc.SpawnY = 7;
                            npc.MinLevel = 50;
                            npc.MaxLevel = 50;
                            npc.NpcNum = 1234;
                            map.SpawnNpc(npc);
                        //}
                    }
                    break;
                case 1715: { //Tanren Dojo Hollow
                			MapNpcPreset npc = new MapNpcPreset();
                			
                			//Hitmontop
                			npc.SpawnX = 10;
                            npc.SpawnY = 7;
                            npc.MinLevel = 60;
                            npc.MaxLevel = 60;
                            npc.NpcNum = 1235;
                            map.SpawnNpc(npc);
                            
                            //Hitmonlee
                            npc.SpawnX = 8;
                            npc.SpawnY = 6;
                            npc.MinLevel = 60;
                            npc.MaxLevel = 60;
                            npc.NpcNum = 1234;
                            map.SpawnNpc(npc);
                            
                            //Hitmonchan
                            npc.SpawnX = 11;
                            npc.SpawnY = 6;
                            npc.MinLevel = 60;
                            npc.MaxLevel = 60;
                            npc.NpcNum = 1233;
                            map.SpawnNpc(npc);
                        //}
                    }
                    break;
                case 982: { //Honeydrop Meadow Clearing
                
                		//if (map.Name == "Honeydrop Meadow Clearing") {
                			MapNpcPreset npc = new MapNpcPreset();
                			npc.SpawnX = 9;
                			npc.SpawnY = 4;
                			npc.MinLevel = 32;
                			npc.MaxLevel = 32;
                			npc.NpcNum = 1397;
                			
                			map.SpawnNpc(npc);
                			
                			MapNpcPreset minion = new MapNpcPreset();
                			npc.SpawnX = 10;
                			npc.SpawnY = 4;
                			npc.MinLevel = 21;
                			npc.MaxLevel = 21;
                			npc.NpcNum = 1398;
                			
                			map.SpawnNpc(minion);
                			
                			minion.SpawnX = 8;
                			
                			map.SpawnNpc(minion);
                			
                			minion.SpawnX = 8;
                			minion.SpawnY = 3;
                			
                			map.SpawnNpc(minion);
                			
                			minion.SpawnX = 9;
                			 
                			map.SpawnNpc(minion);
                			
                			minion.SpawnX = 10;
                			 
                			map.SpawnNpc(minion);
                			
                			minion.SpawnY = 5;
                			 
                			map.SpawnNpc(minion);
                			
                			minion.SpawnX = 9;
                			minion.SpawnY = 5;
                			
                			map.SpawnNpc(minion);
                			
                			minion.SpawnX = 8;
                			minion.SpawnY = 5;
                			
                			map.SpawnNpc(minion);
                		//}
                	}
                	break;
                case 1754: { //Tanren Mines, Dead End
                
                		//if (map.Name == "Tanren Mines, Dead End") {
                			MapNpcPreset minion = new MapNpcPreset();
                			minion.SpawnX = 9;
                			minion.SpawnY = 7;
                			minion.MinLevel = 50;
                			minion.MaxLevel = 50;
                			minion.NpcNum = 1503;
                			
                			map.SpawnNpc(minion);
                			
                			minion.SpawnX = 10;
                			
                			map.SpawnNpc(minion);
                			
                			minion.SpawnX = 8;
                			
                			map.SpawnNpc(minion);
                			
                			minion.SpawnX = 8;
                			minion.SpawnY = 6;
                			
                			map.SpawnNpc(minion);
                			
                			minion.SpawnX = 9;
                			 
                			map.SpawnNpc(minion);
                			
                			minion.SpawnX = 10;
                			 
                			map.SpawnNpc(minion);
                			
                			minion.SpawnX = 11;
                			 
                			map.SpawnNpc(minion);
                			
                			minion.SpawnX = 7;
                			 
                			map.SpawnNpc(minion);
                			
                			minion.SpawnX = 4;
                			minion.SpawnY = 7;
                			
                			map.SpawnNpc(minion);
                			
                			minion.SpawnX = 5;
                			
                			map.SpawnNpc(minion);
                			
                			minion.SpawnY = 8;
                			
                			map.SpawnNpc(minion);
                			
                			minion.SpawnX = 4;
                			 
                			map.SpawnNpc(minion);
                			
                			minion.SpawnX = 13;
                			 
                			map.SpawnNpc(minion);
                			
                			minion.SpawnX = 14;
                			 
                			map.SpawnNpc(minion);
                			
                			minion.SpawnY = 7;
                			 
                			map.SpawnNpc(minion);
                			
                			minion.SpawnX = 13;
                			 
                			map.SpawnNpc(minion);
                		//}
                	}
                	break;
                case 1007: { //Tanren Mines Ancient Cavern
                        
                        
                        map.SetAttribute(12, 12, Enums.TileType.Walkable, 0, 0, 0, "", "", "");
                        Messenger.SendTile(12, 12, map);
                        
                        map.SetTile(12, 13, 5, 4, 1);
                        map.SetAttribute(12, 13, Enums.TileType.Blocked, 0, 0, 0, "", "", "");
                        Messenger.SendTile(12, 13, map);
                        
                        	MapNpcPreset npc = new MapNpcPreset();
                            //Regirock
                            npc.NpcNum = 1529;
                            npc.MinLevel = 65;
                            npc.MaxLevel = 65;
                            npc.SpawnX = 11;
                            npc.SpawnY = 12;
                            map.SpawnNpc(npc);
                        }
                	    break;
            }
        }

        public static void EndBossBattle(Client client, string tag) {
			if (client.Player.Map.MapType != Enums.MapType.Instanced) return;
            InstancedMap map = client.Player.Map as InstancedMap;
            switch (map.MapBase) {
                case 1287: {//"Tiny Grotto Depths": {
                        
                        //if (map.Name == "Tiny Grotto Depths") {
                            StoryManager.PlayStory(client, 103);
                        //}
                    }
                    break;
                case 114: {//"Sour Root Chamber": {
                        
                        //if (map.Name == "Sour Root Chamber") {
                            RunBossBattleCleanup(client, map.Name);
                        //}
                    }
                    break;
                case 115: {//"Sour Root Lair": {
                        
                        //if (map.Name == "Sour Root Lair") {
                            RunBossBattleCleanup(client, map.Name);
                        //}
                    }
                    break;
                case 255: {//"Seaside Cavern Pit": {
                        
                        //if (map.Name == "Seaside Cavern Pit") {

                            StoryManager.PlayStory(client, 117);
                        //}
                    }
                    break;
                case 1745: {//"Mysterious Jungle Undergrowth": {
                        
                        //if (map.Name == "Mysterious Jungle Undergrowth") {

                            StoryManager.PlayStory(client, 347);
                        //}
                    }
                    break;
//                case "ElectrostasisTowerA1": {
//                        
//                        if (map.Name == "Electrostasis Tower Chamber") {
//                            map.SetAttribute(17, 1, Enums.TileType.Warp, 1601, 40, 15, "", "", "");
//                            map.SetAttribute(18, 1, Enums.TileType.Warp, 1601, 40, 15, "", "", "");
//                            map.SetTile(17, 1, 1435, 9, 3);
//                            map.SetTile(18, 1, 1436, 9, 3);
//                            map.SetTile(17, 0, 1421, 9, 7);
//                            map.SetTile(18, 0, 1422, 9, 7);
                            // Animation layers
//                            map.SetTile(17, 1, 1437, 9, 4);
//                            map.SetTile(18, 1, 1438, 9, 4);
//                            map.SetTile(17, 0, 1423, 9, 8);
//                            map.SetTile(18, 0, 1424, 9, 8);

//                            Messenger.SendTile(17, 1, map);
//                            Messenger.SendTile(18, 1, map);
//                            Messenger.SendTile(17, 0, map);
//                            Messenger.SendTile(18, 0, map);

//                            int xRangeStart = 4;
//                            int xRangeEnd = 18;
//                            int yRangeStart = 2;
//                            int yRangeEnd = 8;

//                            int selectedX = Server.Math.Rand(xRangeStart, xRangeEnd + 1);
//                            int selectedY = Server.Math.Rand(yRangeStart, yRangeEnd + 1);
//                            map.SpawnItem(385, 1, false, true, "", selectedX, selectedY, client);
//                            if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
//                                Messenger.PlayerMsg(client, "X: " + selectedX + " Y: " + selectedY, Text.Yellow);
//                            }
//                            StoryManager.PlayStory(client, 202);
//                        }
//                    }
//                    break;
                case 1288: {//"Cliffside Relic": {
                        
                        //if (map.Name == "Cliffside Relic") {

                            StoryManager.PlayStory(client, 119);
                        //}
                    }
                    break;
                case 531: {//"ThunderstormForest": {
                        
                        if (tag == "ThunderstormForest") {

                            StoryManager.PlayStory(client, 123);
                        } else if (tag == "ThunderstormForestPart2") {

                            //StoryManager.PlayStory(client, 124);
                            RunBossBattleCleanup(client, map.Name);
                        }
                    }
                    break;
                case 1248: {//"Sauna Cavern Chamber": {
                        
                        //if (map.Name == "Sauna Cavern Chamber") {
                            RunBossBattleCleanup(client, map.Name);
                        //}
                    }
                    break;
                case 1263: {//"Sauna Cavern Source": {
                        
                        //if (map.Name == "Sauna Cavern Source") {
                            RunBossBattleCleanup(client, map.Name);
                        //}
                    }
                    break;
                case 1392: {//"Crystal Ruins Hall": {
                        
                        //if (map.Name == "Crystal Ruins Hall") {
                            RunBossBattleCleanup(client, map.Name);
                        //}
                    }
                    break;
                case 1394: {//"Crystal Castle Throne": {
                        
                        //if (map.Name == "Crystal Castle Throne") {
                            RunBossBattleCleanup(client, map.Name);
                        //}
                    }
                    break;
                case 1362: {//"Southern Sea Depths": {
                        
                        //if (map.Name == "Southern Sea Depths") {
                            RunBossBattleCleanup(client, map.Name);
                        //}
                    }
                    break;
                case 1292: {//"Mt. Barricade Chamber": {
                        
                        //if (map.Name == "Mt. Barricade Chamber") {
                            RunBossBattleCleanup(client, map.Name);
                        //}
                    }
                    break;
                case 1294: {//"Mt. Barricade Peak": {
                        
                        //if (map.Name == "Mt. Barricade Peak") {
                            RunBossBattleCleanup(client, map.Name);
                        //}
                    }
                    break;
                case 1335: {//"Abandoned Attic End": {
                        
                        //if (map.Name == "Abandoned Attic End") {
                        	
                            RunBossBattleCleanup(client, map.Name);
                        //}
                    }
                    break;
                case 718: {//"Abandoned Basement Pit": {
                        
                        //if (map.Name == "Abandoned Basement Pit") {
                            RunBossBattleCleanup(client, map.Name);
                        //}
                    }
                    break;
                case 534: {//"Lenile Cavern Pit": {
                        
                        //if (map.Name == "Lenile Cavern Pit") {
                            RunBossBattleCleanup(client, map.Name);
                        //}
                    }
                    break;
                case 1563: {//"Pitch-Black Pit": {
                		
                		//if (map.Name == "Pitch-Black Pit") {
                			RunBossBattleCleanup(client, map.Name);
                        //}
                	}
                	break;
                 case 1048: {//"Exbel Woods Clearing": {
                		
                		//if (map.Name == "Pitch-Black Pit") {
                			StoryManager.PlayStory(client, 198);
                        //}
                	}
                	break;
                 case 1851: {//beach bunker
                		
                		StoryManager.PlayStory(client, 182);
                	}
                	break;
                case 1637: {//snowveil
                		StoryManager.PlayStory(client, 327);
                	}
                	break;
                case 1670: {//snowveil pit
                		StoryManager.PlayStory(client, 329);
                	}
                	break;
                case 1722: {//articuno
                        StoryManager.PlayStory(client, 171);
                    }
                    break;
                case 1744: {//"Inferno Volcano Core": {
                
                		//if (map.Name == "Inferno Volcano Core") {
                			StoryManager.PlayStory(client, 343);
                		//}
                	}
                	break;
                case 1526: {//"Deep Winden Forest Clearing": {
                
                		//if (map.Name == "Deep Winden Forest Clearing") {
                			RunBossBattleCleanup(client, map.Name);
                		//}
                	}
                	break;
                case 1765: {//"Inferno Volcano": {
                
                		//if (map.Name == "Inferno Volcano") {
                			StoryManager.PlayStory(client, 143);
                		//}
                	}
                	break;
                case 1701: {//"Sacred Tower, Fiery Passage": {
                
                		//if (map.Name == "Sacred Tower, Fiery Passage") {
                			RunBossBattleCleanup(client, map.Name);
                		//}
                	}
                	break;
                case 1702: {//"Sacred Tower, Garden Passage": {
                
                		//if (map.Name == "Sacred Tower, Garden Passage") {
                			RunBossBattleCleanup(client, map.Name);
                		//}
                	}
                	break;
                case 1703: {//"Sacred Tower, Aquatic Passage": {
                
                		//if (map.Name == "Sacred Tower, Aquatic Passage") {
                			RunBossBattleCleanup(client, map.Name);
                		//}
                	}
                	break;
                case 1704: {//"Sacred Tower, Frigid Passage": {
                
                		//if (map.Name == "Sacred Tower, Frigid Passage") {
                			RunBossBattleCleanup(client, map.Name);
                		//}
                	}
                	break;
                case 1705: {//"Sacred Tower, Electrical Passage": {
                
                		//if (map.Name == "Sacred Tower, Electrical Passage") {
                			RunBossBattleCleanup(client, map.Name);
                		//}
                	}
                	break;
                case 1706: {//"Sacred Tower, Malign Passage": {
                
                		//if (map.Name == "Sacred Tower, Malign Passage") {
                			RunBossBattleCleanup(client, map.Name);
                		//}
                	}
                	break;
                case 1707: {//"Sacred Tower, Esoteric Passage": {
                
                		//if (map.Name == "Sacred Tower, Esoteric Passage") {
                			RunBossBattleCleanup(client, map.Name);
                		//}
                	}
                	break;
                case 1708: {//"Sacred Spire": {
                
                		//if (map.Name == "Sacred Spire") {
                			StoryManager.PlayStory(client, 387);
                		//}
                	}
                	break;
                case 1692: {//"Tanren Dojo Hollow": {
                
                		//if (map.Name == "Tanren Dojo Hollow") {
                			StoryManager.PlayStory(client, 248);
                		//}
                	}
                	break;
                case 1694: {//"Tanren Dojo Hollow": {
                
                		//if (map.Name == "Tanren Dojo Hollow") {
                			StoryManager.PlayStory(client, 268);
                		//}
                	}
                	break;
                case 1715: {//"Tanren Dojo Hollow": {
                
                		//if (map.Name == "Tanren Dojo Hollow") {
                			StoryManager.PlayStory(client, 325);
                		//}
                	}
                	break;
                case 982: {//"Honeydrop Meadow Clearing": {
                        
                        //if (map.Name == "Honeydrop Meadow Clearing") {
                            RunBossBattleCleanup(client, map.Name);
                        //}
                    }
                    break;
                case 1994: {//"Boss Rush Map Test": {
                        
                        //if (map.Name == "Boss Rush Map Test") {
                            RunBossBattleCleanup(client, map.Name);
                        //}
                    }
                    break;
                case 1990: {//"Boss Rush 2": {
                        
                        //if (map.Name == "Boss Rush Map Test") {
                            RunBossBattleCleanup(client, map.Name);
                        //}
                    }
                    break;
                case 1983: {//"Boss Rush 3": {
                        
                        //if (map.Name == "Boss Rush Map Test") {
                            RunBossBattleCleanup(client, map.Name);
                        //}
                    }
                    break;
                case 539: {//"Boss Rush 4": {
                        
                        //if (map.Name == "Boss Rush Map Test") {
                            RunBossBattleCleanup(client, map.Name);
                        //}
                    }
                    break;
                case 496: {//"Boss Rush 5": {
                        
                        //if (map.Name == "Boss Rush Map Test") {
                            RunBossBattleCleanup(client, map.Name);
                        //}
                    }
                    break;
                case 1786: {//"Boggy Wastes Chamber": {
                        
                        //if (map.Name == "Boggy Wastes Chamber") {
                            StoryManager.PlayStory(client, 361);
                        //}
                    }
                    break;
                case 787: {//"Boggy Wastes Lair": {
                        
                        //if (map.Name == "Boggy Wastes Chamber") {
                            StoryManager.PlayStory(client, 363);
                        //}
                    }
                    break;
                case 1754: {//"Tanren Mines, Dead End": {
                        
                        //if (map.Name == "Tanren Mines, Dead End") {
                            RunBossBattleCleanup(client, map.Name);
                        //}
                    }
                    break;
                case 1954: {//"Tanren Mines Vestibule": {
                        
                        //if (map.Name == "Boggy Wastes Chamber") {
                            StoryManager.PlayStory(client, 211);
                        //}
                    }
                    break;
                case 1007: {//"Tanren Mines Ancient Cavern": {
                        
                        //if (map.Name == "Boggy Wastes Chamber") {
                            StoryManager.PlayStory(client, 213);
                        //}
                    }
                    break;
            }
        }

        public static void RunBossBattleCleanup(Client client, string tag) {
        	if (client.Player.Map.MapType != Enums.MapType.Instanced) return;
            InstancedMap map = client.Player.Map as InstancedMap;
			
        	foreach (Client target in client.Player.Map.GetClients()) {
            	if (target.Player.Dead) {
            		Main.ReviveCharacter(target.Player.GetActiveRecruit(), target.Player.Map, false, null);
            	}
            }
        	
            switch (map.MapBase) {
                case 1287: {//tiny grotto
                        
                        map.SetTile(9, 3, 3, 4, 3);
                        map.SetAttribute(9, 3, Enums.TileType.Scripted, 46, 0, 0, "1", "1015", "25:25");
                        Messenger.SendTile(9, 3, map);
                    }
                    break;
                case 114: {//sour root mini
                        
                        map.SetTile(4, 5, 3, 4, 3);
                        map.SetAttribute(4, 5, Enums.TileType.Warp, 111, 9, 1, "", "", "");
                        Messenger.SendTile(4, 5, map);
                    }
                    break;
                case 115: {//sour root
                        
                        map.SetTile(16, 4, 3, 4, 3);
                        map.SetAttribute(16, 4, Enums.TileType.Scripted, 46, 0, 0, "3", "1015", "25:25");
                        Messenger.SendTile(16, 4, map);
                    }
                    break;
                case 255: {//seaside
                        
                        //if (map.Name == "Seaside Cavern Pit") {
                            map.SetTile(9, 3, 3, 4, 3);
	                        map.SetAttribute(9, 3, Enums.TileType.Scripted, 46, 0, 0, "2", "1015", "25:25");
	                        Messenger.SendTile(9, 3, map);
                        //}
                    }
                    break;
//                case "ElectrostasisTowerA1": {
                        
//                        if (map.Name == "Electrostasis Tower Chamber") {
//                            map.SetTile(9, 4, 0, 0, 0, 3);
//                            map.SetAttribute(9, 4, Server.Enums.TileType.Walkable, 0, 0, 0, "", "", "");
//                            Messenger.SendTile(9, 4, map);
//                            map.SetTile(9, 3, 3, 0, 4, 3);
//                            map.SetAttribute(9, 3, Server.Enums.TileType.Scripted, 46, 0, 0, "2", "1015", "25:25");
//                            Messenger.SendTile(9, 3, map);
//                        }
//                    }
//                    break;
                case 1288: {//cliffside
                        
                        map.SetTile(10, 3, 3, 4, 3);
                        map.SetAttribute(10, 3, Enums.TileType.Scripted, 46, 0, 0, "10", "1015", "25:25");
                        Messenger.SendTile(10, 3, map);
                        map.SetTile(10, 1, 196, 10, 3);
                        map.SetTile(10, 1, 197, 10, 4);
                        map.SetAttribute(10, 1, Enums.TileType.ScriptedSign, 14, 0, 0, "0", "0", "0");
                        Messenger.SendTile(10, 1, map);
                    }
                    break;
                case 531: { //thunderstorm
                        
                        map.SetTile(9, 13, 3, 4, 3);
                        map.SetAttribute(9, 13, Enums.TileType.Scripted, 46, 0, 0, "8", "1015", "25:25");
                        Messenger.SendTile(9, 13, map);

                    }
                    break;
                case 1248: { //sauna cave mini
                        
                        map.SetTile(17, 3, 3, 4, 3);
                        map.SetAttribute(17, 3, Enums.TileType.Warp, 1249, 8, 12, "", "", "");
                        Messenger.SendTile(17, 3, map);
                    }
                    break;
                case 1263: {//sauna cave
                        
                        map.SetTile(13, 3, 3, 4, 3);
                        map.SetAttribute(13, 3, Enums.TileType.Scripted, 46, 0, 0, "13", "1991", "26:27");
                        Messenger.SendTile(13, 3, map);
                    }
                    break;
                case 1392: {//crystal ruins
                        
                        map.SetTile(11, 3, 3, 4, 3);
                        map.SetAttribute(11, 3, Enums.TileType.Warp, 1393, 9, 12, "", "", "");
                        Messenger.SendTile(11, 3, map);
                    }
                    break;
                case 1394: {//crystal castle
                        
                        map.SetTile(10, 1, 3, 4, 3);
                        map.SetAttribute(10, 1, Enums.TileType.Scripted, 46, 0, 0, "14", "1991", "26:27");
                        Messenger.SendTile(10, 1, map);
                    }
                    break;
                case 1362: {//southern sea
                        
                        map.SetTile(17, 7, 3, 4, 3);
                        map.SetAttribute(17, 7, Enums.TileType.Scripted, 46, 0, 0, "18", "1015", "25:25");
                        Messenger.SendTile(17, 7, map);
                    }
                    break;
                case 1292: {//barricade mini
                        
                        map.SetTile(15, 2, 0, 0, 1);
                        map.SetAttribute(15, 2, Enums.TileType.Walkable, 0, 0, 0, "", "", "");
                        Messenger.SendTile(15, 2, map);
                        map.SetTile(5, 10, 0, 0, 1);
                        map.SetAttribute(5, 10, Enums.TileType.Walkable, 0, 0, 0, "", "", "");
                        Messenger.SendTile(5, 10, map);
                
                	}
                	break;
                case 1294: {//barricade
                        
                        map.SetTile(17, 10, 3, 4, 3);
                        map.SetAttribute(17, 10, Enums.TileType.Scripted, 46, 0, 0, "19", "1015", "25:25");
                        Messenger.SendTile(17, 10, map);
                        
                        map.SetTile(18, 21, 0, 0, 3);
                        map.SetAttribute(18, 21, Enums.TileType.Walkable, 0, 0, 0, "", "", "");
                        Messenger.SendTile(18, 21, map);
                
                	}
                	break;
                case 1335: {//attic
                	
                        
	                    if (map.Tile[9,1].Type != Enums.TileType.Scripted) {
	                        map.SetTile(9, 1, 3, 4, 3);
	                        map.SetAttribute(9, 1, Enums.TileType.Scripted, 21, 0, 0, "", "", "");
	                        Messenger.SendTile(9, 1, map);
	                		int players = 0;
	                        foreach (Client target in client.Player.Map.GetClients()) {
				            	client.Player.Map.SpawnItem(452, 1, false, false, "", 8 + 2 * (players % 2), 5 + 2 * (players / 2), target);
	                                players++;
				            }
			            }
                	}
                	break;
                case 718: {//basement
                        
                        if (map.Tile[9,1].Type != Enums.TileType.Scripted) {
	                        map.SetTile(9, 1, 3, 4, 3);
	                        map.SetAttribute(9, 1, Enums.TileType.Scripted, 21, 0, 0, "", "", "");
	                        Messenger.SendTile(9, 1, map);
	                        int players = 0;
	                        foreach (Client target in client.Player.Map.GetClients()) {
				            	client.Player.Map.SpawnItem(452, 1, false, false, "", 8 + 2 * (players % 2), 6 + 2 * (players / 2), target);
	                                players++;
				            }
                		}
                	}
                	break;
                case 534: {//lenile
                        
                        map.SetTile(10, 2, 3, 4, 3);
                        map.SetAttribute(10, 2, Enums.TileType.Scripted, 46, 0, 0, "20", "1015", "8:33");
                        Messenger.SendTile(10, 2, map);
                
                	}
                	break;
                case 1563: { //pitch-black
                		
                		map.SetTile(10, 12, 3, 4, 3);
                        map.SetAttribute(10, 12, Enums.TileType.Scripted, 46, 0, 0, "22", "1015", "8:33");
                        Messenger.SendTile(10, 12, map);
                        StoryManager.PlayStory(client, 176);
                	}
                	break;
                 case 1048: {//Exbel Woods Clearing
                        
                        map.SetTile(9, 3, 3, 4, 3);
                        map.SetAttribute(9, 3, Enums.TileType.Scripted, 46, 0, 0, "23", "1015", "25:25");
                        Messenger.SendTile(9, 3, map);
                    }
                    break;
                case 1851: {//lenile
                        
                        map.SetTile(3, 1, 3, 4, 3);
                        map.SetAttribute(3, 1, Enums.TileType.Scripted, 46, 0, 0, "27", "1329", "24:42");
                        Messenger.SendTile(3, 1, map);
                
                	}
                	break;
                case 1722: {//articuno
                        
                        map.SetTile(9, 4, 3, 4, 3);
                        map.SetAttribute(9, 4, Enums.TileType.Scripted, 21, 0, 0, "", "", "");
                        Messenger.SendTile(9, 4, map);
                
                	}
                	break;
                case 1744: {//Inferno Volcano Core
                
                		map.SetTile(9, 1, 3, 4, 3);
                		map.SetAttribute(9, 1, Enums.TileType.Scripted, 46, 0, 0, "29", "1015", "25:25");
                		Messenger.SendTile(9, 1, map);
                	
                	}
                	break;
                case 1745: {//Mysterious Jungle Undergrowth
                
                		map.SetTile(10, 2, 3, 4, 3);
                		map.SetAttribute(10, 2, Enums.TileType.Scripted, 46, 0, 0, "29", "1015", "25:25");
                		Messenger.SendTile(10, 2, map);
                	
                	}
                	break;
                case 1526: {//Deep Winden Forest Clearing
                
                		map.SetTile(9, 1, 3, 4, 3);
                		map.SetAttribute(9, 1, Enums.TileType.Scripted, 46, 0, 0, "28", "1991", "26:27");
                		Messenger.SendTile(9, 1, map);
                	
                	}
                	break;
                case 1701: {//Sacred Tower, Fiery Passage
                
                		map.SetTile(9, 1, 3, 4, 3);
                		map.SetAttribute(9, 1, Enums.TileType.Scripted, 36, 0, 0, "90", "11", "");
                		Messenger.SendTile(9, 1, map);
                	
                	}
                	break;
                case 1702: {//Sacred Tower, Garden Passage
                
                		map.SetTile(10, 1, 3, 4, 3);
                		map.SetAttribute(10, 1, Enums.TileType.Scripted, 36, 0, 0, "90", "21", "");
                		Messenger.SendTile(10, 1, map);
                	
                	}
                	break;
                case 1703: {//Sacred Tower, Aquatic Passage
                
                		map.SetTile(9, 1, 3, 4, 3);
                		map.SetAttribute(9, 1, Enums.TileType.Scripted, 36, 0, 0, "90", "31", "");
                		Messenger.SendTile(9, 1, map);
                	
                	}
                	break;
                case 1704: {//Sacred Tower, Frigid Passage
                
                		map.SetTile(9, 2, 3, 4, 3);
                		map.SetAttribute(9, 2, Enums.TileType.Scripted, 36, 0, 0, "90", "41", "");
                		Messenger.SendTile(9, 2, map);
                	
                	}
                	break;
                case 1705: {//Sacred Tower, Electrical Passage
                
                		map.SetTile(6, 1, 3, 4, 3);
                		map.SetAttribute(6, 1, Enums.TileType.Scripted, 36, 0, 0, "90", "51", "");
                		Messenger.SendTile(6, 1, map);
                	
                	}
                	break;
                case 1706: {//Sacred Tower, Malign Passage
                
                		map.SetTile(10, 4, 3, 4, 3);
                		map.SetAttribute(10, 4, Enums.TileType.Scripted, 36, 0, 0, "90", "61", "");
                		Messenger.SendTile(10, 4, map);
                	
                	}
                	break;
                case 1707: {//Sacred Tower, Esoteric Passage
                
                		map.SetTile(10, 1, 3, 4, 3);
                		map.SetAttribute(10, 1, Enums.TileType.Scripted, 36, 0, 0, "90", "71", "");
                		Messenger.SendTile(10, 1, map);
                	
                	}
                	break;
                case 1708: {//Sacred Spire
                
                		map.SetTile(9, 3, 3, 4, 3);
                		map.SetAttribute(9, 3, Enums.TileType.Scripted, 46, 0, 0, "30", "1720", "25:25");
                		Messenger.SendTile(9, 3, map);
                	}
                	break;
                case 1692: {//Tanren Dojo, Hitmonchan
                
                		map.SetTile(10, 2, 3, 4, 3);
                		map.SetAttribute(10, 2, Enums.TileType.Scripted, 46, 0, 0, "31", "1698", "9:8");
                		Messenger.SendTile(10, 2, map);
                	}
                	break;
                case 1694: {//Tanren Dojo, Hitmonlee
                
                		map.SetTile(9, 2, 3, 4, 3);
                		map.SetAttribute(9, 2, Enums.TileType.Scripted, 46, 0, 0, "32", "1698", "9:8");
                		Messenger.SendTile(9, 2, map);
                	}
                	break;
                case 1715: {//Tanren Dojo, Hitmontop
                
                		map.SetTile(10, 2, 3, 4, 3);
                		map.SetAttribute(10, 2, Enums.TileType.Scripted, 46, 0, 0, "33", "1698", "9:8");
                		Messenger.SendTile(10, 2, map);
                	}
                	break;
                case 982: {//Honeydrop Meadow Clearing
                
                		map.SetTile(10, 1, 3, 4, 3);
                		map.SetAttribute(10, 1, Enums.TileType.Scripted, 46, 0, 0, "26", "1015", "25:25");
                		Messenger.SendTile(10, 1, map);
                	
                	}
                	break;
                case 1994: {//Boss Rush 1
                        
                        map.SetTile(9, 3, 0, 0, 1);
                        map.SetAttribute(9, 3, Enums.TileType.Walkable, 0, 0, 0, "", "", "");
                        Messenger.SendTile(9, 3, map);
                        
                	}
                	break;
                case 1990: {//Boss Rush 2
                        
                        map.SetTile(9, 3, 0, 0, 1);
                        map.SetAttribute(9, 3, Enums.TileType.Walkable, 0, 0, 0, "", "", "");
                        Messenger.SendTile(9, 3, map);
                        
                	}
                	break;
                case 1983: {//Boss Rush 3
                        
                        map.SetTile(10, 3, 0, 0, 1);
                        map.SetAttribute(10, 3, Enums.TileType.Walkable, 0, 0, 0, "", "", "");
                        Messenger.SendTile(10, 3, map);
                        
                	}
                	break;
                case 539: {//Boss Rush 4
                        
                        map.SetTile(10, 1, 0, 0, 1);
                        map.SetAttribute(10, 1, Enums.TileType.Walkable, 0, 0, 0, "", "", "");
                        Messenger.SendTile(10, 1, map);
                        
                	}
                	break;
                case 496: {//Boss Rush 5
                        
                        map.SetTile(9, 6, 0, 0, 1);
                        map.SetAttribute(9, 6, Enums.TileType.Walkable, 0, 0, 0, "", "", "");
                        Messenger.SendTile(9, 6, map);
                        map.SetTile(10, 6, 0, 0, 1);
                        map.SetAttribute(10, 6, Enums.TileType.Walkable, 0, 0, 0, "", "", "");
                        Messenger.SendTile(10, 6, map);
                        
                	}
                	break;
                case 1786: {//Boggy Wastes Chamber
                        
                        map.SetTile(9, 2, 3, 4, 3);
                        map.SetAttribute(9, 2, Enums.TileType.Warp, 1785, 9, 30, "", "", "");
                        Messenger.SendTile(9, 2, map);
                    }
                    break;
                case 787: {//Boggy Wastes Lair
                        
                        map.SetTile(9, 2, 3, 4, 3);
                        map.SetAttribute(9, 2, Enums.TileType.Warp, 1787, 9, 30, "", "", "");
                        Messenger.SendTile(9, 2, map);
                    }
                    break;
                case 1754: {//Tanren Mines, Dead End
                        
                        map.SetTile(9, 5, 3, 4, 3);
                        map.SetAttribute(9, 5, Enums.TileType.Warp, 1753, 12, 47, "", "", "");
                        Messenger.SendTile(9, 5, map);
                        map.SetTile(13, 5, 3, 4, 3);
                        map.SetAttribute(13, 5, Enums.TileType.Warp, 1753, 6, 18, "", "", "");
                        Messenger.SendTile(13, 5, map);
                        map.SetTile(5, 5, 3, 4, 3);
                        map.SetAttribute(5, 5, Enums.TileType.Warp, 1753, 25, 39, "", "", "");
                        Messenger.SendTile(5, 5, map);
                    }
                    break;
                case 1954: {//Tanren Mines Vestibule
                        
                        map.SetTile(18, 19, 0, 0, 1);
                        map.SetAttribute(18, 19, Enums.TileType.Walkable, 0, 0, 0, "", "", "");
                        Messenger.SendTile(18, 19, map);
                        map.SetTile(18, 18, 0, 0, 1);
                        map.SetAttribute(18, 18, Enums.TileType.Walkable, 0, 0, 0, "", "", "");
                        Messenger.SendTile(18, 18, map);
                        map.SetTile(18, 17, 0, 0, 1);
                        map.SetAttribute(18, 17, Enums.TileType.Walkable, 0, 0, 0, "", "", "");
                        Messenger.SendTile(18, 17, map);
                        
                	}
                	break;
                case 1007: {//Tanren Mines Ancient Cavern
                        
                        map.SetTile(19, 3, 0, 0, 1);
                        map.SetAttribute(19, 3, Enums.TileType.Walkable, 0, 0, 0, "", "", "");
                        Messenger.SendTile(19, 3, map);
                        
                	}
                	break;
            }
        }

        public static void StartBossWave(Client client, string bossBattle, int wave) {
            switch (bossBattle) {
                case "Registeel": {
                
                        if (wave == 1) { // First wave of Bronzongs
                            IMap map = client.Player.Map;
                            map.Npc[1].SpawnX = 4;
                            map.Npc[1].SpawnY = 5;
                            map.Npc[2].SpawnX = 15;
                            map.Npc[2].SpawnY = 5;
                            map.Npc[1].NpcNum = 305;
                            map.Npc[2].NpcNum = 305;
                            map.Npc[1].MinLevel = 18;
                            map.Npc[2].MinLevel = 18;
                            //map.SpawnNpc(1);
                            //map.SpawnNpc(2);
                        } else if (wave == 2) { // Second wave of Bronzongs
                            IMap map = client.Player.Map;
                            map.Npc[3].SpawnX = 4;
                            map.Npc[3].SpawnY = 12;
                            map.Npc[4].SpawnX = 15;
                            map.Npc[4].SpawnY = 12;
                            map.Npc[3].NpcNum = 305;
                            map.Npc[4].NpcNum = 305;
                            map.Npc[3].MinLevel = 20;
                            map.Npc[4].MinLevel = 20;
                            //map.SpawnNpc(3);
                            //map.SpawnNpc(4);
                        } else if (wave == 3) { // Registeel itself
                            IMap map = client.Player.Map;
                            map.Npc[5].SpawnX = 10;
                            map.Npc[5].SpawnY = 4;
                            map.Npc[5].NpcNum = 304;
                            map.Npc[5].MinLevel = 22;
                            //map.SpawnNpc(5);
                        }
                    }
                    break;
                case "ElectrostasisTowerA1": {
                        if (wave == 1) {
                            IMap map = client.Player.Map;
                            if (map.Name == "Electrostasis Tower Chamber") {
                                int nosepassLevel = 1;
                                map.Npc[1].SpawnX = 8;
                                map.Npc[1].SpawnY = 24;
                                map.Npc[1].NpcNum = 587;
                                map.Npc[1].MinLevel = nosepassLevel;

                                map.Npc[2].SpawnX = 13;
                                map.Npc[2].SpawnY = 22;
                                map.Npc[2].NpcNum = 587;
                                map.Npc[2].MinLevel = nosepassLevel;

                                map.Npc[3].SpawnX = 7;
                                map.Npc[3].SpawnY = 20;
                                map.Npc[3].NpcNum = 587;
                                map.Npc[3].MinLevel = nosepassLevel;

                                map.Npc[4].SpawnX = 15;
                                map.Npc[4].SpawnY = 19;
                                map.Npc[4].NpcNum = 587;
                                map.Npc[4].MinLevel = nosepassLevel;

                                for (int i = 1; i <= 4; i++) {
                                    map.SpawnNpc(map.Npc[i]);
                                    map.ActiveNpc[i].Target = client;
                                }
                            }
                        } else if (wave == 2) {
                            IMap map = client.Player.Map;
                            if (map.Name == "Electrostasis Tower Chamber") {
                                int nosepassLevel = 1;
                                map.Npc[5].SpawnX = 6;
                                map.Npc[5].SpawnY = 6;
                                map.Npc[5].NpcNum = 587;
                                map.Npc[5].MinLevel = nosepassLevel;

                                map.Npc[6].SpawnX = 14;
                                map.Npc[6].SpawnY = 9;
                                map.Npc[6].NpcNum = 587;
                                map.Npc[6].MinLevel = nosepassLevel;

                                map.Npc[7].SpawnX = 9;
                                map.Npc[7].SpawnY = 7;
                                map.Npc[7].NpcNum = 588;
                                map.Npc[7].MinLevel = nosepassLevel;

                                map.SetTile(4, 12, 6, 4, 1);
                                map.SetAttribute(4, 12, Enums.TileType.Blocked, 0, 0, 0, "", "", "");
                                map.SetAttribute(9, 12, Enums.TileType.Walkable, 0, 0, 0, "", "", "");

                                Messenger.SendTile(4, 12, map);
                                Messenger.SendTile(9, 12, map);

                                for (int i = 5; i <= 7; i++) {
                                    //map.SpawnNpc(i);
                                    map.ActiveNpc[i].Target = client;
                                }
                            }
                        }
                    }
                    break;
            }
        }

        public static void EndRegiceBattle(Client client) {
            IMap map = client.Player.Map;
            if (map.Name == "Iceberg Cave") {
                map.SetAttribute(9, 5, Enums.TileType.Warp, 1402, 10, 8, "", "", "");
                map.SetAttribute(10, 5, Enums.TileType.Warp, 1402, 10, 8, "", "", "");
                map.SetTile(9, 5, 224, 3264, 9, 3);
                map.SetTile(10, 5, 256, 3264, 9, 3);
                map.SetTile(9, 4, 224, 3232, 9, 7);
                map.SetTile(10, 4, 256, 3232, 9, 7);
                map.SetTile(9, 3, 224, 3200, 9, 7);
                map.SetTile(10, 3, 256, 3200, 9, 7);
                // Animation layers
                map.SetTile(9, 5, 288, 3264, 9, 4);
                map.SetTile(10, 5, 320, 3264, 9, 4);
                map.SetTile(9, 4, 288, 3232, 9, 8);
                map.SetTile(10, 4, 320, 3232, 9, 8);
                map.SetTile(9, 3, 288, 3200, 9, 8);
                map.SetTile(10, 3, 320, 3200, 9, 8);

                Messenger.SendTile(9, 5, map);
                Messenger.SendTile(10, 5, map);
                Messenger.SendTile(9, 4, map);
                Messenger.SendTile(10, 4, map);
                Messenger.SendTile(9, 3, map);
                Messenger.SendTile(10, 3, map);
                StoryManager.PlayStory(client, 990);
                //ObjectFactory.GetPlayer(index).GiveItem(365, 1);
            }
        }

        public static void EndJirachiBattle(Client client) {
            IMap map = client.Player.Map;
            if (map.Name == "Mt. Moon Summit") {
                map.SetAttribute(9, 3, Enums.TileType.Warp, 915, 3, 6, "", "", "");
                StoryManager.PlayStory(client, 150);
                map.SetTile(9, 3, 96, 0, 4, 1);
                Messenger.SendTile(9, 3, map);
            }
        }

        public static void EndRegisteelBattle(Client client) {
            IMap map = client.Player.Map;
            if (map.Name == "Steel Chamber") {
                map.SetAttribute(9, 4, Enums.TileType.Scripted, 46, 0, 0, "6", "1018", "38:28");
                map.SetAttribute(10, 4, Enums.TileType.Scripted, 46, 0, 0, "6", "1018", "38:28");
                map.SetTile(9, 4, 1435, 9, 3);
                map.SetTile(10, 4, 1436, 9, 3);
                map.SetTile(9, 3, 1421, 9, 7);
                map.SetTile(10, 3, 1422, 9, 7);
                map.SetTile(9, 2, 1407, 9, 7);
                map.SetTile(10, 2, 1408, 9, 7);
                // Animation layers
                map.SetTile(9, 4, 1437, 9, 4);
                map.SetTile(10, 4, 1438, 9, 4);
                map.SetTile(9, 3, 1423, 9, 8);
                map.SetTile(10, 3, 1424, 9, 8);
                map.SetTile(9, 2, 1409, 9, 8);
                map.SetTile(10, 2, 1410, 9, 8);

                Messenger.SendTile(9, 4, map);
                Messenger.SendTile(10, 4, map);
                Messenger.SendTile(9, 3, map);
                Messenger.SendTile(10, 3, map);
                Messenger.SendTile(9, 2, map);
                Messenger.SendTile(10, 2, map);
                StoryManager.PlayStory(client, 57);
            }
        }

        public static void RemoveLuxioTribe(Client client) {
            IMap map = client.Player.Map;
            map.RemoveNpc(1);
            map.RemoveNpc(2);
            map.RemoveNpc(3);
            map.RemoveNpc(4);
            map.RemoveNpc(5);
            map.RemoveNpc(6);
            map.RemoveNpc(7);
            map.SetAttribute(9, 5, Enums.TileType.Warp, 1, 10, 7, "", "", "");
            map.SetAttribute(10, 5, Enums.TileType.Warp, 1, 10, 7, "", "", "");
            map.SetTile(9, 5, 224, 3264, 9, 3);
            map.SetTile(10, 5, 256, 3264, 9, 3);
            map.SetTile(9, 4, 224, 3232, 9, 7);
            map.SetTile(10, 4, 256, 3232, 9, 7);
            map.SetTile(9, 3, 224, 3200, 9, 7);
            map.SetTile(10, 3, 256, 3200, 9, 7);
            // Animation layers
            map.SetTile(9, 5, 288, 3264, 9, 4);
            map.SetTile(10, 5, 320, 3264, 9, 4);
            map.SetTile(9, 4, 288, 3232, 9, 8);
            map.SetTile(10, 4, 320, 3232, 9, 8);
            map.SetTile(9, 3, 288, 3200, 9, 8);
            map.SetTile(10, 3, 320, 3200, 9, 8);

            Messenger.SendTile(9, 5, map);
            Messenger.SendTile(10, 5, map);
            Messenger.SendTile(9, 4, map);
            Messenger.SendTile(10, 4, map);
            Messenger.SendTile(9, 3, map);
            Messenger.SendTile(10, 3, map);
            StoryManager.PlayStory(client, 152);
            //ObjectFactory.GetPlayer(index).GiveItem(365, 1);
        }

    }
}

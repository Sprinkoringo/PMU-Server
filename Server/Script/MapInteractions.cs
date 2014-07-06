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


namespace Script
{
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

    public partial class Main
    {
    

        public static void OnPickupItem(ICharacter character, int itemSlot, InventoryItem invItem) {
            try {
                PacketHitList hitlist = null;
                IMap map = MapManager.RetrieveActiveMap(character.MapID);
                PacketHitList.MethodStart(ref hitlist);

                if (character.CharacterType == Enums.CharacterType.MapNpc) {
                    hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("magic131.wav"), character.X, character.Y, 50);
                } else {
                    if (ItemManager.Items[invItem.Num].Type == Enums.ItemType.Currency) {
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("magic" + ItemManager.Items[invItem.Num].Data1 + ".wav"), character.X, character.Y, 10);
                    } else if (ItemManager.Items[invItem.Num].Type == Enums.ItemType.Held
                        || ItemManager.Items[invItem.Num].Type == Enums.ItemType.HeldByParty
                        || ItemManager.Items[invItem.Num].Type == Enums.ItemType.HeldInBag) {
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("magic148.wav"), character.X, character.Y, 10);
                    } else if (ItemManager.Items[invItem.Num].Type == Enums.ItemType.Key ||
                    	ItemManager.Items[invItem.Num].Type == Enums.ItemType.Scripted &&
                    	(ItemManager.Items[invItem.Num].Data1 == 4 || ItemManager.Items[invItem.Num].Data1 == 43)) {
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("magic681.wav"), character.X, character.Y, 10);
                    } else if (ItemManager.Items[invItem.Num].Type == Enums.ItemType.TM) {
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("magic162.wav"), character.X, character.Y, 10);
                    } else if (ItemManager.Items[invItem.Num].Type == Enums.ItemType.Scripted &&
                    	ItemManager.Items[invItem.Num].Data1 == 12) {
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("magic99.wav"), character.X, character.Y, 10);
                    } else {
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("magic130.wav"), character.X, character.Y, 10);
                    }
                }

                PacketHitList.MethodEnded(ref hitlist);
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: OnPickupItem", Text.Black);
            }
        }

        public static void OnDropItem(ICharacter character, int itemSlot, MapItem mapItem) {
            
            try {
            	PacketHitList hitlist = null;
                IMap map = MapManager.RetrieveActiveMap(character.MapID);
                PacketHitList.MethodStart(ref hitlist);
                
				hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("magic693.wav"), character.X, character.Y, 10);
				
				PacketHitList.MethodEnded(ref hitlist);
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: OnDropItem", Text.Black);
            }
        }

        public static void OnMapTick(IMap map) {
            bool pointReached = false;
            try {
            	
                PacketHitList hitlist = null;
                PacketHitList.MethodStart(ref hitlist);
				
				
                MapStatus status;
                if (!map.ProcessingPaused) {
                    status = map.TempStatus.GetStatus("TrickRoom");
                    if (status != null) {
                        status.Counter--;
                        if (status.Counter <= 0) {
                            RemoveMapStatus(map, "TrickRoom", hitlist);
                        }
                    }
                    status = map.TempStatus.GetStatus("Gravity");
                    if (status != null) {
                        status.Counter--;
                        if (status.Counter <= 0) {
                            RemoveMapStatus(map, "Gravity", hitlist);
                        }
                    }
                    status = map.TempStatus.GetStatus("WonderRoom");
                    if (status != null) {
                        status.Counter--;
                        if (status.Counter <= 0) {
                            RemoveMapStatus(map, "WonderRoom", hitlist);
                        }
                    }
                    status = map.TempStatus.GetStatus("MagicRoom");
                    if (status != null) {
                        status.Counter--;
                        if (status.Counter <= 0) {
                            RemoveMapStatus(map, "MagicRoom", hitlist);
                        }
                    }
                    status = map.TempStatus.GetStatus("Flash");
                    if (status != null) {
                        status.Counter--;
                        if (status.Counter <= 0) {
                            RemoveMapStatus(map, "Flash", hitlist);
                        }
                    }
					
					
                    status = map.TempStatus.GetStatus("Wind");
                    
                    if (status != null) {
                    	
                        status.Counter++;
                        if (map.TimeLimit <= 0) {

                        } else if (status.Counter > map.TimeLimit) {
                        	int victims = 0;
                            
                            foreach (Client client in map.GetClients()) {
                                if (Ranks.IsDisallowed(client, Enums.Rank.Mapper)) {
                                	
                                    HandleGameOver(client, Enums.KillType.Other);
                                    victims++;
                                }
                            }
                            if (victims > 0) {
                            	hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("magic203.wav"));
                            	hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("You were blown out by a mysterious force...", Text.Black));
							}
                        } else if (status.Counter == map.TimeLimit - 6) {
                            hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("magic855.wav"));
                            hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("It's right nearby!  It's gusting hard!", Text.BrightRed));
                            hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleDivider());
                            hitlist.AddPacketToMap(map, PacketBuilder.CreateChatMsg("It's right nearby!  It's gusting hard!", Text.BrightRed));
                        } else if (status.Counter == map.TimeLimit - 60) {
                            hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("magic854.wav"));
                            hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("...It's getting closer!", Text.Blue));
                            hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleDivider());
                            hitlist.AddPacketToMap(map, PacketBuilder.CreateChatMsg("...It's getting closer!", Text.Blue));
                        } else if (status.Counter == map.TimeLimit - 120) {
                            hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("magic579.wav"));
                            hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("Something's approaching...", Text.BrightBlue));
                            hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleDivider());
                            hitlist.AddPacketToMap(map, PacketBuilder.CreateChatMsg("Something's approaching...", Text.BrightBlue));
                        } else if (status.Counter == map.TimeLimit - 240) {
                            hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("magic434.wav"));
                            hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("Something's stirring...", Text.BrightCyan));
                            hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleDivider());
                            hitlist.AddPacketToMap(map, PacketBuilder.CreateChatMsg("Something's stirring...", Text.BrightCyan));
                        }
                    } else if (map.Moral != Enums.MapMoral.House) {
                        AddMapStatus(map, "Wind", 0, "", -1, hitlist);
                    }
					
                    //semi-invulnerable attacks
                    TargetCollection targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Floor, 0, map, null, 0, 0, Enums.Direction.Up, true, true, false);
                    pointReached = true;
                    foreach (ICharacter i in targets.Foes) {
                        ExtraStatus extraStatus = i.VolatileStatus.GetStatus("SemiInvul");
                        if (extraStatus != null) {
                            extraStatus.Counter--;
                            if (extraStatus.Counter <= 0) {
                                BattleSetup setup = new BattleSetup();
                                setup.Attacker = i;
                                setup.moveSlot = -1;

                                BattleProcessor.HandleAttack(setup);


                                BattleProcessor.FinalizeAction(setup, hitlist);
                            }
                        }
                    }
                    

                }
                

                status = map.TempStatus.GetStatus("TotalTime");
                if (status != null) {
                    status.Counter++;
                    if (status.Counter % 16 == 0) {
                    	for (int i = 0; i < Server.Constants.MAX_MAP_ITEMS; i++) {
                			if (map.ActiveItem[i].Num != 0 && map.ActiveItem[i].Hidden) {
		                    	if (Server.Math.Rand(0,5) == 0) {
		                    		foreach (Client n in map.GetClients()) {
		                    			if (Server.AI.MovementProcessor.CanCharacterSeeDestination(map, n.Player.GetActiveRecruit(), map.ActiveItem[i].X, map.ActiveItem[i].Y)) {
		                					hitlist.AddPacketToMap(map, PacketBuilder.CreateSpellAnim(491, map.ActiveItem[i].X, map.ActiveItem[i].Y));
		                					hitlist.AddPacket(n, PacketBuilder.CreateSoundPacket("magic1022.wav"));
		                				}
		                    		}
		                    	}
		                    }
		                }
                    }
                } else {
                    AddMapStatus(map, "TotalTime", 0, "", -1, hitlist);
                }

                PacketHitList.MethodEnded(ref hitlist);
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: OnMapTick", Text.Black);
                Messenger.AdminMsg("Map ID: " + map.MapID, Text.Black);
                Messenger.AdminMsg("Map Name: " + map.Name, Text.Black);
                Messenger.AdminMsg(pointReached.ToString(), Text.Black);
                Messenger.AdminMsg(ex.ToString(), Text.Black);
            }
        }

        public static bool ScriptedCharacterIdentifyCharacter(IMap viewerMap, ICharacter viewer, ICharacter target) {
            if (target != null) {
	            ExtraStatus status = target.VolatileStatus.GetStatus("SemiInvul");
	            if (status != null && status.Tag == "ShadowForce" ||
	            	target.VolatileStatus.GetStatus("Invisible") != null) {
					return false;
				}
			}
			
            return true;
        }

        public static bool ScriptedCharacterSeeCharacter(IMap viewerMap, ICharacter viewer, ICharacter target) {
            if (target != null) {
	            ExtraStatus status = target.VolatileStatus.GetStatus("SemiInvul");
	            if (status != null && status.Tag == "ShadowForce" ||
	            	target.VolatileStatus.GetStatus("Invisible") != null) {
					return false;
				}
			}
			
            return true;
        }

        public static bool ScriptedWillCharacterSee(IMap viewerMap, ICharacter viewer, ICharacter target) {
            if (target != null) {
	            ExtraStatus status = target.VolatileStatus.GetStatus("SemiInvul");
	            if (status != null && status.Tag == "ShadowForce" ||
	            	target.VolatileStatus.GetStatus("Invisible") != null) {
					return false;
				}
			}
			
            return true;
        }

        public static void OnStep(IMap map, ICharacter character, Enums.Speed speed, PacketHitList hitlist) {
            try {
                ExtraStatus status;
                PacketHitList.MethodStart(ref hitlist);
            	if (speed != Enums.Speed.Slip) {
	                int stepCounter;
	                if (character.CharacterType == Enums.CharacterType.Recruit) {
	                    
	                    ((Recruit)character).Owner.Player.HPStepCounter++;
	                    if (((Recruit)character).Owner.Player.Map.HungerEnabled) {
	                        if (character.HasActiveItem(151)) {
	                            ((Recruit)character).Owner.Player.BellyStepCounter += 5;
	                        } else if (character.HasActiveItem(152)) {
	                            //nothing
	                        } else if (character.HasActiveItem(162)) {
	                            ((Recruit)character).Owner.Player.BellyStepCounter += 20;
	                        } else {
	                            ((Recruit)character).Owner.Player.BellyStepCounter += 10;
	                        }
	                        if (HasAbility(character, "Gluttony")) {
	                            ((Recruit)character).Owner.Player.BellyStepCounter += 10;
	                        }
	                    }
	                    stepCounter = ((Recruit)character).Owner.Player.HPStepCounter;
	                    ElectrostasisTower.OnPlayerStep(((Recruit)character).Owner);
	
	                    if (stepCounter % 5 == 0) {
	                        if (map.MapID == MapManager.GenerateMapID(737)) {
	                            bool hasBeenWelcomed = ((Recruit)character).Owner.Player.StoryHelper.ReadSetting("[Gameplay]-HasBeenWelcomed").ToBool();
	                            if (!hasBeenWelcomed) {
	                                hitlist.AddPacketToMap(map, PacketBuilder.CreateChatMsg(((Recruit)character).Owner.Player.Name + " is new to PMU! Welcome!", Text.BrightGreen));
	                                ((Recruit)character).Owner.Player.StoryHelper.SaveSetting("[Gameplay]-HasBeenWelcomed", "True");
	                            }
	                        }
	                    }
	                } else {
	                    ((MapNpc)character).HPStepCounter++;
	                    stepCounter = ((MapNpc)character).HPStepCounter;
	                }
	
	                if (MapManager.IsMapActive(character.MapID)) {
	
	                    RemoveExtraStatus(character, map, "Roost", hitlist);
	                    RemoveExtraStatus(character, map, "Endure", hitlist);
	                    RemoveExtraStatus(character, map, "DestinyBond", hitlist);
                        status = character.VolatileStatus.GetStatus("Rage");
	                    if (status != null) {
	                        status.Counter--;
	                        if (status.Counter <= 0) {
	                            RemoveExtraStatus(character, map, "Rage", hitlist);
	                        }
	                    }
                        status = character.VolatileStatus.GetStatus("MetalBurst");
	                    if (status != null) {
	                        status.Counter--;
	                        if (status.Counter <= 0) {
	                            RemoveExtraStatus(character, map, "MetalBurst", hitlist);
	                        }
	                    }
	                    //if (character.VolatileStatus.GetStatus("Taunt") != null) {
	                    //	character.VolatileStatus.GetStatus("Taunt").Counter--;
	                    //	if (character.VolatileStatus.GetStatus("Taunt").Counter <= 0) {
	                    //		RemoveExtraStatus(character, map, "Taunt", hitlist);
	                    //	}
	                    //}
	
	                    Tile steppedTile = map.Tile[character.X, character.Y];
	                    if (steppedTile.Type == Enums.TileType.MobileBlock) {
	                        if (steppedTile.Data1 % 32 >= 16) {
	                            if (character.CharacterType == Enums.CharacterType.Recruit) {
	                                if (((Recruit)character).Owner.Player.Map.HungerEnabled && character.VolatileStatus.GetStatus("SuperMobile") == null) {
	                                    ((Recruit)character).Owner.Player.BellyStepCounter += 400;
	                                }
	                            }
	                        } else if (steppedTile.Data1 % 4 >= 2) {
	                            if (steppedTile.String1 == "1") {
	                                if (!CheckStatusProtection(character, map, Enums.StatusAilment.Burn.ToString(), false, hitlist)) {
	                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("The water's boiling hot!", Text.BrightRed), character.X, character.Y, 10);
	                                    SetStatusAilment(character, map, Enums.StatusAilment.Burn, 0, hitlist);
	                                }
	                            } else if (steppedTile.String1 == "2") {
	                                if (!CheckStatusProtection(character, map, Enums.StatusAilment.Freeze.ToString(), false, hitlist)) {
	                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("The water's freezing cold!", Text.BrightRed), character.X, character.Y, 10);
	                                    SetStatusAilment(character, map, Enums.StatusAilment.Freeze, 0, hitlist);
	                                }
	                            } else if (steppedTile.String1 == "3") {
	                                if (!CheckStatusProtection(character, map, Enums.StatusAilment.Paralyze.ToString(), false, hitlist)) {
	                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("The water is numbing!", Text.BrightRed), character.X, character.Y, 10);
	                                    SetStatusAilment(character, map, Enums.StatusAilment.Paralyze, 0, hitlist);
	                                }
	                            } else if (steppedTile.String1 == "4") {
	                                if (!CheckStatusProtection(character, map, Enums.StatusAilment.Poison.ToString(), false, hitlist)) {
	                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("The water's poisoned!", Text.BrightRed), character.X, character.Y, 10);
	                                    SetStatusAilment(character, map, Enums.StatusAilment.Poison, 1, hitlist);
	                                }
	                            } else if (steppedTile.String1 == "5") {
	                                if (!CheckStatusProtection(character, map, Enums.StatusAilment.Sleep.ToString(), false, hitlist)) {
	                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("The water is oddly calming...", Text.BrightRed), character.X, character.Y, 10);
	                                    SetStatusAilment(character, map, Enums.StatusAilment.Sleep, 3, hitlist);
	                                }
	                            } else if (character.StatusAilment == Enums.StatusAilment.Burn && !CheckStatusProtection(character, map, Enums.StatusAilment.OK.ToString(), false, hitlist)) {
	                                SetStatusAilment(character, map, Enums.StatusAilment.OK, 0, hitlist);
	                            }
	                        } else if (steppedTile.Data1 % 16 >= 8 && !character.HasActiveItem(805) &&
	                        	!CheckStatusProtection(character, map, Enums.StatusAilment.Burn.ToString(), false, hitlist)) {
	                            SetStatusAilment(character, map, Enums.StatusAilment.Burn, 0, hitlist);
	                        }
	                    }
	
	                }
	
	                if (stepCounter % 5 == 0) {
	                    if (character.HasActiveItem(344) && map.MapType == Enums.MapType.RDungeonMap && Server.Math.Rand(0, 3) == 0) {
	                        RandomWarp(character, map, false, hitlist);
	                    }
	
	                    int newHP = character.HP;
	
	                    //subtract weather damage, sticky items
	                    if (stepCounter % 10 == 0) {
	
	                        //Overcoat
	                        bool overcoat = false;
	
	                        TargetCollection targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 10, map, character, character.X, character.Y, Enums.Direction.Up, false, true, true);
	                        
	                        for (int i = 0; i < targets.Count; i++) {
                                
                                if (HasAbility(targets[i], "Overcoat")) {
                                	
	                                overcoat = true;
	                                break;
	                            }
	                        }
	                        
	
	                        switch (GetCharacterWeather(character)) {
	                            case (Enums.Weather.Hail): {
	                                if (HasAbility(character, "Ice Body") && (character.CharacterType == Enums.CharacterType.MapNpc || ((Recruit)character).Belly > 0)) {
	                                        newHP += 6;
	                                    }
	                                    if (HasAbility(character, "Snow Cloak")) {
	                                    } else if (HasAbility(character, "Ice Body")) {
	                                    } else if (HasAbility(character, "Magic Guard")) {
	                                    } else if (character.Type1 == Enums.PokemonType.Ice || character.Type2 == Enums.PokemonType.Ice) {
	
	                                    } else if (overcoat) {
	                                    } else {
	                                        newHP -= 6;
	                                    }
	                                }
	                                break;
	                            case (Enums.Weather.Sandstorm): {
	                                    if (HasAbility(character, "Magic Guard")) {
	                                    } else if (HasAbility(character, "Sand Veil")) {
	                                    } else if (HasAbility(character, "Sand Force")) {
	                                    } else if (character.Type1 == Enums.PokemonType.Rock || character.Type2 == Enums.PokemonType.Rock) {
	                                    } else if (character.Type1 == Enums.PokemonType.Ground || character.Type2 == Enums.PokemonType.Ground) {
	                                    } else if (character.Type1 == Enums.PokemonType.Steel || character.Type2 == Enums.PokemonType.Steel) {
	                                    } else if (overcoat) {
	                                    } else {
	                                        newHP -= 5;
	                                    }
	                                }
	                                break;
	                            case (Enums.Weather.Sunny): {
	                                    if (HasAbility(character, "Dry Skin") && !overcoat && !HasAbility(character, "Magic Guard")) {
	                                        newHP -= 5;
	                                    }
	                                    if (HasAbility(character, "Solar Power") && !overcoat && !HasAbility(character, "Magic Guard")) {
	                                        newHP -= 5;
	                                    }
	                                }
	                                break;
	                            case (Enums.Weather.Thunder):
	                            case (Enums.Weather.Raining): {
	                                    if (HasAbility(character, "Dry Skin") && (character.CharacterType == Enums.CharacterType.MapNpc || ((Recruit)character).Belly > 0)) {
	                                        newHP += 5;
	                                    }
	                                    if (HasAbility(character, "Rain Dish") && (character.CharacterType == Enums.CharacterType.MapNpc || ((Recruit)character).Belly > 0)) {
	                                        newHP += 5;
	                                    }
	                                }
	                                break;
	                        }
	                        
	                        if (character.HasActiveItem(431)) {
	                        	Sticky(character, map, hitlist);
	                        }
	                    }
	
	                    if (character.CharacterType == Enums.CharacterType.Recruit && ((Recruit)character).Belly <= 0) {
	                        newHP -= 5;
	                        hitlist.AddPacket(((Recruit)character).Owner, PacketBuilder.CreateSoundPacket("magic129.wav"));
	                    }
	
	                    //add HP based on natural healing
	                    int h = 0;
	                    
	                    if ((character.CharacterType == Enums.CharacterType.MapNpc || ((Recruit)character).Belly > 0) && character.StatusAilment != Enums.StatusAilment.Poison) {
	                    	h += 100;
	                    }
	
	                    
	                    if (character.StatusAilment == Enums.StatusAilment.Poison) {
	                        if (HasAbility(character, "Poison Heal") && (character.CharacterType == Enums.CharacterType.MapNpc || ((Recruit)character).Belly > 0)) {
	                            h += (200 * character.StatusAilmentCounter);
	                        } else if (HasAbility(character, "Magic Guard") && (character.CharacterType == Enums.CharacterType.MapNpc || ((Recruit)character).Belly > 0) || (HasAbility(character, "Toxic Boost") && HasAbility(character, "Immunity"))) {
	                            
	                        } else {
	                            h -= (100 * character.StatusAilmentCounter);
	                        }
	                    }
	                    if (character.HasActiveItem(298) && (character.CharacterType == Enums.CharacterType.MapNpc || ((Recruit)character).Belly > 0)) {//heal ribbon
	                        h += 150;
	                    }
	                    if (HasActiveBagItem(character, 12, 0, 0) && (character.CharacterType == Enums.CharacterType.MapNpc || ((Recruit)character).Belly > 0)) {//heal ribbon
	                        h += 150;
	                    }
	                    if (character.HasActiveItem(154)) {
	                    	if (character.Type1 == Enums.PokemonType.Poison || character.Type2 == Enums.PokemonType.Poison) {
	                        	h += 50;
	                        } else {
	                        	h -= 100;
	                        }
	                    }
	
	                    if (h != 0) {
	                        if (character.CharacterType == Enums.CharacterType.Recruit && character.HasActiveItem(298) && newHP < character.MaxHP && ((character.MaxHP + character.HPRemainder) * h / 4000) > 0) {
	                            if (((Recruit)character).Owner.Player.Map.HungerEnabled) {
	                                ((Recruit)character).Owner.Player.BellyStepCounter += 20;
	                            }
	                        }
	                        if (h > 0 && character.VolatileStatus.GetStatus("HealBlock") != null) {
	
	                        } else {
	                            newHP += ((character.MaxHP + character.HPRemainder) * h / 4000);
	                        }
	                        int newRemainder = ((character.MaxHP + character.HPRemainder) % 40);
	                        character.HPRemainder = newRemainder;
	
	                    }
	
	
	                    if (newHP > character.MaxHP) {
	                        newHP = character.MaxHP;
	                    }
	
	
	                    if (newHP != character.HP) {
	                        if (character.CharacterType == Enums.CharacterType.Recruit) {
	
	                            if (newHP <= 0) {
	                                hitlist.AddPacketToMap(map, PacketBuilder.CreateChatMsg("Oh, no!  " + character.Name + " fainted!", Text.BrightRed));
	                                OnDeath(((Recruit)character).Owner, Enums.KillType.Other);
	                            } else {
	                                ((Recruit)character).Owner.Player.SetHP(newHP);
	                            }
	                        } else {
	
	                            if (newHP <= 0) {
	                                OnNpcDeath(hitlist, null, (MapNpc)character);
	                                //((MapNpc)character).Num = 0;
	                                //((MapNpc)character).HP = 0;
	                                MapManager.RetrieveActiveMap(character.MapID).ActiveNpc[((MapNpc)character).MapSlot] = new MapNpc(character.MapID, ((MapNpc)character).MapSlot);
	
	                                hitlist.AddPacketToMap(map, TcpPacket.CreatePacket("npcdead", ((MapNpc)character).MapSlot));
	                            } else {
	                                character.HP = newHP;
	                            }
	                        }
	
	                    }
	
	                    if ((character.CharacterType == Enums.CharacterType.MapNpc || ((Recruit)character).Belly > 0) && character.VolatileStatus.GetStatus("AquaRing") != null) {
	                        int heal = character.MaxHP / 32;
	                        if (heal < 1) heal = 1;
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s health is restored by Aqua Ring!", Text.BrightGreen), character.X, character.Y, 10);
	                        HealCharacter(character, map, heal, hitlist);
	                    }
	
                        //if (character.VolatileStatus.GetStatus("LeechSeed:0") != null && !HasAbility(character, "Magic Guard")) {
                        //    int seeddmg = character.MaxHP / 32;
                        //    if (seeddmg < 1) seeddmg = 1;
                        //    int heal = 5;
                        //    if (character.VolatileStatus.GetStatus("LeechSeed:0").Target.HasActiveItem(194)) heal = 6;
                        //    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s health is sapped by Leech Seed!", Text.BrightRed), character.X, character.Y, 10);
                        //    DamageCharacter(character, map, seeddmg, Enums.KillType.Other, hitlist, true);
                        //    if (HasAbility(character, "Liquid Ooze")) {
                        //        hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.VolatileStatus.GetStatus("LeechSeed:0").Target.Name + " sucked up the Liquid Ooze!", Text.BrightRed), character.X, character.Y, 10);
                        //        DamageCharacter(character.VolatileStatus.GetStatus("LeechSeed:0").Target, MapManager.RetrieveActiveMap(character.VolatileStatus.GetStatus("LeechSeed:0").Target.MapID), seeddmg, Enums.KillType.Other, hitlist, true);
                        //    } else {
                        //        HealCharacter(character.VolatileStatus.GetStatus("LeechSeed:0").Target, MapManager.RetrieveActiveMap(character.VolatileStatus.GetStatus("LeechSeed:0").Target.MapID), seeddmg * heal / 5, hitlist);
                        //    }
                        //}
	
	                }
                    status = character.VolatileStatus.GetStatus("Confusion");
	                if (status != null) {
	                    status.Counter--;
	                    if (status.Counter <= 0) {
	                        RemoveExtraStatus(character, map, "Confusion", hitlist);
	                    }
	                }
                    status = character.VolatileStatus.GetStatus("Invisible");
	                if (status != null) {
	                    status.Counter--;
	                    if (status.Counter <= 0) {
	                        RemoveExtraStatus(character, map, "Invisible", hitlist);
	                    }
	                }
                    status = character.VolatileStatus.GetStatus("Blind");
	                if (status != null) {
	                    status.Counter--;
	                    if (status.Counter <= 0) {
	                        RemoveExtraStatus(character, map, "Blind", hitlist);
	                    }
	                }
                    status = character.VolatileStatus.GetStatus("Shocker");
	                if (status != null) {
	                    status.Counter--;
	                    if (status.Counter <= 0) {
	                        RemoveExtraStatus(character, map, "Shocker", hitlist);
	                    }
	                }
	
	                //if (character.CharacterType == Enums.CharacterType.Recruit) {
	                //    if (((Recruit)character).Owner.Player.Confusion != 0) {
	                //        ((Recruit)character).Owner.Player.Confusion--;
	                //        if (((Recruit)character).Owner.Player.Confusion <= 0) {
	                //            ((Recruit)character).Owner.Player.Confusion = 1;
	                //            if (!CheckStatusProtection(character, map, "Confusion:0", false, hitlist)) {
	                //                Confuse(character, map, 0, hitlist);
	                //            }
	                //        }
	                //    }
	                //} else {
	                //    if (((MapNpc)character).Confused != 0) {
	                //        ((MapNpc)character).Confused--;
	                //        if (((MapNpc)character).Confused <= 0) {
	                //            ((MapNpc)character).Confused = 1;
	                //            if (!CheckStatusProtection(character, map, "Confusion:0", false, hitlist)) {
	                //                Confuse(character, map, 0, hitlist);
	                //            }
	                //        }
	                //    }
	                //}
	
	                if (character.CharacterType == Enums.CharacterType.Recruit) {
	                    ((Recruit)character).Owner.Player.ProcessHunger(hitlist);
	                }
	
	                if (character.CharacterType == Enums.CharacterType.Recruit) {
	                    Client clientOwner = ((Recruit)character).Owner;
	                    StoryHelper.OnStep(clientOwner, hitlist);
	                    if (stepCounter % 10 == 0) {
	                        EggStep(clientOwner);
	                    }
	
	                }
                }
                PacketHitList.MethodEnded(ref hitlist);

            } catch (Exception ex) {
                Messenger.AdminMsg("Error: OnStep", Text.Black);
            }
            
        }

        

        public static bool WillTrapActivate(ICharacter character, IMap map, int x, int y) {
            Tile tile = map.Tile[x, y];
            bool willActivate = true;

            if (character.CharacterType == Enums.CharacterType.MapNpc) {
                if (tile.String1 == "") willActivate = false;

                if (NpcManager.Npcs[((MapNpc)character).Num].Behavior == Enums.NpcBehavior.Friendly) willActivate = false;

            } else if (character.CharacterType == Enums.CharacterType.Recruit) {
                if (((Recruit)character).Owner.Player.CharID == tile.String1) {
                    willActivate = false;
                } else if (!string.IsNullOrEmpty(((Recruit)character).Owner.Player.GuildName) && ((Recruit)character).Owner.Player.GuildName == tile.String2) {
                    willActivate = false;
                } else if (((Recruit)character).Owner.Player.IsInParty()) {
                    Server.Players.Parties.Party party = PartyManager.FindPlayerParty(((Recruit)character).Owner);
                    if (party != null) {
                        if (party.IsPlayerInParty(tile.String1)) {
                            willActivate = false;
                        }
                    }
                }

                //if (((Recruit)character).Owner.Player.PK) willActivate = true;
            }

            if (character.HasActiveItem(142)) willActivate = false;

            return willActivate;
        }

        public static void ActivateTrap(IMap map, int x, int y, int script, PacketHitList hitlist) {
            PacketHitList.MethodStart(ref hitlist);
            
            if (script != 7 && script != 26 && Server.Math.Rand(0, 5) == 0 && map.Tile[x, y].Data3 < 1) {
            	hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("magic126.wav"), x, y, 10);
            	hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("The trap failed to work!", Text.Blue), x, y, 10);
            } else {
	            
	            TargetCollection targets;
	
	            switch (script) {
	                case 2: {//EXPLOSIONNNNN
	                		//Damp
	                		bool explode = true;
				            
				            	TargetCollection checkedTargets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Floor, 0, map, null, x, y, Enums.Direction.Up, true, true, false);
								for (int i = 0; i < checkedTargets.Count; i++) {
				                    if (HasAbility(checkedTargets[i], "Damp")) {
				                        explode = false;
				                    }
				                }
				            
				            if (explode) {
		                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("magic215.wav"), x, y, 10);
		                        for (int i = x - 2; i <= x + 2; i++) {
		                            for (int j = y - 2; j <= y + 2; j++) {
		                                if (i < 0 || j < 0 || i > map.MaxX || j > map.MaxY) {
		
		                                } else {
		                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateSpellAnim(497, i, j));
		                                }
		                            }
		                        }
		                        targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 2, map, null, x, y, Enums.Direction.Up, true, true, true);
                                for (int i = 0; i < targets.Count; i++) {
                                	if (HasActiveBagItem(targets[i], 6, 0, 0)) {
                                		DamageCharacter(targets[i], MapManager.RetrieveActiveMap(targets[i].MapID), 1, Enums.KillType.Tile, hitlist, true);
                                	} else {
                                    	DamageCharacter(targets[i], MapManager.RetrieveActiveMap(targets[i].MapID), targets[i].HP / 2, Enums.KillType.Tile, hitlist, true);
                                    }
                                }
	                        } else {
	                        	hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("magic320.wav"), x, y, 10);
	                        	hitlist.AddPacketToMap(map, PacketBuilder.CreateSpellAnim(505, x, y));
	                        	hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("The damp conditions prevented an explosion!", Text.Blue), x, y, 10);
	                        }
	                    }
	                    break;
	                case 3: {//chestnut
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("magic218.wav"), x, y, 10);
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSpellAnim(551, x, y));
	                        targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 0, map, null, x, y, Enums.Direction.Up, true, true, false);
                            for (int i = 0; i < targets.Count; i++) {
                                hitlist.AddPacketToMap(MapManager.RetrieveActiveMap(targets[i].MapID), PacketBuilder.CreateBattleMsg(targets[i].Name + " was pelted by chestnuts!", Text.BrightRed), x, y, 10);
                                DamageCharacter(targets[i], MapManager.RetrieveActiveMap(targets[i].MapID), 15, Enums.KillType.Tile, hitlist, true);
	                        }
	                    }
	                    break;
	                case 4: {//ppzero
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("magic179.wav"), x, y, 10);
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSpellAnim(493, x, y));
	                        targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 0, map, null, x, y, Enums.Direction.Up, true, true, false);
                            for (int i = 0; i < targets.Count; i++) {
	                            int moveSlot = Server.Math.Rand(0, 4);
                                if (targets[i].Moves[moveSlot].MoveNum > -1 && targets[i].Moves[moveSlot].CurrentPP > 0) {
                                    targets[i].Moves[moveSlot].CurrentPP = 0;
                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(targets[i].Name + "'s move " + MoveManager.Moves[targets[i].Moves[moveSlot].MoveNum].Name + " lost all of its PP!", Text.BrightRed), x, y, 10);
                                    if (targets[i].CharacterType == Enums.CharacterType.Recruit) {
                                        PacketBuilder.AppendMovePPUpdate(((Recruit)targets[i]).Owner, hitlist, moveSlot);
	                                }
	                            } else {
                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("Nothing particularly bad happened to " + targets[i].Name + "...", Text.Blue), x, y, 10);
	                            }
	                        }
	                    }
	                    break;
	                case 5: {//grimy
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("Magic220.wav"), x, y, 10);
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSpellAnim(494, x, y));
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("Sludge splashed up from the ground!", Text.BrightRed), x, y, 10);
                            targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 0, map, null, x, y, Enums.Direction.Up, true, true, false);
                            for (int i = 0; i < targets.Count; i++) {
                                Grimy(targets[i], MapManager.RetrieveActiveMap(targets[i].MapID), hitlist);
	                        }
	                    }
	                    break;
	                case 6: {//poison
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("Magic221.wav"), x, y, 10);
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSpellAnim(496, x, y));
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("Poison spikes shot out!", Text.BrightRed), x, y, 10);
                            targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 0, map, null, x, y, Enums.Direction.Up, true, true, false);
                            for (int i = 0; i < targets.Count; i++) {
                                SetStatusAilment(targets[i], MapManager.RetrieveActiveMap(targets[i].MapID), Enums.StatusAilment.Poison, 1, hitlist);
	                        }
	                    }
	                    break;
	                case 7: {//random
	                		string trapString = "";
	                		if (map.Tile[x, y].String3.Contains(";")) {
	                			trapString = map.Tile[x, y].String3;
	                		} else {
	                			trapString = "2;3;4;5;6;15;16;17;18;19;23;25;26;28;39;42;43;44;49;50;51;52;53;70";
	                		}
	                		string[] trapSelection = trapString.Split(';');
	                        string chosenTrap = trapSelection[Server.Math.Rand(0, trapSelection.Length)];
	                        if (chosenTrap.IsNumeric() && chosenTrap.ToInt() != 7) {
                                targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Floor, 0, map, null, x, y, Enums.Direction.Up, true, true, false);
                                for (int i = 0; i < targets.Count; i++) {
                                    if (i != null && WillTrapActivate(targets[i], map, x, y)) {
                                        ActivateTrap(MapManager.RetrieveActiveMap(targets[i].MapID), targets[i].X, targets[i].Y, chosenTrap.ToInt(), hitlist);
	                                }
	                            }
	                        } else {
	                            hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("But nothing happened...", Text.Blue), x, y, 10);
	                        }
	                    }
	                    break;
	                case 15: {//warp
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("magic256.wav"), x, y, 10);
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSpellAnim(555, x, y));
                            targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 0, map, null, x, y, Enums.Direction.Up, true, true, false);
                            for (int i = 0; i < targets.Count; i++) {
                                RandomWarp(targets[i], MapManager.RetrieveActiveMap(targets[i].MapID), true, hitlist);
	                        }
	                    }
	                    break;
	                case 16: {//pokemon
	                        //hitlist.AddPacketToMap(mapID, PacketBuilder.CreateSoundPacket(), x, y, 10);
	                        //hitlist.AddPacketToMap(mapID, PacketBuilder.CreateSpellAnim(552, x, y));
	                        //replace all nearby items with Pokemon
	                    }
	                    break;
	                case 17: {//spikes
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("magic122.wav"), x, y, 10);
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSpellAnim(553, x, y));
                            targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 0, map, null, x, y, Enums.Direction.Up, true, true, false);
                            for (int i = 0; i < targets.Count; i++) {
	                            if (map.Tile[x, y].String3.IsNumeric()) {
                                    DamageCharacter(targets[i], MapManager.RetrieveActiveMap(targets[i].MapID), targets[i].MaxHP * map.Tile[x, y].String3.ToInt() / 8, Enums.KillType.Tile, hitlist, true);
	                            } else {
                                    DamageCharacter(targets[i], MapManager.RetrieveActiveMap(targets[i].MapID), targets[i].MaxHP * Server.Math.Rand(1, 4) / 8, Enums.KillType.Tile, hitlist, true);
	                            }
	                        }
	                    }
	                    break;
	                case 18: {//toxic spikes
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("magic122.wav"), x, y, 10);
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSpellAnim(554, x, y));
                            targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 0, map, null, x, y, Enums.Direction.Up, true, true, false);
                            for (int i = 0; i < targets.Count; i++) {
                                DamageCharacter(targets[i], MapManager.RetrieveActiveMap(targets[i].MapID), 10, Enums.KillType.Tile, hitlist, true);
	                            if (map.Tile[x, y].String3.IsNumeric()) {
                                    SetStatusAilment(targets[i], MapManager.RetrieveActiveMap(targets[i].MapID), Enums.StatusAilment.Poison, map.Tile[x, y].String3.ToInt(), hitlist);
	                            } else {
                                    SetStatusAilment(targets[i], MapManager.RetrieveActiveMap(targets[i].MapID), Enums.StatusAilment.Poison, Server.Math.Rand(1, 3), hitlist);
	                            }
	                        }
	                    }
	                    break;
	                case 19: {//stealth rock
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("magic122.wav"), x, y, 10);
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSpellAnim(556, x, y));
                            targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 0, map, null, x, y, Enums.Direction.Up, true, true, false);
                            for (int i = 0; i < targets.Count; i++) {
                                int HPFraction = DamageCalculator.Effectiveness[DamageCalculator.CalculateTypeMatchup(Enums.PokemonType.Rock, targets[i].Type1) + DamageCalculator.CalculateTypeMatchup(Enums.PokemonType.Rock, targets[i].Type2)];
                                DamageCharacter(targets[i], MapManager.RetrieveActiveMap(targets[i].MapID), targets[i].MaxHP * HPFraction / 1280, Enums.KillType.Tile, hitlist, true);
	                        }
	                    }
	                    break;
	                case 23: {//sticky
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("magic159.wav"), x, y, 10);
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSpellAnim(494, x, y));
                            targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 0, map, null, x, y, Enums.Direction.Up, true, true, false);
                            for (int i = 0; i < targets.Count; i++) {
                                Sticky(targets[i], MapManager.RetrieveActiveMap(targets[i].MapID), hitlist);
	                        }
	                    }
	                    break;
	                case 25: {//mud
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("magic473.wav"), x, y, 10);
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSpellAnim(495, x, y));
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("Mud splashed up from the ground!", Text.BrightRed), x, y, 10);
                            targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 0, map, null, x, y, Enums.Direction.Up, true, true, false);
                            for (int i = 0; i < targets.Count; i++) {
	                            switch (Server.Math.Rand(0, 7)) {
	                                case 0: {
                                        ChangeAttackBuff(targets[i], MapManager.RetrieveActiveMap(targets[i].MapID), -3, hitlist);
	                                    }
	                                    break;
	                                case 1: {
                                        ChangeDefenseBuff(targets[i], MapManager.RetrieveActiveMap(targets[i].MapID), -3, hitlist);
	                                    }
	                                    break;
	                                case 2: {
                                        ChangeSpAtkBuff(targets[i], MapManager.RetrieveActiveMap(targets[i].MapID), -3, hitlist);
	                                    }
	                                    break;
	                                case 3: {
                                        ChangeSpDefBuff(targets[i], MapManager.RetrieveActiveMap(targets[i].MapID), -3, hitlist);
	                                    }
	                                    break;
	                                case 4: {
                                        ChangeSpeedBuff(targets[i], MapManager.RetrieveActiveMap(targets[i].MapID), -3, hitlist);
	                                    }
	                                    break;
	                                case 5: {
                                        ChangeAccuracyBuff(targets[i], MapManager.RetrieveActiveMap(targets[i].MapID), -3, hitlist);
	                                    }
	                                    break;
	                                case 6: {
                                        ChangeEvasionBuff(targets[i], MapManager.RetrieveActiveMap(targets[i].MapID), -3, hitlist);
	                                    }
	                                    break;
	                            }
	                        }
	                    }
	                    break;
	                case 26: {//wonder tile
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("Magic121.wav"), x, y, 10);
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSpellAnim(499, x, y));
                            targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 0, map, null, x, y, Enums.Direction.Up, true, true, false);
                            for (int i = 0; i < targets.Count; i++) {
                                RemoveBuffs(targets[i]);
                                hitlist.AddPacketToMap(MapManager.RetrieveActiveMap(targets[i].MapID), PacketBuilder.CreateBattleMsg(targets[i].Name + "'s stats were returned to normal!", Text.WhiteSmoke), x, y, 10);
	                        }
	                    }
	                    break;
	                case 28: {//trip
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("Magic172.wav"), x, y, 10);
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSpellAnim(557, x, y));
                            targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 0, map, null, x, y, Enums.Direction.Up, true, true, false);
                            for (int i = 0; i < targets.Count; i++) {
                                hitlist.AddPacketToMap(MapManager.RetrieveActiveMap(targets[i].MapID), PacketBuilder.CreateBattleMsg(targets[i].Name + " tripped!", Text.BrightRed), x, y, 10);
                                InvDrop(targets[i], map, hitlist);
	                        }
	                    }
	                    break;
	                case 34: {//secret (Stairs, etc), not used here
	
	                    }
	                    break;
	                case 39: {//pitfall
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("DeepFall.wav"), x, y, 10);
	                        //hitlist.AddPacketToMap(mapID, PacketBuilder.CreateSpellAnim(494, x, y));
                            targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 0, map, null, x, y, Enums.Direction.Up, true, true, false);
                            for (int i = 0; i < targets.Count; i++) {
                                if (targets[i].CharacterType == Enums.CharacterType.Recruit) {
                                    Client client = ((Recruit)targets[i]).Owner;
	                                string[] trapData = map.Tile[x, y].String3.Split(';');
	                                try {
	                                    if (map.Tile[x, y].String3 == "" && map.MapType == Enums.MapType.RDungeonMap) {//fall down a level in a random dungeon
	                                        if (RDungeonManager.RDungeons[((RDungeonMap)map).RDungeonIndex].Direction == Enums.Direction.Up) {//fall down a level
	                                            client.Player.WarpToRDungeon(((RDungeonMap)map).RDungeonIndex, ((RDungeonMap)map).RDungeonFloor - 1);
	                                        } else {//fall up a level
	                                            client.Player.WarpToRDungeon(((RDungeonMap)map).RDungeonIndex, ((RDungeonMap)map).RDungeonFloor + 1);
	                                        }
	                                    } else {//fall down specific coordinates
                                            hitlist.AddPacketToMap(MapManager.RetrieveActiveMap(targets[i].MapID), PacketBuilder.CreateBattleMsg(targets[i].Name + " fell through!", Text.BrightRed), x, y, 10);
	                                        Messenger.PlayerWarp(client, trapData[0].ToInt(), trapData[1].ToInt(), trapData[2].ToInt());
	                                    }
                                        DamageCharacter(targets[i], MapManager.RetrieveActiveMap(targets[i].MapID), 10, Enums.KillType.Tile, hitlist, true);
	                                } catch {
                                        hitlist.AddPacketToMap(MapManager.RetrieveActiveMap(targets[i].MapID), PacketBuilder.CreateChatMsg("This is a bad trap.  Report this to an admin.", Text.Black), x, y, 10);
	                                }
                                } else if (targets[i].CharacterType == Enums.CharacterType.MapNpc) {
                                    hitlist.AddPacketToMap(MapManager.RetrieveActiveMap(targets[i].MapID), PacketBuilder.CreateBattleMsg(targets[i].Name + " fell through!", Text.BrightRed), x, y, 10);
	                                //((MapNpc)i).Num = 0;
	                                //((MapNpc)i).HP = 0;
                                    MapManager.RetrieveActiveMap(targets[i].MapID).ActiveNpc[((MapNpc)targets[i]).MapSlot] = new MapNpc(targets[i].MapID, ((MapNpc)targets[i]).MapSlot);
                                    hitlist.AddPacketToMap(MapManager.RetrieveActiveMap(targets[i].MapID), TcpPacket.CreatePacket("npcdead", ((MapNpc)targets[i]).MapSlot));
	                            }
	                        }
	                    }
	                    break;
	                case 42: {//seal
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("Magic275.wav"), x, y, 10);
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSpellAnim(558, x, y));
                            targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 0, map, null, x, y, Enums.Direction.Up, true, true, false);
                            for (int i = 0; i < targets.Count; i++) {
	                            int rand = Server.Math.Rand(0, 4);
                                AddExtraStatus(targets[i], MapManager.RetrieveActiveMap(targets[i].MapID), "MoveSeal:" + rand, 0, null, "", hitlist);
	                        }
	                    }
	                    break;
	                case 43: {//slow
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("Magic180.wav"), x, y, 10);
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSpellAnim(559, x, y));
                            targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 0, map, null, x, y, Enums.Direction.Up, true, true, false);
                            for (int i = 0; i < targets.Count; i++) {
                                ExtraStatus status = targets[i].VolatileStatus.GetStatus("MovementSpeed");
                                if (status != null) {
                                    AddExtraStatus(targets[i], MapManager.RetrieveActiveMap(targets[i].MapID), "MovementSpeed", status.Counter - 1, null, "", hitlist);
	                            } else {
                                    AddExtraStatus(targets[i], MapManager.RetrieveActiveMap(targets[i].MapID), "MovementSpeed", -1, null, "", hitlist);
	                            }
	                            //AddExtraStatus(i, MapManager.RetrieveActiveMap(i.MapID), "MovementSpeed", (int)GetSpeedLimit(i) - 1, null, hitlist);
	                        }
	                    }
	                    break;
	                case 44: {//spin
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("Magic198.wav"), x, y, 10);
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSpellAnim(560, x, y));
                            targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 0, map, null, x, y, Enums.Direction.Up, true, true, false);
                            for (int i = 0; i < targets.Count; i++) {
                                hitlist.AddPacketToMap(MapManager.RetrieveActiveMap(targets[i].MapID), PacketBuilder.CreateBattleMsg(targets[i].Name + " spun around and around...", Text.BrightRed), x, y, 10);
                                AddExtraStatus(targets[i], MapManager.RetrieveActiveMap(targets[i].MapID), "Confusion", 15, null, "", hitlist);
                                //Confuse(i, MapManager.RetrieveActiveMap(i.MapID), 15, hitlist);
	                        }
	                    }
	                    break;
	                case 49: {//summon
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("Magic197.wav"), x, y, 10);
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSpellAnim(561, x, y));
	                        //hitlist.AddPacketToMap(map, PacketBuilder.CreateSpellAnim(492, x, y));
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("A sweet scent wafted out from the ground!", Text.BrightRed), x, y, 10);
	                        for (int i = 0; i < Constants.MAX_MAP_NPCS / 4; i++) {
	                        	if (Server.Math.Rand(0,2) == 0) {
	                            	SpawnNpcInRange(map, x, y, 2, hitlist);
	                            } else {
	                            	map.SpawnNpc();
	                            }
	                        }
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("Wild Pokémon were attracted to the scent!", Text.BrightRed), x, y, 10);
	                    }
	                    break;
	                case 50: {//grudge
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("Magic468.wav"), x, y, 10);
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSpellAnim(562, x, y));
	                        //hitlist.AddPacketToMap(mapID, PacketBuilder.CreateSpellAnim(492, x, y));
	                        for (int i = 0; i < Constants.MAX_MAP_NPCS / 4; i++) {
	                            SpawnNpcInRange(map, x, y, 2, hitlist);
	                        }
	                        for (int i = 0; i < Constants.MAX_MAP_NPCS; i++) {
	                            if (map.ActiveNpc[i].Num > 0) {
                                    AddExtraStatus(map.ActiveNpc[i], map, "Grudge", 0, null, "", hitlist);
	                            }
	                        }
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("All Pokémon on the floor now hold a grudge!", Text.BrightRed), x, y, 10);
	                    }
	                    break;
	                case 51: {//selfdestruct
	                		bool explode = true;
				            
				            	TargetCollection checkedTargets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Floor, 0, map, null, x, y, Enums.Direction.Up, true, true, false);
								for (int i = 0; i < checkedTargets.Count; i++) {
				                    if (HasAbility(checkedTargets[i], "Damp")) {
				                        explode = false;
				                    }
				                }
				            
				            if (explode) {
		                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("magic216.wav"), x, y, 10);
			                        for (int i = x - 1; i <= x + 1; i++) {
			                            for (int j = y - 1; j <= y + 1; j++) {
			                                if (i < 0 || j < 0 || i > map.MaxX || j > map.MaxY) {
			
			                                } else {
			                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateSpellAnim(497, i, j));
			                                }
			                            }
			                        }
                                    targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 1, map, null, x, y, Enums.Direction.Up, true, true, false);
                                    for (int i = 0; i < targets.Count; i++) {
                                    	if (HasActiveBagItem(targets[i], 6, 0, 0)) {
                                			DamageCharacter(targets[i], MapManager.RetrieveActiveMap(targets[i].MapID), 1, Enums.KillType.Tile, hitlist, true);
                                		} else {
                                        	DamageCharacter(targets[i], MapManager.RetrieveActiveMap(targets[i].MapID), targets[i].MaxHP / 4, Enums.KillType.Tile, hitlist, true);
                                        }
			                        }
		                    } else {
	                        	hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("magic320.wav"), x, y, 10);
	                        	hitlist.AddPacketToMap(map, PacketBuilder.CreateSpellAnim(505, x, y));
	                        	hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("The damp conditions prevented an explosion!", Text.Blue), x, y, 10);
	                        }
	                    }
	                    break;
	                case 52: {//slumber
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("Magic217.wav"), x, y, 10);
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSpellAnim(492, x, y));
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("Sleeping gas erupted from the ground!", Text.BrightRed), x, y, 10);
	                        targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 0, map, null, x, y, Enums.Direction.Up, true, true, false);
                            for (int i = 0; i < targets.Count; i++) {
                                SetStatusAilment(targets[i], MapManager.RetrieveActiveMap(targets[i].MapID), Enums.StatusAilment.Sleep, 8, hitlist);
	                        }
	                    }
	                    break;
	                case 53: {//gust
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("Magic199.wav"), x, y, 10);
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSpellAnim(564, x, y));
	                        targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 0, map, null, x, y, Enums.Direction.Up, true, true, false);
                            for (int i = 0; i < targets.Count; i++) {

                                BlowBack(targets[i], map, (Enums.Direction)Server.Math.Rand(0, 4), hitlist);
	                        }
	                    }
	                    break;
	                case 70: {//shocker
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("Magic356.wav"), x, y, 10);
	                        hitlist.AddPacketToMap(map, PacketBuilder.CreateSpellAnim(538, x, y));
                            targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 0, map, null, x, y, Enums.Direction.Up, true, true, false);
                            for (int i = 0; i < targets.Count; i++) {
                                AddExtraStatus(targets[i], MapManager.RetrieveActiveMap(targets[i].MapID), "Shocker", 30, null, "", hitlist);
	                        }
	                    }
	                    break;
	            }
	        }
            PacketHitList.MethodEnded(ref hitlist);

        }

        public static void RevealTrap(IMap map, int x, int y, PacketHitList hitlist) {
            PacketHitList.MethodStart(ref hitlist);
            map.Tile[x, y].Data2++;
            if (map.Tile[x, y].Data3 == 1) {//fake stairs
            	hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("It's not a stairway!  It's a trap!", Text.BrightRed), x, y, 10);
            }
            switch (map.Tile[x, y].Data1) {
                case 2: {//EXPLOSIONNNNN
                        map.Tile[x, y].Mask2 = 3995;
                        map.Tile[x, y].Mask2Set = 4;
                        map.TempChange = true;
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateTilePacket(x, y, map));
                    }
                    break;
                case 3: {//chestnut
                        map.Tile[x, y].Mask2 = 3997;
                        map.Tile[x, y].Mask2Set = 4;
                        map.TempChange = true;
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateTilePacket(x, y, map));
                    }
                    break;
                case 4: {//ppzero
                        map.Tile[x, y].Mask2 = 4000;
                        map.Tile[x, y].Mask2Set = 4;
                        map.TempChange = true;
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateTilePacket(x, y, map));
                    }
                    break;
                case 5: {//grimy
                        map.Tile[x, y].Mask2 = 3999;
                        map.Tile[x, y].Mask2Set = 4;
                        map.TempChange = true;
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateTilePacket(x, y, map));
                    }
                    break;
                case 6: {//poison
                        map.Tile[x, y].Mask2 = 3993;
                        map.Tile[x, y].Mask2Set = 4;
                        map.TempChange = true;
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateTilePacket(x, y, map));
                    }
                    break;
                case 7: {//random
                        map.Tile[x, y].Mask2 = 4011;
                        map.Tile[x, y].Mask2Set = 4;
                        map.TempChange = true;
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateTilePacket(x, y, map));
                    }
                    break;
                case 15: {//warp
                        map.Tile[x, y].Mask2 = 3;
                        map.Tile[x, y].Mask2Set = 4;
                        map.TempChange = true;
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateTilePacket(x, y, map));
                    }
                    break;
                case 16: {//pokemon
                        map.Tile[x, y].Mask2 = 4007;
                        map.Tile[x, y].Mask2Set = 4;
                        map.TempChange = true;
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateTilePacket(x, y, map));
                    }
                    break;
                case 17: {//spikes
                        map.Tile[x, y].Mask2 = 4009;
                        map.Tile[x, y].Mask2Set = 4;
                        map.TempChange = true;
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateTilePacket(x, y, map));
                    }
                    break;
                case 18: {//toxic spikes
                        map.Tile[x, y].Mask2 = 4008;
                        map.Tile[x, y].Mask2Set = 4;
                        map.TempChange = true;
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateTilePacket(x, y, map));
                    }
                    break;
                case 19: {//stealth rock
                        map.Tile[x, y].Mask2 = 4010;
                        map.Tile[x, y].Mask2Set = 4;
                        map.TempChange = true;
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateTilePacket(x, y, map));
                    }
                    break;
                case 23: {//sticky
                        map.Tile[x, y].Mask2 = 3998;
                        map.Tile[x, y].Mask2Set = 4;
                        map.TempChange = true;
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateTilePacket(x, y, map));
                    }
                    break;
                case 25: {//mud
                        map.Tile[x, y].Mask2 = 3991;
                        map.Tile[x, y].Mask2Set = 4;
                        map.TempChange = true;
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateTilePacket(x, y, map));
                    }
                    break;
                case 26: {//wonder tile
                        map.Tile[x, y].Mask2 = 4;
                        map.Tile[x, y].Mask2Set = 4;
                        map.TempChange = true;
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateTilePacket(x, y, map));
                    }
                    break;
                case 28: {//trip
                        map.Tile[x, y].Mask2 = 4005;
                        map.Tile[x, y].Mask2Set = 4;
                        map.TempChange = true;
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateTilePacket(x, y, map));
                    }
                    break;
                case 34: {//secret (Stairs, etc)
                        map.Tile[x, y].Mask2 = 0;
                        map.Tile[x, y].Mask2Set = 0;
                        map.TempChange = true;
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateTilePacket(x, y, map));
                    }
                    break;
                case 39: {//pitfall
                        map.Tile[x, y].Mask2 = 3996;
                        map.Tile[x, y].Mask2Set = 4;
                        map.TempChange = true;
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateTilePacket(x, y, map));
                    }
                    break;
                case 42: {//seal
                        map.Tile[x, y].Mask2 = 4004;
                        map.Tile[x, y].Mask2Set = 4;
                        map.TempChange = true;
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateTilePacket(x, y, map));
                    }
                    break;
                case 43: {//slow
                        map.Tile[x, y].Mask2 = 4006;
                        map.Tile[x, y].Mask2Set = 4;
                        map.TempChange = true;
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateTilePacket(x, y, map));
                    }
                    break;
                case 44: {//spin
                        map.Tile[x, y].Mask2 = 4001;
                        map.Tile[x, y].Mask2Set = 4;
                        map.TempChange = true;
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateTilePacket(x, y, map));
                    }
                    break;
                case 49: {//summon
                        map.Tile[x, y].Mask2 = 4003;
                        map.Tile[x, y].Mask2Set = 4;
                        map.TempChange = true;
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateTilePacket(x, y, map));
                    }
                    break;
                case 50: {//grudge
                        map.Tile[x, y].Mask2 = 4012;
                        map.Tile[x, y].Mask2Set = 4;
                        map.TempChange = true;
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateTilePacket(x, y, map));
                    }
                    break;
                case 51: {//selfdestruct
                        map.Tile[x, y].Mask2 = 3994;
                        map.Tile[x, y].Mask2Set = 4;
                        map.TempChange = true;
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateTilePacket(x, y, map));
                    }
                    break;
                case 52: {//slumber
                        map.Tile[x, y].Mask2 = 3990;
                        map.Tile[x, y].Mask2Set = 4;
                        map.TempChange = true;
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateTilePacket(x, y, map));
                    }
                    break;
                case 53: {//gust
                        map.Tile[x, y].Mask2 = 3992;
                        map.Tile[x, y].Mask2Set = 4;
                        map.TempChange = true;
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateTilePacket(x, y, map));
                    }
                    break;
                case 70: {//shocker
                        map.Tile[x, y].Mask2 = 3976;
                        map.Tile[x, y].Mask2Set = 4;
                        map.TempChange = true;
                        hitlist.AddPacketToMap(map, PacketBuilder.CreateTilePacket(x, y, map));
                    }
                    break;
            }
            
            PacketHitList.MethodEnded(ref hitlist);
        }
        

        public static void RemoveTrap(IMap map, int x, int y, PacketHitList hitlist) {
            PacketHitList.MethodStart(ref hitlist);
            map.Tile[x, y].Type = Enums.TileType.Walkable;
            map.Tile[x, y].Data1 = 0;
            map.Tile[x, y].Data2 = 0;
            map.Tile[x, y].Data3 = 0;
            map.Tile[x, y].String1 = "";
            map.Tile[x, y].String2 = "";
            map.Tile[x, y].String3 = "";
            map.Tile[x, y].Mask2 = 0;
            map.Tile[x, y].Mask2Set = 0;
            map.TempChange = true;
            hitlist.AddPacketToMap(map, PacketBuilder.CreateTilePacket(x, y, map));
            PacketHitList.MethodEnded(ref hitlist);
        }

        public static string GetTrapType(int scriptNum) {
            switch (scriptNum) {
                case 2:
                    return "Explosion";
                case 3:
                    return "Chestnut";
                case 4:
                    return "PP-Zero";
                case 5:
                    return "Grimy";
                case 6:
                    return "Poison";
                case 7:
                    return "Random";
                case 15:
                    return "Warp";
                case 16:
                    return "Pokemon";
                case 17:
                    return "Spikes";
                case 18:
                    return "ToxicSpikes";
                case 19:
                    return "StealthRock";
                case 23:
                    return "Sticky";
                case 25:
                    return "Mud";
                case 26:
                    return "WonderTile";
                case 28:
                    return "Trip";
                case 34:
                    return "SecretRoom";
                case 39:
                    return "Pitfall";
                case 42:
                    return "Seal";
                case 43:
                    return "Slow";
                case 44:
                    return "Spin";
                case 49:
                    return "Summon";
                case 50:
                    return "Grudge";
                case 51:
                    return "Selfdestruct";
                case 52:
                    return "Slumber";
                case 53:
                    return "Gust";
                case 70:
                    return "Shocker";
                default:
                    return null;
            }
        }

        public static int GetTrapItem(int scriptNum) {
            switch (scriptNum) {
                case 2:
                    return 722;//"Explosion";
                case 3:
                    return 721;//"Chestnut";
                case 4:
                    return 723;//"PP-Zero";
                case 5:
                    return 724;//"Grimy";
                case 6:
                    return 725;//"Poison";
                case 7:
                    return 726;//"Random";
                case 15:
                    return 727;//"Warp";
                case 16:
                    return -1;//"Pokemon";
                case 17:
                    return 728;//"Spikes";
                case 18:
                    return 729;//"ToxicSpikes";
                case 19:
                    return 730;//"StealthRock";
                case 23:
                    return 731;//"Sticky";
                case 25:
                    return 732;//"Mud";
                case 26:
                    return 734;//"WonderTile";
                case 28:
                    return 733;//"Trip";
                case 34:
                    return -1;//"SecretRoom";
                case 39:
                    return 725;//"Pitfall";
                case 42:
                    return 736;//"Seal";
                case 43:
                    return 737;//"Slow";
                case 44:
                    return 738;//"Spin";
                case 49:
                    return 739;//"Summon";
                case 50:
                    return 740;//"Grudge";
                case 51:
                    return 741;//"Selfdestruct";
                case 52:
                    return 742;//"Slumber";
                case 53:
                    return 743;//"Gust";
                case 70:
                    return 744;//"Shocker";
                default:
                    return -1;
            }
        }

        public static string MobilityInfo(Client client, int mobilityNum) {
            switch (mobilityNum) {
                case 0:
                    return mobilityNum + ": None";
                case 1:
                    return mobilityNum + ": Water";
                case 2:
                    return mobilityNum + ": Air";
                case 3:
                    return mobilityNum + ": Lava";
                case 4:
                    return mobilityNum + ": Ghost";
                case 5:
                    return mobilityNum + ": Ice";
                case 6:
                    return mobilityNum + ": Web";
                case 7:
                    return mobilityNum + ": Grass";
                case 8:
                    return mobilityNum + ": Sand";

                default:
                    return mobilityNum.ToString() + ": Unknown";
            }
        }
        
        public static Enums.PokemonType GetMobileBlockTerrainType(int data1) {
        	//Messenger.AdminMsg(data1.ToString(), Text.Black);
			switch (data1) {
				case 2:
					return Enums.PokemonType.Water;
				case 8:
					return Enums.PokemonType.Fire;
				default:
					return Enums.PokemonType.None;
			
			}
			
			//int mobilityList = data1;
            //for (int i = 0; i < 16; i++) {
            //    if (mobilityList % 2 == 1 && !client.Player.GetActiveRecruit().Mobility[i]) {
            //        return true;
            //    }
            //    mobilityList /= 2;
            //}
        }

        public static void PlayerCollide(Client client, Client collidedWith, int playerCount) {
            try {
            	//if (collidedWith.Player.Name == "Dausk") {
            	//	Messenger.PlayerMsg(collidedWith, client.Player.Name, Text.Blue);
            	//}
            
                #region CTF Collision Checks

                if (exPlayer.Get(client).InCTF) {
                    if (ActiveCTF.GameState == CTF.CTFGameState.Started) {
                        switch (exPlayer.Get(client).CTFSide) {
                            case CTF.Teams.Red: {
                                    if (exPlayer.Get(collidedWith).CTFSide != CTF.Teams.Red) {
                                        if (collidedWith.Player.MapID == MapManager.GenerateMapID(CTF.REDMAP) && client.Player.MapID == MapManager.GenerateMapID(CTF.REDMAP)) {
                                            if (collidedWith == ActiveCTF.RedFlagHolder) {
                                                ActiveCTF.CTFMsg(client.Player.Name + " has rescued the flag from " + ActiveCTF.RedFlagHolder.Player.Name + "!", Text.Yellow);
                                                exPlayer.Get(ActiveCTF.RedFlagHolder).CTFState = CTF.PlayerState.Free;

                                                Messenger.PlayerWarp(ActiveCTF.RedFlagHolder, CTF.BLUEMAP, CTF.BLUEX, CTF.BLUEY);
                                                ActiveCTF.RedFlagHolder = null;
                                                ActiveCTF.RedFlags++;
                                            } else {
                                                ActiveCTF.CTFMsg(client.Player.Name + " has caught " + collidedWith.Player.Name + "!", Text.Yellow);
                                                Messenger.PlayerWarp(collidedWith, CTF.BLUEMAP, CTF.BLUEX, CTF.BLUEY);
                                            }
                                        }
                                    }
                                }
                                break;
                            case CTF.Teams.Blue: {
                                    if (exPlayer.Get(collidedWith).CTFSide != CTF.Teams.Blue) {
                                        if (collidedWith.Player.MapID == MapManager.GenerateMapID(CTF.BLUEMAP) && client.Player.MapID == MapManager.GenerateMapID(CTF.BLUEMAP)) {
                                            if (collidedWith == ActiveCTF.BlueFlagHolder) {
                                                ActiveCTF.CTFMsg(client.Player.Name + " has rescued the flag from " + ActiveCTF.BlueFlagHolder.Player.Name + "!", Text.Yellow);
                                                exPlayer.Get(ActiveCTF.BlueFlagHolder).CTFState = CTF.PlayerState.Free;

                                                Messenger.PlayerWarp(ActiveCTF.BlueFlagHolder, CTF.REDMAP, CTF.REDX, CTF.REDY);
                                                ActiveCTF.BlueFlagHolder = null;
                                                ActiveCTF.BlueFlags++;
                                            } else {
                                                ActiveCTF.CTFMsg(client.Player.Name + " has caught " + collidedWith.Player.Name + "!", Text.Yellow);

                                                Messenger.PlayerWarp(collidedWith, CTF.REDMAP, CTF.REDX, CTF.REDY);
                                            }
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }

                #endregion CTF Collision Checks
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: PlayerCollide", Text.Black);
            }
        }

        public static void ScriptedSign(Client client, int script, string string1, string string2, string string3, Enums.Direction dir) {
            try {
                switch (script) {
                    case 0: {
                            //Messenger.PlayerMsg(index, "High scores for: Pit of 100 Trials", Text.Black);
                            //for (int i = 0; i < 10; i++) {
                            //    int scoreFloor = ((CXmlEditor)ObjectFactory.GetObject("pitof100trialsscores")).TryGetAttributeValue("score" + i.ToString(), "Scores", "floor").ToInt(-1);
                            //    if (scoreFloor > -1) {
                            //        NetScript.PlayerMsg(index, (i + 1).ToString() + ". Player: " + ((CXmlEditor)ObjectFactory.GetObject("pitof100trialsscores")).TryGetAttributeValue("score" + i.ToString(), "Scores", "player") + ", Floor: " + (scoreFloor + 1).ToString(), Text.Grey);
                            //    } else {
                            //        NetScript.PlayerMsg(index, (i + 1).ToString() + ". No score registered.", Text.Grey);
                            //    }
                            //}
                        }
                        break;
                    case 1: {
                            if (dir == Enums.Direction.Up) {
                                Messenger.PlayerMsg(client, "Read the back of this sign to win a prize!", Text.Yellow);
                            } else if (dir == Enums.Direction.Down) {
                                Messenger.PlayerMsg(client, "Read the front of this sign to learn how to win a prize!", Text.Yellow);
                            } else if (dir == Enums.Direction.Left) {
                                Messenger.PlayerMsg(client, "You didn't even think to read the sign from the right? WELL YOU SHOULD NEXT TIME", Text.Yellow);
                            } else {
                                Messenger.PlayerMsg(client, "Of course, reading this sign from the left is the way to go.", Text.Yellow);
                            }
                        }
                        break;
                    case 2: {
                            IMap map = client.Player.Map;
                            if (dir == Enums.Direction.Up) {
                                if (map.Tile[client.Player.X, client.Player.Y - 2].Type == Enums.TileType.Walkable) {
                                    client.Player.Y -= 2;
                                    //Messenger.SendPlayerXY(client);
                                    Messenger.PlaySoundToMap(client.Player.MapID, "magic19.wav");
                                } else {
                                    Messenger.PlayerMsg(client, "You can't jump here!", Text.BrightRed);
                                }
                            } else if (dir == Enums.Direction.Down) {
                                if (map.Tile[client.Player.X, client.Player.Y + 2].Type == Enums.TileType.Walkable) {
                                    client.Player.Y += 2;
                                    //Messenger.SendPlayerXY(client);
                                    Messenger.PlaySoundToMap(client.Player.MapID, "magic19.wav");
                                } else {
                                    Messenger.PlayerMsg(client, "You can't jump here!", Text.BrightRed);
                                }
                            } else if (dir == Enums.Direction.Left) {
                                if (map.Tile[client.Player.X - 2, client.Player.Y].Type == Enums.TileType.Walkable) {
                                    client.Player.X -= 2;
                                    //Messenger.SendPlayerXY(client);
                                    Messenger.PlaySoundToMap(client.Player.MapID, "magic19.wav");
                                } else {
                                    Messenger.PlayerMsg(client, "You can't jump here!", Text.BrightRed);
                                }
                            } else {
                                if (map.Tile[client.Player.X + 2, client.Player.Y].Type == Enums.TileType.Walkable) {
                                    client.Player.X += 2;
                                    //Messenger.SendPlayerXY(client);
                                    Messenger.PlaySoundToMap(client.Player.MapID, "magic19.wav");
                                } else {
                                    Messenger.PlayerMsg(client, "You can't jump here!", Text.BrightRed);
                                }
                            }

                        }
                        break;
                    case 3: {
                            StoryManager.PlayStory(client, 36);
                        }
                        break;
                    case 4: {//FFF warp
                            if (client.Player.GetDungeonCompletionCount(5) == 0) {
                                StoryManager.PlayStory(client, 157);
                            } else {
                                StoryManager.PlayStory(client, 158);
                            }
                        }
                        break;
                    case 5: {// Lottery Board
                            if (dir == Enums.Direction.Up) {
                                if (DateTime.Now.DayOfWeek != Lottery.LOTTERY_DAY) {
                                    if (client.Player.HasItem(1) >= 25) {
                                        //if (DateTime.Now.DayOfWeek != LOTTERY_DAY) {
                                        //    NetScript.PlayerMsg(index, "Remember: There is no draw today. Come back on " + LOTTERY_DAY.ToString() + " to enter in the draw!", Text.Grey);
                                        //}
                                        Messenger.AskQuestion(client, "buylotteryticket", "Would you like to purchase one lottery ticket for 25 Poké?", -1);
                                    } else {
                                        //NetScript.PlayerMsg(index, "There is no draw today. Come back on " + LOTTERY_DAY.ToString(), Text.BrightRed);
                                        Messenger.PlayerMsg(client, "You don't have enough money to buy a lottery ticket!", Text.BrightRed);
                                    }
                                } else if (client.Player.HasItem(381) > 0) {
                                    Messenger.AskQuestion(client, "uselotteryticket", "Would you like to use your lottery ticket?", -1);
                                    //} else if (ObjectFactory.GetPlayer(index).HasItem(1) >= 25) {
                                    //    NetScript.AskQuestion(index, "buylotteryticket", "Would you like to purchase one lottery ticket for 25 Poké?", -1);
                                } else {
                                    Messenger.PlayerMsg(client, "Come back tomorrow to purchase next week's lottery tickets.", Text.BrightRed);
                                }
                            }
                        }
                        break;
                    case 6: {//for rock climb
                            Messenger.PlayerMsg(client, "This wall is very rocky... perhaps there is some way to scale it?", Text.Grey);
                        }
                        break;
                    case 7: {//for waterfall
                            Messenger.PlayerMsg(client, "A waterfall crashes down with a mighty roar...", Text.Grey);
                        }
                        break;
                    case 8: {//for west wing
                            if (client.Player.HasItem(249) > 0) {
                                Messenger.AskQuestion(client, "AbandonedMansion", "Will you use the West Wing Key?", -1);
                            } else {
                                StoryManager.PlayStory(client, 298);
                            }
                        }
                        break;
                    case 9: {//chamber tile; doesn't do anything here
                            
                        }
                        break;
                    case 10: {//chamber key
                    		int slot = 0;
                    		for (int i = 1; i <= client.Player.Inventory.Count; i++) {
				                if (client.Player.Inventory[i].Num == 356 && !client.Player.Inventory[i].Sticky) {
				                    slot = i;
				                    break;
				                }
				            }
                            if (slot > 0) {
                                Messenger.AskQuestion(client, "UseItem:356", "Will you use your Silver Key to open the chamber?", -1);
                            }
                        }
                        break;
                    case 11: {//secret room
                            if (client.Player.HasItem(250) > 0) {
                                StoryManager.PlayStory(client, 299);
                            } else {
                                StoryManager.PlayStory(client, 300);
                            }
                        }
                        break;
                    case 12: {//Hallowed Well
                    		
								Story story = new Story();
								StorySegment segment = new StorySegment();
					        	segment.Action = Enums.StoryAction.Say;
					            segment.AddParameter("Text", "This well makes you feel uneasy...  It's feels as though it wants to suck you in...");
					            segment.AddParameter("Mugshot", "-1");
					            segment.Parameters.Add("Speed", "0");
					            segment.Parameters.Add("PauseLocation", "0");
					            story.Segments.Add(segment);
					            
					            segment = new StorySegment();
					        	segment.Action = Enums.StoryAction.Say;
					            segment.AddParameter("Text", "Something shiny seems to glint from the bottom...");
					            segment.AddParameter("Mugshot", "-1");
					            segment.Parameters.Add("Speed", "0");
					            segment.Parameters.Add("PauseLocation", "0");
					            story.Segments.Add(segment);
					            
					            
			            
			            StoryManager.PlayStory(client, story);
                    		
                            //Messenger.AskQuestion(client, "HallowedWell", "Will you jump into the well?", -1);
                        }
                        break;
                    case 13: {//for the Dive Whirlpool itself
                            Messenger.PlayerMsg(client, "A powerful whirlpool is pulling pulling everything into the depths...", Text.Grey);
                        }
                        break;
                    case 14: {
                            Messenger.AskQuestion(client, "CliffsideTablet", "Will you rip one of the tablets off the wall?", -1);
                        }
                        break;
                    case 15: {
                            if (client.Player.HasItem(479) > 0) {
                                StoryManager.PlayStory(client, 121);
                            } else {
                                StoryManager.PlayStory(client, 120);
                            }
                        }
                        break;
                    case 16: { // Register for tournament
                            if (client.Player.Tournament == null) {
                                if (TournamentManager.Tournaments.Count > 0) {
                                    Story story = new Story();
                                    StoryBuilderSegment segment = StoryBuilder.BuildStory();
                                    StoryBuilder.AppendSaySegment(segment, "Hello! I can help you join a tournament!", -1, 0, 0);
                                    StoryBuilder.AppendAskQuestionAction(segment, "Would you like to join a tournament?", 3, 5, -1);
                                    // "Yes" section
                                    StoryBuilder.AppendRunScriptAction(segment, 55, "", "", "", true);
                                    StoryBuilder.AppendGoToSegmentAction(segment, 6);
                                    // "No" section
                                    StoryBuilder.AppendSaySegment(segment, "Come back if you want to join a tournament!", -1, 0, 0);
                                    StoryBuilder.AppendPauseAction(segment, 1);

                                    segment.AppendToStory(story);
                                    StoryManager.PlayStory(client, story);
                                } else {
                                    Story story = new Story();
                                    StoryBuilderSegment segment = StoryBuilder.BuildStory();
                                    StoryBuilder.AppendSaySegment(segment, "Sorry! No tournaments have been created yet! Come back when one has been made!", -1, 0, 0);

                                    segment.AppendToStory(story);
                                    StoryManager.PlayStory(client, story);
                                }
                            }
                        }
                        break;
                    case 17: {
                            int surfSlot = -1;
                            for (int i = 0; i < Constants.MAX_PLAYER_MOVES; i++) {
                                if (client.Player.GetActiveRecruit().Moves[i].MoveNum == 462) {
                                    surfSlot = i;
                                }
                            }

                            if (surfSlot > -1) {
                                Messenger.AskQuestion(client, "Surf", "Water currents lead into the distance... Would you like to use Surf?", -1);
                            }

                        }
                        break;
                    case 18: { // Start the tournament
                            if (client.Player.Tournament != null) {
                                Tournament tourny = client.Player.Tournament;
                                if (tourny.RegisteredMembers[client].Admin) {
                                    Messenger.AskQuestion(client, "TournamentAdminOptions", "What would you like to do?", -1, new string[] { "Start Tournament", "Edit Rules" });
                                }
                            }
                        }
                        break;
                    case 19: {
                            //if (client.Player.HasItem(479) > 0) {
                            StoryManager.PlayStory(client, 184);
                            //} else {
                            //    StoryManager.PlayStory(client, 120);
                            //}
                        }
                        break;
                    case 20: {
                            int surfSlot = -1;
                            for (int i = 0; i < Constants.MAX_PLAYER_MOVES; i++) {
                                if (client.Player.GetActiveRecruit().Moves[i].MoveNum == 456) {
                                    surfSlot = i;
                                }
                            }

                            if (surfSlot > -1) {
                                Messenger.AskQuestion(client, "RockClimb", "This wall is very rocky.  Would you like to use Rock Climb?", -1);
                            }

                        }
                        break;
                    case 21: {
                            if (SnowballGame.IsGameOwner(client) == false) {
                                string waitingGameOwner = SnowballGame.FindWaitingGame();
                                if (string.IsNullOrEmpty(waitingGameOwner)) {
                                    // There are no games waiting to start
                                    Messenger.AskQuestion(client, "SnowballGameNewGame", "Would you like to create a new game?", -1);
                                } else {
                                    // There is a game waiting to start! Join in!
                                    Messenger.AskQuestion(client, "SnowballGameJoinGame", "Would you like to join the game?", -1);
                                }
                            } else {
                                Messenger.AskQuestion(client, "SnowballGameStart", "Would you like to start the game with these players?", -1);
                            }
                    	}
                    	break;
                }
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: ScriptedSign", Text.Black);
            }
        }



        public static string ScriptedSignInfo(Client client, int scriptNum) {
            switch (scriptNum) {
                case 0:
                    return scriptNum + ": Empty";
                case 1:
                    return scriptNum + ": Test";
                case 2:
                    return scriptNum + ": Poorly made ledge";
                case 3:
                    return scriptNum + ": Fireplace";
                case 4:
                    return scriptNum + ": FFF Warp";
                case 5:
                    return scriptNum + ": Lottery";
                case 6:
                    return scriptNum + ": Rock Climb";
                case 7:
                    return scriptNum + ": Waterfall";
                case 8: 
                    return scriptNum + ": West Wing";
                case 9:
                    return scriptNum + ": East Wing";
                case 10:
                    return scriptNum + ": Abandoned Mansion";
                case 11:
                    return scriptNum + ": Mansion Secret Room";
                case 12:
                    return scriptNum + ": Hallowed Well";
                case 13:
                    return scriptNum + ": Dive";
                case 14:
                    return scriptNum + ": Cliffside Tablet";
                case 15:
                    return scriptNum + ": Faded Relic Entrance";
                case 16:
                    return scriptNum + ": Tourny Registration";
                case 17:
                    return scriptNum + ": Surf";
                case 18:
                    return scriptNum + ": Tourny Star";
                case 19:
                    return scriptNum + ": Winden Relic";
                case 20:
                    return scriptNum + ": Rock Climb";
                default:
                    return scriptNum.ToString() + ": Unknown";
            }
        }

        public static RDungeonChamberReq GetChamberReq(int chamberNum, string string1, string string2, string string3) {
            RDungeonChamberReq req = new RDungeonChamberReq();
            try {
                switch (chamberNum) {
                    case 1: {//pre-mapped, does not allow start or end
                        string[] start = string2.Split(':');
                        string[] end = string3.Split(':');
                        int sourceX = start[0].ToInt();
                        int sourceY = start[1].ToInt();
                        req.MinX = end[0].ToInt() - sourceX + 1;
                        req.MinY = end[1].ToInt() - sourceY + 1;
                        req.MaxX = req.MinX;
                        req.MaxY = req.MinY;
                        req.Start = Enums.Acceptance.Never;
                        req.End = Enums.Acceptance.Never;
                        
                        
                        IMap sourceMap = MapManager.RetrieveMap(string1.ToInt());
                        List<int> entrances = new List<int>();
                        for (int x = 0; x < req.MinX; x++) {
                        	if (sourceMap.Tile[sourceX + x, sourceY].RDungeonMapValue == DungeonArrayFloor.GROUND ||
                        		sourceMap.Tile[sourceX + x, sourceY].RDungeonMapValue == DungeonArrayFloor.HALLTILE ||
                        		sourceMap.Tile[sourceX + x, sourceY].RDungeonMapValue == DungeonArrayFloor.DOORTILE) {
                        		entrances.Add(x);
                        	}
                        }
                        if (entrances.Count == 0) {
                        	req.TopAcceptance = Enums.Acceptance.Never;
                        } else {
                        	req.TopPassage = entrances[Server.Math.Rand(0, entrances.Count)];
                        }
                        
                        entrances = new List<int>();
                        for (int x = 0; x < req.MinX; x++) {
                        	if (sourceMap.Tile[sourceX + x, sourceY+req.MinY-1].RDungeonMapValue == DungeonArrayFloor.GROUND ||
                        		sourceMap.Tile[sourceX + x, sourceY+req.MinY-1].RDungeonMapValue == DungeonArrayFloor.HALLTILE ||
                        		sourceMap.Tile[sourceX + x, sourceY+req.MinY-1].RDungeonMapValue == DungeonArrayFloor.DOORTILE) {
                        		entrances.Add(x);
                        	}
                        }
                        if (entrances.Count == 0) {
                        	req.BottomAcceptance = Enums.Acceptance.Never;
                        } else {
                        	req.BottomPassage = entrances[Server.Math.Rand(0, entrances.Count)];
                        }
                        
                        entrances = new List<int>();
                        for (int y = 0; y < req.MinY; y++) {
                        	if (sourceMap.Tile[sourceX, sourceY+y].RDungeonMapValue == DungeonArrayFloor.GROUND ||
                        		sourceMap.Tile[sourceX, sourceY+y].RDungeonMapValue == DungeonArrayFloor.HALLTILE ||
                        		sourceMap.Tile[sourceX, sourceY+y].RDungeonMapValue == DungeonArrayFloor.DOORTILE) {
                        		entrances.Add(y);
                        	}
                        }
                        if (entrances.Count == 0) {
                        	req.LeftAcceptance = Enums.Acceptance.Never;
                        } else {
                        	req.LeftPassage = entrances[Server.Math.Rand(0, entrances.Count)];
                        }
                        
                        entrances = new List<int>();
                        for (int y = 0; y < req.MinY; y++) {
                        	if (sourceMap.Tile[sourceX+req.MinX-1, sourceY+y].RDungeonMapValue == DungeonArrayFloor.GROUND ||
                        		sourceMap.Tile[sourceX+req.MinX-1, sourceY+y].RDungeonMapValue == DungeonArrayFloor.HALLTILE ||
                        		sourceMap.Tile[sourceX+req.MinX-1, sourceY+y].RDungeonMapValue == DungeonArrayFloor.DOORTILE) {
                        		entrances.Add(y);
                        	}
                        }
                        
                        if (entrances.Count == 0) {
                        	req.RightAcceptance = Enums.Acceptance.Never;
                        } else {
                        	req.RightPassage = entrances[Server.Math.Rand(0, entrances.Count)];
                        }
                        }
                        break;
                    case 2: {//no chamber
                    		req.TopPassage = -1;
                        	req.TopAcceptance = Enums.Acceptance.Never;
                            req.BottomPassage = -1;
                            req.BottomAcceptance = Enums.Acceptance.Never;
                            req.LeftPassage = -1;
                            req.LeftAcceptance = Enums.Acceptance.Never;
                            req.RightPassage = -1;
                            req.RightAcceptance = Enums.Acceptance.Never;
                        }
                        break;
                    case 3: {//chamber that accepts start and end
                    
                    	}
                    	break;
                    case 4: {//Ice Puzzle
                    		req.Start = Enums.Acceptance.Never;
                        	req.End = Enums.Acceptance.Never;
                    		string[] parse = string1.Split(':');
                    		if (parse[0] == "1") {
                    			req.Start = Enums.Acceptance.Always;
                    		}
                    		if (parse[1] == "1") {
                    			req.End = Enums.Acceptance.Always;
                    		}
                    		parse = string3.Split(':');
                        	req.MinX = parse[0].ToInt()+2;
                        	req.MinY = parse[1].ToInt()+2;
                        	
                        	
                        	if (req.MinX > 50) req.MinX = 50;
                        	if (req.MinY > 50) req.MinY = 50;
                        	if (req.MinX < 4) req.MinX = 4;
                        	if (req.MinY < 4) req.MinY = 4;
                        	
                        	req.MaxX = req.MinX;
                        	req.MaxY = req.MinY;
                        	
                        	req.TopPassage = 0;
                            req.BottomPassage = req.MinX-1;
                            req.LeftPassage = 0;
                            req.RightPassage = req.MinY-1;
                    	}
                    	break;
                    case 10: {//single-item chamber, does not allow start or end
                        req.MinX = 5;
                        req.MinY = 5;
                        req.MaxX = req.MinX;
                        req.MaxY = req.MinY;
                        req.Start = Enums.Acceptance.Never;
                        req.End = Enums.Acceptance.Never;
                        
                        
                        }
                        break;
                    case 11: {//3x3 kecleon shop, does not allow start or end
                        req.MinX = 7;
                        req.MinY = 7;
                        req.MaxX = req.MinX;
                        req.MaxY = req.MinY;
                        req.Start = Enums.Acceptance.Never;
                        req.End = Enums.Acceptance.Never;
                        
                        
                        }
                        break;
                    default: {
                            req.MinX = 2;
                            req.MaxX = 2;
                            req.MinY = 2;
                            req.MaxY = 2;
                            req.TopPassage = -1;
                            req.TopAcceptance = Enums.Acceptance.Maybe;
                            req.BottomPassage = -1;
                            req.BottomAcceptance = Enums.Acceptance.Maybe;
                            req.LeftPassage = -1;
                            req.LeftAcceptance = Enums.Acceptance.Maybe;
                            req.RightPassage = -1;
                            req.RightAcceptance = Enums.Acceptance.Maybe;
                            req.Start = Enums.Acceptance.Always;
                            req.End = Enums.Acceptance.Always;
                        }
                        break;
                }
            } catch (Exception e) {
                Messenger.AdminMsg("Error: GetChamberReq", Text.Black);
                Messenger.AdminMsg(e.ToString(), Text.Black);
            }

            return req;
        }

        public static void CreateChamber(RDungeonMap map, DungeonArrayFloor arrayFloor, int chamberNum, string string1, string string2, string string3) {
            try {
            	bool started = false;
            	//Messenger.AdminMsg(chamberNum.ToString(), Text.Black);
            	switch (chamberNum) {
                    case 1: {//pre-mapped
                        DungeonArrayRoom room = arrayFloor.Rooms[arrayFloor.Chamber.X, arrayFloor.Chamber.Y];
                        string[] start = string2.Split(':');
                        string[] end = string3.Split(':');
                        int sourceX = start[0].ToInt();
                        int sourceY = start[1].ToInt();
                        int xDiff = end[0].ToInt() - sourceX + 1;
                        int yDiff = end[1].ToInt() - sourceY + 1;
                        
                        IMap sourceMap = MapManager.RetrieveMap(string1.ToInt());
                        for (int x = 0; x < xDiff; x++) {
                            for (int y = 0; y < yDiff; y++) {
                                MapCloner.CloneTile(sourceMap, sourceX + x, sourceY + y, map.Tile[room.StartX + x, room.StartY + y]);
                                arrayFloor.MapArray[room.StartX + x, room.StartY + y] = sourceMap.Tile[sourceX + x, sourceY + y].RDungeonMapValue;
                            }
                        }
                        }
                        break;
                    case 2: {//no chamber
                    
                    	}
                    	break;
                    case 3: {//chamber that accepts start and end
                    
                    	}
                    	break;
                    case 4: {//Ice Puzzle
                    		int startX = 0;
                    		int startY = 0;
                    		string[] parse = string1.Split(':');
                    		bool start = false;
                    		bool end = false;
                    		if (parse[0] == "1") {
                    			start = true;
                    		}
                    		if (parse[1] == "1") {
                    			end = true;
                    		}
                    		if (start && end) {
	                    		for (int x = 0; x < map.MaxX; x++) {
		                            for (int y = 0; y < map.MaxY; y++) {
		                            	map.Tile[x, y].Type = Enums.TileType.Blocked;
		                            	arrayFloor.MapArray[x, y] = 1025;
		                            }
		                        }
	                        }
                    		
                    		DungeonArrayRoom room = arrayFloor.Rooms[arrayFloor.Chamber.X, arrayFloor.Chamber.Y];
                        	parse = string3.Split(':');
                        	int xDiff = room.EndX - room.StartX + 1;
                        	int yDiff = room.EndY - room.StartY + 1;
                    		int[,] intArray = new int[xDiff - 2, yDiff-2];
                    		
                    		if (start && end) {
                        		
                        		intArray = RandomIce.GenIcePuzzle(xDiff - 2, yDiff-2, parse[2].ToInt(), parse[3].ToInt(), parse[4].ToInt(), parse[5].ToInt(), parse[6].ToInt(), false, false);
                        	} else if (end) {
                        		intArray = RandomIce.GenIcePuzzle(xDiff - 2, yDiff-2, parse[2].ToInt(), parse[3].ToInt(), parse[4].ToInt(), parse[5].ToInt(), parse[6].ToInt(), true, false);
                        	}
                        	
                        	//put it on the map
                        	for (int y = 0; y < yDiff; y++) {
	                            for (int x = 0; x < xDiff; x++) {
	                            	if (x == 0 || x == xDiff - 1 || y == 0 || y == yDiff - 1) {
	                            		map.Tile[room.StartX + x, room.StartY + y].Type = Enums.TileType.Slippery;
	                            		arrayFloor.MapArray[room.StartX + x, room.StartY + y] = 2;
	                            	} else if (intArray[x-1,y-1] == RandomIce.START) {
	                            		
	                            		map.Tile[room.StartX + x, room.StartY + y].Type = Enums.TileType.Walkable;
	                            		arrayFloor.MapArray[room.StartX + x, room.StartY + y] = 6;
	                            		startX = x;
	                            		startY = y;
	                            		started = true;
	                            	} else if (intArray[x-1,y-1] == RandomIce.END) {
	                            		
	                            		map.Tile[room.StartX + x, room.StartY + y].Type = Enums.TileType.RDungeonGoal;
	                            		arrayFloor.MapArray[room.StartX + x, room.StartY + y] = 7;
	                            	} else if (intArray[x-1,y-1] == RandomIce.BLOCK) {
	                            		map.Tile[room.StartX + x, room.StartY + y].Type = Enums.TileType.Blocked;
	                            		arrayFloor.MapArray[room.StartX + x, room.StartY + y] = 1;
	                            	} else if (intArray[x-1,y-1] == RandomIce.GROUND) {
	                            		map.Tile[room.StartX + x, room.StartY + y].Type = Enums.TileType.Hallway;
	                            		arrayFloor.MapArray[room.StartX + x, room.StartY + y] = 3;
	                            	} else {
	                            		map.Tile[room.StartX + x, room.StartY + y].Type = Enums.TileType.Slippery;
	                            		arrayFloor.MapArray[room.StartX + x, room.StartY + y] = 2;
	                            	}
	                                
	                            }
	                        }
	                        
	                        if (!start && end) {
	                        	if (startX == 1) {
	                        		map.Tile[room.StartX + startX - 1, room.StartY + startY].Type = Enums.TileType.Walkable;
	                        		arrayFloor.MapArray[room.StartX + startX - 1, room.StartY + startY] = 6;
	                        	} else if (startX == xDiff - 2) {
	                        		map.Tile[room.StartX + startX + 1, room.StartY + startY].Type = Enums.TileType.Walkable;
	                            	arrayFloor.MapArray[room.StartX + startX + 1, room.StartY + startY] = 6;
	                            	}
	                            if (startY == 1) {
	                            	map.Tile[room.StartX + startX, room.StartY + startY - 1].Type = Enums.TileType.Walkable;
	                            	arrayFloor.MapArray[room.StartX + startX, room.StartY + startY - 1] = 6;
	                            } else if (startY == yDiff - 2) {
	                            	map.Tile[room.StartX + startX, room.StartY + startY + 1].Type = Enums.TileType.Walkable;
	                            	arrayFloor.MapArray[room.StartX + startX, room.StartY + startY + 1] = 6;
	                            }
	                        }
	                        
	                        if (start && end) {
		                        map.Tile[room.StartX, room.StartY].Type = Enums.TileType.Scripted;
		                        map.Tile[room.StartX, room.StartY].Data1 = 15;
		                        map.Tile[room.StartX, room.StartY].Data3 = 1;
		                        arrayFloor.MapArray[room.StartX, room.StartY] = 4;
	                        } else if (end) {
	                        	map.Tile[room.StartX, room.StartY].Type = Enums.TileType.Walkable;
	                        	arrayFloor.MapArray[room.StartX, room.StartY] = 3;
	                        	map.Tile[room.EndX, room.EndY].Type = Enums.TileType.Walkable;
	                        	arrayFloor.MapArray[room.EndX, room.EndY] = 3;
	                        }
                        	if (!started && start) {
                        		Messenger.AdminMsg("Bad puzzle generated", Text.Red);
                        	}
                    	}
                    	break;
                    case 10: {//pre-mapped locked chamber
                        DungeonArrayRoom room = arrayFloor.Rooms[arrayFloor.Chamber.X, arrayFloor.Chamber.Y];
                        
                        //IMap sourceMap = MapManager.RetrieveMap(string1.ToInt());
                        for (int x = 0; x < 5; x++) {
                            for (int y = 0; y < 5; y++) {
                            	if (x == 2 && y == 3) {
                            		map.Tile[room.StartX + x, room.StartY + y].Type = Enums.TileType.ScriptedSign;
                            		map.Tile[room.StartX + x, room.StartY + y].Mask2 = 6;
                            		map.Tile[room.StartX + x, room.StartY + y].Mask2Set = 4;
                            		map.Tile[room.StartX + x, room.StartY + y].Data1 = 10;
                            		map.Tile[room.StartX + x, room.StartY + y].String1 = "-2:-3";
                            		map.Tile[room.StartX + x, room.StartY + y].String2 = "2:1";
                            		arrayFloor.MapArray[room.StartX + x, room.StartY + y] = 1025;
                            	} else if (x == 0 || y == 0 || x == 4 || y == 4) {
                            		arrayFloor.MapArray[room.StartX + x, room.StartY + y] = 4;
                            	} else if (x == 1 || y == 1 || x == 3 || y == 3) {
                            		map.Tile[room.StartX + x, room.StartY + y].Type = Enums.TileType.Blocked;
                            		arrayFloor.MapArray[room.StartX + x, room.StartY + y] = 1025;
                            	} else if (x == 2 && y == 2) {
                            		map.Tile[room.StartX + x, room.StartY + y].Type = Enums.TileType.ScriptedSign;
                            		map.Tile[room.StartX + x, room.StartY + y].Data1 = 9;
                            		map.Tile[room.StartX + x, room.StartY + y].String1 = string1;
                            		map.Tile[room.StartX + x, room.StartY + y].String2 = string2;
                            		map.Tile[room.StartX + x, room.StartY + y].String3 = "3";
                            		arrayFloor.MapArray[room.StartX + x, room.StartY + y] = 1025;
                            	}
                                
                            }
                        }
                        }
                        break;
                    case 11: {//3x3 kecleon shop
                        DungeonArrayRoom room = arrayFloor.Rooms[arrayFloor.Chamber.X, arrayFloor.Chamber.Y];
                        
                        //IMap sourceMap = MapManager.RetrieveMap(string1.ToInt());
                        //Messenger.AdminMsg("Room creating", Text.Red);
                        for (int x = 0; x < 7; x++) {
                            for (int y = 0; y < 7; y++) {
                            	if (x <= 1 || y <= 1 || x >= 5 || y >= 5) {
                            		//free space
                            		arrayFloor.MapArray[room.StartX + x, room.StartY + y] = 4;
                            	} else if (x == 2 || y == 2 || x == 4 || y == 4) {
                            		if (Server.Math.Rand(0,2) == 0) {
	                            		//shop
	                            		int itemNum = 0;
	                            		int itemVal = 1;
	                            		string tag = "";
	                            		int price = 1;
	                            		string[] itemSelection = string2.Split(';');
	                            		string[] priceSelection = string3.Split(';');
	                            		int chosenIndex = Server.Math.Rand(0, itemSelection.Length);
							            string[] chosenItem = itemSelection[chosenIndex].Split(',');
							            if (chosenItem[0].IsNumeric()) {
							                if (chosenItem.Length == 2 && chosenItem[1].IsNumeric()) {
							                    itemNum = chosenItem[0].ToInt();
							                    itemVal = chosenItem[1].ToInt();
							                    price = priceSelection[chosenIndex].ToInt();
							                } else if (chosenItem.Length == 3 && chosenItem[2].IsNumeric()) {
							                	itemNum = chosenItem[0].ToInt();
							                    itemVal = chosenItem[1].ToInt();
							                    tag = chosenItem[2];
							                    price = priceSelection[chosenIndex].ToInt();
							                } else {
							                	itemNum = chosenItem[0].ToInt();
							                	price = priceSelection[chosenIndex].ToInt();
							                }
							            }
	                            		map.Tile[room.StartX + x, room.StartY + y].Type = Enums.TileType.DropShop;
	                            		map.Tile[room.StartX + x, room.StartY + y].Mask2 = 67;
	                            		map.Tile[room.StartX + x, room.StartY + y].Mask2Set = 0;
	                            		map.Tile[room.StartX + x, room.StartY + y].Data1 = price; //price per unit
	                            		map.Tile[room.StartX + x, room.StartY + y].Data2 = itemNum; //item
	                            		map.Tile[room.StartX + x, room.StartY + y].Data3 = itemVal; //amount
	                            		map.Tile[room.StartX + x, room.StartY + y].String1 = "";//no tag//no charID
	                            		map.Tile[room.StartX + x, room.StartY + y].String2 = "";//no tag
	                            		arrayFloor.MapArray[room.StartX + x, room.StartY + y] = 1027;
                            		} else {
                            			map.Tile[room.StartX + x, room.StartY + y].Type = Enums.TileType.DropShop;
	                            		map.Tile[room.StartX + x, room.StartY + y].Mask2 = 67;
	                            		map.Tile[room.StartX + x, room.StartY + y].Mask2Set = 0;
	                            		map.Tile[room.StartX + x, room.StartY + y].Data1 = 0; //price per unit
	                            		map.Tile[room.StartX + x, room.StartY + y].Data2 = 0; //item
	                            		map.Tile[room.StartX + x, room.StartY + y].Data2 = 0; //amount
	                            		map.Tile[room.StartX + x, room.StartY + y].String1 = "";//no tag//no charID
	                            		map.Tile[room.StartX + x, room.StartY + y].String2 = "";//no tag
	                            		arrayFloor.MapArray[room.StartX + x, room.StartY + y] = 1027;
                            		}
                            	} else if (x == 3 && y == 3) {
                            		map.Tile[room.StartX + x, room.StartY + y].Type = Enums.TileType.NPCAvoid;
                            		map.Tile[room.StartX + x, room.StartY + y].Mask2 = 67;
                            		map.Tile[room.StartX + x, room.StartY + y].Mask2Set = 0;
                            		arrayFloor.MapArray[room.StartX + x, room.StartY + y] = 1027;
                            		MapNpcPreset npc = new MapNpcPreset();
		                            npc.SpawnX = room.StartX + x;
		                            npc.SpawnY = room.StartY + y;
		                            npc.NpcNum = 32;
		                            npc.MinLevel = 100;
		                            npc.MaxLevel = 100;
		                            map.Npc.Add(npc);
                            	}
                                
                            }
                        }
                        //Messenger.AdminMsg("Room created: " + map.Name, Text.Red);
                        }
                        break;
                    default: {
                            if (arrayFloor.Start.X == arrayFloor.Chamber.X && arrayFloor.Start.Y == arrayFloor.Chamber.Y) {
                                DungeonArrayRoom room = arrayFloor.Rooms[arrayFloor.Start.X, arrayFloor.Start.Y];
                                arrayFloor.MapArray[room.StartX, room.StartY] = DungeonArrayFloor.STARTTILE;
                            }
                            if (arrayFloor.End.X == arrayFloor.Chamber.X && arrayFloor.End.Y == arrayFloor.Chamber.Y) {
                                DungeonArrayRoom room = arrayFloor.Rooms[arrayFloor.End.X, arrayFloor.End.Y];
                                arrayFloor.MapArray[room.StartX + 1, room.StartY] = DungeonArrayFloor.ENDTILE;
                            }
                        }
                        break;
                }
            } catch (Exception e) {
                Messenger.AdminMsg(e.ToString(), Text.Black);
            }

        }


        public static bool IsPlayerOnNoSuicideTile(Client client) {
            IMap map = client.Player.Map;
            if (map.Tile[client.Player.X, client.Player.Y].Type == Enums.TileType.Scripted &&
                map.Tile[client.Player.X, client.Player.Y].Data1 == 22) {
                return true;
            } else {
                return false;
            }
        }

        public static bool VerifyMapInstancedTiles(Client client, IMap map, PacketHitList packetList) {
            PacketHitList.MethodStart(ref packetList);
            bool unlocked = false;

            for (int x = 0; x <= map.MaxX; x++) {
                for (int y = 0; y <= map.MaxY; y++) {
                    bool state = VerifyEvolutionBlockTile(client, map, x, y, packetList);
                    if (state) {
                        unlocked = true;
                    }
                }
            }
            PacketHitList.MethodEnded(ref packetList);

            return unlocked;
        }

        public static bool VerifyEvolutionBlockTile(Client client, IMap map, int x, int y, PacketHitList packetList) {
            PacketHitList.MethodStart(ref packetList);
            bool unlocked = false;

            Tile mapTile = map.Tile[x, y];
            if (mapTile.Type == Server.Enums.TileType.Scripted) {
                if (mapTile.Data1 == 40) {
                    if (exPlayer.Get(client).EvolutionActive) {
                        unlocked = true;
                        DisplayInvisibleKeyTile(client, map, x, y, packetList);
                    } else {
                        DisplayVisibleKeyTile(client, map, x, y, packetList);
                    }
                }
            }
            PacketHitList.MethodEnded(ref packetList);
            return unlocked;
        }

        public static void DisplayVisibleKeyTile(Client client, IMap map, int x, int y, PacketHitList packetList) {
            Tile tile = new Tile(new DataManager.Maps.Tile());
                MapCloner.CloneTile(map, x, y, tile);
            tile.Mask = 6;
            tile.MaskSet = 4;
            tile.Type = Enums.TileType.Blocked;

            Messenger.SendTemporaryTileTo(packetList, client, x, y, tile);
        }

        public static void DisplayInvisibleKeyTile(Client client, IMap map, int x, int y, PacketHitList packetList) {
            Tile tile = new Tile(new DataManager.Maps.Tile());
            MapCloner.CloneTile(map, x, y, tile);
            tile.Mask = 0;
            tile.MaskSet = 4;
            tile.Type = Enums.TileType.Walkable;

            Messenger.SendTemporaryTileTo(packetList, client, x, y, tile);
        }

        private static bool IsSuicideTile(Client client) {

            IMap playerMap = client.Player.Map;

            if (playerMap.Tile[client.Player.X, client.Player.Y].Type == Enums.TileType.Scripted && playerMap.Tile[client.Player.X, client.Player.Y].Data1 == 22) {

                return true;


            } else if (playerMap.MapID == "s41" || playerMap.MapID == "s120" || playerMap.MapID == "s121" || playerMap.MapID == "s122" || playerMap.MapID == "s123" || playerMap.MapID == "s124" || playerMap.MapID == "s125" || playerMap.MapID == "s126") {

                return true;

            } else {
                return false;
            }
        }

        #region Path Methods

        public static int FindNearestNpc(Client client) {
            Enums.Direction dir = client.Player.Direction;
            IMap map = client.Player.Map;
            //MapNpc activeNpc = new MapNpc();
            int x = client.Player.X;
            int y = client.Player.Y;
            int closestNpc = -1;
            if (dir == Enums.Direction.Up || dir == Enums.Direction.Down) {
                int closestY = 999;
                for (int i = 0; i < Constants.MAX_MAP_NPCS; i++) {
                    //activeNpc = ObjectFactory.GetMapNpc(map, i);
                    if (map.ActiveNpc[i].X == x && map.ActiveNpc[i].HP > 0) {
                        if ((dir == Enums.Direction.Up && map.ActiveNpc[i].Y < y) || (dir == Enums.Direction.Down && map.ActiveNpc[i].Y > y)) {
                            if (System.Math.Abs(map.ActiveNpc[i].Y - y) < System.Math.Abs(closestY - y)) {
                                closestY = map.ActiveNpc[i].Y;
                                closestNpc = i;
                            }
                        }
                    }
                }
            } else {
                int closestX = 999;
                for (int i = 0; i < 15; i++) {
                    //activeNpc = ObjectFactory.GetMapNpc(map, i);
                    if (map.ActiveNpc[i].Y == y && map.ActiveNpc[i].HP > 0) {
                        if ((dir == Enums.Direction.Left && map.ActiveNpc[i].X < x) || (dir == Enums.Direction.Right && map.ActiveNpc[i].X > x)) {
                            if (System.Math.Abs(map.ActiveNpc[i].X - x) < System.Math.Abs(closestX - x)) {
                                closestX = map.ActiveNpc[i].X;
                                closestNpc = i;
                            }
                        }
                    }
                }
            }
            return closestNpc;
        }

        public static bool IsPathClear(Client client, int targetNpc) {
            if (targetNpc < 0 || targetNpc > 14) {
                return false;
            }
            IMap map = client.Player.Map;
            int playerX = client.Player.X;
            int playerY = client.Player.Y;
            Enums.Direction playerDir = client.Player.Direction;
            MapNpc activeNpc = map.ActiveNpc[targetNpc];
            if (playerDir == Enums.Direction.Up) {
                if (playerX != activeNpc.X) {
                    return false;
                }
                for (int currY = playerY; currY >= activeNpc.Y; currY--) {
                    if (IsBlocked(map, playerX, currY)) {
                        return false;
                    }
                }
            } else if (playerDir == Enums.Direction.Down) {
                if (playerX != activeNpc.X) {
                    return false;
                }
                for (int currY = playerY; currY <= activeNpc.Y; currY++) {
                    if (IsBlocked(map, playerX, currY)) {
                        return false;
                    }
                }
            } else if (playerDir == Enums.Direction.Left) {
                if (playerY != activeNpc.Y) {
                    return false;
                }
                for (int currX = playerX; currX >= activeNpc.X; currX--) {
                    if (IsBlocked(map, currX, playerY)) {
                        return false;
                    }
                }
            } else if (playerDir == Enums.Direction.Right) {
                if (playerY != activeNpc.Y) {
                    return false;
                }
                for (int currX = playerX; currX <= activeNpc.X; currX++) {
                    if (IsBlocked(map, currX, playerY)) {
                        return false;
                    }
                }
            }
            return true;
        }

        public static void FindNearestBlock(IMap map, Enums.Direction dir, ref int x, ref int y) {
            // NOTE: actually returns space before the nearest block
            int checkedX = x;
            int checkedY = y;
            for (int i = 0; i < 100; i++) {
                MoveInDirection(dir, 1, ref checkedX, ref checkedY);
                if (checkedX >= 0 && checkedX <= map.MaxX && checkedY >= 0 && checkedY <= map.MaxY && !IsBlocked(map, checkedX, checkedY)) {
                    MoveInDirection(dir, 1, ref x, ref y);
                    //break;
                } else {
                    //MoveInDirection(dir, 1, ref x, ref y);
                    //MoveInDirection((Enums.Direction)(((int)dir + 1) % 2 + (int)dir / 2 * 2), 1, ref x, ref y);
                    break;
                }
            }
        }

        public static bool IsBlocked(IMap map, int x, int y) {
            Enums.TileType attrib = map.Tile[x, y].Type;
            switch (attrib) {
                case Enums.TileType.Blocked:
                    return true;
                case Enums.TileType.Warp:
                    return true;
                case Enums.TileType.Key:
                    return true;
                case Enums.TileType.Door:
                    return true;
                case Enums.TileType.Sign:
                    return true;
                case Enums.TileType.LevelBlock:
                    return true;
                case Enums.TileType.SpriteBlock:
                    return true;
                case Enums.TileType.MobileBlock:
                    return true;
                case Enums.TileType.Story:
                    return true;
                case Enums.TileType.ScriptedSign:
                    return true;
                case Enums.TileType.Scripted:
                    return IsScriptedTileBlocked(map, x, y);
                default:
                    return false;
            }
        }

        public static void MoveInDirection(Enums.Direction dir, int steps, ref int x, ref int y) {
            switch (dir) {
                case Enums.Direction.Up: {
                        y -= steps;
                    }
                    break;
                case Enums.Direction.Down: {
                        y += steps;
                    }
                    break;
                case Enums.Direction.Left: {
                        x -= steps;
                    }
                    break;
                case Enums.Direction.Right: {
                        x += steps;
                    }
                    break;

            }
        }

        #endregion Path Methods
        
        public static void EnterArena(Client client, ICharacter character, IMap map, string param2, string param3, PacketHitList hitlist) {
        	PacketHitList.MethodStart(ref hitlist);
        	
        	//activate set-level mode for tanren arena
        	if (map.MapID == MapManager.GenerateMapID(1718)) client.Player.BeginTempStatMode(50, true);
        	
        	if (param2.IsNumeric() && param3.IsNumeric()) {
                RandomWarp(character, map, false, hitlist, param2.ToInt(), param2.ToInt(), param3.ToInt(), param3.ToInt());
            } else {
                RandomWarp(character, map, false, hitlist, 5, 24, 7, 23);
            }
            hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("magic256.wav"), character.X, character.Y, 10);
            hitlist.AddPacketToMap(map, PacketBuilder.CreateChatMsg(client.Player.Name + " has entered the arena!", Text.BrightGreen));

            for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
                if (client.Player.Team[i].Loaded) { // Yes, there needs to be a check
                    client.Player.Team[i].HP = client.Player.Team[i].MaxHP;
                    client.Player.Team[i].RestoreBelly();
                    client.Player.Team[i].StatusAilment = Enums.StatusAilment.OK;
                    client.Player.Team[i].StatusAilmentCounter = 0;

                    for (int j = 0; j < 4; j++) {
                        if (client.Player.GetActiveRecruit().Moves[i].MoveNum != 0) {

                        	client.Player.Team[i].Moves[j].CurrentPP = client.Player.Team[i].Moves[j].MaxPP;
                    	}
                    }
                    RemoveBuffs(client.Player.Team[i]);
                    RemoveAllBondedExtraStatus(client.Player.Team[i], map, hitlist, false);
            		client.Player.Team[i].VolatileStatus.Clear();
                }
            }
            

            //remove statuses
            

            if (HasAbility(client.Player.GetActiveRecruit(), "Illusion")) {
                int selectedSlot = client.Player.ActiveSlot;
                    for (int i = 1; i < 4; i++) {
                        if (client.Player.Team[(selectedSlot + i) % 4] != null && client.Player.Team[(selectedSlot + i) % 4].Loaded) {
                            selectedSlot = (selectedSlot + i) % 4;
                            break;
                        }
                    }
                    if (selectedSlot != client.Player.ActiveSlot) {
                        AddExtraStatus(client.Player.GetActiveRecruit(), client.Player.Map, "Illusion", client.Player.Team[selectedSlot].Sprite, null, "", hitlist);
                    }
                }

                //set illusion
                if (client.Player.GetActiveRecruit().HasActiveItem(213)) {
                    AddExtraStatus(client.Player.GetActiveRecruit(), client.Player.Map, "Illusion", 25, null, "", hitlist);
                }
                if (client.Player.GetActiveRecruit().HasActiveItem(224)) {
                    AddExtraStatus(client.Player.GetActiveRecruit(), client.Player.Map, "Illusion", 300, null, "", hitlist);
                }
                if (client.Player.GetActiveRecruit().HasActiveItem(244)) {
                    AddExtraStatus(client.Player.GetActiveRecruit(), client.Player.Map, "Illusion", 387, null, "", hitlist);
                }
                if (client.Player.GetActiveRecruit().HasActiveItem(245)) {
                    AddExtraStatus(client.Player.GetActiveRecruit(), client.Player.Map, "Illusion", 390, null, "", hitlist);
                }
                if (client.Player.GetActiveRecruit().HasActiveItem(246)) {
                    AddExtraStatus(client.Player.GetActiveRecruit(), client.Player.Map, "Illusion", 393, null, "", hitlist);
                }

                RefreshCharacterTraits(client.Player.GetActiveRecruit(), map, hitlist);

				if (!client.Player.Dead) {
	                if (HasAbility(client.Player.GetActiveRecruit(), "Sand Stream")) {
	                    SetMapWeather(map, Enums.Weather.Sandstorm, hitlist);
	                } else if (HasAbility(client.Player.GetActiveRecruit(), "Snow Warning")) {
	                    SetMapWeather(map, Enums.Weather.Hail, hitlist);
	                } else if (HasAbility(client.Player.GetActiveRecruit(), "Drizzle")) {
	                    SetMapWeather(map, Enums.Weather.Raining, hitlist);
	                } else if (HasAbility(client.Player.GetActiveRecruit(), "Drought")) {
	                    SetMapWeather(map, Enums.Weather.Sunny, hitlist);
	                } else if (HasAbility(client.Player.GetActiveRecruit(), "Air Lock")) {
	                    SetMapWeather(map, Enums.Weather.None, hitlist);
	                } else if (HasAbility(client.Player.GetActiveRecruit(), "Cloud Nine")) {
	                    SetMapWeather(map, Enums.Weather.None, hitlist);
	                }
                }


                if (HasAbility(client.Player.GetActiveRecruit(), "Download")) {
                    int totalDef = 0;
                    int totalSpDef = 0;
                    TargetCollection targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Floor, 0, map, client.Player.GetActiveRecruit(), client.Player.GetActiveRecruit().X, client.Player.GetActiveRecruit().Y, Enums.Direction.Up, true, false, false);
                    for (int i = 0; i < targets.Foes.Count; i++) {
                        totalDef += targets.Foes[i].Def;
                        totalSpDef += targets.Foes[i].SpclDef;
                    }

                if (totalDef < totalSpDef) {
                    ChangeAttackBuff(client.Player.GetActiveRecruit(), map, 1, hitlist);
                } else {
                    ChangeSpAtkBuff(client.Player.GetActiveRecruit(), map, 1, hitlist);
                }
            }
			PacketHitList.MethodEnded(ref hitlist);
        }
        
        public static void SetMapWeather(IMap map, Enums.Weather weather, PacketHitList hitlist) {
            TargetCollection checkedTargets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Floor, 0, map, null, 0, 0, Enums.Direction.Up, true, true, false);
            for (int i = 0; i < checkedTargets.Count; i++) {
                
                if (map.Weather == Enums.Weather.None && (HasAbility(checkedTargets[i], "Cloud Nine") || HasAbility(checkedTargets[i], "Air Lock"))) {
                	if (checkedTargets[i].CharacterType == Enums.CharacterType.Recruit) {
                		if (((Recruit)checkedTargets[i]).Owner.Player.Dead) {
                		
                		} else {
                			return;
                		}
                	} else {
                    	return;
                    }
                }
            }
        	map.Weather = weather;
            hitlist.AddPacketToMap(map, TcpPacket.CreatePacket("weather", ((int)map.Weather).ToString()));
        }

        public static void SpawnNpcInRange(IMap map, int x, int y, int range, PacketHitList hitlist) {
            if (map.Npc.Count == 0) return;

            if (map.SpawnMarker >= map.Npc.Count) map.SpawnMarker = map.Npc.Count - 1;

            for (int i = 0; i < 100; i++) {
                if (Server.Math.Rand(0, 100) < map.Npc[map.SpawnMarker].AppearanceRate && map.WillSpawnNow(map.Npc[map.SpawnMarker])) {
                    break;
                }
                map.SpawnMarker++;
                if (map.SpawnMarker >= map.Npc.Count) map.SpawnMarker = 0;
            }

            MapNpcPreset spawningNpc = new MapNpcPreset();
            spawningNpc.NpcNum = map.Npc[map.SpawnMarker].NpcNum;
            spawningNpc.MinLevel = map.Npc[map.SpawnMarker].MinLevel;
            spawningNpc.MaxLevel = map.Npc[map.SpawnMarker].MaxLevel;
            spawningNpc.StartStatus = Enums.StatusAilment.OK;

            for (int i = 1; i <= 500; i++) {
                int X = Server.Math.Rand(x - range, x + range + 1);
                int Y = Server.Math.Rand(y - range, y + range + 1);

                // Check if the tile is walkable
                if (X >= 0 && X <= map.MaxX && Y >= 0 && Y <= map.MaxY && map.Tile[X, Y].Type == Enums.TileType.Walkable) {
                    spawningNpc.SpawnX = X;
                    spawningNpc.SpawnY = Y;
                    break;
                }
            }

            if (spawningNpc.SpawnX > -1) map.SpawnNpc(spawningNpc);

            map.SpawnMarker++;
            if (map.SpawnMarker >= map.Npc.Count) map.SpawnMarker = 0;

        }
        
        public static void BlowBack(ICharacter character, IMap map, Enums.Direction dir, PacketHitList hitlist) {
        	if (HasAbility(character, "Suction Cups")) {
        		hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Suction Cups prevented it from being blown back!", Text.WhiteSmoke), character.X, character.Y, 10);
        	} else if (character.HasActiveItem(111)) {
            	hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s Iron Ball prevented it from being blown back!", Text.WhiteSmoke), character.X, character.Y, 10);
        	} else {
	        	WarpToNearestWall(character, map, dir, hitlist);	
	        	hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " was blown back!", Text.WhiteSmoke), character.X, character.Y, 10);
        	}
        }
        
        public static void Pounce(ICharacter character, IMap map, Enums.Direction dir, bool checkAllies, bool checkFoes, PacketHitList hitlist) {
        	WarpToNearestWallOrChar(character, map, dir, checkAllies, checkFoes, hitlist);	
	        hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " went flying forward!", Text.WhiteSmoke), character.X, character.Y, 10);
        }
        
        
        public static void WarpToNearestWallOrChar(ICharacter character, IMap map, Enums.Direction dir, bool checkAllies, bool checkFoes, PacketHitList hitlist) {
        	int blockX = character.X;
            int blockY = character.Y;
            
            
            FindNearestBlock(map, dir, ref blockX, ref blockY);
						
            int candX = blockX;
            int candY = blockY;
            			
            TargetCollection checkedTargets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.LineUntilHit, 100, map, character, character.X, character.Y, dir, checkAllies, checkFoes, false);
            
            if (checkedTargets.Count > 0) {
            	candX = checkedTargets[0].X;
            	candY = checkedTargets[0].Y;
            	if (candX != character.X || candY != character.Y) {
            		MoveInDirection((Enums.Direction)(((int)dir + 1) % 2 + (int)dir / 2 * 2), 1, ref candX, ref candY);
            	}
            }
			
			
			
			if ((candX - character.X)*(candX - character.X) + (candY - character.Y)*(candY - character.Y) <
				(blockX - character.X)*(blockX - character.X) + (blockY - character.Y)*(blockY - character.Y)) {
				character.Y = candY;
            	character.X = candX;
			} else {
				character.Y = blockY;
            	character.X = blockX;
			}
			
            if (character.CharacterType == Enums.CharacterType.Recruit) {
                PacketBuilder.AppendPlayerXY(((Recruit)character).Owner, hitlist);
            } else if (character.CharacterType == Enums.CharacterType.MapNpc) {
                PacketBuilder.AppendNpcXY(map, hitlist, ((MapNpc)character).MapSlot);
            }
        }
        
        public static void WarpToNearestWall(ICharacter character, IMap map, Enums.Direction dir, PacketHitList hitlist) {
        	int blockX = character.X;
            int blockY = character.Y;
            FindNearestBlock(map, dir, ref blockX, ref blockY);

            character.Y = blockY;
            character.X = blockX;

            if (character.CharacterType == Enums.CharacterType.Recruit) {
                PacketBuilder.AppendPlayerXY(((Recruit)character).Owner, hitlist);
            } else if (character.CharacterType == Enums.CharacterType.MapNpc) {
                PacketBuilder.AppendNpcXY(map, hitlist, ((MapNpc)character).MapSlot);
            }
        }
        
        
        public static void WarpToStairs(ICharacter character, IMap map, PacketHitList hitlist, int range) {
        	 int stairX = -1, stairY = -1;
        	 for (int x = 0; x < map.MaxX; x++) {
                                        for (int y = 0; y < map.MaxY; y++) {
                                            if (map.Tile[x, y].Type == Enums.TileType.RDungeonGoal) {
                                                stairX = x;
                                                stairY = y;
                                                break;
                                            }
                                        }
                                    }
            
            for (int i = 1; i <= 500; i++) {
                int X = Server.Math.Rand(stairX - range, stairX + range + 1);
                int Y = Server.Math.Rand(stairY - range, stairY + range + 1);

                // Check if the tile is walkable
                if (X >= 0 && X <= map.MaxX && Y >= 0 && Y <= map.MaxY && map.Tile[X, Y].Type == Enums.TileType.Walkable) {
                    PointWarp(character, map, hitlist, X, Y, true);
                    break;
                }
            }

        }
        
        public static void PointWarp(ICharacter character, IMap warpMap, PacketHitList hitlist, int X, int Y) {
        	PointWarp(character, warpMap, hitlist, X, Y, true);
        }

        public static void PointWarp(ICharacter character, IMap warpMap, PacketHitList hitlist, int X, int Y, bool msg) {
            if (HasAbility(character, "Suction Cups")) {
            	if (msg) {
        			hitlist.AddPacketToMap(warpMap, PacketBuilder.CreateBattleMsg(character.Name + "'s Suction Cups prevented it from being warped!", Text.WhiteSmoke), character.X, character.Y, 10);
        		}
        	} else if (character.HasActiveItem(111)) {
            	if (msg) {
        			hitlist.AddPacketToMap(warpMap, PacketBuilder.CreateBattleMsg(character.Name + "'s Iron Ball prevented it from being warped!", Text.WhiteSmoke), character.X, character.Y, 10);
        		}
        	} else {
	            PacketHitList.MethodStart(ref hitlist);
	
				if (msg) {
					hitlist.AddPacketToMap(warpMap, PacketBuilder.CreateBattleMsg(character.Name + " warped!", Text.WhiteSmoke), character.X, character.Y, 10);
				}
	
	            character.X = X;
	            character.Y = Y;
				
	            if (character.CharacterType == Enums.CharacterType.Recruit) {
	                PacketBuilder.AppendPlayerXY(((Recruit)character).Owner, hitlist);
	            } else if (character.CharacterType == Enums.CharacterType.MapNpc) {
	                PacketBuilder.AppendNpcXY(warpMap, hitlist, ((MapNpc)character).MapSlot);
	            }
	
	            PacketHitList.MethodEnded(ref hitlist);
            }
        }

        public static void RandomWarp(ICharacter character, IMap warpMap, bool msg, PacketHitList hitlist) {
            RandomWarp(character, warpMap, msg, hitlist, 0, warpMap.MaxX, 0, warpMap.MaxY);
        }

        public static void RandomWarp(ICharacter character, IMap warpMap, bool msg, PacketHitList hitlist, int startX, int endX, int startY, int endY) {
            if (HasAbility(character, "Suction Cups")) {
            	if (msg) {
        			hitlist.AddPacketToMap(warpMap, PacketBuilder.CreateBattleMsg(character.Name + "'s Suction Cups prevented it from being warped!", Text.WhiteSmoke), character.X, character.Y, 10);
        		}
        	} else if (character.HasActiveItem(111)) {
            	if (msg) {
        			hitlist.AddPacketToMap(warpMap, PacketBuilder.CreateBattleMsg(character.Name + "'s Iron Ball prevented it from being warped!", Text.WhiteSmoke), character.X, character.Y, 10);
        		}
        	} else {
	            PacketHitList.MethodStart(ref hitlist);
	
	            if (startX < 0) startX = 0;
	            if (startX > warpMap.MaxX) startX = warpMap.MaxX;
	            if (endX < 0) endX = 0;
	            if (endX > warpMap.MaxX) endX = warpMap.MaxX;
	            if (startY < 0) startY = 0;
	            if (startY > warpMap.MaxY) startY = warpMap.MaxY;
	            if (endY < 0) endY = 0;
	            if (endY > warpMap.MaxY) endY = warpMap.MaxY;
	
	            int x, y;
	
	            FindFreeTile(warpMap, startX, startY, endX, endY, out x, out y);
				
	            if (x == -1) {
	            	if (msg) {
	                	hitlist.AddPacketToMap(warpMap, PacketBuilder.CreateBattleMsg(character.Name + " didn't warp.", Text.BrightRed), character.X, character.Y, 10);
	                }
	            } else {
	                PointWarp(character, warpMap, hitlist, x, y);
	            }
	            PacketHitList.MethodEnded(ref hitlist);
            }
        }

        public static void FindFreeTile(IMap map, int startX, int startY, int endX, int endY, out int freeX, out int freeY) {
            freeX = -1;
            freeY = -1;
            for (int i = 1; i <= 500; i++) {
                int X = Server.Math.Rand(startX, endX + 1);
                int Y = Server.Math.Rand(startY, endY + 1);

                // Check if the tile is walkable or arena
                if (X >= 0 && X <= map.MaxX && Y >= 0 && Y <= map.MaxY) {
                    if (map.Tile[X, Y].Type == Enums.TileType.Walkable || map.Tile[X, Y].Type == Enums.TileType.Arena) {
                        freeX = X;
                        freeY = Y;
                        break;
                    }
                }
            }
            
            if (freeX == -1) {
            	for (int X = 0; X < map.MaxX; X++) {
            		for (int Y = 0; Y < map.MaxY; Y++) {
            			if (map.Tile[X, Y].Type == Enums.TileType.Walkable || map.Tile[X, Y].Type == Enums.TileType.Arena) {
	                        freeX = X;
	                        freeY = Y;
	                        break;
	                    }
            		}
            	}
            }
        }

		public static void ClearChamber(IMap map, int startX, int startY, int endX, int endY, PacketHitList hitlist) {
			if (map.MapType != Enums.MapType.RDungeonMap) return;
			PacketHitList.MethodStart(ref hitlist);
			int[,] intArray = new int[map.MaxX + 1, map.MaxY + 1];
			for (int x = startX; x <= endX; x++) {
				for (int y = startY; y <= endY; y++) {
					if (map.Tile[x,y].Type == Enums.TileType.ScriptedSign) {
						if (map.Tile[x,y].Data1 == 9) {
							if (map.Tile[x, y].String1.IsNumeric()) {
								map.SpawnItem(map.Tile[x, y].String1.ToInt(), 1, false, false, map.Tile[x, y].String2, x, y, null);
							}
							intArray[x, y] = map.Tile[x, y].String3.ToInt();
							RDungeonFloorGen.AmbiguateTile(map.Tile[x, y]);
						} else if (map.Tile[x,y].Data1 == 10) {
							intArray[x, y] = 3;
							RDungeonFloorGen.AmbiguateTile(map.Tile[x, y]);
							hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("magic161.wav"), x, y, 10);
							hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("The chamber was opened!", Text.WhiteSmoke), x, y, 10);
						}
					} else if (map.Tile[x, y].RDungeonMapValue >= 1280 && map.Tile[x, y].RDungeonMapValue < 1536) {
						intArray[x, y] = 1025;
						RDungeonFloorGen.AmbiguateTile(map.Tile[x, y]);
						map.Tile[x, y].Type = Enums.TileType.Blocked;
					}
				}
			}
			DungeonArrayFloor.TextureDungeon(intArray, startX+1, startY+1, endX-1, endY-1);
			for (int x = startX; x <= endX; x++) {
            	for (int y = startY; y <= endY; y++) {
                	map.Tile[x, y].RDungeonMapValue = intArray[x, y];
                }
            }
                                
            RDungeonFloorGen.TextureDungeonMap((RDungeonMap)map, startX, startY, endX, endY);
                                
            for (int x = startX; x <= endX; x++) {
            	for (int y = startY; y <= endY; y++) {
                	hitlist.AddPacketToMap(map, PacketBuilder.CreateTilePacket(x, y, map));
                }
            }
            
			PacketHitList.MethodEnded(ref hitlist);
		}

        public static bool IsScriptedTileBlocked(IMap map, int x, int y) {
        	//Messenger.AdminMsg(map.Name + " X" + x + "Y" + y + " S" + map.Tile[x, y].Data1, Text.Yellow);
            switch (map.Tile[x, y].Data1) {
                case 24:
                    return true;
                case 47:
                    return true;
                case 65:
                    return true;
                default:
                    return false;
            }
        }


        public static void BlockPlayer(Client client) {
            switch (client.Player.Direction) {
                case Enums.Direction.Up: {
                        Messenger.PlayerXYWarp(client, client.Player.X, client.Player.Y + 1);

                    }
                    break;
                case Enums.Direction.Down: {
                        Messenger.PlayerXYWarp(client, client.Player.X, client.Player.Y - 1);

                    }
                    break;
                case Enums.Direction.Left: {
                        Messenger.PlayerXYWarp(client, client.Player.X + 1, client.Player.Y);

                    }
                    break;
                case Enums.Direction.Right: {
                        Messenger.PlayerXYWarp(client, client.Player.X - 1, client.Player.Y);

                    }
                    break;
            }
        }
        
        
    public static bool IsHardModeEntrance(IMap map) {
    	if (map.MapType == Enums.MapType.Instanced) {
                switch (((InstancedMap)map).MapBase) {
                	case 1624://tiny grotto entrance
                	case 1830://lenile entrance
                	case 1616://cliffside entrance
                	case 1659://thunderstorm forest entrance
                	case 1657://mt. barricade
                	case 1683://winden forest
                	case 1695://sauna
                	case 1686://jailbreak
                	return true;
                }
        }
        return false;
    }

    }
}
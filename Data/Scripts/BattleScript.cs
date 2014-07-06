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
    using Server.Database;

    public partial class Main {

        public static void BeforeMoveUsed(BattleSetup setup) {
            try {
                ExtraStatus status;
                setup.AttackerMultiplier = 1000;
                bool sayMove = false;
                bool semiInvul = false;

                //For forcing the next attack
                //of chargeup attacks (solarbeam/skull bash/razor wind/sky attack/dig/fly/etc.
                if (setup.Attacker.VolatileStatus.GetStatus("SolarBeam") != null) {
                    setup.moveSlot = -1;
                    setup.moveIndex = 218;
                    sayMove = true;
                } else if (setup.Attacker.VolatileStatus.GetStatus("SkullBash") != null) {
                    setup.moveSlot = -1;
                    setup.moveIndex = 424;
                    sayMove = true;
                } else if (setup.Attacker.VolatileStatus.GetStatus("RazorWind") != null) {
                    setup.moveSlot = -1;
                    setup.moveIndex = 243;
                    sayMove = true;
                } else if (setup.Attacker.VolatileStatus.GetStatus("SkyAttack") != null) {
                    setup.moveSlot = -1;
                    setup.moveIndex = 345;
                    sayMove = true;
                } else if (setup.Attacker.VolatileStatus.GetStatus("FocusPunch") != null) {
                    setup.moveSlot = -1;
                    setup.moveIndex = 441;
                    sayMove = true;
                } else if (setup.Attacker.VolatileStatus.GetStatus("Bide") != null) {
                    setup.moveSlot = -1;
                    setup.moveIndex = 248;
                    sayMove = true;
                } else if (setup.Attacker.VolatileStatus.GetStatus("Avalanche") != null) {
                    setup.moveSlot = -1;
                    setup.moveIndex = 362;
                    sayMove = true;
                } else if (setup.Attacker.VolatileStatus.GetStatus("VitalThrow") != null) {
                    setup.moveSlot = -1;
                    setup.moveIndex = 321;
                    sayMove = true;
                } else if (setup.Attacker.VolatileStatus.GetStatus("SkyDrop:1") != null) {
                    setup.moveSlot = -1;
                    setup.moveIndex = 640;
                    sayMove = true;
                } else {
                    status = setup.Attacker.VolatileStatus.GetStatus("SemiInvul");
                    if (status != null) {
                        if (status.Tag == "Dig") {
                            setup.moveSlot = -1;
                            setup.moveIndex = 146;
                            sayMove = true;
                        } else if (status.Tag == "Bounce") {
                            setup.moveSlot = -1;
                            setup.moveIndex = 73;
                            sayMove = true;
                        } else if (status.Tag == "Fly") {
                            setup.moveSlot = -1;
                            setup.moveIndex = 423;
                            sayMove = true;
                        } else if (status.Tag == "ShadowForce") {
                            setup.moveSlot = -1;
                            setup.moveIndex = 42;
                            sayMove = true;
                        } else if (status.Tag == "Dive") {
                            setup.moveSlot = -1;
                            setup.moveIndex = 297;
                            sayMove = true;
                        }
                    	RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "SemiInvul", setup.PacketStack, false);
                    	semiInvul = true;
                    }
                }

                //of rampage attacks
                status = setup.Attacker.VolatileStatus.GetStatus("Rampage");
                if (status != null) {
                    
                        setup.moveSlot = -1;
                        setup.moveIndex = status.Tag.ToInt();
                        sayMove = true;
                }

                //of rolling attacks
                status = setup.Attacker.VolatileStatus.GetStatus("Rolling");
                if (status != null) {
                    setup.moveSlot = -1;
                    setup.moveIndex = status.Tag.ToInt();
                    sayMove = true;
                }


                //cancel fury cutter multipliers if using a different move
                if (setup.Attacker.VolatileStatus.GetStatus("FuryCutter") != null) {
                    
                        if (setup.moveIndex != 175) {
                            RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "FuryCutter", setup.PacketStack);
                        }
                    
                }

                if (setup.moveSlot > -1 && setup.moveSlot < 4) {
                    setup.moveIndex = setup.Attacker.Moves[setup.moveSlot].MoveNum;
                    sayMove = true;
                }
                if (setup.moveIndex < 1) {
                	setup.moveIndex = -1;
                    setup.SetupMove(MoveManager.StandardAttack);
                } else {
                    setup.SetupMove(MoveManager.Moves[setup.moveIndex]);
                    if (GetBattleTagArg(setup.BattleTags, "InvokedMove", 0) != null ||
                    	GetBattleTagArg(setup.BattleTags, "OrderedMove", 0) != null) {
                    	sayMove = true;
                    }
                }
				
				
				//surf check
                if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit &&
                        setup.moveIndex == 462) {
                    int x = setup.Attacker.X;
                    int y = setup.Attacker.Y;
                    MoveInDirection(setup.Attacker.Direction, 1, ref x, ref y);
                    if (x >= 0 && x <= setup.AttackerMap.MaxX && y >= 0 && y <= setup.AttackerMap.MaxY) {
                    	Tile tile = ((Recruit)setup.Attacker).Owner.Player.Map.Tile[x, y];
	                    if (tile.Type == Enums.TileType.ScriptedSign) {
	                        if (tile.Data1 == 17) {
	                            Messenger.PlayerWarp(((Recruit)setup.Attacker).Owner, tile.String1.ToInt(), tile.String2.ToInt(), tile.String3.ToInt());
	                            setup.Cancel = true;
	                            return;
	                        }
	                    }
                    }
                }
                //fly check
                if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit &&
                        setup.moveIndex == 423) {
                        Tile tile = ((Recruit)setup.Attacker).Owner.Player.Map.Tile[setup.Attacker.X, setup.Attacker.Y];
                    if (tile.Type == Enums.TileType.Scripted) {
	                    if (tile.Data1 == 14) {
	                    	//Messenger.PlayerMsg(((Recruit)setup.Attacker).Owner, "But it failed...", Text.BrightRed);
	                    	//setup.Cancel = true;
	                    	//return;
	                        Messenger.PlayerWarp(((Recruit)setup.Attacker).Owner, tile.String1.ToInt(), tile.String2.ToInt(), tile.String3.ToInt());
	                        setup.Cancel = true;
	                        return;
	                    }
	                }
                }
                //Dive check
                if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit &&
                        setup.moveIndex == 297) {
                        Tile tile = ((Recruit)setup.Attacker).Owner.Player.Map.Tile[setup.Attacker.X, setup.Attacker.Y];
                    if (tile.Type == Enums.TileType.Scripted) {
	                    if (tile.Data1 == 76) {
	                    	//Messenger.PlayerMsg(((Recruit)setup.Attacker).Owner, "But it failed...", Text.BrightRed);
	                    	//setup.Cancel = true;
	                    	//return;
	                        Messenger.PlayerWarp(((Recruit)setup.Attacker).Owner, tile.String1.ToInt(), tile.String2.ToInt(), tile.String3.ToInt());
	                        setup.Cancel = true;
	                        return;
	                    }
	                }
                }
                //rock climb check
                if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit &&
                        setup.moveIndex == 456) {
                    int x = setup.Attacker.X;
                    int y = setup.Attacker.Y;
                    MoveInDirection(setup.Attacker.Direction, 1, ref x, ref y);
                    if (x >= 0 && x <= setup.AttackerMap.MaxX && y >= 0 && y <= setup.AttackerMap.MaxY) {
                    	Tile tile = ((Recruit)setup.Attacker).Owner.Player.Map.Tile[x, y];
	                    if (tile.Type == Enums.TileType.ScriptedSign) {
	                        if (tile.Data1 == 20) {
	                            setup.Attacker.X = x;
	            				setup.Attacker.Y = y;
	            				PacketBuilder.AppendPlayerXY(((Recruit)setup.Attacker).Owner, setup.PacketStack);
	                            setup.Cancel = true;
	                            return;
	                        }
	                    }
                    }
                }

                //unfinished move check
                if (setup.moveIndex > 0) {
                    if (MoveManager.Moves[setup.moveIndex].Name.EndsWith("\'") || MoveManager.Moves[setup.moveIndex].Name.EndsWith("`")) {
                        if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit && Ranks.IsAllowed(((Recruit)setup.Attacker).Owner, Enums.Rank.Moniter)) {

                        } else {
                            setup.Cancel = true;
                            return;
                        }
                    }
                    
                }
                

                if (sayMove) {
                    setup.Move.Name = setup.Attacker.Name + " used " + setup.Move.Name + "!";
                } else {
                    setup.Move.Name = "";
                }

				if (GetBattleTagArg(setup.BattleTags, "InvokedMove", 0) == null) {
	                //check against cooldown
	                if (GetBattleTagArg(setup.BattleTags, "OrderedMove", 0) == null) {
		                if (setup.Attacker.PauseTimer == null) {
		                    setup.Attacker.PauseTimer = new TickCount(Core.GetTickCount().Tick);
		                }
		                if (setup.Attacker.PauseTimer.Tick > Core.GetTickCount().Tick) {
		                    setup.Cancel = true;
		                    return;
		                }
		                if (setup.Attacker.AttackTimer == null ||
		                    setup.Move.AdditionalEffectData1 == 9 ||
		                    setup.Move.AdditionalEffectData1 == 128 ||
		                    setup.Move.AdditionalEffectData1 == 132) {
		                    setup.Attacker.AttackTimer = new TickCount(Core.GetTickCount().Tick);
		                }
		                if (setup.Attacker.AttackTimer.Tick > Core.GetTickCount().Tick) {
		                    setup.Cancel = true;
		                    return;
		                }
	                }
	
	                //check against freeze/paralyze/sleep
	                if (setup.Attacker.StatusAilment == Enums.StatusAilment.Freeze) {
	                    if (Server.Math.Rand(0, 3) == 0 && !CheckStatusProtection(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.OK.ToString(), false, setup.PacketStack)) {
	                        SetStatusAilment(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.OK, 0, setup.PacketStack);
	                    } else if ((setup.Move.AdditionalEffectData1 == 37 || setup.Move.AdditionalEffectData1 == 38) && !CheckStatusProtection(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.OK.ToString(), true, setup.PacketStack)) {
	
	                        SetStatusAilment(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.OK, 0, setup.PacketStack);
	                    } else {
	                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " is frozen solid!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
	                        setup.Cancel = true;
	                        return;
	                    }
	                } else if (setup.Attacker.StatusAilment == Enums.StatusAilment.Paralyze) {
	                    if (Server.Math.Rand(0, 5) == 0) {
	                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " is paralyzed!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
	                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSoundPacket("magic184.wav"));
	                        setup.Cancel = true;
	                        return;
	                    }
	                } else if (setup.Attacker.StatusAilment == Enums.StatusAilment.Sleep) {
	                    setup.Attacker.StatusAilmentCounter--;
	                    if (setup.Attacker.StatusAilmentCounter <= 0 && !CheckStatusProtection(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.OK.ToString(), false, setup.PacketStack)) {
	                        SetStatusAilment(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.OK, 0, setup.PacketStack);
	                    } else if (setup.Move.AdditionalEffectData1 == 35 || setup.Move.Data1 == 36) {
	                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " is fast asleep...", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
	                    } else {
	                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " is fast asleep...", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
	                        setup.Cancel = true;
	                        return;
	                    }
	                }
	
	                //check against wrapping (victim)
	                int trapdmg;
                    status = setup.Attacker.VolatileStatus.GetStatus("Bind:0");
	                if (status != null) {
	                    status.Counter--;
                        if (status.Target == null) {
                            setup.Attacker.VolatileStatus.Remove(status);
                        } else if (status.Counter <= 0 || !MoveProcessor.IsInAreaRange(1, setup.Attacker.X, setup.Attacker.Y, status.Target.X, status.Target.Y) ||
	                        status.Target.MapID != setup.AttackerMap.MapID || setup.Move.AdditionalEffectData1 == 85) {
	                        RemoveBondedExtraStatus(setup.Attacker, setup.AttackerMap, "Bind:0", setup.PacketStack, true);
	                    } else {
	                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " is struggling to get free!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
	                        setup.Cancel = true;
	                        return;
	                    }
	                }
                    status = setup.Attacker.VolatileStatus.GetStatus("Bind:1");
                    if (status != null) {
                        if (status.Target == null) {
                            setup.Attacker.VolatileStatus.Remove(status);
                        } else if (!MoveProcessor.IsInAreaRange(1, setup.Attacker.X, setup.Attacker.Y, status.Target.X, status.Target.Y) ||
	                        status.Target.MapID != setup.AttackerMap.MapID) {
	                        RemoveBondedExtraStatus(setup.Attacker, setup.AttackerMap, "Bind:1", setup.PacketStack, true);
	                    } else {
	                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s attack continues!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
	                        if (status.Tag == "1") {
                            	trapdmg = status.Target.MaxHP / 16;
                            } else {
                            	trapdmg = status.Target.MaxHP / 32;
                            }
	                        if (trapdmg < 1) trapdmg = 1;
	                        if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
	                            DamageCharacter(status.Target, MapManager.RetrieveActiveMap(status.Target.MapID), trapdmg, Enums.KillType.Player, setup.PacketStack, true);
	                        } else if (setup.Attacker.CharacterType == Enums.CharacterType.MapNpc) {
	                            DamageCharacter(status.Target, MapManager.RetrieveActiveMap(status.Target.MapID), trapdmg, Enums.KillType.Npc, setup.PacketStack, true);
	                        }
	                        setup.Cancel = true;
	                        return;
	                    }
	                }

                    status = setup.Attacker.VolatileStatus.GetStatus("Clamp:0");
	                if (status != null) {
	                    status.Counter--;
	                    if (status.Counter <= 0 ||
	                        !MoveProcessor.IsInAreaRange(1, setup.Attacker.X, setup.Attacker.Y, status.Target.X, status.Target.Y) ||
	                        status.Target.MapID != setup.AttackerMap.MapID ||
	                        setup.Move.AdditionalEffectData1 == 85) {
	                        RemoveBondedExtraStatus(setup.Attacker, setup.AttackerMap, "Clamp:0", setup.PacketStack, true);
	                    } else {
	                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " is struggling to get free!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
	                        setup.Cancel = true;
	                        return;
	                    }
	                }
                    status = setup.Attacker.VolatileStatus.GetStatus("Clamp:1");
	                if (status != null) {
                        if (status.Target == null) {
                            setup.Attacker.VolatileStatus.Remove(status);
                        } else if (!MoveProcessor.IsInAreaRange(1, setup.Attacker.X, setup.Attacker.Y, status.Target.X, status.Target.Y) ||
                            status.Target.MapID != setup.AttackerMap.MapID) {
	                        RemoveBondedExtraStatus(setup.Attacker, setup.AttackerMap, "Clamp:1", setup.PacketStack, true);
	                    } else {
		                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s attack continues!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                            if (status.Tag == "1") {
                            	trapdmg = status.Target.MaxHP / 16;
                            } else {
                            	trapdmg = status.Target.MaxHP / 32;
                            }
		                    if (trapdmg < 1) trapdmg = 1;
		                    if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                                DamageCharacter(status.Target, MapManager.RetrieveActiveMap(status.Target.MapID), trapdmg, Enums.KillType.Player, setup.PacketStack, true);
		                    } else if (setup.Attacker.CharacterType == Enums.CharacterType.MapNpc) {
                                DamageCharacter(status.Target, MapManager.RetrieveActiveMap(status.Target.MapID), trapdmg, Enums.KillType.Npc, setup.PacketStack, true);
		                    }
		                    setup.Cancel = true;
		                    return;
	                    }
	                }

                    status = setup.Attacker.VolatileStatus.GetStatus("Wrap:0");
	                if (status != null) {
	                    status.Counter--;
	                    if (status.Counter <= 0 || !MoveProcessor.IsInAreaRange(1, setup.Attacker.X, setup.Attacker.Y, status.Target.X, status.Target.Y) ||
                            status.Target.MapID != setup.AttackerMap.MapID || setup.Move.AdditionalEffectData1 == 85) {
	                        RemoveBondedExtraStatus(setup.Attacker, setup.AttackerMap, "Wrap:0", setup.PacketStack, true);
	                    } else {
	                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " is struggling to get free!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
	                        setup.Cancel = true;
	                        return;
	                    }
	                }
                    status = setup.Attacker.VolatileStatus.GetStatus("Wrap:1");
	                if (status != null) {
                        if (status.Target == null) {
                            setup.Attacker.VolatileStatus.Remove(status);
                        } else if (!MoveProcessor.IsInAreaRange(1, setup.Attacker.X, setup.Attacker.Y, status.Target.X, status.Target.Y) ||
                            status.Target.MapID != setup.AttackerMap.MapID) {
	                        RemoveBondedExtraStatus(setup.Attacker, setup.AttackerMap, "Wrap:1", setup.PacketStack, true);
	                    } else {
		                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s attack continues!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                            if (status.Tag == "1") {
                            	trapdmg = status.Target.MaxHP / 16;
                            } else {
                            	trapdmg = status.Target.MaxHP / 32;
                            }
		                    if (trapdmg < 1) trapdmg = 1;
		                    if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                                DamageCharacter(status.Target, MapManager.RetrieveActiveMap(status.Target.MapID), trapdmg, Enums.KillType.Player, setup.PacketStack, true);
		                    } else if (setup.Attacker.CharacterType == Enums.CharacterType.MapNpc) {
                                DamageCharacter(status.Target, MapManager.RetrieveActiveMap(status.Target.MapID), trapdmg, Enums.KillType.Npc, setup.PacketStack, true);
		                    }
		                    setup.Cancel = true;
		                    return;
	                    }
	                }

                    status = setup.Attacker.VolatileStatus.GetStatus("SkyDrop:0");
	                if (status != null) {
                        status.Counter--;
                        if (status.Counter <= 0 || !MoveProcessor.IsInAreaRange(1, setup.Attacker.X, setup.Attacker.Y, status.Target.X, status.Target.Y) ||
                            status.Target.MapID != setup.AttackerMap.MapID) {
	                        RemoveBondedExtraStatus(setup.Attacker, setup.AttackerMap, "SkyDrop:0", setup.PacketStack, true);
	                    } else {
	                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " is struggling to get free!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
	                        setup.Cancel = true;
	                        return;
	                    }
	                }
                    status = setup.Attacker.VolatileStatus.GetStatus("SkyDrop:1");
	                if (status != null) {
                        if (status.Target == null) {
                            setup.Attacker.VolatileStatus.Remove(status);
                        } else if (!MoveProcessor.IsInAreaRange(1, setup.Attacker.X, setup.Attacker.Y, status.Target.X, status.Target.Y) ||
                            status.Target.MapID != setup.AttackerMap.MapID) {
	                        RemoveBondedExtraStatus(setup.Attacker, setup.AttackerMap, "SkyDrop:1", setup.PacketStack, true);
	                    } else {
	                    	RemoveBondedExtraStatus(setup.Attacker, setup.AttackerMap, "SkyDrop:1", setup.PacketStack, false);
	                        //execute the move
	                        setup.BattleTags.Add("SkyDrop");
	                    }
	                }

                //check against confusion
                    status = setup.Attacker.VolatileStatus.GetStatus("Confusion");
                if (status != null) {
                    status.Counter -= 2;
                    if (status.Counter <= 0) {
                        RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "Confusion", setup.PacketStack, true);
                    } else if (Server.Math.Rand(0, 2) == 0 && !(HasAbility(setup.Attacker, "Tangled Feet") && HasAbility(setup.Attacker, "Own Tempo"))) {
                        setup.moveSlot = -1;
                        setup.moveIndex = -1;
                        setup.SetupMove(MoveManager.StandardAttack);
                        setup.Move.TargetType = Enums.MoveTarget.User;
                        setup.Move.Name = setup.Attacker.Name + " attacked itself in confusion!";
                    }
                }
	
	                //check against attraction
                status = setup.Attacker.VolatileStatus.GetStatus("Attract");
	                if (status != null) {
                        status.Counter--;
                        if (status.Counter <= 0) {
	                        RemoveBondedExtraStatus(setup.Attacker, setup.AttackerMap, "Attract", setup.PacketStack, true);
	                    } else {
	
	                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " is immobilized by love!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
	                        setup.Cancel = true;
	                        return;
	                    }
	                }
	
	
	                if (setup.moveSlot > -1 && setup.moveSlot < 4) {
	                    if (setup.Attacker.Moves[setup.moveSlot].CurrentPP <= 0) {
	                    	bool noMovesLeft = true;
	                    	for (int i = 0; i < Constants.MAX_PLAYER_MOVES; i++) {
	                    		if (setup.Attacker.Moves[i].CurrentPP > 0 && !setup.Attacker.Moves[i].Sealed) {
	                    			noMovesLeft = false;
	                    			break;
	                    		}
	                    	}
	                    	if (noMovesLeft) {
	                    		setup.moveSlot = -1;
	                    		setup.moveIndex = 466;//struggle
	                    		setup.SetupMove(MoveManager.Moves[setup.moveIndex]);
                    			setup.Move.Name = setup.Attacker.Name + " used " + setup.Move.Name + "!";
	                    	} else {
		                        if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
		                            setup.PacketStack.AddPacket(((Recruit)setup.Attacker).Owner, PacketBuilder.CreateBattleMsg("There's no PP left for that move!", Text.BrightRed));
		                            PacketBuilder.AppendMovePPUpdate(((Recruit)setup.Attacker).Owner, setup.PacketStack, setup.moveSlot);
		                        }
		                        setup.Cancel = true;
		                        return;
	                        }
	
	
	                    } else if (setup.Attacker.Moves[setup.moveSlot].Sealed) {
	                        if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
	                            setup.PacketStack.AddPacket(((Recruit)setup.Attacker).Owner, PacketBuilder.CreateBattleMsg("The move is sealed!  It can't be used!", Text.BrightRed));
	                            PacketBuilder.AppendPlayerMoves(((Recruit)setup.Attacker).Owner, setup.PacketStack);
	                        }
	                        setup.Cancel = true;
	                        return;
	
	
	                    } else {
	                    	bool deductPP = true;
	                    	if (setup.Attacker.HasActiveItem(340)) deductPP = false;
	                        
	                        if (Server.Math.Rand(0, 4) <= 0 && HasActiveBagItem(setup.Attacker, 13, 0, 0)) {
	                        	deductPP = false;
	                        }
	                        if (deductPP) setup.Attacker.Moves[setup.moveSlot].CurrentPP--;
	                        
                            status = setup.Attacker.VolatileStatus.GetStatus("LastUsedMoveSlot");
                            if (status == null || status.Counter != setup.moveSlot) {
                                AddExtraStatus(setup.Attacker, setup.AttackerMap, "LastUsedMoveSlot", setup.moveSlot, null, "", setup.PacketStack);
                                AddExtraStatus(setup.Attacker, setup.AttackerMap, "TimesLastMoveUsed", 1, null, "", setup.PacketStack);
                            } else {
                                status = setup.Attacker.VolatileStatus.GetStatus("TimesLastMoveUsed");
                                if (status != null) {
                                    AddExtraStatus(setup.Attacker, setup.AttackerMap, "TimesLastMoveUsed", status.Counter + 1, null, "", setup.PacketStack);
                                } else {
                                    AddExtraStatus(setup.Attacker, setup.AttackerMap, "TimesLastMoveUsed", 1, null, "", setup.PacketStack);
                                }
                            }
	
	                        //send PP update to players
	                        if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
	                            PacketBuilder.AppendMovePPUpdate(((Recruit)setup.Attacker).Owner, setup.PacketStack, setup.moveSlot);
	                        }
	
	
	                        if (setup.Attacker.HasActiveItem(256) || setup.Attacker.HasActiveItem(257) || setup.Attacker.HasActiveItem(258)) {
	                            for (int i = 0; i < Constants.MAX_PLAYER_MOVES; i++) {
	                                if (i != setup.moveSlot) {
	                                    AddExtraStatus(setup.Attacker, setup.AttackerMap, "MoveSeal:" + i, 0, null, "", setup.PacketStack, false);
	                                }
	                            }
	                        }
	
	                    }
	                }// else if (setup.Move.EffectType == Enums.MoveType.SubHP) {
	                //    switch (setup.Move.Data2) {
	                //        case 1: {
	                //                if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
	                //                    ((Recruit)setup.Attacker).Owner.Player.TakeItem(setup.Move.Data3, 1);
	                //                }
	                //                break;
	                //            }
	
	                //    }
	                //}
                
                }

                //For the charging of chargeup attacks (solarbeam/skull bash/razor wind/sky attack/dig/fly/etc.
                if (setup.Attacker.VolatileStatus.GetStatus("SolarBeam") != null) {
                    RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "SolarBeam", setup.PacketStack);
                } else if (setup.Attacker.VolatileStatus.GetStatus("SkullBash") != null) {
                    RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "SkullBash", setup.PacketStack);
                } else if (setup.Attacker.VolatileStatus.GetStatus("RazorWind") != null) {
                    RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "RazorWind", setup.PacketStack);
                } else if (setup.Attacker.VolatileStatus.GetStatus("SkyAttack") != null) {
                    RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "SkyAttack", setup.PacketStack);
                } else if (setup.Attacker.VolatileStatus.GetStatus("FocusPunch") != null) {
                    RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "FocusPunch", setup.PacketStack, false);
                } else if (setup.Attacker.VolatileStatus.GetStatus("Bide") != null) {
                    setup.Move.Data1 = 30;
                    setup.Move.Data2 = setup.Attacker.VolatileStatus.GetStatus("Bide").Counter * 2;
                    RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "Bide", setup.PacketStack);
                } else if (setup.Attacker.VolatileStatus.GetStatus("Avalanche") != null) {
                    if (GetBattleTagArg(setup.BattleTags, "Avalanche", 0) != null) {
                    	setup.Move.Data1 *= 2;
                    }
                    RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "Avalanche", setup.PacketStack);
                } else if (setup.Attacker.VolatileStatus.GetStatus("VitalThrow") != null) {
                    RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "VitalThrow", setup.PacketStack);
                    if (GetBattleTagArg(setup.BattleTags, "VitalThrow", 0) != null) {
                    	
                    } else {
                    	setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " gave up on Vital Throw!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                    	setup.Cancel = true;
                        return;
                    }
                } else if (semiInvul) {
                    
                } else {
                    if (setup.Move.AdditionalEffectData1 == 60 && setup.Attacker.VolatileStatus.GetStatus("SuperCharge") == null) {
                        switch (setup.Move.AdditionalEffectData2) {
                            case 1: {//solarbeam
                                    if (GetCharacterWeather(setup.Attacker) != Enums.Weather.Sunny) {
                                        AddExtraStatus(setup.Attacker, setup.AttackerMap, "SolarBeam", 0, null, "", setup.PacketStack);
                                        setup.Cancel = true;
                                        return;
                                    }
                                }
                                break;
                            case 2: {//skull bash
                                    AddExtraStatus(setup.Attacker, setup.AttackerMap, "SkullBash", 0, null, "", setup.PacketStack);
                                    setup.Cancel = true;
                                    return;
                                }
                                break;
                            case 3: {//razor wind
                                    AddExtraStatus(setup.Attacker, setup.AttackerMap, "RazorWind", 0, null, "", setup.PacketStack);
                                    setup.Cancel = true;
                                    return;
                                }
                                break;
                            case 4: {//sky attack
                                    AddExtraStatus(setup.Attacker, setup.AttackerMap, "SkyAttack", 0, null, "", setup.PacketStack);
                                    setup.Cancel = true;
                                    return;
                                }
                                break;
                            case 5: {//focus punch
                                    AddExtraStatus(setup.Attacker, setup.AttackerMap, "FocusPunch", 0, null, "", setup.PacketStack);
                                    setup.Cancel = true;
                                    return;
                                }
                                break;
                            case 6: {//Dig
                            		bool passable = false;
                            		Tile tile = setup.AttackerMap.Tile[setup.Attacker.X, setup.Attacker.Y];
                            		if (tile.Type == Enums.TileType.Arena) {
                            			passable = true;
                            		} else if (tile.Type == Enums.TileType.Walkable || tile.Type == Enums.TileType.Hallway || tile.Type == Enums.TileType.NPCAvoid) {
                            			if (tile.Data1 == 0) passable = true;
                            		} else if (tile.Type == Enums.TileType.MobileBlock) {
                            			if (tile.Data1 <= 0) passable = true;
                            		}
                            		if (passable) {
                                    	AddExtraStatus(setup.Attacker, setup.AttackerMap, "SemiInvul", 20, null, "Dig", setup.PacketStack);
                                    } else {
                                    	setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " can't dig here!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                                    }
                                    setup.Cancel = true;
                                    return;
                                }
                                break;
                            case 7: {//Bounce
                                    AddExtraStatus(setup.Attacker, setup.AttackerMap, "SemiInvul", 8, null, "Bounce", setup.PacketStack);
                                    setup.Cancel = true;
                                    return;
                                }
                                break;
                            case 8: {//Fly
                                    AddExtraStatus(setup.Attacker, setup.AttackerMap, "SemiInvul", 12, null, "Fly", setup.PacketStack);
                                    setup.Cancel = true;
                                    return;
                                }
                                break;
                            case 9: {//Shadow Force
                                    AddExtraStatus(setup.Attacker, setup.AttackerMap, "SemiInvul", 14, null, "ShadowForce", setup.PacketStack);
                                    setup.Cancel = true;
                                    return;
                                }
                                break;
                            case 10: {//Dive
                            		bool passable = false;
                            		Tile tile = setup.AttackerMap.Tile[setup.Attacker.X, setup.Attacker.Y];
                            		if (tile.Type == Enums.TileType.Arena) {
                            			passable = true;
                            		} else if (tile.Type == Enums.TileType.Walkable || tile.Type == Enums.TileType.Hallway) {
                            			if (tile.Data1 == 1) passable = true;
                            		} else if (tile.Type == Enums.TileType.MobileBlock) {
                            			if (tile.Data1 == 2) passable = true;
                            		}
                            		if (passable) {
                                    	AddExtraStatus(setup.Attacker, setup.AttackerMap, "SemiInvul", 20, null, "Dive", setup.PacketStack);
                                    } else {
                                    	setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " can't dive here!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                                    }
                                    setup.Cancel = true;
                                    return;
                                }
                                break;
                        }
                    } else if (setup.Move.EffectType == Enums.MoveType.Scripted && setup.Move.Data1 == 68) {
                        AddExtraStatus(setup.Attacker, setup.AttackerMap, "Bide", 0, null, "", setup.PacketStack);
                        setup.Cancel = true;
                        return;
                    } else if (setup.Move.AdditionalEffectData1 == 186) {
                    	AddExtraStatus(setup.Attacker, setup.AttackerMap, "Avalanche", 0, null, "", setup.PacketStack);
                        setup.Cancel = true;
                        return;
                    } else if (setup.Move.AdditionalEffectData1 == 187) {
                    	AddExtraStatus(setup.Attacker, setup.AttackerMap, "VitalThrow", 0, null, "", setup.PacketStack);
                        setup.Cancel = true;
                        return;
                    } else if (setup.Move.AdditionalEffectData1 == 197 && GetBattleTagArg(setup.BattleTags, "SkyDrop", 0) == null) {//sky drop
                    	setup.Move.Name = "";
                    }
                }
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: BeforeMoveUsed", Text.Black);
                Messenger.AdminMsg("Slot: " + setup.moveSlot + ", Name: " + MoveManager.Moves[setup.moveIndex].Name, Text.Black);
                Messenger.AdminMsg("Map: " + setup.AttackerMap.MapID + " Name: " + setup.AttackerMap.Name, Text.Black);
                Messenger.AdminMsg(ex.ToString(), Text.Black);
            }
        }

        public static void AfterMoveExecuted(BattleSetup setup) {
        int point = 0;
            try {
                ExtraStatus status;
                
                if (setup.AttackerMap.TempStatus.GetStatus("Gravity") != null) {
                	if (setup.moveIndex == 73 || setup.moveIndex == 423 || 
                		setup.moveIndex == 380 || setup.moveIndex == 197 || 
                		setup.moveIndex == 338 || setup.moveIndex == 339 || 
                		setup.moveIndex == 610 || setup.moveIndex == 640) {
                	setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("But "+setup.Attacker.Name+"'s move failed due to gravity!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
	                setup.Cancel = true;
	                return;
	                }
	            }
	            
	            if (setup.Move.AdditionalEffectData1 == 35 && setup.Attacker.StatusAilment != Enums.StatusAilment.Sleep) {
	            	setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("But "+setup.Attacker.Name+" wasn't asleep!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
	                setup.Cancel = true;
	                return;
	            }
                
                if (setup.AttackerMap.TempStatus.GetStatus("WonderRoom") != null) {
                    if (setup.Move.MoveCategory == Enums.MoveCategory.Physical) {
                        setup.Move.MoveCategory = Enums.MoveCategory.Special;
                    } else if (setup.Move.MoveCategory == Enums.MoveCategory.Special) {
                        setup.Move.MoveCategory = Enums.MoveCategory.Physical;
                    }
                }

                //use the correct attack/sp.attack stats
                if (setup.Move.MoveCategory == Enums.MoveCategory.Physical) {
	                //proxy attack
	                status = setup.Attacker.VolatileStatus.GetStatus("ProxyAttack");
	                if (status != null) {
	                	setup.AttackStat = status.Counter;
	                } else {
	                    setup.AttackStat = setup.Attacker.Atk;
	                }
                } else if (setup.Move.MoveCategory == Enums.MoveCategory.Special) {
                //proxy special
                	status = setup.Attacker.VolatileStatus.GetStatus("ProxySpAtk");
	                if (status != null) {
	                	setup.AttackStat = status.Counter;
	                } else {
                    	setup.AttackStat = setup.Attacker.SpclAtk;
                    }
                }
                
                status = setup.Attacker.VolatileStatus.GetStatus("Shocker");
                if (status != null) {
                	switch (setup.Move.TargetType) {
                		case Enums.MoveTarget.Foes:
                		case Enums.MoveTarget.AllAlliesButUser: {
                				setup.Move.TargetType = Enums.MoveTarget.AllButUser;
                			} break;
                		case Enums.MoveTarget.UserAndFoe:
                		case Enums.MoveTarget.UserAndAllies: {
                				setup.Move.TargetType = Enums.MoveTarget.All;
                			} break;
                	}
                }
                
                
                status = setup.Attacker.VolatileStatus.GetStatus("Longtoss");
                if (status != null) {
                	if (setup.Move.RangeType == Enums.MoveRange.LineUntilHit ||
                		setup.Move.RangeType == Enums.MoveRange.FrontOfUserUntil ||
                		setup.Move.RangeType == Enums.MoveRange.ArcThrow) {
                		setup.Move.Range = 50;
                	}
                }
                
                status = setup.Attacker.VolatileStatus.GetStatus("Pierce");
                if (status != null || setup.Attacker.HasActiveItem(699)) {
                	if (setup.Move.RangeType == Enums.MoveRange.LineUntilHit ||
                		setup.Move.RangeType == Enums.MoveRange.FrontOfUserUntil) {
                		setup.Move.RangeType = Enums.MoveRange.FrontOfUser;
                	}
                }

                //attacker-based move modification
                if (setup.Move.EffectType == Enums.MoveType.Scripted) {
                    switch (setup.Move.Data1) {
                        case 64: {//curse
                                if (setup.Attacker.Type1 == Enums.PokemonType.Ghost ||
                                    setup.Attacker.Type2 == Enums.PokemonType.Ghost) {
                                    setup.Move.RangeType = Enums.MoveRange.FrontOfUser;
                                    setup.Move.TargetType = Enums.MoveTarget.Foes;
                                    setup.Move.TravelingAnim.AnimationIndex = 77;
                                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSoundPacket("magic3.wav"), setup.Attacker.X, setup.Attacker.Y, 10);
                                    int curseDmg = setup.Attacker.HP / 4;
                                    DamageCharacter(setup.Attacker, setup.AttackerMap, curseDmg, Enums.KillType.Other, setup.PacketStack, true);
                                    if (curseDmg < 1) curseDmg = 1;
                                    setup.BattleTags.Add("Curse:" + curseDmg);
                                }
                            }
                            break;
                        case 75: {//present
                                int rand = Server.Math.Rand(0, 4);
                                setup.BattleTags.Add("Present:" + rand);
                            }
                            break;
                        case 88: {//teleport
                                IMap warpMap = setup.AttackerMap;

                                for (int i = 1; i <= 500; i++) {
                                    int X = Server.Math.Rand(0, warpMap.MaxX + 1);
                                    int Y = Server.Math.Rand(0, warpMap.MaxY + 1);

                                    // Check if the tile is walkable
                                    if (X >= 0 && X <= warpMap.MaxX && Y >= 0 && Y <= warpMap.MaxY && warpMap.Tile[X, Y].Type == Enums.TileType.Walkable) {
                                        setup.BattleTags.Add("Teleport:" + X + ":" + Y);
                                        break;
                                    }
                                }
                            }
                            break;
                        case 129: {//psycho shift
                                int statusAilment = (int)setup.Attacker.StatusAilment;
                                int statusCounter = setup.Attacker.StatusAilmentCounter;
                                setup.BattleTags.Add("PsychoShift:" + statusAilment + ":" + statusCounter);
                                if (setup.Attacker.StatusAilment == Enums.StatusAilment.OK) {
                                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("But " + setup.Attacker.Name + " didn't have a status problem to shift!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                                } else {
                                    SetStatusAilment(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.OK, 0, setup.PacketStack);
                                }
                            }
                            break;
                        case 159: {//ally switch
                                TargetCollection friendlyTargets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Floor, 0, setup.AttackerMap, setup.Attacker, setup.Attacker.X, setup.Attacker.Y, Enums.Direction.Up, false, true, false);
                                int size = friendlyTargets.Count;
                                string battleTag = setup.Attacker.X + ":" + setup.Attacker.Y;
                                for (int i = 0; i < friendlyTargets.Friends.Count; i++) {
                                    battleTag += ":" + friendlyTargets.Friends[i].X + ":" + friendlyTargets.Friends[i].Y;
                                }
                                setup.BattleTags.Add("AllySwitchCount:1");
                                setup.BattleTags.Add("AllySwitch:" + battleTag);

                            }
                            break;
                        case 162: {//rollcall
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " called over everyone on the floor!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                                setup.BattleTags.Add("Rollcall:" + setup.Attacker.X + ":" + setup.Attacker.Y);
                            }
                            break;
                        case 190: {//copy move
                        		switch (setup.Move.Data2) {
                        			case 2: {// assist
                        					TargetCollection targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Floor, 0, setup.AttackerMap, setup.Attacker, setup.Attacker.X, setup.Attacker.Y, Enums.Direction.Up, false, true, false);
                        					List<int> moveNums = new List<int>();
                                            for (int i = 0; i < targets.Friends.Count; i++) {
                                                for (int j = 0; j < Constants.MAX_PLAYER_MOVES; j++) {
                                                    if (targets.Friends[i].Moves[j].MoveNum > 0 && !moveNums.Contains(targets.Friends[i].Moves[j].MoveNum)) {
                                                        moveNums.Add(targets.Friends[i].Moves[j].MoveNum);
                                                    }
                                                }
                                            }
			                                string tag = "Assist:";
			                                if (moveNums.Count > 0) {
			                                	tag += moveNums[0];
			                                } else {
			                                	tag += "0";
			                                }
			                                for (int i = 1; i < moveNums.Count; i++) {
			                                	tag += ("," + moveNums[i]);
			                                }
                        					setup.BattleTags.Add(tag);
                        				}
                        				break;
                        			case 3: {//me first
                        					TargetCollection targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Floor, 0, setup.AttackerMap, setup.Attacker, setup.Attacker.X, setup.Attacker.Y, Enums.Direction.Up, true, false, false);
                        					List<int> moveNums = new List<int>();
                                            for (int i = 0; i < targets.Foes.Count; i++) {
                                                for (int j = 0; j < Constants.MAX_PLAYER_MOVES; j++) {
                                                    if (targets.Foes[i].Moves[j].MoveNum > 0 && !moveNums.Contains(targets.Foes[i].Moves[j].MoveNum)) {
                                                        moveNums.Add(targets.Foes[i].Moves[j].MoveNum);
                                                    }
                                                }
                                            }
			                                string tag = "MeFirst:";
			                                if (moveNums.Count > 0) {
			                                	tag += moveNums[0];
			                                } else {
			                                	tag += "0";
			                                }
			                                for (int i = 1; i < moveNums.Count; i++) {
			                                	tag += ("," + moveNums[i]);
			                                }
                        					setup.BattleTags.Add(tag);
                        				}
                        				break;
                        		
                        		}
                        	}
                        	break;
                        case 195: {//pounce
                                setup.BattleTags.Add("Pounce:" + setup.Attacker.X + ":" + setup.Attacker.Y);
                            }
                            break;
                    }
                }
                							point = 1;
                

                switch (setup.Move.AdditionalEffectData1) {
                    case 32: {//HP damage
                            setup.Move.Data1 *= setup.Attacker.HP;
                            setup.Move.Data1 /= setup.Attacker.MaxHP;
                        }
                        break;
                    case 33: {//Reverse HP damage
                            int n = setup.Attacker.HP * 64 / setup.Attacker.MaxHP;
                            if (n > 42) {
                                setup.Move.Data1 = 20;
                            } else if (n > 42) {
                                setup.Move.Data1 = 40;
                            } else if (n > 21) {
                                setup.Move.Data1 = 80;
                            } else if (n > 5) {
                                setup.Move.Data1 = 100;
                            } else if (n > 1) {
                                setup.Move.Data1 = 150;
                            } else {
                                setup.Move.Data1 = 200;
                            }
                        }
                        break;
                    case 62: {//rolling damage
                            if (setup.Attacker.VolatileStatus.GetStatus("DefenseCurl") != null) {
                                setup.Move.Data1 *= 2;
                            }
                            status = setup.Attacker.VolatileStatus.GetStatus("Rolling");
                            if (status != null) {
                                setup.Move.Data1 *= (int)System.Math.Pow(2, status.Counter);
                            }
                        }
                        break;
                    case 63: {//fury cutter damage
                        status = setup.Attacker.VolatileStatus.GetStatus("FuryCutter");
                            if (status != null) {
                                setup.Move.Data1 *= (int)System.Math.Pow(2, status.Counter);
                            }
                        }
                        break;
                    case 70: {//spit up
                        status = setup.Attacker.VolatileStatus.GetStatus("Stockpile");
                            if (status == null) {
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("But there was nothing to spit up!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                                setup.AttackerMultiplier = 0;
                            } else {
                                setup.Move.Data1 = 100 * status.Counter;
                            }
                        }
                        break;
                    case 72: {//magnitude
                            int rand = Server.Math.Rand(4, 11);
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("It's magnitude " + rand + "!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                            switch (rand) {
                                case 4: {
                                        setup.Move.Data1 = 10;
                                    }
                                    break;
                                case 5: {
                                        setup.Move.Data1 = 30;
                                    }
                                    break;
                                case 6: {
                                        setup.Move.Data1 = 50;
                                    }
                                    break;
                                case 7: {
                                        setup.Move.Data1 = 70;
                                    }
                                    break;
                                case 8: {
                                        setup.Move.Data1 = 90;
                                    }
                                    break;
                                case 9: {
                                        setup.Move.Data1 = 110;
                                    }
                                    break;
                                case 10: {
                                        setup.Move.Data1 = 150;
                                    }
                                    break;
                            }
                        }
                        break;
                    case 76: {//Trump Card
                            setup.Move.Data1 = 50;
                            if (setup.moveSlot > -1 && setup.moveSlot < 4) {
                                switch (setup.Attacker.Moves[setup.moveSlot].CurrentPP) {
                                    case 3: {
                                            setup.Move.Data1 = 50;
                                        }
                                        break;
                                    case 2: {
                                            setup.Move.Data1 = 60;
                                        }
                                        break;
                                    case 1: {
                                            setup.Move.Data1 = 80;
                                        }
                                        break;
                                    case 0: {
                                            setup.Move.Data1 = 200;
                                        }
                                        break;
                                    case 4: {
                                            setup.Move.Data1 = 40;
                                        }
                                        break;
                                }
                            }
                        }
                        break;
                    case 82: {//Weather Ball
                            switch (GetCharacterWeather(setup.Attacker)) {
                                case Enums.Weather.Raining:
                                case Enums.Weather.Thunder: {
                                        setup.Move.Data1 *= 2;
                                        setup.Move.Element = Enums.PokemonType.Water;
                                        setup.Move.TravelingAnim.AnimationIndex = 15;
                                        setup.Move.DefenderAnim.AnimationIndex = 71;
                                    }
                                    break;
                                case Enums.Weather.Snowing:
                                case Enums.Weather.Snowstorm:
                                case Enums.Weather.Hail: {
                                        setup.Move.Data1 *= 2;
                                        setup.Move.Element = Enums.PokemonType.Ice;
                                        setup.Move.TravelingAnim.AnimationIndex = 410;
                                        setup.Move.DefenderAnim.AnimationIndex = 10;
                                    }
                                    break;
                                case Enums.Weather.DiamondDust: {
                                        setup.Move.Data1 *= 2;
                                        setup.Move.Element = Enums.PokemonType.Normal;
                                        setup.Move.DefenderAnim.AnimationIndex = 72;
                                    }
                                    break;
                                case Enums.Weather.Cloudy:
                                case Enums.Weather.Fog: {
                                        setup.Move.Data1 *= 2;
                                        setup.Move.Element = Enums.PokemonType.Normal;
                                    }
                                    break;
                                case Enums.Weather.Sunny: {
                                        setup.Move.Data1 *= 2;
                                        setup.Move.Element = Enums.PokemonType.Fire;
                                        setup.Move.TravelingAnim.AnimationIndex = 35;
                                        setup.Move.DefenderAnim.AnimationIndex = 20;
                                    }
                                    break;
                                case Enums.Weather.Sandstorm:
                                case Enums.Weather.Ashfall: {
                                        setup.Move.Data1 *= 2;
                                        setup.Move.Element = Enums.PokemonType.Rock;
                                        setup.Move.TravelingAnim.AnimationIndex = 412;
                                        setup.Move.DefenderAnim.AnimationIndex = 14;
                                    }
                                    break;

                            }
                        }
                        break;
                    case 83: {//facade
                            if (setup.Attacker.StatusAilment != Enums.StatusAilment.OK) {
                                setup.Move.Data1 *= 2;
                            }
                        }
                        break;
                    case 126: {//last resort
                            int lostMoves = 0;

                            for (int i = 0; i < Constants.MAX_PLAYER_MOVES; i++) {
                                if (setup.Attacker.Moves[i].CurrentPP == 0) lostMoves++;
                            }
                            if (lostMoves == 0) {
                                setup.AttackerMultiplier = 0;
                            } else {
                                setup.Move.Data1 *= lostMoves;
                            }
                        }
                        break;
                    case 154: {//acrobatics
                            if (setup.Attacker.HeldItem == null) {
                                setup.Move.Data1 *= 2;
                            }
                        }
                        break;
                    case 160: {//round

                            if (GetBattleTagArg(setup.BattleTags, "Round", 0) != null) {
                                setup.Move.Data1 *= 2;
                            }

                            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                                if (!setup.BattleTags.Contains("Round:" + (int)setup.Attacker.CharacterType + ":" + ((Recruit)setup.Attacker).Owner.Player.CharID)) {
                                    setup.BattleTags.Add("Round:" + (int)setup.Attacker.CharacterType + ":" + ((Recruit)setup.Attacker).Owner.Player.CharID);
                                }
                            } else if (setup.Attacker.CharacterType == Enums.CharacterType.MapNpc) {
                                if (!setup.BattleTags.Contains("Round:" + (int)setup.Attacker.CharacterType + ":" + ((MapNpc)setup.Attacker).MapSlot)) {
                                    setup.BattleTags.Add("Round:" + (int)setup.Attacker.CharacterType + ":" + ((MapNpc)setup.Attacker).MapSlot);
                                }
                            }

                        }
                        break;
                    case 169: {//Stored Power
                            if (setup.Attacker.AttackBuff > 0) setup.Move.Data1 += (20 * setup.Attacker.AttackBuff);
                            if (setup.Attacker.DefenseBuff > 0) setup.Move.Data1 += (20 * setup.Attacker.DefenseBuff);
                            if (setup.Attacker.SpeedBuff > 0) setup.Move.Data1 += (20 * setup.Attacker.SpeedBuff);
                            if (setup.Attacker.SpAtkBuff > 0) setup.Move.Data1 += (20 * setup.Attacker.SpAtkBuff);
                            if (setup.Attacker.SpDefBuff > 0) setup.Move.Data1 += (20 * setup.Attacker.SpDefBuff);
                            if (setup.Attacker.AccuracyBuff > 0) setup.Move.Data1 += (20 * setup.Attacker.AccuracyBuff);
                            if (setup.Attacker.EvasionBuff > 0) setup.Move.Data1 += (20 * setup.Attacker.EvasionBuff);
                        }
                        break;
                    case 196: {//jump kick/hi jump kick
                    		Pounce(setup.Attacker, setup.AttackerMap, setup.Attacker.Direction, true, false, setup.PacketStack);
                    	}
                    	break;
                    case 197: {//sky drop
                        	if (GetBattleTagArg(setup.BattleTags, "SkyDrop", 0) == null) {
                                setup.Move.EffectType = Enums.MoveType.Scripted;
                                setup.Move.Data1 = 197;
                            } else {
                            	setup.Move.Accuracy = -1;
                            }
                        }
                        break;
                    case 203: {//retaliate
                            if (setup.Attacker.VolatileStatus.GetStatus("SawAllyFaint") != null) {
                                setup.Move.Data1 *= 2;
                            }
                        }
                        break;
                    case 211: {//hidden power
                    		int sum = 0;
                    		int pwr = 60;
                    		if (setup.AttackerMap.MapType == Enums.MapType.RDungeonMap) {
                    			RDungeonMap rmap = setup.AttackerMap as RDungeonMap;
                    			sum = (rmap.RDungeonIndex + rmap.RDungeonFloor) % 17 + 1;
                    		} else {
                    			sum = Server.Math.Rand(1, 18);
                    		}
                    		if (sum >= 13) sum++;
                    		setup.Move.Element = (Enums.PokemonType)sum;
                    		setup.Move.Data1 = pwr;
                        }
                        break;
                    case 217: {//natural gift
                    		Enums.PokemonType berryType = Enums.PokemonType.Normal;
                    		int pwr = 30;
                    		int berryNum = -1;
                    		if (setup.Attacker.HeldItem != null) berryNum = setup.Attacker.HeldItem.Num;
                    		switch (berryNum) {
                    			case 492: pwr += 30; berryType = Enums.PokemonType.Fire; break;
								case 493: pwr += 30; berryType = Enums.PokemonType.Water; break;
								case 4: pwr += 30; berryType = Enums.PokemonType.Poison; break;
								case 524: pwr += 50; berryType = Enums.PokemonType.Grass; break;
								case 74: pwr += 20; berryType = Enums.PokemonType.Poison; break;
								case 222: pwr += 50; berryType = Enums.PokemonType.Bug; break;
								case 86: pwr += 30; berryType = Enums.PokemonType.Fighting; break;
								case 474: pwr += 40; berryType = Enums.PokemonType.Fighting; break;
								case 475: pwr += 40; berryType = Enums.PokemonType.Poison; break;
								case 476: pwr += 40; berryType = Enums.PokemonType.Psychic; break;
								case 477: pwr += 40; berryType = Enums.PokemonType.Ground; break;
								case 487: pwr += 30; berryType = Enums.PokemonType.Electric; break;
								case 494: pwr += 30; berryType = Enums.PokemonType.Grass; break;
								case 495: pwr += 30; berryType = Enums.PokemonType.Ice; break;
								case 496: pwr += 30; berryType = Enums.PokemonType.Ground; break;
								case 497: pwr += 30; berryType = Enums.PokemonType.Flying; break;
								case 523: pwr += 50; berryType = Enums.PokemonType.Ground; break;
								case 525: pwr += 50; berryType = Enums.PokemonType.Ice; break;
								case 526: pwr += 50; berryType = Enums.PokemonType.Fighting; break;
								case 527: pwr += 50; berryType = Enums.PokemonType.Poison; break;
								case 528: pwr += 50; berryType = Enums.PokemonType.Psychic; break;
								case 529: pwr += 50; berryType = Enums.PokemonType.Rock; break;
								case 641: pwr += 30; berryType = Enums.PokemonType.Bug; break;
								case 642: pwr += 30; berryType = Enums.PokemonType.Dark; break;
								case 643: pwr += 30; berryType = Enums.PokemonType.Dragon; break;
								case 644: pwr += 30; berryType = Enums.PokemonType.Electric; break;
								case 645: pwr += 30; berryType = Enums.PokemonType.Fighting; break;
								case 646: pwr += 30; berryType = Enums.PokemonType.Fire; break;
								case 647: pwr += 30; berryType = Enums.PokemonType.Flying; break;
								case 648: pwr += 30; berryType = Enums.PokemonType.Ghost; break;
								case 649: pwr += 30; berryType = Enums.PokemonType.Grass; break;
								case 650: pwr += 30; berryType = Enums.PokemonType.Ground; break;
								case 651: pwr += 30; berryType = Enums.PokemonType.Ice; break;
								case 652: pwr += 30; berryType = Enums.PokemonType.Normal; break;
								case 653: pwr += 30; berryType = Enums.PokemonType.Poison; break;
								case 654: pwr += 30; berryType = Enums.PokemonType.Psychic; break;
								case 655: pwr += 30; berryType = Enums.PokemonType.Rock; break;
								case 656: pwr += 30; berryType = Enums.PokemonType.Steel; break;
								case 657: pwr += 30; berryType = Enums.PokemonType.Water; break;
								case 767: pwr += 30; berryType = Enums.PokemonType.Bug; break;
								case 768: pwr += 30; berryType = Enums.PokemonType.Rock; break;
								case 769: pwr += 30; berryType = Enums.PokemonType.Ghost; break;
								case 770: pwr += 30; berryType = Enums.PokemonType.Dragon; break;
								case 771: pwr += 30; berryType = Enums.PokemonType.Dark; break;
								case 772: pwr += 30; berryType = Enums.PokemonType.Steel; break;
								case 773: pwr += 40; berryType = Enums.PokemonType.Fire; break;
								case 774: pwr += 40; berryType = Enums.PokemonType.Water; break;
								case 775: pwr += 40; berryType = Enums.PokemonType.Electric; break;
								case 776: pwr += 40; berryType = Enums.PokemonType.Grass; break;
								case 777: pwr += 40; berryType = Enums.PokemonType.Ice; break;
								case 778: pwr += 40; berryType = Enums.PokemonType.Bug; break;
								case 779: pwr += 40; berryType = Enums.PokemonType.Rock; break;
								case 780: pwr += 40; berryType = Enums.PokemonType.Ghost; break;
								case 781: pwr += 40; berryType = Enums.PokemonType.Flying; break;
								case 782: pwr += 40; berryType = Enums.PokemonType.Dragon; break;
								case 783: pwr += 40; berryType = Enums.PokemonType.Dark; break;
								case 784: pwr += 40; berryType = Enums.PokemonType.Steel; break;
								case 785: pwr += 50; berryType = Enums.PokemonType.Fire; break;
								case 786: pwr += 50; berryType = Enums.PokemonType.Water; break;
								case 787: pwr += 50; berryType = Enums.PokemonType.Electric; break;
								case 788: pwr += 50; berryType = Enums.PokemonType.Flying; break;
								case 789: pwr += 50; berryType = Enums.PokemonType.Dragon; break;
								case 790: pwr += 50; berryType = Enums.PokemonType.Dark; break;
								case 3: pwr += 60; berryType = Enums.PokemonType.Ice; break;
								case 8: pwr += 30; berryType = Enums.PokemonType.Psychic; break;
                    			
                    		}
                    		
                    		setup.Move.Element = berryType;
                    		setup.Move.Data1 = pwr;
                        }
                        break;
                }


                if (HasAbility(setup.Attacker, "Normalize")) {
                    setup.Move.Element = Enums.PokemonType.Normal;

                }

                //apply STAB
                if (setup.Move.EffectType == Enums.MoveType.SubHP && setup.Move.Element != Enums.PokemonType.None && (setup.Move.Element == setup.Attacker.Type1 || setup.Move.Element == setup.Attacker.Type2)) {
                    setup.AttackerMultiplier *= 5;
                    setup.AttackerMultiplier /= 4;
                }

                //apply Choice
                if (setup.Attacker.HasActiveItem(256) && setup.Move.MoveCategory == Enums.MoveCategory.Physical
                || setup.Attacker.HasActiveItem(257) && setup.Move.MoveCategory == Enums.MoveCategory.Special) {
                    setup.AttackerMultiplier *= 3;
                    setup.AttackerMultiplier /= 2;
                }

                //apply life orb
                if (setup.Attacker.HasActiveItem(143)) {
                    setup.AttackerMultiplier *= 5;
                    setup.AttackerMultiplier /= 4;
                }

                //type-boosting items
                if (setup.Attacker.HasActiveItem(71) && setup.Move.Element == Enums.PokemonType.Bug
                || setup.Attacker.HasActiveItem(44) && setup.Move.Element == Enums.PokemonType.Dark
                || setup.Attacker.HasActiveItem(76) && setup.Move.Element == Enums.PokemonType.Dragon
                || setup.Attacker.HasActiveItem(83) && setup.Move.Element == Enums.PokemonType.Electric
                || setup.Attacker.HasActiveItem(58) && setup.Move.Element == Enums.PokemonType.Fighting
                || setup.Attacker.HasActiveItem(64) && setup.Move.Element == Enums.PokemonType.Fire
                || setup.Attacker.HasActiveItem(95) && setup.Move.Element == Enums.PokemonType.Flying
                || setup.Attacker.HasActiveItem(97) && setup.Move.Element == Enums.PokemonType.Ghost
                || setup.Attacker.HasActiveItem(100) && setup.Move.Element == Enums.PokemonType.Grass
                || setup.Attacker.HasActiveItem(67) && setup.Move.Element == Enums.PokemonType.Ground
                || setup.Attacker.HasActiveItem(106) && setup.Move.Element == Enums.PokemonType.Ice
                || setup.Attacker.HasActiveItem(399) && setup.Move.Element == Enums.PokemonType.Normal
                || setup.Attacker.HasActiveItem(122) && setup.Move.Element == Enums.PokemonType.Poison
                || setup.Attacker.HasActiveItem(107) && setup.Move.Element == Enums.PokemonType.Psychic
                || setup.Attacker.HasActiveItem(108) && setup.Move.Element == Enums.PokemonType.Rock
                || setup.Attacker.HasActiveItem(68) && setup.Move.Element == Enums.PokemonType.Steel
                || setup.Attacker.HasActiveItem(314) && setup.Move.Element == Enums.PokemonType.Water) {
                    setup.AttackerMultiplier *= 11;
                    setup.AttackerMultiplier /= 10;
                }
                
                //power boosting items
                 
                //muscle band/wise glasses
                if (setup.Attacker.HasActiveItem(248) && setup.Move.MoveCategory == Enums.MoveCategory.Physical ||
                	setup.Attacker.HasActiveItem(249) && setup.Move.MoveCategory == Enums.MoveCategory.Special) {
                	setup.AttackStat *= 11;
                	setup.AttackStat /= 10;
                }
                
                point = 2;

                status = setup.Attacker.VolatileStatus.GetStatus("TypeBoost");
                if (status != null && status.Counter == (int)setup.Move.Element) {
                    setup.AttackerMultiplier *= 3;
                    setup.AttackerMultiplier /= 2;
                }

                if (setup.Move.Element == Enums.PokemonType.Fire && setup.Attacker.VolatileStatus.GetStatus("FlashFire") != null) {
                    setup.AttackerMultiplier *= 4;
                    setup.AttackerMultiplier /= 3;
                }

                //Metronome (item)
                if (setup.Attacker.HasActiveItem(125)) {
                    status = setup.Attacker.VolatileStatus.GetStatus("TimesLastMoveUsed");
                    if (status != null) {
                        int repeats = status.Counter - 1;
                        if (repeats > 10) repeats = 10;
                        setup.AttackerMultiplier *= (100 + 8 * repeats);
                        setup.AttackerMultiplier /= 100;
                    }
                }
                point = 3;
                
                
                    
                //wide lens
                if (setup.Attacker.HasActiveItem(175) && setup.Move.Accuracy != -1) {
                	setup.Move.Accuracy *= 11;
                	setup.Move.Accuracy /= 10;
                }

                //Charge
                if (setup.Move.Element == Enums.PokemonType.Electric && setup.Attacker.VolatileStatus.GetStatus("Charge") != null) {
                    setup.AttackerMultiplier *= 2;
                }

                //sure shot
                if (setup.Attacker.VolatileStatus.GetStatus("SureShot") != null) {
                    setup.Move.Accuracy = -1;
                }

                //attacker-based weather effects
                switch (GetCharacterWeather(setup.Attacker)) {
                    case Enums.Weather.Raining:
                    case Enums.Weather.Thunder: {
                            if (setup.Move.Element == Enums.PokemonType.Water) {
                                setup.AttackerMultiplier *= 5;
                                setup.AttackerMultiplier /= 4;
                            }
                            if (setup.Move.Element == Enums.PokemonType.Fire) {
                                setup.AttackerMultiplier /= 2;
                            }
                            if (setup.Move.AdditionalEffectData1 == 60 && setup.Move.AdditionalEffectData2 == 1) setup.AttackerMultiplier /= 2;
                        }
                        break;
                    case Enums.Weather.Snowing:
                    case Enums.Weather.Snowstorm: {
                            if (setup.Move.AdditionalEffectData1 == 60 && setup.Move.AdditionalEffectData2 == 1) setup.AttackerMultiplier /= 2;
                        }
                        break;
                    case Enums.Weather.Hail: {
                            if (setup.Move.AdditionalEffectData1 == 60 && setup.Move.AdditionalEffectData2 == 1) setup.AttackerMultiplier /= 2;
                        }
                        break;
                    case Enums.Weather.DiamondDust: {
                        }
                        break;
                    case Enums.Weather.Cloudy: {
                            if (setup.Move.Element != Enums.PokemonType.Normal) {
                                setup.AttackerMultiplier /= 2;
                            }
                            if (setup.Move.AdditionalEffectData1 == 60 && setup.Move.AdditionalEffectData2 == 1) setup.AttackerMultiplier /= 2;
                        }
                        break;
                    case Enums.Weather.Fog: {
                            if (setup.Move.AdditionalEffectData1 == 60 && setup.Move.AdditionalEffectData2 == 1) setup.AttackerMultiplier /= 2;
                        }
                        break;
                    case Enums.Weather.Sunny: {
                            if (setup.Move.Element == Enums.PokemonType.Fire) {
                                setup.AttackerMultiplier *= 5;
                                setup.AttackerMultiplier /= 4;
                            }
                            if (setup.Move.Element == Enums.PokemonType.Water) {
                                setup.AttackerMultiplier /= 2;
                            }
                        }
                        break;
                    case Enums.Weather.Sandstorm: {
                            if (setup.Move.AdditionalEffectData1 == 60 && setup.Move.AdditionalEffectData2 == 1) setup.AttackerMultiplier /= 2;
                        }
                        break;
                    case Enums.Weather.Ashfall: {
                            if (setup.Move.Element == Enums.PokemonType.Poison) {
                                setup.AttackerMultiplier *= 5;
                                setup.AttackerMultiplier /= 4;
                            }
                            if (setup.Move.Element == Enums.PokemonType.Electric) {
                                setup.AttackerMultiplier /= 2;
                            }
                            if (setup.Move.AdditionalEffectData1 == 60 && setup.Move.AdditionalEffectData2 == 1) setup.AttackerMultiplier /= 2;
                        }
                        break;

                }
                point = 4;

                //burn reduction
                if (setup.Move.MoveCategory == Enums.MoveCategory.Physical && setup.Attacker.StatusAilment == Enums.StatusAilment.Burn) {
                    setup.AttackStat /= 2;
                }


                //check for attacker modifying abilities
                CheckAttackerModAbility(setup);
                
                point = 5;
                
                bool unaware = false;
                if (!HasAbility(setup.Attacker, "Mold Breaker")) {
	                TargetCollection abilityTargets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 10, setup.AttackerMap, setup.Attacker, setup.Attacker.X, setup.Attacker.Y, Enums.Direction.Up, true, false, false);
	                for (int i = 0; i < abilityTargets.Foes.Count; i++) {
	                    if (HasAbility(abilityTargets.Foes[i], "Unaware")) {
	                        unaware = true;
	                        break;
	                    }
	                }
                }
                if (setup.Move.MoveCategory == Enums.MoveCategory.Physical) {
                    if (unaware) {
                    } else {
                        setup.AttackStat = DamageCalculator.ApplyAttackBuff(setup.AttackStat, setup.Attacker.AttackBuff) / 256;
                    }
                } else if (setup.Move.MoveCategory == Enums.MoveCategory.Special) {
                    if (unaware) {
                    } else {
                        setup.AttackStat = DamageCalculator.ApplyAttackBuff(setup.AttackStat, setup.Attacker.SpAtkBuff) / 256;
                    }
                }
                point = 6;
                
                //pain split check
                if (setup.Move.EffectType == Enums.MoveType.Scripted && setup.Move.Data1 == 204) {
                	int health = setup.Attacker.HP;
                	TargetCollection checkedTargets = MoveProcessor.GetTargetsInRange(setup.Move.RangeType,
                	setup.Move.Range, setup.AttackerMap, setup.Attacker, setup.Attacker.X, setup.Attacker.Y, setup.Attacker.Direction,
                	BattleProcessor.HitsFoes(setup.Move.TargetType), BattleProcessor.HitsAllies(setup.Move.TargetType), false);
                	for (int i = 0; i < checkedTargets.Count; i++) {
                		health += checkedTargets[i].HP;
                	}
                	health /= (checkedTargets.Count + 1);
                	setup.BattleTags.Add("PainSplit:" + health);
                }
                
                //stat split check
                if (setup.Move.EffectType == Enums.MoveType.Scripted && setup.Move.Data1 == 209) {
                	int stat = 0, special = 0;
                	if (setup.Move.Data2 == 1) {
                		stat = setup.Attacker.Atk;
                		special = setup.Attacker.SpclAtk;
                	} else if (setup.Move.Data2 == 2) {
                		stat = setup.Attacker.Def;
                		special = setup.Attacker.SpclDef;
                	}
                	TargetCollection checkedTargets = MoveProcessor.GetTargetsInRange(setup.Move.RangeType,
                	setup.Move.Range, setup.AttackerMap, setup.Attacker, setup.Attacker.X, setup.Attacker.Y, setup.Attacker.Direction,
                	BattleProcessor.HitsFoes(setup.Move.TargetType), BattleProcessor.HitsAllies(setup.Move.TargetType), false);
                	for (int i = 0; i < checkedTargets.Count; i++) {
                		if (setup.Move.Data2 == 1) {
	                		stat += checkedTargets[i].Atk;
	                		special += checkedTargets[i].SpclAtk;
	                	} else if (setup.Move.Data2 == 2) {
	                		stat += checkedTargets[i].Def;
	                		special += checkedTargets[i].SpclDef;
	                	}
                	}
                	stat /= (checkedTargets.Count + 1);
                	special /= (checkedTargets.Count + 1);
                	setup.BattleTags.Add("StatSplit:" + stat + ":" + special);
                }


            } catch (Exception ex) {
                Messenger.AdminMsg("Error: AfterMoveExecuted", Text.Black);
                Messenger.AdminMsg("Point: " + point, Text.Black);
            }
        }

        public static void BeforeMoveHits(BattleSetup setup) {
            try {
            	
                ExtraStatus status;
                //if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                //	if (Ranks.IsAllowed(((Recruit)setup.Attacker).Owner, Enums.Rank.Moniter)) {
                //		Messenger.PlayerMsg(((Recruit)setup.Attacker).Owner, "Place Hit", Text.BrightRed);
                //	}
                //}
                bool followAway = false;

                if (setup.Defender != null) {
                	
                    status = setup.Attacker.VolatileStatus.GetStatus("Follow");
                    if (status != null) {
                        if (status.Target == null) {
                            setup.Attacker.VolatileStatus.Remove(status);
                        } else if (status.Target != setup.Defender) {
                            if (status.Target != null) {
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " can only attack " + status.Target.Name + "!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                            } else {
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " can only attack one target!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                            }
                            setup.Defender = null;
                            followAway = true;
                        }
                    }
                }

                if (setup.Defender != null) {
                    int criticalRate = 625;

                    if (setup.Move.AdditionalEffectData1 == 157) {
                        if (setup.Move.MoveCategory == Enums.MoveCategory.Physical) {
                        	status = setup.Attacker.VolatileStatus.GetStatus("ProxyAttack");
                            if (status != null) {
                    			setup.AttackStat = status.Counter;
                    		} else {
                            	setup.AttackStat = setup.Defender.Atk;
                            }
                        } else if (setup.Move.MoveCategory == Enums.MoveCategory.Special) {
                            status = setup.Attacker.VolatileStatus.GetStatus("ProxySpAtk");
                            if (status != null) {
                    			setup.AttackStat = status.Counter;
                    		} else {
                            	setup.AttackStat = setup.Defender.SpclAtk;
                            }
                        }
                    }

                    //defender-based move modification
                    switch (setup.Move.AdditionalEffectData1) {
                        case 7: {//extra minimize damage
                                if (setup.Move.AdditionalEffectData3 == 1 && setup.Defender.VolatileStatus.GetStatus("Minimize") != null) {
                                    setup.Multiplier *= 2;
                                }
                            }
                            break;
                        case 29: {//custom critical rate
                                criticalRate = setup.Move.AdditionalEffectData2;
                            }
                            break;
                        case 49: {//high critical rate + status
                                criticalRate = 1250;
                            }
                            break;
                        case 60: {//high critical rate of razor wind
                                if (setup.Move.AdditionalEffectData2 == 3) {
                                    criticalRate = 1250;
                                }
                            }
                            break;
                        case 73: {//Punishment
                                if (setup.Defender.AttackBuff > 0) setup.Move.Data1 += (20 * setup.Defender.AttackBuff);
                                if (setup.Defender.DefenseBuff > 0) setup.Move.Data1 += (20 * setup.Defender.DefenseBuff);
                                if (setup.Defender.SpeedBuff > 0) setup.Move.Data1 += (20 * setup.Defender.SpeedBuff);
                                if (setup.Defender.SpAtkBuff > 0) setup.Move.Data1 += (20 * setup.Defender.SpAtkBuff);
                                if (setup.Defender.SpDefBuff > 0) setup.Move.Data1 += (20 * setup.Defender.SpDefBuff);
                                if (setup.Defender.AccuracyBuff > 0) setup.Move.Data1 += (20 * setup.Defender.AccuracyBuff);
                                if (setup.Defender.EvasionBuff > 0) setup.Move.Data1 += (20 * setup.Defender.EvasionBuff);
                            }
                            break;
                        case 74: {//Brine
                                if (setup.Defender.HP <= setup.Defender.MaxHP / 2) setup.Move.Data1 *= 2;
                            }
                            break;
                        case 77: {//wring out
                                setup.Move.Data1 = 120 * setup.Defender.HP / setup.Defender.MaxHP;
                            }
                            break;
                        case 78: {//Gyro Ball
                                int attackerSpeed = setup.Attacker.Spd;
                                int defenderSpeed = setup.Defender.Spd;

                                attackerSpeed = DamageCalculator.ApplySpeedBuff(attackerSpeed, setup.Attacker.SpeedBuff);
                                defenderSpeed = DamageCalculator.ApplySpeedBuff(defenderSpeed, setup.Defender.SpeedBuff);

                                if (setup.Attacker.HasActiveItem(258)) {
                                    attackerSpeed *= 3;
                                    attackerSpeed /= 2;
                                }
                                if (setup.Defender.HasActiveItem(258)) {
                                    defenderSpeed *= 3;
                                    defenderSpeed /= 2;
                                }

                                if (attackerSpeed < 1) attackerSpeed = 1;
                                if (defenderSpeed < 1) defenderSpeed = 1;

                                switch (setup.Move.AdditionalEffectData2) {
                                    case 1: {
                                            setup.Move.Data1 = 25 * defenderSpeed / attackerSpeed;
                                            if (setup.Move.Data1 > 150) setup.Move.Data1 = 150;
                                        }
                                        break;
                                    case 2: {
                                            int ratio = attackerSpeed / defenderSpeed;
                                            if (ratio >= 4) {
                                                setup.Move.Data1 = 150;
                                            } else if (ratio >= 3) {
                                                setup.Move.Data1 = 120;
                                            } else if (ratio >= 2) {
                                                setup.Move.Data1 = 80;
                                            } else {
                                                setup.Move.Data1 = 60;
                                            }
                                        }
                                        break;
                                }
                            }
                            break;
                        case 79: {//smellingsalt
                                if (setup.Defender.StatusAilment != Enums.StatusAilment.OK && (setup.Move.AdditionalEffectData2 == 0 || setup.Defender.StatusAilment == (Enums.StatusAilment)setup.Move.AdditionalEffectData2)) {
                                    setup.Move.Data1 *= 2;
                                }
                            }
                            break;
                        case 80: {//revenge/payback
                            status = setup.Attacker.VolatileStatus.GetStatus("LastHitBy");
                                if (status != null && status.Counter == 1) {
                                    if (status.Target == null) {
                                        setup.Attacker.VolatileStatus.Remove(status);
                                    } else if (status.Target == setup.Defender) {
                                        setup.Move.Data1 *= 2;
                                    }
                                }
                            }
                            break;
                        case 81: {//Assurance
                            status = setup.Attacker.VolatileStatus.GetStatus("LastHitBy");
                                if (status != null && status.Counter == 1) {
                                    setup.Move.Data1 *= 2;
                                }
                            }
                            break;
                        case 113: {//Uproar
                            if (MoveProcessor.GetMatchupWith(setup.Attacker, setup.Defender) != Enums.CharacterMatchup.Foe) {
                                    setup.Move.EffectType = Enums.MoveType.Scripted;
                                    setup.Move.Data1 = 0;
                                }
                            }
                            break;
                        case 132: {//sucker punch
                                if (MoveProcessor.IsInFront(1, setup.Attacker.Direction, setup.Attacker.X, setup.Attacker.Y, setup.Defender.X, setup.Defender.Y)) {
                                    setup.Move.Accuracy = 0;
                                }
                            }
                            break;
                        case 135: {//pursuit
                                if (setup.Defender.Direction == setup.Attacker.Direction) {
                                    setup.Multiplier *= 2;
                                }
                            }
                            break;
                        case 142: {//weight-based
                                double weight = Server.Pokedex.Pokedex.GetPokemonForm(setup.Defender.Species, setup.Defender.Form).Weight;

                                if (HasAbility(setup.Defender, "Heavy Metal") && !HasAbility(setup.Attacker, "Mold Breaker")) weight = weight * (double)2;
                                if (HasAbility(setup.Defender, "Light Metal") && !HasAbility(setup.Attacker, "Mold Breaker")) weight = weight / (double)2;
                                if (setup.Defender.HasActiveItem(347)) weight = weight / (double)2;
                                if (setup.Defender.VolatileStatus.GetStatus("Autotomize") != null) weight = weight / (double)2;

                                if (weight > 200) {
                                    setup.Move.Data1 = 120;
                                } else if (weight > 100) {
                                    setup.Move.Data1 = 100;
                                } else if (weight > 50) {
                                    setup.Move.Data1 = 80;
                                } else if (weight > 25) {
                                    setup.Move.Data1 = 60;
                                } else if (weight > 10) {
                                    setup.Move.Data1 = 40;
                                } else {
                                    setup.Move.Data1 = 20;
                                }
                            }
                            break;
                        case 143: {//weight-based (attacker sits on defender)
                                double weight = Server.Pokedex.Pokedex.GetPokemonForm(setup.Defender.Species, setup.Defender.Form).Weight;
                                if (HasAbility(setup.Defender, "Heavy Metal") && !HasAbility(setup.Attacker, "Mold Breaker")) weight = weight * (double)2;
                                if (HasAbility(setup.Defender, "Light Metal") && !HasAbility(setup.Attacker, "Mold Breaker")) weight = weight / (double)2;
                                if (setup.Defender.HasActiveItem(347)) weight = weight / (double)2;
                                if (setup.Defender.VolatileStatus.GetStatus("Autotomize") != null) weight = weight / (double)2;

                                double userWeight = Server.Pokedex.Pokedex.GetPokemonForm(setup.Attacker.Species, setup.Attacker.Form).Weight;
                                if (HasAbility(setup.Attacker, "Heavy Metal")) userWeight = userWeight * (double)2;
                                if (HasAbility(setup.Attacker, "Light Metal")) userWeight = userWeight / (double)2;
                                if (setup.Attacker.HasActiveItem(347)) userWeight = userWeight / (double)2;
                                if (setup.Attacker.VolatileStatus.GetStatus("Autotomize") != null) userWeight = userWeight / (double)2;
                                int weightRatio = (int)(userWeight / weight);
                                if (weightRatio > 5) {
                                    setup.Move.Data1 = 120;
                                } else if (weightRatio > 4) {
                                    setup.Move.Data1 = 100;
                                } else if (weightRatio > 3) {
                                    setup.Move.Data1 = 80;
                                } else if (weightRatio > 2) {
                                    setup.Move.Data1 = 60;
                                } else {
                                    setup.Move.Data1 = 40;
                                }
                            }
                            break;
                        case 151: {//synchronoise
                                if (setup.Attacker.Type1 != Enums.PokemonType.None && (setup.Attacker.Type1 == setup.Defender.Type1 || setup.Attacker.Type1 == setup.Defender.Type2) ||
                                    setup.Attacker.Type2 != Enums.PokemonType.None && (setup.Attacker.Type2 == setup.Defender.Type1 || setup.Attacker.Type2 == setup.Defender.Type2)) {
                                } else {
                                    setup.Move.Accuracy = 0;
                                }
                            }
                            break;
                        case 160: {//round
                                if (MoveProcessor.GetMatchupWith(setup.Attacker, setup.Defender) == Enums.CharacterMatchup.Friend) {
                                    setup.Move.EffectType = Enums.MoveType.Scripted;
                                    setup.Move.Data1 = 0;
                                }
                            }
                            break;
	                    case 163: {//echoed voice
                            status = setup.Defender.VolatileStatus.GetStatus("EchoedVoice");
	                            if (status != null) {
	                                setup.Move.Data1 *= (1 * status.Counter + 1);
	                            }
	
	                        }
	                        break;
	                    case 197: {//sky drop
                            	//no effect on flyers/levitators?
	                        }
	                        break;
                        case 199: {//Fake Out
                                if (MoveProcessor.IsInFront(2, setup.Attacker.Direction, setup.Attacker.X, setup.Attacker.Y, setup.Defender.X, setup.Defender.Y)) {
                                    setup.Move.Accuracy = 0;
                                }
                            }
                            break;
                    }

                    //absorb
                    if (HasActiveBagItem(setup.Defender, 2, 0, (int)setup.Move.Element)) {
                    	SetAsNeutralizedMove(setup.Move);
                    }
                    
                    if (HasActiveBagItem(setup.Defender, 1, (int)setup.Move.Element, 0)) {
                    	setup.Multiplier = 0;
                    }
					
					if (setup.Defender.HasActiveItem(170) && setup.Attacker != setup.Defender && (setup.Defender.CharacterType == Enums.CharacterType.MapNpc || ((Recruit)setup.Defender).Belly > 0)) {
						string name = setup.Defender.Name;
						TargetCollection targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 2, setup.DefenderMap, setup.Defender, setup.Defender.X, setup.Defender.Y, Enums.Direction.Up, true, true, false);
						int lowestSpeed = -1;
                        ICharacter singleTarget = null;
                        for (int i = 0; i < targets.Count; i++) {
                            if (setup.Attacker != targets[i] && setup.Defender != targets[i]) {
                            	bool eligible = true;
                            	if (targets[i].CharacterType == Enums.CharacterType.MapNpc) {
                            		if (NpcManager.Npcs[((MapNpc)targets[i]).Num].Behavior == Enums.NpcBehavior.Scripted
                            		|| NpcManager.Npcs[((MapNpc)targets[i]).Num].Behavior == Enums.NpcBehavior.Friendly) {
                            			eligible = false;
                            		}
                            	} else {
                            		if (Ranks.IsAllowed(((Recruit)targets[i]).Owner, Enums.Rank.Moniter)) {
                            				eligible = false;
                            		}
                                }
                                if (eligible && (lowestSpeed == -1 || targets[i].Spd > lowestSpeed)) {
                                    singleTarget = targets[i];
                                    lowestSpeed = targets[i].Spd;
                                }
                            }
                        }
                        if (singleTarget != null) {
                            if (setup.Defender.CharacterType == Enums.CharacterType.Recruit) {
                            	((Recruit)setup.Defender).Owner.Player.BellyStepCounter += 500;
	                    		((Recruit)setup.Defender).Owner.Player.ProcessHunger(setup.PacketStack);
	                    	}
                            setup.Defender = singleTarget;
                            
                            setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(name + "'s Pass Scarf passed the attack to " + setup.Defender.Name + "!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                        }
						
						
						
					}
					
					//ring target
		            if (setup.Attacker != setup.Defender && setup.Move.MoveCategory != Enums.MoveCategory.Status) {
		                TargetCollection targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 10, setup.DefenderMap, setup.Defender, setup.Defender.X, setup.Defender.Y, Enums.Direction.Up, false, true, false);
		                int highestSpeed = 0;
		                ICharacter singleTarget = null;
		                for (int i = 0; i < targets.Count; i++) {
		                	
		                    if (targets[i].HasActiveItem(372) &&
		                    	DamageCalculator.CalculateTypeMatchup(setup.Move.Element, targets[i].Type1) + DamageCalculator.CalculateTypeMatchup(setup.Move.Element, targets[i].Type2) > 6) {
			                        if (targets[i].Spd > highestSpeed) {
			                            singleTarget = targets[i];
			                            highestSpeed = targets[i].Spd;
			                        }
		                        
		                    }
		                }
		
		                if (singleTarget != null) {
		                    setup.Defender = singleTarget;
		                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + "'s Ring Target attracted the attack!", Text.WhiteSmoke), setup.Defender.X, setup.Defender.Y, 10);;
		                }
		            }
		            
		            //fluffy tail
		            if (setup.Attacker != setup.Defender) {
		                TargetCollection targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 10, setup.DefenderMap, setup.Defender, setup.Defender.X, setup.Defender.Y, Enums.Direction.Up, false, true, false);
		                int highestSpeed = 0;
		                ICharacter singleTarget = null;
		                for (int i = 0; i < targets.Count; i++) {
		                	
		                    if (targets[i].HasActiveItem(90) && Server.Math.Rand(0,3) == 0) {
			                        if (targets[i].Spd > highestSpeed) {
			                            singleTarget = targets[i];
			                            highestSpeed = targets[i].Spd;
			                        }
		                        
		                    }
		                }
		
		                if (singleTarget != null) {
		                    setup.Defender = singleTarget;
		                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " drew in the attack with a Fluffy Tail!", Text.WhiteSmoke), setup.Defender.X, setup.Defender.Y, 10);;
		                }
		            }
					
                    //magic coat
                    status = setup.Defender.VolatileStatus.GetStatus("MagicCoat");
                    if (status != null) {
                    	status.Counter--;
                        if (status.Counter <= 0) {
                            RemoveExtraStatus(setup.Defender, setup.DefenderMap, "MagicCoat", setup.PacketStack);
                        } else {
		                    if (MoveProcessor.GetMatchupWith(setup.Attacker, setup.Defender) == Enums.CharacterMatchup.Foe && setup.Move.MoveCategory == Enums.MoveCategory.Status
		                        && !(HasAbility(setup.Attacker, "Infiltrator") || HasActiveBagItem(setup.Attacker, 7, 0, 0))) {
		                        setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + "'s Magic Coat bounced the attack back!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
		                        setup.Defender = setup.Attacker;
		                    }
	                    }
                    }

                    //magic bounce
                    if (HasAbility(setup.Defender, "Magic Bounce") && MoveProcessor.GetMatchupWith(setup.Attacker, setup.Defender) == Enums.CharacterMatchup.Foe && setup.Move.MoveCategory == Enums.MoveCategory.Status && !HasAbility(setup.Attacker, "Mold Breaker")) {
                        setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + "'s Magic Bounce bounced the attack back!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                        setup.Defender = setup.Attacker;
                    }

                    //snatch
                    if (setup.Move.MoveCategory == Enums.MoveCategory.Status && MoveProcessor.GetMatchupWith(setup.Attacker, setup.Defender) != Enums.CharacterMatchup.Foe) {
                        TargetCollection targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 10, setup.DefenderMap, setup.Defender, setup.Defender.X, setup.Defender.Y, Enums.Direction.Up, true, false, false);
                        int highestSpeed = 0;
                        ICharacter singleTarget = null;
                        for (int i = 0; i < targets.Foes.Count; i++) {
                            if (targets.Foes[i].VolatileStatus.GetStatus("Snatch") != null) {
                                if (targets.Foes[i].Spd > highestSpeed) {
                                    singleTarget = targets.Foes[i];
                                    highestSpeed = targets.Foes[i].Spd;
                                }
                            }
                        }
                        if (singleTarget != null) {
                            setup.Defender = singleTarget;
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " snatched the move away!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                            setup.BattleTags.Add("Snatch");
                        }
                    }

                    //checks against ability-based immunity
                    CheckImmunityAbility(setup);
                    
                    status = setup.Defender.VolatileStatus.GetStatus("TypeReduce");
                    if (status != null && status.Counter == (int)setup.Move.Element) {
                        setup.Multiplier /= 4;
                    }

                    if (setup.Defender.VolatileStatus.GetStatus("SkullBash") != null) {
                        setup.Multiplier /= 2;
                    }
                    
					//eviolite
					if (setup.Defender.HasActiveItem(113)) {
						setup.Multiplier *= 3;
						setup.Multiplier /= 4;
					}


                    //use the correct attack/sp.attack stats
                    bool hitAcross = false;
                    if (setup.Move.AdditionalEffectData1 == 146) {
                    	hitAcross = true;
                    }
                    if (setup.Move.MoveCategory == Enums.MoveCategory.Physical) {
                    	if (hitAcross) {
                    		if (setup.Defender.VolatileStatus.GetStatus("ProxySpDef") != null) {
                    			setup.DefenseStat = setup.Defender.VolatileStatus.GetStatus("ProxySpDef").Counter;
                    		} else {
                    			setup.DefenseStat = setup.Defender.SpclDef;
                    		}
                    	} else {
                    		if (setup.Defender.VolatileStatus.GetStatus("ProxyDefense") != null) {
                    			setup.DefenseStat = setup.Defender.VolatileStatus.GetStatus("ProxyDefense").Counter;
                    		} else {
                        		setup.DefenseStat = setup.Defender.Def;
                        	}
                        }
                    } else if (setup.Move.MoveCategory == Enums.MoveCategory.Special) {
                        if (hitAcross) {
                        	if (setup.Defender.VolatileStatus.GetStatus("ProxyDefense") != null) {
                    			setup.DefenseStat = setup.Defender.VolatileStatus.GetStatus("ProxyDefense").Counter;
                    		} else {
                            	setup.DefenseStat = setup.Defender.Def;
                            }
                        } else {
                        	if (setup.Defender.VolatileStatus.GetStatus("ProxySpDef") != null) {
                    			setup.DefenseStat = setup.Defender.VolatileStatus.GetStatus("ProxySpDef").Counter;
                    		} else {
                            	setup.DefenseStat = setup.Defender.SpclDef;
                            }
                        }
                    }


                    //defender-based weather effects

                    switch (GetCharacterWeather(setup.Defender)) {
                        case Enums.Weather.Raining:
                        case Enums.Weather.Thunder: {
                                if (setup.moveIndex == 13 || setup.moveIndex == 675) {
                                    setup.Move.Accuracy = -1;
                                }
                            }
                            break;
                        case Enums.Weather.Snowing:
                        case Enums.Weather.Snowstorm: {
                                if (setup.moveIndex == 74) {
                                    setup.Move.Accuracy = -1;
                                }

                            }
                            break;
                        case Enums.Weather.Hail: {
                                if (setup.moveIndex == 74) {
                                    setup.Move.Accuracy = -1;
                                }
                            }
                            break;
                        case Enums.Weather.DiamondDust: {
                                if (setup.Defender != null && setup.Defender.CharacterType == Enums.CharacterType.Recruit && setup.Move.Accuracy != -1) {
                                    setup.Move.Accuracy /= 2;
                                } else if (setup.Defender != null && setup.Defender.CharacterType == Enums.CharacterType.MapNpc && setup.Move.Accuracy != -1) {
                                    setup.Move.Accuracy *= 3;
                                    setup.Move.Accuracy /= 2;
                                }
                            }
                            break;
                        case Enums.Weather.Cloudy: {
                            }
                            break;
                        case Enums.Weather.Fog: {
                            }
                            break;
                        case Enums.Weather.Sunny: {
                                if (setup.moveIndex == 74) {
                                    setup.Move.Accuracy /= 2;
                                }
                                if (setup.moveIndex == 13) {
                                    setup.Move.Accuracy /= 2;
                                }
                                if (setup.moveIndex == 675) {
                                    setup.Move.Accuracy /= 2;
                                }
                            }
                            break;
                        case Enums.Weather.Sandstorm: {
                                if (setup.Move.MoveCategory == Enums.MoveCategory.Special && setup.Defender != null && (setup.Defender.Type1 == Enums.PokemonType.Rock || setup.Defender.Type2 == Enums.PokemonType.Rock)) {
                                    setup.DefenseStat *= 3;
                                    setup.DefenseStat /= 2;
                                }
                            }
                            break;
                        case Enums.Weather.Ashfall: {

                            }
                            break;

                    }
                    
                    //zoom lens
                    if (setup.Attacker.HasActiveItem(176)) {
                    	status = setup.Attacker.VolatileStatus.GetStatus("LastHitBy");
	                    if (status != null && status.Counter == 1) {
	                        if (status.Target == null) {
	                            setup.Attacker.VolatileStatus.Remove(status);
	                        } else if (status.Target == setup.Defender) {
	                            setup.Move.Accuracy *= 5;
                    			setup.Move.Accuracy /= 4;
	                        }
	                    }
                    }
                    
                    //detect band
                    if (setup.Defender.HasActiveItem(18)) {
                    	setup.Move.Accuracy *= 9;
                    	setup.Move.Accuracy /= 10;
                    }
                    
                    //brightpowder
                    if (setup.Defender.HasActiveItem(16)) {
                    	int dis = System.Math.Max(System.Math.Abs(setup.Defender.X - setup.Attacker.X), System.Math.Abs(setup.Defender.Y - setup.Attacker.Y));
                    	if (dis > 12) dis = 12;
                    	setup.Move.Accuracy *= (100-2*dis);
                    	setup.Move.Accuracy /= 100;
                    }
                    
                    
                
	                //effect of gravity
	                if (setup.Move.Accuracy != -1 && setup.AttackerMap.TempStatus.GetStatus("Gravity") != null) {
	                	setup.Move.Accuracy *= 2;
	                }


                    if (setup.Move.Accuracy != -1) {
                        if (HasAbility(setup.Defender, "Unaware") && !HasAbility(setup.Attacker, "Mold Breaker")) {
                            setup.Move.Accuracy = DamageCalculator.ApplyAccuracyBuff(setup.Move.Accuracy, 0);
                        } else {
                            setup.Move.Accuracy = DamageCalculator.ApplyAccuracyBuff(setup.Move.Accuracy, setup.Attacker.AccuracyBuff);
                        }
                        if (setup.Attacker.VolatileStatus.GetStatus("MiracleEye") != null || setup.Defender.VolatileStatus.GetStatus("Exposed") != null) {
                            setup.Move.Accuracy = DamageCalculator.ApplyEvasionBuff(setup.Move.Accuracy, 0);
                        } else {
                            if (HasAbility(setup.Attacker, "Unaware") && !HasAbility(setup.Attacker, "Mold Breaker") || setup.Move.AdditionalEffectData1 == 156) {
                                setup.Move.Accuracy = DamageCalculator.ApplyEvasionBuff(setup.Move.Accuracy, 0);
                            } else {
                                setup.Move.Accuracy = DamageCalculator.ApplyEvasionBuff(setup.Move.Accuracy, setup.Defender.EvasionBuff);
                            }
                        }
                        setup.Move.Accuracy /= 65536;
                    }



                    //normal attack has 0 crit rate and 1/2 damage
                    if (setup.moveIndex == -1) {
                        setup.Multiplier /= 2;
                        criticalRate = 0;
                    }

                    //Endure
                    if (setup.Defender.HasActiveItem(289) && setup.Defender.HP == setup.Defender.MaxHP
                        || setup.Defender.HasActiveItem(290) & Server.Math.Rand(0, 4) == 0
                        || setup.Defender.HasActiveItem(272) && setup.Move.Element == Enums.PokemonType.Water
                        || setup.Defender.HasActiveItem(273) && setup.Move.Element == Enums.PokemonType.Fire
                        || setup.Defender.HasActiveItem(274) && setup.Move.Element == Enums.PokemonType.Grass
                        || setup.Defender.HasActiveItem(275) && setup.Move.Element == Enums.PokemonType.Electric
                        || setup.Defender.HasActiveItem(276) && setup.Move.Element == Enums.PokemonType.Psychic
                        || setup.Defender.HasActiveItem(277) && setup.Move.Element == Enums.PokemonType.Normal
                        || setup.Defender.HasActiveItem(278) && setup.Move.Element == Enums.PokemonType.Ground
                        || setup.Defender.HasActiveItem(279) && setup.Move.Element == Enums.PokemonType.Bug
                        || setup.Defender.HasActiveItem(280) && setup.Move.Element == Enums.PokemonType.Dark
                        || setup.Defender.HasActiveItem(281) && setup.Move.Element == Enums.PokemonType.Fighting
                        || setup.Defender.HasActiveItem(282) && setup.Move.Element == Enums.PokemonType.Flying
                        || setup.Defender.HasActiveItem(283) && setup.Move.Element == Enums.PokemonType.Ghost
                        || setup.Defender.HasActiveItem(284) && setup.Move.Element == Enums.PokemonType.Dragon
                        || setup.Defender.HasActiveItem(285) && setup.Move.Element == Enums.PokemonType.Ice
                        || setup.Defender.HasActiveItem(286) && setup.Move.Element == Enums.PokemonType.Poison
                        || setup.Defender.HasActiveItem(287) && setup.Move.Element == Enums.PokemonType.Rock
                        || setup.Defender.HasActiveItem(288) && setup.Move.Element == Enums.PokemonType.Steel
                        || setup.Defender.HasActiveItem(340)) {
                        setup.KnockedOut = false;
                    }

                    if (setup.Defender.VolatileStatus.GetStatus("Endure") != null) {
                        setup.KnockedOut = false;
                    }

                    

                    //npc limitations
                    if (setup.Attacker.CharacterType == Enums.CharacterType.MapNpc) {
                        setup.Multiplier *= 3;
                        setup.Multiplier /= 4;
                    }


                    //OHKO
                    if (setup.Move.EffectType == Enums.MoveType.Scripted && setup.Move.Data1 == 39) {
                        setup.Move.Accuracy = 30 + setup.Attacker.Level - setup.Defender.Level;
                        if (setup.Move.Accuracy < 0) setup.Move.Accuracy = 0;
                    }

                    //perish song
                    if (setup.Move.EffectType == Enums.MoveType.Scripted && setup.Move.Data1 == 90) {
                        setup.Move.Accuracy = 30 + setup.Attacker.Level - setup.Defender.Level;
                        if (setup.Move.Accuracy < 0) setup.Move.Accuracy = 0;
                    }

                    //dream eater prereq
                    if (setup.Move.AdditionalEffectData1 == 31 && setup.Move.AdditionalEffectData2 == 1 && setup.Defender.StatusAilment != Enums.StatusAilment.Sleep) {
                        setup.Multiplier = 0;
                    }

					
		            if (setup.Defender.VolatileStatus.GetStatus("MagnetRise") != null && IsGroundImmune(setup.Defender, setup.DefenderMap) && setup.Defender != setup.Attacker) {
		                if (setup.Move.Element == Enums.PokemonType.Ground) {
		                    setup.Move.Accuracy = 0;
		                }
		            }
		            
		            if (setup.Defender.HasActiveItem(82) && IsGroundImmune(setup.Defender, setup.DefenderMap) && setup.Defender != setup.Attacker) {
		                if (setup.Move.Element == Enums.PokemonType.Ground) {
		                    setup.Move.Accuracy = 0;
		                }
		            }
		            
                    //scope lens
                    if (setup.Attacker.HasActiveItem(324)) {
                        criticalRate *= 2;
                    }


                    if (HasAbility(setup.Attacker, "Super Luck")) {
                        criticalRate *= 2;
                    }

                    //focus energy
                    if (setup.Attacker.VolatileStatus.GetStatus("FocusEnergy") != null) {
                        criticalRate *= setup.Attacker.VolatileStatus.GetStatus("FocusEnergy").Counter;
                    }

                    //reflect/Light Screen
                    if ((setup.Move.MoveCategory == Enums.MoveCategory.Physical || setup.Move.AdditionalEffectData1 == 146) &&
                        setup.Defender.VolatileStatus.GetStatus("Reflect") != null &&
                        !(HasAbility(setup.Attacker, "Infiltrator") || HasActiveBagItem(setup.Attacker, 7, 0, 0))) {
                        if (setup.Move.AdditionalEffectData1 != 136) {
                            setup.Multiplier *= 3;
                            setup.Multiplier /= 4;
                        }
                    }
                    if (setup.Move.MoveCategory == Enums.MoveCategory.Special && setup.Move.AdditionalEffectData1 != 146 &&
                        setup.Defender.VolatileStatus.GetStatus("LightScreen") != null &&
                        !(HasAbility(setup.Attacker, "Infiltrator") || HasActiveBagItem(setup.Attacker, 7, 0, 0))) {
                        if (setup.Move.AdditionalEffectData1 != 136) {
                            setup.Multiplier *= 3;
                            setup.Multiplier /= 4;
                        }
                    }


                    if (setup.Defender.VolatileStatus.GetStatus("LuckyChant") != null) {
                        criticalRate = 0;
                    }


                    if (HasAbility(setup.Defender, "Battle Armor") && !HasAbility(setup.Attacker, "Mold Breaker")) {
                        criticalRate = 0;
                    }
                    if (HasAbility(setup.Defender, "Shell Armor") && !HasAbility(setup.Attacker, "Mold Breaker")) {
                        criticalRate = 0;
                    }


                    if (HasAbility(setup.Defender, "No Guard") || HasAbility(setup.Attacker, "No Guard")) {
                        setup.Move.Accuracy = -1;
                    }


                    if (setup.Defender.VolatileStatus.GetStatus("Telekinesis") != null && setup.Defender != setup.Attacker) {
                        if (setup.Move.Element == Enums.PokemonType.Ground && IsGroundImmune(setup.Defender, setup.DefenderMap)) {
                            setup.Move.Accuracy = 0;
                        } else {
                            setup.Move.Accuracy = -1;
                        }
                    }


                    //Wonder Skin
                    if (HasAbility(setup.Defender, "Wonder Skin") && MoveProcessor.GetMatchupWith(setup.Attacker, setup.Defender) == Enums.CharacterMatchup.Foe && setup.Move.MoveCategory == Enums.MoveCategory.Status && !HasAbility(setup.Attacker, "Mold Breaker")) {
                        if (Server.Math.Rand(0, 2) == 0) {
                            setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + "'s Wonder Skin warded off the move!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                            setup.Move.Accuracy = 0;
                        }
                    }

                    //Wide Guard
                    if (setup.Attacker != setup.Defender && (setup.Move.RangeType == Enums.MoveRange.Floor || setup.Move.RangeType == Enums.MoveRange.Room || setup.Move.RangeType == Enums.MoveRange.FrontAndSides)
                        && setup.Defender.VolatileStatus.GetStatus("WideGuard") != null) {
                        if (setup.Move.AdditionalEffectData1 == 128) {
                            setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " broke through the protection!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                            RemoveExtraStatus(setup.Defender, setup.DefenderMap, "WideGuard", setup.PacketStack, false);
                        } else {
                            setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " is protected by Wide Guard!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                            setup.Move.Accuracy = 0;
                        }
                    }


                    //Quick Guard
                    if (setup.Attacker != setup.Defender && (setup.Move.RangeType == Enums.MoveRange.Floor || setup.Move.Range > 2)
                        && setup.Defender.VolatileStatus.GetStatus("QuickGuard") != null) {
                        if (setup.Move.AdditionalEffectData1 == 128) {
                            setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " broke through the protection!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                            RemoveExtraStatus(setup.Defender, setup.DefenderMap, "QuickGuard", setup.PacketStack, false);
                        } else {
                            setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " is protected by Quick Guard!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                            setup.Move.Accuracy = 0;
                        }
                    }

                    //protect
                    if (setup.Attacker != setup.Defender && setup.Defender.VolatileStatus.GetStatus("Protect") != null) {
                        if (setup.Move.AdditionalEffectData1 == 128) {
                            setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " broke through the protection!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                            RemoveExtraStatus(setup.Defender, setup.DefenderMap, "Protect", setup.PacketStack, false);
                        } else if (setup.Move.AdditionalEffectData1 == 60 && setup.Move.AdditionalEffectData2 == 9) {
                        
                        } else {
                            setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " protected itself!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                            setup.Move.Accuracy = 0;
                        }
                    }
                    
                    //semi-invulnerable
                    if (setup.Attacker != setup.Defender) {
                        status = setup.Defender.VolatileStatus.GetStatus("SemiInvul");
                        if (status != null) {
                            if (status.Tag == "Bounce") {
                            	if (setup.moveIndex == 88 || setup.moveIndex == 113) {//gust and twister
                                    setup.Multiplier *= 3;
                                    setup.Multiplier /= 2;
                                } else if (setup.moveIndex == 13 || setup.moveIndex == 334 ||
                                	setup.moveIndex == 675 || setup.moveIndex == 612) {//thunder/sky upper/hurricane/smack down

                                } else {
                                    setup.Move.Accuracy = 0;
                                }
                            } else if (status.Tag == "Fly") {
                                if (setup.moveIndex == 88 || setup.moveIndex == 113) {//gust and twister
                                    setup.Multiplier *= 3;
                                    setup.Multiplier /= 2;
                                } else if (setup.moveIndex == 13 || setup.moveIndex == 334 ||
                                	setup.moveIndex == 675 || setup.moveIndex == 612 || setup.moveIndex == 179) {//thunder/sky upper/hurricane/smack down

                                } else {
                                    setup.Move.Accuracy = 0;
                                }
                            } else if (status.Tag == "Dig") {
                                if (setup.moveIndex == 15 || setup.moveIndex == 45 || setup.moveIndex == 151) {
                                    setup.Multiplier *= 3;
                                    setup.Multiplier /= 2;
                                } else {
                                    setup.Move.Accuracy = 0;
                                }
                            } else if (status.Tag == "Dive") {
                                if (setup.moveIndex == 462 || setup.moveIndex == 203) {
                                    setup.Multiplier *= 3;
                                    setup.Multiplier /= 2;
                                } else {
                                    setup.Move.Accuracy = 0;
                                }
                            } else if (status.Tag == "ShadowForce") {
                                setup.Move.Accuracy = 0;
                            }
                        }
                        
                        status = setup.Defender.VolatileStatus.GetStatus("SkyDrop:0");
                        if (status != null) {
                        	if (setup.moveIndex == 88 || setup.moveIndex == 113 || 
                               	setup.moveIndex == 13 || setup.moveIndex == 334 ||
                              	setup.moveIndex == 675 || setup.moveIndex == 612 || setup.moveIndex == 179) {//thunder/sky upper/hurricane/smack down
                            } else {
                                setup.Move.Accuracy = 0;
                            }
                        }
                        status = setup.Defender.VolatileStatus.GetStatus("SkyDrop:1");
                        if (status != null) {
                        	if (setup.moveIndex == 88 || setup.moveIndex == 113 || 
                               	setup.moveIndex == 13 || setup.moveIndex == 334 ||
                              	setup.moveIndex == 675 || setup.moveIndex == 612 || setup.moveIndex == 179) {//thunder/sky upper/hurricane/smack down
                            } else {
                                setup.Move.Accuracy = 0;
                            }
                        }
                    }



                    //checks against ability-based avoidance
                    CheckDefenderModAbility(setup);

					//AoE drawbacks
                    if (!(BattleProcessor.HitsSelf(setup.Move.TargetType) || BattleProcessor.HitsAllies(setup.Move.TargetType)) || setup.Move.Accuracy != -1) {
	                    if (setup.Move.RangeType == Enums.MoveRange.Room || setup.Move.RangeType == Enums.MoveRange.Floor) {
	                        if (setup.Move.Range > 1) {
		                        if (setup.Move.Range > 2) {
		                        	if (setup.Move.Accuracy == -1 || setup.Move.Accuracy > 100) {
		                            	setup.Move.Accuracy = 100;
		                            }
		                            setup.Move.Accuracy *= 3;
		                            setup.Move.Accuracy /= 4;
		                        }
		                        setup.Multiplier *= 3;
		                        setup.Multiplier /= 4;
	                        }
	                    } else if (setup.Move.RangeType == Enums.MoveRange.FrontAndSides && setup.Move.Range > 2) {
	                    	if (setup.Move.Accuracy == -1 || setup.Move.Accuracy > 100) {
	                    		setup.Move.Accuracy = 100;
	                    	}
	                    	setup.Multiplier *= 4;
	                        setup.Multiplier /= 5;
	                        setup.Move.Accuracy *= 4;
	                        setup.Move.Accuracy /= 5;
	                    }
                    }
                    
                    //haze and pain split hit no matter what
                    if (setup.Move.EffectType == Enums.MoveType.Scripted &&
                    	(setup.Move.Data1 == 103 || setup.Move.AdditionalEffectData1 == 103 || setup.Move.Data1 == 204 || setup.Move.Data1 == 209)) {
                    	setup.Move.Accuracy = -1;
                    }
                    
                    //safe zone attack-all protection
                    if (SafeZoneCheck(setup.Attacker, setup.AttackerMap, setup.Defender, setup.DefenderMap)) {
                        if (BattleProcessor.HitsAllies(setup.Move.TargetType) && setup.Move.MoveCategory != Enums.MoveCategory.Status
                            || setup.Move.EffectType == Enums.MoveType.Scripted && setup.Move.Data1 == 90) {
                           
                            setup.Move.Accuracy = 0;
                            
                            // Used in the snowball game - this is the snowball move
                            if (setup.moveIndex == 526) {
                            	setup.Move.Accuracy = 100;
                            }
                        }
                    }
					
                    //see if it hits
                    if (setup.Move.Accuracy == -1 || Server.Math.Rand(0, 100) < setup.Move.Accuracy) {
                        setup.Hit = true;
                        if (!setup.BattleTags.Contains("HitSomething")) setup.BattleTags.Add("HitSomething");


                        if (setup.Move.EffectType == Enums.MoveType.SubHP && setup.Defender != null && criticalRate > 0 && Server.Math.Rand(0, 10000) < criticalRate) {//see if it criticals
                            //setup.Critical = true;
                            if (HasAbility(setup.Attacker, "Sniper")) {
                                setup.Multiplier *= 5;
                            } else {
                                setup.Multiplier *= 3;
                            }
                            setup.Multiplier /= 2;
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("A critical hit!", Text.WhiteSmoke), setup.Defender.X, setup.Defender.Y, 10);
                            setup.BattleTags.Add("Critical");
                            
                        } else {
                            int stockpileBonus = 0;
                            status = setup.Defender.VolatileStatus.GetStatus("Stockpile");
                            if (status != null) {
                                stockpileBonus = status.Counter;
                            }
                            if (setup.Move.MoveCategory == Enums.MoveCategory.Physical && setup.Defender != null) {
                                if (setup.Move.AdditionalEffectData1 == 157) {
                                    setup.AttackStat = DamageCalculator.ApplyAttackBuff(setup.AttackStat, setup.Defender.AttackBuff) / 256;
                                }
                                if ((!HasAbility(setup.Attacker, "Unaware") || HasAbility(setup.Defender, "Mold Breaker")) && setup.Move.AdditionalEffectData1 != 156) {

                                    setup.DefenseStat = DamageCalculator.ApplyDefenseBuff(setup.DefenseStat, setup.Defender.DefenseBuff + stockpileBonus) / 256;
                                }
                            } else if (setup.Move.MoveCategory == Enums.MoveCategory.Special && setup.Defender != null) {
                                if (setup.Move.AdditionalEffectData1 == 157) {
                                    setup.AttackStat = DamageCalculator.ApplyAttackBuff(setup.AttackStat, setup.Defender.SpAtkBuff) / 256;
                                }
                                if (!HasAbility(setup.Attacker, "Unaware") || HasAbility(setup.Defender, "Mold Breaker")) {
                                    if (setup.Move.AdditionalEffectData1 == 146) {
                                        setup.DefenseStat = DamageCalculator.ApplyDefenseBuff(setup.DefenseStat, setup.Defender.DefenseBuff + stockpileBonus) / 256;
                                    } else if (setup.Move.AdditionalEffectData1 != 156){
                                        setup.DefenseStat = DamageCalculator.ApplyDefenseBuff(setup.DefenseStat, setup.Defender.SpDefBuff + stockpileBonus) / 256;
                                    }
                                }
                            }
                        }

                        if (setup.Defender != null && setup.Move.EffectType == Enums.MoveType.SubHP) {
                            int effectiveness = DamageCalculator.CalculateTypeMatchup(setup.Move.Element, setup.Defender.Type1) + DamageCalculator.CalculateTypeMatchup(setup.Move.Element, setup.Defender.Type2);

                            if (setup.Move.Element == Enums.PokemonType.Psychic && setup.Attacker.VolatileStatus.GetStatus("MiracleEye") != null) {
                                if (setup.Defender.Type1 == Enums.PokemonType.Dark) {
                                    effectiveness += 3;
                                }
                                if (setup.Defender.Type2 == Enums.PokemonType.Dark) {
                                    effectiveness += 3;
                                }
                            }
                            
                            if (setup.Move.Element == Enums.PokemonType.Ground && !IsGroundImmune(setup.Defender, setup.DefenderMap)) {
                                if (setup.Defender.Type1 == Enums.PokemonType.Flying) {
                                    effectiveness += 3;
                                }
                                if (setup.Defender.Type2 == Enums.PokemonType.Flying) {
                                    effectiveness += 3;
                                }
                            }

                            if ((setup.Move.Element == Enums.PokemonType.Normal || setup.Move.Element == Enums.PokemonType.Fighting) && (HasAbility(setup.Attacker, "Scrappy") || setup.Defender.VolatileStatus.GetStatus("Exposed") != null)) {
                                if (setup.Defender.Type1 == Enums.PokemonType.Ghost) {
                                    effectiveness += 3;
                                }
                                if (setup.Defender.Type2 == Enums.PokemonType.Ghost) {
                                    effectiveness += 3;
                                }
                            }
                            
                            if (effectiveness < 6 && setup.Defender.HasActiveItem(372)) {
                            	effectiveness = 6;
                            }


                            if (effectiveness != 6) {
                                setup.Multiplier *= DamageCalculator.Effectiveness[effectiveness];
                                setup.Multiplier /= 256;

                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(BattleProcessor.EffectivenessToPhrase(effectiveness), Text.WhiteSmoke), setup.Defender.X, setup.Defender.Y, 10);
                            }

                            //Expert Belt
                            if (setup.Attacker.HasActiveItem(38) && effectiveness > 6) {
                                setup.Multiplier *= 11;
                                setup.Multiplier /= 10;
                            }
                            
                            if (setup.Defender.HasActiveItem(120)) {
                            	TargetCollection frontTargets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.FrontAndSides, 50, setup.DefenderMap, setup.Defender, setup.Defender.X, setup.Defender.Y, setup.Defender.Direction, true, true, false);
                            	bool front = false;
                            	for (int i = 0; i < frontTargets.Count; i++) {
                            		if (frontTargets[i] == setup.Attacker) {
                            			front = true;
                            			break;
                            		}
                            	}
                            	if (true) {
			                        int damage = DamageCalculator.CalculateDamage(setup.Move.Data1, setup.AttackStat, setup.Attacker.Level, setup.DefenseStat) * setup.Multiplier / 1000;
	                                setup.Defender.HeldItem.Tag = (setup.Defender.HeldItem.Tag.ToInt() - damage).ToString();
			                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("The Pokédoll took the attack instead!", Text.BrightGreen), setup.Defender.X, setup.Defender.Y, 10);
			                        setup.Multiplier = -1;
		                        }
		                        //take held item
		                    }

                            //substitute
                            status = setup.Defender.VolatileStatus.GetStatus("Substitute");
                            if (status != null) {
                                int damage = DamageCalculator.CalculateDamage(setup.Move.Data1, setup.AttackStat, setup.Attacker.Level, setup.DefenseStat) * setup.Multiplier / 1000;
                                status.Counter -= damage;
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + "'s substitute took " + damage + " damage!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                                setup.Multiplier = -1;
                            }
                        }
                    } else {
                        setup.Hit = false;
                        //setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg("The move missed " + setup.Defender.Name + "!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                    }
                    setup.BattleTags.Add("PrevHP:" + setup.Defender.HP);
                } else {
                    if (followAway) {
                        setup.Hit = false;
                    } else {
                        setup.Hit = true;
                    }
                }
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: BeforeMoveHits", Text.Black);
            }
        }

        public static void ScriptedMoveHitCharacter(BattleSetup setup) {
            try {
                ExtraStatus status;
                switch (setup.Move.Data1) {
                	case -6: {
                			if (setup.AttackerMap.MapType == Enums.MapType.Instanced && ((InstancedMap)setup.AttackerMap).MapBase == 1945
                				&& setup.DefenderMap.MapType == Enums.MapType.Instanced && ((InstancedMap)setup.DefenderMap).MapBase == 1945) {
	                			if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit &&
	                                setup.Defender.CharacterType == Enums.CharacterType.Recruit) {
	
	                                Client attacker = ((Recruit)setup.Attacker).Owner;
	                                Client defender = ((Recruit)setup.Defender).Owner;
									
									SnowballGame attackerInstance = exPlayer.Get(attacker).SnowballGameInstance;
									SnowballGame defenderInstance = exPlayer.Get(defender).SnowballGameInstance;
									if (attackerInstance != null && defenderInstance != null
										&& attackerInstance == defenderInstance) {
										
										attackerInstance.HandleHit(attacker, defender);
	                                }
	                            }
	                    	}
                		}
                		break;
                    case -5: {//laser tag
                            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit &&
                                setup.Defender.CharacterType == Enums.CharacterType.Recruit) {

                                Client attacker = ((Recruit)setup.Attacker).Owner;
                                Client defender = ((Recruit)setup.Defender).Owner;

                                Messenger.PlayerMsg(defender, "Hit by laser by " + attacker.Player.Name + "! You lost some points!", Text.BrightRed);
                                Messenger.PlayerMsg(attacker, "You hit " + defender.Player.Name + " and gained some points!", Text.BrightRed);
                            }
                        }
                        break;
                    case -4: {//HP heal
                            HealCharacter(setup.Defender, setup.DefenderMap, setup.Move.Data2, setup.PacketStack);
                        }
                        break;
                    case -3: {//PP heal
                            HealCharacterPP(setup.Defender, setup.DefenderMap, setup.Move.Data2, setup.PacketStack);
                        }
                        break;
                    case -2: {//Full Restore
                            HealCharacter(setup.Defender, setup.DefenderMap, setup.Defender.MaxHP, setup.PacketStack);
                            SetStatusAilment(setup.Defender, setup.DefenderMap, Enums.StatusAilment.OK, 0, setup.PacketStack);
                            //Confuse(setup.Defender, setup.DefenderMap, 0, setup.PacketStack);
                        }
                        break;

                    case 1: {//burn
                            SetStatusAilment(setup.Defender, setup.Attacker, setup.DefenderMap, Enums.StatusAilment.Burn, 1, setup.PacketStack);

                        }
                        break;
                    case 2: {//freeze
                            SetStatusAilment(setup.Defender, setup.Attacker, setup.DefenderMap, Enums.StatusAilment.Freeze, 1, setup.PacketStack);

                        }
                        break;
                    case 3: {//paralysis
                            SetStatusAilment(setup.Defender, setup.Attacker, setup.DefenderMap, Enums.StatusAilment.Paralyze, 1, setup.PacketStack);

                        }
                        break;
                    case 4: {//poison
                            SetStatusAilment(setup.Defender, setup.Attacker, setup.DefenderMap, Enums.StatusAilment.Poison, setup.Move.Data2, setup.PacketStack);

                        }
                        break;
                    case 5: {//sleep
                            SetStatusAilment(setup.Defender, setup.Attacker, setup.DefenderMap, Enums.StatusAilment.Sleep, Server.Math.Rand(setup.Move.Data2, setup.Move.Data3), setup.PacketStack);

                        }
                        break;
                    case 6: {//confuse
                            AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Confusion", Server.Math.Rand(5, 11), null, "", setup.PacketStack, true);
                            //Confuse(setup.Defender, setup.Attacker, setup.DefenderMap, Server.Math.Rand(5, 11), setup.PacketStack);
                        }
                        break;
                    case 7: {//flinch
							if (Server.Math.Rand(0, 100) < setup.Move.Data2) {
                                Flinch(setup.Defender, setup.Attacker, setup.DefenderMap, setup.PacketStack);
                            }
                        }
                        break;
                    case 8: {//pause

                        }
                        break;
                    case 14: {//atk ~ self

                        }
                        break;
                    case 15: {//def ~ self

                        }
                        break;
                    case 16: {//spatk ~ self

                        }
                        break;
                    case 17: {//spdef ~ self

                        }
                        break;
                    case 18: {//speed ~ self

                        }
                        break;
                    case 19: {//acc ~ self

                        }
                        break;
                    case 20: {//eva ~ self

                        }
                        break;
                    case 21: {//atk
                            ChangeAttackBuff(setup.Defender, setup.Attacker, setup.DefenderMap, setup.Move.Data2, setup.PacketStack);
                        }
                        break;
                    case 22: {//def
                            ChangeDefenseBuff(setup.Defender, setup.Attacker, setup.DefenderMap, setup.Move.Data2, setup.PacketStack);
                        }
                        break;
                    case 23: {//spatk
                            ChangeSpAtkBuff(setup.Defender, setup.Attacker, setup.DefenderMap, setup.Move.Data2, setup.PacketStack);
                        }
                        break;
                    case 24: {//spdef
                            ChangeSpDefBuff(setup.Defender, setup.Attacker, setup.DefenderMap, setup.Move.Data2, setup.PacketStack);
                        }
                        break;
                    case 25: {//speed
                            ChangeSpeedBuff(setup.Defender, setup.Attacker, setup.DefenderMap, setup.Move.Data2, setup.PacketStack);
                        }
                        break;
                    case 26: {//acc
                            ChangeAccuracyBuff(setup.Defender, setup.Attacker, setup.DefenderMap, setup.Move.Data2, setup.PacketStack);
                        }
                        break;
                    case 27: {//eva
                            ChangeEvasionBuff(setup.Defender, setup.Attacker, setup.DefenderMap, setup.Move.Data2, setup.PacketStack);
                        }
                        break;
                    case 28: {//recoil

                        }
                        break;
                    case 29: {//critical

                        }
                        break;
                    case 30: {//set dmg
                            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                                DamageCharacter(setup.Defender, setup.DefenderMap, setup.Move.Data2, Enums.KillType.Player, setup.PacketStack, false);
                            } else {
                                DamageCharacter(setup.Defender, setup.DefenderMap, setup.Move.Data2, Enums.KillType.Npc, setup.PacketStack, false);
                            }
                            setup.Damage = setup.Move.Data2;
                        }
                        break;
                    case 34: {//Rest
                            if (!HasAbility(setup.Defender, "Insomnia") && setup.Defender.VolatileStatus.GetStatus("Sleepless") == null) {
                                setup.Defender.StatusAilment = Enums.StatusAilment.Sleep;
                                setup.Defender.StatusAilmentCounter = 4;
                                setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " went to sleep and became healthy!", Text.BrightGreen), setup.Defender.X, setup.Defender.Y, 10);
                                if (setup.Defender.VolatileStatus.GetStatus("Nightmare") != null) {
                                    setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " couldn't recover HP by sleeping due to the nightmare!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                                } else {
                                    HealCharacter(setup.Defender, setup.DefenderMap, setup.Defender.MaxHP, setup.PacketStack);
                                }
                                if (setup.Defender.CharacterType == Enums.CharacterType.Recruit) {
                                    PacketBuilder.AppendStatusAilment(((Recruit)setup.Defender).Owner, setup.PacketStack);
                                } else {
                                    PacketBuilder.AppendNpcStatusAilment((MapNpc)setup.Defender, setup.PacketStack);
                                }
                            } else {
                                setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " couldn't fall asleep!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                            }
                        }
                        break;
                    case 36: {//sleep talk
                    		if (setup.Attacker.StatusAilment == Enums.StatusAilment.Sleep && GetBattleTagArg(setup.BattleTags, "InvokedMove", 0) == null) {
								List<int> slots = new List<int>();
	                	
			                	for (int i = 0; i < Constants.MAX_PLAYER_MOVES; i++) {
			                		if (setup.Attacker.Moves[i].MoveNum > 0 && setup.moveSlot != i) {
			                			slots.Add(i);
			                		}
			                	}
			                	
			                	if (slots.Count > 0) {
				                	BattleSetup subSetup = new BattleSetup();
				                    subSetup.Attacker = setup.Attacker;
				                    subSetup.moveSlot = -1;
				                    subSetup.moveIndex = setup.Attacker.Moves[slots[Server.Math.Rand(0, slots.Count)]].MoveNum;
				                    
									subSetup.BattleTags.Add("InvokedMove");
									BattleProcessor.HandleAttack(subSetup);
				
				
				                    BattleProcessor.FinalizeAction(subSetup, setup.PacketStack);
				                	
				                    setup.Cancel = true;
				                    return;
			                    } else {
			                    	setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " couldn't pick a move!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
			                    }
		                    } else {
		                    	setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " isn't asleep!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
		                    }
                        }
                        break;
                    case 39: {//OHKO
                            if (DamageCalculator.CalculateTypeMatchup(setup.Move.Element, setup.Defender.Type1) == 0 || DamageCalculator.CalculateTypeMatchup(setup.Move.Element, setup.Defender.Type2) == 0) {
                                setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg("Poor type match-up!  " + setup.Defender.Name + " took no damage!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                            } else {
                            	int damage = setup.Defender.HP;
                                if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                                    DamageCharacter(setup.Defender, setup.DefenderMap, damage, Enums.KillType.Player, setup.PacketStack, false);
                                } else {
                                    DamageCharacter(setup.Defender, setup.DefenderMap, damage, Enums.KillType.Npc, setup.PacketStack, false);
                                }
                                setup.Damage = damage;
                            }
                        }
                        break;
                    case 40: {//Heal
                            switch (setup.Move.Data2) {
                                case 1: {//Weather Dependent
                                        switch (GetCharacterWeather(setup.Attacker)) {
                                            case Enums.Weather.Sunny: {
                                                    HealCharacter(setup.Defender, setup.DefenderMap, 120, setup.PacketStack);
                                                }
                                                break;
                                            case Enums.Weather.Cloudy:
                                            case Enums.Weather.Raining:
                                            case Enums.Weather.Snowing: {
                                                    HealCharacter(setup.Defender, setup.DefenderMap, 60, setup.PacketStack);
                                                }
                                                break;
                                            case Enums.Weather.Thunder:
                                            case Enums.Weather.Snowstorm:
                                            case Enums.Weather.Sandstorm:
                                            case Enums.Weather.Hail: {
                                                    HealCharacter(setup.Defender, setup.DefenderMap, 40, setup.PacketStack);
                                                }
                                                break;
                                            case Enums.Weather.Fog:
                                            case Enums.Weather.Ashfall: {
                                                    HealCharacter(setup.Defender, setup.DefenderMap, 1, setup.PacketStack);
                                                }
                                                break;
                                            default: {
                                                    HealCharacter(setup.Defender, setup.DefenderMap, 80, setup.PacketStack);
                                                }
                                                break;

                                        }
                                    }
                                    break;
                                case 2: {//user HP dependent
                                        HealCharacter(setup.Defender, setup.DefenderMap, setup.Attacker.MaxHP / setup.Move.Data3, setup.PacketStack);
                                    }
                                    break;
                                case 3: {//Target HP dependent
                                        HealCharacter(setup.Defender, setup.DefenderMap, setup.Defender.MaxHP / setup.Move.Data3, setup.PacketStack);
                                    }
                                    break;
                                case 4: {//Stockpile-dependent
                                    status = setup.Defender.VolatileStatus.GetStatus("Stockpile");
                                        if (status == null) {
                                            setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg("But there was nothing to swallow!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                                        } else {
                                            HealCharacter(setup.Defender, setup.DefenderMap, setup.Defender.MaxHP * status.Counter / 3, setup.PacketStack);
                                            RemoveExtraStatus(setup.Defender, setup.DefenderMap, "Stockpile", setup.PacketStack);
                                        }
                                    }
                                    break;
                                case 5: {//roost
                                        if (!CheckStatusProtection(setup.Defender, setup.DefenderMap, "Roost", true, setup.PacketStack)) {
                                            HealCharacter(setup.Defender, setup.DefenderMap, setup.Defender.MaxHP / setup.Move.Data3, setup.PacketStack);
                                            AddExtraStatus(setup.Defender, setup.DefenderMap, "Roost", 0, null, "", setup.PacketStack);
                                        }
                                    }
                                    break;

                            }
                        }
                        break;
                    case 42: {//Recover status
                            switch (setup.Move.Data2) {
                                case 1: {//Refresh; heals team members
                                        if (setup.Defender.CharacterType == Enums.CharacterType.Recruit) {
                                            Client client = ((Recruit)setup.Defender).Owner;
                                            for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
                                                if (client.Player.Team[i].Loaded) {
                                                    if (!CheckStatusProtection(client.Player.Team[i], setup.DefenderMap, Enums.StatusAilment.OK.ToString(), true, setup.PacketStack)) {
                                                        client.Player.Team[i].StatusAilment = Enums.StatusAilment.OK;
                                                        client.Player.Team[i].StatusAilmentCounter = 0;
                                                    }
                                                }
                                            }
                                            PacketBuilder.AppendStatusAilment(((Recruit)setup.Defender).Owner, setup.PacketStack);
                                            setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg("All of " + setup.Defender.Name + "'s team members were cured of their status problems!", Text.BrightCyan), setup.Defender.X, setup.Defender.Y, 10);
                                        } else {
                                            SetStatusAilment(setup.Defender, setup.DefenderMap, Enums.StatusAilment.OK, 0, setup.PacketStack);
                                        }
                                    }
                                    break;
                                case 2: {//All status
                                        SetStatusAilment(setup.Defender, setup.DefenderMap, Enums.StatusAilment.OK, 0, setup.PacketStack);
                                    }
                                    break;
                                case 3: {//nonvolatile status
                                        SetStatusAilment(setup.Defender, setup.DefenderMap, Enums.StatusAilment.OK, 0, setup.PacketStack);
                                    }
                                    break;
                            }

                        }
                        break;
                    case 43: {//Heals all others; drop to 1 HP
                            switch (setup.Move.Data2) {
                                case 1: {//Healing Wish
                                        HealCharacter(setup.Defender, setup.DefenderMap, setup.Defender.MaxHP, setup.PacketStack);
                                        SetStatusAilment(setup.Defender, setup.DefenderMap, Enums.StatusAilment.OK, 0, setup.PacketStack);
                                        //Confuse(setup.Defender, setup.DefenderMap, 0, setup.PacketStack);
                                    }
                                    break;
                                case 2: {//Lunar Dance
                                        HealCharacter(setup.Defender, setup.DefenderMap, setup.Defender.MaxHP, setup.PacketStack);
                                        SetStatusAilment(setup.Defender, setup.DefenderMap, Enums.StatusAilment.OK, 0, setup.PacketStack);
                                        //Confuse(setup.Defender, setup.DefenderMap, 0, setup.PacketStack);
                                        ChangeDefenseBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 1, setup.PacketStack);
                                        ChangeSpDefBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 1, setup.PacketStack);
                                    }
                                    break;
                            }
                        }
                        break;
                    case 44: {//Weather change

                            if (setup.AttackerMap.Tile[setup.Attacker.X, setup.Attacker.Y].Type == Enums.TileType.Arena || setup.AttackerMap.Moral == Enums.MapMoral.None || setup.AttackerMap.Moral == Enums.MapMoral.NoPenalty) {
                                SetMapWeather(setup.AttackerMap, (Enums.Weather)setup.Move.Data2, setup.PacketStack);
                            } else {
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("The weather can only change in dungeons and arenas!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                            }
                        }
                        break;
                    case 45: {//Raises Two Stats
                            switch (setup.Move.Data2) {
                                case 1: {
                                        ChangeAttackBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 1, setup.PacketStack);
                                    }
                                    break;
                                case 2: {
                                        ChangeDefenseBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 1, setup.PacketStack);
                                    }
                                    break;
                                case 3: {
                                        ChangeSpAtkBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 1, setup.PacketStack);
                                    }
                                    break;
                                case 4: {
                                        ChangeSpDefBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 1, setup.PacketStack);
                                    }
                                    break;
                                case 5: {
                                        ChangeSpeedBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 1, setup.PacketStack);
                                    }
                                    break;
                                case 6: {
                                        ChangeAccuracyBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 1, setup.PacketStack);
                                    }
                                    break;
                                case 7: {
                                        ChangeEvasionBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 1, setup.PacketStack);
                                    }
                                    break;
                            }
                            switch (setup.Move.Data3) {
                                case 1: {
                                        ChangeAttackBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 1, setup.PacketStack);
                                    }
                                    break;
                                case 2: {
                                        ChangeDefenseBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 1, setup.PacketStack);
                                    }
                                    break;
                                case 3: {
                                        ChangeSpAtkBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 1, setup.PacketStack);
                                    }
                                    break;
                                case 4: {
                                        ChangeSpDefBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 1, setup.PacketStack);
                                    }
                                    break;
                                case 5: {
                                        ChangeSpeedBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 1, setup.PacketStack);
                                    }
                                    break;
                                case 6: {
                                        ChangeAccuracyBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 1, setup.PacketStack);
                                    }
                                    break;
                                case 7: {
                                        ChangeEvasionBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 1, setup.PacketStack);
                                    }
                                    break;
                            }

                        }
                        break;
                    case 52: {//recycle
                            if (setup.Attacker.HeldItem == null) {
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " isn't holding anything!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                            } else if (setup.Attacker.HeldItem.Num != 74 && setup.Attacker.HeldItem.Num != 75) {
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("The " + ItemManager.Items[setup.Attacker.HeldItem.Num].Name + " isn't grimy!  It couldn't be recycled!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                            } else if (setup.Attacker.HeldItem.Tag.IsNumeric() && setup.Attacker.HeldItem.Tag.ToInt() > 0) {
                                int grimyItem = setup.Attacker.HeldItem.Num;
                                setup.Attacker.HeldItem.Num = setup.Attacker.HeldItem.Tag.ToInt();
                                setup.Attacker.HeldItem.Tag = "";
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("The " + ItemManager.Items[grimyItem].Name + " was recycled back into a " + ItemManager.Items[setup.Attacker.HeldItem.Num].Name + "!", Text.BrightGreen), setup.Attacker.X, setup.Attacker.Y, 10);
                            } else {
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("The " + ItemManager.Items[setup.Attacker.HeldItem.Num].Name + " was too spoiled.  It couldn't be recycled.", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                            }
                            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                                PacketBuilder.AppendInventory(((Recruit)setup.Attacker).Owner, setup.PacketStack);
                            }
                        }
                        break;
                    case 53: {//level dmg
                            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                                DamageCharacter(setup.Defender, setup.DefenderMap, setup.Attacker.Level, Enums.KillType.Player, setup.PacketStack, false);
                            } else {
                                DamageCharacter(setup.Defender, setup.DefenderMap, setup.Attacker.Level, Enums.KillType.Npc, setup.PacketStack, false);
                            }
                            setup.Damage = setup.Attacker.Level;
                        }
                        break;
                    case 54: {//random dmg
                            int rand = Server.Math.Rand((setup.Attacker.Level / 2 + 1), (setup.Attacker.Level * 3 / 2 + 1));
                            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                                DamageCharacter(setup.Defender, setup.DefenderMap, rand, Enums.KillType.Player, setup.PacketStack, false);
                            } else {
                                DamageCharacter(setup.Defender, setup.DefenderMap, rand, Enums.KillType.Npc, setup.PacketStack, false);
                            }
                            setup.Damage = rand;
                        }
                        break;
                    case 55: {//HP fraction dmg
                            int fraction = 0;
                            if (setup.Move.Data2 == 1) {
                                fraction = setup.Defender.MaxHP / setup.Move.Data3;
                            } else {
                                fraction = setup.Defender.HP / setup.Move.Data3;
                            }
                            if (HasActiveBagItem(setup.Defender, 6, 0, 0)) fraction = 1;
                            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                                DamageCharacter(setup.Defender, setup.DefenderMap, fraction, Enums.KillType.Player, setup.PacketStack, false);
                            } else {
                                DamageCharacter(setup.Defender, setup.DefenderMap, fraction, Enums.KillType.Npc, setup.PacketStack, false);
                            }
                            setup.Damage = fraction;
                        }
                        break;
                    case 56: {// Endeavor
                            int HPDmg = 0;
                            if (setup.Attacker.HP < setup.Defender.HP) {
                                HPDmg = setup.Defender.HP - setup.Attacker.HP;
                            }
                            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                                DamageCharacter(setup.Defender, setup.DefenderMap, HPDmg, Enums.KillType.Player, setup.PacketStack, false);
                            } else {
                                DamageCharacter(setup.Defender, setup.DefenderMap, HPDmg, Enums.KillType.Npc, setup.PacketStack, false);
                            }
                            setup.Damage = HPDmg;
                        }
                        break;
                    case 57: {//Immobilize
                            AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Immobilize", Server.Math.Rand(5, 8), null, "", setup.PacketStack, true);
                        }
                        break;
                    case 59: {//Leech Seed
                            if (setup.Attacker.VolatileStatus.GetStatus("LeechSeed:1") == null && setup.Defender.VolatileStatus.GetStatus("LeechSeed:0") == null && !CheckStatusProtection(setup.Defender, setup.Attacker, setup.DefenderMap, "LeechSeed:0", true, setup.PacketStack)) {
                                AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "LeechSeed:0", 0, setup.Attacker, "", setup.PacketStack, true);
                                AddExtraStatus(setup.Attacker, setup.AttackerMap, "LeechSeed:1", 0, setup.Defender, "", setup.PacketStack, true);
                            }
                        }
                        break;
                    case 64: {//Curse
                            if (setup.Attacker.Type1 == Enums.PokemonType.Ghost || setup.Attacker.Type2 == Enums.PokemonType.Ghost) {
                                int curseDmg = GetBattleTagArg(setup.BattleTags, "Curse", 1).ToInt(-1);
                                AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Curse", curseDmg, null, "", setup.PacketStack, true);
                            } else {
                                ChangeAttackBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 1, setup.PacketStack);
                                ChangeDefenseBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 1, setup.PacketStack);
                                ChangeSpeedBuff(setup.Defender, setup.Attacker, setup.DefenderMap, -1, setup.PacketStack);
                            }
                        }
                        break;
                    case 65: {//Defense Curl
                            ChangeDefenseBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 1, setup.PacketStack);
                            AddExtraStatus(setup.Defender, setup.DefenderMap, "DefenseCurl", 0, null, "", setup.PacketStack);
                        }
                        break;
                    case 66: {//Minimize
                            ChangeEvasionBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 2, setup.PacketStack);
                            AddExtraStatus(setup.Defender, setup.DefenderMap, "Minimize", 0, null, "", setup.PacketStack);
                        }
                        break;
                    case 67: {//Growth
                            if (GetCharacterWeather(setup.Defender) == Enums.Weather.Sunny) {
                                ChangeAttackBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 2, setup.PacketStack);
                                ChangeSpAtkBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 2, setup.PacketStack);
                            } else {
                                ChangeAttackBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 1, setup.PacketStack);
                                ChangeSpAtkBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 1, setup.PacketStack);
                            }
                        }
                        break;
                    case 68: {//Bide, doesn't do anything here
                            //AddExtraStatus(setup.Defender, setup.DefenderMap, "Bide", 0, null, setup.PacketStack);
                        }
                        break;
                    case 69: {//stockpile
                        status = setup.Defender.VolatileStatus.GetStatus("Stockpile");
                            if (status != null) {
                                AddExtraStatus(setup.Defender, setup.DefenderMap, "Stockpile", status.Counter + 1, null, "", setup.PacketStack);
                            } else {
                                AddExtraStatus(setup.Defender, setup.DefenderMap, "Stockpile", 1, null, "", setup.PacketStack);
                            }
                        }
                        break;
                    case 71: {//Lowers Two Stats
                            switch (setup.Move.Data2) {
                                case 1: {
                                        ChangeAttackBuff(setup.Defender, setup.Attacker, setup.DefenderMap, -1, setup.PacketStack);
                                    }
                                    break;
                                case 2: {
                                        ChangeDefenseBuff(setup.Defender, setup.Attacker, setup.DefenderMap, -1, setup.PacketStack);
                                    }
                                    break;
                                case 3: {
                                        ChangeSpAtkBuff(setup.Defender, setup.Attacker, setup.DefenderMap, -1, setup.PacketStack);
                                    }
                                    break;
                                case 4: {
                                        ChangeSpDefBuff(setup.Defender, setup.Attacker, setup.DefenderMap, -1, setup.PacketStack);
                                    }
                                    break;
                                case 5: {
                                        ChangeSpeedBuff(setup.Defender, setup.Attacker, setup.DefenderMap, -1, setup.PacketStack);
                                    }
                                    break;
                                case 6: {
                                        ChangeAccuracyBuff(setup.Defender, setup.Attacker, setup.DefenderMap, -1, setup.PacketStack);
                                    }
                                    break;
                                case 7: {
                                        ChangeEvasionBuff(setup.Defender, setup.Attacker, setup.DefenderMap, -1, setup.PacketStack);
                                    }
                                    break;
                            }
                            switch (setup.Move.Data3) {
                                case 1: {
                                        ChangeAttackBuff(setup.Defender, setup.Attacker, setup.DefenderMap, -1, setup.PacketStack);
                                    }
                                    break;
                                case 2: {
                                        ChangeDefenseBuff(setup.Defender, setup.Attacker, setup.DefenderMap, -1, setup.PacketStack);
                                    }
                                    break;
                                case 3: {
                                        ChangeSpAtkBuff(setup.Defender, setup.Attacker, setup.DefenderMap, -1, setup.PacketStack);
                                    }
                                    break;
                                case 4: {
                                        ChangeSpDefBuff(setup.Defender, setup.Attacker, setup.DefenderMap, -1, setup.PacketStack);
                                    }
                                    break;
                                case 5: {
                                        ChangeSpeedBuff(setup.Defender, setup.Attacker, setup.DefenderMap, -1, setup.PacketStack);
                                    }
                                    break;
                                case 6: {
                                        ChangeAccuracyBuff(setup.Defender, setup.Attacker, setup.DefenderMap, -1, setup.PacketStack);
                                    }
                                    break;
                                case 7: {
                                        ChangeEvasionBuff(setup.Defender, setup.Attacker, setup.DefenderMap, -1, setup.PacketStack);
                                    }
                                    break;
                            }

                        }
                        break;
                    case 75: {//Present
                            int rand = GetBattleTagArg(setup.BattleTags, "Present", 1).ToInt(-1);
                            switch (rand) {
                                case 0: {
                                        HealCharacter(setup.Defender, setup.DefenderMap, 80, setup.PacketStack);
                                    }
                                    break;
                                case 1:
                                case 2:
                                case 3: {
                                        if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                                            DamageCharacter(setup.Defender, setup.DefenderMap, 40 * rand, Enums.KillType.Player, setup.PacketStack, false);
                                        } else {
                                            DamageCharacter(setup.Defender, setup.DefenderMap, 40 * rand, Enums.KillType.Npc, setup.PacketStack, false);
                                        }
                                        setup.Damage = 40 * rand;
                                    }
                                    break;
                            }
                        }
                        break;
                    case 84: {//psych up
                            setup.Attacker.AttackBuff = setup.Defender.AttackBuff;
                            setup.Attacker.DefenseBuff = setup.Defender.DefenseBuff;
                            setup.Attacker.SpAtkBuff = setup.Defender.SpAtkBuff;
                            setup.Attacker.SpDefBuff = setup.Defender.SpDefBuff;
                            setup.Attacker.SpeedBuff = setup.Defender.SpeedBuff;
                            setup.Attacker.AccuracyBuff = setup.Defender.AccuracyBuff;
                            setup.Attacker.EvasionBuff = setup.Defender.EvasionBuff;
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " copied " + setup.Defender.Name + "'s stat changes!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                            setup.Cancel = true;
                        }
                        break;
                    case 87: {//Traps
                            bool trapPlaced = false;
                            if (setup.AttackerMap.Moral == Enums.MapMoral.None || setup.AttackerMap.Moral == Enums.MapMoral.NoPenalty) {
                                if (setup.AttackerMap.Tile[setup.Attacker.X, setup.Attacker.Y].Type == Enums.TileType.Scripted && GetTrapType(setup.AttackerMap.Tile[setup.Attacker.X, setup.Attacker.Y].Data1) != null) {
                                    //remove preexisting traps, unless it's a spikes or T-spikes trap, in which case it can stack
                                    if (setup.Move.Data2 == 17 && setup.AttackerMap.Tile[setup.Attacker.X, setup.Attacker.Y].Data1 == 17 && setup.AttackerMap.Tile[setup.Attacker.X, setup.Attacker.Y].String3.ToInt() < 3) {
                                        setup.AttackerMap.Tile[setup.Attacker.X, setup.Attacker.Y].String3 = (setup.AttackerMap.Tile[setup.Attacker.X, setup.Attacker.Y].String3.ToInt() + 1).ToString();
                                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " placed a trap!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                                        trapPlaced = true;
                                    } else if (setup.Move.Data2 == 18 && setup.AttackerMap.Tile[setup.Attacker.X, setup.Attacker.Y].Data1 == 18 && setup.AttackerMap.Tile[setup.Attacker.X, setup.Attacker.Y].String3.ToInt() < 2) {
                                        setup.AttackerMap.Tile[setup.Attacker.X, setup.Attacker.Y].String3 = (setup.AttackerMap.Tile[setup.Attacker.X, setup.Attacker.Y].String3.ToInt() + 1).ToString();
                                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " placed a trap!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                                        trapPlaced = true;
                                    } else {
                                        RemoveTrap(setup.AttackerMap, setup.Attacker.X, setup.Attacker.Y, setup.PacketStack);
                                    }
                                }

                                if (setup.AttackerMap.Tile[setup.Attacker.X, setup.Attacker.Y].Type == Enums.TileType.Walkable) {
                                    //set the trap on a walkable tile
                                    Tile tile = setup.AttackerMap.Tile[setup.Attacker.X, setup.Attacker.Y];
                                    tile.Type = Enums.TileType.Scripted;
                                    tile.Data1 = setup.Move.Data2;
                                    tile.Data2 = 0;
                                    tile.Data3 = 2;
                                    if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                                        tile.String1 = ((Recruit)setup.Attacker).Owner.Player.CharID;
                                        tile.String2 = ((Recruit)setup.Attacker).Owner.Player.GuildName;
                                    } else {
                                        tile.String1 = "";
                                        tile.String2 = "";
                                    }
                                    if (setup.Move.Data2 == 17 || setup.Move.Data2 == 18) tile.String3 = "1";
                                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " placed a trap!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                                    trapPlaced = true;
                                }
                            }

                            if (!trapPlaced) {
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("A trap can't be placed here!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                            }
                        }
                        break;
                    case 88: {//Teleport
                            if (setup.DefenderMap.MapType == Enums.MapType.RDungeonMap) {
                                if (GetBattleTagArg(setup.BattleTags, "Teleport", 1) != null) {
                                    //setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " warped!", Text.WhiteSmoke), setup.Defender.X, setup.Defender.Y, 10);
                                    PointWarp(setup.Defender, setup.DefenderMap, setup.PacketStack, GetBattleTagArg(setup.BattleTags, "Teleport", 1).ToInt(), GetBattleTagArg(setup.BattleTags, "Teleport", 2).ToInt());
                                } else {
                                    setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " didn't warp.", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                                }
                            } else {
                                setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " can only warp in mystery dungeons!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                            }
                        }
                        break;
                    case 89: {//Memento
                            ChangeAttackBuff(setup.Defender, setup.Attacker, setup.DefenderMap, -2, setup.PacketStack);
                            ChangeSpAtkBuff(setup.Defender, setup.Attacker, setup.DefenderMap, -2, setup.PacketStack);
                        }
                        break;
                    case 90: {//Perish Song
                            if (setup.Attacker == setup.Defender) {
                                AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "PerishCount", 4, null, "", setup.PacketStack, false);
                            } else {
                                AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "PerishCount", 3, null, "", setup.PacketStack, true);
                            }
                        }
                        break;
                    case 91: {//Future Sight
                            int damage = DamageCalculator.CalculateDamage(100, setup.Attacker.SpclAtk, setup.Attacker.Level, setup.Defender.SpclDef);
                            AddExtraStatus(setup.Defender, setup.DefenderMap, "FutureSight", 2, null, "", setup.PacketStack);
                        }
                        break;
                    case 92: {//Doom Desire
                            //AddExtraStatus(setup.Defender, setup.DefenderMap, "FutureSight", 2, null, setup.PacketStack);
                        }
                        break;
                    case 93: {//Focus Energy
                            AddExtraStatus(setup.Defender, setup.DefenderMap, "FocusEnergy", 3, null, "", setup.PacketStack);
                        }
                        break;
                    case 94: {//Ingrain
                            AddExtraStatus(setup.Defender, setup.DefenderMap, "Ingrain", Server.Math.Rand(5, 8), null, "", setup.PacketStack);
                        }
                        break;
                    case 95: {//Charge
                            AddExtraStatus(setup.Defender, setup.DefenderMap, "Charge", 2, null, "", setup.PacketStack);
                            ChangeSpDefBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 1, setup.PacketStack);
                        }
                        break;
                    case 96: {//Accupressure
                            switch (Server.Math.Rand(0, 7)) {
                                case 0: {
                                        ChangeAttackBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 2, setup.PacketStack);
                                    }
                                    break;
                                case 1: {
                                        ChangeDefenseBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 2, setup.PacketStack);
                                    }
                                    break;
                                case 2: {
                                        ChangeSpAtkBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 2, setup.PacketStack);
                                    }
                                    break;
                                case 3: {
                                        ChangeSpDefBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 2, setup.PacketStack);
                                    }
                                    break;
                                case 4: {
                                        ChangeSpeedBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 2, setup.PacketStack);
                                    }
                                    break;
                                case 5: {
                                        ChangeAccuracyBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 2, setup.PacketStack);
                                    }
                                    break;
                                case 6: {
                                        ChangeEvasionBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 2, setup.PacketStack);
                                    }
                                    break;
                            }
                        }
                        break;
                    case 97: {//Disable
                        status = setup.Defender.VolatileStatus.GetStatus("LastUsedMoveSlot");
                            if (status != null) {
                                //AddExtraStatus(i, "MoveSeal:" + rand, -1, 0, null, hitlist);
                                AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Disable", status.Counter, null, "", setup.PacketStack, true);
                            } else {
                                setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg("But it failed on " + setup.Defender.Name + "!", Text.WhiteSmoke), setup.Defender.X, setup.Defender.Y, 10);
                            }
                        }
                        break;
                    case 98: {//Yawn
                            AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Yawn", 2, null, "", setup.PacketStack, true);
                        }
                        break;
                    case 99: {//Foresight
                            AddExtraStatus(setup.Defender, setup.DefenderMap, "Exposed", 0, null, "", setup.PacketStack);
                        }
                        break;
                    case 100: {//Miracle Eye
                            AddExtraStatus(setup.Defender, setup.DefenderMap, "MiracleEye", 0, null, "", setup.PacketStack);
                        }
                        break;
                    case 101: {//Protect/Detect
                            AddExtraStatus(setup.Defender, setup.DefenderMap, "Protect", setup.Move.Data2, null, "", setup.PacketStack);
                        }
                        break;
                    case 102: {//Endure
                            AddExtraStatus(setup.Defender, setup.DefenderMap, "Endure", setup.Move.Data2, null, "", setup.PacketStack);
                        }
                        break;
                    case 103: {//Haze
                            RemoveBuffs(setup.Defender);
                            setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + "'s stat changes were removed!", Text.WhiteSmoke), setup.Defender.X, setup.Defender.Y, 10);
                        }
                        break;
                    case 104: {//Magnet Rise
                            AddExtraStatus(setup.Defender, setup.DefenderMap, "MagnetRise", 0, null, "", setup.PacketStack);
                        }
                        break;
                    case 105: {//Mirror Coat
                            AddExtraStatus(setup.Defender, setup.DefenderMap, "MirrorCoat", 2, null, "", setup.PacketStack);
                        }
                        break;
                    case 106: {//Counter
                            AddExtraStatus(setup.Defender, setup.DefenderMap, "Counter", 2, null, "", setup.PacketStack);
                        }
                        break;
                    case 107: {//Belly Drum
                            if (setup.Defender.HP > setup.Defender.MaxHP / 2) {
                                DamageCharacter(setup.Defender, setup.DefenderMap, setup.Defender.MaxHP / 2, Enums.KillType.Other, setup.PacketStack, true);
                                ChangeAttackBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 10, setup.PacketStack);
                            } else {
                                setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg("But it failed!", Text.WhiteSmoke), setup.Defender.X, setup.Defender.Y, 10);
                            }
                        }
                        break;
                    case 108: {//Safeguard
                    		int duration = setup.Move.Data2;
                        	if (setup.Attacker.HasActiveItem(84)) {
                        	    duration *= 2;
                        	}
                            AddExtraStatus(setup.Defender, setup.DefenderMap, "Safeguard", duration, null, "", setup.PacketStack);
                        }
                        break;
                    case 109: {//Mist
                    		int duration = setup.Move.Data2;
                        	if (setup.Attacker.HasActiveItem(84)) {
                        	    duration *= 2;
                        	}
                            AddExtraStatus(setup.Defender, setup.DefenderMap, "Mist", duration, null, "", setup.PacketStack);
                        }
                        break;
                    case 110: {//Reflect
                        	int duration = setup.Move.Data2;
                        	if (setup.Attacker.HasActiveItem(84)) {
                        	    duration *= 2;
                        	}
                        	AddExtraStatus(setup.Defender, setup.DefenderMap, "Reflect", duration, null, "", setup.PacketStack);
                        }
                        break;
                    case 111: {//Light Screen
                    		int duration = setup.Move.Data2;
                            if (setup.Attacker.HasActiveItem(84)) {
                                duration *= 2;
                            }
                            AddExtraStatus(setup.Defender, setup.DefenderMap, "LightScreen", duration, null, "", setup.PacketStack);
                        }
                        break;
                    case 114: {//Rage
                            AddExtraStatus(setup.Defender, setup.DefenderMap, "Rage", 10, null, "", setup.PacketStack);
                        }
                        break;
                    case 115: {//Taunt
                            AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Taunt", 10, null, "", setup.PacketStack, true);
                        }
                        break;
                    case 116: {//Encore
                            if (setup.Defender.VolatileStatus.GetStatus("LastUsedMoveSlot") != null) {
                                AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Encore", 5, null, "", setup.PacketStack, true);
                            } else {
                                setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg("But it failed on " + setup.Defender.Name + "!", Text.WhiteSmoke), setup.Defender.X, setup.Defender.Y, 10);
                            }
                        }
                        break;
                    case 117: {//Torment
                            AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Torment", 0, null, "", setup.PacketStack, true);
                        }
                        break;
                    case 118: {//GRUDGE
                            AddExtraStatus(setup.Defender, setup.DefenderMap, "Grudge", 0, null, "", setup.PacketStack);
                        }
                        break;
                    case 119: {//Lucky Chant
                            AddExtraStatus(setup.Defender, setup.DefenderMap, "LuckyChant", 5, null, "", setup.PacketStack);
                        }
                        break;
                    case 120: {//Spite (deduct PP)
                        status = setup.Defender.VolatileStatus.GetStatus("LastUsedMoveSlot");
                            if (status != null) {

                                setup.Defender.Moves[status.Counter].CurrentPP -= setup.Move.Data2;
                                if (setup.Defender.Moves[status.Counter].CurrentPP < 0) {
                                    setup.Defender.Moves[status.Counter].CurrentPP = 0;
                                }
                                if (setup.Defender.CharacterType == Enums.CharacterType.Recruit) {
                                    PacketBuilder.AppendPlayerMoves(((Recruit)setup.Defender).Owner, setup.PacketStack);
                                }
                                setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg("The last move used by " + setup.Defender.Name + " lost some PP!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                            } else {
                                setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg("But it failed on " + setup.Defender.Name + "!", Text.WhiteSmoke), setup.Defender.X, setup.Defender.Y, 10);
                            }
                        }
                        break;
                    case 121: {//Aqua Ring
                            AddExtraStatus(setup.Defender, setup.DefenderMap, "AquaRing", 0, null, "", setup.PacketStack);
                        }
                        break;
                    case 122: {//Wish
                            AddExtraStatus(setup.Defender, setup.DefenderMap, "Wish", 2, null, "", setup.PacketStack);
                        }
                        break;
                    case 123: {//Embargo
                            AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Embargo", 10, null, "", setup.PacketStack, true);
                        }
                        break;
                    case 124: {//Imprison
                            int rand = Server.Math.Rand(0, 4);
                            AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "MoveSeal:" + rand, 0, null, "", setup.PacketStack, true);
                            //for (int i = 0; i < Constants.MAX_PLAYER_MOVES; i++) {
                            //	for (int i = 0; j < Constants.MAX_PLAYER_MOVES; j++) {

                            //}
                        }
                        break;
                    case 125: {//phazer
                            BlowBack(setup.Defender, setup.DefenderMap, setup.Attacker.Direction, setup.PacketStack);
                        }
                        break;
                    case 127: {//Destiny Bond
                            AddExtraStatus(setup.Defender, setup.DefenderMap, "DestinyBond", 3, null, "", setup.PacketStack);
                        }
                        break;
                    case 129: {//psycho shift
                            int statusAilment = GetBattleTagArg(setup.BattleTags, "PsychoShift", 1).ToInt(-1);
                            int statusCounter = GetBattleTagArg(setup.BattleTags, "PsychoShift", 2).ToInt(-1);

                            if (statusAilment < 1) {

                            } else {
                                setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s status problem was shifted on to " + setup.Defender.Name + "!", Text.WhiteSmoke), setup.Defender.X, setup.Defender.Y, 10);
                                SetStatusAilment(setup.Defender, setup.Attacker, setup.DefenderMap, (Enums.StatusAilment)statusAilment, statusCounter, setup.PacketStack);
                            }
                        }
                        break;
                    case 130: {//Heal Block
                            AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "HealBlock", 10, null, "", setup.PacketStack, true);
                        }
                        break;
                    case 131: {//Magic Coat
                            AddExtraStatus(setup.Defender, setup.DefenderMap, "MagicCoat", 5, null, "", setup.PacketStack);
                        }
                        break;
                    case 134: {//Metal Burst
                            AddExtraStatus(setup.Defender, setup.DefenderMap, "MetalBurst", 1, null, "", setup.PacketStack);
                        }
                        break;
                    case 137: {//Neutralized move effects
                    		if (HasActiveBagItem(setup.Defender, 2, 0, (int)setup.Move.Element)) {
		                    	HealCharacter(setup.Defender, setup.DefenderMap, setup.Defender.MaxHP / 4, setup.PacketStack);
		                    }
                    		
                            if (HasAbility(setup.Defender, "Dry Skin")) {
                                if (setup.Move.Element == Enums.PokemonType.Water) {
                                    HealCharacter(setup.Defender, setup.DefenderMap, setup.Defender.MaxHP / 4, setup.PacketStack);
                                }
                            }
                            if (HasAbility(setup.Defender, "Flash Fire")) {
                                if (setup.Move.Element == Enums.PokemonType.Fire) {
                                    AddExtraStatus(setup.Defender, setup.DefenderMap, "FlashFire", 0, null, "", setup.PacketStack);
                                }
                            }
                            if (HasAbility(setup.Defender, "Lightningrod")) {
                                if (setup.Move.Element == Enums.PokemonType.Electric) {
                                    ChangeSpAtkBuff(setup.Defender, setup.DefenderMap, 1, setup.PacketStack);
                                }
                            }
                            if (HasAbility(setup.Defender, "Motor Drive")) {
                                if (setup.Move.Element == Enums.PokemonType.Electric) {
                                    ChangeSpeedBuff(setup.Defender, setup.DefenderMap, 1, setup.PacketStack);
                                }
                            }
                            if (HasAbility(setup.Defender, "Sap Sipper")) {
                                if (setup.Move.Element == Enums.PokemonType.Grass) {
                                    ChangeAttackBuff(setup.Defender, setup.DefenderMap, 1, setup.PacketStack);
                                }
                            }
                            if (HasAbility(setup.Defender, "Storm Drain")) {
                                if (setup.Move.Element == Enums.PokemonType.Water) {
                                    ChangeSpAtkBuff(setup.Defender, setup.DefenderMap, 1, setup.PacketStack);
                                }
                            }
                            if (HasAbility(setup.Defender, "Volt Absorb")) {
                                if (setup.Move.Element == Enums.PokemonType.Electric) {
                                    HealCharacter(setup.Defender, setup.DefenderMap, setup.Defender.MaxHP / 4, setup.PacketStack);
                                }
                            }
                            if (HasAbility(setup.Defender, "Water Absorb")) {
                                if (setup.Move.Element == Enums.PokemonType.Water) {
                                    HealCharacter(setup.Defender, setup.DefenderMap, setup.Defender.MaxHP / 4, setup.PacketStack);
                                }
                            }
                        }
                        break;
                    case 138: {//Wide Guard
                            AddExtraStatus(setup.Defender, setup.DefenderMap, "WideGuard", setup.Move.Data2, null, "", setup.PacketStack);
                        }
                        break;
                    case 139: {//Quiver Dance
                            ChangeSpAtkBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 1, setup.PacketStack);
                            ChangeSpDefBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 1, setup.PacketStack);
                            ChangeSpeedBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 1, setup.PacketStack);
                        }
                        break;
                    case 140: {//Coil
                            ChangeAttackBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 1, setup.PacketStack);
                            ChangeDefenseBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 1, setup.PacketStack);
                            ChangeAccuracyBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 1, setup.PacketStack);
                        }
                        break;
                    case 141: {//Shell Smash
                            ChangeAttackBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 2, setup.PacketStack);
                            ChangeDefenseBuff(setup.Defender, setup.Attacker, setup.DefenderMap, -1, setup.PacketStack);
                            ChangeSpAtkBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 2, setup.PacketStack);
                            ChangeSpDefBuff(setup.Defender, setup.Attacker, setup.DefenderMap, -1, setup.PacketStack);
                            ChangeSpeedBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 2, setup.PacketStack);
                        }
                        break;
                    case 144: {//Autotomize
                            AddExtraStatus(setup.Defender, setup.DefenderMap, "Autotomize", 0, null, "", setup.PacketStack);
                            ChangeSpeedBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 2, setup.PacketStack);
                        }
                        break;
                    case 145: {//Shift Gear
                            ChangeAttackBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 1, setup.PacketStack);
                            ChangeSpeedBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 2, setup.PacketStack);
                        }
                        break;
                    case 147: {//swagger/flatter
                            
                            //Confuse(setup.Defender, setup.Attacker, setup.DefenderMap, Server.Math.Rand(5, 11), setup.PacketStack);
                            AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Confusion", Server.Math.Rand(5, 11), null, "", setup.PacketStack, true);
                            switch (setup.Move.Data2) {
                                case 1: {
                                        ChangeAttackBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 2, setup.PacketStack);
                                    }
                                    break;
                                case 2: {
                                        ChangeDefenseBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 2, setup.PacketStack);
                                    }
                                    break;
                                case 3: {
                                        ChangeSpAtkBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 2, setup.PacketStack);
                                    }
                                    break;
                                case 4: {
                                        ChangeSpDefBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 2, setup.PacketStack);
                                    }
                                    break;
                                case 5: {
                                        ChangeSpeedBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 2, setup.PacketStack);
                                    }
                                    break;
                                case 6: {
                                        ChangeAccuracyBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 2, setup.PacketStack);
                                    }
                                    break;
                                case 7: {
                                        ChangeEvasionBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 2, setup.PacketStack);
                                    }
                                    break;
                            }
                        }
                        break;
                    case 148: {//Captivate
                            if (setup.Attacker.Sex == Enums.Sex.Genderless && setup.Defender.Sex == Enums.Sex.Genderless
                            || setup.Attacker.Sex == Enums.Sex.Male && setup.Defender.Sex == Enums.Sex.Female
                            || setup.Defender.Sex == Enums.Sex.Male && setup.Attacker.Sex == Enums.Sex.Female) {
                                if (HasAbility(setup.Defender, "Oblivious")) {
                                    setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " wasn't affected due to Oblivious!", Text.WhiteSmoke), setup.Defender.X, setup.Defender.Y, 10);
                                } else {
                                    ChangeSpAtkBuff(setup.Defender, setup.Attacker, setup.DefenderMap, -2, setup.PacketStack);
                                }
                            } else {
                                setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " wasn't affected!", Text.WhiteSmoke), setup.Defender.X, setup.Defender.Y, 10);
                            }
                        }
                        break;
                    case 149: {//Attract
                            if (setup.Attacker.Sex == Enums.Sex.Genderless && setup.Defender.Sex == Enums.Sex.Genderless
                            || setup.Attacker.Sex == Enums.Sex.Male && setup.Defender.Sex == Enums.Sex.Female
                            || setup.Defender.Sex == Enums.Sex.Male && setup.Attacker.Sex == Enums.Sex.Female) {
                                AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Attract", Server.Math.Rand(4, 6), null, "", setup.PacketStack, true);
                            } else {
                                setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " wasn't affected!", Text.WhiteSmoke), setup.Defender.X, setup.Defender.Y, 10);
                            }
                        }
                        break;
                    case 150: {//Follow
                            AddExtraStatus(setup.Defender, setup.DefenderMap, "Follow", 3, setup.Attacker, "", setup.PacketStack);
                        }
                        break;
                    case 152: {//Set Type
                            AddExtraStatus(setup.Defender, setup.DefenderMap, "Type1", setup.Move.Data2, null, "", setup.PacketStack);
                            AddExtraStatus(setup.Defender, setup.DefenderMap, "Type2", setup.Move.Data3, null, "", setup.PacketStack);
                        }
                        break;
                    case 153: {//Nullify Ability
                            AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "GastroAcid", setup.Move.Data2, null, "", setup.PacketStack, true);
                        }
                        break;
                    case 155: {//reflect Type
                            if (GetBattleTagArg(setup.BattleTags, "ReflectType", 1) != null) {
                                string typeParams = GetBattleTagArg(setup.BattleTags, "ReflectType", 1);
                                typeParams += "," + (int)setup.Defender.Type1 + "," + (int)setup.Defender.Type2;
                                RemoveBattleTag(setup.BattleTags, "ReflectType");
                                setup.BattleTags.Add("ReflectType:" + typeParams);
                            } else {
                                setup.BattleTags.Add("ReflectType:" + (int)setup.Defender.Type1 + "," + (int)setup.Defender.Type2);
                            }
                        }
                        break;
                    case 158: {//lock on/mind reader
                    		if (setup.Defender == setup.Attacker) {
                            	AddExtraStatus(setup.Defender, setup.DefenderMap, "SureShot", setup.Move.Data2 + 1, null, "", setup.PacketStack);
                            } else {
                            	AddExtraStatus(setup.Defender, setup.DefenderMap, "SureShot", setup.Move.Data2, null, "", setup.PacketStack);
                            }
                        }
                        break;
                    case 159: {//ally switch
                            if (setup.DefenderMap.MapType == Enums.MapType.RDungeonMap) {
                                if (GetBattleTagArg(setup.BattleTags, "AllySwitchCount", 1) != null) {
                                    int count = GetBattleTagArg(setup.BattleTags, "AllySwitchCount", 1).ToInt();
                                    if (GetBattleTagArg(setup.BattleTags, "AllySwitch", count) != null) {
                                        setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " warped!", Text.WhiteSmoke), setup.Defender.X, setup.Defender.Y, 10);
                                        PointWarp(setup.Defender, setup.DefenderMap, setup.PacketStack, GetBattleTagArg(setup.BattleTags, "AllySwitch", count).ToInt(), GetBattleTagArg(setup.BattleTags, "AllySwitch", count + 1).ToInt());
                                    }
                                    RemoveBattleTag(setup.BattleTags, "AllySwitchCount");
                                    setup.BattleTags.Add("AllySwitchCount:" + (count + 2));
                                } else {
                                    setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " didn't warp.", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                                }
                            } else {
                                setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " can only warp in mystery dungeons!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                            }
                        }
                        break;
                    case 161: {//movement speed
                        status = setup.Defender.VolatileStatus.GetStatus("MovementSpeed");
                            if (status != null) {
                                AddExtraStatus(setup.Defender, setup.DefenderMap, "MovementSpeed", status.Counter + setup.Move.Data2, null, "", setup.PacketStack);
                            } else {
                                AddExtraStatus(setup.Defender, setup.DefenderMap, "MovementSpeed", setup.Move.Data2, null, "", setup.PacketStack);
                            }
                        }
                        break;
                    case 162: {//rollcall
                            if (setup.DefenderMap.MapType == Enums.MapType.RDungeonMap) {
                                if (GetBattleTagArg(setup.BattleTags, "Rollcall", 1) != null) {
                                    //setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " warped!", Text.WhiteSmoke), setup.Defender.X, setup.Defender.Y, 10);
                                    RandomWarp(setup.Defender, setup.DefenderMap, true, setup.PacketStack, GetBattleTagArg(setup.BattleTags, "Rollcall", 1).ToInt() - 1, GetBattleTagArg(setup.BattleTags, "Rollcall", 1).ToInt() + 1, GetBattleTagArg(setup.BattleTags, "Rollcall", 2).ToInt() - 1, GetBattleTagArg(setup.BattleTags, "Rollcall", 2).ToInt() + 1);
                                } else {
                                    setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " didn't warp.", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                                }
                            } else {
                                setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " can only warp in mystery dungeons!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                            }
                        }
                        break;
                    case 164: {//Quick Guard
                            AddExtraStatus(setup.Defender, setup.DefenderMap, "QuickGuard", setup.Move.Data2, null, "", setup.PacketStack);
                        }
                        break;
                    case 166: {//Stat Swap

                            switch (setup.Move.Data2) {
                                case 1: {//power Swap
                                        if (GetBattleTagArg(setup.BattleTags, "PowerSwap", 1) != null) {
                                            int atk = setup.Defender.AttackBuff;
                                            int spAtk = setup.Defender.SpAtkBuff;
                                            setup.Defender.AttackBuff = GetBattleTagArg(setup.BattleTags, "PowerSwap", 2).ToInt();
                                            setup.Defender.SpAtkBuff = GetBattleTagArg(setup.BattleTags, "PowerSwap", 3).ToInt();
                                            setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " received " + GetBattleTagArg(setup.BattleTags, "PowerSwap", 1) + "'s changes to Attack and Special Attack!", Text.WhiteSmoke), setup.Defender.X, setup.Defender.Y, 10);
                                            RemoveBattleTag(setup.BattleTags, "PowerSwap");
                                            setup.BattleTags.Add("PowerSwap:" + setup.Defender.Name + ":" + atk + ":" + spAtk);

                                        } else {

                                            setup.BattleTags.Add("PowerSwap:" + setup.Defender.Name + ":" + setup.Defender.AttackBuff + ":" + setup.Defender.SpAtkBuff);
                                            setup.Defender.AttackBuff = setup.Attacker.AttackBuff;
                                            setup.Defender.SpAtkBuff = setup.Attacker.SpAtkBuff;
                                            setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " received " + setup.Attacker.Name + "'s changes to Attack and Special Attack!", Text.WhiteSmoke), setup.Defender.X, setup.Defender.Y, 10);
                                        }
                                    }
                                    break;
                                case 2: {//guard swap
                                        if (GetBattleTagArg(setup.BattleTags, "GuardSwap", 1) != null) {
                                            int def = setup.Defender.DefenseBuff;
                                            int spDef = setup.Defender.SpDefBuff;
                                            setup.Defender.DefenseBuff = GetBattleTagArg(setup.BattleTags, "GuardSwap", 2).ToInt();
                                            setup.Defender.SpDefBuff = GetBattleTagArg(setup.BattleTags, "GuardSwap", 3).ToInt();
                                            setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " received " + GetBattleTagArg(setup.BattleTags, "GuardSwap", 1) + "'s changes to Defense and Special Defense!", Text.WhiteSmoke), setup.Defender.X, setup.Defender.Y, 10);
                                            RemoveBattleTag(setup.BattleTags, "GuardSwap");
                                            setup.BattleTags.Add("GuardSwap:" + setup.Defender.Name + ":" + def + ":" + spDef);

                                        } else {

                                            setup.BattleTags.Add("GuardSwap:" + setup.Defender.Name + ":" + setup.Defender.DefenseBuff + ":" + setup.Defender.SpDefBuff);
                                            setup.Defender.DefenseBuff = setup.Attacker.DefenseBuff;
                                            setup.Defender.SpDefBuff = setup.Attacker.SpDefBuff;
                                            setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " received " + setup.Attacker.Name + "'s changes to Defense and Special Defense!", Text.WhiteSmoke), setup.Defender.X, setup.Defender.Y, 10);
                                        }
                                    }
                                    break;
                                case 3: {//heart Swap
                                        if (GetBattleTagArg(setup.BattleTags, "HeartSwap", 1) != null) {
                                            int atk = setup.Defender.AttackBuff;
                                            int spAtk = setup.Defender.SpAtkBuff;
                                            int def = setup.Defender.DefenseBuff;
                                            int spDef = setup.Defender.SpDefBuff;
                                            int speed = setup.Defender.SpeedBuff;
                                            int acc = setup.Defender.AccuracyBuff;
                                            int eva = setup.Defender.EvasionBuff;
                                            setup.Defender.AttackBuff = GetBattleTagArg(setup.BattleTags, "HeartSwap", 2).ToInt();
                                            setup.Defender.SpAtkBuff = GetBattleTagArg(setup.BattleTags, "HeartSwap", 3).ToInt();
                                            setup.Defender.DefenseBuff = GetBattleTagArg(setup.BattleTags, "HeartSwap", 4).ToInt();
                                            setup.Defender.SpDefBuff = GetBattleTagArg(setup.BattleTags, "HeartSwap", 5).ToInt();
                                            setup.Defender.SpeedBuff = GetBattleTagArg(setup.BattleTags, "HeartSwap", 6).ToInt();
                                            setup.Defender.AccuracyBuff = GetBattleTagArg(setup.BattleTags, "HeartSwap", 7).ToInt();
                                            setup.Defender.EvasionBuff = GetBattleTagArg(setup.BattleTags, "HeartSwap", 8).ToInt();
                                            setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " received " + GetBattleTagArg(setup.BattleTags, "HeartSwap", 1) + "'s stat changes!", Text.WhiteSmoke), setup.Defender.X, setup.Defender.Y, 10);
                                            RemoveBattleTag(setup.BattleTags, "HeartSwap");
                                            setup.BattleTags.Add("PowerSwap:" + setup.Defender.Name + ":" + atk + ":" + spAtk + ":" + def + ":" + spDef + ":" + speed + ":" + acc + ":" + eva);

                                        } else {

                                            setup.BattleTags.Add("HeartSwap:" + setup.Defender.Name + ":" + setup.Defender.AttackBuff + ":" + setup.Defender.SpAtkBuff + ":" +
                                            	setup.Defender.DefenseBuff + ":" + setup.Defender.SpDefBuff + ":" + setup.Defender.SpeedBuff + ":" + setup.Defender.AccuracyBuff + ":" + setup.Defender.EvasionBuff);
                                            setup.Defender.AttackBuff = setup.Attacker.AttackBuff;
                                            setup.Defender.SpAtkBuff = setup.Attacker.SpAtkBuff;
                                            setup.Defender.DefenseBuff = setup.Attacker.DefenseBuff;
                                            setup.Defender.SpDefBuff = setup.Attacker.SpDefBuff;
                                            setup.Defender.SpeedBuff = setup.Attacker.SpeedBuff;
                                            setup.Defender.AccuracyBuff = setup.Attacker.AccuracyBuff;
                                            setup.Defender.EvasionBuff = setup.Attacker.EvasionBuff;
                                            setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " received " + setup.Attacker.Name + "'s stat changes!", Text.WhiteSmoke), setup.Defender.X, setup.Defender.Y, 10);
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    case 167: {//substitute
                            if (setup.Defender.HP > setup.Defender.MaxHP / 4) {
                                if (setup.Defender.VolatileStatus.GetStatus("Substitute") == null) {
                                    setup.Defender.HP -= setup.Defender.MaxHP / 4;
                                    AddExtraStatus(setup.Defender, setup.DefenderMap, "Substitute", setup.Defender.MaxHP / 4, null, "", setup.PacketStack);
                                } else {
                                    setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " already has a substitute up!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                                }
                            } else {
                                setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " was too weak to make a substitute!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                            }
                        }
                        break;
                    case 168: {//Telekinesis
                            AddExtraStatus(setup.Defender, setup.DefenderMap, "Telekinesis", setup.Move.Data2, null, "", setup.PacketStack);
                        }
                        break;
                    case 170: {// Final Gambit
                            int HPDmg = 0;
                            if (setup.Attacker.HP > 0) {
                                HPDmg = setup.Attacker.HP - 1;
                            }
                            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                                DamageCharacter(setup.Defender, setup.DefenderMap, HPDmg, Enums.KillType.Player, setup.PacketStack, false);
                            } else {
                                DamageCharacter(setup.Defender, setup.DefenderMap, HPDmg, Enums.KillType.Npc, setup.PacketStack, false);
                            }
                            setup.Damage = HPDmg;
                        }
                        break;
                    case 173: {//Nightmare
                            AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Nightmare", 0, null, "", setup.PacketStack, true);
                        }
                        break;
                    case 174: {//Trick Room
                            if (setup.AttackerMap.Tile[setup.Attacker.X, setup.Attacker.Y].Type == Enums.TileType.Arena || setup.AttackerMap.Moral == Enums.MapMoral.None || setup.AttackerMap.Moral == Enums.MapMoral.NoPenalty) {
                                if (setup.AttackerMap.TempStatus.GetStatus("TrickRoom") != null) {
                                    RemoveMapStatus(setup.AttackerMap, "TrickRoom", setup.PacketStack);
                                } else {
                                    AddMapStatus(setup.AttackerMap, "TrickRoom", setup.Move.Data2, "", -1, setup.PacketStack);
                                }
                            } else {
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("The effect of Trick Room only works in dungeons and arenas!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                            }

                        }
                        break;
                    case 175: {//Simple Beam
                            AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Ability1", 0, null, "Simple", setup.PacketStack, true);
                            AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Ability2", 0, null, "None", setup.PacketStack, true);
                            AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Ability3", 0, null, "None", setup.PacketStack, true);
                        }
                        break;
                    case 176: {//Entrainment
                            AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Ability1", 0, null, setup.Attacker.Ability1, setup.PacketStack, true);
                            AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Ability2", 0, null, setup.Attacker.Ability2, setup.PacketStack, true);
                            AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Ability3", 0, null, setup.Attacker.Ability3, setup.PacketStack, true);
                        }
                        break;
                    case 177: {//Simple Beam
                            AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Ability1", 0, null, "Insomnia", setup.PacketStack, true);
                            AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Ability2", 0, null, "None", setup.PacketStack, true);
                            AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Ability3", 0, null, "None", setup.PacketStack, true);
                        }
                        break;
                    case 178: {//role play
                            if (GetBattleTagArg(setup.BattleTags, "RolePlay", 1) != null) {
                                string typeParams = GetBattleTagArg(setup.BattleTags, "RolePlay", 1);
                                typeParams += "," + setup.Defender.Ability1 + "," + setup.Defender.Ability2 + "," + setup.Defender.Ability3;
                                RemoveBattleTag(setup.BattleTags, "RolePlay");
                                setup.BattleTags.Add("RolePlay:" + typeParams);
                            } else {
                                setup.BattleTags.Add("RolePlay:" + setup.Defender.Ability1 + "," + setup.Defender.Ability2 + "," + setup.Defender.Ability3);
                            }
                        }
                        break;
                    case 179: {//skill swap
                            if (GetBattleTagArg(setup.BattleTags, "SkillSwap", 1) != null) {
                                string ability1 = setup.Defender.Ability1;
                                string ability2 = setup.Defender.Ability2;
                                string ability3 = setup.Defender.Ability3;

                                AddExtraStatus(setup.Defender, setup.DefenderMap, "Ability1", 0, null, GetBattleTagArg(setup.BattleTags, "SkillSwap", 1), setup.PacketStack);
                                AddExtraStatus(setup.Defender, setup.DefenderMap, "Ability2", 0, null, GetBattleTagArg(setup.BattleTags, "SkillSwap", 2), setup.PacketStack);
                                AddExtraStatus(setup.Defender, setup.DefenderMap, "Ability3", 0, null, GetBattleTagArg(setup.BattleTags, "SkillSwap", 3), setup.PacketStack);

                                RemoveBattleTag(setup.BattleTags, "SkillSwap");
                                setup.BattleTags.Add("SkillSwap:" + ability1 + ":" + ability2 + ":" + ability3);

                            } else {

                                setup.BattleTags.Add("SkillSwap:" + setup.Defender.Ability1 + ":" + setup.Defender.Ability2 + ":" + setup.Defender.Ability3);
                                AddExtraStatus(setup.Defender, setup.DefenderMap, "Ability1", 0, null, setup.Attacker.Ability1, setup.PacketStack);
                                AddExtraStatus(setup.Defender, setup.DefenderMap, "Ability2", 0, null, setup.Attacker.Ability2, setup.PacketStack);
                                AddExtraStatus(setup.Defender, setup.DefenderMap, "Ability3", 0, null, setup.Attacker.Ability3, setup.PacketStack);
                            }
                        }
                        break;
                    case 182: {//snatch
                            AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Snatch", setup.Move.Data2, null, "", setup.PacketStack, true);
                        }
                        break;
                    case 183: {//Wonder Room
                            if (setup.AttackerMap.Tile[setup.Attacker.X, setup.Attacker.Y].Type == Enums.TileType.Arena || setup.AttackerMap.Moral == Enums.MapMoral.None || setup.AttackerMap.Moral == Enums.MapMoral.NoPenalty) {
                                if (setup.AttackerMap.TempStatus.GetStatus("WonderRoom") != null) {
                                    RemoveMapStatus(setup.AttackerMap, "WonderRoom", setup.PacketStack);
                                } else {
                                    AddMapStatus(setup.AttackerMap, "WonderRoom", setup.Move.Data2, "", -1, setup.PacketStack);
                                }
                            } else {
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("The effect of Wonder Room only works in dungeons and arenas!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                            }

                        }
                        break;
                    case 184: {//Magic Room
                            if (setup.AttackerMap.Tile[setup.Attacker.X, setup.Attacker.Y].Type == Enums.TileType.Arena || setup.AttackerMap.Moral == Enums.MapMoral.None || setup.AttackerMap.Moral == Enums.MapMoral.NoPenalty) {
                                if (setup.AttackerMap.TempStatus.GetStatus("MagicRoom") != null) {
                                    RemoveMapStatus(setup.AttackerMap, "MagicRoom", setup.PacketStack);
                                } else {
                                    AddMapStatus(setup.AttackerMap, "MagicRoom", setup.Move.Data2, "", -1, setup.PacketStack);
                                }
                            } else {
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("The effect of Magic Room only works in dungeons and arenas!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                            }

                        }
                        break;
                    case 185: {//Gravity
                            if (setup.AttackerMap.Tile[setup.Attacker.X, setup.Attacker.Y].Type == Enums.TileType.Arena || setup.AttackerMap.Moral == Enums.MapMoral.None || setup.AttackerMap.Moral == Enums.MapMoral.NoPenalty) {
                                if (setup.AttackerMap.TempStatus.GetStatus("Gravity") != null) {
                                    
                                } else {
                                    AddMapStatus(setup.AttackerMap, "Gravity", setup.Move.Data2, "", -1, setup.PacketStack);
                                }
                            } else {
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("The effect of Gravity only works in dungeons and arenas!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                            }

                        }
                        break;
                    case 188: {//foe-seal
                            int rand = Server.Math.Rand(0, 4);
                            AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "MoveSeal:" + rand, 0, null, "", setup.PacketStack, true);
                            int rand2 = Server.Math.Rand(0, 3);
                            if (rand2 == rand) rand = 3;
                            AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "MoveSeal:" + rand2, 0, null, "", setup.PacketStack, true);
                            //for (int i = 0; i < Constants.MAX_PLAYER_MOVES; i++) {
                            //	for (int i = 0; j < Constants.MAX_PLAYER_MOVES; j++) {

                            //}
                        }
                        break;
                    case 189: {//baton pass
                            setup.Defender.AttackBuff = setup.Attacker.AttackBuff;
				            setup.Defender.DefenseBuff = setup.Attacker.DefenseBuff;
				            setup.Defender.SpeedBuff = setup.Attacker.SpeedBuff;
				            setup.Defender.SpAtkBuff = setup.Attacker.SpAtkBuff;
				            setup.Defender.SpDefBuff = setup.Attacker.SpDefBuff;
				            setup.Defender.AccuracyBuff = setup.Attacker.AccuracyBuff;
				            setup.Defender.EvasionBuff = setup.Attacker.EvasionBuff;
				            
				            RemoveAllBondedExtraStatus(setup.Defender, setup.DefenderMap, setup.PacketStack, false);
				            
				            List<ExtraStatus> passedStatus = GetTeamStatus(setup.Defender);
				            
				            setup.Defender.VolatileStatus.Clear();
				            
				            foreach (ExtraStatus extraStatus in passedStatus) {
				                setup.Defender.VolatileStatus.Add(extraStatus);
				            }

                            for (int i = 0; i < setup.Attacker.VolatileStatus.Count; i++ ) {
                                if (!IsBondedStatus(setup.Attacker.VolatileStatus[i].Name) && !IsTeamStatus(setup.Attacker.VolatileStatus[i].Name)) {
                                    setup.Defender.VolatileStatus.Add(setup.Attacker.VolatileStatus[i]);
                                }
                            }
				            
				            RefreshCharacterTraits(setup.Defender, setup.DefenderMap, setup.PacketStack);
				            
				            setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " passed its stat changes and special effects to " + setup.Defender.Name + "!", Text.WhiteSmoke), setup.Defender.X, setup.Defender.Y, 10);
                        }
                        break;
                    case 190: {//move-invoking moves
                    		if (GetBattleTagArg(setup.BattleTags, "InvokedMove", 0) == null) {
								int moveIndex = -1;
				                	
				                switch (setup.Move.Data2) {
				                   	case 1: {//mirror move/copycat
                                        status = setup.Attacker.VolatileStatus.GetStatus("LastMoveHitBy");
                                        
				                    		if (status != null) {
				                    			moveIndex = status.Counter;
				                    		}
				                   		}
				                   		break;
				                   	case 2: {//assist - calls a move from the party
				                    		if (GetBattleTagArg(setup.BattleTags, "Assist", 1) != null) {
				                    			//Messenger.AdminMsg(GetBattleTagArg(setup.BattleTags, "Assist", 1), Text.Black);
				                    			string[] possibleMoves = GetBattleTagArg(setup.BattleTags, "Assist", 1).Split(',');
				                    			moveIndex = possibleMoves[Server.Math.Rand(0, possibleMoves.Length)].ToInt();
				                    		}
				                   		}
				                   		break;
				                   	case 3: {//me first - calls a move from the enemies
				                    		if (GetBattleTagArg(setup.BattleTags, "MeFirst", 1) != null) {
				                    			//Messenger.AdminMsg(GetBattleTagArg(setup.BattleTags, "Assist", 1), Text.Black);
				                    			string[] possibleMoves = GetBattleTagArg(setup.BattleTags, "MeFirst", 1).Split(',');
				                    			moveIndex = possibleMoves[Server.Math.Rand(0, possibleMoves.Length)].ToInt();
				                    		}
				                   		}
				                   		break;
				                   	case 4: {//nature power
				                    		Tile tile = setup.AttackerMap.Tile[setup.Attacker.X, setup.Attacker.Y];

			                                int type1 = 0;
			                                int type2 = 0;
			                                if (tile.Data2 > 0) {
			                                    if (tile.Type == Enums.TileType.Walkable || tile.Type == Enums.TileType.Hallway) {
			                                        //set to first type
			                                        type1 = tile.Data2;
			                                    }
			                                } else if (tile.Type == Enums.TileType.MobileBlock) {
			                                    Enums.PokemonType terrainType = GetMobileBlockTerrainType(tile.Data1);
			                                    if (terrainType != Enums.PokemonType.None) {
			                                        type1 = (int)terrainType;
			                                    }
			                                }
			
			                                if (tile.String2.IsNumeric() && tile.String2.ToInt() > 0) {
			                                    //set to second type
			                                    if (type1 != 0) {
			                                        type2 = tile.String2.ToInt();
			                                    } else {
			                                        type1 = tile.String2.ToInt();
			                                    }
			                                }
		                                
		                                    switch ((Enums.PokemonType)type1) {
		                                        case Enums.PokemonType.Bug: {
		                                                moveIndex = 616; //quiver dance
		                                            }
		                                            break;
		                                        case Enums.PokemonType.Dark: {
		                                        		moveIndex = 672; //night daze
		                                            }
		                                            break;
		                                        case Enums.PokemonType.Dragon: {
		                                                moveIndex = 457; //draco meteor
		                                            }
		                                            break;
		                                        case Enums.PokemonType.Electric: {
		                                                moveIndex = 13; //thunder
		                                            }
		                                            break;
		                                        case Enums.PokemonType.Fighting: {
		                                                moveIndex = 70; //aura sphere
		                                            }
		                                            break;
		                                        case Enums.PokemonType.Fire: {
		                                                moveIndex = 95; //fire blast
		                                            }
		                                            break;
		                                        case Enums.PokemonType.Flying: {
		                                                moveIndex = 675; //hurricane
		                                            }
		                                            break;
		                                        case Enums.PokemonType.Ghost: {
		                                                moveIndex = 162; //night shade
		                                            }
		                                            break;
		                                        case Enums.PokemonType.Grass: {
		                                                moveIndex = 416; //spore
		                                            }
		                                            break;
		                                        case Enums.PokemonType.Ground: {
		                                                moveIndex = 15; //earthquake
		                                            }
		                                            break;
		                                        case Enums.PokemonType.Ice: {
		                                                moveIndex = 74; //blizzard
		                                            }
		                                            break;
		                                        case Enums.PokemonType.Normal: {
		                                                moveIndex = 49; //tri attack
		                                            }
		                                            break;
		                                        case Enums.PokemonType.Poison: {
		                                                moveIndex = 622; //coil
		                                            }
		                                            break;
		                                        case Enums.PokemonType.Psychic: {
		                                                moveIndex = 110; //light screen
		                                            }
		                                            break;
		                                        case Enums.PokemonType.Rock: {
		                                                moveIndex = 374; //rock slide
		                                            }
		                                            break;
		                                        case Enums.PokemonType.Steel: {
		                                                moveIndex = 319; //metal burst
		                                            }
		                                            break;
		                                        case Enums.PokemonType.Water: {
		                                                moveIndex = 205; //hydro pump
		                                            }
		                                            break;
		                                        default: {
		                                                moveIndex = 46; //swift
		                                            }
		                                            break;
		                                    }
		                                
				                   		}
				                   		break;
				                    
				                }
				                
				                    
				                if (moveIndex > 0 && IsInvokableMove(moveIndex)) {
				                    BattleSetup subSetup = new BattleSetup();
				                    subSetup.Attacker = setup.Attacker;
				                    subSetup.moveSlot = -1;
				                    
				                    subSetup.moveIndex = moveIndex;
				                    
									subSetup.BattleTags.Add("InvokedMove");
									BattleProcessor.HandleAttack(subSetup);
				
				
				                    BattleProcessor.FinalizeAction(subSetup, setup.PacketStack);
				                	
				                    setup.Cancel = true;
				                    return;
			                    } else {
			                    	setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("But it failed!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
			                    }
		                    } else {
			                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("But it failed!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
			                }
                    
                    	}
                    	break;
                    case 191: {//invisible
                            
                            AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Invisible", setup.Move.Data2, null, "", setup.PacketStack, true);
                            
                        }
                        break;
                    case 194: {//HP to 1
                    		if (setup.Defender.Level > setup.Attacker.Level) {
                    			setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + "'s level is too high to be affected!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                    		} else {
	                    		int dmg = setup.Defender.HP - 1;
	                            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
	                                DamageCharacter(setup.Defender, setup.DefenderMap, dmg, Enums.KillType.Player, setup.PacketStack, false);
	                            } else {
	                                DamageCharacter(setup.Defender, setup.DefenderMap, dmg, Enums.KillType.Npc, setup.PacketStack, false);
	                            }
	                            setup.Damage = dmg;
                            }
                        }
                        break;
                    case 195: {//pounce
                    		if (setup.AttackerMap.Moral == Enums.MapMoral.None || setup.Attacker == setup.Defender) {
	                    		if (GetBattleTagArg(setup.BattleTags, "Pounce", 1) != null) {
	                                PointWarp(setup.Defender, setup.DefenderMap, setup.PacketStack, GetBattleTagArg(setup.BattleTags, "Pounce", 1).ToInt(), GetBattleTagArg(setup.BattleTags, "Pounce", 2).ToInt());
	                            }
	                            Pounce(setup.Defender, setup.DefenderMap, setup.Attacker.Direction, true, false, setup.PacketStack);
                            }
                        }
                        break;
                    case 197: {//sky drop kidnapping phase
                    		if (setup.Attacker.VolatileStatus.GetStatus("SkyDrop:1") == null && setup.Defender.VolatileStatus.GetStatus("SkyDrop:0") == null) {
                                AddExtraStatus(setup.Defender, setup.DefenderMap, "SkyDrop:0", 5, setup.Attacker, "", setup.PacketStack);
                                AddExtraStatus(setup.Attacker, setup.AttackerMap, "SkyDrop:1", -1, setup.Defender, "", setup.PacketStack);
                            }
                    	}
                    	break;
                    case 200: {//quash
                    		if (setup.Defender.AttackTimer == null || setup.Defender.AttackTimer.Tick < Core.GetTickCount().Tick) {
			                    setup.Defender.AttackTimer = new TickCount(Core.GetTickCount().Tick);
			                }
			                setup.Defender.AttackTimer = new TickCount(setup.Defender.AttackTimer.Tick + 4000);
			                setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + "'s attacks were postponed!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                    	}
                    	break;
                    case 201: {//after you
				                    
				                    List<int> moves = new List<int>();
				                    
				                    for (int i = 0; i < Constants.MAX_PLAYER_MOVES; i++) {
				                    	if (IsInvokableMove(setup.Defender.Moves[i].MoveNum)) {
				                    		moves.Add(setup.Defender.Moves[i].MoveNum);
				                    	}
				                    }
					                BattleSetup groupSetup = new BattleSetup();
                                    groupSetup.Attacker = setup.Defender;
                                    groupSetup.moveSlot = -1;
                                    
					                if (moves.Count > 0) {
					                	int rand = Server.Math.Rand(0, moves.Count);
					                    
					                    groupSetup.moveIndex = moves[rand];
					                } else {
                                    	groupSetup.moveIndex = 466;
                                    }
					                
                        			
					                    
					                    
										groupSetup.BattleTags.Add("OrderedMove");
	
	
	
	                                    BattleProcessor.HandleAttack(groupSetup);
	
	
	                                    BattleProcessor.FinalizeAction(groupSetup, setup.PacketStack);
                                    

                        }
                        break;
                    case 204: {//pain split
                    		if (GetBattleTagArg(setup.BattleTags, "PainSplit", 1) != null) {
                    			int hp = GetBattleTagArg(setup.BattleTags, "PainSplit", 1).ToInt();
                    			if (hp >= setup.Defender.HP) {
                    				HealCharacter(setup.Defender, setup.DefenderMap, hp - setup.Defender.HP, setup.PacketStack);
                    			} else {
                    				DamageCharacter(setup.Defender, setup.DefenderMap, setup.Defender.HP - hp, Enums.KillType.Player, setup.PacketStack, true);
                    			}
                    		}
                    	}
                    	break;
                    case 206: {//conversion
                    		if (setup.Move.Data2 == 1) {
                    			AddExtraStatus(setup.Defender, setup.DefenderMap, "Conversion", 0, null, "", setup.PacketStack);
                    			setup.BattleTags.Add("Conversion");
                    		} else if (setup.Move.Data2 == 2) {
                    			AddExtraStatus(setup.Defender, setup.DefenderMap, "Conversion2", 0, null, "", setup.PacketStack);
                    		}
                    	}
                    	break;
                    case 207: {//rebound
                            AddExtraStatus(setup.Defender, setup.DefenderMap, "Rebound", 10, null, "", setup.PacketStack);
                        }
                        break;
                    case 208: {//Power Trick
                    		int atk, def;
			                if (setup.Defender.VolatileStatus.GetStatus("ProxyAttack") != null) {
			                	atk = setup.Defender.VolatileStatus.GetStatus("ProxyAttack").Counter;
			                } else {
			                    atk = setup.Defender.Atk;
			                }
			                if (setup.Defender.VolatileStatus.GetStatus("ProxyDefense") != null) {
                    			def = setup.Defender.VolatileStatus.GetStatus("ProxyDefense").Counter;
                    		} else {
                        		def = setup.Defender.Def;
                        	}
			                
                            AddExtraStatus(setup.Defender, setup.DefenderMap, "ProxyAttack", def, null, "", setup.PacketStack);
                            AddExtraStatus(setup.Defender, setup.DefenderMap, "ProxyDefense", atk, null, "", setup.PacketStack);
                            
                            setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " swapped its Attack and Defense!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                            
                        }
                        break;
                    case 209: {//stat split
                    		int stat = 0, special = 0;
                    		if (GetBattleTagArg(setup.BattleTags, "StatSplit", 0) != null) {
                    			stat = GetBattleTagArg(setup.BattleTags, "StatSplit", 1).ToInt();
                    			special = GetBattleTagArg(setup.BattleTags, "StatSplit", 2).ToInt();
                    		}
                    		if (setup.Move.Data2 == 1) {
                    			AddExtraStatus(setup.Defender, setup.DefenderMap, "ProxyAttack", stat, null, "", setup.PacketStack);
                            	AddExtraStatus(setup.Defender, setup.DefenderMap, "ProxySpAtk", special, null, "", setup.PacketStack);
                    			setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + "'s Attack and Sp. Attack was shared!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                    		} else if (setup.Move.Data2 == 2) {
                    			AddExtraStatus(setup.Defender, setup.DefenderMap, "ProxyDefense", stat, null, "", setup.PacketStack);
                            	AddExtraStatus(setup.Defender, setup.DefenderMap, "ProxySpDef", special, null, "", setup.PacketStack);
                    			setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + "'s Defense and Sp. Defense was shared!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                    		}
                        }
                        break;
                    case 210: {//nonvolatile status cure with immunity
                    		if (setup.Defender.StatusAilment == (Enums.StatusAilment)setup.Move.Data2) {
                                SetStatusAilment(setup.Defender, setup.DefenderMap, Enums.StatusAilment.OK, 0, setup.PacketStack);
                            } else {
                                AddExtraStatus(setup.Defender, setup.DefenderMap, "Status Guard", 10, null, setup.Move.Data2.ToString(), setup.PacketStack);
                            }
                    	}
                    	break;
                    case 212: {//sketch
                    		if (setup.moveSlot >= 0 && setup.moveSlot < Constants.MAX_PLAYER_MOVES) {
	                    		if (setup.Attacker.Moves[setup.moveSlot].MoveNum == 435) {
	                    			for (int i = 0; i < Constants.MAX_PLAYER_MOVES; i++) {
	                    				int moveNum = setup.Defender.Moves[i].MoveNum;
	                    				if (moveNum <= 0 || moveNum == 435) {
	                    					setup.Attacker.Moves[i].MoveNum = -1;
						                    setup.Attacker.Moves[i].MaxPP = -1;
						                    setup.Attacker.Moves[i].CurrentPP = -1;
						                    setup.Attacker.Moves[i].Sealed = false;
	                    				} else {
		                    				setup.Attacker.Moves[i].MoveNum = moveNum;
						                    setup.Attacker.Moves[i].MaxPP = MoveManager.Moves[moveNum].MaxPP;
						                    setup.Attacker.Moves[i].CurrentPP = setup.Attacker.Moves[i].MaxPP;
						                    setup.Attacker.Moves[i].Sealed = false;
					                    }
					                }
					                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " sketched " + setup.Defender.Name + "'s moves!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
					                if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
		                    			RefreshCharacterMoves(setup.Attacker, setup.AttackerMap, setup.PacketStack);
		                    		}
	                    		}
	                    	}
                    	}
                    	break;
                    case 213: {//shocker
                    		AddExtraStatus(setup.Defender, setup.DefenderMap, "Shocker", 10, null, "", setup.PacketStack);
                    	}
                    	break;
                    case 214: {//pierce
                    		AddExtraStatus(setup.Defender, setup.DefenderMap, "Pierce", 0, null, "", setup.PacketStack);
                    	}
                    	break;
                    case 215: {//longtoss
                    		AddExtraStatus(setup.Defender, setup.DefenderMap, "Longtoss", 0, null, "", setup.PacketStack);
                    	}
                    	break;
                    case 216: {//stayaway
                            AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Immobilize", Server.Math.Rand(5, 8), null, "", setup.PacketStack, true);
                            WarpToStairs(setup.Defender, setup.DefenderMap, setup.PacketStack, 1);
                        }
                        break;
                    case 218: {//bestow
                    		//check to see if there's room in players' inventory to accept item
                    		bool willAccept = true;
                    		if (setup.Defender.HeldItem != null) {
                    			willAccept = false;
                    			setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name+" is already holding something!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                    		}
                    		if (setup.Attacker.HeldItem == null) {
                    			willAccept = false;
                    			setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name+" isn't holding anything to bestow!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                    		}
                    		if (willAccept && ItemManager.Items[setup.Attacker.HeldItem.Num].Bound) {
                    			willAccept = false;
                    			setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name+" can't bestow its item!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                    		}
                    		if (willAccept && setup.Defender.CharacterType == Enums.CharacterType.Recruit) {
                    			if (((Recruit)setup.Defender).Owner.Player.FindInvSlot(setup.Attacker.HeldItem.Num, setup.Attacker.HeldItem.Amount) == -1) {
                    				willAccept = false;
                    				setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name+" has no more room to accept the item!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                    			}
                    		}
                    		if (willAccept) {
                    			setup.Defender.GiveHeldItem(setup.Attacker.HeldItem.Num, setup.Attacker.HeldItem.Amount, setup.Attacker.HeldItem.Tag, setup.Attacker.HeldItem.Sticky);
                    			setup.Attacker.TakeHeldItem(setup.Attacker.HeldItem.Amount);
                    			setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " received " + setup.Attacker.Name + "'s " + ItemManager.Items[setup.Defender.HeldItem.Num].Name + "!", Text.BrightGreen), setup.Defender.X, setup.Defender.Y, 10);
                    		}
                        }
                        break;
                    case 219: {//nonvolatile status cure
                    		if (setup.Defender.StatusAilment == (Enums.StatusAilment)setup.Move.Data2) {
                                SetStatusAilment(setup.Defender, setup.DefenderMap, Enums.StatusAilment.OK, 0, setup.PacketStack);
                            }
                    	}
                    	break;
                    case 220: {//Increases Atk and SpAtk by 2 (all power up orb)
                            ChangeAttackBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 2, setup.PacketStack);
                            ChangeSpAtkBuff(setup.Defender, setup.Attacker, setup.DefenderMap, 2, setup.PacketStack);
                        }
                        break;
                    case 221: {//health orb
                    		int duration = setup.Move.Data2;
                        	if (setup.Attacker.HasActiveItem(84)) {
                        	    duration *= 2;
                        	}
                        	AddExtraStatus(setup.Defender, setup.DefenderMap, "Safeguard", duration, null, "", setup.PacketStack);
                            AddExtraStatus(setup.Defender, setup.DefenderMap, "Mist", duration, null, "", setup.PacketStack);
                    	}
                    	break;
                    case 222: {//recovery orb
                    		SetStatusAilment(setup.Defender, setup.DefenderMap, Enums.StatusAilment.OK, 0, setup.PacketStack);
                    		RemoveBuffs(setup.Defender);
                            setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + "'s stat changes were removed!", Text.WhiteSmoke), setup.Defender.X, setup.Defender.Y, 10);
                    	}
                    	break;
                }
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: ScriptedMoveHitCharacter", Text.Black);
            }
        }

        public static void MoveAdditionalEffect(BattleSetup setup) {
            try {
                ExtraStatus status;
                switch (setup.Move.AdditionalEffectData1) {
                    case 1: {//burn
                            if (setup.Hit && (!setup.KnockedOut || setup.Defender.HP > 0)) {
                                if (Server.Math.Rand(0, 100) < setup.Move.AdditionalEffectData2) {
                                    SetStatusAilment(setup.Defender, setup.Attacker, setup.DefenderMap, Enums.StatusAilment.Burn, 1, setup.PacketStack);

                                }
                            }
                        }
                        break;
                    case 2: {//freeze
                            if (setup.Hit && (!setup.KnockedOut || setup.Defender.HP > 0)) {
                                if (Server.Math.Rand(0, 100) < setup.Move.AdditionalEffectData2) {
                                    SetStatusAilment(setup.Defender, setup.Attacker, setup.DefenderMap, Enums.StatusAilment.Freeze, 1, setup.PacketStack);

                                }
                            }
                        }
                        break;
                    case 3: {//paralysis
                            if (setup.Hit && (!setup.KnockedOut || setup.Defender.HP > 0)) {
                                if (Server.Math.Rand(0, 100) < setup.Move.AdditionalEffectData2) {
                                    SetStatusAilment(setup.Defender, setup.Attacker, setup.DefenderMap, Enums.StatusAilment.Paralyze, 1, setup.PacketStack);

                                }
                            }
                        }
                        break;
                    case 4: {//poison
                            if (setup.Hit && (!setup.KnockedOut || setup.Defender.HP > 0)) {
                                if (Server.Math.Rand(0, 100) < setup.Move.AdditionalEffectData2) {
                                    SetStatusAilment(setup.Defender, setup.Attacker, setup.DefenderMap, Enums.StatusAilment.Poison, setup.Move.AdditionalEffectData3, setup.PacketStack);

                                }
                            }
                        }
                        break;
                    case 5: {//sleep
                            if (setup.Hit && (!setup.KnockedOut || setup.Defender.HP > 0)) {
                                if (Server.Math.Rand(0, 100) < setup.Move.AdditionalEffectData2) {
                                    SetStatusAilment(setup.Defender, setup.Attacker, setup.DefenderMap, Enums.StatusAilment.Sleep, setup.Move.AdditionalEffectData3, setup.PacketStack);

                                }
                            }
                        }
                        break;
                    case 6: {//confuse
                            if (setup.Hit && (!setup.KnockedOut || setup.Defender.HP > 0)) {
                                if (Server.Math.Rand(0, 100) < setup.Move.AdditionalEffectData2) {
                                    //Confuse(setup.Defender, setup.Attacker, setup.DefenderMap, Server.Math.Rand(5, 11), setup.PacketStack);
                                    AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Confusion", Server.Math.Rand(5, 11), null, "", setup.PacketStack, true);
                                }
                            }
                        }
                        break;
                    case 7: {//flinch
                            if (setup.Hit && (!setup.KnockedOut || setup.Defender.HP > 0)) {
                                if (Server.Math.Rand(0, 100) < setup.Move.AdditionalEffectData2) {
                                    Flinch(setup.Defender, setup.Attacker, setup.DefenderMap, setup.PacketStack);
                                }
                            }
                        }
                        break;
                    case 8: {//pause
                            if (setup.Hit && (!setup.KnockedOut || setup.Defender.HP > 0)) {
                                if (Server.Math.Rand(0, 100) < setup.Move.AdditionalEffectData2) {

                                }
                            }
                        }
                        break;
                    case 14: {//atk ~ self
                            if (setup.BattleTags.Contains("HitSomething")) {
                                if (Server.Math.Rand(0, 100) < setup.Move.AdditionalEffectData2) {
                                    ChangeAttackBuff(setup.Attacker, setup.AttackerMap, setup.Move.AdditionalEffectData3, setup.PacketStack);
                                }
                            }
                        }
                        break;
                    case 15: {//def ~ self
                            if (setup.BattleTags.Contains("HitSomething")) {
                                if (Server.Math.Rand(0, 100) < setup.Move.AdditionalEffectData2) {
                                    ChangeDefenseBuff(setup.Attacker, setup.AttackerMap, setup.Move.AdditionalEffectData3, setup.PacketStack);
                                }
                            }
                        }
                        break;
                    case 16: {//spatk ~ self
                            if (setup.BattleTags.Contains("HitSomething")) {
                                if (Server.Math.Rand(0, 100) < setup.Move.AdditionalEffectData2) {
                                    ChangeSpAtkBuff(setup.Attacker, setup.AttackerMap, setup.Move.AdditionalEffectData3, setup.PacketStack);
                                }
                            }
                        }
                        break;
                    case 17: {//spdef ~ self
                            if (setup.BattleTags.Contains("HitSomething")) {
                                if (Server.Math.Rand(0, 100) < setup.Move.AdditionalEffectData2) {
                                    ChangeSpDefBuff(setup.Attacker, setup.AttackerMap, setup.Move.AdditionalEffectData3, setup.PacketStack);
                                }
                            }
                        }
                        break;
                    case 18: {//speed ~ self
                            if (setup.BattleTags.Contains("HitSomething")) {
                                if (Server.Math.Rand(0, 100) < setup.Move.AdditionalEffectData2) {
                                    ChangeSpeedBuff(setup.Attacker, setup.AttackerMap, setup.Move.AdditionalEffectData3, setup.PacketStack);
                                }
                            }
                        }
                        break;
                    case 19: {//acc ~ self
                            if (setup.BattleTags.Contains("HitSomething")) {
                                if (Server.Math.Rand(0, 100) < setup.Move.AdditionalEffectData2) {
                                    ChangeAccuracyBuff(setup.Attacker, setup.AttackerMap, setup.Move.AdditionalEffectData3, setup.PacketStack);
                                }
                            }
                        }
                        break;
                    case 20: {//eva ~ self
                            if (setup.BattleTags.Contains("HitSomething")) {
                                if (Server.Math.Rand(0, 100) < setup.Move.AdditionalEffectData2) {
                                    ChangeEvasionBuff(setup.Attacker, setup.AttackerMap, setup.Move.AdditionalEffectData3, setup.PacketStack);
                                }
                            }
                        }
                        break;
                    case 21: {//atk
                            if (setup.Hit && (!setup.KnockedOut || setup.Defender.HP > 0)) {
                                if (Server.Math.Rand(0, 100) < setup.Move.AdditionalEffectData2) {
                                    ChangeAttackBuff(setup.Defender, setup.Attacker, setup.DefenderMap, setup.Move.AdditionalEffectData3, setup.PacketStack);
                                }
                            }
                        }
                        break;
                    case 22: {//def
                            if (setup.Hit && (!setup.KnockedOut || setup.Defender.HP > 0)) {
                                if (Server.Math.Rand(0, 100) < setup.Move.AdditionalEffectData2) {
                                    ChangeDefenseBuff(setup.Defender, setup.Attacker, setup.DefenderMap, setup.Move.AdditionalEffectData3, setup.PacketStack);
                                }
                            }
                        }
                        break;
                    case 23: {//spatk
                            if (setup.Hit && (!setup.KnockedOut || setup.Defender.HP > 0)) {
                                if (Server.Math.Rand(0, 100) < setup.Move.AdditionalEffectData2) {
                                    ChangeSpAtkBuff(setup.Defender, setup.Attacker, setup.DefenderMap, setup.Move.AdditionalEffectData3, setup.PacketStack);
                                }
                            }
                        }
                        break;
                    case 24: {//spdef
                            if (setup.Hit && (!setup.KnockedOut || setup.Defender.HP > 0)) {
                                if (Server.Math.Rand(0, 100) < setup.Move.AdditionalEffectData2) {
                                    ChangeSpDefBuff(setup.Defender, setup.Attacker, setup.DefenderMap, setup.Move.AdditionalEffectData3, setup.PacketStack);
                                }
                            }
                        }
                        break;
                    case 25: {//speed
                            if (setup.Hit && (!setup.KnockedOut || setup.Defender.HP > 0)) {
                                if (Server.Math.Rand(0, 100) < setup.Move.AdditionalEffectData2) {
                                    ChangeSpeedBuff(setup.Defender, setup.Attacker, setup.DefenderMap, setup.Move.AdditionalEffectData3, setup.PacketStack);
                                }
                            }
                        }
                        break;
                    case 26: {//acc
                            if (setup.Hit && (!setup.KnockedOut || setup.Defender.HP > 0)) {
                                if (Server.Math.Rand(0, 100) < setup.Move.AdditionalEffectData2) {
                                    ChangeAccuracyBuff(setup.Defender, setup.Attacker, setup.DefenderMap, setup.Move.AdditionalEffectData3, setup.PacketStack);
                                }
                            }
                        }
                        break;
                    case 27: {//eva
                            if (setup.Hit && (!setup.KnockedOut || setup.Defender.HP > 0)) {
                                if (Server.Math.Rand(0, 100) < setup.Move.AdditionalEffectData2) {
                                    ChangeEvasionBuff(setup.Defender, setup.Attacker, setup.DefenderMap, setup.Move.AdditionalEffectData3, setup.PacketStack);
                                }
                            }
                        }
                        break;
                    case 28: {//recoil
                            if (setup.Hit && !HasAbility(setup.Attacker, "Rock Head") && !HasAbility(setup.Attacker, "Magic Guard")) {
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " was hit with recoil!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                                int newHP = setup.Defender.HP;
                                if (newHP < 0) newHP = 0;
                                int recoil = (GetBattleTagArg(setup.BattleTags, "PrevHP", 1).ToInt() - newHP) / (setup.Move.AdditionalEffectData2 * 2);
                    			if (recoil < 0) recoil = 0;
                                if (recoil >= setup.Attacker.HP) {
                                    DamageCharacter(setup.Attacker, setup.AttackerMap, recoil, Enums.KillType.Other, setup.PacketStack, true);
                                    setup.Cancel = true;
                                } else {
                                    DamageCharacter(setup.Attacker, setup.AttackerMap, recoil, Enums.KillType.Other, setup.PacketStack, true);
                                }
                            }
                        }
                        break;
                    case 29: {//critical

                        }
                        break;
                    case 30: {//set dmg

                        }
                        break;
                    case 31: {//absorb
                            if (setup.Hit && setup.Damage > 0) {
                                //if  (!setup.KnockedOut || setup.Defender.HP > 0) {
                                int heal = setup.Move.AdditionalEffectData2;
                                //big root
                                if (setup.Attacker.HasActiveItem(194)) heal += 20;
                                //pokemon-specific item
			                    if (HasActiveBagItem(setup.Attacker, 5, 0, 0)) {
			                    	heal += 30;
			                    }
                                if (HasAbility(setup.Defender, "Liquid Ooze")) {
                                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " sucked up the Liquid Ooze!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                                    DamageCharacter(setup.Attacker, setup.AttackerMap, setup.Damage * heal / 100, Enums.KillType.Other, setup.PacketStack, true);
                                } else {
                                    HealCharacter(setup.Attacker, setup.AttackerMap, setup.Damage * heal / 100, setup.PacketStack);
                                }
                                //} else {
                                //HealCharacter(setup.Attacker, setup.AttackerMap, setup.Damage / 2, setup.PacketStack);
                                //}
                            }
                        }
                        break;
                    case 35: {//Snore; flinch chance
                            if (setup.Hit && (!setup.KnockedOut || setup.Defender.HP > 0)) {
                                if (Server.Math.Rand(0, 100) < setup.Move.AdditionalEffectData2) {
                                    Flinch(setup.Defender, setup.Attacker, setup.DefenderMap, setup.PacketStack);
                                }
                            }
                        }
                        break;
                    case 37: {//breaks through ice; may cause burn
                            if (setup.Hit && (!setup.KnockedOut || setup.Defender.HP > 0)) {
                                if (Server.Math.Rand(0, 100) < setup.Move.AdditionalEffectData2) {
                                    SetStatusAilment(setup.Defender, setup.Attacker, setup.DefenderMap, Enums.StatusAilment.Burn, 1, setup.PacketStack);
                                }
                            }
                        }
                        break;
                    case 38: {//breaks through ice; recoil; may cause burn
                            if (setup.Hit) {
                                if (!setup.KnockedOut || setup.Defender.HP > 0) {
                                    if (Server.Math.Rand(0, 100) < setup.Move.AdditionalEffectData2) {
                                        SetStatusAilment(setup.Defender, setup.Attacker, setup.DefenderMap, Enums.StatusAilment.Burn, 1, setup.PacketStack);
                                    }
                                }
                                if (!HasAbility(setup.Attacker, "Rock Head") && !HasAbility(setup.Attacker, "Magic Guard")) {
                                	setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " was hit with recoil!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
	                                int newHP = setup.Defender.HP;
	                                if (newHP < 0) newHP = 0;
	                                int recoil = (GetBattleTagArg(setup.BattleTags, "PrevHP", 1).ToInt() - newHP) / (setup.Move.AdditionalEffectData3 * 2);
	                    			if (recoil < 0) recoil = 0;
	                                if (recoil >= setup.Attacker.HP) {
	                                    DamageCharacter(setup.Attacker, setup.AttackerMap, recoil, Enums.KillType.Other, setup.PacketStack, true);
	                                    setup.Cancel = true;
	                                } else {
	                                    DamageCharacter(setup.Attacker, setup.AttackerMap, recoil, Enums.KillType.Other, setup.PacketStack, true);
	                                }
                                }
                            }
                        }
                        break;
                    case 46: {//Lowers Two Stats (attacker)
                            if (setup.BattleTags.Contains("HitSomething")) {
                                switch (setup.Move.AdditionalEffectData2) {
                                    case 1: {
                                            ChangeAttackBuff(setup.Attacker, setup.AttackerMap, -1, setup.PacketStack);
                                        }
                                        break;
                                    case 2: {
                                            ChangeDefenseBuff(setup.Attacker, setup.AttackerMap, -1, setup.PacketStack);
                                        }
                                        break;
                                    case 3: {
                                            ChangeSpAtkBuff(setup.Attacker, setup.AttackerMap, -1, setup.PacketStack);
                                        }
                                        break;
                                    case 4: {
                                            ChangeSpDefBuff(setup.Attacker, setup.AttackerMap, -1, setup.PacketStack);
                                        }
                                        break;
                                    case 5: {
                                            ChangeSpeedBuff(setup.Attacker, setup.AttackerMap, -1, setup.PacketStack);
                                        }
                                        break;
                                }
                                switch (setup.Move.AdditionalEffectData3) {
                                    case 1: {
                                            ChangeAttackBuff(setup.Attacker, setup.AttackerMap, -1, setup.PacketStack);
                                        }
                                        break;
                                    case 2: {
                                            ChangeDefenseBuff(setup.Attacker, setup.AttackerMap, -1, setup.PacketStack);
                                        }
                                        break;
                                    case 3: {
                                            ChangeSpAtkBuff(setup.Attacker, setup.AttackerMap, -1, setup.PacketStack);
                                        }
                                        break;
                                    case 4: {
                                            ChangeSpDefBuff(setup.Attacker, setup.AttackerMap, -1, setup.PacketStack);
                                        }
                                        break;
                                    case 5: {
                                            ChangeSpeedBuff(setup.Attacker, setup.AttackerMap, -1, setup.PacketStack);
                                        }
                                        break;
                                }
                            }

                        }
                        break;
                    case 47: {//+Atk/Def/SpAtk/SpDef/Speed
                            if (setup.Hit) {
                                if (Server.Math.Rand(0, 100) < setup.Move.AdditionalEffectData2) {
                                    ChangeAttackBuff(setup.Attacker, setup.AttackerMap, 1, setup.PacketStack);
                                    ChangeDefenseBuff(setup.Attacker, setup.AttackerMap, 1, setup.PacketStack);
                                    ChangeSpAtkBuff(setup.Attacker, setup.AttackerMap, 1, setup.PacketStack);
                                    ChangeSpDefBuff(setup.Attacker, setup.AttackerMap, 1, setup.PacketStack);
                                    ChangeSpeedBuff(setup.Attacker, setup.AttackerMap, 1, setup.PacketStack);
                                }
                            }
                        }
                        break;
                    case 48: {//recoil; may cause paralyze
                            if (setup.Hit) {
                                if (!setup.KnockedOut || setup.Defender.HP > 0) {
                                    if (Server.Math.Rand(0, 100) < setup.Move.AdditionalEffectData2) {
                                        SetStatusAilment(setup.Defender, setup.Attacker, setup.DefenderMap, Enums.StatusAilment.Paralyze, 1, setup.PacketStack);
                                    }
                                }
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " was hit with recoil!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
	                                int newHP = setup.Defender.HP;
	                                if (newHP < 0) newHP = 0;
                                	int recoil = (GetBattleTagArg(setup.BattleTags, "PrevHP", 1).ToInt() - newHP) / (setup.Move.AdditionalEffectData3 * 2);
	                    			if (recoil < 0) recoil = 0;
	                                if (recoil >= setup.Attacker.HP) {
	                                    DamageCharacter(setup.Attacker, setup.AttackerMap, recoil, Enums.KillType.Other, setup.PacketStack, true);
	                                    setup.Cancel = true;
	                                } else {
	                                    DamageCharacter(setup.Attacker, setup.AttackerMap, recoil, Enums.KillType.Other, setup.PacketStack, true);
	                                }
                            }
                        }
                        break;
                    case 49: {// critical + status
                            if (setup.Hit && (!setup.KnockedOut || setup.Defender.HP > 0)) {
                                if (Server.Math.Rand(0, 100) < setup.Move.AdditionalEffectData2) {
                                    SetStatusAilment(setup.Defender, setup.Attacker, setup.DefenderMap, (Enums.StatusAilment)setup.Move.AdditionalEffectData3, 1, setup.PacketStack);
                                }
                            }
                        }
                        break;
                    case 50: {// tri Attack
                            if (setup.Hit && (!setup.KnockedOut || setup.Defender.HP > 0)) {
                                if (Server.Math.Rand(0, 100) < setup.Move.AdditionalEffectData2) {
                                    switch (Server.Math.Rand(0, 3)) {
                                        case 0: {
                                                SetStatusAilment(setup.Defender, setup.Attacker, setup.DefenderMap, Enums.StatusAilment.Burn, 1, setup.PacketStack);
                                            }
                                            break;
                                        case 1: {
                                                SetStatusAilment(setup.Defender, setup.Attacker, setup.DefenderMap, Enums.StatusAilment.Freeze, 1, setup.PacketStack);
                                            }
                                            break;
                                        case 2: {
                                                SetStatusAilment(setup.Defender, setup.Attacker, setup.DefenderMap, Enums.StatusAilment.Paralyze, 1, setup.PacketStack);
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                        break;
                    case 51: {//elemental fangs; flinch + status
                            if (setup.Hit && (!setup.KnockedOut || setup.Defender.HP > 0)) {
                                if (Server.Math.Rand(0, 100) < setup.Move.AdditionalEffectData2) {
                                    SetStatusAilment(setup.Defender, setup.Attacker, setup.DefenderMap, (Enums.StatusAilment)setup.Move.AdditionalEffectData3, Server.Math.Rand(3, 9), setup.PacketStack);
                                }
                            }
                            if (setup.Hit && (!setup.KnockedOut || setup.Defender.HP > 0)) {
                                if (Server.Math.Rand(0, 100) < setup.Move.AdditionalEffectData2) {
                                    Flinch(setup.Defender, setup.Attacker, setup.DefenderMap, setup.PacketStack);
                                }
                            }
                        }
                        break;
                    case 58: {//Partial trapper
                            if (setup.Hit && (!setup.KnockedOut || setup.Defender.HP > 0)) {
                                int trapTime = 2;
                                if (setup.Attacker.HasActiveItem(45)) {//Grip Claw
                                    trapTime = 4;
                                }
                                string intensity = "";
                                if (setup.Attacker.HasActiveItem(481)) {
                                	intensity = "1";
                                }
                                switch (setup.Move.AdditionalEffectData2) {
                                    case 1: {//Bind
                                            if (setup.Attacker.VolatileStatus.GetStatus("Bind:1") == null && setup.Defender.VolatileStatus.GetStatus("Bind:0") == null && !CheckStatusProtection(setup.Defender, setup.DefenderMap, "Bind:0", true, setup.PacketStack)) {
                                                AddExtraStatus(setup.Defender, setup.DefenderMap, "Bind:0", Server.Math.Rand(trapTime, trapTime + 2), setup.Attacker, intensity, setup.PacketStack);
                                                AddExtraStatus(setup.Attacker, setup.AttackerMap, "Bind:1", -1, setup.Defender, intensity, setup.PacketStack);
                                            }
                                        }
                                        break;
                                    case 2: {//Wrap
                                            if (setup.Attacker.VolatileStatus.GetStatus("Wrap:1") == null && setup.Defender.VolatileStatus.GetStatus("Wrap:0") == null && !CheckStatusProtection(setup.Defender, setup.DefenderMap, "Wrap:0", true, setup.PacketStack)) {
                                                AddExtraStatus(setup.Defender, setup.DefenderMap, "Wrap:0", Server.Math.Rand(trapTime, trapTime + 2), setup.Attacker, intensity, setup.PacketStack);
                                                AddExtraStatus(setup.Attacker, setup.AttackerMap, "Wrap:1", -1, setup.Defender, intensity, setup.PacketStack);
                                            }
                                        }
                                        break;
                                    case 3: {//Clamp
                                            if (setup.Attacker.VolatileStatus.GetStatus("Clamp:1") == null && setup.Defender.VolatileStatus.GetStatus("Clamp:0") == null && !CheckStatusProtection(setup.Defender, setup.DefenderMap, "Clamp:0", true, setup.PacketStack)) {
                                                AddExtraStatus(setup.Defender, setup.DefenderMap, "Clamp:0", Server.Math.Rand(trapTime, trapTime + 2), setup.Attacker, intensity, setup.PacketStack);
                                                AddExtraStatus(setup.Attacker, setup.AttackerMap, "Clamp:1", -1, setup.Defender, intensity, setup.PacketStack);
                                            }
                                        }
                                        break;
                                    case 4: {//Fire Spin
                                            AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "FireSpin", Server.Math.Rand(trapTime, trapTime + 2), null, intensity, setup.PacketStack, true);
                                        }
                                        break;
                                    case 5: {//Whirlpool
                                            AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Whirlpool", Server.Math.Rand(trapTime, trapTime + 2), null, intensity, setup.PacketStack, true);
                                        }
                                        break;
                                    case 6: {//Sand Tomb
                                            AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "SandTomb", Server.Math.Rand(trapTime, trapTime + 2), null, intensity, setup.PacketStack, true);
                                        }
                                        break;
                                    case 7: {//Magma Storm
                                            AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "MagmaStorm", Server.Math.Rand(trapTime, trapTime + 2), null, intensity, setup.PacketStack, true);
                                        }
                                        break;

                                }
                            }
                        }
                        break;
                    case 60: {//charge-up moves (extra effects here)
                            switch (setup.Move.AdditionalEffectData2) {
                                case 4: {//sky attack
                                        if (setup.Hit && (!setup.KnockedOut || setup.Defender.HP > 0)) {
                                            if (Server.Math.Rand(0, 100) <= 30) {
                                                Flinch(setup.Defender, setup.Attacker, setup.DefenderMap, setup.PacketStack);
                                            }
                                        }
                                    }
                                    break;
                                case 7: {//bounce
                                        if (setup.Hit && (!setup.KnockedOut || setup.Defender.HP > 0)) {
                                            if (Server.Math.Rand(0, 100) <= 30) {
                                                SetStatusAilment(setup.Defender, setup.Attacker, setup.DefenderMap, Enums.StatusAilment.Paralyze, 1, setup.PacketStack);
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    case 61: {//rampage moves (thrash, petal dance, outrage)
                            if (setup.Attacker.VolatileStatus.GetStatus("Rampage") == null) {
                                AddExtraStatus(setup.Attacker, setup.AttackerMap, "Rampage", Server.Math.Rand(2, 5), null, setup.moveIndex.ToString(), setup.PacketStack);
                            }
                        }
                        break;
                    case 62: {//rolling moves (rollout, ice ball)
                            if (setup.BattleTags.Contains("HitSomething")) {
                                if (setup.Attacker.VolatileStatus.GetStatus("Rolling") == null) {
                                    AddExtraStatus(setup.Attacker, setup.AttackerMap, "Rolling", 1, null, setup.moveIndex.ToString(), setup.PacketStack);
                                }
                            }
                        }
                        break;
                    case 63: {//fury cutter
                            if (setup.BattleTags.Contains("HitSomething")) {
                                if (setup.Attacker.VolatileStatus.GetStatus("FuryCutter") == null) {
                                    AddExtraStatus(setup.Attacker, setup.AttackerMap, "FuryCutter", 1, null, "", setup.PacketStack);
                                }
                            }
                        }
                        break;
                    case 70: {//spit up
                            RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "Stockpile", setup.PacketStack);
                        }
                        break;
                    case 79: {//smellingsalt
                            if (setup.Hit && (!setup.KnockedOut || setup.Defender.HP > 0)
                                && setup.Defender.StatusAilment == (Enums.StatusAilment)setup.Move.AdditionalEffectData2
                                && setup.Move.AdditionalEffectData3 == 0) {
                                SetStatusAilment(setup.Defender, setup.DefenderMap, Enums.StatusAilment.OK, 0, setup.PacketStack);
                            }
                        }
                        break;
                    case 85: {//rapid spin
                            RemoveBondedExtraStatus(setup.Attacker, setup.AttackerMap, "Wrap:0", setup.PacketStack, true);
                            RemoveBondedExtraStatus(setup.Attacker, setup.AttackerMap, "Bind:0", setup.PacketStack, true);
                            RemoveBondedExtraStatus(setup.Attacker, setup.AttackerMap, "Clamp:0", setup.PacketStack, true);
                            RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "FireSpin", setup.PacketStack);
                            RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "Whirlpool", setup.PacketStack);
                            RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "SandTomb", setup.PacketStack);
                            RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "MagmaStorm", setup.PacketStack);
                            RemoveBondedExtraStatus(setup.Attacker, setup.AttackerMap, "LeechSeed:0", setup.PacketStack, true);
                        }
                        break;
                    case 86: {//Defog
                            RemoveExtraStatus(setup.Defender, setup.DefenderMap, "Reflect", setup.PacketStack);
                            RemoveExtraStatus(setup.Defender, setup.DefenderMap, "LightScreen", setup.PacketStack);
                            RemoveExtraStatus(setup.Defender, setup.DefenderMap, "Mist", setup.PacketStack);
                            RemoveExtraStatus(setup.Defender, setup.DefenderMap, "Safeguard", setup.PacketStack);
                        }
                        break;
                    case 103: {//Haze effect
                            RemoveBuffs(setup.Defender);
                            setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + "'s stat changes were removed!", Text.WhiteSmoke), setup.Defender.X, setup.Defender.Y, 10);
                        }
                        break;
                    case 112: {//false swipe
                            setup.KnockedOut = false;
                        }
                        break;
                    case 113: {//uproar
                            if (setup.Hit && (!setup.KnockedOut || setup.Defender.HP > 0)) {
                                AddExtraStatus(setup.Defender, setup.DefenderMap, "Sleepless", 0, null, "", setup.PacketStack);
                            }
                        }
                        break;
                    case 125: {//phazer effect
                            if (setup.Hit) {
                                BlowBack(setup.Defender, setup.DefenderMap, setup.Attacker.Direction, setup.PacketStack);
                            }
                        }
                        break;
                    case 133: {//u-turn
                            int blockX = setup.Attacker.X;
                            int blockY = setup.Attacker.Y;
                            FindNearestBlock(setup.AttackerMap, (Enums.Direction)(((int)setup.Attacker.Direction + 1) % 2 + (int)setup.Attacker.Direction / 2 * 2), ref blockX, ref blockY);

                            setup.Attacker.Y = blockY;
                            setup.Attacker.X = blockX;

                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " jumped back!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                                PacketBuilder.AppendPlayerXY(((Recruit)setup.Attacker).Owner, setup.PacketStack);
                            } else if (setup.Attacker.CharacterType == Enums.CharacterType.MapNpc) {
                                PacketBuilder.AppendNpcXY(setup.AttackerMap, setup.PacketStack, ((MapNpc)setup.Attacker).MapSlot);
                            }
                        }
                        break;
                    case 136: {//brick break
                            RemoveExtraStatus(setup.Defender, setup.DefenderMap, "Reflect", setup.PacketStack);
                            RemoveExtraStatus(setup.Defender, setup.DefenderMap, "LightScreen", setup.PacketStack);
                        }
                        break;
                    case 155: {//Reflect Type
                            if (GetBattleTagArg(setup.BattleTags, "ReflectType", 1) != null) {
                                string[] typeParams = GetBattleTagArg(setup.BattleTags, "ReflectType", 1).Split(',');

                                int type1 = typeParams[Server.Math.Rand(0, typeParams.Length / 2) * 2].ToInt();
                                int type2 = typeParams[Server.Math.Rand(0, typeParams.Length / 2) * 2 + 1].ToInt();

                                AddExtraStatus(setup.Attacker, setup.AttackerMap, "Type1", type1, null, "", setup.PacketStack);
                                AddExtraStatus(setup.Attacker, setup.AttackerMap, "Type2", type2, null, "", setup.PacketStack);

                                RemoveBattleTag(setup.BattleTags, "ReflectType");

                            } else {

                            }

                        }
                        break;
                    case 160: {//round
                        if (MoveProcessor.GetMatchupWith(setup.Attacker, setup.Defender) == Enums.CharacterMatchup.Friend) {
                                int roundSlot = -1;

                                for (int i = 0; i < Constants.MAX_PLAYER_MOVES; i++) {
                                    if (setup.Defender.Moves[i].MoveNum == 629) {
                                        roundSlot = i;
                                    }
                                }

                                if (setup.Defender.CharacterType == Enums.CharacterType.Recruit) {
                                    if (setup.BattleTags.Contains("Round:" + (int)setup.Defender.CharacterType + ":" + ((Recruit)setup.Defender).Owner.Player.CharID)) {
                                        roundSlot = -1;
                                    }
                                } else if (setup.Defender.CharacterType == Enums.CharacterType.MapNpc) {
                                    if (setup.BattleTags.Contains("Round:" + (int)setup.Defender.CharacterType + ":" + ((MapNpc)setup.Defender).MapSlot)) {
                                        roundSlot = -1;
                                    }
                                }

                                if (roundSlot > -1) {

                                    BattleSetup groupSetup = new BattleSetup();
                                    groupSetup.Attacker = setup.Defender;
                                    groupSetup.moveSlot = roundSlot;

                                    foreach (string tag in setup.BattleTags) {
                                        if (tag.StartsWith("Round")) {
                                            groupSetup.BattleTags.Add(tag);
                                        }
                                    }



                                    BattleProcessor.HandleAttack(groupSetup);


                                    BattleProcessor.FinalizeAction(groupSetup, setup.PacketStack);
                                }
                            }

                        }
                        break;
                    case 163: {//echoed voice
                    		if (setup.Hit && (!setup.KnockedOut || setup.Defender.HP > 0)) {
                                status = setup.Defender.VolatileStatus.GetStatus("EchoedVoice");
	                    		if (status != null) {
                                    status.Counter++;
	                            } else {
	                                AddExtraStatus(setup.Defender, setup.DefenderMap, "EchoedVoice", 1, null, "", setup.PacketStack);
	                            }
                    		}
                    	}
                    	break;
                    case 165: {//flame burst
                    		if (setup.Hit) {
	                            if (GetBattleTagArg(setup.BattleTags, "FlameBurst", 0) == null) {
	                                setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateSoundPacket("magic215.wav"), setup.Defender.X, setup.Defender.Y, 10);
	                                for (int i = setup.Defender.X - 2; i <= setup.Defender.X + 2; i++) {
	                                    for (int j = setup.Defender.Y - 2; j <= setup.Defender.Y + 2; j++) {
	                                        if (i < 0 || j < 0 || i > setup.DefenderMap.MaxX || j > setup.DefenderMap.MaxY
	                                            || (i == setup.Defender.X && j == setup.Defender.Y)) {
	
	                                        } else {
	                                            setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateSpellAnim(506, i, j));
	                                        }
	                                    }
	                                }
	                                TargetCollection targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 2, setup.DefenderMap, setup.Defender, setup.Defender.X, setup.Defender.Y, Enums.Direction.Up, true, true, true);
                                    for (int i = 0; i < targets.Count; i++) {
                                        if (!(targets[i].X == setup.Defender.X && targets[i].Y == setup.Defender.Y)) {
                                            if (!SafeZoneCheck(setup.Attacker, setup.AttackerMap, targets[i], setup.DefenderMap)) {
                                                DamageCharacter(targets[i], setup.DefenderMap, targets[i].MaxHP / 16, Enums.KillType.Other, setup.PacketStack, true);
                                            }
                                        }
                                    }
	                                setup.BattleTags.Add("FlameBurst");
	                            }
							}
                        }
                        break;
                    case 166: {//Stat Swap
                            switch (setup.Move.AdditionalEffectData2) {
                                case 1: {//power Swap
                                        if (GetBattleTagArg(setup.BattleTags, "PowerSwap", 1) != null) {
                                            setup.Attacker.AttackBuff = GetBattleTagArg(setup.BattleTags, "PowerSwap", 2).ToInt();
                                            setup.Attacker.SpAtkBuff = GetBattleTagArg(setup.BattleTags, "PowerSwap", 3).ToInt();
                                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " received " + GetBattleTagArg(setup.BattleTags, "PowerSwap", 1) + "'s changes to Attack and Special Attack!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                                            RemoveBattleTag(setup.BattleTags, "PowerSwap");

                                        } else {

                                        }
                                    }
                                    break;
                                case 2: {//guard swap
                                        if (GetBattleTagArg(setup.BattleTags, "GuardSwap", 1) != null) {
                                            setup.Attacker.DefenseBuff = GetBattleTagArg(setup.BattleTags, "GuardSwap", 2).ToInt();
                                            setup.Attacker.SpDefBuff = GetBattleTagArg(setup.BattleTags, "GuardSwap", 3).ToInt();
                                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " received " + GetBattleTagArg(setup.BattleTags, "GuardSwap", 1) + "'s changes to Defense and Special Defense!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                                            RemoveBattleTag(setup.BattleTags, "GuardSwap");

                                        } else {

                                        }
                                    }
                                    break;
                                case 3: {//heart swap
										if (GetBattleTagArg(setup.BattleTags, "HeartSwap", 1) != null) {
                                            setup.Attacker.AttackBuff = GetBattleTagArg(setup.BattleTags, "HeartSwap", 2).ToInt();
                                            setup.Attacker.SpAtkBuff = GetBattleTagArg(setup.BattleTags, "HeartSwap", 3).ToInt();
                                            setup.Attacker.DefenseBuff = GetBattleTagArg(setup.BattleTags, "HeartSwap", 4).ToInt();
                                            setup.Attacker.SpDefBuff = GetBattleTagArg(setup.BattleTags, "HeartSwap", 5).ToInt();
                                            setup.Attacker.SpeedBuff = GetBattleTagArg(setup.BattleTags, "HeartSwap", 6).ToInt();
                                            setup.Attacker.AccuracyBuff = GetBattleTagArg(setup.BattleTags, "HeartSwap", 7).ToInt();
                                            setup.Attacker.EvasionBuff = GetBattleTagArg(setup.BattleTags, "HeartSwap", 8).ToInt();
                                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " received " + GetBattleTagArg(setup.BattleTags, "HeartSwap", 1) + "'s stat changes!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                                            RemoveBattleTag(setup.BattleTags, "HeartSwap");

                                        } else {
                                        
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    case 171: {//Bug Bite/Pluck
                    		if (setup.Hit) {
	                            BattleSetup itemSetup = new BattleSetup();
	                            itemSetup.Attacker = setup.Attacker;
	                            //itemSetup.PacketStack = setup.PacketStack;
	                            if (setup.Defender.CharacterType == Enums.CharacterType.Recruit) {
	                                Client client = ((Recruit)setup.Defender).Owner;
	                                List<int> itemsToRemove = new List<int>();
	                                for (int i = 1; i <= client.Player.MaxInv; i++) {
	                                    if (CheckItemEdibility(client.Player.Inventory[i].Num) > 0) {
	                                        itemsToRemove.Add(i);
	                                    }
	                                }
	                                if (itemsToRemove.Count > 0) {
	                                    int randomItem = itemsToRemove[Server.Math.Rand(0, itemsToRemove.Count)];
	                                    if (CanUseItem(setup, client.Player.Inventory[randomItem])) {
	                                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " took the " + ItemManager.Items[client.Player.Inventory[randomItem].Num].Name + "!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
	                                        BattleProcessor.HandleItemUse(client.Player.Inventory[randomItem], -1, itemSetup);
	                                        client.Player.TakeItemSlot(randomItem, 1, true);
	                                    }
	                                    Messenger.SendInventory(client);
	                                }
	                            } else {
	                                MapNpc npc = (MapNpc)setup.Defender;
	                                if (npc.HeldItem != null && CheckItemEdibility(npc.HeldItem.Num) > 0) {
	                                    if (CanUseItem(setup, npc.HeldItem)) {
	                                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " took the " + ItemManager.Items[npc.HeldItem.Num].Name + "!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
	                                        BattleProcessor.HandleItemUse(npc.HeldItem, -1, itemSetup);
	                                        npc.TakeHeldItem(1);
	                                    }
	                                }
	                            }
	                            setup.PacketStack.AddHitList(itemSetup.PacketStack);
                            }
                        }
                        break;
                    case 172: {//Incinerate
                    		if (setup.Hit) {
	                            if (setup.Defender.CharacterType == Enums.CharacterType.Recruit) {
	                                Client client = ((Recruit)setup.Defender).Owner;
	                                List<int> itemsToRemove = new List<int>();
	                                for (int i = 1; i <= client.Player.MaxInv; i++) {
	                                    if (CheckItemEdibility(client.Player.Inventory[i].Num) > 0) {
	                                        itemsToRemove.Add(i);
	                                    }
	                                }
	                                if (itemsToRemove.Count > 0) {
	                                    int randomItem = itemsToRemove[Server.Math.Rand(0, itemsToRemove.Count)];
	                                    if (CanUseItem(setup, client.Player.Inventory[randomItem])) {
	                                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " incinerated the " + ItemManager.Items[client.Player.Inventory[randomItem].Num].Name + "!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
	                                        client.Player.TakeItemSlot(randomItem, 1, true);
	                                    }
	                                    Messenger.SendInventory(client);
	                                }
	                            } else {
	                                MapNpc npc = (MapNpc)setup.Defender;
	                                if (npc.HeldItem != null && CheckItemEdibility(npc.HeldItem.Num) > 0) {
	                                    if (CanUseItem(setup, npc.HeldItem)) {
	                                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " incinerated the " + ItemManager.Items[npc.HeldItem.Num].Name + "!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
	                                        npc.TakeHeldItem(1);
	                                    }
	                                }
	                            }
                            }
                        }
                        break;
                    case 178: {//Role Play
                            if (GetBattleTagArg(setup.BattleTags, "RolePlay", 1) != null) {
                                string[] typeParams = GetBattleTagArg(setup.BattleTags, "RolePlay", 1).Split(',');

                                string ability1 = typeParams[Server.Math.Rand(0, typeParams.Length / 3) * 3];
                                string ability2 = typeParams[Server.Math.Rand(0, typeParams.Length / 3) * 3 + 1];
                                string ability3 = typeParams[Server.Math.Rand(0, typeParams.Length / 3) * 3 + 2];

                                AddExtraStatus(setup.Attacker, setup.AttackerMap, "Ability1", 0, null, ability1, setup.PacketStack);
                                AddExtraStatus(setup.Attacker, setup.AttackerMap, "Ability2", 0, null, ability2, setup.PacketStack);
                                AddExtraStatus(setup.Attacker, setup.AttackerMap, "Ability3", 0, null, ability3, setup.PacketStack);

                                RemoveBattleTag(setup.BattleTags, "RolePlay");

                            } else {

                            }

                        }
                        break;
                    case 179: {//skill swap
                            if (GetBattleTagArg(setup.BattleTags, "SkillSwap", 1) != null) {

                                AddExtraStatus(setup.Attacker, setup.AttackerMap, "Ability1", 0, null, GetBattleTagArg(setup.BattleTags, "SkillSwap", 1), setup.PacketStack);
                                AddExtraStatus(setup.Attacker, setup.AttackerMap, "Ability2", 0, null, GetBattleTagArg(setup.BattleTags, "SkillSwap", 2), setup.PacketStack);
                                AddExtraStatus(setup.Attacker, setup.AttackerMap, "Ability3", 0, null, GetBattleTagArg(setup.BattleTags, "SkillSwap", 3), setup.PacketStack);

                                RemoveBattleTag(setup.BattleTags, "SkillSwap");

                            } else {

                            }
                        }
                        break;
                    case 180: {//camouflage
                            int x = setup.Attacker.X;
                            int y = setup.Attacker.Y;
                            MoveInDirection(setup.Attacker.Direction, 1, ref x, ref y);
                            if (x >= 0 && x <= setup.AttackerMap.MaxX && y >= 0 && y <= setup.AttackerMap.MaxY) {
                                Tile tile = setup.AttackerMap.Tile[x, y];
                                bool type1Changed = false;
                                if (tile.Data2 > 0) {
                                    if (tile.Type == Enums.TileType.Walkable || tile.Type == Enums.TileType.Hallway) {
                                        //set to first type
                                        AddExtraStatus(setup.Attacker, setup.AttackerMap, "Type1", tile.Data2, null, "", setup.PacketStack);
                                        type1Changed = true;
                                    }
                                } else if (tile.Type == Enums.TileType.MobileBlock) {
                                    Enums.PokemonType terrainType = GetMobileBlockTerrainType(tile.Data1);
                                    if (terrainType != Enums.PokemonType.None) {
                                        AddExtraStatus(setup.Attacker, setup.AttackerMap, "Type1", (int)terrainType, null, "", setup.PacketStack);
                                        type1Changed = true;
                                    }
                                }

                                if (tile.String2.IsNumeric() && tile.String2.ToInt() > 0) {
                                    //set to second type
                                    if (type1Changed == true) {
                                        AddExtraStatus(setup.Attacker, setup.AttackerMap, "Type2", tile.String2.ToInt(), null, "", setup.PacketStack);
                                    } else {
                                        AddExtraStatus(setup.Attacker, setup.AttackerMap, "Type1", tile.String2.ToInt(), null, "", setup.PacketStack);
                                        AddExtraStatus(setup.Attacker, setup.AttackerMap, "Type2", 0, null, "", setup.PacketStack);
                                    }
                                }
                            }
                        }
                        break;
                    case 181: {//Secret Power
                            if (setup.Hit && (!setup.KnockedOut || setup.Defender.HP > 0)) {

                                Tile tile = setup.DefenderMap.Tile[setup.Defender.X, setup.Defender.Y];

                                int type1 = 0;
                                int type2 = 0;
                                if (tile.Data2 > 0) {
                                    if (tile.Type == Enums.TileType.Walkable || tile.Type == Enums.TileType.Hallway) {
                                        //set to first type
                                        type1 = tile.Data2;
                                    }
                                } else if (tile.Type == Enums.TileType.MobileBlock) {
                                    Enums.PokemonType terrainType = GetMobileBlockTerrainType(tile.Data1);
                                    if (terrainType != Enums.PokemonType.None) {
                                        type1 = (int)terrainType;
                                    }
                                }

                                if (tile.String2.IsNumeric() && tile.String2.ToInt() > 0) {
                                    //set to second type
                                    if (type1 != 0) {
                                        type2 = tile.String2.ToInt();
                                    } else {
                                        type1 = tile.String2.ToInt();
                                    }
                                }
                                if (Server.Math.Rand(0, 100) < setup.Move.AdditionalEffectData2) {
                                    switch ((Enums.PokemonType)type1) {
                                        case Enums.PokemonType.Bug: {
                                                ChangeSpAtkBuff(setup.Defender, setup.Attacker, setup.DefenderMap, -1, setup.PacketStack);
                                            }
                                            break;
                                        case Enums.PokemonType.Dark: {
                                                //blinded
                                                AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Blind", Server.Math.Rand(15, 20), null, "", setup.PacketStack, true);
                                            }
                                            break;
                                        case Enums.PokemonType.Dragon: {
                                                Flinch(setup.Defender, setup.Attacker, setup.DefenderMap, setup.PacketStack);
                                            }
                                            break;
                                        case Enums.PokemonType.Electric: {
                                                SetStatusAilment(setup.Defender, setup.Attacker, setup.DefenderMap, Enums.StatusAilment.Paralyze, 1, setup.PacketStack);
                                            }
                                            break;
                                        case Enums.PokemonType.Fighting: {
                                                ChangeDefenseBuff(setup.Defender, setup.Attacker, setup.DefenderMap, -1, setup.PacketStack);
                                            }
                                            break;
                                        case Enums.PokemonType.Fire: {
                                                SetStatusAilment(setup.Defender, setup.Attacker, setup.DefenderMap, Enums.StatusAilment.Burn, 1, setup.PacketStack);
                                            }
                                            break;
                                        case Enums.PokemonType.Flying: {
                                                //Confuse(setup.Defender, setup.Attacker, setup.DefenderMap, Server.Math.Rand(5, 11), setup.PacketStack);
                                                AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Confusion", Server.Math.Rand(5, 11), null, "", setup.PacketStack, true);
                                            }
                                            break;
                                        case Enums.PokemonType.Ghost: {
                                                AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "MoveSeal:" + Server.Math.Rand(0, 4), 0, null, "", setup.PacketStack, true);
                                            }
                                            break;
                                        case Enums.PokemonType.Grass: {
                                                AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Immobilize", Server.Math.Rand(5, 8), null, "", setup.PacketStack, true);
                                            }
                                            break;
                                        case Enums.PokemonType.Ground: {
                                                ChangeAccuracyBuff(setup.Defender, setup.Attacker, setup.DefenderMap, -1, setup.PacketStack);
                                            }
                                            break;
                                        case Enums.PokemonType.Ice: {
                                                SetStatusAilment(setup.Defender, setup.Attacker, setup.DefenderMap, Enums.StatusAilment.Freeze, 1, setup.PacketStack);
                                            }
                                            break;
                                        case Enums.PokemonType.Normal: {
                                                SetStatusAilment(setup.Defender, setup.Attacker, setup.DefenderMap, Enums.StatusAilment.Sleep, Server.Math.Rand(3, 9), setup.PacketStack);
                                            }
                                            break;
                                        case Enums.PokemonType.Poison: {
                                                SetStatusAilment(setup.Defender, setup.Attacker, setup.DefenderMap, Enums.StatusAilment.Poison, 1, setup.PacketStack);
                                            }
                                            break;
                                        case Enums.PokemonType.Psychic: {
                                                ChangeSpDefBuff(setup.Defender, setup.Attacker, setup.DefenderMap, -1, setup.PacketStack);
                                            }
                                            break;
                                        case Enums.PokemonType.Rock: {
                                                ChangeSpeedBuff(setup.Defender, setup.Attacker, setup.DefenderMap, -1, setup.PacketStack);
                                            }
                                            break;
                                        case Enums.PokemonType.Steel: {
                                                ChangeAttackBuff(setup.Defender, setup.Attacker, setup.DefenderMap, -1, setup.PacketStack);
                                            }
                                            break;
                                        case Enums.PokemonType.Water: {
                                                ChangeEvasionBuff(setup.Defender, setup.Attacker, setup.DefenderMap, -1, setup.PacketStack);
                                            }
                                            break;
                                    }
                                }

                                if (Server.Math.Rand(0, 100) < setup.Move.AdditionalEffectData2) {
                                    switch ((Enums.PokemonType)type2) {
                                        case Enums.PokemonType.Bug: {
                                                ChangeSpAtkBuff(setup.Defender, setup.Attacker, setup.DefenderMap, -1, setup.PacketStack);
                                            }
                                            break;
                                        case Enums.PokemonType.Dark: {
                                                //blinded
                                                AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Blind", Server.Math.Rand(15, 20), null, "", setup.PacketStack, true);
                                            }
                                            break;
                                        case Enums.PokemonType.Dragon: {
                                                Flinch(setup.Defender, setup.Attacker, setup.DefenderMap, setup.PacketStack);
                                            }
                                            break;
                                        case Enums.PokemonType.Electric: {
                                                SetStatusAilment(setup.Defender, setup.Attacker, setup.DefenderMap, Enums.StatusAilment.Paralyze, 1, setup.PacketStack);
                                            }
                                            break;
                                        case Enums.PokemonType.Fighting: {
                                                ChangeDefenseBuff(setup.Defender, setup.Attacker, setup.DefenderMap, -1, setup.PacketStack);
                                            }
                                            break;
                                        case Enums.PokemonType.Fire: {
                                                SetStatusAilment(setup.Defender, setup.Attacker, setup.DefenderMap, Enums.StatusAilment.Burn, 1, setup.PacketStack);
                                            }
                                            break;
                                        case Enums.PokemonType.Flying: {
                                                //Confuse(setup.Defender, setup.Attacker, setup.DefenderMap, Server.Math.Rand(5, 11), setup.PacketStack);
                                                AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Confusion", Server.Math.Rand(5, 11), null, "", setup.PacketStack, true);
                                            }
                                            break;
                                        case Enums.PokemonType.Ghost: {
                                                AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "MoveSeal:" + Server.Math.Rand(0, 4), 0, null, "", setup.PacketStack, true);
                                            }
                                            break;
                                        case Enums.PokemonType.Grass: {
                                                AddExtraStatus(setup.Defender, setup.Attacker, setup.DefenderMap, "Immobilize", Server.Math.Rand(5, 8), null, "", setup.PacketStack, true);
                                            }
                                            break;
                                        case Enums.PokemonType.Ground: {
                                                ChangeAccuracyBuff(setup.Defender, setup.Attacker, setup.DefenderMap, -1, setup.PacketStack);
                                            }
                                            break;
                                        case Enums.PokemonType.Ice: {
                                                SetStatusAilment(setup.Defender, setup.Attacker, setup.DefenderMap, Enums.StatusAilment.Freeze, 1, setup.PacketStack);
                                            }
                                            break;
                                        case Enums.PokemonType.Normal: {
                                                SetStatusAilment(setup.Defender, setup.Attacker, setup.DefenderMap, Enums.StatusAilment.Sleep, Server.Math.Rand(3, 9), setup.PacketStack);
                                            }
                                            break;
                                        case Enums.PokemonType.Poison: {
                                                SetStatusAilment(setup.Defender, setup.Attacker, setup.DefenderMap, Enums.StatusAilment.Poison, 1, setup.PacketStack);
                                            }
                                            break;
                                        case Enums.PokemonType.Psychic: {
                                                ChangeSpDefBuff(setup.Defender, setup.Attacker, setup.DefenderMap, -1, setup.PacketStack);
                                            }
                                            break;
                                        case Enums.PokemonType.Rock: {
                                                ChangeSpeedBuff(setup.Defender, setup.Attacker, setup.DefenderMap, -1, setup.PacketStack);
                                            }
                                            break;
                                        case Enums.PokemonType.Steel: {
                                                ChangeAttackBuff(setup.Defender, setup.Attacker, setup.DefenderMap, -1, setup.PacketStack);
                                            }
                                            break;
                                        case Enums.PokemonType.Water: {
                                                ChangeEvasionBuff(setup.Defender, setup.Attacker, setup.DefenderMap, -1, setup.PacketStack);
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                        break;
                    case 192: {//flash lighting up the room
                    		if (setup.AttackerMap.Tile[setup.Attacker.X, setup.Attacker.Y].Type == Enums.TileType.Arena || setup.AttackerMap.Moral == Enums.MapMoral.None || setup.AttackerMap.Moral == Enums.MapMoral.NoPenalty) {
                               	AddMapStatus(setup.AttackerMap, "Flash", setup.Move.AdditionalEffectData2, "", -1, setup.PacketStack);
                            }
                    
                    	}
                    	break;
                    case 193: {//halve the user's HP
                    		if (setup.BattleTags.Contains("HitSomething")) {
                               	int fraction = setup.Attacker.MaxHP / 2;
                               	DamageCharacter(setup.Attacker, setup.AttackerMap, fraction, Enums.KillType.Other, setup.PacketStack, false);
                    		}
                    	}
                    	break;
                    case 196: { //jump kick/hi jump kick recoil
                    		if (!setup.BattleTags.Contains("HitSomething")) {
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " kept going and crashed!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                                int recoil = setup.Attacker.MaxHP / 4;
                    			if (recoil < 0) recoil = 0;
                                if (recoil >= setup.Attacker.HP) {
                                    DamageCharacter(setup.Attacker, setup.AttackerMap, recoil, Enums.KillType.Other, setup.PacketStack, true);
                                    setup.Cancel = true;
                                } else {
                                    DamageCharacter(setup.Attacker, setup.AttackerMap, recoil, Enums.KillType.Other, setup.PacketStack, true);
                                }
                            }
                    	}
                    	break;
                    case 198: { //smack down
                    		if (setup.Hit && (!setup.KnockedOut || setup.Defender.HP > 0)) {
                                AddExtraStatus(setup.Defender, setup.DefenderMap, "Grounded", 0, null, "", setup.PacketStack);
                            }
                    	}
                    	break;
                    case 199: {//fake out flinch
                            if (setup.Hit && (!setup.KnockedOut || setup.Defender.HP > 0)) {
                                Flinch(setup.Defender, setup.Attacker, setup.DefenderMap, setup.PacketStack);
                            }
                        }
                        break;
                    case 202: {//struggle recoil
                            if (setup.Hit) {
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " was hit with recoil!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                                int recoil = setup.Attacker.MaxHP / 8;
                    			if (recoil < 0) recoil = 0;
                                if (recoil >= setup.Attacker.HP) {
                                    DamageCharacter(setup.Attacker, setup.AttackerMap, recoil, Enums.KillType.Other, setup.PacketStack, true);
                                    setup.Cancel = true;
                                } else {
                                    DamageCharacter(setup.Attacker, setup.AttackerMap, recoil, Enums.KillType.Other, setup.PacketStack, true);
                                }
                            }
                        }
                        break;
                    case 205: {//pay day payout
                            if (setup.Hit && setup.KnockedOut && setup.Defender.HP <= 0) {
                                if (setup.Defender.CharacterType == Enums.CharacterType.MapNpc && setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
	                                int payout = setup.Attacker.Level + setup.Defender.Level;
	                                setup.DefenderMap.SpawnItem(1, payout, false, false, "", setup.Defender.X, setup.Defender.Y, ((Recruit)setup.Attacker).Owner);
                                }
                            }
                        }
                        break;
                    case 223: {//surf effect
                    		AddExtraStatus(setup.Attacker, setup.AttackerMap, "Slip", 0, null, "", setup.PacketStack);
                    	}
                    	break;

                }
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: MoveAdditionalEffect: " + setup.Move.AdditionalEffectData1 + " " + setup.Defender.Name, Text.Black);
                Messenger.AdminMsg(ex.ToString(), Text.Black);
            }
        }

        public static void AfterMoveHits(BattleSetup setup) {
        	int point = 0;
            try {
                ExtraStatus status;
                //if (setup.Defender.CharacterType == Enums.CharacterType.Recruit) {
                //	if (Ranks.IsAllowed(((Recruit)setup.Defender).Owner, Enums.Rank.Moniter)) {
                //		Messenger.PlayerMsg(((Recruit)setup.Defender).Owner, "HP: " + setup.Attacker.HP, Text.BrightRed);
                //	}
                //}
                if (setup.Hit && setup.Defender != null) {
                    
					for (int i = 1; i < 6; i++) {
						if (Server.Math.Rand(0, 5) == 0) {
							if (HasActiveBagItem(setup.Defender, 10, 0, i)) {
								if (i == 5) {
			                        SetStatusAilment(setup.Attacker, setup.AttackerMap, (Enums.StatusAilment)i, Server.Math.Rand(2, 4), setup.PacketStack);
			                    } else {
			                        SetStatusAilment(setup.Attacker, setup.AttackerMap, (Enums.StatusAilment)i, 1, setup.PacketStack);
			                    }
							}
						}
					}
					
					for (int i = 1; i < 11; i++) {
						if (Server.Math.Rand(0, 5) == 0) {
							if (HasActiveBagItem(setup.Defender, 11, 0, i)) {
								switch (i) {
									case 1:
										AddExtraStatus(setup.Attacker, setup.AttackerMap, "Immobilize", 5, null, "", setup.PacketStack);
										break;
									case 2:
										AddExtraStatus(setup.Attacker, setup.AttackerMap, "Confusion", 10, null, "", setup.PacketStack);
										break;
									case 3:
										if (setup.moveSlot > -1 && setup.moveSlot < 4) {
						                    AddExtraStatus(setup.Attacker, setup.AttackerMap, "MoveSeal:" + setup.moveSlot, 0, null, "", setup.PacketStack);
						                }
										break;
									case 4:
										ExtraStatus exStatus = setup.Attacker.VolatileStatus.GetStatus("MovementSpeed");
		                                if (exStatus != null) {
		                                    AddExtraStatus(setup.Attacker, setup.AttackerMap, "MovementSpeed", exStatus.Counter - 1, null, "", setup.PacketStack);
			                            } else {
		                                    AddExtraStatus(setup.Attacker, setup.AttackerMap, "MovementSpeed", -1, null, "", setup.PacketStack);
			                            }
										break;
									case 5:
										AddExtraStatus(setup.Attacker, setup.AttackerMap, "Blind", 15, null, "", setup.PacketStack);
										break;
									case 6:
										Flinch(setup.Attacker, null, setup.AttackerMap, setup.PacketStack);
										break;
									case 7:
										AddExtraStatus(setup.Attacker, setup.AttackerMap, "PerishCount", 4, null, "", setup.PacketStack);
										break;
									case 8:
										if (setup.Attacker.Sex == Enums.Sex.Genderless && setup.Defender.Sex == Enums.Sex.Genderless
						                    || setup.Attacker.Sex == Enums.Sex.Male && setup.Defender.Sex == Enums.Sex.Female
						                    || setup.Defender.Sex == Enums.Sex.Male && setup.Attacker.Sex == Enums.Sex.Female) {
					                        AddExtraStatus(setup.Attacker, setup.AttackerMap, "Attract", 10, null, "", setup.PacketStack);
						                }
										break;
									case 9:
										AddExtraStatus(setup.Defender, setup.DefenderMap, "Invisible", 10, null, "", setup.PacketStack);
										break;
									case 10:
										AddExtraStatus(setup.Attacker, setup.AttackerMap, "Nightmare", 0, null, "", setup.PacketStack);
										break;
								}
							}
						}
					}
					
                    //move hit consequences like static, shadow tag, etc.
                    CheckAfterMoveHitAbility(setup);

                    if (setup.Defender.HP <= 0 && setup.KnockedOut == false) {
                        setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " barely endured the hit!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                    }



                    if (setup.Damage > 0 && setup.Hit) {
                        //Ruby/Sapphire Plate
                        if (setup.Defender.HasActiveItem(159) && setup.Move.Element != Enums.PokemonType.None && setup.Move.MoveCategory == Enums.MoveCategory.Physical
                            || setup.Defender.HasActiveItem(178) && setup.Move.Element != Enums.PokemonType.None && setup.Move.MoveCategory == Enums.MoveCategory.Special) {
                            setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg("Its power rebounded!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                            int recoil = (GetBattleTagArg(setup.BattleTags, "PrevHP", 1).ToInt() - setup.Defender.HP) / 2;
                    		if (recoil < 0) recoil = 0;
                            if (recoil >= setup.Attacker.HP) {
                                DamageCharacter(setup.Attacker, setup.AttackerMap, recoil, Enums.KillType.Other, setup.PacketStack, true);
                                setup.Cancel = true;
                            } else {
                                DamageCharacter(setup.Attacker, setup.AttackerMap, recoil, Enums.KillType.Other, setup.PacketStack, true);
                            }
                        }
                        
                        //Jaboca/Rowap
                        status = setup.Defender.VolatileStatus.GetStatus("AttackReturn");
                        if (status != null) {
	                        if (setup.Move.MoveCategory == Enums.MoveCategory.Physical && status.Counter == 0
	                            || setup.Move.MoveCategory == Enums.MoveCategory.Special && status.Counter == 1) {
	                            setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg("Its power rebounded to everyone!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                            int dmg = (GetBattleTagArg(setup.BattleTags, "PrevHP", 1).ToInt() - setup.Defender.HP) / 2;
                            
                            TargetCollection targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 6, setup.DefenderMap, setup.Defender, setup.Defender.X, setup.Defender.Y, Enums.Direction.Up, true, false, false);

                            for (int i = 0; i < targets.Foes.Count; i++) {
                                if (dmg >= targets.Foes[i].HP) {
                                    DamageCharacter(targets.Foes[i], MapManager.RetrieveActiveMap(targets.Foes[i].MapID), dmg, Enums.KillType.Other, setup.PacketStack, true);
                                    setup.Cancel = true;
                                } else {
                                    DamageCharacter(targets.Foes[i], MapManager.RetrieveActiveMap(targets.Foes[i].MapID), dmg, Enums.KillType.Other, setup.PacketStack, true);
                                }
                            }
	                        }
                        }
                        
                        //rocky helmet
                        if (setup.Defender.HasActiveItem(482) && MoveProcessor.IsInAreaRange(1, setup.Defender.X, setup.Defender.Y, setup.Attacker.X, setup.Attacker.Y)) {
                            setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " was jurt by the Rocky Helmet!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                            int recoil = (GetBattleTagArg(setup.BattleTags, "PrevHP", 1).ToInt() - setup.Defender.HP) / 4;
                    		if (recoil < 0) recoil = 0;
                            if (recoil >= setup.Attacker.HP) {
                                DamageCharacter(setup.Attacker, setup.AttackerMap, recoil, Enums.KillType.Other, setup.PacketStack, true);
                                setup.Cancel = true;
                            } else {
                                DamageCharacter(setup.Attacker, setup.AttackerMap, recoil, Enums.KillType.Other, setup.PacketStack, true);
                            }
                        }
                        
                        //sticky barb
                        if (MoveProcessor.IsInAreaRange(1, setup.Defender.X, setup.Defender.Y, setup.Attacker.X, setup.Attacker.Y) &&
                        	(setup.Defender.HasActiveItem(134) || setup.Attacker.HasActiveItem(134))) {
                            setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " was hurt by the Sticky Barb!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                            int recoil = setup.Attacker.HP / 16;
                    		if (recoil < 0) recoil = 0;
                            if (recoil >= setup.Attacker.HP) {
                                DamageCharacter(setup.Attacker, setup.AttackerMap, recoil, Enums.KillType.Other, setup.PacketStack, true);
                                setup.Cancel = true;
                            } else {
                                DamageCharacter(setup.Attacker, setup.AttackerMap, recoil, Enums.KillType.Other, setup.PacketStack, true);
                            }
                        }


                        //Type-reflect Plates
                        if (setup.Defender.HasActiveItem(295) && setup.Move.Element == Enums.PokemonType.Rock
                            || setup.Defender.HasActiveItem(296) && setup.Move.Element == Enums.PokemonType.Ground
                            || setup.Defender.HasActiveItem(297) && setup.Move.Element == Enums.PokemonType.Steel
                            || setup.Defender.HasActiveItem(416) && setup.Move.Element == Enums.PokemonType.Ice
                            || setup.Defender.HasActiveItem(417) && setup.Move.Element == Enums.PokemonType.Dragon
                            || setup.Defender.HasActiveItem(418) && setup.Move.Element == Enums.PokemonType.Dark
                            || setup.Defender.HasActiveItem(419) && setup.Move.Element == Enums.PokemonType.Fighting
                            || setup.Defender.HasActiveItem(420) && setup.Move.Element == Enums.PokemonType.Fire
                            || setup.Defender.HasActiveItem(423) && setup.Move.Element == Enums.PokemonType.Bug
                            || setup.Defender.HasActiveItem(424) && setup.Move.Element == Enums.PokemonType.Grass
                            || setup.Defender.HasActiveItem(425) && setup.Move.Element == Enums.PokemonType.Psychic
                            || setup.Defender.HasActiveItem(426) && setup.Move.Element == Enums.PokemonType.Flying
                            || setup.Defender.HasActiveItem(427) && setup.Move.Element == Enums.PokemonType.Water
                            || setup.Defender.HasActiveItem(428) && setup.Move.Element == Enums.PokemonType.Ghost
                            || setup.Defender.HasActiveItem(429) && setup.Move.Element == Enums.PokemonType.Poison
                            || setup.Defender.HasActiveItem(430) && setup.Move.Element == Enums.PokemonType.Electric) {
                            setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg("Its power rebounded!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                            int recoil = (GetBattleTagArg(setup.BattleTags, "PrevHP", 1).ToInt() - setup.Defender.HP);
                    		if (recoil < 0) recoil = 0;
                            if (recoil >= setup.Attacker.HP) {
                                DamageCharacter(setup.Attacker, setup.AttackerMap, recoil, Enums.KillType.Other, setup.PacketStack, true);
                                setup.Cancel = true;
                            } else {
                                DamageCharacter(setup.Attacker, setup.AttackerMap, recoil, Enums.KillType.Other, setup.PacketStack, true);
                            }
                        }

                        //counter/mirror coat
                        if (setup.Move.MoveCategory == Enums.MoveCategory.Physical && setup.Defender.VolatileStatus.GetStatus("Counter") != null
                            || setup.Move.MoveCategory == Enums.MoveCategory.Special && setup.Defender.VolatileStatus.GetStatus("MirrorCoat") != null) {
                            setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg("Its power rebounded!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                            int recoil = (GetBattleTagArg(setup.BattleTags, "PrevHP", 1).ToInt() - setup.Defender.HP) * 2;
                    		if (recoil < 0) recoil = 0;
                            if (recoil >= setup.Attacker.HP) {
                                DamageCharacter(setup.Attacker, setup.AttackerMap, recoil, Enums.KillType.Other, setup.PacketStack, true);
                                setup.Cancel = true;
                            } else {
                                DamageCharacter(setup.Attacker, setup.AttackerMap, recoil, Enums.KillType.Other, setup.PacketStack, true);
                            }
                        }
						
						//metal burst
                        if (setup.Move.MoveCategory != Enums.MoveCategory.Status && setup.Defender.VolatileStatus.GetStatus("MetalBurst") != null) {
                            setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg("Its power rebounded to everyone!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                            int dmg = (GetBattleTagArg(setup.BattleTags, "PrevHP", 1).ToInt() - setup.Defender.HP) * 3 / 2;
                            setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateSoundPacket("magic392.wav"), setup.Defender.X, setup.Defender.Y, 10);
                            for (int i = setup.Defender.X - 4; i <= setup.Defender.X + 4; i++) {
                                for (int j = setup.Defender.Y - 4; j <= setup.Defender.Y + 4; j++) {
                                    if (i < 0 || j < 0 || i > setup.DefenderMap.MaxX || j > setup.DefenderMap.MaxY) {

                                    } else {
                                        setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateSpellAnim(319, i, j));
                                    }
                                }
                            }
                            TargetCollection targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 4, setup.DefenderMap, setup.Defender, setup.Defender.X, setup.Defender.Y, Enums.Direction.Up, true, false, false);

                            for (int i = 0; i < targets.Foes.Count; i++) {
                                if (dmg >= targets.Foes[i].HP) {
                                    DamageCharacter(targets.Foes[i], MapManager.RetrieveActiveMap(targets.Foes[i].MapID), dmg, Enums.KillType.Other, setup.PacketStack, true);
                                    setup.Cancel = true;
                                } else {
                                    DamageCharacter(targets.Foes[i], MapManager.RetrieveActiveMap(targets.Foes[i].MapID), dmg, Enums.KillType.Other, setup.PacketStack, true);
                                }
                            }
                        }
                        
                        

                        //destiny bond
                        if (setup.Defender.VolatileStatus.GetStatus("DestinyBond") != null) {
                            setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " is trying to take its foe with it!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                            int recoil = (GetBattleTagArg(setup.BattleTags, "PrevHP", 1).ToInt() - setup.Defender.HP);
                    		if (recoil < 0) recoil = 0;
                            if (recoil >= setup.Attacker.HP) {
                                DamageCharacter(setup.Attacker, setup.AttackerMap, recoil, Enums.KillType.Other, setup.PacketStack, true);
                                setup.Cancel = true;
                            } else {
                                DamageCharacter(setup.Attacker, setup.AttackerMap, recoil, Enums.KillType.Other, setup.PacketStack, true);
                            }
                        }
                        
                        //rebound
                        if (setup.Defender.VolatileStatus.GetStatus("Rebound") != null) {
                            setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg("Its power rebounded!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                            int recoil = (GetBattleTagArg(setup.BattleTags, "PrevHP", 1).ToInt() - setup.Defender.HP) / 2;
                    		if (recoil < 0) recoil = 0;
                            if (recoil >= setup.Attacker.HP) {
                                DamageCharacter(setup.Attacker, setup.AttackerMap, recoil, Enums.KillType.Other, setup.PacketStack, true);
                                setup.Cancel = true;
                            } else {
                                DamageCharacter(setup.Attacker, setup.AttackerMap, recoil, Enums.KillType.Other, setup.PacketStack, true);
                            }
                        }
                        
                        //mini counter item
                        if (HasActiveBagItem(setup.Attacker, 8, 0, 0)) {
                        	setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg("Its power rebounded!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                            int recoil = (GetBattleTagArg(setup.BattleTags, "PrevHP", 1).ToInt() - setup.Defender.HP) / 4;
                    		if (recoil < 0) recoil = 0;
                            if (recoil >= setup.Attacker.HP) {
                                DamageCharacter(setup.Attacker, setup.AttackerMap, recoil, Enums.KillType.Other, setup.PacketStack, true);
                                setup.Cancel = true;
                            } else {
                                DamageCharacter(setup.Attacker, setup.AttackerMap, recoil, Enums.KillType.Other, setup.PacketStack, true);
                            }
                        }

                        //grudge
                        status = setup.Attacker.VolatileStatus.GetStatus("LastUsedMoveSlot");
                        if (setup.KnockedOut && setup.Defender.HP <= 0 && setup.Defender.VolatileStatus.GetStatus("Grudge") != null && status != null) {

                            setup.Attacker.Moves[status.Counter].CurrentPP = 0;
                            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                                PacketBuilder.AppendPlayerMoves(((Recruit)setup.Attacker).Owner, setup.PacketStack);
                            }
                            setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg("The grudge took effect!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                        }


                        if (setup.KnockedOut && setup.Defender.HP <= 0
                         && HasAbility(setup.Attacker, "Moxie") && Server.Math.Rand(0, 3) == 0) {
                            ChangeAttackBuff(setup.Attacker, setup.AttackerMap, 1, setup.PacketStack);
                        }
                    }


                    if (setup.Defender.StatusAilment == Enums.StatusAilment.Freeze && setup.Move.Element == Enums.PokemonType.Fire) {
                        SetStatusAilment(setup.Defender, setup.DefenderMap, Enums.StatusAilment.OK, 0, setup.PacketStack);
                    }

                    //Bide
                    status = setup.Defender.VolatileStatus.GetStatus("Bide");
                    if (status != null) {
                        status.Counter += setup.Damage;
                        setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " is storing energy!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                    }
                    
                    if (setup.Defender.VolatileStatus.GetStatus("Avalanche") != null) {
                       	BattleSetup counterSetup = new BattleSetup();
                       	if (setup.Damage > 0) {
                       		counterSetup.BattleTags.Add("Avalanche");
                       	}
                        	
                       	counterSetup.Attacker = setup.Defender;
                        counterSetup.moveSlot = -1;

                        BattleProcessor.HandleAttack(counterSetup);

                        BattleProcessor.FinalizeAction(counterSetup, setup.PacketStack);
                    }
                    
                    if (setup.Defender.VolatileStatus.GetStatus("VitalThrow") != null) {
                       	BattleSetup counterSetup = new BattleSetup();
                       	counterSetup.BattleTags.Add("VitalThrow");
                        	
                       	counterSetup.Attacker = setup.Defender;
                        counterSetup.moveSlot = -1;

                        BattleProcessor.HandleAttack(counterSetup);

                        BattleProcessor.FinalizeAction(counterSetup, setup.PacketStack);
                    }

                    //Focus Punch
                    if (setup.Damage > 0 && setup.Defender.VolatileStatus.GetStatus("FocusPunch") != null) {
                        RemoveExtraStatus(setup.Defender, setup.DefenderMap, "FocusPunch", setup.PacketStack);
                        if (setup.Attacker.PauseTimer.Tick < Core.GetTickCount().Tick) {
                            setup.Attacker.PauseTimer = new TickCount(Core.GetTickCount().Tick);
                        }
                        setup.Attacker.PauseTimer = new TickCount(setup.Attacker.PauseTimer.Tick + 1500);
                    }

                    //joy ribbon
                    if (GetBattleTagArg(setup.BattleTags, "PrevHP", 1) != null) {
                    	if (setup.Defender.HasActiveItem(139) && setup.Defender.CharacterType == Enums.CharacterType.Recruit && (!setup.KnockedOut || setup.Defender.HP > 0) && setup.Damage > 0
                    		&& setup.Attacker != null && setup.Attacker.CharacterType != Enums.CharacterType.Recruit) {
                    		int damageChange = (GetBattleTagArg(setup.BattleTags, "PrevHP", 1).ToInt() - setup.Defender.HP) / 2;
                    		if (damageChange < 0) damageChange = 0;
                    		
	                        ((Recruit)setup.Defender).Exp += (ulong)damageChange / 2;
	                        setup.PacketStack.AddPacket(((Recruit)setup.Defender).Owner, PacketBuilder.CreateBattleMsg("You gained " + damageChange + " experience.", Text.BrightCyan));
	                        BattleProcessor.CheckPlayerLevelUp(setup.PacketStack, ((Recruit)setup.Defender).Owner);
	                        
	                        
                        }
                    }
                    
                    if (setup.Attacker.HasActiveItem(174) && setup.Damage > 0) {
                    
                        int heal = setup.Damage / 20;
                        if (heal > 0) {
                        	HealCharacter(setup.Attacker, setup.AttackerMap, heal, setup.PacketStack);
                        }
                    }
					//setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + "/"+setup.Hit+"" + setup.moveSlot + "!", Text.WhiteSmoke), setup.Defender.X, setup.Defender.Y, 10);
					if (setup.Hit) {
	                    if (setup.Damage > 0) {
	                    	point = 1;
	                        AddExtraStatus(setup.Defender, setup.DefenderMap, "LastHitBy", 1, setup.Attacker, "", setup.PacketStack);
	                    } else {
	                    	point = 2;
	                        AddExtraStatus(setup.Defender, setup.DefenderMap, "LastHitBy", 0, setup.Attacker, "", setup.PacketStack);
	                    }
	                    point = 3;
	                    if (setup.moveSlot >= 0) {
	                    	//setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " Hit by " + setup.moveIndex + "!", Text.WhiteSmoke), setup.Defender.X, setup.Defender.Y, 10);
		                    AddExtraStatus(setup.Defender, setup.DefenderMap, "LastMoveHitBy", setup.moveIndex, null, "", setup.PacketStack);
		                    if (setup.Attacker != setup.Defender) {
		                    	point = 4;
		                    	AddExtraStatus(setup.Defender, setup.DefenderMap, "LastMoveHitByOther", setup.moveIndex, null, "", setup.PacketStack);
		                    }
	                    }
                    }


                    if (setup.Hit && setup.Damage > 0) {
                    	if (setup.Defender.VolatileStatus.GetStatus("Illusion") != null) {
                        	RemoveExtraStatus(setup.Defender, setup.DefenderMap, "Illusion", setup.PacketStack);
                        }
                        if (setup.Defender.HasActiveItem(82)) {
                        	setup.Defender.TakeHeldItem(1);
                        	setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + "'s balloon popped!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                        	setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateSpellAnim(503, setup.Defender.X, setup.Defender.Y));
                        }
                        
                        if (setup.Defender.HasActiveItem(711) || setup.Defender.HasActiveItem(715) ||
                        	setup.Defender.HasActiveItem(716) || setup.Defender.HasActiveItem(717)) {
                        	setup.Defender.TakeHeldItem(1);
                        	setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateSpellAnim(540, setup.Defender.X, setup.Defender.Y));
                        	setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateSoundPacket("Magic6.wav"), setup.Defender.X, setup.Defender.Y, 50);
                        	setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg("Oh no!  " + setup.Defender.Name + "'s artifact shattered!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 50);
                        }
                    }
                    
                    if (setup.Hit && (setup.Defender.HP > 0 || !setup.KnockedOut) &&
                    	(setup.Defender.HasActiveItem(807) && setup.Move.Element == Enums.PokemonType.Water ||
                    	setup.Defender.HasActiveItem(808) && setup.Move.Element == Enums.PokemonType.Electric)) {
                    	setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + "'s item absorbed the attack!", Text.WhiteSmoke), setup.Defender.X, setup.Defender.Y, 10);
                    	ChangeAttackBuff(setup.Defender, null, setup.DefenderMap, 1, setup.PacketStack);
                        ChangeDefenseBuff(setup.Defender, null, setup.DefenderMap, 1, setup.PacketStack);
                        ChangeSpAtkBuff(setup.Defender, null, setup.DefenderMap, 1, setup.PacketStack);
                        ChangeSpDefBuff(setup.Defender, null, setup.DefenderMap, 1, setup.PacketStack);
                        ChangeSpeedBuff(setup.Defender, null, setup.DefenderMap, 1, setup.PacketStack);
                        ChangeAccuracyBuff(setup.Defender, null, setup.DefenderMap, 1, setup.PacketStack);
                        ChangeEvasionBuff(setup.Defender, null, setup.DefenderMap, 1, setup.PacketStack);
                    	setup.Defender.TakeHeldItem(1);
                    }
                    
                    if (setup.Defender.HasActiveItem(120)) {
                    	if (setup.Defender.HeldItem.Tag.ToInt() <= 0) {
                    		setup.Defender.TakeHeldItem(1);
                    		setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + "'s Pokédoll turned into a pile of fluff...", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                    	setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateSpellAnim(503, setup.Defender.X, setup.Defender.Y));
                    	}
                    }

                } else {

                }


                if (setup.Defender != null) {

                    //effects of moves that wear off after a certain amount of hits
                    if (GetBattleTagArg(setup.BattleTags, "Snatch", 0) == null) {
                        
                        if (setup.Attacker != setup.Defender) {
                        	//statuses that kick in when the attacker is not the defender
                        	
                            status = setup.Defender.VolatileStatus.GetStatus("Wish");
                            if (status != null) {
                                status.Counter--;
                                if (status.Counter <= 0) {
                                    RemoveExtraStatus(setup.Defender, setup.DefenderMap, "Wish", setup.PacketStack);
                                    setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + "'s wish came true!", Text.BrightGreen), setup.Defender.X, setup.Defender.Y, 10);
                                    setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateSoundPacket("magic11.wav"));
                                    HealCharacter(setup.Defender, setup.DefenderMap, setup.Defender.MaxHP / 2, setup.PacketStack);
                                }
                            }
                            status = setup.Defender.VolatileStatus.GetStatus("WideGuard");
                            if (status != null) {
                                status.Counter--;
                                if (status.Counter <= 0) {
                                    RemoveExtraStatus(setup.Defender, setup.DefenderMap, "WideGuard", setup.PacketStack);
                                }
                            }
                            status = setup.Defender.VolatileStatus.GetStatus("QuickGuard");
                            if (status != null) {
                                status.Counter--;
                                if (status.Counter <= 0) {
                                    RemoveExtraStatus(setup.Defender, setup.DefenderMap, "QuickGuard", setup.PacketStack);
                                }
                            }

                            status = setup.Defender.VolatileStatus.GetStatus("Protect");
                            if (status != null) {
                                status.Counter--;
                                if (status.Counter <= 0) {
                                    RemoveExtraStatus(setup.Defender, setup.DefenderMap, "Protect", setup.PacketStack);
                                }
                            }

                            status = setup.Defender.VolatileStatus.GetStatus("Endure");
                            if (status != null) {
                                status.Counter--;
                                if (status.Counter <= 0) {
                                    RemoveExtraStatus(setup.Defender, setup.DefenderMap, "Endure", setup.PacketStack);
                                }
                            }

                            status = setup.Defender.VolatileStatus.GetStatus("Counter");
                            if (status != null) {
                                status.Counter--;
                                if (status.Counter <= 0) {
                                    RemoveExtraStatus(setup.Defender, setup.DefenderMap, "Counter", setup.PacketStack);
                                }
                            }
                            status = setup.Defender.VolatileStatus.GetStatus("MirrorCoat");
                            if (status != null) {
                                status.Counter--;
                                if (status.Counter <= 0) {
                                    RemoveExtraStatus(setup.Defender, setup.DefenderMap, "MirrorCoat", setup.PacketStack);
                                }
                            }

                            status = setup.Defender.VolatileStatus.GetStatus("MetalBurst");
                            if (status != null) {
                                status.Counter--;
                                if (status.Counter <= 0) {
                                    RemoveExtraStatus(setup.Defender, setup.DefenderMap, "MetalBurst", setup.PacketStack);
                                }
                            }

                            status = setup.Defender.VolatileStatus.GetStatus("DestinyBond");
                            if (status != null) {
                                status.Counter--;
                                if (status.Counter <= 0) {
                                    RemoveExtraStatus(setup.Defender, setup.DefenderMap, "DestinyBond", setup.PacketStack);
                                }
                            }
							
							//rebound
                            status = setup.Defender.VolatileStatus.GetStatus("Rebound");
                            if (status != null) {
                                status.Counter--;
                                if (status.Counter <= 0) {
                                    RemoveExtraStatus(setup.Defender, setup.DefenderMap, "Rebound", setup.PacketStack);
                                }
                            }

                            status = setup.Defender.VolatileStatus.GetStatus("Rage");
                            if (status != null) {
                                if (setup.Hit && setup.Move.MoveCategory != Enums.MoveCategory.Status) {
                                    ChangeAttackBuff(setup.Defender, setup.DefenderMap, 1, setup.PacketStack);
                                }
                                status.Counter--;
                                if (status.Counter <= 0) {
                                    RemoveExtraStatus(setup.Defender, setup.DefenderMap, "Rage", setup.PacketStack);
                                }
                            }
                            
                            //conversion 2
			                status = setup.Defender.VolatileStatus.GetStatus("Conversion2");
			                if (status != null) {
			                	List<Enums.PokemonType> elements = new List<Enums.PokemonType>();
			                	Enums.PokemonType element = Enums.PokemonType.None;
			                	for (int i = 0; i < 19; i++) {
			                		int effectiveness = DamageCalculator.CalculateTypeMatchup(setup.Move.Element, (Enums.PokemonType)i);
			                		if (effectiveness == 0) {
			                			element = (Enums.PokemonType)i;
			                			break;
			                		} else if (effectiveness < 3) {
			                			elements.Add((Enums.PokemonType)i);
			                		}
			                	}
			                	//Messenger.AdminMsg(element.ToString(), Text.WhiteSmoke);
			                	if (element == Enums.PokemonType.None && elements.Count > 0) {
			                		element = elements[Server.Math.Rand(0, elements.Count)];
			                	}
			                	
			                	if (element != Enums.PokemonType.None) {
				                	if (setup.Defender.Type1 != element) {
				                		AddExtraStatus(setup.Defender, setup.DefenderMap, "Type1", (int)element, null, "", setup.PacketStack);
				                	}
				                	if (setup.Defender.Type2 != Enums.PokemonType.None) {
				                		AddExtraStatus(setup.Defender, setup.DefenderMap, "Type2", 0, null, "", setup.PacketStack, false);
				                	}
			                	}
			                }
                        }
                        if (MoveProcessor.GetMatchupWith(setup.Attacker, setup.Defender) == Enums.CharacterMatchup.Foe) {
                        //statuses that kick in when the attacker is explicitly on a different side
                            status = setup.Defender.VolatileStatus.GetStatus("Safeguard");
                            if (status != null) {
                                status.Counter--;
                                if (status.Counter <= 0) {
                                    RemoveExtraStatus(setup.Defender, setup.DefenderMap, "Safeguard", setup.PacketStack);
                                }
                            }

                            status = setup.Defender.VolatileStatus.GetStatus("Status Guard");
                            if (status != null) {
                                status.Counter--;
                                if (status.Counter <= 0) {
                                    RemoveExtraStatus(setup.Defender, setup.DefenderMap, "Status Guard", setup.PacketStack);
                                }
                            }


                            status = setup.Defender.VolatileStatus.GetStatus("Mist");
                            if (status != null) {
                                status.Counter--;
                                if (status.Counter <= 0) {
                                    RemoveExtraStatus(setup.Defender, setup.DefenderMap, "Mist", setup.PacketStack);
                                }
                            }


                            status = setup.Defender.VolatileStatus.GetStatus("LuckyChant");
                            if (status != null) {
                                status.Counter--;
                                if (status.Counter <= 0) {
                                    RemoveExtraStatus(setup.Defender, setup.DefenderMap, "LuckyChant", setup.PacketStack);
                                }
                            }



                            status = setup.Defender.VolatileStatus.GetStatus("Reflect");
                            if (status != null) {
                                status.Counter--;
                                if (status.Counter <= 0) {
                                    RemoveExtraStatus(setup.Defender, setup.DefenderMap, "Reflect", setup.PacketStack);
                                }
                            }



                            status = setup.Defender.VolatileStatus.GetStatus("LightScreen");
                            if (status != null) {
                                status.Counter--;
                                if (status.Counter <= 0) {
                                    RemoveExtraStatus(setup.Defender, setup.DefenderMap, "LightScreen", setup.PacketStack);
                                }
                            }
                        }
                        
                        
                    }

                    status = setup.Defender.VolatileStatus.GetStatus("Substitute");
                    if (status != null && status.Counter <= 0) {
                        RemoveExtraStatus(setup.Defender, setup.DefenderMap, "Substitute", setup.PacketStack);
                    }
                    
                    
                }
                
                if (setup.Hit && setup.Defender != null) {
                    int attackerSpeed = setup.Attacker.Spd;
                    int defenderSpeed = setup.Defender.Spd;


                    if (setup.Defender.AttackTimer == null || setup.Defender.AttackTimer.Tick < Core.GetTickCount().Tick) {
                        setup.Defender.AttackTimer = new TickCount(Core.GetTickCount().Tick);
                    }
                    //if (!HasAbility(setup.Defender, "Unaware")) {
                    attackerSpeed = DamageCalculator.ApplySpeedBuff(attackerSpeed, setup.Attacker.SpeedBuff);
                    //}
                    //if (!HasAbility(setup.Attacker, "Unaware")) {
                    defenderSpeed = DamageCalculator.ApplySpeedBuff(defenderSpeed, setup.Defender.SpeedBuff);
                    //}

                    if (setup.Attacker.HasActiveItem(258)) {
                        attackerSpeed *= 3;
                        attackerSpeed /= 2;
                    }
                    if (setup.Defender.HasActiveItem(258)) {
                        defenderSpeed *= 3;
                        defenderSpeed /= 2;
                    }

                    if (attackerSpeed < 1) attackerSpeed = 1;
                    if (defenderSpeed < 1) defenderSpeed = 1;

                    if (setup.AttackerMap.TempStatus.GetStatus("TrickRoom") != null) {
                    	int temp = attackerSpeed;
                    	attackerSpeed = defenderSpeed;
                    	defenderSpeed = temp;
                        
                    }
                    if (defenderSpeed < attackerSpeed) {
                    	setup.Defender.AttackTimer = new TickCount(setup.Defender.AttackTimer.Tick + 200 * defenderSpeed / attackerSpeed);
                    }
                }
                
                if (setup.Hit && setup.Defender != null) {
                    if (setup.Damage > 0 && setup.Defender.HasActiveItem(480)) {//eject button
                    	if (setup.DefenderMap.MapType == Enums.MapType.RDungeonMap) {
                    		setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateSoundPacket("magic256.wav"), setup.Defender.X, setup.Defender.Y, 10);
                    		RandomWarp(setup.Defender, setup.DefenderMap, false, setup.PacketStack);
                    	}
                    }
                    if (setup.Damage > 0 &&
                    	MoveProcessor.IsInAreaRange(1, setup.Defender.X, setup.Defender.Y, setup.Attacker.X, setup.Attacker.Y)
                    	&& setup.Defender.HasActiveItem(240)) {//red card
                    	BlowBack(setup.Attacker, setup.AttackerMap, (Enums.Direction)(((int)setup.Attacker.Direction + 1) % 2 + (int)setup.Attacker.Direction / 2 * 2), setup.PacketStack);
                    }
                }

                if (setup.BattleTags.Contains("Critical")) setup.BattleTags.Remove("Critical");
                
                if (setup.Defender != null && setup.DefenderMap != null && setup.DefenderMap.MapType == Enums.MapType.Instanced && ((InstancedMap)setup.DefenderMap).MapBase == 1945) {
					setup.Defender.AttackTimer = null;
					// Snowball game attack timer override
				}
				
				RemoveBattleTag(setup.BattleTags, "PrevHP");
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: AfterMoveHits", Text.Black);
                //Messenger.AdminMsg(ex.ToString(), Text.Black);
                //Messenger.AdminMsg(point.ToString(), Text.Black);
            }
        }

        public static void AfterMoveUsed(BattleSetup setup) {
        	
            try {
                ExtraStatus status;

                //Recharge time
                if (setup.Move.AdditionalEffectData1 == 9 || setup.Move.AdditionalEffectData1 == 128 ||
                    setup.Move.AdditionalEffectData1 == 132) {
                    setup.Attacker.PauseTimer = new TickCount(setup.Attacker.PauseTimer.Tick + 1000);
                } else if (setup.Move.AdditionalEffectData1 == 8) {
                    setup.Attacker.PauseTimer = new TickCount(setup.Attacker.PauseTimer.Tick + setup.Move.AdditionalEffectData2);
                } else {
                    //setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.AttackTimer.Tick.ToString(), Text.BrightGreen), setup.Attacker.X, setup.Attacker.Y, 10);
                    setup.Attacker.AttackTimer = new TickCount(setup.Attacker.AttackTimer.Tick + setup.Attacker.TimeMultiplier * setup.Move.HitTime / 1000);
                    //setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.AttackTimer.Tick.ToString(), Text.BrightGreen), setup.Attacker.X, setup.Attacker.Y, 10);
                }

                //for Standard Attacks
                if (setup.moveIndex < 1) {

                    if (setup.BattleTags.Contains("HitSomething")) {
                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSoundPacket("magic2.wav"));
                        //Messenger.PlaySoundToMap(setup.AttackerMap, "magic2.wav");
                    }
                    if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                        //if (((Recruit)setup.Attacker).Owner.Player.WaitingBattleMsg.FindIn
                        int x = setup.Attacker.X;
                        int y = setup.Attacker.Y;
                        MoveInDirection(setup.Attacker.Direction, 1, ref x, ref y);
                        if (x >= 0 && x <= setup.AttackerMap.MaxX && y >= 0 && y <= setup.AttackerMap.MaxY) {
                            Tile tile = setup.AttackerMap.Tile[x, y];
                            if (tile.Type == Enums.TileType.Scripted && GetTrapItem(tile.Data1) > 0 && tile.Data2 > 0) {
                            	//if the trap is revealed, destroy it
                            	if (setup.Attacker.HasActiveItem(116) && WillTrapActivate(setup.Attacker, setup.AttackerMap, x, y)) {
                            		//spawn item
                            		setup.AttackerMap.SpawnItem(GetTrapItem(tile.Data1), 1, false, false, "", x, y, null);
                            		setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSoundPacket("Magic22.wav"), setup.Attacker.X, setup.Attacker.Y, 10);
                            		setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSpellAnim(505, x, y));
                            		//delete trap
                            		RemoveTrap(setup.AttackerMap, x, y, setup.PacketStack);
                            	}
                            } else if (tile.Type == Enums.TileType.Scripted && GetTrapType(tile.Data1) != null && tile.Data3 != 1) {
                            //if the trap is hidden, reveal it
                                RevealTrap(setup.AttackerMap, x, y, setup.PacketStack);
                            }
                            
                            for (int i = 0; i < Server.Constants.MAX_MAP_ITEMS; i++) {
	                        	if (setup.AttackerMap.ActiveItem[i].Num != 0 && setup.AttackerMap.ActiveItem[i].Hidden) {
	                        		if (setup.AttackerMap.ActiveItem[i].X == x && setup.AttackerMap.ActiveItem[i].Y == y) {
	                        			setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSpellAnim(491, x, y));
	                        			setup.AttackerMap.SpawnItemSlot(i, setup.AttackerMap.ActiveItem[i].Num, setup.AttackerMap.ActiveItem[i].Value,
	                            		setup.AttackerMap.ActiveItem[i].Sticky, false, setup.AttackerMap.ActiveItem[i].Tag,
	                            		setup.AttackerMap.ActiveItem[i].X, setup.AttackerMap.ActiveItem[i].Y, null);
	                        		}
	                        	}
	                        }
                        }
                    }
                }
                
                //conversion
                status = setup.Attacker.VolatileStatus.GetStatus("Conversion");
                if (status != null && GetBattleTagArg(setup.BattleTags, "Conversion", 0) == null && setup.Move.Element != Enums.PokemonType.None) {
                	if (setup.Attacker.Type1 != setup.Move.Element) {
                		AddExtraStatus(setup.Attacker, setup.AttackerMap, "Type1", (int)setup.Move.Element, null, "", setup.PacketStack);
                	}
                	if (setup.Attacker.Type2 != Enums.PokemonType.None) {
                		AddExtraStatus(setup.Attacker, setup.AttackerMap, "Type2", 0, null, "", setup.PacketStack, false);
                	}
                }

                //trap destruction on Rapid Spin
                if (setup.Move.AdditionalEffectData1 == 85) {
                    bool trapRemoved = false;
                    for (int x = setup.Attacker.X - 1; x <= setup.Attacker.X + 1; x++) {
                        for (int y = setup.Attacker.Y - 1; y <= setup.Attacker.Y + 1; y++) {
                            if (x >= 0 && y >= 0 && x <= setup.AttackerMap.MaxX && y <= setup.AttackerMap.MaxY) {
                                Tile tile = setup.AttackerMap.Tile[x, y];
                                if (tile.Type == Enums.TileType.Scripted && GetTrapType(tile.Data1) != null) {
                                    RemoveTrap(setup.AttackerMap, x, y, setup.PacketStack);
                                    trapRemoved = true;
                                }
                            }
                        }
                    }
                    if (trapRemoved) {
                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("All nearby traps were destroyed!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                    }
                }

                //fog clearing and trap destruction on Defog
                if (setup.Move.AdditionalEffectData1 == 86) {
                    if (setup.AttackerMap.Tile[setup.Attacker.X, setup.Attacker.Y].Type == Enums.TileType.Arena || setup.AttackerMap.Moral == Enums.MapMoral.None || setup.AttackerMap.Moral == Enums.MapMoral.NoPenalty) {
                        if (setup.AttackerMap.Weather == Enums.Weather.Fog) {
                            setup.AttackerMap.Weather = Enums.Weather.None;
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, TcpPacket.CreatePacket("weather", ((int)setup.AttackerMap.Weather).ToString()));
                        }
                    } else {
                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("The weather can only change in dungeons and arenas!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                    }
                    bool trapRemoved = false;
                    for (int x = 0; x <= setup.AttackerMap.MaxX; x++) {
                        for (int y = 0; y <= setup.AttackerMap.MaxX; y++) {
                            Tile tile = setup.AttackerMap.Tile[x, y];
                            if (tile.Type == Enums.TileType.Scripted && GetTrapType(tile.Data1) != null && tile.Data2 == 1) {
                                RemoveTrap(setup.AttackerMap, x, y, setup.PacketStack);
                                trapRemoved = true;
                            }
                        }
                    }
                    if (trapRemoved) {
                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("All revealed traps on the floor were destroyed!", Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 10);
                    }
                }
				
                //Final Gambit backlash
                if (setup.Move.EffectType == Enums.MoveType.Scripted && setup.Hit && setup.Move.Data1 == 170) {
                    setup.Attacker.HP = 1;
                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " is on the verge of fainting after that last move!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                }

                //Healing Wish backlash
                if (setup.Move.EffectType == Enums.MoveType.Scripted && setup.Move.Data1 == 43) {
                    setup.Attacker.HP = 1;
                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " is on the verge of fainting after that last move!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                }

                //Memento backlash
                if (setup.Move.EffectType == Enums.MoveType.Scripted && setup.Move.Data1 == 89) {
                    setup.Attacker.HP = 1;
                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " is on the verge of fainting after that last move!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                    if (setup.AttackerMap.MapType == Enums.MapType.RDungeonMap) {
                        RandomWarp(setup.Attacker, setup.AttackerMap, true, setup.PacketStack);
                    } else {
                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " can only warp in mystery dungeons!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                    }
                }

                status = setup.Attacker.VolatileStatus.GetStatus("Rampage");
							if (status != null) {
                                status.Counter--;
                                if (status.Counter <= 0) {
                                    RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "Rampage", setup.PacketStack);
                                    AddExtraStatus(setup.Attacker, setup.AttackerMap, "Confusion", Server.Math.Rand(5, 11), null, "", setup.PacketStack, true);
                                }
                            }
                            status = setup.Attacker.VolatileStatus.GetStatus("Rolling");
							if (status != null) {
								if (setup.BattleTags.Contains("HitSomething")) {
                                
                                    status.Counter++;
                                    if (status.Counter >= 5) {
                                        RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "Rolling", setup.PacketStack);
                                    }
                                } else {
									RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "Rolling", setup.PacketStack);
								}
                            }
                            status = setup.Attacker.VolatileStatus.GetStatus("FuryCutter");
							if (status != null) {
								if (setup.BattleTags.Contains("HitSomething")) {
                                    if (status.Counter < 5) {
                                        status.Counter++;
                                    }
                                } else {
									RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "FuryCutter", setup.PacketStack);
								}
                            }

            } catch (Exception ex) {
                Messenger.AdminMsg("Error: AfterMoveUsed", Text.Black);
                Messenger.AdminMsg(ex.ToString(), Text.Black);
                if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                	Messenger.AdminMsg("Attacker: " + ((Recruit)setup.Attacker).Owner.Player.Name, Text.Black);
                }
                if (setup.Defender.CharacterType == Enums.CharacterType.Recruit) {
                	Messenger.AdminMsg("Defender: " + ((Recruit)setup.Defender).Owner.Player.Name, Text.Black);
                }
                
            }
        }

        public static void AfterActionTaken(BattleSetup setup) {
        int pointReached = 0;
            try {
                ExtraStatus status;
            	if (GetBattleTagArg(setup.BattleTags, "InvokedMove", 0) == null) {
	            	pointReached = 1;
	            	if (setup.Attacker.AttackTimer == null || setup.Attacker.AttackTimer.Tick < Core.GetTickCount().Tick) {
	
	                    setup.Attacker.AttackTimer = new TickCount(Core.GetTickCount().Tick);
	
	                }
	                pointReached = 2;
	                if (setup.Attacker.PauseTimer == null || setup.Attacker.PauseTimer.Tick < Core.GetTickCount().Tick) {
	                    setup.Attacker.PauseTimer = new TickCount(Core.GetTickCount().Tick);
	                }
	                
	                pointReached = 3;
	            
	                //gluttony rewards
	                if (GetBattleTagArg(setup.BattleTags, "Gluttony", 1) != null) {
	                    HealCharacter(setup.Attacker, setup.AttackerMap, GetBattleTagArg(setup.BattleTags, "Gluttony", 1).ToInt(), setup.PacketStack);
	                }
	                
	                
	
	
	                //magic coat
                    status = setup.Attacker.VolatileStatus.GetStatus("MagicCoat");
	                if (status != null) {
	                    status.Counter--;
	                    if (status.Counter <= 0) {
	                        RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "MagicCoat", setup.PacketStack);
	                    }
	                }
	                
	                //mirror coat
                    status = setup.Attacker.VolatileStatus.GetStatus("MirrorCoat");
	                if (status != null) {
	                    status.Counter--;
	                    if (status.Counter <= 0) {
	                        RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "MirrorCoat", setup.PacketStack);
	                    }
	                }
	                
	                //counter
                    status = setup.Attacker.VolatileStatus.GetStatus("Counter");
	                if (status != null) {
	                    status.Counter--;
	                    if (status.Counter <= 0) {
	                        RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "Counter", setup.PacketStack);
	                    }
	                }


                    status = setup.Attacker.VolatileStatus.GetStatus("Charge");
	                if (status != null) {
	                    status.Counter--;
	                    if (status.Counter <= 0) {
	                        RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "Charge", setup.PacketStack);
	                    }
	                }
					pointReached = 4;
	
	                //yawn
                    status = setup.Attacker.VolatileStatus.GetStatus("Yawn");
	                if (status != null) {
	                    status.Counter--;
	                    if (status.Counter <= 0) {
	                        RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "Yawn", setup.PacketStack);
	                        SetStatusAilment(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.Sleep, Server.Math.Rand(2, 4), setup.PacketStack);
	                    }
	                }
	                
	                pointReached = 5;
	
	                //nightmare
	                if (setup.Attacker.VolatileStatus.GetStatus("Nightmare") != null && setup.Attacker.StatusAilment == Enums.StatusAilment.Sleep) {
	                    if (setup.Attacker.HasActiveItem(498)) {
	                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("The Lunar Wing dispelled the nightmare!", Text.BrightGreen), setup.Attacker.X, setup.Attacker.Y, 10);
	                        RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "Nightmare", setup.PacketStack);
	                    } else {
	                        int dmg = setup.Attacker.MaxHP / 8;
	                        if (dmg < 1) dmg = 1;
	                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " is having nightmares!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
	                        if (dmg >= setup.Attacker.HP) {
                                DamageCharacter(setup.Attacker, setup.AttackerMap, dmg, Enums.KillType.Other, setup.PacketStack, true);
                                setup.Cancel = true;
                            return;
                            } else {
                                DamageCharacter(setup.Attacker, setup.AttackerMap, dmg, Enums.KillType.Other, setup.PacketStack, true);
                            }
	                    }
	                }
	                
	                //lunar wing
	                if (setup.Attacker.HasActiveItem(498) && setup.Attacker.StatusAilment == Enums.StatusAilment.Sleep) {
	                    int heal = setup.Attacker.MaxHP / 32;
	                    if (heal < 1) heal = 1;
	                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s health is restored by the Lunar Wing!", Text.BrightGreen), setup.Attacker.X, setup.Attacker.Y, 10);
	                    HealCharacter(setup.Attacker, setup.AttackerMap, heal, setup.PacketStack);
	                }
	                
	                pointReached = 6;
	
	                //Leech Seed Damage
                    status = setup.Attacker.VolatileStatus.GetStatus("LeechSeed:0");
	                if (!HasAbility(setup.Attacker, "Magic Guard") && status != null) {
                        if (status.Target == null) {
                            setup.Attacker.VolatileStatus.Remove(status);
                        } else {
                            pointReached = 7;
                            int seeddmg = setup.Attacker.MaxHP / 32;
                            if (seeddmg < 1) seeddmg = 1;
                            int heal = 5;
                            if (status.Target.HasActiveItem(194)) heal = 6;
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s health is sapped by Leech Seed!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
	                        if (seeddmg >= setup.Attacker.HP) {
                                DamageCharacter(setup.Attacker, setup.AttackerMap, seeddmg, Enums.KillType.Other, setup.PacketStack, true);
                                setup.Cancel = true;
                            	return;
                            } else {
                                DamageCharacter(setup.Attacker, setup.AttackerMap, seeddmg, Enums.KillType.Other, setup.PacketStack, true);
                            }
                            //HealCharacter(status.Target, MapManager.RetrieveActiveMap(status.Target.MapID), seeddmg * heal / 5, setup.PacketStack);
                            if (HasAbility(setup.Attacker, "Liquid Ooze")) {
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(status.Target.Name + " sucked up the Liquid Ooze!", Text.BrightRed), status.Target.X, status.Target.Y, 10);
                                DamageCharacter(status.Target, MapManager.RetrieveActiveMap(status.Target.MapID), seeddmg, Enums.KillType.Other, setup.PacketStack, true);    
		                        if (seeddmg >= status.Target.HP) {
	                                DamageCharacter(status.Target, MapManager.RetrieveActiveMap(status.Target.MapID), seeddmg, Enums.KillType.Other, setup.PacketStack, true);    
	                                setup.Cancel = true;
                            return;
	                            } else {
	                                DamageCharacter(status.Target, MapManager.RetrieveActiveMap(status.Target.MapID), seeddmg, Enums.KillType.Other, setup.PacketStack, true);    
	                            }
                            } else {
                                HealCharacter(status.Target, MapManager.RetrieveActiveMap(status.Target.MapID), seeddmg * heal / 5, setup.PacketStack);
                            }
                        }
	                }
	                
	                pointReached = 8;
	
	                //sure shot
                    status = setup.Attacker.VolatileStatus.GetStatus("SureShot");
	                if (status != null) {
	                    status.Counter--;
	                    if (status.Counter <= 0) {
	                        RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "SureShot", setup.PacketStack);
	                    }
	                }
	                
	
	                //taunt
                    status = setup.Attacker.VolatileStatus.GetStatus("Taunt");
	                if (status != null) {
	                    status.Counter--;
	                    if (status.Counter <= 0) {
	                        RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "Taunt", setup.PacketStack);
	                    }
	                }
	                
	                pointReached = 9;
	
	                //Snatch
                    status = setup.Attacker.VolatileStatus.GetStatus("Snatch");
	                if (status != null) {
	                    status.Counter--;
	                    if (status.Counter <= 0) {
	                        RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "Snatch", setup.PacketStack);
	                    }
	                }
	                
	                pointReached = 10;
	
	                //encore
                    status = setup.Attacker.VolatileStatus.GetStatus("Encore");
	                if (status != null) {
	                    status.Counter--;
	                    if (status.Counter <= 0) {
	                        RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "Encore", setup.PacketStack);
	                    }
	                }
	                
	                pointReached = 11;
	
	
	                //follow
                    status = setup.Attacker.VolatileStatus.GetStatus("Follow");
	                if (status != null) {
	                    status.Counter--;
	                    if (status.Counter <= 0) {
	                        RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "Follow", setup.PacketStack);
	                    }
	                }
	                
	                pointReached = 12;
	
	                //torment
	                //if (setup.Attacker.VolatileStatus.GetStatus("Torment") != null) {
	                //	setup.Attacker.VolatileStatus.GetStatus("Torment").Counter--;
	                //	if (setup.Attacker.VolatileStatus.GetStatus("Torment").Counter <= 0) {
	                //		RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "Torment", setup.PacketStack);
	                //	}
	                //}
	
	                //embargo
                    status = setup.Attacker.VolatileStatus.GetStatus("Embargo");
	                if (status != null) {
	                    status.Counter--;
	                    if (status.Counter <= 0) {
	                        RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "Embargo", setup.PacketStack);
	                    }
	                }
	                
	                pointReached = 13;
	
	                //heal block
                    status = setup.Attacker.VolatileStatus.GetStatus("HealBlock");
	                if (status != null) {
	                    status.Counter--;
	                    if (status.Counter <= 0) {
	                        RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "HealBlock", setup.PacketStack);
	                    }
	                }
	                
	                pointReached = 14;
	                
	                //Burn Damage
	                if (setup.Attacker.StatusAilment == Enums.StatusAilment.Burn
	                	&& !HasAbility(setup.Attacker, "Heatproof")
	                	&& !(HasAbility(setup.Attacker, "Water Veil") && HasAbility(setup.Attacker, "Flare Boost"))
	                	&& !HasAbility(setup.Attacker, "Magic Guard")) {
	                    int burndmg = setup.Attacker.MaxHP / 16;
	                    if (burndmg < 1) burndmg = 1;
	                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " is hurt by the burn!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
	                    if (burndmg >= setup.Attacker.HP) {
                            DamageCharacter(setup.Attacker, setup.AttackerMap, burndmg, Enums.KillType.Other, setup.PacketStack, true);
                            setup.Cancel = true;
                            return;
                        } else {
                            DamageCharacter(setup.Attacker, setup.AttackerMap, burndmg, Enums.KillType.Other, setup.PacketStack, true);
                        }
	                }
	                
	                pointReached = 15;
	
	                //future sight
                    status = setup.Attacker.VolatileStatus.GetStatus("FutureSight");
	                if (status != null) {
	                    status.Counter--;
	                    if (status.Counter <= 0) {
	                        RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "FutureSight", setup.PacketStack);
	                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSoundPacket("magic118.wav"));
	                        if (50 >= setup.Attacker.HP) {
                                DamageCharacter(setup.Attacker, setup.AttackerMap, 50, Enums.KillType.Other, setup.PacketStack, true);
                                setup.Cancel = true;
                            return;
                            } else {
                                DamageCharacter(setup.Attacker, setup.AttackerMap, 50, Enums.KillType.Other, setup.PacketStack, true);
                            }
	                    }
	                }
	                
	                pointReached = 16;
	
	                //Curse Damage
	                if (!HasAbility(setup.Attacker, "Magic Guard")) {
                        status = setup.Attacker.VolatileStatus.GetStatus("Curse");
                        if (status != null) {
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " is haunted by the curse...", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSoundPacket("magic3.wav"), setup.Attacker.X, setup.Attacker.Y, 10);
                            if (status.Counter >= setup.Attacker.HP) {
                                DamageCharacter(setup.Attacker, setup.AttackerMap, status.Counter, Enums.KillType.Other, setup.PacketStack, true);
                                setup.Cancel = true;
                            return;
                            } else {
                                DamageCharacter(setup.Attacker, setup.AttackerMap, status.Counter, Enums.KillType.Other, setup.PacketStack, true);
                            }
                        }
	                }
	                
	                pointReached = 17;
	
	                //Immobilization struggling
                    status = setup.Attacker.VolatileStatus.GetStatus("Immobilize");
	                if (status != null) {
	                    status.Counter--;
	                    if (status.Counter <= 0) {
	                        RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "Immobilize", setup.PacketStack, true);
	                    }
	                }
	                
	                pointReached = 19;
	
	                //Telekinesis struggling
                    status = setup.Attacker.VolatileStatus.GetStatus("Telekinesis");
	                if (status != null) {
	                    status.Counter--;
	                    if (status.Counter <= 0) {
	                        RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "Telekinesis", setup.PacketStack, true);
	                    }
	                }
	                
	                pointReached = 20;
	
	                //Ingrain timer
                    status = setup.Attacker.VolatileStatus.GetStatus("Ingrain");
	                if (status != null) {
	                    status.Counter--;
	                    HealCharacter(setup.Attacker, setup.AttackerMap, setup.Attacker.MaxHP / 6, setup.PacketStack);
	                    if (status.Counter <= 0) {
	                        RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "Ingrain", setup.PacketStack, true);
	                    }
	                }
	                
	                pointReached = 21;
	                
	                //Aqua Ring
	                if ((setup.Attacker.CharacterType == Enums.CharacterType.MapNpc || ((Recruit)setup.Attacker).Belly > 0) &&
                        setup.Attacker.VolatileStatus.GetStatus("AquaRing") != null) {
	                    int heal = setup.Attacker.MaxHP / 32;
	                    if (heal < 1) heal = 1;
	                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + "'s health is restored by Aqua Ring!", Text.BrightGreen), setup.Attacker.X, setup.Attacker.Y, 10);
	                    HealCharacter(setup.Attacker, setup.AttackerMap, heal, setup.PacketStack);
	                }
	                
	                pointReached = 22;

                    status = setup.Attacker.VolatileStatus.GetStatus("Invisible");
	                if (status != null) {
                    	status.Counter -= 2;
	                    if (status.Counter <= 0) {
	                        RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "Invisible", setup.PacketStack, true);
	                    }
	                }
	                
	                pointReached = 23;
	
	                int trapdmg = 0;
	                if (trapdmg < 1) trapdmg = 1;
                    status = setup.Attacker.VolatileStatus.GetStatus("FireSpin");
	                if (status != null) {
	                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSpellAnim(206, setup.Attacker.X, setup.Attacker.Y));
	                    if (!HasAbility(setup.Attacker, "Magic Guard")) {
	                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " is hurt by Fire Spin!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
	                        if (status.Tag == "1") {
                            	trapdmg = setup.Attacker.MaxHP / 16;
                            } else {
                            	trapdmg = setup.Attacker.MaxHP / 32;
                            }
	                        if (trapdmg >= setup.Attacker.HP) {
                                DamageCharacter(setup.Attacker, setup.AttackerMap, trapdmg, Enums.KillType.Other, setup.PacketStack, true);
                                setup.Cancel = true;
                            return;
                            } else {
                                DamageCharacter(setup.Attacker, setup.AttackerMap, trapdmg, Enums.KillType.Other, setup.PacketStack, true);
                            }
	                    }
                        status.Counter--;
                        if (status.Counter <= 0) {
	                        RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "FireSpin", setup.PacketStack, true);
	                    }
	                }
                    status = setup.Attacker.VolatileStatus.GetStatus("Whirlpool");
	                if (status != null) {
	                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSpellAnim(203, setup.Attacker.X, setup.Attacker.Y));
	                    if (!HasAbility(setup.Attacker, "Magic Guard")) {
	                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " is hurt by Whirlpool!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
	                        if (status.Tag == "1") {
                            	trapdmg = setup.Attacker.MaxHP / 16;
                            } else {
                            	trapdmg = setup.Attacker.MaxHP / 32;
                            }
	                        if (trapdmg >= setup.Attacker.HP) {
                                DamageCharacter(setup.Attacker, setup.AttackerMap, trapdmg, Enums.KillType.Other, setup.PacketStack, true);
                                setup.Cancel = true;
                            return;
                            } else {
                                DamageCharacter(setup.Attacker, setup.AttackerMap, trapdmg, Enums.KillType.Other, setup.PacketStack, true);
                            }
	                    }
	                    status.Counter--;
	                    if (status.Counter <= 0) {
	                        RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "Whirlpool", setup.PacketStack, true);
	                    }
	                }
                    status = setup.Attacker.VolatileStatus.GetStatus("SandTomb");
	                if (status != null) {
	                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSpellAnim(148, setup.Attacker.X, setup.Attacker.Y));
	                    if (!HasAbility(setup.Attacker, "Magic Guard")) {
	                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " is hurt by Sand Tomb!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
	                        if (status.Tag == "1") {
                            	trapdmg = setup.Attacker.MaxHP / 16;
                            } else {
                            	trapdmg = setup.Attacker.MaxHP / 32;
                            }
	                        if (trapdmg >= setup.Attacker.HP) {
                                DamageCharacter(setup.Attacker, setup.AttackerMap, trapdmg, Enums.KillType.Other, setup.PacketStack, true);
                                setup.Cancel = true;
                            return;
                            } else {
                                DamageCharacter(setup.Attacker, setup.AttackerMap, trapdmg, Enums.KillType.Other, setup.PacketStack, true);
                            }
	                    }
	                    status.Counter--;
	                    if (status.Counter <= 0) {
	                        RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "SandTomb", setup.PacketStack, true);
	                    }
	                }
                    status = setup.Attacker.VolatileStatus.GetStatus("MagmaStorm");
	                if (status != null) {
	                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSpellAnim(94, setup.Attacker.X, setup.Attacker.Y));
	                    if (!HasAbility(setup.Attacker, "Magic Guard")) {
	                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Attacker.Name + " is hurt by Magma Storm!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
	                        if (status.Tag == "1") {
                            	trapdmg = setup.Attacker.MaxHP / 16;
                            } else {
                            	trapdmg = setup.Attacker.MaxHP / 32;
                            }
	                        if (trapdmg >= setup.Attacker.HP) {
                                DamageCharacter(setup.Attacker, setup.AttackerMap, trapdmg, Enums.KillType.Other, setup.PacketStack, true);
                                setup.Cancel = true;
                            return;
                            } else {
                                DamageCharacter(setup.Attacker, setup.AttackerMap, trapdmg, Enums.KillType.Other, setup.PacketStack, true);
                            }
	                    }
	                    status.Counter--;
	                    if (status.Counter <= 0) {
	                        RemoveExtraStatus(setup.Attacker, setup.AttackerMap, "MagmaStorm", setup.PacketStack, true);
	                    }
	                }
	                
	                pointReached = 24;
	
	                //Life Orb
	                if (setup.Attacker.HasActiveItem(143) && setup.BattleTags.Contains("HitSomething") && !HasAbility(setup.Attacker, "Magic Guard")) {
	                	if (setup.Attacker.MaxHP / 32 >= setup.Attacker.HP) {
                            DamageCharacter(setup.Attacker, setup.AttackerMap, setup.Attacker.MaxHP / 32, Enums.KillType.Other, setup.PacketStack, true);
                            setup.Cancel = true;
                            return;
                        } else {
                            DamageCharacter(setup.Attacker, setup.AttackerMap, setup.Attacker.MaxHP / 32, Enums.KillType.Other, setup.PacketStack, true);
                        }
	                }
	                
	                pointReached = 25;
	
	                //Orb Damage
	                if (setup.Attacker.HasActiveItem(169) && !CheckStatusProtection(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.Poison.ToString() + ":2", false, setup.PacketStack)) {
	                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("The Toxic Orb activated!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
	                    SetStatusAilment(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.Poison, 2, setup.PacketStack);
	                } else if (setup.Attacker.HasActiveItem(163) && !CheckStatusProtection(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.Burn.ToString(), false, setup.PacketStack)) {
	                    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("The Flame Orb activated!", Text.BrightRed), setup.Attacker.X, setup.Attacker.Y, 10);
	                    SetStatusAilment(setup.Attacker, setup.AttackerMap, Enums.StatusAilment.Burn, 1, setup.PacketStack);
	                }
	                
	                pointReached = 26;
	
	                //check ability
	                CheckAfterActionTakenAbility(setup);
	                
	                pointReached = 27;
	
	                //Perish Song
                    status = setup.Attacker.VolatileStatus.GetStatus("PerishCount");
	                if (setup.Attacker.VolatileStatus.GetStatus("PerishCount") != null) {
	                    AddExtraStatus(setup.Attacker, setup.AttackerMap, "PerishCount", status.Counter - 1, null, "", setup.PacketStack);
	
	                    if (status.Counter <= 1) {
	                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSoundPacket("magic118.wav"));
	                        DamageCharacter(setup.Attacker, setup.AttackerMap, setup.Attacker.HP, Enums.KillType.Other, setup.PacketStack, true);
                            setup.Cancel = true;
                            return;
	                    }
	                }
	                
	                pointReached = 28;


                    if (setup.Attacker.MapID == setup.AttackerMap.MapID) {
                    	if (setup.AttackerMap.MapType != Enums.MapType.RDungeonMap) {
                        } else if (setup.AttackerMap.Tile[setup.Attacker.X, setup.Attacker.Y].Type == Enums.TileType.MobileBlock) {
                            
                            int mobilityList = setup.AttackerMap.Tile[setup.Attacker.X, setup.Attacker.Y].Data1;
                            bool blocked = false;
                            for (int i = 0; i < 16; i++) {
                                if (mobilityList % 2 == 1 && !setup.Attacker.Mobility[i]) {
                                    blocked = true;
                                }
                                mobilityList /= 2;
                            }

                            if (blocked == true) {
                                RandomWarp(setup.Attacker, setup.AttackerMap, false, setup.PacketStack);
                            }
                        } else if (setup.AttackerMap.Tile[setup.Attacker.X, setup.Attacker.Y].Type == Enums.TileType.Blocked) {
                            RandomWarp(setup.Attacker, setup.AttackerMap, false, setup.PacketStack);
                        }
                        if (setup.AttackerMap.Tile[setup.Attacker.X, setup.Attacker.Y].Type == Enums.TileType.Slippery &&
                            setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                            if (((Recruit)setup.Attacker).Belly <= 0) {
                                RandomWarp(setup.Attacker, setup.AttackerMap, false, setup.PacketStack);
                            }
                        }
                    }
	                
	                pointReached = 29;
	
	                if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit && setup.AttackerMap.HungerEnabled) {
	                	if (setup.Attacker.HasActiveItem(103)) {
            	
		            	} else if (setup.Attacker.HasActiveItem(151)) {
			            	if (setup.moveIndex > 0) {
			            	((Recruit)setup.Attacker).Owner.Player.BellyStepCounter += 25;
			            	} else {
			            		((Recruit)setup.Attacker).Owner.Player.BellyStepCounter += 5;
			            	}
		            	} else {
			            	if (setup.moveIndex > 0) {
			            	((Recruit)setup.Attacker).Owner.Player.BellyStepCounter += 50;
			            	} else {
			            		((Recruit)setup.Attacker).Owner.Player.BellyStepCounter += 10;
			            	}
		            	}
		            	if (setup.Attacker.HasActiveItem(144)) {
            				((Recruit)setup.Attacker).Owner.Player.BellyStepCounter += 50;
		            	}
	                    ((Recruit)setup.Attacker).Owner.Player.ProcessHunger(setup.PacketStack);
	                }
                }
                pointReached = 30;
            } catch (Exception ex) {
            	Messenger.AdminMsg("Error: AfterActionTaken", Text.Black);
                Messenger.AdminMsg(pointReached.ToString(), Text.Black);
                if (setup.AttackerMap != null) {
                	Messenger.AdminMsg(setup.AttackerMap.Name, Text.Black);
                }
                if (setup.Attacker != null) {
                	Messenger.AdminMsg(setup.Attacker.Name, Text.Black);
                }
                if (setup.Defender != null) {
                	Messenger.AdminMsg(setup.Defender.Name, Text.Black);
                }
                Messenger.AdminMsg(ex.ToString(), Text.Black);
            }
        }

        public static string GetBattleTagArg(List<string> battleTags, string tagName, int index) {
            foreach (string s in battleTags) {
                if (s.Split(':')[0] == tagName) {
                    if (s.Split(':').Length > index) {
                        return s.Split(':')[index];
                    }
                }
            }
            return null;
        }

        public static void RemoveBattleTag(List<string> battleTags, string tagName) {
            for (int i = 0; i < battleTags.Count; i++) {
                if (battleTags[i].Split(':')[0] == tagName) {
                    battleTags.RemoveAt(i);
                    return;
                }
            }
        }
        
        public static void ReviveCharacter(ICharacter character, IMap map, bool fullHeal, PacketHitList hitlist) {
        	try {
        	PacketHitList.MethodStart(ref hitlist);
        	if (fullHeal) {
        		character.HP = character.MaxHP;
	            for (int i = 0; i < character.Moves.Length; i++) {
	                character.Moves[i].CurrentPP = character.Moves[i].MaxPP;
	            }
	            if (character.CharacterType == Enums.CharacterType.Recruit) {
        			((Recruit)character).RestoreBelly();
        		}
        		hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " was fully revived!", Text.BrightGreen));
        	} else {
        		int hp = character.MaxHP / 2;
        		if (hp < 1) hp = 1;
        		character.HP = hp;
        		//restore belly to 20
        		if (character.CharacterType == Enums.CharacterType.Recruit) {
        			Recruit recruit = character as Recruit;
					if (recruit.Belly < 25) {
						recruit.Belly = 25;
						PacketBuilder.AppendBelly(recruit.Owner, hitlist);
					}
        		}
        		hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " was revived!", Text.BrightGreen));
        	}
        	
        	hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("magic682.wav"), character.X, character.Y, 10);
        	hitlist.AddPacketToMap(map, PacketBuilder.CreateSpellAnim(509, character.X, character.Y));
        	
        	if (character.CharacterType == Enums.CharacterType.Recruit) {
        		((Recruit)character).Owner.Player.Dead = false;
        		PacketBuilder.AppendDead(((Recruit)character).Owner, hitlist);
        		Messenger.ForceEndStoryTo(((Recruit)character).Owner);
        	}
        	
        	RefreshCharacterMoves(character, map, hitlist);
        	
        	PacketHitList.MethodEnded(ref hitlist);
        	} catch (Exception ex) {
        		Messenger.AdminMsg(ex.ToString(), Text.Black);
        	}
        }

        public static void HandleGameOver(Client client, Enums.KillType killType) {
            bool pointReached = false;
            try {
	            Messenger.ForceEndStoryTo(client);
	            
	            //if (Ranks.IsAllowed(client, Enums.Rank.Developer)) {
	            //    //RemoveInvOnDeath(index);
	            //} else 
	            if (client.Player.Map.Moral == Enums.MapMoral.None) {
	                RemoveInvOnDeath(client);
	            }
				
	            try {
	                client.Player.EndTempStatMode();
	                client.Player.GetActiveRecruit().RestoreVitals();
	            } catch (Exception ex) {
	            }
	            if (client.Player.MapID != MapManager.GenerateMapID(660)) {
                    if (exPlayer.Get(client) != null && exPlayer.Get(client).VerifySpawnPoint() == true) {
						DungeonRules.ExitDungeon(client, -1, 0, 0);
                    } else {
                    	exPlayer.Get(client).SpawnMap = Crossroads;
                            exPlayer.Get(client).SpawnX = 25;
                            exPlayer.Get(client).SpawnY = 25;
						DungeonRules.ExitDungeon(client, -1, 0, 0);
                    }
	            } else {
	                Messenger.PlayerWarp(client, 660, 15, 4);
	                Messenger.MapMsg(client.Player.MapID, client.Player.Name + " was eliminated from the arena!", Text.Black);
	            }
				
	            client.Player.Dead = false;
	            PacketHitList hitlist = null;
	            PacketHitList.MethodStart(ref hitlist);
	            PacketBuilder.AppendDead(client, hitlist);
	            PacketHitList.MethodEnded(ref hitlist);
	        } catch (Exception ex) {
	        	Messenger.AdminMsg("Error: OnGameOver", Text.Black);
	        	Messenger.AdminMsg(pointReached.ToString(), Text.Black);
	        	Messenger.AdminMsg(ex.ToString(), Text.Black);
	        }
        }

        public static void HandleDeath(PacketHitList hitlist, Client client, Enums.KillType killType, bool autoSwitch) {
            PacketHitList.MethodStart(ref hitlist);

            //aftermath
            if (HasAbility(client.Player.GetActiveRecruit(), "Aftermath")) {
                bool explode = true;
				if (!HasAbility(client.Player.GetActiveRecruit(), "Mold Breaker")) {
	                TargetCollection checkedTargets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Floor, 0, client.Player.Map, null, 0, 0, Enums.Direction.Up, true, true, false);
	                for (int i = 0; i < checkedTargets.Count; i++) {
	                    if (HasAbility(checkedTargets[i], "Damp")) {
	                        explode = false;
	                    }
	                }
                }

                if (explode) {
                    hitlist.AddPacketToMap(client.Player.Map, PacketBuilder.CreateBattleMsg(client.Player.GetActiveRecruit().Name + " exploded!", Text.BrightRed), client.Player.X, client.Player.Y, 10);
                    hitlist.AddPacketToMap(client.Player.Map, PacketBuilder.CreateSoundPacket("magic216.wav"), client.Player.X, client.Player.Y, 10);
                    for (int i = client.Player.X - 1; i <= client.Player.X + 1; i++) {
                        for (int j = client.Player.Y - 1; j <= client.Player.Y + 1; j++) {
                            if (i < 0 || j < 0 || i > client.Player.Map.MaxX || j > client.Player.Map.MaxY) {

                            } else {
                                hitlist.AddPacketToMap(client.Player.Map, PacketBuilder.CreateSpellAnim(497, i, j));
                            }
                        }
                    }
                    
	                    TargetCollection targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 1, client.Player.Map, client.Player.GetActiveRecruit(), client.Player.X, client.Player.Y, Enums.Direction.Up, true, true, true);
	                    for (int i = 0; i < targets.Count; i++) {
	                    if (client.Player.Map.Moral == Enums.MapMoral.None) {
	                        if (targets[i].HP > 0) {
	                        	if (HasActiveBagItem(targets[i], 6, 0, 0)) {
	                        		DamageCharacter(targets[i], client.Player.Map, 1, Enums.KillType.Tile, hitlist, true);
	                        	} else {
	                            	DamageCharacter(targets[i], client.Player.Map, targets[i].MaxHP / 4, Enums.KillType.Tile, hitlist, true);
	                            }
	                        }
	                        }
	                    }
                    
                } else {
                    hitlist.AddPacketToMap(client.Player.Map, PacketBuilder.CreateSoundPacket("magic320.wav"), client.Player.X, client.Player.Y, 10);
                    hitlist.AddPacketToMap(client.Player.Map, PacketBuilder.CreateSpellAnim(505, client.Player.X, client.Player.Y));
                    hitlist.AddPacketToMap(client.Player.Map, PacketBuilder.CreateBattleMsg("The damp conditions prevented an explosion!", Text.Blue), client.Player.X, client.Player.Y, 10);
                }
            }
            
            //death flag
            TargetCollection witnesses = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Floor, 0, client.Player.Map, client.Player.GetActiveRecruit(), 0, 0, Enums.Direction.Up, false, true, false);
			for (int i = 0; i < witnesses.Count; i++) {
                AddExtraStatus(witnesses[i], client.Player.Map, "SawAllyFaint", 0, null, "", hitlist);
            }
			
            // Remove status
            client.Player.GetActiveRecruit().StatusAilment = Enums.StatusAilment.OK;
            client.Player.GetActiveRecruit().StatusAilmentCounter = 0;

            List<ExtraStatus> passedStatus = GetTeamStatus(client.Player.GetActiveRecruit());

            RemoveAllBondedExtraStatus(client.Player.GetActiveRecruit(), client.Player.Map, hitlist, false);

            client.Player.GetActiveRecruit().VolatileStatus.Clear();

            foreach (ExtraStatus status in passedStatus) {
                client.Player.GetActiveRecruit().VolatileStatus.Add(status);
            }

            RefreshCharacterTraits(client.Player.GetActiveRecruit(), client.Player.Map, hitlist);

            PacketBuilder.AppendStatusAilment(client, hitlist);

            RemoveBuffs(client.Player.GetActiveRecruit());
            
			if (Ranks.IsDisallowed(client, Enums.Rank.Moniter)) {
	            client.Player.Hunted = false;
	            PacketBuilder.AppendHunted(client, hitlist);
            }
            client.Player.Dead = true;
            PacketBuilder.AppendDead(client, hitlist);

            AskAfterDeathQuestion(client);

            /*
            if (client.Player.GetActiveRecruit().HeldItemSlot > -1) {
                if (client.Player.GetActiveRecruit().HeldItem.Num == 222) {//Enigma Berry
                    if (Server.Math.Rand(0, 10) == 0) {
                        Messenger.MapMsg(client.Player.MapID, client.Player.Name + " was revived!!!", Text.BrightGreen);
                        client.Player.GetActiveRecruit().RestoreVitals();
                        client.Player.GetActiveRecruit().RestoreBelly();
                        Messenger.SendPlayerMoves(client);

                        foreach (ExtraStatus status in passedStatus) {
                            client.Player.GetActiveRecruit().VolatileStatus.Add(status);
                        }
                        PacketHitList.MethodEnded(ref hitlist);
                        return;
                    }
                }
            }
            

            if (client.Player.TournamentMatchUp != null) {
                client.Player.TournamentMatchUp.EndMatchUp(client.Player.TournamentMatchUp.SelectOtherMember(client).Client.Player.CharID);
                PacketHitList.MethodEnded(ref hitlist);
                return;
            }

            //For Reviver Seeds
            int slot = client.Player.HasItem(489);
            if (client.Player.MapID == MapManager.GenerateMapID(660)) {
                slot = 0;
            }
            if (slot > 0) {
                Messenger.MapMsg(client.Player.MapID, client.Player.Name + " was revived!", Text.BrightGreen);
                client.Player.GetActiveRecruit().RestoreVitals();
                client.Player.GetActiveRecruit().RestoreBelly();
                Messenger.SendPlayerMoves(client);

                client.Player.TakeItem(489, 1);
                client.Player.GiveItem(488, 1);
                //NetScript.SetPlayerInvItemNum(index, slot, 488);
                Messenger.SendInventory(client);

                foreach (ExtraStatus status in passedStatus) {
                    client.Player.GetActiveRecruit().VolatileStatus.Add(status);
                }
                PacketHitList.MethodEnded(ref hitlist);
                return;

            }
            /*

            //Sacred Ash
            slot = client.Player.HasItem(523);
            if (client.Player.MapID == MapManager.GenerateMapID(660)) {
                slot = 0;
            }
            if (slot > 0) {
                Messenger.MapMsg(client.Player.MapID, client.Player.Name + " was revived!", Text.BrightGreen);
                client.Player.GetActiveRecruit().RestoreVitals();
                client.Player.GetActiveRecruit().RestoreBelly();
                Messenger.SendPlayerMoves(client);
                client.Player.GetActiveRecruit().AttackBuff += 2;
                client.Player.GetActiveRecruit().DefenseBuff += 2;
                client.Player.GetActiveRecruit().SpeedBuff += 2;
                client.Player.GetActiveRecruit().SpAtkBuff += 2;

                client.Player.TakeItem(523, 1);

                Messenger.SendInventory(client);
                return;
            }
            
            
            if (autoSwitch && client.Player.ActiveSlot != 0) {
                int activeSlot = client.Player.ActiveSlot;
                string name = client.Player.GetActiveRecruit().Name;
                foreach (ExtraStatus status in passedStatus) {
                    client.Player.GetActiveRecruit().VolatileStatus.Add(status);
                    Messenger.BattleMsg(client, status.Name, Text.BrightRed);
                }

                client.Player.SwapActiveRecruit(0);
                client.Player.RestoreRecruitStats(activeSlot);

                client.Player.RemoveFromTeam(activeSlot);
                if (!string.IsNullOrEmpty(name)) {
                    Messenger.BattleMsg(client, name + " has fainted and returned home!", Text.BrightRed);
                } else {
                    Messenger.BattleMsg(client, "Your team member has fainted and returned home!", Text.BrightRed);
                }
                Messenger.SendActiveTeam(client);
            } else {
                HandleGameOver(client, killType);
            }
            if (exPlayer.Get(client).VoltorbFlip) {
                Messenger.PlaySound(client, "Leave Slot Machine.wav");
            } else {
                Messenger.PlaySound(client, "Death.wav");
            }
            */
            //Messenger.SendBattleDivider(client);
            //RefreshCharacterTraits(client.Player.GetActiveRecruit(), client.Player.Map, hitlist);
            PacketHitList.MethodEnded(ref hitlist);
        }

        public static void OnDeath(Client client, Enums.KillType killType) {
            try {
                HandleDeath(null, client, killType, true);
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: OnDeath", Text.Black);
            }
        }

        public static void OnDeath(PacketHitList hitlist, Client client, Enums.KillType killType) {
            try {
            HandleDeath(hitlist, client, killType, true);
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: OnDeath", Text.Black);
            }
        }

        public static void RemoveInvOnDeath(Client client) {
            if (client.Player.HasItem(1) > 1) {
                if (client.Player.GetActiveRecruit().HeldItemSlot > -1) {
                    if (client.Player.GetActiveRecruit().HeldItem.Num == 225) { //Holiday Ribbon

                    } else {
                        client.Player.TakeItem(1, client.Player.HasItem(1));
                    }
                } else {
                    client.Player.TakeItem(1, client.Player.HasItem(1));
                }
            }

            if (client.Player.GetActiveRecruit().HeldItemSlot > -1) {
                if (ItemManager.Items[client.Player.GetActiveRecruit().HeldItem.Num].Loseable == false) {
                    client.Player.TakeItem(client.Player.GetActiveRecruit().HeldItem.Num, 1);
                    client.Player.GetActiveRecruit().HeldItemSlot = -1;
                }
            }

            // First get a list of all item slots
            List<int> itemsToRemove = new List<int>();
            for (int i = 1; i <= client.Player.MaxInv; i++) {
                if (client.Player.Inventory[i].Num > 0) {
                	if (ItemManager.Items[client.Player.Inventory[i].Num].StackCap > 0) {
                		client.Player.TakeItem(client.Player.Inventory[i].Num, client.Player.Inventory[i].Amount);
                	} else {
                    	itemsToRemove.Add(i);
                    }
                }
            }
            
                int count = itemsToRemove.Count;
                if (itemsToRemove.Count == 1) count = 1;
                for (int i = 0; i < count; i++) {
                	int rand = Server.Math.Rand(0,itemsToRemove.Count);
                    if (client.Player.Inventory[itemsToRemove[rand]].Num > 0) {
                        if (ItemManager.Items[client.Player.Inventory[itemsToRemove[rand]].Num].Loseable == false) {
                        } else {
                            client.Player.TakeItem(client.Player.Inventory[itemsToRemove[rand]].Num, 1);
                        }
                    }
                    itemsToRemove.RemoveAt(rand);
                }
                Messenger.SendInventory(client);
                Messenger.PlayerMsg(client, "You dropped some of the items in your inventory!", Text.BrightRed);
            
        }

        public static void OnNpcDeath(PacketHitList hitlist, ICharacter attacker, MapNpc npc) {
            try {
                // Called when a npc faints
                PacketHitList.MethodStart(ref hitlist);

                hitlist.AddPacketToMap(MapManager.RetrieveActiveMap(npc.MapID), PacketBuilder.CreateBattleMsg(npc.Name + " fainted!", Text.WhiteSmoke), npc.X, npc.Y, 10);

                //aftermath
                if (HasAbility(npc, "Aftermath")) {
                    bool explode = true;
					if (!HasAbility(npc, "Mold Breaker")) {
	                    TargetCollection checkedTargets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Floor, 0, MapManager.RetrieveActiveMap(npc.MapID), null, 0, 0, Enums.Direction.Up, true, true, false);
	                    for (int i = 0; i < checkedTargets.Count; i++) {
	                        if (HasAbility(checkedTargets[i], "Damp")) {
	                            explode = false;
	                        }
	                    }
                    }

                    if (explode) {
                        hitlist.AddPacketToMap(MapManager.RetrieveActiveMap(npc.MapID), PacketBuilder.CreateBattleMsg(npc.Name + " exploded!", Text.BrightRed), npc.X, npc.Y, 10);
                        hitlist.AddPacketToMap(MapManager.RetrieveActiveMap(npc.MapID), PacketBuilder.CreateSoundPacket("magic216.wav"), npc.X, npc.Y, 10);
                        for (int i = npc.X - 1; i <= npc.X + 1; i++) {
                            for (int j = npc.Y - 1; j <= npc.Y + 1; j++) {
                                if (i < 0 || j < 0 || i > MapManager.RetrieveActiveMap(npc.MapID).MaxX || j > MapManager.RetrieveActiveMap(npc.MapID).MaxY) {

                                } else {
                                    hitlist.AddPacketToMap(MapManager.RetrieveActiveMap(npc.MapID), PacketBuilder.CreateSpellAnim(497, i, j));
                                }
                            }
                        }
                        
	                        TargetCollection targets = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Room, 1, MapManager.RetrieveActiveMap(npc.MapID), npc, npc.X, npc.Y, Enums.Direction.Up, true, true, false);
	                        for (int i = 0; i < targets.Count; i++) {
	                            if (MapManager.RetrieveActiveMap(npc.MapID).Moral == Enums.MapMoral.None) {
	                            if (targets[i].HP > 0) {
	                            	if (HasActiveBagItem(targets[i], 6, 0, 0)) {
		                        		DamageCharacter(targets[i], MapManager.RetrieveActiveMap(targets[i].MapID), 1, Enums.KillType.Tile, hitlist, true);
		                        	} else {
	                                	DamageCharacter(targets[i], MapManager.RetrieveActiveMap(targets[i].MapID), targets[i].MaxHP / 4, Enums.KillType.Tile, hitlist, true);
	                                }
	                            }
	                            }
	                        }
                        
                    } else {
                        hitlist.AddPacketToMap(MapManager.RetrieveActiveMap(npc.MapID), PacketBuilder.CreateSoundPacket("magic320.wav"), npc.X, npc.Y, 10);
                        hitlist.AddPacketToMap(MapManager.RetrieveActiveMap(npc.MapID), PacketBuilder.CreateSpellAnim(505, npc.X, npc.Y));
                        hitlist.AddPacketToMap(MapManager.RetrieveActiveMap(npc.MapID), PacketBuilder.CreateBattleMsg("The damp conditions prevented an explosion!", Text.Blue), npc.X, npc.Y, 10);
                    }
                }
                
                //death flag
	            TargetCollection witnesses = MoveProcessor.GetTargetsInRange(Enums.MoveRange.Floor, 0, MapManager.RetrieveActiveMap(npc.MapID), npc, 0, 0, Enums.Direction.Up, false, true, false);
				for (int i = 0; i < witnesses.Count; i++) {
	                AddExtraStatus(witnesses[i], MapManager.RetrieveActiveMap(npc.MapID), "SawAllyFaint", 0, null, "", hitlist);
	            }

                RemoveAllBondedExtraStatus(npc, MapManager.RetrieveActiveMap(npc.MapID), hitlist, false);
				IMap map = MapManager.RetrieveActiveMap(npc.MapID);
				Client client = null;
				foreach (Client n in map.GetClients()) {
                				client = n;
                				break;
                			}
				
                if (client != null) {
                    

                    
					bool doBossCheck = false;

                    switch (npc.Num) {
                    /*
                        case 276:
                        case 277:
                        case 278: { //Dynamo Trio
                                map.RemoveNpc(npc.MapSlot);
                                if (map.IsNpcSlotEmpty(1) && map.IsNpcSlotEmpty(2) && map.IsNpcSlotEmpty(3)) {
                                    map.Tile[9, 1].Mask = 96;
                                    map.Tile[9, 1].MaskSet = 4;
                                    map.Tile[9, 1].Type = Enums.TileType.Scripted;
                                    map.Tile[9, 1].Data1 = 46;
                                    map.Tile[9, 1].Data2 = 0;
                                    map.Tile[9, 1].Data3 = 0;
                                    map.Tile[9, 1].String1 = "19";
                                    map.Tile[9, 1].String2 = "1";
                                    map.Tile[9, 1].String3 = "10:7";
                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateTilePacket(9, 1, map));
                                }
                            }
                            break;
                        */
                        case 304: {//registeel
                                map.RemoveNpc(npc.MapSlot);
                                if (map.IsNpcSlotEmpty(5)) {
                                    BossBattles.EndRegisteelBattle(client);
                                }
                            }
                            break;/*
                        case 305: {//bronzong
                                map.RemoveNpc(npc.MapSlot);
                                if (map.IsNpcSlotEmpty(1) && map.IsNpcSlotEmpty(2) && (npc.MapSlot == 1 || npc.MapSlot == 2)) {
                                    if (client.Player.GetStoryState(43) == true) {
                                        client.Player.SetStoryState(43, false);
                                    }
                                    StoryManager.PlayStory(client, 43);
                                } else if (map.IsNpcSlotEmpty(1) && map.IsNpcSlotEmpty(2) && map.IsNpcSlotEmpty(3) && map.IsNpcSlotEmpty(4)) {
                                    if (client.Player.GetStoryState(56) == true) {
                                        client.Player.SetStoryState(56, false);
                                    }
                                    StoryManager.PlayStory(client, 56);
                                }
                            }
                            break;*/
                        case 220: {//Murkrow minions 2.0
                                if (map.IsNpcSlotEmpty(14)) {
                                    map.RemoveNpc(npc.MapSlot);
                                }

                                for (int i = 1; i < 14; i++) {
                                    if (map.IsNpcSlotEmpty(i) == false) {
                                        return;
                                    }
                                }
                                StoryManager.PlayStory(client, 160);
                            }
                            break;
                        case 221: {//Honchkrow
                                map.RemoveNpc(npc.MapSlot);
                                for (int i = 1; i < 14; i++) {
                                    if (map.IsNpcSlotEmpty(i) == false) {
                                        map.RemoveNpc(i);
                                    }
                                }
                                StoryManager.PlayStory(client, 161);
                            }
                            break;
                        case 224: {//Murkrow minions
                                map.RemoveNpc(npc.MapSlot);
                                for (int i = 1; i < 14; i++) {
                                    if (map.IsNpcSlotEmpty(i) == false) {
                                        return;
                                    }
                                }
                                StoryManager.PlayStory(client, 160);
                            }
                            break;
                        case 259:
                        case 306: {
                                if (map.Name == "Iceberg Cave") {
                                    map.RemoveNpc(npc.MapSlot);
                                    if (map.IsNpcSlotEmpty(1) && map.IsNpcSlotEmpty(2) && map.IsNpcSlotEmpty(3)) {
                                        BossBattles.EndRegiceBattle(client);
                                    }
                                }
                            }
                            break;
                        //case 177: {
                        //    map.RemoveNpc(npc.MapSlot);
                        //    if (map.IsNpcSlotEmpty(1) && map.IsNpcSlotEmpty(2) && map.IsNpcSlotEmpty(3) && map.IsNpcSlotEmpty(4) &&
                        //    map.IsNpcSlotEmpty(5) && map.IsNpcSlotEmpty(6) && map.IsNpcSlotEmpty(7)) {
                        //        BossBattles.RemoveLuxioTribe(client);
                        //    }
                        //}
                        //break;
                        case 167: {
                                //map.RemoveNpc(npc.MapSlot);
                                //if (map.IsNpcSlotEmpty(1) && map.IsNpcSlotEmpty(2) && map.IsNpcSlotEmpty(3) && map.IsNpcSlotEmpty(4) &&
                                //map.IsNpcSlotEmpty(5) && map.IsNpcSlotEmpty(6) && map.IsNpcSlotEmpty(7)) {
                                //    BossBattles.RemoveLuxioTribe(client);
                                //}
                            }
                            break;
                            /*
                        case 465: {
                                map.RemoveNpc(1);
                                map.Tile[9, 4].Type = Enums.TileType.Scripted;
                                map.Tile[9, 4].Data1 = 46;
                                map.Tile[9, 4].Data2 = 0;
                                map.Tile[9, 4].Data3 = 0;
                                map.Tile[9, 4].String1 = "15";
                                map.Tile[9, 4].String2 = "93";
                                map.Tile[9, 4].String3 = "8:10";
                                map.Tile[9, 4].Mask = 96;
                                map.Tile[9, 4].MaskSet = 4;
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateTilePacket(9, 4, map));
                            }
                            break;
                            */
                        //case 212: {//fake Gengar (adjust for the actual NPC number)
                        //        if (map.MapType == Enums.MapType.RDungeonMap) {
                        //           int num = (map as RDungeonMap).RDungeonIndex;
                        //            if ((num + 1) == 20) {
                        //                StoryManager.PlayStory(client, 287);
                        //                StoryManager.PlayStory(client, 288);
                        //                StoryManager.PlayStory(client, 289);
                        //                StoryManager.PlayStory(client, 286);
                        //                StoryManager.PlayStory(client, 290);
                        //                StoryManager.PlayStory(client, 291);
                        //                StoryManager.PlayStory(client, 292);
                        //            }
                        //        }

                        //    }
                        //    break;
                        //case 804: {//real gengar defeated
                        //        map.RemoveNpc(npc.MapSlot);
                        //        for (int i = 2; i <= 3; i++) {
                        //            if (map.IsNpcSlotEmpty(i) == false) {
                        //                map.RemoveNpc(i);
                        //            }
                        //        }
                        //        map.SetAttribute(8, 4, Enums.TileType.Scripted, 36, 0, 0, "19", "1", "");
                        //        map.SetAttribute(9, 10, Enums.TileType.Scripted, 13, 0, 0, "", "", "");
                        //        map.SetAttribute(10, 10, Enums.TileType.Scripted, 13, 0, 0, "", "", "");
                        //        map.SetAttribute(8, 5, Enums.TileType.Walkable, 0, 0, 0, "", "", "");
                        //        map.SetAttribute(13, 6, Enums.TileType.Notice, 0, 0, 0, "You hesitate...", "It's probably a good idea not to sleep here again...", "");
                        //        StoryManager.PlayStory(client, 285);
                        //    }
                        //    break;
                        //case 806:
                        //case 807: {//rotom forms
                        //        map.RemoveNpc(npc.MapSlot);
                        //        map.SetAttribute(9, 10, Enums.TileType.Scripted, 13, 0, 0, "", "", "");
                        //        map.SetAttribute(10, 10, Enums.TileType.Scripted, 13, 0, 0, "", "", "");
                        //        Messenger.PlayerMsg(client, "The exit was opened!", Text.Yellow);
                        //    }
                        //    break;
                        case 805: {//spiritomb defeated
                                map.RemoveNpc(npc.MapSlot);
                                map.SetAttribute(9, 2, Enums.TileType.Scripted, 36, 0, 0, "21", "30", "");
                                map.SetTile(9, 2, 96, 4, 1);
                                StoryManager.PlayStory(client, 303);
                            }
                            break;
                            /*
                        case 440: {//Articuno*
                                map.RemoveNpc(1);
                                map.SetTile(12, 5, 96, 4, 1);
                                map.SetAttribute(12, 5, Enums.TileType.Scripted, 46, 0, 0, "20", "342", "13:13");
                                StoryManager.PlayStory(client, 171);
                            }
                            break;
                            */
                        case 404: {//Heatran
                                map.RemoveNpc(1);
                                map.SetTile(10, 6, 96, 4, 3);
                                map.SetAttribute(10, 6, Enums.TileType.Scripted, 46, 0, 0, "18", "1313", "12:12");
                                //NetScript.PlayStory(index, 171);
                            }
                            break;
                        case 579:
                        case 558:
                        case 559:
                        case 560:
                        case 571:
                        case 572:
                        case 854:
                        case 855:
                        case 856:
                        case 857:
                        case 858:
                        case 859:
                        case 860:
                        case 861:
                        case 862:
                        case 863: { //normal PBA enemies + boss
                                if (map.Name == "Pitch-Black Pit") {
                                    int remainingEnemies = 0;
            						for (int i = 0; i < Constants.MAX_MAP_NPCS; i++) {
            							if (!map.IsNpcSlotEmpty(i)) remainingEnemies++;
            						}
            						if (remainingEnemies <= 1) {
                                        BossBattles.EndBossBattle(client, "Pitch-Black Pit");
                                    }
                                } else PitchBlackAbyss.UnlockRoom(client, map);
                        	}
                        	break;
                        case 139: {
                                BossBattles.EndBossBattle(client, "ThunderstormForest");
                            }
                            break;
                        case 177: {
                                BossBattles.EndBossBattle(client, "ThunderstormForestPart2");
                            }
                            break;
                            /*
                        case 588:
                        case 587: { // EST Boss A Wave 1
                                if (map.Name == "Electrostasis Tower Chamber") {
                                    map.RemoveNpc(npc.MapSlot);
                                    if (map.IsNpcSlotEmpty(1) && map.IsNpcSlotEmpty(2) && map.IsNpcSlotEmpty(3) && map.IsNpcSlotEmpty(4) &&
                                        (npc.MapSlot == 1 || npc.MapSlot == 2 || npc.MapSlot == 3 || npc.MapSlot == 4)) {

                                        map.SetTile(4, 12, 0, 0, 1);
                                        map.SetAttribute(4, 12, Enums.TileType.Walkable, 0, 0, 0, "", "", "");
                                        map.SetAttribute(9, 12, Enums.TileType.Story, 201, 0, 0, "", "", "");

                                        hitlist.AddPacketToMap(map, PacketBuilder.CreateTilePacket(4, 12, map));
                                        hitlist.AddPacketToMap(map, PacketBuilder.CreateTilePacket(9, 12, map));
                                    } else if (map.IsNpcSlotEmpty(1) && map.IsNpcSlotEmpty(2) && map.IsNpcSlotEmpty(3) && map.IsNpcSlotEmpty(4) &&
                                                map.IsNpcSlotEmpty(5) && map.IsNpcSlotEmpty(6) && map.IsNpcSlotEmpty(7) &&
                                                (npc.MapSlot == 5 || npc.MapSlot == 6 || npc.MapSlot == 7)) {
                                        BossBattles.EndBossBattle(client, "ElectrostasisTowerA1");
                                    }
                                }
                            }
                            break;
                            */
                        case 963: {// Joey, Exbel Woods Clearing
                        		int recruitIndex = -1;
                        		using (DatabaseConnection dbConnection = new DatabaseConnection(DatabaseID.Players)) {
                                	recruitIndex = client.Player.AddToRecruitmentBank(dbConnection, client.Player.CreateRecruitFromNpc(npc));
                                }
								int openSlot = client.Player.FindOpenTeamSlot();
                                if (recruitIndex != -1 && openSlot != -1) {
                                	
                                	client.Player.AddToTeam(recruitIndex, openSlot);
                                	client.Player.Team[openSlot].HP = client.Player.Team[openSlot].MaxHP;
                                	Messenger.BattleMsg(client, "You have recruited a new team member!", Text.BrightGreen);

                                	Messenger.SendActiveTeam(client);
                                } else {
                                	Messenger.BattleMsg(client, "You cant recruit! You have too many team members in the assembly!", Text.BrightRed);
                                }
                                doBossCheck = true;
	                        }
	                        break;
                        default: {
                                doBossCheck = true;
                            }
                            break;
                    }
                    
                    if (doBossCheck) {
	                    if (BossBattles.IsBossBattleMap(client)) {
		                    int remainingEnemies = 0;
		                    for (int i = 0; i < Constants.MAX_MAP_NPCS; i++) {
		                    	if (!map.IsNpcSlotEmpty(i)) remainingEnemies++;
	                        }
	                        //Messenger.AdminMsg(remainingEnemies.ToString(), Text.Black);
	                        if (remainingEnemies <= 1) BossBattles.EndBossBattle(client, map.Name);
	                    }
                    }


                }

                PacketHitList.MethodEnded(ref hitlist);
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: OnNpcDeath", Text.Black);
                Messenger.AdminMsg("NPCNum: " + npc.Num, Text.Black);
            }
        }

        public static Enums.Weather GetCharacterWeather(ICharacter character) {
        	try {
            if (character.HasActiveItem(303)) {
                return Enums.Weather.None;
            }
            IMap map = MapManager.RetrieveActiveMap(character.MapID);
            if (map != null) {
                if (map.Weather == Enums.Weather.Ambiguous || map.Weather == Enums.Weather.None) {
                    if (character.HasActiveItem(302)) {
                        return Enums.Weather.Sunny;
                    } else if (character.HasActiveItem(301)) {
                        return Enums.Weather.Raining;
                    } else if (character.HasActiveItem(300)) {
                        return Enums.Weather.Sandstorm;
                    } else if (character.HasActiveItem(299)) {
                        return Enums.Weather.Hail;
                    }

                }

                return map.Weather;
            }
			} catch (Exception ex) {
				Messenger.AdminMsg("Error: GetCharacterWeather", Text.Black);
			}

            return Enums.Weather.Ambiguous;
        }


        public static void SetIllusion(int spriteNum) {

        }

        private static void SetAsNeutralizedMove(Move move) {
            move.EffectType = Enums.MoveType.Scripted;
            move.MoveCategory = Enums.MoveCategory.Status;
            move.Data1 = 137;
            move.Data2 = 0;
            move.Data3 = 0;
            move.AdditionalEffectData1 = 0;
            move.AdditionalEffectData2 = 0;
            move.AdditionalEffectData3 = 0;
        }


        private static void HealCharacterBelly(ICharacter character, int belly, PacketHitList hitlist) {
            if (character.CharacterType == Enums.CharacterType.Recruit) {
                Recruit recruit = character as Recruit;

                recruit.Belly += belly;
                if (recruit.Belly >= recruit.MaxBelly) {
                    recruit.Belly = recruit.MaxBelly;
                    if (belly > 10) {
                    	hitlist.AddPacketToMap(recruit.Owner.Player.Map, PacketBuilder.CreateBattleMsg(character.Name + "'s belly was filled completely.", Text.BrightGreen), character.X, character.Y, 10);
                    }
                } else {
                	if (belly > 10) {
                		hitlist.AddPacketToMap(recruit.Owner.Player.Map, PacketBuilder.CreateBattleMsg(character.Name + "'s belly was filled somewhat.", Text.BrightGreen), character.X, character.Y, 10);
                	}
                }
				
                PacketBuilder.AppendBelly(((Recruit)character).Owner, hitlist);
            }
        }

        private static void HealCharacterPP(ICharacter character, IMap map, int pp, PacketHitList hitlist) {
            for (int i = 0; i < Constants.MAX_PLAYER_MOVES; i++) {
                if (character.Moves[i] != null) {
                    character.Moves[i].CurrentPP += pp;
                    if (character.Moves[i].CurrentPP > character.Moves[i].MaxPP) {
                        character.Moves[i].CurrentPP = character.Moves[i].MaxPP;
                    }
                }
            }
            if (character.CharacterType == Enums.CharacterType.Recruit) {
                PacketBuilder.AppendPlayerMoves(((Recruit)character).Owner, hitlist);
            }
            hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s moves regained " + pp + " PP.", Text.BrightGreen), character.X, character.Y, 10);
        }

        private static void HealCharacter(ICharacter character, IMap map, int hp, PacketHitList hitlist) {
            try {
	            if (character.VolatileStatus.GetStatus("HealBlock") != null) {
	                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " couldn't recover HP due to the Heal Block!", Text.BrightRed), character.X, character.Y, 10);
	                return;
	            }
	
	            if (character.HP + hp > character.MaxHP) {
	                hp = character.MaxHP - character.HP;
	            }
	
	            character.HP += hp;
	
	            if (hp <= 0) {
	                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + "'s HP didn't change.", Text.BrightGreen), character.X, character.Y, 10);
	                //BothWaysBattleMsg(setup, setup.Defender.Name + " took no damage!", Text.BrightRed);
	            } else if (character.HP == character.MaxHP) {
	                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " was fully healed!", Text.BrightGreen), character.X, character.Y, 10);
	            } else {
	                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " recovered " + hp + " HP!", Text.BrightGreen), character.X, character.Y, 10);
	                //BothWaysBattleMsg(setup, setup.Defender.Name + " took " + damage + " damage!", Text.BrightRed);
	            }
            } catch (Exception ex) {
            	Messenger.AdminMsg("Error: HealCharacter: " + character.Name, Text.Black);
                Messenger.AdminMsg(ex.ToString(), Text.Black);
                throw new Exception();
            }
        }

        private static void DamageCharacter(ICharacter character, IMap map, int dmg, Enums.KillType killType, PacketHitList hitlist, bool checkDeath) {
            int point = 0;
            try {
	            character.HP -= dmg;
	
	            if (dmg <= 0) {
	                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " took no damage!", Text.BrightRed), character.X, character.Y, 10);
	                //BothWaysBattleMsg(setup, setup.Defender.Name + " took no damage!", Text.BrightRed);
	            } else {
	                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " took " + dmg + " damage!", Text.BrightRed), character.X, character.Y, 10);
	                //BothWaysBattleMsg(setup, setup.Defender.Name + " took " + damage + " damage!", Text.BrightRed);
	            }
	            
	            point = 1;
	
	            if (checkDeath && character.HP <= 0) {
	                if (character.CharacterType == Enums.CharacterType.Recruit) {
	
						point = 2;
	                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("Oh, no!  " + character.Name + " fainted!", Text.BrightRed));
	                    point = 3;
	                    OnDeath(((Recruit)character).Owner, Enums.KillType.Other);
	                    point = 4;
	
	                } else {
	
						if (character.HeldItem != null) {
	                        character.MapDropItem(character.HeldItem.Amount, null);
	                    }
						
	                    OnNpcDeath(hitlist, null, (MapNpc)character);
	                    //((MapNpc)character).Num = 0;
	                    //((MapNpc)character).HP = 0;
	                    map.ActiveNpc[((MapNpc)character).MapSlot] = new MapNpc(character.MapID, ((MapNpc)character).MapSlot);
	
	                    hitlist.AddPacketToMap(map, TcpPacket.CreatePacket("npcdead", ((MapNpc)character).MapSlot));
	
	                }
	            }
            } catch(Exception ex) {
            	Messenger.AdminMsg("Error: DamageCharacter", Text.Black);
            	Messenger.AdminMsg(ex.ToString(), Text.Black);
            	Messenger.AdminMsg(character.Name + " took " + dmg + " dmg at " + map.Name + " from " + killType + " with checkDeath=" + checkDeath + " and point=" + point, Text.Black);
            	throw new Exception();
            }
        }
        
        public static bool IsInvokableMove(int moveNum) {
        	switch (moveNum) {
        		case 402: //metronome
        		case 224: //sleep talk
        		case 287: //assist
        		case 314: //copycat
        		case 230: //mirror move
        		case 259: //me first
        		case 357: //mimic
        		case 435: //sketch
        		case 401: //transform
        		case 466: //struggle
        		case 628: {//after you
        			return false;
        		}
        	}
        	if (moveNum <= 0) return false;
        	if (moveNum >= 470 && moveNum <= 600) return false;
        
        	return true;
        }



        private static void DropHeldItem(ICharacter character, bool playerFor) {
            if (character.HeldItem == null) return;
            if (character.CharacterType == Enums.CharacterType.Recruit) {
                if (playerFor) {
                    ((Recruit)character).Owner.Player.DropItem(((Recruit)character).Owner.Player.GetActiveRecruit().HeldItemSlot, character.HeldItem.Amount, ((Recruit)character).Owner);
                } else {
                    ((Recruit)character).Owner.Player.DropItem(((Recruit)character).Owner.Player.GetActiveRecruit().HeldItemSlot, character.HeldItem.Amount, null);
                }
            } else {
                ((MapNpc)character).MapDropItem(character.HeldItem.Amount, null);
            }
        }

        private static bool SafeZoneCheck(ICharacter attacker, IMap attackerMap, ICharacter defender, IMap defenderMap) {
            //safe zone attack-all protection
            if ((attackerMap.Tile[attacker.X, attacker.Y].Type != Enums.TileType.Arena
                || defenderMap.Tile[defender.X, defender.Y].Type != Enums.TileType.Arena)
                && (attackerMap.Moral == Enums.MapMoral.House || attackerMap.Moral == Enums.MapMoral.Safe)) {
                if (attacker.CharacterType == Enums.CharacterType.Recruit && defender.CharacterType == Enums.CharacterType.Recruit) {
                    //if (!((Recruit)defender).Owner.Player.PK) {
                        return true;
                    //}
                }
            }
            return false;
        }
    }
}
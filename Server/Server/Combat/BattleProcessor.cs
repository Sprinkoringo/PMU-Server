using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Server.Players;
using Server.Items;
using Server.Network;
using Server.Maps;
using Server.Npcs;
using Server.RDungeons;
using Server.Exp;
using Server.Moves;
using PMU.Sockets;
using Server.Players.Parties;

namespace Server.Combat
{
    public class BattleProcessor
    {

        #region Obsolete

        #region old player-npc methods

        /*
        public static void PlayerAttackNpc(Client attacker, int npcSlot, int damage) {//fix from main
            string Name = null;
            ulong Exp = 0;
            int i;
            int n;
            int NPCNum;

            // Check for subscript out of range
            if (attacker.IsPlaying() == false || npcSlot < 0 || npcSlot > Constants.MAX_MAP_NPCS || damage < 0) {
                return;
            }

            // Check for weapon ~ never used
            //if (attacker.Player.Weapon > 0)
            //{
            //    n = attacker.Player.Inventory[attacker.Player.Weapon].Num;
            //}
            //else
            //{
            //    n = 0;
            //}

            // Send this packet so they can see the person attacking
            //Messenger.SendDataToMapBut(attacker, attacker.Player.MapID, Messenger.CreateAttackPacket(attacker));

            IMap map = attacker.Player.GetCurrentMap();

            NPCNum = map.ActiveNpc[npcSlot].Num;
            Name = NpcManager.Npcs[NPCNum].Name.Trim();

            if (Settings.Scripting == true) {
                Scripting.ScriptManager.InvokeSub("OnNpcAttacked", attacker, map, NPCNum, npcSlot, damage);
            }

            int @add = 0;

            // Check for a weapon and say damage
            Messenger.BattleMsg(attacker, Name + " took " + damage + " damage!", Text.BrightBlue);

            if (damage >= map.ActiveNpc[npcSlot].HP) {
                // Check for a weapon and say damage

                //script add OnNpcDeath here
                if (Settings.Scripting == true) {
                    Scripting.ScriptManager.InvokeSub("OnNpcDeath", attacker, map, NPCNum, npcSlot);

                } else {
                    Messenger.BattleMsg(attacker, "A " + Name + " has fainted!", Text.BrightRed);
                }

                bool skipRecruit = false;

                try {
                    if (attacker.Player.Map.MapType == Enums.MapType.RDungeonMap) {
                        if (RDungeonManager.RDungeons[((RDungeonMap)attacker.Player.Map).RDungeonIndex].Recruitment == true) {
                            skipRecruit = false;
                        } else {
                            skipRecruit = true;
                        }
                    } else {
                        skipRecruit = false;
                    }
                } catch { }
                if (skipRecruit == false) {
                    if (NpcManager.Npcs[NPCNum].RecruitRate != 0) {
                        int openSlot = attacker.Player.FindOpenTeamSlot();
                        if (openSlot != -1) {
                            if (NpcManager.Npcs[NPCNum].RecruitRate < 0) {
                                int bonus = attacker.Player.GetRecruitBonus();
                                if (bonus > 0) {
                                    if (Math.Rand(1, 1001) < (NpcManager.Npcs[NPCNum].RecruitRate + bonus)) {
                                        if (attacker.Player.GetTeamSize() + Pokedex.Pokedex.GetPokemon(NpcManager.Npcs[NPCNum].Species).Size <= 4) {
                                            //attacker.Player.AddToRequested(NpcManager.Npcs[NPCNum], NPCNum, map.Npc[npcSlot].Level);
                                        }
                                    }
                                }
                            } else {
                                if (Math.Rand(1, 1001) < (NpcManager.Npcs[NPCNum].RecruitRate + attacker.Player.GetRecruitBonus())) {
                                    if (attacker.Player.GetTeamSize() + Pokedex.Pokedex.GetPokemon(NpcManager.Npcs[NPCNum].Species).Size <= 4) {
                                        //attacker.Player.AddToRequested(NpcManager.Npcs[NPCNum], NPCNum, map.Npc[npcSlot].Level);
                                    }
                                }
                            }
                        }
                    }
                }


                @add = attacker.Player.GetActiveRecruit().EXPBoost;


                try {
                    // Calculate exp to give attacker
                    if (@add > 0) {
                        Exp = (ulong)(Pokedex.Pokedex.GetPokemon(NpcManager.Npcs[NPCNum].Species).GetRewardExp(map.ActiveNpc[npcSlot].Level) * (100 + @add) / 100);
                    } else {
                        Exp = (ulong)Pokedex.Pokedex.GetPokemon(NpcManager.Npcs[NPCNum].Species).GetRewardExp(map.ActiveNpc[npcSlot].Level);
                    }
                } catch {
                    Messenger.PlayerMsg(attacker, "Error calculating Exp", Text.BrightRed);
                }

                // Make sure we dont get less then 0
                if (Exp < 0) {
                    Exp = 1;
                }

                bool skipExp = false;
                try {
                    if (attacker.Player.Map.MapType == Enums.MapType.RDungeonMap) {
                        if (RDungeonManager.RDungeons[((RDungeonMap)attacker.Player.Map).RDungeonIndex].Exp == false) {
                            skipExp = true;
                        } else {
                            skipExp = false;
                        }
                    } else {
                        skipExp = false;
                    }
                } catch { }

                if (skipExp == true) {
                    Exp = 0;
                    //Messenger.BattleMsg(attacker, "You can't gain any experience in this dungeon!", Text.BrightBlue);
                } else {
                    if (attacker.Player.partyID == null) {
                        if (attacker.Player.GetActiveRecruit().Level == Exp.ExpManager.Exp.MaxLevels) {

                            //Messenger.BattleMsg(attacker, "You cant gain anymore experience!", Text.BrightBlue);
                        } else {
                            attacker.Player.GetActiveRecruit().Exp += Exp;
                            Messenger.BattleMsg(attacker, "You gained " + Exp + " experience.", Text.BrightBlue);
                        }
                    } else {
                        Party party = PartyManager.FindPlayerParty(attacker);
                        if (party != null) {
                            //party.HandoutExp(Exp, setup);
                        }
                    }
                }

                for (i = 0; i < Constants.MAX_NPC_DROPS; i++) {
                    if (NpcManager.Npcs[NPCNum].Drops[i].Chance == 1) {
                        ///map.SpawnNpcDrop(NpcManager.Npcs[NPCNum].Drops[i].ItemNum, NpcManager.Npcs[NPCNum].Drops[i].ItemValue, map.ActiveNpc[npcSlot].X, map.ActiveNpc[npcSlot].Y, attacker);
                    } else {
                        if (NpcManager.Npcs[NPCNum].Drops[i].Chance > 0) {
                            n = Math.Rand(0, NpcManager.Npcs[NPCNum].Drops[i].Chance) + 1;
                            if (n == 1) {
                                //map.SpawnNpcDrop(NpcManager.Npcs[NPCNum].Drops[i].ItemNum, NpcManager.Npcs[NPCNum].Drops[i].ItemValue, map.ActiveNpc[npcSlot].X, map.ActiveNpc[npcSlot].Y, attacker);
                            }
                        }
                    }
                }

                // Now set HP to 0 so we know to actually kill them in the server loop (this prevents subscript out of range)
                map.ActiveNpc[npcSlot].Num = 0;
                map.ActiveNpc[npcSlot].HP = 0;
                Messenger.SendBattleDivider(attacker);
                Messenger.SendDataToMap(attacker.Player.MapID, TcpPacket.CreatePacket("npcdead", npcSlot.ToString()));
                // Check for level up
                //CheckPlayerLevelUp(attacker);
                Messenger.SendEXP(attacker);
            } else {
                // NPC not dead, just do the damage
                map.ActiveNpc[npcSlot].HP = map.ActiveNpc[npcSlot].HP - damage;// make npc death check its own method

                // Check if we should send a message
                if (map.ActiveNpc[npcSlot].Target == null && map.ActiveNpc[npcSlot].Target != attacker) {
                    if (NpcManager.Npcs[NPCNum].AttackSay.Trim() != "") {
                        Messenger.PlayerMsg(attacker, "A " + NpcManager.Npcs[NPCNum].Name.Trim() + " : " + NpcManager.Npcs[NPCNum].AttackSay.Trim() + "", Text.Grey);
                    }
                }

                // Set the NPC target to the player
                map.ActiveNpc[npcSlot].Target = attacker;

                // Now check for guard ai and if so have all onmap guards come after'm
                if (NpcManager.Npcs[map.ActiveNpc[npcSlot].Num].Behavior == Enums.NpcBehavior.Guard) {
                    for (i = 0; i < Constants.MAX_MAP_NPCS; i++) {
                        if (map.ActiveNpc[i].Num == map.ActiveNpc[npcSlot].Num) {
                            map.ActiveNpc[i].Target = attacker;
                        }
                    }
                }
            }

            //Call SendDataToMap(MapNum, "npchp" & SEP_CHAR & MapNpcNum & SEP_CHAR & map.ActiveNpc[MapNpcNum].HP & SEP_CHAR & GetNpcMaxHP(map.ActiveNpc[MapNpcNum].num) & SEP_CHAR & END_CHAR)

            // Reset attack timer
            attacker.Player.AttackTimer = Core.GetTickCount();
        }
        

        public static void NpcAttackPlayer(int npcSlot, Client victim, int damage) {
            string Name = null;
            //ulong Exp = 0;

            // Check for subscript out of range
            if (npcSlot < 0 || npcSlot > Constants.MAX_MAP_NPCS || victim.IsPlaying() == false || damage < 0) {
                return;
            }

            IMap map = victim.Player.GetCurrentMap();

            // Check for subscript out of range
            if (map.ActiveNpc[npcSlot].Num <= 0) {
                return;
            }

            // Send this packet so they can see the person attacking
            Messenger.SendDataToMap(victim.Player.MapID, TcpPacket.CreatePacket("npcattack", npcSlot.ToString()));

            Name = NpcManager.Npcs[map.ActiveNpc[npcSlot].Num].Name.Trim();

            if (damage >= victim.Player.GetActiveRecruit().HP) {
                // Say damage
                Messenger.BattleMsg(victim, "You took " + damage + " damage!", Text.BrightRed);
                if (Settings.Scripting == true) {
                    Scripting.ScriptManager.InvokeSub("OnNpcAttack", victim, map, npcSlot, damage, true);
                }
                //Call PlayerMsg(Victim, "A " & Name & " hit you for " & Damage & " hit points.", BrightRed)

                // Player is dead
                //Program.mResponder.GlobalMsg("Oh no! " + Program.ClassMan.mPlayers[Victim].mName + " has fainted!", Text.BrightRed);
                Messenger.PlayerMsg(victim, "You have fainted!", Text.BrightRed);
                Messenger.MapMsg(map.MapID, "Oh no! " + victim.Player.Name + " fainted!", Text.BrightRed);
                //Program.mResponder.MapMsgTo("Oh no! " + Program.ClassMan.mPlayers[Victim].mName + " has fainted!", Text.BrightRed, Program.ClassMan.mPlayers[Attacker]);

                //if (map.Moral != Enums.MapMoral.NoPenalty)
                //{
                //    DropItems(victim);

                //if (Settings.LoseExpOnDeath) {
                // Calculate exp to give attacker
                //    Exp = (victim.Player.GetActiveRecruit().Exp / 3);

                // Make sure we dont get less then 0
                //    if (Exp < 0) {
                //        Exp = 0;
                //    }

                //    if (Exp == 0) {
                //        Messenger.BattleMsg(victim, "You lost no experience.", Text.BrightRed);
                //    } else {
                //        victim.Player.GetActiveRecruit().Exp -= Exp;
                //        Messenger.BattleMsg(victim, "You lost " + Exp + " experience.", Text.BrightRed);
                //    }
                //}
                //}

                // Warp player away
                if (Settings.Scripting == true) {
                    Scripting.ScriptManager.InvokeSub("OnDeath", victim, Enums.KillType.Npc);
                } else {
                    Messenger.PlayerWarp(victim, Settings.StartMap, Settings.StartX, Settings.StartY);
                }

                // Restore vitals
                victim.Player.GetActiveRecruit().HP = victim.Player.GetMaxHP();


                // Set NPC target to -1
                map.ActiveNpc[npcSlot].Target = null;

                // If the player the attacker killed was a pk then take it away
                if (victim.Player.PK == true) {
                    victim.Player.PK = false;
                    Messenger.SendPlayerData(victim);
                    Messenger.GlobalMsg(victim.Player.Name + " has paid the price for being an Outlaw!", Text.BrightRed);
                }
            } else {
                // Player not dead, just do the damage
                victim.Player.GetActiveRecruit().HP -= damage;


                // Say damage
                Messenger.BattleMsg(victim, "You took " + damage + " damage!", Text.BrightRed);
                if (Settings.Scripting == true) {
                    Scripting.ScriptManager.InvokeSub("OnNpcAttack", victim, map, npcSlot, damage, false);
                }
            }
            //Call PlayerMsg(Victim, "A " & Name & " hit you for " & Damage & " hit points.", BrightRed)

            Messenger.SendDataTo(victim, TcpPacket.CreatePacket("blitnpcdmg", damage.ToString()));
            Messenger.PlaySoundToMap(victim.Player.MapID, "pain.wav");
        }
        */


        //public static void MoveHitPlayer(ICharacter attacker, Client victim, int moveNum) {
        //    Move move = MoveManager.Moves[moveNum];
        //    switch (move.EffectType) {
        //        case Enums.MoveType.SubHP: {
        //                int damage = DamageCalculator.CalculateDamage(move.Data1, attacker.SpclAtk, attacker.Level, victim.Player.GetActiveRecruit().SpclDef);



        //                if (attacker.CharacterType == Enums.CharacterType.Recruit) {
        //                    Messenger.BattleMsg(victim, ((Recruit)attacker).Owner.Player.Name + " used " + MoveManager.Moves[moveNum].Name + "!", Text.BrightRed);
        //                    BattleProcessor.PlayerAttackPlayer(((Recruit)attacker).Owner, victim, damage);
        //                } else if (attacker.CharacterType == Enums.CharacterType.MapNpc) {
        //                    Messenger.BattleMsg(victim, ((MapNpc)attacker).Name + " used " + MoveManager.Moves[moveNum].Name + "!", Text.BrightRed);
        //                    BattleProcessor.NpcAttackPlayer(((MapNpc)attacker).MapSlot, victim, damage);
        //                }
        //                Messenger.SpellAnim(moveNum, victim.Player.MapID, victim.Player.X, victim.Player.Y);
        //            }
        //            break;
        //        case Enums.MoveType.AddHP: {
        //                victim.Player.SetHP(victim.Player.GetActiveRecruit().HP + move.Data1);
        //                Messenger.SpellAnim(moveNum, victim.Player.MapID, victim.Player.X, victim.Player.Y);
        //            }
        //            break;
        //        case Enums.MoveType.SubMP: {
        //                int randomSlot = Server.Math.Rand(0, 4);
        //                if (victim.Player.GetActiveRecruit().Moves[randomSlot].MoveNum > 0 && victim.Player.GetActiveRecruit().Moves[randomSlot].CurrentPP > 0) {
        //                    victim.Player.GetActiveRecruit().Moves[randomSlot].CurrentPP--;
        //                    Messenger.SendMovePPUpdate(victim, randomSlot);
        //                } else {
        //                    for (int i = 0; i < victim.Player.GetActiveRecruit().Moves.Length; i++) {
        //                        if (victim.Player.GetActiveRecruit().Moves[i].MoveNum > 0 && victim.Player.GetActiveRecruit().Moves[i].CurrentPP > 0) {
        //                            victim.Player.GetActiveRecruit().Moves[i].CurrentPP--;
        //                            Messenger.SendMovePPUpdate(victim, i);
        //                            break;
        //                        }
        //                    }
        //                }
        //            }
        //            break;
        //        case Enums.MoveType.AddMP: {
        //                int randomSlot = Server.Math.Rand(0, 4);
        //                if (victim.Player.GetActiveRecruit().Moves[randomSlot].MoveNum > 0 && victim.Player.GetActiveRecruit().Moves[randomSlot].CurrentPP < victim.Player.GetActiveRecruit().Moves[randomSlot].MaxPP) {
        //                    victim.Player.GetActiveRecruit().Moves[randomSlot].CurrentPP++;
        //                    Messenger.SendMovePPUpdate(victim, randomSlot);
        //                } else {
        //                    for (int i = 0; i < victim.Player.GetActiveRecruit().Moves.Length; i++) {
        //                        if (victim.Player.GetActiveRecruit().Moves[i].MoveNum > 0 && victim.Player.GetActiveRecruit().Moves[i].CurrentPP < victim.Player.GetActiveRecruit().Moves[i].MaxPP) {
        //                            victim.Player.GetActiveRecruit().Moves[i].CurrentPP++;
        //                            Messenger.SendMovePPUpdate(victim, i);
        //                            break;
        //                        }
        //                    }
        //                }
        //            }
        //            break;
        //        case Enums.MoveType.Scripted: {

        //                if (attacker.CharacterType == Enums.CharacterType.Recruit) {
        //                    Messenger.BattleMsg(victim, ((Recruit)attacker).Owner.Player.Name + " used " + MoveManager.Moves[moveNum].Name + "!", Text.BrightRed);
        //                    Scripting.ScriptManager.InvokeSub("ScriptedMoveHitPlayer", attacker, victim, moveNum, move);
        //                } else if (attacker.CharacterType == Enums.CharacterType.MapNpc) {
        //                    Messenger.BattleMsg(victim, ((MapNpc)attacker).Name + " used " + MoveManager.Moves[moveNum].Name + "!", Text.BrightRed);
        //                    Scripting.ScriptManager.InvokeSub("ScriptedMoveHitPlayer", attacker, victim, moveNum, move);
        //                }


        //            }
        //            break;
        //    }
        //}

        //public static void MoveHitNpc(ICharacter attacker, IMap map, int npcSlot, int moveNum) {
        //    Move move = MoveManager.Moves[moveNum];
        //    switch (move.EffectType) {
        //        case Enums.MoveType.SubHP: {
        //                int damage = DamageCalculator.CalculateDamage(move.Data1, attacker.SpclAtk, attacker.Level, map.ActiveNpc[npcSlot].SpclDef);
        //                //if (damage < 0) {

        //                //}
        //                if (attacker.CharacterType == Enums.CharacterType.Recruit) {
        //                    PlayerAttackNpc(((Recruit)attacker).Owner, npcSlot, damage);
        //                }
        //                //} else if (attacker.CharacterType == Enums.CharacterType.MapNpc) {
        //                //    BattleProcessor.NpcAttackPlayer(((MapNpc)attacker).MapSlot, victim, damage);
        //                //}
        //                Messenger.SpellAnim(moveNum, map.MapID, map.ActiveNpc[npcSlot].X, map.ActiveNpc[npcSlot].Y);
        //            }
        //            break;
        //        case Enums.MoveType.AddHP: {
        //                map.ActiveNpc[npcSlot].HP += move.Data1;
        //                Messenger.SpellAnim(moveNum, map.MapID, map.ActiveNpc[npcSlot].X, map.ActiveNpc[npcSlot].Y);
        //            }
        //            break;
        //        case Enums.MoveType.Scripted: {
        //                Scripting.ScriptManager.InvokeSub("ScriptedMoveHitNpc", attacker, map, npcSlot, moveNum, move);
        //            }
        //            break;
        //    }
        //}



        #endregion

        #region (beta attack system)
        /*

            if (HitsAllies(originalMove.TargetType)) {
                if (attacker.CharacterType == Enums.CharacterType.Recruit) {//ally is any player in white zone, any player in guild or party otherwise
                    if (((Recruit)attacker).Owner.Player.GetCurrentMap().Moral == Enums.MapMoral.House || ((Recruit)attacker).Owner.Player.GetCurrentMap().Moral == Enums.MapMoral.Safe) {
                        foreach (Client i in ((Recruit)attacker).Owner.Player.GetCurrentMap().GetClients()) {
                            if (i != ((Recruit)attacker).Owner && MoveProcessor.IsInRange(originalMove.RangeType, originalMove.Range, ((Recruit)attacker).Owner.Player.X, ((Recruit)attacker).Owner.Player.Y, ((Recruit)attacker).Owner.Player.Direction, i.Player.X, i.Player.Y))// Morality Checks (same map, not getting map, access, level)
                            {
                                Messenger.BattleMsg(i, ((Recruit)attacker).Owner.Player.Name + " used " + originalMove.Name + "!", Text.BrightCyan);

                                setup = new BattleSetup(attacker, i.Player.GetActiveRecruit(), originalMove, 100, Constants.STANDARDCRITICALRATE, true);

                                //setup = (BattleSetup)Scripting.ScriptManager.InvokeFunction("BeforeMoveHits", setup); 

                                dmg = -1;

                                if (setup.Move.Accuracy < 0 || Math.Rand(1, 101) < setup.Move.Accuracy) {//attack hit
                                    hit = true;
                                    hitSomeone = true;

                                    if (setup.Multiplier > 0) {//check against absorb abilities/status/items


                                        if (setup.Move.EffectType == Enums.MoveType.SubHP) {
                                            int effectiveness = 4;


                                            if (Math.Rand(1, 1001) < setup.CriticalRate) {//criticaled
                                                
                                                critical = true;
                                                if (setup.Move.MoveCategory == Enums.MoveCategory.Physical) {

                                                    dmg = DamageCalculator.CalculateDamage(setup.Move.Data1, setup.Attacker.Atk, setup.Attacker.Level, setup.Defender.Def) * 3 / 2;
                                                } else {

                                                    dmg = DamageCalculator.CalculateDamage(setup.Move.Data1, setup.Attacker.SpclAtk, setup.Attacker.Level, setup.Defender.SpclDef) * 3 / 2;
                                                }
                                            } else {//not criticaled
                                                critical = false;

                                                if (setup.Move.MoveCategory == Enums.MoveCategory.Physical) {

                                                    dmg = DamageCalculator.CalculateDamage(setup.Move.Data1, DamageCalculator.ApplyAttackBuff(setup.Attacker.Atk, setup.Attacker.AttackBuff), setup.Attacker.Level, DamageCalculator.ApplyDefenseBuff(setup.Defender.Def, setup.Defender.DefenseBuff));
                                                } else {

                                                    dmg = DamageCalculator.CalculateDamage(setup.Move.Data1, DamageCalculator.ApplyAttackBuff(setup.Attacker.SpclAtk, setup.Attacker.SpAtkBuff), setup.Attacker.Level, DamageCalculator.ApplyDefenseBuff(setup.Defender.SpclDef, setup.Defender.SpDefBuff));
                                                }

                                            }
                                            effectiveness += (DamageCalculator.CalculateTypeMatchup(setup.Move.Element, setup.Defender.Type1) - 2);
                                            effectiveness += (DamageCalculator.CalculateTypeMatchup(setup.Move.Element, setup.Defender.Type2) - 2);


                                            

                                            dmg *= DamageCalculator.Effectiveness[effectiveness];
                                            dmg *= setup.Multiplier;
                                            dmg /= 25600;

                                            //say damage to both parties

                                            i.Player.GetActiveRecruit().HP -= dmg;

                                        } else if (setup.Move.EffectType == Enums.MoveType.Scripted) {


                                            //Scripting.ScriptManager.InvokeSub("ScriptedMove", setup.attacker, setup.defender, setup.move); ~


                                        }

                                        //after main effects made, check for HP <= 0
                                        if (i.Player.GetActiveRecruit().HP <= 0) {
                                            if (setup.KnockedOut == false)//check against hang-on-by-1-HP cases
                                            {

                                                i.Player.GetActiveRecruit().HP = 1;
                                            }
                                            //otherwise, let KnockedOut remain true

                                        } else {
                                            //character was not knocked out, set to false
                                            setup.KnockedOut = false;
                                        }

                                        //apply additional effects, if the move option was selected
                                        if (setup.Move.PerPlayer) {
                                            //Scripting.ScriptManager.InvokeSub("MoveAdditionalEffect", setup.Attacker, setup.defender, setup.move, dmg, setup.KnockedOut); ~
                                        }

                                    }
                                } else {//attack missed
                                    hit = false;
                                    // say the attack missed to both parties

                                }

                                //Scripting.ScriptManager.InvokeSub("AfterMoveDone", setup.Attacker, setup.defender, setup.move, dmg, setup.KnockedOut, hit, critical); ~

                                if (setup.KnockedOut) {
                                    HandleDeath(setup.Attacker, setup.Defender);

                                }


                            }
                        }
                    } else {
                        foreach (Client i in ((Recruit)attacker).Owner.Player.GetCurrentMap().GetClients()) {
                            if (i != ((Recruit)attacker).Owner && MoveProcessor.IsInRange(originalMove.RangeType, originalMove.Range, ((Recruit)attacker).Owner.Player.X, ((Recruit)attacker).Owner.Player.Y, ((Recruit)attacker).Owner.Player.Direction, i.Player.X, i.Player.Y))//: Morality Checks (same map, not getting map, access, level)
                            {

                                if (((Recruit)attacker).Owner.Player.GuildName == i.Player.GuildName || ((Recruit)attacker).Owner.Player.PartyID == i.Player.PartyID) {
                                    Messenger.BattleMsg(i, ((Recruit)attacker).Owner.Player.Name + " used " + originalMove.Name + "!", Text.BrightCyan);

                                    setup = new BattleSetup(attacker, i.Player.GetActiveRecruit(), originalMove, 1, Constants.STANDARDCRITICALRATE, true);

                                    //setup = (BattleSetup)Scripting.ScriptManager.InvokeFunction("BeforeMoveHits", setup);

                                    dmg = -1;

                                    if (setup.Move.Accuracy < 0 || Math.Rand(1, 101) < setup.Move.Accuracy) {//attack hit
                                        hit = true;
                                        hitSomeone = true;

                                        if (setup.Multiplier > 0) {//check against absorb abilities/status/items


                                            if (setup.Move.EffectType == Enums.MoveType.SubHP) {
                                                int effectiveness = 4;


                                                if (Math.Rand(1, 1001) < setup.CriticalRate) {//criticaled
                                                    //: say it criticaled to both parties
                                                    critical = true;
                                                    if (setup.Move.MoveCategory == Enums.MoveCategory.Physical) {

                                                        dmg = DamageCalculator.CalculateDamage(setup.Move.Data1, setup.Attacker.Atk, setup.Attacker.Level, setup.Defender.Def) * 3 / 2;
                                                    } else {

                                                        dmg = DamageCalculator.CalculateDamage(setup.Move.Data1, setup.Attacker.SpclAtk, setup.Attacker.Level, setup.Defender.SpclDef) * 3 / 2;
                                                    }
                                                } else {//not criticaled
                                                    critical = false;

                                                    if (setup.Move.MoveCategory == Enums.MoveCategory.Physical) {

                                                        dmg = DamageCalculator.CalculateDamage(setup.Move.Data1, DamageCalculator.ApplyAttackBuff(setup.Attacker.Atk, setup.Attacker.AttackBuff), setup.Attacker.Level, DamageCalculator.ApplyDefenseBuff(setup.Defender.Def, setup.Defender.DefenseBuff));
                                                    } else {

                                                        dmg = DamageCalculator.CalculateDamage(setup.Move.Data1, DamageCalculator.ApplyAttackBuff(setup.Attacker.SpclAtk, setup.Attacker.SpAtkBuff), setup.Attacker.Level, DamageCalculator.ApplyDefenseBuff(setup.Defender.SpclDef, setup.Defender.SpDefBuff));
                                                    }

                                                }
                                                effectiveness += (DamageCalculator.CalculateTypeMatchup(setup.Move.Element, setup.Defender.Type1) - 2);
                                                effectiveness += (DamageCalculator.CalculateTypeMatchup(setup.Move.Element, setup.Defender.Type2) - 2);


                                                //: say effectiveness to both parties

                                                dmg *= DamageCalculator.Effectiveness[effectiveness];
                                                dmg *= setup.Multiplier;
                                                dmg /= 25600;

                                                //say damage to both parties

                                                i.Player.GetActiveRecruit().HP -= dmg;

                                            } else if (setup.Move.EffectType == Enums.MoveType.Scripted) {


                                                //Scripting.ScriptManager.InvokeSub("ScriptedMove", setup.attacker, setup.defender, setup.move); ~


                                            }

                                            //after main effects made, check for HP <= 0
                                            if (i.Player.GetActiveRecruit().HP <= 0) {
                                                if (setup.KnockedOut == false)//check against hang-on-by-1-HP cases
                                                {

                                                    i.Player.GetActiveRecruit().HP = 1;
                                                }
                                                //otherwise, let KnockedOut remain true

                                            } else {
                                                //character was not knocked out, set to false
                                                setup.KnockedOut = false;
                                            }

                                            //apply additional effects, if the move option was selected
                                            if (setup.Move.PerPlayer) {
                                                //Scripting.ScriptManager.InvokeSub("MoveAdditionalEffect", setup.Attacker, setup.defender, setup.move, dmg, setup.KnockedOut); ~
                                            }

                                        }
                                    } else {//attack missed
                                        hit = false;
                                        //: say the attack missed to both parties

                                    }

                                    //Scripting.ScriptManager.InvokeSub("AfterMoveDone", setup.Attacker, setup.defender, setup.move, dmg, setup.KnockedOut, hit, critical); ~

                                    if (setup.KnockedOut) {
                                        HandleDeath(setup.Attacker, setup.Defender);

                                    }


                                }
                            }
                        }


                    }

                } else {//ally is any NPC
                    IMap currentMap = MapManager.ActiveMaps[((MapNpc)attacker).MapID];

                    for (int i = 0; i < currentMap.ActiveNpc.Length; i++) {
                        if (currentMap.ActiveNpc[i].Num > 0 && currentMap.ActiveNpc[i].HP > 0 && i != ((MapNpc)attacker).MapSlot && MoveProcessor.IsInRange(originalMove.RangeType, originalMove.Range, ((MapNpc)attacker).X, ((MapNpc)attacker).Y, ((MapNpc)attacker).Direction, currentMap.ActiveNpc[i].X, currentMap.ActiveNpc[i].Y)) {

                            setup = new BattleSetup(attacker, currentMap.ActiveNpc[i], originalMove, 1, Constants.STANDARDCRITICALRATE, true);

                            //setup = (BattleSetup)Scripting.ScriptManager.InvokeFunction("BeforeMoveHits", setup);

                            dmg = -1;

                            if (setup.Move.Accuracy < 0 || Math.Rand(1, 101) < setup.Move.Accuracy) {//attack hit
                                hit = true;
                                hitSomeone = true;

                                if (setup.Multiplier > 0) {//check against absorb abilities/status/items


                                    if (setup.Move.EffectType == Enums.MoveType.SubHP) {
                                        int effectiveness = 4;


                                        if (Math.Rand(1, 1001) < setup.CriticalRate) {//criticaled
                                            //: dont say it criticaled
                                            critical = true;
                                            if (setup.Move.MoveCategory == Enums.MoveCategory.Physical) {

                                                dmg = DamageCalculator.CalculateDamage(setup.Move.Data1, setup.Attacker.Atk, setup.Attacker.Level, setup.Defender.Def) * 3 / 2;
                                            } else {

                                                dmg = DamageCalculator.CalculateDamage(setup.Move.Data1, setup.Attacker.SpclAtk, setup.Attacker.Level, setup.Defender.SpclDef) * 3 / 2;
                                            }
                                        } else {//not criticaled
                                            critical = false;

                                            if (setup.Move.MoveCategory == Enums.MoveCategory.Physical) {

                                                dmg = DamageCalculator.CalculateDamage(setup.Move.Data1, DamageCalculator.ApplyAttackBuff(setup.Attacker.Atk, setup.Attacker.AttackBuff), setup.Attacker.Level, DamageCalculator.ApplyDefenseBuff(setup.Defender.Def, setup.Defender.DefenseBuff));
                                            } else {

                                                dmg = DamageCalculator.CalculateDamage(setup.Move.Data1, DamageCalculator.ApplyAttackBuff(setup.Attacker.SpclAtk, setup.Attacker.SpAtkBuff), setup.Attacker.Level, DamageCalculator.ApplyDefenseBuff(setup.Defender.SpclDef, setup.Defender.SpDefBuff));
                                            }

                                        }
                                        effectiveness += (DamageCalculator.CalculateTypeMatchup(setup.Move.Element, setup.Defender.Type1) - 2);
                                        effectiveness += (DamageCalculator.CalculateTypeMatchup(setup.Move.Element, setup.Defender.Type2) - 2);


                                        //: dont say effectiveness

                                        dmg *= DamageCalculator.Effectiveness[effectiveness];
                                        dmg *= setup.Multiplier;
                                        dmg /= 25600;

                                        //dont say damage

                                        currentMap.ActiveNpc[i].HP -= dmg;

                                    } else if (setup.Move.EffectType == Enums.MoveType.Scripted) {


                                        //Scripting.ScriptManager.InvokeSub("ScriptedMove", setup.attacker, setup.defender, setup.move); ~


                                    }

                                    //after main effects made, check for HP <= 0
                                    if (currentMap.ActiveNpc[i].HP <= 0) {
                                        if (setup.KnockedOut == false)//check against hang-on-by-1-HP cases
                                        {

                                            currentMap.ActiveNpc[i].HP = 1;
                                        }
                                        //otherwise, let KnockedOut remain true

                                    } else {
                                        //character was not knocked out, set to false
                                        setup.KnockedOut = false;
                                    }

                                    //apply additional effects, if the move option was selected
                                    if (setup.Move.PerPlayer) {
                                        //Scripting.ScriptManager.InvokeSub("MoveAdditionalEffect", setup.Attacker, setup.defender, setup.move, dmg, setup.KnockedOut); ~
                                    }

                                }
                            } else {//attack missed
                                hit = false;
                                //: dont say the attack missed

                            }

                            //Scripting.ScriptManager.InvokeSub("AfterMoveDone", setup.Attacker, setup.defender, setup.move, dmg, setup.KnockedOut, hit, critical); ~

                            if (setup.KnockedOut) {
                                HandleDeath(setup.Attacker, setup.Defender);

                            }



                        }
                    }
                }
            }

            if (HitsFoes(originalMove.TargetType)) {
                if (attacker.CharacterType == Enums.CharacterType.Recruit) {//a player not in the same guild/party and any NPC

                    if (((Recruit)attacker).Owner.Player.GetCurrentMap().Moral != Enums.MapMoral.House && ((Recruit)attacker).Owner.Player.GetCurrentMap().Moral != Enums.MapMoral.Safe) {//attack all players not in guild/party
                        foreach (Client i in ((Recruit)attacker).Owner.Player.GetCurrentMap().GetClients()) {
                            if (i != ((Recruit)attacker).Owner && MoveProcessor.IsInRange(originalMove.RangeType, originalMove.Range, ((Recruit)attacker).Owner.Player.X, ((Recruit)attacker).Owner.Player.Y, ((Recruit)attacker).Owner.Player.Direction, i.Player.X, i.Player.Y))//: Morality Checks (same map, not getting map, access, level)
                            {

                                if (((Recruit)attacker).Owner.Player.GuildName != i.Player.GuildName && ((Recruit)attacker).Owner.Player.PartyID != i.Player.PartyID) {
                                    Messenger.BattleMsg(i, ((Recruit)attacker).Owner.Player.Name + " used " + originalMove.Name + "!", Text.BrightCyan);

                                    setup = new BattleSetup(attacker, i.Player.GetActiveRecruit(), originalMove, 1, Constants.STANDARDCRITICALRATE, true);

                                    //setup = (BattleSetup)Scripting.ScriptManager.InvokeFunction("BeforeMoveHits", setup);

                                    dmg = -1;

                                    if (setup.Move.Accuracy < 0 || Math.Rand(1, 101) < setup.Move.Accuracy) {//attack hit
                                        hit = true;
                                        hitSomeone = true;

                                        if (setup.Multiplier > 0) {//check against absorb abilities/status/items


                                            if (setup.Move.EffectType == Enums.MoveType.SubHP) {
                                                int effectiveness = 4;


                                                if (Math.Rand(1, 1001) < setup.CriticalRate) {//criticaled
                                                    //: say it criticaled to both parties
                                                    critical = true;
                                                    if (setup.Move.MoveCategory == Enums.MoveCategory.Physical) {

                                                        dmg = DamageCalculator.CalculateDamage(setup.Move.Data1, setup.Attacker.Atk, setup.Attacker.Level, setup.Defender.Def) * 3 / 2;
                                                    } else {

                                                        dmg = DamageCalculator.CalculateDamage(setup.Move.Data1, setup.Attacker.SpclAtk, setup.Attacker.Level, setup.Defender.SpclDef) * 3 / 2;
                                                    }
                                                } else {//not criticaled
                                                    critical = false;

                                                    if (setup.Move.MoveCategory == Enums.MoveCategory.Physical) {

                                                        dmg = DamageCalculator.CalculateDamage(setup.Move.Data1, DamageCalculator.ApplyAttackBuff(setup.Attacker.Atk, setup.Attacker.AttackBuff), setup.Attacker.Level, DamageCalculator.ApplyDefenseBuff(setup.Defender.Def, setup.Defender.DefenseBuff));
                                                    } else {

                                                        dmg = DamageCalculator.CalculateDamage(setup.Move.Data1, DamageCalculator.ApplyAttackBuff(setup.Attacker.SpclAtk, setup.Attacker.SpAtkBuff), setup.Attacker.Level, DamageCalculator.ApplyDefenseBuff(setup.Defender.SpclDef, setup.Defender.SpDefBuff));
                                                    }

                                                }
                                                effectiveness += (DamageCalculator.CalculateTypeMatchup(setup.Move.Element, setup.Defender.Type1) - 2);
                                                effectiveness += (DamageCalculator.CalculateTypeMatchup(setup.Move.Element, setup.Defender.Type2) - 2);


                                                //: say effectiveness to both parties

                                                dmg *= DamageCalculator.Effectiveness[effectiveness];
                                                dmg *= setup.Multiplier;
                                                dmg /= 25600;

                                                //say damage to both parties

                                                i.Player.GetActiveRecruit().HP -= dmg;

                                            } else if (setup.Move.EffectType == Enums.MoveType.Scripted) {


                                                //Scripting.ScriptManager.InvokeSub("ScriptedMove", setup.attacker, setup.defender, setup.move); ~


                                            }

                                            //after main effects made, check for HP <= 0
                                            if (i.Player.GetActiveRecruit().HP <= 0) {
                                                if (setup.KnockedOut == false)//check against hang-on-by-1-HP cases
                                                {

                                                    i.Player.GetActiveRecruit().HP = 1;
                                                }
                                                //otherwise, let KnockedOut remain true

                                            } else {
                                                //character was not knocked out, set to false
                                                setup.KnockedOut = false;
                                            }

                                            //apply additional effects, if the move option was selected
                                            if (setup.Move.PerPlayer) {
                                                //Scripting.ScriptManager.InvokeSub("MoveAdditionalEffect", setup.Attacker, setup.defender, setup.move, dmg, setup.KnockedOut); ~
                                            }

                                        }
                                    } else {//attack missed
                                        hit = false;
                                        //: say the attack missed to both parties

                                    }

                                    //Scripting.ScriptManager.InvokeSub("AfterMoveDone", setup.Attacker, setup.defender, setup.move, dmg, setup.KnockedOut, hit, critical); ~

                                    if (setup.KnockedOut) {
                                        HandleDeath(setup.Attacker, setup.Defender);

                                    }


                                }
                            }
                        }



                    }

                    //attack all NPCs

                    IMap currentMap = ((Recruit)attacker).Owner.Player.Map;

                    for (int i = 0; i < currentMap.ActiveNpc.Length; i++) {
                        if (currentMap.ActiveNpc[i].Num > 0 && currentMap.ActiveNpc[i].HP > 0 && MoveProcessor.IsInRange(originalMove.RangeType, originalMove.Range, ((MapNpc)attacker).X, ((MapNpc)attacker).Y, ((MapNpc)attacker).Direction, currentMap.ActiveNpc[i].X, currentMap.ActiveNpc[i].Y)) {

                            setup = new BattleSetup(attacker, currentMap.ActiveNpc[i], originalMove, 1, Constants.STANDARDCRITICALRATE, true);

                            //setup = (BattleSetup)Scripting.ScriptManager.InvokeFunction("BeforeMoveHits", setup);

                            dmg = -1;

                            if (setup.Move.Accuracy < 0 || Math.Rand(1, 101) < setup.Move.Accuracy) {//attack hit
                                hit = true;
                                hitSomeone = true;

                                if (setup.Multiplier > 0) {//check against absorb abilities/status/items


                                    if (setup.Move.EffectType == Enums.MoveType.SubHP) {
                                        int effectiveness = 4;


                                        if (Math.Rand(1, 1001) < setup.CriticalRate) {//criticaled
                                            //: say it criticaled to attacker
                                            critical = true;
                                            if (setup.Move.MoveCategory == Enums.MoveCategory.Physical) {

                                                dmg = DamageCalculator.CalculateDamage(setup.Move.Data1, setup.Attacker.Atk, setup.Attacker.Level, setup.Defender.Def) * 3 / 2;
                                            } else {

                                                dmg = DamageCalculator.CalculateDamage(setup.Move.Data1, setup.Attacker.SpclAtk, setup.Attacker.Level, setup.Defender.SpclDef) * 3 / 2;
                                            }
                                        } else {//not criticaled
                                            critical = false;

                                            if (setup.Move.MoveCategory == Enums.MoveCategory.Physical) {

                                                dmg = DamageCalculator.CalculateDamage(setup.Move.Data1, DamageCalculator.ApplyAttackBuff(setup.Attacker.Atk, setup.Attacker.AttackBuff), setup.Attacker.Level, DamageCalculator.ApplyDefenseBuff(setup.Defender.Def, setup.Defender.DefenseBuff));
                                            } else {

                                                dmg = DamageCalculator.CalculateDamage(setup.Move.Data1, DamageCalculator.ApplyAttackBuff(setup.Attacker.SpclAtk, setup.Attacker.SpAtkBuff), setup.Attacker.Level, DamageCalculator.ApplyDefenseBuff(setup.Defender.SpclDef, setup.Defender.SpDefBuff));
                                            }

                                        }
                                        effectiveness += (DamageCalculator.CalculateTypeMatchup(setup.Move.Element, setup.Defender.Type1) - 2);
                                        effectiveness += (DamageCalculator.CalculateTypeMatchup(setup.Move.Element, setup.Defender.Type2) - 2);


                                        //: say effectiveness to attacker

                                        dmg *= DamageCalculator.Effectiveness[effectiveness];
                                        dmg *= setup.Multiplier;
                                        dmg /= 25600;

                                        //say damage to attacker

                                        currentMap.ActiveNpc[i].HP -= dmg;

                                    } else if (setup.Move.EffectType == Enums.MoveType.Scripted) {


                                        //Scripting.ScriptManager.InvokeSub("ScriptedMove", setup.attacker, setup.defender, setup.move); ~


                                    }

                                    //after main effects made, check for HP <= 0
                                    if (currentMap.ActiveNpc[i].HP <= 0) {
                                        if (setup.KnockedOut == false)//check against hang-on-by-1-HP cases
                                        {

                                            currentMap.ActiveNpc[i].HP = 1;
                                        }
                                        //otherwise, let KnockedOut remain true

                                    } else {
                                        //character was not knocked out, set to false
                                        setup.KnockedOut = false;
                                    }

                                    //apply additional effects, if the move option was selected
                                    if (setup.Move.PerPlayer) {
                                        //Scripting.ScriptManager.InvokeSub("MoveAdditionalEffect", setup.Attacker, setup.defender, setup.move, dmg, setup.KnockedOut); ~
                                    }

                                }
                            } else {//attack missed
                                hit = false;
                                //: say the attack missed to attacker

                            }

                            //Scripting.ScriptManager.InvokeSub("AfterMoveDone", setup.Attacker, setup.defender, setup.move, dmg, setup.KnockedOut, hit, critical); ~

                            if (setup.KnockedOut) {
                                HandleDeath(setup.Attacker, setup.Defender);

                            }



                        }
                    }

                } else {//foe is any player



                }

            }*/
        #endregion

        #region Pre-Attack Checks

        //public static bool CanPlayerAttackPlayer(Client Attacker, Client Victim) {
        //int AttackSpeed = 0;

        //if (Attacker.Player.GetActiveRecruit().HeldItemSlot > 0) {
        //    AttackSpeed = ItemManager.Items[Attacker.Player.GetActiveRecruit().HeldItem.Num].AttackSpeed;

        //} else {
        //    AttackSpeed = 1000;
        //}

        // int SpeedTime = GetPlayerSPEED(Attacker)
        // If SpeedTime > 200 Then
        //     SpeedTime = 200
        // End If
        // AttackSpeed = AttackSpeed - SpeedTime

        // Check for subscript out of range 
        //if (NetworkManager.IsPlaying(Attacker) == false || NetworkManager.IsPlaying(Victim) == false) {
        //    return false;
        //}

        // Make sure they have more then 0 hp (targeting check)
        //if (Victim.Player.GetActiveRecruit().HP <= 0) {
        //    return false;
        //}

        // Make sure we don't attack the player if they are switching maps (range check)
        //if (Victim.Player.mGettingMap == true) {
        //    return false;
        //}

        // Make sure they are on the same map (targeting check)
        //if ((Attacker.Player.Map == Victim.Player.Map) && (Core.GetTickCount().Elapsed(Attacker.Player.AttackTimer, AttackSpeed))) {

        // Check if at same coordinates (targeting check)
        //switch (Attacker.Player.Direction) {
        //    case Enums.Direction.Up:
        //        if (!((Victim.Player.Y + 1 == Attacker.Player.Y) & (Victim.Player.X == Attacker.Player.X))) {
        //            return false;
        //        }
        //        break;
        //    case Enums.Direction.Down:
        //        if (!((Victim.Player.Y - 1 == Attacker.Player.Y) & (Victim.Player.X == Attacker.Player.X))) {
        //            return false;
        //        }
        //        break;
        //    case Enums.Direction.Left:
        //        if (!((Victim.Player.Y == Attacker.Player.Y) & (Victim.Player.X + 1 == Attacker.Player.X))) {
        //            return false;
        //        }
        //        break;
        //    case Enums.Direction.Right:
        //        if (!((Victim.Player.Y == Attacker.Player.Y) & (Victim.Player.X - 1 == Attacker.Player.X))) {
        //            return false;
        //        }
        //        break;
        //}
        //if (Victim.Player.GetCurrentMap().Tile[Victim.Player.X, Victim.Player.Y].Type != Enums.TileType.Arena & Attacker.Player.GetCurrentMap().Tile[Attacker.Player.X, Attacker.Player.Y].Type != Enums.TileType.Arena) {
        //    // Check to make sure that they don't have access
        //    if (Ranks.IsAllowed(Attacker, Enums.Rank.Moniter) && !Settings.AdminAttack) {
        //        Messenger.PlayerMsg(Attacker, "You cannot attack any player for thou art an admin!", Text.BrightBlue);
        //    } else {
        //        // Check to make sure the victim isn't an admin
        //        if (Ranks.IsAllowed(Victim, Enums.Rank.Moniter) && !Settings.AdminAttack) {
        //            Messenger.PlayerMsg(Attacker, "You cannot attack " + Victim.Player.Name + "!", Text.BrightRed);
        //        } else {
        //            // Check if map is attackable
        //            if (Attacker.Player.GetCurrentMap().Moral == Enums.MapMoral.None || Attacker.Player.GetCurrentMap().Moral == Enums.MapMoral.NoPenalty || Victim.Player.PK == true) {
        //                //// Make sure they are high enough level
        //                if (Attacker.Player.GetActiveRecruit().Level < 10) {
        //                    Messenger.PlayerMsg(Attacker, "You are below level 10, you cannot attack another player yet!", Text.BrightRed);
        //                } else {
        //                    if (Victim.Player.GetActiveRecruit().Level < 10) {
        //                        Messenger.PlayerMsg(Attacker, Victim.Player.Name + " is below level 10, you cannot attack this player yet!", Text.BrightRed);
        //                    } else {
        //                        if (string.IsNullOrEmpty(Attacker.Player.GuildName.Trim()) && string.IsNullOrEmpty(Victim.Player.GuildName.Trim())) {
        //                            if (Attacker.Player.GuildName.Trim() != Victim.Player.GuildName.Trim()) {
        //                                return true;
        //                            } else {
        //                                Messenger.PlayerMsg(Attacker, "You cannot attack a player in the same guild as you!", Text.BrightRed);
        //                            }
        //                        } else {
        //                            return true;
        //                        }
        //                    }
        //                }
        //            } else {
        //                Messenger.PlayerMsg(Attacker, "This is a safe zone!", Text.BrightRed);
        //            }
        //        }
        //    }
        //} else {
        //    return true;
        //}
        //}
        //return false;
        //}

        //public static bool CanPlayerAttackNpc(Client Attacker, int MapNpcNum) {
        //int AttackSpeed = 0;
        // Dim SpeedTime As Long

        //if (Attacker.Player.GetActiveRecruit().HeldItemSlot > 0) {
        //    AttackSpeed = ItemManager.Items[Attacker.Player.GetActiveRecruit().HeldItem.Num].AttackSpeed;
        //} else {
        //    AttackSpeed = 1000;
        //}
        // SpeedTime = GetPlayerSPEED(Attacker)
        // If SpeedTime > 200 Then
        //     SpeedTime = 200
        // End If
        // AttackSpeed = AttackSpeed - SpeedTime

        // Check for subscript out of range
        //if (NetworkManager.IsPlaying(Attacker) == false || MapNpcNum < 0 || MapNpcNum > Constants.MAX_MAP_NPCS) {
        //    return false;
        //}

        //IMap map = Attacker.Player.GetCurrentMap();

        // Check for subscript out of range
        //if (map.ActiveNpc[MapNpcNum].Num <= 0) {
        //    return false;
        //}

        //int NPCNum = map.ActiveNpc[MapNpcNum].Num;

        // Make sure the npc isn't already dead
        //if (map.ActiveNpc[MapNpcNum].HP <= 0) {
        //    return false;
        //}

        // Make sure they are on the same map
        //if (NPCNum > 0 & Core.GetTickCount().Elapsed(Attacker.Player.AttackTimer, AttackSpeed)) {
        // Check if at same coordinates
        //switch (Attacker.Player.Direction) {
        //    case Enums.Direction.Up:
        //        if (!((map.ActiveNpc[MapNpcNum].Y + 1 == Attacker.Player.Y) && (map.ActiveNpc[MapNpcNum].X == Attacker.Player.X))) {
        //            return false;
        //        }
        //        break;
        //    case Enums.Direction.Down:
        //        if (!((map.ActiveNpc[MapNpcNum].Y - 1 == Attacker.Player.Y) && (map.ActiveNpc[MapNpcNum].X == Attacker.Player.X))) {
        //            return false;
        //        }
        //        break;
        //    case Enums.Direction.Left:
        //        if (!((map.ActiveNpc[MapNpcNum].Y == Attacker.Player.Y) && (map.ActiveNpc[MapNpcNum].X + 1 == Attacker.Player.X))) {
        //            return false;
        //        }
        //        break;
        //    case Enums.Direction.Right:
        //        if (!((map.ActiveNpc[MapNpcNum].Y == Attacker.Player.Y) && (map.ActiveNpc[MapNpcNum].X - 1 == Attacker.Player.X))) {
        //            return false;
        //        }
        //        break;
        //}

        //if (NpcManager.Npcs[NPCNum].Behaviour != Enums.NpcBehaviour.Friendly && NpcManager.Npcs[NPCNum].Behaviour != Enums.NpcBehaviour.Shopkeeper && NpcManager.Npcs[NPCNum].Behaviour != Enums.NpcBehaviour.Scripted) {
        //    return true;
        //} else {
        //    if (NpcManager.Npcs[NPCNum].Behaviour == Enums.NpcBehaviour.Scripted && Settings.Scripting == true) {
        //        Scripting.ScriptManager.InvokeSub("ScriptedNpc", Attacker, NpcManager.Npcs[NPCNum].SpawnRate, NPCNum, map, MapNpcNum);
        //    } else {
        //        Stories.Story story = new Stories.Story();
        //        Stories.StoryBuilderSegment segment = Stories.StoryBuilder.BuildStory();
        //        Stories.StoryBuilder.AppendSaySegment(segment, NpcManager.Npcs[NPCNum].Name.Trim() + ": " + NpcManager.Npcs[NPCNum].AttackSay.Trim(),
        //            Pokedex.Pokedex.GetPokemon(NpcManager.Npcs[NPCNum].Species).Mugshot, 0, 0);
        //        segment.AppendToStory(story);
        //        Stories.StoryManager.PlayStory(Attacker, story);
        // Messenger.PlayerMsg(Attacker, NpcManager.Npcs[NPCNum].Name.Trim() + " :" + NpcManager.Npcs[NPCNum].AttackSay.Trim(), Text.Green);
        //    }
        //}
        //}
        //return false;
        //}


        //public static bool CanNpcAttackTarget(IMap map, int MapNpcNum, int x, int y) {
        // Check for subscript out of range
        //if (MapNpcNum < 0 || MapNpcNum > Constants.MAX_MAP_NPCS) {
        //    return false;
        //}

        // Check for subscript out of range
        //if (map.ActiveNpc[MapNpcNum].Num <= 0) {
        //    return false;
        //}

        //int NPCNum = map.ActiveNpc[MapNpcNum].Num;

        // Make sure the npc isn't already dead
        //if (map.ActiveNpc[MapNpcNum].HP <= 0) {
        //    return false;
        //}

        // Make sure npcs dont attack more then once a second
        //if (!(Core.GetTickCount().Elapsed(map.ActiveNpc[MapNpcNum].AttackTimer, 1000))) {
        //    return false;
        //}

        //map.ActiveNpc[MapNpcNum].AttackTimer = Core.GetTickCount();

        // Make sure they are on the same map
        //if (NPCNum > 0) {
        // Check if at same coordinates
        //    if ((y + 1 == map.ActiveNpc[MapNpcNum].Y) & (x == map.ActiveNpc[MapNpcNum].X)) {
        //        return true;
        //    }
        //    if ((y - 1 == map.ActiveNpc[MapNpcNum].Y) & (x == map.ActiveNpc[MapNpcNum].X)) {
        //        return true;
        //    }
        //    if ((y == map.ActiveNpc[MapNpcNum].Y) & (x + 1 == map.ActiveNpc[MapNpcNum].X)) {
        //        return true;
        //    }
        //    if ((y == map.ActiveNpc[MapNpcNum].Y) & (x - 1 == map.ActiveNpc[MapNpcNum].X)) {
        //        return true;
        //    }
        //}
        //return false;
        //}


        //public static bool CanNpcUseMoveOnPlayer(int MapNpcNum, Client client) {
        // Check for subscript out of range
        //if (MapNpcNum < 0 || MapNpcNum > Constants.MAX_MAP_NPCS || client.IsPlaying() == false) {
        //    return false;
        //}

        //IMap map = client.Player.GetCurrentMap();

        // Check for subscript out of range
        //if (map.ActiveNpc[MapNpcNum].Num <= 0) {
        //    return false;
        //}

        //int NPCNum = map.ActiveNpc[MapNpcNum].Num;

        // Make sure the npc isn't already dead
        //if (map.ActiveNpc[MapNpcNum].HP <= 0) {
        //    return false;
        //}

        // : Make AI for npc move using
        //if (NpcManager.Npcs[map.ActiveNpc[MapNpcNum].Num].Spell <= 0) {
        //    return false;
        //}

        // Make sure npcs dont attack more then once a second ~ merge with attacktimer
        // : Add AI for npc move using
        //if (!(Core.GetTickCount().Elapsed(map.ActiveNpc[MapNpcNum].MoveTimer, NpcManager.Npcs[map.ActiveNpc[MapNpcNum].Num].Frequency))) {
        //    return false;
        //}

        // Make sure we dont attack the player if they are switching maps
        //if (client.Player.mGettingMap == true) {
        //    return false;
        //}

        //map.ActiveNpc[MapNpcNum].MoveTimer = Core.GetTickCount();

        //if (NPCNum > 0) {
        // Check if in range
        //return MoveProcessor.IsInRange(MoveManager.Moves[NpcManager.Npcs[map.ActiveNpc[MapNpcNum].Num].Spell].RangeType, MoveManager.Moves[NpcManager.Npcs[map.ActiveNpc[MapNpcNum].Num].Spell].Range, map.ActiveNpc[MapNpcNum].X, map.ActiveNpc[MapNpcNum].Y, map.ActiveNpc[MapNpcNum].Direction, client.Player.X, client.Player.Y);
        //}
        //return false;
        //}


        //public static bool CanNpcAttackPlayer(int mapNpcNum, Client client) {
        //    if (client.IsPlaying() && !client.Player.mGettingMap) {
        //        return CanNpcAttackTarget(client.Player.GetCurrentMap(), mapNpcNum, client.Player.X, client.Player.Y);
        //    } else {
        //        return false;
        //    }
        //}



        //public static bool CanAttackPlayerWithArrow(Client attacker, Client victim) {
        //IMap map = attacker.Player.GetCurrentMap();
        // Check If map Is attackable
        //if (map.Moral == Enums.MapMoral.None || map.Moral == Enums.MapMoral.NoPenalty || victim.Player.PK == true) {
        // Make sure they are high enough level
        //    if (attacker.Player.GetActiveRecruit().Level < 10) {
        //        Messenger.PlayerMsg(attacker, "You are below level 10, you cannot attack another player yet!", Text.BrightRed);
        //    } else {
        //        if (victim.Player.GetActiveRecruit().Level < 10) {
        //            Messenger.PlayerMsg(attacker, victim.Player.Name + " is below level 10, you cannot attack this player yet!", Text.BrightRed);
        //        } else {
        //            if (attacker.Player.GuildName.Trim() != "" && victim.Player.GuildName.Trim() != "") {
        //                if (attacker.Player.GuildName.Trim() != victim.Player.GuildName.Trim()) {
        //                    return true;
        //                } else {
        //                    Messenger.PlayerMsg(attacker, "You can't attack a guild member!", Text.BrightRed);
        //                }
        //            } else {
        //                return true;
        //            }
        //        }
        //    }
        //} else {
        //    Messenger.PlayerMsg(attacker, "This is a safe zone!", Text.BrightRed);
        //}
        //return false;
        //}

        //public static bool CanAttackNpcWithArrow(Client attacker, int npcSlot) {
        //int AttackSpeed = 0;

        //if (attacker.Player.GetActiveRecruit().HeldItemSlot > 0) {
        //    AttackSpeed = ItemManager.Items[attacker.Player.GetActiveRecruit().HeldItem.Num].AttackSpeed;
        //} else {
        //    AttackSpeed = 1000;
        //}

        // Check For subscript out of range
        //if (attacker.IsPlaying() == false || npcSlot < 0 || npcSlot > Constants.MAX_MAP_NPCS) {
        //    return false;
        //}

        //IMap map = attacker.Player.GetCurrentMap();

        // Check For subscript out of range
        //if (map.ActiveNpc[npcSlot].Num <= 0) {
        //    return false;
        //}

        //int NPCNum = map.ActiveNpc[npcSlot].Num;

        // Make sure the npc isn't already dead
        //if (map.ActiveNpc[npcSlot].HP <= 0) {
        //    return false;
        //}

        // Make sure they are On the same map
        //if (NPCNum > 0 & Core.GetTickCount().Elapsed(attacker.Player.AttackTimer, AttackSpeed)) {
        // Check If at same coordinates
        //switch (attacker.Player.Direction) {
        //case Enums.Direction.Up:
        //    if (NpcManager.Npcs[NPCNum].Behaviour != Enums.NpcBehaviour.Friendly & NpcManager.Npcs[NPCNum].Behaviour != Enums.NpcBehaviour.Shopkeeper & NpcManager.Npcs[NPCNum].Behaviour != Enums.NpcBehaviour.Scripted) {
        //        return true;
        //    } else {
        //        if (NpcManager.Npcs[NPCNum].Behaviour == Enums.NpcBehaviour.Scripted) {
        //            Scripting.ScriptManager.InvokeSub("ScriptedNPC", attacker, NpcManager.Npcs[NPCNum].SpawnRate);
        //        } else {
        //            Messenger.PlayerMsg(attacker, NpcManager.Npcs[NPCNum].Name.Trim() + ": " + NpcManager.Npcs[NPCNum].AttackSay.Trim(), Text.Green);
        //        }
        //    }
        //    break;
        //case Enums.Direction.Down:
        //    if (NpcManager.Npcs[NPCNum].Behaviour != Enums.NpcBehaviour.Friendly & NpcManager.Npcs[NPCNum].Behaviour != Enums.NpcBehaviour.Shopkeeper & NpcManager.Npcs[NPCNum].Behaviour != Enums.NpcBehaviour.Scripted) {
        //        return true;
        //    } else {
        //        if (NpcManager.Npcs[NPCNum].Behaviour == Enums.NpcBehaviour.Scripted) {
        //            Scripting.ScriptManager.InvokeSub("ScriptedNPC", attacker, NpcManager.Npcs[NPCNum].SpawnRate);
        //        } else {
        //            Messenger.PlayerMsg(attacker, NpcManager.Npcs[NPCNum].Name.Trim() + ": " + NpcManager.Npcs[NPCNum].AttackSay.Trim(), Text.Green);
        //        }
        //    }
        //    break;
        //case Enums.Direction.Left:
        //    if (NpcManager.Npcs[NPCNum].Behaviour != Enums.NpcBehaviour.Friendly & NpcManager.Npcs[NPCNum].Behaviour != Enums.NpcBehaviour.Shopkeeper & NpcManager.Npcs[NPCNum].Behaviour != Enums.NpcBehaviour.Scripted) {
        //        return true;
        //    } else {
        //        if (NpcManager.Npcs[NPCNum].Behaviour == Enums.NpcBehaviour.Scripted) {
        //            Scripting.ScriptManager.InvokeSub("ScriptedNPC", attacker, NpcManager.Npcs[NPCNum].SpawnRate);
        //        } else {
        //            Messenger.PlayerMsg(attacker, NpcManager.Npcs[NPCNum].Name.Trim() + ": " + NpcManager.Npcs[NPCNum].AttackSay.Trim(), Text.Green);
        //        }
        //    }
        //    break;
        //case Enums.Direction.Right:
        //    if (NpcManager.Npcs[NPCNum].Behaviour != Enums.NpcBehaviour.Friendly && NpcManager.Npcs[NPCNum].Behaviour != Enums.NpcBehaviour.Shopkeeper && NpcManager.Npcs[NPCNum].Behaviour != Enums.NpcBehaviour.Scripted) {
        //        return true;
        //    } else {
        //        if (NpcManager.Npcs[NPCNum].Behaviour == Enums.NpcBehaviour.Scripted) {
        //            Scripting.ScriptManager.InvokeSub("ScriptedNPC", attacker, NpcManager.Npcs[NPCNum].SpawnRate);
        //        } else {
        //            Messenger.PlayerMsg(attacker, NpcManager.Npcs[NPCNum].Name.Trim() + ": " + NpcManager.Npcs[NPCNum].AttackSay.Trim(), Text.Green);
        //        }
        //    }
        //    break;
        //}
        //}
        //return false;
        //}

        #endregion Pre-Attack Checks

        //public static void UseMove(ICharacter attacker, int moveSlot) {
        //    if (moveSlot > -1) {
        //        Move move = MoveManager.Moves[attacker.Moves[moveSlot].MoveNum];
        //        IMap currentMap = MapManager.RetrieveActiveMap(attacker.MapID);
        //        if (move.IsKey) {
        //            if (attacker.CharacterType == Enums.CharacterType.Recruit) {
        //                ((Recruit)attacker).UseMoveKey(currentMap, move, moveSlot);
        //            }
        //        }
        //        //Confusion check
        //        if (attacker.Moves[moveSlot].CurrentPP > 0) {
        //            attacker.Moves[moveSlot].CurrentPP--;
        //            // Attack any players that are in range
        //            switch (move.TargetType) {
        //                case Enums.MoveTarget.Foes: {
        //                //        //attacker.UseMoveOnFoes(currentMap, move, moveSlot);
        //                    }
        //                    break;
        //                case Enums.MoveTarget.User: {
        //                //        // Attack the user
        //                //        //attacker.UseMoveOnSelf(attacker.Moves[moveSlot].MoveNum);
        //                    }
        //                    break;
        //                case Enums.MoveTarget.UserAndAllies: {
        //                //        //attacker.UseMoveOnAllies(currentMap, move, moveSlot);
        //                //        // Attack the user
        //                //        //attacker.UseMoveOnSelf(attacker.Moves[moveSlot].MoveNum);
        //                    }
        //                    break;
        //                case Enums.MoveTarget.UserAndFoe: {
        //                //        // Attack the user
        //                //        ///attacker.UseMoveOnSelf(attacker.Moves[moveSlot].MoveNum);
        //                //        //attacker.UseMoveOnFoes(currentMap, move, moveSlot);
        //                    }
        //                    break;
        //                case Enums.MoveTarget.All: {
        //                //        // Attack allies
        //                //        //attacker.UseMoveOnAllies(currentMap, move, moveSlot);
        //                //        // Attack foes
        //                //        //attacker.UseMoveOnFoes(currentMap, move, moveSlot);
        //                //        // Attack the user
        //                //        //attacker.UseMoveOnSelf(attacker.Moves[moveSlot].MoveNum);
        //                    }
        //                    break;
        //                case Enums.MoveTarget.AllAlliesButUser: {
        //                //        //attacker.UseMoveOnAllies(currentMap, move, moveSlot);
        //                    }
        //                    break;
        //                case Enums.MoveTarget.AllButUser: {
        //                //        //attacker.UseMoveOnAllies(currentMap, move, moveSlot);
        //                //        //attacker.UseMoveOnFoes(currentMap, move, moveSlot);
        //                    }
        //                    break;
        //            }
        //            if (attacker.CharacterType == Enums.CharacterType.Recruit) {
        //                Messenger.SendMovePPUpdate(((Recruit)attacker).Owner, moveSlot);
        //            }
        //        }
        //    }
        //}

        //public static void DropItems(Client client) {//not the type of item drop used in PMD deaths
        //    if (Settings.Scripting == true) {
        //        Scripting.ScriptManager.InvokeSub("DropItems", client);
        //    } else {
        //        if (client.Player.Weapon > -1) {
        //            client.Player.MapDropItem(client.Player.Weapon, 0);
        //        }

        //        if (client.Player.Armor > -1) {
        //            client.Player.MapDropItem(client.Player.Armor, 0);
        //        }

        //        if (client.Player.Helmet > -1) {
        //            client.Player.MapDropItem(client.Player.Helmet, 0);
        //        }

        //        if (client.Player.Shield > -1) {
        //            client.Player.MapDropItem(client.Player.Shield, 0);
        //        }

        //        if (client.Player.Legs > -1) {
        //            client.Player.MapDropItem(client.Player.Legs, 0);
        //        }

        //        if (client.Player.Ring > -1) {
        //            client.Player.MapDropItem(client.Player.Ring, 0);
        //        }

        //        if (client.Player.Necklace > -1) {
        //            client.Player.MapDropItem(client.Player.Necklace, 0);
        //        }
        //    }
        //}

        //public static void DamagePlayer(Client victim, int damage) {
        //    DamagePlayer(victim, damage, Enums.KillType.Other);
        //}

        //public static void DamagePlayer(Client victim, int damage, Enums.KillType damageType) {
        //    IMap map = victim.Player.GetCurrentMap();

        // Say damage
        //    Messenger.BattleMsg(victim, "You took " + damage + " damage!", Text.BrightRed);

        //    if (damage >= victim.Player.GetActiveRecruit().HP) {


        // Player is dead
        //        Messenger.PlayerMsg(victim, "You have fainted!", Text.BrightRed);

        //if (map.Moral != Enums.MapMoral.NoPenalty) {
        //    DropItems(victim);

        //if (Settings.LoseExpOnDeath) {
        // Calculate exp to give attacker
        //    ulong expLoss = (victim.Player.GetActiveRecruit().Exp / 3);

        // Make sure we dont get less then 0
        //    if (expLoss < 0) {
        //        expLoss = 0;
        //    }

        //    if (expLoss == 0) {
        //        Messenger.BattleMsg(victim, "You lost no experience.", Text.BrightRed);
        //    } else {
        //        victim.Player.GetActiveRecruit().Exp -= expLoss;
        //        Messenger.BattleMsg(victim, "You lost " + expLoss + " experience.", Text.BrightRed);
        //    }
        //}
        //}

        // Warp player away
        //      if (Settings.Scripting == true) {
        //          Scripting.ScriptManager.InvokeSub("OnDeath", victim, damageType);
        //      } else {
        //          Messenger.PlayerWarp(victim, Settings.StartMap, Settings.StartX, Settings.StartY);
        //      }

        // Restore vitals
        //      victim.Player.GetActiveRecruit().HP = victim.Player.GetMaxHP();


        // If the player the attacker killed was a pk then take it away
        //      if (victim.Player.PK == true) {
        //          victim.Player.PK = false;
        //          Messenger.SendPlayerData(victim);
        //          Messenger.GlobalMsg(victim.Player.Name + " has paid the price for being an Outlaw!", Text.BrightRed);
        //      }
        //  } else {
        // Player not dead, just do the damage
        //     victim.Player.GetActiveRecruit().HP -= damage;



        //  }
        //}



        ///public static void ArrowHit(int index, Enums.TargetType targetType, int target) {
        //int damage = 0;
        //if (targetType == Enums.TargetType.Player) {
        //    if (target != index) {
        //        if (BattleProcessor.CanAttackPlayerWithArrow(index, target)) {
        //            if (!PlayerManager.Players[target].CanBlockHit()) {
        //                if (!client.Player.CanCriticalHit()) {
        //                    damage = DamageCalculator.CalculateDamage(Constants.STANDARDATTACKBASEPOWER, client.Player.GetActiveRecruit().GetAtk(),
        //                                            client.Player.GetActiveRecruit().Level, PlayerManager.Players[target].GetActiveRecruit().GetDef());
        //                    Messenger.SendDataToMap(client.Player.MapID, Messenger.CreateSoundPacket("attack"));
        //                } else {
        //                    damage = DamageCalculator.CalculateDamageCritical(Constants.STANDARDATTACKBASEPOWER, client.Player.GetActiveRecruit().GetAtk(),
        //                                            client.Player.GetActiveRecruit().Level, PlayerManager.Players[target].GetActiveRecruit().GetDef());
        //                    Messenger.BattleMsg(index, "A critical hit!", Text.BrightCyan);
        //                    Messenger.BattleMsg(target, client.Player.Name + " gets a critical hit!", Text.BrightCyan);
        //                }

        //                if (damage > 0) {
        //                    BattleProcessor.PlayerAttackPlayer(index, target, damage);
        //                    Scripting.ScriptManager.InvokeSub("OnArrowHitPlayer", index, target, damage, client.Player.GetCurrentMap());
        //                } else {
        //                    Messenger.BattleMsg(index, PlayerManager.Players[target].Name + " took no damage!", Text.BrightRed);
        //                    Messenger.BattleMsg(target, "You took no damage!", Text.BrightRed);

        //                    Messenger.SendDataToMap(client.Player.MapID, Messenger.CreateSoundPacket("miss"));
        //                }
        //            } else {
        //                Messenger.BattleMsg(index, PlayerManager.Players[target].Name + " avoided the attack!", Text.BrightCyan);
        //                Messenger.BattleMsg(target, "You avoided the attack!", Text.BrightCyan);

        //                Messenger.SendDataToMap(client.Player.MapID, Messenger.CreateSoundPacket("miss"));
        //            }
        //            return;
        //        }
        //    }
        //} else if (targetType == Enums.TargetType.Npc) {
        //    if (BattleProcessor.CanAttackNpcWithArrow(index, target)) {
        //        IMap map = client.Player.GetCurrentMap();
        //        if (!client.Player.CanCriticalHit()) {
        //            damage = DamageCalculator.CalculateDamage(Constants.STANDARDATTACKBASEPOWER, client.Player.GetActiveRecruit().GetAtk(),
        //                                client.Player.GetActiveRecruit().Level, map.ActiveNpc[target].Def);
        //            Messenger.SendDataToMap(client.Player.MapID, Messenger.CreateSoundPacket("attack"));
        //        } else {
        //            damage = DamageCalculator.CalculateDamageCritical(Constants.STANDARDATTACKBASEPOWER, client.Player.GetActiveRecruit().GetAtk(),
        //                                client.Player.GetActiveRecruit().Level, map.ActiveNpc[target].Def);
        //            Messenger.BattleMsg(index, "A critical hit!", Text.BrightCyan);

        //            Messenger.SendDataToMap(client.Player.MapID, Messenger.CreateSoundPacket("critical"));
        //        }

        //        if (damage > 0) {
        //            BattleProcessor.PlayerAttackNpc(index, target, damage);
        //            Messenger.SendDataTo(index, TcpPacket.CreatePacket("blitplayerdmg", damage.ToString(), target.ToString()));
        //            Scripting.ScriptManager.InvokeSub("OnArrowHitNpc", index, target, damage, map, map.ActiveNpc[target].Num);
        //        } else {
        //            Messenger.BattleMsg(index, "Your attack does nothing.", Text.BrightRed);
        //            Messenger.SendDataTo(index, TcpPacket.CreatePacket("blitplayerdmg", damage.ToString(), target.ToString()));
        //            Messenger.SendDataToMap(client.Player.MapID, Messenger.CreateSoundPacket("miss"));
        //        }
        //        return;
        //    }
        //}
        //}

        //public static void PlayerAttackPlayer(Client attacker, Client victim, int damage) {
        //ulong Exp = 0;
        ////int n = 0;

        //// Check for subscript out of range
        //if (NetworkManager.IsPlaying(attacker) == false || NetworkManager.IsPlaying(victim) == false || damage < 0) {
        //    return;
        //}

        //// Check for weapon ~never used
        ////if (attacker.Player.Weapon > 0)
        ////{
        ////    n = attacker.Player.Inventory[attacker.Player.Weapon].Num;
        ////}
        ////else
        ////{
        ////    n = 0;
        ////}

        //// Send this packet so they can see the person attacking
        //Messenger.SendDataToMapBut(attacker, attacker.Player.MapID, Messenger.CreateAttackPacket(attacker));

        //if (attacker.Player.GetCurrentMap().Tile[attacker.Player.X, attacker.Player.Y].Type != Enums.TileType.Arena && victim.Player.GetCurrentMap().Tile[victim.Player.X, victim.Player.Y].Type != Enums.TileType.Arena) {
        //    if (Settings.Scripting == true) {
        //        Scripting.ScriptManager.InvokeSub("OnPlayerAttackPlayer", victim, attacker, victim.Player.GetCurrentMap(), damage);
        //    }
        //    if (damage >= victim.Player.GetActiveRecruit().HP) {
        //        // Set HP to nothing
        //        victim.Player.GetActiveRecruit().HP = 0;

        //        // Check for a weapon and say damage
        //        Messenger.BattleMsg(attacker, "You hit " + victim.Player.Name + " for " + damage + " damage.", Text.BrightBlue);
        //        Messenger.BattleMsg(victim, attacker.Player.Name + " hit you for " + damage + " damage.", Text.BrightRed);

        //        // Player is dead
        //        //Messenger.GlobalMsg("Oh no! " + victim.Player.mName + " has fainted!", Text.BrightRed);
        //        Messenger.PlayerMsg(victim, "You have fainted!", Text.BrightRed);
        //        Messenger.MapMsg(attacker.Player.MapID, "Oh no! " + victim.Player.Name + " fainted!", Text.BrightRed);

        //        //if (victim.Player.GetCurrentMap().Moral != Enums.MapMoral.NoPenalty)
        //        //{
        //        //    DropItems(victim);

        //        //if (Settings.LoseExpOnDeath) {
        //        // Calculate exp to give attacker
        //        //    Exp = victim.Player.GetActiveRecruit().Exp / 10;

        //        // Make sure we dont get less then 0
        //        //    if (Exp < 0) {
        //        //        Exp = 0;
        //        //    }

        //        //    if (victim.Player.GetActiveRecruit().Level == Exp.ExpManager.Exp.MaxLevels) {
        //        //        Messenger.BattleMsg(victim, "You cant lose any experience!", Text.BrightRed);
        //        //        Messenger.BattleMsg(attacker, victim.Player.Name + " is the max level!", Text.BrightBlue);
        //        //    } else {
        //        //        if (Exp == 0) {
        //        //            //Messenger.BattleMsg(victim, "You lost no experience.", Text.BrightRed);
        //        //            //Messenger.BattleMsg(attacker, "You received no experience.", Text.BrightBlue);
        //        //        } else {
        //        //            //victim.Player.GetActiveRecruit().Exp -= Exp;
        //        //            //Messenger.BattleMsg(victim, "You lost " + Exp + " experience.", Text.BrightRed);
        //        //            //attacker.Player.GetActiveRecruit().Exp += Exp;
        //        //            //Messenger.BattleMsg(attacker, "You got " + Exp + " experience for killing " + victim.Player.Name + ".", Text.BrightBlue);
        //        //        }
        //        //    }
        //        //}
        //        //}

        //        // Warp player away
        //        if (Settings.Scripting == true) {
        //            //Scripting.ScriptManager.InvokeSub("OnDeath", victim, Enums.KillType.Player);
        //        } else {
        //            //Messenger.PlayerWarp(victim, Settings.StartMap, Settings.StartX, Settings.StartY);
        //        }

        //        // Restore vitals

        //        victim.Player.GetActiveRecruit().HP = victim.Player.GetMaxHP();
        //        

        //        // Check for a level up
        //        CheckPlayerLevelUp(attacker);

        //        if (victim.Player.PK == false) {
        //            //if (attacker.Player.PK == false) {
        //            //    attacker.Player.PK = true;
        //            //    Messenger.SendPlayerData(attacker);
        //            //    Messenger.GlobalMsg(attacker.Player.Name + " has been deemed an Outlaw!", Text.BrightRed);
        //            //}
        //        } else {
        //            //victim.Player.PK = false;
        //            //Messenger.SendPlayerData(victim);
        //            //Messenger.GlobalMsg(victim.Player.Name + " has paid the price for being an Outlaw!", Text.BrightRed);
        //        }
        //    } else {
        //        // Player not dead, just do the damage
        //        victim.Player.GetActiveRecruit().HP -= damage;
        //        

        //        // Check for a weapon and say damage
        //        Messenger.BattleMsg(attacker, "You hit " + victim.Player.Name + " for " + damage + " damage.", Text.BrightBlue);
        //        Messenger.BattleMsg(victim, attacker.Player.Name + " hit you for " + damage + " damage.", Text.BrightRed);
        //    }
        //} else if (attacker.Player.GetCurrentMap().Tile[attacker.Player.X, attacker.Player.Y].Type == Enums.TileType.Arena && victim.Player.GetCurrentMap().Tile[victim.Player.X, victim.Player.Y].Type == Enums.TileType.Arena) {
        //    if (damage >= victim.Player.GetActiveRecruit().HP) {
        //        // Set HP to nothing
        //        attacker.Player.GetActiveRecruit().HP = 0;

        //        // Check for a weapon and say damage
        //        Messenger.BattleMsg(attacker, "You hit " + victim.Player.Name + " for " + damage + " damage.", Text.BrightBlue);
        //        Messenger.BattleMsg(victim, attacker.Player.Name + " hit you for " + damage + " damage.", Text.BrightRed);

        //        if (Settings.Scripting == true) {
        //            //Scripting.ScriptManager.InvokeSub("OnArenaDeath", victim, attacker, victim.Player.GetCurrentMap());
        //        } else {
        //            //// Player is dead
        //            //Messenger.GlobalMsg(victim.Player.Name + " has been knocked out of the arena by " + attacker.Player.Name, Text.BrightRed);
        //        }
        //        // Warp player away
        //        Messenger.PlayerWarp(victim, victim.Player.GetCurrentMap().Tile[victim.Player.X, victim.Player.Y].Data1, victim.Player.GetCurrentMap().Tile[victim.Player.X, victim.Player.Y].Data2, victim.Player.GetCurrentMap().Tile[victim.Player.X, victim.Player.Y].Data3);

        //        // Restore vitals
        //        victim.Player.GetActiveRecruit().HP = victim.Player.GetMaxHP();
        //        

        //    } else {
        //        // Player not dead, just do the damage
        //        victim.Player.GetActiveRecruit().HP -= damage;
        //        

        //        // Check for a weapon and say damage
        //        Messenger.BattleMsg(attacker, victim.Player.Name + " took " + damage + " damage!", Text.BrightBlue);
        //        Messenger.BattleMsg(victim, /*attacker.Player.Name + */ "You took " + damage + " damage!", Text.BrightRed);
        //    }
        //}

        //// Reset timer for attacking
        //attacker.Player.AttackTimer = Core.GetTickCount();
        //Messenger.PlaySoundToMap(victim.Player.MapID, "pain.wav");


        #endregion



        #region New Area

        public static void HandleItemUse(InventoryItem item, int invNum, BattleSetup setup) {

            // Prevent hacking
            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                if (invNum != -1 && (invNum < 1 || invNum > ((Recruit)setup.Attacker).Owner.Player.MaxInv)) {
                    Messenger.HackingAttempt(((Recruit)setup.Attacker).Owner, "Invalid InvNum");
                    return;
                }
            }


            if (item.Num < 0 || item.Num >= Items.ItemManager.Items.MaxItems) {
                if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                    Messenger.HackingAttempt(((Recruit)setup.Attacker).Owner, "Invalid ItemNum");
                }
                return;
            }


            setup.AttackerMap.ProcessingPaused = false;


            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                if (Ranks.IsDisallowed(((Recruit)setup.Attacker).Owner, Enums.Rank.Moniter) || ((Recruit)setup.Attacker).Owner.Player.ProtectionOff) {
                    ((Recruit)setup.Attacker).Owner.Player.Hunted = true;
                    PacketBuilder.AppendHunted(((Recruit)setup.Attacker).Owner, setup.PacketStack);
                }
            }

            if (!(bool)Scripting.ScriptManager.InvokeFunction("CanUseItem", setup, item)) {
                return;
            }


            // Find out what kind of item it is
            switch (ItemManager.Items[item.Num].Type) {
                case Enums.ItemType.PotionAddHP: {
                        setup.Attacker.HP += ItemManager.Items[item.Num].Data1;
                        if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                            ((Recruit)setup.Attacker).Belly += ItemManager.Items[item.Num].Data2;
                            if (((Recruit)setup.Attacker).Belly > ((Recruit)setup.Attacker).MaxBelly) {
                                ((Recruit)setup.Attacker).Belly = ((Recruit)setup.Attacker).MaxBelly;
                            }
                            PacketBuilder.AppendBelly(((Recruit)setup.Attacker).Owner, setup.PacketStack);
                            if (ItemManager.Items[item.Num].StackCap > 0) {
                                ((Recruit)setup.Attacker).Owner.Player.TakeItemSlot(invNum, 1, true);
                            } else {
                                ((Recruit)setup.Attacker).Owner.Player.TakeItemSlot(invNum, 0, true);
                            }
                        } else {
                            if (invNum != -1) {
                                setup.Attacker.TakeHeldItem(1);
                            }
                        }
                    }
                    break;
                case Enums.ItemType.PotionAddMP: {
                        for (int i = 0; i < Constants.MAX_PLAYER_MOVES; i++) {
                            if (setup.Attacker.Moves[i] != null) {
                                setup.Attacker.Moves[i].CurrentPP += ItemManager.Items[item.Num].Data1;
                                if (setup.Attacker.Moves[i].CurrentPP > setup.Attacker.Moves[i].MaxPP) {
                                    setup.Attacker.Moves[i].CurrentPP = setup.Attacker.Moves[i].MaxPP;
                                }

                            }
                        }
                        if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                            PacketBuilder.AppendPlayerMoves(((Recruit)setup.Attacker).Owner, setup.PacketStack);
                            ((Recruit)setup.Attacker).Belly += ItemManager.Items[item.Num].Data2;
                            if (((Recruit)setup.Attacker).Belly > ((Recruit)setup.Attacker).MaxBelly) {
                                ((Recruit)setup.Attacker).Belly = ((Recruit)setup.Attacker).MaxBelly;
                            }
                            PacketBuilder.AppendBelly(((Recruit)setup.Attacker).Owner, setup.PacketStack);
                            if (ItemManager.Items[item.Num].StackCap > 0) {
                                ((Recruit)setup.Attacker).Owner.Player.TakeItemSlot(invNum, 1, true);
                            } else {
                                ((Recruit)setup.Attacker).Owner.Player.TakeItemSlot(invNum, 0, true);
                            }
                        } else {
                            if (invNum != -1) {
                                setup.Attacker.TakeHeldItem(1);
                            }
                        }
                    }
                    break;
                case Enums.ItemType.PotionAddBelly: {
                        if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                            if (((Recruit)setup.Attacker).Belly < ((Recruit)setup.Attacker).MaxBelly) {
                                ((Recruit)setup.Attacker).MaxBelly += ItemManager.Items[item.Num].Data2;
                                ((Recruit)setup.Attacker).Belly += ItemManager.Items[item.Num].Data1;
                            } else {
                                ((Recruit)setup.Attacker).MaxBelly += ItemManager.Items[item.Num].Data3;
                                ((Recruit)setup.Attacker).Belly += ItemManager.Items[item.Num].Data1;
                            }
                            if (((Recruit)setup.Attacker).MaxBelly > 200) {
                                ((Recruit)setup.Attacker).MaxBelly = 200;
                            }
                            if (((Recruit)setup.Attacker).Belly > ((Recruit)setup.Attacker).MaxBelly) {
                                ((Recruit)setup.Attacker).Belly = ((Recruit)setup.Attacker).MaxBelly;
                            }
                            PacketBuilder.AppendBelly(((Recruit)setup.Attacker).Owner, setup.PacketStack);
                            if (ItemManager.Items[item.Num].StackCap > 0) {
                                ((Recruit)setup.Attacker).Owner.Player.TakeItemSlot(invNum, 1, true);
                            } else {
                                ((Recruit)setup.Attacker).Owner.Player.TakeItemSlot(invNum, 0, true);
                            }
                        } else {
                            if (invNum != -1) {
                                setup.Attacker.TakeHeldItem(1);
                            }
                        }
                    }
                    break;
                case Enums.ItemType.Key: {
                        if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                            int x = 0;
                            int y = 0;
                            IMap map = ((Recruit)setup.Attacker).Owner.Player.GetCurrentMap();
                            switch (((Recruit)setup.Attacker).Direction) {
                                case Enums.Direction.Up: {
                                        if (((Recruit)setup.Attacker).Y > 0) {
                                            x = ((Recruit)setup.Attacker).X;
                                            y = ((Recruit)setup.Attacker).Y - 1;
                                        } else {
                                            return;
                                        }
                                    }
                                    break;
                                case Enums.Direction.Down: {
                                        if (((Recruit)setup.Attacker).Y < map.MaxY) {
                                            x = ((Recruit)setup.Attacker).X;
                                            y = ((Recruit)setup.Attacker).Y + 1;
                                        } else {
                                            return;
                                        }
                                    }
                                    break;
                                case Enums.Direction.Left: {
                                        if (((Recruit)setup.Attacker).X > 0) {
                                            x = ((Recruit)setup.Attacker).X - 1;
                                            y = ((Recruit)setup.Attacker).Y;
                                        } else {
                                            return;
                                        }
                                    }
                                    break;
                                case Enums.Direction.Right: {
                                        if (((Recruit)setup.Attacker).X < map.MaxX) {
                                            x = ((Recruit)setup.Attacker).X + 1;
                                            y = ((Recruit)setup.Attacker).Y;
                                        } else {
                                            return;
                                        }
                                    }
                                    break;
                            }

                            // Check if a key exists
                            if (map.Tile[x, y].Type == Enums.TileType.Key) {
                                // Check if the key they are using matches the map key
                                if (item.Num == map.Tile[x, y].Data1) {
                                    map.Tile[x, y].DoorOpen = true;
                                    map.Tile[x, y].DoorTimer = Core.GetTickCount();

                                    Messenger.SendDataToMap(((Recruit)setup.Attacker).MapID, TcpPacket.CreatePacket("mapkey", x.ToString(), y.ToString(), "1"));

                                    if (map.Tile[x, y].String1.Trim() != "") {
                                        Messenger.PlayerMsg(((Recruit)setup.Attacker).Owner, "A door has been unlocked!", Text.White);
                                    } else {
                                        Messenger.PlayerMsg(((Recruit)setup.Attacker).Owner, map.Tile[x, y].String1.Trim(), Text.White);
                                    }

                                    Messenger.PlaySoundToMap(((Recruit)setup.Attacker).MapID, "key.wav");

                                    // Check if we are supposed to take the item away
                                    if (map.Tile[x, y].Data2 == 1) {
                                        ((Recruit)setup.Attacker).Owner.Player.TakeItemSlot(invNum, 0, true);
                                        Messenger.PlayerMsg(((Recruit)setup.Attacker).Owner, "The key dissolves.", Text.Yellow);
                                    }
                                }
                            }

                            if (map.Tile[x, y].Type == Enums.TileType.Door) {
                                map.Tile[x, y].DoorOpen = true;
                                map.Tile[x, y].DoorTimer = Core.GetTickCount();

                                Messenger.SendDataToMap(((Recruit)setup.Attacker).MapID, TcpPacket.CreatePacket("mapkey", x.ToString(), y.ToString(), "1"));
                                Messenger.PlaySoundToMap(((Recruit)setup.Attacker).MapID, "key.wav");
                            }
                        }
                    }
                    break;
                case Enums.ItemType.TM: {
                        // Get the spell num

                        if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                            int n = ItemManager.Items[item.Num].Data1;

                            if (n > 0) {
                                // Make sure they are the right class
                                if (((Recruit)setup.Attacker).CanLearnTMMove(n)) {



                                    if (((Recruit)setup.Attacker).CanLearnNewMove()) {
                                        if (((Recruit)setup.Attacker).HasMove(n) == false) {
                                            ((Recruit)setup.Attacker).LearnNewMove(n);
                                            ((Recruit)setup.Attacker).Owner.Player.TakeItemSlot(invNum, 0, true);
                                            Messenger.PlayerMsg(((Recruit)setup.Attacker).Owner, "You have learned a new move!", Text.White);
                                            Messenger.SendPlayerMoves(((Recruit)setup.Attacker).Owner);
                                        } else {
                                            Messenger.PlayerMsg(((Recruit)setup.Attacker).Owner, "You already know this move!", Text.BrightRed);
                                        }
                                    } else {
                                        if (((Recruit)setup.Attacker).HasMove(n) == false) {
                                            int itemNum = item.Num;
                                            ((Recruit)setup.Attacker).MoveItem = itemNum;
                                            ((Recruit)setup.Attacker).LearnNewMove(n);
                                        } else {
                                            Messenger.PlayerMsg(((Recruit)setup.Attacker).Owner, "You already know this move!", Text.BrightRed);
                                        }
                                    }
                                } else {
                                    Messenger.PlayerMsg(((Recruit)setup.Attacker).Owner, "You cannot learn this move!", Text.White);
                                }
                            } else {
                                Messenger.PlayerMsg(((Recruit)setup.Attacker).Owner, "There is no move that can be learned. Please inform an admin.", Text.BrightRed);
                            }
                        }
                    }
                    break;
                case Enums.ItemType.Scripted: {
                        Scripting.ScriptManager.InvokeSub("ScriptedItem", setup, item, invNum);
                    }
                    break;
            }

        }

        public static void FinalizeAction(BattleSetup setup, PacketHitList parent) {
            Scripting.ScriptManager.InvokeSub("AfterActionTaken", setup);

            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                if (setup.AttackerMap.MapType == Enums.MapType.Standard) {
                    setup.ExpGained /= 2;
                }
                if (setup.ExpGained > 0) {

                    if (((Recruit)setup.Attacker).Owner.Player.PartyID == null) {
                        if (!(setup.Attacker.Level == Exp.ExpManager.Exp.MaxLevels)) {
                            Scripting.ScriptManager.InvokeSub("PlayerEXP", setup.packetStack, ((Recruit)setup.Attacker).Owner, setup.ExpGained);

                            CheckPlayerLevelUp(setup.PacketStack, ((Recruit)setup.Attacker).Owner);
                        }
                    } else {
                        Party party = PartyManager.FindPlayerParty(((Recruit)setup.Attacker).Owner);
                        if (party != null) {
                            party.HandoutExp(((Recruit)setup.Attacker).Owner, setup.ExpGained, setup.PacketStack);
                        }
                    }
                }
                //setup.PacketStack.AddPacket(((Recruit)setup.Attacker).Owner, PacketBuilder.CreateBattleDivider());
            }

            // Disable caching and build the hitlist to allow for post-processing
            setup.PacketStack.CachingEnabled = false;

            for (int i = 0; i < setup.PacketStack.HitList.Count; i++) {
                if (setup.PacketStack.HitList.ValueByIndex(i).ContainsPacket("battlemsg") && parent == null) {
                    setup.PacketStack.AddPacket(setup.PacketStack.HitList.KeyByIndex(i), PacketBuilder.CreateBattleDivider());
                }

            }

            if (parent != null) {
                parent.AddHitList(setup.PacketStack);
            } else {
                PacketHitList.MethodEnded(ref setup.packetStack);
            }

        }

        public static void FinalizeAction(BattleSetup setup) {
            FinalizeAction(setup, null);
            

        }

        public static void HandleAttack(BattleSetup setup) {
            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {//if player, check if he's playing
                if (NetworkManager.IsPlaying(((Recruit)setup.Attacker).Owner) == false) {
                    return;

                }
            } else {//if NPC, check if it exists
                if (((MapNpc)setup.Attacker).Num <= 0) {
                    return;
                }

            }

            //BattleSetup setup = new BattleSetup();
            //setup.Attacker = attacker;
            //setup.moveSlot = moveSlot;


            Scripting.ScriptManager.InvokeSub("BeforeMoveUsed", setup);

            if (!setup.Cancel) {
                ExecuteAttack(setup);
            }




            if (!setup.Cancel) {
                Scripting.ScriptManager.InvokeSub("AfterMoveUsed", setup);
            }


        }

        public static void ExecuteAttack(BattleSetup setup) {


            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {

                PacketBuilder.AppendAttackPacket(((Recruit)setup.Attacker).Owner, setup.PacketStack);

            } else {
                PacketBuilder.AppendNpcAttack(setup.AttackerMap, setup.PacketStack, ((MapNpc)setup.Attacker).MapSlot);
            }

            if (!string.IsNullOrEmpty(setup.Move.Name)) {
                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Move.Name, Text.WhiteSmoke), setup.Attacker.X, setup.Attacker.Y, 16);
                //setup.PacketStack.AddPacket(PacketBuilder.CreateBattleMsg(((Recruit)setup.Attacker).Owner.Player.Name + setup.Move.Name, Text.WhiteSmoke));
            }

            if (setup.Move.Sound > 0) {
                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSoundPacket("magic" + setup.Move.Sound + ".wav"), setup.Attacker.X, setup.Attacker.Y, 10);
                //Messenger.PlaySoundToMap(setup.Attacker.MapID, "magic" + setup.Move.Sound + ".wav");
            }

            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSpellAnim(setup.Move.AttackerAnim, setup.Attacker.X, setup.Attacker.Y, setup.Attacker.X, setup.Attacker.Y, 0));

            Scripting.ScriptManager.InvokeSub("AfterMoveExecuted", setup);

            bool hitsFoes = HitsFoes(setup.Move.TargetType);
            bool hitsAllies = HitsAllies(setup.Move.TargetType);

            TargetCollection targets = MoveProcessor.GetTargetsInRange(setup.Move.RangeType, setup.Move.Range, setup.AttackerMap, setup.Attacker, setup.Attacker.X, setup.Attacker.Y, setup.Attacker.Direction, hitsFoes, hitsAllies, false);

            if (hitsFoes) {
                setup.Attacker.UseMoveOnFoes(setup, targets);
            }

            if (hitsAllies) {
                setup.Attacker.UseMoveOnAllies(setup, targets);
            }

            if (HitsSelf(setup.Move.TargetType)) {
                setup.Defender = setup.Attacker;
                MoveHitCharacter(setup);
                if (setup.Cancel) {
                    return;
                }

            }

            if (setup.Move.TargetType == Enums.MoveTarget.NoOne) {
                setup.Defender = null;
                MoveHitCharacter(setup);
                if (setup.Cancel) {
                    return;
                }
            }

            switch (setup.Move.TravelingAnim.AnimationType) {
                case Enums.MoveAnimationType.Normal: {
                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSpellAnim(setup.Move.TravelingAnim, setup.Attacker.X, setup.Attacker.Y, setup.Attacker.X, setup.Attacker.Y, 0));
                    }
                    break;
                case Enums.MoveAnimationType.Overlay: {
                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSpellAnim(setup.Move.TravelingAnim, setup.Attacker.X, setup.Attacker.Y, setup.Attacker.X, setup.Attacker.Y, 0));
                    }
                    break;
                case Enums.MoveAnimationType.Arrow:
                case Enums.MoveAnimationType.ItemArrow: {
                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSpellAnim(setup.Move.TravelingAnim, setup.Attacker.X, setup.Attacker.Y, (int)setup.Attacker.Direction, setup.Move.Range, 0));
                    }
                    break;
                case Enums.MoveAnimationType.Throw:
                case Enums.MoveAnimationType.ItemThrow: {
                        if (setup.Defender == null) {
                            switch (setup.Attacker.Direction) {
                                case Enums.Direction.Up: {
                                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSpellAnim(setup.Move.TravelingAnim, setup.Attacker.X, setup.Attacker.Y, 0, -2, 0));
                                    }
                                    break;
                                case Enums.Direction.Down: {
                                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSpellAnim(setup.Move.TravelingAnim, setup.Attacker.X, setup.Attacker.Y, 0, 2, 0));
                                    }
                                    break;
                                case Enums.Direction.Left: {
                                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSpellAnim(setup.Move.TravelingAnim, setup.Attacker.X, setup.Attacker.Y, -2, 0, 0));
                                    }
                                    break;
                                case Enums.Direction.Right: {
                                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSpellAnim(setup.Move.TravelingAnim, setup.Attacker.X, setup.Attacker.Y, 2, 0, 0));
                                    }
                                    break;
                            }
                        } else {
                            setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSpellAnim(setup.Move.TravelingAnim, setup.Attacker.X, setup.Attacker.Y, setup.Defender.X - setup.Attacker.X, setup.Defender.Y - setup.Attacker.Y, 0));
                        }
                    }
                    break;
                case Enums.MoveAnimationType.Beam: {
                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSpellAnim(setup.Move.TravelingAnim, setup.Attacker.X, setup.Attacker.Y, (int)setup.Attacker.Direction, setup.Move.Range, 0));
                    }
                    break;
                case Enums.MoveAnimationType.Tile: {
                        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateSpellAnim(setup.Move.TravelingAnim, setup.Attacker.X, setup.Attacker.Y, (int)setup.Move.RangeType, (int)setup.Attacker.Direction, setup.Move.Range));
                    }
                    break;
            }

            if (!setup.Move.PerPlayer && !setup.Cancel) {
                Scripting.ScriptManager.InvokeSub("MoveAdditionalEffect", setup);
            }


        }

        public static void MoveHitCharacter(BattleSetup setup) {
            setup.Multiplier = setup.AttackerMultiplier;
            setup.KnockedOut = true;
            //setup.Critical = false;
            setup.Damage = 0;
            //if (setup.Defender.CharacterType == Enums.CharacterType.Recruit) {
            //    Recruit recruit = setup.Defender as Recruit;
            //    if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
            //Messenger.BattleMsg(setup.Attacker.Owner, ((Recruit)setup.Attacker).Owner.Player.Name + MoveManager.Moves[moveNum].Name, Text.BrightRed);
            //    	   if (!string.IsNullOrEmpty(setup.Move.Name)) {
            //    	   	setup.DefenderPacketStack.AddPacket(PacketBuilder.CreateBattleMsg(setup.Attacker.Name + setup.Move.Name, Text.BrightRed));
            //    	   }

            //BattleProcessor.PlayerAttackPlayer(((Recruit)setup.Attacker).Owner, recruit.Owner, damage);
            //    } else if (setup.Attacker.CharacterType == Enums.CharacterType.MapNpc) {
            //    	if (!string.IsNullOrEmpty(setup.Move.Name)) {
            //    		setup.DefenderPacketStack.AddPacket(PacketBuilder.CreateBattleMsg(setup.Attacker.Name + setup.Move.Name, Text.BrightRed));
            //    	}
            //BattleProcessor.NpcAttackPlayer(((MapNpc)setup.Attacker).MapSlot, recruit.Owner, damage);
            //    }
            //}
            if (setup.Defender != null) {
                setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateSpellAnim(setup.Move.DefenderAnim, setup.Defender.X, setup.Defender.Y, setup.Defender.X, setup.Defender.Y, 0));
                //Messenger.SpellAnim(setup.Move, setup.Defender.MapID, setup.Defender.X, setup.Defender.Y);
            }


            Scripting.ScriptManager.InvokeSub("BeforeMoveHits", setup);

            if (setup.Cancel) {
                return;
            }

            Move move = setup.Move;

            if (setup.Hit == true) {
                switch (move.EffectType) {
                    case Enums.MoveType.SubHP: {
                            int damage = DamageCalculator.CalculateDamage(move.Data1, setup.AttackStat, setup.Attacker.Level, setup.DefenseStat) * setup.Multiplier / 1000;
                            if (damage < 0) damage = 0;
                            setup.Defender.HP -= damage;

                            if (damage <= 0 && setup.Multiplier > -1) {
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " took no damage!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                                //BothWaysBattleMsg(setup, setup.Defender.Name + " took no damage!", Text.BrightRed);
                            } else if (damage > 0) {
                                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg(setup.Defender.Name + " took " + damage + " damage!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                                //BothWaysBattleMsg(setup, setup.Defender.Name + " took " + damage + " damage!", Text.BrightRed);
                            }

                            setup.Damage = damage;
                        }
                        break;
                    case Enums.MoveType.AddHP: {
                            //Todo?: messages for the other movetypes




                            if (setup.Defender.HP + move.Data1 > setup.Defender.MaxHP) {
                                setup.Defender.HP = setup.Defender.MaxHP;
                            } else {
                                setup.Defender.HP += move.Data1;
                            }


                        }
                        break;
                    case Enums.MoveType.SubMP: {




                            int randomSlot = Server.Math.Rand(0, 4);
                            if (setup.Defender.Moves[randomSlot].MoveNum > 0 && setup.Defender.Moves[randomSlot].CurrentPP > 0) {
                                setup.Defender.Moves[randomSlot].CurrentPP--;
                                if (setup.Defender.CharacterType == Enums.CharacterType.Recruit) {
                                    Recruit recruit = setup.Defender as Recruit;
                                    PacketBuilder.AppendMovePPUpdate(recruit.Owner, setup.PacketStack, randomSlot);
                                }
                            }// else {
                            //    for (int i = 0; i < setup.Defender.Moves.Length; i++) {
                            //        if (setup.Defender.Moves[i].MoveNum > 0 && setup.Defender.Moves[i].CurrentPP > 0) {
                            //            setup.Defender.Moves[i].CurrentPP--;
                            //            if (setup.Defender.CharacterType == Enums.CharacterType.Recruit) {
                            //                Recruit recruit = setup.Defender as Recruit;
                            //                Messenger.SendMovePPUpdate(recruit.Owner, i);
                            //            }
                            //            break;
                            //        }
                            //    }
                            //}


                        }
                        break;
                    case Enums.MoveType.AddMP: {



                            int randomSlot = Server.Math.Rand(0, 4);
                            if (setup.Defender.Moves[randomSlot].MoveNum > 0 && setup.Defender.Moves[randomSlot].CurrentPP < setup.Defender.Moves[randomSlot].MaxPP) {
                                setup.Defender.Moves[randomSlot].CurrentPP++;
                                if (setup.Defender.CharacterType == Enums.CharacterType.Recruit) {
                                    Recruit recruit = setup.Defender as Recruit;
                                    PacketBuilder.AppendMovePPUpdate(recruit.Owner, setup.PacketStack, randomSlot);
                                }
                            }// else {
                            //    for (int i = 0; i < setup.Defender.Moves.Length; i++) {
                            //        if (setup.Defender.Moves[i].MoveNum > 0 && setup.Defender.Moves[i].CurrentPP < setup.Defender.Moves[i].MaxPP) {
                            //            setup.Defender.Moves[i].CurrentPP++;
                            //            if (setup.Defender.CharacterType == Enums.CharacterType.Recruit) {
                            //                Recruit recruit = setup.Defender as Recruit;
                            //                Messenger.SendMovePPUpdate(recruit.Owner, i);
                            //            }
                            //            break;
                            //        }
                            //    }
                            //}


                        }
                        break;
                    case Enums.MoveType.Scripted: {


                        Scripting.ScriptManager.InvokeSub("ScriptedMoveHitCharacter", setup);
                        }
                        break;
                }





            } else {

                if (setup.Defender != null) {
                    setup.PacketStack.AddPacketToMap(setup.DefenderMap, PacketBuilder.CreateBattleMsg("The move missed " + setup.Defender.Name + "!", Text.BrightRed), setup.Defender.X, setup.Defender.Y, 10);
                    //BothWaysBattleMsg(setup, "The move missed " + setup.Defender.Name + "!", Text.BrightRed);
                }

            }



            if (setup.Defender != null && setup.Defender.CharacterType == Enums.CharacterType.MapNpc) {
                if (setup.moveIndex > 0) {
                    ((MapNpc)setup.Defender).HitByMove = true;
                }
                if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                    if (Ranks.IsDisallowed(((Recruit)setup.Attacker).Owner, Enums.Rank.Moniter) || ((Recruit)setup.Attacker).Owner.Player.Hunted) {
                        ((MapNpc)setup.Defender).Target = ((Recruit)setup.Attacker).Owner;
                        if (NpcManager.Npcs[((MapNpc)setup.Defender).Num].Behavior == Enums.NpcBehavior.Guard) {
                            for (int i = 0; i < Constants.MAX_MAP_NPCS; i++) {
                                if (setup.DefenderMap.ActiveNpc[i].Num == ((MapNpc)setup.Defender).Num) {
                                    setup.DefenderMap.ActiveNpc[i].Target = ((Recruit)setup.Attacker).Owner;
                                }
                            }

                        }
                    }
                }
            }

            if (!setup.Cancel) {
                if (setup.Move.PerPlayer) {
                    Scripting.ScriptManager.InvokeSub("MoveAdditionalEffect", setup);
                }
                Scripting.ScriptManager.InvokeSub("AfterMoveHits", setup);
            }

            if (setup.Defender != null && setup.Defender.HP <= 0 && setup.KnockedOut == false) {
                setup.Defender.HP = 1;
                if (setup.Defender.CharacterType == Enums.CharacterType.Recruit) {
                    PacketBuilder.AppendHP(((Recruit)setup.Defender).Owner, setup.PacketStack);
                }
            }

            if (setup.Defender == null || setup.Defender.HP > 0) {
                setup.KnockedOut = false;
            }

            //if (setup.Defender.CharacterType == Enums.CharacterType.Recruit) {
            //	Messenger.SendDataTo(((Recruit)setup.Defender).Owner, setup.DefenderPacketStack);
            //	setup.DefenderPacketStack = new PacketList();
            //}

            if (setup.KnockedOut) HandleDeath(setup);//""uncancellable

        }

        //no longer needed?
        //public static void BothWaysBattleMsg(BattleSetup setup, string msg, Color color) {
        //	if (setup.Defender.CharacterType == Enums.CharacterType.Recruit) {
        //        Recruit recruit = setup.Defender as Recruit;
        //        if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {//PvP
        //        	setup.AttackerPacketStack.AddPacket(PacketBuilder.CreateBattleMsg(msg, color));
        //        	setup.DefenderPacketStack.AddPacket(PacketBuilder.CreateBattleMsg(msg, color));



        //        } else if (setup.Attacker.CharacterType == Enums.CharacterType.MapNpc) {//NvP
        //        	setup.DefenderPacketStack.AddPacket(PacketBuilder.CreateBattleMsg(msg, color));

        //        }
        //	  	} else if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit){//PvN
        //		setup.AttackerPacketStack.AddPacket(PacketBuilder.CreateBattleMsg(msg, color));
        //	  	}
        //}



        public static bool ShouldUseMove(IMap map, MapNpc attacker, int moveSlot) {
            Move move = null;
            if (moveSlot > -1 && moveSlot < 4 && attacker.Moves[moveSlot].MoveNum > 0) {
                move = MoveManager.Moves[attacker.Moves[moveSlot].MoveNum];
            } else {
                move = MoveManager.StandardAttack;
            }

            //Confusion check
            if (moveSlot == -1 || attacker.Moves[moveSlot].CurrentPP > 0) {
                // Attack any players that are in range
                switch (move.TargetType) {
                    case Enums.MoveTarget.Foes: {
                            if (attacker.IsInRangeOfFoes(map, move, moveSlot)) {
                                return true;
                            } else {
                                return false;
                            }
                        }
                    case Enums.MoveTarget.User: {
                            return true;
                        }
                    case Enums.MoveTarget.UserAndAllies: {
                            return true;
                        }
                    case Enums.MoveTarget.UserAndFoe: {
                            return true;
                        }
                    case Enums.MoveTarget.All: {
                            return true;
                        }
                    case Enums.MoveTarget.AllAlliesButUser: {
                            if (attacker.IsInRangeOfAllies(map, move, moveSlot)) {
                                return true;
                            } else {
                                return false;
                            }
                        }
                    case Enums.MoveTarget.AllButUser: {
                            if (attacker.IsInRangeOfAllies(map, move, moveSlot) ||
                                attacker.IsInRangeOfFoes(map, move, moveSlot)) {
                                return true;
                            } else {
                                return false;
                            }
                        }
                    default: {
                            return false;
                        }
                }
            } else {
                return false;
            }

        }



        public static void HandleDeath(BattleSetup setup) {
            IMap map = setup.DefenderMap;
            if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                if (setup.Defender.CharacterType == Enums.CharacterType.Recruit) {
                    Recruit defender = (Recruit)setup.Defender;
                    if (((Recruit)setup.Attacker).Owner.Player.Map.Tile[setup.Attacker.X, setup.Attacker.Y].Type == Enums.TileType.Arena/* || defender.Owner.Player.PK == true*/) {
                        ((Recruit)setup.Attacker).Owner.Player.PokemonSeen(setup.Defender.Species);
                    }
                } else if (setup.Defender.CharacterType == Enums.CharacterType.MapNpc) {
                    ((Recruit)setup.Attacker).Owner.Player.PokemonSeen(setup.Defender.Species);
                }
            }

            if (setup.Defender.CharacterType == Enums.CharacterType.Recruit) {
                Recruit defender = (Recruit)setup.Defender;
                setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateBattleMsg("Oh, no!  " + defender.Name + " fainted!", Text.BrightRed));
                //Messenger.MapMsg(defender.MapID, "Oh, no!  " + defender.Name + " fainted!", Text.BrightRed);

                //if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit && setup.Attacker != setup.Defender) {
                //    if (((Recruit)setup.Defender).Owner.Player.Map.Tile[setup.Defender.X, setup.Defender.Y].Type != Enums.TileType.Arena && ((Recruit)setup.Attacker).Owner.Player.PK == false && defender.Owner.Player.PK == false) {
                //        ((Recruit)setup.Attacker).Owner.Player.PK = true;
                //        setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateChatMsg(setup.Attacker.Name + " has been deemed an Outlaw!", Text.BrightRed));
                        //Messenger.GlobalMsg(setup.Attacker.Name + " has been deemed an Outlaw!", Text.BrightRed);  
                //    }
                //}

                //if (defender.Owner.Player.PK == true) {
                //    defender.Owner.Player.PK = false;
                //    setup.PacketStack.AddPacketToMap(setup.AttackerMap, PacketBuilder.CreateChatMsg(defender.Name + " has paid the price for being an Outlaw!", Text.BrightRed));
                    //Messenger.GlobalMsg(defender.Name + " has paid the price for being an Outlaw!", Text.BrightRed);
                //}

                

                if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {
                    Scripting.ScriptManager.InvokeSub("OnDeath", setup.PacketStack, defender.Owner, Enums.KillType.Player);
                } else {
                    Scripting.ScriptManager.InvokeSub("OnDeath", setup.PacketStack, defender.Owner, Enums.KillType.Npc);
                }

                if (setup.Attacker == setup.Defender) setup.Cancel = true;

                bool allOut = true;
                foreach (Client client in map.GetClients()) {
                    if (client.Player.Hunted) {
                        allOut = false;
                    }
                }
                if (allOut) {
                    map.ProcessingPaused = true;
                }

            } else if (setup.Defender.CharacterType == Enums.CharacterType.MapNpc) {
                MapNpc defender = (MapNpc)setup.Defender;


                if (setup.Attacker.CharacterType == Enums.CharacterType.Recruit) {

                    if (defender.HeldItem != null) {
                        defender.MapDropItem(defender.HeldItem.Amount, ((Recruit)setup.Attacker).Owner);
                    }

                    Scripting.ScriptManager.InvokeSub("OnNpcDeath", setup.PacketStack, setup.Attacker, defender);

                    bool skipRecruit = false;

                    try {

                        if (!((Recruit)setup.Attacker).Owner.Player.Map.RecruitEnabled) {
                            skipRecruit = true;
                        }
                        if (NpcManager.Npcs[defender.Num].RecruitRate == 0) skipRecruit = true;
                        //if (((Recruit)setup.Attacker).Owner.Player.GetTeamSize() + Pokedex.Pokedex.GetPokemon(NpcManager.Npcs[defender.Num].Species).Size > 4) {
                        //    skipRecruit = true;
                        //}
                        //if (((Recruit)setup.Attacker).Owner.Player.PK) skipRecruit = true;

                        if (((Recruit)setup.Attacker).Owner.Player.RequestedRecruit != null) skipRecruit = true;

                        //if (((Recruit)setup.Attacker).Owner.Player.Team[0] != setup.Attacker) skipRecruit = true;
                        if (setup.Attacker.Level <= defender.Level) skipRecruit = true;
                    } catch { }

                    if (skipRecruit == false) {

                        int openSlot = ((Recruit)setup.Attacker).Owner.Player.FindOpenTeamSlot();
                        if (openSlot != -1) {

                            int bonus = ((Recruit)setup.Attacker).Owner.Player.GetRecruitBonus();

                            int recruitRate = NpcManager.Npcs[defender.Num].RecruitRate;

                            if (Math.Rand(1, 1001) < (recruitRate + bonus)) {

                                ((Recruit)setup.Attacker).Owner.Player.AddToRequested((MapNpc)defender);

                            }
                        }
                    }

                    bool skipExp = false;
                    try {
                        if (!((Recruit)setup.Attacker).Owner.Player.Map.ExpEnabled) {
                            skipExp = true;
                        }

                        if (((Recruit)setup.Attacker).Owner.Player.GetActiveRecruit().Level == Exp.ExpManager.Exp.MaxLevels) {
                            skipExp = true;
                        }
                    } catch { }

                    if (!skipExp) {
                        ulong Exp = (ulong)Pokedex.Pokedex.GetPokemonForm(NpcManager.Npcs[defender.Num].Species, defender.Form).GetRewardExp(defender.Level);
                        if (!defender.HitByMove) {
                            Exp /= 2;
                        }
                        // Make sure we dont get less then 0
                        if (Exp < 0) {
                            Exp = 1;
                        }

                        Exp = (ulong)(Exp);

                        setup.ExpGained += Exp;

                    }

                } else {//attacker is null, or another npc
                    if (defender.HeldItem != null) {
                        defender.MapDropItem(defender.HeldItem.Amount, null);
                    }
                    Scripting.ScriptManager.InvokeSub("OnNpcDeath", setup.PacketStack, setup.Attacker, defender);

                }

                //reset NPC slot
                setup.DefenderMap.ClearActiveNpc(defender.MapSlot);

                setup.PacketStack.AddPacketToMap(setup.AttackerMap, TcpPacket.CreatePacket("npcdead", defender.MapSlot));
                //Messenger.SendDataToMap(defender.MapID, TcpPacket.CreatePacket("npcdead", defender.MapSlot));

            }
        }



        public static string EffectivenessToPhrase(int effectiveness) {
            if (effectiveness < 4) {
                return "It has little effect...";
            } else if (effectiveness < 6) {
                return "It's not very effective...";
            } else if (effectiveness == 7) {
                return "It's super-effective!";

            } else if (effectiveness > 7) {
                return "It's super-effective!!!";
            } else {
                return null;
            }
        }

        public static bool HitsFoes(Enums.MoveTarget moveTargetType) {
            switch (moveTargetType) {
                case Enums.MoveTarget.Foes:
                case Enums.MoveTarget.All:
                case Enums.MoveTarget.UserAndFoe:
                case Enums.MoveTarget.AllButUser: {
                        return true;
                    }
                    break;
                default: {
                        return false;
                    }
                    break;
            }

        }

        public static bool HitsAllies(Enums.MoveTarget moveTargetType) {
            switch (moveTargetType) {
                case Enums.MoveTarget.UserAndAllies:
                case Enums.MoveTarget.All:
                case Enums.MoveTarget.AllButUser:
                case Enums.MoveTarget.AllAlliesButUser: {
                        return true;
                    }
                    break;
                default: {
                        return false;
                    }
                    break;
            }

        }

        public static bool HitsSelf(Enums.MoveTarget moveTargetType) {
            switch (moveTargetType) {
                case Enums.MoveTarget.UserAndAllies:
                case Enums.MoveTarget.All:
                case Enums.MoveTarget.User:
                case Enums.MoveTarget.UserAndFoe: {
                        return true;
                    }
                    break;
                default: {
                        return false;
                    }
                    break;
            }

        }




        public static void CheckPlayerLevelUp(PacketHitList hitList, Client client) {
            PacketHitList.MethodStart(ref hitList);
            if (client.Player.GetActiveRecruit().Exp > client.Player.GetActiveRecruit().GetNextLevel()) {
                if (client.Player.GetActiveRecruit().Level < Exp.ExpManager.Exp.MaxLevels) {
                    Scripting.ScriptManager.InvokeSub("PlayerLevelUp", hitList, client);

                    hitList.AddPacketToMap(client.Player.Map, TcpPacket.CreatePacket("levelup", client.ConnectionID.ToString()));
                    //Messenger.SendDataToMap(client.Player.MapID, TcpPacket.CreatePacket("levelup", client.ConnectionID.ToString()));
                }
                
                if (client.Player.GetActiveRecruit().Level >= Exp.ExpManager.Exp.MaxLevels) {
                    client.Player.GetActiveRecruit().Level = Exp.ExpManager.Exp.MaxLevels;
                    client.Player.GetActiveRecruit().Exp = ExpManager.Exp[Exp.ExpManager.Exp.MaxLevels - 1];
                }
            } else {
                PacketBuilder.AppendEXP(client, hitList);
                //Messenger.SendEXP(client);
            }

            PacketHitList.MethodEnded(ref hitList);

            //Messenger.SendActiveTeam(index);

            //Messenger.SendHP(index);
            //Messenger.SendStats(index);
            //Messenger.BattleMsg(index, "You.", Text.BrightBlue);
        }

        #endregion New Area





    }


}

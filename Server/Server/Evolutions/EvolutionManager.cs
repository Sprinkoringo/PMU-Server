using System;
using System.Collections.Generic;
using System.Text;
using Server.Moves;
using Server.Network;

namespace Server.Evolutions
{
    public class EvolutionManager : EvolutionManagerBase
    {
        public static void StartEvolution(Players.Player player) {
            try {
                int speciesNum = player.GetActiveRecruit().Species;
                int evoIndex = -1;
                List<int> evoToLoad;
                for (int z = 0; z < Evolutions.MaxEvos; z++) {
                    if (Evolutions[z].Species == speciesNum) {
                        evoIndex = z;
                    }
                }
                
                
                if (evoIndex == -1) {
                    Messenger.PlayerMsg(player.Client, "You cannot evolve anymore.", Text.BrightRed);
                } else {
                		
                		evoToLoad = FindViableEvolutions(player.Client, evoIndex);
                	if (evoToLoad.Count == 0) {
                		Messenger.PlayerMsg(player.Client, "You do not meet the requirements to evolve.", Text.BrightRed);
                	} else if (evoToLoad.Count == 1) {
                		Messenger.AskQuestion(player.Client, "SingleEvolution", "You who seek awakening, you shall evolve into " + Evolutions[evoIndex].Branches[evoToLoad[0]].Name + ".  Will you evolve?", -1);
                	} else {
                		string[] evochoices = new string[evoToLoad.Count + 1];
                		for(int i = 0; i < evoToLoad.Count; i++) {
                			evochoices[i] = Evolutions[evoIndex].Branches[evoToLoad[i]].Name;
                		}
                		evochoices[evochoices.Length - 1] = "Cancel";
                		Messenger.AskQuestion(player.Client, "BranchEvolution", "You who seek awakening, into what do you wish to evolve?", -1, evochoices);
                	}
                		
                		
                		/*
                    if (evoToLoad != -1) {
                        if (player.GetActiveRecruit().Level >= Evolutions[evoIndex].MinLvl) {
                            //Classes.ClassManager.ChangePlayerClass(player.Client, Evolutions[evoIndex].SplitEvos[evoToLoad].NewClass);
                            player.GetActiveRecruit().Sprite = Evolutions[evoIndex].SplitEvos[evoToLoad].NewSprite;
                            if (!player.GetActiveRecruit().HasMove(Evolutions[evoIndex].SplitEvos[evoToLoad].MoveLearned)) {
                                player.GetActiveRecruit().LearnNewMove(Evolutions[evoIndex].SplitEvos[evoToLoad].MoveLearned);
                                Messenger.SendPlayerMoves(player.Client);
                                Messenger.PlayerMsg(player.Client, "You have learned " + MoveManager.Moves[Evolutions[evoIndex].SplitEvos[evoToLoad].MoveLearned].Name + "!", Text.White);
                            }
                            player.TakeItem(Evolutions[evoIndex].SplitEvos[evoToLoad].RequiredItem, 1);
                            if (player.GetActiveRecruit().Nickname == false) {
                                player.GetActiveRecruit().Name = Evolutions[evoIndex].SplitEvos[evoToLoad].Name;
                            }
                            player.GetActiveRecruit().Save(player.GetCharFolder(), player.GetActiveRecruitSlot());
                            Messenger.SendActiveTeam(player.Client);
                            Messenger.SendPlayerData(player.Client);
                            Messenger.PlayerMsg(player.Client, "You have evolved into a " + Evolutions[evoIndex].SplitEvos[evoToLoad].Name + "! Congratulations!", Text.Yellow);
                        } else {
                            Messenger.PlayerMsg(player.Client, "You can't evolve yet... You must raise your level!", Text.BrightRed);
                        }
                    } else {
                        Messenger.PlayerMsg(player.Client, "You do not have the proper item equipped to evolve!", Text.BrightRed);
                    }
                    */
                }
            } catch (Exception ex) {
                Exceptions.ErrorLogger.WriteToErrorLog(ex, "Evolution, [Data] Index: " + player.Client.ToString() + ", Player Sprite: " + player.GetActiveRecruit().Sprite.ToString());
                Messenger.PlayerMsg(player.Client, "Evolution Error", Text.BrightRed);
            }
        }

        public static List<int> FindViableEvolutions(Client client, int evoIndex) {
            List<int> evoToLoad = new List<int>();


            for (int i = 0; i < Evolutions[evoIndex].Branches.Count; i++) {
                bool canEvolve = Scripting.ScriptManager.InvokeFunction("ScriptedEvoReq", client, Evolutions[evoIndex].Branches[i].ReqScript, Evolutions[evoIndex].Branches[i].Data1, Evolutions[evoIndex].Branches[i].Data2, Evolutions[evoIndex].Branches[i].Data3).ToBool();
                if (canEvolve) {
                    evoToLoad.Add(i);
                }
            }
            return evoToLoad;
        }

        public static List<int> FindAllEvolutions(Client client, int evoIndex) {
            List<int> evoToLoad = new List<int>();


            for (int i = 0; i < Evolutions[evoIndex].Branches.Count; i++) {
                evoToLoad.Add(i);
            }
            return evoToLoad;
        }

        public static int FindPreEvolution(int species) {

            for (int i = 0; i < EvolutionManager.Evolutions.MaxEvos; i++)
            {
                foreach (EvolutionBranch branch in EvolutionManager.Evolutions[i].Branches) {
                    if (branch.NewSpecies == species) {
                        return EvolutionManager.Evolutions[i].Species;
                    }
                }

            }

            return -1;
        }
    	
    	
    }
}

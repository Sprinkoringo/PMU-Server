using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server;
using Server.Network;
using Server.Players;
using Server.Stories;
using Server.Dungeons;
using Server.Players.Parties;

namespace Script {
    public class DungeonRules {
    
    	
    	/*
        public static bool ItemLimit(Client client, int limit) {
            int itemCounter = 0;
            for (int i = 1; i <= client.Player.MaxInv; i++) {
                if (client.Player.Inventory[i].Num > 0) {
                    itemCounter++;
                    if (itemCounter > limit) {
                        Story story = new Story();
                        StorySegment segment = new StorySegment();
                        segment.Action = Enums.StoryAction.Say;
                        segment.AddParameter("Text", "You may only enter this dungeon if your bag has no more than " + limit + " items.");
                        segment.AddParameter("Mugshot", "-1");
                        segment.Parameters.Add("Speed", "0");
                        segment.Parameters.Add("PauseLocation", "0");
                        story.Segments.Add(segment);

                        StoryManager.PlayStory(client, story);
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool MinLevelLimit(Client client, int minLevel) {
            for (int i = 0; i < client.Player.Team.Length; i++) {
                if (client.Player.Team[i].Level < minLevel) {
                    Story story = new Story();
                    StorySegment segment = new StorySegment();
                    segment.Action = Enums.StoryAction.Say;
                    segment.AddParameter("Text", "You may only enter this dungeon with a team that is over level " + minLevel + "!");
                    segment.AddParameter("Mugshot", "-1");
                    segment.Parameters.Add("Speed", "0");
                    segment.Parameters.Add("PauseLocation", "0");
                    story.Segments.Add(segment);

                    StoryManager.PlayStory(client, story);
                    return false;
                }
            }
            return true;
        }

        public static bool MaxLevelLimit(Client client, int maxLevel) {
            for (int i = 0; i < client.Player.Team.Length; i++) {
                if (client.Player.Team[i].Level > maxLevel) {
                    Story story = new Story();
                    StorySegment segment = new StorySegment();
                    segment.Action = Enums.StoryAction.Say;
                    segment.AddParameter("Text", "You may only enter this dungeon with a team that is under level " + maxLevel + "!");
                    segment.AddParameter("Mugshot", "-1");
                    segment.Parameters.Add("Speed", "0");
                    segment.Parameters.Add("PauseLocation", "0");
                    story.Segments.Add(segment);

                    StoryManager.PlayStory(client, story);
                    return false;
                }
            }
            return true;
        }

        public static bool TypesAllowed(Client client, params Enums.PokemonType[] type) {
            for (int i = 0; i < type.Length; i++) {
                for (int team = 0; team < client.Player.Team.Length; team++) {
                    if (client.Player.Team[team].Type1 != type[i] || client.Player.Team[team].Type2 != type[i]) {
                        string allowedTypes = "";
                        for (int n = 0; n < type.Length; n++) {
                            allowedTypes += type[n].ToString() + ", ";
                        }
                        Story story = new Story();
                        StorySegment segment = new StorySegment();
                        segment.Action = Enums.StoryAction.Say;
                        segment.AddParameter("Text", "You may only enter this dungeon with " + allowedTypes + " Pokemon!");
                        segment.AddParameter("Mugshot", "-1");
                        segment.Parameters.Add("Speed", "0");
                        segment.Parameters.Add("PauseLocation", "0");
                        story.Segments.Add(segment);

                        StoryManager.PlayStory(client, story);
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool OneTypeAllowed(Client client, params Enums.PokemonType[] type) {
            for (int i = 0; i < type.Length; i++) {
                for (int team = 0; team < client.Player.Team.Length; team++) {
                    if (client.Player.Team[team].Type1 == type[i] || client.Player.Team[team].Type2 == type[i]) {
                        return true;
                    }
                }
            }
            string allowedTypes = "";
            for (int n = 0; n < type.Length; n++) {
            	allowedTypes += type[n].ToString() + ", ";
            }
            Story story = new Story();
            StorySegment segment = new StorySegment();
            segment.Action = Enums.StoryAction.Say;
            segment.AddParameter("Text", "You must enter this dungeon with at least one " + allowedTypes + " Pokemon!");
            segment.AddParameter("Mugshot", "-1");
            segment.Parameters.Add("Speed", "0");
            segment.Parameters.Add("PauseLocation", "0");
            story.Segments.Add(segment);

            StoryManager.PlayStory(client, story);
            return false;
        }

        public static bool TeamLimit(Client client, int limit) {
            if (limit < 1) {
                limit = 1;
            }
            if (limit > client.Player.Team.Length) {
                limit = client.Player.Team.Length;
            }
            int teamMemberCount = 0;
            for (int i = 0; i < client.Player.Team.Length; i++) {
                if (client.Player.Team[i].Loaded) {
                    teamMemberCount++;
                }
            }
            if (teamMemberCount > limit) {
                Story story = new Story();
                StorySegment segment = new StorySegment();
                segment.Action = Enums.StoryAction.Say;
                segment.AddParameter("Text", "You may only enter this dungeon with " + limit + " team members! Come back when you've sent some home!");
                segment.AddParameter("Mugshot", "-1");
                segment.Parameters.Add("Speed", "0");
                segment.Parameters.Add("PauseLocation", "0");
                story.Segments.Add(segment);

                StoryManager.PlayStory(client, story);
                return false;
            } else {
                return true;
            }
        }
        
        */
        
        
        public static string GetRequirementString(string[] parameters) {
        	//Messenger.AdminMsg(parameters[0], Text.Black);
        	int index = parameters[0].ToInt();
        	switch (index) {
        		case 1:
        			return "At least one of your team members must be Level " + parameters[1] + " or above.";
        		case 2: 
        			return "At all of your team members must be Level " + parameters[1] + " or above.";
        		case 3: {
        				if (parameters[1].ToInt() > 0) {
        					return "Your inventory must have no more than " + parameters[1] + " items.";
        				} else {
        					return "Your inventory must be completely empty.";
        				}
        			}
        			break;
        		case 4: {
        				string typesAllowed = "";
        				for (int i = 1; i < parameters.Length; i++) {
        					if (i > 1) {
        						if (i == parameters.Length - 1) {
        							typesAllowed += ", or " + ((Enums.PokemonType)parameters[1].ToInt()).ToString() + "-";
        						} else {
        							typesAllowed += ", " + ((Enums.PokemonType)parameters[1].ToInt()).ToString() + "-";
        						}
        					} else {
        						typesAllowed += ((Enums.PokemonType)parameters[1].ToInt()).ToString() + "-";
        					}
        				}
        				return "At least one of your team members must be a " + typesAllowed + " type Pokémon.";
        			
        			}
        			break;
        		case 5: {
        				string typesAllowed = "";
        				for (int i = 1; i < parameters.Length; i++) {
        					if (i > 1) {
        						if (i == parameters.Length - 1) {
        							typesAllowed += ", or " + ((Enums.PokemonType)parameters[1].ToInt()).ToString() + "-";
        						} else {
        							typesAllowed += ", " + ((Enums.PokemonType)parameters[1].ToInt()).ToString() + "-";
        						}
        					} else {
        						typesAllowed += ((Enums.PokemonType)parameters[1].ToInt()).ToString() + "-";
        					}
        				}
        				
        				return "All of your team members must be " + typesAllowed + "-type Pokémon.";
        			
        			}
        			break;
        		case 6:
        			return "Your team must have no more than " + parameters[1] + " Pokémon.";
        		default:
        			return "[ERROR]An unknown requirement.  Report this to a staff member.";
        	
        	}
        }
        
        
        public static bool MeetsDungeonRequirement(Client client, string[] parameters) {
        	//Messenger.AdminMsg(parameters[0], Text.Black);
        	int index = parameters[0].ToInt();
        	switch (index) {
        		case 1: {
        				//return "At least one of your team members must be Level " + parameters[1] + " or above.";
        				for (int i = 0; i < client.Player.Team.Length; i++) {
                			if (client.Player.Team[i].Level >= parameters[1].ToInt()) {
                				return true;
                			}
                		}
        				return false;
        			
        			}
        			break;
        		case 2: {
        				//return "At all of your team members must be Level " + parameters[1] + " or above.";
        				for (int i = 0; i < client.Player.Team.Length; i++) {
                			if (client.Player.Team[i].Level < parameters[1].ToInt()) {
                				return false;
                			}
                		}
        				return true;
        			}
        			break;
        		case 3: {
        				//return "Your inventory must have no more than " + parameters[1] + " items.";
        				int itemCounter = 0;
        				//Messenger.AdminMsg(parameters[1], Text.Black);
			            for (int i = 1; i <= client.Player.MaxInv; i++) {
			                if (client.Player.Inventory[i].Num > 0) {
			                    itemCounter++;
			                    if (itemCounter > parameters[1].ToInt()) return false;
			                }
			            }
			            return true;
        			}
        			break;
        		case 4:{
        				//return "At least one of your team members must be a " + ((Enums.PokemonType)parameters[1].ToInt()).ToString() + "-type Pokémon.";
        				for (int i = 1; i < parameters.Length; i++) {
			                for (int team = 0; team < client.Player.Team.Length; team++) {
			                    if (client.Player.Team[team].Type1 == (Enums.PokemonType)parameters[i].ToInt() || client.Player.Team[team].Type2 == (Enums.PokemonType)parameters[i].ToInt()) {
			                    	return true;
			                    }
			                }
			            }
			            
			            return false;
        			}
        			break;
        		case 5: {
        				//return "All of your team members must be " + ((Enums.PokemonType)parameters[1].ToInt()).ToString() + "-type Pokémon.";
        				
        				for (int i = 1; i < parameters.Length; i++) {
			                for (int team = 0; team < client.Player.Team.Length; team++) {
			                    if (client.Player.Team[team].Type1 != (Enums.PokemonType)parameters[i].ToInt() && client.Player.Team[team].Type2 != (Enums.PokemonType)parameters[i].ToInt()) {
			                    	return false;
			                    }
			                }
			            }
			            
			            return true;
			            
        			}
        			break;
        		case 6: {
        				//return "Your team must have no more than " + parameters[1] + " Pokémon.";
        				int teamMemberCount = 0;
			            for (int i = 0; i < client.Player.Team.Length; i++) {
			                if (client.Player.Team[i].Loaded) {
			                    teamMemberCount++;
			                }
			            }
			            return (teamMemberCount <= parameters[1].ToInt());
        			}
        			break;
        		default:
        			return true;
        	
        	}
        }
        
        public static bool CheckDungeonRequirements(Client client, string[] reqString) {
        
        	
        	
        	//if (dungeonInfo.IsNumeric()) {
					//official dungeon
					
					//if (!DungeonManager.Dungeons[dungeonInfo.ToInt() - 1].ScriptList.ContainsKey(1)) return true;
					
					//reqString = DungeonManager.Dungeons[dungeonInfo.ToInt() - 1].ScriptList[1].Split(';');
				//} else {
					//unofficial dungeon
					
					//reqString = dungeonInfo.Split(':')[1].Split(';');
				//}
        	if (reqString != null) {
	        	for (int i = 0; i < reqString.Length; i++) {
	        		
		        	if (!MeetsDungeonRequirement(client, reqString[i].Split(','))) return false;
		        }
	        }
	        return true;
        }
        
        
        
        public static void AppendEntranceReqs(Story story, string dungeonName, string[] reqString) {
        	
        	//if (DungeonManager.Dungeons[dungeonIndex].ScriptList.ContainsKey(2)) {
	        	
	        	
	        	StorySegment segment = new StorySegment();
	        	
	        	if (reqString.Length == 1) {
		        	string req = GetRequirementString(reqString[0].Split(','));
		        	string start = req.Substring(0,1).ToLower();
		        	segment.Action = Enums.StoryAction.Say;
		            segment.AddParameter("Text", "Notice: In order to enter " + dungeonName + ", " + start + req.Substring(1, req.Length - 1));
		            segment.AddParameter("Mugshot", "-1");
		            segment.Parameters.Add("Speed", "0");
		            segment.Parameters.Add("PauseLocation", "0");
		            story.Segments.Add(segment);
	            	return;
	            } else {
	            	
					
					
		        	segment.Action = Enums.StoryAction.Say;
		            segment.AddParameter("Text", "Notice: You must meet certain requirements before entering " + dungeonName + ".");
		            segment.AddParameter("Mugshot", "-1");
		            segment.Parameters.Add("Speed", "0");
		            segment.Parameters.Add("PauseLocation", "0");
		            story.Segments.Add(segment);
	            }
	        	
	        	
	        	
	        	for (int i = 0; i < reqString.Length; i++) {
	        	
	        	
	        	
	        		string req = GetRequirementString(reqString[i].Split(','));
	        		
	        		segment = new StorySegment();
	        		segment.Action = Enums.StoryAction.Say;
	        		
	        		if (i == reqString.Length - 1) {
		        		string start = req.Substring(0,1).ToLower();
			            segment.AddParameter("Text", "Also, " + start + req.Substring(1, req.Length - 1));
		            } else {
		            	segment.AddParameter("Text", req + "..");
		            }
		            
			        segment.AddParameter("Mugshot", "-1");
			        segment.Parameters.Add("Speed", "0");
			        segment.Parameters.Add("PauseLocation", "0");
			        story.Segments.Add(segment);
	        	}
	        	
	        	
        	//} else {
        	//	return null;
        	//}
        }
        
        
        public static string GetWarningString(string[] parameters) {
        	//Messenger.AdminMsg(parameters[0], Text.Black);
        	int index = parameters[0].ToInt();
        	switch (index) {
        		case 0:
        			return "You cannot be rescued.";
        		case 1: {
        				if (parameters[2] == "1") {
        					return "The team's levels will be temporarily set to Level " + parameters[1] + ", though they will keep their current moves. (They will be restored upon leaving the dungeon)";
        				} else {
        					return "The team's levels will be temporarily set to Level " + parameters[1] + ". (They will be restored upon leaving the dungeon)";
        				}
        			}
        			break;
        		case 2:
        			return "You cannot recruit any Pokémon you defeat.";
        		case 3:
        			return "You will not earn Exp. Points from Pokémon you defeat.";
        		case 4:
        			return "Every item in your bag will be lost upon leaving the dungeon, even if you were not defeated.";
        		case 5:
        			return "Any parties that you are in will be disbanded.";
        		default:
        			return "[ERROR]An unknown restriction.  Report this to a staff member.";
        	
        	}
        }
        
        
        
        public static void ApplyDungeonRestriction(Client client, string[] parameters) {
        	int index = parameters[0].ToInt();
        	switch (index) {
        		case 0: {
        			//return "You cannot be rescued.";
        			}
        			break;
        		case 1: {
        				if (parameters[2] == "1") {
        					//return "The team's levels will be temporarily set to Level " + parameters[1] + ", though they will keep their current moves. (They will be restored upon leaving the dungeon)";
        					client.Player.BeginTempStatMode(parameters[1].ToInt(), true);
        				} else {
        					//return "The team's levels will be temporarily set to Level " + parameters[1] + ". (They will be restored upon leaving the dungeon)";
        					client.Player.BeginTempStatMode(parameters[1].ToInt(), false);
        				}
        			}
        			break;
        		case 2: {
        			//return "You cannot recruit any Pokémon you defeat.";
        			}
        			break;
        		case 3: {
        			//return "You will not earn Exp. Points from Pokémon you defeat.";
        			}
        			break;
        		case 4: {
        			//return "Every item in your bag will be lost upon leaving the dungeon, even if you were not defeated.";
        			}
        			break;
        		case 5: {
        			//return "Any parties that you are in will be disbanded.";
        				if (client.Player.PartyID != null) {
                            Party party = PartyManager.FindPlayerParty(client);
	                        PartyManager.RemoveFromParty(party, client);
                        }
        			}
        			break;
        	
        	}
        }
        
        public static void ApplyDungeonRestrictions(Client client, string dungeonInfo) {
        	
        	string[] restrictionString = null;
        	
        	if (dungeonInfo.IsNumeric()) {
					//official dungeon
					
					if (DungeonManager.Dungeons[dungeonInfo.ToInt() - 1].ScriptList.ContainsKey(2)) {
						restrictionString = DungeonManager.Dungeons[dungeonInfo.ToInt() - 1].ScriptList[2].Split(';');
					}
				} else {
					//unofficial dungeon
					
					restrictionString = dungeonInfo.Split(':')[2].Split(';');
				}
        	if (restrictionString != null) {
	        	for (int i = 0; i < restrictionString.Length; i++) {
		        	ApplyDungeonRestriction(client, restrictionString[i].Split(','));
		        }
	        }
        }
        
        public static void AppendEntranceWarning(Story story, string dungeonName, string[] restrictionString) {
        	
        	//if (DungeonManager.Dungeons[dungeonIndex].ScriptList.ContainsKey(2)) {
	        	
	        	
	        	StorySegment segment = new StorySegment();
	        	
	        	if (restrictionString.Length == 1) {
		        	
		        	segment.Action = Enums.StoryAction.Say;
		            segment.AddParameter("Text", "Caution!  " + GetWarningString(restrictionString[0].Split(',')));
		            segment.AddParameter("Mugshot", "-1");
		            segment.Parameters.Add("Speed", "0");
		            segment.Parameters.Add("PauseLocation", "0");
		            story.Segments.Add(segment);
	            	return;
	            } else {
	            	
					
					
		        	segment.Action = Enums.StoryAction.Say;
		            segment.AddParameter("Text", "Caution!  There are certain restrictions to entering " + dungeonName + ".");
		            segment.AddParameter("Mugshot", "-1");
		            segment.Parameters.Add("Speed", "0");
		            segment.Parameters.Add("PauseLocation", "0");
		            story.Segments.Add(segment);
	            }
	        	
	        	
	        	
	        	for (int i = 0; i < restrictionString.Length; i++) {
	        	
	        	
	        	
	        		string warning = GetWarningString(restrictionString[i].Split(','));
	        		
	        		segment = new StorySegment();
	        		segment.Action = Enums.StoryAction.Say;
	        		
	        		if (i == restrictionString.Length - 1) {
		        		string start = warning.Substring(0,1).ToLower();
			            segment.AddParameter("Text", "Also, " + start + warning.Substring(1, warning.Length - 1));
		            } else {
		            	segment.AddParameter("Text", warning + "..");
		            }
		            
			        segment.AddParameter("Mugshot", "-1");
			        segment.Parameters.Add("Speed", "0");
			        segment.Parameters.Add("PauseLocation", "0");
			        story.Segments.Add(segment);
	        	}
	        	
	        	
        	//} else {
        	//	return null;
        	//}
        }
		
		public static Story CreatePreDungeonStory(Client client, string param1, string param2, string param3, bool random) {
			
			Story story = new Story();
			
			StorySegment segment = new StorySegment();
            segment.Action = Enums.StoryAction.PlayerPadlock;
            segment.AddParameter("MovementState", "Lock");
            story.Segments.Add(segment);
            
			if (param3 == "") {
				//continuing a dungeon
				
				segment = new StorySegment();
				segment.Action = Enums.StoryAction.AskQuestion;
				segment.AddParameter("Question", "Will you go on?");
				segment.AddParameter("SegmentOnYes", (story.Segments.Count + 2).ToString());
				segment.AddParameter("SegmentOnNo", (story.Segments.Count + 3).ToString());
				segment.AddParameter("Mugshot", "-1");
				story.Segments.Add(segment);
			} else {
				
				string dungeonName = "";
				string reqString = "";
				string restrictionString = "";
				
				if (param3.IsNumeric()) {
					//official dungeon
					
					dungeonName = DungeonManager.Dungeons[param3.ToInt() - 1].Name;
					
					if (DungeonManager.Dungeons[param3.ToInt() - 1].ScriptList.ContainsKey(1)) {
						reqString = DungeonManager.Dungeons[param3.ToInt() - 1].ScriptList[1];
					}
					
					if (DungeonManager.Dungeons[param3.ToInt() - 1].ScriptList.ContainsKey(2)) {
						restrictionString = DungeonManager.Dungeons[param3.ToInt() - 1].ScriptList[2];
					}
				} else {
					//unofficial dungeon
					
					dungeonName = param3.Split(':')[0];
					reqString = param3.Split(':')[1];
					restrictionString = param3.Split(':')[2];
				}
				
				
				if (CheckDungeonRequirements(client, reqString.Split(';'))) {
				
					
					if (restrictionString != "") {
						
						AppendEntranceWarning(story, dungeonName, restrictionString.Split(';'));
						
						segment = new StorySegment();
						segment.Action = Enums.StoryAction.AskQuestion;
						segment.AddParameter("Question", "Will you enter " + dungeonName + "?");
						segment.AddParameter("SegmentOnYes", "3");
						segment.AddParameter("SegmentOnNo", (story.Segments.Count + 4).ToString());
						segment.AddParameter("Mugshot", "-1");
						story.Segments.Insert(1, segment);
						
						
						segment = new StorySegment();
						segment.Action = Enums.StoryAction.AskQuestion;
						segment.AddParameter("Question", "Is that OK?");
						segment.AddParameter("SegmentOnYes", (story.Segments.Count + 2).ToString());
						segment.AddParameter("SegmentOnNo", (story.Segments.Count + 3).ToString());
						segment.AddParameter("Mugshot", "-1");
						story.Segments.Add(segment);
						
					} else {
						
						segment = new StorySegment();
						segment.Action = Enums.StoryAction.AskQuestion;
						segment.AddParameter("Question", "Will you enter " + dungeonName + "?");
						segment.AddParameter("SegmentOnYes", (story.Segments.Count + 2).ToString());
						segment.AddParameter("SegmentOnNo", (story.Segments.Count + 3).ToString());
						segment.AddParameter("Mugshot", "-1");
						story.Segments.Insert(1, segment);
						
					}
					
				} else {
					
					
						AppendEntranceReqs(story, dungeonName, reqString.Split(';'));
						
						segment = new StorySegment();
			            segment.Action = Enums.StoryAction.GoToSegment;
			            segment.AddParameter("Segment", (story.Segments.Count + 3).ToString());
			
			            story.Segments.Add(segment);
						
						segment = new StorySegment();
						segment.Action = Enums.StoryAction.AskQuestion;
						segment.AddParameter("Question", "Will you enter " + dungeonName + "?");
						segment.AddParameter("SegmentOnYes", "3");
						segment.AddParameter("SegmentOnNo", (story.Segments.Count + 3).ToString());
						segment.AddParameter("Mugshot", "-1");
						story.Segments.Insert(1, segment);
						
					
				}
				
			}
			
			segment = new StorySegment();
            segment.Action = Enums.StoryAction.RunScript;
            if (random) {
            	segment.AddParameter("ScriptIndex", "42");
            } else {
            	segment.AddParameter("ScriptIndex", "52");
            }
            segment.AddParameter("ScriptParam1", param1);
            segment.AddParameter("ScriptParam2", param2);
            segment.AddParameter("ScriptParam3", param3);
            segment.AddParameter("Pause", "1");
            story.Segments.Add(segment);
			
			
			segment = new StorySegment();
            segment.Action = Enums.StoryAction.PlayerPadlock;
            segment.AddParameter("MovementState", "Unlock");
            story.Segments.Add(segment);
			
			foreach (StorySegment segments in story.Segments) {
				//Messenger.AdminMsg(segments.Action.ToString(), Text.Black);
			}
			
			return story;
		
		}
		
		public static void StartRDungeon(Client client, string param1, string param2, string param3) {
			
			if (client.Player.PartyID == null) {
				StartRDungeonSolo(client, param1, param2, param3);
			} else {
				//StartRDungeonSolo(client, param1, param2, param3);
				bool ready = true;
				Party party = PartyManager.FindPlayerParty(client);
				foreach (string charID in party.GetMemberCharacters()) {
					Client member = ClientManager.FindClientFromCharID(charID);
					if (member != client && (member == null || member.Player.QuestionID != ("PartyRDungeon|" + param1 + "|" + param2 + "|" + param3))) {
						ready = false;
					}
				}
				
				if (ready) {
					List<Client> members = new List<Client>();
					foreach (Client member in party.GetOnlineMemberClients()) {
						members.Add(member);
					}
					
					foreach (Client member in members) {
						StartRDungeonSolo(member, param1, param2, param3);
						Messenger.ForceEndStoryTo(member);
					}
				} else {
					AskRDungeonCancelQuestion(client, param1, param2, param3);
				}
			}
			
		}

        public static void StartRDungeonSolo(Client client, string param1, string param2, string param3) {

            if (param3 == "") {
                client.Player.WarpToRDungeon(param1.ToInt() - 1, param2.ToInt() - 1);
            } else if (param3.IsNumeric()) {
                ApplyDungeonRestrictions(client, param3);
                client.Player.WarpToRDungeon(param1.ToInt() - 1, param2.ToInt() - 1);
                client.Player.AddDungeonAttempt(param3.ToInt() - 1);

            } else {
                ApplyDungeonRestrictions(client, param3);
                client.Player.WarpToRDungeon(param1.ToInt() - 1, param2.ToInt() - 1);
            }
            OnDungeonStart(client, param3.ToInt() - 1);
            
            string dungeonName = "";
			string reqString = "";
			string restrictionString = "";
            
            if (param3.IsNumeric()) {
				//official dungeon
				
				dungeonName = DungeonManager.Dungeons[param3.ToInt() - 1].Name;
				
				if (DungeonManager.Dungeons[param3.ToInt() - 1].ScriptList.ContainsKey(1)) {
					reqString = DungeonManager.Dungeons[param3.ToInt() - 1].ScriptList[1];
				}
				
				if (DungeonManager.Dungeons[param3.ToInt() - 1].ScriptList.ContainsKey(2)) {
					restrictionString = DungeonManager.Dungeons[param3.ToInt() - 1].ScriptList[2];
				}
			} else if (param3 != "") {
				//unofficial dungeon
				
				dungeonName = param3.Split(':')[0];
				reqString = param3.Split(':')[1];
				restrictionString = param3.Split(':')[2];
			}
			
			
			if (!CheckDungeonRequirements(client, reqString.Split(';'))) {
				ExitDungeon(client, -1, 0, 0);
			}
        }
		
		public static void StartDungeon(Client client, string param1, string param2, string param3) {
			
			if (client.Player.PartyID == null) {
				StartDungeonSolo(client, param1, param2, param3);
			} else {
				//StartDungeonSolo(client, param1, param2, param3);
				
				bool ready = true;
				Party party = PartyManager.FindPlayerParty(client);
				foreach (string charID in party.GetMemberCharacters()) {
					Client member = ClientManager.FindClientFromCharID(charID);
					if (member != client && (member == null || member.Player.QuestionID != ("PartyDungeon|" + param1 + "|" + param2 + "|" + param3))) {
						ready = false;
					}
				}
				
				if (ready) {
					List<Client> members = new List<Client>();
					foreach (Client member in party.GetOnlineMemberClients()) {
						members.Add(member);
					}
					
					foreach (Client member in members) {
						StartDungeonSolo(member, param1, param2, param3);
						Messenger.ForceEndStoryTo(member);
					}
				} else {
					AskDungeonCancelQuestion(client, param1, param2, param3);
				}
			}
			
		}


        public static void StartDungeonSolo(Client client, string param1, string param2, string param3) {

            string[] XY = param2.Split(':');
            if (param3 == "") {
                Messenger.PlayerWarp(client, param1.ToInt(), XY[0].ToInt(), XY[1].ToInt());
            } else if (param3.IsNumeric()) {
                ApplyDungeonRestrictions(client, param3);
                Messenger.PlayerWarp(client, param1.ToInt(), XY[0].ToInt(), XY[1].ToInt());
                client.Player.AddDungeonAttempt(param3.ToInt() - 1);

            } else {
                ApplyDungeonRestrictions(client, param3);
                Messenger.PlayerWarp(client, param1.ToInt(), XY[0].ToInt(), XY[1].ToInt());
            }
            OnDungeonStart(client, param3.ToInt() - 1);
        }

        public static void OnDungeonStart(Client client, int dungeonIndex) {
            //add escorts
            bool joined = false;
            for (int i = 0; i < client.Player.JobList.JobList.Count; i++) {
                if (client.Player.JobList.JobList[i].Mission.DungeonIndex == dungeonIndex && client.Player.JobList.JobList[i].Accepted == Enums.JobStatus.Taken
                    && client.Player.JobList.JobList[i].Mission.MissionType == Enums.MissionType.Escort) {
                    int openSlot = client.Player.FindOpenTeamSlot();
                    if (openSlot > -1) {
                        client.Player.AddToTeamTemp(openSlot,
                            Server.WonderMails.WonderMailManager.Missions[(int)client.Player.JobList.JobList[i].Mission.Difficulty - 1].MissionClients[client.Player.JobList.JobList[i].Mission.MissionClientIndex].Species,
                            Server.WonderMails.WonderMailManager.Missions[(int)client.Player.JobList.JobList[i].Mission.Difficulty - 1].MissionClients[client.Player.JobList.JobList[i].Mission.MissionClientIndex].Form,
                            5, i);
                        Messenger.PlayerMsg(client, Server.Pokedex.Pokedex.GetPokemon(Server.WonderMails.WonderMailManager.Missions[(int)client.Player.JobList.JobList[i].Mission.Difficulty - 1].MissionClients[client.Player.JobList.JobList[i].Mission.MissionClientIndex].Species).Name +
                            " joined the team!", Text.BrightGreen);
                        joined = true;

                    }
                }
            }
            for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
                    if (client.Player.Team[i] != null) {
                        if (client.Player.Team[i].MaxBelly > 100) {

                            client.Player.Team[i].MaxBelly = 100;

                        }
                        client.Player.Team[i].RestoreBelly();

                    }
                }
            if (joined) Messenger.SendActiveTeam(client);
        }
        
        
        public static void ExitDungeon(Client client, int exitMap, int x, int y) {
        	if (Ranks.IsAllowed(client, Enums.Rank.Mapper)) {
        		Messenger.AdminMsg(client.Player.Name + "Completed dungeon.", Text.Blue);
        	}
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
            bool mission = client.Player.HandleMissionRewardDump();
            if (!mission) {
                if (exitMap > 0) {
                	Messenger.PlayerWarp(client, exitMap, x, y);
                } else {
                	exPlayer.Get(client).WarpToSpawn(false);
                }
            }
        }

        public static void AskRDungeonCancelQuestion(Client client, string param1, string param2, string param3) {
            
            List<string> choices = new List<string>();

            choices.Add("Cancel");

            Messenger.AskQuestion(client, "PartyRDungeon|" + param1 + "|" + param2 + "|" + param3, "Waiting for party members to enter...", -1, choices.ToArray());
        }
		

        public static void AskDungeonCancelQuestion(Client client, string param1, string param2, string param3) {
            
            List<string> choices = new List<string>();

            choices.Add("Cancel");

            Messenger.AskQuestion(client, "PartyDungeon|" + param1 + "|" + param2 + "|" + param3, "Waiting for party members to enter...", -1, choices.ToArray());
        }

        //public static bool IsDungeonUnlocked(Client client, int dungeonIndex) {
        //    if (client.Player.GetDungeonCompletionCount(dungeonIndex) > 0) {
        //        return true;
        //    } else {
        //        if (client.Player.ActiveMission != null) {
        //            return VerifyDungeonCode(client.Player.ActiveMission.WonderMail.Code, dungeonIndex);
        //        } else {
        //            return false;
        //        }
        //    }
        //}

        private static bool VerifyDungeonCode(string code, int dungeonIndex) {
            switch (dungeonIndex) {
                case 0: { // Tiny Grotto missions
                        switch (code) {
                            case "0|-1|0|0|7|0|0|3|1|9|-1|46|0|3|0|0|-1|-1|-1|-1|0|-1|-1|-1|":
                                return true;
                            default:
                                return false;
                        }
                    }
                    break;
                default:
                    return false;
            }
        }
    }
}

using Server;
using Server.Scripting;
using Server.Network;
using Server.Maps;
using Server.Stories;
using Server.Players;
using System;
using System.Drawing;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Script 
{
	public class StoryHelper
	{
		public static void ResetStory(Client client) {
			if (exPlayer.Get(client).StoryEnabled) {
				client.Player.StoryHelper.SaveSetting("[MainStory]-CurrentSection", "0");
				
				for (int i = 611 - 1; i < 620 - 1; i++) {
					client.Player.SetStoryState(i, false);
				}
				
				Messenger.PlayerWarp(client, 1015, 25, 25);
				Messenger.PlayerMsg(client, "The main story has been reset!", Text.BrightGreen);
            }
		}
	
		public static void OnMapLoaded(Client client, IMap map, PacketHitList packetList) {
			if (exPlayer.Get(client).StoryEnabled) {
				int currentStorySection = client.Player.StoryHelper.ReadSetting("[MainStory]-CurrentSection").ToInt();
				switch (map.MapID) {
					case "s50": {
							if (currentStorySection == 4) { // Retreive 500 Poke for Team Jaw
								if (client.Player.HasItem(1) >= 500) {
									// We have the 500 Poke
									client.Player.TakeItem(1, 500);
									// Move forward one section so we don't have to take the money again
									currentStorySection++;
									client.Player.StoryHelper.SaveSetting("[MainStory]-CurrentSection", currentStorySection.ToString());
								} else {
									// We don't have the 500 Poke
									StoryManager.PlayStory(client, 619 - 1);
								}
							}
							if (currentStorySection == 5) { // We already delivered the 500 Poke to Team Jaw
								StoryManager.PlayStory(client, 615 - 1);
							}
						}
						break;
					case "s737": {
							if (currentStorySection == 1) {
								StoryManager.PlayStory(client, 612 - 1);
							}
						}
						break;
					case "s1286": {
							if (client.Player.Y > 8 && client.Player.Y < 40 &&
								currentStorySection == 2) {
								StoryManager.PlayStory(client, 613 - 1);
							}
						}
						break;
				}
			
			}
		}
		
		// Add new riddle data here
		// [MapId, X, Y, Story # from editor]
		public static AnniversaryRiddle[] annivRiddles = new AnniversaryRiddle[] {
			new AnniversaryRiddle("s350", 20, 21, 360),
			new AnniversaryRiddle("s350", 22, 23, 360)
		};
		public static void OnStep(Client client, PacketHitList hitlist) {
			// NOTE: Remove the mapper+ check for release
			if (exPlayer.Get(client).AnniversaryEnabled && client.Player.CurrentChapter == null && Ranks.IsAllowed(client, Enums.Rank.Mapper)) {
				int currentAnniversarySection = client.Player.StoryHelper.ReadSetting("[Anniversary12]-CurrentSection").ToInt();
				//Messenger.PlayerMsg(client, "Section: " + currentAnniversarySection, Text.BrightRed);
				// NOTE: Do /not/ remove the mapper+ check here, this resets the story when you reach the end
				if (currentAnniversarySection == annivRiddles.Length && Ranks.IsAllowed(client, Enums.Rank.Mapper)) {
					currentAnniversarySection = 0;
					client.Player.StoryHelper.SaveSetting("[Anniversary12]-CurrentSection", currentAnniversarySection.ToString());
					Messenger.PlayerMsg(client, "Section reset!", Text.BrightRed);
				}
				if (currentAnniversarySection >= 0 && currentAnniversarySection < annivRiddles.Length) {
					AnniversaryRiddle currentRiddle = annivRiddles[currentAnniversarySection];
					if (client.Player.MapID == currentRiddle.MapId &&
						client.Player.X == currentRiddle.X && client.Player.Y == currentRiddle.Y) {
						
						client.Player.SetStoryState(currentRiddle.Story - 1, false);
                		StoryManager.PlayStory(client, currentRiddle.Story - 1);
                		
                		currentAnniversarySection++;
                		client.Player.StoryHelper.SaveSetting("[Anniversary12]-CurrentSection", currentAnniversarySection.ToString());
					}
				}
				
				
			}
			if (exPlayer.Get(client).StoryEnabled && client.Player.CurrentChapter == null) {
				int currentStorySection = client.Player.StoryHelper.ReadSetting("[MainStory]-CurrentSection").ToInt();
				switch (client.Player.MapID) {
					case "s737": {
							if (currentStorySection == 3) { // Tiny Grotto complete, first time [Bulbasaur mission]
								if (client.Player.X > 20 && client.Player.X < 35 && client.Player.Y > 43) {
									if (client.Player.GetDungeonCompletionCount(1 - 1) >= 1) {
										StoryManager.PlayStory(client, 614 - 1);
									}
								}
							}
						}
						break;
					case "s1934": {
							if (currentStorySection == 6) {
								if (client.Player.X == 10 && client.Player.Y == 9) {
									StoryManager.PlayStory(client, 616 - 1);
								}
							}
						}
						break;
				}
			}
			
			
		}
	}
	
	public class AnniversaryRiddle
	{
		public string MapId { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public int Story { get; set; }
		
		public AnniversaryRiddle(string mapId, int x, int y, int story) {
			this.MapId = mapId;
			this.X = x;
			this.Y = y;
			this.Story = story;
		}
	}
}
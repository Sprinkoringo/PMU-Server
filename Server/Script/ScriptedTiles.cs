
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
    
	public partial class Main {
	
		public static void ScriptedTile(IMap map, ICharacter character, int script, string param1, string param2, string param3, PacketHitList hitlist) {
            try {
                PacketHitList.MethodStart(ref hitlist);
                Client client = null;
                if (character.CharacterType == Enums.CharacterType.Recruit) {
                    client = ((Recruit)character).Owner;
                }
                switch (script) {
                    case 0: {
                            if (character.CharacterType == Enums.CharacterType.Recruit) {
                                hitlist.AddPacket(((Recruit)character).Owner, PacketBuilder.CreateChatMsg("Empty Script", Text.Black));
                            }
                        }
                        break;
                    case 1: { // Story runner
                            if (exPlayer.Get(client).StoryEnabled) {
                                int storyNum = param1.ToInt() - 1;
                                if (client.Player.GetStoryState(storyNum) == false) {
                                    StoryManager.PlayStory(client, storyNum);
                                }
                            }
                        }
                        break;

                    case 2: {//Explosion trap
                            if (WillTrapActivate(character, map, character.X, character.Y)) {
                                RevealTrap(map, character.X, character.Y, hitlist);
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " stepped on an Explosion Trap!", Text.BrightRed), character.X, character.Y, 10);
                                ActivateTrap(map, character.X, character.Y, script, hitlist);
                            }
                        }
                        break;
                    case 3: {//Chestnut Trap
                            if (WillTrapActivate(character, map, character.X, character.Y)) {
                                RevealTrap(map, character.X, character.Y, hitlist);
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " stepped on a Chestnut Trap!", Text.BrightRed), character.X, character.Y, 10);
                                ActivateTrap(map, character.X, character.Y, script, hitlist);
                            }
                        }
                        break;

                    case 4: {//PP-Zero Trap
                            if (WillTrapActivate(character, map, character.X, character.Y)) {
                                RevealTrap(map, character.X, character.Y, hitlist);
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " stepped on an PP-Zero Trap!", Text.BrightRed), character.X, character.Y, 10);
                                ActivateTrap(map, character.X, character.Y, script, hitlist);
                            }
                        }
                        break;
                    case 5: {//Grimy Trap
                            if (WillTrapActivate(character, map, character.X, character.Y)) {
                                RevealTrap(map, character.X, character.Y, hitlist);
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " stepped on a Grimy Trap!", Text.BrightRed), character.X, character.Y, 10);
                                ActivateTrap(map, character.X, character.Y, script, hitlist);
                            }
                        }
                        break;

                    case 6: {//Poison Trap
                            if (WillTrapActivate(character, map, character.X, character.Y)) {
                                RevealTrap(map, character.X, character.Y, hitlist);
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " stepped on a Poison Trap!", Text.BrightRed), character.X, character.Y, 10);
                                ActivateTrap(map, character.X, character.Y, script, hitlist);
                            }
                        }
                        break;
                    case 7: {//Random Trap
                            if (WillTrapActivate(character, map, character.X, character.Y)) {
                                RevealTrap(map, character.X, character.Y, hitlist);
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " stepped on a Random Trap!", Text.BrightRed), character.X, character.Y, 10);
                                ActivateTrap(map, character.X, character.Y, script, hitlist);
                                if (Server.Math.Rand(0, 10) + 6 < map.Tile[character.X, character.Y].Data2) {
                                    RemoveTrap(map, character.X, character.Y, hitlist);
                                }
                            }
                        }
                        break;
                    case 8: {//Appraisal;
                            if (client != null) {
                                int boxes = 0;
                                for (int i = 1; i <= client.Player.Inventory.Count; i++) {
                                    if (client.Player.Inventory[i].Num != 0
                                        && ItemManager.Items[client.Player.Inventory[i].Num].Type == Enums.ItemType.Scripted
                                        && ItemManager.Items[client.Player.Inventory[i].Num].Data1 == 12
                                        && !client.Player.Inventory[i].Sticky) {
                                        boxes++;
                                    }
                                }
                                if (boxes > 0) {
                                    Messenger.AskQuestion(client, "Appraisal", "Will you have your boxes opened?  It will cost " + 150 * boxes + " Poké.", -1);
                                } else {
                                    Story story = new Story();
                                    StoryBuilderSegment segment = StoryBuilder.BuildStory();
                                    StoryBuilder.AppendSaySegment(segment, "You can bring your treasure boxes here to have them opened for 150 Poké each.", -1, 0, 0);
                                    segment.AppendToStory(story);
                                    StoryManager.PlayStory(client, story);
                                }
                            }
                        }
                        break;
                    case 9: {//Un-sticky;
                            if (client != null) {
                                Messenger.AskQuestion(client, "Unsticky", "Will you have your sticky items cleansed?", -1);

                            }
                        }
                        break;
                    case 10: {//Full heal
                            if (client != null) {
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
                                    }
                                }


                                hitlist.AddPacket(client, PacketBuilder.CreateBattleMsg("The entire party was fully healed!", Text.BrightGreen));
                                hitlist.AddPacket(client, PacketBuilder.CreateSoundPacket("magic25.wav"));
                                PacketBuilder.AppendPlayerMoves(client, hitlist);
                                PacketBuilder.AppendActiveTeamNum(client, hitlist);
                                PacketBuilder.AppendStatusAilment(client, hitlist);
                            }
                        }
                        break;

                    case 11: {//Warp out of Destiny Cavern
                            if (client != null) {
                                if (Settings.NewCharForm > 0) {
                                    //for (int i = 0; i < 4; i++) {

                                    client.Player.GetActiveRecruit().GenerateMoveset();
                                    client.Player.GetActiveRecruit().HP = client.Player.GetActiveRecruit().MaxHP;
                                    //if (client.Player.Team[i].Loaded) {//does there need to be a check for if the team member of the slot is there?
                                    //   client.Player.Team[i].HP = client.Player.Team[i].MaxHP;

                                    for (int j = 0; j < Constants.MAX_PLAYER_MOVES; j++) {
                                        if (client.Player.GetActiveRecruit().Moves[j].MoveNum != 0) {

                                            client.Player.GetActiveRecruit().Moves[j].CurrentPP = client.Player.GetActiveRecruit().Moves[j].MaxPP;
                                        }
                                    }
                                    //}
                                    //}
                                    //Messenger.PlayerWarp(client, 1, 10, 7);
            						exPlayer.Get(client).SpawnMap = "s1035";
		                            exPlayer.Get(client).SpawnX = 9;
		                            exPlayer.Get(client).SpawnY = 7;
            						StoryManager.PlayStory(client, 639);
            						//StoryManager.PlayStory(client, 640);
                                    //} else {
                                   	//	StoryManager.PlayStory(client, StoryConstruction.CreateIntroStory(client));
                                	//}
                                }
                            }
                        }
                        break;

                    case 12: {//Next floor of a different dungeon
                            if (client != null && map.MapType == Enums.MapType.RDungeonMap && ((RDungeonMap)map).RDungeonIndex > -1) {

                                PartyManager.AttemptPartyWarp(client, (Client warpClient) =>
                                {
                                    warpClient.Player.WarpToRDungeon(param1.ToInt() - 1, ((RDungeonMap)map).RDungeonFloor + 1);
                                });
                            }

                        }
                        break;

                    case 13: {//drop from the sky
                            if (client != null) {
                            	Messenger.AskQuestion(client, "SkyDrop:"+param1+":"+param2, "Will you land at " + param3 + "?", -1);
                            }
                        }
                        break;
                    case 14: {//Fly
                            if (client != null) {
                                Messenger.PlayerMsg(client, "A strong updraft can be felt from here...", Text.Grey);
                                //Messenger.PlaySound(client, "Magic632.wav");
                                hitlist.AddPacket(client, PacketBuilder.CreateSoundPacket("Magic632.wav"));
                            }
                        }
                        break;
                    case 15: {// Warp Trap
                            if (WillTrapActivate(character, map, character.X, character.Y)) {
                                RevealTrap(map, character.X, character.Y, hitlist);
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " stepped on a Warp Trap!", Text.BrightRed), character.X, character.Y, 10);
                                ActivateTrap(map, character.X, character.Y, script, hitlist);
                            }
                        }
                        break;
                    case 16: {// Pokemon Trap (unfinished)
                            if (WillTrapActivate(character, map, character.X, character.Y)) {
                                RevealTrap(map, character.X, character.Y, hitlist);
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " stepped on a Pokémon Trap!", Text.BrightRed), character.X, character.Y, 10);
                                ActivateTrap(map, character.X, character.Y, script, hitlist);
                                RemoveTrap(map, character.X, character.Y, hitlist);
                            }
                        }
                        break;
                    case 17: {// Spikes
                            if (WillTrapActivate(character, map, character.X, character.Y)) {
                                RevealTrap(map, character.X, character.Y, hitlist);
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " stepped on the Spikes!", Text.BrightRed), character.X, character.Y, 10);
                                ActivateTrap(map, character.X, character.Y, script, hitlist);
                            }
                        }
                        break;
                    case 18: {// Toxic spikes
                            if (WillTrapActivate(character, map, character.X, character.Y)) {
                                RevealTrap(map, character.X, character.Y, hitlist);
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " stepped on the Toxic Spikes!", Text.BrightRed), character.X, character.Y, 10);
                                ActivateTrap(map, character.X, character.Y, script, hitlist);
                            }
                        }
                        break;
                    case 19: { // Stealth Rock
                            if (WillTrapActivate(character, map, character.X, character.Y)) {
                                RevealTrap(map, character.X, character.Y, hitlist);
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " stepped on a Stealth Rock Trap!", Text.BrightRed), character.X, character.Y, 10);
                                ActivateTrap(map, character.X, character.Y, script, hitlist);
                            }
                        }
                        break;
                    case 20: { // Void
                            if (client != null) {
                                if (Ranks.IsAllowed(client, Enums.Rank.Scripter)) {
                                    Messenger.PlayerWarpToVoid(client);
                                }
                            }
                        }
                        break;
                    case 21: {//completed level x dungeon
                            if (client != null) {
                                client.Player.EndTempStatMode();
                                exPlayer.Get(client).WarpToSpawn(false);
                            }
                        }
                        break;
                    case 22: {
                            //Anti-suicide; doesn't do anything here
                        }
                        break;
                    case 23: {//sticky trap
                            if (WillTrapActivate(character, map, character.X, character.Y)) {
                                RevealTrap(map, character.X, character.Y, hitlist);
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " stepped on a Sticky Trap!", Text.BrightRed), character.X, character.Y, 10);
                                ActivateTrap(map, character.X, character.Y, script, hitlist);
                            }
                        }
                        break;
                    case 24: {//Admin-only
                            if (client != null) {
                                if (Ranks.IsDisallowed(client, Enums.Rank.Moniter)) {
                                    BlockPlayer(client);
                                    Messenger.PlayerMsg(client, "You must be an Admin to get through!", Text.BrightRed);
                                }
                            }
                        }
                        break;
                    case 25: {//mud trap
                            if (WillTrapActivate(character, map, character.X, character.Y)) {
                                RevealTrap(map, character.X, character.Y, hitlist);
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " stepped on a Mud Trap!", Text.BrightRed), character.X, character.Y, 10);
                                ActivateTrap(map, character.X, character.Y, script, hitlist);
                                if (Server.Math.Rand(0, 10) + 2 < map.Tile[character.X, character.Y].Data2) {
                                    RemoveTrap(map, character.X, character.Y, hitlist);
                                }
                            }
                        }
                        break;
                    case 26: {//wonder tile
                            if (WillTrapActivate(character, map, character.X, character.Y)) {
                                RevealTrap(map, character.X, character.Y, hitlist);
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " stepped on a Wonder Tile!", Text.BrightGreen), character.X, character.Y, 10);
                                ActivateTrap(map, character.X, character.Y, script, hitlist);
                            }
                        }
                        break;
                    case 27: {//activation
                            if (client != null) {
                                List<int> switches = new List<int>();
                                foreach (Client i in map.GetClients()) {
                                    if (i.Player.Map.Tile[i.Player.X, i.Player.Y].Type == Enums.TileType.Scripted
                                    && i.Player.Map.Tile[i.Player.X, i.Player.Y].Data1 == 27) {
                                        if (!switches.Contains(i.Player.Map.Tile[i.Player.X, i.Player.Y].Data2)) {
                                            switches.Add(i.Player.Map.Tile[i.Player.X, i.Player.Y].Data2);
                                        }
                                    }

                                }

                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " stepped on a switch!", Text.BrightGreen), character.X, character.Y, 50);

                                if (switches.Count >= param1.ToInt()) {
                                    for (int x = 0; x < map.MaxX; x++) {
                                        for (int y = 0; y < map.MaxY; y++) {
                                            if (map.Tile[x, y].Type == Enums.TileType.RDungeonGoal) {
                                                map.Tile[x, y].Mask2Set = 4;
                                                map.Tile[x, y].Mask2 = 1;
                                                map.Tile[x, y].Data1 = 1;
                                                map.TempChange = true;
                                                hitlist.AddPacketToMap(map, PacketBuilder.CreateTilePacket(x, y, map));
                                            } else if (map.Tile[x, y].Type == Enums.TileType.Scripted && map.Tile[x, y].Data1 == 27) {
                                                map.Tile[x, y].Type = Enums.TileType.Walkable;
                                                map.Tile[x, y].Fringe = 0;
                                                map.Tile[x, y].FAnim = 0;
                                                hitlist.AddPacketToMap(map, PacketBuilder.CreateTilePacket(x, y, map));
                                            }
                                        }
                                    }
                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("The passage to the next floor was opened!", Text.BrightGreen), character.X, character.Y, 50);
                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("Magic127.wav"), character.X, character.Y, 50);
                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleDivider(), character.X, character.Y, 50);
                                } else if (param1.ToInt() - switches.Count == 1) {
                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("1 more needs to be pressed at the same time...", Text.BrightGreen), character.X, character.Y, 50);
                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("Magic126.wav"), character.X, character.Y, 50);
                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleDivider(), character.X, character.Y, 50);
                                } else {
                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg((param1.ToInt() - switches.Count) + " more need to be pressed at the same time...", Text.BrightGreen), character.X, character.Y, 50);
                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateSoundPacket("Magic126.wav"), character.X, character.Y, 50);
                                    hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleDivider(), character.X, character.Y, 50);
                                }
                            }
                        }
                        break;
                    case 28: {//Trip Trap
                            if (WillTrapActivate(character, map, character.X, character.Y)) {
                                RevealTrap(map, character.X, character.Y, hitlist);
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " stepped on a Trip Trap!", Text.BrightRed), character.X, character.Y, 10);
                                ActivateTrap(map, character.X, character.Y, script, hitlist);
                            }
                        }
                        break;
                    case 29: {//CTF Red Flag Tile
                            if (ActiveCTF.GameState == CTF.CTFGameState.Started) {
                                if (exPlayer.Get(client).InCTF) {
                                    if (exPlayer.Get(client).CTFSide != CTF.Teams.Red) {
                                        if (ActiveCTF.RedFlags > 0) {
                                            if (ActiveCTF.RedFlagHolder == null) {
                                                if (exPlayer.Get(client).CTFState == CTF.PlayerState.Free) {
                                                    exPlayer.Get(client).CTFState = CTF.PlayerState.HoldingFlag;
                                                    ActiveCTF.RedFlagHolder = client;
                                                    ActiveCTF.RedFlags--;
                                                    ActiveCTF.CTFMsg(client.Player.Name + " has stolen a red flag!", Text.Yellow);
                                                }
                                            } else {
                                                Messenger.PlayerMsg(client, ActiveCTF.RedFlagHolder.Player.Name + " is already holding a flag!", Text.Yellow);
                                            }
                                        } else {
                                            Messenger.PlayerMsg(client, "The red team has no more flags!", Text.Yellow);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case 30: {//CTF Blue Flag Tile
                            if (ActiveCTF.GameState == CTF.CTFGameState.Started) {
                                if (exPlayer.Get(client).InCTF && exPlayer.Get(client).CTFSide != CTF.Teams.Blue) {
                                    if (ActiveCTF.BlueFlags > 0) {
                                        if (ActiveCTF.BlueFlagHolder == null) {
                                            if (exPlayer.Get(client).CTFState == CTF.PlayerState.Free) {
                                                exPlayer.Get(client).CTFState = CTF.PlayerState.HoldingFlag;
                                                ActiveCTF.BlueFlagHolder = client;
                                                ActiveCTF.BlueFlags--;
                                                ActiveCTF.CTFMsg(client.Player.Name + " has stolen a blue flag!", Text.Yellow);
                                            }
                                        } else {
                                            Messenger.PlayerMsg(client, ActiveCTF.BlueFlagHolder.Player.Name + " is already holding a flag!", Text.Yellow);
                                        }
                                    } else {
                                        Messenger.PlayerMsg(client, "The blue team has no more flags!", Text.Yellow);
                                    }
                                }
                            }
                        }
                        break;
                    case 31: {//CTF Red Flag Check
                            if (ActiveCTF.GameState == CTF.CTFGameState.Started) {
                                ActiveCTF.CheckFlag(client, CTF.Teams.Blue);
                            }
                        }
                        break;
                    case 32: {//CTF Blue Flag Check
                            if (ActiveCTF.GameState == CTF.CTFGameState.Started) {
                                ActiveCTF.CheckFlag(client, CTF.Teams.Red);
                            }
                        }
                        break;
                    case 33: {//R Dungeon Goal for secret rooms; doesn't do anything here
                            client.Player.WarpToRDungeon(param1.ToInt() - 1, param2.ToInt() - 1);
                            //if (client.Player.Map.MapType == Enums.MapType.RDungeonMap && ((RDungeonMap)client.Player.Map).RDungeonIndex > -1) {
                            //	client.Player.WarpToDungeon(((RDungeonMap)client.Player.Map).RDungeonIndex, ((RDungeonMap)client.Player.Map).RDungeonFloor + 1);
                            //}
                        }
                        break;
                    case 34: {//Reveals stairs when stepped on
                            //if (client.Player.JobList.HasCompletedMission("dsksanasd984r487") == false) {
                            //    client.Player.JobList.AddJob("dsksanasd984r487");
                            //} else {
                            //    Messenger.PlayerMsg(client, "There are no special missions that you can play. Come back later!", Text.BrightRed);
                            //}
                            if (character.CharacterType != Enums.CharacterType.MapNpc) {
                                RevealTrap(map, character.X, character.Y, hitlist);
                                map.Tile[character.X, character.Y].Data1 = 35;
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg("Stairs appeared!", Text.BrightRed), character.X, character.Y, 10);
                            }
                        }
                        break;
                    case 35: {//R Dungeon secret room
                    		if (client != null && map.MapType == Enums.MapType.RDungeonMap && ((RDungeonMap)map).RDungeonIndex > -1) {

                                InstancedMap iMap = null;

                                PartyManager.AttemptPartyWarp(client, (Client warpClient) =>
                                {
                                    if (iMap == null)
                                    {
                                        iMap = new InstancedMap(MapManager.GenerateMapID("i"));
                                        IMap baseMap = MapManager.RetrieveMap(param1.ToInt());
                                        MapCloner.CloneMapTiles(baseMap, iMap);
                                        MapCloner.CloneMapNpcs(baseMap, iMap);
                                        MapCloner.CloneMapGeneralProperties(baseMap, iMap);
                                        iMap.MapBase = param1.ToInt();
                                        iMap.SpawnItems();
                                        for (int x = 0; x < iMap.MaxX; x++)
                                        {
                                            for (int y = 0; y < iMap.MaxY; y++)
                                            {
                                                if (iMap.Tile[x, y].Type == Enums.TileType.Scripted && iMap.Tile[x, y].Data1 == 33)
                                                {
                                                    //iMap.Tile[x, y].Data1 = 36;
                                                    iMap.Tile[x, y].String1 = (((RDungeonMap)map).RDungeonIndex + 1).ToString();
                                                    iMap.Tile[x, y].String2 = (((RDungeonMap)map).RDungeonFloor + 2).ToString();
                                                }
                                                else if (iMap.Tile[x, y].Type == Enums.TileType.Item)
                                                {
                                                    iMap.Tile[x, y].Type = Enums.TileType.Walkable;
                                                }
                                            }
                                        }
                                    }

                                    Messenger.PlayerWarp(warpClient, iMap, param2.ToInt(), param3.ToInt());
                                });

                            }
                        }
                        break;
                    case 36: {// Dungeon Entrance (Random)
                            if (client != null) {
                                //RDungeonManager.LoadRDungeon(param1.ToInt() - 1);
                                Story story = DungeonRules.CreatePreDungeonStory(client, param1, param2, param3, true);
                                StoryManager.PlayStory(client, story);

                                //if (param3 == "") {
                                //	Messenger.AskQuestion(client, "EnterRDungeon:" + (param1.ToInt() - 1) + ":" + (param2.ToInt() - 1) + ":" + param3, "Will you go on?", -1);
                                //} else if (param3.IsNumeric()) {
                                //	if (param3.ToInt() > 0) {
                                //normal dungeon, has dungeon entry
                                //		Messenger.AskQuestion(client, "EnterRDungeon:" + (param1.ToInt() - 1) + ":" + (param2.ToInt() - 1) + ":" + param3, "Will you enter " + DungeonManager.Dungeons[param3.ToInt() - 1].Name + "?", -1);
                                //	}
                                //} else {
                                //Event dungeon; does not have an official dungeon entry
                                //	Messenger.AskQuestion(client, "EnterRDungeon:" + (param1.ToInt() - 1) + ":" + (param2.ToInt() - 1) + ":" + param3, "Will you go enter " + param3 + "?", -1);
                                //}
                            }
                        }
                        break;
                    case 37: { // Dungeon Entrance (mapped)
                            if (client != null) {

                                Story story = DungeonRules.CreatePreDungeonStory(client, param1, param2, param3, false);
                                StoryManager.PlayStory(client, story);
                            }
                        }
                        break;
                    case 38: {// Housing Center
                            if (client != null) {
                                exPlayer.Get(client).HousingCenterMap = client.Player.MapID;
                                exPlayer.Get(client).HousingCenterX = client.Player.X;
                                exPlayer.Get(client).HousingCenterY = client.Player.Y;

                                Messenger.OpenVisitHouseMenu(client);
                            }
                        }
                        break;
                    case 39: { // Pitfall trap
                            if (WillTrapActivate(character, map, character.X, character.Y)) {
                                RevealTrap(map, character.X, character.Y, hitlist);
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " stepped on a Pitfall Trap!", Text.BrightRed), character.X, character.Y, 10);
                                ActivateTrap(map, character.X, character.Y, script, hitlist);
                            }
                        }
                        break;
                    case 40: { // Evolution block tile
                            if (client != null) {
                                if (exPlayer.Get(client).EvolutionActive == false) {
                                    BlockPlayer(client);
                                    //    Messenger.PlayerMsg(client, "You can't enter this room!", Text.BrightRed);
                                }
                            }
                        }
                        break;
                    case 41: { // Boss battle
                            if (client != null) {
                                //BossBattles.StartBossBattle(client, param1);
                            }
                        }
                        break;
                    case 42: { // Seal Trap
                            if (WillTrapActivate(character, map, character.X, character.Y)) {
                                RevealTrap(map, character.X, character.Y, hitlist);
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " stepped on a Seal Trap!", Text.BrightRed), character.X, character.Y, 10);
                                ActivateTrap(map, character.X, character.Y, script, hitlist);
                            }
                        }
                        break;
                    case 43: { // Slow Trap
                            if (WillTrapActivate(character, map, character.X, character.Y)) {
                                RevealTrap(map, character.X, character.Y, hitlist);
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " stepped on a Slow Trap!", Text.BrightRed), character.X, character.Y, 10);
                                ActivateTrap(map, character.X, character.Y, script, hitlist);
                            }
                        }
                        break;
                    case 44: { // Spin Trap
                            if (WillTrapActivate(character, map, character.X, character.Y)) {
                                RevealTrap(map, character.X, character.Y, hitlist);
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " stepped on a Spin Trap!", Text.BrightRed), character.X, character.Y, 10);
                                ActivateTrap(map, character.X, character.Y, script, hitlist);
                            }
                        }
                        break;
                    case 45: { // DungeonAttempted
                            if (client != null) {
                                int dungeonIndex = param1.ToInt();
                                int warpMap = param2.ToInt();
                                int warpX = param3.Split(':')[0].ToInt();
                                int warpY = param3.Split(':')[1].ToInt();
                                //Messenger.AskQuestion(client, "EnterDungeon:" + param1, "Will you enter " + DungeonManager.Dungeons[param1.ToInt()].Name + "?", -1);
                                client.Player.AddDungeonAttempt(dungeonIndex - 1);
                                Messenger.PlayerWarp(client, warpMap, warpX, warpY);
                            }
                        }
                        break;
                    case 46: {//Dungeon completion count incrementer
                            if (client != null) {

                                PartyManager.AttemptPartyWarp(client, (Client warpClient) =>
                                {
                                    int dungeonIndex = param1.ToInt();
                                    int warpMap = param2.ToInt();
                                    int warpX = param3.Split(':')[0].ToInt();
                                    int warpY = param3.Split(':')[1].ToInt();

                                    if (dungeonIndex > 0)
                                    {
                                        warpClient.Player.IncrementDungeonCompletionCount(dungeonIndex - 1, 1);
                                        PostDungeonCompletion(warpClient, dungeonIndex);
                                    }
                                    exPlayer.Get(warpClient).FirstMapLoaded = false;
                                    DungeonRules.ExitDungeon(warpClient, warpMap, warpX, warpY);
                                });

                            }
                        }
                        break;
                    case 47: {//Dungeon exit without increment
                            
                        }
                        break;
                    case 48: { // Removes snowballs
                            if (client != null) {
                                if (client.Player.HasItem(152) > 0) {
                                    client.Player.TakeItem(152, 1);
                                }
                            }
                        }
                        break;
                    case 49: { // Sweet scent trap (summon)
                            if (WillTrapActivate(character, map, character.X, character.Y)) {
                                RevealTrap(map, character.X, character.Y, hitlist);
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " stepped on a Summon Trap!", Text.BrightRed), character.X, character.Y, 10);
                                ActivateTrap(map, character.X, character.Y, script, hitlist);
                                RemoveTrap(map, character.X, character.Y, hitlist);
                            }
                        }
                        break;
                    case 50: { // GRUUUUUDGE Trap (Grudge)
                            if (WillTrapActivate(character, map, character.X, character.Y)) {
                                RevealTrap(map, character.X, character.Y, hitlist);
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " stepped on a Grudge Trap!", Text.BrightRed), character.X, character.Y, 10);
                                ActivateTrap(map, character.X, character.Y, script, hitlist);
                                RemoveTrap(map, character.X, character.Y, hitlist);
                            }
                        }
                        break;
                    case 51: { // SelfDestruct Trap
                            if (WillTrapActivate(character, map, character.X, character.Y)) {
                                RevealTrap(map, character.X, character.Y, hitlist);
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " stepped on a Selfdestruct Trap!", Text.BrightRed), character.X, character.Y, 10);
                                ActivateTrap(map, character.X, character.Y, script, hitlist);
                            }
                        }
                        break;
                    case 52: { // Sleep Trap /slumber
                            if (WillTrapActivate(character, map, character.X, character.Y)) {
                                RevealTrap(map, character.X, character.Y, hitlist);
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " stepped on a Slumber Trap!", Text.BrightRed), character.X, character.Y, 10);
                                ActivateTrap(map, character.X, character.Y, script, hitlist);
                            }
                        }
                        break;
                    case 53: { // Fan Trap /Gust
                            if (WillTrapActivate(character, map, character.X, character.Y)) {
                                RevealTrap(map, character.X, character.Y, hitlist);
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " stepped on a Gust Trap!", Text.BrightRed), character.X, character.Y, 10);
                                ActivateTrap(map, character.X, character.Y, script, hitlist);
                            }
                        }
                        break;
                    case 54: { // Arena
                            if (client != null) {
                                bool canEnter = true;
                                for (int i = 1; i <= client.Player.MaxInv; i++) {
                                    if (client.Player.Inventory[i].Num > 0
                                        && ItemManager.Items[client.Player.Inventory[i].Num].Type != Enums.ItemType.Held && ItemManager.Items[client.Player.Inventory[i].Num].Type != Enums.ItemType.HeldByParty
                                        && ItemManager.Items[client.Player.Inventory[i].Num].Type != Enums.ItemType.HeldInBag) {
                                        bool held = false;

                                        for (int j = 0; j < Constants.MAX_ACTIVETEAM; j++) {
                                            if (client.Player.Team[j] != null
                                                && client.Player.Team[j].HeldItemSlot == i) {
                                                held = true;
                                            }

                                        }

                                        if (!held) canEnter = false;
                                    }
                                }

                                if (!canEnter) {
                                    Story story = new Story();
                                    StorySegment segment = new StorySegment();
                                    segment.Action = Enums.StoryAction.Say;
                                    segment.AddParameter("Text", "Notice:  You can only enter the arena with held-effect, team-effect, or bag-effect items.  Any other item must be held by a team member.");
                                    segment.AddParameter("Mugshot", "-1");
                                    segment.Parameters.Add("Speed", "0");
                                    segment.Parameters.Add("PauseLocation", "0");
                                    story.Segments.Add(segment);
                                    
                                    segment = new StorySegment();
									segment.Action = Enums.StoryAction.AskQuestion;
									segment.AddParameter("Question", "All items that do not fit arena restrictions will be sent to storage.  Is that OK?");
									segment.AddParameter("SegmentOnYes", (story.Segments.Count + 2).ToString());
									segment.AddParameter("SegmentOnNo", (story.Segments.Count + 3).ToString());
									segment.AddParameter("Mugshot", "-1");
									story.Segments.Add(segment);
									
									segment = new StorySegment();
						            segment.Action = Enums.StoryAction.RunScript;
						            segment.AddParameter("ScriptIndex", "61");
						            segment.AddParameter("ScriptParam1", param1);
						            segment.AddParameter("ScriptParam2", param2);
						            segment.AddParameter("ScriptParam3", param3);
						            segment.AddParameter("Pause", "1");
						            story.Segments.Add(segment);

                                    StoryManager.PlayStory(client, story);

                                } else {
                                    EnterArena(client, character, map, param2, param3, hitlist);
                                }
                            }
                        }
                        break;
                    case 55: { // Staff Elevator
                            if (client != null) {
                                if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                    // TODO: OpenStaffElevator(client);
                                }
                            }
                        }
                        break;
                    case 56: { // Pitch-Black Abyss warps
                            if (client != null) {
                            	//if DungeonMap.Length < 1
                            		//GenerateMap
                            	/*Enums.Direction dir;
            					if (param1.ToInt() == 1) {
            						dir = Enums.Direction.Up;
            					} else if (param1.ToInt() == 2) {
            						dir = Enums.Direction.Down;
            					} else if (param1.ToInt() == 3) {
            						dir = Enums.Direction.Left;
            					} else { //param1.ToInt() = 4
            						dir = Enums.Direction.Right;
           						}*/
           						//switch to use dir when done rewriting GetDungeonRoom
           						if (exPlayer.Get(client).DungeonGenerated == false) {
                    				PitchBlackAbyss.GenerateMap(client);
                    			}
                                PitchBlackAbyss.GetDungeonRoom(client, param1.ToInt()); 
                            }
                        }
                        break;
                    case 57: { // Pitch-Black Abyss entrance
                            if (client != null) {
                                if (client.Player.MapID == MapManager.GenerateMapID(1545)) { //easy PBA
                                	PitchBlackAbyss.InitializeMap(client, PitchBlackAbyss.Difficulty.Easy);
                                	PitchBlackAbyss.GenerateMap(client);
                                	Story story = DungeonRules.CreatePreDungeonStory(client, "1546", "9:9", "22", false);
                                	StoryManager.PlayStory(client, story);
                                }
                            }
                        }
                        break;
                    case 58: { // Electrostasis Tower Electrolock tile
                            if (client != null) {
                                ElectrostasisTower.SteppedOnElectrolock(client, param1.ToInt());
                            }
                        }
                        break;
                    case 59: { // Electrostasis Tower Sublevel Setter
                            if (client != null) {
                                string[] splitData = param2.Split(':');
                                ElectrostasisTower.SetSublevelCheckpoint(client, param1.ToInt(), splitData[0], splitData[1].ToInt(), splitData[2].ToInt());
                            }
                        }
                        break;
                    case 60: { // Warp to tournament hub
                            if (client != null) {
                                if (client.Player.Tournament != null) {
                                    client.Player.Tournament.WarpToHub(client);
                                }
                            }
                        }
                        break;
                    case 61: { // Warp to tournament combat map
                            if (client != null) {
                                if (client.Player.Tournament != null && client.Player.TournamentMatchUp != null) {
                                    client.Player.TournamentMatchUp.WarpToCombatMap(client);
                                }
                            }
                        }
                        break;
                    case 62: { // Open tournament spectator selection list
                            if (client != null) {
                                if (client.Player.Tournament == null) {
                                    Messenger.SendTournamentSpectateListingTo(client, null);
                                } else {
                                    client.Player.Tournament.WarpToHub(client);
                                }
                            }
                        }
                        break;
                    case 63: { // Leave tournament waiting room
                            if (client != null) {
                                if (client.Player.TournamentMatchUp == null) { // Prevent leaving if the player is in a match-up
                                    if (client.Player.Tournament != null) {
                                        client.Player.Tournament.RemoveRegisteredPlayer(client);
                                    }
                                    Messenger.PlayerWarp(client, 1192, 10, 10);
                                }
                            }
                        }
                        break;
                    case 64: {
                            if (exPlayer.Get(client).StoryEnabled) {
                                int[] rangeStart;
                                int[] rangeEnd;
                                string[] data1 = param1.Split('!');
                                string[] data2 = param2.Split(':');
                                rangeStart = new int[data1.Length];
                                rangeEnd = new int[data1.Length];
                                for (int i = 0; i < data1.Length; i++) {
                                    string[] vals = data1[i].Split(':');
                                    rangeStart[i] = vals[0].ToInt();
                                    rangeEnd[i] = vals[1].ToInt();
                                }
                                int currentStorySection = client.Player.StoryHelper.ReadSetting("[MainStory]-CurrentSection").ToInt();
                                for (int i = 0; i < rangeStart.Length; i++) {
                                    if (currentStorySection >= rangeStart[i] && currentStorySection <= rangeEnd[i]) {
                                        BlockPlayer(client);
                                        Messenger.PlayerMsg(client, data2[i], Text.BrightRed);
                                        break;
                                    }
                                }
                            }
                        }
                        break;
                    case 65: {//dungeon completion block
                            if (client != null) {
                                if (client.Player.GetDungeonCompletionCount(param1.ToInt()-1) < 1) {
                                    BlockPlayer(client);
                                    Messenger.PlayerMsg(client, "You must complete " + DungeonManager.Dungeons[param1.ToInt() - 1].Name + " to get through!", Text.BrightRed);
                                }
                            }
                        }
                        break;
                    case 66: {// Delite Plaza exit warp
                    		if (!string.IsNullOrEmpty(exPlayer.Get(client).PlazaEntranceMap)) {
                    			Messenger.PlayerWarp(client, exPlayer.Get(client).PlazaEntranceMap, exPlayer.Get(client).PlazaEntranceX, exPlayer.Get(client).PlazaEntranceY);
                    		
                    			exPlayer.Get(client).PlazaEntranceMap = null;
                    			exPlayer.Get(client).PlazaEntranceX = 0;
                    			exPlayer.Get(client).PlazaEntranceY = 0;
                    		} else {
                    			Messenger.PlayerWarp(client, 737, 6, 41);
                    		}
                    	}
                    	break;
                    case 67: {// Auction master + bid winner only
                    		if (client.Player.CharID != Auction.AuctionMaster && client.Player.CharID != Auction.LastAuctionMaster && client.Player.Name != Auction.HighestBidder
                    			&& Ranks.IsDisallowed(client, Enums.Rank.Moniter)) {
                    			BlockPlayer(client);
                    		}
                    	}
                    	break;
                    case 68: {// key-blocked next floor of different dungeon
                    	if (client != null) {
                    			int slot = 0;
	                    		for (int i = 1; i <= client.Player.Inventory.Count; i++) {
					                if (client.Player.Inventory[i].Num == param2.ToInt() && !client.Player.Inventory[i].Sticky) {
					                    slot = i;
					                    break;
					                }
					            }
	                            if (slot > 0) {
	                                Messenger.AskQuestion(client, "UseItem:"+param2, "Will you use your " + ItemManager.Items[param2.ToInt()].Name + " on this tile?", -1);
	                            } else {
	                    			Messenger.PlaySoundToMap(client.Player.MapID, "magic132.wav");
	                    			//Messenger.PlayerMsg(client, Server.RDungeons.RDungeonManager.RDungeons[param1.ToInt()-1].DungeonName, Text.Pink);
	                            	Messenger.PlayerMsg(client, "There is a peculiar marking on the floor... It seems to need a key.", Text.BrightRed);
	                            }
	                        }
                    	}
                    	break;
                    case 69: {// next floor of different dungeon
                    	if (client != null) {

                            PartyManager.AttemptPartyWarp(client, (Client warpClient) =>
                            {
                                warpClient.Player.WarpToRDungeon(param1.ToInt() - 1, ((RDungeonMap)warpClient.Player.Map).RDungeonFloor + 1);
                            });

                            }
                    	}
                    	break;
                    case 70: { // Shocker Trap
                            if (WillTrapActivate(character, map, character.X, character.Y)) {
                                RevealTrap(map, character.X, character.Y, hitlist);
                                hitlist.AddPacketToMap(map, PacketBuilder.CreateBattleMsg(character.Name + " stepped on a Shocker Trap!", Text.BrightRed), character.X, character.Y, 10);
                                ActivateTrap(map, character.X, character.Y, script, hitlist);
                            }
                        }
                        break;
                    case 71: {// fossil revival
                    	if (client != null) {
                    			if (Server.Globals.ServerTime != Enums.Time.Night) {
                    				Story story = new Story();
		                            StoryBuilderSegment segment = StoryBuilder.BuildStory();
		                            StoryBuilder.AppendSaySegment(segment, "The light of " + Server.Globals.ServerTime.ToString().ToLower() + " is seeping down from above.", -1, 0, 0);
		                            
		                            segment.AppendToStory(story);
		                            StoryManager.PlayStory(client, story);
                    			} else {
                    				int slot = 0;
                    				int itemNum = -1;
		                    		for (int i = 1; i <= client.Player.Inventory.Count; i++) {
						                if (client.Player.Inventory[i].Num >= 791 && client.Player.Inventory[i].Num <= 799
						                	&& !client.Player.Inventory[i].Sticky) {
						                    slot = i;
						                    itemNum = client.Player.Inventory[i].Num;
						                    break;
						                }
						            }
		                            if (slot > 0) {
		                                Messenger.AskQuestion(client, "UseItem:"+itemNum, "Red moonlight is pouring down from above... Will you hold up your " + ItemManager.Items[itemNum].Name + " to the light?", -1);
		                                Messenger.PlaySoundToMap(client.Player.MapID, "magic848.wav");
		                            } else {
		                            
	                    				Story story = new Story();
			                            StoryBuilderSegment segment = StoryBuilder.BuildStory();
			                            StoryBuilder.AppendSaySegment(segment, "Red moonlight is pouring down from above...", -1, 0, 0);
			                            
			                            segment.AppendToStory(story);
			                            StoryManager.PlayStory(client, story);
			                            
	                    				Messenger.PlaySoundToMap(client.Player.MapID, "magic848.wav");
                    				}
                    			}
	                        }
                    	}
                    	break;
                    case 72: { // Warp to hard mode entrance
                            if (client != null) {
                                if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                	Messenger.PlayerWarp(client, param1.ToInt(), param2.ToInt(), param3.ToInt());
                                }
                            }
                        }
                        break;
                    case 73: { // Warp from hard mode entrance
                            if (client != null) {
                                if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                                	Messenger.PlayerWarp(client, param1.ToInt(), param2.ToInt(), param3.ToInt());
                                }
                            }
                        }
                        break;
                    case 74: { // Tanren Arena
                            if (client != null) {
                                bool canEnter = true;
                                for (int i = 1; i <= client.Player.MaxInv; i++) {
                                    if (client.Player.Inventory[i].Num > 0
                                        && ItemManager.Items[client.Player.Inventory[i].Num].Type != Enums.ItemType.Held && ItemManager.Items[client.Player.Inventory[i].Num].Type != Enums.ItemType.HeldByParty
                                        && ItemManager.Items[client.Player.Inventory[i].Num].Type != Enums.ItemType.HeldInBag) {
                                        bool held = false;

                                        for (int j = 0; j < Constants.MAX_ACTIVETEAM; j++) {
                                            if (client.Player.Team[j] != null
                                                && client.Player.Team[j].HeldItemSlot == i) {
                                                held = true;
                                            }

                                        }

                                        if (!held) canEnter = false;
                                    }
                                }

                                if (!canEnter) {
                                    Story story = new Story();
                                    StorySegment segment = new StorySegment();
                                    segment.Action = Enums.StoryAction.Say;
                                    segment.AddParameter("Text", "Notice:  You can only enter the arena with held-effect, team-effect, or bag-effect items.  Any other item must be held by a team member.");
                                    segment.AddParameter("Mugshot", "-1");
                                    segment.Parameters.Add("Speed", "0");
                                    segment.Parameters.Add("PauseLocation", "0");
                                    story.Segments.Add(segment);
                                    
                                    segment = new StorySegment();
									segment.Action = Enums.StoryAction.AskQuestion;
									segment.AddParameter("Question", "All items that do not fit arena restrictions will be sent to storage.  Is that OK?");
									segment.AddParameter("SegmentOnYes", (story.Segments.Count + 2).ToString());
									segment.AddParameter("SegmentOnNo", (story.Segments.Count + 3).ToString());
									segment.AddParameter("Mugshot", "-1");
									story.Segments.Add(segment);
									
									segment = new StorySegment();
						            segment.Action = Enums.StoryAction.RunScript;
						            segment.AddParameter("ScriptIndex", "61");
						            segment.AddParameter("ScriptParam1", param1);
						            segment.AddParameter("ScriptParam2", param2);
						            segment.AddParameter("ScriptParam3", param3);
						            segment.AddParameter("Pause", "1");
						            story.Segments.Add(segment);

                                    StoryManager.PlayStory(client, story);

                                } else {
                                    client.Player.BeginTempStatMode(50, true);
                                    EnterArena(client, character, map, param2, param3, hitlist);
                                }
                            }
                        }
                        break;
                	case 75: {
    						for (int i = 1; i <= client.Player.MaxInv; i++) {
                				if (client.Player.Inventory[i].Num > -1) {
                					BlockPlayer(client);
                					Messenger.PlayerMsg(client, "You cannot have any items in your inventory!", Text.Red);
                					break;
                				}
                			}
                			
                			for (int j = 1; j < Constants.MAX_ACTIVETEAM; j++) {
                            	if (client.Player.Team[j] != null) {
                                	BlockPlayer(client);
                					Messenger.PlayerMsg(client, "You cannot have any team members in your team!", Text.Red);
                					break;
                            	}

                            }
 	               		}
                		break;
                	case 76: {//Dive
                            if (client != null) {
                                Messenger.PlayerMsg(client, "The water goes pretty deep here...", Text.Grey);
                                //Messenger.PlaySound(client, "Magic477.wav");
                                hitlist.AddPacket(client, PacketBuilder.CreateSoundPacket("Magic477.wav"));
                            }
                        }
                        break;
                }
                PacketHitList.MethodEnded(ref hitlist);
            } catch (Exception ex) {
                Messenger.AdminMsg("Error: ScriptedTile", Text.Black);
                Messenger.AdminMsg(script + ", " + param1 + " " + param2 + " " + param3, Text.Black);
                Messenger.AdminMsg(map.Name, Text.Black);
                Messenger.AdminMsg(ex.ToString(), Text.Black);
            }
        }

        public static string ScriptedTileInfo(Client client, int scriptNum) {
            switch (scriptNum) {
                case 0:
                    return scriptNum + ": Empty";
                case 1:
                    return scriptNum + ": Main Story Launcher";
                case 2:
                    return scriptNum + ": Explosion Trap";
                case 3:
                    return scriptNum + ": Chestnut Trap";
                case 4:
                    return scriptNum + ": PP-Zero Trap";
                case 5:
                    return scriptNum + ": Grimy Trap";
                case 6:
                    return scriptNum + ": Toxic Trap";
                case 7:
                    return scriptNum + ": Random Trap";
                case 8:
                    return scriptNum + ": Appraisal";
                case 9:
                    return scriptNum + ": Lickilicky";
                case 10:
                    return scriptNum + ": HP/MP Healing Tile";
                case 11:
                    return scriptNum + ": Destiny Cavern Warpout";
                case 12:
                    return scriptNum + ": RDungeon Branch";
                case 13:
                    return scriptNum + ": drop from the sky";
                case 14:
                    return scriptNum + ": Fly";
                case 15:
                    return scriptNum + ": Warp Trap";
                case 16:
                    return scriptNum + ": Pokemon Trap";
                case 17:
                    return scriptNum + ": Spikes";
                case 18:
                    return scriptNum + ": T-Spikes";
                case 19:
                    return scriptNum + ": Stealth Rock";
                case 20:
                    return scriptNum + ": VOID";
                case 21:
                    return scriptNum + ": End Level X Dungeon";
                case 22:
                    return scriptNum + ": Anti-Suicide";
                case 23:
                    return scriptNum + ": Sticky Trap";
                case 24:
                    return scriptNum + ": Admin Only Block";
                case 25:
                    return scriptNum + ": Mud Trap";
                case 26:
                    return scriptNum + ": Wonder Tile";
                case 27:
                    return scriptNum + ": HT Switch";
                case 28:
                    return scriptNum + ": Trip Trap";
                case 29:
                    return scriptNum + ": Red Flag (CTF) [Not Done]";
                case 30:
                    return scriptNum + ": Blue Flag (CTF) [Not Done]";
                case 31:
                    return scriptNum + ": Flag Check (CTF) [Red Side] [Not Done]";
                case 32:
                    return scriptNum + ": Flag Check (CTF) [Blue Side] [Not Done]";
                case 33:
                    return scriptNum + ": Scripted RDungeon Goal";
                case 34:
                    return scriptNum + ": Mask2 Covered Secret Room";
                case 35:
                    return scriptNum + ": R Dungeon Secret Room";
                case 36:
                    return scriptNum + ": Dungeon Entrance (Random)";
                case 37:
                    return scriptNum + ": Dungeon Entrance (Mapped)";
                case 38:
                    return scriptNum + ": Housing Center";
                case 39:
                    return scriptNum + ": Pitfall Trap";
                case 40:
                    return scriptNum + ": Evolution Block";
                case 41:
                    return scriptNum + ": Boss Battle";
                case 42:
                    return scriptNum + ": Seal Trap";
                case 43:
                    return scriptNum + ": Slow Trap";
                case 44:
                    return scriptNum + ": Spin Trap";
                case 45:
                    return scriptNum + ": Dungeon Attempt";
                case 46:
                    return scriptNum + ": Dungeon Complete";
                case 47:
                    return scriptNum + ": Dungeon Exit";
                case 48:
                    return scriptNum + ": Removes Snowballs";
                case 49:
                    return scriptNum + ": Summon (aka Sweet Scent) Trap";
                case 50:
                    return scriptNum + ": Grudge Trap";
                case 51:
                    return scriptNum + ": SelfDestruct Trap";
                case 52:
                    return scriptNum + ": Slumber Trap";
                case 53:
                    return scriptNum + ": Gust Trap";
                case 54:
                    return scriptNum + ": The Arena Entrance";
                case 55:
                    return scriptNum + ": Staff Elevator";
                case 56:
                    return scriptNum + ": Pitch-Black Abyss warps";
                case 57:
                    return scriptNum + ": Pitch-Black Abyss Entrance";
                case 58:
                    return scriptNum + ": EST Electrolock";
                case 59:
                    return scriptNum + ": EST Sublevel Checkpoint";
                case 60:
                    return scriptNum + ": Warp To Tourny Hub";
                case 61:
                    return scriptNum + ": Warp to Tourny Combat";
                case 62:
                    return scriptNum + ": Open Tourny Spectator List";
                case 63:
                    return scriptNum + ": Leave Tourny Waiting Room";
                case 64:
                    return scriptNum + ": Story Block Tile";
                case 65:
                    return scriptNum + ": Completion Count check";
                case 66:
                    return scriptNum + ": Delite Plaza Exit";
                case 67: 
                	return scriptNum + ": Auction Block";
                case 70: 
                	return scriptNum + ": Shocker Trap";
                case 71: 
                	return scriptNum + ": Fossil Revival";
                case 72: 
                	return scriptNum + ": To Hard Entrance";
                case 73: 
                	return scriptNum + ": From Hard Entrance";
                case 74: 
                	return scriptNum + ": Tanren Arena Entrance";
                case 75:
                	return scriptNum + ": Dodgeball Block";
                case 76:
                	return scriptNum + ": Dive";
                default:
                    return scriptNum.ToString() + ": Unknown";
            }
        }
	}
}
using Server;
using Server.Scripting;
using System;
using System.Drawing;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Server.Network;
using Server.Maps;
using Server.Players;

using Server.Events.Player.TriggerEvents;

namespace Script {
    public class ElectrostasisTower {
        public static bool IsInTower(IMap map) {
            if (map.MapType == Server.Enums.MapType.Standard) {
                Map sMap = map as Map;
                if (sMap.MapNum >= 1601 && sMap.MapNum <= 1618) {
                    return true;
                } else {
                    return false;
                }
            } else {
                return false;
            }
        }

        public static void SaveExPlayer(exPlayer player, XmlWriter writer) {
            writer.WriteStartElement("ElectrostasisTower");

            writer.WriteElementString("ElectrolockCharge", player.ElectrolockCharge.ToString());
            writer.WriteElementString("ElectrolockLevel", player.ElectrolockLevel.ToString());

            writer.WriteStartElement("ElectrolockSublevels");
            for (int i = 0; i < player.ElectrolockSublevel.Count; i++) {
                writer.WriteElementString("ID", player.ElectrolockSublevel[i].ToString());
            }
            writer.WriteEndElement();

            writer.WriteStartElement("ElectrolockSublevelTriggersActive");
            for (int i = 0; i < player.ElectrolockSublevelTriggersActive.Count; i++) {
                writer.WriteElementString("ID", player.ElectrolockSublevelTriggersActive[i].ToString());
            }
            writer.WriteEndElement();

            writer.WriteStartElement("ElectrolockSublevelTriggersActive");
            for (int i = 0; i < player.ElectrolockSublevelTriggersActive.Count; i++) {
                writer.WriteElementString("ID", player.ElectrolockSublevelTriggersActive[i].ToString());
            }
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        public static void LoadExPlayer(exPlayer player, XmlReader reader) {
            player.ElectrolockSublevel = new List<int>();

            player.ElectrolockSublevelTriggersActive = new List<string>();
            using (reader) {
                while (reader.Read()) {
                    if (reader.IsStartElement()) {
                        switch (reader.Name) {
                            case "ElectrolockCharge": {

                                    player.ElectrolockCharge = reader.ReadString().ToInt();
                                }
                                break;
                            case "ElectrolockLevel": {
                                    player.ElectrolockLevel = reader.ReadString().ToInt();
                                }
                                break;
                            case "ElectrolockSublevels": {
                                    player.ElectrolockSublevel.Clear();
                                    using (XmlReader subReader = reader.ReadSubtree()) {
                                        while (subReader.Read()) {
                                            if (subReader.IsStartElement()) {
                                                switch (subReader.Name) {
                                                    case "ID": {
                                                            player.ElectrolockSublevel.Add(subReader.ReadString().ToInt());
                                                        }
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                }
                                break;

                            case "ElectrolockSublevelTriggersActive": {
                                    player.ElectrolockSublevelTriggersActive.Clear();
                                    using (XmlReader subReader = reader.ReadSubtree()) {
                                        while (subReader.Read()) {
                                            if (subReader.IsStartElement()) {
                                                switch (subReader.Name) {
                                                    case "ID": {
                                                            player.ElectrolockSublevelTriggersActive.Add(subReader.ReadString());
                                                        }
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                }
                                break;

                        }
                    }
                }
            }
        }

        public static void OnMapLoaded(Client client, IMap map, PacketHitList packetList) {
            PacketHitList.MethodStart(ref packetList);
            if (IsInTower(map)) {
                // Prevent others from entering until finished. Spoiler prevention!

                if (Ranks.IsDisallowed(client, Enums.Rank.Scripter)) {
                    packetList.HitList.Clear(); // Remove the warp packets to cancel the map load
                    Messenger.PlayerWarp(client, 1015, 25, 25);
                    if (Ranks.IsAllowed(client, Enums.Rank.Moniter)) {
                        Messenger.PlayerMsg(client, "Sorry, you can't go here! ~Pikachu", Text.BrightRed);
                    } else {
                        Messenger.PlayerMsg(client, "Sorry, you can't go here!", Text.BrightRed);

                    }

                } else {
                    if (VerifyMapKeyTiles(client, map, packetList)) {

                        packetList.AddPacket(client, PacketBuilder.CreateBattleMsg("An electrolock has been opened nearby!", Text.BrightGreen));
                    }
                    CheckForSublevelGoal(client, packetList);
                    VerifyMapSublevelTriggerTiles(client, map, packetList);
                }
            } else {
                ResetPlayer(client);
            }
            PacketHitList.MethodEnded(ref packetList);
        }

        public static void ResetPlayer(Client client) {
            exPlayer.Get(client).ElectrolockCharge = 0;
            exPlayer.Get(client).ElectrolockLevel = 0;
            exPlayer.Get(client).ElectrolockSublevel.Clear();

            exPlayer.Get(client).ElectrolockSublevelTriggersActive.Clear();
            for (int i = client.Player.TriggerEvents.Count - 1; i >= 0; i--) {
                if (client.Player.TriggerEvents[i].ID.StartsWith("ESTSublevelGoal-")) {
                    client.Player.RemoveTriggerEvent(client.Player.TriggerEvents[i]);
                }

            }
            while (client.Player.HasItem(385) > 0) {
                client.Player.TakeItem(385, 1);
            }
        }

        public static void OnPlayerStep(Client client) {
            if (IsInTower(client.Player.Map)) {
                if (exPlayer.Get(client).ElectrolockCharge == -1) {
                    exPlayer.Get(client).ElectrolockCharge = 0;
                    Messenger.BattleMsg(client, "Your shock flask seems empty...", Text.BrightRed);
                    VerifyMapKeyTiles(client, client.Player.Map, null);
                }
                if (client.Player.HasItemHeldBy(385, Enums.PokemonType.Electric)) {
                    exPlayer.Get(client).ElectrolockCharge++;
                    int chargesNeeded = ChargesNeededForLevel(exPlayer.Get(client).ElectrolockLevel);
                    if (chargesNeeded > -1) {
                        if (exPlayer.Get(client).ElectrolockCharge == 1) {
                            Messenger.BattleMsg(client, "Your shock flask is charging!", Text.BrightGreen);
                        } else if (exPlayer.Get(client).ElectrolockCharge == chargesNeeded / 2) {
                            Messenger.BattleMsg(client, "Your shock flask is half-charged!", Text.BrightGreen);
                        } else if (exPlayer.Get(client).ElectrolockCharge == chargesNeeded - 10) {
                            Messenger.BattleMsg(client, "Your shock flask is almost charged!", Text.BrightGreen);
                        } else if (exPlayer.Get(client).ElectrolockCharge == chargesNeeded) {
                            Messenger.BattleMsg(client, "Your shock flask is fully charged!", Text.BrightGreen);
                            if (VerifyMapKeyTiles(client, client.Player.Map, null)) {
                                Messenger.BattleMsg(client, "An electrolock has been opened nearby!", Text.BrightGreen);
                            }
                        }
                    }
                }
            }
        }

        public static int ChargesNeededForLevel(int level) {
            switch (level) {
                case 0:
                    return 30;

                case 1:
                    return 200;
                case 2:
                    return 300;
                case 3:
                    return 200;

                default:
                    return -1;
            }
        }

        public static int SublevelsNeededForLevel(int level) {
            switch (level) {
                case 0:

                case 1:
                case 2:
                    return 0;
                case 3:
                    return 2;
                default:
                    return -1;
            }
        }

        #region Electrolock

        public static bool VerifyMapKeyTiles(Client client, IMap map, PacketHitList packetList) {
            PacketHitList.MethodStart(ref packetList);
            bool unlocked = false;

            for (int x = 0; x <= map.MaxX; x++) {
                for (int y = 0; y <= map.MaxY; y++) {
                    bool state = VerifyKeyTile(client, map, x, y, packetList);
                    if (state) {
                        unlocked = true;
                    }
                }
            }
            PacketHitList.MethodEnded(ref packetList);

            return unlocked;
        }

        public static bool VerifyKeyTile(Client client, IMap map, int x, int y, PacketHitList packetList) {
            PacketHitList.MethodStart(ref packetList);
            bool unlocked = false;

            Tile mapTile = map.Tile[x, y];
            if (mapTile.Type == Server.Enums.TileType.Scripted) {
                if (mapTile.Data1 == 58) {
                    int chargeLevel = mapTile.String1.ToInt();
                    exPlayer exPlayer = exPlayer.Get(client);

                    if ((exPlayer.ElectrolockLevel == chargeLevel && exPlayer.ElectrolockCharge >= ChargesNeededForLevel(chargeLevel) && exPlayer.ElectrolockSublevel.Count >= SublevelsNeededForLevel(chargeLevel))
                        || exPlayer.ElectrolockLevel > chargeLevel) {


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

        public static void SteppedOnElectrolock(Client client, int chargeLevelNeeded) {
            if ((exPlayer.Get(client).ElectrolockLevel == chargeLevelNeeded && exPlayer.Get(client).ElectrolockCharge >= ChargesNeededForLevel(chargeLevelNeeded) &&
                exPlayer.Get(client).ElectrolockSublevel.Count >= SublevelsNeededForLevel(chargeLevelNeeded))
                || (exPlayer.Get(client).ElectrolockLevel > chargeLevelNeeded)) {

                if (exPlayer.Get(client).ElectrolockLevel == chargeLevelNeeded) {
                    exPlayer.Get(client).ElectrolockLevel++;
                    exPlayer.Get(client).ElectrolockSublevel.Clear();
                    exPlayer.Get(client).ElectrolockCharge = -1;

                    for (int i = client.Player.TriggerEvents.Count - 1; i >= 0; i--) {
                        if (client.Player.TriggerEvents[i].ID.StartsWith("ESTSublevelGoal-")) {
                            client.Player.RemoveTriggerEvent(client.Player.TriggerEvents[i]);
                        }
                    }

                    for (int i = client.Player.TriggerEvents.Count - 1; i >= 0; i--) {
                        if (client.Player.TriggerEvents[i].ID.StartsWith("ESTSublevelGoal-")) {
                            client.Player.RemoveTriggerEvent(client.Player.TriggerEvents[i]);
                        }
                    }
                }
            } else {
                Main.BlockPlayer(client);
            }
        }

        #endregion

        #region Sublevel

        public static void SetSublevelCheckpoint(Client client, int id, string mapID, int x, int y) {
            SteppedOnTileTriggerEvent trigger = new SteppedOnTileTriggerEvent("ESTSublevelGoal-" + id, TriggerEventAction.RunScript, 1, true, client, mapID, x, y);
            client.Player.AddTriggerEvent(trigger);
            CheckForSublevelGoal(client, null);
        }


        public static void CheckForSublevelGoal(Client client, PacketHitList packetList) {
            for (int i = 0; i < client.Player.TriggerEvents.Count; i++) {

                if (client.Player.TriggerEvents[i].Trigger == TriggerEventTrigger.SteppedOnTile &&
                    client.Player.TriggerEvents[i].ID.StartsWith("ESTSublevelGoal")) {

                    SteppedOnTileTriggerEvent tEvent = client.Player.TriggerEvents[i] as SteppedOnTileTriggerEvent;

                    if (client.Player.MapID == tEvent.MapID) {
                        DisplayVisibleSublevelGoalTile(client, client.Player.Map, tEvent.X, tEvent.Y, packetList);

                    }
                }
            }
        }

        public static void DisplayVisibleSublevelGoalTile(Client client, IMap map, int x, int y, PacketHitList packetList) {
            PacketHitList.MethodStart(ref packetList);
            Tile tile = new Tile(new DataManager.Maps.Tile());
            MapCloner.CloneTile(map, x, y, tile);
            tile.Mask = 3505;
            tile.MaskSet = 7;
            tile.Type = Enums.TileType.Walkable;
            Messenger.SendTemporaryTileTo(packetList, client, x, y, tile);

            MapCloner.CloneTile(map, x, y - 1, tile);
            tile.Fringe = 3491;
            tile.FringeSet = 7;
            tile.Type = Enums.TileType.Walkable;
            Messenger.SendTemporaryTileTo(packetList, client, x, y - 1, tile);

            PacketHitList.MethodEnded(ref packetList);
        }

        public static void DisplayInvisibleSublevelGoalTile(Client client, IMap map, int x, int y, PacketHitList packetList) {
            PacketHitList.MethodStart(ref packetList);
            Tile tile = new Tile(new DataManager.Maps.Tile());
            MapCloner.CloneTile(map, x, y, tile);
            tile.Mask = 0;
            tile.MaskSet = 7;
            tile.Type = Enums.TileType.Walkable;
            Messenger.SendTemporaryTileTo(packetList, client, x, y, tile);

            MapCloner.CloneTile(map, x, y - 1, tile);
            tile.Fringe = 0;
            tile.FringeSet = 7;
            tile.Type = Enums.TileType.Walkable;
            Messenger.SendTemporaryTileTo(packetList, client, x, y - 1, tile);

            PacketHitList.MethodEnded(ref packetList);
        }

        public static void ReachedSublevelGoal(Client client, SteppedOnTileTriggerEvent tEvent) {
            DisplayInvisibleSublevelGoalTile(client, client.Player.Map, tEvent.X, tEvent.Y, null);
            string[] split = tEvent.ID.Split('-');
            if (split[1].IsNumeric()) {
                int id = split[1].ToInt();
                if (exPlayer.Get(client).ElectrolockSublevel.Contains(id) == false) {
                    exPlayer.Get(client).ElectrolockSublevel.Add(id);
                    Messenger.BattleMsg(client, "The light of the crystal enters into your shock flask!", Text.BrightGreen);
                    Messenger.BattleMsg(client, "Your shock flask feels heavier...", Text.WhiteSmoke);
                }
                if (VerifyMapKeyTiles(client, client.Player.Map, null)) {
                    Messenger.BattleMsg(client, "An electrolock has been opened nearby!", Text.BrightGreen);
                }

            }
        }

        #endregion


        public static void VerifyMapSublevelTriggerTiles(Client client, IMap map, PacketHitList packetList) {
            PacketHitList.MethodStart(ref packetList);
            bool unlocked = false;

            for (int x = 0; x <= map.MaxX; x++) {
                for (int y = 0; y <= map.MaxY; y++) {
                    VerifySublevelTriggerTile(client, map, x, y, packetList);
                }
            }
            PacketHitList.MethodEnded(ref packetList);
        }

        public static void VerifySublevelTriggerTile(Client client, IMap map, int x, int y, PacketHitList packetList) {
            PacketHitList.MethodStart(ref packetList);

            Tile mapTile = map.Tile[x, y];
            if (mapTile.Type == Server.Enums.TileType.Scripted) {
                if (mapTile.Data1 == 59) {
                    string triggerID = mapTile.String1;
                    exPlayer exPlayer = exPlayer.Get(client);
                    if (exPlayer.ElectrolockSublevelTriggersActive.Contains(triggerID) == false) {
                        DisplayVisibleSublevelTriggerTile(client, map, x, y, packetList);
                    } else {
                        DisplayInvisibleSublevelTriggerTile(client, map, x, y, packetList);
                    }
                }
            }
            PacketHitList.MethodEnded(ref packetList);
        }

        public static void DisplayVisibleSublevelTriggerTile(Client client, IMap map, int x, int y, PacketHitList packetList) {
            Tile tile = new Tile(new DataManager.Maps.Tile());
            MapCloner.CloneTile(map, x, y, tile);
            tile.Mask = 196;
            tile.MaskSet = 10;
            tile.Anim = 197;
            tile.AnimSet = 10;
            tile.Type = Enums.TileType.Walkable;

            Messenger.SendTemporaryTileTo(packetList, client, x, y, tile);
        }

        public static void DisplayInvisibleSublevelTriggerTile(Client client, IMap map, int x, int y, PacketHitList packetList) {
            Tile tile = new Tile(new DataManager.Maps.Tile());
            MapCloner.CloneTile(map, x, y, tile);
            tile.Mask = 0;
            tile.MaskSet = 10;
            tile.Anim = 0;
            tile.AnimSet = 10;
            tile.Type = Enums.TileType.Walkable;

            Messenger.SendTemporaryTileTo(packetList, client, x, y, tile);
        }
        
        public static void EnterRDungeon(Client client, int dungeonNum, int floor) {
        	Messenger.PlayerMsg(client, "You've entered EST!", Text.BrightRed);
        }

    }
}
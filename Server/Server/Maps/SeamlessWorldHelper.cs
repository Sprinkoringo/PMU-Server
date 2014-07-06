using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server.Network;
using Server.Maps;
using Server.Scripting;
using Server.Stories;

namespace Server.Maps
{
    class SeamlessWorldHelper
    {
        public static void SwitchSeamlessMaps(Client client, int mapNum, int x, int y) {
            SwitchSeamlessMaps(client, MapManager.RetrieveMap(mapNum), x, y);
        }

        public static void SwitchSeamlessMaps(Client client, IMap map, int x, int y) {
            if (x > map.MaxX)
                x = map.MaxX;
            if (y > map.MaxY)
                y = map.MaxY;

            ScriptManager.InvokeSub("OnMapLoad", client, client.Player.Map, map, client.Player.LoggedIn);

            IMap oldMap = client.Player.Map;
            Messenger.SendLeaveMap(client, oldMap, false, false);

            client.Player.MapID = map.MapID;
            client.Player.X = x;
            client.Player.Y = y;

            // Sets it so we know to process npcs on the map
            map.PlayersOnMap.Add(client.Player.CharID);
            MapManager.AddActiveMap(map);
            MapManager.LoadBorderingMaps(map);

            //client.Player.GettingMap = true;
            Messenger.SendDataTo(client, PMU.Sockets.TcpPacket.CreatePacket("seamlessmapchange"));
            Messenger.SendCheckForMap(client);

            //SendInventory(client);// removed
            //SendWornEquipment(client);

            if (map.Tile[x, y].Type == Enums.TileType.Story) {
                StoryManager.PlayStory(client, map.Tile[x, y].Data1);
            }
        }

        public static bool IsInSight(IMap map, int x, int y, IMap targetMap, Enums.MapID targetMapID, int targetX, int targetY) {
            return IsInSight(map, x, y, targetMap, targetMapID, targetX, targetY,  10, 10, 8, 7);
        }

        public static bool IsInSight(IMap map, int x, int y, IMap targetMap, Enums.MapID targetMapID, int targetX, int targetY, int leftDistance, int rightDistance, int topDistance, int bottomDistance) {
            int leftX = x - leftDistance;
            int rightX = x + rightDistance;

            int topY = y - topDistance;
            int bottomY = y + bottomDistance;

            if (map.MapID == targetMap.MapID || !IsMapSeamless(map, map) || !IsMapSeamless(targetMap, targetMap)) {
                return (targetX >= leftX && targetX <= rightX &&
                    targetY >= topY && targetY <= bottomY);
            }

            int newX = targetX;
            int newY = targetY;
            switch (targetMapID) {
                case Enums.MapID.Right: {
                        newX += map.MaxX + 1;
                    }
                    break;
                case Enums.MapID.Left: {
                        newX -= targetMap.MaxX + 1;
                    }
                    break;
                case Enums.MapID.Down: {
                        newY += map.MaxY + 1;
                    }
                    break;
                case Enums.MapID.Up: {
                        newY -= targetMap.MaxY + 1;
                    }
                    break;
                case Enums.MapID.TopLeft: {
                        newY -= targetMap.MaxY + 1;
                        newX -= targetMap.MaxX + 1;
                    }
                    break;
                case Enums.MapID.TopRight: {
                        newY -= targetMap.MaxY + 1;
                        newX += map.MaxX + 1;
                    }
                    break;
                case Enums.MapID.BottomLeft: {
                        newY += map.MaxY + 1;
                        newX -= targetMap.MaxX + 1;
                    }
                    break;
                case Enums.MapID.BottomRight: {
                        newY += map.MaxY + 1;
                        newX += map.MaxX + 1;
                    }
                    break;
            }

            return (newX >= leftX && newX <= rightX &&
                    newY >= topY && newY <= bottomY);
        }

        public static Enums.MapID GetBorderingMapID(IMap map, IMap targetMap) {
            for (int i = 1; i < 9; i++) {
                string borderingMapID = MapManager.RetrieveBorderingMapID(map, (Enums.MapID)i);
                //IMap borderingMap = MapManager.RetrieveBorderingMap(map, (Enums.MapID)i, true);
                if (borderingMapID == targetMap.MapID) {
                    return (Enums.MapID)i;
                }
            }
            return Enums.MapID.Active;
        }

        public static bool IsMapSeamless(IMap map, IMap borderingMap) {
            if (borderingMap.MapType == Enums.MapType.Standard) {
                Map borderingMapStandard = (Map)borderingMap;
                if (!borderingMapStandard.Instanced) {
                    return true;
                }
            }
            return false;
        }

        public static bool IsMapSeamless(IMap map, Enums.MapID direction) {
            // return false;

            if (map.MapType == Enums.MapType.Standard) {
                string borderingMapID = MapManager.RetrieveBorderingMapID(map, direction);
                if (!string.IsNullOrEmpty(borderingMapID)) {
                    IMap borderingMap = MapManager.RetrieveActiveMap(borderingMapID);
                    if (borderingMap != null) {
                        return IsMapSeamless(map, borderingMap);
                    }
                }
            }

            return false;
        }

    }
}

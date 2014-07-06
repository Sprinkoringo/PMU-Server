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


using System;
using System.Collections.Generic;
using System.Text;
using Server.Players;
using Server.Maps;
using Server.Network;
using Server.Items;

namespace Server
{
    class CommandHandler
    {
        Forms.MainUI mainUI;

        public CommandHandler(Forms.MainUI mainUI) {
            this.mainUI = mainUI;
        }

        public void ProcessCommand(int activeLine, string command) {
            ThreadManager.StartOnThreadParams(new System.Threading.WaitCallback(ProcessCommandCallback), new ParamObject(activeLine, command));
        }

        public void ProcessCommandCallback(object obj) {
            int activeLine = (int)((ParamObject)obj).Param[0];
            string command = (string)((ParamObject)obj).Param[1];
            Command fullCommand = Server.CommandProcessor.ParseCommand(command);
            string fullArgs = CommandProcessor.JoinArgs(fullCommand.CommandArgs.ToArray());
            switch (fullCommand.CommandArgs[0].ToLower()) {
                case "/help": {
                        DisplayHelp();
                    }
                    break;
                case "/clear": {
                        mainUI.ClearCommands();
                    }
                    break;
                case "/global": {
                        Messenger.GlobalMsg(fullArgs, Text.White);
                        mainUI.AddCommandLine("Global: " + fullArgs);
                    }
                    break;
                case "/masskick": {
                        foreach (Client i in ClientManager.GetClients()) {
                            if (i.IsPlaying() && Ranks.IsDisallowed(i, Enums.Rank.Moniter)) {
                                Messenger.GlobalMsg(i.Player.Name + " has been kicked by the server!", Text.White);
                                Messenger.AlertMsg(i, "You have been kicked by the server!");
                            }
                        }
                        mainUI.AddCommandLine("Everyone has been kicked.");
                    }
                    break;
                case "/dumpstats": {
                        Statistics.PacketStatistics.DumpStatistics();
                        mainUI.AddCommandLine("Packet statistics dumped to database.");
                    }
                    break;
                case "/clearstats": {
                        Statistics.PacketStatistics.ClearStatistics();
                        mainUI.AddCommandLine("Packet statistics cleared."); 
                    }
                    break;
                case "/masswarp": {
                        if (fullCommand.CommandArgs.Count == 4) {
                            int map = fullCommand.CommandArgs[1].ToInt(-1);
                            int x = fullCommand.CommandArgs[2].ToInt(-1);
                            int y = fullCommand.CommandArgs[3].ToInt(-1);
                            if (map <= 0) {
                                mainUI.AddCommandLine("Invalid Map.");
                                break;
                            } else if (x == -1) {
                                mainUI.AddCommandLine("Invalid X coordinate.");
                                break;
                            } else if (y == -1) {
                                mainUI.AddCommandLine("Invalid Y coordinate.");
                                break;
                            }
                            // TODO: Mass Warp
                            //if (x > MapManager.Maps[map].MaxX) {
                            //    mainUI.AddCommandLine("Invalid X coordinate.");
                            //    break;
                            //}
                            //if (y > MapManager.Maps[map].MaxY) {
                            //    mainUI.AddCommandLine("Invalid Y coordinate.");
                            //    break;
                            //}
                            foreach (Client i in ClientManager.GetClients()) {
                                if (i.IsPlaying() && Ranks.IsDisallowed(i, Enums.Rank.Moniter)) {
                                    Messenger.GlobalMsg("The server has warped everyone!", Text.White);
                                    Messenger.PlayerWarp(i, map, x, y);
                                }
                            }
                            mainUI.AddCommandLine("Everyone has been warped.");
                        } else {
                            mainUI.AddCommandLine("Invalid arguments.");
                        }
                    }
                    break;
                case "/kick": {
                        if (fullCommand.CommandArgs.Count == 2) {
                            Client client = ClientManager.FindClient(fullCommand.CommandArgs[1]);
                            if (client == null) {
                                mainUI.AddCommandLine("Player is offline.");
                            } else {
                                Messenger.GlobalMsg(client.Player.Name + " has been kicked by the server!", Text.White);
                                Messenger.AlertMsg(client, "You have been kicked by the server!");
                                mainUI.AddCommandLine(client.Player.Name + " has been kicked!");
                            }
                        } else {
                            mainUI.AddCommandLine("Invalid arguments.");
                        }
                    }
                    break;
                case "/warp": {
                        if (fullCommand.CommandArgs.Count == 5) {
                            Client client = ClientManager.FindClient(fullCommand.CommandArgs[1]);
                            if (client == null) {
                                mainUI.AddCommandLine("Player is offline.");
                            } else {
                                int mapNum = fullCommand.CommandArgs[2].ToInt(-1);
                                int x = fullCommand.CommandArgs[3].ToInt(-1);
                                int y = fullCommand.CommandArgs[4].ToInt(-1);
                                if (mapNum <= 0) {
                                    mainUI.AddCommandLine("Invalid Map.");
                                    break;
                                } else if (x == -1) {
                                    mainUI.AddCommandLine("Invalid X coordinate.");
                                    break;
                                } else if (y == -1) {
                                    mainUI.AddCommandLine("Invalid Y coordinate.");
                                    break;
                                }
                                IMap map;
                                if (MapManager.IsMapActive(MapManager.GenerateMapID(mapNum))) {
                                    map = MapManager.RetrieveActiveMap(MapManager.GenerateMapID(mapNum));
                                } else {
                                    using (Database.DatabaseConnection dbConnection = new Database.DatabaseConnection(Database.DatabaseID.Data)) {
                                        map = MapManager.LoadStandardMap(dbConnection, MapManager.GenerateMapID(mapNum));
                                    }
                                }
                                if (x > map.MaxX) {
                                    mainUI.AddCommandLine("Invalid X coordinate.");
                                    break;
                                }
                                if (y > map.MaxY) {
                                    mainUI.AddCommandLine("Invalid Y coordinate.");
                                    break;
                                }
                                Messenger.PlayerMsg(client, "You have been warped by the server!", Text.White);
                                Messenger.PlayerWarp(client, mapNum, x, y);
                                mainUI.AddCommandLine(client.Player.Name + " has been warped.");
                            }
                        } else {
                            mainUI.AddCommandLine("Invalid arguments.");
                        }
                    }
                    break;
                case "/mapmsg": {
                        if (fullCommand.CommandArgs.Count == 3) {
                            string map = fullCommand.CommandArgs[1];
                            // Check if the map is active
                            if (!MapManager.IsMapActive(map)) {
                                mainUI.AddCommandLine("Invalid Map.");
                                break;
                            }
                            Messenger.MapMsg(map, fullCommand.CommandArgs[2], Text.DarkGrey);
                            mainUI.AddCommandLine("Map Msg (Map " + map.ToString() + "): " + fullCommand.CommandArgs[2]);
                        } else {
                            mainUI.AddCommandLine("Invalid arguments.");
                        }
                    }
                    break;
                case "/reloadscripts": {
                        Scripting.ScriptManager.Reload();
                        mainUI.AddCommandLine("Scripts reloaded.");
                    }
                    break;
                case "/players": {
                        string players = "";
                        int count = 0;
                        foreach (Client i in ClientManager.GetClients()) {
                            if (i.IsPlaying()) {
                                count++;
                                players += i.Player.Name + "\r\n";
                            }
                        }
                        mainUI.AddCommandLine("Players online: \r\n" + players);
                        mainUI.AddCommandLine("There are " + count.ToString() + " players online");
                    }
                    break;
                case "/test": {
                        //Email.Email.SendEmail("test");
                        //mainUI.AddCommandLine("Mail sent!");
                        //mainUI.AddCommandLine("There are currently no benchmarking tests");
                        //System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
                        //watch.Start();
                        //MapGeneralInfo genInfo = MapManager.RetrieveMapGeneralInfo(10);
                        //watch.Stop();
                        //mainUI.AddCommandLine("Elapsed, New: " + watch.Elapsed.ToString());
                        //watch.Reset();
                        //watch.Start();
                        //Map map = MapManager.LoadMap(10);
                        //watch.Stop();
                        //mainUI.AddCommandLine("Elapsed, Old: " + watch.Elapsed.ToString());
                        //mainUI.AddCommandLine("Name: " + genInfo.Name);
                    }
                    break;
                case "/finditem": {
                        int itemsFound = 0;
                        for (int i = 0; i < Server.Items.ItemManager.Items.MaxItems; i++) {
                            if (ItemManager.Items[i].Name.ToLower().StartsWith(fullCommand.CommandArgs[1].ToLower())) {
                                mainUI.AddCommandLine(ItemManager.Items[i].Name + "'s number is " + i.ToString());
                                itemsFound++;
                                //return;
                            }
                        }
                        if (itemsFound == 0) {
                            mainUI.AddCommandLine("Unable to find an item that starts with '" + fullCommand.CommandArgs[1] + "'");
                        }
                    }
                    break;
                case "/finditemc": {
                        int itemsFound = 0;
                        for (int i = 0; i < Server.Items.ItemManager.Items.MaxItems; i++) {
                            if (ItemManager.Items[i].Name.ToLower().Contains(fullCommand.CommandArgs[1].ToLower())) {
                                mainUI.AddCommandLine(ItemManager.Items[i].Name + "'s number is " + i.ToString());
                                itemsFound++;
                                //return;
                            }
                        }
                        if (itemsFound == 0) {
                            mainUI.AddCommandLine("Unable to find an item that starts with '" + fullCommand.CommandArgs[1] + "'");
                        }
                    }
                    break;
                case "/calcwm": {
                        mainUI.AddCommandLine("Factorial: " + Server.Math.CalculateFactorial(fullCommand.CommandArgs[1].ToInt()).ToString("R"));
                    }
                    break;
                case "/gmmode": {
                        Globals.GMOnly = !Globals.GMOnly;
                        mainUI.AddCommandLine("GM Only Mode Active: " + Globals.GMOnly);
                    }
                    break;
                default: {
                    Scripting.ScriptManager.InvokeSub("ProcessServerCommand", mainUI, fullCommand, fullArgs);
                    }
                    break;
            }
            mainUI.CommandComplete(activeLine, command);
        }

        internal bool IsValidCommand(string command) {
            string[] args = CommandProcessor.SplitCommand(command);
            switch (args[0].ToLower()) {
                case "/help":
                case "/clear":
                case "/global":
                case "/masskick":
                case "/masswarp":
                case "/dumpstats":
                case "/clearstats":
                case "/kick":
                case "/warp":
                case "/mapmsg":
                case "/mute":
                case "/reloadscripts":
                case "/togglescripts":
                case "/players":
                case "/test":
                case "/finditem":
                case "/finditemc":
                case "/calcwm":
                case "/gmmode":
                    return true;
                default:
                    return Scripting.ScriptManager.InvokeFunction("IsValidServerCommand", args[0], command).ToBool();
            }
        }

        private void DisplayHelp() {
            mainUI.AddCommandLine("-=- Server Command Help -=-");
            mainUI.AddCommandLine("Available Commands:");
            mainUI.AddCommandLine("/help - Shows help");
            mainUI.AddCommandLine("/clear - Clears the command prompt");
            mainUI.AddCommandLine("/global [string]msg - Sends a global message");
            mainUI.AddCommandLine("/masskick - Kicks all players, excluding staff");
            mainUI.AddCommandLine("/masswarp [int]map [int]x [int]y - Warps all players to the specified location");
            mainUI.AddCommandLine("/kick [string]playername - Kicks the player specified");
            mainUI.AddCommandLine("/warp [string]playername [int]map [int]x [int]y - Warps the player to the specified location");
            mainUI.AddCommandLine("/mapmsg [int]mapnum [string]msg - Sends a message to the map");
            mainUI.AddCommandLine("/reloadscripts - Reloads the scripts");
            mainUI.AddCommandLine("/togglescripts [bool]value - Turns the scripts on/off based on the value");
            mainUI.AddCommandLine("/players - Gets a list of all online players");
            Scripting.ScriptManager.InvokeSub("DisplayServerCommandHelp", mainUI);
        }


    }
}

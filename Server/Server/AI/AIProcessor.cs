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
using Server.Maps;
using Server.Npcs;
using Server.Players;
using Server.Moves;
using Server.Combat;
using Server.Network;
using PMU.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Server.AI
{
    class AIProcessor
    {
        public static readonly int MapTTL = 10000;
        public static MapGC mapGC;

        internal static void InitGameLoop() {
            ThreadManager.StartOnThreadParams(new System.Threading.WaitCallback(ProcessAI), Constants.GAME_SPEED);
        }

        internal static void ProcessAI(Object obj) {
            // Get the game speed
            int loopPause = (int)obj;
            if (loopPause == 0) {
                loopPause = 500;
            }
            Events.World.TimedEventManager.Initialize();
            Events.World.TimedEventManager.LoadBasicEvents(Core.GetTickCount());
            // The variables used in the loop
            TickCount tickCount = null;
            TickCount lastSpawn = Core.GetTickCount();
            TickCount lastConnectionCheck = Core.GetTickCount();
            Debug.InfiniteLoopDetector loopDetector = new Debug.InfiniteLoopDetector("AIProcessor");
            mapGC = new MapGC();
            System.Threading.Thread mapGCThread = new Thread(mapGC.Cleanup);
            // Start the map garbage collection thread
            mapGCThread.Start();

            Events.World.MapGCTimedEvent mapGCTimedEvent = new Events.World.MapGCTimedEvent("MapGC", mapGC);
            mapGCTimedEvent.SetInterval(Core.GetTickCount(), 1 * 60 * 1000);
            Events.World.TimedEventManager.TimedEvents.Add(mapGCTimedEvent);

            do {
                try {
                    if (tickCount != null) {
                        int timePassed = Core.GetTickCount().Tick - tickCount.Tick;

                        if (timePassed < loopPause) {
                            loopDetector.IncrementLoopCount();
                        }
                    }

                    tickCount = Core.GetTickCount();

                    // Copy all active map references to a local list to prevent modification during processing
                    IMap[] activeMaps = MapManager.ToArray();
                    // Now that we have a list of all active maps, lets process the AI on each one
                    //Parallel.ForEach(activeMaps, map =>
                    //{
                    foreach (IMap map in activeMaps) {
                        MapAIProcessingTask aiProcessingTask = new MapAIProcessingTask(map);
                        aiProcessingTask.ProcessAIThreadPoolCallback(null);
                        //ThreadPool.QueueUserWorkItem(new WaitCallback(aiProcessingTask.ProcessAIThreadPoolCallback));
                    }
                    //}
                    //);

                    Events.World.TimedEventManager.Process(tickCount);

                    // Pauses the loop for the time specified
                    System.Threading.Thread.Sleep(loopPause);
                } catch (Exception ex) {
                    Server.Exceptions.ErrorLogger.WriteToErrorLog(ex, "AIProcessor");
                }
            } while (true);
        }

        public static bool MoveNpcInDirection(Enums.Direction direction, IMap map, Client target, PacketHitList packetList, int mapNpcSlot) {
            switch (direction) {
                case Enums.Direction.Up: {
                        return MoveNpcUp(map, target, packetList, mapNpcSlot);
                    }
                case Enums.Direction.Down: {
                        return MoveNpcDown(map, target, packetList, mapNpcSlot);
                    }
                case Enums.Direction.Left: {
                        return MoveNpcLeft(map, target, packetList, mapNpcSlot);
                    }
                case Enums.Direction.Right: {
                        return MoveNpcRight(map, target, packetList, mapNpcSlot);
                    }
            }
            return false;
        }

        public static bool MoveNpcUp(IMap map, Client target, PacketHitList packetList, int mapNpcSlot) {
            // Up
            if (map.ActiveNpc[mapNpcSlot].Y > target.Player.Y) {
                if (MovementProcessor.CanNpcMove(map, mapNpcSlot, Enums.Direction.Up)) {
                    MovementProcessor.NpcMove(packetList, map, mapNpcSlot, map.ActiveNpc[mapNpcSlot].Direction, Enums.Speed.Walking);
                    return true;
                }
            }
            return false;
        }

        public static bool MoveNpcDown(IMap map, Client target, PacketHitList packetList, int mapNpcSlot) {
            // Down
            if (map.ActiveNpc[mapNpcSlot].Y < target.Player.Y) {
                if (MovementProcessor.CanNpcMove(map, mapNpcSlot, Enums.Direction.Down)) {
                    MovementProcessor.NpcMove(packetList, map, mapNpcSlot, map.ActiveNpc[mapNpcSlot].Direction, Enums.Speed.Walking);
                    return true;
                }
            }
            return false;
        }

        public static bool MoveNpcLeft(IMap map, Client target, PacketHitList packetList, int mapNpcSlot) {
            // Left
            if (map.ActiveNpc[mapNpcSlot].X > target.Player.X) {
                if (MovementProcessor.CanNpcMove(map, mapNpcSlot, Enums.Direction.Left)) {
                    MovementProcessor.NpcMove(packetList, map, mapNpcSlot, map.ActiveNpc[mapNpcSlot].Direction, Enums.Speed.Walking);
                    return true;
                }
            }
            return false;
        }

        public static bool MoveNpcRight(IMap map, Client target, PacketHitList packetList, int mapNpcSlot) {
            // Right
            if (map.ActiveNpc[mapNpcSlot].X < target.Player.X) {
                if (MovementProcessor.CanNpcMove(map, mapNpcSlot, Enums.Direction.Right)) {
                    MovementProcessor.NpcMove(packetList, map, mapNpcSlot, map.ActiveNpc[mapNpcSlot].Direction, Enums.Speed.Walking);
                    return true;
                }
            }
            return false;
        }






    }
}

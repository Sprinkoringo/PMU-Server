using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Server.Network;

namespace Server.Processing
{
    public class PlayerEventThread
    {
        Thread playerThread;
        bool quitFlag = false;
        ManualResetEvent resetEvent;
        PlayerEvent activeEvent;
        PlayerEventQueue eventQueue;
        Client ownerClient;

        public PlayerEventThread(Client ownerClient) {
            playerThread = new Thread(new ThreadStart(ProcessQueuedEvents));
            resetEvent = new ManualResetEvent(false);
            eventQueue = new PlayerEventQueue();
            this.ownerClient = ownerClient;

            playerThread.IsBackground = true;
            playerThread.Start();
        }

        public void HandleClientDisconnect() {
            quitFlag = true;
        }

        private void ProcessQueuedEvents() {
            while (!quitFlag) {
                if (eventQueue.Empty()) {
                    resetEvent.WaitOne();
                    resetEvent.Reset();
                }
                if (eventQueue.Empty() == false) {
                    activeEvent = eventQueue.Dequeue();
                    Network.MessageProcessor.ProcessData(ownerClient, activeEvent.Data);
                }
            }
        }

        public void AddEvent(PlayerEvent playerEvent) {
            eventQueue.Enqueue(playerEvent);
            resetEvent.Set();
        }
    }
}

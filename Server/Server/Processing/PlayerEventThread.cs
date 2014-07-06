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

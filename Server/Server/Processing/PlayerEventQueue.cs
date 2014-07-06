using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Processing
{
    class PlayerEventQueue
    {
        Queue<PlayerEvent> queue;

        public PlayerEventQueue() {
            queue = new Queue<PlayerEvent>();
        }

        public PlayerEvent Dequeue() {
            return queue.Dequeue();
        }

        public void Enqueue(PlayerEvent playerEvent) {
            queue.Enqueue(playerEvent);
        }

        public bool Empty() {
            return Count == 0;
        }

        public int Count {
            get { return queue.Count; }
        }
    }
}

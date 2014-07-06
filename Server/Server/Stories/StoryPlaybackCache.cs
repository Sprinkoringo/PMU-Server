using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Stories
{
    public class StoryPlaybackCache
    {
        List<Story> cache;

        public StoryPlaybackCache() {
            cache = new List<Story>();
        }

        public void Add(Story story) {
            cache.Add(story);
        }

        public int Count {
            get { return cache.Count; }
        }

        public Story this[int index] {
            get {
                return cache[index];
            }
        }

        public void RemoveAt(int index) {
            cache.RemoveAt(index);
        }
    }
}

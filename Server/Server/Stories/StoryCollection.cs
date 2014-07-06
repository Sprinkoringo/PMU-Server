using System;
using System.Collections.Generic;
using System.Text;
using PMU.Core;

namespace Server.Stories
{
    public class StoryCollection
    {
        #region Fields

        ListPair<int, Story> stories;
        int maxStories;

        #endregion Fields

        #region Constructors

        public StoryCollection(int maxStories) {
            if (maxStories == 0)
                maxStories = 50;
            this.maxStories = maxStories;
            stories = new ListPair<int, Story>();
        }

        #endregion Constructors

        #region Properties

        public ListPair<int, Story> Stories {
            get { return stories; }
        }

        public int MaxStories {
            get { return maxStories; }
        }

        #endregion Properties

        #region Indexers

        public Story this[int index] {
            get { return stories[index]; }
        }

        #endregion Indexers
    }
}

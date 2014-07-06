using System;
using System.Collections.Generic;
using System.Text;

namespace Server.DataConverter.Stories.V2
{
    public class Story
    {
        #region Fields

        public List<int> ExitAndContinue;

        #endregion Fields

        #region Constructors

        public Story() {
            ExitAndContinue = new List<int>();
        }

        #endregion Constructors

        #region Properties

        public int MaxSegments {
            get;
            set;
        }

        public string Name {
            get;
            set;
        }

        public int Revision {
            get;
            set;
        }

        public StorySegment[] Segments {
            get;
            set;
        }

        public int StoryStart {
            get;
            set;
        }

        public int Version {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public void UpdateStorySegments() {
            Segments = new StorySegment[MaxSegments + 1];
        }

        #endregion Methods
    }
}

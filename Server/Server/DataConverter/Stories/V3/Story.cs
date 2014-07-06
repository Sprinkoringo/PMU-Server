using System;
using System.Collections.Generic;
using System.Text;

namespace Server.DataConverter.Stories.V3
{
    public class Story
    {
        #region Fields

        public List<int> ExitAndContinue;
        List<StorySegment> segments;

        #endregion Fields

        #region Constructors

        public Story() {
            ExitAndContinue = new List<int>();
            segments = new List<StorySegment>();
        }

        #endregion Constructors

        #region Properties

        public string Name {
            get;
            set;
        }

        public int Revision {
            get;
            set;
        }

        public List<StorySegment> Segments {
            get { return segments; }
        }

        public int StoryStart {
            get;
            set;
        }

        #endregion Properties
    }
}

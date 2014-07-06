namespace Server.Stories
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Story
    {
        #region Fields

        public List<int> ExitAndContinue;
        List<StorySegment> segments;
        string id;

        #endregion Fields

        #region Constructors

        public Story(string id) {
            ExitAndContinue = new List<int>();
            segments = new List<StorySegment>();
            this.id = id;
        }

        public Story() {
            ExitAndContinue = new List<int>();
            segments = new List<StorySegment>();
            this.id = null;
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

        public string ID {
            get { return id; }
        }

        #endregion Properties
    }
}
namespace Server.Stories
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using PMU.Core;

    public class StorySegment
    {
        ListPair<string, string> parameters;

        public StorySegment() {
            parameters = new ListPair<string, string>();
        }

        #region Properties

        public Enums.StoryAction Action {
            get;
            set;
        }

        public ListPair<string, string> Parameters {
            get { return parameters; }
        }

        public void AddParameter(string paramID, string value) {
            parameters.Add(paramID, value);
        }

        #endregion Properties
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using PMU.Core;

namespace Server.DataConverter.Stories.V3
{
    public class StorySegment
    {
        ListPair<string, string> parameters;

        public StorySegment() {
            parameters = new ListPair<string, string>();
        }

        #region Properties

        public StoryAction Action {
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

        public enum StoryAction
        {
            Say = 0,
            Pause = 1,
            Padlock = 2,
            MapVisibility = 3,
            PlayMusic = 4,
            StopMusic = 5,
            ShowImage = 6,
            HideImage = 7,
            Warp = 8,
            PlayerPadlock = 9,
            ShowBackground = 10,
            HideBackground = 11,
            CreateFNPC = 12,
            MoveFNPC = 13,
            WarpFNPC = 14,
            ChangeFNPCDir = 15,
            DeleteFNPC = 16,
            RunScript = 17,
            HidePlayers = 18,
            ShowPlayers = 19,
            FNPCEmotion = 20,
            ChangeWeather = 21,
            HideNPCs = 22,
            ShowNPCs = 23,
            WaitForMap = 24,
            WaitForLoc = 25,
            AskQuestion = 26,
            GoToSegment = 27,
            ScrollCamera = 28,
            ResetCamera = 29
        }
    }
}

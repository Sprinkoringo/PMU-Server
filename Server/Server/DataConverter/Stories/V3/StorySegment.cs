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

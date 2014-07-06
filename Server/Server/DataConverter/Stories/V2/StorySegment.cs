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

namespace Server.DataConverter.Stories.V2
{
    public class StorySegment
    {
        #region Properties

        public StoryAction Action {
            get;
            set;
        }

        public string Data1 {
            get;
            set;
        }

        public string Data2 {
            get;
            set;
        }

        public string Data3 {
            get;
            set;
        }

        public string Data4 {
            get;
            set;
        }

        public string Data5 {
            get;
            set;
        }

        #endregion Properties

        public enum StoryAction
        {
            Say = 0,
            SaySlow = 1,
            Pause = 2,
            ShowMap = 3,
            HideMap = 4,
            Unlock = 5,
            Lock = 6,
            PlayMusic = 7,
            StopMusic = 8,
            ShowImage = 9,
            HideImage = 10,
            WarpToMap = 11,
            LockPlayer = 12,
            UnlockPlayer = 13,
            ShowBackground = 14,
            HideBackground = 15,
            CreateNPC = 16,
            MoveNPC = 17,
            MoveNPCQuick = 18,
            ChangeNPCDir = 19,
            DeleteNPC = 20,
            StoryScript = 21,
            PlayMovie = 22,
            HidePlayers = 23,
            ShowPlayers = 24,
            NPCEmotion = 25,
            Weather = 26,
            HideNPCs = 27,
            ShowNPCs = 28,
            WaitForMap = 29,
            WaitForLoc = 30,
            AskQuestion = 31,
            GoToSegment = 32,
            MovePlayer = 33,
            ChangePlayerDir = 34,
            PlaySoundEffect = 35,
            ScrollCamera = 36,
            ResetCamera = 37
        }
    }
}

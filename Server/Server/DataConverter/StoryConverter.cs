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

namespace Server.DataConverter
{
    public class StoryConverter
    {
        public static void ConvertV2ToV3(int num) {
            DataConverter.Stories.V3.Story storyV3 = new Stories.V3.Story();

            DataConverter.Stories.V2.Story storyV2 = DataConverter.Stories.V2.StoryManager.LoadStory(num);

            storyV3.Name = storyV2.Name;
            storyV3.Revision = storyV2.Revision + 1;
            storyV3.StoryStart = storyV2.StoryStart;

            for (int i = 0; i < storyV2.Segments.Length; i++) {
                storyV3.Segments.Add(ConvertSegment(storyV2.Segments[i]));
            }

            Stories.V3.StoryManager.SaveStory(storyV3, num);
        }

        public static Stories.V3.StorySegment ConvertSegment(Stories.V2.StorySegment segment) {
            Stories.V3.StorySegment newSegment = new Stories.V3.StorySegment();
            //bool paramsNeeded = true; ;
            switch (segment.Action) {
                case Stories.V2.StorySegment.StoryAction.SaySlow: {
                        newSegment.Action = Stories.V3.StorySegment.StoryAction.Say;

                        newSegment.Parameters.Add("Text", segment.Data1);
                        newSegment.Parameters.Add("Mugshot", segment.Data3);
                        newSegment.Parameters.Add("Speed", segment.Data2);
                        newSegment.Parameters.Add("PauseLocation", segment.Data4);
                    }
                    break;
                case Stories.V2.StorySegment.StoryAction.Say: {
                        newSegment.Action = Stories.V3.StorySegment.StoryAction.Say;

                        newSegment.AddParameter("Text", segment.Data1);
                        newSegment.AddParameter("Mugshot", segment.Data3);
                        newSegment.Parameters.Add("Speed", "0");
                        newSegment.Parameters.Add("PauseLocation", "0");
                    }
                    break;
                case Stories.V2.StorySegment.StoryAction.Pause: {
                        newSegment.Action = Stories.V3.StorySegment.StoryAction.Pause;

                        newSegment.AddParameter("Length", segment.Data1);
                    }
                    break;
                case Stories.V2.StorySegment.StoryAction.HideMap: {
                        newSegment.Action = Stories.V3.StorySegment.StoryAction.MapVisibility;

                        newSegment.Parameters.Add("Visible", "False");
                    }
                    break;
                case Stories.V2.StorySegment.StoryAction.ShowMap: {
                        newSegment.Action = Stories.V3.StorySegment.StoryAction.MapVisibility;

                        newSegment.Parameters.Add("Visible", "True");
                    }
                    break;
                case Stories.V2.StorySegment.StoryAction.Unlock: {
                        newSegment.Action = Stories.V3.StorySegment.StoryAction.Padlock;

                        newSegment.AddParameter("State", "Unlock");
                    }
                    break;
                case Stories.V2.StorySegment.StoryAction.Lock: {
                        newSegment.Action = Stories.V3.StorySegment.StoryAction.Padlock;

                        newSegment.AddParameter("State", "Lock");
                    }
                    break;
                case Stories.V2.StorySegment.StoryAction.PlayMusic: {
                        newSegment.Action = Stories.V3.StorySegment.StoryAction.PlayMusic;

                        newSegment.AddParameter("File", segment.Data1);
                        newSegment.AddParameter("HonorSettings", segment.Data2);
                        newSegment.AddParameter("Loop", segment.Data3);
                    }
                    break;
                case Stories.V2.StorySegment.StoryAction.StopMusic: {
                        newSegment.Action = Stories.V3.StorySegment.StoryAction.StopMusic;
                    }
                    break;
                case Stories.V2.StorySegment.StoryAction.ShowImage: {
                        newSegment.Action = Stories.V3.StorySegment.StoryAction.ShowImage;

                        newSegment.AddParameter("File", segment.Data1);
                        newSegment.AddParameter("ImageID", segment.Data4);
                        newSegment.AddParameter("X", segment.Data2);
                        newSegment.AddParameter("Y", segment.Data3);
                    }
                    break;
                case Stories.V2.StorySegment.StoryAction.HideImage: {
                        newSegment.Action = Stories.V3.StorySegment.StoryAction.HideImage;

                        newSegment.AddParameter("ImageID", segment.Data1);
                    }
                    break;
                case Stories.V2.StorySegment.StoryAction.WarpToMap: {
                        newSegment.Action = Stories.V3.StorySegment.StoryAction.Warp;

                        newSegment.AddParameter("MapID", "s" + segment.Data1);
                        newSegment.AddParameter("X", segment.Data2);
                        newSegment.AddParameter("Y", segment.Data3);
                    }
                    break;
                case Stories.V2.StorySegment.StoryAction.LockPlayer: {
                        newSegment.Action = Stories.V3.StorySegment.StoryAction.PlayerPadlock;

                        newSegment.AddParameter("MovementState", "Lock");
                    }
                    break;
                case Stories.V2.StorySegment.StoryAction.UnlockPlayer: {
                        newSegment.Action = Stories.V3.StorySegment.StoryAction.PlayerPadlock;

                        newSegment.AddParameter("MovementState", "Unlock");
                    }
                    break;
                case Stories.V2.StorySegment.StoryAction.ShowBackground: {
                        newSegment.Action = Stories.V3.StorySegment.StoryAction.ShowBackground;

                        newSegment.AddParameter("File", segment.Data1);
                    }
                    break;
                case Stories.V2.StorySegment.StoryAction.HideBackground: {
                        newSegment.Action = Stories.V3.StorySegment.StoryAction.HideBackground;
                    }
                    break;
                case Stories.V2.StorySegment.StoryAction.CreateNPC: {
                        newSegment.Action = Stories.V3.StorySegment.StoryAction.CreateFNPC;

                        newSegment.AddParameter("ID", segment.Data1);
                        newSegment.AddParameter("ParentMapID", segment.Data2);
                        newSegment.AddParameter("X", segment.Data3);
                        newSegment.AddParameter("Y", segment.Data4);
                        newSegment.AddParameter("Sprite", segment.Data5);
                    }
                    break;
                case Stories.V2.StorySegment.StoryAction.MoveNPC: {
                        newSegment.Action = Stories.V3.StorySegment.StoryAction.MoveFNPC;

                        newSegment.AddParameter("ID", segment.Data1);
                        newSegment.AddParameter("X", segment.Data2);
                        newSegment.AddParameter("Y", segment.Data3);
                        newSegment.AddParameter("Speed", segment.Data4);
                        newSegment.AddParameter("Pause", segment.Data5);
                    }
                    break;
                case Stories.V2.StorySegment.StoryAction.MoveNPCQuick: {
                        newSegment.Action = Stories.V3.StorySegment.StoryAction.WarpFNPC;

                        newSegment.AddParameter("ID", segment.Data1);
                        newSegment.AddParameter("X", segment.Data2);
                        newSegment.AddParameter("Y", segment.Data3);
                    }
                    break;
                case Stories.V2.StorySegment.StoryAction.ChangeNPCDir: {
                        newSegment.Action = Stories.V3.StorySegment.StoryAction.ChangeFNPCDir;

                        newSegment.AddParameter("ID", segment.Data1);
                        newSegment.AddParameter("Direction", segment.Data2);
                    }
                    break;
                case Stories.V2.StorySegment.StoryAction.DeleteNPC: {
                        newSegment.Action = Stories.V3.StorySegment.StoryAction.DeleteFNPC;

                        newSegment.AddParameter("ID", segment.Data1);
                    }
                    break;
                case Stories.V2.StorySegment.StoryAction.StoryScript: {
                        newSegment.Action = Stories.V3.StorySegment.StoryAction.RunScript;

                        newSegment.AddParameter("ScriptIndex", segment.Data1);
                        newSegment.AddParameter("Pause", segment.Data2);
                    }
                    break;
                case Stories.V2.StorySegment.StoryAction.HidePlayers: {
                        newSegment.Action = Stories.V3.StorySegment.StoryAction.HidePlayers;
                    }
                    break;
                case Stories.V2.StorySegment.StoryAction.ShowPlayers: {
                        newSegment.Action = Stories.V3.StorySegment.StoryAction.ShowPlayers;
                    }
                    break;
                case Stories.V2.StorySegment.StoryAction.NPCEmotion: {
                        newSegment.Action = Stories.V3.StorySegment.StoryAction.FNPCEmotion;

                        newSegment.AddParameter("ID", segment.Data1);
                        newSegment.AddParameter("Emotion", segment.Data2);
                    }
                    break;
                case Stories.V2.StorySegment.StoryAction.Weather: {
                        newSegment.Action = Stories.V3.StorySegment.StoryAction.ChangeWeather;

                        newSegment.AddParameter("Weather", segment.Data1);
                    }
                    break;
                case Stories.V2.StorySegment.StoryAction.HideNPCs: {
                        newSegment.Action = Stories.V3.StorySegment.StoryAction.HideNPCs;
                    }
                    break;
                case Stories.V2.StorySegment.StoryAction.ShowNPCs: {
                        newSegment.Action = Stories.V3.StorySegment.StoryAction.ShowNPCs;
                    }
                    break;
                case Stories.V2.StorySegment.StoryAction.WaitForMap: {
                        newSegment.Action = Stories.V3.StorySegment.StoryAction.WaitForMap;

                        newSegment.AddParameter("MapID", segment.Data1);
                    }
                    break;
                case Stories.V2.StorySegment.StoryAction.WaitForLoc: {
                        newSegment.Action = Stories.V3.StorySegment.StoryAction.WaitForLoc;

                        newSegment.AddParameter("MapID", segment.Data1);
                        newSegment.AddParameter("X", segment.Data2);
                        newSegment.AddParameter("Y", segment.Data3);
                    }
                    break;
                case Stories.V2.StorySegment.StoryAction.AskQuestion: {
                        newSegment.Action = Stories.V3.StorySegment.StoryAction.AskQuestion;

                        newSegment.AddParameter("Question", segment.Data1);
                        newSegment.AddParameter("SegmentOnYes", segment.Data2);
                        newSegment.AddParameter("SegmentOnNo", segment.Data3);
                        newSegment.AddParameter("Mugshot", segment.Data5); // I used Data5 instead of Data4 in the old system, for some reason...
                    }
                    break;
                case Stories.V2.StorySegment.StoryAction.GoToSegment: {
                        newSegment.Action = Stories.V3.StorySegment.StoryAction.GoToSegment;

                        newSegment.AddParameter("Segment", segment.Data1);
                    }
                    break;
                case Stories.V2.StorySegment.StoryAction.ScrollCamera: {
                        newSegment.Action = Stories.V3.StorySegment.StoryAction.ScrollCamera;

                        newSegment.AddParameter("X", segment.Data1);
                        newSegment.AddParameter("Y", segment.Data2);
                        newSegment.AddParameter("Speed", segment.Data3);
                        newSegment.AddParameter("Pause", segment.Data4);
                    }
                    break;
                case Stories.V2.StorySegment.StoryAction.ResetCamera: {
                        newSegment.Action = Stories.V3.StorySegment.StoryAction.ResetCamera;
                    }
                    break;
            }
            return newSegment;
        }
    }
}

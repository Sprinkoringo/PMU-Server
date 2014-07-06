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

namespace Server.Stories
{
    public class StoryBuilder
    {
        public static StoryBuilderSegment BuildStory() {
            return new StoryBuilderSegment();
        }

        public static void AppendSaySegment(StoryBuilderSegment story, string text, int mugshot, int speed, int pauseLocation) {
            StorySegment segment = new StorySegment();
            segment.Action = Enums.StoryAction.Say;
            segment.AddParameter("Text", text);
            segment.AddParameter("Mugshot", mugshot.ToString());
            segment.Parameters.Add("Speed", speed.ToString());
            segment.Parameters.Add("PauseLocation", pauseLocation.ToString());
            story.Segments.Add(segment);
        }

        public static void AppendPauseAction(StoryBuilderSegment story, int length) {
            StorySegment segment = new StorySegment();
            segment.Action = Enums.StoryAction.Pause;
            segment.AddParameter("Length", length.ToString());
            story.Segments.Add(segment);
        }

        public static void AppendWaitForMapAction(StoryBuilderSegment story, string map) {
            StorySegment segment = new StorySegment();
            segment.Action = Enums.StoryAction.WaitForMap;
            segment.AddParameter("MapID", map);
            story.Segments.Add(segment);
        }

        public static void AppendMapVisibilityAction(StoryBuilderSegment story, bool visible) {
            StorySegment segment = new StorySegment();
            segment.Action = Enums.StoryAction.MapVisibility;
            segment.AddParameter("Visible", visible.ToString());
            story.Segments.Add(segment);
        }

        public static void AppendPlayMusicAction(StoryBuilderSegment story, string file, bool honorSettings, bool loop) {
            StorySegment segment = new StorySegment();
            segment.Action = Enums.StoryAction.PlayMusic;
            segment.AddParameter("File", file);
            segment.AddParameter("HonorSettings", honorSettings.ToString());
            segment.AddParameter("Loop", loop.ToString());
            story.Segments.Add(segment);
        }

        public static void AppendStopMusicAction(StoryBuilderSegment story) {
            StorySegment segment = new StorySegment();
            segment.Action = Enums.StoryAction.StopMusic;
            story.Segments.Add(segment);
        }

        public static void AppendShowImageAction(StoryBuilderSegment story, string file, string imageID, int x, int y) {
            StorySegment segment = new StorySegment();
            segment.Action = Enums.StoryAction.ShowImage;
            segment.AddParameter("File", file);
            segment.AddParameter("ImageID", imageID);
            segment.AddParameter("X", x.ToString());
            segment.AddParameter("Y", y.ToString());
            story.Segments.Add(segment);
        }

        public static void AppendHideImageAction(StoryBuilderSegment story, string imageID) {
            StorySegment segment = new StorySegment();
            segment.Action = Enums.StoryAction.HideImage;
            segment.AddParameter("ImageID", imageID);
            story.Segments.Add(segment);
        }

        public static void AppendWarpAction(StoryBuilderSegment story, string mapID, int x, int y) {
            StorySegment segment = new StorySegment();
            segment.Action = Enums.StoryAction.Warp;
            segment.AddParameter("MapID", mapID);
            segment.AddParameter("X", x.ToString());
            segment.AddParameter("Y", y.ToString());
            story.Segments.Add(segment);
        }

        public static void AppendPlayerPadlockAction(StoryBuilderSegment story, string state) {
            StorySegment segment = new StorySegment();
            segment.Action = Enums.StoryAction.PlayerPadlock;
            segment.AddParameter("MovementState", state);
            story.Segments.Add(segment);
        }

        public static void AppendShowBackgroundAction(StoryBuilderSegment story, string file) {
            StorySegment segment = new StorySegment();
            segment.Action = Enums.StoryAction.ShowBackground;
            segment.AddParameter("File", file);
            story.Segments.Add(segment);
        }

        public static void AppendHideBackgroundAction(StoryBuilderSegment story) {
            StorySegment segment = new StorySegment();
            segment.Action = Enums.StoryAction.HideBackground;
            story.Segments.Add(segment);
        }

        public static void AppendCreateFNPCAction(StoryBuilderSegment story, string id, string parentMapID, int x, int y, int sprite) {
            StorySegment segment = new StorySegment();
            segment.Action = Enums.StoryAction.CreateFNPC;
            segment.AddParameter("ID", id);
            segment.AddParameter("ParentMapID", parentMapID);
            segment.AddParameter("X", x.ToString());
            segment.AddParameter("Y", y.ToString());
            segment.AddParameter("Sprite", sprite.ToString());
            story.Segments.Add(segment);
        }

        public static void AppendMoveFNPCAction(StoryBuilderSegment story, string id, int x, int y, Enums.Speed speed, bool pause) {
            StorySegment segment = new StorySegment();
            segment.Action = Enums.StoryAction.MoveFNPC;
            segment.AddParameter("ID", id);
            segment.AddParameter("X", x.ToString());
            segment.AddParameter("Y", y.ToString());
            segment.AddParameter("Speed", ((int)speed).ToString());
            segment.AddParameter("Pause", pause.ToIntString());
            story.Segments.Add(segment);
        }

        public static void AppendWarpFNPCAction(StoryBuilderSegment story, string id, int x, int y) {
            StorySegment segment = new StorySegment();
            segment.Action = Enums.StoryAction.WarpFNPC;
            segment.AddParameter("ID", id);
            segment.AddParameter("X", x.ToString());
            segment.AddParameter("Y", y.ToString());
            story.Segments.Add(segment);
        }

        public static void AppendChangeFNPCDirAction(StoryBuilderSegment story, string id, Enums.Direction direction) {
            StorySegment segment = new StorySegment();
            segment.Action = Enums.StoryAction.ChangeFNPCDir;
            segment.AddParameter("ID", id);
            segment.AddParameter("Direction", ((int)direction).ToString());
            story.Segments.Add(segment);
        }

        public static void AppendDeleteFNPCAction(StoryBuilderSegment story, string id) {
            StorySegment segment = new StorySegment();
            segment.Action = Enums.StoryAction.DeleteFNPC;
            segment.AddParameter("ID", id);
            story.Segments.Add(segment);
        }

        public static void AppendRunScriptAction(StoryBuilderSegment story, int scriptIndex, string param1, string param2, string param3, bool pause) {
            StorySegment segment = new StorySegment();
            segment.Action = Enums.StoryAction.RunScript;
            segment.AddParameter("ScriptIndex", scriptIndex.ToString());
            segment.AddParameter("ScriptParam1", param1);
            segment.AddParameter("ScriptParam2", param2);
            segment.AddParameter("ScriptParam3", param3);
            segment.AddParameter("Pause", pause.ToIntString());
            story.Segments.Add(segment);
        }

        public static void AppendHidePlayersAction(StoryBuilderSegment story) {
            StorySegment segment = new StorySegment();
            segment.Action = Enums.StoryAction.HidePlayers;
            story.Segments.Add(segment);
        }

        public static void AppendShowPlayersAction(StoryBuilderSegment story) {
            StorySegment segment = new StorySegment();
            segment.Action = Enums.StoryAction.ShowPlayers;
            story.Segments.Add(segment);
        }

        public static void AppendFNPCEmotionAction(StoryBuilderSegment story, string id, int emotion) {
            StorySegment segment = new StorySegment();
            segment.Action = Enums.StoryAction.FNPCEmotion;
            segment.AddParameter("ID", id);
            segment.AddParameter("Emotion", emotion.ToString());
            story.Segments.Add(segment);
        }

        public static void AppendChangeWeatherAction(StoryBuilderSegment story, Enums.Weather weather) {
            StorySegment segment = new StorySegment();
            segment.Action = Enums.StoryAction.ChangeWeather;
            segment.AddParameter("Weather", ((int)weather).ToString());
            story.Segments.Add(segment);
        }

        public static void AppendHideNPCsAction(StoryBuilderSegment story) {
            StorySegment segment = new StorySegment();
            segment.Action = Enums.StoryAction.HideNPCs;
            story.Segments.Add(segment);
        }

        public static void AppendShowNPCsAction(StoryBuilderSegment story) {
            StorySegment segment = new StorySegment();
            segment.Action = Enums.StoryAction.ShowNPCs;
            story.Segments.Add(segment);
        }

        public static void AppendGoToSegmentAction(StoryBuilderSegment story, int segment) {
            StorySegment tempSegment = new StorySegment();
            tempSegment.Action = Enums.StoryAction.GoToSegment;
            tempSegment.AddParameter("Segment", segment.ToString());

            story.Segments.Add(tempSegment);
        }

        public static void AppendAskQuestionAction(StoryBuilderSegment story, string question, int segmentOnYes, int segmentOnNo, int mugshot) {
            StorySegment segment = new StorySegment();
            segment.Action = Enums.StoryAction.AskQuestion;
            segment.AddParameter("Question", question);
            segment.AddParameter("SegmentOnYes", segmentOnYes.ToString());
            segment.AddParameter("SegmentOnNo", segmentOnNo.ToString());
            segment.AddParameter("Mugshot", mugshot.ToString());

            story.Segments.Add(segment);
        }

        public static void AppendAskQuestionAction(StoryBuilderSegment story, string question, string questionID, int mugshot, string[] options) {
            StorySegment segment = new StorySegment();
            segment.Action = Enums.StoryAction.AskQuestion;
            segment.AddParameter("Question", question);
            segment.AddParameter("SegmentOnYes", "-1");
            segment.AddParameter("SegmentOnNo", "-1");
            segment.AddParameter("Mugshot", mugshot.ToString());
            StringBuilder optionsString = new StringBuilder();
            for (int i = 0; i < options.Length; i++) {
                optionsString.Append(options[i]);
                optionsString.Append("\\");
            }
            segment.AddParameter("Options", optionsString.ToString());
            segment.AddParameter("QuestionID", questionID);

            story.Segments.Add(segment);
        }

        public static void AppendMovePlayerAction(StoryBuilderSegment story, int targetX, int targetY, Enums.Speed speed, bool pause) {
            StorySegment segment = new StorySegment();
            segment.Action = Enums.StoryAction.MovePlayer;
            segment.AddParameter("X", targetX.ToString());
            segment.AddParameter("Y", targetY.ToString());
            segment.AddParameter("Speed", ((int)speed).ToString());
            segment.AddParameter("Pause", pause.ToString());

            story.Segments.Add(segment);
        }
    }
}

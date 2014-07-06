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
using Server.Scripting;
using Server.Players;
using Server.Network;
using PMU.Sockets;

namespace Server.Stories
{
    public class StoryManager : StoryManagerBase
    {
        public static void RunScript(Client client, int script, string param1, string param2, string param3, bool wait) {
            if (wait) {
                ScriptManager.InvokeSub("StoryScript", client, script, param1, param2, param3, wait);
                Messenger.SendDataTo(client, TcpPacket.CreatePacket("storyscriptcomplete"));
            } else {
                Messenger.SendDataTo(client, TcpPacket.CreatePacket("storyscriptcomplete"));
                ScriptManager.InvokeSub("StoryScript", client, script, param1, param2, param3, wait);
            }
        }

        private static void StartStoryCheck(Client client, int storyNum) {
            if (client.Player.GetStoryState(storyNum) == false) {
                if (Stories[storyNum].StoryStart != 0) {
                    if (client.Player.GetStoryState(Stories[storyNum].StoryStart - 1) == false) {
                        return;
                    }
                }
                client.Player.LoadingStory = true;
                Messenger.SendLoadingStoryTo(client);
                client.Player.CurrentChapter = Stories[storyNum];
                Messenger.SendDataTo(client, TcpPacket.CreatePacket("storycheck", storyNum.ToString(), Stories[storyNum].Revision.ToString()));
            }
        }

        public static StorySegment GetSegment(int chapter, int segment) {
            return Stories[chapter].Segments[segment];
        }

        internal static void ResumeStory(Client client, int storyNum) {
            client.Player.LoadingStory = true;
            Messenger.SendLoadingStoryTo(client);
            client.Player.CurrentChapter = Stories[storyNum];
            Messenger.SendDataTo(client, TcpPacket.CreatePacket("storycheck", storyNum.ToString(), Stories[storyNum].Revision.ToString()));
        }

        public static void PlayStory(Client client, int storyNum) {
            if (storyNum > -1 && storyNum <= Stories.MaxStories) {
                if (client.Player.CurrentChapter != null) {
                    client.Player.StoryPlaybackCache.Add(Stories[storyNum]);
                } else {
                    StartStoryCheck(client, storyNum);
                    //PlayStory(client, Stories.Stories[storyNum]);
                }
            }
        }

        public static void PlayStory(Client client, Story story) {
            if (client.Player.CurrentChapter != null) {
                client.Player.StoryPlaybackCache.Add(story);
            } else {
                if (string.IsNullOrEmpty(story.Name)) {
                    story.Name = "Scripted Story";
                }
                client.Player.CurrentChapter = story;
                client.Player.LoadingStory = true;
                Messenger.SendLoadingStoryTo(client);
                Messenger.SendRunStoryTo(client, story);
            }
        }
    }
}

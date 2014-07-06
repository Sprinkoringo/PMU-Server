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
   public class StoryManager
    {
        public static Story LoadStory(int storyNum) {
            Story story = new Story();
            string FileName = IO.Paths.StoriesFolder + "story" + storyNum + ".dat";
            using (System.IO.StreamReader read = new System.IO.StreamReader(FileName)) {
                string str;
                string[] parse;
                while (!(read.EndOfStream)) {
                    str = read.ReadLine();
                    parse = str.Split('|');
                    switch (parse[0].ToLower()) {
                        case "storydata":
                            if (parse[1].ToLower() == "v1") {
                                story.Version = 1;
                            } else if (parse[1].ToLower() == "v2") {
                                story.Version = 2;
                            }
                            story.Revision = parse[2].ToInt();
                            break;
                        case "data":
                            story.Name = parse[1];
                            story.MaxSegments = parse[2].ToInt();
                            if (parse.Length > 4) {
                                story.StoryStart = parse[3].ToInt();
                            }
                            story.UpdateStorySegments();
                            break;
                        case "segment":
                            if (story.Segments[parse[1].ToInt()] == null) {
                                story.Segments[parse[1].ToInt()] = new StorySegment();
                            }
                            story.Segments[parse[1].ToInt()].Action = (StorySegment.StoryAction)parse[2].ToInt();
                            story.Segments[parse[1].ToInt()].Data1 = parse[3];
                            story.Segments[parse[1].ToInt()].Data2 = parse[4];
                            story.Segments[parse[1].ToInt()].Data3 = parse[5];
                            story.Segments[parse[1].ToInt()].Data4 = parse[6];
                            story.Segments[parse[1].ToInt()].Data5 = parse[7];
                            break;
                        case "exitandcontinue": {
                                int max = parse[1].ToInt();
                                int z = 2;
                                for (int i = 0; i < max; i++) {
                                    story.ExitAndContinue.Add(parse[z].ToInt());
                                    z++;
                                }
                            }
                            break;
                    }
                }
            }
            return story;
        }

        public static void SaveStory(Story story, int storyNum) {
            string FileName = IO.Paths.StoriesFolder + "story" + storyNum + ".dat";
            using (System.IO.StreamWriter write = new System.IO.StreamWriter(FileName)) {
                write.WriteLine("StoryData|V2|" + story.Revision + "|");
                write.WriteLine("Data|" + story.Name + "|" + story.MaxSegments + "|" + story.StoryStart + "|");
                if (story.Segments == null) {
                    story.UpdateStorySegments();
                }
                for (int i = 0; i <= story.MaxSegments; i++) {
                    if (story.Segments[i] == null)
                        story.Segments[i] = new StorySegment();
                    write.WriteLine("Segment|" + i + "|" + (int)story.Segments[i].Action + "|" + story.Segments[i].Data1 + "|" + story.Segments[i].Data2 + "|" + story.Segments[i].Data3 + "|" + story.Segments[i].Data4 + "|" + story.Segments[i].Data5 + "|");
                }
                string exitAndContinue = "ExitAndContinue|" + story.ExitAndContinue.Count.ToString() + "|";
                for (int i = 0; i < story.ExitAndContinue.Count; i++) {
                    exitAndContinue += story.ExitAndContinue[i].ToString() + "|";
                }
                write.WriteLine(exitAndContinue);
            }
        }
    }
}

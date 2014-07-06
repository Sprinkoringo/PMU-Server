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

namespace Server.DataConverter.Stories.V3
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
                            if (parse[1].ToLower() != "v3") {
                                read.Close();
                                return null;
                            }
                            story.Revision = parse[2].ToInt();
                            break;
                        case "data": {
                                story.Name = parse[1];
                                story.StoryStart = parse[2].ToInt();
                            }
                            break;
                        case "segment": {
                                StorySegment segment = new StorySegment();
                                story.Segments.Add(segment);
                                story.Segments[parse[1].ToInt()].Action = (StorySegment.StoryAction)parse[2].ToInt();
                                int totalParameters = parse[3].ToInt();
                                int z = 4;
                                for (int i = 0; i < totalParameters; i++) {
                                    segment.Parameters.Add(parse[z], parse[z + 1]);

                                    z += 2;
                                }
                            }
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
                write.WriteLine("StoryData|V3|" + story.Revision + "|");
                write.WriteLine("Data|" + story.Name + "|" + story.StoryStart + "|");
                for (int i = 0; i < story.Segments.Count; i++) {
                    if (story.Segments[i] != null) {
                        write.Write("Segment|" + i + "|" + (int)story.Segments[i].Action + "|" + story.Segments[i].Parameters.Count + "|");
                        for (int z = 0; z < story.Segments[i].Parameters.Count; z++) {
                            write.Write(story.Segments[i].Parameters.KeyByIndex(z) + "|" + story.Segments[i].Parameters.ValueByIndex(z) + "|");
                        }
                        write.WriteLine();
                    }
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

using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Stories
{
    public class StoryBuilderSegment
    {
        List<StorySegment> segments;

        public List<StorySegment> Segments {
            get { return segments; }
        }

        public StoryBuilderSegment() {
            segments = new List<StorySegment>();
        }

        public void AppendToStory(Story story) {
            for (int i = 0; i < segments.Count; i++) {
                story.Segments.Add(segments[i]);
            }
        }
    }
}

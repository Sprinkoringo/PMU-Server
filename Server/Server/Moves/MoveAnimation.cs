using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Moves {
    public class MoveAnimation {

        public Enums.MoveAnimationType AnimationType { get; set; }

        public int AnimationIndex { get; set; }

        public int FrameSpeed { get; set; }

        public int Repetitions { get; set; }

    }
}

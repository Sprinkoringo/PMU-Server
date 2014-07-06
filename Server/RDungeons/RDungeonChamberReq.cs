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
using System.Linq;
using System.Text;

namespace Server.RDungeons {
    public class RDungeonChamberReq {
        //passed to the random dungeon generator to know what the chamber requires

        public int MinX { get; set; }
        public int MinY { get; set; }

        public int MaxX { get; set; }
        public int MaxY { get; set; }

        public int TopPassage { get; set; }
        public int BottomPassage { get; set; }
        public int LeftPassage { get; set; }
        public int RightPassage { get; set; }

        public Enums.Acceptance TopAcceptance { get; set; }
        public Enums.Acceptance BottomAcceptance { get; set; }
        public Enums.Acceptance LeftAcceptance { get; set; }
        public Enums.Acceptance RightAcceptance { get; set; }

        public Enums.Acceptance Start { get; set; }
        public Enums.Acceptance End { get; set; }

        public RDungeonChamberReq() {
            MinX = 2;
            MaxX = 2;
            MinY = 2;
            MaxY = 2;
            TopPassage = -1;
            TopAcceptance = Enums.Acceptance.Maybe;
            BottomPassage = -1;
            BottomAcceptance = Enums.Acceptance.Maybe;
            LeftPassage = -1;
            LeftAcceptance = Enums.Acceptance.Maybe;
            RightPassage = -1;
            RightAcceptance = Enums.Acceptance.Maybe;
        }
    }
}

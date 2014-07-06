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

namespace DataManager.Players
{
    public class RecruitData
    {
        public bool UsingTempStats { get; set; }
        public string Name { get; set; }
        public bool Nickname { get; set; }
        public int NpcBase { get; set; }
        public int Species { get; set; }
        public int Form { get; set; }
        public int Shiny { get; set; }
        public byte Sex { get; set; }
        public int HeldItemSlot { get; set; }
        public int Level { get; set; }
        public ulong Exp { get; set; }
        public int HP { get; set; }
        public int StatusAilment { get; set; }
        public int StatusAilmentCounter { get; set; }
        public int IQ { get; set; }
        public int Belly { get; set; }
        public int MaxBelly { get; set; }
        public int AtkBonus { get; set; }
        public int DefBonus { get; set; }
        public int SpeedBonus { get; set; }
        public int SpclAtkBonus { get; set; }
        public int SpclDefBonus { get; set; }

        public Characters.Move[] Moves { get; set; }
        public List<Characters.VolatileStatus> VolatileStatus { get; set; }


        public RecruitData() {
            HeldItemSlot = -1;
            Moves = new Characters.Move[4];
            for (int i = 0; i < Moves.Length; i++) {
                Moves[i] = new Characters.Move();
            }
            VolatileStatus = new List<Characters.VolatileStatus>();
        }

    }
}

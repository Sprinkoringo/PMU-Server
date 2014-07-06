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


namespace Server.Players
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class RecruitMove
    {
        DataManager.Characters.Move rawMove;

        #region Constructors

        public RecruitMove(DataManager.Characters.Move rawMove) {
            this.rawMove = rawMove;
        }

        public RecruitMove() {
            this.rawMove = new DataManager.Characters.Move();

            MoveNum = -1;
            CurrentPP = -1;
            MaxPP = -1;
        }

        #endregion Constructors

        #region Properties

        public int CurrentPP {
            get { return rawMove.CurrentPP; }
            set { rawMove.CurrentPP = value; }
        }

        public int MaxPP {
            get { return rawMove.MaxPP; }
            set { rawMove.MaxPP = value; }
        }

        public int MoveNum {
            get { return rawMove.MoveNum; }
            set { rawMove.MoveNum = value; }
        }

        public bool Sealed {
            //get { return rawMove.Sealed; }
            //set { rawMove.Sealed = value; }
            get;
            set;
        }

        public int AttackTime {
            
            get;
            set;
        }

        #endregion Properties
    }
}
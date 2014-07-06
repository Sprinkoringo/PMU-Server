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
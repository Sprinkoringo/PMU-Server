using System;
using System.Collections.Generic;
using System.Text;
using PMU.Core;

namespace Server.Moves
{
    public class MoveCollection
    {
         #region Fields

        ListPair<int, Move> moves;
        int maxMoves;

        #endregion Fields

        #region Constructors

        public MoveCollection(int maxMoves)
        {
            if (maxMoves == 0)
                maxMoves = 50;
            this.maxMoves = maxMoves;
            moves = new ListPair<int, Move>();
        }

        #endregion Constructors

        #region Properties

        public ListPair<int, Move> Moves
        {
            get { return moves; }
        }

        public int MaxMoves
        {
            get { return maxMoves; }
        }

        #endregion Properties

        #region Indexers

        public Move this[int index]
        {
            get { return moves[index]; }
            set { moves[index] = value; }
        }

        #endregion Indexers
    }
}

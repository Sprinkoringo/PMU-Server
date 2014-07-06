namespace Server.Exp
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class ExpCollection
    {
        #region Fields

        ulong[] exp;
        int maxLevels;

        #endregion Fields

        #region Constructors

        public ExpCollection(int maxLevels)
        {
            if (maxLevels == 0)
                maxLevels = 100;
            exp = new ulong[maxLevels];
            this.maxLevels = maxLevels;
        }

        #endregion Constructors

        #region Properties

        public int MaxLevels
        {
            get { return maxLevels; }
        }

        #endregion Properties

        #region Indexers

        public ulong this[int index]
        {
            get { return exp[index]; }
            set { exp[index] = value; }
        }

        #endregion Indexers
    }
}
namespace Server.Evolutions
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using PMU.Core;

    public class EvolutionCollection
    {
        #region Fields

        ListPair<int, Evolution> evolutions;
        int maxEvos;

        #endregion Fields

        #region Constructors

        public EvolutionCollection(int maxEvos)
        {
            if (maxEvos == 0)
                maxEvos = 50;
            this.maxEvos = maxEvos;
            evolutions = new ListPair<int,Evolution>();
        }

        #endregion Constructors

        #region Properties

        public ListPair<int, Evolution> Evolutions
        {
            get { return evolutions; }
        }

        public int MaxEvos
        {
            get { return maxEvos; }
        }

        #endregion Properties

        #region Indexers

        public Evolution this[int index]
        {
            get { return evolutions[index]; }
            set { evolutions[index] = value; }
        }

        #endregion Indexers
    }
}
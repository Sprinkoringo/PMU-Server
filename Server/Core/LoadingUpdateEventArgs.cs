namespace Server
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class LoadingUpdateEventArgs : EventArgs
    {
        #region Fields

        int currentValue;
        int maxValue;
        int percent;

        #endregion Fields

        #region Constructors

        public LoadingUpdateEventArgs(int currentValue, int maxValue)
        {
            this.currentValue = currentValue;
            this.maxValue = maxValue;
            this.percent = Math.CalculatePercent(currentValue, maxValue);
        }

        #endregion Constructors

        #region Properties

        public int CurrentValue
        {
            get { return currentValue; }
        }

        public int MaxValue
        {
            get { return maxValue; }
        }

        public int Percent
        {
            get { return percent; }
        }

        #endregion Properties
    }
}
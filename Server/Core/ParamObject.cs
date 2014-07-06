namespace Server
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class ParamObject
    {
        #region Fields

        object[] param;

        #endregion Fields

        #region Constructors

        public ParamObject(params object[] param) {
            this.param = param;
        }

        #endregion Constructors

        #region Properties

        public Object[] Param
        {
            get { return param; }
        }

        #endregion Properties
    }
}
namespace Server.Evolutions
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Evolution
    {
        

        #region Properties

        
        public string Name {get; set;}
        public int Species {get; set; }
        
        public List<EvolutionBranch> Branches { get; set; }

        #endregion Properties

        //#region Methods

        //public void UpdateSplitEvos()
        //{
        //    splitEvos = new Evolution[splitEvosNum + 1];
        //    for (int i = 0; i <= splitEvosNum; i++) {
        //        splitEvos[i] = new Evolution(true);
        //    }
        //}

        //#endregion Methods

        public Evolution() {
            //if (!splitEvo) {
            //    splitEvos = new Evolution[1];
            //    splitEvos[0] = new Evolution(true);
            //}
            Branches = new List<EvolutionBranch>();
        }
    }
}
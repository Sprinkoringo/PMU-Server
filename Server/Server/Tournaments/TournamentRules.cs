using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Tournaments
{
    public class TournamentRules
    {
        public bool SleepClause { get; set; }
        public bool AccuracyClause { get; set; }
        public bool SpeciesClause { get; set; }
        public bool FreezeClause { get; set; }
        public bool OHKOClause { get; set; }
        public bool SelfKOClause { get; set; }
    }
}

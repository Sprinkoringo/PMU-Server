using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Combat
{
    class BattleResult
    {
        bool victimFainted;
        ICharacter attacker;
        ICharacter victim;

        public BattleResult(ICharacter attacker, ICharacter victim) {
            this.attacker = attacker;
            this.victim = victim;
        }

        internal void SetVictimFainted(bool victimFainted) {
            this.victimFainted = victimFainted;
        }

        public bool VictimFainted {
            get { return victimFainted; }
        }
    }
}

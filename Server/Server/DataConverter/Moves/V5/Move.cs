using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.DataConverter.Moves.V5 {
    public class Move {

        #region Properties


        public string Name { get; set; }

        public int MaxPP { get; set; }


        public Enums.MoveType EffectType { get; set; }


        public Enums.PokemonType Element { get; set; }

        public Enums.MoveCategory MoveCategory { get; set; }


        public Enums.MoveRange RangeType { get; set; }

        public int Range { get; set; }

        public Enums.MoveTarget TargetType { get; set; }



        public int Data1 { get; set; }

        public int Data2 { get; set; }

        public int Data3 { get; set; }

        public int Accuracy { get; set; }

        public int HitTime { get; set; }

        public bool HitFreeze { get; set; }

        public int AdditionalEffectData1 { get; set; }

        public int AdditionalEffectData2 { get; set; }

        public int AdditionalEffectData3 { get; set; }

        public bool PerPlayer { get; set; }

        public int KeyItem { get; set; }


        public int Sound { get; set; }

        public MoveAnimation AttackerAnim { get; set; }

        public MoveAnimation TravelingAnim { get; set; }

        public MoveAnimation DefenderAnim { get; set; }



        #endregion Properties

        public Move() {
            AttackerAnim = new MoveAnimation();
            TravelingAnim = new MoveAnimation();
            DefenderAnim = new MoveAnimation();
        }
    }
}

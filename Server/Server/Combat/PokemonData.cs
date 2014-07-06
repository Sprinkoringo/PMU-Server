using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Combat {
    public class PokemonData {

        public PokemonData() {

        }

        public PokemonData(PokemonData data) {
            DexNum = data.DexNum;
            Form = data.Form;
            Shiny = data.Shiny;
            Sex = data.Sex;
        }

        public static bool operator ==(PokemonData data1, PokemonData data2) {
            return (data1.DexNum == data2.DexNum &&
            data1.Form == data2.Form &&
            data1.Shiny == data2.Shiny &&
                data1.Sex == data2.Sex);
        }

        public static bool operator !=(PokemonData data1, PokemonData data2) {
            return !(data1 == data2);
        }

        public int DexNum { get; set; }
        public int Form { get; set; }
        public Enums.Coloration Shiny { get; set; }
        public Enums.Sex Sex { get; set; }
    }
}

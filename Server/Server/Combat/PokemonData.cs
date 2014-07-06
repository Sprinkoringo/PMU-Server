/*The MIT License (MIT)

Copyright (c) 2014 PMU Staff

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/


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

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
    public class TargetCollection {

        public List<ICharacter> Foes { get; set; }

        public List<ICharacter> Friends { get; set; }

        public List<ICharacter> Self { get; set; }

        public TargetCollection() {
            Foes = new List<ICharacter>();
            Friends = new List<ICharacter>();
            Self = new List<ICharacter>();
        }

        public void Add(ICharacter character, Enums.CharacterMatchup matchup) {
            switch (matchup) {
                case Enums.CharacterMatchup.Foe: {
                    Foes.Add(character);
                    }
                    break;
                case Enums.CharacterMatchup.Friend: {
                    Friends.Add(character);
                    }
                    break;
                case Enums.CharacterMatchup.Self: {
                    Self.Add(character);
                    }
                    break;
            }
        }

        public List<ICharacter> GetTargets(Enums.CharacterMatchup matchup) {
            switch (matchup) {
                case Enums.CharacterMatchup.Foe: {
                    return Foes;
                    }
                    break;
                case Enums.CharacterMatchup.Friend: {
                    return Friends;
                    }
                    break;
                case Enums.CharacterMatchup.Self: {
                    return Self;
                    }
                    break;
            }

            return null;
        }

        public int Count { get { return Foes.Count + Friends.Count + Self.Count; } }

        public ICharacter this[int index] {
            get {
                if (index >= Foes.Count + Friends.Count) {
                    return Self[index - Friends.Count - Foes.Count];
                } else if (index >= Foes.Count) {
                    return Friends[index - Foes.Count];
                } else {
                    return Foes[index];
                }
            }

        }

    }
}

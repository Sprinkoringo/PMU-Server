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

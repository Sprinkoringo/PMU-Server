namespace Server.Players
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class CharacterInformation
    {
        #region Fields

        string account;
        string id;
        string name;
        int slot;

        #endregion Fields

        #region Constructors

        public CharacterInformation(string name, string account, int slot, string id) {
            this.name = name;
            this.account = account;
            this.slot = slot;
            this.id = id;
        }

        #endregion Constructors

        #region Properties

        public string Account {
            get { return account; }
        }

        public string ID {
            get { return id; }
        }

        public string Name {
            get {
                return name;
            }
        }

        public int Slot {
            get { return slot; }
        }

        #endregion Properties
    }
}
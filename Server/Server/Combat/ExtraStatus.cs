using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Combat
{
    public class ExtraStatus
    {
        DataManager.Characters.VolatileStatus rawVolatileStatus;

        public DataManager.Characters.VolatileStatus RawVolatileStatus {
            get {
                return rawVolatileStatus;
            }
        }

        public string Name {
            get { return rawVolatileStatus.Name; }
            set { rawVolatileStatus.Name = value; }
        }

        public int Emoticon {
            get { return rawVolatileStatus.Emoticon; }
            set { rawVolatileStatus.Emoticon = value; }
        }

        public int Counter {
            get { return rawVolatileStatus.Counter; }
            set { rawVolatileStatus.Counter = value; }
        }

        public string Tag {
            get { return rawVolatileStatus.Tag; }
            set { rawVolatileStatus.Tag = value; }
        }

        public ICharacter Target { get; set; }

        public ExtraStatus() {
            this.rawVolatileStatus = new DataManager.Characters.VolatileStatus();
        }

        public ExtraStatus(DataManager.Characters.VolatileStatus rawVolatileStatus) {
            this.rawVolatileStatus = rawVolatileStatus;
        }

    }
}

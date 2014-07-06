using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Players
{
    public class AvailableExpKitModule
    {
        Enums.ExpKitModules type;
        bool temporary;

        public Enums.ExpKitModules Type {
            get { return type; }
        }

        public bool Temporary {
            get { return temporary; }
            set { temporary = value; }
        }

        public AvailableExpKitModule(Enums.ExpKitModules type, bool temporary) {
            this.type = type;
            this.temporary = temporary;
        }
    }
}

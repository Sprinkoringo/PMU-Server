using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Server.Players
{
    class PlayerPet
    {
        public int Sprite {
            get;
            set;
        }

        public Enums.Direction Direction {
            get;
            set;
        }

        public bool Attacking {
            get;
            set;
        }

        public System.Drawing.Point Offset {
            get;
            set;
        }

        public System.Drawing.Point Location {
            get;
            set;
        }

        public int AttackTimer {
            get;
            set;
        }

        public int X {
            get { return Location.X; }
            set { Location = new Point(value, Location.Y); }
        }

        public int Y {
            get { return Location.Y; }
            set { Location = new Point(Location.X, value); }
        }

        public bool Confused {
            get;
            set;
        }

        public Enums.StatusAilment StatusAilment {
            get;
            set;
        }

    }
}

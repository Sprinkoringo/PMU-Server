using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Maps
{
    public class MapStatus
    {
        DataManager.Maps.MapStatus rawStatus;


        public DataManager.Maps.MapStatus RawStatus {
            get {
                return rawStatus;
            }
        }

        public MapStatus(DataManager.Maps.MapStatus rawStatus) {
            this.rawStatus = rawStatus;
        }

        public MapStatus(string name, int counter, string tag, int graphicEffect) 
            : this(new DataManager.Maps.MapStatus(name, counter, tag, graphicEffect)) {
        }

        public string Name {
            get { return rawStatus.Name; }
            set { rawStatus.Name = value; }
        }

        public int Counter {
            get { return rawStatus.Counter; }
            set { rawStatus.Counter = value; }
        }

        public String Tag {
            get { return rawStatus.Tag; }
            set { rawStatus.Tag = value; }
        }

        public int GraphicEffect {
            get { return rawStatus.GraphicEffect; }
            set { rawStatus.GraphicEffect = value; }
        }
    }
}

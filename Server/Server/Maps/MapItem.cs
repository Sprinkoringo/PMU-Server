using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Maps
{
    public class MapItem
    {
        DataManager.Maps.MapItem rawItem;

        public DataManager.Maps.MapItem RawItem {
            get {
                return rawItem;
            }
        }

        public MapItem(DataManager.Maps.MapItem rawItem) {
            this.rawItem = rawItem;
        }

        public int Num {
            get { return rawItem.Num; }
            set { rawItem.Num = value; }
        }
        public int Value {
            get { return rawItem.Value; }
            set { rawItem.Value = value; }
        }
        public bool Sticky {
            get { return rawItem.Sticky; }
            set { rawItem.Sticky = value; }
        }
        public bool Hidden {
            get { return rawItem.Hidden; }
            set { rawItem.Hidden = value; }
        }
        public string Tag {
            get { return rawItem.Tag; }
            set { rawItem.Tag = value; }
        }
        public int X {
            get { return rawItem.X; }
            set { rawItem.X = value; }
        }
        public int Y {
            get { return rawItem.Y; }
            set { rawItem.Y = value; }
        }
        public string PlayerFor {
            get { return rawItem.PlayerFor; }
            set { rawItem.PlayerFor = value; }
        }
        public TickCount TimeDropped { get; set; }
    }
}

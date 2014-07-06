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

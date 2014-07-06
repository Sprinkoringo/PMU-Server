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

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

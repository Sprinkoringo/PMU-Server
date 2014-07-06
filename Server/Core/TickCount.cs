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

namespace Server
{
    public class TickCount
    {
        int tick;

        public int Tick {
            get { return tick; }
        }

        public TickCount(int tick) {
            this.tick = tick;
        }

        public bool Elapsed(int storedTick, int interval) {
            int tickSign = System.Math.Sign(this.tick);
            int storedTickSign = System.Math.Sign(storedTick);

            if (tickSign == -1 && storedTickSign == -1) {
                return (this.tick > storedTick + interval);
            } else if (tickSign == 1 && storedTickSign == 1) {
                return (this.tick > storedTick + interval);
            } else if (tickSign == -1 && storedTickSign == 1) {
                int remainder = Int32.MaxValue - storedTick;
                if (remainder > interval) {
                    return true;
                }
                return (this.tick > Int32.MinValue + (interval - remainder));
            }

            return (this.tick > storedTick + interval);
        }

        public bool Elapsed(TickCount storedTick, int interval) {
            if (storedTick != null) {
                return Elapsed(storedTick.Tick, interval);
            } else {
                return true;
            }
        }

        public static TickCount operator +(TickCount tick1, TickCount tick2) {
            int overflowVal = Int32.MaxValue - tick2.Tick - tick1.tick;
            int newTick;
            if (overflowVal < 0) {
                newTick = System.Math.Abs(overflowVal);
            } else {
                newTick = tick1.Tick + tick2.Tick;
            }
            return new TickCount(newTick);
        }
    }
}

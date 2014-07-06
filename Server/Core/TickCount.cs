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

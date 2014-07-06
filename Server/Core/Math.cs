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
using System.Numerics;

namespace Server
{
    public class Math
    {
        //static readonly object RandSyncObject = new object();
        static readonly Random random = new Random(Environment.TickCount);
        private static Object lockObj = new Object();

        public static int CalculatePercent(int currentValue, int maxValue) {
            return currentValue * 100 / maxValue;
        }

        public static ulong CalculatePercent(ulong currentValue, ulong maxValue) {
            return currentValue * 100 / maxValue;
        }

        public static int RoundToMultiple(int number, int multiple) {
            double d = number / multiple;
            d = System.Math.Round(d, 0);
            return Convert.ToInt32(d * multiple);
        }

        public static int Rand(int low, int high) {
            lock (lockObj) {
                return random.Next(low, high);
            }
        }

        public static BigInteger CalculateFactorial(int factorial) {
            BigInteger currentNum = new BigInteger(factorial);
            for (int i = factorial - 1; i > 0; i--) {
                currentNum = currentNum * i;
            }
            return currentNum;
        }
    }
}

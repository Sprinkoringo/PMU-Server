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

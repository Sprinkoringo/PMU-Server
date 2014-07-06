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
    public static class Extensions
    {
        public static int ToInt(this string str) {
            int result = 0;
            if (!string.IsNullOrEmpty(str) && int.TryParse(str, out result)) {
                return result;
            } else
                return 0;
        }

        public static int ToInt(this string str, int defaultVal) {
            int result = 0;
            if (str != null && int.TryParse(str, out result) == true) {
                return result;
            } else
                return defaultVal;
        }

        public static double ToDbl(this string str) {
            double result = 0;
            if (str != null && double.TryParse(str, out result) == true) {
                return result;
            } else
                return 0;
        }

        public static double ToDbl(this string str, double defaultVal) {
            double result = 0;
            if (str != null && double.TryParse(str, out result) == true) {
                return result;
            } else
                return defaultVal;
        }

        public static string ToIntString(this bool boolval) {
            if (boolval == true)
                return "1";
            else
                return "0";
        }

        public static bool IsNumeric(this string str) {
            int result;
            return int.TryParse(str, out result);
        }

        public static ulong ToUlng(this string str) {
            ulong result = 0;
            if (ulong.TryParse(str, out result) == true) {
                return result;
            } else
                return 0;
        }

        public static bool ToBool(this string str) {
            if (!string.IsNullOrEmpty(str)) {
                switch (str.ToLower()) {
                    case "true":
                        return true;
                    case "false":
                        return false;
                    case "1":
                        return true;
                    case "0":
                        return false;
                    case "yes":
                        return true;
                    case "no":
                        return false;
                    default:
                        return false;
                }
            } else {
                return false;
            }
        }

        public static bool ToBool(this object obj) {
            if (obj != null) {
                return (bool)obj;
            } else {
                return false;
            }
        }

        public static DateTime? ToDate(this string date) {
            DateTime tmpDate;
            if (DateTime.TryParse(date, out tmpDate)) {
                return tmpDate;
            } else {
                return null;
            }
        }
    }
}

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ExtensionAttribute : Attribute
    {
    }
}


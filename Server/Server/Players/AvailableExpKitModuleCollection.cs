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

namespace Server.Players
{
    public class AvailableExpKitModuleCollection
    {
        List<AvailableExpKitModule> availableModules;

        public AvailableExpKitModule this[int index] {
            get { return availableModules[index]; }
        }

        public void Add(AvailableExpKitModule module) {
            availableModules.Add(module);
        }

        public void RemoveAt(int index) {
            availableModules.RemoveAt(index);
        }

        public void Remove(Enums.ExpKitModules type) {
            int index = IndexOf(type);
            if (index > -1) {
                RemoveAt(index);
            }
        }

        public int IndexOf(Enums.ExpKitModules type) {
            for (int i = 0; i < availableModules.Count; i++) {
                if (availableModules[i].Type == type) {
                    return i;
                }
            }
            return -1;
        }

        public bool Contains(Enums.ExpKitModules type) {
            return IndexOf(type) != -1;
        }

        public int Count {
            get { return availableModules.Count; }
        }

        public AvailableExpKitModuleCollection() {
            availableModules = new List<AvailableExpKitModule>();
        }

        public AvailableExpKitModuleCollection(int capacity) {
            availableModules = new List<AvailableExpKitModule>(capacity);
        }

    }
}

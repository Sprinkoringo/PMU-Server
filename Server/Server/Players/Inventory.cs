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

using PMU.Core;

namespace Server.Players
{
    public class Inventory
    {
        ListPair<int, InventoryItem> items;

        public Inventory(ListPair<int, DataManager.Characters.InventoryItem> loadedInventory) {
            items = new ListPair<int, InventoryItem>();
            for (int i = 0; i < loadedInventory.Count; i++) {
                items.Add(loadedInventory.KeyByIndex(i), new InventoryItem(loadedInventory.ValueByIndex(i)));
            }
        }

        public void Add(int slot, InventoryItem item) {
            items.Add(slot, item);
        }

        public InventoryItem this[int slot] {
            get {
                return items[slot];
            }
            set {
                items[slot] = value;
            }
        }

        public bool ContainsKey(int slot) {
            return items.ContainsKey(slot);
        }

        public int Count {
            get { return items.Count; }
        }
    }
}

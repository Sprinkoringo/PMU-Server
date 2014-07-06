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

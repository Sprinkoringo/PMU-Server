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

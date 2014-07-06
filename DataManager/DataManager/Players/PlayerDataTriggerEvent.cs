using System;
using System.Collections.Generic;
using System.Text;

using PMU.Core;

namespace DataManager.Players
{
    public class PlayerDataTriggerEvent
    {
        public ListPair<string, string> Items { get; set; }

        public PlayerDataTriggerEvent() {
            Items = new ListPair<string, string>();
        }
    }
}

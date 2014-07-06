using System;
using System.Collections.Generic;
using System.Text;

using PMU.Sockets;

namespace Server.Shops
{
    public class ShopItem : ISendable
    {
        public int GetItem { get; set; }
    	 public int GiveItem { get; set; }
        public int GiveValue { get; set; }

        //public int GetValue { get; set; }

        public void AppendToPacket(IPacket packet) {

        }
    }
}

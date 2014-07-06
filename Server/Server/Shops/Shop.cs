using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Shops
{
    public class Shop
    {
        public string Name { get; set; }
        public string JoinSay { get; set; }
        public string LeaveSay { get; set; }
        //public bool FixesItems { get; set; }
        public ShopItem[] Items{get;set;}

        public Shop() {
            Items = new ShopItem[Constants.MAX_TRADES];
            for (int i = 0; i < Constants.MAX_TRADES; i++) {
                Items[i] = new ShopItem();
            }
        }
    }
}

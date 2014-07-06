using System;
using System.Collections.Generic;
using System.Text;

namespace Server.DataConverter.Shops.V1
{
	/// <summary>
	/// Description of ShopSection.
	/// </summary>
	public class ShopSection
    {
        public ShopItem[] Items{get;set;}

        public ShopSection() {
            Items = new ShopItem[Constants.MAX_TRADES + 1];
            for (int i = 1; i <= Constants.MAX_TRADES; i++) {
                Items[i] = new ShopItem();
            }
        }
    }
}

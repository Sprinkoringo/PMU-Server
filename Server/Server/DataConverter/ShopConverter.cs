using System;
using System.Collections.Generic;
using System;

namespace Server.DataConverter
{
	/// <summary>
	/// Description of ShopConverter.
	/// </summary>
	public class ShopConverter
	{
		public static void ConvertV1ToV2(int num) {
            DataConverter.Shops.V2.Shop shopV2 = new Server.DataConverter.Shops.V2.Shop();

            DataConverter.Shops.V1.Shop shopV1 = Server.DataConverter.Shops.V1.ShopManager.LoadShop(num);

            shopV2.Name = shopV1.Name;
            shopV2.JoinSay = shopV1.JoinSay;
            shopV2.LeaveSay = shopV1.LeaveSay;
            int n = 0;
            for (int i = 1; i <= 7; i++) {
                for (int z = 1; z <= 66; z++) {
            			if (shopV1.Sections[i].Items[z].GetItem > 0 && n < 100) {
            				shopV2.Items[n].GetItem = shopV1.Sections[i].Items[z].GetItem;
            				shopV2.Items[n].GiveItem = shopV1.Sections[i].Items[z].GiveItem;
            				shopV2.Items[n].GiveValue = shopV1.Sections[i].Items[z].GiveValue;
            				n++;
            			}
                }
            }

            Shops.V2.ShopManager.SaveShop(shopV2, num);
        }
	}
}

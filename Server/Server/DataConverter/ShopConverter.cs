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

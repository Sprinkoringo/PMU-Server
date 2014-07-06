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
using System.Text;

namespace Server.DataConverter.Shops.V1
{
	/// <summary>
	/// Description of ShopManager.
	/// </summary>
	public class ShopManager
	{
		#region Methods

        

        public static Shop LoadShop(int shopNum) {
			Shop shop = new Shop();
            using (System.IO.StreamReader Read = new System.IO.StreamReader(IO.Paths.ShopsFolder + "shop" + shopNum + ".dat")) {
                string[] ShopInfo = Read.ReadLine().Split('|');
                
                string[] info;
                shop.Name = ShopInfo[0];
                shop.JoinSay = ShopInfo[1];
                shop.LeaveSay = ShopInfo[2];
                shop.FixesItems = ShopInfo[3].ToBool();
                for (int i = 1; i <= 7; i++) {
                    for (int z = 1; z <= 66; z++) {
                        info = Read.ReadLine().Split('|');
                        shop.Sections[i].Items[z].GetItem = info[0].ToInt();
                        shop.Sections[i].Items[z].GetValue = info[1].ToInt();
                        shop.Sections[i].Items[z].GiveItem = info[2].ToInt();
                        shop.Sections[i].Items[z].GiveValue = info[3].ToInt();
                    }
                }
            }
			return shop;
        }
        

        public static void SaveShop(Shop shop, int shopNum) {
            
            using (System.IO.StreamWriter Write = new System.IO.StreamWriter(IO.Paths.ShopsFolder + "shop" + shopNum + ".dat")) {
                Write.WriteLine(shop.Name + "|" + shop.JoinSay + "|" + shop.LeaveSay + "|" + shop.FixesItems + "|");
                for (int i = 1; i <= 7; i++) {
                    for (int z = 1; z <= 66; z++) {
                        Write.WriteLine(shop.Sections[i].Items[z].GetItem + "|" + shop.Sections[i].Items[z].GetValue + "|" + shop.Sections[i].Items[z].GiveItem + "|" + shop.Sections[i].Items[z].GiveValue);
                    }
                }
            }
        }


        #endregion
	}
}

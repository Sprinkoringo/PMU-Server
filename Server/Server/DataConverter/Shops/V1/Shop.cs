using System;
using System.Collections.Generic;
using System.Text;

namespace Server.DataConverter.Shops.V1
{
	/// <summary>
	/// Description of Shop.
	/// </summary>
	public class Shop
    {
        public string Name { get; set; }
        public string JoinSay { get; set; }
        public string LeaveSay { get; set; }
        public bool FixesItems { get; set; }
        public ShopSection[] Sections { get; set; }

        public Shop() {
            Sections = new ShopSection[8];
            for (int i = 1; i < 8; i++) {
                Sections[i] = new ShopSection();
            }
        }
    }
}

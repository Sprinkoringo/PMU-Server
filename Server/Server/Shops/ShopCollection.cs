using System;
using System.Collections.Generic;
using System.Text;
using PMU.Core;

namespace Server.Shops
{
    public class ShopCollection
    {
        #region Fields

        ListPair<int, Shop> shops;
        int maxShops;

        #endregion Fields

        #region Constructors

        public ShopCollection(int maxShops) {
            if (maxShops == 0)
                maxShops = 50;
            this.maxShops = maxShops;
            shops = new ListPair<int, Shop>();
        }

        #endregion Constructors

        #region Properties

        public ListPair<int, Shop> Shops {
            get { return shops; }
        }

        public int MaxShops {
            get { return maxShops; }
        }

        #endregion Properties

        #region Indexers

        public Shop this[int index] {
            get { return shops[index]; }
            set { shops[index] = value; }
        }

        #endregion Indexers
    }
}

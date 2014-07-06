namespace Server.Items
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using PMU.Core;

    public class ItemCollection
    {
        #region Fields

        ListPair<int, Item> items;
        int maxItems;

        #endregion Fields

        #region Constructors

        public ItemCollection(int maxItems)
        {
            if (maxItems == 0)
                maxItems = 50;
            this.maxItems = maxItems;
            items = new ListPair<int, Item>();
        }

        #endregion Constructors

        #region Properties

        public ListPair<int, Item> Items
        {
            get { return items; }
        }

        public int MaxItems
        {
            get { return maxItems; }
        }

        #endregion Properties

        #region Indexers

        public Item this[int index]
        {
            get { return items[index]; }
        }

        #endregion Indexers

        #region Methods

        public bool ContainsItem(int itemNum)
        {
            return items.ContainsKey(itemNum);
        }

        #endregion Methods
    }
}
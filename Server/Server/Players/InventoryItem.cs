using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Characters = DataManager.Characters;
namespace Server.Players
{
    public class InventoryItem
    {
        Characters.InventoryItem baseInventoryItem;

        bool updated = false;

        public Characters.InventoryItem BaseInventoryItem {
            get { return baseInventoryItem; }
        }

        public InventoryItem() {
            this.baseInventoryItem = new Characters.InventoryItem();
        }

        public InventoryItem(Characters.InventoryItem baseInventoryItem) {
            this.baseInventoryItem = baseInventoryItem;
        }

        public int Num {
            get { return baseInventoryItem.Num; }
            set {
                baseInventoryItem.Num = value;
                updated = true;
            }
        }

        public int Amount {
            get { return baseInventoryItem.Amount; }
            set { 
                baseInventoryItem.Amount = value;
                updated = true;
            }
        }

        public bool Sticky {
            get { return baseInventoryItem.Sticky; }
            set {
                baseInventoryItem.Sticky = value;
                updated = true;
            }
        }

        public string Tag {
            get { return baseInventoryItem.Tag; }
            set {
                if (value == null) {
                    value = "";
                }
                baseInventoryItem.Tag = value;

                updated = true;
            }
        }

        public bool Updated {
            get { return updated; }
            internal set {
                updated = value;
            }
        }
    }
}

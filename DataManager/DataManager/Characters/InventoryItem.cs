using System;
using System.Collections.Generic;
using System.Text;

namespace DataManager.Characters
{
    public class InventoryItem
    {
        int num;
        int amount;
        bool sticky;
        string tag;

        public int Num {
            get { return num; }
            set { num = value; }
        }

        public int Amount {
            get { return amount; }
            set { amount = value; }
        }

        public bool Sticky {
            get { return sticky; }
            set { sticky = value; }
        }

        public string Tag {
            get { return tag; }
            set {
                if (value == null) {
                    value = "";
                }
                tag = value; 
            }
        }

        public InventoryItem() {
            Tag = "";
        }
    }
}

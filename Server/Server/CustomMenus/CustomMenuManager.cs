using System;
using System.Collections.Generic;
using System.Text;
using Server.Scripting;
using Server.Network;

namespace Server.CustomMenus
{
    public class CustomMenuManager
    {
        Client Client;
        Dictionary<string, CustomMenu> mMenus;

        internal CustomMenuManager(Client client) {
            Client = client;
            mMenus = new Dictionary<string, CustomMenu>();
        }

        public CustomMenu CreateMenu(string menuName, string backgroundImagePath, bool closeable) {
            return new CustomMenu(menuName, backgroundImagePath, closeable);
        }

        public void AddMenu(CustomMenu menuToAdd) {
            if (mMenus.ContainsKey(menuToAdd.MenuName) == false) {
                mMenus.Add(menuToAdd.MenuName, menuToAdd);
            }
        }

        public void SendMenu(string menuName) {
            if (mMenus.ContainsKey(menuName)) {
                mMenus[menuName].SendMenuTo(Client);
            }
        }

        public bool IsMenuOpen(string menuName) {
            return mMenus.ContainsKey(menuName);
        }

        // TODO: Add subs to scripts
        internal void ProcessTCP(string[] parse) {
            if (IsMenuOpen(parse[1])) {
                switch (parse[0].ToLower()) {
                    case "picclick":
                        ScriptManager.InvokeSub("MenuPicClicked", Client, parse[1], parse[2].ToInt());
                        break;
                    case "lblclick":
                        ScriptManager.InvokeSub("MenuLblClicked", Client, parse[1], parse[2].ToInt());
                        break;
                    case "txtclick":
                        ScriptManager.InvokeSub("MenuTxtClicked", Client, parse[1], parse[2].ToInt(), parse[3]);
                        break;
                    case "menuclosed":
                        ScriptManager.InvokeSub("MenuClosed", Client, parse[1]);
                        mMenus.Remove(parse[1]);
                        break;
                }
            }
        }
    }
}

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

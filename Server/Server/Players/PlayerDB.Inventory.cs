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
using System.Linq;
using System.Text;
using Server.Network;
using Characters = DataManager.Characters;
using DataManager.Players;
using Server.Missions;
using Server.Items;
using Server.Maps;
using PMU.Sockets;
using Server.Database;

namespace Server.Players {
    public partial class Player {
        public bool IsInvFull() {
            if (FindInvSlot(-1) == -1) {
                return true;
            } else {
                return false;
            }
        }

        //public bool IsEquiptable(int itemNum) {
        //    if (ItemManager.Items.ContainsItem(itemNum)) {
        //        switch (ItemManager.Items[itemNum].Type) {
        //            case Enums.ItemType.Armor:
        //            case Enums.ItemType.Weapon:
        //            case Enums.ItemType.Helmet:
        //            case Enums.ItemType.Shield:
        //            case Enums.ItemType.Legs:
        //            case Enums.ItemType.Ring:
        //            case Enums.ItemType.Necklace:
        //                return true;
        //            default:
        //                return false;
        //        }
        //    } else
        //        return false;
        //}

        public void CheckAllHeldItems() {
            for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
                if (team[i].Loaded) {
                    team[i].CheckHeldItem();
                }
            }
        }

        public int GetItemSlotHolder(int itemSlot) {
            for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
                if (team[i].Loaded && team[i].HeldItemSlot == itemSlot) {
                    return i;
                }
            }

            return -1;
        }

        public bool HasItemHeld(int itemNum) {
            return HasItemHeld(itemNum, false);
        }

        public bool HasItemHeld(int itemNum, bool stickyCheck) {
            for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
                if (team[i].Loaded && team[i].HeldItem != null && team[i].HeldItem.Num == itemNum && (!stickyCheck || !team[i].HeldItem.Sticky)) {
                    return true;
                }
            }

            return false;
        }

        public bool HasItemHeldBy(int itemNum, Enums.PokemonType type) {
            for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
                if (team[i] != null && team[i].HeldItem != null && team[i].HeldItem.Num == itemNum && (team[i].Type1 == type || team[i].Type2 == type)) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Takes an item from the inventory.
        /// </summary>
        /// <param name="itemnum">The item num.</param>
        /// <param name="amount">Amount to take.</param>
        /// <returns><c>true</c> if the item was successfully taken, otherwise <c>false</c></returns>
        public bool TakeItem(int itemnum, int amount) {
            return TakeItem(itemnum, amount, true);
        }

        /// <summary>
        /// Takes an item from the inventory.
        /// </summary>
        /// <param name="itemNum">The item num.</param>
        /// <param name="amount">Amount to take.</param>
        /// <param name="ignoreSticky">if set to <c>true</c>, the item will be taken even if it is sticky.</param>
        /// <returns><c>true</c> if the item was successfully taken, otherwise <c>false</c></returns>
        public bool TakeItem(int itemNum, int amount, bool ignoreSticky) {
            // Check for subscript out of range
            if (itemNum < 0 || itemNum >= ItemManager.Items.MaxItems) {
                return false;
            }

            for (int i = 1; i <= Inventory.Count; i++) {
                if (Inventory.ContainsKey(i)) {
                    // Check to see if the player has the item
                    if (Inventory[i].Num == itemNum) {
                        return TakeItemSlot(i, amount, ignoreSticky);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Takes the item slot that is in the specified inventory slot.
        /// </summary>
        /// <param name="slot">The slot.</param>
        /// <param name="amount">Amount to take.</param>
        /// <param name="ignoreSticky">if set to <c>true</c>, the item will be taken even if it is sticky.</param>
        /// <returns><c>true</c> if the item was successfully taken, otherwise <c>false</c></returns>
        public bool TakeItemSlot(int slot, int amount, bool ignoreSticky) {
            if (amount < 0) return false;
            bool result = false;
            if (slot < 1 || slot > MaxInv) {
                return false;
            }
            if (Inventory.ContainsKey(slot) && Inventory[slot].Num > 0) {
                bool TakeItem = false;
                int itemNum = Inventory[slot].Num;
                if (ignoreSticky || !Inventory[slot].Sticky) {
                    if (ItemManager.Items[itemNum].Type == Enums.ItemType.Currency || ItemManager.Items[itemNum].StackCap > 0) {
                        // Is what we are trying to take away more then what they have?  If so just set it to zero
                        if (amount >= Inventory[slot].Amount) {
                            TakeItem = true;
                        } else {
                            Inventory[slot].Amount = Inventory[slot].Amount - amount;
                            Messenger.SendInventoryUpdate(client, slot);

                            result = true;
                        }
                    } else {


                        TakeItem = true;

                        //n = ItemManager.Items[Inventory[i].Num].Type;
                        // Check if its not an equipable weapon, and if it isn't then take it away
                        //if ((n != Enums.ItemType.Weapon) && (n != Enums.ItemType.Armor) && (n != Enums.ItemType.Helmet) && (n != Enums.ItemType.Shield) && (n != Enums.ItemType.Legs) & (n != Enums.ItemType.Ring) & (n != Enums.ItemType.Necklace)) {
                        //    TakeItem = true;
                        //}
                    }

                    if (TakeItem == true) {
                        result = true;

                        // Check to see if its held by anyone

                        if (GetItemSlotHolder(slot) > -1)//remove from all equippers
                        {
                            RemoveItem(slot, true, false);
                            Messenger.SendWornEquipment(client);
                        }

                        Inventory[slot].Num = 0;
                        Inventory[slot].Amount = 0;
                        Inventory[slot].Sticky = false;
                        Inventory[slot].Tag = "";
                        //mInventory[i].Dur = 0;

                        //check if it's held-in-bag, and if it was the last functional copy
                        if (ItemManager.Items[itemNum].Type == Enums.ItemType.HeldInBag && HasItem(itemNum, true) == 0) {
                            for (int j = 0; j < Constants.MAX_ACTIVETEAM; j++) {
                                if (team[j].Loaded) {
                                    //the function auto-checks if it's in the activeItemlist to begin with
                                    team[j].RemoveFromActiveItemList(itemNum);
                                }

                            }
                        }

                        // Send the inventory update
                        Messenger.SendInventoryUpdate(client, slot);
                    }
                }
            }
            return result;
        }

        public bool GiveItem(int itemNum, int amount) {
            // Check for subscript out of range
            return GiveItem(itemNum, amount, false);
        }

        public bool GiveItem(int itemNum, int amount, string tag) {
            // Check for subscript out of range
            return GiveItem(itemNum, amount, tag, false);
        }

        public bool GiveItem(int itemNum, int amount, bool sticky) {
            // Check for subscript out of range
            return GiveItem(itemNum, amount, "", sticky);
        }

        public bool GiveItem(int itemNum, int amount, string tag, bool sticky) {
            bool result = false;
            // Check for subscript out of range
            if (itemNum < 0 || itemNum >= ItemManager.Items.MaxItems) {
                return false;
            }
            int i = FindInvSlot(itemNum, amount);
            if (i == -1) {
                Messenger.PlayerMsg(client, "Your inventory is full.", Text.BrightRed);
                result = false;
            } else {
                Inventory[i].Num = itemNum;
                if (ItemManager.Items[itemNum].StackCap > 0 || ItemManager.Items[itemNum].Type == Enums.ItemType.Currency) {
                    Inventory[i].Amount += amount;
                    if (Inventory[i].Amount > ItemManager.Items[itemNum].StackCap) {
                        Inventory[i].Amount = ItemManager.Items[itemNum].StackCap;
                    }
                } else {
                    Inventory[i].Amount = 0;
                }

                Inventory[i].Sticky = sticky;
                Inventory[i].Tag = tag;

                if (ItemManager.Items[itemNum].Type == Enums.ItemType.HeldInBag && !sticky) {
                    for (int j = 0; j < Constants.MAX_ACTIVETEAM; j++) {
                        if (team[j].Loaded && team[j].MeetsReqs(itemNum)) {
                            //the function auto-checks if it's in the activeItemlist to begin with
                            team[j].AddToActiveItemList(itemNum);
                        }

                    }
                }
                Messenger.SendInventoryUpdate(client, i);

                result = true;
            }
            return result;
        }

        public int FindInvSlot(int itemNum) {
            return FindInvSlot(itemNum, 1);
        }

        public int FindInvSlot(int itemNum, int amount) {
            
            
            if (itemNum != -1) {
                if (ItemManager.Items[itemNum].StackCap > 0 || ItemManager.Items[itemNum].Type == Enums.ItemType.Currency) {
                    if (amount > ItemManager.Items[itemNum].StackCap) {
                        return -1;
                    }
                }

                if (ItemManager.Items[itemNum].StackCap > 0 || ItemManager.Items[itemNum].Type == Enums.ItemType.Currency) {
                    for (int i = 1; i <= Inventory.Count; i++) {
                        if (Inventory[i].Num == itemNum) {
                            if (Inventory[i].Amount + amount > ItemManager.Items[itemNum].StackCap) {
                                return -1;
                            } else {
                                return i;
                            }
                        }
                    }
                }
            }

            //search for any empty space
            for (int i = 1; i <= Inventory.Count; i++) {
                if (Inventory[i].Num == 0) {
                    return i;
                }
            }
            return -1;
        }

        public int FindBankSlot(int itemNum, int amount) {
            
            if (itemNum != -1) {
                if (ItemManager.Items[itemNum].StackCap > 0 || ItemManager.Items[itemNum].Type == Enums.ItemType.Currency) {
                    if (amount > ItemManager.Items[itemNum].StackCap) {
                        return -1;
                    }
                }

                if (ItemManager.Items[itemNum].StackCap > 0 || ItemManager.Items[itemNum].Type == Enums.ItemType.Currency) {
                    for (int i = 1; i <= Bank.Count; i++) {
                        if (Bank[i].Num == itemNum) {
                            if (Bank[i].Amount + amount > ItemManager.Items[itemNum].StackCap) {
                                return -1;
                            } else {
                                return i;
                            }
                            return i;
                        }
                    }
                }
            }

            for (int i = 1; i <= Bank.Count; i++) {
                if (Bank[i].Num == 0) {
                    return i;
                }
            }
            return -1;
        }

        public int HasItem(int itemNum) {
            return HasItem(itemNum, false);
        }

        public int HasItem(int itemNum, bool stickyCheck) {
            if (client.IsPlaying() == false || itemNum < 0 || itemNum >= ItemManager.Items.MaxItems) {
                return 0;
            }

            for (int i = 1; i <= Inventory.Count; i++) {
                if (Inventory[i].Num == itemNum && (!stickyCheck || !Inventory[i].Sticky)) {
                    if (ItemManager.Items[Inventory[i].Num].Type == Enums.ItemType.Currency || ItemManager.Items[Inventory[i].Num].StackCap > 0) {
                        return Inventory[i].Amount;
                    } else {
                        return 1;
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// Only for the active recruit
        /// </summary>
        /// <param name="invNum"></param>
        public void HoldItem(int invNum) {
            int ownItem = -1;
            if (GetItemSlotHolder(invNum) > -1) {
                if (Inventory[invNum].Sticky) {
                    Messenger.PlayerMsg(client, "The " + ItemManager.Items[Inventory[invNum].Num].Name + " is all sticky!  You can't get it off!", Text.BrightRed);
                    return;
                } else {
                    RemoveItem(invNum, false, false);
                }
            }

            //remove any item the active recruit may already be holding
            if (GetActiveRecruit().HeldItemSlot != -1) {
                //make sure the already held item isn't sticky
                if (GetActiveRecruit().HeldItem.Sticky) {
                    Messenger.PlayerMsg(client, "The " + ItemManager.Items[GetActiveRecruit().HeldItem.Num].Name + " is all sticky!  You can't get it off!", Text.BrightRed);
                    return;
                } else {
                    ownItem = GetActiveRecruit().HeldItemSlot;
                    RemoveItem(GetActiveRecruit().HeldItemSlot, false, false);

                }
            }
            GetActiveRecruit().HeldItemSlot = invNum;

            if (!Inventory[invNum].Sticky) {
                int itemNum = Inventory[invNum].Num;

                if (ItemManager.Items[itemNum].Type == Enums.ItemType.Held && GetActiveRecruit().MeetsReqs(itemNum)) {
                    GetActiveRecruit().AddToActiveItemList(itemNum);
                }

                if (ItemManager.Items[itemNum].Type == Enums.ItemType.HeldByParty) {
                    for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
                        if (Team[i] != null && Team[i].MeetsReqs(itemNum)) {
                            Team[i].AddToActiveItemList(itemNum);
                        }
                    }
                }
            }
            Messenger.SendWornEquipment(client);
            if (ownItem != -1) {
                Messenger.SendInventoryUpdate(client, ownItem);
            }
            Messenger.SendInventoryUpdate(client, invNum);
        }

        public bool RemoveItem(int invNum) {
            return RemoveItem(invNum, false);
        }

        public bool RemoveItem(int invNum, bool ignoreSticky) {
            return RemoveItem(invNum, ignoreSticky, true);
        }

        /// <summary>
        /// Has an extra check to dequip item from EVERY recruit, just in case multiple recruits somehow equipped the same invNum
        /// </summary>
        /// <param name="invNum"></param>
        /// <param name="ignoreSticky"></param>
        /// <param name="updateItem"></param>
        public bool RemoveItem(int invNum, bool ignoreSticky, bool updateItem) {
            // Prevent hacking
            if (invNum < 1 || invNum > MaxInv) {
                Messenger.HackingAttempt(client, "Invalid InvNum");
                return false;
            }

            if (Inventory[invNum].Sticky && !ignoreSticky) {
                Messenger.PlayerMsg(client, "The " + ItemManager.Items[Inventory[invNum].Num].Name + " is all sticky!  You can't get it off!", Text.BrightRed);
                return false;
            }


            bool removed = false;
            for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
                if (Team[i] != null && Team[i].HeldItemSlot == invNum) {

                    removed = true;
                    Team[i].HeldItemSlot = -1;
                    int itemNum = Inventory[invNum].Num;

                    if (ItemManager.Items[itemNum].Type == Enums.ItemType.Held) {
                        Team[i].RemoveFromActiveItemList(itemNum);
                    }

                    if (ItemManager.Items[itemNum].Type == Enums.ItemType.HeldByParty && !HasItemHeld(itemNum, true)) {
                        for (int j = 0; j < Constants.MAX_ACTIVETEAM; j++) {
                            if (Team[j] != null) {
                                Team[j].RemoveFromActiveItemList(itemNum);
                            }
                        }
                    }

                }
            }
            if (updateItem && removed) {
                Messenger.SendWornEquipment(client);
                Messenger.SendInventoryUpdate(client, invNum);
            }
            return removed;
        }

        public void SetItemSticky(int invNum, bool sticky) {
            // Prevent hacking
            if (invNum < 1 || invNum > MaxInv) {
                Messenger.HackingAttempt(client, "Invalid InvNum");
                return;
            }

            if (sticky && !Inventory[invNum].Sticky) {
                Inventory[invNum].Sticky = true;

                int itemNum = Inventory[invNum].Num;
                //held item
                if (ItemManager.Items[itemNum].Type == Enums.ItemType.Held && GetItemSlotHolder(invNum) > -1) {
                    Team[GetItemSlotHolder(invNum)].RemoveFromActiveItemList(itemNum);
                }

                //held-in-party, check for remaining functional copies
                if (ItemManager.Items[itemNum].Type == Enums.ItemType.HeldByParty) {
                    if (!HasItemHeld(itemNum, true)) {
                        for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
                            Team[i].RemoveFromActiveItemList(itemNum);
                        }
                    }
                }

                //held-in-bag item, check for remaining functional copies
                if (ItemManager.Items[itemNum].Type == Enums.ItemType.HeldInBag && HasItem(itemNum, true) == 0) {
                    for (int j = 0; j < Constants.MAX_ACTIVETEAM; j++) {
                        //the following function auto-checks if it's in the activeItemlist to begin with
                        Team[j].RemoveFromActiveItemList(itemNum);
                    }
                }

            } else if (!sticky && Inventory[invNum].Sticky) { //cleanse
                Inventory[invNum].Sticky = false;
                int itemNum = Inventory[invNum].Num;
                //held item
                if (ItemManager.Items[itemNum].Type == Enums.ItemType.Held && GetItemSlotHolder(invNum) > -1) {
                    if (Team[GetItemSlotHolder(invNum)].MeetsReqs(itemNum)) {
                        Team[GetItemSlotHolder(invNum)].AddToActiveItemList(itemNum);
                    }
                }

                //held-in-party
                if (ItemManager.Items[itemNum].Type == Enums.ItemType.HeldByParty && GetItemSlotHolder(invNum) > -1) {
                    for (int i = 0; i < Constants.MAX_ACTIVETEAM; i++) {
                        if (Team[i] != null && Team[i].MeetsReqs(itemNum)) {
                            Team[i].AddToActiveItemList(itemNum);
                        }
                    }
                }

                //held-in-bag item
                if (ItemManager.Items[itemNum].Type == Enums.ItemType.HeldInBag) {
                    for (int j = 0; j < Constants.MAX_ACTIVETEAM; j++) {
                        if (Team[j] != null && Team[j].MeetsReqs(itemNum)) {
                            //the following function auto-checks if it's in the activeItemlist to begin with
                            Team[j].AddToActiveItemList(itemNum);
                        }
                    }
                }
            }

            Messenger.SendInventoryUpdate(client, invNum);
        }

        public void UseItem(InventoryItem item, int invNum) {
            if (Dead) {
                return;
            }

            Combat.BattleSetup setup = new Combat.BattleSetup();

            setup.Attacker = GetActiveRecruit();

            Combat.BattleProcessor.HandleItemUse(item, invNum, setup);

            Combat.BattleProcessor.FinalizeAction(setup);
        }

        public void ThrowItem(InventoryItem item, int invNum) {

            Combat.BattleSetup setup = new Combat.BattleSetup();

            setup.Attacker = GetActiveRecruit();

            Scripting.ScriptManager.InvokeSub("ThrewItem", setup, item, invNum);

            Combat.BattleProcessor.FinalizeAction(setup);
        }

        public void SetMaxInv(DatabaseConnection dbConnection, int max) {
            SetMaxInv(dbConnection, max, true);
        }

        public void SetMaxInv(DatabaseConnection dbConnection, int max, bool saveInv) {
            MaxInv = max;
            for (int i = 1; i <= MaxInv; i++) {
                if (!Inventory.ContainsKey(i)) {
                    Characters.InventoryItem invItem = new Characters.InventoryItem();
                    playerData.Inventory.Add(i, invItem);
                    Inventory.Add(i, new InventoryItem(invItem));
                }
            }
            Messenger.SendInventory(client);
            //really that easy?

            if (saveInv) {
                PlayerDataManager.SavePlayerInventory(dbConnection.Database, playerData);
            }
        }

        public bool TakeBankItem(int slot, int amount) {
            //int i = 0;
            //int n = 0;
            bool TakeBankItem = false;
            bool result = false;

            // Check for subscript out of range
            if (slot <= 0 || slot > MaxBank) {
                return false;
            }

            //for (i = 1; i <= MaxBank; i++) {
            // Check to see if the player has the item
            //    if (Bank[i].Num == itemNum) {
            if (ItemManager.Items[Bank[slot].Num].Type == Enums.ItemType.Currency || ItemManager.Items[Bank[slot].Num].StackCap > 0) {
                // Is what we are trying to take away more then what they have? If so just set it to zero
                if (amount >= Bank[slot].Amount) {
                    TakeBankItem = true;
                } else {
                    Bank[slot].Amount -= amount;
                    Messenger.SendBankUpdate(client, slot);
                    result = true;
                }
            } else {
                // Check to see if its any sort of ArmorSlot/WeaponSlot ~unneeded for takebankitem
                // Check if its not an equipable weapon, and if it isn't then take it away ~ irrelevant
                TakeBankItem = true;
            }

            if (TakeBankItem == true) {
                Bank[slot].Num = 0;
                Bank[slot].Amount = 0;
                Bank[slot].Sticky = false;
                Bank[slot].Tag = "";

                // Send the Bank update
                Messenger.SendBankUpdate(client, slot);
                result = true;

            }
            //    }
            //}


            return result;
        }

        public bool GiveBankItem(int itemNum, int amount, string tag, int bankSlot) {
            // Check for subscript out of range
            if (itemNum < 0 || itemNum >= ItemManager.Items.MaxItems || bankSlot > Bank.Count) {
                return false;
            }

            // Check to see if Bankentory is full
            if (bankSlot != -1) {
                Bank[bankSlot].Num = itemNum;
                Bank[bankSlot].Amount += amount;
                Bank[bankSlot].Tag = tag;

                return true;
            } else {
                Messenger.StorageMessage(client, "Storage full!");

                return false;
            }
        }

        public void SetMaxBank(DatabaseConnection dbConnection, int max) {
            SetMaxBank(dbConnection, max, true);
        }

        public void SetMaxBank(DatabaseConnection dbConnection, int max, bool saveBank) {
            MaxBank = max;
            for (int i = 1; i <= MaxBank; i++) {
                if (!Bank.ContainsKey(i)) {
                    Characters.InventoryItem invItem = new Characters.InventoryItem();
                    playerData.Bank.Add(i, invItem);
                    Bank.Add(i, new InventoryItem(invItem));
                }
            }
            //no need to send bank
            //really that easy?

            if (saveBank) {
                PlayerDataManager.SavePlayerBank(dbConnection.Database, playerData);
            }
        }
    }
}

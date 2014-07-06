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
using PMU.DatabaseConnector.MySql;
using PMU.DatabaseConnector;

namespace DataManager.Players
{
    public class PlayerFriendsList
    {
        public static readonly bool UpdateOnDemand = true;

        List<string> friends;
        string ownerCharID;
        bool loaded;

        public bool Loaded {
            get { return loaded; }
            internal set {
                loaded = value;
            }
        }

        public PlayerFriendsList(string ownerCharID) {
            this.ownerCharID = ownerCharID;
            friends = new List<string>();
        }

        public int Count {
            get { return friends.Count; }
        }

        /// <summary>
        /// Adds a new friend. Return codes are as follows:
        /// <list type="table">
        /// <item>
        /// 0 - Success
        /// </item>
        /// <item>
        /// 1 - Friend is already on list
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="name"></param>
        public int AddFriend(MySql database, string name) {
            int error = 0;
            if (friends.Contains(name) == false) {
                QuickAdd(name);

                if (UpdateOnDemand && database != null) {
                    database.UpdateOrInsert("friends", new IDataColumn[] {
                        database.CreateColumn(false, "CharID", ownerCharID),
                        database.CreateColumn(false, "FriendListSlot", (friends.Count - 1).ToString()),
                        database.CreateColumn(false, "FriendName", name)
                    });
                }
            } else {
                error = 1;
            }
            return error;
        }

        /// <summary>
        /// Removes a friend based on name. Return codes are as follows:
        /// <list type="number">
        /// <item>
        /// 0 - Success
        /// </item>
        /// <item>
        /// 1 - Friend not found
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int RemoveFriend(MySql database, string name) {
            int error = 0;
            int friendIndex = friends.IndexOf(name);
            if (friendIndex > -1) {
                QuickRemove(friendIndex);

                if (UpdateOnDemand && database != null) {
                    database.DeleteRow("friends", "CharID = \'" + ownerCharID + "\' AND FriendName = \'" + name + "\'");
                }
            } else {
                error = 1;
            }

            return error;
        }

        public bool HasFriend(string name) {
            return friends.Contains(name);
        }

        public void QuickAdd(string name) {
            friends.Add(name);
        }

        public void QuickRemove(int index) {
            friends.RemoveAt(index);
        }

        public string this[int index] {
            get { return friends[index]; }
        }
    }
}

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

namespace Server.Tournaments
{
    public class TournamentMemberCollection : ICloneable
    {
        List<TournamentMember> members;

        internal TournamentMemberCollection(List<TournamentMember> members) {
            this.members = members;
        }

        public TournamentMemberCollection() {
            members = new List<TournamentMember>();
        }

        public void Add(TournamentMember member) {
            if (members.Contains(member) == false) {
                members.Add(member);
            }
        }

        public int Count {
            get { return members.Count; }
        }

        public int IndexOf(string playerID) {
            for (int i = 0; i < members.Count; i++) {
                if (members[i].Client.Player.CharID == playerID) {
                    return i;
                }
            }
            return -1;
        }

        public bool Contains(string playerID) {
            return (IndexOf(playerID) > -1);
        }

        public TournamentMember this[string playerID] {
            get {
                int memberIndex = IndexOf(playerID);
                if (memberIndex > -1) {
                    return members[memberIndex];
                } else {
                    return null;
                }
            }
        }

        public TournamentMember this[Network.Client client] {
            get {
                return this[client.Player.CharID];
            }
        }

        public TournamentMember this[int index] {
            get { return members[index]; }
        }

        public void RemoveAt(int index) {
            if (index > -1) {
                members.RemoveAt(index);
            }
        }

        public void Remove(Network.Client client) {
            RemoveAt(IndexOf(client.Player.CharID));
        }

        public void Remove(TournamentMember member) {
            RemoveAt(IndexOf(member.Client.Player.CharID));
        }

        public object Clone() {
            TournamentMemberCollection memberCollection = new TournamentMemberCollection();
            for (int i = 0; i < members.Count; i++) {
                memberCollection.Add(members[i]);
            }
            return memberCollection;
        }
    }
}

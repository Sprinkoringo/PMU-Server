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
using PMU.Core;
using PMU.Sockets.Tcp;
using System.Threading;

namespace Server.Network
{
    public class ClientManager
    {
        static ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();
        static Dictionary<TcpClientIdentifier, Client> clients;

        public static void Initialize() {
            clients = new Dictionary<TcpClientIdentifier, Client>();
        }

        public static int CountActiveClients() {
            rwLock.EnterReadLock();
            try {
                return clients.Count;
            } finally {
                rwLock.ExitReadLock();
            }
        }

        public static void AddNewClient(TcpClientIdentifier clientID, TcpClient tcpClient) {
            Client client = new Client(clientID, tcpClient);

            rwLock.EnterWriteLock();
            try {
                clients.Add(clientID, client);
            } finally {
                rwLock.ExitWriteLock();
            }
        }

        public static void RemoveClient(Client client) {
            rwLock.EnterWriteLock();
            try {
                clients.Remove(client.TcpID);
            } finally {
                rwLock.ExitWriteLock();
            }
        }

        public static Client GetClient(TcpClientIdentifier clientID) {
            rwLock.EnterReadLock();
            try {
                Client client; 

                if (clients.TryGetValue(clientID, out client)) {
                    return client;
                } else {
                    return null;
                }
            } finally {
                rwLock.ExitReadLock();
            }
        }

        public static IEnumerable<Client> GetClients() {
            Client[] clientsCopy;

            rwLock.EnterReadLock();
            try {
                clientsCopy = clients.Values.ToArray();
            } finally {
                rwLock.ExitReadLock();
            }

            for (int i = 0; i < clientsCopy.Length; i++) {
                Client client = clientsCopy[i];
                if (client.IsPlaying()) {
                    yield return client;
                }
            }
        }

        public static IEnumerable<Client> GetAllClients() {
            Client[] clientsCopy;

            rwLock.EnterReadLock();
            try {
                clientsCopy = clients.Values.ToArray();
            } finally {
                rwLock.ExitReadLock();
            }

            for (int i = 0; i < clientsCopy.Length; i++) {
                yield return clientsCopy[i];
            }
        }


        public static Client FindClientFromCharID(string charID) {
            foreach (Client i in GetClients()) {
                if (i.IsPlaying()) {
                    if (i.Player.CharID == charID) {
                        return i;
                    }
                }
            }

            return null;
        }

        public static Client FindClient(string playerName) {
            return FindClient(playerName, false);
        }

        public static Client FindClient(string playerName, bool autofill) {
            foreach (Client i in GetClients()) {
                if (i.IsPlaying()) {
                    if (autofill) {
                        if (i.Player.Name.StartsWith(playerName) || i.Player.Name.ToLower().StartsWith(playerName.ToLower())) {
                            return i;
                        }
                    } else {
                        if (i.Player.Name.ToLower() == playerName.ToLower()) {
                            return i;
                        }
                    }

                }
            }

            return null;
        }

        public static bool CanLogin(string accountName) {
            //return true;
#if DEBUG
            return true;
#endif
            foreach (Client i in GetClients()) {
                if (i.Player.Loaded) {
                    if (i.Player.AccountName == accountName && i.Player.LoggingOut == true) {
                        return false;
                    }
                    if (i.Player.AccountName == accountName && i.TcpClient.Socket.Connected) {
                        Messenger.SendHeartBeat(i);
                        if (i.TcpClient.Socket.Connected) {
                            return false;
                        } else {
                            i.CloseConnection();
                            return true;
                        }
                    } else if (i.TcpClient.Socket.Connected == false) {
                        Server.Logging.Logger.AppendToLog("/LoginDC.txt", "Player: \'" + i.Player.Name + "\' was stil in the client list, but disconnected. Removed from list.", true);
                        // Just a bit of extra insurance.
                        i.CloseConnection();
                    }
                }
            }
            return true;
        }

        public static bool IsMacAddressConnected(string macAddress) {
#if DEBUG
            return false;
#endif
            if (macAddress == "") return false;
            foreach (Client i in GetClients()) {
                if (i.MacAddress == macAddress && i.TcpClient.Socket.Connected) {
                    return true;
                }
            }
            return false;
        }

        public static bool IsCharacterLoggedIn(string charId) {
            foreach (Client i in GetClients()) {
                if (i.Player != null && i.Player.CharID.Equals(charId, StringComparison.InvariantCultureIgnoreCase)) {
                    return true;
                }
            }
            return false;
        }

        public static bool ContainsClient(Client client) {
            rwLock.EnterReadLock();
            try {
                return clients.ContainsValue(client);
            } finally {
                rwLock.ExitReadLock();
            }
        }
    }
}

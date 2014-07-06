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


namespace Server.Players
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;
    using Server.Network;
    using PMU.Core;
    using System.Threading;

    class PlayerID
    {
        #region Fields

        const string ENCRYPTION_KEY = "sdfs86897y3d2h#45fg313e29";

        static ulong lastAccountIntID = 0;
        static string lastAccountStringID = "a";
        static ulong lastPlayerIntID = 0;
        static string lastPlayerStringID = "a";
        static ListPair<string, TcpClientIdentifier> playerIDToTcpIDList;

        static ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

        #endregion Fields

        #region Properties

        public static string LastAccountID {
            get { return lastAccountStringID + lastAccountIntID.ToString(); }
        }

        public static string LastAccountStringID {
            get { return lastAccountStringID; }
        }

        public static ulong LastAccountUlongID {
            get { return lastAccountIntID; }
        }

        public static string LastPlayerID {
            get { return lastPlayerStringID + lastPlayerIntID.ToString(); }
        }

        public static string LastPlayerStringID {
            get { return lastPlayerStringID; }
        }

        public static ulong LastPlayerUlongID {
            get { return lastPlayerIntID; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Initializes this module
        /// </summary>
        public static void Initialize() {
            playerIDToTcpIDList = new ListPair<string, TcpClientIdentifier>();
        }

        /// <summary>
        /// Finds a connected players index based on their player id
        /// </summary>
        /// <param name="playerID">The id of the player to find</param>
        /// <returns>The players index if found; otherwise, returns -1</returns>
        public static TcpClientIdentifier FindTcpID(string playerID) {
            rwLock.EnterReadLock();
            try {
                int index = playerIDToTcpIDList.IndexOfKey(playerID);
                if (index > -1) {
                    return playerIDToTcpIDList.ValueByIndex(index);
                } else {
                    return null;
                }
            } finally {
                rwLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Adds a player to the index list
        /// </summary>
        /// <param name="playerID">The id of the player to add</param>
        /// <param name="tcpID">The id of the tcp client to add</param>
        public static void AddPlayerToIndexList(string playerID, TcpClientIdentifier tcpID) {
            rwLock.EnterUpgradeableReadLock();
            try {
                int index = playerIDToTcpIDList.IndexOfKey(playerID);
                if (index == -1) {

                    rwLock.EnterWriteLock();
                    try {
                        playerIDToTcpIDList.Add(playerID, tcpID);
                    } finally {
                        rwLock.ExitWriteLock();
                    }

                }
            } finally {
                rwLock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// Removes a player from the index list based on the players id
        /// </summary>
        /// <param name="playerID">The id of the player to remove</param>
        public static void RemovePlayerFromIndexList(string playerID) {
            rwLock.EnterUpgradeableReadLock();
            try {
                int index = playerIDToTcpIDList.IndexOfKey(playerID);
                if (index > -1) {

                    rwLock.EnterWriteLock();
                    try {
                        playerIDToTcpIDList.RemoveAt(index);
                    } finally {
                        rwLock.ExitWriteLock();
                    }

                }
            } finally {
                rwLock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// Removes a player from the index list based on the players index
        /// </summary>
        /// <param name="index">The index of the player to remove</param>
        public static void RemovePlayerFromIndexList(TcpClientIdentifier tcpID) {
            rwLock.EnterWriteLock();
            try {
                int index = playerIDToTcpIDList.IndexOfValue(tcpID);
                if (index > -1) {
                    playerIDToTcpIDList.RemoveAt(index);
                }
            } finally {
                rwLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Generates a new, unique ID used for player accounts
        /// </summary>
        /// <returns></returns>
        public static string GenerateAccountID() {
            if (lastAccountIntID == ulong.MaxValue) {
                lastAccountStringID = IncrementStringID(lastAccountStringID);
                lastAccountIntID = 0;
            } else {
                lastAccountIntID++;
            }
            SaveIDInfo();
            return lastAccountStringID + lastAccountIntID.ToString();
        }

        /// <summary>
        /// Generates a new, unique ID used for player characters
        /// </summary>
        /// <returns></returns>
        public static string GeneratePlayerID() {
            if (lastPlayerIntID == ulong.MaxValue) {
                lastPlayerStringID = IncrementStringID(lastPlayerStringID);
                lastPlayerIntID = 0;
            } else {
                lastPlayerIntID++;
            }
            SaveIDInfo();
            return lastPlayerStringID + lastPlayerIntID.ToString();
        }

        /// <summary>
        /// Increments the string ID by one letter (ie. 'a' becomes 'b', 'b' becomes 'c')
        /// </summary>
        /// <param name="lastID">The ID to increment</param>
        /// <returns>The incremented ID</returns>
        public static string IncrementStringID(string lastID) {
            switch (lastID.Substring(lastID.Length - 1, 1)) {
                case "a":
                    return CombineIDs(lastID, new string('b', 1));
                case "b":
                    return CombineIDs(lastID, new string('c', 1));
                case "c":
                    return CombineIDs(lastID, new string('d', 1));
                case "d":
                    return CombineIDs(lastID, new string('e', 1));
                case "e":
                    return CombineIDs(lastID, new string('f', 1));
                case "f":
                    return CombineIDs(lastID, new string('g', 1));
                case "g":
                    return CombineIDs(lastID, new string('h', 1));
                case "h":
                    return CombineIDs(lastID, new string('i', 1));
                case "i":
                    return CombineIDs(lastID, new string('j', 1));
                case "j":
                    return CombineIDs(lastID, new string('k', 1));
                case "k":
                    return CombineIDs(lastID, new string('l', 1));
                case "l":
                    return CombineIDs(lastID, new string('m', 1));
                case "m":
                    return CombineIDs(lastID, new string('n', 1));
                case "n":
                    return CombineIDs(lastID, new string('o', 1));
                case "o":
                    return CombineIDs(lastID, new string('p', 1));
                case "p":
                    return CombineIDs(lastID, new string('q', 1));
                case "q":
                    return CombineIDs(lastID, new string('r', 1));
                case "r":
                    return CombineIDs(lastID, new string('s', 1));
                case "s":
                    return CombineIDs(lastID, new string('t', 1));
                case "t":
                    return CombineIDs(lastID, new string('u', 1));
                case "u":
                    return CombineIDs(lastID, new string('v', 1));
                case "v":
                    return CombineIDs(lastID, new string('w', 1));
                case "w":
                    return CombineIDs(lastID, new string('x', 1));
                case "x":
                    return CombineIDs(lastID, new string('y', 1));
                case "y":
                    return CombineIDs(lastID, new string('z', 1));
                case "z":
                    return CombineIDs(lastID, new string('a', 1 + 1));
                default:
                    return lastID;
            }
        }

        /// <summary>
        /// Loads the ID settings from the configuration file
        /// </summary>
        public static void LoadIDInfo() {
            // Check if the configuration file exists, if not, create it
            if (IO.IO.FileExists(IO.Paths.DataFolder + "playerid.dat") == false) {
                SaveIDInfo();
            }
            // Decrypt the configuration file into a string
            Security.Encryption crypt = new Security.Encryption(ENCRYPTION_KEY);
            string xmlData = System.Text.Encoding.Unicode.GetString(crypt.DecryptBytes(System.IO.File.ReadAllBytes(IO.Paths.DataFolder + "playerid.dat")));
            using (XmlTextReader reader = new XmlTextReader(new System.IO.StringReader(xmlData))) {
                while (reader.Read()) {
                    if (reader.IsStartElement()) {
                        switch (reader.Name) {
                            case "LastPlayerStringID": {
                                    lastPlayerStringID = reader.ReadString();
                                }
                                break;
                            case "LastPlayerIntID": {
                                    lastPlayerIntID = reader.ReadString().ToUlng();
                                }
                                break;
                            case "LastAccountStringID": {
                                    lastAccountStringID = reader.ReadString();
                                }
                                break;
                            case "LastAccountIntID": {
                                    lastAccountIntID = reader.ReadString().ToUlng();
                                }
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Saves the ID settings to the configuration file
        /// </summary>
        public static void SaveIDInfo() {
            StringBuilder output = new StringBuilder();
            // Write a new xml document to the 'output' StringBuilder
            using (XmlWriter writer = XmlWriter.Create(output, Settings.XmlWriterSettings)) {
                writer.WriteStartDocument();
                // Write the root node
                writer.WriteStartElement("ID");
                // Write the 'PlayerID' node
                writer.WriteStartElement("PlayerID");

                writer.WriteElementString("LastPlayerStringID", lastPlayerStringID);
                writer.WriteElementString("LastPlayerIntID", lastPlayerIntID.ToString());
                // Close the 'PlayerID' node
                writer.WriteEndElement();
                // Write the 'AccountID' node
                writer.WriteStartElement("AccountID");

                writer.WriteElementString("LastAccountStringID", lastAccountStringID);
                writer.WriteElementString("LastAccountIntID", lastAccountIntID.ToString());
                // Close the 'AccountID' node
                writer.WriteEndElement();

                // Close the root node
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            // Encrypt the xml string and write the encrypted value to the file
            Security.Encryption crypt = new Security.Encryption(ENCRYPTION_KEY);
            System.IO.File.WriteAllBytes(IO.Paths.DataFolder + "playerid.dat", crypt.EncryptBytes(System.Text.Encoding.Unicode.GetBytes(output.ToString())));
        }

        private static string CombineIDs(string lastID, string newID) {
            if (lastID.Length - 1 > 0) {
                return lastID.Substring(0, lastID.Length - 1) + newID;
            } else {
                return newID;
            }
        }

        #endregion Methods
    }
}
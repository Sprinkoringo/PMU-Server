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


namespace Server.IO
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Paths
    {
        #region Fields

        static string dataFolder;
        static string itemsFolder;
        static string movesFolder;
        static string npcsFolder;
        static string rdungeonsFolder;
        static string startFolder;
        static string storiesFolder;
        static string shopsFolder;
        static string dungeonsFolder;
        static string logsFolder;
        static string scriptsFolder;

        #endregion Fields

        #region Properties

        public static string DataFolder {
            get { return dataFolder; }
        }

        public static string ItemsFolder {
            get { return itemsFolder; }
        }

        public static string MovesFolder {
            get { return movesFolder; }
        }

        public static string NpcsFolder {
            get { return npcsFolder; }
        }

        public static string RDungeonsFolder {
            get { return rdungeonsFolder; }
        }

        public static string StartFolder {
            get { return startFolder; }
        }

        public static string StoriesFolder {
            get { return storiesFolder; }
        }

        public static string ShopsFolder {
            get { return shopsFolder; }
        }

        public static string DungeonsFolder {
            get { return dungeonsFolder; }
        }

        public static string LogsFolder {
            get { return logsFolder; }
        }

        public static string ScriptsFolder {
            get { return scriptsFolder; }
        }

        public static string ScriptsIOFolder {
            get { return scriptsIOFolder; }
        }

        #endregion Properties

        #region Methods

        static string scriptsIOFolder;

        internal static void Initialize(string startFolder) {
            if (startFolder.EndsWith("\\") == false)
                startFolder += "\\";
            startFolder = System.IO.Path.GetFullPath(startFolder);
            Paths.startFolder = startFolder;
            Paths.dataFolder = IO.ProcessPath(startFolder + "Data\\");
            Paths.itemsFolder = IO.ProcessPath(startFolder + "Items\\");
            Paths.npcsFolder = IO.ProcessPath(startFolder + "Npcs\\");
            Paths.movesFolder = IO.ProcessPath(startFolder + "Moves\\");
            Paths.storiesFolder = IO.ProcessPath(startFolder + "Stories\\");
            Paths.rdungeonsFolder = IO.ProcessPath(startFolder + "RDungeons\\");
            Paths.shopsFolder = IO.ProcessPath(startFolder + "Shops\\");
            Paths.dungeonsFolder = IO.ProcessPath(startFolder + "Dungeons\\");
            Paths.logsFolder = IO.ProcessPath(startFolder + "Logs\\");
            Paths.scriptsFolder = IO.ProcessPath(startFolder + "Scripts\\");
            Paths.scriptsIOFolder = IO.ProcessPath(startFolder + "Scripts\\ScriptIO\\");

            if (!IO.DirectoryExists(Paths.startFolder))
                IO.CreateDirectory(Paths.startFolder);
            if (!IO.DirectoryExists(Paths.dataFolder))
                IO.CreateDirectory(Paths.dataFolder);
            if (!IO.DirectoryExists(Paths.logsFolder))
                IO.CreateDirectory(Paths.logsFolder);
            if (!IO.DirectoryExists(Paths.scriptsFolder))
                IO.CreateDirectory(Paths.scriptsFolder);
            if (!IO.DirectoryExists(Paths.scriptsIOFolder))
                IO.CreateDirectory(Paths.scriptsIOFolder);

            if (!IO.DirectoryExists(Paths.itemsFolder))
                IO.CreateDirectory(Paths.itemsFolder);
            if (!IO.DirectoryExists(Paths.npcsFolder))
                IO.CreateDirectory(Paths.npcsFolder);
            if (!IO.DirectoryExists(Paths.movesFolder))
                IO.CreateDirectory(Paths.movesFolder);
            if (!IO.DirectoryExists(Paths.storiesFolder))
                IO.CreateDirectory(Paths.storiesFolder);
            if (!IO.DirectoryExists(Paths.rdungeonsFolder))
                IO.CreateDirectory(Paths.rdungeonsFolder);
            if (!IO.DirectoryExists(Paths.shopsFolder))
                IO.CreateDirectory(Paths.shopsFolder);
            if (!IO.DirectoryExists(Paths.dungeonsFolder))
                IO.CreateDirectory(Paths.dungeonsFolder);
        }

        #endregion Methods
    }
}
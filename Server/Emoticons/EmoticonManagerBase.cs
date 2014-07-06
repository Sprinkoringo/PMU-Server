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


namespace Server.Emoticons
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;

    public class EmoticonManagerBase
    {
        #region Fields

        static EmoticonCollection emoticons;

        #endregion Fields

        #region Events

        public static event EventHandler LoadComplete;

        public static event EventHandler<LoadingUpdateEventArgs> LoadUpdate;

        #endregion Events

        #region Properties

        public static EmoticonCollection Emoticons {
            get { return emoticons; }
        }

        #endregion Properties

        #region Methods

        public static void CheckEmotions() {
            if (System.IO.File.Exists(IO.Paths.DataFolder + "emoticons.xml") == false) {
                //SaveEmotions();
            }
        }

        public static void Initialize(int maxEmotions) {
            emoticons = new EmoticonCollection(maxEmotions);
            CheckEmotions();
        }

        public static void LoadEmotions(object object1) {
            try {
                using (XmlReader reader = XmlReader.Create(IO.Paths.DataFolder + "emoticons.xml")) {
                    while (reader.Read()) {
                        if (reader.IsStartElement()) {
                            switch (reader.Name) {
                                case "Emoticon": {
                                        string idval = reader["id"];
                                        int id = 0;
                                        if (idval != null) {
                                            id = idval.ToInt();
                                        }
                                        emoticons[id] = new Emoticon();
                                        if (reader.Read()) {
                                            emoticons[id].Pic = reader.ReadElementString("Pic").ToInt();
                                            emoticons[id].Command = reader.ReadElementString("Command");
                                        }
                                        if (LoadUpdate != null)
                                            LoadUpdate(null, new LoadingUpdateEventArgs(id, emoticons.MaxEmoticons));
                                    }
                                    break;
                            }
                        }
                    }
                }
                if (LoadComplete != null)
                    LoadComplete(null, null);
            } catch (Exception ex) {
                Exceptions.ErrorLogger.WriteToErrorLog(ex);
            }
        }

        #endregion Methods
    }
}
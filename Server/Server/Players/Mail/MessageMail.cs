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


namespace Server.Players.Mail
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;

    public class MessageMail// : IMail
    {
        //#region Fields

        //InventoryItem attachedItem;
        //string recieverID;
        //string senderID;
        //string text;
        //string title;

        //#endregion Fields

        //#region Properties

        //public InventoryItem AttachedItem {
        //    get { return attachedItem; }
        //    set { attachedItem = value; }
        //}

        //public string RecieverID {
        //    get { return recieverID; }
        //    set { recieverID = value; }
        //}

        //public string SenderID {
        //    get { return senderID; }
        //    set { senderID = value; }
        //}

        //public string Text {
        //    get { return text; }
        //    set { text = value; }
        //}

        //public string Title {
        //    get { return title; }
        //    set { title = value; }
        //}

        //public MailType Type {
        //    get { return MailType.Message; }
        //}

        //public bool Unread {
        //    get;
        //    set;
        //}

        //#endregion Properties

        //#region Methods

        //public void Load(System.Xml.XmlReader reader) {
        //    while (reader.Read()) {
        //        if (reader.IsStartElement()) {
        //            switch (reader.Name) {
        //                case "SenderID": {
        //                        senderID = reader.ReadString();
        //                    }
        //                    break;
        //                case "RecieverID": {
        //                        recieverID = reader.ReadString();
        //                    }
        //                    break;
        //                case "Title": {
        //                        title = reader.ReadString();
        //                    }
        //                    break;
        //                case "Text": {
        //                        text = reader.ReadString();
        //                    }
        //                    break;
        //                case "InvItem": {
        //                        attachedItem = new InventoryItem();
        //                        using (XmlReader subReader = reader.ReadSubtree()) {
        //                            attachedItem.Load(subReader);
        //                        }
        //                    }
        //                    break;
        //            }
        //        }
        //    }
        //}

        //public void Save(System.Xml.XmlWriter writer) {
        //    writer.WriteElementString("SenderID", senderID);
        //    writer.WriteElementString("RecieverID", recieverID);
        //    writer.WriteElementString("Title", title);
        //    writer.WriteElementString("Text", text);
        //    if (attachedItem != null) {
        //        attachedItem.Save(writer, "InvItem");
        //    }
        //}

        //#endregion Methods
    }
}
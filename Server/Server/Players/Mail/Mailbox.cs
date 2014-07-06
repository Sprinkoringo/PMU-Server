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
using System.Xml;

namespace Server.Players.Mail
{
    public class Mailbox
    {
        //MailCollection mail;
        //int mailLimit;
        //string mailboxPath;

        //public string MailboxPath {
        //    get { return mailboxPath; }
        //}

        //public int MailLimit {
        //    get { return mailLimit; }
        //    set { mailLimit = value; }
        //}

        //public MailCollection Mail {
        //    get { return mail; }
        //}

        //Mailbox(string mailboxPath) {
        //    mail = new MailCollection();
        //    this.mailboxPath = mailboxPath;
        //}

        //public void Save() {
        //    using (XmlWriter writer = XmlWriter.Create(mailboxPath, Settings.XmlWriterSettings)) {
        //        writer.WriteStartDocument();
        //        writer.WriteStartElement("Mailbox");

        //        writer.WriteStartElement("Settings");
        //        writer.WriteElementString("MailLimit", this.mailLimit.ToString());
        //        writer.WriteEndElement();

        //        writer.WriteStartElement("MailboxMail");
        //        for (int i = 0; i < mail.Count; i++) {
        //            writer.WriteStartElement("Mail");
        //            writer.WriteAttributeString("type", mail[i].Type.ToString());
        //            mail[i].Save(writer);
        //            writer.WriteEndElement();
        //        }

        //        writer.WriteEndElement();
        //        writer.WriteEndDocument();
        //    }
        //}

        //public static Mail.Mailbox LoadMailbox(string path) {
        //    Mail.Mailbox mailbox = new Mail.Mailbox(path);
        //    using (XmlReader reader = XmlReader.Create(path)) {
        //        while (reader.Read()) {
        //            if (reader.IsStartElement()) {
        //                switch (reader.Name) {
        //                    case "MailLimit": {
        //                            mailbox.mailLimit = reader.ReadString().ToInt();
        //                        }
        //                        break;
        //                    case "Mail": {
        //                            Mail.IMail mail = Mailer.CreateInstance((Mail.MailType)Enum.Parse(typeof(Mail.MailType), reader["type"], true));
        //                            using (XmlReader subReader = reader.ReadSubtree()) {
        //                                mail.Load(subReader);
        //                            }
        //                            mailbox.Mail.Add(mail);
        //                        }
        //                        break;
        //                }
        //            }
        //        }
        //    }
        //    return mailbox;
        //}
    }
}

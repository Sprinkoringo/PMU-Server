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

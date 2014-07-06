using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Server.Players.Mail
{
    public interface IMail
    {
        string SenderID { get; set; }
        string RecieverID { get; set; }
        MailType Type { get; }
        bool Unread { get; set; }

        void Save(XmlWriter writer);
        void Load(XmlReader reader);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Players.Mail
{
    public class MailCollection
    {
        List<IMail> mail;

        public MailCollection() {
            mail = new List<IMail>();
        }

        public void Add(IMail mail) {
            this.mail.Add(mail);
        }

        public IMail this[int index] {
            get { return mail[index]; }
        }

        public int Count {
            get { return mail.Count; }
        }
    }
}

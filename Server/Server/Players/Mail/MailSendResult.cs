using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Players.Mail
{
    public enum MailSendResult
    {
        Success,
        UserNotFound,
        RecieverMailboxFull
    }
}

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

using Server.Network;
using Server.Players;

namespace Server.Players.Mail
{
    public class Mailer
    {
        //public static IMail CreateInstance(MailType type) {
        //    switch (type) {
        //        case MailType.Message:
        //            return new MessageMail();
        //        default:
        //            return null;
        //    }
        //}

        //public static void SetMailDestinationByPlayerName(string recieverName, IMail mail) {
        //    mail.RecieverID = PlayerManager.RetrieveCharacterID(recieverName);
        //}

        //public static void SetMailPackaging(Client sender, string recieverName, IMail mail) {
        //    mail.SenderID = sender.Player.CharID;
        //    mail.RecieverID = PlayerManager.RetrieveCharacterID(recieverName);
        //}

        //public static MailSendResult SendMail(IMail mail) {
        //    Mailbox recieverMailbox = null;
        //    Client reciever;
        //    // Our goal: create an instance of the reciever's mailbox!
        //    // First, check if the reciever is already online
        //    TcpClientIdentifier tcpID = PlayerID.FindTcpID(mail.RecieverID);
        //    if (tcpID != null) {
        //        // The player is online!
        //        reciever = ClientManager.GetClient(tcpID);
        //        if (reciever != null) {
        //            recieverMailbox = reciever.Player.Mailbox;
        //        }
        //    }
        //    if (recieverMailbox == null) {
        //        // Player wasn't online, lets find them in the big database
        //        CharacterInformation charInfo = PlayerManager.RetrieveCharacterInformation("id", mail.RecieverID);
        //        if (charInfo != null) {
        //            // We found the player 
        //            recieverMailbox = Mailbox.LoadMailbox(charInfo.GetCharacterDirectory() + "Mail/Mailbox.xml");
        //        }
        //    }
        //    // Check if we have the destination mailbox
        //    if (recieverMailbox != null) {
        //        return SendMailInternal(recieverMailbox, mail);
        //    } else {
        //        return MailSendResult.UserNotFound;
        //    }
        //}

        //internal static MailSendResult SendMailInternal(Mailbox recieverMailbox, IMail mail) {
        //    if (recieverMailbox.Mail.Count >= recieverMailbox.MailLimit) {
        //        return MailSendResult.RecieverMailboxFull;
        //    }

        //    recieverMailbox.Mail.Add(mail);
        //    recieverMailbox.Save();

        //    return MailSendResult.Success;
        //}
    }
}

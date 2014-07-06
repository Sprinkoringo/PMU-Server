namespace Server.Emoticons
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using PMU.Sockets;

    public class Emoticon : ISendable
    {
        #region Fields

        string command;
        int pic;

        #endregion Fields

        #region Properties

        public string Command
        {
            get { return command; }
            set { command = value; }
        }

        public int Pic
        {
            get { return pic; }
            set { pic = value; }
        }

        #endregion Properties

        public void AppendToPacket(IPacket packet) {
            packet.AppendParameters(command, pic.ToString());
        }
    }
}
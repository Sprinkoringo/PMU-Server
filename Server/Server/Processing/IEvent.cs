using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Processing
{
    interface IEvent
    {
        string Command { get; }

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class Core
    {
        public static TickCount GetTickCount() {
            return new TickCount(Environment.TickCount);
        }


    }
}

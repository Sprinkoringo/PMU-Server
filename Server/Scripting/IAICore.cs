using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Scripting
{
    public interface IAICore
    {
        //void Init(CNetScript netScript, CObjectFactory objFactory, CIOTools ioTools, CDebug debug);
        void ProcessAI(int tickCount, int mapNum, int mapNpcSlot);
        void ProcessInstancedAI(int tickCount, int playerNum, int mapNpcSlot);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Events.Player.TriggerEvents
{
    public enum TriggerEventTrigger
    {
        Custom = 0,
        MapLoad = 1,
        SteppedOnTile = 2,
        StepCounter = 3
    }
}

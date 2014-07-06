using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Stories;
using Server.Network;

namespace Server.Events.Player.TriggerEvents
{
    public class TriggerEventHelper
    {
        public static void InvokeGenericTrigger(ITriggerEvent triggerEvent, Client client) {
            switch (triggerEvent.Action) {
                case TriggerEventAction.PlayStory: {
                    StoryManager.PlayStory(client, triggerEvent.TriggerCommand);
                    }
                    break;
                case TriggerEventAction.RunScript: {
                    Scripting.ScriptManager.InvokeSub("TriggerEventScript", client, triggerEvent);
                    }
                    break;
            }
        }

        internal static ITriggerEvent CreateTriggerEventInstance(TriggerEventTrigger triggerEvent) {
            switch (triggerEvent) {
                case TriggerEventTrigger.MapLoad:
                    return new MapLoadTriggerEvent();
                case TriggerEventTrigger.SteppedOnTile:
                    return new SteppedOnTileTriggerEvent();
                case TriggerEventTrigger.StepCounter:
                    return new StepCounterTriggerEvent();
                default:
                    return null;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Server.Network;

namespace Server.Events.Player.TriggerEvents
{
    public interface ITriggerEvent
    {
        int TriggerCommand { get; }
        string ID { get; }
        TriggerEventTrigger Trigger { get; }
        TriggerEventAction Action { get; }
        bool AutoRemove { get; }
        void InvokeTrigger();
        bool CanInvokeTrigger();
        void Save(DataManager.Players.PlayerDataTriggerEvent triggerEvent);
        void Load(DataManager.Players.PlayerDataTriggerEvent triggerEvent, Client client);
    }
}

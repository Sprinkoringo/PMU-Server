namespace Server.Events.Player.TriggerEvents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Server.Network;
    using System.Xml;

    public class SteppedOnTileTriggerEvent : ITriggerEvent
    {
        #region Fields

        TriggerEventAction action;
        Client client;
        string id;
        string mapID;
        int x;
        int y;
        int triggerCommand;
        bool autoRemove;

        #endregion Fields

        #region Constructors

        internal SteppedOnTileTriggerEvent() {

        }

        public SteppedOnTileTriggerEvent(string id, TriggerEventAction action, int triggerCommand, bool autoRemove,
            Client client, string mapID, int x, int y) {
            this.id = id;
            this.action = action;
            this.triggerCommand = triggerCommand;
            this.autoRemove = autoRemove;
            this.client = client;
            this.mapID = mapID;
            this.x = x;
            this.y = y;
        }

        #endregion Constructors

        #region Properties

        public string ID {
            get { return id; }
        }

        public string MapID {
            get { return mapID; }
        }

        public int X {
            get { return x; }
        }

        public int Y {
            get { return y; }
        }

        public TriggerEventTrigger Trigger {
            get { return TriggerEventTrigger.SteppedOnTile; }
        }

        public int TriggerCommand {
            get { return triggerCommand; }
        }

        public bool AutoRemove {
            get { return autoRemove; }
        }

        public TriggerEventAction Action {
            get { return action; }
        }

        #endregion Properties

        #region Methods

        public bool CanInvokeTrigger() {
            if (client.Player.MapID == mapID && client.Player.X == x && client.Player.Y == y) {
                return true;
            } else {
                return false;
            }
        }

        public void InvokeTrigger() {
            TriggerEventHelper.InvokeGenericTrigger(this, client);
            if (autoRemove) {
                client.Player.RemoveTriggerEvent(this);
            }
        }

        public void Load(DataManager.Players.PlayerDataTriggerEvent triggerEvent, Client client) {
            this.client = client;

            id = triggerEvent.Items.GetValue("ID");
            action = (TriggerEventAction)Enum.Parse(typeof(TriggerEventAction), triggerEvent.Items.GetValue("Action"), true);
            triggerCommand = triggerEvent.Items.GetValue("TriggerCommand").ToInt();
            autoRemove = triggerEvent.Items.GetValue("AutoRemove").ToBool();

            mapID = triggerEvent.Items.GetValue("MapID");
            x = triggerEvent.Items.GetValue("X").ToInt();
            y = triggerEvent.Items.GetValue("Y").ToInt();
        }

        public void Save(DataManager.Players.PlayerDataTriggerEvent triggerEvent) {
            triggerEvent.Items.Clear();

            triggerEvent.Items.Add("Type", ((int)Trigger).ToString());
            triggerEvent.Items.Add("ID", id);
            triggerEvent.Items.Add("Action", ((int)action).ToString());
            triggerEvent.Items.Add("TriggerCommand", triggerCommand.ToString());
            triggerEvent.Items.Add("AutoRemove", autoRemove.ToIntString());

            triggerEvent.Items.Add("MapID", mapID);
            triggerEvent.Items.Add("X", x.ToString());
            triggerEvent.Items.Add("Y", y.ToString());
        }

        #endregion Methods
    }
}
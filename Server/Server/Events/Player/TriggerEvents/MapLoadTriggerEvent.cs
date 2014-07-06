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


namespace Server.Events.Player.TriggerEvents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Server.Network;
    using System.Xml;

    public class MapLoadTriggerEvent : ITriggerEvent
    {
        #region Fields

        TriggerEventAction action;
        Client client;
        string id;
        string mapID;
        int triggerCommand;
        bool autoRemove;

        #endregion Fields

        #region Constructors

        internal MapLoadTriggerEvent() {

        }

        public MapLoadTriggerEvent(string id, TriggerEventAction action, int triggerCommand, bool autoRemove,
            Client client, string mapID) {
            this.id = id;
            this.action = action;
            this.triggerCommand = triggerCommand;
            this.autoRemove = autoRemove;
            this.client = client;
            this.mapID = mapID;
        }

        #endregion Constructors

        #region Properties

        public string ID {
            get { return id; }
        }

        public string MapID {
            get { return mapID; }
        }

        public TriggerEventTrigger Trigger {
            get { return TriggerEventTrigger.MapLoad; }
        }

        public int TriggerCommand {
            get { return triggerCommand; }
        }

        public TriggerEventAction Action {
            get { return action; }
        }

        public bool AutoRemove {
            get { return autoRemove; }
        }

        #endregion Properties

        #region Methods

        public bool CanInvokeTrigger() {
            if (client.Player.MapID == mapID) {
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
        }

        public void Save(DataManager.Players.PlayerDataTriggerEvent triggerEvent) {
            triggerEvent.Items.Clear();

            triggerEvent.Items.Add("Type", ((int)Trigger).ToString());
            triggerEvent.Items.Add("ID", id);
            triggerEvent.Items.Add("Action", ((int)action).ToString());
            triggerEvent.Items.Add("TriggerCommand", triggerCommand.ToString());
            triggerEvent.Items.Add("AutoRemove", autoRemove.ToIntString());

            triggerEvent.Items.Add("MapID", mapID);
        }

        #endregion Methods
    }
}
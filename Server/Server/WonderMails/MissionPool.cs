using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.WonderMails {
    public class MissionPool {

        private List<MissionClientData> missionClients;
        private List<MissionEnemyData> enemies;
        private List<MissionRewardData> rewards;

        public List<MissionClientData> MissionClients {
            get { return missionClients; }
        }

        public List<MissionEnemyData> Enemies {
            get { return enemies; }
        }

        public List<MissionRewardData> Rewards {
            get { return rewards; }
        }

        public MissionPool() {
            missionClients = new List<MissionClientData>();
            enemies = new List<MissionEnemyData>();
            rewards = new List<MissionRewardData>();
        }

    }
}

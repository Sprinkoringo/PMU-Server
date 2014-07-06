using System;
using System.Collections.Generic;
using System.Text;

namespace DataManager.Players
{
    public class PlayerDataTeamMember
    {
        public int RecruitIndex { get; set; }
        public bool UsingTempStats { get; set; }

        public PlayerDataTeamMember() {
            RecruitIndex = -1;
        }
    }
}

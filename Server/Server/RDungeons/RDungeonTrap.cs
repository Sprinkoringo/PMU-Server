using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.RDungeons;
using Server.Maps;

namespace Server.RDungeons
{
    public class RDungeonTrap : Tile
    {
        public int AppearanceRate { get; set; }

        public RDungeonTrap()
            : base(new DataManager.Maps.Tile()) {
        }
    }
}

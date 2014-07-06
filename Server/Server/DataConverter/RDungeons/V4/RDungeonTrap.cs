using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.RDungeons;
using Server.Maps;

namespace Server.DataConverter.RDungeons.V4
{
    public class RDungeonTrap
    {
        public Tile SpecialTile { get; set; }
        public int AppearanceRate { get; set; }

        public RDungeonTrap()
        {
            SpecialTile = new Tile(new DataManager.Maps.Tile());
        }
    }
}

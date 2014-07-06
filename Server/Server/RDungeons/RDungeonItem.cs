using System;
using System.Collections.Generic;
using System.Text;
using Server.Maps;

namespace Server.RDungeons
{
    public class RDungeonItem
    {
        public int ItemNum { get; set; }
    	public int MinAmount { get; set; }
    	public int MaxAmount { get; set; }
    	public int AppearanceRate { get; set; }
    	public int StickyRate { get; set; }
    	public string Tag { get; set; }
    	public bool Hidden { get; set; }
    	public bool OnGround { get; set; }
    	public bool OnWater { get; set; }
    	public bool OnWall { get; set; }
    	
    	public RDungeonItem() {
    		Tag = "";
    	}
    }
}
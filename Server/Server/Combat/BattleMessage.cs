using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;

namespace Server.Combat
{
	/// <summary>
	/// Description of BattleMessage.
	/// </summary>
	public class BattleMessage
	{
		public string Message { get; set;}
		public Color Color { get; set; }
		
		
		
		public BattleMessage(string msg, Color color)
		{
			Message = msg;
			Color = color;
		}
	}
}

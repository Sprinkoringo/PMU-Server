using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Server.CustomMenus
{
    public class MenuLabel
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public string Text { get; set; }
        public string Font { get; set; }
        public int FontSize { get; set; }
        public Color Color { get; set; }
    }
}

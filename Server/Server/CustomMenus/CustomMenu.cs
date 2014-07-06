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


using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Server.Network;
using PMU.Sockets;

namespace Server.CustomMenus
{
    public class CustomMenu
    {
        public string MenuName { get; set; }
        public bool Closeable { get; set; }
        public string ImagePath { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        private Dictionary<int, MenuPicture> mPictures { get; set; }
        private Dictionary<int, MenuLabel> mLabels { get; set; }
        private Dictionary<int, MenuTextBox> mTextBoxs { get; set; }

        internal CustomMenu(string menuName, string backgroundImagePath, bool closeable) {
            MenuName = menuName;
            ImagePath = backgroundImagePath;
            Closeable = closeable;

            mPictures = new Dictionary<int, MenuPicture>();
            mLabels = new Dictionary<int, MenuLabel>();
            mTextBoxs = new Dictionary<int, MenuTextBox>();

            Width = -1;
            Height = -1;
        }

        public void AddPicture(int index, string imagePath, int x, int y) {
            MenuPicture pic = new MenuPicture();
            pic.ImagePath = imagePath;
            pic.X = x;
            pic.Y = y;
            mPictures.Add(index, pic);
        }

        public void AddLabel(int index, int x, int y, int width, int height, string text, string font, int fontSize,
            Color color) {
            MenuLabel lbl = new MenuLabel();
            lbl.X = x;
            lbl.Y = y;
            lbl.Width = width;
            lbl.Height = height;
            lbl.Text = text;
            lbl.Font = font;
            lbl.FontSize = fontSize;
            lbl.Color = color;
            mLabels.Add(index, lbl);
        }

        public void AddTextBox(int index, int x, int y, int width, string text) {
            MenuTextBox txt = new MenuTextBox();
            txt.X = x;
            txt.Y = y;
            txt.Width = width;
            txt.Text = text;
            mTextBoxs.Add(index, txt);
        }

        public void UpdateSize(int width, int height) {
            Width = width;
            Height = height;
        }

        public void SendMenuTo(Client client) {
            TcpPacket packet = new TcpPacket("custommenu");
            packet.AppendParameters(MenuName, Closeable.ToString());
            packet.AppendParameters(Width.ToString(), Height.ToString());
            packet.AppendParameters(mPictures.Count.ToString(), mLabels.Count.ToString(), mTextBoxs.Count.ToString());
            for (int i = 0; i < mPictures.Count; i++) {
                packet.AppendParameters(mPictures[i].ImagePath, mPictures[i].X.ToString(), mPictures[i].Y.ToString());
            }
            for (int i = 0; i < mLabels.Count; i++) {
                packet.AppendParameters(mLabels[i].X.ToString(), mLabels[i].Y.ToString(), mLabels[i].Width.ToString(), mLabels[i].Height.ToString(),
                    mLabels[i].Text, mLabels[i].Font, mLabels[i].FontSize.ToString(), mLabels[i].Color.ToArgb().ToString());
            }
            for (int i = 0; i < mTextBoxs.Count; i++) {
                packet.AppendParameters(mTextBoxs[i].X.ToString(), mTextBoxs[i].Y.ToString(), mTextBoxs[i].Width.ToString(), mTextBoxs[i].Text);
            }
            packet.FinalizePacket();
            Messenger.SendDataTo(client, packet);
        }

        public void SendCloseMenu(Client client, string menuName) {
            Messenger.SendDataTo(client, TcpPacket.CreatePacket("closemenu", menuName));
            MenuClosed(client, menuName);
        }

        private void MenuClosed(Client client, string menuName) {

        }
    }
}

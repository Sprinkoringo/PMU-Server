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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Server.Forms
{
    public partial class LoadingUI : Form
    {
        bool closeNormal;

        public LoadingUI() {
            InitializeComponent();
        }

        public delegate void UpdateStatusDelegate(string text);
        public void UpdateStatus(string text) {
            if (InvokeRequired) {
                Invoke(new UpdateStatusDelegate(UpdateStatus), text);
            } else {
                lblStatus.Text = text;
            }
        }

        private delegate void CloseDelegate(bool normal);
        public void Close(bool normal) {
            if (InvokeRequired) {
                Invoke(new CloseDelegate(Close), normal);
            } else {
                if (normal) {
                    closeNormal = true;
                    this.Close();
                } else {
                    this.Close();
                }
            }
        }

        private void LoadingUI_FormClosing(object sender, FormClosingEventArgs e) {
            if (!closeNormal) {
                Environment.Exit(0);
            }
        }

        private void LoadingUI_Load(object sender, EventArgs e)
        {

        }

    }
}

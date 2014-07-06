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

    }
}

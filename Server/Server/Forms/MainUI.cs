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
    public partial class MainUI : Form
    {
        CommandHandler commandProcessor;

        public MainUI() {
            InitializeComponent();
            commandProcessor = new CommandHandler(this);
            this.Text = "PMU Server";
        }

        private void txtCommand_TextChanged(object sender, EventArgs e) {
            int activeLine = txtCommandOutput.Lines.Length - 1;
            string[] lines = txtCommandOutput.Lines;
            lines[activeLine] = "Server> " + txtCommand.Text;
            txtCommandOutput.Lines = lines;
            ScrollToBottom(txtCommandOutput);
        }

        private void ScrollToBottom(TextBox txtBox) {
            txtBox.SelectionStart = txtBox.Text.Length - 1;
            txtBox.SelectionLength = 1;
            txtBox.ScrollToCaret();
        }

        private void txtCommand_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter && ProcessingCommand == false) {
                string command = txtCommand.Text;
                txtCommand.Text = "";
                int activeLine = txtCommandOutput.Lines.Length - 1;
                string[] lines = txtCommandOutput.Lines;
                lines[activeLine] = "Server> " + command;
                txtCommandOutput.Lines = lines;
                if (commandProcessor.IsValidCommand(command)) {
                    ProcessCommand(command);
                } else {
                    AddCommandLine(command + " is not a valid command.");
                    AddCommandLine("Server> ");
                }
            }
        }

        private delegate void AddCommandLineDelegate(string line);
        public void AddCommandLine(string line) {
            if (InvokeRequired) {
                Invoke(new AddCommandLineDelegate(AddCommandLine), line);
            } else {
                txtCommandOutput.Text += "\r\n" + line;
                ScrollToBottom(txtCommandOutput);
            }
        }
        private bool ProcessingCommand = false;

        private void ProcessCommand(string command) {
            txtCommand.Enabled = false;
            int activeLine = txtCommandOutput.Lines.Length - 1;
            string[] lines = txtCommandOutput.Lines;
            lines[activeLine] = "Server> " + command + " [Processing]";
            txtCommandOutput.Lines = lines;
            commandProcessor.ProcessCommand(activeLine, command);
        }

        private delegate void CommandCompleteDelegate(int activeLine, string command);
        internal void CommandComplete(int activeLine, string command) {
            if (InvokeRequired) {
                Invoke(new CommandCompleteDelegate(CommandComplete), activeLine, command);
            } else {
                if (txtCommandOutput.Lines.Length > activeLine) {
                    string[] lines = txtCommandOutput.Lines;
                    lines[activeLine] = "Server> " + command;
                    txtCommandOutput.Lines = lines;
                }
                AddCommandLine("Server> ");
                txtCommand.Enabled = true;
                txtCommand.Focus();
            }
        }

        private delegate void ClearCommandsDelegate();
        internal void ClearCommands() {
            if (InvokeRequired) {
                Invoke(new ClearCommandsDelegate(ClearCommands));
            } else {
                txtCommandOutput.Text = "PMU Server Command Prompt";
            }
        }

        private void txtCommandOutput_TextChanged(object sender, EventArgs e)
        {

        }

    }
}

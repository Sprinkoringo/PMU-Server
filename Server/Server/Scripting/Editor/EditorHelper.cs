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
using System.Reflection;
using PMU.Sockets;
using Server.Network;

namespace Server.Scripting.Editor
{
    public class EditorHelper
    {
        static Assembly assembly;
        static EditorClassCollection classes;

        internal static void Initialize() {
            assembly = Assembly.GetEntryAssembly();
            classes = new EditorClassCollection();
            classes.LoadClasses();
        }

        public static void InitTempScript(Client client) {
            if (System.IO.Directory.Exists(GetTempFolder(client)) == true) {
                System.IO.Directory.Delete(GetTempFolder(client), true);
            }
            System.IO.Directory.CreateDirectory(GetTempFolder(client));
            string[] files = System.IO.Directory.GetFiles(IO.Paths.ScriptsFolder, "*.cs", System.IO.SearchOption.TopDirectoryOnly);
            for (int i = 0; i < files.Length; i++) {
                System.IO.File.Copy(files[i], GetTempFolder(client) + System.IO.Path.GetFileNameWithoutExtension(files[i]) + ".cs");
            }
        }

        public static string GetTempFolder(Client client) {
            return IO.IO.ProcessPath(IO.Paths.ScriptsFolder + "Temp" + client.Player.CharID + "\\");
        }

        public static void AppendFileListToPacket(Client client, TcpPacket packet) {
            string[] files = System.IO.Directory.GetFiles(GetTempFolder(client));
            packet.AppendParameter(files.Length.ToString());
            for (int i = 0; i < files.Length; i++) {
                packet.AppendParameter(System.IO.Path.GetFileNameWithoutExtension(files[i]));
            }
        }

        public static void SendScriptClasses(Client client) {
            TcpPacket packet = new TcpPacket("scriptclasses");
            packet.AppendParameter(classes.Classes.Count.ToString());
            for (int i = 0; i < classes.Classes.Count; i++) {
                packet.AppendParameters(classes.Classes[i].Name);
            }
            packet.FinalizePacket();
            Messenger.SendDataTo(client, packet);
        }

        public static void SendScriptMethods(Client client, string ClassName) {
            TcpPacket packet = new TcpPacket("scriptmethods");
            EditorClass @class = classes.FindByName(ClassName);
            if (@class != null) {
                packet.AppendParameters(@class.Methods.Count.ToString(), ClassName);
                for (int i = 0; i < @class.Methods.Count; i++) {
                    packet.StartParameterSegment();
                    if (@class.Methods[i].Static) {
                        packet.AppendParameterSegment("[static] ");
                    }
                    if (!string.IsNullOrEmpty(@class.Methods[i].Type)) {
                        packet.AppendParameterSegment(@class.Methods[i].Type);
                        packet.AppendParameterSegment(" ");
                    }
                    packet.AppendParameterSegment(@class.Methods[i].Name);
                    packet.EndParameterSegment();
                }
                packet.FinalizePacket();
                Messenger.SendDataTo(client, packet);
            }
        }

        public static void SendScriptParameters(Client client, string ClassName, string MethodName, int overload) {
            TcpPacket packet = new TcpPacket("scriptparam");
            string tempStr = "";
            EditorClass @class = classes.FindByName(ClassName);
            if (@class != null) {
                int slot = @class.FindMethodByName(MethodName.Trim().Replace("[static] ", ""));
                if (slot > -1) {
                    if (overload - 1 > @class.Methods[slot].Params.Count) {
                        overload = 1;
                    }
                    tempStr = @class.Methods[slot].Params[overload - 1].ParamString;
                    if (!string.IsNullOrEmpty(@class.Methods[slot].Params[overload - 1].ReturnVal)) {
                        tempStr += "\n" + @class.Methods[slot].Params[overload - 1].ReturnVal;
                    }
                }
            }
            packet.AppendParameter(tempStr);
            Messenger.SendDataTo(client, packet);
        }

        public static void SendScriptErrors(Client client) {
            int i = 1;
            TcpPacket packet = new TcpPacket("scriptediterrors");
            packet.AppendParameter(ScriptManager.Errors.Count.ToString());
            foreach (System.CodeDom.Compiler.CompilerError Err in ScriptManager.Errors) {
                if (Err.ErrorText.ToLower().Contains("not a valid win32") == false) {
                    packet.AppendParameters(i.ToString(), Err.Line.ToString(), Err.IsWarning.ToString(), Err.ErrorText);
                    i += 1;
                }
            }
            packet.FinalizePacket();
            Messenger.SendDataTo(client, packet);
        }

        public static void SaveScript(Client client) {
            foreach (string File in System.IO.Directory.GetFiles(IO.Paths.ScriptsFolder, "*.cs", System.IO.SearchOption.TopDirectoryOnly)) {
                System.IO.File.Delete(File);
            }
            foreach (string File in System.IO.Directory.GetFiles(GetTempFolder(client))) {
                //IO.File.Delete(ScriptFolder & Path.GetFileNameWithoutExtension(File) & ".txt")
                System.IO.File.Copy(File, IO.Paths.ScriptsFolder + System.IO.Path.GetFileNameWithoutExtension(File) + ".cs", true);
            }
        }

        public static string GetScriptFile(Client client, string File) {
            return System.IO.File.ReadAllText(GetTempFolder(client) + File + ".cs");
        }

        public static void SaveTempScript(Client client, string File, string Code) {
            if (System.IO.Directory.Exists(GetTempFolder(client)) == false) {
                System.IO.Directory.CreateDirectory(GetTempFolder(client));
            }
            System.IO.File.WriteAllText(GetTempFolder(client) + File + ".cs", Code);
        }

        public static void AddNewClass(Client client, string ClassName) {
            if (System.IO.File.Exists(GetTempFolder(client) + ClassName + ".cs") == false) {
                System.IO.File.WriteAllText(GetTempFolder(client) + ClassName + ".cs", "using Server;\nusing Server.Scripting;\nusing Server.Database;\nusing System;\nusing System.Drawing;\nusing System.Xml;\nusing System.Collections.Generic;\nusing System.Linq;\nusing System.Text;\nusing System.Windows.Forms;\n\nnamespace Script \n{\npublic class " + ClassName + "\n{\n\n}\n}");
                Messenger.SendDataTo(client, TcpPacket.CreatePacket("scriptfiledata", ClassName, GetScriptFile(client, ClassName)));
            }
        }
    }
}

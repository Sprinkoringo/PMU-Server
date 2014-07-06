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


namespace Server.Scripting
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;
    using PMU.Core;

    public class ScriptManager
    {
        #region Fields

        static ListPair<string, IAICore> AIInstances;
        static ListPair<string, Type> AITypes;
        static List<System.CodeDom.Compiler.CompilerError> errors;
        static Assembly script;
        static string scriptFolder;
        static Type type;

        #endregion Fields

        #region Properties

        public static ListPair<string, string> NPCAIType {
            get;
            set;
        }

        public static List<System.CodeDom.Compiler.CompilerError> Errors {
            get { return errors; }
        }

        #endregion Properties

        #region Methods

        public static void CallAIProcessSub(string aiType, int tickCount, int mapNum, int mapNpcSlot) {
            if (aiType != null && script != null && NPCAIType.ContainsKey(aiType)) {
                string className = NPCAIType.GetValue(aiType);
                InvokeAISub(className, "ProcessAI", tickCount, mapNum, mapNpcSlot);
            }
        }

        public static void CompileScript(string folder, bool loading) {
            List<string> lstFiles = new List<string>();
            string[] files = System.IO.Directory.GetFiles(folder, "*.cs");
            script = Compile(files);
            CreateInstance("Script.Main");
            if (loading) {
                if (script == null) {
                    //Settings.Scripting = false;
                }
                PopulateAIList();
            }
            scriptFolder = folder;
        }

        public static bool TestCompile(string folder) {
            List<string> lstFiles = new List<string>();
            string[] files = System.IO.Directory.GetFiles(folder, "*.cs");
            return (Compile(files) != null);
        }

        public static void CreateAIInstance(string className, string aiType) {
            if (script != null) {
                if (NPCAIType.ContainsKey(aiType) == false) {
                    Type type = script.GetType(className);
                    IAICore instance = (IAICore)script.CreateInstance(className);
                    AITypes.Add(className, type);
                    AIInstances.Add(className, instance);
                    NPCAIType.Add(aiType, className);
                    //InvokeSub(className, "Init", Globals.mNetScript, Globals.mObjFactory, Globals.mIOTools, Globals.mDebug);
                }
            }
        }

        public static void Initialize() {
            AITypes = new ListPair<string, Type>();
            AIInstances = new ListPair<string, IAICore>();
            NPCAIType = new ListPair<string, string>();
        }

        public static object InvokeFunction(string functionName, params object[] args) {
            try {
                return type.InvokeMember(functionName,
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.InvokeMethod |
                    System.Reflection.BindingFlags.Static,
                    null, null, args);
            } catch (Exception ex) {
                TriggerError(ex);
                return null;
            }
        }

        public static void InvokeSub(string subName, params object[] args) {
            try {
                type.InvokeMember(subName,
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.InvokeMethod |
                    System.Reflection.BindingFlags.Static,
                    null, null, args);
            } catch (Exception ex) {
                TriggerError(ex);
            }
        }

        public static void InvokeSubSimple(string subName, object[] args) {
            try {
                type.InvokeMember(subName,
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.InvokeMethod |
                    System.Reflection.BindingFlags.Static,
                    null, null, args);
            } catch (Exception ex) {
                TriggerError(ex);
            }
        }

        public static void PopulateAIList() {
            AITypes.Clear();
            AIInstances.Clear();
            NPCAIType.Clear();
            Type ti = typeof(IAICore);
            if (script != null) {
                foreach (Type t in script.GetTypes()) {
                    if (ti.IsAssignableFrom(t) && t.IsPublic) {
                        CreateAIInstance(t.FullName, t.Name);
                    }
                }
            }
        }

        public static void Reload() {
            InvokeSub("BeforeScriptReload");
            CompileScript(scriptFolder, false);
            PopulateAIList();
            InvokeSub("AfterScriptReload");
        }

        internal static void CreateInstance(string className) {
            if (script != null) {
                type = script.GetType(className);
            }
        }

        private static Assembly Compile(string[] files) {
            errors = new List<System.CodeDom.Compiler.CompilerError>();

            Microsoft.CSharp.CSharpCodeProvider provider;
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("CompilerVersion", "v4.0");
            provider = new Microsoft.CSharp.CSharpCodeProvider(param);
            System.CodeDom.Compiler.CompilerParameters options = new System.CodeDom.Compiler.CompilerParameters();

            options.GenerateInMemory = true;
            options.TreatWarningsAsErrors = false;
            options.IncludeDebugInformation = true;

            List<string> refs = new List<string>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++) {
                if (!assemblies[i].FullName.Contains("System.") && !assemblies[i].FullName.Contains("Microsoft.")) {
                    refs.Add(System.IO.Path.GetFileName(assemblies[i].Location));
                    //refs.Add(assemblies[i].Location);
                
                }
            }
            refs.Add("System.dll");
            refs.Add("System.Data.dll");
            refs.Add("System.Drawing.dll");
            refs.Add("System.Xml.dll");
            refs.Add("System.Windows.Forms.dll"); 
            //refs.Add("DatabaseConnector.dll");
            refs.Add("System.Core.dll");
            refs.Add("MySql.Data.dll");
            refs.Add("DataManager.dll");
            refs.Add(System.Windows.Forms.Application.ExecutablePath);
            options.ReferencedAssemblies.AddRange(refs.ToArray());

            System.CodeDom.Compiler.ICodeCompiler compiler = provider.CreateCompiler();
            System.CodeDom.Compiler.CompilerResults results = compiler.CompileAssemblyFromFileBatch(options, files);

            foreach (System.CodeDom.Compiler.CompilerError err in results.Errors) {
                errors.Add(err);
            }

            if (results.Errors.Count == 0) {
                return results.CompiledAssembly;
            } else {
                return null;
            }
        }

        private static void InvokeAISub(string className, string subName, params object[] args) {
            try {
                AITypes.GetValue(className).InvokeMember(subName,
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.InvokeMethod |
                    System.Reflection.BindingFlags.Static,
                    null, AIInstances.GetValue(className), args);
            } catch (Exception) {
                //TriggerError(ex);
            }
        }

        private static void TriggerError(Exception ex) {
            try {
                type.InvokeMember("OnScriptError",
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.InvokeMethod |
                    System.Reflection.BindingFlags.Static,
                    null, null, new object[] { ex });
            } catch {
            }
        }

        #endregion Methods
    }
}
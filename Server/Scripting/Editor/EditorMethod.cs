using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Scripting.Editor
{
    public class EditorMethod
    {
        public List<ParamSet> Params { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool Static { get; set; }

        public EditorMethod() {
            Params = new List<ParamSet>();
        }
    }
}

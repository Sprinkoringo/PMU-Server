using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Server.Scripting.Editor
{
    public class ParamSet
    {
        public ParameterInfo[] Parameters { get; set; }
        public string ParamString { get; set; }
        public string ReturnVal { get; set; }
    }
}

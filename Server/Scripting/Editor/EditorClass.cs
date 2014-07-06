using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Scripting.Editor
{
    public class EditorClass
    {
        public string Name { get; set; }
        public List<EditorMethod> Methods { get; set; }

        public EditorClass() {
            Methods = new List<EditorMethod>();
        }

        public int FindMethodByName(string name) {
            for (int i = 0; i < Methods.Count; i++) {
                if (Methods[i].Name == name) {
                    return i;
                }
            }
            return -1;
        }
    }
}

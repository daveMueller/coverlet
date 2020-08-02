using System;
using System.Collections.Generic;
using System.Text;
using Mono.Cecil;

namespace coverlet.core.Symbols
{
    internal class GeneratedMethod
    {
        public string ParentMethodName { get; set; }
        public MethodDefinition Method { get; set; }
        public int StartLine { get; set; }
        public int EndLine { get; set; }
    }
}

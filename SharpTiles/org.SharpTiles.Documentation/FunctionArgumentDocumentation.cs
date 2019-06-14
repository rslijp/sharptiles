using System;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using org.SharpTiles.Expressions.Functions;

namespace org.SharpTiles.Documentation
{
    [DataContract]
    public class FunctionArgumentDocumentation
    {
        public FunctionArgumentDocumentation(FunctionArgument functionArgument)
        {
            Name = functionArgument.Name;
            TypeValue = functionArgument.Type;
            Params = functionArgument.Params;
        }

        [DataMember]
        public string Name { get; set; }
        [ScriptIgnore]
        public Type TypeValue { get; set; }
        [DataMember]
        public string Type => TypeValue.AssemblyQualifiedName;
        [DataMember]
        public bool Params { get; set; }

    }
}
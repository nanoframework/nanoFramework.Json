using System;
using System.Text;

namespace nanoFramework.Json.Test.Shared
{
    internal class JsonTestStaticProperty
    {
        public static JsonTestStaticProperty StaticProperty = new() { InstanceProperty = "StaticValue" };

        public string InstanceProperty { get; set;  } = "DefaultValue";
    }
}

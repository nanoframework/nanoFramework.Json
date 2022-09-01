using System;
using System.Text;

namespace nanoFramework.Json.Benchmark.TestClasses
{
    class JsonTestClassChild
    {
        public int one { get; set; }
        public int two { get; set; }
        public int three { get; set; }
#pragma warning disable S1104 // Fields should not have public accessibility
        public int four; //not a property on purpose!
#pragma warning restore S1104 // Fields should not have public accessibility
    }
}

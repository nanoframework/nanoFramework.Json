using nanoFramework.Benchmark;
using nanoFramework.Json.Configuration;
using nanoFramework.Json.Resolvers;
using System;
using System.Diagnostics;
using System.Threading;

namespace nanoFramework.Json.Benchmark
{
    public class Program
    {
        private class TestClass
        {
            public int NoGetProperty { private get; set; } = 1;
            public int NoSetProperty { get; } = 1;
        }

        public static void Main()
        {
            Settings.ThrowExceptionWhenPropertyNotFound = true;
            Settings.CaseSensitive = false;

            var resolver = new MemberResolver();
            resolver.Get(nameof(TestClass.NoGetProperty), typeof(TestClass));
            Thread.Sleep(Timeout.Infinite);
        }
    }
}

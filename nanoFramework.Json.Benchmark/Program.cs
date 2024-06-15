//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;
using nanoFramework.Benchmark;
using System.Threading;

namespace nanoFramework.Json.Benchmark
{
    public static class Program
    {
        public static void Main()
        {
            Console.WriteLine("Hello from nanoFramework JSON benchmark!");
            //BenchmarkRunner.Run(typeof(IAssemblyHandler).Assembly);
            BenchmarkRunner.RunClass(typeof(TypeBenchmarks));
            Thread.Sleep(Timeout.Infinite);
        }
    }
}

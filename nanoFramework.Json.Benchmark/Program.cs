//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;
using nanoFramework.Benchmark;
using System.Threading;
using nanoFramework.Json.Benchmark.DeserializationBenchmarks;
using nanoFramework.Json.Benchmark.SerializationBenchmarks;

namespace nanoFramework.Json.Benchmark
{
    public static class Program
    {
        public static void Main()
        {
            Console.WriteLine("********** Starting benchmarks **********");
            BenchmarkRunner.RunClass(typeof(ReferenceTypesDeserializationBenchmark));
            BenchmarkRunner.RunClass(typeof(ValueTypesDeserializationBenchmark));
            BenchmarkRunner.RunClass(typeof(ReferenceTypesSerializationBenchmark));
            BenchmarkRunner.RunClass(typeof(ValueTypesSerializationBenchmark));
            BenchmarkRunner.RunClass(typeof(TypeBenchmarks));
            Console.WriteLine("********** Completed benchmarks **********");
            Thread.Sleep(Timeout.Infinite);
        }
    }
}

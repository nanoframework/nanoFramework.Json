//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;
using System.Collections;
using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;
using nanoFramework.Json.Benchmark.Base;

namespace nanoFramework.Json.Benchmark
{
    [IterationCount(100)]
    public class TypeBenchmarks: BaseIterationBenchmark
    {
        protected override int IterationCount => 200;
        
        private readonly ArrayList _list = new();
        private const string ArrayListFullName = "System.Collections.ArrayList";
        private static readonly Type ArrayListType = typeof(ArrayList);

        [Benchmark]
        public void Benchmark_FullName_Comparison()
        {
            RunInIteration(() =>
            {
                if (!ArrayListFullName.Equals(_list.GetType().FullName))
                {
                    throw new ApplicationException();
                }
            });
        }

        [Benchmark]
        public void Benchmark_Type_Comparison()
        {
            RunInIteration(() =>
            {
                if (_list.GetType() != typeof(ArrayList))
                {
                    throw new ApplicationException();
                }
            });
        }

        [Benchmark]
        public void Benchmark_Type_Comparison_Static()
        {
            RunInIteration(() =>
            {
                if (_list.GetType() != ArrayListType)
                {
                    throw new ApplicationException();
                }
            });
        }

        [Benchmark]
        public void Benchmark_TypeUtils_Comparison()
        {
            RunInIteration(() =>
            {
                if (!TypeUtils.IsArrayList(_list.GetType()))
                {
                    throw new ApplicationException();
                }
            });
        }
    }
}

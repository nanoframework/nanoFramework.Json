//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;
using nanoFramework.Json.Benchmark.Base;
using nanoFramework.Json.Test.Shared;
using System;
using System.Collections;

namespace nanoFramework.Json.Benchmark.SerializationBenchmarks
{
    [IterationCount(5)]
    public class IgnoreSerializationBenchmark : BaseIterationBenchmark
    {
        private JsonIgnoreTestClass TestObject;

        [Setup]
        public void Setup()
        {
            // Create a test object
            TestObject = JsonIgnoreTestClass.CreateTestClass();
        }

        [Benchmark]
        public void DoTest()
        {
            RunInIteration(() =>
            {
                // Turn ON the UseIgnore setting
                Configuration.Settings.UseIgnoreAttribute = true;
                JsonConvert.SerializeObject(TestObject);
                // Turn OFF the UseIgnore setting
                Configuration.Settings.UseIgnoreAttribute = false;
            });
        }
    }
    [IterationCount(5)]
    public class NoIgnoreSerializationBenchmark : BaseIterationBenchmark
    {
        private JsonIgnoreTestClass TestObject;

        [Setup]
        public void Setup()
        {
            // Create a test object
            TestObject = JsonIgnoreTestClass.CreateTestClass();
        }

        [Benchmark]
        public void DoTest()
        {
            RunInIteration(() =>
            {
                // Turn OFF the UseIgnore setting
                Configuration.Settings.UseIgnoreAttribute = false;
                JsonConvert.SerializeObject(TestObject);
            });
        }
    }

}

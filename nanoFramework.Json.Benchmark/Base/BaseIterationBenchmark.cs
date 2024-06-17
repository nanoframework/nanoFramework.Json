//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;

namespace nanoFramework.Json.Benchmark.Base
{
    public abstract class BaseIterationBenchmark
    {
        protected virtual int IterationCount => 20;

        public void RunInIteration(Action methodToRun)
        {
            for (var i = 0; i < IterationCount; i++)
            {
                methodToRun();
            }
        }
    }
}

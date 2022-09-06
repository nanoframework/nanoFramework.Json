using System;
using System.Text;

namespace nanoFramework.Json.Benchmark.Base
{
    public abstract class BaseIterationBenchmark
    {
        protected virtual int IterationCount => 20;

        public void RunInIteration(Action methodToRun)
        {
            for (int i = 0; i < IterationCount; i++)
            {
                methodToRun();
            }
        }
    }
}

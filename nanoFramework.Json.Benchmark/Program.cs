using nanoFramework.Benchmark;
using System.Diagnostics;
using System.Threading;

namespace nanoFramework.Json.Benchmark
{
    public class Program
    {
        public static void Main()
        {
            Debug.WriteLine("Hello from nanoFramework JSON benchmark!");
            BenchmarkRunner.Run(typeof(IAssemblyHandler).Assembly);
            Thread.Sleep(Timeout.Infinite);
        }
    }
}

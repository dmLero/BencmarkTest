using BenchmarkDotNet.Running;

namespace BencmarkTest
{
    class Program
    {
        static void Main(string[] args)
        {
             BenchmarkRunner.Run<HttpYieldBenchmark>();
        }              
    }
}

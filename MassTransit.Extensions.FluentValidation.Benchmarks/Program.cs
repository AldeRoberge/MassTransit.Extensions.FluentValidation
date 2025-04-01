using BenchmarkDotNet.Running;

namespace MassTransit.Extensions.FluentValidation.Benchmarks;

public abstract class Program
{
    public static void Main()
    {
        BenchmarkRunner.Run<ValidationBenchmark>();
    }
}
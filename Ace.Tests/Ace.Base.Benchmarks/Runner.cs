using System;
using System.Diagnostics;
using System.Linq;
using Ace.Base.Benchmarking.Benchmarks;
using BenchmarkDotNet.Running;

namespace Ace.Base.Benchmarking
{
    static class Program
    {
        private const long WarmRunsCount = 1000;
        private const long HotRunsCount = 10000000; // 10 000 000

        static void Main()
        {
            BenchmarkRunner.Run<TypeOfBenchmarks>();
            BenchmarkRunner.Run<RipeTypeBenchmarks>();
            
            TypeofVsTypeOf();
            RawTypeVsRipeType();

            Console.ReadKey();
        }

        static void RawTypeVsRipeType()
        {
            Console.WriteLine($"Count of warm iterations: {WarmRunsCount}");
            Console.WriteLine($"Count of hot iterations: {HotRunsCount}");
            Console.WriteLine();
            var o = new object();
            var rawType = o.GetType();
            var ripeType = o.GetRipeType();

            RunBenchmarks(
                (() => rawType.Name, "() => rawType.Name"),
                (() => ripeType.Name, "() => ripeType.Name"),
                (() => o.GetType().Name, "() => o.GetType().Name"),
                (() => o.GetRipeType().Name, "() => o.GetRipeType().Name")
            );

            Console.WriteLine();

            RunBenchmarks(
                (() => rawType.Assembly, "() => rawType.Assembly"),
                (() => ripeType.Assembly, "() => ripeType.Assembly"),
                (() => o.GetType().Assembly, "() => o.GetType().Assembly"),
                (() => o.GetRipeType().Assembly, "() => o.GetRipeType().Assembly")
            );

            Console.WriteLine();

            RunBenchmarks(
                (() => rawType.IsValueType, "() => rawType.IsValueType"),
                (() => ripeType.IsValueType, "() => ripeType.IsValueType"),
                (() => o.GetType().IsValueType, "() => o.GetType().IsValueType"),
                (() => o.GetRipeType().IsValueType, "() => o.GetRipeType().IsValueType")
            );
        }

        static void TypeofVsTypeOf()
        {
            Console.WriteLine($"Count of warm iterations: {WarmRunsCount}");
            Console.WriteLine($"Count of hot iterations: {HotRunsCount}");
            Console.WriteLine();

            RunBenchmarks(
                (() => typeof(int), "() => typeof(int)"),
                (() => TypeOf<int>.Raw, "() => TypeOf<int>.Raw"),
                (() => typeof(string), "() => typeof(string)"),
                (() => TypeOf<string>.Raw, "() => TypeOf<string>.Raw")
            );

            Console.WriteLine();

            RunBenchmarks(
                (() => typeof(int).Name, "() => typeof(int).Name"),
                (() => TypeOf<int>.Name, "() => TypeOf<int>.Name"),
                (() => typeof(string).Name, "() => typeof(string).Name"),
                (() => TypeOf<string>.Name, "() => TypeOf<string>.Name")
            );

            Console.WriteLine();

            RunBenchmarks(
                (() => typeof(int).Assembly, "() => typeof(int).Assembly"),
                (() => TypeOf<int>.Assembly, "() => TypeOf<int>.Assembly"),
                (() => typeof(string).Assembly, "() => typeof(string).Assembly"),
                (() => TypeOf<string>.Assembly, "() => TypeOf<string>.Assembly")
            );

            Console.WriteLine();

            RunBenchmarks(
                (() => typeof(int).IsValueType, "() => typeof(int).IsValueType"),
                (() => TypeOf<int>.IsValueType, "() => TypeOf<int>.IsValueType"),
                (() => typeof(string).IsValueType, "() => typeof(string).IsValueType"),
                (() => TypeOf<string>.IsValueType, "() => TypeOf<string>.IsValueType")
            );
        }

        static void RunBenchmarks<T>(params (Func<T> Func, string StringRepresentation)[] funcAndViewTuples) =>
            funcAndViewTuples
                .Select(t => (
                    BenchmarkResults: t.Func.InvokeBenchmark(HotRunsCount, WarmRunsCount),
                    StringRepresentation: t.StringRepresentation))
                .ToList().ForEach(t =>
                    Console.WriteLine(
                        $"{t.StringRepresentation}\t{t.BenchmarkResults.Result}\t{t.BenchmarkResults.ElapsedMilliseconds} (ms)"));

        static (Func<T> Func, long ElapsedMilliseconds, T Result) InvokeBenchmark<T>(this Func<T> func,
            long hotRunsCount, long warmRunsCount)
        {
            var stopwatch = new Stopwatch();
            var result = default(T);

            for (var i = 0L; i < warmRunsCount; i++)
                result = func();

            stopwatch.Start();
            for (var i = 0L; i < hotRunsCount; i++)
                result = func();

            stopwatch.Stop();
            return (func, stopwatch.ElapsedMilliseconds, result);
        }
    }
}
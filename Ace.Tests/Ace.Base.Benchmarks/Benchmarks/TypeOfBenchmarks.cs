using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;

namespace Ace.Base.Benchmarking.Benchmarks
{
	[
		CoreJob,
		ClrJob,
		MonoJob("Mono", @"C:\Program Files\Mono\bin\mono.exe")
	]
	public class TypeOfBenchmarks
	{
		[Benchmark] public string typeof_int_Name() => typeof(int).Name;
		[Benchmark] public string TypeOf_int_Name() => TypeOf<int>.Name;
		[Benchmark] public string typeof_string_Name() => typeof(string).Name;
		[Benchmark] public string TypeOf_string_Name() => TypeOf<string>.Name;
		
		[Benchmark] public Assembly typeof_int_Assembly() => typeof(int).Assembly;
		[Benchmark] public Assembly TypeOf_int_Assembly() => TypeOf<int>.Assembly;
		[Benchmark] public Assembly typeof_string_Assembly() => typeof(string).Assembly;
		[Benchmark] public Assembly TypeOf_string_Assembly() => TypeOf<string>.Assembly;
		
		[Benchmark] public bool typeof_int_IsValueType() => typeof(int).IsValueType;
		[Benchmark] public bool TypeOf_int_IsValueType() => TypeOf<int>.IsValueType;
		[Benchmark] public bool typeof_string_IsValueType() => typeof(string).IsValueType;
		[Benchmark] public bool TypeOf_string_IsValueType() => TypeOf<string>.IsValueType;
	}
}
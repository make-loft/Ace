using System;
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
	public class RipeTypeBenchmarks
	{
		static object o = new object();
		readonly Type rawType = o.GetType();
		readonly RipeType ripeType = o.GetRipeType();
		
		[Benchmark] public string RawType_Name() => rawType.Name;
		[Benchmark] public string RipeType_Name() => ripeType.Name;
		[Benchmark] public string GetRawType_Name() => o.GetType().Name;
		[Benchmark] public string GetRipeType_Name() => o.GetRipeType().Name;

		[Benchmark] public Assembly RawType_Assembly() => rawType.Assembly;
		[Benchmark] public Assembly RipeType_Assembly() => ripeType.Assembly;
		[Benchmark] public Assembly GetRawType_Assembly() => o.GetType().Assembly;
		[Benchmark] public Assembly GetRipeType_Assembly() => o.GetRipeType().Assembly;

		[Benchmark] public bool RawType_IsValueType() => rawType.IsValueType;
		[Benchmark] public bool RipeType_IsValueType() => ripeType.IsValueType;
		[Benchmark] public bool GetRawType_IsValueType() => o.GetType().IsValueType;
		[Benchmark] public bool GetRipeType_IsValueType() => o.GetRipeType().IsValueType;
	}
}
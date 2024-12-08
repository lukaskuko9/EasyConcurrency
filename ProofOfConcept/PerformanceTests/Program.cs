using BenchmarkDotNet.Running;
using PerformanceTests;

BenchmarkRunner.Run<Benchmark>();

/*
| Method         | Mean          | Error         | StdDev        |
|--------------- |--------------:|--------------:|--------------:|
| IsNotLockedRaw |      6.153 ns |     0.2025 ns |     0.5908 ns |
| IsNotLocked    | 54,123.748 ns | 1,019.7421 ns | 1,812.5907 ns |

54,123.748 / 6.153 = IsNotLockedRaw is 8796 times faster
*/
using System.Linq.Expressions;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Infrastructure.Database.Entities;

BenchmarkRunner.Run<Benchmark>();

/*
| Method         | Mean          | Error         | StdDev        |
|--------------- |--------------:|--------------:|--------------:|
| IsNotLockedRaw |      6.153 ns |     0.2025 ns |     0.5908 ns |
| IsNotLocked    | 54,123.748 ns | 1,019.7421 ns | 1,812.5907 ns |

54,123.748 / 6.153 = IsNotLockedRaw is 8796 times faster
*/
public class Benchmark
{
    private record DummyConcurrentEntity : ConcurrentEntity;

    private static class Methods
    {
        public static bool IsNotLockedRaw<T>(T entity) where T : ConcurrentEntity
        {
            return entity.LockedUntil == null || entity.LockedUntil < DateTimeOffset.Now;
        }
        
        public static bool IsNotLocked<T>(T entity) where T : ConcurrentEntity
        {
            return IsNotLocked<T>().Compile().Invoke(entity);
        }

        private static Expression<Func<T, bool>> IsNotLocked<T>() where T : ConcurrentEntity
        {
            return entity => IsNotLockedRaw(entity);
        }
    }

    [Benchmark]
    public void IsNotLockedRaw()
    {
        var concurrentEntity = new DummyConcurrentEntity();
        _ = Methods.IsNotLockedRaw(concurrentEntity);
    }
    
    [Benchmark]
    public void IsNotLocked()
    {
        var concurrentEntity = new DummyConcurrentEntity();
        _ = Methods.IsNotLocked(concurrentEntity);
    }
    
}
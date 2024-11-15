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
    private record DummyLockableEntity : LockableEntity;

    private static class Methods
    {
        public static bool IsNotLockedRaw<T>(T entity) where T : LockableEntity
        {
            return entity.LockedUntil == null || entity.LockedUntil < DateTimeOffset.Now;
        }
        
        public static bool IsNotLocked<T>(T entity) where T : LockableEntity
        {
            return IsNotLocked<T>().Compile().Invoke(entity);
        }

        private static Expression<Func<T, bool>> IsNotLocked<T>() where T : LockableEntity
        {
            return entity => IsNotLockedRaw(entity);
        }
    }

    [Benchmark]
    public void IsNotLockedRaw()
    {
        var lockableEntity = new DummyLockableEntity();
        _ = Methods.IsNotLockedRaw(lockableEntity);
    }
    
    [Benchmark]
    public void IsNotLocked()
    {
        var lockableEntity = new DummyLockableEntity();
        _ = Methods.IsNotLocked(lockableEntity);
    }
    
}
using System.Linq.Expressions;
using BenchmarkDotNet.Attributes;
using Infrastructure.Database.Entities;

namespace PerformanceTests
{
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
}
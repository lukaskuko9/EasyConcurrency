using BenchmarkDotNet.Attributes;
using EntityFrameworkCore.PessimisticConcurrency.Abstractions;

namespace PerformanceTests
{
    public class Benchmark
    {
        private class DummyLockableEntity : LockableEntity;
        
        [Benchmark]
        public void IsNotLockedRaw()
        {
            var concurrentEntity = new DummyLockableEntity();
        }
    }
}
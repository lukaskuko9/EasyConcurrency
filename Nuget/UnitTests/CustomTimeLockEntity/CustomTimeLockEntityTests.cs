using System.Reflection;
using EasyConcurrency.Abstractions.TimeLock;
using EasyConcurrency.Tests.Shared;

namespace EasyConcurrency.Tests.UnitTests.CustomTimeLockEntity;

public class CustomTimeLockEntityTests
{
    [Fact]
    public void CustomTimeLockEntity_Uses_CustomTimeLock()
    {
        var assembly = Assembly.GetAssembly(typeof(Shared.CustomTimeLockEntity))!;
        var customEntityType = assembly.DefinedTypes.Single(x=>x.Name == nameof(Shared.CustomTimeLockEntity));
        
        Assert.Contains(customEntityType.ImplementedInterfaces, type=>type == typeof(IHasTimeLock<CustomTimeLock>));
        Assert.Contains(customEntityType.DeclaredProperties, type=> type.PropertyType == typeof(CustomTimeLock?));
    }
}
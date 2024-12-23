# EasyConcurrency
EasyConcurrency nugets aim to make concurrency easier for your .NET Core Applications. The main motivation for this project was the fact that Entity Framework does not have a built-in solution for pessimistic concurrency control.

Be it optimistic or pessimistic concurrency control, EasyConcurrency can help you with both of them.

If you are new to concurrency handling, you can read more about [Optimistic concurrency control](https://github.com/lukaskuko9/EasyConcurrency/blob/readmes/Readme/OptimisticConcurrency.md) or
[Pessimistic concurrency control](https://github.com/lukaskuko9/EasyConcurrency/blob/readmes/Readme/PessimisticConcurrency.md) directly in this repository.

For installation options, visit the package on [Nuget.org](https://www.nuget.org/packages/EasyConcurrency.EntityFramework/).

## EasyConcurrency.Abstractions
EasyConcurrency.Abstractions nuget provides the essential classes and interfaces
that form the foundation of the implementation.

### TimeLock
*TimeLock* is a custom value type that wraps around `DateTimeOffset`. The *TimeLock* is locked for a given duration until it naturally expires,
or is unlocked / released before that can happen.

It provides functionality to manipulate with time until which the *TimeLock* should be locked, such as:

`IsNotLocked` - for checking if this *TimeLock* is not locked / is available for locking

`SetLock` - for setting the date and time until which the entity is locked. When the lock naturally expires,
the entity will be unlocked. Important thing to note is that this does not lock all the entities,
only the single row for a single entity that is locked.

`Unlock` - for unlocking the time lock before it naturally expires.
E.g. we have locked an entity to do write operations on it, but are finished working with it.
We can unlock the *TimeLock* so that other processes can claim it for write changes.


### ILockableEntity
*ILockableEntity* is an interface providing various methods for locking and unlocking
entity. This is the entity you'd want to lock before reaching critical section.

Naturally this interface also provides `LockedUntil` property of type *TimeLock*,
that has [Concurrency check attribute](https://learn.microsoft.com/en-us/ef/ef6/modeling/code-first/data-annotations#concurrencycheck).

#### LockableEntity
Implementations methods from `ILockableEntity` interface.

#### LockableConcurrentEntity
`LockableConcurrentEntity` inherits from `LockableEntity`, providing also `Version`
property with [Timestamp attribute](https://learn.microsoft.com/en-us/ef/ef6/modeling/code-first/data-annotations#timestamp).

Both of these types can be used for pessimistic concurrency control.
The difference between `LockableEntity` and `LockableConcurrentEntity`
is that the former reacts to concurrences only on the `LockedUntil` property to lock it properly,
while the latter reacts to concurrences on any of the properties on that entity.

## EasyConcurrency.EntityFramework
EasyConcurrency.EntityFramework provides base implementations for concurrency handling when using EntityFramework. 

### Setup

#### Database entity
The entity you want to save to database should inherit from `LockableEntity` or `LockableConcurrentEntity` class.
This provides it with the `LockedUntil` property.

Example from [Samples](https://github.com/lukaskuko9/EasyConcurrency/tree/master/Samples) solution:
````csharp
public class SampleEntity : LockableConcurrentEntity
{
    public long Id { get; set; }
    public Guid MyUuid { get; set; }
    public bool IsProcessed { get; set; }
}
````

#### Database context configuration
Because we use `TimeLock` which is a custom value type,
we need to tell entity framework how to convert it to primity type `DateTimeOffset`.
This can easily be done with extension method `.AddTimeLockConversion();` 
for the `LockedUntil` property on our database entity.

Depending on your database queries you might want to have an index, 
or maybe a composite index on `LockedUntil` property.

Example from [Samples](https://github.com/lukaskuko9/EasyConcurrency/tree/master/Samples) solution:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<SampleEntity>(entityBuilder =>
    {
        entityBuilder.HasKey(sampleEntity => sampleEntity.Id);
        entityBuilder.Property(sampleEntity => sampleEntity.LockedUntil).AddTimeLockConversion();
        
        entityBuilder.HasIndex(sampleEntity => new {sampleEntity.LockedUntil, sampleEntity.IsProcessed});
        entityBuilder.HasIndex(sampleEntity => sampleEntity.MyUuid).IsUnique();
    });
}
```

#### Locking entity

#### Unlocking entity



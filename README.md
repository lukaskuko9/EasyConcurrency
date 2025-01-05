# Table of Contents
1. [EasyConcurrency](#easyconcurrency)
2. [EasyConcurrency.Abstractions](#easyconcurrencyabstractions)
    * [TimeLock](#timelock)
    * [ITimeLockEntity](#ITimeLockEntity)
    * [TimeLockEntity](#TimeLockEntity)
    * [TimeLockVersioningEntity](#TimeLockVersioningEntity)
3. [EasyConcurrency.EntityFramework](#easyconcurrencyentityframework)
    * [Setup](#setup)
        * [Database entity](#database-entity)
        * [Database context configuration](#database-context-configuration)
    * [Usage](#usage)
        * [Fetching entities with no lock](#fetching-entities-with-no-lock)
        * [Locking entity](#locking-entity)
        * [Unlocking entity](#unlocking-entity)

# EasyConcurrency
EasyConcurrency nuget packages aim to make concurrency easier for your .NET Core Applications. The main motivation for this project was the fact that Entity Framework does not have a built-in solution for pessimistic concurrency control.

Be it optimistic or pessimistic concurrency control, EasyConcurrency can help you with both of them.

If you are new to concurrency handling, you can read more about [Optimistic concurrency control](https://github.com/lukaskuko9/EasyConcurrency/blob/master/Readme/OptimisticConcurrency.md) or
[Pessimistic concurrency control](https://github.com/lukaskuko9/EasyConcurrency/blob/master/Readme/PessimisticConcurrency.md) directly in this repository.

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


### ITimeLockEntity
*ITimeLockEntity* is an interface providing various methods for locking and unlocking
entity. This is the entity you'd want to lock before reaching critical section.

Naturally this interface also provides `LockedUntil` property of type *TimeLock*,
that has [Concurrency check attribute](https://learn.microsoft.com/en-us/ef/ef6/modeling/code-first/data-annotations#concurrencycheck).

#### TimeLockEntity
Implementations methods from `ITimeLockEntity` interface.

#### TimeLockVersioningEntity
`TimeLockVersioningEntity` inherits from `TimeLockEntity`, providing also `Version`
property with [Timestamp attribute](https://learn.microsoft.com/en-us/ef/ef6/modeling/code-first/data-annotations#timestamp).

Both of these types can be used for pessimistic concurrency control.
The difference between `TimeLockEntity` and `TimeLockVersioningEntity`
is that the former reacts to concurrences only on the `LockedUntil` property to lock it properly,
while the latter reacts to concurrences on any of the properties on that entity.

## EasyConcurrency.EntityFramework
EasyConcurrency.EntityFramework provides base implementations for concurrency handling when using EntityFramework.

### Setup

#### Database entity
The entity you want to save to database should inherit from `TimeLockEntity` or `TimeLockVersioningEntity` class.
This provides it with the `LockedUntil` property.

Example from [Samples](https://github.com/lukaskuko9/EasyConcurrency/tree/master/Samples) solution:
````csharp
public class SampleEntity : TimeLockVersioningEntity
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
public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<SampleEntity> SampleEntities { get; set; }
   
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
}
```

### Usage

#### Fetching entities with no lock
To fetch only entities that are currently not locked during database query,
you can use `.WhereIsNotLocked()` extension method.

This will add the `.Where` condition to the query to get you only entities
that you can safely write changes to.

```csharp
//get entity only if not locked; if entity is locked this will return null;
var entity = await dbContext.SampleEntities
    .WhereIsNotLocked()
    .SingleOrDefaultAsync(sampleEntity => sampleEntity.MyUuid == ConcurrentEntityUuid);

if (entity is null)
{
    //entity is null, we cannot process it
    return;
}
```
#### Locking entity
Locking the entity is no more work than calling `SetLock` method to set the lock for the entity,
and calling `SaveChanges` or `SaveChangesAsync` method afterward to save the changes to database.

While saving the changes, a [DbUpdateConcurrencyException](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbupdateconcurrencyexception)
can be raised if the lock was changed on the entity by other process
after it was fetched from database.

You might catch this exception and [resolve concurrency conflicts](https://learn.microsoft.com/en-us/ef/core/saving/concurrency?tabs=data-annotations#resolving-concurrency-conflicts).

```csharp
//lock entity for 5 minutes
entity.SetLock(TimeSpan.FromMinutes(5));

try
{
    //save lock to database
    await dbContext.SaveChangesAsync();
    Console.WriteLine($"Task {i} successfully locked entity");
}
catch (DbUpdateConcurrencyException)
{
    //failed to lock entity, entity was locked by other task after we got the entity from database
    Console.WriteLine($"Task {i} failed to lock entity. Exception was handled");
    continue;
}
```

If everything is done correctly, then the process that locked the entity is the
only one able to manipulate with it.

*Note: even if lock is set in database, it will not work unless you respect the lock with
`IsNotLocked` methods before making any write changes to it.*

#### Unlocking entity
Unlocking the entity is not any harder than locking it. Simply call method `Unlock` on the entity.

```csharp
//unlock entity and save to database
entity.Unlock();
await dbContext.SaveChangesAsync(); 

This will set the `LockedUntil` property to value `null`, resetting the lock.
Other processes can lock it again for themselves to do some other changes.
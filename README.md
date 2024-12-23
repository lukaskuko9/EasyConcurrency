# EasyConcurrency
EasyConcurrency nugets aim to make concurrency easier for your .NET Core Applications. The main motivation for this project was the fact that Entity Framework does not have a built-in solution for pessimistic concurrency control.

Be it optimistic or pessimistic concurrency control, EasyConcurrency can help you with both of them.

If you are new to concurrency handling, you can read more about [Optimistic concurrency control](https://github.com/lukaskuko9/EasyConcurrency/blob/readmes/Readme/OptimisticConcurrency.md) or
[Pessimistic concurrency control](https://github.com/lukaskuko9/EasyConcurrency/blob/readmes/Readme/PessimisticConcurrency.md) directly in this repository.

## EasyConcurrency.EntityFramework
EasyConcurrency.EntityFramework provides base implementations for concurrency handling when using EntityFramework. 

For installation options, visit the package on [Nuget.org](https://www.nuget.org/packages/EasyConcurrency.EntityFramework/).

### Setup

#### Database entity
Start with your database entity you want to save to database. 
This entity should inherit from `LockableEntity` or `LockableConcurrentEntity` class.

The `LockableEntity` contains a `LockedUntil` property with some additional methods. 
The property is used for indicating the duration of the lock for pessimistic concurrency.
The `LockedUntil` property has [Concurrency check attribute](https://learn.microsoft.com/en-us/ef/ef6/modeling/code-first/data-annotations#concurrencycheck).

`LockableConcurrentEntity` inherits from `LockableEntity`, providing also `Version` 
property with [Timestamp attribute](https://learn.microsoft.com/en-us/ef/ef6/modeling/code-first/data-annotations#timestamp).

To make keep it simple, both of these types can be used for pessimistic concurrency control. 
The difference between `LockableEntity` and `LockableConcurrentEntity` 
is that the former detects concurrences only on the `LockedUntil` property to lock it properly,
while the latter detects concurrences on any of the properties on that entity.


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
* wrapper conversion
* index on LockedUntil



#### Locking entity

#### Unlocking entity



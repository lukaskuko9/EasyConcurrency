# EasyConcurrency
EasyConcurrency nugets aim to make concurrency easier for your .NET Core Applications. The main motivation for this project was the fact that Entity Framework does not have a built-in solution for pessimistic concurrency control.

Be it optimistic or pessimistic concurrency control, EasyConcurrency can help you with both of them.

If you are new to concurrency handling, you can read more about [Optimistic concurrency control](https://github.com/lukaskuko9/EasyConcurrency/blob/readmes/Readme/OptimisticConcurrency.md) or
[Pessimistic concurrency control](https://github.com/lukaskuko9/EasyConcurrency/blob/readmes/Readme/PessimisticConcurrency.md) directly in this repository.

For installation options using EntityFramework, visit the package on [Nuget.org](https://www.nuget.org/packages/EasyConcurrency.EntityFramework/).


## EasyConcurrency.Abstractions
EasyConcurrency.Abstractions nuget provides the essential classes and interfaces
that form the foundation of the implementation.

### TimeLock
*TimeLock* is a custom value type that wraps around `DateTimeOffset`. The *TimeLock* is locked for a given duration until it naturally expires,
or is unlocked / released before that can happen.

It provides functionality to manipulate with time until which the *TimeLock* should be locked, such as:

`IsLocked` / `IsNotLocked` - for checking if this *TimeLock* is not locked / is available for locking

`SetLock` - for setting the date and time until which the entity is locked. When the lock naturally expires,
the entity will be unlocked.

`Unlock` - for unlocking the time lock before it naturally expires.
E.g. we have locked an entity to do write operations on it, but are finished working with it.
We can unlock the *TimeLock* so that other processes can claim it for write changes.
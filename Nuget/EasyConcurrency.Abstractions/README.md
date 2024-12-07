# EasyConcurrency.Abstractions

EasyConcurrency.Abstractions project provides the essential classes and interfaces 
that form the foundation of the implementation.

## ConcurrentEntity
Solution for optimistic concurrency.
Contains only <i>Version</i> property.

When we fetch the (original) <i>Version</i> property, it already has value.
If we make changes to this entity, <i>Version</i> also changes.

If the value in data source equals our original value, the changes are saved.
If the original value differs from the value that the data source has, an exception is raised and changes are not saved.

## TimeLock
<i>TimeLock</i> is a custom value type that wraps around `DateTimeOffset`. The <i>TimeLock</i> is locked for a given duration until it naturally expires,
or is unlocked / released before that can happen.

It provides functionality to manipulate with time until which the <i>TimeLock</i> should be locked, such as:

`IsNotLocked` - for checking if this <i>TimeLock</i> is not locked

`SetLock` - for setting the date and time until which the entity is locked. When the lock naturally expires, 
the entity will be automatically unlocked.

`Unlock` - for unlocking the time lock before it naturally expires.
E.g. we have locked an entity to do write operations on it, but are finished. 
We can unlock the <i>TimeLock</i> so that other processes can claim it for write changes.


## LockableEntity
<i>LockableEntity</i> is an entity that has a <i>TimeLock</i> and inherits from <i>ConcurrentEntity</i>.
This entity is what you would store in database.
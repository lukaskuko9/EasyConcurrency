using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EasyConcurrency.EntityFramework.TimeLock;

/// <summary>
/// Value converter from <see cref="TimeLock"/> value to <see cref="DateTimeOffset"/> value.
/// </summary>
public class TimeLockConverter() : ValueConverter<Abstractions.Entities.TimeLock?, DateTimeOffset?>
(
    timeLock => timeLock == null ? null : timeLock.Value.Value,
    dateTimeOffset => new Abstractions.Entities.TimeLock(dateTimeOffset)
);
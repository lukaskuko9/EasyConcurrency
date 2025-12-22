using EasyConcurrency.Abstractions.TimeLock;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EasyConcurrency.EntityFramework.ValueConverter;

/// <summary>
/// Value converter from <see cref="TimeLock"/> value to <see cref="DateTimeOffset"/> value.
/// </summary>
public class TimeLockConverter() : ValueConverter<TimeLock?, DateTimeOffset?>
(
    timeLock => timeLock == null ? null : timeLock.Value.Value,
    dateTimeOffset => new TimeLock(dateTimeOffset)
);
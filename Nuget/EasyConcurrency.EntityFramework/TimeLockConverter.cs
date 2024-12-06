using EasyConcurrency.Abstractions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EasyConcurrency.EntityFramework;

public class TimeLockConverter() : ValueConverter<TimeLock?, DateTimeOffset?>
(
    timeLock => timeLock == null ? null : timeLock.Value.Value,
    dateTimeOffset => new TimeLock(dateTimeOffset)
);
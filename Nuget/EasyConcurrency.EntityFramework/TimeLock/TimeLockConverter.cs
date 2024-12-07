using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EasyConcurrency.EntityFramework.TimeLock;

public class TimeLockConverter() : ValueConverter<Abstractions.TimeLock?, DateTimeOffset?>
(
    timeLock => timeLock == null ? null : timeLock.Value.Value,
    dateTimeOffset => new Abstractions.TimeLock(dateTimeOffset)
);
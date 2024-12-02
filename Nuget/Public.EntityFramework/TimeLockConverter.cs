using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Public.EntityFramework;

public class TimeLockConverter() : ValueConverter<TimeLock?, DateTimeOffset?>
(
    timeLock => timeLock == null ? null : timeLock.Value.Value,
    dateTimeOffset => new TimeLock(dateTimeOffset)
    );
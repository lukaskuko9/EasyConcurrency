using EntityFrameworkCore.PessimisticConcurrency.Abstractions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EntityFrameworkCore.PessimisticConcurrency;

public class TimeLockConverter() : ValueConverter<TimeLock?, DateTimeOffset?>
(
    timeLock => timeLock == null ? null : timeLock.Value.Value,
    dateTimeOffset => new TimeLock(dateTimeOffset)
    );
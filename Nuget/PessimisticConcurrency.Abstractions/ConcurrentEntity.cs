using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkCore.PessimisticConcurrency.Abstractions;

public abstract class ConcurrentEntity
{
    [Timestamp] 
    public byte[] Version { get; } = [];
}
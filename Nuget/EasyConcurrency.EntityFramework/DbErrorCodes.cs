using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EasyConcurrency.EntityFramework;

/// <summary>
/// Error codes:
/// https://learn.microsoft.com/en-us/sql/relational-databases/errors-events/database-engine-events-and-errors-2000-to-2999?view=sql-server-ver16
/// </summary>
public static class DbExceptionErrorCodes
{
    /// <summary>
    /// Cannot insert duplicate key row in object ‘%.ls’ with unique index ‘%.ls’. The duplicate key value is %ls.
    /// </summary>
    private const int DuplicateKeyRowError = 2601;
    
    /// <summary>
    /// Violation of %ls constraint ‘%.ls’. Cannot insert duplicate key in object ‘%.ls’. The duplicate key value is %ls.
    /// </summary>
    private const int UniqueConstraintViolation = 2627;

    public static bool IsUniqueConstraintViolation(DbUpdateException exception)
    {
        return exception.InnerException is SqlException
        {
            Number: DuplicateKeyRowError or UniqueConstraintViolation
        };
    }
}
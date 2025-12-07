using EasyConcurrency.EntityFramework.Experimental.ConcurrentRepository;

namespace EasyConcurrency.IntegrationTests.Database;

public class MyConcurrentRepository(DatabaseContext databaseContext) : ConcurrentRepository<DatabaseContext>(databaseContext);
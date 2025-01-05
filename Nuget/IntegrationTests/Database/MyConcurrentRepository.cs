using EasyConcurrency.EntityFramework.Experimental.ConcurrentRepository;

namespace IntegrationTests.Database;

public class MyConcurrentRepository(DatabaseContext databaseContext) : ConcurrentRepository<DatabaseContext>(databaseContext);
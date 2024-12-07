using Xunit;

namespace IntegrationTests;

[CollectionDefinition(CollectionName)]
public class DatabaseCollection : ICollectionFixture<DatabaseCollection>
{
    // This class' purpose is for database integration sets of tests to run sequentially,
    // Meaning a set of tests will only start when the previous set is run
    // This is because all sets are connected to the same database
    public const string CollectionName = "Database collection";
}
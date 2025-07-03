using Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace TestUtilities.Database;

public class TestDatabase : IDisposable
{
    public SqliteConnection Connection { get; }

    private TestDatabase(string connectionString)
    {
        Connection = new SqliteConnection(connectionString);
    }

    public static TestDatabase CreateDatabase()
    {
        var testDatabase = new TestDatabase("DataSource=:memory:");

        testDatabase.InitializeDatabase();

        return testDatabase;
    }

    public PetrichorDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<PetrichorDbContext>()
            .UseSqlite(Connection)
            .Options;

        return new PetrichorDbContext(options, null!, null!);
    }

    public void ResetDatabase()
    {
        Connection.Close();

        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        Connection.Open();

        using var context = CreateDbContext();

        context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Connection.Dispose();
        GC.SuppressFinalize(this);
    }
}

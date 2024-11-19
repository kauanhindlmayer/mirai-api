using Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Application.SubcutaneousTests.Common;

/// <summary>
/// In Subcutaneous tests we aren't testing integration with a real database,
/// so even if we weren't using SQLite we would use some in-memory database.
/// </summary>
public class SqliteTestDatabase : IDisposable
{
    public SqliteConnection Connection { get; }

    public static SqliteTestDatabase CreateAndInitialize()
    {
        var testDatabase = new SqliteTestDatabase("DataSource=:memory:");

        testDatabase.InitializeDatabase();

        return testDatabase;
    }

    public void InitializeDatabase()
    {
        Connection.Open();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(Connection)
            .Options;

        using var context = new ApplicationDbContext(options, null!, null!);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }

    public void ResetDatabase()
    {
        Connection.Close();

        InitializeDatabase();
    }

    public void Dispose()
    {
        Connection.Close();
    }

    private SqliteTestDatabase(string connectionString)
    {
        Connection = new SqliteConnection(connectionString);
    }
}
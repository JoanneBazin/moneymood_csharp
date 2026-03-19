using System.Data.Common;
using Npgsql;
using Respawn;

namespace MoneyMood.Tests.Helpers;

public class DbReset
{
    private Respawner _respawner = null!;
    private DbConnection _connection = null!;

    public async Task InitializeAsync(string connectionString)
    {
        _connection = new NpgsqlConnection(connectionString);
        await _connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"]
        });
    }

    public async Task ResetAsync()
    {
        await _respawner.ResetAsync(_connection);
    }
}
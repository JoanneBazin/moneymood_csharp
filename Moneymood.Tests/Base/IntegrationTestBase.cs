using MoneyMood.Tests.Factory;
using MoneyMood.Tests.Fixtures;
using MoneyMood.Tests.Helpers;

namespace MoneyMood.Tests.Base;

[Collection("Database")]
public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected readonly HttpClient Client;
    private readonly DbReset _dbReset;
    private readonly string _connectionString;

    protected IntegrationTestBase(DatabaseFixture fixture)
    {
        var factory = new TestWebApplicationFactory(fixture);
        Client = factory.CreateClient();

        _connectionString = fixture.Container.GetConnectionString();
        _dbReset = new DbReset();
    }

    public async Task InitializeAsync()
    {
        await _dbReset.InitializeAsync(_connectionString);
        await _dbReset.ResetAsync();
    } 
    public Task DisposeAsync() => Task.CompletedTask;

    protected async Task ResetDb() => await _dbReset.ResetAsync();
    
}
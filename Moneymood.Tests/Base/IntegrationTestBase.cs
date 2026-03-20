using Microsoft.Extensions.DependencyInjection;
using MoneyMood.Data;
using MoneyMood.Tests.Factory;
using MoneyMood.Tests.Fixtures;
using MoneyMood.Tests.Helpers;

namespace MoneyMood.Tests.Base;

[Collection("Database")]
public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected readonly HttpClient Client;
    protected readonly TestWebApplicationFactory Factory;
    private readonly DbReset _dbReset;
    private readonly string _connectionString;

    protected IntegrationTestBase(DatabaseFixture fixture)
    {
        Factory = new TestWebApplicationFactory(fixture);
        Client = Factory.CreateClient();

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

    protected AppDbContext GetDbContext()
    {
        var scope = Factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    protected AuthHelper CreateAuthHelper()
    {
        return new AuthHelper(Client, GetDbContext);
    }
    
}
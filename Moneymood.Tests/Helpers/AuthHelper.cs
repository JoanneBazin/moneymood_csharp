using System.Net.Http.Json;
using Moneymood.Tests.Helpers.TestDataBuilders;
using MoneyMood.Data;
using MoneyMood.Dtos.Auth;

namespace MoneyMood.Tests.Helpers;

public class AuthHelper(HttpClient client, Func<AppDbContext> getDb)
{
    private readonly HttpClient _client = client;
    private readonly Func<AppDbContext> _getDb = getDb;

    public async Task<UserResponse> CreateAuthenticatedUserAsync(
        string email = "test@example.com",
        string password = "Password1234",
        string name = "Test User"
    )
    {
        using var db = _getDb();
        var user = await new UserBuilder()
            .WithEmail(email)
            .WithPassword(password)
            .WithName(name)
            .BuildAndSaveAsync(db);

        var signInRequest = new { email, password };
        var response = await _client.PostAsJsonAsync("/api/auth/login", signInRequest);
        response.EnsureSuccessStatusCode();

        return user;
    }
}
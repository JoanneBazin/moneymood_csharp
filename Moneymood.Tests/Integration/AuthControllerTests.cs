using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MoneyMood.Dtos.Auth;
using MoneyMood.Tests.Fixtures;
using MoneyMood.Tests.Base;
using Microsoft.EntityFrameworkCore;
using Moneymood.Tests.Helpers.TestDataBuilders;

namespace MoneyMood.Tests.Integration;

public class AuthControllerTests(DatabaseFixture dbFixture) : IntegrationTestBase(dbFixture)
{
    [Fact]
    public async Task SignUp_WithValidData_ReturnsOkAndUser()
    {
        await ResetDb();
        var signupRequest = new
        {
            email = "test@example.com",
            password = "TestUser1234",
            name = "Test User"
        };

        var response = await Client.PostAsJsonAsync("/api/auth/signup", signupRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var user = await response.Content.ReadFromJsonAsync<UserResponse>();
        user.Should().NotBeNull();
        user.Email.Should().Be(signupRequest.email);
        user.Name.Should().Be(signupRequest.name);
        user.Id.Should().NotBeEmpty();

        response.Headers.TryGetValues("Set-Cookie", out var cookies).Should().BeTrue();
        var sessionCookie = cookies!.FirstOrDefault(c => c.StartsWith("session="));
        sessionCookie.Should().NotBeNullOrEmpty();

        using var db = GetDbContext();
        var userInDb = await db.Users.FirstOrDefaultAsync(u => u.Email == signupRequest.email);
        userInDb.Should().NotBeNull();
    }

    [Fact]
    public async Task SignUp_WithInvalidEmail_ReturnsBadRequest()
    {
        await ResetDb();
        var signupRequest = new
        {
            email = "not-an-email",
            password = "TestUser1234",
            name = "Test User"
        };

        var response = await Client.PostAsJsonAsync("/api/auth/signup", signupRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        error.Should().NotBeNull();
        error.Error.Should().Contain("email");

        if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
        {
            var sessionCookie = cookies.FirstOrDefault(c => c.StartsWith("session=") && !c.Contains("expires="));
            sessionCookie.Should().BeNullOrEmpty();
        };

        using var db = GetDbContext();
        var userInDb = await db.Users.FirstOrDefaultAsync(u => u.Email == signupRequest.email);
        userInDb.Should().BeNull();
    }

    [Fact]
    public async Task SignIn_WithValidCredentials_ReturnsOkAndUser()
    {
        await ResetDb();
        using var db = GetDbContext();
        var user = await new UserBuilder().BuildAndSaveAsync(db);
        var signInRequest = new
        {
            email = user.Email,
            password = user.Password,
        };

        var response = await Client.PostAsJsonAsync("/api/auth/login", signInRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var userResponse = await response.Content.ReadFromJsonAsync<UserResponse>();
        userResponse.Should().NotBeNull();
        userResponse.Email.Should().Be(user.Email);
        userResponse.Name.Should().Be(user.Name);
        userResponse.Id.Should().NotBeEmpty();

        response.Headers.TryGetValues("Set-Cookie", out var cookies).Should().BeTrue();
        var sessionCookie = cookies!.FirstOrDefault(c => c.StartsWith("session="));
        sessionCookie.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SignIn_WithInvalidCredentials_ReturnsUnauthorized()
    {
        await ResetDb();
        using var db = GetDbContext();
        var user = await new UserBuilder().BuildAndSaveAsync(db);
        var signInRequest = new
        {
            email = user.Email,
            password = "RandomFalsePass1234",
        };

        var response = await Client.PostAsJsonAsync("/api/auth/login", signInRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

       var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        error.Should().NotBeNull();
        error.Error.Should().Contain("mot de passe");

        if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
        {
            var sessionCookie = cookies.FirstOrDefault(c => c.StartsWith("session=") && !c.Contains("expires="));
            sessionCookie.Should().BeNullOrEmpty();
        };
    }

    [Fact]
    public async Task SignIn_WithUnknownEmail_ReturnsUnauthorized()
    {
        await ResetDb();
        using var db = GetDbContext();
        var user = await new UserBuilder().BuildAndSaveAsync(db);
        var signInRequest = new
        {
            email = "wrong-email@exemple.com",
            password = user.Password,
        };

        var response = await Client.PostAsJsonAsync("/api/auth/login", signInRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

       var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        error.Should().NotBeNull();
        error.Error.Should().Contain("mot de passe");

       if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
        {
            var sessionCookie = cookies.FirstOrDefault(c => c.StartsWith("session=") && !c.Contains("expires="));
            sessionCookie.Should().BeNullOrEmpty();
        };
    }

    public record ErrorResponse(string Error);
}
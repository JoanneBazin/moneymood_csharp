using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MoneyMood.Dtos.Auth;
using MoneyMood.Tests.Fixtures;
using MoneyMood.Tests.Base;

namespace MoneyMood.Tests.Integration;

public class AuthControllerTests(DatabaseFixture dbFixture) : IntegrationTestBase(dbFixture)
{
    [Fact]
    public async Task SignUp_WithValidData_ReturnsCreatedAndUser()
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
    }

    [Fact]
    public async Task SignUp_WithInvalidEmail_ReturnsBadRequest()
    {
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

    }

    public record ErrorResponse(string Error);
}
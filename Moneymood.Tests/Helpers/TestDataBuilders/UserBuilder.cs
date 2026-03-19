using MoneyMood.Data;
using MoneyMood.Models;

namespace Moneymood.Tests.Helpers.TestDataBuilders;

public class UserBuilder
{
    private string _email = "test@example.com";
    private string _password = "Password1234";
    private string _name = "Test User";

    public UserBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public UserBuilder WithPassword(string password)
    {
        _password = password;
        return this;
    }
    public UserBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public async Task<UserData> BuildAndSaveAsync(AppDbContext db)
    {
        var user = new User
        {
            Email = _email,
            Password = BCrypt.Net.BCrypt.HashPassword(_password),
            Name = _name
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();
        return MapToReturn();
    }

    public UserData Build()
    {
        return MapToReturn();
    }

    private UserData MapToReturn()
    {
        return new UserData(_email, _password, _name);
    }

    public record UserData(string Email, string Password, string Name);
}
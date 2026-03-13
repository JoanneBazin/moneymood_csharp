using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using MoneyMood.Auth;
using MoneyMood.Configuration;
using MoneyMood.Data;
using MoneyMood.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAuthentication("SessionAuth").AddScheme<AuthenticationSchemeOptions, SessionAuthHandler>("SessionAuth", null);

builder.Services.ConfigureValidation();
builder.Services.AddEndpointsApiExplorer();

builder.Services.ConfigureApplicationServices();

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

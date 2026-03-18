using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using MoneyMood.Auth;
using MoneyMood.Configuration;
using MoneyMood.Data;
using MoneyMood.Middlewares;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAuthentication("SessionAuth").AddScheme<AuthenticationSchemeOptions, SessionAuthHandler>("SessionAuth", null);

builder.Services.ConfigureValidation();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.ConfigureApplicationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

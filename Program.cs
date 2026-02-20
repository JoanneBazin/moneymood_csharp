using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using MoneyMood.Auth;
using MoneyMood.Data;
using MoneyMood.Middlewares;
using MoneyMood.Services.Auth;
using MoneyMood.Services.Budget;
using MoneyMood.Services.FixedUserEntry;
using MoneyMood.Services.Project;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers(options =>
{
    options.Filters.Add(new AuthorizeFilter());
})
.ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = false;
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ISessionService, SessionService>();

builder.Services.AddScoped<IMonthlyBudgetService, MonthlyBudgetService>();
builder.Services.AddScoped<IMonthlyEntryService, MonthlyEntryService>();
builder.Services.AddScoped<IMonthlyExpenseService, MonthlyExpenseService>();

builder.Services.AddScoped<ISpecialBudgetService, SpecialBudgetService>();
builder.Services.AddScoped<ISpecialCategoryService, SpecialCategoryService>();
builder.Services.AddScoped<ISpecialExpenseService, SpecialExpenseService>();

builder.Services.AddScoped<IFixedEntryService, FixedEntryService>();

builder.Services.AddScoped<IBudgetCalculationService, BudgetCalculationService>();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication("SessionAuth").AddScheme<AuthenticationSchemeOptions, SessionAuthHandler>("SessionAuth", null);

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

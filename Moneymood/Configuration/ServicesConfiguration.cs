using MoneyMood.Services.Auth;
using MoneyMood.Services.Budget;
using MoneyMood.Services.FixedUserEntry;
using MoneyMood.Services.Project;
using MoneyMood.Services.UserProfile;

namespace MoneyMood.Configuration;

public static class ServicesConfiguration
{
    public static void ConfigureApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ISessionService, SessionService>();

        services.AddScoped<IMonthlyBudgetService, MonthlyBudgetService>();
        services.AddScoped<IMonthlyEntryService, MonthlyEntryService>();
        services.AddScoped<IMonthlyExpenseService, MonthlyExpenseService>();
        services.AddScoped<IBudgetCalculationService, BudgetCalculationService>();

        services.AddScoped<ISpecialBudgetService, SpecialBudgetService>();
        services.AddScoped<ISpecialCategoryService, SpecialCategoryService>();
        services.AddScoped<ISpecialExpenseService, SpecialExpenseService>();

        services.AddScoped<IFixedEntryService, FixedEntryService>();

        services.AddScoped<IUserProfileService, UserProfileService>();
        
    }
}
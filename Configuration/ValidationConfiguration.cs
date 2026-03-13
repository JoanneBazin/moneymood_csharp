using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace MoneyMood.Configuration;

public static class ValidationConfiguration
{
    public static void ConfigureValidation(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<Program>();

        services.AddControllers(options =>
        {
            options.Filters.Add(new AuthorizeFilter());
        })
        .ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .SelectMany(e =>
                    {
                       var fieldName = e.Key.Contains('.') 
                       ? e.Key.Split('.').Last()
                       : e.Key;  

                       return e.Value!.Errors.Select(err =>
                        {
                            var msg = err.ErrorMessage;
                            if (msg.Contains("could not be converted") ||
                                msg.Contains("is required"))
                            {
                                if (fieldName == "$" || fieldName == "request" || string.IsNullOrEmpty(fieldName))
                                {
                                    return "Le format de la requête est invalide";
                                }
                                return $"Le champ {fieldName} est invalide";
                            }

                            return msg;
                        });
                    })
                    .ToList();
            
                return new BadRequestObjectResult(new { error = string.Join(". ", errors) });
                        
            };
        });
    }
};
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MoneyMood.Data;
using MoneyMood.Tests.Fixtures;

namespace MoneyMood.Tests.Factory;

public class TestWebApplicationFactory(DatabaseFixture dbFixture) : WebApplicationFactory<Program>
{
    private readonly DatabaseFixture _dbFixture = dbFixture;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            var descriptors = services.Where(d => 
                d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                d.ServiceType == typeof(AppDbContext)).ToList();

            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }


            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(_dbFixture.Container.GetConnectionString()));

        });
    }
    
}
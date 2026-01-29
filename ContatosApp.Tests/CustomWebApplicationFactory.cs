using System.Linq;
using ContatosApp.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ContatosApp.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            //Forçar connection string pro MariaDB do docker-compose
            var settings = new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] =
                    "Server=db;Port=3306;Database=contatosdb;User=app;Password=app123;"
            };

            config.AddInMemoryCollection(settings);
        });

        builder.ConfigureServices(services =>
        {
            //Se existir remove DbContext anterior
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            //registra DbContext apontando para o banco do docker
            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(
                    "Server=db;Port=3306;Database=contatosdb;User=app;Password=app123;",
                    ServerVersion.AutoDetect("Server=db;Port=3306;Database=contatosdb;User=app;Password=app123;")
                )
            );

            //aplica migrations automaticamente para garantir tabela Contacts
            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
        });
    }
}

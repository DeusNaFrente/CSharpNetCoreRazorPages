using ContatosApp.Data;
using ContatosApp.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

//Services
builder.Services.AddRazorPages();

var isTesting = builder.Environment.IsEnvironment("Testing");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = isTesting
        ? "Test"
        : CookieAuthenticationDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme = isTesting
        ? "Test"
        : CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/Login";
    options.AccessDeniedPath = "/Login";
});

if (isTesting)
{
    builder.Services.AddAuthentication()
        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
}

//Banco de dados
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' não encontrada no appsettings.json.");
}


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);


//App
var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}



if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();

//Permite a classe Program ser usada pelo teste
public partial class Program { }

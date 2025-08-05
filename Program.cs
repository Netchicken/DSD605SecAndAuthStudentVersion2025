using DSD605SecAndAuthStudentVersion2025.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);



builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:3000", "http://localhost:3001")  // Just the origin, not the full API path
                                  .AllowAnyMethod()                        // Allow GET, POST, PUT, DELETE, etc.
                                  .AllowAnyHeader();                       // Allow any headers in requests

                      });
});

//api interface
builder.Services.AddOpenApi();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3;
    options.Password.RequiredUniqueChars = 1;
    options.SignIn.RequireConfirmedEmail = false;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});

builder.Services.AddAuthorization(options =>
{
    // Add custom policies here if needed
    options.AddPolicy("RequireAdministratorRole", policyBuilder => policyBuilder.RequireRole("Admin"));
    options.AddPolicy("AdminPolicy", policyBuilder => policyBuilder.RequireClaim("Admin"));

});


builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/RolesManager", "RequireAdministratorRole");
});

builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

var app = builder.Build();




// Configure API and Swagger to only run in development
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // Add Swagger UI using Swashbuckle (standard for ASP.NET Core)

    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
        // options.RoutePrefix = "swagger";
    });
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();
// CORS must be placed after UseRouting and before UseAuthentication
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();

public partial class Program { }
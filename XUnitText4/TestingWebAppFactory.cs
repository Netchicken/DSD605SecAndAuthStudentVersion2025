using DSD605SecAndAuthStudentVersion2025.Data;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;


namespace DSD605SecAndAuthStudentVersion2025.XUnitText4
{
    //Inherits from WebApplicationFactory<Program>: This is Microsoft's built-in class for creating test versions of web apps
    //•	Purpose: Creates a controlled testing environment for your Razor Pages application
    public class TestingWebAppFactory<TEntryPoint> : WebApplicationFactory<Program> where TEntryPoint : Program
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {

                //What it does: Finds and removes the real database connection from your app's services
                //Why: Your tests shouldn't touch your real production database
                //Analogy: Like unplugging your app from the real database before testing
                var descriptor = services.SingleOrDefault(d => d.ServiceType ==
                        typeof(DbContextOptions<ApplicationDbContext>));
                // we remove the ApplicationDbContext registration from the Program class
                if (descriptor != null)
                    services.Remove(descriptor);

                //we add the database context to the service container and instruct it to use the in-memory database instead of the real database
                //•	Adds fake database: Creates a temporary database that exists only in RAM
                //•	Database name: "InMemoryMoviesTest" - this is isolated from other tests
                //•	Benefits: Fast, safe, and automatically cleaned up after tests
                services.AddDbContext<ApplicationDbContext>(options =>

                {
                    options.UseInMemoryDatabase("InMemoryMoviesTest");
                });
                //Finally, we ensure that we seed the data from the ApplicationDbContext class (The same data you inserted into a real SQL Server database).
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())

                using (var appContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                {
                    try
                    {
                        appContext.Database.EnsureCreated();
                    }
                    catch (Exception ex)
                    {
                        //Log errors or do anything you think it's needed
                        // Log errors or handle as needed
                        throw new InvalidOperationException("Failed to initialize test database", ex);
                    }
                }
                //1.BuildServiceProvider(): Creates the dependency injection container
                //2.CreateScope(): Creates a temporary "scope" for database operations
                //3.GetRequiredService<ApplicationDbContext>(): Gets your database context
                //4.EnsureCreated(): Creates all database tables and seeds data

            });

        }
    }
}

using DSD605SecAndAuthStudentVersion2025.Data;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace XUnit_Tests
{
    public class TestingWebAppFactory<TEntryPoint> : WebApplicationFactory<Program> where TEntryPoint : Program
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType ==
                        typeof(DbContextOptions<ApplicationDbContext>));
                // we remove the ApplicationDbContext registration from the Program class
                if (descriptor != null)
                    services.Remove(descriptor);

                //we add the database context to the service container and instruct it to use the in-memory database instead of the real database
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
            });

        }
    }
}

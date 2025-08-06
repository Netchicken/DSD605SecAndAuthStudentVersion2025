using DSD605SecAndAuthStudentVersion2025.Data;
using DSD605SecAndAuthStudentVersion2025.Models;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using System.Net;
using System.Net.Http.Json;


namespace XUnitTest4
{
    public class MovieIntegrationTestSampleData : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly string _databaseName;

        public MovieIntegrationTestSampleData(WebApplicationFactory<Program> factory)
        {

            // Use a consistent database name for all operations
            _databaseName = $"InMemoryDbForTesting_{Guid.NewGuid()}";

            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove all EF-related services
                    var efServices = services.Where(s =>
                        s.ServiceType.Namespace != null &&
                        s.ServiceType.Namespace.StartsWith("Microsoft.EntityFrameworkCore"))
                        .ToList();

                    foreach (var service in efServices)
                    {
                        services.Remove(service);
                    }

                    // Add only InMemory database
                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase(_databaseName);
                    });

                    // Ensure the database is created
                    using var scope = services.BuildServiceProvider().CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var wasCreated = context.Database.EnsureCreated();

                    // Log or assert the result
                    Console.WriteLine($"Database was created: {wasCreated}");
                    Console.WriteLine($"Database '{_databaseName}' was created: {wasCreated}");

                    // Check if using InMemory provider
                    var isInMemory = context.Database.IsInMemory();
                    Console.WriteLine($"Using InMemory database: {isInMemory}");
                });
            });

            _client = _factory.CreateClient();
        }

        private async Task<int> SeedTestData()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Verify we're using the same database
            Console.WriteLine($"SeedTestData using database: {context.Database.ProviderName}");


            // Clear existing data
            var existing = await context.Movie.ToListAsync();
            Console.WriteLine($"Found {existing.Count} existing movies before clearing");

            context.Movie.RemoveRange(existing);
            await context.SaveChangesAsync();

            // Add test data
            var movies = new List<Movie>
            {
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "Test Movie 1",
                    ReleaseDate = new DateTime(2023, 1, 1),
                    Overview = "Test overview 1",
                    Genre = "Action",
                    Price = 9.99m
                },
                new Movie
                {
                    Id = Guid.NewGuid(),
                    Title = "Test Movie 2",
                    ReleaseDate = new DateTime(2023, 6, 15),
                    Overview = "Test overview 2",
                    Genre = "Comedy",
                    Price = 12.99m
                }
            };

            context.Movie.AddRange(movies);
            var saveResult = await context.SaveChangesAsync();
            Console.WriteLine($"SaveChanges affected {saveResult} rows");

            // Verify seeding worked
            var count = await context.Movie.CountAsync();
            Console.WriteLine($"Database now contains {count} movies after seeding");

            // List all movies for verification
            var allMovies = await context.Movie.ToListAsync();
            foreach (var movie in allMovies)
            {
                Console.WriteLine($"- {movie.Id}: {movie.Title}");
            }

            return count;
        }

        [Fact]
        public async Task GetMovies_ReturnsAllMovies()
        {
            // Arrange
            Console.WriteLine("=== Starting GetMovies_ReturnsAllMovies test ===");
            var seededCount = await SeedTestData();
            Console.WriteLine($"Seeded {seededCount} movies");

            // Act
            Console.WriteLine("Making GET request to /api/movies");
            var response = await _client.GetAsync("/api/movies");

            // Assert
            Console.WriteLine($"API Response Status: {response.StatusCode}");
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"API Response Content: {responseContent}");

            var movies = await response.Content.ReadFromJsonAsync<List<Movie>>();
            Console.WriteLine($"Deserialized {movies?.Count} movies from API response");

            Assert.NotNull(movies);
            Assert.Equal(2, movies.Count);
            Assert.Contains(movies, m => m.Title == "Test Movie 1");
            Assert.Contains(movies, m => m.Title == "Test Movie 2");

            Console.WriteLine("=== GetMovies_ReturnsAllMovies test completed successfully ===");
        }

        [Fact]
        public async Task GetMovie_WithValidId_ReturnsMovie()
        {
            // Arrange
            await SeedTestData();

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var existingMovie = await context.Movie.FirstAsync();

            // Act
            var response = await _client.GetAsync($"/api/movies/{existingMovie.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            var movie = await response.Content.ReadFromJsonAsync<Movie>();

            Assert.NotNull(movie);
            Assert.Equal(existingMovie.Id, movie.Id);
            Assert.Equal(existingMovie.Title, movie.Title);
        }

        [Fact]
        public async Task GetMovie_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var invalidId = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/movies/{invalidId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PostMovie_WithValidData_CreatesMovie()
        {
            // Arrange
            var newMovie = new Movie
            {
                Title = "New Test Movie",
                ReleaseDate = new DateTime(2024, 3, 15),
                Overview = "New test overview",
                Genre = "Drama",
                Price = 14.99m
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/movies", newMovie);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var createdMovie = await response.Content.ReadFromJsonAsync<Movie>();

            Assert.NotNull(createdMovie);
            Assert.Equal(newMovie.Title, createdMovie.Title);
            Assert.Equal(newMovie.Genre, createdMovie.Genre);
            Assert.Equal(newMovie.Price, createdMovie.Price);
            Assert.NotEqual(Guid.Empty, createdMovie.Id);


            // Verify it was actually created in the database
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var movieInDb = await context.Movie.FindAsync(createdMovie.Id);
            Assert.NotNull(movieInDb);
        }

        [Fact]
        public async Task PutMovie_WithValidData_UpdatesMovie()
        {
            // Arrange
            await SeedTestData();

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var existingMovie = await context.Movie.FirstAsync();

            existingMovie.Title = "Updated Title";
            existingMovie.Price = 19.99m;

            // Act
            var response = await _client.PutAsJsonAsync($"/api/movies/{existingMovie.Id}", existingMovie);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify the update in database
            using var verifyScope = _factory.Services.CreateScope();
            var verifyContext = verifyScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var updatedMovie = await verifyContext.Movie.FindAsync(existingMovie.Id);

            Assert.NotNull(updatedMovie);
            Assert.Equal("Updated Title", updatedMovie.Title);
            Assert.Equal(19.99m, updatedMovie.Price);
        }

        [Fact]
        public async Task PutMovie_WithMismatchedId_ReturnsBadRequest()
        {
            // Arrange
            await SeedTestData();

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var existingMovie = await context.Movie.FirstAsync();

            var differentId = Guid.NewGuid();

            // Act
            var response = await _client.PutAsJsonAsync($"/api/movies/{differentId}", existingMovie);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PutMovie_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            var movie = new Movie
            {
                Id = nonExistentId,
                Title = "Non-existent Movie",
                ReleaseDate = DateTime.Now,
                Overview = "Test",
                Genre = "Test",
                Price = 10.00m
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/movies/{nonExistentId}", movie);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteMovie_WithValidId_DeletesMovie()
        {
            // Arrange
            await SeedTestData();

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var existingMovie = await context.Movie.FirstAsync();

            // Act
            var response = await _client.DeleteAsync($"/api/movies/{existingMovie.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify deletion in database
            using var verifyScope = _factory.Services.CreateScope();
            var verifyContext = verifyScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var deletedMovie = await verifyContext.Movie.FindAsync(existingMovie.Id);
            Assert.Null(deletedMovie);
        }

        [Fact]
        public async Task DeleteMovie_WithInvalidId_ReturnsNotFound()
        {
            // Arrange - Clear database to ensure clean state
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Movie.RemoveRange(context.Movie);
            await context.SaveChangesAsync();

            var invalidId = Guid.NewGuid();

            // Act
            var response = await _client.DeleteAsync($"/api/movies/{invalidId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
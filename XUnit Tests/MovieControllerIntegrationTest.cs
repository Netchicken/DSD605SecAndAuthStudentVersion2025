namespace XUnit_Tests
{
    public class MovieControllerIntegrationTests : IClassFixture<TestingWebAppFactory<Program>>
    {
        //// private readonly WebApplicationFactory<Program> _factory;
        //private readonly HttpClient _client;

        //public MovieControllerIntegrationTests(TestingWebAppFactory<Program> factory)
        //{
        //    _factory = factory.WithWebHostBuilder(builder =>
        //    {
        //        // Use the same database configuration as the main application
        //        // This will use the real database with existing data

        //        // Remove any existing DbContext registration to avoid conflicts
        //        var descriptor = services.SingleOrDefault(
        //            d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
        //        if (descriptor != null)
        //            services.Remove(descriptor);

        //        // Re-add the DbContext with the same configuration as the main application
        //        // This reads from the same appsettings.json and uses the DefaultConnection
        //        var configuration = new ConfigurationBuilder()
        //            .SetBasePath(Directory.GetCurrentDirectory())
        //            .AddJsonFile("../DSD605SecAndAuthStudentVersion2025/appsettings.json", optional: false)
        //            .AddJsonFile("../DSD605SecAndAuthStudentVersion2025/appsettings.Development.json", optional: true)
        //            .Build();

        //        var connectionString = configuration.GetConnectionString("DefaultConnection")
        //            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        //        services.AddDbContext<ApplicationDbContext>(options =>
        //            options.UseSqlServer(connectionString));
        //    });
        //});


        //    _client = _factory.CreateClient();
        //}

        //private async Task<List<Movie>> GetAllMoviesFromDatabase()
        //{
        //    using var scope = _factory.Services.CreateScope();
        //    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        //    return await context.Movie.ToListAsync();
        //}

        //private async Task<Movie?> GetMovieFromDatabase(Guid id)
        //{
        //    using var scope = _factory.Services.CreateScope();
        //    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        //    return await context.Movie.FindAsync(id);
        //}

        //[Fact]
        //public async Task GetMovies_ReturnsAllMoviesFromDatabase()
        //{
        //    // Arrange - Get expected data from database
        //    var expectedMovies = await GetAllMoviesFromDatabase();

        //    // Act
        //    var response = await _client.GetAsync("/api/movies");

        //    // Assert
        //    response.EnsureSuccessStatusCode();
        //    var actualMovies = await response.Content.ReadFromJsonAsync<List<Movie>>();

        //    Assert.NotNull(actualMovies);
        //    Assert.Equal(expectedMovies.Count, actualMovies.Count);

        //    // Verify all expected movies are returned
        //    foreach (var expectedMovie in expectedMovies)
        //    {
        //        var actualMovie = actualMovies.FirstOrDefault(m => m.Id == expectedMovie.Id);
        //        Assert.NotNull(actualMovie);
        //        Assert.Equal(expectedMovie.Title, actualMovie.Title);
        //        Assert.Equal(expectedMovie.Genre, actualMovie.Genre);
        //        Assert.Equal(expectedMovie.Price, actualMovie.Price);
        //    }
        //}

        //[Fact]
        //public async Task GetMovie_WithExistingId_ReturnsCorrectMovie()
        //{
        //    // Arrange - Get an existing movie from database
        //    var existingMovies = await GetAllMoviesFromDatabase();

        //    // Skip test if no movies exist
        //    if (!existingMovies.Any())
        //    {
        //        Assert.True(true, "No movies in database to test with");
        //        return;
        //    }

        //    var existingMovie = existingMovies.First();

        //    // Act
        //    var response = await _client.GetAsync($"/api/movies/{existingMovie.Id}");

        //    // Assert
        //    response.EnsureSuccessStatusCode();
        //    var movie = await response.Content.ReadFromJsonAsync<Movie>();

        //    Assert.NotNull(movie);
        //    Assert.Equal(existingMovie.Id, movie.Id);
        //    Assert.Equal(existingMovie.Title, movie.Title);
        //    Assert.Equal(existingMovie.Genre, movie.Genre);
        //    Assert.Equal(existingMovie.Price, movie.Price);
        //}

        //[Fact]
        //public async Task GetMovie_WithNonExistentId_ReturnsNotFound()
        //{
        //    // Arrange - Use a GUID that definitely doesn't exist
        //    var nonExistentId = Guid.NewGuid();

        //    // Verify it doesn't exist in database
        //    var movieFromDb = await GetMovieFromDatabase(nonExistentId);
        //    Assert.Null(movieFromDb);

        //    // Act
        //    var response = await _client.GetAsync($"/api/movies/{nonExistentId}");

        //    // Assert
        //    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        //}

        //[Fact]
        //public async Task PostMovie_WithValidData_CreatesMovieInDatabase()
        //{
        //    // Arrange
        //    var initialCount = (await GetAllMoviesFromDatabase()).Count;

        //    var newMovie = new Movie
        //    {
        //        Title = $"Integration Test Movie {DateTime.Now:yyyyMMddHHmmss}",
        //        ReleaseDate = new DateTime(2024, 3, 15),
        //        Overview = "Created by integration test",
        //        Genre = "Test",
        //        Price = 15.99m
        //    };

        //    // Act
        //    var response = await _client.PostAsJsonAsync("/api/movies", newMovie);

        //    // Assert
        //    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        //    var createdMovie = await response.Content.ReadFromJsonAsync<Movie>();

        //    Assert.NotNull(createdMovie);
        //    Assert.Equal(newMovie.Title, createdMovie.Title);
        //    Assert.Equal(newMovie.Genre, createdMovie.Genre);
        //    Assert.Equal(newMovie.Price, createdMovie.Price);
        //    Assert.NotEqual(Guid.Empty, createdMovie.Id);

        //    // Verify it was actually created in the database
        //    var movieInDb = await GetMovieFromDatabase(createdMovie.Id);
        //    Assert.NotNull(movieInDb);
        //    Assert.Equal(newMovie.Title, movieInDb.Title);

        //    // Verify total count increased
        //    var finalCount = (await GetAllMoviesFromDatabase()).Count;
        //    Assert.Equal(initialCount + 1, finalCount);

        //    // Cleanup - Remove the test movie
        //    await DeleteTestMovie(createdMovie.Id);
        //}

        //[Fact]
        //public async Task PutMovie_WithExistingMovie_UpdatesMovieInDatabase()
        //{
        //    // Arrange - Get an existing movie and create a test movie if none exist
        //    var existingMovies = await GetAllMoviesFromDatabase();
        //    Movie testMovie;

        //    if (!existingMovies.Any())
        //    {
        //        // Create a test movie if database is empty
        //        testMovie = await CreateTestMovie();
        //    }
        //    else
        //    {
        //        testMovie = existingMovies.First();
        //    }

        //    var originalTitle = testMovie.Title;
        //    var originalPrice = testMovie.Price;

        //    // Modify the movie
        //    testMovie.Title = $"Updated {testMovie.Title} {DateTime.Now:HHmmss}";
        //    testMovie.Price = testMovie.Price + 5.00m;

        //    // Act
        //    var response = await _client.PutAsJsonAsync($"/api/movies/{testMovie.Id}", testMovie);

        //    // Assert
        //    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        //    // Verify the update in database
        //    var updatedMovie = await GetMovieFromDatabase(testMovie.Id);
        //    Assert.NotNull(updatedMovie);
        //    Assert.Equal(testMovie.Title, updatedMovie.Title);
        //    Assert.Equal(testMovie.Price, updatedMovie.Price);

        //    // Cleanup - Restore original values if we used an existing movie
        //    if (existingMovies.Any())
        //    {
        //        testMovie.Title = originalTitle;
        //        testMovie.Price = originalPrice;
        //        await _client.PutAsJsonAsync($"/api/movies/{testMovie.Id}", testMovie);
        //    }
        //}

        //[Fact]
        //public async Task PutMovie_WithMismatchedId_ReturnsBadRequest()
        //{
        //    // Arrange - Get or create a test movie
        //    var existingMovies = await GetAllMoviesFromDatabase();
        //    Movie testMovie;

        //    if (!existingMovies.Any())
        //    {
        //        testMovie = await CreateTestMovie();
        //    }
        //    else
        //    {
        //        testMovie = existingMovies.First();
        //    }

        //    var differentId = Guid.NewGuid();

        //    // Act
        //    var response = await _client.PutAsJsonAsync($"/api/movies/{differentId}", testMovie);

        //    // Assert
        //    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        //    // Cleanup if we created a test movie
        //    if (!existingMovies.Any())
        //    {
        //        await DeleteTestMovie(testMovie.Id);
        //    }
        //}

        //[Fact]
        //public async Task DeleteMovie_WithExistingMovie_RemovesMovieFromDatabase()
        //{
        //    // Arrange - Create a test movie specifically for deletion
        //    var testMovie = await CreateTestMovie();
        //    var initialCount = (await GetAllMoviesFromDatabase()).Count;

        //    // Act
        //    var response = await _client.DeleteAsync($"/api/movies/{testMovie.Id}");

        //    // Assert
        //    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        //    // Verify deletion in database
        //    var deletedMovie = await GetMovieFromDatabase(testMovie.Id);
        //    Assert.Null(deletedMovie);

        //    // Verify count decreased
        //    var finalCount = (await GetAllMoviesFromDatabase()).Count;
        //    Assert.Equal(initialCount - 1, finalCount);
        //}

        //[Fact]
        //public async Task DeleteMovie_WithNonExistentId_ReturnsNotFound()
        //{
        //    // Arrange
        //    var nonExistentId = Guid.NewGuid();

        //    // Verify it doesn't exist
        //    var movieFromDb = await GetMovieFromDatabase(nonExistentId);
        //    Assert.Null(movieFromDb);

        //    // Act
        //    var response = await _client.DeleteAsync($"/api/movies/{nonExistentId}");

        //    // Assert
        //    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        //}

        //// Helper method to create a test movie for tests that need one
        //private async Task<Movie> CreateTestMovie()
        //{
        //    var testMovie = new Movie
        //    {
        //        Title = $"Test Movie for Integration {DateTime.Now:yyyyMMddHHmmss}",
        //        ReleaseDate = DateTime.Now.AddDays(-30),
        //        Overview = "Created for integration testing",
        //        Genre = "Test",
        //        Price = 10.99m
        //    };

        //    var response = await _client.PostAsJsonAsync("/api/movies", testMovie);
        //    response.EnsureSuccessStatusCode();

        //    var createdMovie = await response.Content.ReadFromJsonAsync<Movie>();
        //    return createdMovie!;
        //}

        //// Helper method to delete test movies
        //private async Task DeleteTestMovie(Guid movieId)
        //{
        //    try
        //    {
        //        await _client.DeleteAsync($"/api/movies/{movieId}");
        //    }
        //    catch
        //    {
        //        // Ignore errors during cleanup
        //    }
        //}
    }
}
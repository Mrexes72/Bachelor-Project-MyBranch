using Microsoft.Extensions.Logging;

namespace Backend.DAL
{
    public class TestRepository : ITestRepository
    {
        private readonly ILogger<TestRepository> _logger;
        private readonly AppDbContext _context;

        public TestRepository(ILogger<TestRepository> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public string GetTestMessage()
        {
            _logger.LogInformation("TestRepository.GetTestMessage() was called.");
            return "Hello from the repository!";
        }

        public object TestDatabaseConnection()
        {
            try
            {
                // Check if we can connect to the database
                bool canConnect = _context.Database.CanConnect();

                // Get some simple stats to confirm we can read data
                int categoriesCount = _context.Categories.Count();
                int imagesCount = _context.Images.Count();
                int ingredientsCount = _context.Ingredients.Count();

                _logger.LogInformation("Database connection test successful. Found {CategoriesCount} categories, {ImagesCount} images, and {IngredientsCount} ingredients.",
                    categoriesCount, imagesCount, ingredientsCount);

                return new
                {
                    Connected = canConnect,
                    Stats = new
                    {
                        CategoriesCount = categoriesCount,
                        ImagesCount = imagesCount,
                        IngredientsCount = ingredientsCount
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database connection test failed");
                throw; // Re-throw so the controller can handle it
            }
        }
    }
}
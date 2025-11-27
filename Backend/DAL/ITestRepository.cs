namespace Backend.DAL
{
    public interface ITestRepository
    {
        string GetTestMessage();
        object TestDatabaseConnection();
    }
}
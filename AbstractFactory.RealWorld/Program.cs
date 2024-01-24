// EN: Real World Example for the Abstract Factory design pattern
//
// Need: Provide different infrastructure connectors for different
// environments, for example to mock some dependencies in testing
// environments, use cloud services in production, etc.
//
// Solution: Create an abstract factory to supply variants of file systems,
// databases and log providers. There is a concrete factory for each
// environment. This factory is configured to provide different concrete
// connectors for each type of environment. For example, in development we
// use the console to log messages, whereas in production we use the Sentry
// service.

namespace RefactoringGuru.DesignPatterns.AbstractFactory.RealWorld;

// EN: First of all create some abstract products = connectors
interface IDatabase
{
    void Connect();
}

interface IFileSystem
{
    void Read();
}

interface ILogger
{
    void Log();
}

// EN: Declare the different concrete product variants
class SqlServerDatabase : IDatabase
{
    public void Connect()
    {
        Console.WriteLine("Connected to SQL Server");
    }
}

class InMemoryDatabase : IDatabase
{
    public void Connect()
    {
        Console.WriteLine("Connected to in-memory database");
    }
}

class LocalFileSystem : IFileSystem
{
    public void Read()
    {
        Console.WriteLine("Reading file from local file system");
    }
}

class CloudFileSystem : IFileSystem
{
    public void Read()
    {
        Console.WriteLine("Reading file from cloud file system");
    }
}

class ConsoleLogger : ILogger
{
    public void Log()
    {
        Console.WriteLine("Logging to console");
    }
}

class SeqLogger : ILogger
{
    public void Log()
    {
        Console.WriteLine("Logging to Seq");
    }
}

// EN: Then create the abstract factory
interface IEnvironmentFactory
{
    IDatabase CreateDatabase();
    IFileSystem CreateFileSystem();
    ILogger CreateLogger();
}

// EN: Finally create a concrete factory, one for each environment. Each
// factory produces different concrete products = connectors, depending on
// each environment needs
class DevelopmentEnvironmentFactory : IEnvironmentFactory
{
    public IDatabase CreateDatabase()
    {
        return new InMemoryDatabase();
    }

    public IFileSystem CreateFileSystem()
    {
        return new LocalFileSystem();
    }

    public ILogger CreateLogger()
    {
        return new ConsoleLogger();
    }
}

class ProductionEnvironmentFactory : IEnvironmentFactory
{
    public IDatabase CreateDatabase()
    {
        return new SqlServerDatabase();
    }

    public IFileSystem CreateFileSystem()
    {
        return new CloudFileSystem();
    }

    public ILogger CreateLogger()
    {
        return new SeqLogger();
    }
}

class Client
{
    // EN: The client function receives a factory to produce what it needs to
    // execute the application. It's not concerned about the environment.
    public void ClientMethod(IEnvironmentFactory factory)
    {
        var database = factory.CreateDatabase();
        var fileSystem = factory.CreateFileSystem();
        var logger = factory.CreateLogger();

        database.Connect();
        fileSystem.Read();
        logger.Log();
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("Testing client code with the development environment factory type...");
        new Client().ClientMethod(new DevelopmentEnvironmentFactory());
        
        Console.WriteLine();

        Console.WriteLine("Testing the same client code with the production environment factory type...");
        new Client().ClientMethod(new ProductionEnvironmentFactory());
    }
}


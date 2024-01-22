//EN: Real World Example for the Factory Method design pattern
//
// Need: Create different database connectors and be able to switch the
// connector with an environment variable
// Solution: Create an abstract class with a factory method that returns
// a concrete implementation of a database connection

namespace RefactoringGuru.DesignPatterns.FactoryMethod.RealWorld;



// EN: Interface with the factory method
interface IDbConnectionFactory
{
    IDbConnection CreateDbConnection();
}

// EN: Concrete factories, each of them produces a concrete connection
class MongoDbConnectionFactory : IDbConnectionFactory
{
    public IDbConnection CreateDbConnection()
    {
        return new MongoDbConnection();
    }
}

class SqlServerDbConnectionFactory : IDbConnectionFactory
{
    public IDbConnection CreateDbConnection()
    {
        return new SqlServerDbConnection();
    }
}

// EN: Interface product to be created = database connection
interface IDbConnection
{
    void Connect();
}

// EN: Concrete product to be created = database connection
class MongoDbConnection : IDbConnection
{
    public void Connect()
    {
        Console.WriteLine("Connected to MongoDB");
    }
}

class SqlServerDbConnection : IDbConnection
{
    public void Connect()
    {
        Console.WriteLine("Connected to SqlServer");
    }
}

class Client
{
    // EN: The client method accepts any concrete factory
    public void ClientCode(IDbConnectionFactory dbConnectionFactory)
    {
        var dbConnection = dbConnectionFactory.CreateDbConnection();
        dbConnection.Connect();
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("App: Launched with the MongoDbConnectionFactory.");
        new Client().ClientCode(new MongoDbConnectionFactory());
            
        Console.WriteLine("");

        Console.WriteLine("App: Launched with the SqlServerDbConnectionFactory.");
        new Client().ClientCode(new SqlServerDbConnectionFactory());
    }
}
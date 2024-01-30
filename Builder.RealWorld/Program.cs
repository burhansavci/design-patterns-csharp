// EN: Builder Design Pattern
//
// Intent: Lets you construct complex objects step by step. The pattern allows
// you to produce different types and representations of an object using the
// same construction code.
//
// Example: One of the best applications of the Builder pattern is an SQL query
// builder. The Builder interface defines the common steps required to build a
// generic SQL query. On the other hand, Concrete Builders, corresponding to
// different SQL dialects, implement these steps by returning parts of SQL
// queries that can be executed in a particular database engine.

namespace RefactoringGuru.DesignPatterns.Builder.RealWorld;

// EN: The Builder interface declares a set of methods to assemble an SQL query.
//
// All of the construction steps are returning the current builder object to
// allow chaining: builder.Select(...).Where(...)

interface ISqlQueryBuilder
{
    public ISqlQueryBuilder Select(string table, params string[] fields);

    public ISqlQueryBuilder Where(string field, string value, string @operator = "=");

    public ISqlQueryBuilder Limit(int start, int offset);

    // EN: +100 other SQL syntax methods...

    public string GetSql();
}

// EN: Each Concrete Builder corresponds to a specific SQL dialect and may
// implement the builder steps a little bit differently from the others.
//
// This Concrete Builder can build SQL queries compatible with MySQL.

class MySqlQueryBuilder : ISqlQueryBuilder
{
    protected string BaseQuery = string.Empty;
    protected readonly List<string> WhereQuery = [];
    protected string LimitQuery = string.Empty;
    protected string Type = string.Empty;

    protected void Reset()
    {
        BaseQuery = string.Empty;
        Type = string.Empty;
    }

    // EN: Build a base SELECT query.
    public ISqlQueryBuilder Select(string table, params string[] fields)
    {
        Reset();
        BaseQuery = $"SELECT {string.Join(", ", fields)} FROM {table}";
        Type = "select";

        return this;
    }

    // EN: Add a WHERE condition.
    public ISqlQueryBuilder Where(string field, string value, string @operator = "=")
    {
        if (Type != "select" && Type != "update" && Type != "delete")
        {
            throw new Exception("WHERE can only be added to SELECT, UPDATE or DELETE");
        }

        WhereQuery.Add($"{field} {@operator} '{value}'");

        return this;
    }

    // EN: Add a LIMIT constraint.
    public virtual ISqlQueryBuilder Limit(int start, int offset)
    {
        if (Type != "select")
        {
            throw new Exception("LIMIT can only be added to SELECT");
        }

        LimitQuery = $" LIMIT {start}, {offset}";

        return this;
    }

    // EN: Get the final query string.
    public string GetSql()
    {
        var sql = BaseQuery;
        if (WhereQuery.Count != 0)
        {
            sql += $" WHERE {string.Join(" AND ", WhereQuery)}";
        }

        if (!string.IsNullOrWhiteSpace(LimitQuery))
        {
            sql += LimitQuery;
        }

        sql += ";";

        return sql;
    }
}

// EN: This Concrete Builder is compatible with PostgreSQL. While Postgres is
// very similar to Mysql, it still has several differences. To reuse the common
// code, we extend it from the MySQL builder, while overriding some of the
// building steps.

class PostgresQueryBuilder : MySqlQueryBuilder
{
    // EN: Among other things, PostgreSQL has slightly different LIMIT syntax.
    public override ISqlQueryBuilder Limit(int start, int offset)
    {
        base.Limit(start, offset);

        LimitQuery = $" LIMIT {start} OFFSET {offset}";

        return this;
    }

    // EN: + tons of other overrides...
}

class Client
{
    // EN: Note that the client code uses the builder object directly. A designated
    // Director class is not necessary in this case, because the client code needs
    // different queries almost every time, so the sequence of the construction
    // steps cannot be easily reused.
    //
    // Since all our query builders create products of the same type (which is a
    // string), we can interact with all builders using their common interface.
    // Later, if we implement a new Builder class, we will be able to pass its
    // instance to the existing client code without breaking it thanks to the
    // SQLQueryBuilder interface.

    public void ClientCode(ISqlQueryBuilder queryBuilder)
    {
        var query = queryBuilder.Select("users", ["name", "email", "password"])
            .Where("age", "18", ">")
            .Where("age", "30", "<")
            .Limit(10, 20)
            .GetSql();

        Console.WriteLine(query);
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Testing MySql query builder:");
        var mySqlQueryBuilder = new MySqlQueryBuilder();
        new Client().ClientCode(mySqlQueryBuilder);

        Console.WriteLine();

        Console.WriteLine("Testing PostgresSQL query builder:");
        var postgresQueryBuilder = new PostgresQueryBuilder();
        new Client().ClientCode(postgresQueryBuilder);
    }
}
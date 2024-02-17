// EN: Chain of Responsibility Design Pattern
// 
// Intent: Lets you pass requests along a chain of handlers. Upon receiving a
// request, each handler decides either to process the request or to pass it to
// the next handler in the chain.
// 
// Example: The most widely known use of the Chain of Responsibility (CoR)
// pattern in the CSharp world is found in HTTP request middleware.
// 
// It works like this: an HTTP request must pass through a stack of middleware
// objects in order to be handled by the app. Each middleware can either reject
// the further processing of the request or pass it to the next middleware. Once
// the request successfully passes all middleware, the primary handler of the
// app can finally handle it.
// 
// You might have noticed that this approach is kind of inverse to the original
// intent of the pattern. Indeed, in the typical implementation, a request is
// only passed along a chain if a current handler CANNOT process it, while a
// middleware passes the request further down the chain when it thinks that the
// app CAN handle the request. Nevertheless, since middleware are chained, the
// whole concept is still considered an example of the CoR pattern.

namespace RefactoringGuru.DesignPatterns.ChainOfResponsibility.RealWorld;

// EN: The classic CoR pattern declares a single role for objects that make up a
// chain, which is a Handler. In our example, let's differentiate between
// middleware and a final application's handler, which is executed when a
// request gets through all the middleware objects.
// 
// The base Middleware class declares an interface for linking middleware
// objects into a chain.

abstract class Middleware
{
    private Middleware? _next;

    // EN: This method can be used to build a chain of middleware objects.
    public Middleware LinkWith(Middleware next)
    {
        _next = next;
        return next;
    }

    // EN: Subclasses must override this method to provide their own checks. A
    // subclass can fall back to the parent implementation if it can't process a
    // request.
    public virtual bool Check(string email, string password)
    {
        if (_next is null)
        {
            return true;
        }

        return _next.Check(email, password);
    }
}
// EN: This Concrete Middleware checks whether a user with given credentials
// exists.

class UserExistsMiddleware : Middleware
{
    private Server _server;

    public UserExistsMiddleware(Server server)
    {
        _server = server;
    }

    public override bool Check(string email, string password)
    {
        if (!_server.HasEmail(email))
        {
            Console.WriteLine("UserExistsMiddleware: This email is not registered");
            return false;
        }

        if (!_server.IsValidPassword(email, password))
        {
            Console.WriteLine("UserExistsMiddleware: Wrong password!");
            return false;
        }

        return base.Check(email, password);
    }
}

// EN: This Concrete Middleware checks whether a user associated with the
// request has sufficient permissions.

class RoleCheckMiddleware : Middleware
{
    public override bool Check(string email, string password)
    {
        if (email == "admin@example.com")
        {
            Console.WriteLine("RoleCheckMiddleware: Hello, admin!");
            return base.Check(email, password);
        }

        Console.WriteLine("RoleCheckMiddleware: Hello, user!");

        return base.Check(email, password);
    }
}

// EN: This Concrete Middleware checks whether there are too many failed login
// requests.

class ThrottlingMiddleware : Middleware
{
    private readonly int _requestPerMinute;
    private int _request;
    private DateTime _currentTime;

    public ThrottlingMiddleware(int requestPerMinute)
    {
        _requestPerMinute = requestPerMinute;
        _currentTime = DateTime.UtcNow;
    }

    // EN: Please, note that the base.Check call can be inserted both at the
    // beginning of this method and at the end. 
    // This gives much more flexibility than a simple loop over all middleware
    // objects. For instance, a middleware can change the order of checks by
    // running its check after all the others.
    public override bool Check(string email, string password)
    {
        if (DateTime.UtcNow > _currentTime.AddMinutes(1))
        {
            _request = 0;
            _currentTime = DateTime.UtcNow;
        }

        _request++;

        if (_request > _requestPerMinute)
        {
            Console.WriteLine("ThrottlingMiddleware: Request limit exceeded!");
            return false;
        }

        return base.Check(email, password);
    }
}

// EN: This is an application's class that acts as a real handler. The Server
// class uses the CoR pattern to execute a set of various authentication
// middleware before launching some business logic associated with a request.

class Server
{
    private Dictionary<string, string> _users = new();
    private Middleware _middleware;

    // EN: The client can configure the server with a chain of middleware
    // objects.
    public void SetMiddleware(Middleware middleware)
    {
        _middleware = middleware;
    }

    // EN: The server gets the email and password from the client and sends the
    // authorization request to the middleware.
    public bool LogIn(string email, string password)
    {
        if (_middleware.Check(email, password))
        {
            Console.WriteLine("Server: Authorization have been successful!");
            // EN: Do something useful for authorized users.
            return true;
        }

        return false;
    }

    public void Register(string email, string password)
    {
        _users[email] = password;
    }

    public bool HasEmail(string email)
    {
        return _users.ContainsKey(email);
    }

    public bool IsValidPassword(string email, string password)
    {
        return _users.TryGetValue(email, out var value) && value == password;
    }
}

class Program
{
    static void Main(string[] args)
    {
        // EN: The client code.

        var server = new Server();
        server.Register("admin@example.com", "admin_pass");
        server.Register("user@example.com", "user_pass");

        // EN: All middleware are chained. The client can build various chains
        // using the same components.

        var middleware = new ThrottlingMiddleware(2);
        middleware.LinkWith(new UserExistsMiddleware(server))
            .LinkWith(new RoleCheckMiddleware());

        // EN: The server gets a chain from the client code.
        server.SetMiddleware(middleware);

        bool success;
        do
        {
            Console.WriteLine("Enter your email:");
            var email = Console.ReadLine();
            Console.WriteLine("Enter your password:");
            var password = Console.ReadLine();
            Console.WriteLine();
            success = server.LogIn(email, password);
        } while (!success);
    }
}
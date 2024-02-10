using System.Diagnostics;

namespace RefactoringGuru.DesignPatterns.Decorator.RealWorld;

// These interfaces are relevant to the Real World example.
// They are not part of the Decorator pattern.

class ControllerRequest
{
    public string Url { get; set; }
    public string Method { get; set; }
    public object? Data { get; set; }
}

class ControllerResponse
{
    public int StatusCode { get; set; }
    public object? Data { get; set; }
}

interface Controller
{
    public ControllerResponse Execute(ControllerRequest request);
}

class UserController : Controller
{
    public ControllerResponse Execute(ControllerRequest request)
    {
        List<dynamic> user =
        [
            new { id = 1, name = "John" },
            new { id = 2, name = "Bob" },
            new { id = 3, name = "Alice" }
        ];

        var response = new ControllerResponse
        {
            StatusCode = 200,
        };

        if (request.Method == "GET")
        {
            response.Data = user;
        }
        else
        {
            response.StatusCode = 405;
            response.Data = "Method Not Allowed";
        }

        return response;
    }
}

class Decorator : Controller
{
    protected Controller _controller;

    public Decorator(Controller controller)
    {
        _controller = controller;
    }

    public virtual ControllerResponse Execute(ControllerRequest request)
    {
        return _controller.Execute(request);
    }
}

class TelemetryDecorator : Decorator
{
    public TelemetryDecorator(Controller controller) : base(controller)
    {
    }

    public override ControllerResponse Execute(ControllerRequest request)
    {
        var sw = new Stopwatch();
        sw.Start();
        Task.Delay(100).Wait();
        var response = base.Execute(request);
        sw.Stop();
        Console.WriteLine($"Telemetry: {sw.ElapsedMilliseconds}ms");
        return response;
    }
}

class LoggingDecorator : Decorator
{
    public LoggingDecorator(Controller controller) : base(controller)
    {
    }

    public override ControllerResponse Execute(ControllerRequest request)
    {
        Console.WriteLine($"Logging: {request.Method} request to {request.Url}");
        return base.Execute(request);
    }
}

class Program
{
    static void Main(string[] args)
    {
        var userController = new UserController();
        var userControllerWithTelemetry = new TelemetryDecorator(userController);
        var userControllerWithTelemetryAndLogging = new LoggingDecorator(userControllerWithTelemetry);

        var request = new ControllerRequest
        {
            Url = "/users",
            Method = "GET",
        };

        userControllerWithTelemetryAndLogging.Execute(request);
    }
}
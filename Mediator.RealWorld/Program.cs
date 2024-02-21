// EN: Real World Example for the Mediator design pattern
//
// Need: To have a messaging application to notify groups of people. Users
// should not know about each other.
//
// Solution: Create a mediator to manage subscriptions and messages

namespace RefactoringGuru.DesignPatterns.Mediator.RealWorld;

// EN: Extending the Mediator interface to have a payload to include messages
interface IMediator
{
    void Notify(User sender, string @event, string? payload);
}

// EN: The user plays the role of the independent component. It has an
// instance of the mediator.

class User
{
    private IMediator _mediator;
    public string Name { get; private set; }

    public User(IMediator mediator, string name)
    {
        _mediator = mediator;
        Name = name;
        _mediator.Notify(this, "subscribe", null);
    }

    public void PublishMessage(string message)
    {
        _mediator.Notify(this, "publish", message);
    }

    public void ReceiveMessage(string message)
    {
        Console.WriteLine($"Message received by {Name}: {message}");
    }
}

// EN: The app is the concrete Mediator and implements all the events that
// collaborators can notify: subscribe and publish

class ChatAppMediator : IMediator
{
    private readonly List<User> _users = [];

    public void Notify(User sender, string @event, string? payload)
    {
        if (@event == "subscribe")
        {
            _users.Add(sender);
            Console.WriteLine($"{sender.Name} subscribed to the chat");
        }
        else if (@event == "publish")
        {
            foreach (var user in _users.Where(user => user != sender))
            {
                user.ReceiveMessage(payload!);
            }
        }
    }
}

// EN: The client code. Creating a user automatically subscribes them to the
// group.

class Program
{
    static void Main(string[] args)
    {
        var mediator = new ChatAppMediator();
        var user1 = new User(mediator, "Lightning");
        var user2 = new User(mediator, "Doc");
        var user3 = new User(mediator, "Mater");
        
        user1.PublishMessage("Catchaw");
        user2.PublishMessage("Ey kid");
        user3.PublishMessage("Tomato");
    }
}
// EN: Adapter Design Pattern
// 
// Intent: Provides a unified interface that allows objects with incompatible
// interfaces to collaborate.
// 
// Example: The Adapter pattern allows you to use 3rd-party or legacy classes
// even if they are incompatible with the bulk of your code. For example,
// instead of rewriting the notification interface of your app to support each
// 3rd-party service such as Slack, Facebook, SMS or (you-name-it), you can
// create a set of special wrappers that adapt calls from your app to an
// interface and format required by each 3rd-party class. 

namespace RefactoringGuru.DesignPatterns.Adapter.RealWorld;

// EN: The Target interface represents the interface that your application's
// classes already follow.
interface INotification
{
    void Send(string message);
}

// EN: Here's an example of the existing class that follows the Target
// interface.
// The truth is that many real apps may not have this interface clearly defined.
// If you're in that boat, your best bet would be to extend the Adapter from one
// of your application's existing classes. If that's awkward (for instance,
// SlackNotification doesn't feel like a subclass of EmailNotification), then
// extracting an interface should be your first step.
class EmailNotification : INotification
{
    public void Send(string message)
    {
        Console.WriteLine($"Message: {message} sent via Email.");
    }
}

// EN: The Adaptee is some useful class, incompatible with the Target interface.
// You can't just go in and change the code of the class to follow the Target
// interface, since the code might be provided by a 3rd-party library.
class SlackApi
{
    public void Login()
    {
        // Send authentication request to Slack web service.
        Console.WriteLine("Logged in to a slack account.");
    }

    public void SendMessage(string message)
    {
        // Send message post request to Slack web service.
        Console.WriteLine($"Message: {message} sent via Slack.");
    }
}

// EN: The Adapter is a class that links the Target interface and the Adaptee
// class. In this case, it allows the application to send notifications using
// Slack API.
class SlackNotification : INotification
{
    private readonly SlackApi _slackApi;

    public SlackNotification(SlackApi slackApi)
    {
        _slackApi = slackApi;
    }


    // EN: An Adapter is not only capable of adapting interfaces, but it can
    // also convert incoming data to the format required by the Adaptee.
    public void Send(string message)
    {
        var slackMessage = $"Slack {message}";
        _slackApi.Login();
        _slackApi.SendMessage(slackMessage);
    }
}

// EN: The client code can work with any class that follows the Target 
// interface.
class Client
{
    public void ClientCode(INotification notification)
    {
        notification.Send("Message from client");
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Client code is designed correctly and works with email notifications";
        var emailNotification = new EmailNotification();
        new Client().ClientCode(emailNotification);

        Console.WriteLine();

        // The same client code can work with other classes via adapter";
        var slackApi = new SlackApi();
        var slackNotification = new SlackNotification(slackApi);
        new Client().ClientCode(slackNotification);
    }
}
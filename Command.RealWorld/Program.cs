// EN: Real World Example for the Command Design Pattern
//
// Need: Execute a command to retrieve random information every X seconds
//
// Solution: Create an object that has all the information to execute the query

using System.Text.Json;
using System.Text.Json.Serialization;

namespace RefactoringGuru.DesignPatterns.Command.RealWorld;

// EN: The Command interface declares a method for executing a command.

interface ICommand
{
    Task Execute();
}

// EN: We will use a receiver object to run the business logic

class PrintRandomFactCommand : ICommand
{
    private readonly RandomFactDomainServiceReceiver _receiver;

    public PrintRandomFactCommand(RandomFactDomainServiceReceiver receiver)
    {
        _receiver = receiver;
    }

    public async Task Execute()
    {
        var fact = await RandomFactDomainServiceReceiver.GetRandomFact();
        Console.WriteLine(fact);
    }
}

// EN: The Receiver class contains all the business logic to retrieve the
// information

class RandomFactDomainServiceReceiver
{
    public static async Task<string> GetRandomFact()
    {
        var client = new HttpClient();
        var response = await client.GetAsync("https://uselessfacts.jsph.pl/api/v2/facts/random");
        var factStream = await response.Content.ReadAsStreamAsync();
        var factObject = await JsonSerializer.DeserializeAsync<RandomFact>(factStream);

        return factObject!.Text;
    }
}

class RandomFact
{
    [JsonPropertyName("text")] 
    public string Text { get; set; }
}

// EN: The Invoker will execute any command every X seconds.

class CommandInvoker
{
    private readonly ICommand _command;
    private readonly int _interval;

    public CommandInvoker(ICommand command, int interval = 5)
    {
        _command = command;
        _interval = interval;
    }

    public async Task Start()
    {
        while (true)
        {
            await _command.Execute();
            await Task.Delay(TimeSpan.FromSeconds(_interval));
        }
    }
}

// EN: The client code invokes the command

class Program
{
    static async Task Main(string[] args)
    {
        var receiver = new RandomFactDomainServiceReceiver();
        var command = new PrintRandomFactCommand(receiver);
        var invoker = new CommandInvoker(command, 3);
        await invoker.Start();
    }
}
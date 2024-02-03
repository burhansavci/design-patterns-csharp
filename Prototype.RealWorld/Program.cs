// EN: Real World Example for the Prototype design pattern
//
// Need: To have a Document class with several components that can be copied & pasted
//
// Solution: All components should have the ability to clone themselves
namespace RefactoringGuru.DesignPatterns.Prototype.RealWorld;

interface IComponent
{
    IComponent Clone();
    void Print();
}

class Document : IComponent
{
    private List<IComponent> _components = [];

    public IComponent Clone()
    {
        var clone = (Document)MemberwiseClone();
        clone._components = _components.Select(c => c.Clone()).ToList();
        return clone;
    }

    public void AddComponent(IComponent component)
    {
        _components.Add(component);
    }

    public void Print()
    {
        foreach (var component in _components)
        {
            component.Print();
        }
    }
}

// EN: Some components to add to a document
class Title : IComponent
{
    public string Text { get; set; }

    public IComponent Clone()
    {
        return (Title)MemberwiseClone();
    }

    public void Print()
    {
        Console.WriteLine($"Title: {Text}");
    }
}

class Drawing : IComponent
{
    public string Shape { get; set; }

    public IComponent Clone()
    {
        return (Drawing)MemberwiseClone();
    }

    public void Print()
    {
        Console.WriteLine($"Drawing: {Shape}");
    }
}

class TextBox : IComponent
{
    public string Text { get; set; }

    public IComponent Clone()
    {
        return (TextBox)MemberwiseClone();
    }

    public void Print()
    {
        Console.WriteLine($"TextBox: {Text}");
    }
}

class Link : IComponent
{
    public string Text { get; set; }
    public string Url { get; set; }

    public IComponent Clone()
    {
        return (Link)MemberwiseClone();
    }

    public void Print()
    {
        Console.WriteLine($"Link: {Text} - {Url}");
    }
}

// EN: The client code.
class Program
{
    static void Main()
    {
        var document = new Document();
        var title = new Title { Text = "Prototype Design Pattern" };
        var textBox = new TextBox { Text = "Hello, world!" };

        document.AddComponent(title);
        document.AddComponent(new Drawing { Shape = "Square" });
        document.AddComponent(textBox);
        document.AddComponent(new Link { Text = "Click me", Url = "https://refactoring.guru/design-patterns/prototype" });

        var clonedDocument = document.Clone();

        title.Text = "New title for the original document";
        textBox.Text = "New textbox text for the original document";

        Console.WriteLine("Original document:");
        document.Print();

        Console.WriteLine();

        Console.WriteLine("Cloned document:");
        clonedDocument.Print();
    }
}
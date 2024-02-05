// EN: Real World Example for the Bridge Design Pattern
// 
// Need: Define different views (with or w/o icon, with or w/o description...)
// for different content types in a list inside a webpage
// 
// Solution: Create a bridge for the 2 dimensions:
//             - View (abstraction)
//             - Content type (implementation)
// 
// Credits to: https://github.com/martincrb/bridge-design-pattern/blob/main/bridgePattern.ts

namespace RefactoringGuru.DesignPatterns.Bridge.RealWorld;

// EN: The Abstraction defines the interface for the first dimension, in this
// case the different list item views we have for a given content
abstract class ListItemViewAbstraction
{
    protected IContentTypeImplementation ContentType;

    protected ListItemViewAbstraction(IContentTypeImplementation contentType)
    {
        ContentType = contentType;
    }

    public abstract string GetRenderedItem();
}

// EN: The Implementation defines the interface for the second dimension = all
// the content types
interface IContentTypeImplementation
{
    public string RenderTitle();
    public string RenderCaption();
    public string RenderThumbnail();
    public string RenderLink();
}

// EN: Now we can extend the Abstraction with the different views and
// independently of the content types
class VisualListItemView : ListItemViewAbstraction
{
    public VisualListItemView(IContentTypeImplementation contentType) : base(contentType)
    {
    }

    public override string GetRenderedItem()
    {
        return $"<li>{ContentType.RenderThumbnail()}{ContentType.RenderLink()}</li>";
    }
}

class DescriptiveListItemView : ListItemViewAbstraction
{
    public DescriptiveListItemView(IContentTypeImplementation contentType) : base(contentType)
    {
    }

    public override string GetRenderedItem()
    {
        return $"<li>{ContentType.RenderTitle()}{ContentType.RenderCaption()}</li>";
    }
}

// EN: Time to create the different implementations, in this case the different
// content types we have in our application: posts, videos, articles, tweets...
class PostContentType : IContentTypeImplementation
{
    private readonly string _title;
    private readonly string _caption;
    private readonly string _imageUrl;
    private readonly string _url;

    public PostContentType(
        string title,
        string caption,
        string imageUrl,
        string url)
    {
        _title = title;
        _caption = caption;
        _imageUrl = imageUrl;
        _url = url;
    }

    public string RenderTitle()
    {
        return $"<h1>{_title}</h1>";
    }

    public string RenderCaption()
    {
        return $"<p>{_caption}</p>";
    }

    public string RenderThumbnail()
    {
        return $"<img src='{_title}' src='{_imageUrl}'>";
    }

    public string RenderLink()
    {
        return $"<a href='{_url}'>{_title}</a>";
    }
}

class VideoContentType : IContentTypeImplementation
{
    private readonly string _title;
    private readonly string _description;
    private readonly string _thumbnailUrl;
    private readonly string _url;

    public VideoContentType(
        string title,
        string description,
        string thumbnailUrl,
        string url)
    {
        _title = title;
        _description = description;
        _thumbnailUrl = thumbnailUrl;
        _url = url;
    }

    public string RenderTitle()
    {
        return $"<h2>{_title}</h2>";
    }

    public string RenderCaption()
    {
        return $"<p>{_description}</p>";
    }

    public string RenderThumbnail()
    {
        return $"<img alt='{_title}' src='{_thumbnailUrl}'>";
    }

    public string RenderLink()
    {
        return $"<a href='{_url}'>{_title}</a>";
    }
}

class TweetContentType : IContentTypeImplementation
{
    private readonly string _tweet;
    private readonly string _profilePictureUrl;
    private readonly string _tweetUrl;

    public TweetContentType(
        string tweet,
        string profilePictureUrl,
        string tweetUrl)
    {
        _tweet = tweet;
        _profilePictureUrl = profilePictureUrl;
        _tweetUrl = tweetUrl;
    }

    public string RenderTitle()
    {
        return $"<h2>{_tweet[..50]}...</h2>";
    }

    public string RenderCaption()
    {
        return $"<p>{_tweet}</p>";
    }

    public string RenderThumbnail()
    {
        return $"<img alt='{_tweet[..50]}...' src='{_profilePictureUrl}'>";
    }

    public string RenderLink()
    {
        return $"<a href='{_tweetUrl}'>{_tweet[..50]}...</a>";
    }
}

// EN: The client code can use any Abstraction to render items
class Client
{
    public void ClientCode(IList<IContentTypeImplementation> content)
    {
        var visualList = content.Select(x => new VisualListItemView(x));
        Console.WriteLine("<h1>Visual Page</h1>");
        Console.WriteLine("<ul>");
        foreach (var visualItem in visualList)
        {
            Console.WriteLine(visualItem.GetRenderedItem());
        }
        Console.WriteLine("</ul>");

        Console.WriteLine();

        var descriptiveList = content.Select(x => new DescriptiveListItemView(x));
        Console.WriteLine("<h1>Descriptive Page</h1>");
        Console.WriteLine("<ul>");
        foreach (var descriptiveItem in descriptiveList)
        {
            Console.WriteLine(descriptiveItem.GetRenderedItem());
        }

        Console.WriteLine("</ul>");
    }
}

class Program
{
    static void Main()
    {
        // EN: The client code only depends on the Abstraction. Now we can extend
        // abstractions (i.e. add new views) without impacting implementations
        // (content types). Also we can add new content types without impacting
        // anything from the views.
        List<IContentTypeImplementation> content =
        [
            new PostContentType(
                "New example available on RefactoringGuru",
                "Bridge design pattern now has a real world example",
                "http://img.sample.org/bridge.jpg",
                "https://refactoring.guru/design-patterns/bridge"
            ),
            new TweetContentType(
                "Windows will support Linux executables natively on Windows 12",
                "http://img.sample.org/profile.jpg",
                "https://twitter.com/genbeta/387487346856/"
            ),
            new VideoContentType(
                "BRIDGE | Patrones de Diseño",
                "En éste vídeo de la serie de PATRONES DE DISEÑO veremos el PATRÓN BRIDGE!",
                "http://img.sample.org/bridge.jpg",
                "https://www.youtube.com/watch?v=6bIHhzqMdgg"
            ),
        ];

        var client = new Client();
        client.ClientCode(content);
    }
}
//  EN: Proxy Design Pattern
//
// Intent: Provide a surrogate or placeholder for another object to control
// access to the original object or to add other responsibilities.
//
// Example: There are countless ways proxies can be used: caching, logging,
// access control, delayed initialization, etc. This example demonstrates how
// the Proxy pattern can improve the performance of a downloader object by
// caching its results.

namespace RefactoringGuru.DesignPatterns.Proxy.RealWorld;

// EN: The Subject interface describes the interface of a real object.
// The truth is that many real apps may not have this interface clearly defined.
// If you're in that boat, your best bet would be to extend the Proxy from one
// of your existing application classes. If that's awkward, then extracting a
// proper interface should be your first step.

interface IDownloader
{
    string Download(string url);
}

// EN: The Real Subject does the real job, albeit not in the most efficient way.
// When a client tries to download the same file for the second time, our
// downloader does just that, instead of fetching the result from cache.
class SimpleDownloader : IDownloader
{
    public string Download(string url)
    {
        Console.WriteLine($"Downloading a file from the Internet. URL: {url}");
        return "Downloaded file";
    }
}

// EN: The Proxy class is our attempt to make the download more efficient. It
// wraps the real downloader object and delegates it the first download calls.
// The result is then cached, making subsequent calls return an existing file
// instead of downloading it again.
//
// Note that the Proxy MUST implement the same interface as the Real Subject.
class CachingDownloader : IDownloader
{
    private readonly SimpleDownloader _downloader;
    private readonly Dictionary<string, string> _cache = new();

    public CachingDownloader(SimpleDownloader downloader)
    {
        _downloader = downloader;
    }

    public string Download(string url)
    {
        if (!_cache.TryGetValue(url, out var value))
        {
            Console.WriteLine("CacheProxy MISS.");
            _cache[url] = _downloader.Download(url);
            return _cache[url];
        }

        Console.WriteLine($"CacheProxy HIT. Retrieving result from cache.");
        return value;
    }
}


// EN: The client code may issue several similar download requests. In this
// case, the caching proxy saves time and traffic by serving results from cache.
//
// The client is unaware that it works with a proxy because it works with
// downloaders via the abstract interface.

class Program
{
    private static void Main(string[] args)
    {
        var simpleDownloader = new SimpleDownloader();
        var cachingDownloader = new CachingDownloader(simpleDownloader);

        Console.WriteLine("Client: Executing the first request...");
        Console.WriteLine(cachingDownloader.Download("http://example.com/"));
        
        // EN: Duplicate download requests could be cached for a speed gain.

        Console.WriteLine("Client: Executing the same request again...");
        Console.WriteLine(cachingDownloader.Download("http://example.com/"));
    }
}
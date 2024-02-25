namespace RefactoringGuru.DesignPatterns.Strategy.RealWorld;

record UploadResult(bool Success, string Message);

interface IUploadStrategy
{
    Task<UploadResult> Upload(string filePath, string name, string content);
}

// Working implementation of the strategy to upload a file to a local directory.
class LocalUpload : IUploadStrategy
{
    public async Task<UploadResult> Upload(string filePath, string name, string content)
    {
        try
        {
            await File.WriteAllTextAsync(filePath + name, content);
            return new UploadResult(true, "File uploaded to local directory");
        }
        catch (Exception ex)
        {
            return new UploadResult(false, $"Error uploading file to local directory:{Environment.NewLine}{ex.Message}");
        }
    }
}

class AWSUpload : IUploadStrategy
{
    public async Task<UploadResult> Upload(string filePath, string name, string content)
    {
        await Task.Delay(1000);
        return new UploadResult(true, "File uploaded to AWS storage");
    }
}

class Context(IUploadStrategy strategy)
{
    public void SetStrategy(IUploadStrategy uploadStrategy)
    {
        strategy = uploadStrategy;
    }

    // EN: The Context delegates some work to the Strategy object instead of
    // implementing multiple versions of the algorithm on its own.
    public async Task<UploadResult> UploadFile(string filePath, string name, string content)
    {
        return await strategy.Upload(filePath, name, content);
    }
}

// I'm creating to different strategies to upload a file to different places.
class Program
{
    static async Task Main(string[] args)
    {
        var localUpload = new LocalUpload();
        var awsUpload = new AWSUpload();
        
        var context = new Context(localUpload);
        // upload to the current directory of the application
        var result = await context.UploadFile("../../../", "Output.txt", "Hello, World!");
        Console.WriteLine(result);
        
        context.SetStrategy(awsUpload);
        result = await context.UploadFile("", "Output.txt", "Hello, World!");
        Console.WriteLine(result);
    }
}
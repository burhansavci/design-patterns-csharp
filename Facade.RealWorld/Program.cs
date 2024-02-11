// Example Facade pattern for a ETL process.
// In this example I have created three subsystems.
// The first one is the Loader (DataSource), which is a file system.
// The second one is the Parser (DataTransformer), which is a string parser.
// The third one is the Writer (DataSink), which is a file system.
// 
// To keep the example simple, in the loader I'm not doing input validation.
// In a real world scenario, I would do it creating a validation layer 
// on the extractor and passing the parsed result to the transformer.

namespace RefactoringGuru.DesignPatterns.Facade.RealWorld;

interface Extractor
{
    string Extract();
}

interface Transformer
{
    string Transform(string data);
}

interface Loader
{
    string Load(string data);
}

class FileExtractor : Extractor
{
    private readonly string _filePath;

    public FileExtractor(string filePath)
    {
        _filePath = filePath;
    }

    public string Extract()
    {
        return $"Extracted data from file {_filePath}";
    }
}

class FileTransformer : Transformer
{
    public string Transform(string data)
    {
        return $"Transformed data: {data}";
    }
}

class FileLoader : Loader
{
    private readonly string _filePath;

    public FileLoader(string filePath)
    {
        _filePath = filePath;
    }

    public string Load(string data)
    {
        return $"Loaded data into file {_filePath}: {data}";
    }
}

// The Facade class is the main class of the Facade pattern.
// It's responsible for creating the subsystems and calling their methods.
// I'm injecting the subsystems in the constructor.
// In the process method I'm calling the extract, transform and load methods of the subsystems.

class EtlProcessor
{
    private readonly Extractor _extractor;
    private readonly Transformer _transformer;
    private readonly Loader _loader;

    public EtlProcessor(Extractor extractor, Transformer transformer, Loader loader)
    {
        _extractor = extractor;
        _transformer = transformer;
        _loader = loader;
    }

    public string Process()
    {
        var data = _extractor.Extract();
        var transformedData = _transformer.Transform(data);
        return _loader.Load(transformedData);
    }
}

class Program
{
    static void Main(string[] args)
    {
        const string filePath = "data.txt";
        var extractor = new FileExtractor(filePath);
        var transformer = new FileTransformer();
        var loader = new FileLoader(filePath);

        var etlProcessor = new EtlProcessor(extractor, transformer, loader);
        var result = etlProcessor.Process();
        Console.WriteLine(result);
    }
}
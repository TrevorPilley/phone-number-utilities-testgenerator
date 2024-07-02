var dataFilePath = args.SingleOrDefault(x => x.StartsWith("/dataFilePath=", StringComparison.Ordinal))?.Split('=')[1];

if (string.IsNullOrWhiteSpace(dataFilePath))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.Write("The data file to be parsed must be specified with the argument '/dataFilePath='");
    Console.ForegroundColor = ConsoleColor.White;
    Environment.Exit(-1);
}

if (!Directory.Exists(dataFilePath))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.Write($"The data file path {dataFilePath} does not exist.");
    Console.ForegroundColor = ConsoleColor.White;
    Environment.Exit(-1);
}

foreach (var dataFile in Directory.EnumerateFiles(dataFilePath, "*.txt"))
{
    Console.WriteLine("Processing file {0}", Path.GetFileName(dataFile));

    var solutionRoot = Path.GetDirectoryName(dataFile)!;

    // Traverse up from src/PhoneNumbers/DataFiles
    for (int i = 0; i < 3; i++)
    {
        solutionRoot = Directory.GetParent(solutionRoot)!.FullName;
    }
    
    var countryCode = Path.GetFileNameWithoutExtension(dataFile).ToUpperInvariant();
    
    var customParser = File.Exists(
        Path.Combine(Directory.GetParent(Path.GetDirectoryName(dataFile))!.FullName, "Parsers", countryCode + "PhoneNumberParser.cs"));

    var testOutputPath = Path.Combine(
        solutionRoot,
        "test",
        customParser ? "PhoneNumbers.Tests" : "PhoneNumbers.Data.Tests",
        "Parsers");

    var data = DataFileReader.ReadData(dataFile);

    TestFileWriter.WriteTests(countryCode, testOutputPath, customParser, data);
}

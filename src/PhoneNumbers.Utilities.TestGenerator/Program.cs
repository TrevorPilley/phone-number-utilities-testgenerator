var dataFilePath = args.SingleOrDefault(x => x.StartsWith("/dataFilePath=", StringComparison.Ordinal))?.Split('=')[1];

if (string.IsNullOrWhiteSpace(dataFilePath))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.Write("The data file to be parsed must be specified with the argument '/dataFilePath='");
    Environment.Exit(-1);
}

if (!Directory.Exists(dataFilePath))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.Write($"The data file path {dataFilePath} does not exist.");
    Environment.Exit(-1);
}

foreach (var dataFile in Directory.EnumerateFiles(dataFilePath, "*.txt"))
{
    var countryCode = Path.GetFileNameWithoutExtension(dataFile).ToUpperInvariant();
    var customParser = File.Exists(Path.Combine(Directory.GetParent(Path.GetDirectoryName(dataFile)).FullName, "Parsers", countryCode + "PhoneNumberParser.cs"));

    var solutionRoot = Path.GetDirectoryName(dataFile)!;

    // Traverse up from src/PhoneNumbers/DataFiles
    for (int i = 0; i < 3; i++)
    {
        solutionRoot = Directory.GetParent(solutionRoot)!.FullName;
    }

    var testOutputPath = customParser
        ? Path.Combine(solutionRoot, "test", "PhoneNumbers.Tests", "Parsers")
        : Path.Combine(solutionRoot, "test", "PhoneNumbers.Data.Tests", "Parsers");

    var data = DataFileReader.ReadData(dataFile);

    Console.WriteLine("Processing file {0}", Path.GetFileName(dataFile));
    TestFileWriter.WriteTests(countryCode, testOutputPath, customParser, data);
}

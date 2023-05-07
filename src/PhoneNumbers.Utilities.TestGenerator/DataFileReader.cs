internal static class DataFileReader
{
    internal static List<CountryNumberDataLine> ReadData(string dataFilePath)
    {
        var data = new List<CountryNumberDataLine>();

        using var dataFile = File.OpenRead(dataFilePath);
        using var dataFileReader = new StreamReader(dataFile);

        string? line;

        while ((line = dataFileReader.ReadLine()) != null)
        {
            if (string.IsNullOrEmpty(line) || line[0] == '#')
            {
                continue;
            }

            // Kind | NdcRanges | GeoArea | SnRanges | Hint
            //  0   |     1     |    2    |    3     |  4
            var lineParts = line.Split('|');
            var ndcRanges = lineParts[1].Split(',');

            // further improve this so we only get the overall lower and upper
            // when the ndc is also a range
            foreach (var ndcRange in ndcRanges)
            {
                foreach (var ndc in ndcRange.Split('-'))
                {
                    var snRanges = lineParts[3].Split(',');

                    foreach (var snPart in snRanges)
                    {
                        foreach (var sn in snPart.Split('-'))
                        {
                            data.Add(new CountryNumberDataLine
                            {
                                NationalDestinationCode = ndc,
                                GeographicArea = lineParts[2],
                                Hint = lineParts[4].Length > 0 ? lineParts[4][0] : '\0',
                                Kind = lineParts[0][0],
                                SubscriberNumber = sn,
                            });
                        }
                    }
                }
            }
        }

        return data;
    }
}

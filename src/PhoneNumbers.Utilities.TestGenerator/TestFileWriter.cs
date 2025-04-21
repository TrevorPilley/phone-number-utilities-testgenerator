using System.Text;
using Humanizer;

internal static class TestFileWriter
{
    private static readonly char LF = '\n';

    private static string LookUpTrunkCode(string countryCode) =>
        countryCode switch
        {
            "AE" or "AL" or "AT" or "AU" or "BA" or "BE" or "BG" or "BR" or "CH" or "DE" or "EG" or "FI" or "FR" or "GB" or "GG" or "HR" or "IE" or "IM" or "IL" or "JE" or "JO" or "KE" or "MD" or "ME" or "MK" or "NL" or "NG" or "NZ" or "RO" or "RS" or "SA" or "SE" or "SK" or "SL" or "TR" or "TZ" or "UA" or "UG" or "XK" or "YE" or "ZA" => "0",
            "HU" => "06",
            "BY" or "LT" => "8",
            _ => string.Empty,
        };

    private static string CountryCodeToCountryInfoName(string countryCode) =>
        countryCode switch
        {
            "AD" => "Andorra",
            "AE" => "UnitedArabEmirates",
            "AG" => "AntiguaAndBarbuda",
            "AI" => "Anguilla",
            "AL" => "Albania",
            "AS" => "AmericanSamoa",
            "AT" => "Austria",
            "AU" => "Australia",
            "BA" => "BosniaAndHerzegovina",
            "BB" => "Barbados",
            "BE" => "Belgium",
            "BG" => "Bulgaria",
            "BM" => "Bermuda",
            "BR" => "Brazil",
            "BS" => "Bahamas",
            "BY" => "Belarus",
            "CA" => "Canada",
            "CH" => "Switzerland",
            "CO" => "Colombia",
            "CZ" => "CzechRepublic",
            "CY" => "Cyprus",
            "DE" => "Germany",
            "DK" => "Denmark",
            "DM" => "Dominica",
            "DO" => "DominicanRepublic",
            "EE" => "Estonia",
            "EG" => "Egypt",
            "ES" => "Spain",
            "FI" => "Finland",
            "FK" => "FalklandIslands",
            "FO" => "FaroeIslands",
            "FR" => "France",
            "GD" => "Grenada",
            "GB" => "UnitedKingdom",
            "GG" => "Guernsey",
            "GI" => "Gibraltar",
            "GR" => "Greece",
            "GU" => "Guam",
            "HK" => "HongKong",
            "HR" => "Croatia",
            "HU" => "Hungary",
            "IE" => "Ireland",
            "IL" => "Israel",
            "IM" => "IsleOfMan",
            "IS" => "Iceland",
            "IT" => "Italy",
            "JO" => "Jordan",
            "JE" => "Jersey",
            "JM" => "Jamaica",
            "LC" => "SaintLucia",
            "LT" => "Lithuania",
            "LI" => "Liechtenstein",
            "LU" => "Luxembourg",
            "LV" => "Latvia",
            "KE" => "Kenya",
            "KN" => "SaintKittsAndNevis",
            "KY" => "CaymanIslands",
            "MC" => "Monaco",
            "MD" => "Moldova",
            "ME" => "Montenegro",
            "MK" => "NorthMacedonia",
            "MO" => "Macau",
            "MP" => "NorthernMarianaIsland",
            "MS" => "Montserrat",
            "MT" => "Malta",
            "MX" => "Mexico",
            "NL" => "Netherlands",
            "NG" => "Nigeria",
            "NO" => "Norway",
            "NZ" => "NewZealand",
            "OM" => "Oman",
            "PG" => "PapuaNewGuinea",
            "PL" => "Poland",
            "PR" => "PuertoRico",
            "PT" => "Portugal",
            "RO" => "Romania",
            "QA" => "Qatar",
            "RS" => "Serbia",
            "SA" => "SaudiArabia",
            "SE" => "Sweden",
            "SG" => "Singapore",
            "SK" => "Slovakia",
            "SL" => "Slovenia",
            "SM" => "SanMarino",
            "SX" => "SintMaarten",
            "TC" => "TurksAndCaicosIslands",
            "TR" => "Turkey",
            "TT" => "TrinidadAndTobago",
            "TZ" => "Tanzania",
            "UA" => "Ukraine",
            "UG" => "Uganda",
            "US" => "UnitedStates",
            "VC" => "SaintVincentAndTheGrenadines",
            "VI" => "UnitedStatesVirginIslands",
            "VG" => "BritishVirginIslands",
            "XK" => "Kosovo",
            "YE" => "Yemen",
            "ZA" => "SouthAfrica",
            _ => throw new NotSupportedException(countryCode),
        };

    internal static void WriteTests(
        string countryCode,
        string testOutputPath,
        bool customParser,
        List<CountryNumberDataLine> dataLines)
    {
        var geoFileName = customParser
            ? $"{countryCode}PhoneNumberParserTests_GeographicPhoneNumber.cs"
            : $"DefaultPhoneNumberParserTests_{countryCode}_GeographicNumber.cs";

        DeleteFileIfExists(geoFileName);

        if (dataLines.Any(x => x.Kind == 'G'))
        {
            using var geoWriter = WriteFileStart(countryCode, Path.Combine(testOutputPath, geoFileName), customParser);
            WriteGeoTests(countryCode, geoWriter, dataLines.Where(x => x.Kind == 'G'));
            WriteFileEnd(geoWriter);
        }

        var mobileFileName = customParser
            ? $"{countryCode}PhoneNumberParserTests_MobilePhoneNumber.cs"
            : $"DefaultPhoneNumberParserTests_{countryCode}_MobilePhoneNumber.cs";

        DeleteFileIfExists(mobileFileName);

        if (dataLines.Any(x => x.Kind == 'M'))
        {
            using var mobileWriter = WriteFileStart(countryCode, Path.Combine(testOutputPath, mobileFileName), customParser);
            WriteMobileTests(countryCode, mobileWriter, dataLines.Where(x => x.Kind == 'M'));
            WriteFileEnd(mobileWriter);
        }

        var nonGeoFileName = customParser
            ? $"{countryCode}PhoneNumberParserTests_NonGeographicPhoneNumber.cs"
            : $"DefaultPhoneNumberParserTests_{countryCode}_NonGeographicPhoneNumber.cs";

        DeleteFileIfExists(nonGeoFileName);

        if (dataLines.Any(x => x.Kind == 'N'))
        {
            using var nonGeoWriter = WriteFileStart(countryCode, Path.Combine(testOutputPath, nonGeoFileName), customParser);
            WriteNonGeoTests(countryCode, nonGeoWriter, dataLines.Where(x => x.Kind == 'N'));
            WriteFileEnd(nonGeoWriter);
        }
    }

    private static void DeleteFileIfExists(string testFilePath)
    {
        if (File.Exists(testFilePath))
        {
            File.Delete(testFilePath);
        }

        // Sometimes the file doesn't get deleted before we re-create it leaving an invalid file if the new content is shorter.
        Thread.Sleep(100);
    }

    private static void WriteFileEnd(StreamWriter writer)
    {
        writer.Write("}" + LF);
        writer.Flush();
        writer.Dispose();
    }

    private static StreamWriter WriteFileStart(string countryCode, string testFilePath, bool customParser)
    {
        Console.WriteLine("\tCreating {0}", Path.GetFileName(testFilePath));

        var fileStream = File.OpenWrite(testFilePath);
        var writer = new StreamWriter(fileStream, new UTF8Encoding(false));

        writer.Write("namespace PhoneNumbers.Tests.Parsers;" + LF);
        writer.Write(LF);

        writer.Write("/// <summary>" + LF);
        writer.Write($"/// Contains unit tests for the <see cref=\"{(customParser ? countryCode : "Default")}PhoneNumberParser\"/> class for {CountryCodeToCountryInfoName(countryCode).Humanize()} <see cref=\"PhoneNumber\"/>s." + LF);
        writer.Write("/// </summary>" + LF);

        writer.Write($"public class {Path.GetFileNameWithoutExtension(testFilePath)}" + LF);
        writer.Write("{" + LF);

        if (customParser)
        {
            writer.Write($"    private static readonly PhoneNumberParser s_parser = {countryCode}PhoneNumberParser.Create();" + LF);
        }
        else
        {
            writer.Write($"    private static readonly PhoneNumberParser s_parser = DefaultPhoneNumberParser.Create(CountryInfo.{CountryCodeToCountryInfoName(countryCode)});" + LF);
        }

        return writer;
    }

    private static void WriteGeoTests(string countryCode, StreamWriter writer, IEnumerable<CountryNumberDataLine> dataLines)
    {
        foreach (var parentGroup in dataLines.GroupBy(x => x.NationalDestinationCode.Length > 0 ? x.NationalDestinationCode[0] : '\0'))
        {
            foreach (var group in parentGroup.GroupBy(x => x.NationalDestinationCode.Length).OrderBy(x => x.Key))
            {
                writer.Write(LF);
                writer.Write("    [Theory]" + LF);

                if (group.First().NationalDestinationCode.Length > 0)
                {
                    foreach (var inlineData in group.OrderBy(x => x.NationalDestinationCode).ThenBy(x => x.SubscriberNumber.Length).ThenBy(x => x.SubscriberNumber))
                    {
                        writer.Write($"    [InlineData(\"{LookUpTrunkCode(countryCode)}{inlineData.NationalDestinationCode}{inlineData.SubscriberNumber}\", \"{inlineData.NationalDestinationCode}\", \"{inlineData.SubscriberNumber}\", \"{inlineData.GeographicArea}\")]" + LF);
                    }

                    writer.Write($"    public void Parse_Known_GeographicPhoneNumber_{group.First().NationalDestinationCode[0]}{new string(Enumerable.Repeat('X', group.Key - 1).ToArray())}_NationalDestinationCode(string value, string NationalDestinationCode, string subscriberNumber, string geographicArea)" + LF);
                }
                else
                {
                    foreach (var inlineData in group.OrderBy(x => x.NationalDestinationCode).ThenBy(x => x.SubscriberNumber.Length).ThenBy(x => x.SubscriberNumber))
                    {
                        writer.Write($"    [InlineData(\"{LookUpTrunkCode(countryCode)}{inlineData.NationalDestinationCode}{inlineData.SubscriberNumber}\", \"{inlineData.SubscriberNumber}\", \"{inlineData.GeographicArea}\")]" + LF);
                    }

                    writer.Write($"    public void Parse_Known_GeographicPhoneNumber(string value, string subscriberNumber, string geographicArea)" + LF);
                }

                writer.Write("    {" + LF);
                writer.Write("        var parseResult = s_parser.Parse(value);" + LF);
                writer.Write("        parseResult.ThrowIfFailure();" + LF);
                writer.Write(LF);
                writer.Write("        var phoneNumber = parseResult.PhoneNumber;" + LF);
                writer.Write(LF);
                writer.Write("        Assert.NotNull(phoneNumber);" + LF);
                writer.Write("        Assert.IsType<GeographicPhoneNumber>(phoneNumber);" + LF);
                writer.Write(LF);
                writer.Write("        var geographicPhoneNumber = (GeographicPhoneNumber)phoneNumber;" + LF);

                writer.Write($"        Assert.Equal(CountryInfo.{CountryCodeToCountryInfoName(countryCode)}, geographicPhoneNumber.Country);" + LF);
                writer.Write("        Assert.Equal(geographicArea, geographicPhoneNumber.GeographicArea);" + LF);

                if (group.First().NationalDestinationCode.Length > 0)
                {
                    writer.Write("        Assert.Equal(NationalDestinationCode, geographicPhoneNumber.NationalDestinationCode);" + LF);
                }
                else
                {
                    writer.Write("        Assert.Null(geographicPhoneNumber.NationalDestinationCode);" + LF);
                }

                writer.Write("        Assert.Equal(subscriberNumber, geographicPhoneNumber.SubscriberNumber);" + LF);
                writer.Write("    }" + LF);
            }
        }
    }

    private static void WriteMobileTests(string countryCode, StreamWriter writer, IEnumerable<CountryNumberDataLine> dataLines)
    {
        if (dataLines.Count(x => x.Hint == '\0') > 20)
        {
            foreach (var group in dataLines.Where(x => x.Hint == '\0').OrderBy(x => x.NationalDestinationCode).GroupBy(x => x.NationalDestinationCode.Length > 1 ? x.NationalDestinationCode.Substring(0, 2) : String.Empty))
            {
                writer.Write(LF);
                writer.Write("    [Theory]" + LF);

                if (group.All(x => x.NationalDestinationCode?.Length > 0))
                {
                    foreach (var inlineData in group.OrderBy(x => x.NationalDestinationCode).ThenBy(x => x.SubscriberNumber.Length).ThenBy(x => x.SubscriberNumber))
                    {
                        writer.Write($"    [InlineData(\"{LookUpTrunkCode(countryCode)}{inlineData.NationalDestinationCode}{inlineData.SubscriberNumber}\", \"{inlineData.NationalDestinationCode}\", \"{inlineData.SubscriberNumber}\")]" + LF);
                    }

                    writer.Write($"    public void Parse_Known_MobilePhoneNumber_{group.Key}{new string(Enumerable.Repeat('X', group.First().NationalDestinationCode.Length - 2).ToArray())}_NationalDestinationCode(string value, string NationalDestinationCode, string subscriberNumber)" + LF);
                }
                else
                {
                    foreach (var inlineData in group.OrderBy(x => x.SubscriberNumber.Length).ThenBy(x => x.SubscriberNumber))
                    {
                        writer.Write($"    [InlineData(\"{LookUpTrunkCode(countryCode)}{inlineData.NationalDestinationCode}{inlineData.SubscriberNumber}\", \"{inlineData.SubscriberNumber}\")]" + LF);
                    }

                    writer.Write($"    public void Parse_Known_MobilePhoneNumber(string value, string subscriberNumber)" + LF);
                }

                writer.Write("    {" + LF);
                writer.Write("        var parseResult = s_parser.Parse(value);" + LF);
                writer.Write("        parseResult.ThrowIfFailure();" + LF);
                writer.Write(LF);
                writer.Write("        var phoneNumber = parseResult.PhoneNumber;" + LF);
                writer.Write(LF);
                writer.Write("        Assert.NotNull(phoneNumber);" + LF);
                writer.Write("        Assert.IsType<MobilePhoneNumber>(phoneNumber);" + LF);
                writer.Write(LF);
                writer.Write("        var mobilePhoneNumber = (MobilePhoneNumber)phoneNumber;" + LF);

                writer.Write($"        Assert.Equal(CountryInfo.{CountryCodeToCountryInfoName(countryCode)}, mobilePhoneNumber.Country);" + LF);
                writer.Write("        Assert.False(mobilePhoneNumber.IsPager);" + LF);
                writer.Write("        Assert.False(mobilePhoneNumber.IsVirtual);" + LF);

                if (group.All(x => x.NationalDestinationCode?.Length > 0))
                {
                    writer.Write("        Assert.Equal(NationalDestinationCode, mobilePhoneNumber.NationalDestinationCode);" + LF);
                }
                else
                {
                    writer.Write("        Assert.Null(mobilePhoneNumber.NationalDestinationCode);" + LF);
                }

                writer.Write("        Assert.Equal(subscriberNumber, mobilePhoneNumber.SubscriberNumber);" + LF);
                writer.Write("    }" + LF);
            }
        }
        else if(dataLines.Any(x => x.Hint == '\0'))
        {
            writer.Write(LF);
            writer.Write("    [Theory]" + LF);

            if (dataLines.All(x => x.NationalDestinationCode?.Length > 0))
            {
                foreach (var inlineData in dataLines.Where(x => x.Hint == '\0').OrderBy(x => x.NationalDestinationCode).ThenBy(x => x.SubscriberNumber.Length).ThenBy(x => x.SubscriberNumber))
                {
                    writer.Write($"    [InlineData(\"{LookUpTrunkCode(countryCode)}{inlineData.NationalDestinationCode}{inlineData.SubscriberNumber}\", \"{inlineData.NationalDestinationCode}\", \"{inlineData.SubscriberNumber}\")]" + LF);
                }

                writer.Write($"    public void Parse_Known_MobilePhoneNumber(string value, string NationalDestinationCode, string subscriberNumber)" + LF);
            }
            else
            {
                foreach (var inlineData in dataLines.Where(x => x.Hint == '\0').OrderBy(x => x.SubscriberNumber.Length).ThenBy(x => x.SubscriberNumber))
                {
                    writer.Write($"    [InlineData(\"{LookUpTrunkCode(countryCode)}{inlineData.NationalDestinationCode}{inlineData.SubscriberNumber}\", \"{inlineData.SubscriberNumber}\")]" + LF);
                }

                writer.Write($"    public void Parse_Known_MobilePhoneNumber(string value, string subscriberNumber)" + LF);
            }

            writer.Write("    {" + LF);
            writer.Write("        var parseResult = s_parser.Parse(value);" + LF);
            writer.Write("        parseResult.ThrowIfFailure();" + LF);
            writer.Write(LF);
            writer.Write("        var phoneNumber = parseResult.PhoneNumber;" + LF);
            writer.Write(LF);
            writer.Write("        Assert.NotNull(phoneNumber);" + LF);
            writer.Write("        Assert.IsType<MobilePhoneNumber>(phoneNumber);" + LF);
            writer.Write(LF);
            writer.Write("        var mobilePhoneNumber = (MobilePhoneNumber)phoneNumber;" + LF);

            writer.Write($"        Assert.Equal(CountryInfo.{CountryCodeToCountryInfoName(countryCode)}, mobilePhoneNumber.Country);" + LF);
            writer.Write("        Assert.False(mobilePhoneNumber.IsPager);" + LF);
            writer.Write("        Assert.False(mobilePhoneNumber.IsVirtual);" + LF);

            if (dataLines.All(x => x.NationalDestinationCode?.Length > 0))
            {
                writer.Write("        Assert.Equal(NationalDestinationCode, mobilePhoneNumber.NationalDestinationCode);" + LF);
            }
            else
            {
                writer.Write("        Assert.Null(mobilePhoneNumber.NationalDestinationCode);" + LF);
            }

            writer.Write("        Assert.Equal(subscriberNumber, mobilePhoneNumber.SubscriberNumber);" + LF);
            writer.Write("    }" + LF);
        }

        if (dataLines.Any(x => x.Hint == 'P'))
        {
            writer.Write(LF);
            writer.Write("    [Theory]" + LF);

            if (dataLines.All(x => x.NationalDestinationCode?.Length > 0))
            {
                foreach (var inlineData in dataLines.Where(x => x.Hint == 'P').OrderBy(x => x.NationalDestinationCode).ThenBy(x => x.SubscriberNumber.Length).ThenBy(x => x.SubscriberNumber))
                {
                    writer.Write($"    [InlineData(\"{LookUpTrunkCode(countryCode)}{inlineData.NationalDestinationCode}{inlineData.SubscriberNumber}\", \"{inlineData.NationalDestinationCode}\", \"{inlineData.SubscriberNumber}\")]" + LF);
                }

                writer.Write($"    public void Parse_Known_MobilePhoneNumber_Pager(string value, string NationalDestinationCode, string subscriberNumber)" + LF);
            }
            else
            {
                foreach (var inlineData in dataLines.Where(x => x.Hint == 'P').OrderBy(x => x.SubscriberNumber.Length).ThenBy(x => x.SubscriberNumber))
                {
                    writer.Write($"    [InlineData(\"{LookUpTrunkCode(countryCode)}{inlineData.NationalDestinationCode}{inlineData.SubscriberNumber}\", \"{inlineData.SubscriberNumber}\")]" + LF);
                }

                writer.Write($"    public void Parse_Known_MobilePhoneNumber_Pager(string value, string subscriberNumber)" + LF);
            }

            writer.Write("    {" + LF);
            writer.Write("        var parseResult = s_parser.Parse(value);" + LF);
            writer.Write("        parseResult.ThrowIfFailure();" + LF);
            writer.Write(LF);
            writer.Write("        var phoneNumber = parseResult.PhoneNumber;" + LF);
            writer.Write(LF);
            writer.Write("        Assert.NotNull(phoneNumber);" + LF);
            writer.Write("        Assert.IsType<MobilePhoneNumber>(phoneNumber);" + LF);
            writer.Write(LF);
            writer.Write("        var mobilePhoneNumber = (MobilePhoneNumber)phoneNumber;" + LF);

            writer.Write($"        Assert.Equal(CountryInfo.{CountryCodeToCountryInfoName(countryCode)}, mobilePhoneNumber.Country);" + LF);
            writer.Write("        Assert.True(mobilePhoneNumber.IsPager);" + LF);
            writer.Write("        Assert.False(mobilePhoneNumber.IsVirtual);" + LF);

            if (dataLines.All(x => x.NationalDestinationCode?.Length > 0))
            {
                writer.Write("        Assert.Equal(NationalDestinationCode, mobilePhoneNumber.NationalDestinationCode);" + LF);
            }
            else
            {
                writer.Write("        Assert.Null(mobilePhoneNumber.NationalDestinationCode);" + LF);
            }

            writer.Write("        Assert.Equal(subscriberNumber, mobilePhoneNumber.SubscriberNumber);" + LF);
            writer.Write("    }" + LF);
        }

        if (dataLines.Any(x => x.Hint == 'V'))
        {
            writer.Write(LF);
            writer.Write("    [Theory]" + LF);

            if (dataLines.All(x => x.NationalDestinationCode?.Length > 0))
            {
                foreach (var inlineData in dataLines.Where(x => x.Hint == 'V').OrderBy(x => x.NationalDestinationCode).ThenBy(x => x.SubscriberNumber.Length).ThenBy(x => x.SubscriberNumber))
                {
                    writer.Write($"    [InlineData(\"{LookUpTrunkCode(countryCode)}{inlineData.NationalDestinationCode}{inlineData.SubscriberNumber}\", \"{inlineData.NationalDestinationCode}\", \"{inlineData.SubscriberNumber}\")]" + LF);
                }

                writer.Write($"    public void Parse_Known_MobilePhoneNumber_Virtual(string value, string NationalDestinationCode, string subscriberNumber)" + LF);
            }
            else
            {
                foreach (var inlineData in dataLines.Where(x => x.Hint == 'V').OrderBy(x => x.SubscriberNumber.Length).ThenBy(x => x.SubscriberNumber))
                {
                    writer.Write($"    [InlineData(\"{LookUpTrunkCode(countryCode)}{inlineData.NationalDestinationCode}{inlineData.SubscriberNumber}\", \"{inlineData.SubscriberNumber}\")]" + LF);
                }

                writer.Write($"    public void Parse_Known_MobilePhoneNumber_Virtual(string value, string subscriberNumber)" + LF);
            }

            writer.Write("    {" + LF);
            writer.Write("        var parseResult = s_parser.Parse(value);" + LF);
            writer.Write("        parseResult.ThrowIfFailure();" + LF);
            writer.Write(LF);
            writer.Write("        var phoneNumber = parseResult.PhoneNumber;" + LF);
            writer.Write(LF);
            writer.Write("        Assert.NotNull(phoneNumber);" + LF);
            writer.Write("        Assert.IsType<MobilePhoneNumber>(phoneNumber);" + LF);
            writer.Write(LF);
            writer.Write("        var mobilePhoneNumber = (MobilePhoneNumber)phoneNumber;" + LF);

            writer.Write($"        Assert.Equal(CountryInfo.{CountryCodeToCountryInfoName(countryCode)}, mobilePhoneNumber.Country);" + LF);
            writer.Write("        Assert.False(mobilePhoneNumber.IsPager);" + LF);
            writer.Write("        Assert.True(mobilePhoneNumber.IsVirtual);" + LF);

            if (dataLines.All(x => x.NationalDestinationCode?.Length > 0))
            {
                writer.Write("        Assert.Equal(NationalDestinationCode, mobilePhoneNumber.NationalDestinationCode);" + LF);
            }
            else
            {
                writer.Write("        Assert.Null(mobilePhoneNumber.NationalDestinationCode);" + LF);
            }

            writer.Write("        Assert.Equal(subscriberNumber, mobilePhoneNumber.SubscriberNumber);" + LF);
            writer.Write("    }" + LF);
        }
    }

    private static void WriteNonGeoTests(string countryCode, StreamWriter writer, IEnumerable<CountryNumberDataLine> dataLines)
    {
        foreach (var group in dataLines.Where(x => x.Hint == '\0').GroupBy(x => x.NationalDestinationCode.Length > 0 ? x.NationalDestinationCode[0] : '\0'))
        {
            writer.Write(LF);
            writer.Write("    [Theory]" + LF);

            if (group.All(x => x.NationalDestinationCode?.Length > 0))
            {
                foreach (var inlineData in group.OrderBy(x => x.NationalDestinationCode).ThenBy(x => x.SubscriberNumber.Length).ThenBy(x => x.SubscriberNumber))
                {
                    writer.Write($"    [InlineData(\"{LookUpTrunkCode(countryCode)}{inlineData.NationalDestinationCode}{inlineData.SubscriberNumber}\", \"{inlineData.NationalDestinationCode}\", \"{inlineData.SubscriberNumber}\")]" + LF);
                }

                writer.Write($"    public void Parse_Known_NonGeographicPhoneNumber_{group.Key}{new string(Enumerable.Repeat('X', group.First().NationalDestinationCode.Length - 1).ToArray())}_NationalDestinationCode(string value, string NationalDestinationCode, string subscriberNumber)" + LF);
            }
            else
            {
                foreach (var inlineData in group.OrderBy(x => x.SubscriberNumber.Length).ThenBy(x => x.SubscriberNumber))
                {
                    writer.Write($"    [InlineData(\"{LookUpTrunkCode(countryCode)}{inlineData.NationalDestinationCode}{inlineData.SubscriberNumber}\", \"{inlineData.SubscriberNumber}\")]" + LF);
                }

                writer.Write($"    public void Parse_Known_NonGeographicPhoneNumber(string value, string subscriberNumber)" + LF);
            }

            writer.Write("    {" + LF);
            writer.Write("        var parseResult = s_parser.Parse(value);" + LF);
            writer.Write("        parseResult.ThrowIfFailure();" + LF);
            writer.Write(LF);
            writer.Write("        var phoneNumber = parseResult.PhoneNumber;" + LF);
            writer.Write(LF);
            writer.Write("        Assert.NotNull(phoneNumber);" + LF);
            writer.Write("        Assert.IsType<NonGeographicPhoneNumber>(phoneNumber);" + LF);
            writer.Write(LF);
            writer.Write("        var nonGeographicPhoneNumber = (NonGeographicPhoneNumber)phoneNumber;" + LF);

            writer.Write($"        Assert.Equal(CountryInfo.{CountryCodeToCountryInfoName(countryCode)}, nonGeographicPhoneNumber.Country);" + LF);
            writer.Write("        Assert.False(nonGeographicPhoneNumber.IsFreephone);" + LF);
            writer.Write("        Assert.False(nonGeographicPhoneNumber.IsMachineToMachine);" + LF);
            writer.Write("        Assert.False(nonGeographicPhoneNumber.IsPremiumRate);" + LF);
            writer.Write("        Assert.False(nonGeographicPhoneNumber.IsSharedCost);" + LF);

            if (group.All(x => x.NationalDestinationCode?.Length > 0))
            {
                writer.Write("        Assert.Equal(NationalDestinationCode, nonGeographicPhoneNumber.NationalDestinationCode);" + LF);
            }
            else
            {
                writer.Write("        Assert.Null(nonGeographicPhoneNumber.NationalDestinationCode);" + LF);
            }

            writer.Write("        Assert.Equal(subscriberNumber, nonGeographicPhoneNumber.SubscriberNumber);" + LF);
            writer.Write("    }" + LF);
        }

        if (dataLines.Any(x => x.Hint == 'F'))
        {
            writer.Write(LF);
            writer.Write("    [Theory]" + LF);

            if (dataLines.Where(x => x.Hint == 'F').All(x => x.NationalDestinationCode?.Length > 0))
            {
                foreach (var inlineData in dataLines.Where(x => x.Hint == 'F').OrderBy(x => x.NationalDestinationCode).ThenBy(x => x.SubscriberNumber.Length).ThenBy(x => x.SubscriberNumber))
                {
                    writer.Write($"    [InlineData(\"{LookUpTrunkCode(countryCode)}{inlineData.NationalDestinationCode}{inlineData.SubscriberNumber}\", \"{inlineData.NationalDestinationCode}\", \"{inlineData.SubscriberNumber}\")]" + LF);
                }

                writer.Write($"    public void Parse_Known_NonGeographicPhoneNumber_Freephone(string value, string NationalDestinationCode, string subscriberNumber)" + LF);
            }
            else
            {
                foreach (var inlineData in dataLines.Where(x => x.Hint == 'F').OrderBy(x => x.SubscriberNumber.Length).ThenBy(x => x.SubscriberNumber))
                {
                    writer.Write($"    [InlineData(\"{LookUpTrunkCode(countryCode)}{inlineData.NationalDestinationCode}{inlineData.SubscriberNumber}\", \"{inlineData.SubscriberNumber}\")]" + LF);
                }

                writer.Write($"    public void Parse_Known_NonGeographicPhoneNumber_Freephone(string value, string subscriberNumber)" + LF);
            }

            writer.Write("    {" + LF);
            writer.Write("        var parseResult = s_parser.Parse(value);" + LF);
            writer.Write("        parseResult.ThrowIfFailure();" + LF);
            writer.Write(LF);
            writer.Write("        var phoneNumber = parseResult.PhoneNumber;" + LF);
            writer.Write(LF);
            writer.Write("        Assert.NotNull(phoneNumber);" + LF);
            writer.Write("        Assert.IsType<NonGeographicPhoneNumber>(phoneNumber);" + LF);
            writer.Write(LF);
            writer.Write("        var nonGeographicPhoneNumber = (NonGeographicPhoneNumber)phoneNumber;" + LF);

            writer.Write($"        Assert.Equal(CountryInfo.{CountryCodeToCountryInfoName(countryCode)}, nonGeographicPhoneNumber.Country);" + LF);
            writer.Write("        Assert.True(nonGeographicPhoneNumber.IsFreephone);" + LF);
            writer.Write("        Assert.False(nonGeographicPhoneNumber.IsMachineToMachine);" + LF);
            writer.Write("        Assert.False(nonGeographicPhoneNumber.IsPremiumRate);" + LF);
            writer.Write("        Assert.False(nonGeographicPhoneNumber.IsSharedCost);" + LF);

            if (dataLines.Where(x => x.Hint == 'F').All(x => x.NationalDestinationCode?.Length > 0))
            {
                writer.Write("        Assert.Equal(NationalDestinationCode, nonGeographicPhoneNumber.NationalDestinationCode);" + LF);
            }
            else
            {
                writer.Write("        Assert.Null(nonGeographicPhoneNumber.NationalDestinationCode);" + LF);
            }

            writer.Write("        Assert.Equal(subscriberNumber, nonGeographicPhoneNumber.SubscriberNumber);" + LF);
            writer.Write("    }" + LF);
        }

        if (dataLines.Any(x => x.Hint == 'M'))
        {
            writer.Write(LF);
            writer.Write("    [Theory]" + LF);

            if (dataLines.Where(x => x.Hint == 'M').All(x => x.NationalDestinationCode?.Length > 0))
            {
                foreach (var inlineData in dataLines.Where(x => x.Hint == 'M').OrderBy(x => x.NationalDestinationCode).ThenBy(x => x.SubscriberNumber.Length).ThenBy(x => x.SubscriberNumber))
                {
                    writer.Write($"    [InlineData(\"{LookUpTrunkCode(countryCode)}{inlineData.NationalDestinationCode}{inlineData.SubscriberNumber}\", \"{inlineData.NationalDestinationCode}\", \"{inlineData.SubscriberNumber}\")]" + LF);
                }

                writer.Write($"    public void Parse_Known_NonGeographicPhoneNumber_MachineToMachine(string value, string NationalDestinationCode, string subscriberNumber)" + LF);
            }
            else
            {
                foreach (var inlineData in dataLines.Where(x => x.Hint == 'M').OrderBy(x => x.SubscriberNumber.Length).ThenBy(x => x.SubscriberNumber))
                {
                    writer.Write($"    [InlineData(\"{LookUpTrunkCode(countryCode)}{inlineData.NationalDestinationCode}{inlineData.SubscriberNumber}\", \"{inlineData.SubscriberNumber}\")]" + LF);
                }

                writer.Write($"    public void Parse_Known_NonGeographicPhoneNumber_MachineToMachine(string value, string subscriberNumber)" + LF);
            }

            writer.Write("    {" + LF);
            writer.Write("        var parseResult = s_parser.Parse(value);" + LF);
            writer.Write("        parseResult.ThrowIfFailure();" + LF);
            writer.Write(LF);
            writer.Write("        var phoneNumber = parseResult.PhoneNumber;" + LF);
            writer.Write(LF);
            writer.Write("        Assert.NotNull(phoneNumber);" + LF);
            writer.Write("        Assert.IsType<NonGeographicPhoneNumber>(phoneNumber);" + LF);
            writer.Write(LF);
            writer.Write("        var nonGeographicPhoneNumber = (NonGeographicPhoneNumber)phoneNumber;" + LF);

            writer.Write($"        Assert.Equal(CountryInfo.{CountryCodeToCountryInfoName(countryCode)}, nonGeographicPhoneNumber.Country);" + LF);
            writer.Write("        Assert.False(nonGeographicPhoneNumber.IsFreephone);" + LF);
            writer.Write("        Assert.True(nonGeographicPhoneNumber.IsMachineToMachine);" + LF);
            writer.Write("        Assert.False(nonGeographicPhoneNumber.IsPremiumRate);" + LF);
            writer.Write("        Assert.False(nonGeographicPhoneNumber.IsSharedCost);" + LF);

            if (dataLines.Where(x => x.Hint == 'M').All(x => x.NationalDestinationCode?.Length > 0))
            {
                writer.Write("        Assert.Equal(NationalDestinationCode, nonGeographicPhoneNumber.NationalDestinationCode);" + LF);
            }
            else
            {
                writer.Write("        Assert.Null(nonGeographicPhoneNumber.NationalDestinationCode);" + LF);
            }

            writer.Write("        Assert.Equal(subscriberNumber, nonGeographicPhoneNumber.SubscriberNumber);" + LF);
            writer.Write("    }" + LF);
        }

        if (dataLines.Any(x => x.Hint == 'R'))
        {
            writer.Write(LF);
            writer.Write("    [Theory]" + LF);

            if (dataLines.Where(x => x.Hint == 'R').All(x => x.NationalDestinationCode?.Length > 0))
            {
                foreach (var inlineData in dataLines.Where(x => x.Hint == 'R').OrderBy(x => x.NationalDestinationCode).ThenBy(x => x.SubscriberNumber.Length).ThenBy(x => x.SubscriberNumber))
                {
                    writer.Write($"    [InlineData(\"{LookUpTrunkCode(countryCode)}{inlineData.NationalDestinationCode}{inlineData.SubscriberNumber}\", \"{inlineData.NationalDestinationCode}\", \"{inlineData.SubscriberNumber}\")]" + LF);
                }

                writer.Write($"    public void Parse_Known_NonGeographicPhoneNumber_PremiumRate(string value, string NationalDestinationCode, string subscriberNumber)" + LF);
            }
            else
            {
                foreach (var inlineData in dataLines.Where(x => x.Hint == 'R').OrderBy(x => x.SubscriberNumber.Length).ThenBy(x => x.SubscriberNumber))
                {
                    writer.Write($"    [InlineData(\"{LookUpTrunkCode(countryCode)}{inlineData.NationalDestinationCode}{inlineData.SubscriberNumber}\", \"{inlineData.SubscriberNumber}\")]" + LF);
                }

                writer.Write($"    public void Parse_Known_NonGeographicPhoneNumber_PremiumRate(string value, string subscriberNumber)" + LF);
            }

            writer.Write("    {" + LF);
            writer.Write("        var parseResult = s_parser.Parse(value);" + LF);
            writer.Write("        parseResult.ThrowIfFailure();" + LF);
            writer.Write(LF);
            writer.Write("        var phoneNumber = parseResult.PhoneNumber;" + LF);
            writer.Write(LF);
            writer.Write("        Assert.NotNull(phoneNumber);" + LF);
            writer.Write("        Assert.IsType<NonGeographicPhoneNumber>(phoneNumber);" + LF);
            writer.Write(LF);
            writer.Write("        var nonGeographicPhoneNumber = (NonGeographicPhoneNumber)phoneNumber;" + LF);

            writer.Write($"        Assert.Equal(CountryInfo.{CountryCodeToCountryInfoName(countryCode)}, nonGeographicPhoneNumber.Country);" + LF);
            writer.Write("        Assert.False(nonGeographicPhoneNumber.IsFreephone);" + LF);
            writer.Write("        Assert.False(nonGeographicPhoneNumber.IsMachineToMachine);" + LF);
            writer.Write("        Assert.True(nonGeographicPhoneNumber.IsPremiumRate);" + LF);
            writer.Write("        Assert.False(nonGeographicPhoneNumber.IsSharedCost);" + LF);

            if (dataLines.Where(x => x.Hint == 'R').All(x => x.NationalDestinationCode?.Length > 0))
            {
                writer.Write("        Assert.Equal(NationalDestinationCode, nonGeographicPhoneNumber.NationalDestinationCode);" + LF);
            }
            else
            {
                writer.Write("        Assert.Null(nonGeographicPhoneNumber.NationalDestinationCode);" + LF);
            }

            writer.Write("        Assert.Equal(subscriberNumber, nonGeographicPhoneNumber.SubscriberNumber);" + LF);
            writer.Write("    }" + LF);
        }

        if (dataLines.Any(x => x.Hint == 'S'))
        {
            writer.Write(LF);
            writer.Write("    [Theory]" + LF);

            if (dataLines.Where(x => x.Hint == 'S').All(x => x.NationalDestinationCode?.Length > 0))
            {
                foreach (var inlineData in dataLines.Where(x => x.Hint == 'S').OrderBy(x => x.NationalDestinationCode).ThenBy(x => x.SubscriberNumber.Length).ThenBy(x => x.SubscriberNumber))
                {
                    writer.Write($"    [InlineData(\"{LookUpTrunkCode(countryCode)}{inlineData.NationalDestinationCode}{inlineData.SubscriberNumber}\", \"{inlineData.NationalDestinationCode}\", \"{inlineData.SubscriberNumber}\")]" + LF);
                }

                writer.Write($"    public void Parse_Known_NonGeographicPhoneNumber_SharedCost(string value, string NationalDestinationCode, string subscriberNumber)" + LF);
            }
            else
            {
                foreach (var inlineData in dataLines.Where(x => x.Hint == 'S').OrderBy(x => x.SubscriberNumber.Length).ThenBy(x => x.SubscriberNumber))
                {
                    writer.Write($"    [InlineData(\"{LookUpTrunkCode(countryCode)}{inlineData.NationalDestinationCode}{inlineData.SubscriberNumber}\", \"{inlineData.SubscriberNumber}\")]" + LF);
                }

                writer.Write($"    public void Parse_Known_NonGeographicPhoneNumber_SharedCost(string value, string subscriberNumber)" + LF);
            }

            writer.Write("    {" + LF);
            writer.Write("        var parseResult = s_parser.Parse(value);" + LF);
            writer.Write("        parseResult.ThrowIfFailure();" + LF);
            writer.Write(LF);
            writer.Write("        var phoneNumber = parseResult.PhoneNumber;" + LF);
            writer.Write(LF);
            writer.Write("        Assert.NotNull(phoneNumber);" + LF);
            writer.Write("        Assert.IsType<NonGeographicPhoneNumber>(phoneNumber);" + LF);
            writer.Write(LF);
            writer.Write("        var nonGeographicPhoneNumber = (NonGeographicPhoneNumber)phoneNumber;" + LF);

            writer.Write($"        Assert.Equal(CountryInfo.{CountryCodeToCountryInfoName(countryCode)}, nonGeographicPhoneNumber.Country);" + LF);
            writer.Write("        Assert.False(nonGeographicPhoneNumber.IsFreephone);" + LF);
            writer.Write("        Assert.False(nonGeographicPhoneNumber.IsMachineToMachine);" + LF);
            writer.Write("        Assert.False(nonGeographicPhoneNumber.IsPremiumRate);" + LF);
            writer.Write("        Assert.True(nonGeographicPhoneNumber.IsSharedCost);" + LF);

            if (dataLines.Where(x => x.Hint == 'S').All(x => x.NationalDestinationCode?.Length > 0))
            {
                writer.Write("        Assert.Equal(NationalDestinationCode, nonGeographicPhoneNumber.NationalDestinationCode);" + LF);
            }
            else
            {
                writer.Write("        Assert.Null(nonGeographicPhoneNumber.NationalDestinationCode);" + LF);
            }

            writer.Write("        Assert.Equal(subscriberNumber, nonGeographicPhoneNumber.SubscriberNumber);" + LF);
            writer.Write("    }" + LF);
        }
    }
}

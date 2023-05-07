// Represents a line in a data file
internal sealed class CountryNumberDataLine
{
    internal string? GeographicArea { get; init; }
    internal char Hint { get; init; }
    internal char Kind { get; init; }
    internal string? NationalDestinationCode { get; init; }
    internal string SubscriberNumber { get; init; } = null!;
}

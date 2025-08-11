using CsvHelper.Configuration.Attributes;

public class ParticleRawData
{
    [Name("ParticleKey", "파티클 키")]
    public string ParticleKey { get; set; }

    [Name("AddressablePath", "어드레서블", "Addressable")]
    public string AddressablePath { get; set; }
}
namespace LuminaSupplemental.SpaghettiGenerator;

public class AppConfig
{
    public BasicConfig Basic { get; set; } = new BasicConfig();
    public ParsingConfig Parsing { get; set; } = new ParsingConfig();
    public ProxyConfig Proxy { get; set; } = new ProxyConfig();
}

public class BasicConfig
{
    public string FFXIVGameDirectory { get; set; }
    public string LibraSQLFilePath { get; set; }

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(FFXIVGameDirectory) &&
               !string.IsNullOrWhiteSpace(LibraSQLFilePath);
    }
}

public class ParsingConfig
{
    public bool ParseOnlineSources { get; set; }
    public string OnlineCacheDirectory { get; set; }
    public bool ProcessMobSpawnHTML { get; set; }
}

public class ProxyConfig
{
    public bool ProxyEnable { get; set; }
    public string ProxyHost { get; set; }
    public int ProxyPort { get; set; }
    public string ProxyUsername { get; set; }
    public string ProxyPassword { get; set; }

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(ProxyHost) &&
               ProxyPort > 0;
    }
}
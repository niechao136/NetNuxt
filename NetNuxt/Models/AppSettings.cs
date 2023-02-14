namespace NetNuxt.Models;

public class AppSettings
{
    public APIPATH? APIPATH { get; set; }
    public POSAPIPATH? POSAPIPATH { get; set; }
}
public class APIPATH
{
    public string? URL { get; set; }
}
public class POSAPIPATH 
{
    public string? URL { get; set; }
}
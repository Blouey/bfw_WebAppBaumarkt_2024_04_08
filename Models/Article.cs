namespace bfw_WebAppBaumarkt_2024_04_08.Models;

public class Article
{
    public int AId { get; set; }
    public int KId { get; set; }
    public string Name { get; set; }
    public double LPreis { get; set; }
    public int Bestand { get; set; }
    public string? Bild { get; set; }
    
    public Category Category { get; set; }
    
    // public string KatBezeichnung { get; set; }
    
}
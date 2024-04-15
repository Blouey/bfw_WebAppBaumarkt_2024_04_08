namespace bfw_WebAppBaumarkt_2024_04_08.Models;

public class IndexViewModel
{
    public List<Article> Articles { get; set; }
    public List<Category> Categories { get; set; }
    
    public string? Search { get; set; }
    
    public int? CategoryId { get; set; }
    
    public IndexViewModel()
    {
        Articles = new List<Article>();
        Categories = new List<Category>();
    }
}
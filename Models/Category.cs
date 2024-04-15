namespace bfw_WebAppBaumarkt_2024_04_08.Models;

public class Category
{
    public int KId { get; set; }
    public string Bezeichnung { get; set; }

    public List<Article> Articles { get; set; } = new List<Article>();

    
    #region Overrides for Equals and GetHashCode
    
    public override bool Equals(object? obj)
    {
        if (obj is Category other)
        {
            return this.KId == other.KId && this.Bezeichnung == other.Bezeichnung;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.KId, this.Bezeichnung);
    }
    
    #endregion
}
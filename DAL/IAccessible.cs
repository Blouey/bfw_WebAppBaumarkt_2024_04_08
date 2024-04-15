using bfw_WebAppBaumarkt_2024_04_08.Models;

namespace bfw_WebAppBaumarkt_2024_04_08.DAL;

public interface IAccessible
{
    public List<Article> GetAllArticles();
    public Article GetArticleById(int id);
    public List<Article> GetArticlesByCategory(int categoryId);
    public IndexViewModel GetArticlesAndCategories();
    public int InsertArticle(Article article);
    public bool UpdateArticle(Article article);
    public bool DeleteArticleById(int id);
    
    public List<Category> GetAllCategories();
    
    public List<Article> GetArticlesByCategory(Category category);
    
    public IndexViewModel GetIndexViewModel();
    
}
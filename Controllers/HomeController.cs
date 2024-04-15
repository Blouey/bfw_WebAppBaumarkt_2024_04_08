using bfw_WebAppBaumarkt_2024_04_08.DAL;
using bfw_WebAppBaumarkt_2024_04_08.Models;
using Microsoft.AspNetCore.Mvc;

namespace bfw_WebAppBaumarkt_2024_04_08.Controllers;

public class HomeController : Controller
{
    private readonly IAccessible _dal;
    private string _conn;


    public HomeController(IConfiguration configuration)
    {
        _conn = configuration.GetConnectionString("DefaultConnection")!;
        _dal = new SqlDal(_conn);
    }

    [HttpGet]
    public IActionResult Index()
    {
        IndexViewModel model = _dal.GetArticlesAndCategories();

        // Liste aus Liste ????

        /*
        _dal.GetAllArticles().ForEach(a =>
        {
            model.Articles.Add(a);
            if (!model.Categories.Contains(a.Category))
            {
                model.Categories.Add(a.Category);
            }

        });
        */
        /*
        foreach (Article article in _dal.GetAllArticles())
        {
            model.Articles.Add(article);
            if (!model.Categories.Contains(article.Category))
            {
                model.Categories.Add(article.Category);
            }
        }
        */

        return View(model);
    }

    [HttpPost]
    public IActionResult Index(string? search, int? param)
    {
        IndexViewModel model = _dal.GetArticlesAndCategories();

        List<Article> articles = new List<Article>();
        model.Articles.ForEach(a =>
        {
            if (a.Name.ToLower().Contains((search ?? "").ToLower() ))
            {
                articles.Add(a);
            }
        });
        
        model.Articles.Clear();
        articles.ForEach(a =>
        {
            if (a.Category.KId == param || a.Category.Bezeichnung == search || param == 0)
            {
                model.Articles.Add(a);
            }
        });

       model.Search = search;
       model.CategoryId = param;
        /*
        IndexViewModel model = new IndexViewModel();
        _dal.GetArticlesByCategory(param ?? 0).ForEach(a =>
        {
            model.Articles.Add(a);
            if (!model.Categories.Contains(a.Category))
            {
                model.Categories.Add(a.Category);
            }
        });
        */
        
        return View(model);
    }
}
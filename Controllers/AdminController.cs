using bfw_WebAppBaumarkt_2024_04_08.DAL;
using bfw_WebAppBaumarkt_2024_04_08.Models;
using Microsoft.AspNetCore.Mvc;

namespace bfw_WebAppBaumarkt_2024_04_08.Controllers;

public class AdminController : Controller
{
    private readonly IAccessible _dal;
    private readonly IWebHostEnvironment _env;
    private string _conn;


    public AdminController(IConfiguration configuration, IWebHostEnvironment env)
    {
        _conn = configuration.GetConnectionString("DefaultConnection")!;
        _dal = new SqlDal(_conn);
        _env = env;
    }
    
    
    [HttpGet]
    public IActionResult Index()
    {
        IndexViewModel model = _dal.GetArticlesAndCategories();

        return View(model);
    }

    [HttpPost]
    public IActionResult Index(string? search, int? param)
    {
        IndexViewModel model = _dal.GetArticlesAndCategories();

        List<Article> articles = new List<Article>();
        model.Articles.ForEach(a =>
        {
            if (a.Name.ToLower().Contains((search ?? "").ToLower()))
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

        return View(model);
    }
    
    [HttpGet]
    public IActionResult Edit(int aid)
    {
        Article article = _dal.GetArticleById(aid);
        UpdateModel updateModel = new UpdateModel
        {
            AId = article.AId,
            KId = article.KId,
            Name = article.Name,
            LPreis = article.LPreis,
            Bestand = article.Bestand,
            Bild = article.Bild,
            Category = article.Category,
        };
        return View(updateModel);
    }
    
    [HttpPost]
    public IActionResult Edit(UpdateModel updateModel)
    {
        if (updateModel.File != null)
        {
            updateModel.Bild = SavePictureAsync(updateModel.File).Result;
        }
        else
        {
            updateModel.Bild = _dal.GetArticleById(updateModel.AId).Bild;
        }

        if (_dal.UpdateArticle(updateModel))
        {
            return RedirectToAction("Index");
        }

        return View(updateModel);
    }
    
    
    private async Task<string> SavePictureAsync(IFormFile file)
    {
        string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        string path = Path.Combine(_env.WebRootPath, "images", fileName);
        using (var fileStream = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        return fileName;
    }
}
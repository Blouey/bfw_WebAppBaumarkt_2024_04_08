using bfw_WebAppBaumarkt_2024_04_08.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace bfw_WebAppBaumarkt_2024_04_08.DAL;

public class SqlDal : IAccessible
{
    private readonly SqlConnection _con;

    public SqlDal(string conn)
    {
        _con = new SqlConnection(conn);
    }

    public List<Article> GetAllArticles()
    {
        List<Article> articles;

        string selectCmd =
            "SELECT A.AID, A.KatID as 'KId', A.Bezeichnung as 'Name', A.LPreis, A.Bestand, C.KID as 'KId', C.Bezeichnung as 'Bezeichnung' " +
            "from Article A right join Category C " +
            "on A.KatID = C.KID;";
        using (var connection = _con)
        {
            articles = connection.Query<Article, Category, Article>(selectCmd, (article, category) =>
                {
                    article.Category = category;
                    return article;
                },
                splitOn: "KId"
            ).ToList();
        }

        return articles;
    }

    public List<Category> GetAllCategories()
    {
        List<Category> categories;
        using (var connection = _con)
        {
            string selectCmd = "SELECT * from Category";
            categories = connection.Query<Category>(selectCmd).ToList();
        }

        return categories;
    }

    #region not implemented

    public Article GetArticleById(int id)
    {
        throw new NotImplementedException();
    }

    public int InsertArticle(Article article)
    {
        throw new NotImplementedException();
    }

    public bool UpdateArticle(Article article)
    {
        throw new NotImplementedException();
    }

    public bool DeleteArticleById(int id)
    {
        throw new NotImplementedException();
    }

    public List<Article> GetArticlesByCategory(Category category)
    {
        throw new NotImplementedException();
    }

    #endregion

    public List<Article> GetArticlesByCategory(int categoryId)
    {
        List<Article> articles;

        using (var connection = _con)
        {
            string selectCmd =
                "SELECT A.AID, A.KatID as 'KId', A.Bezeichnung as 'Name', A.LPreis, A.Bestand, C.KID, C.Bezeichnung as 'Bezeichnung' " +
                "from Article A inner join Category C " +
                "on A.KatID = C.KID";

            if (categoryId != 0)
            {
                selectCmd += " WHERE C.KID = @categoryId";
            }

            articles = connection.Query<Article, Category, Article>(selectCmd, (article, category) =>
                {
                    article.Category = category;
                    return article;
                },
                new { categoryId },
                splitOn: "KId"
            ).ToList();
        }

        return articles;
    }

    public IndexViewModel GetArticlesAndCategories()
    {
        List<Article> articles;
        List<Category> categories;
        string selectCmd =
            "SELECT A.AID, A.KatID as 'KId', A.Bezeichnung as 'Name', A.LPreis, A.Bestand, C.KID as 'KId', C.Bezeichnung as 'Bezeichnung' " +
            "from Article A right join Category C " +
            "on A.KatID = C.KID;" +
            "SELECT * from Category;";
        using (var connection = _con)
        {
            using (var multi = connection.QueryMultiple(selectCmd))
            {
                articles = multi.Read<Article, Category, Article>((article, category) =>
                    {
                        article.Category = category;
                        return article;
                    },
                    splitOn: "KId"
                ).ToList();
                categories = multi.Read<Category>().ToList();
            }
        }

        return new IndexViewModel()
        {
            Articles = articles,
            Categories = categories
        };
    }


    public IndexViewModel GetIndexViewModel()
    {
        IndexViewModel model = new IndexViewModel();

        using (var connection = _con)
        {
            string selectCmd =
                "SELECT A.AID, A.KatID as 'KId', A.Bezeichnung as 'Name', A.LPreis, A.Bestand, C.KID as 'KId', C.Bezeichnung as 'Bezeichnung' " +
                "from Article A inner join Category C " +
                "on A.KatID = C.KID";
            model.Articles = connection.Query<Article, Category, Article>(selectCmd, (article, category) =>
                {
                    article.Category = category;
                    return article;
                },
                splitOn: "KId"
            ).ToList();
/*
            foreach (Article article in model.Articles)
            {
                if (!model.Categories.Contains(article.Category))
                {
                    model.Categories.Add(article.Category);
                }
            }

            selectCmd = "SELECT * from Category";
            model.Categories = connection.Query<Category>(selectCmd).ToList();
*/
        }

        return model;
    }
}
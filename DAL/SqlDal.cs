using System.Globalization;
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
            "SELECT A.AID, A.KatID as 'KId', A.Bezeichnung as 'Name', A.LPreis, A.Bildname as 'Bild', A.Bestand, C.KID as 'KId', C.Bezeichnung as 'Bezeichnung' " +
            "from Article A right join Category C " +
            "on A.KatID = C.KID;";
        using ( var connection = new SqlConnection(_con.ConnectionString))
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
        using ( var connection = new SqlConnection(_con.ConnectionString))
        {
            string selectCmd = "SELECT * from Category";
            categories = connection.Query<Category>(selectCmd).ToList();
        }

        return categories;
    }

    public Article GetArticleById(int id)
    {
        Article article;
        using(var connection = new SqlConnection(_con.ConnectionString))
        {
            string selectCmd =
                "SELECT A.AID, A.KatID as 'KId', A.Bezeichnung as 'Name', A.LPreis, A.Bildname as 'Bild', A.Bestand, C.KID as 'KId', C.Bezeichnung as 'Bezeichnung' " +
                "from Article A inner join Category C " +
                "on A.KatID = C.KID " +
                "WHERE A.AID = @id";
            article = connection.Query<Article, Category, Article>(selectCmd, (a, c) =>
                {
                    a.Category = c;
                    return a;
                },
                new { id },
                splitOn: "KId"
            ).FirstOrDefault()!;
        }
        return article;
    }
    public bool UpdateArticle(UpdateModel updateModel)
    {
        Article article = new Article()
        {
            AId = updateModel.AId,
            KId = updateModel.KId,
            Name = updateModel.Name,
            LPreis = updateModel.LPreis,
            Bestand = updateModel.Bestand,
            Bild = updateModel.Bild,
            Category = updateModel.Category
        };

        using ( var connection = new SqlConnection(_con.ConnectionString))
        {
            string updateCmd =
                "UPDATE Article " +
                "SET KatID = @KId, Bezeichnung = @Name, LPreis = @LPreis, Bestand = @Bestand, Bildname = @Bild " +
                "WHERE AID = @AId";
            int rows = connection.Execute(updateCmd, article);
            
            return rows > 0;
        }
    }

    public List<Article> GetArticlesByCategory(int categoryId)
    {
        List<Article> articles;

        using ( var connection = new SqlConnection(_con.ConnectionString))
        {
            string selectCmd =
                "SELECT A.AID, A.KatID as 'KId', A.Bezeichnung as 'Name', A.LPreis, A.Bildname as 'Bild', A.Bestand, C.KID, C.Bezeichnung as 'Bezeichnung' " +
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
        List<Article> articles = new List<Article>();
        List<Category> categories = new List<Category>();
        string selectCmd =
            "SELECT A.AID, A.KatID as 'KId', A.Bezeichnung as 'Name', A.LPreis, A.Bildname as 'Bild', A.Bestand, C.KID as 'KId', C.Bezeichnung as 'Bezeichnung' " +
            "from Category C left join Article A " +
            "on A.KatID = C.KID;";

        using ( var connection = new SqlConnection(_con.ConnectionString))
        {
            /*
            using (var multi = connection.QueryMultiple(selectCmd))
            {
                articles = multi.Read<Article, Category, Article>((article, category) =>
                    {
                        if(article.AId.Equals(DBNull.Value))
                        {
                            article = null;
                        }
                        else
                        {
                            article.Category = category;
                        }

                        return article;
                    },
                    splitOn: "KId"
                ).ToList();
                categories = multi.Read<Category>().ToList();
            }
            */

            var reader = connection.ExecuteReader(selectCmd);
            while (reader.Read())
            {
                var category = new Category()
                {
                    KId = reader.GetInt32(6),
                    Bezeichnung = reader.GetString(7)
                };
                if (!categories.Contains(category))
                {
                    categories.Add(category);
                }
                else
                {
                    category = categories.Find(c => c.KId == category.KId);
                }


                if (reader["AId"] != DBNull.Value)
                {
                    var article = new Article()
                    {
                        AId = reader.GetInt32(0),

                        KId = reader.GetInt32(1),
                        Name = reader.GetString(2),
                        LPreis = reader.GetDouble(3),
                        Bild = reader["Bild"] == DBNull.Value ? null : reader.GetString(4),
                        Bestand = reader.GetInt32(5),
                        Category = category!
                    };
                    articles.Add(article);
                    category!.Articles.Add(article);
                }
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

        using ( var connection = new SqlConnection(_con.ConnectionString))
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

        }

        return model;
    }
    
    #region not implemented

    public int InsertArticle(Article article)
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
}

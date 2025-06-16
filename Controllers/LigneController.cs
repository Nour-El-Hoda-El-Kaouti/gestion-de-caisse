using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using gestion_de_caisse.Models;

namespace gestion_de_caisse.Controllers
{
    public class LigneController : Controller
    {
        SqlCommand cmd = new SqlCommand();
        SqlDataReader dr;
        SqlConnection con = new SqlConnection();
        private readonly ILogger<LigneController> _logger;
        public LigneController(ILogger<LigneController> logger)
        {
            _logger = logger;
            con.ConnectionString = gestion_de_caisse.Properties.Resources.ConnectionString;
        }
        public ActionResult Index(string search = null, int page = 1, int pageSize = 7)
        {
            ViewBag.SearchTerm = search;
            List<Ligne> Lignes;
            int totalCount;

            if (!string.IsNullOrEmpty(search))
            {
                Lignes = SearchInDatabase(search, page, pageSize);
                totalCount = GetTotalCount(search);
            }
            else
            {
                Lignes = FetchData(page, pageSize);
                totalCount = GetTotalCount();
            }

            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            LigneViewModel viewModel = new LigneViewModel
            {
                Lignes = Lignes,
                TotalPages = totalPages,
                Page = page,
                SearchTerm = search 
            };

            return View(viewModel);
        }

        private int GetTotalCount(string search = null)
        {
            int totalCount = 0;
            try
            {
                con.Open();
                cmd.Connection = con;
                if (!string.IsNullOrEmpty(search))
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM [caisse].[dbo].[Ligne] " +
                                      "WHERE Ligne LIKE @SearchTerm OR CONVERT(NVARCHAR(MAX), Tarif) LIKE @SearchTerm OR Centre LIKE @SearchTerm";
                    cmd.Parameters.Clear(); 
                    cmd.Parameters.AddWithValue("@SearchTerm", search + "%");
                }
                else
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM [caisse].[dbo].[Ligne]";
                }

                totalCount = (int)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return totalCount;
        }


        public List<Ligne> SearchInDatabase(string search, int page, int pageSize)
        {
            List<Ligne> Lignes = new List<Ligne>();
            try
            {
                con.Open();
                cmd.Connection = con;
                int startIndex = (page - 1) * pageSize;
                cmd.CommandText = "SELECT [Ligne],[Tarif],[Centre] " +
                    "FROM [caisse].[dbo].[Ligne] " +
                    "WHERE Ligne LIKE @SearchTerm OR CONVERT(NVARCHAR(MAX), Tarif) LIKE @SearchTerm OR Centre LIKE @SearchTerm " +
                    "ORDER BY [Ligne] OFFSET @StartIndex ROWS FETCH NEXT @PageSize ROWS ONLY";
                cmd.Parameters.AddWithValue("@SearchTerm", search + "%");
                cmd.Parameters.AddWithValue("@StartIndex", startIndex);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Lignes.Add(new Ligne()
                    {
                        ligne = dr["Ligne"].ToString(),
                        Tarif = Convert.ToInt32(dr["Tarif"]),
                        Centre = dr["Centre"].ToString()
                    });
                }
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Lignes;
        }
        private List<Ligne> FetchData(int page, int pageSize)
        {
            List<Ligne> Lignes = new List<Ligne>();
            try
            {
                con.Open();
                cmd.Connection = con;
                int startIndex = (page - 1) * pageSize;
                cmd.CommandText = "SELECT [Ligne],[Tarif],[Centre] FROM [caisse].[dbo].[Ligne] ORDER BY [Ligne] OFFSET @StartIndex ROWS FETCH NEXT @PageSize ROWS ONLY";
                cmd.Parameters.AddWithValue("@StartIndex", startIndex);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Lignes.Add(new Ligne()
                    {                       
                        ligne = dr["Ligne"].ToString(),
                        Tarif= Convert.ToInt32(dr["Tarif"]),
                        Centre = dr["Centre"].ToString()
                    });
                }
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Lignes;
        }
    }
}

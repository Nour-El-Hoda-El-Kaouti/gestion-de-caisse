using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using gestion_de_caisse.Models;

namespace gestion_de_caisse.Controllers
{
    public class FDRController : Controller
    {
        SqlCommand cmd = new SqlCommand();
        SqlDataReader dr;
        SqlConnection con = new SqlConnection();
        private readonly ILogger<FDRController> _logger;
        public FDRController(ILogger<FDRController> logger)
        {
            _logger = logger;
            con.ConnectionString = gestion_de_caisse.Properties.Resources.ConnectionString;
        }
        private int GetTotalCount()
        {
            int totalCount = 0;
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "SELECT COUNT(*) FROM [caisse].[dbo].[FDR_Entete]";
                totalCount = (int)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return totalCount;
        }
        public ActionResult Index(string search = null, int page = 1, int pageSize = 7)
        {
            List<FDR> FDRs;
            int totalCount;

            if (!string.IsNullOrEmpty(search))
            {
                FDRs = SearchInDatabase(search, page, pageSize);
                totalCount = GetTotalCount(search);
            }
            else
            {
                FDRs = FetchData(page, pageSize);
                totalCount = GetTotalCount();
            }

            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            FDRViewModel viewModel = new FDRViewModel
            {
                FDRs = FDRs,
                TotalPages = totalPages,
                Page = page,
                SearchTerm = search
            };
            return View(viewModel);
        }
        private int GetTotalCount(string search = null)
        {
            int count = 0;
            try
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    if (!string.IsNullOrEmpty(search))
                    {
                        cmd.CommandText = "SELECT COUNT(*) FROM [caisse].[dbo].[FDR_Entete] " +
                                          "WHERE NFDR LIKE @SearchTerm OR Date LIKE @SearchTerm OR Vehicule LIKE @SearchTerm " +
                                          "OR Conducteur LIKE @SearchTerm OR Ligne LIKE @SearchTerm OR Centre LIKE @SearchTerm";
                        cmd.Parameters.AddWithValue("@SearchTerm", search + "%");
                    }
                    else
                    {
                        cmd.CommandText = "SELECT COUNT(*) FROM [caisse].[dbo].[FDR_Entete]";
                    }

                    count = (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while getting the total count.");
                throw;
            }
            finally
            {
                con.Close();
            }

            return count;
        }
        public List<FDR> SearchInDatabase(string search, int page, int pageSize)
        {
            List<FDR> FDRs = new List<FDR>();
            try
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    int startIndex = (page - 1) * pageSize;                    
                    cmd.CommandText = "SELECT [NFDR],[Date],[Vehicule],[Conducteur],[Ligne],[Centre] " +
                                      "FROM [caisse].[dbo].[FDR_Entete] " +
                                      "WHERE NFDR LIKE @SearchTerm " +
                                      "OR CONVERT(VARCHAR(10), Date, 120) LIKE @SearchTerm " +
                                      "OR Vehicule LIKE @SearchTerm " +
                                      "OR Conducteur LIKE @SearchTerm " +
                                      "OR Ligne LIKE @SearchTerm " +
                                      "OR Centre LIKE @SearchTerm " +
                                      "ORDER BY [NFDR] OFFSET @StartIndex ROWS FETCH NEXT @PageSize ROWS ONLY";
                    cmd.Parameters.AddWithValue("@SearchTerm", "%" + search + "%");
                    cmd.Parameters.AddWithValue("@StartIndex", startIndex);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            FDRs.Add(new FDR()
                            {
                                NFDR = dr["NFDR"].ToString(),
                                Date = dr.GetDateTime(dr.GetOrdinal("Date")),
                                Vehicule = dr.GetInt32(dr.GetOrdinal("Vehicule")),
                                Conducteur = dr.GetInt32(dr.GetOrdinal("Conducteur")),
                                Ligne = dr["Ligne"].ToString(),
                                Centre = dr["Centre"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {                
                _logger.LogError(ex, "An error occurred while searching the database.");
                throw;
            }
            finally
            {
                con.Close();
            }

            return FDRs;
        }

        private List<FDR> FetchData(int page, int pageSize)
        {
            List<FDR> FDRs = new List<FDR>();
            try
            {
                con.Open();
                cmd.Connection = con;                
                int startIndex = (page - 1) * pageSize;                
                cmd.CommandText = "SELECT [NFDR],[Date],[Vehicule],[Conducteur],[Ligne],[Centre] FROM [caisse].[dbo].[FDR_Entete] ORDER BY [NFDR] OFFSET @StartIndex ROWS FETCH NEXT @PageSize ROWS ONLY";
                cmd.Parameters.AddWithValue("@StartIndex", startIndex);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    FDRs.Add(new FDR()
                    {
                        NFDR = dr["NFDR"].ToString(),
                        Date = dr.GetDateTime(dr.GetOrdinal("Date")),
                        Vehicule = dr.GetInt32(dr.GetOrdinal("Vehicule")),
                        Conducteur = dr.GetInt32(dr.GetOrdinal("Conducteur")),
                        Ligne = dr["Ligne"].ToString(),
                        Centre = dr["Centre"].ToString()
                    });
                }
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return FDRs;
        }
    }
}

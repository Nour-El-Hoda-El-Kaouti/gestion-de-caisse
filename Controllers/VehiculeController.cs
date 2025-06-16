using gestion_de_caisse.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace gestion_de_caisse.Controllers
{
    public class VehiculeController : Controller
    {                
        SqlCommand cmd = new SqlCommand();
        SqlDataReader dr;
        SqlConnection con = new SqlConnection();
        private readonly ILogger<VehiculeController> _logger;
        public VehiculeController(ILogger<VehiculeController> logger)
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
                cmd.CommandText = "SELECT COUNT(*) FROM [caisse].[dbo].[Vehicule]";
                totalCount = (int)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return totalCount;
        }

        public ActionResult Index(string search = null, int page = 1, int pageSize = 3)
        {
            ViewBag.Search=search;
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.ErrorMessage = TempData["ErrorMessage"];
            List<Vehicule> Vehicules;
            if (!string.IsNullOrEmpty(search))
            {
                Vehicules = SearchInDatabase(search, page, pageSize);
            }
            else
            {
                Vehicules = FetchData(page, pageSize);
            }

            int totalCount = GetTotalCount();
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            VehiculeViewModel viewModel = new VehiculeViewModel
            {
                Vehicules = Vehicules,
                TotalPages = totalPages,
                Page = page
            };
            return View(viewModel);
        }

        public List<Vehicule> SearchInDatabase(string search, int page, int pageSize)
        {
            List<Vehicule> Vehicules = new List<Vehicule>();
            try
            {
                con.Open();
                cmd.Connection = con;
                int startIndex = (page - 1) * pageSize;
                cmd.CommandText = "SELECT [NumVehicule], [Modele]" +
                " FROM [caisse].[dbo].[Vehicule] " +
                " WHERE NumVehicule LIKE @SearchTerm OR Modele LIKE @SearchTerm " +
                "ORDER BY [NumVehicule] OFFSET @StartIndex ROWS FETCH NEXT @PageSize ROWS ONLY";
                cmd.Parameters.AddWithValue("@SearchTerm",search + "%");
                cmd.Parameters.AddWithValue("@StartIndex", startIndex);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Vehicules.Add(new Vehicule()
                    {
                        NumVehicule = dr.GetInt32(dr.GetOrdinal("NumVehicule")),
                        Modele = dr["Modele"].ToString(),                        
                    });
                }
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Vehicules;
        }
        private List<Vehicule> FetchData(int page, int pageSize)
        {
            List<Vehicule> Vehicules = new List<Vehicule>();
            try
            {
                con.Open();
                cmd.Connection = con;
                int startIndex = (page - 1) * pageSize;
                cmd.CommandText = "SELECT [NumVehicule],[Modele] FROM [caisse].[dbo].[Vehicule] ORDER BY [NumVehicule] OFFSET @StartIndex ROWS FETCH NEXT @PageSize ROWS ONLY";
                cmd.Parameters.AddWithValue("@StartIndex", startIndex);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Vehicules.Add(new Vehicule()
                    {
                        NumVehicule = dr.GetInt32(dr.GetOrdinal("NumVehicule")),                                               
                        Modele = dr["Modele"].ToString(),                        
                    });
                }
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Vehicules;
        }
        public ActionResult ajoutVehicule(int NumVehicule,string Modele)
        {
            try
            {
                using (SqlConnection con = new SqlConnection("Data Source=DESKTOP-NR3E8D5;Initial Catalog=caisse;Integrated Security=True;"))
                {
                    con.Open();
                    using (SqlCommand com = new SqlCommand("SELECT COUNT(*) FROM Vehicule WHERE NumVehicule=@NumVehicule", con))
                    {
                        com.Parameters.AddWithValue("@NumVehicule", NumVehicule);
                        int count = (int)com.ExecuteScalar();
                        if (count > 0)
                        {
                            TempData["ErrorMessage"] = "La véhicule est déjà inséré";
                            return RedirectToAction("Index", "Vehicule");
                        }
                        else
                        {
                            using (SqlCommand cmd = new SqlCommand("INSERT INTO Vehicule (NumVehicule, Modele) VALUES (@NumVehicule, @Modele)", con))
                            {
                                cmd.Parameters.AddWithValue("@NumVehicule", NumVehicule);
                                cmd.Parameters.AddWithValue("@Modele",Modele);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                return RedirectToAction("Index", "Vehicule");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Une erreur s'est produite lors de l'insertion des informations : " + ex.Message;
                return RedirectToAction("Index", "Vehicule");
            }
        }
        public ActionResult modifVehicule(Vehicule vehicule, int VehiculeAncien, string ModeleAncien)
        {
            try
            {
                using (SqlConnection con = new SqlConnection("Data Source=DESKTOP-NR3E8D5;Initial Catalog=caisse;Integrated Security=True;"))
                {
                    con.Open();
                    using (SqlCommand updateCmd = new SqlCommand("UPDATE Vehicule SET Modele = @Modele, NumVehicule = @NumVehicule WHERE NumVehicule = @VehiculeAncien AND Modele = @ModeleAncien", con))
                    {
                        updateCmd.Parameters.AddWithValue("@Modele", vehicule.Modele);
                        updateCmd.Parameters.AddWithValue("@NumVehicule", vehicule.NumVehicule);
                        updateCmd.Parameters.AddWithValue("@VehiculeAncien", VehiculeAncien);
                        updateCmd.Parameters.AddWithValue("@ModeleAncien", ModeleAncien);
                        updateCmd.ExecuteNonQuery();
                    }
                    return RedirectToAction("Index", "Vehicule");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Une erreur s'est produite lors de la modification : " + ex.Message;
                return RedirectToAction("Index", "Vehicule");
            }
        }


        public ActionResult DeleteVehicule(int VehiculeClic2)
        {
            try
            {
                using (SqlConnection con = new SqlConnection("Data Source=DESKTOP-NR3E8D5;Initial Catalog=caisse;Integrated Security=True;"))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM Vehicule WHERE NumVehicule = @VehiculeClic2", con))
                    {
                        cmd.Parameters.AddWithValue("@VehiculeClic2", VehiculeClic2);
                        cmd.ExecuteNonQuery();
                    }
                    return RedirectToAction("Index", "Vehicule");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Une erreur s'est produite lors de la suppression " + ex.Message;
                return RedirectToAction("Index", "Vehicule");
            }
        }
    }
}


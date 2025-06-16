using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using gestion_de_caisse.Models;
namespace gestion_de_caisse.Controllers
{
    public class MachinisteController : Controller
    {
        SqlCommand cmd = new SqlCommand();
        SqlDataReader dr;
        SqlConnection con = new SqlConnection();
        private readonly ILogger<MachinisteController> _logger;
        public MachinisteController(ILogger<MachinisteController> logger)
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
                cmd.CommandText = "SELECT COUNT(*) FROM [caisse].[dbo].[Conducteur]";
                totalCount = (int)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return totalCount;
        }
        public ActionResult Index(string search = null, int page = 1, int pageSize = 6)
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.ErrorMessage = TempData["ErrorMessage"];
            List<Machiniste> Machinistes;
            List<int> Vehicules = new List<int>();
            if (!string.IsNullOrEmpty(search))
            {
                Machinistes = SearchInDatabase(search, page, pageSize);
            }
            else
            {
                Machinistes = FetchData(page, pageSize);
            }
            Vehicules = FetchVehicule();
            int totalCount = GetTotalCount();
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            MachinisteViewModel viewModel = new MachinisteViewModel
            {
                Vehicules = Vehicules,
                Machinistes = Machinistes,
                TotalPages = totalPages,
                Page = page
            };

            return View(viewModel);
        }

        public List<Machiniste> SearchInDatabase(string search, int page, int pageSize)
        {
            List<Machiniste> Machinistes = new List<Machiniste>();
            try
            {
                con.Open();
                cmd.Connection = con;
                int startIndex = (page - 1) * pageSize;
                cmd.CommandText = "SELECT [Matricule],[NomPrenom] " +
                "FROM [caisse].[dbo].[conducteur] " +
                "WHERE NomPrenom LIKE @Search OR Matricule LIKE @Search " +
                " ORDER BY [Matricule] OFFSET @StartIndex ROWS FETCH NEXT @PageSize ROWS ONLY";
                cmd.Parameters.AddWithValue("@Search", search + "%");
                cmd.Parameters.AddWithValue("@StartIndex", startIndex);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Machinistes.Add(new Machiniste()
                    {
                        Matricule = dr.GetInt32(dr.GetOrdinal("Matricule")),
                        NomPrenom = dr["NomPrenom"].ToString()
                    });
                }
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Machinistes;
        }

        private List<Machiniste> FetchData(int page, int pageSize)
        {
            List<Machiniste> Machinistes = new List<Machiniste>();
            try
            {
                con.Open();
                cmd.Connection = con;
                int startIndex = (page - 1) * pageSize;
                cmd.CommandText = "SELECT [Matricule],[NomPrenom] FROM [caisse].[dbo].[Conducteur] ORDER BY [Matricule] OFFSET @StartIndex ROWS FETCH NEXT @PageSize ROWS ONLY";
                cmd.Parameters.AddWithValue("@StartIndex", startIndex);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Machinistes.Add(new Machiniste()
                    {
                        Matricule = dr.GetInt32(dr.GetOrdinal("Matricule")),
                        NomPrenom = dr["NomPrenom"].ToString()
                    });
                }
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Machinistes;
        }
        private List<int> FetchVehicule() 
        {
            List<int> Vehicules = new List<int>();
            try
            {
                con.Open();
                cmd.Connection = con;                
                cmd.CommandText = "SELECT [NumVehicule] FROM [caisse].[dbo].[Vehicule] ";                
                dr = cmd.ExecuteReader();                
                while (dr.Read())
                {
                    Vehicules.Add(dr.GetInt32(dr.GetOrdinal("NumVehicule")));
                }
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Vehicules;
        }
        public ActionResult ajoutMachiniste(Machiniste machiniste)
        {
            try
            {                
                    con.Open();
                    using (SqlCommand com = new SqlCommand("SELECT COUNT(*) FROM Conducteur WHERE Matricule=@Matricule", con))
                    {
                        com.Parameters.AddWithValue("@Matricule", machiniste.Matricule);
                        int count = (int)com.ExecuteScalar();
                        if (count > 0)
                        {
                            TempData["ErrorMessage"] = "Le Machiniste est déjà inséré";
                            return RedirectToAction("Index", "Machiniste");
                        }
                        else
                        {
                            using (SqlCommand cmd = new SqlCommand("INSERT INTO conducteur (Matricule, NomPrenom) VALUES (@Matricule, @NomPrenom)", con))
                            {
                                cmd.Parameters.AddWithValue("@Matricule", machiniste.Matricule);
                                cmd.Parameters.AddWithValue("@NomPrenom", machiniste.NomPrenom);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }                
                return RedirectToAction("Index", "Machiniste");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Une erreur s'est produite lors de l'insertion des informations : " + ex.Message;
                return RedirectToAction("Index", "Machiniste");
            }
        }
        public ActionResult ModifMachiniste(Machiniste machiniste,int MatriculeAncien,string NomPrenomAncien)
        {
            try
            {                                
                    con.Open();                                           
                    using (SqlCommand updateCmd = new SqlCommand("UPDATE conducteur SET NomPrenom = @NomPrenom,Matricule=@Matricule WHERE Matricule = @MatriculeAncien AND NomPrenom=@NomPrenomAncien", con))
                    {
                                updateCmd.Parameters.AddWithValue("@NomPrenom", machiniste.NomPrenom);
                                updateCmd.Parameters.AddWithValue("@Matricule", machiniste.Matricule);
                                updateCmd.Parameters.AddWithValue("@MatriculeAncien", MatriculeAncien);
                                updateCmd.Parameters.AddWithValue("@NomPrenomAncien", NomPrenomAncien);
                                updateCmd.ExecuteNonQuery();
                    }                                   
                return RedirectToAction("Index", "Machiniste");                
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Une erreur s'est produite lors de la modification : " + ex.Message;
                return RedirectToAction("Index", "Machiniste");
            }
        }
        public ActionResult DeleteMachiniste(int Matricule2)
        {
            try
            {
                using (SqlConnection con = new SqlConnection("Data Source=DESKTOP-NR3E8D5;Initial Catalog=caisse;Integrated Security=True;"))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM conducteur WHERE Matricule = @MatriculeClic", con))
                    {
                        cmd.Parameters.AddWithValue("@MatriculeClic", Matricule2);
                        cmd.ExecuteNonQuery();
                    }
                    return RedirectToAction("Index", "Machiniste");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Une erreur s'est produite lors de la suppression " + ex.Message;
                return RedirectToAction("Index", "Machiniste");
            }
        }
    }
}

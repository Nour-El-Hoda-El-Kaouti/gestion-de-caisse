using System.Data;
using System.Data.SqlClient;
using gestion_de_caisse.Models;
using gestion_de_caisse.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace gestion_de_caisse.Controllers
{
    public class EncaissementController : Controller
    {
        SqlCommand cmd = new SqlCommand();
        SqlDataReader dr;
        SqlConnection con = new SqlConnection();
        List<Encaissement> encaissements = new List<Encaissement>();
        List<string> FDRs = new List<string>();
        List<string> lignes = new List<string>();
        List<int> tarifs = new List<int>();
        List<int> machinistes = new List<int>();
        List<int> vehicules = new List<int>();
        List<string> bus = new List<string>();        
        private readonly ILogger<EncaissementController> _logger;
        private readonly IUserService _userService;
        public EncaissementController(ILogger<EncaissementController> logger, IUserService userService)
        {
            _logger = logger;
            con.ConnectionString = gestion_de_caisse.Properties.Resources.ConnectionString;
            _userService = userService;
        }
        private int GetTotalCount()
        {
            int totalCount = 0;
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "SELECT COUNT(*) FROM [caisse].[dbo].[Encaissement]";
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
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.ErrorMessage = TempData["ErrorMessage"];
            IndexViewModel model = new IndexViewModel();               
            if (!string.IsNullOrEmpty(search))
            {
                model.Encaissements = SearchInDatabase(search, page, pageSize);
            }
            else
            {
                model.Encaissements = FetchData(page, pageSize);
            }
            int totalCount = GetTotalCount();
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            FetchFDR();
            FetchMachiniste();
            FetchLigne();
            FetchTarif();
            FetchVehicule();
            FetchCentre();            
            model.FDRs = FDRs;
            model.Machinistes = machinistes;
            model.Lignes = lignes;
            model.Tarifs = tarifs;
            model.Vehicules = vehicules;
            model.BUs = bus;
            model.TotalPages = totalPages;
            model.Page = page;
            return View(model);
        }
        public List<Encaissement> SearchInDatabase(string search, int page, int pageSize)
        {
            List<Encaissement> Encaissements = new List<Encaissement>();
            try
            {
                con.Open();
                cmd.Connection = con;
                int startIndex = (page - 1) * pageSize;
                cmd.CommandText = "SELECT [Date],[Centre],[Vehicule],[Conducteur],[Ligne],[RecetteVerse],[RecetteSaisi],[Manque] " +
                "FROM [caisse].[dbo].[Encaissement] " +
                "WHERE Date LIKE @Search OR Centre LIKE @Search OR Vehicule LIKE @Search OR Conducteur LIKE @Search" +
                " OR Ligne LIKE @Search OR RecetteVerse LIKE @Search OR RecetteSaisi LIKE @Search OR Manque LIKE @Search" +
                " ORDER BY [Date] OFFSET @StartIndex ROWS FETCH NEXT @PageSize ROWS ONLY";
                cmd.Parameters.AddWithValue("@Search", search + "%");
                cmd.Parameters.AddWithValue("@StartIndex", startIndex);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Encaissements.Add(new Encaissement()
                    {
                        Date = dr.GetDateTime(dr.GetOrdinal("Date")),
                        Centre= dr["Centre"].ToString(),
                        Vehicule = dr["Vehicule"].ToString(),
                        Conducteur = dr["Conducteur"].ToString(),
                        Ligne = dr["Ligne"].ToString(),
                        RecetteVerse = Convert.ToSingle(dr["RecetteVerse"]),
                        RecetteSaisi = Convert.ToSingle(dr["RecetteSaisi"]),
                        Manque = Convert.ToSingle(dr["Manque"])
                    });
                }
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Encaissements;
        }

        private List<Encaissement> FetchData(int page, int pageSize)
        {
            if (encaissements.Count > 0)
            {
                encaissements.Clear();
            }
            try
            {
                using (SqlConnection con = new SqlConnection("Data Source=DESKTOP-NR3E8D5;Initial Catalog=caisse;Integrated Security=True;"))
                {
                    con.Open();
                    cmd.Connection = con;
                    int startIndex = (page - 1) * pageSize;
                    cmd.CommandText = "SELECT [Encaissement_Id],[Date],[Vehicule],[Centre],[RecetteVerse],[RecetteSaisi],[Manque],[Conducteur],[Ligne],[Tarif] FROM [caisse].[dbo].[Encaissement] WHERE Caissier = @Caissier ORDER BY [Date] OFFSET @StartIndex ROWS FETCH NEXT @PageSize ROWS ONLY";
                    cmd.Parameters.AddWithValue("@Caissier", _userService.Id_Login);
                    cmd.Parameters.AddWithValue("@StartIndex", startIndex);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);
                    dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        encaissements.Add(new Encaissement()
                        {
                            Encaissement_Id = (Guid)dr["Encaissement_Id"],
                            Date = dr.GetDateTime(dr.GetOrdinal("Date")).Date,
                            Vehicule = dr["Vehicule"].ToString(),
                            Centre = dr["Centre"].ToString(),
                            RecetteVerse = Convert.ToSingle(dr["RecetteVerse"]),
                            RecetteSaisi = Convert.ToSingle(dr["RecetteSaisi"]),
                            Conducteur = dr["Conducteur"].ToString(),
                            Ligne = dr["Ligne"].ToString(),
                            Tarif = dr.GetInt32(dr.GetOrdinal("Tarif")),
                            Manque = Convert.ToSingle(dr["Manque"])
                        }) ;
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return encaissements;
        }
        private void FetchFDR()
        {
            if (FDRs.Count > 0)
            {
                FDRs.Clear();
            }
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "SELECT DISTINCT [NFDR] FROM [FDR_Entete]";
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    FDRs.Add(dr["NFDR"].ToString());
                }
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void FetchMachiniste()
        {
            if (machinistes.Count > 0)
            {
                machinistes.Clear();
            }
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "SELECT DISTINCT [Matricule] FROM [Conducteur]";
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    machinistes.Add(Convert.ToInt32(dr["Matricule"]));
                }
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void FetchLigne()
        {
            if (lignes.Count > 0)
            {
                lignes.Clear();
            }
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "SELECT DISTINCT [Ligne] FROM [Ligne]";
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lignes.Add(dr["Ligne"].ToString());
                }
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void FetchTarif()
        {
            if (tarifs.Count > 0)
            {
                tarifs.Clear();
            }
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "SELECT DISTINCT [Tarif] FROM [Ligne]";
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    tarifs.Add(Convert.ToInt32(dr["Tarif"]));
                }
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void FetchVehicule()
        {
            if (vehicules.Count > 0)
            {
                vehicules.Clear();
            }
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "SELECT DISTINCT [NumVehicule] FROM [Vehicule]";
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    vehicules.Add(Convert.ToInt32(dr["NumVehicule"]));
                }
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void FetchCentre()
        {
            if (bus.Count > 0)
            {
                bus.Clear();
            }
            try
            {
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "SELECT DISTINCT [Centre] FROM [Ligne]";
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    bus.Add(dr["Centre"].ToString());
                }
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult CheckRadio(Guid code1, float manque1, IFormCollection frm)
        {
            string motif = frm["Motif"].ToString();

            using (SqlConnection con = new SqlConnection("Data Source=DESKTOP-NR3E8D5;Initial Catalog=caisse;Integrated Security=True;"))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("INSERT INTO JustifsManque(Motif,Commentaire,DateSaisie,Caissier,Manque,Encaissement) " +
                                                        "VALUES(@Motif,@Commentaire,@DateSaisie,@Caissier,@Manque,@Encaissement)", con))
                {
                    cmd.Parameters.AddWithValue("@Motif", motif);
                    cmd.Parameters.AddWithValue("@Commentaire", "Aucun commentaire saisi");
                    cmd.Parameters.AddWithValue("@DateSaisie", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Caissier", _userService.Id_Login);
                    cmd.Parameters.AddWithValue("@Manque", manque1);
                    cmd.Parameters.AddWithValue("@Encaissement", code1);
                    cmd.ExecuteNonQuery();
                }                
            }
            return RedirectToAction("Index", "Encaissement");
        }        
        public ActionResult comment(string Commentaire, Guid codeComment)
        {
            con.Open();
            cmd.Connection = con;
            cmd.CommandText = "UPDATE JustifsManque SET Commentaire ='" + Commentaire + "' WHERE Encaissement ='" + codeComment + "';";
            int rowsAffected = cmd.ExecuteNonQuery();
            con.Close();
            if (rowsAffected > 0)
            {
                con.Close();
                return RedirectToAction("Index", "Encaissement");
            }
            else
            {
                return RedirectToAction("Index", "Encaissement");
            }
        }
        public ActionResult ajoutEncaissement(Encaissement encaissement)
        {
            try
            {
                if (encaissement == null)
                {
                    TempData["ErrorMessage"] = "Aucun encaissement fourni.";
                    return RedirectToAction("Index", "Encaissement");
                }

                con.Open();
                using (SqlCommand com = new SqlCommand("SELECT COUNT(*) FROM Encaissement WHERE NFDR=@NFDR", con))
                {
                    com.Parameters.AddWithValue("@NFDR", encaissement.NFDR);
                    int count = (int)com.ExecuteScalar();
                    if (count > 0)
                    {
                        TempData["ErrorMessage"] = "L'encaissement est déjà inséré";
                        return RedirectToAction("Index", "Encaissement");
                    }
                    else
                    {
                        float manque = ((encaissement.NombreTV * encaissement.Tarif) + (encaissement.NombrePV * 35) + encaissement.RechargePMT) - encaissement.RecetteVerse;
                        string insertQuery = "INSERT INTO Encaissement " +
                            "(Date, Vehicule, Conducteur, Ligne, Tarif, NombreTV, MontantTV, NombrePV, MontantPV, RechargePMT, Caissier, Centre, NFDR, RecetteVerse, NbrCourses, Manque, RecetteSaisi) " +
                            "VALUES (@Date, @Vehicule, @Conducteur, @Ligne, @Tarif, @NombreTV, @MontantTV, @NombrePV, @MontantPV, @RechargePMT, @Caissier, @Centre, @NFDR, @RecetteVerse, @NbrCourses, @Manque, @RecetteSaisi);";

                        using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                        {
                            cmd.Parameters.AddWithValue("@Date", encaissement.Date);
                            cmd.Parameters.AddWithValue("@Vehicule", encaissement.Vehicule);
                            cmd.Parameters.AddWithValue("@Conducteur", encaissement.Conducteur);
                            cmd.Parameters.AddWithValue("@Ligne", encaissement.Ligne);
                            cmd.Parameters.AddWithValue("@Tarif", encaissement.Tarif);
                            cmd.Parameters.AddWithValue("@NombreTV", encaissement.NombreTV);
                            cmd.Parameters.AddWithValue("@NombrePV", encaissement.NombrePV);
                            cmd.Parameters.AddWithValue("@RechargePMT", encaissement.RechargePMT);
                            cmd.Parameters.AddWithValue("@Caissier", _userService.Id_Login);
                            cmd.Parameters.AddWithValue("@Centre", encaissement.Centre);
                            cmd.Parameters.AddWithValue("@NFDR", encaissement.NFDR);
                            cmd.Parameters.AddWithValue("@RecetteVerse", encaissement.RecetteVerse);
                            cmd.Parameters.AddWithValue("@NbrCourses", encaissement.NbrCourses);
                            cmd.Parameters.AddWithValue("@Manque", manque);
                            cmd.Parameters.AddWithValue("@MontantTV", encaissement.NombreTV * encaissement.Tarif);
                            cmd.Parameters.AddWithValue("@MontantPV", encaissement.NombrePV * 35);
                            cmd.Parameters.AddWithValue("@RecetteSaisi", (encaissement.NombreTV * encaissement.Tarif) + (encaissement.NombrePV * 35) + encaissement.RechargePMT);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                return RedirectToAction("Index", "Encaissement");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Une erreur s'est produite lors de l'insertion des informations : " + ex.Message;
                return RedirectToAction("Index", "Encaissement");
            }
        }

        public ActionResult regleEncaissement(float PrixVerse, float manque, Guid code,float RecetteVersee)
        {
            float result = manque - PrixVerse;            
                con.Open();   
                using(SqlCommand com1 = new SqlCommand("INSERT INTO Manque(date,Manque,Encaissement_Id)VALUES(@date,@Manque,@Encaissement);",con))
                {
                    com1.Parameters.AddWithValue("@date", DateTime.Now);
                    com1.Parameters.AddWithValue("@Manque", manque);
                    com1.Parameters.AddWithValue("@Encaissement", code);
                    com1.ExecuteNonQuery();
                }
                using (SqlCommand com = new SqlCommand("UPDATE Encaissement SET Manque = @result, RecetteVerse = @RecetteVerse WHERE Encaissement_Id = @Code", con))
                {
                    com.Parameters.AddWithValue("@result", result);
                    com.Parameters.AddWithValue("@RecetteVerse",RecetteVersee + PrixVerse);
                    com.Parameters.AddWithValue("@Code", code);
                    com.ExecuteNonQuery();
                }   
                using(SqlCommand com2 = new SqlCommand("INSERT INTO ManqueRegle(Date,MontantPaye,Caissier,ManqueInitial)VALUES(@Date,@MontantPaye,@Caissier,@Manque);",con))
                {
                    com2.Parameters.AddWithValue("@Date", DateTime.Now);
                    com2.Parameters.AddWithValue("@MontantPaye", PrixVerse);
                    com2.Parameters.AddWithValue("@Manque", manque);
                    com2.Parameters.AddWithValue("@Caissier", _userService.Id_Login);
                    com2.ExecuteNonQuery();
                }            
                return RedirectToAction("Index", "Encaissement");
        }        
    }

}
    

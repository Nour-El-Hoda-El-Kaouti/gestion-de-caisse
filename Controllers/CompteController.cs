using Microsoft.AspNetCore.Mvc;
using gestion_de_caisse.Models;
using System.Data.SqlClient;
using gestion_de_caisse.Services;
namespace gestion_de_caisse.Controllers
{
    public class CompteController : Controller
    {
        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;
        private readonly IUserService _userService;
        public CompteController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        public ActionResult Login()
        {            
            return View();
        }
        [HttpPost]
        void connectionString()
        {
            con.ConnectionString = "Data Source=DESKTOP-NR3E8D5;Initial Catalog=caisse;Integrated Security=True;";
        }
        public ActionResult verification(Compte cmp)
        {
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "select * from Login where Login=@Login and Password=@Password";
            com.Parameters.AddWithValue("@Login",cmp.Login);
            com.Parameters.AddWithValue("@Password",cmp.Password);
            dr = com.ExecuteReader();
            if (dr.Read())
            {
                _userService.Id_Login = (Guid)dr["Id_Login"];

                con.Close();
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                con.Close();
                ViewBag.Result = "le login ou le mot de passe est incorrecte";
                return View("Login", "Compte");
            }
        }
        public ActionResult Inscription(Compte cmp)
        {
            if (cmp == null || string.IsNullOrEmpty(cmp.Email))
            {                
                return View("Login", "Compte");
            }
            connectionString();
                try
                {
                    con.Open();

                    using (SqlCommand com1 = new SqlCommand("SELECT * FROM Login WHERE Email = @Email;", con))
                    {
                        com1.Parameters.AddWithValue("@Email", cmp.Email);

                        using (SqlDataReader dr = com1.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                ViewBag.Result2 = "L'email saisi a déjà un compte.";
                                return View("Login", "Compte");
                            }
                        }
                    }

                    using (SqlCommand com = new SqlCommand("INSERT INTO Login(NomPrenom, Login, Password, Email) VALUES(@NomPrenom, @Login, @Password, @Email);", con))
                    {
                        com.Parameters.AddWithValue("@NomPrenom", cmp.NomPrenom);
                        com.Parameters.AddWithValue("@Login", cmp.Login);
                        com.Parameters.AddWithValue("@Password", cmp.Password);
                        com.Parameters.AddWithValue("@Email", cmp.Email);

                        int rowsAffected = com.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return RedirectToAction("Inscription", "Compte");
                        }
                        else
                        {
                            return View("Error");
                        }
                    }
                }
                catch (Exception ex)
                {                    
                    ViewBag.ErrorMessage = "Une erreur s'est produite lors de l'inscription : " + ex.Message;
                    return View("Error");
                }            
        }       
    }
}

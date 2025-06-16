namespace gestion_de_caisse.Models
{
    public class Compte
    {
        public Guid Id_Login { get; set; }
        public  string NomPrenom {  get; set; }
        public string Login {  get; set; }
        public string Password {  get; set; }        
        public string  Email {  get; set; }

    }
}

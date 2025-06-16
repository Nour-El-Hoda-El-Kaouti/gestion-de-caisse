namespace gestion_de_caisse.Models
{
    public class LigneViewModel
    {
        public List<Ligne> Lignes { get; set; }
        public int TotalPages { get; set; }
        public int Page { get; set; }
        public string SearchTerm { get; set; } 
    }
}

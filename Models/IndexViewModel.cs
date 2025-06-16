namespace gestion_de_caisse.Models
{
    public class IndexViewModel
    {
        public List<Encaissement> Encaissements { get; set; }
        public List<string> FDRs { get; set; }
        public List<string> Lignes { get; set; }
        public List<int> Tarifs { get; set; }
        public List<int> Machinistes { get; set; }
        public List<int> Vehicules { get; set; }
        public List<string> BUs { get; set; }        
        public int TotalPages { get; set; }
        public int Page { get; set; }
    }

}

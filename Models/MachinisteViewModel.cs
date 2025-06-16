namespace gestion_de_caisse.Models
{
    public class MachinisteViewModel
    {
        public List<Machiniste> Machinistes { get; set; }
        public List<int> Vehicules { get; set; }        
        public int TotalPages { get; set; }
        public int Page { get; set; }
    }
}

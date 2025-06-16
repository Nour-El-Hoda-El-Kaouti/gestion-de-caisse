namespace gestion_de_caisse.Models
{
    public class Encaissement
    {
        public Guid Encaissement_Id { get; set; }
        public DateTime Date { get; set; }
        public string NFDR { get; set; }
        public string Centre { get; set; }
        public string Vehicule { get; set; }
        public string Conducteur {  get; set; }
        public string Ligne {  get; set; }
        public int Tarif { get; set; }
        public int NombreTV { get; set; }
        public float MontantTV { get; set; }
        public int NombrePV { get; set; }
        public float MontantPV { get; set; }
        public int RechargePMT { get; set; }        
        public float RecetteVerse {  get; set; }
        public float RecetteSaisi { get; set; }
        public int NbrCourses { get; set; }
        public float Manque {  get; set; }  
        public bool etatManque { get; set; }
        public Guid Caissier { get; set; }
    }
}

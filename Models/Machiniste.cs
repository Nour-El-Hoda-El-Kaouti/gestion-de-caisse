using gestion_de_caisse.Controllers;
using System.Xml;

namespace gestion_de_caisse.Models
{
    public class Machiniste
    {
        public Guid Id_Conducteur;
        public int Matricule { get; set; }
        public string NomPrenom { get; set; }
    }
}

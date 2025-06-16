using System.Globalization;

namespace gestion_de_caisse.Models
{    
        public class FDRViewModel
        {
            public List<FDR> FDRs { get; set; }
            public int TotalPages { get; set; }
            public int Page { get; set; }
            public string SearchTerm { get; set; } 
        }   
}

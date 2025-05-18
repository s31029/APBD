using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    [Table("Prescription_Medicament")]
    public class PrescriptionMedicament
    {
        // uwaga: composite key definiujemy w OnModelCreating, DataAnnotations nie wspiera go bezpośrednio
        public int IdMedicament { get; set; }
        public int IdPrescription { get; set; }

        public int Dose { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string Details { get; set; } = null!;

        public Medicament Medicament { get; set; } = null!;
        public Prescription Prescription { get; set; } = null!;
    }
}

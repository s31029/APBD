using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    [Table("Medicament")]
    public class Medicament
    {
        [Key]
        public int IdMedicament { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Name { get; set; } = null!;

        [Column(TypeName = "nvarchar(100)")]
        public string Description { get; set; } = null!;

        [Column(TypeName = "nvarchar(100)")]
        public string Type { get; set; } = null!;

        public ICollection<PrescriptionMedicament> PrescriptionMedicaments { get; set; }
            = new List<PrescriptionMedicament>();
    }
}
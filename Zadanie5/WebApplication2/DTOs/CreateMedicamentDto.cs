using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.DTOs
{
    public class CreateMedicamentDto
    {
        public int IdMedicament { get; set; }
        
        public int Dose { get; set; }
        
        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Description { get; set; } = null!;
    }
}
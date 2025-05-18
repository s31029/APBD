// DTOs/CreateMedicamentDto.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.DTOs
{
    public class CreateMedicamentDto
    {
        /// <summary>
        /// Identifier leków istniejących w bazie
        /// </summary>
        public int IdMedicament { get; set; }

        /// <summary>
        /// Dawka leku na recepcie
        /// </summary>
        public int Dose { get; set; }

        /// <summary>
        /// Szczegóły dotyczące podawania leku (np. opis, uwagi)
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Description { get; set; } = null!;
    }
}
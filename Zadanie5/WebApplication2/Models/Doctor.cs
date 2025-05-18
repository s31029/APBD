using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    [Table("Doctor")]
    public class Doctor
    {
        [Key]
        public int IdDoctor { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string FirstName { get; set; } = null!;

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string LastName { get; set; } = null!;

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Email { get; set; } = null!;

        public ICollection<Prescription> Prescriptions { get; set; }
            = new List<Prescription>();
    }
}
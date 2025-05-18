using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    [Table("Patient")]
    public class Patient
    {
        [Key]
        public int IdPatient { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string FirstName { get; set; } = null!;

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string LastName { get; set; } = null!;

        [Column(TypeName = "date")]
        public DateTime Birthdate { get; set; }

        public ICollection<Prescription> Prescriptions { get; set; }
            = new List<Prescription>();
    }
}
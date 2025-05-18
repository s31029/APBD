using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    [Table("Prescription")]
    public class Prescription
    {
        [Key]
        public int IdPrescription { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        [Column(TypeName = "date")]
        public DateTime DueDate { get; set; }

        [ForeignKey(nameof(Patient))]
        public int IdPatient { get; set; }
        public Patient Patient { get; set; } = null!;

        [ForeignKey(nameof(Doctor))]
        public int IdDoctor { get; set; }
        public Doctor Doctor { get; set; } = null!;

        public ICollection<PrescriptionMedicament> PrescriptionMedicaments { get; set; }
            = new List<PrescriptionMedicament>();
    }
}
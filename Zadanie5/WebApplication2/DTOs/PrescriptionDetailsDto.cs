using System;
using System.Collections.Generic;

namespace WebApplication2.DTOs
{
    public class PrescriptionDetailsDto
    {
        public int IdPrescription { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public DoctorDto Doctor { get; set; } = null!;
        public List<MedicamentDetailsDto> Medicaments { get; set; } = new();
    }
}
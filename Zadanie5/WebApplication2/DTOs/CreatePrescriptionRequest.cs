using System;
using System.Collections.Generic;

namespace WebApplication2.DTOs
{
    public class CreatePrescriptionRequest
    {
        public PatientDto Patient { get; set; } = null!;
        public List<CreateMedicamentDto> Medicaments { get; set; } = new();
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
    }
}
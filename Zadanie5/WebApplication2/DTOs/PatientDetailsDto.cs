using System;
using System.Collections.Generic;

namespace WebApplication2.DTOs
{
    public class PatientDetailsDto
    {
        public int IdPatient { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime Birthdate { get; set; }

        public List<PrescriptionDetailsDto> Prescriptions { get; set; }
            = new();
    }
}
using System;

namespace WebApplication2.DTOs
{
    public class PatientDto
    {
        public int? IdPatient { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime Birthdate { get; set; }
    }
}
// Services/PatientService.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.DTOs;

namespace WebApplication2.Services
{
    public class PatientService : IPatientService
    {
        private readonly DatabaseContext _ctx;

        public PatientService(DatabaseContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<PatientDetailsDto> GetDetailsAsync(int idPatient)
        {
            var patient = await _ctx.Patients
                .Include(p => p.Prescriptions)
                    .ThenInclude(pr => pr.PrescriptionMedicaments)
                        .ThenInclude(pm => pm.Medicament)
                .Include(p => p.Prescriptions)
                    .ThenInclude(pr => pr.Doctor)
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.IdPatient == idPatient);

            if (patient == null)
                throw new KeyNotFoundException($"Pacjent o Id={idPatient} nie został znaleziony.");

            return new PatientDetailsDto
            {
                IdPatient  = patient.IdPatient,
                FirstName  = patient.FirstName,
                LastName   = patient.LastName,
                Birthdate  = patient.Birthdate,
                Prescriptions = patient.Prescriptions
                    .OrderBy(r => r.DueDate)
                    .Select(r => new PrescriptionDetailsDto
                    {
                        IdPrescription = r.IdPrescription,
                        Date           = r.Date,
                        DueDate        = r.DueDate,
                        Doctor = new DoctorDto
                        {
                            IdDoctor  = r.Doctor.IdDoctor,
                            FirstName = r.Doctor.FirstName,
                            LastName  = r.Doctor.LastName,
                            Email     = r.Doctor.Email
                        },
                        Medicaments = r.PrescriptionMedicaments
                            .Select(pm => new MedicamentDetailsDto
                            {
                                IdMedicament = pm.Medicament.IdMedicament,
                                Name         = pm.Medicament.Name,
                                Dose         = pm.Dose,
                                Description  = pm.Medicament.Description,
                            })
                            .ToList()
                    })
                    .ToList()
            };
        }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.DTOs;
using WebApplication2.Models;

namespace WebApplication2.Services
{
    public class DbService : IDbService
    {
        private readonly DatabaseContext _ctx;

        public DbService(DatabaseContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<int> CreateAsync(CreatePrescriptionRequest dto)
        {
            if (dto.Medicaments.Count is < 1 or > 10)
                throw new ArgumentException("Recepta musi mieć 1–10 leków.");
            if (dto.DueDate < dto.Date)
                throw new ArgumentException("DueDate musi być >= Date.");
            
            Patient patient;
            if (dto.Patient.IdPatient.HasValue &&
                await _ctx.Patients.FindAsync(dto.Patient.IdPatient.Value) is Patient existing)
            {
                patient = existing;
            }
            else
            {
                patient = new Patient
                {
                    FirstName = dto.Patient.FirstName,
                    LastName  = dto.Patient.LastName,
                    Birthdate = dto.Patient.Birthdate
                };
                _ctx.Patients.Add(patient);
            }
            
            var ids = dto.Medicaments.Select(m => m.IdMedicament).ToList();
            var meds = await _ctx.Medicaments
                                 .Where(m => ids.Contains(m.IdMedicament))
                                 .ToListAsync();
            if (meds.Count != ids.Count)
                throw new InvalidOperationException("Jeden z leków nie istnieje.");
            
            var doctor = await _ctx.Doctors.FindAsync(1)
                         ?? throw new InvalidOperationException("Lekarz nie istnieje.");
            
            var pres = new Prescription
            {
                Date    = dto.Date,
                DueDate = dto.DueDate,
                Patient = patient,
                Doctor  = doctor
            };

            foreach (var m in dto.Medicaments)
            {
                pres.PrescriptionMedicaments.Add(new PrescriptionMedicament
                {
                    IdMedicament = m.IdMedicament,
                    Dose         = m.Dose,
                    Details      = m.Description
                });
            }

            _ctx.Prescriptions.Add(pres);
            await _ctx.SaveChangesAsync();
            return pres.IdPrescription;
        }
    }
}

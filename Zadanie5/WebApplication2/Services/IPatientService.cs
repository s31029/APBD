using WebApplication2.DTOs;

namespace WebApplication2.Services
{
    public interface IPatientService
    {
        Task<PatientDetailsDto> GetDetailsAsync(int idPatient);
    }
}
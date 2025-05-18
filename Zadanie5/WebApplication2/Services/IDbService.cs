using System.Threading.Tasks;
using WebApplication2.DTOs;

namespace WebApplication2.Services
{
    public interface IDbService
    {
        Task<int> CreateAsync(CreatePrescriptionRequest dto);
    }
}
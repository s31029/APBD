using WebApplication2.Models.DTOs;

namespace WebApplication2.Services
{
    public interface IDbService
    {
        Task<ClientDto> GetClient(int clientId);
        Task<int> CreateClientWithRental(CreateClientRentalDto dto);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Models.DTOs;

namespace WebApplication1.Services
{
    public interface IClientsService
    {
        Task<List<ClientTripDTO>> GetClientTripsAsync(int clientId);
        Task<int> CreateClientAsync(CreateClientDTO client);
        Task RegisterClientToTripAsync(int clientId, int tripId);
        Task UnregisterClientFromTripAsync(int clientId, int tripId);
    }
}
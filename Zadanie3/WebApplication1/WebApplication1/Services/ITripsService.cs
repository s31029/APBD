using WebApplication1.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication1.Services
{
    public interface ITripsService
    {
        Task<List<TripDTO>> GetTripsAsync();
    }
}
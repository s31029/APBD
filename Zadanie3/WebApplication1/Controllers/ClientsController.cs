using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.DTOs;
using WebApplication1.Services;
using WebApplication1.Exceptions;

namespace WebApplication1.Controllers
{
    [Route("api/clients")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientsService _clientsService;

        public ClientsController(IClientsService clientsService)
            => _clientsService = clientsService;

        [HttpGet("{id}/trips")]
        public async Task<IActionResult> GetClientTrips(int id)
        {
            try
            {
                var trips = await _clientsService.GetClientTripsAsync(id);
                return Ok(trips);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateClient([FromBody] CreateClientDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var newId = await _clientsService.CreateClientAsync(dto);
            return CreatedAtAction(nameof(GetClientTrips),
                                   new { id = newId },
                                   new { IdClient = newId });
        }

        [HttpPut("{id}/trips/{tripId}")]
        public async Task<IActionResult> RegisterClient(int id, int tripId)
        {
            try
            {
                await _clientsService.RegisterClientToTripAsync(id, tripId);
                return Ok(new { Message = $"Client {id} registered to trip {tripId}." });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (ConflictException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}/trips/{tripId}")]
        public async Task<IActionResult> UnregisterClient(int id, int tripId)
        {
            try
            {
                await _clientsService.UnregisterClientFromTripAsync(id, tripId);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
    }
}

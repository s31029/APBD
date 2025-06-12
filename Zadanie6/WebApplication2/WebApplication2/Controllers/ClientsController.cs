using Microsoft.AspNetCore.Mvc;
using WebApplication2.Exceptions;
using WebApplication2.Models.DTOs;
using WebApplication2.Services;
namespace WebApplication2.Controllers
{
    [Route("api/clients")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IDbService _service;
        public ClientsController(IDbService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var client = await _service.GetClient(id);
                return Ok(client);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateClientRentalDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var newClientId = await _service.CreateClientWithRental(dto);
                return CreatedAtAction(nameof(Get), new { id = newClientId }, new { Id = newClientId });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
    }
}
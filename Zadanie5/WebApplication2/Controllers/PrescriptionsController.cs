using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.DTOs;
using WebApplication2.Services; 

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrescriptionsController : ControllerBase
    {
        private readonly IDbService _svc;

        public PrescriptionsController(IDbService svc)
        {
            _svc = svc;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePrescriptionRequest req)
        {
            try
            {
                int id = await _svc.CreateAsync(req);
                return CreatedAtAction(nameof(GetById), new { idPrescription = id }, new { idPrescription = id });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{idPrescription}")]
        public IActionResult GetById(int idPrescription)
            => Ok();
    }
}
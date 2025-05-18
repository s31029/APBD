// Controllers/PatientController.cs
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication2.Services;
using WebApplication2.DTOs;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _svc;

        public PatientController(IPatientService svc)
        {
            _svc = svc;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                PatientDetailsDto dto = await _svc.GetDetailsAsync(id);
                return Ok(dto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Domain.Manager;
using Domain.Models;
using PracticeTwo.Dtos;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace PracticeTwo.Controllers
{
    [ApiController]
    [Route("api/patients")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientManager _manager;
        private readonly IGiftManager _giftManager;
        private readonly ILogger<PatientsController> _logger;

        public PatientsController(IPatientManager manager, IGiftManager giftManager, ILogger<PatientsController> logger)
        {
            _manager = manager;
            _giftManager = giftManager;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PatientDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.LastName) || string.IsNullOrWhiteSpace(dto.CI))
                return BadRequest();

            _logger.LogInformation("Creating patient {CI}", dto.CI);

            var patient = new Patient
            {
                Name = dto.Name,
                LastName = dto.LastName,
                CI = dto.CI
            };

            await _manager.CreateAsync(patient);

            return CreatedAtAction(nameof(GetByCi), new { ci = patient.CI }, patient);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            _logger.LogInformation("Getting all patients");
            var patients = _manager.GetAll();
            return Ok(patients);
        }

        [HttpGet("{ci}")]
        public IActionResult GetByCi(string ci)
        {
            _logger.LogInformation("Getting patient {CI}", ci);
            var patient = _manager.GetByCi(ci);
            if (patient == null)
            {
                _logger.LogWarning("Patient {CI} not found", ci);
                return NotFound("Patient not found");
            }
            return Ok(patient);
        }

        [HttpPut("{ci}")]
        public IActionResult Update(string ci, [FromBody] PatientUpdateDto dto)
        {
            if (dto == null || dto.CI != ci)
                return BadRequest();

            _logger.LogInformation("Updating patient {CI}", ci);
            if (!_manager.Update(ci, dto.Name, dto.LastName))
            {
                _logger.LogWarning("Patient {CI} not found for update", ci);
                return NotFound("Patient not found");
            }
            return NoContent();
        }

        [HttpDelete("{ci}")]
        public IActionResult Delete(string ci)
        {
            _logger.LogInformation("Deleting patient {CI}", ci);
            if (!_manager.Delete(ci))
            {
                _logger.LogWarning("Patient {CI} not found for deletion", ci);
                return NotFound("Patient not found");
            }
            return NoContent();
        }

        [HttpGet("{ci}/gift")]
        public async Task<IActionResult> GetGift(string ci)
        {
            if (_manager.GetByCi(ci) == null)
                return NotFound("Patient not found");

            var gift = await _giftManager.GetRandomGiftAsync(ci);
            if (gift == null)
            {
                _logger.LogWarning("No gift available for patient {CI}", ci);
                return StatusCode(502, "No gift available");
            }
            return Ok(gift);
        }
    }
}

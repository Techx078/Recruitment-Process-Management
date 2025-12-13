using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApis.Data;
using WebApis.Dtos;
using WebApis.Repository.CandidateRepository;

namespace WebApis.Controllers.UserController.CandidateController
{
    [ApiController]
    [Route("/api/[controller]")]
    public class CandidateController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ICandidateRepository _candidateRepository;
        public CandidateController(AppDbContext db , ICandidateRepository candidateRepository ) {
            _db = db;
            _candidateRepository = candidateRepository;
        }
        [HttpGet("{UserId}")]
        public async Task<IActionResult> GetCandidateDetailsByUserId(int UserId)
        {
            try { 
                var Candidate = await _candidateRepository.GetCandidateDetailsByUserId(UserId);
                return Ok(Candidate);
            }catch (KeyNotFoundException ex){
                return NotFound(ex.Message);
            }catch (Exception ex){
                return StatusCode(500, "An error occurred while processing your request.");

            }
        }

        [HttpGet("jobOpening/{UserId}")]
        public async Task<IActionResult> GetCnadidateJobOpeningDetailsByUserId(int UserId)
        {
            try{
                var jobOpenings = await _candidateRepository.GetCnadidateJobOpeningDetailsByUserId(UserId);
                return Ok(jobOpenings);
            }catch (KeyNotFoundException ex){
                return NotFound(ex.Message);
            }catch (Exception ex){
                return StatusCode(500, "An error occurred while processing your request.");

            }
        }

        [HttpPut("update/{UserId}")]
        public async Task<IActionResult> UpdateCandidateDetails(int UserId ,UpdateCandidateDto dto )
        {
            if (!ModelState.IsValid)
                return BadRequest("here");
            var candidate = await _candidateRepository.GetCandidateWithUserAsync(UserId);

            if (candidate == null)
                return NotFound("Candidate not found");

            await _candidateRepository.UpdateCandidateAsync(candidate, dto);
            return Ok(new
            {
                message = "Candidate updated successfully.",
                candidateId = candidate.Id
            });
        }
    }
}

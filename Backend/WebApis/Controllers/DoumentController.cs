using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using WebApis.Data;
using WebApis.Repository;

namespace WebApis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoumentController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ICommonRepository<Document> _repository;
        public DoumentController(AppDbContext db , ICommonRepository<Document>  repository )
        {
            _db = db;
            _repository = repository;
        }
        [HttpGet("All")]
        public async Task<IActionResult> GetAllDocuments()
        {
            var documents = await _repository.GetAllAsync();
            return Ok(documents);
        }
    }
}

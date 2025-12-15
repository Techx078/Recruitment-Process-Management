using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using WebApis.Data;
using WebApis.Repository;

namespace WebApis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly ICommonRepository<Document> _repository;
        public DocumentController(ICommonRepository<Document>  repository )
        {
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

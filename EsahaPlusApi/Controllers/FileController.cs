using Core.Core.Dtos;
using EsahaPlusApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EsahaPlusApi.Controllers
{
    [Route("esaha/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [AllowAnonymous]
        [HttpGet("[action]/{fileKey}")]
        public IActionResult GetAnonymousFile([FromRoute]string fileKey)
        {
            var file = _fileService.FileToByteArray(fileKey);

            if (file == null)
                return NotFound(new ErrorDto() { Message = "Dosya bulunamadı!" });

            return Ok(file);
        }

        [HttpGet("[action]/{fileKey}")]
        public IActionResult GetAuthenticatedFile([FromRoute]string fileKey)
        {
            var file = _fileService.FileToByteArray(fileKey);

            if (file == null)
                return NotFound(new ErrorDto() { Message = "Dosya bulunamadı!" });

            return Ok(file);
        }
    }
}
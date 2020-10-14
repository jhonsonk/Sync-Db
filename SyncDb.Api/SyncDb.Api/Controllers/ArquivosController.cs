using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SyncDb.Api.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SyncDb.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArquivosController : ControllerBase
    {
        private readonly ILogger<ArquivosController> _logger;
        private readonly AppSettings _appSettings;


        public ArquivosController(ILogger<ArquivosController> logger, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
        }

        [HttpPost]
        public async Task<IActionResult> OnPostUploadAsync(List<IFormFile> files, string cliente, string unidade)
        {
            try
            {
                var path = Path.Combine(_appSettings.FileFolder, cliente, unidade, "in");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                long size = files.Sum(f => f.Length);

                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        var filePath = Path.Combine(path, formFile.FileName);

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                    }
                }
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public IActionResult Get(string cliente, string unidade)
        {
            try
            {
                var path = Path.Combine(_appSettings.FileFolder, cliente, unidade, "out");

                if (Directory.Exists(path))
                {
                    return Ok(Directory.GetFiles(path));
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok();
        }

        [HttpDelete]
        public IActionResult Delete(string cliente, string unidade)
        {
            try
            {
                var path = Path.Combine(_appSettings.FileFolder, cliente, unidade, "out");
                if (Directory.Exists(path))
                {
                    int quantidade = Directory.GetFiles(path).Count();
                    Directory.Delete(path, true);
                    return Ok(quantidade);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace FileUploadAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public UploadController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpPost]
        public async Task<IActionResult> saveFileAsync(IFormFile file)
        {
            var storageConnectionString = _configuration["ConnectionString:AzureConnectionString"];
            if(CloudStorageAccount.TryParse(storageConnectionString, out CloudStorageAccount storageAccount))
            {
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("vhds");

                await container.CreateIfNotExistsAsync();
                var picBlob = container.GetBlockBlobReference(file.FileName);

                await picBlob.UploadFromStreamAsync(file.OpenReadStream());
                return Ok(picBlob.Uri);

            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    
}
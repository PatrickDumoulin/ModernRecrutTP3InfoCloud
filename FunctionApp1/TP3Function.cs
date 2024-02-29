using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System.Reflection;

namespace FunctionApp1
{
    public class TP3Function
    {
        private readonly ILogger<TP3Function> _logger;
		private readonly BlobServiceClient _blobServiceClient;
		private readonly IConfiguration _config;
		private string _conteneur;

		public TP3Function(BlobServiceClient blobServiceClient, IConfiguration config)
		{
			_blobServiceClient = blobServiceClient;
			_config = config;
			_conteneur = _config.GetSection("StorageAccount").GetValue<string>("ConteneurDocuments");
		}

		public TP3Function(ILogger<TP3Function> logger)
        {
            _logger = logger;
        }

        [Function("TP3Function")]
		public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
		{
			
			if (req.Body == null)
			{
				return new BadRequestObjectResult("Le fichier est vide");
			}
			
			string blobName = $"fichier_{Guid.NewGuid()}";
			var containerClient = _blobServiceClient.GetBlobContainerClient(_conteneur);
			var blobClient = containerClient.GetBlobClient(blobName);


			await blobClient.UploadAsync(req.Body, overwrite: true);

			return new OkObjectResult($"Fichier sauvegardé avec le nom {blobName}");
		}
	}
}

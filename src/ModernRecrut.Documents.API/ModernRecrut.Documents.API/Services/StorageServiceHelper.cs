using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ModernRecrut.Documents.API.Interfaces;
using ModernRecrut.Documents.API.Models;
using System.Reflection.Metadata;

namespace ModernRecrut.Documents.API.Services
{
	public class StorageServiceHelper : IStorageServiceHelper
	{
		private readonly BlobServiceClient _blobServiceClient;
		private readonly IConfiguration _config;
		private string _conteneur;

		public StorageServiceHelper(BlobServiceClient blobServiceClient, IConfiguration config)
		{
			_blobServiceClient = blobServiceClient;
			_config = config;
			_conteneur = _config.GetSection("StorageAccount").GetValue<string>("ConteneurDocuments");
        }

		public Task EnregistrerFichier(Models.Document document, string utilisateurId)
		{
			int randomNum = new Random().Next(1000, 9999);  // Nombre aléatoire
			string extension = Path.GetExtension(document.DocumentDetails.FileName);  // Extension du fichier
			string fichierNom = $"{utilisateurId}_{document.DocumentType}_{randomNum}{extension}";

            //Obtention d'un conteneur
            var containerClient = _blobServiceClient.GetBlobContainerClient(_conteneur);

			//Initialisation de l'objet BlobClient
			var blobClient = containerClient.GetBlobClient(fichierNom);

			//Sauvegarde du fichier
			return blobClient.UploadAsync(document.DocumentDetails.OpenReadStream(), true);
		}

		public async Task<IEnumerable<string>> ObtenirSelonUtilisateurId(string utilisateurId)
		{
            var sasToken = _config.GetSection("StorageAccount").GetValue<string>("SasToken");

            List<string> urlDocuments = new();

            //Obtention d'un conteneur blob
            var containerClient = _blobServiceClient.GetBlobContainerClient(_conteneur);

            //Lecture des blobs dans le conteneur
            await foreach (BlobItem blob in containerClient.GetBlobsAsync(prefix: utilisateurId))
			{
                //on veut aller chercher le Uri des blobs
                BlobClient blobClient = containerClient.GetBlobClient(blob.Name);

                var uri = blobClient.Uri;

                urlDocuments.Add(uri.ToString() + "?" + sasToken);
            }

            return urlDocuments;
        }

		public async Task<bool> SupprimerFichier(string fichierNom)
		{
		
			//Obtenir le conteneur
			var containerClient = _blobServiceClient.GetBlobContainerClient(_conteneur);

			//Obtenir le blob
			var blobClient = containerClient.GetBlobClient(fichierNom);

			//Supprimer le blob
			return await blobClient.DeleteIfExistsAsync();
		}
	}
}

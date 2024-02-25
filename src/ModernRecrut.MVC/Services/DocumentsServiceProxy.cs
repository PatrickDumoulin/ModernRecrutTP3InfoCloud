using ModernRecrut.MVC.Entites;
using ModernRecrut.MVC.Interfaces;
using ModernRecrut.MVC.Services;
using ModernRecrut.MVC.DTO;
using ModernRecrut.MVC.Models;

namespace ModernRecrut.MVC.Services
{
    public class DocumentsServiceProxy : IDocumentsService
    {
        #region Atttribut
        private readonly ILogger<DocumentsServiceProxy> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _documentsapiUrl = "api/Documents/";
        #endregion

        #region Constructeur
        public DocumentsServiceProxy(
                HttpClient httpClient, 
                ILogger<DocumentsServiceProxy> logger
        )
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        #endregion


        #region Méthodes publiques
        public async Task<IEnumerable<string>?> ObtenirSelonUtilisateurId(string utilisateurId)
		{
            // Journalisation
            _logger.LogInformation($"Requête à l'API Documents afin d'obtnir la liste des documents charger pour l'utilisateur {utilisateurId}");
            HttpResponseMessage response = await _httpClient.GetAsync(_documentsapiUrl + utilisateurId);

            if (!response.IsSuccessStatusCode)
            {
                // Journalisation
                _logger.LogError($"Erreur {response.StatusCode} lors de la requête d'obtention de documents pour l'utilisateur {utilisateurId}");

                return null;
            }

            IEnumerable<string>? documents = await response.Content.ReadFromJsonAsync<IEnumerable<string>>();
            return documents;
		}

		public async Task<bool> PostFileAsync(Document document, string utilisateurId)
		{
            // Journalisation
            _logger.LogInformation($"Envoi du document {document.DocumentDetails.Name} de type {document.DocumentType} par l'utilisatuer {utilisateurId} à l'API Document");
            
            RequeteDocument requeteDocument = new RequeteDocument {
                Document = document,
                UtilisateurId = utilisateurId
            };

            using (var content = new MultipartFormDataContent())
            {
                var fileContent = new StreamContent(requeteDocument.Document.DocumentDetails.OpenReadStream());
                content.Add(fileContent, "Document.DocumentDetails", document.DocumentDetails.FileName);
                content.Add(new StringContent(utilisateurId), "UtilisateurId");
                content.Add(new StringContent(document.DocumentType.ToString()), "Document.DocumentType");

                HttpResponseMessage response = await _httpClient.PostAsync(_documentsapiUrl, content);
            
				string responseBody = await response.Content.ReadAsStringAsync();

				if (!response.IsSuccessStatusCode)
				{
					// Journalisation
                    _logger.LogError($"Erreur {response.StatusCode} lors de la soumission du {document?.DocumentDetails?.Name} de type {document?.DocumentType} par l'utilisateur {utilisateurId} à l'API Document");

					return false;
				}

				return true;
			}
		}

        public async Task<bool> DeleteDocumentByName(string documentName)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync(_documentsapiUrl + documentName);

            if (!response.IsSuccessStatusCode)
            {
                //Journalisation
                _logger.LogError($"erreur {response.StatusCode} lors de la suppression du document {documentName}");
                return false;
            }

            _logger.LogInformation($"La suppression du document {documentName} s'est déroulée avec succès");
            return true;
        }
        #endregion
    }
}

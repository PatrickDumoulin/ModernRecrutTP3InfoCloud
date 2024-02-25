using ModernRecrut.MVC.DTO;
using ModernRecrut.MVC.Interfaces;
using ModernRecrut.MVC.Models;

namespace ModernRecrut.MVC.Services
{
    public class PostulationsServiceProxy : IPostulationsService
    {
        #region Attributs
        private readonly HttpClient _httpClient;
        private const string _postulationApiUrl = "api/Postulations/";
        private readonly ILogger<PostulationsServiceProxy> _logger;
        #endregion

        #region Constructor
        public PostulationsServiceProxy(HttpClient httpClient, ILogger<PostulationsServiceProxy> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        #endregion

        #region Méthodes publiques
        public async Task<IEnumerable<Postulation>?> ObtenirTout()
        {
            HttpResponseMessage response = await _httpClient.GetAsync(_postulationApiUrl);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(CustomLogEvents.Postulation, "Erreur lors de la requête pour obtenir toutes les postulations");

                return null;
            }
            IEnumerable<Postulation>? postulations = await response.Content.ReadFromJsonAsync<IEnumerable<Postulation>>();
            return postulations;
        }

        public async Task<Postulation?> ObtenirSelonId(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(_postulationApiUrl + id);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(CustomLogEvents.Postulation, $"Erreur - ObtenirSelon pour une postulation ID {id}");
                return null;
            }

            return await response.Content.ReadFromJsonAsync<Postulation>();
        }

        public async Task<Postulation> Ajouter(RequetePostulation requetePostulation)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(_postulationApiUrl, requetePostulation);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(CustomLogEvents.Postulation, $"Erreur lors de la création d'une postulation.");
                return null;
            }
            return await response.Content.ReadFromJsonAsync<Postulation>();
        }

        public async Task Modifier(Postulation postulation)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync(_postulationApiUrl + postulation.Id, postulation);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(CustomLogEvents.Postulation, $"Erreur - Modification de la postulation ID {postulation.Id}");
            }

        }

        public async Task Supprimer(Postulation postulation)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync(_postulationApiUrl + postulation.Id);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(CustomLogEvents.OffreEmploi, $"Erreur lors de la suppression de la postulation ID: {postulation.Id}");
            }
        }
        #endregion
    }
}

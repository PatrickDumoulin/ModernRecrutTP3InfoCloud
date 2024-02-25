using ModernRecrut.MVC.DTO;
using ModernRecrut.MVC.Interfaces;
using ModernRecrut.MVC.Models;

namespace ModernRecrut.MVC.Services
{
    public class FavorisServiceProxy : IFavorisService
    {
        #region Attributs
        private readonly HttpClient _httpClient;
        private const string _favorisApiUrl = "api/Favoris/";
        private readonly ILogger<FavorisServiceProxy> _logger;
        #endregion

        #region Constructor
        public FavorisServiceProxy(HttpClient httpClient, ILogger<FavorisServiceProxy> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        #endregion

        public async Task<IEnumerable<OffreEmploi>?> ObtenirSelonClef(string cle)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(_favorisApiUrl + cle);

            if (!response.IsSuccessStatusCode)
            {
                // Journalisation
                int statusCode = (int)response.StatusCode;
                if(statusCode >= 400 && statusCode < 500)
                {
                    // Journalisation comme erreur
					_logger.LogError(CustomLogEvents.Favoris, $"Erreur - ObtenirSelonCle favoris avec cle {cle}");
                } 
                else if (statusCode >= 500 && statusCode < 600)
                {
					// Journalisation comme critique
					_logger.LogCritical(CustomLogEvents.Favoris, $"Erreur critique - ObtenirSelonCle favoris avec cle {cle}");
                }
                
                return null;
            }

            return await response.Content.ReadFromJsonAsync<IEnumerable<OffreEmploi>>();
        }

        public async Task Ajouter(RequeteFavori requeteFavori)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(_favorisApiUrl, requeteFavori);

            if (!response.IsSuccessStatusCode)
            {
                // Journalisation
                int statusCode = (int)response.StatusCode;
                if(statusCode >= 400 && statusCode < 500)
                {
                    // Journalisation comme erreur
					_logger.LogError(CustomLogEvents.Favoris, $"Erreur lors de l'ajout favoris avec l'ip {requeteFavori.Cle}");
                } 
                else if (statusCode >= 500 && statusCode < 600)
                {
					// Journalisation comme critique
					_logger.LogCritical(CustomLogEvents.Favoris, $"Erreur critique - ObtenirSelonCle favoris avec l'ip {requeteFavori.Cle}");
                }
            }
        }

        public async Task Supprimer(string cle, OffreEmploi item)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync(_favorisApiUrl + item.Id + "?cle=" + cle);

            if (!response.IsSuccessStatusCode)
            {
                // Journalisation
                int statusCode = (int)response.StatusCode;
                if(statusCode >= 400 && statusCode < 500)
                {
                    // Journalisation comme erreur
					_logger.LogError(CustomLogEvents.Favoris, $"Erreur lors de la suppression du favoris ID {item.Id } avec l'ip {cle}");
                } 
                else if (statusCode >= 500 && statusCode < 600)
                {
					// Journalisation comme critique
					_logger.LogCritical(CustomLogEvents.Favoris, $"Erreur critique lors de la suppression du favoris ID {item.Id } avec l'ip {cle}");
                }
            }
        }
    }
}

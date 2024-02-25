using Microsoft.AspNetCore.Authorization.Infrastructure;
using ModernRecrut.MVC.DTO;
using ModernRecrut.MVC.Interfaces;
using ModernRecrut.MVC.Models;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace ModernRecrut.MVC.Services
{
	public class OffreEmploisServiceProxy : IOffreEmploisService
	{
		#region Attributs
		private readonly HttpClient _httpClient;
		private const string _offreEmploiApiUrl = "api/OffreEmplois/";
		private readonly ILogger<OffreEmploisServiceProxy> _logger;
        #endregion

        #region Constructor
        public OffreEmploisServiceProxy(HttpClient httpClient, ILogger<OffreEmploisServiceProxy> logger)
        {
            _httpClient = httpClient;
			_logger = logger;
        }
        #endregion

        public async Task<OffreEmploi?> Ajouter(RequeteOffreEmploi requeteOffreEmploi)
		{
			HttpResponseMessage response = await _httpClient.PostAsJsonAsync(_offreEmploiApiUrl, requeteOffreEmploi);

			if (!response.IsSuccessStatusCode)
			{
					// Journalisation
				int statusCode = (int)response.StatusCode;
				if (statusCode >= 400 && statusCode < 500)
				{
					// Journalisation comme erreur
					_logger.LogError(CustomLogEvents.OffreEmploi, $"Erreur lors de la création d'une offre d'emploi.");
				}
				else if (statusCode >= 500 && statusCode < 600)
				{
					// Journalisation comme critique
					_logger.LogCritical(CustomLogEvents.OffreEmploi, $"Erreur critique lors de la création d'une offre d'emploi.");
				}
				return null;
			}

			return await response.Content.ReadFromJsonAsync<OffreEmploi?>();
		}

		public async Task Modifier(OffreEmploi item)
		{
			HttpResponseMessage response = await _httpClient.PutAsJsonAsync(_offreEmploiApiUrl + item.Id, item);

			if (!response.IsSuccessStatusCode)
			{
				// Journalisation
				int statusCode = (int)response.StatusCode;
				if(statusCode >= 400 && statusCode < 500)
				{
					// Journalisation comme erreur
					_logger.LogError(CustomLogEvents.OffreEmploi, $"Erreur - Modification offre d'emploi ID {item.Id}");
				}
                else if (statusCode >= 500 && statusCode < 600)
                {
					// Journalisation comme critique
					_logger.LogCritical(CustomLogEvents.OffreEmploi, $"Erreur critique - Modification offre d'emploi ID {item.Id}");
                }
			}
		}

		public async Task<OffreEmploi?> ObtenirSelonId(int id)
		{
			HttpResponseMessage response = await _httpClient.GetAsync(_offreEmploiApiUrl + id);

			if(!response.IsSuccessStatusCode)
			{
				// Journalisation
				int statusCode = (int)response.StatusCode;
				if(statusCode >= 400 && statusCode < 500)
				{
					// Journalisation comme erreur
					_logger.LogError(CustomLogEvents.OffreEmploi, $"Erreur - ObtenirSelon offre d'emploi ID {id}");
				}
                else if (statusCode >= 500 && statusCode < 600)
                {
					// Journalisation comme critique
					_logger.LogCritical(CustomLogEvents.OffreEmploi, $"Erreur critique - ObtenurSelonId offre d'emploi ID {id}");
                }

                return null;
			}
			
			return await response.Content.ReadFromJsonAsync<OffreEmploi>();
		}

		public async Task<IEnumerable<OffreEmploi>?> ObtenirTout()
		{
			HttpResponseMessage response = await _httpClient.GetAsync(_offreEmploiApiUrl);

			if (!response.IsSuccessStatusCode)
			{
				// Journalisation
				int statusCode = (int)response.StatusCode;
				if(statusCode >= 400 && statusCode < 500)
				{
					// Journalisation comme erreur
					_logger.LogError(CustomLogEvents.OffreEmploi, "Erreur lors de la requête pour obtenir toutes les offres d'emploi");
				}
                else if (statusCode >= 500 && statusCode < 600)
                {
					// Journalisation comme critique
					_logger.LogCritical(CustomLogEvents.OffreEmploi, "Erreur critique lors de la requête pour obtenir toutes les offres d'emploi");
                }
				return null;
			}

			IEnumerable<OffreEmploi>? offreEmplois = await response.Content.ReadFromJsonAsync<IEnumerable<OffreEmploi>>();

			return offreEmplois;
		}

		public async Task Supprimer(OffreEmploi item)
		{
			HttpResponseMessage response = await _httpClient.DeleteAsync(_offreEmploiApiUrl + item.Id);

			if (!response.IsSuccessStatusCode)
			{
				// Journalisation
				int statusCode = (int)response.StatusCode;
				if(statusCode >= 400 && statusCode < 500)
				{
					// Journalisation comme erreur
					_logger.LogError(CustomLogEvents.OffreEmploi, $"Erreur lors de la suppression de l'offre d'emploi ID: {item.Id}");
				}
                else if (statusCode >= 500 && statusCode < 600)
                {
					// Journalisation comme critique
					_logger.LogCritical(CustomLogEvents.OffreEmploi, $"Erreur critique lors de la suppression de l'offre d'emploi ID: {item.Id}");
                }
			}
		}
	}
}

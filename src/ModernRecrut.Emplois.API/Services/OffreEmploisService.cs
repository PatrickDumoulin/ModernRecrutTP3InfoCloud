using ModernRecrut.Emplois.API.DTO;
using ModernRecrut.Emplois.API.Interfaces;
using ModernRecrut.Emplois.API.Models;

namespace ModernRecrut.Emplois.API.Services
{
	public class OffreEmploisService : IOffreEmploisService
	{
		private readonly IAsyncRepository<OffreEmploi> _offreEmploisRepository;

        public OffreEmploisService(IAsyncRepository<OffreEmploi> offreEmploisReposytory)
        {
			_offreEmploisRepository = offreEmploisReposytory;
        }
        public async Task<OffreEmploi?> Ajouter(RequeteOffreEmploi requeteOffreEmploi)
		{
			// Validation des dates de début et de fin
			DateTime today = DateTime.Today;
			bool validateDate =	requeteOffreEmploi.DateAffichage >= today &&
								requeteOffreEmploi.DateFin >= today &&
								requeteOffreEmploi.DateFin >= requeteOffreEmploi.DateAffichage;
			if (!validateDate)
				return null;

			OffreEmploi offreEmploi = new OffreEmploi
			{
				DateAffichage = requeteOffreEmploi.DateAffichage,
				DateFin = requeteOffreEmploi.DateFin,
				Description = requeteOffreEmploi.Description,
				Poste = requeteOffreEmploi.Poste,
			};

			return await _offreEmploisRepository.AddAsync(offreEmploi);
		}

		public async Task<bool> Modifier(OffreEmploi item)
		{
			if(item.DateAffichage > item.DateFin)
				return false;

			await _offreEmploisRepository.EditAsync(item);
			return true;
		}

		public Task<OffreEmploi?> ObtenirSelonId(int id)
		{
			return _offreEmploisRepository.GetByIdAsync(id);
		}

		public async Task<IEnumerable<OffreEmploi>> ObtenirTout()
		{
			IEnumerable<OffreEmploi> offreEmplois = await _offreEmploisRepository.ListAsync();
			offreEmplois = offreEmplois.Where(o => o.DateAffichage.Date <= DateTime.Today && o.DateFin.Date >= DateTime.Today);

			return offreEmplois;
		}

		public Task Supprimer(OffreEmploi item)
		{
			return _offreEmploisRepository.DeleteAsync(item);
		}
	}
}

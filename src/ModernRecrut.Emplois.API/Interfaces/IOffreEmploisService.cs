using ModernRecrut.Emplois.API.DTO;
using ModernRecrut.Emplois.API.Models;

namespace ModernRecrut.Emplois.API.Interfaces
{
	public interface IOffreEmploisService
	{
		public Task<OffreEmploi?> Ajouter(RequeteOffreEmploi requeteOffreEmploi);
		public Task<OffreEmploi?> ObtenirSelonId(int id);
		public Task<IEnumerable<OffreEmploi>> ObtenirTout();
		public Task<bool> Modifier(OffreEmploi item);
		public Task Supprimer(OffreEmploi item);
	}
}

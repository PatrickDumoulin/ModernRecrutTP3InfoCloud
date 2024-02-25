using ModernRecrut.MVC.DTO;
using ModernRecrut.MVC.Models;

namespace ModernRecrut.MVC.Interfaces
{
	public interface IOffreEmploisService
	{
		Task<IEnumerable<OffreEmploi>?> ObtenirTout();
		Task<OffreEmploi?> ObtenirSelonId(int id);
		Task<OffreEmploi?> Ajouter(RequeteOffreEmploi requeteOffreEmploi);
		Task Modifier(OffreEmploi item);
		Task Supprimer(OffreEmploi item);
	}
}

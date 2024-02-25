using ModernRecrut.MVC.DTO;
using ModernRecrut.MVC.Models;

namespace ModernRecrut.MVC.Interfaces
{
    public interface IFavorisService
    {
        Task<IEnumerable<OffreEmploi>?> ObtenirSelonClef(string cle);
        Task Ajouter(RequeteFavori requeteFavori);
        Task Supprimer(string cle, OffreEmploi item);

    }
}

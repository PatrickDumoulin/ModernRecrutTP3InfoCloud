using ModernRecrut.MVC.DTO;
using ModernRecrut.MVC.Models;

namespace ModernRecrut.MVC.Interfaces
{
    public interface IPostulationsService
    {
        Task<IEnumerable<Postulation>?> ObtenirTout();
        Task<Postulation?> ObtenirSelonId(int id);
        Task<Postulation> Ajouter(RequetePostulation requetePostulation);
        Task Modifier(Postulation postulation);
        Task Supprimer(Postulation postulation);
    }
}

using ModernRecrut.Postulation.API.DTO;
using ModernRecrut.Postulation.API.Models;
namespace ModernRecrut.Postulation.API.Interfaces
{
    public interface IPostulationsService
    {
        public Task<Models.Postulation?> Ajouter(RequetePostulation requetePostulation);
        public Task<Models.Postulation?> ObtenirSelonId(int id);
        public Task<IEnumerable<Models.Postulation>> ObtenirTout();
        public Task<bool> Modifier(Models.Postulation postulation);
        public Task Supprimer(Models.Postulation postulation);
    }
}

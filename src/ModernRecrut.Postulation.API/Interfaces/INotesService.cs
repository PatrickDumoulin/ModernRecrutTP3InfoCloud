using ModernRecrut.Postulation.API.DTO;
using ModernRecrut.Postulation.API.Models;

namespace ModernRecrut.Postulation.API.Interfaces
{
    public interface INotesService
    {
        Task<Note> Ajouter(RequeteNote requeteNote);
        Task<Note> ObtenirSelonId(int id);
        Task<IEnumerable<Note>> ObtenirTout();
        Task<bool> Modifier(Note note);
        Task Supprimer(Note note);
    }
}

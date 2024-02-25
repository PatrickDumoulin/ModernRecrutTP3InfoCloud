using ModernRecrut.MVC.DTO;
using ModernRecrut.MVC.Models;
using System.Collections;

namespace ModernRecrut.MVC.Interfaces
{
    public interface INotesService
    {
        Task<IEnumerable<Note>?> ObtenirTout();
        Task<Note?> ObtenirSelonId(int id);
        Task<Note> Ajouter(RequeteNote requeteNote);
        Task Modifier(Note note);
        Task Supprimer(Note note);
    }
}

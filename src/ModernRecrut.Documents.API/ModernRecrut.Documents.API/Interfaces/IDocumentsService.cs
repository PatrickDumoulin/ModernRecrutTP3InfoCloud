using ModernRecrut.Documents.API.Entites;
using ModernRecrut.Documents.API.Models;

namespace ModernRecrut.Documents.API.Interfaces
{
    public interface IDocumentsService
    {
        IEnumerable<string> ObtenirSelonUtilisateurId(string utilisateurId);
        Task PostFileAsync(Document document, string utilisateurId);
        bool Supprimer(string nomFichier);
    }
}

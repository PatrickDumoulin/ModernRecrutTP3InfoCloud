using ModernRecrut.MVC.DTO;
using ModernRecrut.MVC.Entites;
using ModernRecrut.MVC.Models;

namespace ModernRecrut.MVC.Interfaces
{
    public interface IDocumentsService
    {
        Task<IEnumerable<string>?> ObtenirSelonUtilisateurId(string utilisateurId);
        Task<bool> PostFileAsync(Document document, string utilisateurId);
        Task<bool> DeleteDocumentByName(string documentName);

    }
}

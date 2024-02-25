using ModernRecrut.Documents.API.Models;

namespace ModernRecrut.Documents.API.Interfaces
{
    public interface IStorageServiceHelper
    {
        Task<IEnumerable<string>> ObtenirSelonUtilisateurId(string utilisateurId);

        Task EnregistrerFichier(Document document, string utilisateurId);

        Task<bool> SupprimerFichier(string fichierNom);
    }
}

using ModernRecrut.Favoris.API.Models;

namespace ModernRecrut.Favoris.API.Interfaces
{
    public interface ICacheTools
    {
        int CalculTailleListOffresEmploi(IEnumerable<OffreEmploi> offresEmploi);
    }
}

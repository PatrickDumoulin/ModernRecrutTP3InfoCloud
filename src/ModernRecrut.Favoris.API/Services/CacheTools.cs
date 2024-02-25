using ModernRecrut.Favoris.API.Interfaces;
using ModernRecrut.Favoris.API.Models;
using System.Runtime.InteropServices;

namespace ModernRecrut.Favoris.API.Services
{
    public class CacheTools : ICacheTools
    {
        public int CalculTailleListOffresEmploi(IEnumerable<OffreEmploi> offresEmploi)
        {
            int taille = 0;

            foreach (OffreEmploi offreEmploi in offresEmploi)
            {
                taille += offreEmploi.Id.ToString().Length;
                taille += offreEmploi.DateAffichage.ToString().Length;
                taille += offreEmploi.DateFin.ToString().Length;
                taille += offreEmploi.Description.ToString().Length;
                taille += offreEmploi.Poste.ToString().Length;
            }

            return taille;
        }
    }
}

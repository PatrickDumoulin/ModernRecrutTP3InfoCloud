using ModernRecrut.MVC.Models;
using System.ComponentModel.DataAnnotations;

namespace ModernRecrut.MVC.DTO
{
    public class RequeteFavori
    {
        [Key]

        public string Cle { get; set; }


        public OffreEmploi OffreEmploi { get; set; }

    }
}

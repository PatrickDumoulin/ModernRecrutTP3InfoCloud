using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ModernRecrut.MVC.DTO
{
    public class RequetePostulation
    {
        public string CandidatId { get; set; }

        [
            DisplayName ("Offre d'emploi"),
            Required(ErrorMessage = "Veuillez sélectionner une offre d'emploi"),
        ]
        public int OffreDemploiId { get; set; }
        [
            DisplayName ("Prétention salarial"),
            Required(ErrorMessage = "Veuillez entrer votre prétention salarial"),
            DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)
        ]
        public decimal PretentionSalariale { get; set; }
        [
            DisplayName ("Date de disponibilité"),
            Required(ErrorMessage = "Veuillez renseigner la date du début de l'affichage"),
            DataType (DataType.Date),
            DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)
        ]
        public DateTime DateDisponibilite { get; set; }
    }
}

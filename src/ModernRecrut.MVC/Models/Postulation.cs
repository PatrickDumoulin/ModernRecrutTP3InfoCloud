using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ModernRecrut.MVC.Models
{
    public class Postulation
    {

		[JsonPropertyName("id")]
		public int Id { get; set; }

        public string CandidatId { get; set; }
        [
            DisplayName ("Offre d'emploi"),
            Required(ErrorMessage = "Veuillez sélectionner une offre d'emploi"),
        ]
        public int OffreDEmploiId { get; set; }
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

        [JsonIgnore]
        public virtual ICollection<Note>? Notes { get; set; }
    }
}

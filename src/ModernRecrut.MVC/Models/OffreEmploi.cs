using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ModernRecrut.MVC.Models
{
	public class OffreEmploi
	{
		[JsonPropertyName("id")]
		public int Id { get; set; }

		[	
			DisplayName("Date de début affichage"), 
			Required(ErrorMessage = "Veuillez renseigner la date du début de l'affichage"),
			DataType(DataType.Date),
			DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)
		]
		public DateTime DateAffichage { get; set; }

		[	
			DisplayName("Date de fin affichage"), 
			Required(ErrorMessage = "Veuillez renseigner la date de fin de l'affichage"),
			DataType(DataType.Date),
			DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)
		]
		public DateTime DateFin {  get; set; }

		[DisplayName("Description du poste"), Required(ErrorMessage = "Veuillez renseigner la description du poste")]
		public string Description { get; set; }

		[DisplayName("Intitulé du poste"), Required(ErrorMessage = "Veuillez renseigner l'intitulé du poste")]
		public string Poste { get; set; }
	}
}

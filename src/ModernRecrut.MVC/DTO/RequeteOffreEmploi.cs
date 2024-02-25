using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ModernRecrut.MVC.DTO
{
	public class RequeteOffreEmploi
	{
		[	DisplayName("Date de début affichage"), 
			Required(ErrorMessage = "Veuillez renseigner la date du début de l'affichage"),
			DataType(DataType.Date),
			DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)
		]

		public DateTime DateAffichage { get; set; }

		[	DisplayName("Date de fin affichage"), 
			Required(ErrorMessage = "Veuillez renseigner la date de fin de l'affichage"),
			DataType(DataType.Date),
			DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)
		]
		public DateTime DateFin {  get; set; }

		[DisplayName("Description du poste"), Required(ErrorMessage = "Veuillez renseigner la description du poste")]
#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
		public string Description { get; set; }
#pragma warning restore CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.

		[DisplayName("Intitulé du poste"), Required(ErrorMessage = "Veuillez renseigner l'intitulé du poste")]
#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
		public string Poste { get; set; }
#pragma warning restore CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
	}
}

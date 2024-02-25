using ModernRecrut.Documents.API.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ModernRecrut.Documents.API.DTO
{
	public class RequeteDocument
	{
		public Document Document { get; set; }

		[
			DisplayName("ID Utilisateur"),
			Required(ErrorMessage = "L'id de l'utilisateur est requis.")
		]
		public string UtilisateurId { get; set; }
	}
}

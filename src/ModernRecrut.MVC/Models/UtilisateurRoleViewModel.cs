using System.ComponentModel;

namespace ModernRecrut.MVC.Models
{
	public class UtilisateurRoleViewModel
	{
		[
			DisplayName("Utilisateur")
		]
		public string UtilisateurId { get; set; }

		[
			DisplayName("Nom du role")
		]
		public string RoleName { get; set; }
	}
}

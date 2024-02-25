using Microsoft.EntityFrameworkCore;

using ModernRecrut.Documents.API.Entites;
using ModernRecrut.Documents.API.Interfaces;
using ModernRecrut.Documents.API.Models;
using System.IO;

namespace ModernRecrut.Documents.API.Services
{
	public class DocumentService : IDocumentsService
	{
		public IEnumerable<string> ObtenirSelonUtilisateurId(string utilisateurId)
		{
			List<string> ListeDocuments = new List<string>();

			IEnumerable<string> fichiers = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "documents"));

			foreach (var fichier in fichiers)
			{
				var fichierNom = Path.GetFileName(fichier);
				if (fichierNom.StartsWith($"{utilisateurId}_"))
				{
					ListeDocuments.Add(fichierNom);
				}
			}

			return ListeDocuments;
		}

		public async Task PostFileAsync(Document document, string utilisateurId)
		{
			int randomNum = new Random().Next(1000, 9999);  // Nombre aléatoire
			string extension = Path.GetExtension(document.DocumentDetails.FileName);  // Extension du fichier
			string fichierNom = $"{utilisateurId}_{document.DocumentType}_{randomNum}{extension}";

			string cheminFichier = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "documents", fichierNom);

			// vérifie si le dossier existe, si pas => création
			if (!Directory.Exists(Path.GetDirectoryName(cheminFichier)))
				Directory.CreateDirectory(Path.GetDirectoryName(cheminFichier));


			using (var stream = new FileStream(cheminFichier, FileMode.Create))
			{
				await document.DocumentDetails.CopyToAsync(stream); // Copie le contenu du document dans le fichier
			}
		}

		public bool Supprimer(string fichierNom)
		{
			string cheminFichier = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "documents", fichierNom);

			if (File.Exists(cheminFichier))
			{
				File.Delete(cheminFichier);
				return true;
			}
			return false;
		}
	}
}

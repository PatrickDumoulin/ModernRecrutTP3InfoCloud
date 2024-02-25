using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModernRecrut.MVC.Areas.Identity.Data;
using ModernRecrut.MVC.DTO;
using ModernRecrut.MVC.Interfaces;
using ModernRecrut.MVC.Models;

namespace ModernRecrut.MVC.Controllers
{
	public class DocumentsController : Controller
	{
		#region Attributs
		private readonly ILogger<DocumentsController> _logger;
		private readonly IDocumentsService _documentsService;
		private readonly IConfiguration _config;
		private readonly UserManager<Utilisateur> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
        #endregion

        #region Controlleur
        public DocumentsController(
				ILogger<DocumentsController> logger,
				IDocumentsService documentsService,
				IConfiguration config,
				UserManager<Utilisateur> userManager,
				RoleManager<IdentityRole> roleManager
		)
        {
			_logger = logger;
            _documentsService = documentsService;
            _config = config;
			_userManager = userManager;
			_roleManager = roleManager;
        }
        #endregion

        #region Méthodes Publiques
        // GET: DocumentsController
		[Authorize(Roles = "Candidat")]
        public async Task<ActionResult> Index()
		{
			// Obtenir l'utilisateur courant
            Utilisateur utilisateur = await _userManager.GetUserAsync(User);

			// Journalisation
			_logger.LogInformation($"Visite de la page des documents téléchargés par l'utilisateur {utilisateur.UserName}" );

			IEnumerable<string>? documents = await _documentsService.ObtenirSelonUtilisateurId(utilisateur.Id);

			return View(documents);
		}

		[Authorize(Roles = "Candidat")]
		public ActionResult Create()
		{
			// Journalisation
			_logger.LogInformation($"Visite de la page d'ajout de documents téléchargés par l'utilisateur {User.Identity.Name}" );
			return View();
		}

		[Authorize(Roles = "Candidat")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create(Document document)
		{
			// Journalisation
			_logger.LogInformation($"Soumission d'un document de type {document.DocumentType} par l'utilisateur {User.Identity.Name}");

			// Vérifier l'extension du fichier
			string[] extensionPermises = new string[] { ".docx", ".pdf" };
			string? extensionFichier = Path.GetExtension(document.DocumentDetails.FileName).ToLower();

			if (!extensionPermises.Contains(extensionFichier))
			{
				// Journalisation
				_logger.LogWarning($"Le document soumis par l'utilisateur {User.Identity.Name} est dans un format invalide ({extensionFichier})");
				ModelState.AddModelError("document.DocumentDetails", $"Les fichiers avec l'extension {extensionFichier} ne sont pas autorisés. Les extensions autorisées sont : {string.Join(", ", extensionPermises)}");
			}

			if (ModelState.IsValid)
			{
				// Obtenir l'utilisateur actuellement authentifié
				Utilisateur utilisateur = await _userManager.GetUserAsync(User);

				bool uploadDocument = await _documentsService.PostFileAsync(document, utilisateur.Id);

				if (uploadDocument)
				{
					// Journalisation
					_logger.LogInformation($"Le document - {document.DocumentDetails.Name} - soumis de type {document.DocumentType} par l'utilisateur {User.Identity.Name} a été correctement enregistrer dans son compte");
					return RedirectToAction("Index");
				}

				// Journalisation
				_logger.LogWarning($"Le document - {document?.DocumentDetails?.Name} - soumis de type {document?.DocumentType} par l'utilisateur {User.Identity.Name} n'a pu être correctement enregistrer");
			}

			// Journalisation
			_logger.LogWarning($"Erreur de validation lors de la soumission d'un document - {document?.DocumentDetails?.Name} -  de type - {document?.DocumentType}- par l'utilisateur {User.Identity.Name}");
			return View(document);
		}

		//public async Task<ActionResult> Delete(string documentName)
		//{

		//	// Obtenir l'utilisateur courant
  //          Utilisateur utilisateur = await _userManager.GetUserAsync(User);

		//	// Journalisation
		//	_logger.LogInformation($"Page de suprression du document {documentName} par l'utilisateur {utilisateur.UserName}");

		//	if(utilisateur.Id != documentName.Split("_")[0])
		//	{
		//		// Journalisation
		//		_logger.LogWarning($"L'utilisateur {utilisateur.UserName} n'a pas le droit de supprimer le document {documentName}!");

		//		return NotFound();
		//	}


		//	return View(documentName);
		//}

		//[Authorize(Roles = "Candidat")]
		//[HttpPost, ActionName("Delete")]
		//[ValidateAntiForgeryToken]
		//public async Task<ActionResult> DeleteConfirmed(string documentName)
		//{
		//	// Obtenir l'utilisateur courant
  //          Utilisateur utilisateur = await _userManager.GetUserAsync(User);

		//	// Journalisation
		//	_logger.LogInformation($"Page de confirmation de suprression du document {documentName} par l'utilisateur {utilisateur.UserName}");

		//	if(utilisateur.Id != documentName.Split("_")[0])
		//	{
		//		// Journalisation
		//		_logger.LogWarning($"L'utilisateur {utilisateur.UserName} n'a pas le droit de supprimer le document {documentName}!");

		//		return NotFound();
		//	}

		//	bool validationSuppression = await _documentsService.DeleteDocumentByName(documentName);

		//	if (validationSuppression)
		//	{
		//		// Journalisation
		//		_logger.LogInformation($"Le document {documentName} a été supprimer par l'utilisateur {utilisateur.UserName}");
		//	}

		//	// Journalisation
  //          _logger.LogWarning($"Le document {documentName} n'a put être été supprimer par l'utilisateur {utilisateur.UserName}");
  //          return RedirectToAction("Index");
		//}
		#endregion
	}
}

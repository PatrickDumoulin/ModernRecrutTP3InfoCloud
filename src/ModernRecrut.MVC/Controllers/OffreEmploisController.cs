using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModernRecrut.MVC.DTO;
using ModernRecrut.MVC.Interfaces;
using ModernRecrut.MVC.Models;

namespace ModernRecrut.MVC.Controllers
{
	public class OffreEmploisController : Controller
	{
		#region Attributs
		private readonly ILogger<OffreEmploisController> _logger;
		private readonly IOffreEmploisService _offreEmploisService;
        private readonly IFavorisService _favorisService;
        #endregion

        #region Constructor
        public OffreEmploisController(
				ILogger<OffreEmploisController> logger,
				IOffreEmploisService offreEmploisService, 
				IFavorisService favorisService
		)
        {
            _offreEmploisService = offreEmploisService;
			_favorisService = favorisService;
			_logger = logger;
        }
        #endregion

        // GET: OffreEmploisController
        public async Task<ActionResult> Index(string searchString)
		{
			// Journalisation
			_logger.LogInformation($"Consultation de la page des offre d'emploi par l'utilisateur {User?.Identity?.Name ?? Request.HttpContext.Connection?.RemoteIpAddress?.ToString()}");

			ViewData["CurrentFilter"] = searchString;

			IEnumerable<OffreEmploi> offresEmploi = await _offreEmploisService.ObtenirTout() ?? new List<OffreEmploi>();

			if(offresEmploi != null && !string.IsNullOrEmpty(searchString))
			{
				offresEmploi = offresEmploi.Where(o => o.Poste.Contains(searchString, StringComparison.CurrentCultureIgnoreCase));
			}

			// Ajout d'un champ pour favoris
            string? ip = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
			if (ip != null)
			{
				IEnumerable<OffreEmploi>? favoris = await _favorisService.ObtenirSelonClef(ip);
				if(favoris != null)
					ViewData["Favoris"] = favoris.Select(f => f.Id).ToList();
			}

			return View(offresEmploi);
		}

		// GET: OffreEmploisController/Details/5
		public async Task<ActionResult> Details(int id)
		{
			OffreEmploi? offreEmploi = await _offreEmploisService.ObtenirSelonId(id);

			if (offreEmploi == null)
			{
                // Journalisation
                _logger.LogWarning($"Consultation d'une offre d'emploi - introuvalbe - par l'utilisateur {User?.Identity?.Name ?? Request.HttpContext.Connection?.RemoteIpAddress?.ToString()}");
				return NotFound();
			}


			// Journalisation
			_logger.LogInformation($"Consultation de la page de l'offre d'emploi {offreEmploi.Poste} par l'utilisateur {User?.Identity?.Name ?? Request.HttpContext.Connection?.RemoteIpAddress?.ToString()}");

			// Ajout d'un champ pour EstFavoris // Defini par default à faux 
			ViewData["EstFavori"] = false;

            string? ip = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
			if (ip != null)
			{
				IEnumerable<OffreEmploi>? favoris = await _favorisService.ObtenirSelonClef(ip);
				if(favoris != null)
					ViewData["EstFavori"] = favoris.Any(f => f.Id == offreEmploi.Id);
			}

			return View(offreEmploi);
		}

		[Authorize(Roles="Employé")]
		// GET: OffreEmploisController/Create
		public ActionResult Create()
		{
			// Journalisation
			_logger.LogInformation($"Visite de la page de création d'une offre d'emploi par l'utlisateur {User.Identity.Name}");

			return View();
		}

		[Authorize(Roles="Employé")]
		// POST: OffreEmploisController/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create(RequeteOffreEmploi requeteOffreEmploi)
		{
			if (requeteOffreEmploi.DateAffichage < DateTime.Today)
				ModelState.AddModelError("DateAffichage", "La date de début d'affichage ne peut être dans le passé");

			if (requeteOffreEmploi.DateFin <= DateTime.Today)
				ModelState.AddModelError("DateFin", "La date de fin d'affichage ne peut être dans le passé");

			if (requeteOffreEmploi.DateFin < requeteOffreEmploi.DateAffichage)
				ModelState.AddModelError("DateFin", "La date de fin doit être postérieur ou égale à la date de début");

			if (ModelState.IsValid)
			{
				OffreEmploi? offreEmploi = await _offreEmploisService.Ajouter(requeteOffreEmploi);

				if(offreEmploi == null)
				{
					ModelState.AddModelError("all","Problème lors de l'ajout de l'offre d'emploi, veuillez reessayer!");
					return View(requeteOffreEmploi);
				}
				// Jounaliser Création Emploi
				_logger.LogInformation(CustomLogEvents.OffreEmploi, $"Ajout d'une offre d'emploi ID - {offreEmploi.Id}");

				return RedirectToAction(nameof(Index));
			}
			return View(requeteOffreEmploi);
		}

		//GET: OffreEmploisController/Edit/5
		public async Task<ActionResult> Edit(int id)
		{
			OffreEmploi? offreEmploi = await _offreEmploisService.ObtenirSelonId(id);

			if (offreEmploi == null)
				return NotFound();

			return View(offreEmploi);
		}

		// POST: OffreEmploisController/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Edit(OffreEmploi offreEmploi)
		{
			if (offreEmploi.DateFin < offreEmploi.DateAffichage)
				ModelState.AddModelError("DateFin", "La date de fin doit être postérieur ou égale à la date de début");

			if (ModelState.IsValid)
			{
				await _offreEmploisService.Modifier(offreEmploi);
				// Journaliser la modification
				_logger.LogInformation(CustomLogEvents.OffreEmploi, $"Modification de l'offre d'emploi - ID {offreEmploi.Id}");

				return RedirectToAction(nameof(Index));
			}

			return View(offreEmploi);
		}

		// GET: OffreEmploisController/Delete/5
		[Authorize(Roles="Employé")]
		public async Task<ActionResult> Delete(int id)
		{
			OffreEmploi? offreEmploi = await _offreEmploisService.ObtenirSelonId(id);

			if (offreEmploi == null)
				return NotFound();

			return View(offreEmploi);
		}

		// POST: OffreEmploisController/Delete/5
		[Authorize(Roles="Employé")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Delete(int id, OffreEmploi offreEmploi)
		{
			// Suppression au niveau des favoris en premier pour éviter les incohérences dans les favoris
            string? ip = Request.HttpContext.Connection?.RemoteIpAddress?.ToString();
#pragma warning disable CS8604 // Existence possible d'un argument de référence null.
            await _favorisService.Supprimer(ip, offreEmploi);
#pragma warning restore CS8604 // Existence possible d'un argument de référence null.
            // Jounaliser Suppression Favoris
            _logger.LogInformation(CustomLogEvents.Favoris, $"Suppression de l'offre d'emploi ID - {offreEmploi.Id} au Favoris");


			// Suppression de l'offre
			await _offreEmploisService.Supprimer(offreEmploi);
            // Journaliser la modification
            _logger.LogInformation(CustomLogEvents.OffreEmploi, $"Suppression de l'offre d'emploi - ID {offreEmploi.Id}");

			return RedirectToAction(nameof(Index));
		}
	}
}

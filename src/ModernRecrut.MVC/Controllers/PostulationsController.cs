using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ModernRecrut.MVC.Areas.Identity.Data;
using ModernRecrut.MVC.DTO;
using ModernRecrut.MVC.Interfaces;
using ModernRecrut.MVC.Models;
using System.Diagnostics.Eventing.Reader;

namespace ModernRecrut.MVC.Controllers
{
    public class PostulationsController : Controller
    {
        #region Attributs
        private readonly ILogger<PostulationsController> _logger;
        private readonly IPostulationsService _postulationsService;
        private readonly IDocumentsService _documentsService;
        private readonly IOffreEmploisService _offreEmploisService;
        #endregion

        #region Constructeur
        public PostulationsController(ILogger<PostulationsController> logger,
            IPostulationsService postulationsService,
            IDocumentsService documentsService,
            IOffreEmploisService offreEmploisService)
        {
            _logger = logger;
            _postulationsService = postulationsService;
            _documentsService = documentsService;
            _offreEmploisService = offreEmploisService;
        }
        #endregion

        #region Méthodes publiques
        // Postuler (Accessible - Candidat ou Admin)
        [Authorize(Roles = "Admin, Candidat")]
        // GET : Ajout
        public async Task<ActionResult> Postuler(int idOffreEmploi)
        {
            // Journalisation
            _logger.LogInformation($"Visite de la page postuler par l'utilisateur {User.Identity.Name}");

            OffreEmploi? offreEmploi = await _offreEmploisService.ObtenirSelonId(idOffreEmploi);
            if (offreEmploi == null )
                return NotFound();

            ViewData["OffreEmploi"] = offreEmploi;

            return View();
        }

        [Authorize(Roles = "Admin, Candidat")]
        // POST : Ajout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Postuler(RequetePostulation requetPostulation)
        {
            // Verifier que l'offre d'emploi existe et comme c'ezt une valeur passé en hidden, on retourne un NotFound si elle n'existe pas
            OffreEmploi? offreEmploi = await _offreEmploisService.ObtenirSelonId(requetPostulation.OffreDemploiId);
            if (offreEmploi == null )
                return NotFound();

            // Charger les documents pour le candidat
            IEnumerable<string>? documents = await _documentsService.ObtenirSelonUtilisateurId(requetPostulation.CandidatId);

            // Check si candidat a un CV
            bool cvPresent = documents?.Any(d => d.StartsWith($"{requetPostulation.CandidatId}_CV_")) ?? false;
            if (!cvPresent)
                ModelState.AddModelError("CV", "Un CV est obligatoire pour postuler. Veuillez déposer au préalable un CV dans votre espace Documents");

            // Check si candidat a une lettre de motivation _LettreDeMotivation_
            bool lettreMotivationPresent = documents?.Any(d => d.StartsWith($"{requetPostulation.CandidatId}_LettreDeMotivation_")) ?? false;
            if (!lettreMotivationPresent)
                ModelState.AddModelError("LettreMotivation", "Une lettre de motivation est obligatoire pour postuler. Veuillez déposer au préalable une lettre de motivation dans votre espace Documents");

            // Check date
            if (requetPostulation.DateDisponibilite <= DateTime.Today || requetPostulation.DateDisponibilite > DateTime.Today.AddDays(45))
                ModelState.AddModelError("DateDisponibilite", "La date de disponibilité doit être supérieure à la date du jour et inférieure au < date correspondante à date du jour + 45 jours >");

            // Pretention salarial inférieur à 150000
            if(requetPostulation.PretentionSalariale > 150000m)
                ModelState.AddModelError("PretentionSalariale", "Votre présentation salariale est au-delà de nos limites");

            if (ModelState.IsValid)
            {
                Postulation? postulation = await _postulationsService.Ajouter(requetPostulation);

                if (postulation == null)
                {
                    ModelState.AddModelError("AjoutEchoue", "Problème lors de l'ajout de la postulation, veuillez reessayer");
                }
                else
                {
                    return RedirectToAction("Index", "OffreEmplois");
                }
            }


            ViewData["OffreEmploi"] = offreEmploi;

            return View(requetPostulation);
        }

        // Liste Postulation (Accessible - Employé ou Admin)
        [Authorize(Roles = "Admin, Employé")]
        public async Task<ActionResult> ListePostulations()
        {
            // Journalisation
            _logger.LogInformation($"Visite de la page liste des postulation par l'utilisateur {User.Identity.Name}");

            IEnumerable<Postulation>? postulations = await _postulationsService.ObtenirTout();

            return View(postulations);
        }

        // Details
        [Authorize(Roles = "Admin, RH, Candidat")]
        public async Task<ActionResult> Details(int id)
        {
            Postulation? postulation = await _postulationsService.ObtenirSelonId(id);

            if(postulation == null)
            {
                // Journalisation - TODO
                return NotFound();
            }

            // Journalisation - TODO

            return View(postulation);
        }

        // GET : Modifier
        [Authorize(Roles = "Admin, RH, Candidat")]
        public async Task<ActionResult> Edit(int id)
        {
            Postulation? postulation = await _postulationsService.ObtenirSelonId(id);

            if(postulation == null)
                return NotFound();

            ViewData["modificationAuthorisee"] = postulation.DateDisponibilite >= DateTime.Today.AddDays(-5) && postulation.DateDisponibilite <= DateTime.Today.AddDays(5); 

            return View(postulation);
        }

        // POST : Modifier
        [Authorize(Roles = "Admin, RH, Candidat")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Postulation postulation)
        {
            // Validation
            // Verifier que l'offre d'emploi existe et comme c'ezt une valeur passé en hidden, on retourne un NotFound si elle n'existe pas
            OffreEmploi? offreEmploi = await _offreEmploisService.ObtenirSelonId(postulation.OffreDEmploiId);
            if (offreEmploi == null )
                return NotFound();

            // Check si on peut Modifier la postulation
            Postulation postulationExistante = await _postulationsService.ObtenirSelonId(postulation.Id);
            bool modificationAuthorisee = postulationExistante.DateDisponibilite >= DateTime.Today.AddDays(-5) && postulationExistante.DateDisponibilite <= DateTime.Today.AddDays(5);
            ViewData["modificationAuthorisee"] = modificationAuthorisee;

            // Charger les documents pour le candidat
            IEnumerable<string>? documents = await _documentsService.ObtenirSelonUtilisateurId(postulation.CandidatId);

            // Check si candidat a un CV
            bool cvPresent = documents?.Any(d => d.StartsWith($"{postulation.CandidatId}_CV_")) ?? false;
            if (!cvPresent)
                ModelState.AddModelError("CV", "Un CV est obligatoire pour postuler. Veuillez déposer au préalable un CV dans votre espace Documents");

            // Check si candidat a une lettre de motivation _LettreDeMotivation_
            bool lettreMotivationPresent = documents?.Any(d => d.StartsWith($"{postulation.CandidatId}_LettreDeMotivation_")) ?? false;
            if (!lettreMotivationPresent)
                ModelState.AddModelError("LettreMotivation", "Une lettre de motivation est obligatoire pour postuler. Veuillez déposer au préalable une lettre de motivation dans votre espace Documents");

            // Check date
            if (postulation.DateDisponibilite <= DateTime.Today || postulation.DateDisponibilite > DateTime.Today.AddDays(45))
                ModelState.AddModelError("DateDisponibilite", "La date de disponibilité doit être supérieure à la date du jour et inférieure au < date correspondante à date du jour + 45 jours >");

            // Pretention salarial inférieur à 150000
            if(postulation.PretentionSalariale > 150000m)
                ModelState.AddModelError("PretentionSalariale", "Votre présentation salariale est au-delà de nos limites");

            if (ModelState.IsValid && modificationAuthorisee) 
            {
                await _postulationsService.Modifier(postulation);
                // Journalisation la modification
                return RedirectToAction(nameof(ListePostulations));
            }

            ViewData["OffreEmploi"] = offreEmploi;

            return View(postulation);
        }

        // GET : Supprimer
        [Authorize(Roles = "Admin, RH, Candidat")]
        public async Task<ActionResult> Delete(int id)
        {
            Postulation postulation = await _postulationsService.ObtenirSelonId(id);

            if (postulation == null)
                return NotFound();

            ViewData["suppressionAuthorisee"] = postulation.DateDisponibilite >= DateTime.Today.AddDays(-5) && postulation.DateDisponibilite <= DateTime.Today.AddDays(5); 

            return View(postulation);
        }

        // POST : Supprimer
        [Authorize(Roles = "Admin, RH, Candidat")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, Postulation postulation)
        {
            Postulation postulationASupprimer = await _postulationsService.ObtenirSelonId(id);

            if (postulationASupprimer == null)
                return NotFound();

            bool suppressionAuthorisee = postulationASupprimer.DateDisponibilite >= DateTime.Today.AddDays(-5) && postulationASupprimer.DateDisponibilite <= DateTime.Today.AddDays(5);
            if (!suppressionAuthorisee)
            {
                ViewData["suppressionAuthorisee"] = suppressionAuthorisee;
                return View(postulationASupprimer);
            }



            // Journalisation  
            _postulationsService.Supprimer(postulationASupprimer);

            return RedirectToAction(nameof(ListePostulations));
        }

        // Notes (Accessible RH ou Admin)
        [Authorize(Roles = "Admin, RH")]
        public ActionResult Notes()
        {
            // Journalisation
            _logger.LogInformation($"Visite de la page notes() par l'utilisateur {User.Identity.Name}");

            return View();
        }
        #endregion
    }
}

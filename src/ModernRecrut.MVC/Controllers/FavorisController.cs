using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModernRecrut.MVC.DTO;
using ModernRecrut.MVC.Interfaces;
using ModernRecrut.MVC.Models;

namespace ModernRecrut.MVC.Controllers
{
    public class FavorisController : Controller
    {
        #region Attributs
        private readonly IFavorisService _favorisService;
        private readonly IOffreEmploisService _offresEmploiService;
        private readonly ILogger<FavorisController> _logger;
        #endregion

        #region Constructor
        public FavorisController(IFavorisService favorisService, IOffreEmploisService offreEmploisService, ILogger<FavorisController> logger)
        {
            _favorisService = favorisService;
            _offresEmploiService = offreEmploisService;
            _logger = logger;
        }
        #endregion

        // GET: FavorisController
        public async Task<ActionResult> Index()
        {
            string? ip = Request.HttpContext.Connection.RemoteIpAddress?.ToString();

            // Journalisation
            _logger.LogInformation($"Visite de la page des favoris par l'utilisateur {User?.Identity?.Name ?? ip}");

            if (ip != null)
            {
                IEnumerable<OffreEmploi>? offresEmploi =  await _favorisService.ObtenirSelonClef(ip);
                if(offresEmploi != null)
                {
                    // Journalisation
                    _logger.LogInformation($"L'utilisateur {User?.Identity?.Name ?? ip} a des favoris enregistrer");

                    return View(offresEmploi);
                }
            }

            return View(new List<OffreEmploi>());
        }


        // GET: FavorisController/Create
        public async Task<ActionResult> Create(int id)
        {

            string? ip = Request.HttpContext.Connection?.RemoteIpAddress?.ToString();
            string referrer = HttpContext.Request.Headers["Referer"];

            // Journalisation
            _logger.LogInformation($"Soumission d'ajout des favoris par l'utilisateur {User?.Identity?.Name ?? ip}");

            if (ip == null)
                return Redirect(referrer);

            OffreEmploi? offreEmploi = await _offresEmploiService.ObtenirSelonId(id);

            if (offreEmploi == null)
                return Redirect(referrer);

            RequeteFavori requeteFavori = new RequeteFavori()
            {
                Cle = ip,
                OffreEmploi = offreEmploi
            };
            
            await _favorisService.Ajouter(requeteFavori);
            // Jounaliser Ajout Favoris
            _logger.LogInformation(CustomLogEvents.Favoris, $"Ajout de l'offre d'emploi ID - {offreEmploi.Id} au Favoris");


            return Redirect(referrer);
        }


        // GET: FavorisController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            string? ip = Request.HttpContext.Connection?.RemoteIpAddress?.ToString();
            string referrer = HttpContext.Request.Headers["Referer"];

            if(ip == null)
                return Redirect(referrer);

            OffreEmploi? offreEmploi = await _offresEmploiService.ObtenirSelonId(id);

            if(offreEmploi == null)
                return Redirect(referrer);

            await _favorisService.Supprimer(ip, offreEmploi);
            // Jounaliser Suppression Favoris
            _logger.LogInformation(CustomLogEvents.Favoris, $"Suppression de l'offre d'emploi ID - {offreEmploi.Id} au Favoris");

            return Redirect(referrer);
        }
    }
}

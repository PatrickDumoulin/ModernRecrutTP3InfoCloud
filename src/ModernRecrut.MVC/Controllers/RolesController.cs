using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ModernRecrut.MVC.Areas.Identity.Data;
using ModernRecrut.MVC.Models;
using System.Drawing.Text;

namespace ModernRecrut.MVC.Controllers
{
    public class RolesController : Controller
    {
        #region Attributs
        private readonly ILogger<RolesController> _logger;  
        private readonly UserManager<Utilisateur> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        #endregion

        #region Constructeur
        public RolesController(
                ILogger<RolesController> logger,
                UserManager<Utilisateur> userManager,
                RoleManager<IdentityRole> roleManager
        )
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        #endregion

        [Authorize(Roles="Admin")]
		#region Public Method
		public async Task<IActionResult> Index()
        {
            // Journalisation
            _logger.LogInformation($"Visite de la page des listes des roles par l'utilisateur {User.Identity.Name}");

            // Récupérer tous les utilisateurs
            List<Utilisateur>? utilisateurs = await _userManager.Users.ToListAsync();

            // Passer la liste des utilisateurs à la vue
            ViewData["Utilisateurs"] = utilisateurs;

            return View(await _roleManager.Roles.ToListAsync());
        }

        [Authorize(Roles="Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            // Journalisation
            _logger.LogInformation($"Visite de la page des création des roles par l'utilisateur {User.Identity.Name}");

            return View();
        }

        [Authorize(Roles="Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(IdentityRole identityRole)
        {
            if (await _roleManager.Roles.AnyAsync(r => r.Name.ToLower() == identityRole.Name.ToLower()))
            {
                // Journalisation
                _logger.LogWarning($"Erreur - Role {identityRole.Name} déjà existant - lors de la création d'un rôle par l'utilisateur {User.Identity.Name}");

                ModelState.AddModelError("Name", "Le role existe déjà");
            }

            if (ModelState.IsValid)
            {
                // Journalisation
                _logger.LogInformation($"Visite de la page des création des roles par l'utilisateur {User.Identity.Name}");

                await _roleManager.CreateAsync(identityRole);
                return RedirectToAction("Index");
            }

            // Journalisation
            _logger.LogWarning($"Erreur de validation lors de la création du {identityRole?.Name} rôle par l'utilisateur {User.Identity.Name}");

            return View(identityRole);
        }

        [Authorize(Roles="Admin")]
        [HttpGet]
        public async Task<IActionResult> Assigner()
        {
            // Journalisation
            _logger.LogInformation($"Visite de la page d'assignation de roles par l'utilisateur {User.Identity.Name}");

            await RemplirSelectList();
            return View();
        }

        [Authorize(Roles="Admin")]
        [HttpPost]
        public async Task<IActionResult> Assigner(UtilisateurRoleViewModel utilisateurRoleViewModel)
        {
            if (ModelState.IsValid)
            {
                // Journalisation
                _logger.LogInformation($"Traitement d'assignation par l'utilisateur {User.Identity.Name}");

                Utilisateur utilisateur = await _userManager.FindByIdAsync(utilisateurRoleViewModel.UtilisateurId);
                bool hasRole = await _userManager.IsInRoleAsync(utilisateur, utilisateurRoleViewModel.RoleName);
                if(!hasRole) // On attribue le role seulement si l'utilisateur ne l'a pas déjà
                {
                    await _userManager.AddToRoleAsync(utilisateur, utilisateurRoleViewModel.RoleName);

                    // Journalisation
                    _logger.LogInformation($"Assignation du role {utilisateurRoleViewModel.RoleName} à l'utilisatuer {utilisateur.Id} par l'utilisateur {User.Identity.Name}");
                }

                return RedirectToAction("Index");
            }

            _logger.LogWarning($"Erreur de validation lors de l'assignation du role {utilisateurRoleViewModel?.RoleName ?? "Non défini" } à l'utilisateur {utilisateurRoleViewModel?.UtilisateurId ?? "Non défini"} par l'utilisateur {User.Identity.Name}");
            await RemplirSelectList();
            return View(utilisateurRoleViewModel);
        }
		#endregion

		#region Private Methods
        private async Task RemplirSelectList()
        {
			//# var test = await _userManager.Users ;
            ViewBag.Utilisateurs = new SelectList(await _userManager.Users.ToListAsync(), "Id", "NomComplet") ;

            ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
        }
		#endregion

	}
}

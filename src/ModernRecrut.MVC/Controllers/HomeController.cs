using Microsoft.AspNetCore.Mvc;
using ModernRecrut.MVC.Models;
using System.Diagnostics;

namespace ModernRecrut.MVC.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		public IActionResult CodeStatus(int code)
		{
			CodeStatusViewModel codeStatusViewModel = new CodeStatusViewModel();
			codeStatusViewModel.IdRequete = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
			codeStatusViewModel.CodeStatus = code;

			if(code == 404)
			{
				codeStatusViewModel.MessageErreur = "La page demandée est introuvable.";
			} else if (code == 500)
			{
				codeStatusViewModel.MessageErreur = "Plateforme en cours de maintenance.";
			} else
			{
				codeStatusViewModel.MessageErreur = "Une erreur c'est produite lors de l'exécution de la requête.";
			}

			return View(codeStatusViewModel);
		}
	}
}
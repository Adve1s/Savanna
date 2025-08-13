using Microsoft.AspNetCore.Mvc;
using Savanna.Logic;
using Savanna.WebUI.Models;
using Savanna.WebUI.Services;
using System.Diagnostics;

namespace Savanna.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WorldService _worldService;

        public HomeController(ILogger<HomeController> logger, WorldService worldService) 
        {
            _logger = logger;
            _worldService = worldService;
        }

        public IActionResult Index()
        {
            return View(_worldService.CurrentWorld);
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

        /// <summary>
        /// Handles world creation
        /// </summary>
        [HttpPost]
        public IActionResult CreateWorld()
        {
            _worldService.CreateNewWorld(10,10);
            return RedirectToAction("Index");
        }


        /// <summary>
        /// Handles world loading from existing save
        /// </summary>
        [HttpPost]
        public IActionResult LoadWorld()
        {
            //TODO: Create Loading instead of creating.
            _worldService.CreateNewWorld(5,5);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Handles world deletion
        /// </summary>
        [HttpPost]
        public IActionResult ClearWorld()
        {
            _worldService?.ClearWorld();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Handles animal addition
        /// </summary>
        [HttpPost]
        public IActionResult AddAnimal(char animalKey)
        {
            _worldService?.CurrentWorld.AddAnimal(animalKey);
            return RedirectToAction("Index");
        }
    }
}

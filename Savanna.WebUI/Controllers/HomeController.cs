using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Savanna.Logic;
using Savanna.WebUI.Models;
using Savanna.WebUI.Services;
using System.Diagnostics;
using System.Security.Claims;

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

        [Authorize]
        public IActionResult Index()
        {
            _worldService.ClearWorld();
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult Game()
        {
            return View(_worldService.CurrentWorld.WorldToDisplayDTO());
        }

        [Authorize]
        public IActionResult SaveList()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var saves = _worldService.GetUserSaves(userId);
            return View(saves);
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
            _worldService.CreateNewWorld(12, 20);
            return RedirectToAction("Game");
        }

        /// <summary>
        /// Handles world loading from existing save
        /// </summary>
        [Authorize]
        [HttpPost]
        public IActionResult LoadWorld(int saveId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _worldService.LoadWorld(saveId, userId);
            return RedirectToAction("Game");
        }

        /// <summary>
        /// Deletes save
        /// </summary>
        [Authorize]
        [HttpPost]
        public IActionResult DeleteSave(int saveId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _worldService.DeleteSave(saveId, userId);
            return RedirectToAction("SaveList");
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
        /// Handles world saving
        /// </summary>
        [Authorize]
        [HttpPost]
        public IActionResult SaveWorld(string saveName)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _worldService.SaveWorld(userId, saveName);
            return RedirectToAction("Index");
        }
    }
}

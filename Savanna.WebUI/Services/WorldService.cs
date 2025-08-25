using Savanna.Logic;
using Savanna.WebUI.Data;
using Savanna.WebUI.Models;
using System.Text.Json;

namespace Savanna.WebUI.Services
{
    /// <summary>
    /// Represents world with its functions
    /// </summary>
    public class WorldService
    {
        /// <summary>
        /// Gets the current world, if doesn't exits, then null
        /// </summary>
        public World? CurrentWorld { get; private set; }
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Creates instance of WorldService
        /// </summary>
        /// <param name="context">Db context</param>
        public WorldService(IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Create new world
        /// </summary>
        /// <param name="height">Height of new world</param>
        /// <param name="width">Width of new world</param>
        public void CreateNewWorld(int height = 10, int width = 30)
        {
            CurrentWorld = new World(height, width);
        }

        /// <summary>
        /// Load world from save
        /// </summary>
        /// <param name="saveId">primary key of save</param>
        /// <param name="userId">user id</param>
        public void LoadWorld(int saveId, string userId)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var worldSave = context.WorldSaves.FirstOrDefault(x =>  x.Id == saveId && x.UserId == userId);

            if (worldSave != null) {
                var worldSaveDTO = JsonSerializer.Deserialize<WorldSaveDTO>(worldSave.SaveData);
                CurrentWorld = new World();
                CurrentWorld.SetUpWorldFromSaveDTO(worldSaveDTO);
            }
        }

        /// <summary>
        /// Clear existing world
        /// </summary>
        public void ClearWorld()
        {
            CurrentWorld = null;
        }

        /// <summary>
        /// Gets if world exists
        /// </summary>
        public bool HasWorld => CurrentWorld != null;

        /// <summary>
        /// Saves current world to database
        /// </summary>
        /// <param name="userId">User id of person who saved</param>
        /// <param name="saveName">Name of the save</param>
        public void SaveWorld(string userId, string saveName)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var worldDTO = CurrentWorld.WorldToSaveDTO();
            var save = new WorldSave
            {
                UserId = userId,
                SaveName = string.IsNullOrWhiteSpace(saveName) ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : saveName,
                SaveTime = DateTime.Now,
                Iteration = worldDTO.Iteration,
                AnimalCount = worldDTO.AnimalsInWorld,
                SaveData = JsonSerializer.Serialize(worldDTO)
            };
            context.WorldSaves.Add(save);
            context.SaveChanges();
        }

        /// <summary>
        /// Gets saves of user from database
        /// </summary>
        /// <param name="userId">User id of save owner</param>
        /// <returns>List of saves</returns>
        public IEnumerable<SaveListViewModel> GetUserSaves(string userId)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return context.WorldSaves
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.SaveTime)
                .Select(x => new SaveListViewModel
                {
                    Id = x.Id,
                    SaveName = x.SaveName,
                    Iteration = x.Iteration,
                    AnimalCount = x.AnimalCount,
                })
                .ToList();
        }

        /// <summary>
        /// Deletes selected game from database
        /// </summary>
        /// <param name="saveId">primary key of the game to delete</param>
        /// <param name="userId">user that is deleting game</param>
        public void DeleteSave(int saveId, string userId)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var worldSave = context.WorldSaves.FirstOrDefault(x =>  x.Id == saveId && x.UserId == userId);

            if (worldSave != null) {
                context.WorldSaves.Remove(worldSave);
                context.SaveChanges();
            }
        }
    }
}

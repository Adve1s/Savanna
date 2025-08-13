using Microsoft.AspNetCore.SignalR;
using Savanna.WebUI.Services;

namespace Savanna.WebUI.Hubs
{
    /// <summary>
    /// SignalR hub for real-time game communication between server and client
    /// </summary>
    public class GameHub : Hub
    {
        public const string HUB_URL = "/gameHub";
        public const string GAME_UPDATE_MESSAGE = "GameUpdate";
        
        private WorldService _worldService;

        public GameHub(WorldService worldService)
        { 
            _worldService = worldService;
        }

        public async Task AddAnimal(char creationKey)
        {
            _worldService.CurrentWorld.AddAnimal(creationKey);
        }
    }
}

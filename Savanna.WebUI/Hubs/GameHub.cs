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
        public const string ADD_ANIMAL_MESSAGE = "AddAnimal";
        public const string HANDLE_CELL_CLICK_MESSAGE = "HandleCellClick";
        public const string HANDLE_CELL_UNCLICK_MESSAGE = "HandleCellUnclick";
        public const string GET_START_STATE_MESSAGE = "GetStartState";
        public const string TOGGLE_PAUSE_MESSAGE = "TogglePause";

        private WorldService _worldService;
        private GameLoopService _gameLoopService;

        public GameHub(WorldService worldService, GameLoopService gameLoopService)
        { 
            _worldService = worldService;
            _gameLoopService = gameLoopService;
        }

        public async Task AddAnimal(char creationKey)
        {
            if(_worldService.HasWorld) _worldService.CurrentWorld.AddAnimal(creationKey);
            await _gameLoopService.SendGameUpdate();
        }

        public async Task HandleCellUnclick()
        {
            _gameLoopService.ChangeHighLightedAnimal();
            await _gameLoopService.SendGameUpdate();
        }
        public async Task HandleCellClick(int row, int column)
        {
            _gameLoopService.ChangeHighLightedAnimal(row, column);
            await _gameLoopService.SendGameUpdate();
        }

        public Task TogglePause()
        {
            _gameLoopService.TogglePause();
            return Task.CompletedTask;
        }

        public async Task GetStartState()
        {
            _gameLoopService.ChangeHighLightedAnimal();
            await _gameLoopService.SendGameUpdate();
        }
    }
}

using Microsoft.AspNetCore.SignalR;
using Savanna.Logic;
using Savanna.WebUI.Hubs;

namespace Savanna.WebUI.Services
{
    /// <summary>
    /// Background service that runs the game loop advancing simulation
    /// </summary>
    public class GameLoopService : IHostedService, IDisposable
    {
        private const int UPDATE_INTERVAL_MS = 500;
        private readonly WorldService _worldService;
        private readonly IHubContext<GameHub> _hubContext;
        private Timer? _timer;
        private AnimalCardInfoDTO? _highlightedAnimal;
        private bool _isPaused = true;

        /// <summary>
        /// Creates GameLoopService instant
        /// </summary>
        /// <param name="worldService"> World service that contains world</param>
        /// <param name="hubContext"> Hub used for information sending</param>
        public GameLoopService(WorldService worldService, IHubContext<GameHub> hubContext)
        {
            _worldService = worldService;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Starts game loop timer when app starts
        /// </summary>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(NextGameLoop, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(UPDATE_INTERVAL_MS));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Progress the world and send that data to web
        /// </summary>
        private async void NextGameLoop(object? state)
        {
            if (_worldService.HasWorld && !_isPaused)
            {
                _worldService.CurrentWorld.NextTurn();
                await SendGameUpdate();
            }
        }

        /// <summary>
        /// Sends updated board state
        /// </summary>
        internal async Task SendGameUpdate()
        {
            if (_worldService.HasWorld)
            {
                var data = SerializeData();
                await _hubContext.Clients.All.SendAsync(GameHub.GAME_UPDATE_MESSAGE, data);
            }
        }

        /// <summary>
        /// Toggles pause
        /// </summary>
        internal void TogglePause()
        {
            _isPaused = !_isPaused;
        }

        /// <summary>
        /// Serialize data to send to web
        /// </summary>
        /// <returns>Data needed for info displaying.</returns>
        private object SerializeData()
        {
            var world = _worldService.CurrentWorld.WorldToDisplayDTO();

            int? highlightedRow = null;
            int? highlightedColumn = null;

            if(_highlightedAnimal != null)
                (highlightedRow,highlightedColumn) = _worldService.CurrentWorld.GetAnimalPositionByID(_highlightedAnimal.ID);
            if (highlightedRow != null && highlightedColumn != null)
            {
                _highlightedAnimal = _worldService.CurrentWorld.GetAnimalCardDTOByPosition((int)highlightedRow, (int)highlightedColumn);
            } else _highlightedAnimal = null;

            return new { field = world.AnimalField, iteration = world.Iteration, animalCount = world.AnimalsInWorld, highlightRow = highlightedRow, highlightColumn = highlightedColumn, highlightAnimal = _highlightedAnimal , test = _worldService.CurrentWorld};
        }

        /// <summary>
        /// Updates highlighted animal if it exists
        /// </summary>
        /// <returns>Bool value representing if animal exists</returns>
        private bool UpdateHighlightedAnimal()
        {
            if (_highlightedAnimal != null)
            {
                _worldService.CurrentWorld.GetAnimalPositionByID(_highlightedAnimal.ID);
            }
            return false;
        }
        
        /// <summary>
        /// Updates the highlighted animal.
        /// </summary>
        /// <param name="row">Row where new update is clicked</param>
        /// <param name="column">Column where new update is clicked</param>
        internal Task ChangeHighLightedAnimal(int? row = null, int? column = null)
        {
            if (_worldService.HasWorld && row != null && column != null) _highlightedAnimal = _worldService.CurrentWorld.GetAnimalCardDTOByPosition((int)row,(int)column);
            else _highlightedAnimal = null;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Stops game loop timer when app shuts down
        /// </summary>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Clear memory
        /// </summary>
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}

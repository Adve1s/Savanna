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
        private Animal? _highlightedAnimal;

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
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(UPDATE_INTERVAL_MS));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Progress the world and send that data to web
        /// </summary>
        private async void DoWork(object? state)
        {
            if (_worldService.HasWorld)
            {
                _worldService.NextTurn();
                var data = SerializeData();
                await _hubContext.Clients.All.SendAsync(GameHub.GAME_UPDATE_MESSAGE, data);
            }
        }

        /// <summary>
        /// Serialize data to send to web
        /// </summary>
        /// <returns>Data needed for info displaying.</returns>
        private object SerializeData()
        {
            var field = _worldService.CurrentWorld.GetField();

            int? highlightedRow = null;
            int? highlightedColumn = null;
            var returnField = new object?[_worldService.CurrentWorld.Height][];

            for (int row = 0; row < _worldService.CurrentWorld.Height; row++)
            {
                returnField[row] = new object?[_worldService.CurrentWorld.Width];
                for (int column = 0; column < _worldService.CurrentWorld.Width; column++)
                {
                    var animal = field[row, column];
                    if (animal == null) returnField[row][column] = null;
                    else returnField[row][column] = new {displayChar = animal.DisplayChar, isAlive = animal.IsAlive()};
                    if (_highlightedAnimal != null && _highlightedAnimal == field[row, column])
                    {
                        highlightedRow = row;
                        highlightedColumn = column;
                    }
                }

            }
            return new { field = returnField, highlightRow = highlightedRow, highlightColumn = highlightedColumn };
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

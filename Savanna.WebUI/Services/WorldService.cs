using Savanna.Logic;

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
        /// Load existing world as current world
        /// </summary>
        /// <param name="world">World to load</param>
        public void LoadWorld(World world)
        {
            CurrentWorld = world;
        }

        /// <summary>
        /// Clear existing world
        /// </summary>
        public void ClearWorld()
        {
            CurrentWorld = null;
        }

        /// <summary>
        /// Progresses current world to next iteration
        /// </summary>
        public void NextTurn()
        {
            CurrentWorld.NextTurn();
        }

        /// <summary>
        /// Gets if world exists
        /// </summary>
        public bool HasWorld => CurrentWorld != null;
    }
}

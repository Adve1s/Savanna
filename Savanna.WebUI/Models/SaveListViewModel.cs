namespace Savanna.WebUI.Models
{
    /// <summary>
    /// Represents info needed for save list.
    /// </summary>
    public class SaveListViewModel
    {
        /// <summary>
        /// Gets save id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets save name
        /// </summary>
        public string SaveName { get; set; }
        
        /// <summary>
        /// Gets iteration of game
        /// </summary>
        public int Iteration { get; set; }

        /// <summary>
        /// Gets animal count in the save
        /// </summary>
        public int AnimalCount { get; set; }
    }
}

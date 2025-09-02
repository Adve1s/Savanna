namespace Savanna.WebUI.Models
{
    /// <summary>
    /// Represents database table
    /// </summary>
    public class WorldSave
    {
        /// <summary>
        /// Primary-key
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Foreign- key to identify user
        /// </summary>
        public string UserId { get; set; }

        public string SaveName { get; set; }
        public DateTime SaveTime { get; set; }
        public int Iteration { get; set; }
        public int AnimalCount { get; set; }
        public string SaveData { get; set; }
    }
}

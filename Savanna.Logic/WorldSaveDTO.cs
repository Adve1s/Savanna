using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Savanna.Logic
{
    /// <summary>
    /// Represents World needed info for saving
    /// </summary>
    public class WorldSaveDTO
    {
        public AnimalSaveDTO?[][] Field { get; set; }

        /// <summary>
        /// Gets the field width
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets the field height
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets the worlds iteration
        /// </summary>
        public int Iteration { get; set; }

        /// <summary>
        /// Gets the world Animal count in world
        /// </summary>
        public int AnimalsInWorld { get; set; }
    }
}

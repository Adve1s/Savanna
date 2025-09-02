using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Savanna.Logic
{
    public class WorldDisplayDTO
    {
        /// <summary>
        /// Gets the current field representation
        /// </summary>
        public AnimalDisplayInfo?[][]? AnimalField { get; set; }

        /// <summary>
        /// Gets Available animals
        /// </summary>
        public (string Name,char CreationKey)[] AnimalsAvailable { get; set; }
        
        /// <summary>
        /// Gets current game iteration
        /// </summary>
        public int Iteration {  get; set; }

        /// <summary>
        /// Gets current animals in world count
        /// </summary>
        public int AnimalsInWorld {  get; set; }

        /// <summary>
        /// Gets the field height
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets the field width
        /// </summary>
        public int Width { get; set; }

    }
}

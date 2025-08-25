using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Savanna.Logic
{
    /// <summary>
    /// Class represents animal info needed for displays
    /// </summary>
    public class AnimalDisplayInfo
    {
        /// <summary>
        /// Represents Animal in char mode
        /// </summary>
        public char DisplayChar { get; set; }

        /// <summary>
        /// Represents Animal in emoji mode
        /// </summary>
        public string DisplayEmoji { get; set; }

        /// <summary>
        /// Reprsents if animal is alive
        /// </summary>
        public bool IsAlive { get; set; }
    }
}

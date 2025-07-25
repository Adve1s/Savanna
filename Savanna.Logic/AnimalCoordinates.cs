using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Savanna.Logic
{
    /// <summary>
    /// Represents animal position
    /// </summary>
    internal class AnimalCoordinates
    {
        /// <summary>
        /// Get and change the animal of position
        /// </summary>
        public Animal Animal { get; set; }

        /// <summary>
        /// Get and change row of this position
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// Get and change column of this position
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// Initializes new instance of AnimalCoordinates
        /// </summary>
        /// <param name="animal">Animal in the position</param>
        /// <param name="row">Row of position</param>
        /// <param name="column">Column of position</param>
        public AnimalCoordinates(Animal animal, int row, int column)
        {
            Row = row;
            Column = column;
            Animal = animal;
        }
    }
}

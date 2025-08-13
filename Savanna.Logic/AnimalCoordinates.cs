using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Savanna.Logic
{
    /// <summary>
    /// Represents animal position
    /// </summary>
    public struct AnimalCoordinates
    {
        /// <summary>
        /// Get and change the animal of position
        /// </summary>
        public Animal? Animal { get; set; }

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
        /// <param name="row">Row of position</param>
        /// <param name="column">Column of position</param>
        /// <param name="animal">Animal in the position</param>
        public AnimalCoordinates(int row, int column, Animal? animal = null)
        {
            Row = row;
            Column = column;
            Animal = animal;
        }

        /// <summary>
        /// Update position
        /// </summary>
        /// <param name="row">row of position</param>
        /// <param name="column">column of position</param>
        public void Update(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }
}

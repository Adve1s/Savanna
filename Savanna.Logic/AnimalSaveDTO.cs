using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Savanna.Logic
{
    /// <summary>
    /// Represents animal needed info for saving
    /// </summary>
    public class AnimalSaveDTO
    {
        /// <summary>
        /// Gets animal id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets if animal is alive
        /// </summary>
        public bool IsAlive { get; set; }

        /// <summary>
        /// Gets how many rounds animal is dead
        /// </summary>
        public int RoundsDead { get; set; }

        /// <summary>
        /// Gets animal offspring count
        /// </summary>
        public int Offsprings { get; set; }

        /// <summary>
        /// Gets animal age
        /// </summary>
        public double Age { get; set; }

        /// <summary>
        /// Gets animal current pause between children
        /// </summary>
        public double CurrentChildrenPause { get; set; }

        /// <summary>
        /// Gets animal possible mate list
        /// </summary>
        public Dictionary<int, int> PossibleMates { get; set; }

        /// <summary>
        /// Gets animal stamina
        /// </summary>
        public double Stamina { get; set; }

        /// <summary>
        /// Gets Animal Health
        /// </summary>
        public double Health { get; set; }

        /// <summary>
        /// Gets Animal vision range
        /// </summary>
        public int Vision { get; set; }

        /// <summary>
        /// Gets Animal Speed
        /// </summary>
        public int Speed { get; set; }

        /// <summary>
        /// Gets Animal Endurance
        /// </summary>
        public int Endurance { get; set; }

        /// <summary>
        /// Gets Animal Defence
        /// </summary>
        public int Defence { get; set; }

        /// <summary>
        /// Gets Animal creation key
        /// </summary>
        public char CreationKey { get; set; }
    }
}

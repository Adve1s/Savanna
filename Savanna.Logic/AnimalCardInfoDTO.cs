using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Savanna.Logic
{
    /// <summary>
    /// Represents animal needed info for data card display.
    /// </summary>
    public class AnimalCardInfoDTO
    {
        /// <summary>
        /// Gets Animals Name
        /// </summary>
        public string Name {  get; set; }

        /// <summary>
        /// Gets Animal id
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets Animals display emoji
        /// </summary>
        public string DisplayEmoji {  get; set; }

        /// <summary>
        /// Gets Animals Health
        /// </summary>
        public double Health {  get; set; }

        /// <summary>
        /// Gets Animals MaxHealth
        /// </summary>
        public double MaxHealth {  get; set; }

        /// <summary>
        /// Gets Animals Stamina
        /// </summary>
        public double Stamina {  get; set; }

        /// <summary>
        /// Gets Animals MaxStamina
        /// </summary>
        public double MaxStamina {  get; set; }

        /// <summary>
        /// Gets Animals Age
        /// </summary>
        public double Age {  get; set; }

        /// <summary>
        /// Gets Animals Offsprijgs
        /// </summary>
        public int Offsprings {  get; set; }

        /// <summary>
        /// Gets if animal is alive
        /// </summary>
        public bool IsAlive {  get; set; }

        /// <summary>
        /// Gets if animal is decomposed
        /// </summary>
        public bool IsDecomposed {  get; set; }
    }
}

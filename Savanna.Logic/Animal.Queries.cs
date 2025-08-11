using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Savanna.Logic
{
    public abstract partial class Animal
    {
        /// <summary>
        /// Creates a list of visible animals of selected type
        /// </summary>
        /// <param name="surroundings">Visible area</param>
        /// <returns>List of Animal coordinates</returns>
        protected List<AnimalCoordinates> GetAnimalByName(Animal[,] surroundings, string name)
        {
            var typeCoordinates = new List<AnimalCoordinates>();
            for (int i = 0; i < surroundings.GetLength(0); i++)
            {
                for (int j = 0; j < surroundings.GetLength(1); j++)
                {
                    if (surroundings[i, j] != null && surroundings[i, j].Name.Equals(name) && surroundings[i, j].IsAlive()) typeCoordinates.Add(new AnimalCoordinates(i, j, surroundings[i, j]));
                }
            }
            return typeCoordinates;
        }

        /// <summary>
        /// Gets only the mates that are close enough
        /// </summary>
        /// <param name="visibleMates">All mates within visible area</param>
        /// <param name="self">OwnPosition</param>
        /// <returns>List of available mates</returns>
        protected List<AnimalCoordinates> FilterCloseEnoughMates(List<AnimalCoordinates> visibleMates, AnimalCoordinates self)
            => visibleMates.Where(animal => DistanceToCalculator(self, animal) <= self.Animal.ReproductionRange).ToList();

        /// <summary>
        /// Calculates distance from one field to other
        /// </summary>
        /// <param name="from">From coordinates</param>
        /// <param name="to">To Coordinates</param>
        /// <returns>Number of moves between fields</returns>
        public double DistanceToCalculator(AnimalCoordinates from, AnimalCoordinates to)
        {
            return Math.Max(Math.Abs(from.Row - to.Row), Math.Abs(from.Column - to.Column));
        }

        /// <summary>
        /// Checks if animal is alive
        /// </summary>
        /// <returns>Bool value as an answer</returns>
        public bool IsAlive() => _isAlive;

        /// <summary>
        /// Checks if animal is decomposed
        /// </summary>
        /// <returns>Bool value as an answer</returns>
        public bool IsDecomposed() => _roundsDead >= RoundsToDecompose;

        /// <summary>
        /// Check if animal has enough stamina to do action
        /// </summary>
        /// <param name="stamina">Current stamina</param>
        /// <param name="staminaChange">Stamina change</param>
        /// <returns>Bool value answering if have enough stamina</returns>
        protected bool HaveEnoughStamina(double stamina, double staminaChange) => stamina + staminaChange >= 0;

        /// <summary>
        /// Checks if gain would put value above max
        /// </summary>
        /// <param name="maxValue">Max value to check agains</param>
        /// <param name="currentValue">Current value to add to</param>
        /// <param name="gain">Gain to add to current value</param>
        /// <returns>Bool value answering if stat is above max</returns>
        protected bool IsStatAboveMax(double maxValue, double currentValue, double gain) => currentValue + gain > maxValue;
    }
}

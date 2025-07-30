using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Savanna.Logic
{
    /// <summary>
    /// Represents a single animal in Savanna,
    /// manages Animal decition making.
    /// </summary>
    internal abstract class Animal
    {
        /// <summary>
        /// Animal settings used
        /// </summary>
        public abstract string DisplaySymbol { get; }
        public abstract char CreationKey { get; }
        protected abstract int DefaultSpeed { get; }
        protected abstract int DefaultVision { get; }
        protected abstract int DefaultMaxStamina { get; }
        protected abstract double DefaultMaxHealth { get; }
        protected abstract int StaminaAddition { get; }
        protected abstract double HealthDeduction { get; }
        protected abstract (int StaminaChange, int Weight) RestInfo {  get; }
        protected abstract (int StaminaChange, int Weight) SleepInfo {  get; }
        protected abstract (int StaminaChange, int Weight) MoveInfo {  get; }

        /// <summary>
        /// Gets animal current stamina
        /// </summary>
        public int Stamina { get; protected set; }

        /// <summary>
        /// Gets animal current max stamina
        /// </summary>
        public int MaxStamina { get; protected set; }

        /// <summary>
        /// Gets Animal current Health
        /// </summary>
        public double Health { get; protected set; }

        /// <summary>
        /// Gets Animal current max Health
        /// </summary>
        public double MaxHealth { get; protected set; }

        /// <summary>
        /// Gets Animal vision range
        /// </summary>
        public int Vision { get; protected set; }

        /// <summary>
        /// Gets Animal Speed
        /// </summary>
        public int Speed { get; protected set; }

        /// <summary>
        /// Decides what animal wants to do
        /// </summary>
        /// <param name="world">World in which animal lives</param>
        /// <param name="surroundings">Surroundings within vision range</param>
        /// <param name="selfLocaly">Own position within vision range</param>
        /// <param name="selfGlobaly">Own position within world</param>
        public abstract void DoAction(World world, Animal[,] surroundings, AnimalCoordinates selfLocaly, AnimalCoordinates selfGlobaly);

        /// <summary>
        /// Resets stamina to default max
        /// </summary>
        public void AddPerRoundStamina()
        {
            Stamina += StaminaAddition;
        }

        /// <summary>
        /// Animal decides to do nothing
        /// </summary>
        protected void Rest()
        {
            Stamina += RestInfo.StaminaChange;
        }

        /// <summary>
        /// Animal decides to sleep
        /// </summary>
        /// <returns> Sellp action</returns>
        protected void Sleep()
        {
            Stamina += SleepInfo.StaminaChange;
        }

        /// <summary>
        /// Animal spends stamina to move to a direction
        /// </summary>
        /// <param name="world">World where animal is</param>
        /// <param name="self">Own position in world</param>
        /// <param name="direction">Direction where animal wants to move</param>
        protected void Move(World world, AnimalCoordinates self, Direction direction)
        {
            if (self.Animal.Stamina + MoveInfo.StaminaChange >=0)
            {
                Stamina += MoveInfo.StaminaChange;
                world.MoveAnimal(self, direction);
            }
            else
            {
                Rest();
            }
        }

        /// <summary>
        /// Makes animal specific direction choice
        /// </summary>
        /// <param name="self">Own position within vision range</param>
        /// <param name="surroundings">Surroundings within vision range</param>
        /// <param name="enemies">Positions of enemies within vision range</param>
        /// <param name="allies">Positions of allies within vision range</param>
        /// <returns>Direction where animal chose to go</returns>
        protected abstract Direction? DecideMoveDirection(AnimalCoordinates self, Animal[,] surroundings, List<AnimalCoordinates>? enemies = null, List<AnimalCoordinates>? allies = null);
    
        /// <summary>
        /// Animal chooses random action to do
        /// </summary>
        /// <param name="choices">List of actions to choose from</param>
        /// <returns>The action to do.</returns>
        protected Action ChooseRandomWeightedAction(List<(Action action, int weight)> choices)
        {
            var random = new Random();
            int totalWeight = choices.Sum(choice => choice.weight);
            int randomValue = random.Next(totalWeight);
            int cumulativeWeight = 0;
            foreach (var choice in choices)
            {
                cumulativeWeight += choice.weight;
                if (randomValue < cumulativeWeight) return choice.action;
            }
            return choices.Last().action;
        }
    }
}

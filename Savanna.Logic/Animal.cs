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
        /// Constants
        /// </summary>
        protected const double DEFAULT_MAX_STAMINA = 25;
        protected const double DEFAULT_MAX_HEALTH = 25;
        protected const double REST_STAMINA_RECOVERY = 2.5;
        protected const double DEFAULT_ACTION_STAMINA_COST = 25;
        protected const double SLEEP_STAMINA_RECOVERY_PRECENTAGE = 0.75;

        /// <summary>
        /// Animal settings used
        /// </summary>
        public abstract string DisplaySymbol { get; }
        public abstract char CreationKey { get; }
        protected abstract int DefaultSpeed { get; }
        protected abstract int DefaultVision { get; }
        protected abstract int DefaultEndurance { get; }
        protected abstract int DefaultDefence { get; }


        protected abstract double MaxStamina { get; }
        protected abstract double MaxHealth { get; }


        protected abstract double PerRoundHealthDeduction { get; }
        protected abstract int RoundsToDecompose { get; }
        protected abstract double TiredStaminaThreshold { get; }


        protected abstract (double StaminaChange, int Weight) RestInfo {  get; }
        protected abstract (double StaminaChange, int Weight) SleepInfo {  get; }
        protected abstract (double StaminaChange, int Weight) MoveInfo {  get; }

        private bool _isAlive = true;
        private int _roundsDead = 0;

        /// <summary>
        /// Gets animal current stamina
        /// </summary>
        public double Stamina { get; protected set; }

        /// <summary>
        /// Gets animal current max stamina
        /// </summary>
        //public double MaxStamina { get; protected set; }

        /// <summary>
        /// Gets Animal current Health
        /// </summary>
        public double Health { get; protected set; }

        /// <summary>
        /// Gets Animal current max Health
        /// </summary>
        //public double MaxHealth { get; protected set; }

        /// <summary>
        /// Gets Animal vision range
        /// </summary>
        public int Vision { get; protected set; }

        /// <summary>
        /// Gets Animal Speed
        /// </summary>
        public int Speed { get; protected set; }

        /// <summary>
        /// Gets Animal Endurance
        /// </summary>
        public int Endurance { get; protected set; }

        /// <summary>
        /// Gets Animal Defence
        /// </summary>
        public int Defence { get; protected set; }

        /// <summary>
        /// Decides what animal wants to do
        /// </summary>
        /// <param name="world">World in which animal lives</param>
        /// <param name="surroundings">Surroundings within vision range</param>
        /// <param name="selfLocaly">Own position within vision range</param>
        /// <param name="selfGlobaly">Own position within world</param>
        public abstract void DoAction(World world, Animal[,] surroundings, AnimalCoordinates selfLocaly, AnimalCoordinates selfGlobaly);

        /// <summary>
        /// Per round Animal changes.
        /// </summary>
        public void PerRoundStatusChanges()
        {
            if (_isAlive)
            {
                Health -= PerRoundHealthDeduction;
                if (Health <= 0) Death();
            }
            else
            {
                _roundsDead++;
            }
        }

        /// <summary>
        /// Checks if animal is alive
        /// </summary>
        /// <returns>Bool value as an answer</returns>
        public bool IsAlive()
        {
            return _isAlive;
        }

        /// <summary>
        /// Checks if animal is decomposed
        /// </summary>
        /// <returns>Bool value as an answer</returns>
        public bool IsDecomposed()
        {
            return _roundsDead>= RoundsToDecompose;
        }

        /// <summary>
        /// Animal decides to do nothing
        /// </summary>
        protected void Rest()
        {
            if (IsStatAboveMax(MaxStamina, Stamina, RestInfo.StaminaChange))
            {
                Stamina = MaxStamina;
            }
            else
            {
                Stamina += RestInfo.StaminaChange;
            }
        }

        /// <summary>
        /// Animal decides to sleep
        /// </summary>
        protected void Sleep()
        {
            if (IsStatAboveMax(MaxStamina,Stamina,SleepInfo.StaminaChange))
            {
                Stamina = MaxStamina;
            }
            else
            {
                Stamina += SleepInfo.StaminaChange;
            }
        }

        /// <summary>
        /// Animal dies
        /// </summary>
        private void Death()
        {
            _isAlive = false;
        }

        /// <summary>
        /// Animal gets damaged
        /// </summary>
        /// <param name="damageDone">Damage ammount</param>
        public void Damage(double damageDone)
        {
            Health -= damageDone;
            if (Health <= 0) Death();
        }

        /// <summary>
        /// Animal gets healed
        /// </summary>
        /// <param name="damageHealed">Heal ammount</param>
        protected void Heal(double damageHealed)
        {
            if (IsStatAboveMax(MaxHealth, Health, damageHealed))
            {
                Health = MaxHealth;
            }
            else
            {
                Health += damageHealed;
            }
        }

        /// <summary>
        /// Animal spends stamina to move to a direction
        /// </summary>
        /// <param name="world">World where animal is</param>
        /// <param name="self">Own position in world</param>
        /// <param name="direction">Direction where animal wants to move</param>
        protected void Move(World world, AnimalCoordinates self, Direction direction)
        {
            if (HaveEnoughStamina(self.Animal.Stamina, MoveInfo.StaminaChange))
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
        /// Check if animal has enough stamina to do action
        /// </summary>
        /// <param name="stamina">Current stamina</param>
        /// <param name="staminaChange">Stamina change</param>
        /// <returns>Bool value answering if have enough stamina</returns>
        protected bool HaveEnoughStamina(double stamina, double staminaChange)
        {
            return stamina + staminaChange >= 0;
        }

        /// <summary>
        /// Checks if gain would put value above max
        /// </summary>
        /// <param name="maxValue">Max value to check agains</param>
        /// <param name="currentValue">Current value to add to</param>
        /// <param name="gain">Gain to add to current value</param>
        /// <returns>Bool value answering if stat is above max</returns>
        protected bool IsStatAboveMax(double maxValue, double currentValue, double gain)
        {
            return currentValue + gain > maxValue;
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

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Savanna.Logic
{
    /// <summary>
    /// Represents a single animal in Savanna,
    /// manages Animal decition making.
    /// </summary>
    internal abstract partial class Animal
    {
        // Constants
        protected const double DEFAULT_MAX_STAMINA = 25;
        protected const double DEFAULT_MAX_HEALTH = 25;
        protected const double REST_STAMINA_RECOVERY = 2.5;
        protected const double DEFAULT_ACTION_STAMINA_COST = 25;
        protected const double SLEEP_STAMINA_RECOVERY_PRECENTAGE = 0.75;
        internal const int ROUNDS_TO_REPRODUCE = 3;
        internal const double TIME_PER_ROUND = 0.01;

        // Animal settings used
        public abstract string DisplaySymbol { get; }
        public abstract char CreationKey { get; }
        protected abstract int DefaultSpeed { get; }
        protected abstract int DefaultVision { get; }
        protected abstract int DefaultEndurance { get; }
        protected abstract int DefaultDefence { get; }
        
        protected abstract double MaxStamina { get; }
        protected abstract double MaxHealth { get; }

        internal abstract int RoundsToDecompose { get; }
        protected abstract double PerRoundHealthDeduction { get; }
        protected abstract double TiredStaminaThreshold { get; }
        public abstract int ReproductionRange { get; }
        internal abstract double MaxAgeLimit { get; }
        internal abstract double ChildrenBearingAge { get; }
        internal abstract double ChildrenPauseTime { get; }
        protected abstract double HungryThreshold { get; }

        protected abstract (double StaminaChange, int Weight) RestInfo {  get; }
        protected abstract (double StaminaChange, int Weight) SleepInfo {  get; }
        protected abstract (double StaminaChange, int Weight) MoveInfo {  get; }

        internal bool _isAlive = true;
        internal int _roundsDead = 0;
        internal double _age = 0;
        internal double _currentChildrenPause = 0;
        internal Dictionary<Animal,int> possibleMates = new Dictionary<Animal,int>();

        /// <summary>
        /// Gets animal current stamina
        /// </summary>
        public double Stamina { get; internal set; }

        /// <summary>
        /// Gets Animal current Health
        /// </summary>
        public double Health { get; protected set; }

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
        /// Does per turn animal actions
        /// </summary>
        /// <param name="world">World in which animal lives</param>
        /// <param name="selfGlobaly">Own position within world</param>
        public void Turn(World world, AnimalCoordinates selfGlobaly)
        {
            PerRoundUpdate();
            if (selfGlobaly.Animal.IsAlive())
            {
                (Animal[,] visibleArea, AnimalCoordinates selfLocaly) = world.GetVisibleArea(selfGlobaly.Row, selfGlobaly.Column);
                Mating(world,visibleArea, selfLocaly,selfGlobaly);
                DoAction(world,visibleArea,selfLocaly,selfGlobaly);
            }
        }

        /// <summary>
        /// Decides what animal wants to do
        /// </summary>
        /// <param name="world">World in which animal lives</param>
        /// <param name="surroundings">Surroundings within vision range</param>
        /// <param name="selfLocaly">Own position within vision range</param>
        /// <param name="selfGlobaly">Own position within world</param>
        internal abstract void DoAction(World world, Animal[,] surroundings, AnimalCoordinates selfLocaly, AnimalCoordinates selfGlobaly);

        /// <summary>
        /// Per round Animal updates.
        /// </summary>
        internal void PerRoundUpdate()
        {
            if (_isAlive)
            {
                _currentChildrenPause += TIME_PER_ROUND;
                _age += TIME_PER_ROUND;
                Health -= PerRoundHealthDeduction;
                if (Health <= 0 || _age > MaxAgeLimit) Die();
            }
            else _roundsDead++;
        }

        /// <summary>
        /// Animal decides to do nothing
        /// </summary>
        internal void Rest()
        {
            if (IsStatAboveMax(MaxStamina, Stamina, RestInfo.StaminaChange)) Stamina = MaxStamina;
            else Stamina += RestInfo.StaminaChange;
        }

        /// <summary>
        /// Animal decides to sleep
        /// </summary>
        internal void Sleep()
        {
            if (IsStatAboveMax(MaxStamina,Stamina,SleepInfo.StaminaChange)) Stamina = MaxStamina;
            else Stamina += SleepInfo.StaminaChange;
        }

        /// <summary>
        /// Animal dies
        /// </summary>
        private void Die() => _isAlive = false;

        /// <summary>
        /// Animal gets damaged
        /// </summary>
        /// <param name="damageDone">Damage ammount</param>
        internal void Damage(double damageDone)
        {
            Health -= damageDone;
            if (Health <= 0) Die();
        }

        /// <summary>
        /// Animal gets healed
        /// </summary>
        /// <param name="damageHealed">Heal ammount</param>
        internal void Heal(double damageHealed)
        {
            if (IsStatAboveMax(MaxHealth, Health, damageHealed)) Health = MaxHealth;
            else Health += damageHealed;
        }

        /// <summary>
        /// Animal spends stamina to move to a direction
        /// </summary>
        /// <param name="world">World where animal is</param>
        /// <param name="self">Own position in world</param>
        /// <param name="direction">Direction where animal wants to move</param>
        internal void Move(World world, AnimalCoordinates self, Direction? direction)
        {
            if (HaveEnoughStamina(self.Animal.Stamina, MoveInfo.StaminaChange) && direction != null)
            {
                Stamina += MoveInfo.StaminaChange;
                world.MoveAnimal(self, (Direction)direction);
            }
            else Rest();
        }

        /// <summary>
        /// Animal mates
        /// </summary>
        /// <param name="world">Where new animal will be added</param>
        /// <param name="self">Parent</param>
        private void Mate(World world, AnimalCoordinates self)
        {
            var direction = Movement.RandomDirection(Movement.GetValidDirections(world.GetField(), self));
            if (direction != null) world.AddAnimal(world.AnimalFactory.CreateAnimal(CreationKey),
                new AnimalCoordinates(self.Row + Movement.Directions[(Direction)direction].row, self.Column + Movement.Directions[(Direction)direction].column));
            possibleMates.Clear();
            _currentChildrenPause = 0;
        }

        /// <summary>
        /// Makes animal specific direction choice
        /// </summary>
        /// <param name="self">Own position within vision range</param>
        /// <param name="surroundings">Surroundings within vision range</param>
        /// <param name="enemies">Positions of enemies within vision range</param>
        /// <param name="allies">Positions of allies within vision range</param>
        /// <returns>Direction where animal chose to go</returns>
        internal abstract Direction? DecideMoveDirection(AnimalCoordinates self, Animal[,] surroundings, List<AnimalCoordinates>? enemies = null, List<AnimalCoordinates>? allies = null);
    
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

        /// <summary>
        /// Checks available mates and tries to mate
        /// </summary>
        /// <param name="world">Animals world</param>
        /// <param name="visibleArea">Area animal sees</param>
        /// <param name="selfLocaly">Self local coordinates</param>
        /// <param name="selfGlobaly">Self global coordinates</param>
        internal void Mating(World world, Animal?[,] visibleArea, AnimalCoordinates selfLocaly, AnimalCoordinates selfGlobaly)
        {
            if (_age < ChildrenBearingAge || _currentChildrenPause < ChildrenPauseTime) return;
            visibleArea[selfLocaly.Row, selfLocaly.Column] = null;
            var mates = GetMatesVisibleList(visibleArea, GetType());
            mates = FilterCloseEnoughMates(mates, selfLocaly);
            var closeMates = mates.Select(animal => animal.Animal).ToHashSet();
            foreach (var animal in closeMates)
            {
                possibleMates[animal] = possibleMates.GetValueOrDefault(animal, 0) + 1;
                if (possibleMates[animal] >= ROUNDS_TO_REPRODUCE && animal.possibleMates.ContainsKey(this) && animal.possibleMates[this] >= ROUNDS_TO_REPRODUCE)
                {
                    Mate(world,selfGlobaly);
                    return;
                }
            }
            foreach (var key in possibleMates.Keys.ToList())
            {
                if (!closeMates.Contains(key)) possibleMates.Remove(key);
            }
        }
    }
}

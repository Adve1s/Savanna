using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static Savanna.Logic.Lion;

namespace Savanna.Logic
{
    /// <summary>
    /// Represents a specific type of <see cref="Animal"/> - Antelope,
    /// manages Antelope specific decision making.
    /// </summary>
    internal class Antelope : Animal
    {
        /// <summary>
        /// Antelope settings as constants
        /// </summary>
        private const string ANTELOPE_DISPLAY_SYMBOL = "A";
        private const char ANTELOPE_CREATION_KEY = 'A';
        private const int ANTELOPE_DEFAULT_SPEED = 3;
        private const int ANTELOPE_DEFAULT_VISION = 4;
        private const int ANTELOPE_DEFAULT_MAX_STAMINA = 100;
        private const int ANTELOPE_STAMINA_ADDITION = 25;
        private const Double REST_RECOVERY_PRECENTAGE = 0.1;
        private const Double SLEEP_RECOVERY_PRECENTAGE = 0.5;
        private const Double EAT_GRASS_SPENDING_PRECENTAGE = 0.05;
        private const int REST_POSIBILITY_WEIGHT = 20;
        private const int SLEEP_POSIBILITY_WEIGHT = 10;
        private const int MOVE_POSIBILITY_WEIGHT = 60;
        private const int EAT_GRASS_POSIBILITY_WEIGHT = 20;

        /// <summary>
        /// Animal settings used
        /// </summary>
        public override string DisplaySymbol => ANTELOPE_DISPLAY_SYMBOL;
        public override char CreationKey => ANTELOPE_CREATION_KEY;
        protected override int DefaultSpeed => ANTELOPE_DEFAULT_SPEED;
        protected override int DefaultVision => ANTELOPE_DEFAULT_VISION;
        protected override int DefaultMaxStamina => ANTELOPE_DEFAULT_MAX_STAMINA;
        protected override int StaminaAddition => ANTELOPE_STAMINA_ADDITION;
        protected override (int StaminaChange, int Weight) RestInfo => ((int)(Stamina*REST_RECOVERY_PRECENTAGE),REST_POSIBILITY_WEIGHT);
        protected override (int StaminaChange, int Weight) SleepInfo => ((int)(Stamina * SLEEP_RECOVERY_PRECENTAGE), SLEEP_POSIBILITY_WEIGHT);
        protected override (int StaminaChange, int Weight) MoveInfo => (-DefaultMaxStamina / Speed, MOVE_POSIBILITY_WEIGHT);
        protected (int StaminaChange, int Weight) EatGrassInfo => ((int)(-DefaultMaxStamina * EAT_GRASS_SPENDING_PRECENTAGE), EAT_GRASS_POSIBILITY_WEIGHT);

        /// <summary>
        /// Initializes new instance of Antelope
        /// </summary>
        public Antelope() 
        {
            Vision = DefaultVision;
            Speed = DefaultSpeed;
            Stamina = DefaultMaxStamina;
        }

        /// <summary>
        /// Decides what Antelope wants to do
        /// </summary>
        /// <param name="world">World in which animal lives</param>
        /// <param name="surroundings">Surroundings within vision range</param>
        /// <param name="selfLocal">Own position within vision range</param>
        /// <param name="selfGlobal">Own position within world</param>
        public override void DoAction(World world,Animal[,] surroundings, AnimalCoordinates selfLocal, AnimalCoordinates selfGlobal)
        {
            surroundings[selfLocal.Row, selfLocal.Column] = null;
            var lionPositions = World.GetTypePositionsList<Lion>(surroundings);
            var actions = new List<(Action, int)>();
            bool canMove = selfLocal.Animal.Stamina + MoveInfo.StaminaChange >= 0;
            if (!canMove)
            {
                actions.Add((Sleep, SleepInfo.Weight));
                actions.Add((Rest, RestInfo.Weight));
            }
            else if (lionPositions.Count() > 0)
            {
                var direction = DecideMoveDirection(selfLocal,surroundings, lionPositions);
                if (direction != null) actions.Add(( () => Move(world,selfGlobal,(Direction)direction), MoveInfo.Weight));
                else actions.Add((Rest,RestInfo.Weight));
            }
            else
            {
                var direction = DecideMoveDirection(selfLocal, surroundings);
                if (direction != null) actions.Add((() => Move(world, selfGlobal, (Direction)direction), MoveInfo.Weight));
                actions.Add((Rest, RestInfo.Weight));
                actions.Add((Sleep, SleepInfo.Weight));
                actions.Add((EatGrass, EatGrassInfo.Weight));
            }
            ChooseRandomWeightedAction(actions)();
        }

        /// <summary>
        /// Makes animal specific direction choice
        /// </summary>
        /// <param name="self">Own position within vision range</param>
        /// <param name="surroundings">Surroundings within vision range</param>
        /// <param name="enemies">Positions of enemies within vision range</param>
        /// <param name="allies">Positions of allies within vision range</param>
        /// <returns>Direction where animal chose to go</returns>
        protected override Direction? DecideMoveDirection(AnimalCoordinates self, Animal[,] surroundings, List<AnimalCoordinates>? enemies = null, List<AnimalCoordinates>? allies = null)
        {
            var directions = Movement.GetValidDirections(surroundings, self);
            if (directions.Count() == 0) return null;
            if (enemies != null)
            {
                var enemy = GetClosestLion(self, enemies);
                directions = GetFurthestDirectionFromLion(directions, self, enemy);
            }
            return Movement.RandomDirection(directions);
        }

        /// <summary>
        /// Gets closest lion position
        /// </summary>
        /// <param name="self">own position</param>
        /// <param name="lions">all visible lion positions</param>
        /// <returns>AnimalCoordinates of closest lion.</returns>
        private AnimalCoordinates GetClosestLion(AnimalCoordinates self, List<AnimalCoordinates> lions)
        {
            var random = new Random();
            double closestEnemies = lions
                .Min(enemy => World.DistanceToCalculator(self, enemy));
            List<AnimalCoordinates> enemies = lions
                .Where(enemy => World.DistanceToCalculator(self, enemy) == closestEnemies)
                .ToList();
            var enemy = enemies[random.Next(enemies.Count)];
            return enemy;
        }

        /// <summary>
        /// Gets list of best directions for the animal based on lion position
        /// </summary>
        /// <param name="directions">All allowed directions</param>
        /// <param name="self">Own position</param>
        /// <param name="lion">Lion position</param>
        /// <returns>List of all equally good directions.</returns>
        private List<Direction> GetFurthestDirectionFromLion(List<Direction> directions, AnimalCoordinates self, AnimalCoordinates lion)
        {
            var directionWithDistanceToEnemy = directions.Select(direction => new
            {
                direction = direction,
                distance = World.DistanceToCalculator(
                    new AnimalCoordinates(self.Animal, self.Row + Movement.Directions[direction].row, self.Column + Movement.Directions[direction].column), lion)
            }).ToList();

            Double afterMoveDistance = 0;
            afterMoveDistance = directionWithDistanceToEnemy.Max(value => value.distance);

            var returnDirection = directionWithDistanceToEnemy
                    .Where(value => value.distance == afterMoveDistance)
                    .Select(value => value.direction)
                    .ToList();

            return returnDirection;
        }


        /// <summary>
        /// Antelope decides to stop to eat
        /// </summary>
        /// <returns>Antelope eat action</returns>
        protected void EatGrass()
        {
            Stamina += EatGrassInfo.StaminaChange;
        }
    }
}

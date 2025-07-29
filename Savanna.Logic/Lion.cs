using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Savanna.Logic
{
    /// <summary>
    /// Represents a specific type of <see cref="Animal"/> - Lion,
    /// manages Lion specific decision making.
    /// </summary>
    internal class Lion : Animal
    {
        /// <summary>
        /// Lion settings as constants
        /// </summary>
        private const string LION_DISPLAY_SYMBOL = "L";
        private const char LION_CREATION_KEY = 'L';
        private const int LION_DEFAULT_SPEED = 2;
        private const int LION_DEFAULT_VISION = 5;
        private const int LION_DEFAULT_MAX_STAMINA = 100;
        private const int LION_STAMINA_ADDITION = 25;
        private const Double REST_RECOVERY_PRECENTAGE = 0.1;
        private const Double SLEEP_RECOVERY_PRECENTAGE = 0.5;
        private const Double ROAR_SPENDING_PRECENTAGE = 0.1;
        private const int REST_POSIBILITY_WEIGHT = 20;
        private const int SLEEP_POSIBILITY_WEIGHT = 10;
        private const int MOVE_POSIBILITY_WEIGHT = 60;
        private const int ROAR_POSIBILITY_WEIGHT = 10;

        /// <summary>
        /// Animal settings used
        /// </summary>
        public override string DisplaySymbol => LION_DISPLAY_SYMBOL;
        public override char CreationKey => LION_CREATION_KEY;
        protected override int DefaultSpeed => LION_DEFAULT_SPEED;
        protected override int DefaultVision => LION_DEFAULT_VISION;
        protected override int DefaultMaxStamina => LION_DEFAULT_MAX_STAMINA;
        protected override int StaminaAddition => LION_STAMINA_ADDITION;
        protected override (int StaminaChange, int Weight) RestInfo => ((int)(Stamina * REST_RECOVERY_PRECENTAGE), REST_POSIBILITY_WEIGHT);
        protected override (int StaminaChange, int Weight) SleepInfo => ((int)(Stamina * SLEEP_RECOVERY_PRECENTAGE), SLEEP_POSIBILITY_WEIGHT);
        protected override (int StaminaChange, int Weight) MoveInfo => (-DefaultMaxStamina / Speed, MOVE_POSIBILITY_WEIGHT);
        protected (int StaminaChange, int Weight) RoarInfo => ((int)(-DefaultMaxStamina * ROAR_SPENDING_PRECENTAGE), ROAR_POSIBILITY_WEIGHT);

        /// <summary>
        /// Initializes new instance of Lion
        /// </summary>
        public Lion()
        {
            Vision = DefaultVision;
            Speed = DefaultSpeed;
            Stamina = DefaultMaxStamina;
        }

        /// <summary>
        /// Decides what Lion wants to do
        /// </summary>
        /// <param name="world">World in which animal lives</param>
        /// <param name="surroundings">Surroundings within vision range</param>
        /// <param name="selfLocal">Own position within vision range</param>
        /// <param name="selfGlobal">Own position within world</param>
        public override void DoAction(World world, Animal[,] surroundings, AnimalCoordinates selfLocal, AnimalCoordinates selfGlobal)
        {
            surroundings[selfLocal.Row, selfLocal.Column] = null;
            var antelopePositions = World.GetTypePositionsList<Antelope>(surroundings);
            var actions = new List<(Action, int)>();
            bool canMove = selfLocal.Animal.Stamina + MoveInfo.StaminaChange >= 0;
            if (!canMove)
            {
                actions.Add((Sleep, SleepInfo.Weight));
            }
            else if (antelopePositions.Count() > 0)
            {
                var direction = DecideMoveDirection(selfLocal, surroundings, antelopePositions);
                if (direction != null) actions.Add((() => Move(world, selfGlobal, (Direction)direction), MoveInfo.Weight));
                else actions.Add((Rest, RestInfo.Weight));
            }
            else
            {
                var direction = DecideMoveDirection(selfLocal, surroundings);
                if (direction != null) actions.Add((() => Move(world, selfGlobal, (Direction)direction), MoveInfo.Weight));
                actions.Add((Rest, RestInfo.Weight));
                actions.Add((Sleep, SleepInfo.Weight));
                actions.Add((Roar, RoarInfo.Weight));
            }
            ChooseRandomWeightedAction(actions)();
        }

        /// <summary>
        /// Makes animal specific direction choice
        /// </summary>
        /// <param name="self">Own position within vision range</param>
        /// <param name="surroundings">Surroundings within vision range</param>
        /// <param name="lions">Positions of lions within vision range</param>
        /// <param name="allies">Positions of allies within vision range</param>
        /// <returns>Direction where animal chose to go</returns>
        protected override Direction? DecideMoveDirection(AnimalCoordinates self, Animal[,] surroundings, List<AnimalCoordinates>? lions = null, List<AnimalCoordinates>? allies = null)
        {
            var directions = Movement.GetValidDirections(surroundings, self);
            if (directions.Count() == 0) return null;
            if (lions != null)
            {
                var lion = GetClosestAntelope(self, lions);
                directions = GetClosestDirectionToAntelope(directions, self, lion);
            }
            return Movement.RandomDirection(directions);
        }


        /// <summary>
        /// Gets closest antelope position
        /// </summary>
        /// <param name="self">own position</param>
        /// <param name="antelopes">all visible antelope positions</param>
        /// <returns>AnimalCoordinates of closest antelope.</returns>
        private AnimalCoordinates GetClosestAntelope(AnimalCoordinates self, List<AnimalCoordinates> antelopes)
        {
            var random = new Random();
            double closestEnemies = antelopes
                .Min(enemy => World.DistanceToCalculator(self, enemy));
            List<AnimalCoordinates> enemies = antelopes
                .Where(enemy => World.DistanceToCalculator(self, enemy) == closestEnemies)
                .ToList();
            var enemy = enemies[random.Next(enemies.Count)];
            return enemy;
        }

        /// <summary>
        /// Gets list of best directions for the animal based on antelope position
        /// </summary>
        /// <param name="directions">All allowed directions</param>
        /// <param name="self">Own position</param>
        /// <param name="antelope">Antelope position</param>
        /// <returns>List of all equally good directions.</returns>
        private List<Direction> GetClosestDirectionToAntelope(List<Direction> directions, AnimalCoordinates self, AnimalCoordinates antelope)
        {
            var directionWithDistanceToEnemy = directions.Select(direction => new
            {
                direction = direction,
                distance = World.DistanceToCalculator(
                    new AnimalCoordinates(self.Animal, self.Row + Movement.Directions[direction].row, self.Column + Movement.Directions[direction].column), antelope)
            }).ToList();

            Double afterMoveDistance = 0;
            afterMoveDistance = directionWithDistanceToEnemy.Min(value => value.distance);

            var returnDirection = directionWithDistanceToEnemy
                    .Where(value => value.distance == afterMoveDistance)
                    .Select(value => value.direction)
                    .ToList();

            return returnDirection;
        }

        /// <summary>
        /// Lion decides to roar
        /// </summary>
        /// <returns>Roar action</returns>
        public void Roar()
        {
            Stamina += RoarInfo.StaminaChange;
        }

    }
}

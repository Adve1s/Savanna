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
        private const int LION_DEFAULT_SPEED = 8;
        private const int LION_DEFAULT_VISION = 4;
        private const int LION_DEFAULT_ENDURANCE = 2;
        private const int LION_DEFAULT_DEFENCE = 1;

        private const int LION_ROUNDS_TO_DECOMPOSE = 15;
        private const double LION_HEALTH_DEDUCTION = 0.5;
        private const double LION_TIRED_PRECENTAGE = 0.7;

        private const int REST_POSIBILITY_WEIGHT = 20;
        private const int SLEEP_POSIBILITY_WEIGHT = 5;
        private const int MOVE_POSIBILITY_WEIGHT = 60;
        private const int ROAR_POSIBILITY_WEIGHT = 15;

        private const double ROAR_ACTION_COST_MULTIPLIER = 0.4;
        private const double ATTACK_ACTION_COST_MULTIPLIER = 0.5;
        private const double ATTACK_DAMAGE_MULTIPLIER = 0.8;
        private const double ATTACK_HEALTH_GAIN_PRECENTAGE = 0.5;


        /// <summary>
        /// Animal settings used
        /// </summary>
        public override string DisplaySymbol => LION_DISPLAY_SYMBOL;
        public override char CreationKey => LION_CREATION_KEY;
        protected override int DefaultSpeed => LION_DEFAULT_SPEED;
        protected override int DefaultVision => LION_DEFAULT_VISION;
        protected override int DefaultEndurance => LION_DEFAULT_ENDURANCE;
        protected override int DefaultDefence => LION_DEFAULT_DEFENCE;


        protected override double MaxStamina => DEFAULT_MAX_STAMINA * Speed;
        protected override double MaxHealth => DEFAULT_MAX_HEALTH * Defence;

        protected override int RoundsToDecompose => LION_ROUNDS_TO_DECOMPOSE;
        protected override double PerRoundHealthDeduction => LION_HEALTH_DEDUCTION;
        protected override double TiredStaminaThreshold => MaxStamina * LION_TIRED_PRECENTAGE;


        protected override (double StaminaChange, int Weight) RestInfo => (REST_STAMINA_RECOVERY * Endurance, REST_POSIBILITY_WEIGHT);
        protected override (double StaminaChange, int Weight) SleepInfo => (MaxStamina * SLEEP_STAMINA_RECOVERY_PRECENTAGE, SLEEP_POSIBILITY_WEIGHT);
        protected override (double StaminaChange, int Weight) MoveInfo => (-DEFAULT_ACTION_STAMINA_COST, MOVE_POSIBILITY_WEIGHT);
        protected (double StaminaChange, int Weight) RoarInfo => (-DEFAULT_ACTION_STAMINA_COST * ROAR_ACTION_COST_MULTIPLIER, ROAR_POSIBILITY_WEIGHT);
        protected (double StaminaChange, double Damage) AttackInfo => (-DEFAULT_ACTION_STAMINA_COST * ATTACK_ACTION_COST_MULTIPLIER, DEFAULT_MAX_HEALTH*ATTACK_DAMAGE_MULTIPLIER);


        /// <summary>
        /// Initializes new instance of Lion
        /// </summary>
        public Lion()
        {
            Speed = DefaultSpeed;
            Vision = DefaultVision;
            Endurance = DefaultEndurance;
            Defence = DefaultDefence;
            Health = MaxHealth;
            Stamina = MaxStamina;
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
            if (antelopePositions.Count() > 0)
            {
                var antelope = GetClosestAntelope(selfLocal, antelopePositions);
                if(World.DistanceToCalculator(selfLocal,antelope) == 1)
                {
                    Attack(antelope);
                }

                var direction = DecideMoveDirection(selfLocal, surroundings, antelopePositions);
                if (direction != null) actions.Add((() => Move(world, selfGlobal, (Direction)direction), MoveInfo.Weight));
                else actions.Add((Rest, RestInfo.Weight));
            }
            else if (Stamina < TiredStaminaThreshold)
            {
                actions.Add((Sleep, SleepInfo.Weight));
            }
            else
            {
                if (HaveEnoughStamina(selfLocal.Animal.Stamina, MoveInfo.StaminaChange))
                {
                    var direction = DecideMoveDirection(selfLocal, surroundings);
                    if (direction != null) actions.Add((() => Move(world, selfGlobal, (Direction)direction), MoveInfo.Weight));
                }
                if (HaveEnoughStamina(selfLocal.Animal.Stamina, RoarInfo.StaminaChange))
                {
                    actions.Add((Roar, RoarInfo.Weight));
                }
                actions.Add((Rest, RestInfo.Weight));
                actions.Add((Sleep, SleepInfo.Weight));
            }
            ChooseRandomWeightedAction(actions)();
        }

        /// <summary>
        /// Makes animal specific direction choice
        /// </summary>
        /// <param name="self">Own position within vision range</param>
        /// <param name="surroundings">Surroundings within vision range</param>
        /// <param name="antelopes">Positions of antelopes within vision range</param>
        /// <param name="allies">Positions of allies within vision range</param>
        /// <returns>Direction where animal chose to go</returns>
        protected override Direction? DecideMoveDirection(AnimalCoordinates self, Animal[,] surroundings, List<AnimalCoordinates>? antelopes = null, List<AnimalCoordinates>? allies = null)
        {
            var directions = Movement.GetValidDirections(surroundings, self);
            if (directions.Count() == 0) return null;
            if (antelopes != null)
            {
                var antelope = GetClosestAntelope(self, antelopes);
                directions = GetClosestDirectionToAntelope(directions, self, antelope);
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
                direction,
                distance = World.DistanceToCalculator(
                    new AnimalCoordinates(self.Animal, self.Row + Movement.Directions[direction].row, self.Column + Movement.Directions[direction].column), antelope)
            }).ToList();

            double afterMoveDistance = 0;
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
        private void Roar()
        {
            if (HaveEnoughStamina(Stamina, RoarInfo.StaminaChange))
            {
                Stamina += RoarInfo.StaminaChange;
            }
            else
            {
                Rest();
            }
        }

        /// <summary>
        /// Lion attacks
        /// </summary>
        /// <param name="antelope">AnimalCoordinates where to attack</param>
        private void Attack(AnimalCoordinates antelope)
        {
            if (HaveEnoughStamina(Stamina, AttackInfo.StaminaChange))
            {
                Stamina += AttackInfo.StaminaChange;
                antelope.Animal.Damage(AttackInfo.Damage);
                if (!antelope.Animal.IsAlive()) Heal(MaxHealth * ATTACK_HEALTH_GAIN_PRECENTAGE);
            }
            else
            {
                Rest();
            }

        }

    }
}

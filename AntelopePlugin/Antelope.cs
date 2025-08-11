using Savanna.Logic;

namespace AntelopePlugin
{
    /// <summary>
    /// Represents a specific type of <see cref="Animal"/> - Antelope,
    /// manages Antelope specific decision making.
    /// </summary>
    public class Antelope : Animal
    {
        // Antelope settings as constants
        private const string ANTELOPE_NAME = "Antelope";
        private const string ANTELOPE_DISPLAY_SYMBOL = "A";
        private const char ANTELOPE_CREATION_KEY = 'A';
        private const int ANTELOPE_DEFAULT_SPEED = 3;
        private const int ANTELOPE_DEFAULT_VISION = 5;
        private const int ANTELOPE_DEFAULT_ENDURANCE = 8;
        private const int ANTELOPE_DEFAULT_DEFENCE = 2;

        private const int ANTELOPE_ROUNDS_TO_DECOMPOSE = 10;
        private const double ANTELOPE_HEALTH_DEDUCTION = 0.5;
        private const double ANTELOPE_TIRED_PRECENTAGE = 0.4;
        private const int ANTELOPE_REPRODUCTION_RANGE = 2;
        private const double ANTELOPE_MAX_AGE = 18;
        private const double ANTELOPE_CHILDREN_BEARING_AGE = 2;
        private const double ANTELOPE_CHILDREN_PAUSE_TIME = 1.5;
        private const double ANTELOPE_HUNGRY_PRECENTAGE = 0.1;

        private const int REST_POSIBILITY_WEIGHT = 25;
        private const int SLEEP_POSIBILITY_WEIGHT = 5;
        private const int MOVE_POSIBILITY_WEIGHT = 35;
        private const int EAT_GRASS_POSIBILITY_WEIGHT = 35;

        private const double EAT_GRASS_ACTION_COST_MULTIPLIER = 0.2;
        private const double EAT_GRASS_HEALTH_GAIN_PRECENTAGE = 0.1;

        private const string LION_NAME = "Lion";

        // Animal settings used
        public override string Name => ANTELOPE_NAME;
        public override string DisplaySymbol => ANTELOPE_DISPLAY_SYMBOL;
        public override char CreationKey => ANTELOPE_CREATION_KEY;
        protected override int DefaultSpeed => ANTELOPE_DEFAULT_SPEED;
        protected override int DefaultVision => ANTELOPE_DEFAULT_VISION;
        protected override int DefaultEndurance => ANTELOPE_DEFAULT_ENDURANCE;
        protected override int DefaultDefence => ANTELOPE_DEFAULT_DEFENCE;

        protected override double MaxStamina => DEFAULT_MAX_STAMINA * Speed;
        protected override double MaxHealth => DEFAULT_MAX_HEALTH * Defence;

        protected override int RoundsToDecompose => ANTELOPE_ROUNDS_TO_DECOMPOSE;
        protected override double PerRoundHealthDeduction => ANTELOPE_HEALTH_DEDUCTION;
        protected override double TiredStaminaThreshold => MaxStamina * ANTELOPE_TIRED_PRECENTAGE;
        protected override int ReproductionRange => ANTELOPE_REPRODUCTION_RANGE;
        protected override double MaxAgeLimit => ANTELOPE_MAX_AGE;
        protected override double ChildrenBearingAge => ANTELOPE_CHILDREN_BEARING_AGE;
        protected override double ChildrenPauseTime => ANTELOPE_CHILDREN_PAUSE_TIME;
        protected override double HungryThreshold => MaxHealth * ANTELOPE_HUNGRY_PRECENTAGE;

        protected override (double StaminaChange, int Weight) RestInfo => (REST_STAMINA_RECOVERY * Endurance, REST_POSIBILITY_WEIGHT);
        protected override (double StaminaChange, int Weight) SleepInfo => (MaxStamina * SLEEP_STAMINA_RECOVERY_PRECENTAGE, SLEEP_POSIBILITY_WEIGHT);
        protected override (double StaminaChange, int Weight) MoveInfo => (-DEFAULT_ACTION_STAMINA_COST, MOVE_POSIBILITY_WEIGHT);
        protected (double StaminaChange, int Weight, double Healing) EatGrassInfo => (-DEFAULT_ACTION_STAMINA_COST * EAT_GRASS_ACTION_COST_MULTIPLIER, EAT_GRASS_POSIBILITY_WEIGHT, MaxHealth * EAT_GRASS_HEALTH_GAIN_PRECENTAGE);

        /// <summary>
        /// Initializes new instance of Antelope
        /// </summary>
        public Antelope()
        {
            Speed = DefaultSpeed;
            Vision = DefaultVision;
            Endurance = DefaultEndurance;
            Defence = DefaultDefence;
            Health = MaxHealth;
            Stamina = MaxStamina;
        }

        /// <summary>
        /// Decides what Antelope wants to do
        /// </summary>
        /// <param name="world">World in which animal lives</param>
        /// <param name="surroundings">Surroundings within vision range</param>
        /// <param name="selfLocal">Own position within vision range</param>
        /// <param name="selfGlobal">Own position within world</param>
        protected override void DoAction(World world, Animal[,] surroundings, AnimalCoordinates selfLocal, AnimalCoordinates selfGlobal)
        {
            surroundings[selfLocal.Row, selfLocal.Column] = null;
            var lionPositions = GetAnimalByName(surroundings, LION_NAME);
            var actions = new List<(Action, int)>();
            if (lionPositions.Count() > 0)
            {
                var direction = DecideMoveDirection(selfLocal, surroundings, lionPositions);
                actions.Add((() => Move(world, selfGlobal, direction), MoveInfo.Weight));
            }
            else if (Health < HungryThreshold)
            {
                actions.Add((EatGrass, EatGrassInfo.Weight));
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
                    actions.Add((() => Move(world, selfGlobal, direction), MoveInfo.Weight));
                }
                if (HaveEnoughStamina(selfLocal.Animal.Stamina, EatGrassInfo.StaminaChange))
                {
                    actions.Add((EatGrass, EatGrassInfo.Weight));
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
        /// <param name="enemies">Positions of enemies within vision range</param>
        /// <param name="allies">Positions of allies within vision range</param>
        /// <returns>Direction where animal chose to go</returns>
        protected override Direction? DecideMoveDirection(AnimalCoordinates self, Animal[,] surroundings, List<AnimalCoordinates>? enemies = null, List<AnimalCoordinates>? allies = null)
        {
            var directions = Movement.GetValidDirections(surroundings, self);
            if (directions.Count() == 0) return null;
            if (enemies != null && enemies.Count != 0)
            {
                directions = GetFurthestDirectionFromLion(directions, self, GetClosestLion(self, enemies));
            }
            return Movement.RandomDirection(directions);
        }

        /// <summary>
        /// Gets closest lion position
        /// </summary>
        /// <param name="self">own position</param>
        /// <param name="lions">all visible lion positions</param>
        /// <returns>AnimalCoordinates of closest lion.</returns>
        protected AnimalCoordinates GetClosestLion(AnimalCoordinates self, List<AnimalCoordinates> lions)
        {
            var random = new Random();
            double closestEnemies = lions
                .Min(enemy => DistanceToCalculator(self, enemy));
            List<AnimalCoordinates> enemies = lions
                .Where(enemy => DistanceToCalculator(self, enemy) == closestEnemies)
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
        protected List<Direction> GetFurthestDirectionFromLion(List<Direction> directions, AnimalCoordinates self, AnimalCoordinates lion)
        {
            var directionWithDistanceToEnemy = directions.Select(direction => new
            {
                direction,
                distance = DistanceToCalculator(
                    new AnimalCoordinates(self.Row + Movement.Directions[direction].row, self.Column + Movement.Directions[direction].column, self.Animal), lion)
            }).ToList();
            double afterMoveDistance = directionWithDistanceToEnemy.Max(value => value.distance);
            var returnDirection = directionWithDistanceToEnemy
                    .Where(value => value.distance == afterMoveDistance)
                    .Select(value => value.direction)
                    .ToList();

            return returnDirection;
        }


        /// <summary>
        /// Antelope decides to stop to eat
        /// </summary>
        protected void EatGrass()
        {
            if (HaveEnoughStamina(Stamina, EatGrassInfo.StaminaChange))
            {
                Stamina += EatGrassInfo.StaminaChange;
                Heal(EatGrassInfo.Healing);
            }
            else Rest();
        }
    }
}

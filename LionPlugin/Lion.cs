using Savanna.Logic;

namespace LionPlugin
{
    /// <summary>
    /// Represents a specific type of <see cref="Animal"/> - Lion,
    /// manages Lion specific decision making.
    /// </summary>
    public class Lion : Animal
    {
        // Lion settings as constants
        private const string LION_NAME = "Lion";
        private const string LION_DISPLAY_SYMBOL = "L";
        private const char LION_CREATION_KEY = 'L';
        private const int LION_DEFAULT_SPEED = 8;
        private const int LION_DEFAULT_VISION = 4;
        private const int LION_DEFAULT_ENDURANCE = 2;
        private const int LION_DEFAULT_DEFENCE = 4;

        private const int LION_ROUNDS_TO_DECOMPOSE = 15;
        private const double LION_HEALTH_DEDUCTION = 0.5;
        private const double LION_TIRED_PRECENTAGE = 0.7;
        private const int LION_REPRODUCTION_RANGE = 2;
        private const double LION_MAX_AGE = 15;
        private const double LION_CHILDREN_BEARING_AGE = 3;
        private const double LION_CHILDREN_PAUSE_TIME = 2.5;
        private const double LION_HUNGRY_PRECENTAGE = 0.5;

        private const int REST_POSIBILITY_WEIGHT = 30;
        private const int SLEEP_POSIBILITY_WEIGHT = 6;
        private const int MOVE_POSIBILITY_WEIGHT = 54;
        private const int ROAR_POSIBILITY_WEIGHT = 10;

        private const double ROAR_ACTION_COST_MULTIPLIER = 0.4;
        private const double ATTACK_ACTION_COST_MULTIPLIER = 0.2;
        private const double ATTACK_DAMAGE_MULTIPLIER = 0.8;
        private const double ATTACK_KILL_HEALTH_GAIN_PRECENTAGE = 0.5;
        private const double SMELL_ACTION_COST_MULTIPLIER = 0.4;

        private const string ANTELOPE_NAME = "Antelope";

        // Animal settings used
        public override string Name => LION_NAME;
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
        protected override int ReproductionRange => LION_REPRODUCTION_RANGE;
        protected override double MaxAgeLimit => LION_MAX_AGE;
        protected override double ChildrenBearingAge => LION_CHILDREN_BEARING_AGE;
        protected override double ChildrenPauseTime => LION_CHILDREN_PAUSE_TIME;
        protected override double HungryThreshold => MaxHealth * LION_HUNGRY_PRECENTAGE;

        protected override (double StaminaChange, int Weight) RestInfo => (REST_STAMINA_RECOVERY * Endurance, REST_POSIBILITY_WEIGHT);
        protected override (double StaminaChange, int Weight) SleepInfo => (MaxStamina * SLEEP_STAMINA_RECOVERY_PRECENTAGE, SLEEP_POSIBILITY_WEIGHT);
        protected override (double StaminaChange, int Weight) MoveInfo => (-DEFAULT_ACTION_STAMINA_COST, MOVE_POSIBILITY_WEIGHT);
        protected (double StaminaChange, int Weight) RoarInfo => (-DEFAULT_ACTION_STAMINA_COST * ROAR_ACTION_COST_MULTIPLIER, ROAR_POSIBILITY_WEIGHT);
        protected (double StaminaChange, double Damage) AttackInfo => (-DEFAULT_ACTION_STAMINA_COST * ATTACK_ACTION_COST_MULTIPLIER, DEFAULT_MAX_HEALTH * ATTACK_DAMAGE_MULTIPLIER);
        protected (double StaminaChange, int Weight) SmellInfo => (-DEFAULT_ACTION_STAMINA_COST * SMELL_ACTION_COST_MULTIPLIER, 0);

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
        protected override void DoAction(World world, Animal[,] surroundings, AnimalCoordinates selfLocal, AnimalCoordinates selfGlobal)
        {
            surroundings[selfLocal.Row, selfLocal.Column] = null;
            var antelopePositions = GetAnimalByName(surroundings, ANTELOPE_NAME);
            var actions = new List<(Action, int)>();
            if (antelopePositions.Count() > 0)
            {
                var antelope = GetClosestAntelope(selfLocal, antelopePositions);
                if (DistanceToCalculator(selfLocal, antelope) == 1)
                {
                    actions.Add((() => Attack(antelope.Animal), 0));
                }
                else
                {
                    var direction = DecideMoveDirection(selfLocal, surroundings, antelopePositions);
                    actions.Add((() => Move(world, selfGlobal, direction), MoveInfo.Weight));
                }
            }
            else if (Stamina < TiredStaminaThreshold)
            {
                actions.Add((Sleep, SleepInfo.Weight));
            }
            else if (Health < HungryThreshold)
            {
                actions.Add((() => MoveBySmell(world, selfGlobal), 0));
            }
            else
            {
                if (HaveEnoughStamina(selfLocal.Animal.Stamina, MoveInfo.StaminaChange))
                {
                    var direction = DecideMoveDirection(selfLocal, surroundings);
                    actions.Add((() => Move(world, selfGlobal, direction), MoveInfo.Weight));
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
            if (antelopes != null && antelopes.Count != 0)
            {
                directions = GetClosestDirectionsToAntelope(directions, self, GetClosestAntelope(self, antelopes));
            }
            return Movement.RandomDirection(directions);
        }


        /// <summary>
        /// Gets closest antelope position
        /// </summary>
        /// <param name="self">own position</param>
        /// <param name="antelopes">all visible antelope positions</param>
        /// <returns>AnimalCoordinates of closest antelope.</returns>
        protected AnimalCoordinates GetClosestAntelope(AnimalCoordinates self, List<AnimalCoordinates> antelopes)
        {
            var random = new Random();
            double closestEnemies = antelopes
                .Min(enemy => DistanceToCalculator(self, enemy));
            List<AnimalCoordinates> enemies = antelopes
                .Where(enemy => DistanceToCalculator(self, enemy) == closestEnemies)
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
        protected List<Direction> GetClosestDirectionsToAntelope(List<Direction> directions, AnimalCoordinates self, AnimalCoordinates antelope)
        {
            var directionWithDistanceToEnemy = directions.Select(direction => new
            {
                direction,
                distance = DistanceToCalculator(
                    new AnimalCoordinates(self.Row + Movement.Directions[direction].row, self.Column + Movement.Directions[direction].column, self.Animal), antelope)
            }).ToList();
            double afterMoveDistance = directionWithDistanceToEnemy.Min(value => value.distance);
            var returnDirections = directionWithDistanceToEnemy
                    .Where(value => value.distance == afterMoveDistance)
                    .Select(value => value.direction)
                    .ToList();

            return returnDirections;
        }

        /// <summary>
        /// Lion attacks
        /// </summary>
        /// <param name="antelope">AnimalCoordinates where to attack</param>
        protected void Attack(Animal antelope)
        {
            if (HaveEnoughStamina(Stamina, AttackInfo.StaminaChange))
            {
                Stamina += AttackInfo.StaminaChange;
                antelope.Damage(AttackInfo.Damage);
                if (!antelope.IsAlive()) Heal(MaxHealth * ATTACK_KILL_HEALTH_GAIN_PRECENTAGE);
            }
            else Rest();
        }

        /// <summary>
        /// Smell to determine direction to prey
        /// </summary>
        /// <param name="world">World where lion lives</param>
        /// <param name="self">Own position to smell from</param>
        protected void MoveBySmell(World world, AnimalCoordinates self)
        {
            if (HaveEnoughStamina(Stamina, MoveInfo.StaminaChange + SmellInfo.StaminaChange))
            {
                Stamina += SmellInfo.StaminaChange;
                var area = world.GetField();
                var antelopePositions = GetAnimalByName(area, ANTELOPE_NAME);
                var direction = DecideMoveDirection(self, area, antelopePositions);
                Move(world, self, direction);
            }
            else Rest();
        }

        /// <summary>
        /// Lion decides to roar
        /// </summary>
        protected void Roar()
        {
            if (HaveEnoughStamina(Stamina, RoarInfo.StaminaChange)) Stamina += RoarInfo.StaminaChange;
            else Rest();
        }
    }
}

using Savanna.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Savanna.Tests
{
    public class TestAnimal : Animal
    {
        public const int TEST_ROUNDS_TO_REPRODUCE = ROUNDS_TO_REPRODUCE;
        public const double TEST_TIME_PER_ROUND = TIME_PER_ROUND;
        // Antelope settings as constants
        private string TEST_ANIMAL_NAME = "Test";
        private const string TEST_ANIMAL_DISPLAY_SYMBOL = "T";
        private const char TEST_ANIMAL_CREATION_KEY = 'T';
        private const int TEST_ANIMAL_DEFAULT_SPEED = 3;
        private const int TEST_ANIMAL_DEFAULT_VISION = 5;
        private const int TEST_ANIMAL_DEFAULT_ENDURANCE = 8;
        private const int TEST_ANIMAL_DEFAULT_DEFENCE = 2;

        private const int TEST_ANIMAL_ROUNDS_TO_DECOMPOSE = 10;
        private const double TEST_ANIMAL_HEALTH_DEDUCTION = 0.5;
        private const double TEST_ANIMAL_TIRED_PRECENTAGE = 0.4;
        private const int TEST_ANIMAL_REPRODUCTION_RANGE = 2;
        private const double TEST_ANIMAL_MAX_AGE = 18;
        private const double TEST_ANIMAL_CHILDREN_BEARING_AGE = 2;
        private const double TEST_ANIMAL_CHILDREN_PAUSE_TIME = 1.5;
        private const double TEST_ANIMAL_HUNGRY_PRECENTAGE = 0.1;

        private const int REST_POSIBILITY_WEIGHT = 25;
        private const int SLEEP_POSIBILITY_WEIGHT = 5;
        private const int MOVE_POSIBILITY_WEIGHT = 35;

        // Animal settings used
        public override string Name => TEST_ANIMAL_NAME;
        public override string DisplaySymbol => TEST_ANIMAL_DISPLAY_SYMBOL;
        public override char CreationKey => TEST_ANIMAL_CREATION_KEY;
        protected override int DefaultSpeed => TEST_ANIMAL_DEFAULT_SPEED;
        protected override int DefaultVision => TEST_ANIMAL_DEFAULT_VISION;
        protected override int DefaultEndurance => TEST_ANIMAL_DEFAULT_ENDURANCE;
        protected override int DefaultDefence => TEST_ANIMAL_DEFAULT_DEFENCE;

        protected override double MaxStamina => DEFAULT_MAX_STAMINA * Speed;
        protected override double MaxHealth => DEFAULT_MAX_HEALTH * Defence;

        protected override int RoundsToDecompose => TEST_ANIMAL_ROUNDS_TO_DECOMPOSE;
        protected override double PerRoundHealthDeduction => TEST_ANIMAL_HEALTH_DEDUCTION;
        protected override double TiredStaminaThreshold => MaxStamina * TEST_ANIMAL_TIRED_PRECENTAGE;
        protected override int ReproductionRange => TEST_ANIMAL_REPRODUCTION_RANGE;
        protected override double MaxAgeLimit => TEST_ANIMAL_MAX_AGE;
        protected override double ChildrenBearingAge => TEST_ANIMAL_CHILDREN_BEARING_AGE;
        protected override double ChildrenPauseTime => TEST_ANIMAL_CHILDREN_PAUSE_TIME;
        protected override double HungryThreshold => MaxHealth * TEST_ANIMAL_HUNGRY_PRECENTAGE;

        protected override (double StaminaChange, int Weight) RestInfo => (REST_STAMINA_RECOVERY * Endurance, REST_POSIBILITY_WEIGHT);
        protected override (double StaminaChange, int Weight) SleepInfo => (MaxStamina * SLEEP_STAMINA_RECOVERY_PRECENTAGE, SLEEP_POSIBILITY_WEIGHT);
        protected override (double StaminaChange, int Weight) MoveInfo => (-DEFAULT_ACTION_STAMINA_COST, MOVE_POSIBILITY_WEIGHT);

        public TestAnimal()
        {
            Speed = DefaultSpeed;
            Vision = DefaultVision;
            Endurance = DefaultEndurance;
            Defence = DefaultDefence;
            Health = MaxHealth;
            Stamina = MaxStamina;
        }

        protected override Direction? DecideMoveDirection(AnimalCoordinates self, Animal[,] surroundings, List<AnimalCoordinates>? enemies = null, List<AnimalCoordinates>? allies = null)
        {
            return null;
        }

        protected override void DoAction(World world, Animal[,] surroundings, AnimalCoordinates selfLocaly, AnimalCoordinates selfGlobaly) { }

        public void UpdateVision(int x)
        {
            Vision = x;
        }
        public void UpdateName(string name)
        {
            TEST_ANIMAL_NAME = name;
        }
        public void MakeDecomposed()
        {
            _isAlive = false;
            _roundsDead = RoundsToDecompose + 1;
        }

        public Dictionary<Animal, int> TestPossibleMates
        {
            get => _possibleMates;
            set => _possibleMates = value;
        }

        public double TestCurrentChildrenPause
        {
            get => _currentChildrenPause;
            set => _currentChildrenPause = value;
        }
        public double TestAge
        {
            get => _age;
            set => _age = value;
        }
        public int TestRoundsDead
        {
            get => _roundsDead;
            set => _roundsDead = value;
        }
        public bool TestIsAlive
        {
            get => _isAlive;
            set => _isAlive = value;
        }
        public double TestChildrenBearingAge => ChildrenBearingAge;
        public double TestChildrenPauseTime => ChildrenPauseTime;
        public int TestReproductionRange => ReproductionRange;

        public void TestPerRoundUpdate() => PerRoundUpdate();
        public void TestRest() => Rest();
        public void TestSleep() => Sleep();
        public void TestHeal(double damageHealed) => Heal(damageHealed);
        public void TestChangeStamina(double staminaChange) => ChangeStamina(staminaChange);
        public void TestMove(World world, AnimalCoordinates self, Direction? direction) => Move(world, self, direction);

        public bool TestHaveEnoughStamina(double stamina, double staminaChange) => HaveEnoughStamina(stamina,staminaChange);
        public bool TestIsStatAboveMax(double maxValue, double currentValue, double gain) => IsStatAboveMax(maxValue,currentValue,gain);
        public void TestMating(World world, Animal?[,] visibleArea, AnimalCoordinates selfLocaly, AnimalCoordinates selfGlobaly) => Mating(world, visibleArea, selfLocaly, selfGlobaly);
        public List<AnimalCoordinates> TestGetAnimalByName(Animal[,] surroundings, string name) => GetAnimalByName(surroundings, name);
        public List<AnimalCoordinates> TestFilterCloseEnoughMates(List<AnimalCoordinates> visibleMates, AnimalCoordinates self) => FilterCloseEnoughMates(visibleMates, self);

    }
}


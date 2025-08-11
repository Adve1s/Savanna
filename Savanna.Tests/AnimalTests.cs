using Microsoft.VisualStudio.TestPlatform.Common.DataCollection;
using Savanna.Logic;

namespace Savanna.Tests
{
    [TestClass]
    public sealed class AnimalTests
    {
        private (World world, Animal?[,] visibleArea, AnimalCoordinates selfLocaly, AnimalCoordinates selfGlobaly, TestAnimal animal) MatingSetup()
        {
            var world = new World(100, 100);

            var animal = new TestAnimal();
            var secondAnimal = new TestAnimal();
            var thirdAnimal = new TestAnimal();
            world.AddAnimal(animal, new AnimalCoordinates(0, 0));
            world.AddAnimal(secondAnimal, new AnimalCoordinates(1, 0));
            world.AddAnimal(thirdAnimal, new AnimalCoordinates(0, 1));
            var field = world.GetField();

            animal.TestPossibleMates[secondAnimal] = TestAnimal.TEST_ROUNDS_TO_REPRODUCE;
            animal.TestPossibleMates[thirdAnimal] = TestAnimal.TEST_ROUNDS_TO_REPRODUCE;
            animal.TestAge = animal.TestChildrenBearingAge;
            animal.TestCurrentChildrenPause = animal.TestChildrenPauseTime;

            secondAnimal.TestPossibleMates[animal] = TestAnimal.TEST_ROUNDS_TO_REPRODUCE;
            secondAnimal.TestPossibleMates[thirdAnimal] = TestAnimal.TEST_ROUNDS_TO_REPRODUCE;
            secondAnimal.TestAge = secondAnimal.TestChildrenBearingAge;
            secondAnimal.TestCurrentChildrenPause = secondAnimal.TestChildrenPauseTime;

            thirdAnimal.TestPossibleMates[animal] = TestAnimal.TEST_ROUNDS_TO_REPRODUCE;
            thirdAnimal.TestPossibleMates[secondAnimal] = TestAnimal.TEST_ROUNDS_TO_REPRODUCE;
            thirdAnimal.TestAge = thirdAnimal.TestChildrenBearingAge;
            thirdAnimal.TestCurrentChildrenPause = thirdAnimal.TestChildrenPauseTime;

            var visibleArea = new Animal[animal.Vision + 1, animal.Vision + 1];
            visibleArea[0, 0] = animal;
            visibleArea[1, 0] = secondAnimal;
            visibleArea[0, 1] = thirdAnimal;

            return (world, visibleArea, new AnimalCoordinates(0, 0, animal), new AnimalCoordinates(0, 0, animal), animal);
        }

        #region Core Animal Methods
        [TestMethod]
        public void Turn_WhenNoMatingAndAlive_AnimalDoesItsActionAndUpdatesStats()
        {
            var world = new World(10, 10);
            var animal = new TestAnimal();
            world.AddAnimal(animal, new AnimalCoordinates(0, 0));
            var startHealth = animal.Health;

            animal.Turn(world, new AnimalCoordinates(0, 0, animal));

            Assert.AreEqual(animal.TestAge, TestAnimal.TEST_TIME_PER_ROUND);
            Assert.AreEqual(animal.TestCurrentChildrenPause, TestAnimal.TEST_TIME_PER_ROUND);
            Assert.IsTrue(animal.IsAlive());
            Assert.AreEqual(0, animal.TestRoundsDead);
            Assert.AreEqual(1, world.GetField().Cast<Animal>().Count(x => x != null));
        }

        [TestMethod]
        public void Turn_WhenMatingAndAlive_AnimalDoesItsActionAndUpdatesStatsAndAnimalIsAdded()
        {
            var value = MatingSetup();
            var animal = value.animal;
            var startHealth = animal.Health;
            var startTestAge = animal.TestAge;

            animal.Turn(value.world, new AnimalCoordinates(0, 0, animal));

            Assert.IsTrue(animal.TestAge > startTestAge);
            Assert.AreEqual(animal.TestCurrentChildrenPause, 0);
            Assert.IsTrue(animal.IsAlive());
            Assert.AreEqual(0, animal.TestRoundsDead);
            Assert.AreEqual(4, value.world.GetField().Cast<Animal>().Count(x => x != null));
        }

        [TestMethod]
        public void Turn_WhenDead_UpdaresRoundsDead()
        {
            var world = new World(10, 10);
            var animal = new TestAnimal();
            world.AddAnimal(animal, new AnimalCoordinates(0, 0));
            animal.TestIsAlive = false;

            animal.Turn(world, new AnimalCoordinates(0, 0, animal));

            Assert.AreEqual(animal.TestAge, 0);
            Assert.AreEqual(animal.TestCurrentChildrenPause, 0);
            Assert.IsFalse(animal.IsAlive());
            Assert.AreEqual(1, animal.TestRoundsDead);
            Assert.AreEqual(1, world.GetField().Cast<Animal>().Count(x => x != null));
        }

        [TestMethod]
        public void PerRoundUpdate_WhenNormal_UpdatesNeededStats()
        {
            var animal = new TestAnimal();
            var startHealth = animal.Health;

            animal.TestPerRoundUpdate();

            Assert.AreEqual(animal.TestAge, TestAnimal.TEST_TIME_PER_ROUND);
            Assert.AreEqual(animal.TestCurrentChildrenPause, TestAnimal.TEST_TIME_PER_ROUND);
            Assert.IsTrue(startHealth > animal.Health);
            Assert.IsTrue(animal.IsAlive());
            Assert.AreEqual(0, animal.TestRoundsDead);
        }

        [TestMethod]
        public void PerRoundUpdate_WhenDieFromTestAge_UpdatesNeededStats()
        {
            var animal = new TestAnimal();
            var startHealth = animal.Health;
            animal.TestAge = double.MaxValue - 100;

            animal.TestPerRoundUpdate();

            Assert.IsTrue(startHealth > animal.Health);
            Assert.IsFalse(animal.IsAlive());
            Assert.AreEqual(0, animal.TestRoundsDead);
        }

        [TestMethod]
        public void PerRoundUpdate_WhenDieFromHealth_UpdatesNeededStats()
        {
            var animal = new TestAnimal();
            animal.Damage(animal.Health - 0.0000000001);

            animal.TestPerRoundUpdate();

            Assert.AreEqual(animal.TestCurrentChildrenPause, TestAnimal.TEST_TIME_PER_ROUND);
            Assert.IsTrue(0 > animal.Health);
            Assert.IsFalse(animal.IsAlive());
            Assert.AreEqual(0, animal.TestRoundsDead);
        }

        [TestMethod]
        public void PerRoundUpdate_WhenAlreadyDead_UpdatesRoundsDead()
        {
            var animal = new TestAnimal();
            animal.TestIsAlive = false;

            animal.TestPerRoundUpdate();

            Assert.IsFalse(animal.IsAlive());
            Assert.AreEqual(1, animal.TestRoundsDead);
        }

        [TestMethod]
        public void Rest_WhenNotOverMax_AnimalGainsSomeStamina()
        {
            var animal = new TestAnimal();
            var maxStamina = animal.Stamina;
            animal.TestChangeStamina(-animal.Stamina);

            animal.TestRest();

            Assert.IsTrue(0 < animal.Stamina);
            Assert.AreNotEqual(maxStamina, animal.Stamina);
        }

        [TestMethod]
        public void Rest_WhenOverMax_AnimalGainsUpToMaxStamina()
        {
            var animal = new TestAnimal();
            var maxStamina = animal.Stamina;
            animal.TestChangeStamina(-1);
            var startStamina = animal.Stamina;

            animal.TestRest();

            Assert.IsTrue(startStamina < animal.Stamina);
            Assert.AreEqual(maxStamina, animal.Stamina);
        }

        [TestMethod]
        public void Rest_WhenAtMax_DoesNothing()
        {
            var animal = new TestAnimal();
            var maxStamina = animal.Stamina;
            var startStamina = animal.Stamina;

            animal.TestRest();

            Assert.AreEqual(maxStamina, animal.Stamina, startStamina);
        }

        [TestMethod]
        public void Sleep_WhenNotOverMax_AnimalGainsSomeStamina()
        {
            var animal = new TestAnimal();
            var maxStamina = animal.Stamina;
            animal.TestChangeStamina(-animal.Stamina);

            animal.TestSleep();

            Assert.IsTrue(0 < animal.Stamina);
            Assert.AreNotEqual(maxStamina, animal.Stamina);
        }

        [TestMethod]
        public void Sleep_WhenOverMax_AnimalGainsUpToMaxStamina()
        {
            var animal = new TestAnimal();
            var maxStamina = animal.Stamina;
            animal.TestChangeStamina(-1);
            var startStamina = animal.Stamina;

            animal.TestSleep();

            Assert.IsTrue(startStamina < animal.Stamina);
            Assert.AreEqual(maxStamina, animal.Stamina);
        }

        [TestMethod]
        public void Sleep_WhenAtMax_DoesNothing()
        {
            var animal = new TestAnimal();
            var maxStamina = animal.Stamina;
            var startStamina = animal.Stamina;

            animal.TestSleep();

            Assert.AreEqual(maxStamina, animal.Stamina, startStamina);
        }

        [TestMethod]
        public void Damage_WhenNotOverHealth_HealthDecreasesBysetAmmountAnimalLives()
        {
            var animal = new TestAnimal();
            var startHealth = animal.Health;

            animal.Damage(10);
            var expectedHealth = startHealth - 10;

            Assert.AreEqual(expectedHealth, animal.Health);
            Assert.IsTrue(startHealth > animal.Health);
            Assert.IsTrue(animal.IsAlive());
        }

        [TestMethod]
        public void Damage_WhenOverHealth_AnimalDies()
        {
            var animal = new TestAnimal();
            var startHealth = animal.Health;

            animal.Damage(startHealth);

            Assert.IsFalse(animal.IsAlive());
        }

        [TestMethod]
        public void Heal_WhenNotOverMax_HealsAnimalToExpectedAmmount()
        {
            var animal = new TestAnimal();
            animal.Damage(10);
            var startHealth = animal.Health;

            animal.TestHeal(5);
            var expectedHealth = startHealth + 5;

            Assert.AreEqual(expectedHealth, animal.Health);
            Assert.IsTrue(startHealth < animal.Health);
        }

        [TestMethod]
        public void Heal_WhenOverMax_HealsAnimalToMaxHealth()
        {
            var animal = new TestAnimal();
            var expectedHealth = animal.Health;
            animal.Damage(10);
            var startHealth = animal.Health;

            animal.TestHeal(25);

            Assert.AreEqual(expectedHealth, animal.Health);
            Assert.IsTrue(startHealth < animal.Health);
        }

        [TestMethod]
        public void Move_WhenSuccess_TestMovesAnimal()
        {
            var world = new World(10, 15);
            var animal = new TestAnimal();
            world.AddAnimal(animal, new AnimalCoordinates(9, 14));

            animal.TestMove(world, new AnimalCoordinates(9, 14, animal), Direction.North);

            Assert.IsNull(world.GetField()[9, 14]);
            Assert.IsInstanceOfType<Animal>(world.GetField()[8, 14]);
        }

        [TestMethod]
        public void Move_WhenNoDirection_TestRests()
        {
            var world = new World(10, 10);
            var animal = new TestAnimal();
            world.AddAnimal(animal, new AnimalCoordinates(0, 0));
            var startStamina = animal.Stamina;

            animal.TestMove(world, new AnimalCoordinates(0, 0, animal), null);

            Assert.AreEqual(world.GetField()[0, 0], animal);
            Assert.IsTrue(startStamina <= animal.Stamina);
        }

        [TestMethod]
        public void Move_WhenNoStamina_TestMovesAnimal()
        {
            var world = new World(10, 10);
            var animal = new TestAnimal();
            world.AddAnimal(animal, new AnimalCoordinates(0, 0));
            animal.TestChangeStamina(-animal.Stamina - 1);
            var startStamina = animal.Stamina;

            animal.TestMove(world, new AnimalCoordinates(0, 0, animal), Direction.South);

            Assert.IsNull(world.GetField()[1, 0]);
            Assert.AreEqual(world.GetField()[0, 0], animal);
            Assert.IsTrue(startStamina <= animal.Stamina);
        }

        [TestMethod]
        public void Mating_WhenSuccessWithOnePlaceToSpawn_OneAnimalIsAddedInTheFreeSpotAndStatusReset()
        {
            var value = MatingSetup();

            value.animal.TestMating(value.world, value.visibleArea, value.selfLocaly, value.selfGlobaly);
            var afterField = value.world.GetField();
            var animalCount = afterField.Cast<Animal>().Count(x => x != null);


            Assert.IsNotNull(afterField[1, 1]);
            Assert.AreEqual(4, animalCount);
            Assert.IsFalse(value.animal.TestPossibleMates.Any());
            Assert.AreEqual(value.animal.TestCurrentChildrenPause, 0);
        }

        [TestMethod]
        public void Mating_WhenSuccessWithMultiplePlacesToSpawn_OneAnimalAddedAndStatusReset()
        {
            var value = MatingSetup();
            value.world.MoveAnimal(value.selfGlobaly, Direction.SouthEast);
            (value.visibleArea[0, 0], value.visibleArea[1, 1]) = (value.visibleArea[1, 1], value.visibleArea[0, 0]);
            value.selfLocaly.Column = 1;
            value.selfLocaly.Row = 1;
            value.selfGlobaly.Column = 1;
            value.selfGlobaly.Row = 1;

            value.animal.TestMating(value.world, value.visibleArea, value.selfLocaly, value.selfGlobaly);
            var afterField = value.world.GetField();
            var animalCount = afterField.Cast<Animal>().Count(x => x != null);
            var potentialChildPlaces = new List<Animal> { afterField[0, 0], afterField[2, 0], afterField[2, 1], afterField[2, 2], afterField[0, 2], afterField[1, 2] };
            var childCount = potentialChildPlaces.Cast<Animal>().Count(x => x != null);


            Assert.AreEqual(1, childCount);
            Assert.AreEqual(4, animalCount);
            Assert.IsFalse(value.animal.TestPossibleMates.Any());
            Assert.AreEqual(value.animal.TestCurrentChildrenPause, 0);
        }

        [TestMethod]
        public void Mating_WhenTooYoung_DoesntChangeAnything()
        {
            var value = MatingSetup();
            value.animal.TestAge = 0;

            value.animal.TestMating(value.world, value.visibleArea, value.selfLocaly, value.selfGlobaly);
            var afterField = value.world.GetField();
            var animalCount = afterField.Cast<Animal>().Count(x => x != null);


            Assert.IsNull(afterField[1, 1]);
            Assert.AreEqual(3, animalCount);
        }

        [TestMethod]
        public void Mating_WhenOnPause_DoesntChangeAnything()
        {
            var value = MatingSetup();
            value.animal.TestCurrentChildrenPause = 0;

            value.animal.TestMating(value.world, value.visibleArea, value.selfLocaly, value.selfGlobaly);
            var afterField = value.world.GetField();
            var animalCount = afterField.Cast<Animal>().Count(x => x != null);


            Assert.IsNull(afterField[1, 1]);
            Assert.AreEqual(3, animalCount);
        }

        [TestMethod]
        public void Mating_WhenNotEnoughtByEachOtherInOwnList_UpdatesTimeInOwnList()
        {
            var value = MatingSetup();
            var secondAnimal = value.visibleArea[1, 0];
            var thirdAnimal = value.visibleArea[0, 1];
            value.animal.TestPossibleMates[secondAnimal] = 0;
            value.animal.TestPossibleMates[thirdAnimal] = 0;

            value.animal.TestMating(value.world, value.visibleArea, value.selfLocaly, value.selfGlobaly);
            var afterField = value.world.GetField();
            var animalCount = afterField.Cast<Animal>().Count(x => x != null);

            Assert.IsNull(afterField[1, 1]);
            Assert.AreEqual(3, animalCount);
            Assert.AreEqual(value.animal.TestPossibleMates[secondAnimal], 1);
            Assert.AreEqual(value.animal.TestPossibleMates[thirdAnimal], 1);
        }

        [TestMethod]
        public void Mating_WhenNotEnoughtByEachOtherInTheirList_UpdatesTimeInOwnList()
        {
            var value = MatingSetup();
            var secondAnimal = (TestAnimal)value.visibleArea[1, 0];
            var thirdAnimal = (TestAnimal)value.visibleArea[0, 1];
            secondAnimal.TestPossibleMates[value.selfGlobaly.Animal] = 0;
            thirdAnimal.TestPossibleMates[value.selfGlobaly.Animal] = 0;

            value.animal.TestMating(value.world, value.visibleArea, value.selfLocaly, value.selfGlobaly);
            var afterField = value.world.GetField();
            var animalCount = afterField.Cast<Animal>().Count(x => x != null);

            Assert.IsNull(afterField[1, 1]);
            Assert.AreEqual(3, animalCount);
            Assert.AreEqual(value.animal.TestPossibleMates[secondAnimal], 4);
            Assert.AreEqual(value.animal.TestPossibleMates[thirdAnimal], 4);
        }

        [TestMethod]
        public void Mating_WhenNMateDissapearsFromRange_ClearsMateListOneByOne()
        {

            var value = MatingSetup();
            var secondAnimal = (TestAnimal)value.visibleArea[1, 0];
            var thirdAnimal = (TestAnimal)value.visibleArea[0, 1];
            value.animal.TestPossibleMates[secondAnimal] = 1;
            thirdAnimal.TestIsAlive = false;
            value.visibleArea[0, 1] = null;

            value.animal.TestMating(value.world, value.visibleArea, value.selfLocaly, value.selfGlobaly);

            Assert.AreEqual(value.animal.TestPossibleMates[secondAnimal], 2);
            CollectionAssert.DoesNotContain(value.animal.TestPossibleMates.Keys.ToList(), thirdAnimal);

            thirdAnimal.TestIsAlive = true;
            value.visibleArea[0, 1] = thirdAnimal;
            secondAnimal.TestIsAlive = false;
            value.visibleArea[1, 0] = null;

            value.animal.TestMating(value.world, value.visibleArea, value.selfLocaly, value.selfGlobaly);

            CollectionAssert.DoesNotContain(value.animal.TestPossibleMates.Keys.ToList(), secondAnimal);
            Assert.AreEqual(value.animal.TestPossibleMates[thirdAnimal], 1);
        }
        #endregion

        #region Query Methods
        [TestMethod]
        public void TestGetAnimalByName_WhenEmpty_ReturnsNoAnimalCoordinates()
        {
            var world = new World(5, 5);
            var field = world.GetField();
            var animal = new TestAnimal();

            var coordinateList = animal.TestGetAnimalByName(field, animal.Name);

            Assert.AreEqual(0, coordinateList.Count);
        }

        [TestMethod]
        public void TestGetAnimalByName_WhenOnlyWrongType_ReturnsNoAnimalCoordinates()
        {
            var world = new World(5, 5);
            var wrongAnimal = new TestAnimal();
            wrongAnimal.UpdateName("Wrong");
            var wrongAnimal2 = new TestAnimal();
            wrongAnimal2.UpdateName("WrongToo");
            world.AddAnimal(wrongAnimal, new AnimalCoordinates(0,0));
            world.AddAnimal(wrongAnimal2, new AnimalCoordinates(0,2));
            var field = world.GetField();
            var animal = new TestAnimal();

            var coordinateList = animal.TestGetAnimalByName(field, animal.Name);

            Assert.AreEqual(0, coordinateList.Count);
        }

        [TestMethod]
        public void TestGetAnimalByName_WhenOnlyRightTypeExists_ReturnsCorrectCoordinates()
        {
            var world = new World(5, 5);
            world.AddAnimal(new TestAnimal(), new AnimalCoordinates(0, 0));
            world.AddAnimal(new TestAnimal(), new AnimalCoordinates(1, 0));
            var field = world.GetField();
            var added1 = new AnimalCoordinates(0, 0, field[0, 0]);
            var added2 = new AnimalCoordinates(1, 0, field[1, 0]);
            var animal = new TestAnimal();

            var coordinateList = animal.TestGetAnimalByName(field, animal.Name);

            CollectionAssert.Contains(coordinateList, added1);
            CollectionAssert.Contains(coordinateList, added2);
            Assert.AreEqual(2, coordinateList.Count);
        }

        [TestMethod]
        public void TestGetAnimalByName_WhenRightAndWrongTypeExists_ReturnsCorrectCoordinates()
        {
            var world = new World(5, 5);
            var wrongAnimal = new TestAnimal();
            wrongAnimal.UpdateName("Wrong");
            world.AddAnimal(new TestAnimal(), new AnimalCoordinates(0, 0));
            world.AddAnimal(wrongAnimal, new AnimalCoordinates(1, 0));
            var field = world.GetField();
            var added1 = new AnimalCoordinates(0, 0, field[0, 0]);
            var added2 = new AnimalCoordinates(1, 0, field[1, 0]);
            var animal = new TestAnimal();

            var coordinateList = animal.TestGetAnimalByName(field, animal.Name);

            CollectionAssert.Contains(coordinateList, added1);
            CollectionAssert.DoesNotContain(coordinateList, added2);
            Assert.AreEqual(1, coordinateList.Count);
        }

        [TestMethod]
        public void FilterCloseEnoughMates_WhenEmpty_ReturnsNoAnimalCoordinates()
        {
            var animal = new TestAnimal();
            var animalCoordinates = new AnimalCoordinates(0, 0, animal);
            var mateList = new List<AnimalCoordinates>();

            var coordinateList = animal.TestFilterCloseEnoughMates(mateList, animalCoordinates);

            Assert.AreEqual(0, coordinateList.Count);
        }

        [TestMethod]
        public void FilterCloseEnoughMates_WhenMatesOnlyNear_ReturnsMateCoordinates()
        {
            var animal = new TestAnimal();
            var animalCoordinates = new AnimalCoordinates(0, 0, animal);
            var mateList = new List<AnimalCoordinates>()
            {
                new AnimalCoordinates(1,0),
                new AnimalCoordinates(0,1)
            };

            var coordinateList = animal.TestFilterCloseEnoughMates(mateList, animalCoordinates);

            Assert.AreEqual(2, coordinateList.Count);
        }

        [TestMethod]
        public void FilterCloseEnoughMates_WhenMatesOnlyFar_ReturnsNoAnimalCoordinates()
        {
            var animal = new TestAnimal();
            var animalCoordinates = new AnimalCoordinates(0, 0, animal);
            var mateList = new List<AnimalCoordinates>()
            {
                new AnimalCoordinates(animal.TestReproductionRange+1,0),
                new AnimalCoordinates(0,animal.TestReproductionRange+1)
            };

            var coordinateList = animal.TestFilterCloseEnoughMates(mateList, animalCoordinates);

            Assert.AreEqual(0, coordinateList.Count);
        }

        [TestMethod]
        public void FilterCloseEnoughMates_WhenMatesBothCloseAndFar_ReturnsOnlyCorrectCoordinates()
        {
            var animal = new TestAnimal();
            var animalCoordinates = new AnimalCoordinates(0, 0, animal);
            var close = new AnimalCoordinates(1, 1);
            var mateList = new List<AnimalCoordinates>()
            {
                new AnimalCoordinates(animal.TestReproductionRange+1,0),
                new AnimalCoordinates(0,animal.TestReproductionRange+1),
                close
            };

            var coordinateList = animal.TestFilterCloseEnoughMates(mateList, animalCoordinates);

            Assert.AreEqual(1, coordinateList.Count);
            CollectionAssert.Contains(coordinateList, close);
        }

        [TestMethod]
        public void DistanceToCalculator_WhenDistanceIsRowDifferenceNegative_ReturnsRowDifference()
        {
            var animal = new TestAnimal();
            var from = new AnimalCoordinates(50, 50, animal);
            var to = new AnimalCoordinates(56, 51);
            var diff = 6;

            var distance = animal.DistanceToCalculator(from, to);

            Assert.AreEqual(distance, diff);
        }

        [TestMethod]
        public void DistanceToCalculator_WhenDistanceIsColumnDifferenceNegative_ReturnsRowDifference()
        {
            var animal = new TestAnimal();
            var from = new AnimalCoordinates(50, 50, animal);
            var to = new AnimalCoordinates(51, 56);
            var diff = 6;

            var distance = animal.DistanceToCalculator(from, to);

            Assert.AreEqual(distance, diff);
        }

        [TestMethod]
        public void DistanceToCalculator_WhenDistanceIsRowDifferencePositive_ReturnsRowDifference()
        {
            var animal = new TestAnimal();
            var from = new AnimalCoordinates(50, 50, animal);
            var to = new AnimalCoordinates(43, 49);
            var diff = 7;

            var distance = animal.DistanceToCalculator(from, to);

            Assert.AreEqual(distance, diff);
        }

        [TestMethod]
        public void DistanceToCalculator_WhenDistanceIsColumnDifferencePositive_ReturnsRowDifference()
        {
            var animal = new TestAnimal();
            var from = new AnimalCoordinates(50, 50, animal);
            var to = new AnimalCoordinates(49, 43);
            var diff = 7;

            var distance = animal.DistanceToCalculator(from, to);

            Assert.AreEqual(distance, diff);
        }

        [TestMethod]
        public void IsDecomposed_WhenAlive_ReturnsFalse()
        {
            var animal = new TestAnimal();

            var value = animal.IsDecomposed();

            Assert.IsFalse(value);
        }

        [TestMethod]
        public void IsDecomposed_WhenDeadButNotDecomposed_ReturnsFalse()
        {
            var animal = new TestAnimal();
            animal.TestIsAlive = false;

            var value = animal.IsDecomposed();

            Assert.IsFalse(value);
        }

        [TestMethod]
        public void IsDecomposed_WhenDeadAndDecomposed_ReturnsTrue()
        {
            var animal = new TestAnimal();
            animal.TestIsAlive = false;
            animal.TestRoundsDead = int.MaxValue;

            var value = animal.IsDecomposed();

            Assert.IsTrue(value);
        }

        [TestMethod]
        public void HaveEnoughStamina_WhenJustEnough_ReturnsTrue()
        {
            var animal = new TestAnimal();

            var value = animal.TestHaveEnoughStamina(10, -10);

            Assert.IsTrue(value);
        }

        [TestMethod]
        public void HaveEnoughStamina_WhenEnough_ReturnsTrue()
        {
            var animal = new TestAnimal();

            var value = animal.TestHaveEnoughStamina(100, -10);

            Assert.IsTrue(value);
        }

        [TestMethod]
        public void HaveEnoughStamina_WhenJustNotEnough_ReturnsTrue()
        {
            var animal = new TestAnimal();

            var value = animal.TestHaveEnoughStamina(9, -10);

            Assert.IsFalse(value);
        }

        [TestMethod]
        public void HaveEnoughStamina_WhenNotEnough_ReturnsTrue()
        {
            var animal = new TestAnimal();

            var value = animal.TestHaveEnoughStamina(0, -10);

            Assert.IsFalse(value);
        }

        [TestMethod]
        public void IsStatAboveMax_WhenJustAboveMax_ReturnsTrue()
        {
            var animal = new TestAnimal();

            var value = animal.TestIsStatAboveMax(10, 9, 1.1);

            Assert.IsTrue(value);
        }

        [TestMethod]
        public void IsStatAboveMax_WhenAboveMax_ReturnsTrue()
        {
            var animal = new TestAnimal();

            var value = animal.TestIsStatAboveMax(10, 9, 10);

            Assert.IsTrue(value);
        }

        [TestMethod]
        public void IsStatAboveMax_WhenJustNotAbove_ReturnsTrue()
        {
            var animal = new TestAnimal();

            var value = animal.TestIsStatAboveMax(10, 9, 1);

            Assert.IsFalse(value);
        }

        [TestMethod]
        public void IsStatAboveMax_WhenNotAbove_ReturnsTrue()
        {
            var animal = new TestAnimal();

            var value = animal.TestIsStatAboveMax(10, 5, 3.5);

            Assert.IsFalse(value);
        }
        #endregion
    }
}

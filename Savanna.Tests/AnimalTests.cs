//using Microsoft.VisualStudio.TestPlatform.Common.DataCollection;
//using Savanna.Logic;

//namespace Savanna.Tests
//{
//    [TestClass]
//    public sealed class AnimalTests
//    {
//        private (World world, Animal?[,] visibleArea, AnimalCoordinates selfLocaly, AnimalCoordinates selfGlobaly) MatingSetup()
//        {
//            var world = new World(100, 100);

//            world.AddAnimal(new Antelope(), new AnimalCoordinates(0, 0));
//            world.AddAnimal(new Antelope(), new AnimalCoordinates(1, 0));
//            world.AddAnimal(new Antelope(), new AnimalCoordinates(0, 1));
//            var field = world.GetField();

//            var animal = field[0, 0];
//            var secondAnimal = field[1, 0];
//            var thirdAnimal = field[0, 1];

//            animal.possibleMates[secondAnimal] = Animal.ROUNDS_TO_REPRODUCE;
//            animal.possibleMates[thirdAnimal] = Animal.ROUNDS_TO_REPRODUCE;
//            animal._age = animal.ChildrenBearingAge;
//            animal._currentChildrenPause = animal.ChildrenPauseTime;

//            secondAnimal.possibleMates[animal] = Animal.ROUNDS_TO_REPRODUCE;
//            secondAnimal.possibleMates[thirdAnimal] = Animal.ROUNDS_TO_REPRODUCE;
//            secondAnimal._age = secondAnimal.ChildrenBearingAge;
//            secondAnimal._currentChildrenPause = secondAnimal.ChildrenPauseTime;

//            thirdAnimal.possibleMates[animal] = Animal.ROUNDS_TO_REPRODUCE;
//            thirdAnimal.possibleMates[secondAnimal] = Animal.ROUNDS_TO_REPRODUCE;
//            thirdAnimal._age = thirdAnimal.ChildrenBearingAge;
//            thirdAnimal._currentChildrenPause = thirdAnimal.ChildrenPauseTime;

//            var visibleArea = new Animal[animal.Vision + 1, animal.Vision + 1];
//            visibleArea[0, 0] = animal;
//            visibleArea[1, 0] = secondAnimal;
//            visibleArea[0, 1] = thirdAnimal;

//            return (world, visibleArea, new AnimalCoordinates(0, 0, animal), new AnimalCoordinates(0, 0, animal));
//        }

//        #region Core Animal Methods
//        [TestMethod]
//        public void Turn_WhenNoMatingAndAlive_AnimalDoesItsActionAndUpdatesStats()
//        {
//            var world = new World(10, 10);
//            world.AddAnimal(new Antelope(), new AnimalCoordinates(0, 0));
//            var animal = world.GetField()[0, 0];
//            var startHealth = animal.Health;

//            animal.Turn(world, new AnimalCoordinates(0, 0, animal));

//            Assert.AreEqual(animal._age, Animal.TIME_PER_ROUND);
//            Assert.AreEqual(animal._currentChildrenPause, Animal.TIME_PER_ROUND);
//            Assert.IsTrue(animal.IsAlive());
//            Assert.AreEqual(0, animal._roundsDead);
//            Assert.AreEqual(1, world.GetField().Cast<Animal>().Count(x => x != null));
//        }

//        [TestMethod]
//        public void Turn_WhenMatingAndAlive_AnimalDoesItsActionAndUpdatesStatsAndAnimalIsAdded()
//        {
//            var value = MatingSetup();
//            var animal = value.selfGlobaly.Animal;
//            var startHealth = animal.Health;
//            var startAge = animal._age;

//            animal.Turn(value.world, new AnimalCoordinates(0, 0, animal));

//            Assert.IsTrue(animal._age > startAge);
//            Assert.AreEqual(animal._currentChildrenPause, 0);
//            Assert.IsTrue(animal.IsAlive());
//            Assert.AreEqual(0, animal._roundsDead);
//            Assert.AreEqual(4, value.world.GetField().Cast<Animal>().Count(x => x != null));
//        }

//        [TestMethod]
//        public void Turn_WhenDead_UpdaresRoundsDead()
//        {
//            var world = new World(10, 10);
//            world.AddAnimal(new Antelope(), new AnimalCoordinates(0, 0));
//            var animal = world.GetField()[0, 0];
//            animal._isAlive = false;

//            animal.Turn(world, new AnimalCoordinates(0, 0, animal));

//            Assert.AreEqual(animal._age, 0);
//            Assert.AreEqual(animal._currentChildrenPause, 0);
//            Assert.IsFalse(animal.IsAlive());
//            Assert.AreEqual(1, animal._roundsDead);
//            Assert.AreEqual(1, world.GetField().Cast<Animal>().Count(x => x != null));
//        }

//        [TestMethod]
//        public void PerRoundUpdate_WhenNormal_UpdatesNeededStats()
//        {
//            var animal = new Antelope();
//            var startHealth = animal.Health;

//            animal.PerRoundUpdate();

//            Assert.AreEqual(animal._age, Animal.TIME_PER_ROUND);
//            Assert.AreEqual(animal._currentChildrenPause, Animal.TIME_PER_ROUND);
//            Assert.IsTrue(startHealth > animal.Health);
//            Assert.IsTrue(animal.IsAlive());
//            Assert.AreEqual(0, animal._roundsDead);
//        }

//        [TestMethod]
//        public void PerRoundUpdate_WhenDieFromAge_UpdatesNeededStats()
//        {
//            var animal = new Antelope();
//            var startHealth = animal.Health;
//            animal._age = double.MaxValue-100;

//            animal.PerRoundUpdate();

//            Assert.IsTrue(startHealth > animal.Health);
//            Assert.IsFalse(animal.IsAlive());
//            Assert.AreEqual(0, animal._roundsDead);
//        }

//        [TestMethod]
//        public void PerRoundUpdate_WhenDieFromHealth_UpdatesNeededStats()
//        {
//            var animal = new Antelope();
//            animal.Damage(animal.Health - 0.0000000001);

//            animal.PerRoundUpdate();

//            Assert.AreEqual(animal._currentChildrenPause, Animal.TIME_PER_ROUND);
//            Assert.IsTrue(0 > animal.Health);
//            Assert.IsFalse(animal.IsAlive());
//            Assert.AreEqual(0, animal._roundsDead);
//        }

//        [TestMethod]
//        public void PerRoundUpdate_WhenAlreadyDead_UpdatesRoundsDead()
//        {
//            var animal = new Antelope();
//            animal._isAlive = false;

//            animal.PerRoundUpdate();

//            Assert.IsFalse(animal.IsAlive());
//            Assert.AreEqual(1, animal._roundsDead);
//        }

//        [TestMethod]
//        public void Rest_WhenNotOverMax_AnimalGainsSomeStamina()
//        {
//            var animal = new Antelope();
//            var maxStamina = animal.Stamina;
//            animal.ChangeStamina(-animal.Stamina);

//            animal.Rest();

//            Assert.IsTrue(0 < animal.Stamina);
//            Assert.AreNotEqual(maxStamina, animal.Stamina);
//        }

//        [TestMethod]
//        public void Rest_WhenOverMax_AnimalGainsUpToMaxStamina()
//        {
//            var animal = new Antelope();
//            var maxStamina = animal.Stamina;
//            animal.ChangeStamina(-1);
//            var startStamina = animal.Stamina;

//            animal.Rest();

//            Assert.IsTrue(startStamina < animal.Stamina);
//            Assert.AreEqual(maxStamina, animal.Stamina);
//        }

//        [TestMethod]
//        public void Rest_WhenAtMax_DoesNothing()
//        {
//            var animal = new Antelope();
//            var maxStamina = animal.Stamina;
//            var startStamina = animal.Stamina;

//            animal.Rest();

//            Assert.AreEqual(maxStamina, animal.Stamina, startStamina);
//        }

//        [TestMethod]
//        public void Sleep_WhenNotOverMax_AnimalGainsSomeStamina()
//        {
//            var animal = new Antelope();
//            var maxStamina = animal.Stamina;
//            animal.ChangeStamina(-animal.Stamina);

//            animal.Sleep();

//            Assert.IsTrue(0 < animal.Stamina);
//            Assert.AreNotEqual(maxStamina, animal.Stamina);
//        }

//        [TestMethod]
//        public void Sleep_WhenOverMax_AnimalGainsUpToMaxStamina()
//        {
//            var animal = new Antelope();
//            var maxStamina = animal.Stamina;
//            animal.ChangeStamina(-1);
//            var startStamina = animal.Stamina;

//            animal.Sleep();

//            Assert.IsTrue(startStamina < animal.Stamina);
//            Assert.AreEqual(maxStamina, animal.Stamina);
//        }

//        [TestMethod]
//        public void Sleep_WhenAtMax_DoesNothing()
//        {
//            var animal = new Antelope();
//            var maxStamina = animal.Stamina;
//            var startStamina = animal.Stamina;

//            animal.Sleep();

//            Assert.AreEqual(maxStamina, animal.Stamina, startStamina);
//        }

//        [TestMethod]
//        public void Damage_WhenNotOverHealth_HealthDecreasesBysetAmmountAnimalLives()
//        {
//            var animal = new Antelope();
//            var startHealth = animal.Health;

//            animal.Damage(10);
//            var expectedHealth = startHealth - 10;

//            Assert.AreEqual(expectedHealth, animal.Health);
//            Assert.IsTrue(startHealth > animal.Health);
//            Assert.IsTrue(animal.IsAlive());
//        }

//        [TestMethod]
//        public void Damage_WhenOverHealth_AnimalDies()
//        {
//            var animal = new Antelope();
//            var startHealth = animal.Health;

//            animal.Damage(startHealth);

//            Assert.IsFalse(animal.IsAlive());
//        }

//        [TestMethod]
//        public void Heal_WhenNotOverMax_HealsAnimalToExpectedAmmount()
//        {
//            var animal = new Antelope();
//            animal.Damage(10);
//            var startHealth = animal.Health;

//            animal.Heal(5);
//            var expectedHealth = startHealth + 5;

//            Assert.AreEqual(expectedHealth, animal.Health);
//            Assert.IsTrue(startHealth < animal.Health);
//        }

//        [TestMethod]
//        public void Heal_WhenOverMax_HealsAnimalToMaxHealth()
//        {
//            var animal = new Antelope();
//            var expectedHealth = animal.Health;
//            animal.Damage(10);
//            var startHealth = animal.Health;

//            animal.Heal(25);

//            Assert.AreEqual(expectedHealth, animal.Health);
//            Assert.IsTrue(startHealth < animal.Health);
//        }

//        [TestMethod]
//        public void Move_WhenSuccess_MovesAnimal()
//        {
//            var world = new World(10, 15);
//            world.AddAnimal(new Antelope(), new AnimalCoordinates(9, 14));
//            var animal = world.GetField()[9, 14];

//            animal.Move(world, new AnimalCoordinates(9, 14, animal), Direction.North);

//            Assert.IsNull(world.GetField()[9, 14]);
//            Assert.IsInstanceOfType<Animal>(world.GetField()[8, 14]);
//        }

//        [TestMethod]
//        public void Move_WhenNoDirection_Rests()
//        {
//            var world = new World(10, 10);
//            world.AddAnimal(new Antelope(), new AnimalCoordinates(0, 0));
//            var animal = world.GetField()[0, 0];
//            var startStamina = animal.Stamina;

//            animal.Move(world, new AnimalCoordinates(0, 0, animal), null);

//            Assert.AreEqual(world.GetField()[0, 0], animal);
//            Assert.IsTrue(startStamina <= animal.Stamina);
//        }

//        [TestMethod]
//        public void Move_WhenNoStamina_MovesAnimal()
//        {
//            var world = new World(10, 10);
//            world.AddAnimal(new Antelope(), new AnimalCoordinates(0, 0));
//            var animal = world.GetField()[0, 0];
//            animal.ChangeStamina(-animal.Stamina-1);
//            var startStamina = animal.Stamina;

//            animal.Move(world, new AnimalCoordinates(0, 0, animal), Direction.South);

//            Assert.IsNull(world.GetField()[1, 0]);
//            Assert.AreEqual(world.GetField()[0, 0], animal);
//            Assert.IsTrue(startStamina <= animal.Stamina);
//        }

//        [TestMethod]
//        public void Mating_WhenSuccessWithOnePlaceToSpawn_OneAnimalIsAddedInTheFreeSpotAndStatusReset()
//        {
//            var value = MatingSetup();

//            value.selfGlobaly.Animal.Mating(value.world, value.visibleArea, value.selfLocaly, value.selfGlobaly);
//            var afterField = value.world.GetField();
//            var animalCount = afterField.Cast<Animal>().Count(x => x != null);


//            Assert.IsNotNull(afterField[1, 1]);
//            Assert.AreEqual(4, animalCount);
//            Assert.IsFalse(value.selfGlobaly.Animal.possibleMates.Any());
//            Assert.AreEqual(value.selfGlobaly.Animal._currentChildrenPause, 0);
//        }

//        [TestMethod]
//        public void Mating_WhenSuccessWithMultiplePlacesToSpawn_OneAnimalAddedAndStatusReset()
//        {
//            var value = MatingSetup();
//            value.world.MoveAnimal(value.selfGlobaly, Direction.SouthEast);
//            (value.visibleArea[0, 0], value.visibleArea[1, 1]) = (value.visibleArea[1, 1], value.visibleArea[0, 0]);
//            value.selfLocaly.Column = 1;
//            value.selfLocaly.Row = 1;
//            value.selfGlobaly.Column = 1;
//            value.selfGlobaly.Row = 1;

//            value.selfGlobaly.Animal.Mating(value.world, value.visibleArea, value.selfLocaly, value.selfGlobaly);
//            var afterField = value.world.GetField();
//            var animalCount = afterField.Cast<Animal>().Count(x => x != null);
//            var potentialChildPlaces = new List<Animal> { afterField[0, 0], afterField[2, 0], afterField[2, 1], afterField[2, 2], afterField[0, 2], afterField[1, 2] };
//            var childCount = potentialChildPlaces.Cast<Animal>().Count(x => x != null);


//            Assert.AreEqual(1, childCount);
//            Assert.AreEqual(4, animalCount);
//            Assert.IsFalse(value.selfGlobaly.Animal.possibleMates.Any());
//            Assert.AreEqual(value.selfGlobaly.Animal._currentChildrenPause, 0);
//        }

//        [TestMethod]
//        public void Mating_WhenTooYoung_DoesntChangeAnything()
//        {
//            var value = MatingSetup();
//            value.selfGlobaly.Animal._age = 0;

//            value.selfGlobaly.Animal.Mating(value.world, value.visibleArea, value.selfLocaly, value.selfGlobaly);
//            var afterField = value.world.GetField();
//            var animalCount = afterField.Cast<Animal>().Count(x => x != null);


//            Assert.IsNull(afterField[1, 1]);
//            Assert.AreEqual(3, animalCount);
//        }

//        [TestMethod]
//        public void Mating_WhenOnPause_DoesntChangeAnything()
//        {
//            var value = MatingSetup();
//            value.selfGlobaly.Animal._currentChildrenPause = 0;

//            value.selfGlobaly.Animal.Mating(value.world, value.visibleArea, value.selfLocaly, value.selfGlobaly);
//            var afterField = value.world.GetField();
//            var animalCount = afterField.Cast<Animal>().Count(x => x != null);


//            Assert.IsNull(afterField[1, 1]);
//            Assert.AreEqual(3, animalCount);
//        }

//        [TestMethod]
//        public void Mating_WhenNotEnoughtByEachOtherInOwnList_UpdatesTimeInOwnList()
//        {
//            var value = MatingSetup();
//            var secondAnimal = value.visibleArea[1, 0];
//            var thirdAnimal = value.visibleArea[0, 1];
//            value.selfGlobaly.Animal.possibleMates[secondAnimal] = 0;
//            value.selfGlobaly.Animal.possibleMates[thirdAnimal] = 0;

//            value.selfGlobaly.Animal.Mating(value.world, value.visibleArea, value.selfLocaly, value.selfGlobaly);
//            var afterField = value.world.GetField();
//            var animalCount = afterField.Cast<Animal>().Count(x => x != null);

//            Assert.IsNull(afterField[1, 1]);
//            Assert.AreEqual(3, animalCount);
//            Assert.AreEqual(value.selfGlobaly.Animal.possibleMates[secondAnimal], 1);
//            Assert.AreEqual(value.selfGlobaly.Animal.possibleMates[thirdAnimal], 1);
//        }

//        [TestMethod]
//        public void Mating_WhenNotEnoughtByEachOtherInTheirList_UpdatesTimeInOwnList()
//        {
//            var value = MatingSetup();
//            var secondAnimal = value.visibleArea[1, 0];
//            var thirdAnimal = value.visibleArea[0, 1];
//            secondAnimal.possibleMates[value.selfGlobaly.Animal] = 0;
//            thirdAnimal.possibleMates[value.selfGlobaly.Animal] = 0;

//            value.selfGlobaly.Animal.Mating(value.world, value.visibleArea, value.selfLocaly, value.selfGlobaly);
//            var afterField = value.world.GetField();
//            var animalCount = afterField.Cast<Animal>().Count(x => x != null);

//            Assert.IsNull(afterField[1, 1]);
//            Assert.AreEqual(3, animalCount);
//            Assert.AreEqual(value.selfGlobaly.Animal.possibleMates[secondAnimal], 4);
//            Assert.AreEqual(value.selfGlobaly.Animal.possibleMates[thirdAnimal], 4);
//        }

//        [TestMethod]
//        public void Mating_WhenNMateDissapearsFromRange_ClearsMateListOneByOne()
//        {

//            var value = MatingSetup();
//            var secondAnimal = value.visibleArea[1, 0];
//            var thirdAnimal = value.visibleArea[0, 1];
//            value.selfGlobaly.Animal.possibleMates[secondAnimal] = 1;
//            thirdAnimal._isAlive = false;
//            value.visibleArea[0, 1] = null;

//            value.selfGlobaly.Animal.Mating(value.world, value.visibleArea, value.selfLocaly, value.selfGlobaly);

//            Assert.AreEqual(value.selfGlobaly.Animal.possibleMates[secondAnimal], 2);
//            CollectionAssert.DoesNotContain(value.selfGlobaly.Animal.possibleMates.Keys.ToList(), thirdAnimal);

//            thirdAnimal._isAlive = true;
//            value.visibleArea[0, 1] = thirdAnimal;
//            secondAnimal._isAlive = false;
//            value.visibleArea[1, 0] = null;

//            value.selfGlobaly.Animal.Mating(value.world, value.visibleArea, value.selfLocaly, value.selfGlobaly);

//            CollectionAssert.DoesNotContain(value.selfGlobaly.Animal.possibleMates.Keys.ToList(), secondAnimal);
//            Assert.AreEqual(value.selfGlobaly.Animal.possibleMates[thirdAnimal], 1);
//        }
//        #endregion

//        #region Query Methods
//        [TestMethod]
//        public void GetTypePositionsList_WhenEmpty_ReturnsNoAnimalCoordinates()
//        {
//            var world = new World(5, 5);
//            var field = world.GetField();
//            var animal = new Antelope();

//            var coordinateList = animal.GetTypePositionsList<Antelope>(field);

//            Assert.AreEqual(0, coordinateList.Count);
//        }

//        [TestMethod]
//        public void GetTypePositionsList_WhenOnlyWrongType_ReturnsNoAnimalCoordinates()
//        {
//            var world = new World(5, 5);
//            world.AddAnimal(new Lion().CreationKey);
//            world.AddAnimal(new Lion().CreationKey);
//            var field = world.GetField();
//            var animal = new Antelope();

//            var coordinateList = animal.GetTypePositionsList<Antelope>(field);

//            Assert.AreEqual(0, coordinateList.Count);
//        }

//        [TestMethod]
//        public void GetTypePositionsList_WhenOnlyRightTypeExists_ReturnsCorrectCoordinates()
//        {
//            var world = new World(5, 5);
//            world.AddAnimal(new Antelope(), new AnimalCoordinates(0, 0));
//            world.AddAnimal(new Antelope(), new AnimalCoordinates(1, 0));
//            var field = world.GetField();
//            var added1 = new AnimalCoordinates(0, 0, field[0, 0]);
//            var added2 = new AnimalCoordinates(1, 0, field[1, 0]);
//            var animal = new Antelope();

//            var coordinateList = animal.GetTypePositionsList<Antelope>(field);

//            CollectionAssert.Contains(coordinateList, added1);
//            CollectionAssert.Contains(coordinateList, added2);
//            Assert.AreEqual(2, coordinateList.Count);
//        }

//        [TestMethod]
//        public void GetTypePositionsList_WhenRightAndWrongTypeExists_ReturnsCorrectCoordinates()
//        {
//            var world = new World(5, 5);
//            world.AddAnimal(new Antelope(), new AnimalCoordinates(0, 0));
//            world.AddAnimal(new Lion(), new AnimalCoordinates(1, 0));
//            var field = world.GetField();
//            var added1 = new AnimalCoordinates(0, 0, field[0, 0]);
//            var added2 = new AnimalCoordinates(1, 0, field[1, 0]);
//            var animal = new Antelope();

//            var coordinateList = animal.GetTypePositionsList<Antelope>(field);

//            CollectionAssert.Contains(coordinateList, added1);
//            CollectionAssert.DoesNotContain(coordinateList, added2);
//            Assert.AreEqual(1, coordinateList.Count);
//        }

//        [TestMethod]
//        public void GetMatesVisibleList_WhenEmpty_ReturnsNoAnimalCoordinates()
//        {
//            var world = new World(5, 5);
//            var field = world.GetField();
//            var animal = new Antelope();

//            var coordinateList = animal.GetMatesVisibleList(field, typeof(Antelope));

//            Assert.AreEqual(0, coordinateList.Count);
//        }

//        [TestMethod]
//        public void GetMatesVisibleList_WhenOnlyWrongType_ReturnsNoAnimalCoordinates()
//        {
//            var world = new World(5, 5);
//            world.AddAnimal(new Lion().CreationKey);
//            world.AddAnimal(new Lion().CreationKey);
//            var field = world.GetField();
//            var animal = new Antelope();

//            var coordinateList = animal.GetMatesVisibleList(field, typeof(Antelope));

//            Assert.AreEqual(0, coordinateList.Count);
//        }

//        [TestMethod]
//        public void GetMatesVisibleList_WhenOnlyRightTypeExists_ReturnsCorrectCoordinates()
//        {
//            var world = new World(5, 5);
//            world.AddAnimal(new Antelope(), new AnimalCoordinates(0, 0));
//            world.AddAnimal(new Antelope(), new AnimalCoordinates(1, 0));
//            var field = world.GetField();
//            var added1 = new AnimalCoordinates(0, 0, field[0, 0]);
//            var added2 = new AnimalCoordinates(1, 0, field[1, 0]);
//            var animal = new Antelope();

//            var coordinateList = animal.GetMatesVisibleList(field, typeof(Antelope));

//            CollectionAssert.Contains(coordinateList, added1);
//            CollectionAssert.Contains(coordinateList, added2);
//            Assert.AreEqual(2, coordinateList.Count);
//        }

//        [TestMethod]
//        public void GetMatesVisibleList_WhenRightAndWrongTypeExists_ReturnsCorrectCoordinates()
//        {
//            var world = new World(5, 5);
//            world.AddAnimal(new Antelope(), new AnimalCoordinates(0, 0));
//            world.AddAnimal(new Lion(), new AnimalCoordinates(1, 0));
//            var field = world.GetField();
//            var added1 = new AnimalCoordinates(0, 0, field[0, 0]);
//            var added2 = new AnimalCoordinates(1, 0, field[1, 0]);
//            var animal = new Antelope();

//            var coordinateList = animal.GetMatesVisibleList(field, typeof(Antelope));

//            CollectionAssert.Contains(coordinateList, added1);
//            CollectionAssert.DoesNotContain(coordinateList, added2);
//            Assert.AreEqual(1, coordinateList.Count);
//        }

//        [TestMethod]
//        public void FilterCloseEnoughMates_WhenEmpty_ReturnsNoAnimalCoordinates()
//        {
//            var animal = new Antelope();
//            var animalCoordinates = new AnimalCoordinates(0, 0, animal);
//            var mateList = new List<AnimalCoordinates>();

//            var coordinateList = animal.FilterCloseEnoughMates(mateList, animalCoordinates);

//            Assert.AreEqual(0, coordinateList.Count);
//        }

//        [TestMethod]
//        public void FilterCloseEnoughMates_WhenMatesOnlyNear_ReturnsMateCoordinates()
//        {
//            var animal = new Antelope();
//            var animalCoordinates = new AnimalCoordinates(0, 0, animal);
//            var mateList = new List<AnimalCoordinates>()
//            {
//                new AnimalCoordinates(1,0),
//                new AnimalCoordinates(0,1)
//            };

//            var coordinateList = animal.FilterCloseEnoughMates(mateList, animalCoordinates);

//            Assert.AreEqual(2, coordinateList.Count);
//        }

//        [TestMethod]
//        public void FilterCloseEnoughMates_WhenMatesOnlyFar_ReturnsNoAnimalCoordinates()
//        {
//            var animal = new Antelope();
//            var animalCoordinates = new AnimalCoordinates(0, 0, animal);
//            var mateList = new List<AnimalCoordinates>()
//            {
//                new AnimalCoordinates(animal.ReproductionRange+1,0),
//                new AnimalCoordinates(0,animal.ReproductionRange+1)
//            };

//            var coordinateList = animal.FilterCloseEnoughMates(mateList, animalCoordinates);

//            Assert.AreEqual(0, coordinateList.Count);
//        }

//        [TestMethod]
//        public void FilterCloseEnoughMates_WhenMatesBothCloseAndFar_ReturnsOnlyCorrectCoordinates()
//        {
//            var animal = new Antelope();
//            var animalCoordinates = new AnimalCoordinates(0, 0, animal);
//            var close = new AnimalCoordinates(1, 1);
//            var mateList = new List<AnimalCoordinates>()
//            {
//                new AnimalCoordinates(animal.ReproductionRange+1,0),
//                new AnimalCoordinates(0,animal.ReproductionRange+1),
//                close
//            };

//            var coordinateList = animal.FilterCloseEnoughMates(mateList, animalCoordinates);

//            Assert.AreEqual(1, coordinateList.Count);
//            CollectionAssert.Contains(coordinateList, close);
//        }

//        [TestMethod]
//        public void DistanceToCalculator_WhenDistanceIsRowDifferenceNegative_ReturnsRowDifference()
//        {
//            var animal = new Antelope();
//            var from = new AnimalCoordinates(50, 50, animal);
//            var to = new AnimalCoordinates(56, 51);
//            var diff = 6;

//            var distance = animal.DistanceToCalculator(from, to);

//            Assert.AreEqual(distance, diff);
//        }

//        [TestMethod]
//        public void DistanceToCalculator_WhenDistanceIsColumnDifferenceNegative_ReturnsRowDifference()
//        {
//            var animal = new Antelope();
//            var from = new AnimalCoordinates(50, 50, animal);
//            var to = new AnimalCoordinates(51, 56);
//            var diff = 6;

//            var distance = animal.DistanceToCalculator(from, to);

//            Assert.AreEqual(distance, diff);
//        }

//        [TestMethod]
//        public void DistanceToCalculator_WhenDistanceIsRowDifferencePositive_ReturnsRowDifference()
//        {
//            var animal = new Antelope();
//            var from = new AnimalCoordinates(50, 50, animal);
//            var to = new AnimalCoordinates(43, 49);
//            var diff = 7;

//            var distance = animal.DistanceToCalculator(from, to);

//            Assert.AreEqual(distance, diff);
//        }

//        [TestMethod]
//        public void DistanceToCalculator_WhenDistanceIsColumnDifferencePositive_ReturnsRowDifference()
//        {
//            var animal = new Antelope();
//            var from = new AnimalCoordinates(50, 50, animal);
//            var to = new AnimalCoordinates(49, 43);
//            var diff = 7;

//            var distance = animal.DistanceToCalculator(from, to);

//            Assert.AreEqual(distance, diff);
//        }

//        [TestMethod]
//        public void IsDecomposed_WhenAlive_ReturnsFalse()
//        {
//            var animal = new Antelope();

//            var value = animal.IsDecomposed();

//            Assert.IsFalse(value);
//        }

//        [TestMethod]
//        public void IsDecomposed_WhenDeadButNotDecomposed_ReturnsFalse()
//        {
//            var animal = new Antelope();
//            animal._isAlive = false;

//            var value = animal.IsDecomposed();

//            Assert.IsFalse(value);
//        }

//        [TestMethod]
//        public void IsDecomposed_WhenDeadAndDecomposed_ReturnsTrue()
//        {
//            var animal = new Antelope();
//            animal._isAlive = false;
//            animal._roundsDead = int.MaxValue;

//            var value = animal.IsDecomposed();

//            Assert.IsTrue(value);
//        }

//        [TestMethod]
//        public void HaveEnoughStamina_WhenJustEnough_ReturnsTrue()
//        {
//            var animal = new Antelope();

//            var value = animal.HaveEnoughStamina(10, -10);

//            Assert.IsTrue(value);
//        }

//        [TestMethod]
//        public void HaveEnoughStamina_WhenEnough_ReturnsTrue()
//        {
//            var animal = new Antelope();

//            var value = animal.HaveEnoughStamina(100, -10);

//            Assert.IsTrue(value);
//        }

//        [TestMethod]
//        public void HaveEnoughStamina_WhenJustNotEnough_ReturnsTrue()
//        {
//            var animal = new Antelope();

//            var value = animal.HaveEnoughStamina(9, -10);

//            Assert.IsFalse(value);
//        }

//        [TestMethod]
//        public void HaveEnoughStamina_WhenNotEnough_ReturnsTrue()
//        {
//            var animal = new Antelope();

//            var value = animal.HaveEnoughStamina(0, -10);

//            Assert.IsFalse(value);
//        }

//        [TestMethod]
//        public void IsStatAboveMax_WhenJustAboveMax_ReturnsTrue()
//        {
//            var animal = new Antelope();

//            var value = animal.IsStatAboveMax(10, 9, 1.1);

//            Assert.IsTrue(value);
//        }

//        [TestMethod]
//        public void IsStatAboveMax_WhenAboveMax_ReturnsTrue()
//        {
//            var animal = new Antelope();

//            var value = animal.IsStatAboveMax(10, 9, 10);

//            Assert.IsTrue(value);
//        }

//        [TestMethod]
//        public void IsStatAboveMax_WhenJustNotAbove_ReturnsTrue()
//        {
//            var animal = new Antelope();

//            var value = animal.IsStatAboveMax(10, 9, 1);

//            Assert.IsFalse(value);
//        }

//        [TestMethod]
//        public void IsStatAboveMax_WhenNotAbove_ReturnsTrue()
//        {
//            var animal = new Antelope();

//            var value = animal.IsStatAboveMax(10, 5, 3.5);

//            Assert.IsFalse(value);
//        }
//        #endregion
//    }
//}

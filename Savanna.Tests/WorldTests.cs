using Savanna.Logic;

namespace Savanna.Tests
{
    [TestClass]
    public sealed class WorldTests
    {
        private static PluginManager pluginManager = new PluginManager(Directory.GetCurrentDirectory(), path => new[] { "oneFile.dll" }, file => typeof(TestAnimal).Assembly);
        private static AnimalFactory _animalFactory = new AnimalFactory(pluginManager);
        #region Core World Methods
        [TestMethod]
        public void NextTurn_WhenNoAnimalExists_DoesntCrash()
        {
            var world = new World(_animalFactory);

            world.NextTurn();

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void NextTurn_WhenAnimalsExist_DoesntCrash()
        {
            var world = new World(_animalFactory);
            world.AddAnimal(new TestAnimal().CreationKey);
            world.AddAnimal(new TestAnimal().CreationKey);
            world.AddAnimal(new TestAnimal().CreationKey);

            world.NextTurn();

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void NextTurn_WhenDecomposedAnimalExists_DecomposedAnimalRemoved()
        {
            var world = new World(_animalFactory);
            var animal = new TestAnimal();
            animal.MakeDecomposed();
            world.AddAnimal(animal, new AnimalCoordinates(1, 0));

            world.NextTurn();

            Assert.IsNull(world.GetField()[1, 0]);
        }

        [TestMethod]
        public void AddAnimal_WhenJustKeyGivenAndEmptyBoard_AddsSingleAnimalToBoard()
        {
            var world = new World(_animalFactory);

            world.AddAnimal(new TestAnimal().CreationKey);
            var animalCount = world.GetField().Cast<Animal>().Count(x => x != null);

            Assert.AreEqual(1, animalCount);
        }

        [TestMethod]
        public void AddAnimal_WhenJustKeyGivenAndTooManyAnimalsAdded_AddsAnimalsUpToArrayLimit()
        {
            var world = new World(_animalFactory,10, 11);

            for (int i = 0; i < 115; i++)
            {
                world.AddAnimal(new TestAnimal().CreationKey);
            }
            var animalCount = world.GetField().Cast<Animal>().Count(x => x != null);
            var nullCount = world.GetField().Cast<Animal>().Count(x => x == null);

            Assert.AreEqual(110, animalCount);
            Assert.AreEqual(0, nullCount);
        }

        [TestMethod]
        public void AddAnimal_WhenSpecificFieldChosen_AddsAnimalToSpecifiedField()
        {
            var world = new World(_animalFactory,10, 15);

            world.AddAnimal(new TestAnimal(), new AnimalCoordinates(9, 14));
            var animalCount = world.GetField().Cast<Animal>().Count(x => x != null);
            var field = world.GetField();

            Assert.AreEqual(1, animalCount);
            Assert.IsInstanceOfType<Animal>(field[9, 14]);
        }

        [TestMethod]
        public void AddAnimal_WhenSpecificFieldFunctionWithMissingPlace_DoesntChangeAnything()
        {
            var world = new World(_animalFactory, 10, 15);
            world.AddAnimal(new TestAnimal().CreationKey);
            world.AddAnimal(new TestAnimal().CreationKey);
            var beforeField = world.GetField();

            world.AddAnimal(new TestAnimal(), null);
            var afterAnimalCount = world.GetField().Cast<Animal>().Count(x => x != null);
            var afterField = world.GetField();

            Assert.AreEqual(2, afterAnimalCount);
            CollectionAssert.AreEqual(beforeField, afterField);
        }

        [TestMethod]
        public void MoveAnimal_WhenCalled_MovesAnimalToNewPosition()
        {
            var world = new World(_animalFactory, 10, 15);
            world.AddAnimal(new TestAnimal(), new AnimalCoordinates(8, 14));
            var field = world.GetField();

            world.MoveAnimal(new AnimalCoordinates(8, 14, field[8, 14]), Direction.South);

            Assert.IsNull(field[8, 14]);
            Assert.IsInstanceOfType<Animal>(field[9, 14]);
        }
        #endregion

        #region Query Methods
        [TestMethod]
        public void GetVisibleArea_WhenInCenter_ReturnFullAreaInAnimalVision()
        {
            var world = new World(_animalFactory, 100,90);
            var testAnimal = new TestAnimal();
            world.AddAnimal(testAnimal, new AnimalCoordinates(50, 50));
            testAnimal.UpdateVision(3);

            var smallVision = world.GetVisibleArea(50, 50);
            testAnimal.UpdateVision(7);
            var bigVision = world.GetVisibleArea(50, 50);

            Assert.AreEqual(7, smallVision.visibleArea.GetLength(0));
            Assert.AreEqual(7, smallVision.visibleArea.GetLength(1));
            Assert.AreEqual(15, bigVision.visibleArea.GetLength(0));
            Assert.AreEqual(15, bigVision.visibleArea.GetLength(1));
        }

        [TestMethod]
        public void GetVisibleArea_WhenInCorner_ReturnAreaCorppedByCorner()
        {
            var world = new World(_animalFactory, 20,30);
            var animal = new TestAnimal();
            world.AddAnimal(animal, new AnimalCoordinates(0, 0));
            animal.UpdateVision(5);

            var result = world.GetVisibleArea(0, 0);

            Assert.AreEqual(6, result.visibleArea.GetLength(0));
            Assert.AreEqual(6, result.visibleArea.GetLength(1));
        }

        [TestMethod]
        public void GetVisibleArea_WhenNearWall_ReturnAreaCroppedByWall()
        {
            var world = new World(_animalFactory, 100, 90);
            var animal = new TestAnimal();
            world.AddAnimal(animal, new AnimalCoordinates(50, 0));
            animal.UpdateVision(5);

            var result = world.GetVisibleArea(50, 0);

            Assert.AreEqual(11, result.visibleArea.GetLength(0));
            Assert.AreEqual(6, result.visibleArea.GetLength(1));
        }

        [TestMethod]
        public void GetVisibleArea_WhenAlmostCorner_ReturnAreaCroppedByBordersIfNeeded()
        {
            var world = new World(_animalFactory, 20,30);
            var animal = new TestAnimal();
            world.AddAnimal(animal, new AnimalCoordinates(2, 3));
            animal.UpdateVision(5);

            var resultBigRange = world.GetVisibleArea(2, 3);
            animal.UpdateVision(2);
            var resultSmallRange = world.GetVisibleArea(2, 3);

            Assert.AreEqual(8, resultBigRange.visibleArea.GetLength(0));
            Assert.AreEqual(9, resultBigRange.visibleArea.GetLength(1));
            Assert.AreEqual(5, resultSmallRange.visibleArea.GetLength(0));
            Assert.AreEqual(5, resultSmallRange.visibleArea.GetLength(1));
        }

        [TestMethod]
        public void GetVisibleArea_WhenAnimalIsInArea_ReturnAreaWithSameAnimal()
        {
            var world = new World(_animalFactory, 20,30);
            var animal = new TestAnimal();
            animal.UpdateVision(5);
            world.AddAnimal(animal, new AnimalCoordinates(10, 15));
            world.AddAnimal(new TestAnimal(), new AnimalCoordinates(11,17));

            var result = world.GetVisibleArea(10, 15);
            var fullGrid = world.GetField();

            Assert.AreEqual(fullGrid[11, 17], result.visibleArea[result.self.Row + 1, result.self.Column+2]);
            CollectionAssert.IsSubsetOf(result.visibleArea, fullGrid);
        }

        [TestMethod]
        public void GetAnimalCardDTOByPosition_WhenAnimalExists_ReturnsAnimalCardDTO()
        {
            var world = new World(_animalFactory);
            var animal = new TestAnimal();
            world.AddAnimal(animal, new AnimalCoordinates(5, 0));

            var animalDTO = world.GetAnimalCardDTOByPosition(5, 0);

            Assert.IsNotNull(animalDTO);
            Assert.AreEqual(animal.ID, animalDTO.ID);
            Assert.AreEqual("Test", animalDTO.Name);
        }

        [TestMethod]
        public void GetAnimalCardDTOByPosition_WhenAnimalDoesntExist_ReturnsNull()
        {
            var world = new World(_animalFactory);

            var animalDTO = world.GetAnimalCardDTOByPosition(5, 0);

            Assert.IsNull(animalDTO);
        }

        [TestMethod]
        public void GetAnimalPositionByID_WhenAnimalExists_ReturnsAnimalPosition()
        {
            var world = new World(_animalFactory);
            var animal = new TestAnimal();
            world.AddAnimal(animal, new AnimalCoordinates(5, 0));

            var coordinates = world.GetAnimalPositionByID(animal.ID);

            Assert.IsNotNull(coordinates.Item1);
            Assert.IsNotNull(coordinates.Item2);
            Assert.AreEqual(coordinates.Item1, 5);
            Assert.AreEqual(coordinates.Item2, 0);
        }

        [TestMethod]
        public void GetAnimalPositionByID_WhenAnimalDoesntExist_ReturnsNullNullPosition()
        {
            var world = new World(_animalFactory);

            var coordinates = world.GetAnimalPositionByID(0);

            Assert.IsNull(coordinates.Item1);
            Assert.IsNull(coordinates.Item2);
        }

        [TestMethod]
        public void GetAnimalPositionByID_WhenAnimalDoesntExistInThatWorld_ReturnsNullNullPosition()
        {
            var world = new World(_animalFactory);
            var anotherWorld = new World(_animalFactory);
            var animal = new TestAnimal();
            anotherWorld.AddAnimal(animal, new AnimalCoordinates(10, 0));

            var coordinates = world.GetAnimalPositionByID(animal.ID);

            Assert.IsNull(coordinates.Item1);
            Assert.IsNull(coordinates.Item2);
        }

        [TestMethod]
        public void WorldToDisplayDTO_WhenUsed_ReturnsTheWorldRepresenatation()
        {
            var world = new World(_animalFactory,7,9);
            world.NextTurn();
            world.AddAnimal(new TestAnimal(), new AnimalCoordinates(0,1));
            world.AddAnimal(new TestAnimal(), new AnimalCoordinates(0,5));

            var representation = world.WorldToDisplayDTO();

            Assert.IsNotNull(representation);
            Assert.AreEqual(7, representation.Height);
            Assert.AreEqual(9, representation.Width);
            Assert.AreEqual(1, representation.Iteration);
            Assert.AreEqual(2, representation.AnimalsInWorld);
            Assert.AreEqual(1, representation.AnimalsAvailable.Length);
            Assert.IsNotNull(representation.AnimalField[0][1]);
            Assert.AreEqual('T', representation.AnimalField[0][1].DisplayChar);
            Assert.AreEqual("👽", representation.AnimalField[0][1].DisplayEmoji);
            Assert.IsTrue(representation.AnimalField[0][1].IsAlive);
            Assert.IsNotNull(representation.AnimalField[0][5]);
            Assert.IsNull(representation.AnimalField[0][0]);
        }

        [TestMethod]
        public void WorldToSaveDTO_WhenUsed_ReturnsTheWorldSaveData()
        {
            var world = new World(_animalFactory, 7, 9);
            world.NextTurn();
            var animal = new TestAnimal();
            world.AddAnimal(animal, new AnimalCoordinates(0, 1));
            world.AddAnimal(new TestAnimal(), new AnimalCoordinates(0, 8));
            animal.Damage(10);
            animal.TestChangeStamina(-10);
            animal.TestAge = 6.7;
            animal.TestCurrentChildrenPause = 5.6;
            animal.TestPossibleMates = new Dictionary<int, int> { { 1, 3 } };

            var representation = world.WorldToSaveDTO();

            Assert.IsNotNull(representation);
            Assert.AreEqual(7, representation.Height);
            Assert.AreEqual(9, representation.Width);
            Assert.AreEqual(1, representation.Iteration);
            Assert.AreEqual(2, representation.AnimalsInWorld);
            Assert.IsNotNull(representation.Field[0][1]);
            Assert.AreEqual(40, representation.Field[0][1].Health);
            Assert.AreEqual(65, representation.Field[0][1].Stamina);
            Assert.AreEqual(6.7, representation.Field[0][1].Age);
            Assert.AreEqual(5.6, representation.Field[0][1].CurrentChildrenPause);
            Assert.AreEqual(animal.ID, representation.Field[0][1].Id);
            Assert.AreEqual(0, representation.Field[0][1].Offsprings);
            Assert.AreEqual(0, representation.Field[0][1].RoundsDead);
            Assert.IsTrue(representation.Field[0][1].IsAlive);
            CollectionAssert.AreEquivalent(new Dictionary<int, int> { { 1,3 } }, representation.Field[0][1].PossibleMates);
            Assert.IsNotNull(representation.Field[0][8]);
            Assert.IsNull(representation.Field[0][0]);
            Assert.AreEqual(2, representation.Field.SelectMany(row => row).Count(x => x != null));
        }

        [TestMethod]
        public void SetUpWorldFromSaveDTO_WhenUsed_ReturnsRecreatedWorld()
        {
            var world = new World(_animalFactory, 7, 9);
            world.NextTurn();
            var animal = new TestAnimal();
            world.AddAnimal(animal, new AnimalCoordinates(0, 1));
            world.AddAnimal(new TestAnimal(), new AnimalCoordinates(0, 8));
            animal.Damage(10);
            animal.TestChangeStamina(-10);
            animal.TestAge = 6.7;
            animal.TestCurrentChildrenPause = 5.6;
            animal.TestPossibleMates = new Dictionary<int, int> { { 1, 3 } };

            var worldDTO = world.WorldToSaveDTO();
            var recreatedWorld = new World(_animalFactory);
            recreatedWorld.SetUpWorldFromSaveDTO(worldDTO);
            var field = recreatedWorld.GetField();

            Assert.IsNotNull(recreatedWorld);
            Assert.AreEqual(7, recreatedWorld.Height);
            Assert.AreEqual(9, recreatedWorld.Width);
            Assert.AreEqual(2, field.Cast<Animal>().Count(x => x != null));
            Assert.IsNotNull(field[0,1]);
            Assert.AreEqual(40, field[0,1].Health);
            Assert.AreEqual(65, field[0,1].Stamina);
            Assert.AreEqual(animal.ID, field[0,1].ID);
            Assert.IsTrue(field[0,1].IsAlive());
            Assert.IsNotNull(field[0, 8]);
            Assert.IsNull(field[0, 0]);
        }
        #endregion
    }
}

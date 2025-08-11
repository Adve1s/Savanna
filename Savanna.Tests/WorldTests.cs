using Savanna.Logic;

namespace Savanna.Tests
{
    [TestClass]
    public sealed class WorldTests
    {
        private static AnimalFactory _animalFactory = new AnimalFactory();
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
        #endregion
    }
}

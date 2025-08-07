using Savanna.Logic;

namespace Savanna.Tests
{
    [TestClass]
    public sealed class WorldTests
    {
        private World Create99x99WorldWithAnimal(char creationKey, (int row, int column) mainAnimal)
        {
            var testWorld = new World(99, 99);
            testWorld.AddAnimal(testWorld.AnimalFactory.CreateAnimal(creationKey),new AnimalCoordinates(mainAnimal.row,mainAnimal.column));
            return testWorld;

        }

        #region Core World Methods
        [TestMethod]
        public void NextTurn_WhenNoAnimalExists_DoesntCrash()
        {
            var world = new World();

            world.NextTurn();

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void NextTurn_WhenAnimalsExist_DoesntCrash()
        {
            var world = new World();
            world.AddAnimal(new Antelope().CreationKey);
            world.AddAnimal(new Antelope().CreationKey);
            world.AddAnimal(new Lion().CreationKey);

            world.NextTurn();

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void NextTurn_WhenDecomposedAnimalExists_DecomposedAnimalRemoved()
        {
            var world = new World();
            world.AddAnimal(new Antelope(),new AnimalCoordinates(1,0));
            var field = world.GetField();
            //field[1, 0]._isAlive = false;
            //field[1, 0]._roundsDead = int.MaxValue;

            world.NextTurn();

            Assert.IsNull(field[1, 0]);
        }

        [TestMethod]
        public void AddAnimal_WhenJustKeyGivenAndEmptyBoard_AddsSingleAnimalToBoard ()
        {
            var world = new World();

            world.AddAnimal(new Antelope().CreationKey);
            var animalCount = world.GetField().Cast<Animal>().Count(x => x != null);

            Assert.AreEqual(1, animalCount);
        }

        [TestMethod]
        public void AddAnimal_WhenJustKeyGivenAndTooManyAnimalsAdded_AddsAnimalsUpToArrayLimit ()
        {
            var world = new World(10,11);

            for (int i = 0; i < 115; i++)
            {
                world.AddAnimal(new Antelope().CreationKey);
            }
            var animalCount = world.GetField().Cast<Animal>().Count(x => x != null);
            var nullCount = world.GetField().Cast<Animal>().Count(x => x == null);

            Assert.AreEqual(110, animalCount);
            Assert.AreEqual(0, nullCount);
        }

        [TestMethod]
        public void AddAnimal_WhenSpecificFieldChosen_AddsAnimalToSpecifiedField ()
        {
            var world = new World(10,15);

            world.AddAnimal(new Antelope(), new AnimalCoordinates(9, 14));
            var animalCount = world.GetField().Cast<Animal>().Count(x => x != null);
            var field = world.GetField();

            Assert.AreEqual(1, animalCount);
            Assert.IsInstanceOfType<Animal>(field[9,14]);
        }

        [TestMethod]
        public void AddAnimal_WhenSpecificFieldFunctionWithMissingPlace_DoesntChangeAnything()
        {
            var world = new World(10, 10);
            world.AddAnimal(new Antelope().CreationKey);
            world.AddAnimal(new Antelope().CreationKey);
            var beforeField = world.GetField();

            world.AddAnimal(new Antelope(),null);
            var afterAnimalCount = world.GetField().Cast<Animal>().Count(x => x != null);
            var afterField = world.GetField();

            Assert.AreEqual(2,afterAnimalCount);
            CollectionAssert.AreEqual(beforeField,afterField);
        }

        [TestMethod]
        public void MoveAnimal_WhenCalled_MovesAnimalToNewPosition()
        {
            var world = new World(10, 15);
            world.AddAnimal(new Antelope(), new AnimalCoordinates(8, 14));
            var field = world.GetField();

            world.MoveAnimal(new AnimalCoordinates(8, 14, field[8, 14]), Direction.South);

            Assert.IsNull(field[8,14]);
            Assert.IsInstanceOfType<Animal>(field[9, 14]);
        }
        #endregion

        #region Query Methods
        [TestMethod]
        public void GetVisibleArea_WhenInCenter_ReturnFullAreaInAnimalVision()
        {
            var antelope = new Antelope();
            var antelopeWorld = Create99x99WorldWithAnimal(antelope.CreationKey, (50, 50));
            var antelopeVisionRange = antelope.Vision * 2 + 1;
            var lion = new Lion();
            var lionWorld = Create99x99WorldWithAnimal(lion.CreationKey, (50, 50));
            var lionVisionRange = lion.Vision * 2 + 1;

            var antelopeResult = antelopeWorld.GetVisibleArea(50, 50);
            var lionResult = lionWorld.GetVisibleArea(50, 50);

            Assert.AreEqual(antelopeVisionRange, antelopeResult.visibleArea.GetLength(0));
            Assert.AreEqual(antelopeVisionRange, antelopeResult.visibleArea.GetLength(1));
            Assert.AreEqual(lionVisionRange, lionResult.visibleArea.GetLength(0));
            Assert.AreEqual(lionVisionRange, lionResult.visibleArea.GetLength(1));
        }

        [TestMethod]
        public void GetVisibleArea_WhenInCorner_ReturnAreaCorppedByCorner()
        {
            var animal = new Antelope();
            var testWorld = Create99x99WorldWithAnimal(animal.CreationKey, (0, 0));
            var animalVisionRange = animal.Vision + 1;

            var result = testWorld.GetVisibleArea(0, 0);

            Assert.AreEqual(animalVisionRange, result.visibleArea.GetLength(0));
            Assert.AreEqual(animalVisionRange, result.visibleArea.GetLength(1));
        }

        [TestMethod]
        public void GetVisibleArea_WhenNearWall_ReturnAreaCroppedByWall()
        {
            var animal = new Antelope();
            var testWorld = Create99x99WorldWithAnimal(animal.CreationKey, (50, 0));
            var animalVisionHeight = animal.Vision * 2 + 1;
            var animalVisionWidth = animal.Vision + 1;

            var result = testWorld.GetVisibleArea(50, 0);

            Assert.AreEqual(animalVisionHeight, result.visibleArea.GetLength(0));
            Assert.AreEqual(animalVisionWidth, result.visibleArea.GetLength(1));
        }

        [TestMethod]
        public void GetVisibleArea_WhenAlmostCorner_ReturnAreaCroppedByBordersIfNeeded()
        {
            var animal = new Antelope();
            var testWorld = Create99x99WorldWithAnimal(animal.CreationKey, (3, 3));
            var animalVisionRange = 0;
            if (animal.Vision > 3)
            {
                animalVisionRange = animal.Vision + 3 + 1;
            }
            else
            {
                animalVisionRange = animal.Vision * 2 + 1;
            }

            var result = testWorld.GetVisibleArea(3, 3);

            Assert.AreEqual(animalVisionRange, result.visibleArea.GetLength(0));
            Assert.AreEqual(animalVisionRange, result.visibleArea.GetLength(1));
        }

        [TestMethod]
        public void GetVisibleArea_WhenAnimalIsInArea_ReturnAreaWithSameAnimal()
        {
            var animal = new Antelope();
            var testWorld = Create99x99WorldWithAnimal(animal.CreationKey, (50, 50));
            testWorld.AddAnimal(testWorld.AnimalFactory.CreateAnimal(animal.CreationKey), new AnimalCoordinates(51, 50));
            var fullGrid = testWorld.GetField();

            var result = testWorld.GetVisibleArea(50, 50);

            Assert.AreEqual(fullGrid[51, 50], result.visibleArea[result.self.Row+1, result.self.Column]);
            CollectionAssert.IsSubsetOf(result.visibleArea, fullGrid);
        }
        #endregion
    }
}

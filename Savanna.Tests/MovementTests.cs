using Savanna.Logic;

namespace Savanna.Tests
{
    [TestClass]
    public sealed class MovementTests
    {
        private (Animal[,] field, AnimalCoordinates animal) SetuUp3x3FieldWithAnimals((int row, int column) mainCoords, List<(int row, int column)>? fillers = null)
        {
            //  3x3 layout reference
            //  (0,0) (0,1) (0,2)
            //  (1,0) (1,1) (1,2)
            //  (2,0) (2,1) (2,2)
            var field = new Animal[3, 3];
            if (fillers != null)
                foreach (var coords in fillers)
                {
                    field[coords.row, coords.column] = new Antelope();
                }
            var mainAnimal = new Antelope();
            field[mainCoords.row, mainCoords.column] = mainAnimal;
            var coordinates = new AnimalCoordinates(mainCoords.row, mainCoords.column, mainAnimal);
            return (field, coordinates);
        }

        [TestMethod]
        public void IsDirectionValid_WhenDirectionClear_ReturnsTrue()
        {
            (var field, var coordinates) = SetuUp3x3FieldWithAnimals((1, 1));

            var result = Movement.IsDirectionValid(Direction.North, field, coordinates);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsDirectionValid_WhenAnimalBlocked_ReturnsFalse()
        {
            (var field, var coordinates) = SetuUp3x3FieldWithAnimals((1, 1), [(0, 1)]);

            var result = Movement.IsDirectionValid(Direction.North, field, coordinates);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsDirectionValid_WhenWallBlocked_ReturnsFalse()
        {
            (var field, var coordinates) = SetuUp3x3FieldWithAnimals((0, 1));

            var result = Movement.IsDirectionValid(Direction.North, field, coordinates);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetValidDirections_WhenAllClear_ReturnsAll8dDirections()
        {
            (var field, var coordinates) = SetuUp3x3FieldWithAnimals((1, 1));

            var directions = Movement.GetValidDirections(field, coordinates);

            Assert.AreEqual(8, directions.Count);
        }

        [TestMethod]
        public void GetValidDirections_WhenNearWall_Returns5Directions()
        {
            (var field, var coordinates) = SetuUp3x3FieldWithAnimals((0, 1));

            var directions = Movement.GetValidDirections(field, coordinates);

            Assert.AreEqual(5, directions.Count);
        }

        [TestMethod]
        public void GetValidDirections_WhenInCorner_ReturnsCorrect3Directions()
        {
            (var field, var coordinates) = SetuUp3x3FieldWithAnimals((0, 0));

            var directions = Movement.GetValidDirections(field, coordinates);

            Assert.AreEqual(3, directions.Count);
            CollectionAssert.Contains(directions, Direction.South);
            CollectionAssert.Contains(directions, Direction.SouthEast);
            CollectionAssert.Contains(directions, Direction.East);
        }

        [TestMethod]
        public void GetValidDirections_WhenAllBlocked_Returns0Directions()
        {
            (var field, var coordinates) = SetuUp3x3FieldWithAnimals((1, 1), [(0, 0), (0, 1), (0, 2), (1, 0), (1, 2), (2, 0), (2, 1), (2, 2)]);

            var directions = Movement.GetValidDirections(field, coordinates);

            Assert.AreEqual(0, directions.Count);
        }

        [TestMethod]
        public void GetValidDirections_WhenOneDirectionOpen_ReturnsCorrect1Direction()
        {
            (var field, var coordinates) = SetuUp3x3FieldWithAnimals((1, 1), [(0, 1), (0, 2), (1, 0), (1, 2), (2, 0), (2, 1), (2, 2)]);

            var directions = Movement.GetValidDirections(field, coordinates);

            Assert.AreEqual(1, directions.Count);
            CollectionAssert.Contains(directions, Direction.NorthWest);
        }

        [TestMethod]
        public void GetValidDirections_WhenOneDirectionBlocked_ReturnsCorrect7Directions()
        {
            (var field, var coordinates) = SetuUp3x3FieldWithAnimals((1, 1), [(0, 0)]);

            var directions = Movement.GetValidDirections(field, coordinates);

            Assert.AreEqual(7, directions.Count);
            CollectionAssert.DoesNotContain(directions, Direction.NorthWest);
        }

        [TestMethod]
        public void GetValidDirections_WhenInCornerAndOneBlocked_ReturnsCorrect2Directions()
        {
            (var field, var coordinates) = SetuUp3x3FieldWithAnimals((0, 0), [(1, 0)]);

            var directions = Movement.GetValidDirections(field, coordinates);

            Assert.AreEqual(2, directions.Count);
            CollectionAssert.Contains(directions, Direction.SouthEast);
            CollectionAssert.Contains(directions, Direction.East);
        }
    }
}

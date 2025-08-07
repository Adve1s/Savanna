//using Savanna.Logic;

//namespace Savanna.Tests
//{
//    [TestClass]
//    public sealed class AntelopeTests
//    {

//        [TestMethod]
//        public void DoAction_WhenNormal_DoesRandomAction()
//        {
//            var world = new World(10, 10);
//            var antelope = new Antelope();
//            antelope.Stamina -= 1;
//            var startingStamina = antelope.Stamina;
//            world.AddAnimal(antelope, new AnimalCoordinates(0, 0));
//            var surroundings = world.GetVisibleArea(0, 0);
//            var antelopeCoordinates = new AnimalCoordinates(0, 0, antelope);

//            antelope.DoAction(world, surroundings.visibleArea, surroundings.self, antelopeCoordinates);

//            Assert.AreNotEqual(startingStamina, antelope.Stamina);
//        }

//        [TestMethod]
//        public void DoAction_WhenTired_Sleeps()
//        {
//            var world = new World(10, 10);
//            var antelope = new Antelope();
//            antelope.Stamina = 1;
//            var startingStamina = antelope.Stamina;
//            world.AddAnimal(antelope, new AnimalCoordinates(0, 0));
//            var surroundings = world.GetVisibleArea(0, 0);
//            var antelopeCoordinates = new AnimalCoordinates(0, 0, antelope);
//            var restComparisonLion = new Antelope();
//            restComparisonLion.Stamina = 1;
//            restComparisonLion.Rest();

//            antelope.DoAction(world, surroundings.visibleArea, surroundings.self, antelopeCoordinates);

//            Assert.IsTrue(startingStamina < antelope.Stamina);
//            Assert.IsTrue(restComparisonLion.Stamina < antelope.Stamina);
//        }

//        [TestMethod]
//        public void DoAction_WhenHungry_EatsGrass()
//        {
//            var world = new World(10, 10);
//            var antelope = new Antelope();
//            antelope.Damage(antelope.Health - 1);
//            var startStamina = antelope.Stamina;
//            world.AddAnimal(antelope, new AnimalCoordinates(0, 0));
//            var surroundings = world.GetVisibleArea(0, 0);
//            var antelopeCoordinates = new AnimalCoordinates(0, 0, antelope);

//            antelope.DoAction(world, surroundings.visibleArea, surroundings.self, antelopeCoordinates);

//            Assert.IsTrue(antelope.Health > 1);
//            Assert.IsTrue(antelope.Stamina < startStamina);
//        }

//        [TestMethod]
//        public void DoAction_WhenSeeLion_MovesAwayFromIt()
//        {
//            var world = new World(10, 10);
//            var antelope = new Antelope();
//            world.AddAnimal(antelope, new AnimalCoordinates(0, 2));
//            world.AddAnimal(new Lion(), new AnimalCoordinates(0, 0));
//            var surroundings = new Animal[5, 5];
//            surroundings[0, 2] = antelope;
//            surroundings[0, 0] = new Lion();
//            var antelopeCoordinates = new AnimalCoordinates(0, 2, antelope);

//            antelope.DoAction(world, surroundings, antelopeCoordinates, antelopeCoordinates);
//            var afterField = world.GetField();

//            Assert.IsNull(afterField[0, 2]);
//            Assert.IsTrue(afterField[0, 3] == antelope || afterField[1, 3] == antelope);
//        }

//        [TestMethod]
//        public void DecideMoveDirection_WhenNoDirectionIsAvailable_ReturnsNull()
//        {
//            var surroundings = new Animal[1, 1];
//            var antelope = new Antelope();

//            var value = antelope.DecideMoveDirection(new AnimalCoordinates(0, 0, antelope), surroundings);

//            Assert.IsNull(value);
//        }

//        [TestMethod]
//        public void DecideMoveDirection_WhenNoLions_ReturnsRandomDirection()
//        {
//            var surroundings = new Animal[3, 3];
//            var antelope = new Antelope();

//            var value = antelope.DecideMoveDirection(new AnimalCoordinates(1, 1, antelope), surroundings);

//            Assert.IsNotNull(value);
//        }

//        [TestMethod]
//        public void DecideMoveDirection_WhenLionsGivenButEmpty_ReturnsRandomDirection()
//        {
//            var surroundings = new Animal[3, 3];
//            var antelope = new Antelope();
//            var lions = new List<AnimalCoordinates>();

//            var value = antelope.DecideMoveDirection(new AnimalCoordinates(1, 1, antelope), surroundings, lions);

//            Assert.IsNotNull(value);
//        }

//        [TestMethod]
//        public void DecideMoveDirection_WhenLionsGiven_ReturnsCorrectDirection()
//        {
//            var surroundings = new Animal[5, 5];
//            var antelope = new Antelope();
//            var lions = new List<AnimalCoordinates> { new AnimalCoordinates(0, 0, new Lion()) };

//            var value = antelope.DecideMoveDirection(new AnimalCoordinates(0, 2, antelope), surroundings, lions);

//            Assert.IsTrue((value.Equals(Direction.East) || value.Equals(Direction.SouthEast)));

//        }

//        [TestMethod]
//        public void GetClosestLion_WhenMultipleExistOneCorrect_ReturnsCorrectCoordinate()
//        {
//            var antelope = new Antelope();
//            var antelopeCoordinates = new AnimalCoordinates(1, 1, antelope);
//            var closestCoordinate = new AnimalCoordinates(3, 3, new Lion());
//            var lionList = new List<AnimalCoordinates>
//            {
//                closestCoordinate,
//                new AnimalCoordinates(9,3,new Lion()),
//                new AnimalCoordinates(4,8,new Lion()),
//            };

//            var value = antelope.GetClosestLion(antelopeCoordinates, lionList);

//            Assert.AreEqual(value, closestCoordinate);
//        }

//        [TestMethod]
//        public void GetClosestLion_WhenMultipleExistMultipleCorrect_ReturnsOneOfCorrectCoordinate()
//        {
//            var antelope = new Antelope();
//            var antelopeCoordinates = new AnimalCoordinates(1, 1, antelope);
//            var closestCoordinate1 = new AnimalCoordinates(3, 3, new Lion());
//            var closestCoordinate2 = new AnimalCoordinates(1, 3, new Lion());
//            var lionList = new List<AnimalCoordinates>
//            {
//                closestCoordinate1,
//                closestCoordinate2,
//                new AnimalCoordinates(9,3,new Lion()),
//                new AnimalCoordinates(4,8,new Lion()),
//            };

//            var value = antelope.GetClosestLion(antelopeCoordinates, lionList);

//            Assert.IsTrue(value.Equals(closestCoordinate2) || value.Equals(closestCoordinate1));
//        }

//        [TestMethod]
//        public void GetFurthestDirectionFromLion_WhenItsDiagonal_ReturnsCorrectFiveDirection()
//        {
//            List<Direction> directions = new List<Direction> { Direction.North, Direction.NorthEast, Direction.East, Direction.SouthEast, Direction.South, Direction.SouthWest, Direction.West, Direction.NorthWest };
//            var antelope = new Antelope();
//            var antelopeCoordinates = new AnimalCoordinates(7, 7, antelope);
//            var lionCoordinates = new AnimalCoordinates(1, 1, new Lion());

//            var list = antelope.GetFurthestDirectionFromLion(directions, antelopeCoordinates, lionCoordinates);

//            Assert.AreEqual(5, list.Count);
//            CollectionAssert.DoesNotContain(list, Direction.NorthWest);
//            CollectionAssert.DoesNotContain(list, Direction.North);
//            CollectionAssert.DoesNotContain(list, Direction.West);
//        }

//        [TestMethod]
//        public void GetFurthestDirectionFromLion_WhenItsHorizontal_ReturnsCorrectThreeDirections()
//        {
//            List<Direction> directions = new List<Direction> { Direction.North, Direction.NorthEast, Direction.East, Direction.SouthEast, Direction.South, Direction.SouthWest, Direction.West, Direction.NorthWest };
//            var antelope = new Antelope();
//            var antelopeCoordinates = new AnimalCoordinates(1, 7, antelope);
//            var lionCoordinates = new AnimalCoordinates(1, 1, new Lion());

//            var list = antelope.GetFurthestDirectionFromLion(directions, antelopeCoordinates, lionCoordinates);

//            Assert.AreEqual(3, list.Count);
//            CollectionAssert.Contains(list, Direction.SouthEast);
//            CollectionAssert.Contains(list, Direction.East);
//            CollectionAssert.Contains(list, Direction.NorthEast);
//        }

//        [TestMethod]
//        public void GetFurthestDirectionFromLion_WhenTwoMovesAwaySideways_ReturnsCorrectThreeDirections()
//        {
//            List<Direction> directions = new List<Direction> { Direction.North, Direction.NorthEast, Direction.East, Direction.SouthEast, Direction.South, Direction.SouthWest, Direction.West, Direction.NorthWest };
//            var antelope = new Antelope();
//            var antelopeCoordinates = new AnimalCoordinates(2, 3, antelope);
//            var lionCoordinates = new AnimalCoordinates(1, 1, new Lion());

//            var list = antelope.GetFurthestDirectionFromLion(directions, antelopeCoordinates, lionCoordinates);

//            Assert.AreEqual(3, list.Count);
//            CollectionAssert.Contains(list, Direction.SouthEast);
//            CollectionAssert.Contains(list, Direction.NorthEast);
//            CollectionAssert.Contains(list, Direction.East);
//        }

//        [TestMethod]
//        public void EatGrass_WhenNoStamina_Rests()
//        {
//            var antelope = new Antelope();
//            antelope.Damage(antelope.Health - 1);
//            antelope.Stamina = 0;

//            antelope.EatGrass();

//            Assert.IsTrue(0 < antelope.Stamina);
//            Assert.AreEqual(1 , antelope.Health);
//        }

//        [TestMethod]
//        public void EatGrass_WhenHasStamina_Heals()
//        {
//            var antelope = new Antelope();
//            antelope.Damage(antelope.Health - 1);
//            var startStamina = antelope.Stamina;

//            antelope.EatGrass();

//            Assert.IsTrue(startStamina > antelope.Stamina);
//            Assert.IsTrue(1 < antelope.Health);
//        }
//    }
//}

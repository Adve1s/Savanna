using Savanna.Logic;

namespace Savanna.Tests
{
    [TestClass]
    public sealed class LionTests
    {

        [TestMethod]
        public void DoAction_WhenNormal_DoesRandomAction()
        {
            var world = new World(10,10);
            var lion = new Lion();
            lion.Stamina -= 1;
            var startingStamina = lion.Stamina;
            world.AddAnimal(lion, new AnimalCoordinates(0, 0));
            var surroundings = world.GetVisibleArea(0, 0);
            var lionCoordinates = new AnimalCoordinates(0, 0, lion);

            lion.DoAction(world, surroundings.visibleArea, surroundings.self,lionCoordinates);

            Assert.AreNotEqual(startingStamina, lion.Stamina);
        }

        [TestMethod]
        public void DoAction_WhenTired_Sleeps()
        {
            var world = new World(10,10);
            var lion = new Lion();
            lion.Stamina = 1;
            var startingStamina = lion.Stamina;
            world.AddAnimal(lion, new AnimalCoordinates(0, 0));
            var surroundings = world.GetVisibleArea(0, 0);
            var lionCoordinates = new AnimalCoordinates(0, 0, lion);
            var restComparisonLion = new Lion();
            restComparisonLion.Stamina = 1;
            restComparisonLion.Rest();

            lion.DoAction(world, surroundings.visibleArea, surroundings.self,lionCoordinates);

            Assert.IsTrue(startingStamina < lion.Stamina);
            Assert.IsTrue(restComparisonLion.Stamina < lion.Stamina);
        }

        [TestMethod]
        public void DoAction_WhenHungry_MovesBySmell()
        {
            var world = new World(10, 10);
            var lion = new Lion();
            lion.Damage(lion.Health - 1);
            world.AddAnimal(lion, new AnimalCoordinates(0, 0));
            world.AddAnimal(new Antelope(), new AnimalCoordinates(9, 9));
            var surroundings = new Animal[3, 3];
            surroundings[0, 0] = lion;
            var lionCoordinates = new AnimalCoordinates(0, 0, lion);

            lion.DoAction(world, surroundings, lionCoordinates, lionCoordinates);
            var afterField = world.GetField();

            Assert.IsNull(afterField[0, 0]);
            Assert.AreEqual(afterField[1, 1], lion);
        }

        [TestMethod]
        public void DoAction_WhenSeeAntelope_MovesTowardsIt()
        {
            var world = new World(10, 10);
            var lion = new Lion();
            world.AddAnimal(lion, new AnimalCoordinates(0, 0));
            world.AddAnimal(new Antelope(), new AnimalCoordinates(2, 2));
            var surroundings = new Animal[3, 3];
            surroundings[0, 0] = lion;
            surroundings[2, 2] = new Antelope();
            var lionCoordinates = new AnimalCoordinates(0, 0, lion);

            lion.DoAction(world, surroundings, lionCoordinates, lionCoordinates);
            var afterField = world.GetField();

            Assert.IsNull(afterField[0, 0]);
            Assert.AreEqual(afterField[1, 1], lion);
        }

        [TestMethod]
        public void DoAction_WhenSeeAntelopeAndCanAttack_Attacks()
        {
            var world = new World(10, 10);
            var lion = new Lion();
            var antelope = new Antelope();
            world.AddAnimal(lion, new AnimalCoordinates(0, 0));
            world.AddAnimal(new Antelope(), new AnimalCoordinates(9, 9));
            var surroundings = new Animal[3, 3];
            surroundings[0, 0] = lion;
            surroundings[1, 1] = antelope;
            var antelopeStartHealth = antelope.Health;
            var lionCoordinates = new AnimalCoordinates(0, 0, lion);

            lion.DoAction(world, surroundings, lionCoordinates, lionCoordinates);
            var afterField = world.GetField();

            Assert.AreEqual(afterField[0, 0], lion);
            Assert.IsTrue(antelopeStartHealth > antelope.Health);
        }

        [TestMethod]
        public void DecideMoveDirection_WhenNoDirectionIsAvailable_ReturnsNull()
        {
            var surroundings = new Animal[1, 1];
            var lion = new Lion();

            var value = lion.DecideMoveDirection(new AnimalCoordinates(0,0,lion), surroundings);

            Assert.IsNull(value);
        }

        [TestMethod]
        public void DecideMoveDirection_WhenNoAntelopes_ReturnsRandomDirection()
        {
            var surroundings = new Animal[3, 3];
            var lion = new Lion();

            var value = lion.DecideMoveDirection(new AnimalCoordinates(1,1,lion), surroundings);

            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void DecideMoveDirection_WhenAntelopesGivenButEmpty_ReturnsRandomDirection()
        {
            var surroundings = new Animal[3, 3];
            var lion = new Lion();
            var antelopes = new List<AnimalCoordinates>();

            var value = lion.DecideMoveDirection(new AnimalCoordinates(1,1,lion), surroundings,antelopes);

            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void DecideMoveDirection_WhenAntelopesGiven_ReturnsCorrectDirection()
        {
            var surroundings = new Animal[3, 3];
            var lion = new Lion();
            var antelopes = new List<AnimalCoordinates> { new AnimalCoordinates(2,2,new Antelope())};

            var value = lion.DecideMoveDirection(new AnimalCoordinates(0,0,lion), surroundings,antelopes);

            Assert.AreEqual(value,Direction.SouthEast);
        }

        [TestMethod]
        public void GetClosestAntelope_WhenMultipleExistOneCorrect_ReturnsCorrectCoordinate()
        {
            var lion = new Lion();
            var lionCoordinates = new AnimalCoordinates(1, 1, lion);
            var closestCoordinate = new AnimalCoordinates(3, 3, new Antelope());
            var antelopeList = new List<AnimalCoordinates>
            {
                closestCoordinate,
                new AnimalCoordinates(9,3,new Antelope()),
                new AnimalCoordinates(4,8,new Antelope()),
            };

            var value = lion.GetClosestAntelope(lionCoordinates, antelopeList);

            Assert.AreEqual(value, closestCoordinate);
        }

        [TestMethod]
        public void GetClosestAntelope_WhenMultipleExistMultipleCorrect_ReturnsOneOfCorrectCoordinate()
        {
            var lion = new Lion();
            var lionCoordinates = new AnimalCoordinates(1, 1, lion);
            var closestCoordinate1 = new AnimalCoordinates(3, 3, new Antelope());
            var closestCoordinate2 = new AnimalCoordinates(1, 3, new Antelope());
            var antelopeList = new List<AnimalCoordinates>
            {
                closestCoordinate1,
                closestCoordinate2,
                new AnimalCoordinates(9,3,new Antelope()),
                new AnimalCoordinates(4,8,new Antelope()),
            };

            var value = lion.GetClosestAntelope(lionCoordinates, antelopeList);

            Assert.IsTrue(value.Equals(closestCoordinate2) || value.Equals(closestCoordinate1));
        }

        [TestMethod]
        public void GetClosestDirectionsToAntelope_WhenItsDiagonal_ReturnsCorrectOneDirection()
        {
            List<Direction> directions = new List<Direction> { Direction.North, Direction.NorthEast, Direction.East, Direction.SouthEast, Direction.South, Direction.SouthWest, Direction.West, Direction.NorthWest };
            var lion = new Lion();
            var lionCoordinates = new AnimalCoordinates(1, 1, lion);
            var antelope = new AnimalCoordinates(7, 7, new Antelope());

            var list = lion.GetClosestDirectionsToAntelope(directions, lionCoordinates, antelope);

            Assert.AreEqual(1, list.Count);
            CollectionAssert.Contains(list, Direction.SouthEast);
        }

        [TestMethod]
        public void GetClosestDirectionsToAntelope_WhenItsHorizontal_ReturnsCorrectThreeDirections()
        {
            List<Direction> directions = new List<Direction> { Direction.North, Direction.NorthEast, Direction.East, Direction.SouthEast, Direction.South, Direction.SouthWest, Direction.West, Direction.NorthWest };
            var lion = new Lion();
            var lionCoordinates = new AnimalCoordinates(1, 1, lion);
            var antelope = new AnimalCoordinates(1, 7, new Antelope());

            var list = lion.GetClosestDirectionsToAntelope(directions, lionCoordinates, antelope);

            Assert.AreEqual(3, list.Count);
            CollectionAssert.Contains(list, Direction.SouthEast);
            CollectionAssert.Contains(list, Direction.East);
            CollectionAssert.Contains(list, Direction.NorthEast);
        }

        [TestMethod]
        public void GetClosestDirectionsToAntelope_WhenTwoMovesAwaySideways_ReturnsCorrectTwoDirections()
        {
            List<Direction> directions = new List<Direction> { Direction.North, Direction.NorthEast, Direction.East, Direction.SouthEast, Direction.South, Direction.SouthWest, Direction.West, Direction.NorthWest };
            var lion = new Lion();
            var lionCoordinates = new AnimalCoordinates(1, 1, lion);
            var antelope = new AnimalCoordinates(2, 3, new Antelope());

            var list = lion.GetClosestDirectionsToAntelope(directions, lionCoordinates, antelope);

            Assert.AreEqual(2, list.Count);
            CollectionAssert.Contains(list, Direction.SouthEast);
            CollectionAssert.Contains(list, Direction.East);
        }

        [TestMethod]
        public void Attack_WhenNoStamina_Rests()
        {
            var lion = new Lion();
            lion.Stamina = 0;
            var lionStartStamina = lion.Stamina;
            var prey = new Antelope();
            var preyStartHealth = prey.Health;

            lion.Attack(prey);

            Assert.IsTrue(lionStartStamina < lion.Stamina);
            Assert.AreEqual(preyStartHealth, prey.Health);
        }

        [TestMethod]
        public void Attack_WhenNotKill_DamagesPrey()
        {
            var lion = new Lion();
            var lionStartStamina = lion.Stamina;
            var prey = new Antelope();
            var preyStartHealth = prey.Health;

            lion.Attack(prey);

            Assert.IsTrue(lionStartStamina > lion.Stamina);
            Assert.IsTrue(preyStartHealth > prey.Health);
            Assert.IsTrue(prey.IsAlive());
        }

        [TestMethod]
        public void Attack_WhenKill_KillsPreyAndHeals()
        {
            var lion = new Lion();
            lion.Damage(5);
            var lionStartHealth = lion.Health;
            var lionStartStamina = lion.Stamina;
            var prey = new Antelope();
            prey.Damage(prey.Health - 1);
            var preyStartHealth = prey.Health;

            lion.Attack(prey);

            Assert.IsTrue(lionStartStamina > lion.Stamina);
            Assert.IsTrue(lionStartHealth < lion.Health);
            Assert.IsTrue(preyStartHealth > prey.Health);
            Assert.IsFalse(prey.IsAlive());
        }

        [TestMethod]
        public void Roar_WhenHaveStamina_RoarsAndSpendsEnergy()
        {
            var lion = new Lion();
            var startStamina = lion.Stamina;

            lion.Roar();

            Assert.IsTrue(startStamina > lion.Stamina);
        }

        [TestMethod]
        public void Roar_WhenDontHaveStamina_Rests()
        {
            var lion = new Lion();
            lion.Stamina = 0;
            var startStamina = lion.Stamina;

            lion.Roar();

            Assert.IsTrue(startStamina < lion.Stamina);
        }

        [TestMethod]
        public void MoveBySmell_WhenDontHaveStamina_Rests()
        {
            var world = new World();
            var lion = new Lion();
            lion.Stamina = 0;
            var startStamina = lion.Stamina;

            lion.MoveBySmell(world, new AnimalCoordinates(0, 0, lion));

            Assert.IsTrue(startStamina < lion.Stamina);
        }

        [TestMethod]
        public void MoveBySmell_WhenNoAntelopeExists_MovesRandomly()
        {
            var world = new World();
            var lion = new Lion();
            var startStamina = lion.Stamina;
            world.AddAnimal(lion, new AnimalCoordinates(0, 0));

            lion.MoveBySmell(world, new AnimalCoordinates(0, 0, lion));

            Assert.IsTrue(startStamina > lion.Stamina);
        }

        [TestMethod]
        public void MoveBySmell_WhenAntelopeExists_MovesTowardsAntelope()
        {
            var world = new World(10, 10);
            var lion = new Lion();
            var startStamina = lion.Stamina;
            world.AddAnimal(lion, new AnimalCoordinates(0, 0));
            world.AddAnimal(new Antelope(), new AnimalCoordinates(9, 9));

            lion.MoveBySmell(world, new AnimalCoordinates(0, 0, lion));
            var fieldAfter = world.GetField();

            Assert.IsTrue(startStamina > lion.Stamina);
            Assert.IsNull(fieldAfter[0, 0]);
            Assert.IsInstanceOfType<Lion>(fieldAfter[1, 1]);
        }
    }
}

using Savanna.Logic;

namespace Savanna.Tests
{
    [TestClass]
    public sealed class AnimalFactoryTests
    {
        [TestMethod]
        public void CreateAnimal_WhenExists_ReturnNewAnimal()
        {
            var animalFactory = new AnimalFactory();
            var key = new Antelope().CreationKey;
            var animal = animalFactory.CreateAnimal(key);
            Assert.IsInstanceOfType<Animal>(animal);
        }

        [TestMethod]
        public void CreateAnimal_WhenDoesNotExist_ReturnNull()
        {
            var animalFactory = new AnimalFactory();
            var animal = animalFactory.CreateAnimal('-');
            Assert.IsNull(animal);
        }
    }
}

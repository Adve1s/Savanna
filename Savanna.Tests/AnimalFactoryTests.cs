using Savanna.Logic;

namespace Savanna.Tests
{
    [TestClass]
    public sealed class AnimalFactoryTests
    {
        [TestMethod]
        public void CreateAnimal_WhenExists_ReturnNewAnimal()
        {
            var pluginManager = new PluginManager(Directory.GetCurrentDirectory(), path => new[] { "oneFile.dll" }, file => typeof(TestAnimal).Assembly);
            var animalFactory = new AnimalFactory(pluginManager);
            var key = new TestAnimal().CreationKey;

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

        [TestMethod]
        public void GetAvailableKeys_WhenCalled_ReturnsAvailableKeys()
        {
            var pluginManager = new PluginManager(Directory.GetCurrentDirectory(), path => new[] { "oneFile.dll" }, file => typeof(TestAnimal).Assembly);
            var animalFactory = new AnimalFactory(pluginManager);
            var key = new TestAnimal().CreationKey;
            
            var value = animalFactory.GetAvailableKeys();

            Assert.AreEqual(value.Length, 1);
            Assert.AreEqual(value[0], key);
        }
    }
}

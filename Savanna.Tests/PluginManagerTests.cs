using Savanna.Logic;
using System.Reflection;

namespace Savanna.Tests
{
    [TestClass]
    public sealed class PluginManagerTests
    {
        [TestMethod]
        public void LoadAndValidatePlugins_WhenEverythingWorks_ReturnsDictWithTestAnimalCreator()
        {
            Func<string, string[]> mockGetFiles = path => new[] {"TestAnimal.dll" };
            Func<string, Assembly> mockLoadAssembly = file => typeof(TestAnimal).Assembly;
            var manager = new PluginManager(Directory.GetCurrentDirectory(),mockGetFiles,mockLoadAssembly);

            var result = manager.LoadAndValidatePlugins();

            Assert.IsTrue(result.ContainsKey('T'));
            Assert.AreEqual(result.Count,1);
            Assert.IsInstanceOfType<Animal>(result['T'].Creator());
            Assert.AreEqual(result['T'].AnimalName, "Test");
        }

        [TestMethod]
        public void LoadAndValidatePlugins_WhenWrongPath_ReturnsEmptyDict()
        {
            Func<string, string[]> mockGetFiles = path => new[] {"TestAnimal.dll" };
            Func<string, Assembly> mockLoadAssembly = file => typeof(TestAnimal).Assembly;
            var manager = new PluginManager("Invalid|path<>test",mockGetFiles,mockLoadAssembly);

            var result = manager.LoadAndValidatePlugins();

            Assert.AreEqual(result.Count,0);
        }

        [TestMethod]
        public void LoadAndValidatePlugins_WhenGetFilesReturnNull_ReturnsEmptyDictAndDoesntCrash()
        {
            Func<string, string[]> mockGetFiles = path => null;
            Func<string, Assembly> mockLoadAssembly = file => typeof(TestAnimal).Assembly;
            var manager = new PluginManager(Directory.GetCurrentDirectory(), mockGetFiles, mockLoadAssembly);

            var result = manager.LoadAndValidatePlugins();

            Assert.AreEqual(result.Count, 0);
        }

        [TestMethod]
        public void LoadAndValidatePlugins_WhenBadFilesFiletered_ReturnsDictWithInfoFromGoodFile()
        {
            Func<string, string[]> mockGetFiles = path => new[] {"bad1.dll", "TestAnimal.dll", "bad2.dll" };
            Func<string, Assembly> mockLoadAssembly = file => file.Contains("bad")? throw new Exception("Test file load failiure!") : typeof(TestAnimal).Assembly;
            var manager = new PluginManager(Directory.GetCurrentDirectory(), mockGetFiles, mockLoadAssembly);

            var result = manager.LoadAndValidatePlugins();

            Assert.IsTrue(result.ContainsKey('T'));
            Assert.AreEqual(result.Count, 1);
            Assert.IsInstanceOfType<Animal>(result['T'].Creator());
            Assert.AreEqual(result['T'].AnimalName, "Test");
        }

        [TestMethod]
        public void LoadAndValidatePlugins_WhenEmptyAssebmly_ReturnsEmptyDict()
        {
            Func<string, string[]> mockGetFiles = path => new[] {"empty.dll" };
            Func<string, Assembly> mockLoadAssembly = file => typeof(string).Assembly;
            var manager = new PluginManager(Directory.GetCurrentDirectory(), mockGetFiles, mockLoadAssembly);

            var result = manager.LoadAndValidatePlugins();

            Assert.AreEqual(result.Count, 0);
        }

        [TestMethod]
        public void LoadAndValidatePlugins_WhenTwoOfSameKey_ReturnsDictWithOne()
        {
            Func<string, string[]> mockGetFiles = path => new[] { "TestAnimal.dll", "AnotherOne.dll" };
            Func<string, Assembly> mockLoadAssembly = file => typeof(TestAnimal).Assembly;
            var manager = new PluginManager(Directory.GetCurrentDirectory(), mockGetFiles, mockLoadAssembly);

            var result = manager.LoadAndValidatePlugins();

            Assert.IsTrue(result.ContainsKey('T'));
            Assert.AreEqual(result.Count, 1);
            Assert.IsInstanceOfType<Animal>(result['T'].Creator());
            Assert.AreEqual(result['T'].AnimalName, "Test");
        }
    }
}

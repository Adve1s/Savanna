using System.ComponentModel;
using System.Reflection;

namespace Savanna.Logic
{
    /// <summary>
    /// Represens PluginManager,
    /// loads and validates and sends plugin info.
    /// </summary>
    internal class PluginManager
    {
        private const string PLUGIN_DIRECTORY = "plugins";
        private const string PLUGIN_EXTENTION = "*.dll";
        private const string SOLUTION_EXTENTION = "*.sln";

        public readonly string _pluginPath;
        private readonly Func<string, string[]> _getFiles;
        private readonly Func<string, Assembly> _loadAssembly;

        /// <summary>
        /// Creates instance of PluginManager
        /// </summary>
        /// <param name="pluginPath">path to use</param>
        /// <param name="getFiles">Function to use to get files</param>
        /// <param name="loadAssembly">function to use to get assemblies</param>
        public PluginManager(string pluginPath = null, Func<string, string[]> getFiles = null, Func<string, Assembly> loadAssembly = null)
        {
            _pluginPath = pluginPath ?? Path.Combine(GetSolutionDirectory(), PLUGIN_DIRECTORY);
            _getFiles = getFiles ?? ((path) => Directory.GetFiles(path, PLUGIN_EXTENTION));
            _loadAssembly = loadAssembly ?? ((file) => Assembly.LoadFrom(file));
        }

        /// <summary>
        /// Creates creation dictatory for plugins
        /// </summary>
        /// <returns>Dictatory with key : (creator, name) pairs</returns>
        public Dictionary<char, (Func<Animal> Creator, string AnimalName)> LoadAndValidatePlugins()
        {
            var creators = new Dictionary<char, (Func<Animal> Creator, string AnimalName)>();

            foreach (var assembly in LoadAssemblies())
            {
                foreach (var anmialType in GetAnimalTypes(assembly))
                {
                    if(ValidateAnimal(anmialType, creators))
                    {
                        RegisterAnimal(anmialType, creators);
                    }
                }

            }
            return creators;
        }

        /// <summary>
        /// Loads assemblies from _pluginPath
        /// </summary>
        /// <returns>List of assemblies</returns>
        private List<Assembly> LoadAssemblies()
        {
            var assemblies = new List<Assembly>();
            try
            {
                if (!Directory.Exists(_pluginPath)) throw new Exception(ErrorMessages.PATH_EXIST_FAILED_MESSAGE);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(ErrorMessages.PATH_FAILED_MESSAGE, ex.Message));
                return assemblies;
            }

            var files = _getFiles(_pluginPath) ?? Array.Empty<string>();

            foreach (var file in files)
            {
                try
                {
                    var assembly = _loadAssembly(file);
                    assemblies.Add(assembly);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format(ErrorMessages.FILE_LOAD_FAILED_MESSAGE, file, ex.Message));
                }
            }
            return assemblies;
        }

        /// <summary>
        /// Gets Animal types saved in assembly
        /// </summary>
        /// <param name="assembly">The assembly to get types from</param>
        /// <returns>Enumerable of gotten types</returns>
        private IEnumerable<Type> GetAnimalTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(Animal)) && !type.IsAbstract && type.IsPublic);
            } 
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(ErrorMessages.TYPE_LOAD_FAILED_MESSAGE,assembly.GetName().Name, ex.Message));
                return Enumerable.Empty<Type>();
            }
        }

        /// <summary>
        /// Tests if animal is valid
        /// </summary>
        /// <param name="animalType">Animal type to create.</param>
        /// <param name="dictatory">Dictatory where key : instance pair would be saved</param>
        /// <returns>Bool if animal was valid.</returns>
        private bool ValidateAnimal(Type? animalType, Dictionary<char, (Func<Animal> Creator, string AnimalName)> dictatory)
        {
            try
            {
                var instance = (Animal)Activator.CreateInstance(animalType);
                char key = instance.CreationKey;
                if (dictatory.ContainsKey(key)) throw new Exception(string.Format(ErrorMessages.KEY_USED_MESSAGE, key));
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(ErrorMessages.ANIMAL_ADDITION_FAILED_MESSAGE, animalType, ex.Message));
                return false;
            }
        }

        /// <summary>
        /// Registers animal in the dicctatory
        /// </summary>
        /// <param name="animalType">Animal to register</param>
        /// <param name="dictatory">Dictatory where it is saved</param>
        private void RegisterAnimal(Type? animalType, Dictionary<char, (Func<Animal> Creator, string AnimalName)> dictatory)
        {
            var instance = (Animal)Activator.CreateInstance(animalType);
            char key = instance.CreationKey;
            dictatory[key] = (() => (Animal)Activator.CreateInstance(animalType), instance.Name);
        }

        /// <summary>
        /// Get solution directory
        /// </summary>
        /// <returns>Solution directory if found, if not current directory</returns>
        private string GetSolutionDirectory()
        {
            var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (directory != null && !directory.GetFiles(SOLUTION_EXTENTION).Any())
            {
                directory = directory.Parent;
            }
            return directory.FullName ?? Directory.GetCurrentDirectory();
        }
    }
}

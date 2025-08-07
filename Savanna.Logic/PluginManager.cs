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
        private const int PLUGIN_WORLD_TURN_GENERATION_COUNT = 1000;

        private static readonly string pluginPath = Path.Combine(Directory.GetCurrentDirectory(), PLUGIN_DIRECTORY);
        private static readonly string[]? pluginFiles = Directory.GetFiles(pluginPath, PLUGIN_EXTENTION);

        /// <summary>
        /// Creates creation dictatory for plugins
        /// </summary>
        /// <returns>Dictatory with key : animal pairs</returns>
        public Dictionary<char, Func<Animal>> MakeCreationDictatoryFromPlugins()
        {
            var dictatory = new Dictionary<char, Func<Animal>>();
            try
            {
                Directory.Exists(pluginPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(ErrorMessages.PATH_FAILED_MESSAGE, ex.Message));
                return dictatory;
            }

            foreach (var file in pluginFiles)
            {
                HandleAndValidateFile(file, ref dictatory);
            }
            return dictatory;
        }

        /// <summary>
        /// Handles loading file and parsing its data
        /// </summary>
        /// <param name="file">File to handle</param>
        /// <param name="dictatory">Dictatory to save result at.</param>
        private void HandleAndValidateFile(string? file, ref Dictionary<char, Func<Animal>> dictatory)
        {
            try
            {
                var assembly = Assembly.LoadFrom(file);
                var animalTypes = assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(Animal)) && !type.IsAbstract && type.IsPublic);
                foreach (var animalType in animalTypes)
                {
                    if(ValidateAnimal(animalType, dictatory))
                    {
                        var instance = (Animal)Activator.CreateInstance(animalType);
                        dictatory[instance.CreationKey] = () => (Animal)Activator.CreateInstance(animalType);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(ErrorMessages.FILE_LOAD_FAILED_MESSAGE, file, ex.Message));
            }
        }

        /// <summary>
        /// Tests if animal is valid
        /// </summary>
        /// <param name="animalType">Animal type to create.</param>
        /// <param name="dictatory">Dictatory where key : instance pair would be saved</param>
        /// <exception cref="Exception">Creation key already exists</exception>
        private bool ValidateAnimal(Type? animalType, Dictionary<char, Func<Animal>> dictatory)
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
                Console.WriteLine(string.Format(ErrorMessages.ANIMAL_ADDITION_FAILED_MESSAGE,animalType, ex.Message));
                return false;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Savanna.Logic
{
    /// <summary>
    /// Represents AnimalFactory,
    /// manages Animal creation.
    /// </summary>
    internal class AnimalFactory
    {
        private Dictionary<char, (Func<Animal> Creator, string AnimalName)> _animals = new Dictionary<char, (Func<Animal> Creator, string AnimalName)>();

        public AnimalFactory(PluginManager pluginManager = null)
        {
            var usedManager = pluginManager ?? new PluginManager();
            _animals = usedManager.LoadAndValidatePlugins();
        }

        /// <summary>
        /// Creates animal if key exists
        /// </summary>
        /// <param name="key">Key to search animal by</param>
        /// <returns>New animal object</returns>
        public Animal? CreateAnimal(char key) => _animals.ContainsKey(key) ? _animals[key].Creator() : null;

        /// <summary>
        /// Get available keys
        /// </summary>
        /// <returns>array of animal keys.</returns>
        public char[] GetAvailableKeys() => _animals.Keys.ToArray();
    }
}

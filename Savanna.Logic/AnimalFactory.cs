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
        private Dictionary<char, Func<Animal>> _creators = new Dictionary<char, Func<Animal>>();

        public AnimalFactory()
        {
            var pluginManager = new PluginManager();
            _creators = pluginManager.MakeCreationDictatoryFromPlugins();
        }


        /// <summary>
        /// Creates animal if key exists
        /// </summary>
        /// <param name="key">Key to search animal by</param>
        /// <returns>New animal object</returns>
        public Animal? CreateAnimal(char key) => _creators.ContainsKey(key) ? _creators[key]() : null;
    }
}

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
        /// <summary>
        /// Dictatory with all animal creators saved
        /// </summary>
        private Dictionary<char, Func<Animal>> _creators = new Dictionary<char, Func<Animal>>()
        {
            {'A', () => new Antelope() },
            {'L', () => new Lion() }
        };

        /// <summary>
        /// Gets animal if it exists
        /// </summary>
        /// <param name="key">Key to search animal by</param>
        /// <returns></returns>
        public Animal? CreateAnimal(char key) => _creators.ContainsKey(key) ? _creators[key]() : null;
    }
}

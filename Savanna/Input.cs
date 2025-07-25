using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Savanna
{
    /// <summary>
    /// Handles user input
    /// </summary>
    internal static class Input
    {
        /// <summary>
        /// Get user used key
        /// </summary>
        /// <returns> Key used</returns>
        public static ConsoleKey GetUserKey()
        {
            return Console.ReadKey(true).Key;
        }
    }
}

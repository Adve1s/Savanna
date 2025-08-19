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
        /// <returns> Key used as an upper char</returns>
        public static char GetUserKeyAsUpperChar()
        {
            return char.ToUpper(Console.ReadKey(true).KeyChar);
        }

        /// <summary>
        /// Gets a whole and positive intiger from user.
        /// </summary>
        /// <param name="text">Message to display when asking for an intiger.</param>
        /// <returns>A positive intiger.</returns>
        public static int GetValidIntiger(string text)
        {
            int inputNumber;
            while (true)
            {
                Console.Write(text);
                string? input = Console.ReadLine();
                if (int.TryParse(input, out inputNumber) && inputNumber > 0)
                {
                    return inputNumber;
                }
                else
                {
                    Console.WriteLine(UIConstants.InvalidIntMessage);
                }

            }
        }
    }
}

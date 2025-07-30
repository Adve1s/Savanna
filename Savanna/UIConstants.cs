using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Savanna
{
    /// <summary>
    /// Centralizes all Constants used in Display
    /// </summary>
    internal static class UIConstants
    {
        /// <summary>
        /// Field position representations
        /// </summary>
        public const string EmptyField = ".";
        public const string SpaceBetweenFields = " ";

        /// <summary>
        /// Input texts
        /// </summary>
        public const string InvalidIntMessage = "Invalid input. Please enter a positove whole number.";
        public const string HeightMessage = "Enter height for world: ";
        public const string WidthMessage = "Enter width for world: ";
    }
}

using Savanna.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Savanna
{
    /// <summary>
    /// Represents console display,
    /// Manages game representation
    /// </summary>
    internal class Display
    {
        /// <summary>
        /// Displays current state of field
        /// </summary>
        /// <param name="field">Field information array</param>
        public void DisplayField(AnimalType[,] field)
        {
            Console.Clear();
            string finalField = string.Empty;
            for (int i = 0; i < field.GetLength(0); i++)
            {
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    switch (field[i, j]) 
                    {
                        case AnimalType.Antelope:
                            finalField += DisplayConstants.AntelopeField;
                            break;
                        case AnimalType.Lion:
                            finalField += DisplayConstants.LionField;
                            break;
                        case AnimalType.Empty:
                            finalField += DisplayConstants.EmptyField;
                            break;
                        default:
                            break;
                    }
                }
                finalField += "\n";
            }
            Console.WriteLine(finalField);
        }
    }
}

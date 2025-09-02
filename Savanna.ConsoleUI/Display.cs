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
        public void DisplayField(WorldDisplayDTO world)
        {
            Console.Clear();
            string finalField = string.Empty;
            for (int i = 0; i < world.Height; i++)
            {
                for (int j = 0; j < world.Width; j++)
                {
                    var animal = world.AnimalField[i][j];
                    if (animal != null)
                    {
                        if (animal.IsAlive) finalField += animal.DisplayChar.ToString() + UIConstants.SpaceBetweenFields;
                        else finalField += UIConstants.DeadAnimalField + UIConstants.SpaceBetweenFields;
                    }
                    else finalField += UIConstants.EmptyField.ToString() + UIConstants.SpaceBetweenFields;
                }
                finalField += "\n";
            }
            Console.WriteLine(finalField);
        }
    }
}

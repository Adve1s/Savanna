using Savanna.Logic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Savanna
{
    /// <summary>
    /// Represents single instance of Savanna app,
    /// Creates world and display and manages their communication
    /// </summary>
    internal class App
    {
        private World _savannaWorld;
        private Display _display;

        /// <summary>
        /// Initializes new instance of App
        /// </summary>
        public App()
        {
            int height = Input.GetValidIntiger(UIConstants.HeightMessage);
            int width = Input.GetValidIntiger(UIConstants.WidthMessage);
            _savannaWorld = new World(height,width);
            _display = new Display();
        }

        /// <summary>
        /// Runs the application
        /// </summary>
        public void Run()
        {
            var stopwatch = new Stopwatch();
            int updateIntervalMs = 500;
            stopwatch.Start();
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    char key = Input.GetUserKeyAsUpperChar();
                    _savannaWorld.AddAnimal(key);
                }
                if (stopwatch.ElapsedMilliseconds >= updateIntervalMs)
                {
                    _savannaWorld.NextTurn();
                    _display.DisplayField(_savannaWorld.GetSavannaField());
                    stopwatch.Restart();
                }
            }
        }
    }
}

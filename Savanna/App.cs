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
            _savannaWorld = new World();
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
                    ConsoleKey key = Input.GetUserKey();
                    if (key == ConsoleKey.A) AddAnimal(AnimalType.Antelope);
                    if (key == ConsoleKey.L) AddAnimal(AnimalType.Lion);
                }
                if (stopwatch.ElapsedMilliseconds >= updateIntervalMs)
                {
                    _savannaWorld.NextTurn();
                    _display.DisplayField(_savannaWorld.GetSavannaField());
                    stopwatch.Restart();
                }
            }
        }

        /// <summary>
        /// Adds the animal of selected type to savanna world
        /// </summary>
        /// <param name="animal">Animal type to add</param>
        private void AddAnimal(AnimalType animal)
        {
            var random = new Random();
            int randomRow = random.Next(0, _savannaWorld.Height);
            int randomColumn = random.Next(0, _savannaWorld.Width);
            while (true)
            {
                if (_savannaWorld.AddAnimal(animal, randomRow, randomColumn))
                {
                    break;
                }
                randomRow = random.Next(0, _savannaWorld.Height);
                randomColumn = random.Next(0, _savannaWorld.Width);
            }
        }

    }
}

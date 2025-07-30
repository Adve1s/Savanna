using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Savanna.Logic
{
    /// <summary>
    /// Represents one instance of Savanna world simulation,
    /// Manages Savanna progression.
    /// </summary>
    public partial class World
    {
        private const string DecomposingSymbol = "X";

        private Animal?[,] _field;
        private AnimalFactory _factory;

        /// <summary>
        /// Gets the field height
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Gets the field width
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Initializes new instance of World
        /// </summary>
        public World(int height = 10, int width = 30)
        {
            Height = height;
            Width = width;
            _field = new Animal[Height, Width];
            _factory = new AnimalFactory();
        }

        /// <summary>
        /// Gets the current field state
        /// </summary>
        /// <returns> Current field state in PositonOccupancy 2d array format</returns>
        public string[,] GetSavannaField()
        {
            var returnField = new string[Height, Width];
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (_field[i, j] != null)
                        if (_field[i, j].IsAlive()) returnField[i, j] = _field[i, j].DisplaySymbol;
                        else returnField[i, j] = DecomposingSymbol;
                }
            }
            return returnField;
        }

        /// <summary>
        /// Makes one whole turn for the world
        /// </summary>
        public void NextTurn()
        {
            var animalIdList = new List<(int, int)>();
            for (int i = 0; i < _field.GetLength(0); i++)
            {
                for (int j = 0; j < _field.GetLength(1); j++)
                {
                    if (_field[i, j] != null) animalIdList.Add((i, j));

                }
            }
            var random = new Random();
            animalIdList = animalIdList.OrderBy(x => random.Next()).ToList();
            foreach ((int row, int column) in animalIdList)
            {
                AnimalDoActions(row, column);
            }

        }

        /// <summary>
        /// Animals do decided actions
        /// </summary>
        /// <param name="row">Row of the animal</param>
        /// <param name="column">Column of the animal</param>
        private void AnimalDoActions(int row, int column)
        {
            AnimalCoordinates animalGlobaly = new AnimalCoordinates(_field[row, column], row, column);
            animalGlobaly.Animal.PerRoundStatusChanges();
            if (animalGlobaly.Animal.IsAlive()) 
            {
                (Animal[,] visibleArea, AnimalCoordinates animalLocaly) = GetVisibleArea(animalGlobaly.Row, animalGlobaly.Column);
                animalGlobaly.Animal.DoAction(this, visibleArea, animalLocaly, animalGlobaly);
            }else if (animalGlobaly.Animal.IsDecomposed())
            {
                _field[row, column] = null;
            }
        }

        /// <summary>
        /// Adds animal to the field if place is empty
        /// </summary>
        /// <param name="key"> Key by which animal is added</param>
        public void AddAnimal(char key)
        {
            var random = new Random();
            int randomRow = random.Next(0, Height);
            int randomColumn = random.Next(0, Width);
            while (true)
            {
                if(_field[randomRow, randomColumn] == null) { break; }
                randomRow = random.Next(0, Height);
                randomColumn = random.Next(0, Width);
            }
            _field[randomRow, randomColumn] = _factory.CreateAnimal(key);
        }

        /// <summary>
        /// Moves animal to next position
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="direction"></param>
        internal void MoveAnimal(AnimalCoordinates animal, Direction direction)
        {
            int targetRow = animal.Row + Movement.Directions[direction].row;
            int targetColumn = animal.Column + Movement.Directions[direction].column;
            if (targetRow >= 0 && targetRow < Height && targetColumn >= 0 && targetColumn < Width && _field[targetRow, targetColumn] == null)
            {
                (_field[animal.Row, animal.Column], _field[targetRow, targetColumn]) = (_field[targetRow, targetColumn], _field[animal.Row, animal.Column]);
            }
        }
    }
}

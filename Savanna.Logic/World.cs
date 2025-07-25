using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
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
        private Animal[,] _field;

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
        public World()
        {
            Height = 10;
            Width = 30;
            _field = new Animal[Height, Width];
        }

        /// <summary>
        /// Gets the current field state
        /// </summary>
        /// <returns> Current field state in PositonOccupancy 2d array format</returns>
        public AnimalType[,] GetSavannaField()
        {
            var returnField = new AnimalType[Height, Width];
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    switch (_field[i, j])
                    {
                        case Antelope:
                            returnField[i, j] = AnimalType.Antelope;
                            break;
                        case Lion:
                            returnField[i, j] = AnimalType.Lion;
                            break;
                        default:
                            returnField[i, j] = AnimalType.Empty;
                            break;
                    }
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
            animalGlobaly.Animal.AddStamina();
            (Animal[,] visibleArea, AnimalCoordinates animalLocaly) = GetVisibleArea(animalGlobaly.Row, animalGlobaly.Column);
            Animal.AnimalAction chosenAction = animalGlobaly.Animal.DecideAction(visibleArea, animalLocaly);
            switch (chosenAction)
            {
                case Animal.MoveAction move:
                    MoveAnimal(ref animalGlobaly, move.direction);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Adds animal to the field if place is empty
        /// </summary>
        /// <param name="animal">The type of animal</param>
        /// <param name="row">The row where to add</param>
        /// <param name="column">The column where to add</param>
        /// <returns>If adding was succesful</returns>
        public bool AddAnimal(AnimalType animal, int row, int column)
        {
            if (_field[row, column] != null) return false;
            if (animal == AnimalType.Antelope) _field[row, column] = new Antelope();
            else if (animal == AnimalType.Lion) _field[row, column] = new Lion();
            else return false;
            return true;
        }

        /// <summary>
        /// Moves animal to next position
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="direction"></param>
        private void MoveAnimal(ref AnimalCoordinates animal, Direction direction)
        {
            int targetRow = animal.Row + Movement.Directions[direction].row;
            int targetColumn = animal.Column + Movement.Directions[direction].column;
            if (targetRow >= 0 && targetRow < Height && targetColumn >= 0 && targetColumn < Width && _field[targetRow, targetColumn] == null)
            {
                (_field[animal.Row, animal.Column], _field[targetRow, targetColumn]) = (_field[targetRow, targetColumn], _field[animal.Row, animal.Column]);
                animal.Row = targetRow;
                animal.Column = targetColumn;
            }
        }
    }
}

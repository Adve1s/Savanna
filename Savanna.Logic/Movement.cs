using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Savanna.Logic
{
    /// <summary>
    /// Lists all possible movement directions.
    /// </summary>
    public enum Direction
    {
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest,
    }

    /// <summary>
    /// Saves all posible movement directions.
    /// </summary>
    public static class Movement
    {
        /// <summary>
        /// Accesable Directions with its movement values
        /// </summary>
        public static readonly Dictionary<Direction, (int row, int column)> Directions = new Dictionary<Direction, (int row, int column)>
        {
            { Direction.North, (-1, 0)},
            { Direction.NorthEast, (-1, 1)},
            { Direction.East, (0, 1)},
            { Direction.SouthEast, (1, 1)},
            { Direction.South, (1, 0)},
            { Direction.SouthWest, (1, -1)},
            { Direction.West, (0, -1)},
            { Direction.NorthWest, (-1, -1)},

        };

        /// <summary>
        /// Gives a random direction
        /// </summary>
        /// <param name="exclude">List of excluded directions</param>
        /// <returns>A random direction</returns>
        public static Direction? RandomDirection(List<Direction> directions) 
        {
            var random = new Random();
            if (directions.Count > 0) return directions[random.Next(directions.Count)];
            else return null;
        }

        /// <summary>
        /// Checks if direction is valid
        /// </summary>
        /// <param name="direction">Which direction to check</param>
        /// <param name="surroundings">Area where position is</param>
        /// <param name="self">Own coordinates in that position</param>
        /// <returns> Bool value showing if direction was valid. </returns>
        public static bool IsDirectionValid(Direction direction, Animal?[,] surroundings, AnimalCoordinates self)
        {
            var movement = Directions[direction];
            int targetRow = self.Row + movement.row;
            int targetColumn = self.Column + movement.column;
            return targetRow >= 0 && targetRow < surroundings.GetLength(0) && targetColumn >= 0
            && targetColumn < surroundings.GetLength(1) && surroundings[targetRow, targetColumn] == null;

        }

        /// <summary>
        /// Get list of all valid directions
        /// </summary>
        /// <param name="surroundings">Area where position is</param>
        /// <param name="self">Own coordinates in that position</param>
        /// <returns>A list of valid directions</returns>
        public static List<Direction> GetValidDirections(Animal?[,] surroundings, AnimalCoordinates self)
        {
            var allDirections = Enum.GetValues(typeof(Direction)).Cast<Direction>();
            return allDirections.Where(direction => IsDirectionValid(direction, surroundings, self)).ToList();
        }
    }
}

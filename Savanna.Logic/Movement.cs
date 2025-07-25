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
    internal enum Direction
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
    internal static class Movement
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
        public static Direction RandomDirection(List<Direction> directions) 
        {
            var random = new Random();
            return directions[random.Next(directions.Count)];
        }

        /// <summary>
        /// Checks if direction is valid
        /// </summary>
        /// <param name="direction">Which direction to check</param>
        /// <param name="surroundings">Area where position is</param>
        /// <param name="self">Own coordinates in that position</param>
        /// <returns> Bool value showing if direction was valid. </returns>
        public static bool IsDirectionValid(Direction direction, Animal[,] surroundings, AnimalCoordinates self)
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
        public static List<Direction> GetValidDirections(Animal[,] surroundings, AnimalCoordinates self)
        {
            var allDirections = Enum.GetValues(typeof(Direction)).Cast<Direction>();
            return allDirections.Where(direction => IsDirectionValid(direction, surroundings, self)).ToList();
        }

        /// <summary>
        /// Gets list of best directions foe the animal based on enemy position
        /// </summary>
        /// <param name="directions">All allowed directions</param>
        /// <param name="self">Own position</param>
        /// <param name="enemy">enemy position</param>
        /// <returns>List of all equally good directions.</returns>
        public static List<Direction> GetBestDirections(List<Direction> directions, AnimalCoordinates self, AnimalCoordinates enemy)
        {
            var directionWithDistanceToEnemy = directions.Select(direction => new
            {
                direction = direction,
                distance = World.DistanceToEnemyCalculator(
                    new AnimalCoordinates(self.Animal, self.Row + Directions[direction].row, self.Column + Directions[direction].column), enemy)
            }).ToList();

            Double afterMoveDistance = 0;
            if (self.Animal is Lion) afterMoveDistance = directionWithDistanceToEnemy.Min(value => value.distance);
            else if (self.Animal is Antelope) afterMoveDistance = directionWithDistanceToEnemy.Max(value => value.distance);

            var returnDirection = directionWithDistanceToEnemy
                    .Where(value => value.distance == afterMoveDistance)
                    .Select(value => value.direction)
                    .ToList();

            return returnDirection;
        }
    }
}

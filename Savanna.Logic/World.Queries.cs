using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Savanna.Logic
{
    /// <summary>
    /// Represents one instance of Savanna world simulation,
    /// manages info queries. 
    /// </summary>
    public partial class World
    {
        /// <summary>
        /// Gets visible area array for the coordinates
        /// </summary>
        /// <param name="row">The coordinate row</param>
        /// <param name="column">The coordinate column</param>
        /// <returns>Visible area 2d array</returns>
        private (Animal[,], AnimalCoordinates) GetVisibleArea(int row, int column)
        {
            var animal = _field[row, column];
            (int visibleAreaHeight, int visibleAreaWidth) = GetVisibleDimentions(row, column);
            var visibleArea = new Animal[visibleAreaHeight, visibleAreaWidth];

            AnimalCoordinates globalCoordinates = GetGlobalStartCoordinates(animal, row, column);
            int defaultGlobalX = globalCoordinates.Column;
            var returnCoordinates = new AnimalCoordinates(animal, default, default);
            for (int localY = 0; localY < visibleAreaHeight; localY++)
            {
                for (int localX = 0; localX < visibleAreaWidth; localX++)
                {
                    visibleArea[localY, localX] = _field[globalCoordinates.Row, globalCoordinates.Column];
                    if (globalCoordinates.Row == row && globalCoordinates.Column == column)
                    {
                        returnCoordinates.Row = localY;
                        returnCoordinates.Column = localX;
                    }
                    globalCoordinates.Column++;
                }
                globalCoordinates.Row++;
                globalCoordinates.Column = defaultGlobalX;
            }

            return (visibleArea, returnCoordinates);
        }

        /// <summary>
        /// Calculates global row and column to start vision area from.
        /// </summary>
        /// <param name="animal">Animal that is in position</param>
        /// <param name="row">Row from where calculate</param>
        /// <param name="column">Column from where calculate</param>
        /// <returns>Starting row and column from global array</returns>
        private AnimalCoordinates GetGlobalStartCoordinates(Animal animal, int row, int column)
        {
            int globalRow;
            int globalColumn;
            if (row - animal.Vision < 0) globalRow = 0;
            else globalRow = row - animal.Vision;
            if (column - animal.Vision < 0) globalColumn = 0;
            else globalColumn = column - animal.Vision;
            var animalCoordinates = new AnimalCoordinates(animal, globalRow, globalColumn);
            return animalCoordinates;
        }

        /// <summary>
        /// Gets visible area array size for corrdinates
        /// </summary>
        /// <param name="row">row of coordinates</param>
        /// <param name="column">column of coordinates</param>
        /// <returns>Height and width of visible area</returns>
        private (int, int) GetVisibleDimentions(int row, int column)
        {
            int visibleAreaHeight = 1;
            int visibleAreaWidth = 1;
            for (int visionRange = 1; visionRange <= _field[row, column].Vision; visionRange++)
            {
                if ((row - visionRange) >= 0) visibleAreaHeight++;
                if ((row + visionRange) < Height) visibleAreaHeight++;
                if ((column - visionRange) >= 0) visibleAreaWidth++;
                if ((column + visionRange) < Width) visibleAreaWidth++;

            }
            return (visibleAreaHeight, visibleAreaWidth);
        }

        /// <summary>
        /// Creates a list of visible lion coordinates
        /// </summary>
        /// <param name="surroundings">Visible area</param>
        /// <returns></returns>
        internal static List<AnimalCoordinates> GetEnemyPositionsList<T>(Animal[,] surroundings)
        {
            var rivalCoordinates = new List<AnimalCoordinates>();
            for (int i = 0; i < surroundings.GetLength(0); i++)
            {
                for (int j = 0; j < surroundings.GetLength(1); j++)
                {
                    if (surroundings[i, j] is T) rivalCoordinates.Add(new AnimalCoordinates(surroundings[i, j], i, j));
                }
            }
            return rivalCoordinates;
        }

        /// <summary>
        /// Calculates distance to enemy
        /// </summary>
        /// <param name="self">Own coordinates</param>
        /// <param name="enemy">Enemy Coordinates</param>
        /// <returns>Number of moves to enemy</returns>
        internal static double DistanceToEnemyCalculator(AnimalCoordinates self, AnimalCoordinates enemy)
        {
            return Math.Max(Math.Abs(self.Row-enemy.Row), Math.Abs(self.Column - enemy.Column));
        }

    }
}

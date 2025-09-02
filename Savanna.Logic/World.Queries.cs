using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        internal (Animal[,] visibleArea, AnimalCoordinates self) GetVisibleArea(int row, int column)
        {
            var animal = _field[row, column];
            (int visibleAreaHeight, int visibleAreaWidth) = GetVisibleDimentions(row, column);
            var visibleArea = new Animal[visibleAreaHeight, visibleAreaWidth];

            AnimalCoordinates globalCoordinates = GetGlobalStartVisionAreaCoordinates(animal, row, column);
            int defaultGlobalX = globalCoordinates.Column;
            var returnCoordinates = new AnimalCoordinates(default, default,animal);
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
        private AnimalCoordinates GetGlobalStartVisionAreaCoordinates(Animal animal, int row, int column)
        {
            int globalRow;
            int globalColumn;
            if (row - animal.Vision < 0) globalRow = 0;
            else globalRow = row - animal.Vision;
            if (column - animal.Vision < 0) globalColumn = 0;
            else globalColumn = column - animal.Vision;
            var animalCoordinates = new AnimalCoordinates(globalRow, globalColumn, animal);
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
        /// Gets field
        /// </summary>
        /// <returns>Field</returns>
        public Animal?[,] GetField() => _field;

        /// <summary>
        /// Get info of available animals.
        /// </summary>
        /// <returns>List of animal info objects</returns>
        private (string Name,char CreationKey)[] GetAvailableAnimalInfo()
        {
            var keys = AnimalFactory.GetAvailableKeys();
            var values = new (string,char)[keys.Length];
            for (int keyId = 0; keyId < keys.Length; keyId++)
            {
                var animal = AnimalFactory.CreateAnimal(keys[keyId]);
                values[keyId] = (animal.Name, animal.CreationKey);
            }
            return values;
        }

        /// <summary>
        /// Gets animalCardDTO by position on the board
        /// </summary>
        /// <param name="row">row of the animal</param>
        /// <param name="column">column of the animal</param>
        /// <returns>AnimalCardInfoDTO if animal was in that position or null if wasnt</returns>
        public AnimalCardInfoDTO? GetAnimalCardDTOByPosition(int row, int column)
        {
            var animal = _field[row, column];
            if (animal != null) return animal.AnimalToCardDTO();
            return null;
        }

        /// <summary>
        /// Gets animals position by id
        /// </summary>
        /// <param name="id"> ID of the animal to search for</param>
        /// <returns> row and column pair or null if not found</returns>
        public (int?,int?) GetAnimalPositionByID(int id)
        {
            for (int row = 0; row < Height; row++)
            {
                for (int column = 0; column < Width; column++)
                {
                    var animal = _field[row, column];
                    if (animal != null && animal.ID == id) return (row, column);

                }
            }
            return (null,null);
        }

        /// <summary>
        /// Parses world to display DTO
        /// </summary>
        /// <returns>WorldDisplayDTO object</returns>
        public WorldDisplayDTO WorldToDisplayDTO()
        {
            var animalRepresentations = new AnimalDisplayInfo?[Height][];
            for (int i = 0; i < Height; i++)
            {
                animalRepresentations[i] = new AnimalDisplayInfo?[Width];
                for (int j = 0; j < Width; j++)
                {
                    var animal = _field[i,j];
                    if (animal != null) animalRepresentations[i][j] = new AnimalDisplayInfo { DisplayChar = animal.DisplayChar, DisplayEmoji = animal.DisplayEmoji, IsAlive = animal.IsAlive() };
                }

            }
            var sharedWorld = new WorldDisplayDTO
            {
                AnimalsAvailable = _animalsAvailable,
                AnimalField = animalRepresentations,
                Iteration = _iteration,
                AnimalsInWorld = _animalsInWorld,
                Width = Width,
                Height = Height,
            };
            return sharedWorld;
        }

        /// <summary>
        /// Parses world to save DTO
        /// </summary>
        /// <returns>WorldSaveDTO object with all info</returns>
        public WorldSaveDTO WorldToSaveDTO()
        {
            var animalRepresentations = new AnimalSaveDTO?[Height][];
            for (int row = 0; row < Height; row++)
            {
                animalRepresentations[row] = new AnimalSaveDTO?[Width];
                for (int column = 0; column < Width; column++)
                {
                    var animal = _field[row,column];
                    if (animal != null) animalRepresentations[row][column] = animal.AnimalToSaveDTO();
                }

            }
            return new WorldSaveDTO
            {
                Field = animalRepresentations,
                Height = Height,
                Width = Width,
                AnimalsInWorld = _animalsInWorld,
                Iteration = _iteration,
            };
        }

        /// <summary>
        /// Sets up the world from worldSaveDto
        /// </summary>
        /// <param name="worldSaveDTO">The info needed to set up the world like it was</param>
        public void SetUpWorldFromSaveDTO(WorldSaveDTO worldSaveDTO)
        {
            var animalField = new Animal?[worldSaveDTO.Height,worldSaveDTO.Width];
            for (int row = 0; row < worldSaveDTO.Height; row++)
            {
                for (int column = 0; column < worldSaveDTO.Width; column++)
                {
                    var animalDTO = worldSaveDTO.Field[row][column];
                    if (animalDTO != null)
                    {
                        var animal = AnimalFactory.CreateAnimal(animalDTO.CreationKey);
                        if (animal != null) animal.SetUpAnimalFromSaveDTO(animalDTO);
                        animalField[row,column] = animal;
                    }
                }

            }
            _field = animalField;
            Height = worldSaveDTO.Height;
            Width = worldSaveDTO.Width;
            _animalsInWorld = worldSaveDTO.AnimalsInWorld;
            _iteration = worldSaveDTO.Iteration;
        }
    }
}

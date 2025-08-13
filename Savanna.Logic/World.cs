namespace Savanna.Logic
{
    /// <summary>
    /// Represents one instance of Savanna world simulation,
    /// Manages Savanna progression.
    /// </summary>
    public partial class World
    {
        private const char DecomposingSymbol = 'X';

        private Animal?[,] _field;
        internal AnimalFactory AnimalFactory { get; }

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
            AnimalFactory = new AnimalFactory();
        }

        /// <summary>
        /// Constructor for tests
        /// </summary>
        /// <param name="animalFactory">Animal factory injection</param>
        internal World(AnimalFactory animalFactory, int height = 15, int width = 10)
        {
            Height = height;
            Width = width;
            _field = new Animal[Height, Width];
            AnimalFactory = animalFactory;
        }

        /// <summary>
        /// Gets the current field state
        /// </summary>
        /// <returns> Current field state in PositonOccupancy 2d array format</returns>
        public char[,] GetCharField()
        {
            var returnField = new char[Height, Width];
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (_field[i, j] != null)
                        if (_field[i, j].IsAlive()) returnField[i, j] = _field[i, j].DisplayChar;
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
                try
                {
                    AnimalDoActions(row, column);
                }
                catch (Exception ex)
                {
                    _field[row, column] = null;
                    Console.WriteLine(string.Format(ErrorMessages.ANIMAL_CRASHED_MESSAGE, row, column, ex.Message));
                }
            }

        }

        /// <summary>
        /// Animals do decided actions
        /// </summary>
        /// <param name="row">Row of the animal</param>
        /// <param name="column">Column of the animal</param>
        private void AnimalDoActions(int row, int column)
        {
            AnimalCoordinates animal = new AnimalCoordinates(row, column, _field[row, column]);
            if (!animal.Animal.IsDecomposed()) animal.Animal.Turn(this, animal);
            else _field[row, column] = null;
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
            bool haveEmptySpace = _field.Cast<Object>().Any(field => field == null);
            while (haveEmptySpace)
            {
                if (_field[randomRow, randomColumn] == null)
                {
                    Animal animal = AnimalFactory.CreateAnimal(key);
                    if (animal != null)
                    {
                        _field[randomRow, randomColumn] = animal;
                    }
                    break;
                }
                randomRow = random.Next(0, Height);
                randomColumn = random.Next(0, Width);
            }

        }

        /// <summary>
        /// Adds animal to specified place
        /// </summary>
        /// <param name="animal">animal to add</param>
        /// <param name="place">place to add animal at</param>
        internal void AddAnimal(Animal animal, AnimalCoordinates? place)
        {
            if (place != null && _field[place.Value.Row, place.Value.Column] == null)
            {
                _field[place.Value.Row, place.Value.Column] = animal;
            }

        }

        /// <summary>
        /// Moves animal to next position
        /// </summary>
        /// <param name="animal">Animal position</param>
        /// <param name="direction">adirection to move to</param>
        internal void MoveAnimal(AnimalCoordinates animal, Direction direction)
        {
            int targetRow = animal.Row + Movement.Directions[direction].row;
            int targetColumn = animal.Column + Movement.Directions[direction].column;
            (_field[animal.Row, animal.Column], _field[targetRow, targetColumn]) = (_field[targetRow, targetColumn], _field[animal.Row, animal.Column]);
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
        public (string Name,char CreationKey)[] GetAvailableAnimalInfo()
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
    }
}
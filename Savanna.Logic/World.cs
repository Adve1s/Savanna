namespace Savanna.Logic
{
    /// <summary>
    /// Represents one instance of Savanna world simulation,
    /// Manages Savanna progression.
    /// </summary>
    public partial class World
    {
        private Animal?[,] _field;
        private int _iteration = 0;
        private int _animalsInWorld = 0;
        private (string Name,char CreationKey)[] _animalsAvailable;
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
            _animalsAvailable = GetAvailableAnimalInfo();
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
            _animalsAvailable = GetAvailableAnimalInfo();
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
                    _animalsInWorld--;
                    Console.WriteLine(string.Format(ErrorMessages.ANIMAL_CRASHED_MESSAGE, row, column, ex.Message));
                }
            }
            _iteration++;

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
            else
            {
                _field[row, column] = null;
                _animalsInWorld--;
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
            bool haveEmptySpace = _field.Cast<Object>().Any(field => field == null);
            while (haveEmptySpace)
            {
                if (_field[randomRow, randomColumn] == null)
                {
                    Animal animal = AnimalFactory.CreateAnimal(key);
                    if (animal != null)
                    {
                        _field[randomRow, randomColumn] = animal;
                        _animalsInWorld++;
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
                _animalsInWorld++;
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
    }
}
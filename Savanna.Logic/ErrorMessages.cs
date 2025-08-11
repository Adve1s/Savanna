using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Savanna.Logic
{
    /// <summary>
    /// Class to save error messages at
    /// </summary>
    internal static class ErrorMessages
    {
        public const string ANIMAL_CRASHED_MESSAGE = "Animal at row:{0}, column:{1} crashed and will be removed : {2}";

        public const string TYPE_LOAD_FAILED_MESSAGE = "Error discovering types in {0} : {1}";
        public const string FILE_LOAD_FAILED_MESSAGE = "Failed to load plugin {0} : {1}";
        public const string ANIMAL_ADDITION_FAILED_MESSAGE = "Failed to load animal of type {0} : {1}";
        public const string PATH_FAILED_MESSAGE = "Failed to get path : {0}";

        public const string PATH_EXIST_FAILED_MESSAGE = "Path doesn't exist.";
        public const string KEY_USED_MESSAGE = "Animal wasn't added, this CreationKey: '{0}' is already used!";
    }
}

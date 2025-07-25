using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static Savanna.Logic.Lion;

namespace Savanna.Logic
{
    /// <summary>
    /// Represents a specific type of <see cref="Animal"/> - Antelope,
    /// manages Antelope specific decision making.
    /// </summary>
    internal class Antelope : Animal
    {
        protected override int DefaultSpeed => 3;
        protected override int DefaultVision => 4;
        protected override int DefaultMaxStamina => 100;
        protected override int DefaultStaminaAddition => 25;
        protected override Dictionary<Type, (int StaminaChanges, int FunctionWeight)> ActionInfo
        {
            get
            {
                var changes = base.ActionInfo;
                changes.Add(typeof(EatGrassAction), (-DefaultMaxStamina / 20, 2));
                changes[typeof(MoveAction)] = (-DefaultMaxStamina/Speed, 2);
                return changes;
            }
        }

        /// <summary>
        /// Initializes new instance of Antelope
        /// </summary>
        public Antelope() 
        {
            Vision = DefaultVision;
            Speed = DefaultSpeed;
            Stamina = DefaultMaxStamina;
        }

        /// <summary>
        /// Decides what Antelope wants to do
        /// </summary>
        /// <param name="surroundings">Surroundings within vision range</param>
        /// <param name="self">Own position within vision range</param>
        /// <returns>AnimalAction that Antelope decided to do</returns>
        public override AnimalAction DecideAction(Animal[,] surroundings, AnimalCoordinates self)
        {
            surroundings[self.Row, self.Column] = null;
            var lionPositions = World.GetEnemyPositionsList<Lion>(surroundings);
            bool canMove = self.Animal.Stamina + ActionInfo[typeof(MoveAction)].StaminaChanges >= 0;
            if (!canMove)
            {
                var possibleActions = new List<Func<AnimalAction>> { Sleep, Rest };
                return DecideFromActions(possibleActions)();
            }
            if (lionPositions.Count() > 0)
            {
                return Move(self, surroundings, lionPositions);
            }
            else
            {
                var possibleActions = CreateDefaultFunctionList(self, surroundings);
                return DecideFromActions(possibleActions)();
            }
        }
    }
}

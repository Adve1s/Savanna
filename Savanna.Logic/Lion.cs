using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Savanna.Logic
{
    /// <summary>
    /// Represents a specific type of <see cref="Animal"/> - Lion,
    /// manages Lion specific decision making.
    /// </summary>
    internal class Lion : Animal
    {
        protected override int DefaultSpeed => 2;
        protected override int DefaultVision => 5;
        protected override int DefaultMaxStamina => 100;
        protected override int DefaultStaminaAddition => 25;
        protected override Dictionary<Type, (int StaminaChanges, int FunctionWeight)> ActionInfo
        {
            get
            {
                var changes = base.ActionInfo;
                changes.Add(typeof(RoarAction), (-DefaultMaxStamina / 10, 1));
                return changes;
            }
        }

        /// <summary>
        /// Initializes new instance of Lion
        /// </summary>
        public Lion()
        {
            Vision = DefaultVision;
            Speed = DefaultSpeed;
            Stamina = DefaultMaxStamina;
        }

        /// <summary>
        /// Decides what Lion wants to do
        /// </summary>
        /// <param name="surroundings">Surroundings within vision range</param>
        /// <param name="self">Own position within vision range</param>
        /// <returns>AnimalAction that Lion decided to do</returns>
        public override AnimalAction DecideAction(Animal[,] surroundings, AnimalCoordinates self)
        {
            surroundings[self.Row, self.Column] = null;
            var antelopePositions = World.GetEnemyPositionsList<Antelope>(surroundings);
            bool canMove = self.Animal.Stamina + ActionInfo[typeof(MoveAction)].StaminaChanges >= 0;
            if (!canMove) 
            {
                var possibleActions = new List<Func<AnimalAction>> {Sleep,Rest};
                return DecideFromActions(possibleActions)();
            }
            if (antelopePositions.Count() > 0)
            {
                return Move(self, surroundings, antelopePositions);
            }
            else
            {
                var possibleActions = CreateDefaultFunctionList(self,surroundings);
                return DecideFromActions(possibleActions)();
            }
        }
    }
}

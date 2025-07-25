using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Savanna.Logic
{
    /// <summary>
    /// Represents a single animal in Savanna,
    /// manages Animal decition making.
    /// </summary>
    internal abstract class Animal
    {
        protected abstract int DefaultSpeed { get; }
        protected abstract int DefaultVision { get; }
        protected abstract int DefaultMaxStamina { get; }
        protected abstract int DefaultStaminaAddition { get; }
        protected virtual Dictionary<Type, (int StaminaChanges,int FunctionWeight)> ActionInfo
        {
            get
            {
                return new Dictionary<Type, (int StaminaChanges, int FunctionWeight)>
                {
                    {typeof(RestAction),(DefaultMaxStamina/10,1) },
                    {typeof(SleepAction),(DefaultMaxStamina/3,1) },
                    {typeof(MoveAction),(-DefaultMaxStamina/Speed,6) },
                };
            }
        }

        /// <summary>
        /// Gets animal current stamina
        /// </summary>
        public int Stamina { get; protected set; }

        /// <summary>
        /// Gets Animal vision range
        /// </summary>
        public int Vision { get; protected set; }

        /// <summary>
        /// Gets Animal Speed
        /// </summary>
        public int Speed { get; protected set; }

        /// <summary>
        /// Decides what animal wants to do
        /// </summary>
        /// <param name="surroundings">Surroundings within vision range</param>
        /// <param name="self">Own position within vision range</param>
        /// <returns>AnimalAction that animal decided to do</returns>
        public abstract AnimalAction DecideAction(Animal[,] surroundings, AnimalCoordinates self);

        /// <summary>
        /// Resets stamina to default max
        /// </summary>
        public void AddStamina()
        {
            Stamina += DefaultStaminaAddition;
        }

        /// <summary>
        /// Makes animal specific direction choice
        /// </summary>
        /// <param name="self">Own position within vision range</param>
        /// <param name="surroundings">Surroundings within vision range</param>
        /// <param name="enemyCoordinates">Positions of enemies within vision range</param>
        /// <returns>Direction where animal chose to go</returns>
        protected Direction? DecideMoveDirection(AnimalCoordinates self, Animal[,] surroundings, List<AnimalCoordinates>? enemyCoordinates = null)
        {
            var directions = Movement.GetValidDirections(surroundings, self);
            if (directions.Count() == 0) return null;
            if (enemyCoordinates != null)
            {
                var enemy = GetClosestEnemy(self, enemyCoordinates);
                directions = Movement.GetBestDirections(directions, self, enemy);
            }
            return Movement.RandomDirection(directions);
        }

        /// <summary>
        /// Gets closest enemy position
        /// </summary>
        /// <param name="self">own position</param>
        /// <param name="enemyCoordinates">all visible enemy positions</param>
        /// <returns>AnimalCoordinates of closest enemy.</returns>
        private AnimalCoordinates GetClosestEnemy(AnimalCoordinates self, List<AnimalCoordinates> enemyCoordinates)
        {
            var random = new Random();
            double closestEnemies = enemyCoordinates
                .Min(enemy => World.DistanceToEnemyCalculator(self, enemy));
            List<AnimalCoordinates> enemies = enemyCoordinates
                .Where(enemy => World.DistanceToEnemyCalculator(self, enemy) == closestEnemies)
                .ToList();
            var enemy = enemies[random.Next(enemies.Count)];
            return enemy;
        }

        /// <summary>
        /// Base class for all actions
        /// </summary>
        public abstract class AnimalAction { }

        /// <summary>
        /// Represents action of animal resting
        /// </summary>
        public class RestAction : AnimalAction { }

        /// <summary>
        /// Represents action of moving
        /// It contains needed data for moving.
        /// </summary>
        public class MoveAction : AnimalAction
        {
            public Direction direction { get; }
            public MoveAction(Direction direction) { this.direction = direction; }
        }

        /// <summary>
        /// Represents action of animal sleeping
        /// </summary>
        public class SleepAction : AnimalAction { }

        /// <summary>
        /// Represents action of Lion roaring
        /// </summary>
        public class RoarAction : AnimalAction { }

        /// <summary>
        /// Represents action of Anteope stopping to eat
        /// </summary>
        public class EatGrassAction : AnimalAction { }

        /// <summary>
        /// Animal decides to do nothing
        /// </summary>
        /// <returns>Idle action</returns>
        protected AnimalAction Rest()
        {
            Stamina += ActionInfo[typeof(RestAction)].StaminaChanges;
            return new RestAction();
        }

        /// <summary>
        /// Spends stamina and returns MoveAction to perform
        /// </summary>
        /// <param name="self">Own position in visible area</param>
        /// <param name="surroundings">Visible area</param>
        /// <param name="enemyCoordinates">Enemy positions within visible area</param>
        /// <returns>Move action to perform</returns>
        protected AnimalAction Move(AnimalCoordinates self, Animal[,] surroundings, List<AnimalCoordinates>? enemyCoordinates = null)
        {
            Direction? direction = DecideMoveDirection(self, surroundings, enemyCoordinates);
            if (direction is not null && self.Animal.Stamina + ActionInfo[typeof(MoveAction)].StaminaChanges >=0)
            {
                Stamina += ActionInfo[typeof(MoveAction)].StaminaChanges;
                return new MoveAction((Direction)direction);
            }
            else
            {
                return Rest();
            }
        }

        /// <summary>
        /// Animal decides to sleep
        /// </summary>
        /// <returns> Sellp action</returns>
        protected AnimalAction Sleep()
        {
            Stamina += ActionInfo[typeof(SleepAction)].StaminaChanges;
            return new SleepAction();
        }

        /// <summary>
        /// Lion decides to roar
        /// </summary>
        /// <returns>Roar action</returns>
        public AnimalAction Roar()
        {
            Stamina += ActionInfo[typeof(RoarAction)].StaminaChanges;
            return new RoarAction();
        }

        /// <summary>
        /// Antelope decides to stop to eat
        /// </summary>
        /// <returns>Antelope eat action</returns>
        protected AnimalAction EatGrass()
        {
            Stamina += ActionInfo[typeof(EatGrassAction)].StaminaChanges;
            return new EatGrassAction();
        }

        /// <summary>
        /// Chooses function randomly
        /// </summary>
        /// <param name="possibleActions">functions to choose from</param>
        /// <returns></returns>
        protected Func<AnimalAction> DecideFromActions(List<Func<AnimalAction>> possibleActions)
        {
            if (!possibleActions.Any()) return Rest;
            var random = new Random();
            return possibleActions[random.Next(possibleActions.Count)];

        }

        /// <summary>
        /// From ActionInfo dict create and return default action list
        /// </summary>
        /// <param name="self">Own position within visible area</param>
        /// <param name="surroundings"> visible area</param>
        /// <returns>List of functions</returns>
        protected List<Func<AnimalAction>> CreateDefaultFunctionList(AnimalCoordinates self, Animal[,] surroundings)
        {
            var returnList = new List<Func<AnimalAction>> ();
            foreach (var action in ActionInfo)
            {
                for (int i = 0; i < action.Value.FunctionWeight; i++)
                {
                    if( action.Key == typeof(MoveAction))
                    {
                        returnList.Add(() => Move(self,surroundings));
                    } else if (action.Key == typeof(SleepAction))
                    {
                        returnList.Add(Sleep);
                    } else if (action.Key == typeof(RestAction))
                    {
                        returnList.Add(Rest);
                    } else if(action.Key == typeof(RoarAction))
                    {
                        returnList.Add(Roar);
                    } else if(action.Key == typeof(EatGrassAction))
                    {
                        returnList.Add(EatGrass);
                    }
                }
            }
            return returnList;
        }
    }
}

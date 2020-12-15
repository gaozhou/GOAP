using System;
using Anthill.Utils;

namespace Anthill.AI
{
	public class AntAIAgent
	{
		public ISense Sense;                 // Органы чувств.
		public AntAIState[] States;          // Набор доступных состояний.
		public AntAIState CurrentState;      // Текущее состояние.
		public AntAIState DefaultState;      // Состояние по умолчанию.
		public readonly AntAICondition WorldState;    // Текущее состояние.
		public readonly AntAIPlanner Planner;         // Планировщик.
		public AntAIPlan CurrentPlan;        // Текущий план.
		public AntAICondition CurrentGoal;   // Текущая цель.
		public AntAICondition DefaultGoal;   // Цель по умолчанию.

		// Если установить flase, то план будет построен, но выполнятся не будет.
		public bool AllowSetNewState;

		public AntAIAgent()
		{
			Sense = null;
			CurrentState = null;
			DefaultState = null;
			WorldState = new AntAICondition();
			Planner = new AntAIPlanner();
			CurrentPlan = new AntAIPlan();
			CurrentGoal = null;
			AllowSetNewState = true;
		}

		#region Public Methods

		/// <summary>
		/// Обновление текущего состояния.
		/// </summary>
		public void UpdateState(float aDeltaTime)
		{
			CurrentState.Update(aDeltaTime);
		}

		/// <summary>
		/// Мозговой штурм.
		/// </summary>
		public void Think()
		{
			// Собираем информацию о текущем состоянии игрового мира.
			Sense.GetConditions(this, WorldState);

			if (CurrentState == null)
			{
				// Если текущее состояние не установлено, тогда устанавливаем дефолтное состояние.
				SetDefaultState();
			}
			else
			{
				if (CurrentState.IsFinished(this, WorldState))
				{
					// Если текущее состояние завершено или было прервано, тогда
					// выбираем новое состояние и принудительно устанавливаем его.
					SetState(SelectNewState(WorldState), true);
				}
				else if (CurrentState.AllowForceInterrupting)
				{
					// Если текущее состояние по прежнему активно (не было прервано или закончено), тогда
					// обновляем план на основе текущей обстановки мира и меняем состояние только 
					// в том случае если состояние из обновленного плана будет отличаться от текущего.
					SetState(SelectNewState(WorldState));
				}
			}
		}

		/// <summary>
		/// Выбирает новое состояние на основе текущего состояния мира.
		/// </summary>
		public string SelectNewState(AntAICondition aWorldState)
		{
			var newState = DefaultState.Name;
			if (CurrentGoal != null)
			{
				Planner.MakePlan(ref CurrentPlan, aWorldState, CurrentGoal);
				if (!CurrentPlan.IsSuccess && CurrentPlan.Count <= 0)
					return newState;
				var actionName = Planner.GetAction(CurrentPlan[0]).Name;
				if (AllowSetNewState)
				{
					newState = Planner.GetState(actionName);
				}
					
				/* Отладочный вывод плана в консоль.
					AntAICondition condition = aConditions.Clone();
					string p = string.Format("Conditions: {0}\n", _planner.NameIt(condition.Description()));
					for (int i = 0; i < _currentPlan.Count; i++)
					{
						AntAIAction action = _planner.GetAction(_currentPlan[i]);
						condition.Act(action.post);
						p += string.Format("<color=orange>{0}</color> => {1}\n", action.name, _planner.NameIt(condition.Description()));
					}
					AntLog.Trace(p);
					//*/
			}
			else
			{
				AntLog.Report("AntAIAgent", "<b>Goal</b> is not defined!");
			}

			return newState;
		}

		/// <summary>
		/// Определяет какая цель будет целью по умолчанию.
		/// </summary>
		public void DefaultGoalIs(string aGoalName)
		{
			DefaultGoal = FindGoal(aGoalName);
		}

		/// <summary>
		/// Устанавливает указанную цель как текущую.
		/// </summary>
		public void SetGoal(string aGoalName)
		{
			CurrentGoal = FindGoal(aGoalName);
			if (CurrentGoal != null) 
				return;
			AntLog.Report("AntAIAgent", "Can't find \"{0}\" goal.", aGoalName);
			SetDefaultGoal();
		}

		/// <summary>
		/// Устанавливает цель по умолчанию как текущее.
		/// </summary>
		public void SetDefaultGoal()
		{
			if (DefaultGoal != null)
			{
				CurrentGoal = DefaultGoal;
			}
			else
			{
				AntLog.Report("AntAIAgent", "Default <b>Goal</b> is not defined!");
			}
		}

		/// <summary>
		/// Определяет какое из состояний будет состоянием по умолчанию.
		/// </summary>
		public void DefaultStateIs(string aStateName)
		{
			DefaultState = FindState(aStateName);
			if (DefaultState == null)
			{
				AntLog.Report("AntAIAgent", "Can't set \"{0}\" as <b>Default State</b> because it is not existing!", aStateName);
			}
		}

		/// <summary>
		/// Устанавливает состояение по умолчанию как текущее.
		/// </summary>
		public void SetDefaultState()
		{
			if (CurrentState != null)
			{
				CurrentState.Stop();
			}

			AntLog.Assert(DefaultState == null, "Default <b>State</b> is not defined!", true);
			CurrentState = DefaultState;
			CurrentState.Reset();
			CurrentState.Start();
		}
		
		/// <summary>
		/// Устанавливает указанное состояние как текущее.
		/// </summary>
		public void SetState(string aStateName, bool aForce = false)
		{
			if (!aForce && string.Equals(CurrentState.Name, aStateName)) 
				return;
			
			CurrentState.Stop();
			CurrentState = FindState(aStateName);
			if (CurrentState != null)
			{
				CurrentState.Reset();
				CurrentState.Start();
			}
			else
			{
				AntLog.Report("AntAIAgent", "Can't find \"{0}\" state.", aStateName);
				SetDefaultState();
			}
		}

		#endregion
		#region Private Methods

		/// <summary>
		/// Ищет зарегистрированное состояние по имени.
		/// </summary>
		private AntAIState FindState(string aStateName)
		{
			var index = Array.FindIndex(States, x => string.Equals(x.Name, aStateName));
			return (index >= 0 && index < States.Length) ? States[index] : null;
		}

		/// <summary>
		/// Ищет зарегистрированную цель по имени.
		/// </summary>
		private AntAICondition FindGoal(string aGoalName)
		{
			var index = Planner.Goals.FindIndex(x => x.Name.Equals(aGoalName));
			return (index >= 0 && index < Planner.Goals.Count) ? Planner.Goals[index] : null;
		}

		#endregion
	}
}
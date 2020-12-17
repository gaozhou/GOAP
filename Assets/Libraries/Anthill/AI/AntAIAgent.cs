using System;
using Anthill.Utils;

namespace Anthill.AI
{
	public class AntAIAgent
	{
		public ISense Sense;                 // 传感器
		public AntAIState[] States;          // 可用状态集合
		public AntAIState CurrentState;      // 当前状态
		public AntAIState DefaultState;      // 默认状态
		public readonly AntAICondition WorldState;    // 世界当前状态
		public readonly AntAIPlanner Planner;         // 调度程序
		public AntAIPlan CurrentPlan;        // 当前计划
		public AntAICondition CurrentGoal;   // 当前目标
		public AntAICondition DefaultGoal;   // 默认目标

		// 如果设置flase，则将构建计划，但不会执行该计划。
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
		/// 更新当前状态
		/// </summary>
		public void UpdateState(float aDeltaTime)
		{
			CurrentState.Update(aDeltaTime);
		}

		/// <summary>
		/// 思考
		/// </summary>
		public void Think()
		{
			// 收集有关游戏世界当前状态的信息
			Sense.GetConditions(this, WorldState);

			if (CurrentState == null)
			{
				// 如果未设置当前状态，则设置默认状态
				SetDefaultState();
			}
			else
			{
				if (CurrentState.IsFinished(this, WorldState))
				{
					// 如果当前状态已完成或已被中断，则选择一个新状态并强制执行它。
					SetState(SelectNewState(WorldState), true);
				}
				else if (CurrentState.AllowForceInterrupting)
				{
					// 如果当前状态仍处于活动状态（未中断或未完成），则根据世界当前情况更新计划，仅更改状态如果更新计划的状态与当前计划不同。
					SetState(SelectNewState(WorldState));
				}
			}
		}

		/// <summary>
		/// 根据世界的当前状态选择一个新状态。
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
		/// 设置默认目标状态
		/// </summary>
		public void DefaultGoalIs(string aGoalName)
		{
			DefaultGoal = FindGoal(aGoalName);
		}

		/// <summary>
		/// 设置指定目标
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
		/// 设置默认目标
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
		///  设置默认状态名字
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
		/// 设置到默认状态
		/// </summary>
		public void SetDefaultState()
		{
			CurrentState?.Stop();
			AntLog.Assert(DefaultState == null, "Default <b>State</b> is not defined!", true);
			CurrentState = DefaultState;
			CurrentState.Reset();
			CurrentState.Start();
		}
		
		/// <summary>
		/// 设置当前状态。
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
		/// 查找状态
		/// </summary>
		private AntAIState FindState(string aStateName)
		{
			var index = Array.FindIndex(States, a => string.Equals(a.Name, aStateName));
			return index >= 0 && index < States.Length ? States[index] : null;
		}

		/// <summary>
		/// 查找目标
		/// </summary>
		private AntAICondition FindGoal(string aGoalName)
		{
			var index = Planner.Goals.FindIndex(x => x.Name.Equals(aGoalName));
			return (index >= 0 && index < Planner.Goals.Count) ? Planner.Goals[index] : null;
		}

		#endregion
	}
}
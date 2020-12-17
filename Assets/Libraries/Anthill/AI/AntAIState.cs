using System.Collections.Generic;

namespace Anthill.AI
{
	public class AntAIState
	{
		public readonly string Name;

		private readonly List<string> _interruptions;
		protected bool Finished;
		
		public bool AllowForceInterrupting { get; protected set; }

		public AntAIState(string aName)
		{
			Name = aName;
			_interruptions = new List<string>();
			Finished = false;
			AllowForceInterrupting = true;
		}

		public void AddInterrupt(string aConditionName)
		{
			_interruptions.Add(aConditionName);
		}

		public virtual void Start()
		{
			// 在任务开始之前调用。
		}

		public virtual void Update(float aDeltaTime)
		{
			// 任务处于活动状态时调用每个游戏循环。
		}

		public virtual void Stop()
		{
			// 在任务完成执行后调用（即使任务已中断）。
		}

		public virtual void Reset()
		{
			Finished = false;
		}

		public bool IsFinished(AntAIAgent aAgent, AntAICondition aWorldState)
		{
			return (Finished || OverlapInterrupts(aAgent.Planner, aWorldState));
		}

		public bool OverlapInterrupts(AntAIPlanner aPlanner, AntAICondition aConditions)
		{
			for (int i = 0, n = _interruptions.Count; i < n; i++)
			{
				var index = aPlanner.GetAtomIndex(_interruptions[i]);
				if (aConditions.GetValue(index))
				{
					return true;
				}
			}
			return false;
		}
	}
}